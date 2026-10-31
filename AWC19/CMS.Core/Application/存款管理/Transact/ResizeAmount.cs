using AutoMapper;
using CMS.Core.Application.提款管理;
using ShareEntity.Models;
using ShareEntity.OutputModels;
using ShareEntity.SqlCommandText;
using DBLibs.WXDapper;
using FluentValidation;
using LogLib.Helper;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.lang;
using ShareEntity.InputModels.Pay;

namespace CMS.Core.Application.存款管理.Transact {
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class ResizeAmount : IRequest<ResponseModel<ResizeAmountReturn>> {
        public long rowid { get; set; }
        public string orderNo { set; get; }
        public decimal amount { set; get; }
        /// <summary>
        /// 订单备注
        /// </summary>
        public string remarks { set; get; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class ResizeAmountReturn {
    }
    /// <summary>
    /// 会员总钱包
    /// </summary>
    public class WalletModel {
        /// <summary>
        /// 主账号
        /// </summary>
        public decimal amountactive { set; get; }
        /// <summary>
        /// 游戏钱包
        /// </summary>
        public decimal wallet { set; get; }
        public string account { set; get; }
        /// <summary>
        /// 会员id
        /// </summary>
        public long uid { set; get; }
    }
    /// <summary>
    /// API Model Validation
    /// </summary>
    public class ResizeAmountValidator : AbstractValidator<ResizeAmount> {
        public ResizeAmountValidator() {
            RuleFor(x => x.rowid)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class ResizeAmountHandler : BaseWithdraw<ResizeAmount, ResponseModel<ResizeAmountReturn>> {
        public ResizeAmountHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<ResizeAmountReturn> Handle(ResizeAmount request) {


            lock (this) {
                var sqlArray = new List<string>();
                var param = new List<object>();

                var querySql = string.Format(@"select d.*,t.misc t_misc,t.name payplatformname,u.id uid from ofDeposit d with(nolock)
left join ofUser u with(nolock) on d.account = u.account
left join ofThirdpay t with(nolock) on d.payplatform=t.id
where d.id=@rowid");
                var depositRows = DapperBase.Query<ofDeopsitEx>(querySql, param: request, commandTimeout: 15000,
                    ConnectionString: DapperStringBase.ConnectionString);
                if (depositRows != null) {
                    var depositRow = depositRows.First();

                    #region 申请条件检测

                    if (depositRow.status == 1)
                        return new ResponseModel<ResizeAmountReturn>() {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "定单状态有误(error status)",
                            Data = null
                        };

                    if (!depositRow.merchantno.Equals(request.orderNo))
                        return new ResponseModel<ResizeAmountReturn>() {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "定单号有误(error order no)",
                            Data = null
                        };

                    //要加退的金额
                    if (request.amount>=100000)
                        return new ResponseModel<ResizeAmountReturn>() {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "申请金额有误! ",
                            Data = null
                        };

                    var currentTime = DateTime.UtcNow.AddHours(8);

                    if (depositRow.requesttime > currentTime.AddMinutes(-10))
                        return new ResponseModel<ResizeAmountReturn>() {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "调整金额失败:【定单提交10分后,才能人工调整】 ",
                            Data = null
                        };


                    if (depositRow.requesttime.AddHours(120) < currentTime)
                        return new ResponseModel<ResizeAmountReturn>() {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "调整金额失败:【120小时已过,联系技术后再处理，谢谢】 ",
                            Data = null
                        };

                    #endregion

                    #region 计算 充值手续费fee（代理承担）和 会员承担的 费用（self_fee ）和会员承担费用的比例（self_feerate）
                    var thirdpay_miscModel = JsonConvert.DeserializeObject<ThirdPayMiscModel>(depositRow.t_misc);
                    var feeResult = DepositTimesGroupByPlatform(depositRow.account, thirdpay_miscModel, depositRow.payplatform);
                    var self_fee = request.amount * (feeResult.feecent / 100); //要承担的手续费
                    var self_feerate = feeResult.feecent;//要承担的手续费比例

                    #endregion

                    #region 调整金额操作

                    //退回部份金额
                    //更新当前定金额和余额调整金额
                    request.remarks = request.remarks.Replace("'", "''");

                    var t_misc = new Dictionary<string, object>();
                    if (!string.IsNullOrEmpty(depositRow.misc)) {
                        t_misc = JsonConvert.DeserializeObject<Dictionary<string, object>>(depositRow.misc);
                    }
                    var dkey = "preResizeAmount";//当前订单原始金额
                    if (!t_misc.ContainsKey(dkey)) {
                        t_misc.Add(dkey, depositRow.amount);
                    }
                    else {
                        t_misc[dkey] = depositRow.amount;
                    }
                    var orderendtime = currentTime.ToString("yyyy-MM-dd HH:mm:ss");
                    var crc = Common.Security.MD5Hash(string.Concat(depositRow.id, "_", depositRow.account, "_", request.amount.ToString("F2"), "_", depositRow.payplatform)).ToLower();
                    var updateSql = string.Format(@"update ofDeposit set status=1,reviewperson=@reviewperson,reviewtime=@reviewtime,orderendtime=@orderendtime, self_fee=@self_fee, self_feerate=@self_feerate, crc=@crc, amount=@realAmount,alreadyResizeTimes=alreadyResizeTimes+1, cancelreason=@remarks, misc=@misc where id=@rowid");
                    sqlArray.Add(updateSql);
                    param.Add(new {
                        realAmount = request.amount,
                        misc = JsonConvert.SerializeObject(t_misc),
                        rowid = request.rowid,
                        remarks = request.remarks,
                        crc = crc,
                        reviewperson = request.Admin,
                        reviewtime = orderendtime,
                        orderendtime = orderendtime,
                        self_fee = self_fee,
                        self_feerate = self_feerate
                    });

                    var referenceguid = string.Format(@"resize{0}", depositRow.referenceguid);// 如果是充值补单调整 会以resize开头的。
                    var currency = LangCodes.CurrencyMapLang[depositRow.currencycode].currency;
                    sqlArray.Add(PromoSqlCommandText.CreateResizeSql_V);
                    param.Add(new {
                        currency = currency,
                        account = depositRow.account,
                        amount = request.amount - self_fee,//实际到账金额 - 手续费用金额（部分渠道费用贵代理和会员各承担一部份）
                        createperson = request.Admin,
                        remarks = string.Format(@"充值补单【{6}】【金额调整】id:{0},时间:{1},调前金额:{2}, 单号:{3}, 自承担费用:{4}-{5}", 
                        request.rowid, depositRow.requesttime, depositRow.amount, depositRow.merchantno,self_fee,self_feerate, depositRow.payplatformname),
                        referenceguid = referenceguid,
                        decreasetype = 1,
                        referrer = "",
                        appendType = -1,
                        orderNo = depositRow.merchantno,
                        accountType = 0,
                        amountType = 0,
                        r_f_uid = depositRow.uid
                    });

                    #region 记录充值账号时余额结点 如果我余额<=10才做记录 2024-11-28   

                    if (depositRow.currencycode != 156) {
                        var walletSql = string.Format(@"select amountactive,uc.wallet  from ofUser u
left join ofUserGameWallet uc on u.account = uc.account
where u.account='{0}'", depositRow.account);
                        var totalWallet = 0m;
                        var walletModel = DapperBase.QueryAsync<WalletModel>(walletSql, null,
                                       commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString).Result;
                        if (walletModel != null && walletModel.Count > 0) {
                            totalWallet = walletModel.First().amountactive + walletModel.First().wallet;
                            if (totalWallet <= 10) {
                                var WithdrawalVerifyTimelineParams = new {
                                    account = depositRow.account,
                                    ref_guid = depositRow.merchantno,
                                    currentAmount = totalWallet,
                                    r_type = 0, //0：充值，1：红利优惠，2：反水
                                    r_time = orderendtime,
                                    remark = "后台校正金额充值到账",
                                    currencyType = depositRow.currencycode
                                };
                                var insertWithdrawalVerifyTimelineSql = string.Format(@"INSERT INTO [dbo].[ofWithdrawalVerifyTimeline]
           ([account],[ref_guid],[currentAmount],[r_type],[r_time],[remark],[currencyType])
     VALUES
           (@account, @ref_guid, @currentAmount,@r_type, @r_time, @remark, @currencyType)");
                                sqlArray.Add(insertWithdrawalVerifyTimelineSql);
                                param.Add(WithdrawalVerifyTimelineParams);
                            }
                        }
                        else {
                            return new ResponseModel<ResizeAmountReturn>() {
                                code = ShareEntity.Global.ResponseCode.code.FAIL,
                                message = "会员余额确认失败! ",
                                Data = null
                            };
                        }

                    }
                    #endregion


                    if (DapperBase.ExecuteTransction(sqlArray, param, commandTimeout: 15000,
                        ConnectionString: DapperStringBase.ConnectionString)) {
                        return new ResponseModel<ResizeAmountReturn>() {
                            code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                            message = "操作成功,【请到】【余额微调】确认调整金额是否正确",
                            Data = null
                        };
                    }
                    #endregion
                }

                return new ResponseModel<ResizeAmountReturn>() {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "未查询到提款单! ",
                    Data = null
                };
            }

        }

        private static FeeModel DepositTimesGroupByPlatform(string account, ThirdPayMiscModel miscModel,
            long mid) {

            if (miscModel.deductfee == 1)//后台配置要承担手续费
            {
                var platDepositTimesSql = string.Format(@"select payplatform,count(payplatform) times from ofDeposit d with(nolock)
where DateDiff(dd,d.requesttime,DATEADD(hour,8,getutcdate()))=0 and d.account='{0}' and d.status = 1
group by payplatform", account);

                var ms = new Dictionary<long, DepositTimesByPlatform>();
                var depositTimesModels = DapperBase.Query<DepositTimesByPlatform>(platDepositTimesSql, null,
                    null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                if (depositTimesModels != null && depositTimesModels.Count > 0) {
                    foreach (var m in depositTimesModels) {
                        ms.Add(m.payplatform, m);//渠道和当天对应存款成功的次数
                    }
                }

                var times = miscModel.predeductcount;//免承担手续费用的次数
                if (ms.ContainsKey(mid))//今天商户有存款
                {
                    var depositTime = ms[mid].times;//今天商户存款成功的次数
                    if (depositTime > times)//客人在指定渠道存款次数据 如果超出免手续次数，就会提示客人要扣手续费用
                        return new FeeModel { feecent = miscModel.self_feecent };
                    else
                        return new FeeModel { feecent = 0 };
                }
                else //今天商户没有存款
                {
                    if (times == 0)//商户配置0次，这边要收手续费用 
                        return new FeeModel { feecent = miscModel.self_feecent };
                    else {
                        //今天的存款次数0<配置的免费次数 手续费为0
                        return new FeeModel { feecent = 0 };
                    }
                }
            }
            return new FeeModel { feecent = 0 };//不用收手续费
        }

    }
}
