using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using ShareEntity.Models;
using AWC2019.Core.ViewModel;
using FluentValidation;
using System.Data.SqlClient;
using Dapper;
using AWC2019.Core.Validation;
using Newtonsoft.Json;
using CMS.Core.Application.提款管理;
using ShareEntity.OutputModels;
using System.Runtime.Serialization;
using System.Transactions;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using Dapper.Contrib.Extensions;
using System.Net;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.存款管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class VerifyDepositRequest : IRequest<ResponseModel<VerifyDepositReturn>>
    {
        public long Id { get; set; }
        //public decimal fee { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class VerifyDepositReturn : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class VerifyDepositRequestValidator : AbstractValidator<VerifyDepositRequest>
    {
        public VerifyDepositRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class VerifyDepositRequestHandler : BaseRequestHandler<VerifyDepositRequest, ResponseModel<VerifyDepositReturn>>
    {
        public VerifyDepositRequestHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<VerifyDepositReturn> Handle(VerifyDepositRequest request)
        {
            using (TransactionScope scope = new TransactionScope())
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                try
                {
                    var depositRow = connection.Get<ofDeposit>(request.Id);
                    //var feecent = connection.QueryFirst<decimal>("" +
                    //    "SELECT TOP 1 feecent " +
                    //    "FROM ofNetBank WITH(NOLOCK) " +
                    //    "WHERE realname='" + depositRow.receiveperson+ "'");

                    //var fee = depositRow.amount * feecent;
                    // 已经处理过了
                    if (depositRow.status != 0)
                        return new ResponseModel<VerifyDepositReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "已经处理过了",
                            Data = null
                        };

                    var fee = depositRow.fee;
                    var accountRow = connection.Get<ofUser>(depositRow.account);

                    // Affiliate Reporting Area 
                    // 
                    //var financeMonth = long.Parse(Common.CurrentTime.ToString("yyyyMM"));
                    //var financeDay = long.Parse(Common.CurrentTime.ToString("yyyyMMdd"));
                    //string affaccount = accountRow.affaccount;
                    //if (string.IsNullOrEmpty(affaccount))
                    //{
                    //    affaccount = connection.QueryFirst<string>("SELECT TOP 1 account from ofAffiliate WITH(NOLOCK) WHERE defaultAgent=1");
                    //}



                    #region PlayerReport
                    //var playerMonthBuilder = new SqlBuilder();
                    //var playerMonthtemplate = playerMonthBuilder.AddTemplate("SELECT id FROM ofPlayerMonthReportFinance WITH(NOLOCK) WHERE account=@ACCOUNT and financeMonth=@FINANCEMONTH");
                    //playerMonthBuilder.AddParameters(new { ACCOUNT = accountRow.account, FINANCEMONTH = financeMonth });
                    //var playerMonthID = connection.QueryFirstOrDefault<long?>(playerMonthtemplate.RawSql, playerMonthtemplate.Parameters);

                    //var playerDailyBuilder = new SqlBuilder();
                    //var playerDailytemplate = playerDailyBuilder.AddTemplate("SELECT id FROM ofPlayerDailyReportFinance WITH(NOLOCK) WHERE account=@ACCOUNT and financeDay=@FINANCEDAILY");
                    //playerDailyBuilder.AddParameters(new { ACCOUNT = accountRow.account, FINANCEDAILY = financeDay });
                    //var playerDailyID = connection.QueryFirstOrDefault<long?>(playerDailytemplate.RawSql, playerDailytemplate.Parameters);

                    //if (playerMonthID != null)
                    //{
                    //    // UPDATE
                    //    var playerMonthRow = connection.Get<ofPlayerMonthReportFinance>(playerMonthID);
                    //    playerMonthRow.depositAmount += depositRow.amount;
                    //    playerMonthRow.depositFee += fee;
                    //    playerMonthRow.depositTimes++;
                    //    playerMonthRow.update_time = Common.CurrentTime;
                    //    connection.Update(playerMonthRow);
                    //}
                    //else
                    //{
                    //    // INSERT
                    //    var playerMonth = new ofPlayerMonthReportFinance()
                    //    {
                    //        account = accountRow.account,
                    //        depositTimes = 1,
                    //        financeMonth = financeMonth,
                    //        depositFee = fee,
                    //        affaccount = affaccount,
                    //        depositAmount = depositRow.amount,
                    //        update_time = Common.CurrentTime,
                    //        create_at = Common.CurrentTime
                    //    };
                    //    connection.Insert<ofPlayerMonthReportFinance>(playerMonth);
                    //}

                    //if (playerDailyID != null)
                    //{
                    //    // UPDATE
                    //    var playerDailyRow = connection.Get<ofPlayerDailyReportFinance>(playerDailyID);
                    //    playerDailyRow.depositAmount += depositRow.amount;
                    //    playerDailyRow.depositFee += fee;
                    //    playerDailyRow.depositTimes++;
                    //    playerDailyRow.update_time = Common.CurrentTime;
                    //    connection.Update(playerDailyRow);
                    //}
                    //else
                    //{
                    //    // INSERT
                    //    var playerDaily = new ofPlayerDailyReportFinance()
                    //    {
                    //        account = accountRow.account,
                    //        depositTimes = 1,
                    //        financeDay = financeDay,
                    //        depositFee = fee,
                    //        affaccount = affaccount,
                    //        depositAmount = depositRow.amount,
                    //        update_time = Common.CurrentTime,
                    //        create_at = Common.CurrentTime
                    //    };
                    //    connection.Insert<ofPlayerDailyReportFinance>(playerDaily);
                    //}
                    #endregion

                    #region AffiliateReport

                    // Check For Default Agent

                    //var AffiliateMonthBuilder = new SqlBuilder();
                    //var AffiliateMonthtemplate = AffiliateMonthBuilder.AddTemplate("SELECT id FROM ofAffMonthReportFinance WITH(NOLOCK) WHERE account=@ACCOUNT and financeMonth=@FINANCEMONTH");
                    //AffiliateMonthBuilder.AddParameters(new { ACCOUNT = affaccount, FINANCEMONTH = financeMonth });
                    //var AffiliateMonthID = connection.QueryFirstOrDefault<long?>(AffiliateMonthtemplate.RawSql, AffiliateMonthtemplate.Parameters);

                    //var AffiliateDailyBuilder = new SqlBuilder();
                    //var AffiliateDailytemplate = AffiliateDailyBuilder.AddTemplate("SELECT id FROM ofAffDailyReportFinance WITH(NOLOCK) WHERE account=@ACCOUNT and financeDay=@FINANCEDAILY");
                    //AffiliateDailyBuilder.AddParameters(new { ACCOUNT = affaccount, FINANCEDAILY = financeDay });
                    //var AffiliateDailyID = connection.QueryFirstOrDefault<long?>(AffiliateDailytemplate.RawSql, AffiliateDailytemplate.Parameters);

                    //if (AffiliateMonthID != null)
                    //{
                    //    // UPDATE
                    //    var AffiliateMonthRow = connection.Get<ofAffMonthReportFinance>(AffiliateMonthID);
                    //    AffiliateMonthRow.depositAmount += depositRow.amount;
                    //    AffiliateMonthRow.depositFee += depositRow.fee;
                    //    AffiliateMonthRow.depositTimes++;
                    //    AffiliateMonthRow.update_time = Common.CurrentTime;
                    //    connection.Update(AffiliateMonthRow);
                    //}
                    //else
                    //{
                    //    // INSERT
                    //    var AffiliateMonth = new ofAffMonthReportFinance()
                    //    {
                    //        account = affaccount,
                    //        depositTimes = 1,
                    //        financeMonth = financeMonth,
                    //        depositFee = depositRow.fee,
                    //        depositAmount = depositRow.amount,
                    //        update_time = Common.CurrentTime,
                    //        create_at = Common.CurrentTime
                    //    };
                    //    connection.Insert<ofAffMonthReportFinance>(AffiliateMonth);
                    //}

                    //if (AffiliateDailyID != null)
                    //{
                    //    // UPDATE
                    //    var AffiliateDailyRow = connection.Get<ofAffDailyReportFinance>(AffiliateDailyID);
                    //    AffiliateDailyRow.depositAmount += depositRow.amount;
                    //    AffiliateDailyRow.depositFee += depositRow.fee;
                    //    AffiliateDailyRow.depositTimes++;
                    //    AffiliateDailyRow.update_time = Common.CurrentTime;
                    //    connection.Update(AffiliateDailyRow);
                    //}
                    //else
                    //{
                    //    // INSERT
                    //    var AffiliateDaily = new ofAffDailyReportFinance()
                    //    {
                    //        account = affaccount,
                    //        depositTimes = 1,
                    //        financeDay = financeDay,
                    //        depositFee = depositRow.fee,
                    //        depositAmount = depositRow.amount,
                    //        update_time = Common.CurrentTime,
                    //        create_at = Common.CurrentTime
                    //    };
                    //    connection.Insert<ofAffDailyReportFinance>(AffiliateDaily);
                    //}
                    #endregion


                    #region ofPaymentRecord
                    var paymentRecord = new ofPaymentRecord()
                    {
                        currency = "CNY",
                        account = depositRow.account,
                        createtime = Common.CurrentTime,
                        referenceguid = depositRow.referenceguid,
                        transactiontype =1,
                        status = 1,
                        summaryType = 1,
                        preBalance = accountRow.amountactive,
                        bonus = depositRow.amount + fee,
                        afterBalance = accountRow.amountactive + depositRow.amount + fee,
                        summary = "存款",
                    };
                    connection.Insert<ofPaymentRecord>(paymentRecord);
                    #endregion

                    accountRow.amountactive += depositRow.amount + fee;
                    accountRow.depositamount += depositRow.amount;
                    accountRow.depositnumber = accountRow.depositnumber + 1;

                    depositRow.status = 1;
                    depositRow.reviewtime = Common.CurrentTime;
                    depositRow.reviewperson = request.Admin;
                    var depositResult = connection.Update(depositRow);
                    var accountResult = connection.Update(accountRow);
                    scope.Complete();

                    if (depositResult && accountResult) {

                        if (depositRow.bonuspromocode > 0)
                        {
                            try
                            {
                                var promoApiToken = Common.Security.GetToken3(depositRow.account, 0, depositRow.ipaddress,
                                    DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"), "/api/autopromo/AppDepositPromo", Guid.NewGuid().ToString());


                                WebClient wcching = new WebClient();
                                wcching.Headers[HttpRequestHeader.ContentType] = "application/json";
                                wcching.Headers[HttpRequestHeader.Authorization] = promoApiToken;
                                wcching.Encoding = System.Text.Encoding.UTF8;
                                var result = wcching.UploadString(new Uri("http://awcmgt22-promoservice.azurewebsites.net/api/autopromo/AppDepositPromo"),
                                            Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, dynamic>() {
                                        { "orderNo", depositRow.merchantno },
                                        { "account", depositRow.account },
                                        { "amount", 0},
                                        { "ipaddress", depositRow.ipaddress},
                                            })
                                        );
                                _logger.LogInformation("ChingAutoPromo - [Notify] orderid:{0}, result:{1}", depositRow.merchantno, result);

                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("ChingAutoPromo - [Exception] orderid:{0}, message:{1}", depositRow.merchantno, ex.Message);
                            }
                        }

                        return new ResponseModel<VerifyDepositReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                            message = "",
                            Data = null
                        };
                    }
                    else
                        return new ResponseModel<VerifyDepositReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                            message = "",
                            Data = null
                        };
                }
                catch (Exception ex)
                {
                    string errorStamp = Common.TimeStamp;
                    _logger.LogError("Exception:{0}, Stamp:{1}", ex.Message, errorStamp);
                    return new ResponseModel<VerifyDepositReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "运行出错 " + errorStamp,
                        Data = null
                    };
                }

            }

        }

    }
}

