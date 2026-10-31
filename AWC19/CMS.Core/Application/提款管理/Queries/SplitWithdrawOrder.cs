using AutoMapper;
using AWC2019.Core.Validation;
using AWC2019.Core.ViewModel;
using CMS.Core.Application.提款管理;
using CMS.Data.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using DBLibs.WXDapper;
using FluentValidation;
using GameLibs.PP;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Core.Configuration;
using Newtonsoft.Json;
using NSwag.Annotations;
using ShareEntity.Models;
using ShareEntity.OutputModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace CMS.Core.Application.提款管理.Queries
{
    

    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class SplitWithdrawOrder : IRequest<ResponseModel<SplitWithdrawOrderReturn>>
    {
        public long Id { get; set; }
        /// <summary>
        /// 切割金额 #分割
        /// </summary>
        public string amounts { get; set; } = "";

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class SplitWithdrawOrderReturn
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class SplitWithdrawOrderValidator : AbstractValidator<SplitWithdrawOrder>
    {
        public SplitWithdrawOrderValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    public class WithdrawMiscModel
    {
        //{"cryptoAmount":"0.00","cryptoRate":0.0,"origamount":40000.0}
        public decimal cryptoAmount { get; set; } = 0.0m;
        public decimal cryptoRate { get; set; } = 0.0m;
        public decimal origamount { get; set; } = 0.0m;
    }

    public class BankWithdrawTextSql
    {
        /// <summary>
        /// 更新主账号余额
        /// </summary>
        public const string UpdateUserAmountSql = @"UPDATE OFUSER SET amountactive=amountactive-@amount,amountsuspend=amountsuspend+@amount 
WHERE account=@account and amountactive-@amount>=0 ";

        /// <summary>
        /// 记录提款记录
        /// </summary>
        public const string InsertWithdrawSql = @"
INSERT INTO OFWITHDRAWAL(currencycode, currency, account, realname,  amount,fee,  cardno, cardaddress, cardbank, requesttime, requestip, requestaddress, status,balance,referrer,referrerreg,level,referenceguid,gametype,affaccount,misc,saler,wid,delayToTime,fixWithdrawP,pid,verify_guid)
                 VALUES (@currencycode,@currency,@account,@cardname,@amount,@fee,@cardno,@cardaddress,@cardbank,@requesttime,@requestip,@requestaddress,@status,@balance,@referrer,@referrerreg,@level,@referenceguid,@gametype,@affaccount,@misc,@saler,@wid,@delaytotime,@fixwithdrawp,@pid,@verify_guid)";


        /// <summary>
        /// 更新被拆提款定单状态
        /// </summary>
        public const string UpdateWithdrawSql = @"UPDATE ofWithdrawal SET status=3
WHERE account=@account and id=@id and status=1";

    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class SplitWithdrawOrderHandler : BaseWithdraw<SplitWithdrawOrder, ResponseModel<SplitWithdrawOrderReturn>>
    {
        public SplitWithdrawOrderHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override  ResponseModel<SplitWithdrawOrderReturn> Handle(SplitWithdrawOrder request)
        {

            var sqlArray = new List<string>();
            var param = new List<object>();
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var withdrawRow = connection.Get<ofWithdrawal>(request.Id);
                if (withdrawRow.wid != 1)
                {
                    return new ResponseModel<SplitWithdrawOrderReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "非网银提示，金额不让切割",
                        Data = null
                    };
                }

                if (withdrawRow.status != 1)
                {
                    return new ResponseModel<SplitWithdrawOrderReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "订单状态有误，不能切割金额",
                        Data = null
                    };
                }

                if (string.IsNullOrEmpty(request.amounts))
                {
                    return new ResponseModel<SplitWithdrawOrderReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "分割金额格式有误",
                        Data = null
                    };
                }
                else
                {
                    request.amounts = request.amounts.Replace("'","''");
                }

                var amounts = request.amounts.Split('#');
                var totalAmount = 0.0m;
                foreach (var amount in amounts)
                {
                    totalAmount = totalAmount + decimal.Parse(amount.ToString());
                }

                if (totalAmount != withdrawRow.amount)
                {
                    return new ResponseModel<SplitWithdrawOrderReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = $"总提款金额有误，{totalAmount} != {withdrawRow.amount}",
                        Data = null
                    };
                }
                //{"cryptoAmount":"0.00","cryptoRate":0.0,"origamount":40000.0}
                var miscModel = JsonConvert.DeserializeObject<WithdrawMiscModel>(withdrawRow.misc);
                if (miscModel.origamount > 0)
                {
                    //这前有被切过，
                }
                else
                {
                    //之前没有被切过
                    miscModel.origamount = withdrawRow.amount;
                }

                var miscJsonString = JsonConvert.SerializeObject(miscModel);

                var orderIndx = 0;
                var currentDate = DateTime.UtcNow.AddHours(8);
                //金额有被切割
                //记录提款记录
                foreach (var amount in amounts)
                {
                    sqlArray.Add(BankWithdrawTextSql.InsertWithdrawSql);
                    param.Add(new
                    {
                        currencycode = withdrawRow.currencycode,
                        currency = withdrawRow.currency,
                        account = withdrawRow.account,
                        cardname = withdrawRow.realname,// realname,
                        amount = Convert.ToDecimal(amount),
                        fee = 0, //网银和定额的提款手续费为0
                        cardno = withdrawRow.cardno, //卡号
                        cardaddress = withdrawRow.cardaddress, // bankaddress, //开户行地址
                        cardbank = withdrawRow.cardbank, // bankName, //银行名
                        requesttime = withdrawRow.requesttime, // currentDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        requestip = withdrawRow.requestip, //.ipaddress,
                        requestaddress = withdrawRow.requestaddress, // model.macaddress,
                        status = withdrawRow.status, //0
                        balance = withdrawRow.balance,
                        referrer = withdrawRow.referrer,// userModel.referrer,
                        referrerreg = withdrawRow.referrerreg,
                        level = withdrawRow.level,
                        referenceguid = $"{withdrawRow.referenceguid}{orderIndx}",//
                        gametype = -1, //
                        affaccount = withdrawRow.affaccount,
                        misc = miscJsonString,
                        saler = withdrawRow.saler,
                        wid = withdrawRow.wid,
                        delaytotime = withdrawRow.delayToTime, //fixWithdrawDelayTime,
                        fixwithdrawp = withdrawRow.fixWithdrawP, //fixWithdrawPromo_p,
                        pid = withdrawRow.pid, // model.pid,
                        verify_guid = string.Format(@"{0}{1}{2}", withdrawRow.currency, UtilsLib.Common.GuidToLong(), orderIndx) //
                    });
                    ++orderIndx;
                }

                if (sqlArray.Count > 0)
                {
                    //取消当前提款记录，并生成新的切割记录
                    sqlArray.Add(BankWithdrawTextSql.UpdateWithdrawSql);
                    param.Add(new
                    {
                        account = withdrawRow.account,
                        id = withdrawRow.id,
                    });
                }
            }

            if (sqlArray.Count > 0)
            {
                //开始切割金额
                var r = DapperBase.ExecuteTransction(sqlArray, param,
                            commandTimeout: 1500, ConnectionString: CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING);
                if (r)
                {
                    return new ResponseModel<SplitWithdrawOrderReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "操作成功",
                        Data = null
                    };
                }
                else
                {
                    return new ResponseModel<SplitWithdrawOrderReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "操作失败",
                        Data = null
                    };
                }
            }
            else
            {
                return new ResponseModel<SplitWithdrawOrderReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "操作失败(无可执行脚本)",
                    Data = null
                };
            }


        }

    }
}
