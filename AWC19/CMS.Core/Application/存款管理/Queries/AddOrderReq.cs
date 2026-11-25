using AutoMapper;
using AWC2019.Core.Validation;
using CMS.Core.Application.提款管理;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShareEntity.InputModels.Pay;
using ShareEntity.Models;
using ShareEntity.OutputModels;
using ShareEntity.SqlCommandText;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.lang;
using UtilsLib.ValidationAttr;

namespace CMS.Core.Application.存款管理.Queries
{
   
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class AddOrderReq : IRequest<ResponseModel<AddOrderReqReturn>>
    {
        public int merId { get; set; }

        [CharAndDigitalAndUnderScore(20, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string account { set; get; }
        public decimal amount { set; get; }
        /// <summary>
        /// 订单备注
        /// </summary>
        [MaxLength(200)]
        public string remarks { set; get; } = "";
        [MaxLength(15)]
        public string realname { set; get; } = "";
        public int device { set; get; } = -1;

        [JsonIgnore]
        [CharAndDigitalAndUnderScore(20, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string Admin { get; set; }
    }

    public class AddOrderReqReturn
    {

    }
    /// <summary>
    /// 会员总钱包
    /// </summary>
    public class WalletModel
    {
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
    public class AddOrderReqValidator : AbstractValidator<AddOrderReq>
    {
        public AddOrderReqValidator()
        {
            RuleFor(x => x.account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class AddOrderReqHandler : BaseWithdraw<AddOrderReq, ResponseModel<AddOrderReqReturn>>
    {
        public AddOrderReqHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        /// <summary>
        /// 存款记录写入
        /// </summary>
        public const string insertSql = @"INSERT INTO OFDEPOSIT(currency,account,amount,
            bankname,merchantno,status,referrerreg,ipaddress,macaddress,referenceguid,
            payplatform,affaccount,realname,bonuspromocode,misc,saler,respresult,
            device,crc,depositCategory,self_fee,self_feerate,fixPromoP,currencycode) 
            values(@currency,@account,@amount,@bankname,@merchantno,@status,@referrerreg,@ipaddress,@macaddress,@referenceguid,
@payplatform,@affaccount,@realname,@bonuspromocode,@misc,@saler,@respresult,@device,
            CONVERT(VARCHAR(32), HashBytes('MD5', CONCAT((select top 1 id +1 from ofDeposit order by id desc),'{0}')), 2),@depositCategory,@self_fee,@self_feerate,@fixPromoP,@currencycode)";


        protected override ResponseModel<AddOrderReqReturn> Handle(AddOrderReq request)
        {

            var queryDisableMidsSql = string.Format(@"select u.account,u.level,u.realname,u.currencyType,u.withdrawcards,u.depositnumber,ue.disabledMids,u.referrerreg,affaccount from ofUser u with(nolock)
            left join ofUserExtention ue with(nolock) on u.account = ue.account
            WHERE u.account='{0}'", request.account);

            var queryResult = DapperBase.Query<ofUserExtention>(queryDisableMidsSql, param: null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionStringReadOnly);

            if (queryResult != null && queryResult.Count == 1) { 
            }
            else
            {
                return new ResponseModel<AddOrderReqReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "会员账号有误",
                    Data = null
                };

            }
            request.remarks = request.remarks.Replace("'", "''");
            request.realname = request.realname.Replace("'", "''");
            var user = queryResult.First();

            if (user.status != 1)
                return new ResponseModel<AddOrderReqReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "会员状态有误",
                    Data = null
                };

            var misc = JsonConvert.SerializeObject(new
                {
                    posted_realname = string.IsNullOrEmpty(request.realname) ? user.realname : request.realname,
                    usdt_erc20_xrate = 0
                });

            #region 计算会员自己要承担的手续费用 如果配置了 deductfee = 1

            var merchantModelSql = string.Format(@"select * from ofThirdpay with(nolock) where id={0} ", request.merId);
            var merchantModels = DapperBase.Query<Merchant>(merchantModelSql, null,
                 null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            if (merchantModels == null && merchantModels.Count <= 0)
            {
                return new ResponseModel<AddOrderReqReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "当前通道已经关闭...",
                    Data = null
                };

            }
            var merModel = merchantModels.First();
            var self_fee = merModel.miscModel.deductfee == 1 ? (float)request.amount * float.Parse((merModel.miscModel.self_feecent / 100).ToString("F2")) : 0;
            var self_feerate = merModel.miscModel.deductfee == 1 ? merModel.miscModel.self_feecent : 0;
            #endregion


            var src = string.Concat("_", request.account, "_", request.amount.ToString("F2"), "_", request.merId);
            var sql = string.Format(insertSql, src);

            var orderNo = UtilsLib.Common.GuidToLongString();

            var sqlArray = new List<string>();
            var param = new List<object> { };

            sqlArray.Add(insertSql);
            param.Add(new
            {
                currency = "currency",
                account = request.account,
                amount = request.amount,
                bankname = merModel.name,
                merchantno = string.Format(@"{0}{1}",request.merId, orderNo),
                status = 0,
                referrerreg = user.referrerreg,
                ipaddress = user.ipaddress,
                macaddress = "",
                referenceguid = orderNo,
                payplatform = request.merId,
                affaccount = user.affaccount,
                realname = user.realname,
                bonuspromocode = 0,
                misc = misc,
                saler = "",
                respresult = request.remarks,
                device = request.device,
                src = src, //crc 加密码源串
                depositCategory = 0,//暂时不用
                self_fee = self_fee, //代理要承担的比例
                self_feerate = self_feerate, //会员自己承担的比例
                fixPromoP = 0,
                currencycode = user.currencyType
            });

            if (DapperBase.ExecuteTransction(sqlArray, param, commandTimeout: 15000,
                      ConnectionString: DapperStringBase.ConnectionString))
            {
                return new ResponseModel<AddOrderReqReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "操作成功",
                    Data = null
                };
            }
            else
            {
                return new ResponseModel<AddOrderReqReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "操作失败",
                    Data = null
                };
            }


        }


    }
}
