using AutoMapper;
using CMS.Core.Application.存款管理.Transact;
using CMS.Core.Application.网站管理.Transact;
using ShareEntity.InputModels.Game.ttg;
using ShareEntity.Models;
using ShareEntity.OutputModels;
using ShareEntity.SqlCommandText;
using Dapper;
using Dapper.Contrib.Extensions;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Core.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;
using UtilsLib.lang;

namespace CMS.Core.Application.提款管理.Queries {
    
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class ReturnFunds : IRequest<ResponseModel<ReturnFundsReturn>> {
        public long rowid { get; set; }
        public string account { set; get; }
        public decimal amount { set; get; }
        /// <summary>
        /// 回退订单备注
        /// </summary>
        public string remarks { set; get; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class ReturnFundsReturn {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class ReturnFundsValidator : AbstractValidator<ReturnFunds> {
        public ReturnFundsValidator() {
            RuleFor(x => x.rowid)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class ReturnFundsHandler : BaseWithdraw<ReturnFunds, ResponseModel<ReturnFundsReturn>> {
        public ReturnFundsHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<ReturnFundsReturn> Handle(ReturnFunds request) {


            lock(this) {
                var sqlArray = new List<string>();
                var param = new List<object>();

                var querySql = string.Format(@"select w.*,u.id uid from ofWithdrawal w with(nolock)
left join ofUser u with(nolock) on w.account = u.account
where w.id=@rowid");
                var withdrawRows = DapperBase.Query<ofWithdrawalEx>(querySql, param: request, commandTimeout: 15000,
                    ConnectionString: DapperStringBase.ConnectionString);
                if(withdrawRows != null) {
                    var withdrawRow = withdrawRows.First();
                    if (withdrawRow.status != 2)
                        return new ResponseModel<ReturnFundsReturn>() {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "回撤提款定单失败(error status)",
                            Data = null
                        };
                    //要加退的金额
                    if (withdrawRow.amount < request.amount)
                        return new ResponseModel<ReturnFundsReturn>() {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "回退金额有误! ",
                            Data = null
                        };
                    if (!withdrawRow.account.Trim().Equals(request.account.Trim(), StringComparison.OrdinalIgnoreCase))
                        return new ResponseModel<ReturnFundsReturn>() {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = string.Format(@"退账号有误! req:{0}-{1}",request.account, withdrawRow.account),
                            Data = null
                        };

                    var currentTime = DateTime.UtcNow.AddHours(8);

                    if (withdrawRow.requesttime > currentTime.AddMinutes(-10))
                        return new ResponseModel<ReturnFundsReturn>() {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "回退金额失败:【定单提交10分后,才能人工调整】 ",
                            Data = null
                        };

                    if (withdrawRow.requesttime.AddHours(120) < currentTime)
                        return new ResponseModel<ReturnFundsReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = $"回退金额失败{request.rowid}:【120小时已过,联系技术后再处理，谢谢】 ",
                            Data = null
                        };


                    if (withdrawRow.amount == request.amount) {
                        //完全回退金额（当前订单取消）
                        request.remarks = request.remarks.Replace("'", "''");
                        var cancelSql = string.Format(@"update ofWithdrawal set status=3, alreadyReturnTimes=alreadyReturnTimes+1, cancelreason=@remarks where id=@rowid and status=2");
                        sqlArray.Add(cancelSql);
                        param.Add(request);

                        var referenceguid = string.Format(@"wresize{0}", withdrawRow.referenceguid);
                        var currency = LangCodes.CurrencyMapLang[withdrawRow.currencycode].currency;
                        sqlArray.Add(PromoSqlCommandText.CreateResizeSql_V);
                        param.Add(new {
                            currency = currency,
                            account = withdrawRow.account,
                            amount = withdrawRow.amount,
                            createperson = request.Admin,
                            remarks = string.Format(@"提款补单【全额退还】id:{0},时间:{1}, 退前金额:{2}", request.rowid, withdrawRow.requesttime,withdrawRow.amount),
                            referenceguid = referenceguid,
                            decreasetype = 1,
                            referrer ="",
                            appendType=-1,
                            orderNo = "",
                            accountType = 0,
                            amountType = 0,
                            r_f_uid = withdrawRow.uid
                        }) ;

                        if (DapperBase.ExecuteTransction(sqlArray, param, commandTimeout: 15000,
                            ConnectionString: DapperStringBase.ConnectionString)) {
                            return new ResponseModel<ReturnFundsReturn>() {
                                code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                                message = "操作成功,请到余额列表确认退金额是否正确",
                                Data = null
                            };
                        }
                    }
                    else {
                        //退回部份金额
                        //更新当前定金额和余额调整金额
                        request.remarks = request.remarks.Replace("'", "''");
                        var payAmount = withdrawRow.amount - request.amount;
                        var w_misc = JsonConvert.DeserializeObject<Dictionary<string, object>>(withdrawRow.misc);
                        var dkey = "preRuturnAmount";//当前订单原始金额，撤单后不变
                        if (!w_misc.ContainsKey(dkey)) {
                            w_misc.Add(dkey, withdrawRow.amount);
                        }
                        else {
                            w_misc[dkey] = withdrawRow.amount;
                        }
                        var updateSql = string.Format(@"update ofWithdrawal set amount=@payAmount,alreadyReturnTimes=alreadyReturnTimes+1, cancelreason=@remarks, misc=@misc where id=@rowid");
                        sqlArray.Add(updateSql);
                        param.Add(new {
                            payAmount = payAmount,
                            misc = JsonConvert.SerializeObject(w_misc),
                            rowid = request.rowid,
                            remarks = request.remarks
                        });

                        var referenceguid = string.Format(@"resize{0}", withdrawRow.referenceguid);
                        var currency = LangCodes.CurrencyMapLang[withdrawRow.currencycode].currency;
                        sqlArray.Add(PromoSqlCommandText.CreateResizeSql_V);
                        param.Add(new {
                            currency = currency,
                            account = withdrawRow.account,
                            amount = request.amount,//要退还的金额
                            createperson = request.Admin,
                            remarks = string.Format(@"提款补单【部分金额退还】id:{0},时间:{1},退前金额:{2}", request.rowid, withdrawRow.requesttime,withdrawRow.amount),
                            referenceguid = referenceguid,
                            decreasetype = 1,
                            referrer = "",
                            appendType = -1,
                            orderNo = "",
                            accountType = 0,
                            amountType = 0,
                            r_f_uid = withdrawRow.uid
                        });

                        if (DapperBase.ExecuteTransction(sqlArray, param, commandTimeout: 15000,
                            ConnectionString: DapperStringBase.ConnectionString)) {
                            return new ResponseModel<ReturnFundsReturn>() {
                                code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                                message = "操作成功,请到余额列表确认退金额是否正确",
                                Data = null
                            };
                        }

                    }
                }

                return new ResponseModel<ReturnFundsReturn>() {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "未查询到提款单! ",
                    Data = null
                };
            }
            
        }

    }
}
