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
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Thirdpay.OnePay;
using System.Net;
using Newtonsoft.Json.Linq;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.存款管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class VerifyFailedDepositRequest : IRequest<ResponseModel<VerifyFailedDepositReturn>>
    {
        public long Id { get; set; }
        //public decimal fee { get; set; }
        public string comment { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class VerifyFailedDepositReturn : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class VerifyFailedDepositRequestValidator : AbstractValidator<VerifyFailedDepositRequest>
    {
        public VerifyFailedDepositRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class VerifyFailedDepositRequestHandler : BaseRequestHandler<VerifyFailedDepositRequest, ResponseModel<VerifyFailedDepositReturn>>
    {
        public VerifyFailedDepositRequestHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<VerifyFailedDepositReturn> Handle(VerifyFailedDepositRequest request)
        {
            ofDeposit depositRow = new ofDeposit();
            ofThirdpay thirdpayRow = new ofThirdpay();
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var temp = connection.Get<ofDeposit>(request.Id);

                depositRow.currency = temp.currency;
                depositRow.account = temp.account;
                depositRow.realname = temp.realname;
                depositRow.amount = temp.amount;
                depositRow.fee = temp.fee;
                depositRow.rebate = temp.rebate;
                depositRow.feerate = temp.feerate;
                depositRow.feemax = temp.feemax;
                depositRow.deposittype = temp.deposittype;
                depositRow.depositmethod = temp.depositmethod;
                depositRow.additional = request.comment.Trim();
                depositRow.receiveperson = temp.receiveperson;
                depositRow.receivecardno = temp.receivecardno;
                depositRow.deposittime = temp.deposittime;
                depositRow.requesttime = temp.requesttime;
                depositRow.bankname = temp.bankname;
                depositRow.reviewtime = temp.reviewtime;
                depositRow.reviewperson = temp.reviewperson;
                depositRow.merchantno = temp.merchantno;
                depositRow.bankorderno = temp.bankorderno;
                depositRow.orderendtime = temp.orderendtime;
                depositRow.status = temp.status;
                depositRow.referrer = temp.referrer;
                depositRow.referrerreg = temp.referrerreg;
                depositRow.ipaddress = temp.ipaddress;
                depositRow.macaddress = temp.macaddress;
                depositRow.supplement = temp.supplement;
                depositRow.referenceguid = temp.referenceguid;
                depositRow.payplatform = temp.payplatform;
                depositRow.alert = temp.alert;
                depositRow.affaccount = temp.affaccount;
                depositRow.joinbonus = temp.joinbonus;
                depositRow.bonusamount = temp.bonusamount;
                depositRow.bonusbet = temp.bonusbet;
                depositRow.misc = temp.misc;
                depositRow.saler = temp.saler;
                depositRow.currencycode = temp.currencycode;

                // 侦测是否是第三方
                if (depositRow.payplatform == null)
                    return new ResponseModel<VerifyFailedDepositReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "不是第三方的存款方式",
                        Data = null
                    };


                // 侦测是否是第三方
                if ((Common.CurrentTime - depositRow.requesttime).TotalMinutes > 180)
                    return new ResponseModel<VerifyFailedDepositReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "已经超过可补单的时间范围",
                        Data = null
                    };


                // 已经处理过了
                if (depositRow.status != 0)
                    return new ResponseModel<VerifyFailedDepositReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "已经处理过了",
                        Data = null
                    };

                thirdpayRow = connection.Get<ofThirdpay>(depositRow.payplatform.ToString());
            }

            var response = new ResponseModel<VerifyFailedDepositReturn>();
            bool notifyChingAutoPromo = false;
            string notifyChingAutoPromo_Data = "";
            string notifyChingAutoPromo_Token = "";
            string notifyChingAutoPromo_merchantno = "";
            string notifyChingAutoPromo_account = depositRow.account;

            using (TransactionScope scope = new TransactionScope())
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                try
                {
                    var depositRowToCancel = connection.Get<ofDeposit>(request.Id);
                    var accountRow = connection.Get<ofUser>(depositRow.account);



                    var fee = depositRow.fee.ToString();
                    string misc = thirdpayRow.misc;
                    decimal rebatePercentage = 0;
                    string rebate = "0";

                    // COMPUTE FOR THE FEE AND REBATE
                    decimal feerate = 0;
                    try
                    {
                        if (!string.IsNullOrEmpty(misc))
                        {
                            var miscDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(misc);
                            decimal feecent = 0;
                            if (miscDict.ContainsKey("feecent") && decimal.TryParse(miscDict["feecent"], out feecent))
                            {
                                feerate = feecent;
                                //if (feecent >= 50)
                                //{
                                //    _logger.LogCritical("FeeComputation - [Exceeded 50%] feecent:{0}%, merchantno:{1}",
                                //        feecent.ToString("0.00"),
                                //        depositRow.merchantno);
                                //}
                                //else if (feecent <= -50)
                                //{
                                //    _logger.LogCritical("FeeComputation - [Below -50] feecent:{0}%, merchantno:{1}",
                                //        feecent.ToString("0.00"),
                                //        depositRow.merchantno);
                                //}
                                //else
                                //{
                                    decimal feeFloat = depositRow.amount * (feecent / 100);
                                    _logger.LogCritical("FeeComputation - [Compute] feecent:{0}%, merchantno:{1}, fee:{2}",
                                        feecent.ToString("0.00"),
                                        depositRow.merchantno,
                                        feeFloat.ToString("0.00"));

                                    // Verify Fee
                                    fee = feeFloat.ToString("0.00");
                                    //fee = "0";
                                //}

                            }

                            if (miscDict.ContainsKey("rebate") && decimal.TryParse(miscDict["rebate"], out rebatePercentage))
                            {
                                if (rebatePercentage >= 5)
                                {
                                    _logger.LogError("RebateComputation - [Exceeded 5%] rebate:{0}%, merchantno:{1}",
                                        rebatePercentage.ToString("0.00"),
                                        depositRow.merchantno);
                                }
                                else if (rebatePercentage < 0)
                                {
                                    _logger.LogError("RebateComputation - [Below 0] rebate:{0}%, merchantno:{1}",
                                        rebatePercentage.ToString("0.00"),
                                        depositRow.merchantno);
                                }
                                else
                                {
                                    rebate = (depositRow.amount * (rebatePercentage / 100)).ToString("0.00");
                                    _logger.LogError("RebateComputation - [Compute] rebate:{0}%, merchantno:{1}, computedrebate:{2}",
                                        rebatePercentage.ToString("0.00"), depositRow.merchantno, rebate);
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical("FeeComputation, RebateComputation - [Exception] message :{0}", ex.Message);
                        fee = "0";
                        rebate = "0";
                    }



                    #region ofPaymentRecord
                    var paymentRecord = new ofPaymentRecord()
                    {
                        currency = "CNY",
                        account = depositRow.account,
                        createtime = Common.CurrentTime,
                        referenceguid = depositRow.referenceguid,
                        transactiontype = 1,
                        status = 1,
                        summaryType = 1,
                        preBalance = accountRow.amountactive,
                        bonus = depositRow.amount + decimal.Parse(rebate),
                        afterBalance = accountRow.amountactive + depositRow.amount + decimal.Parse(rebate),
                        summary = "存款-补单",
                    };
                    #endregion

                    accountRow.amountactive += depositRow.amount + decimal.Parse(rebate);
                    accountRow.depositamount += depositRow.amount;
                    accountRow.depositnumber = accountRow.depositnumber + 1;

                    depositRowToCancel.status = 2;
                    depositRowToCancel.reviewtime = Common.CurrentTime;
                    depositRowToCancel.reviewperson = request.Admin;

                    try
                    {
                        JObject tempJson = JObject.Parse(depositRowToCancel.misc);
                        tempJson.Add("admincancel", true);
                        depositRowToCancel.misc = tempJson.ToString(Formatting.None);
                    }
                    catch (Exception){}


                    depositRow.status = 1;
                    depositRow.orderendtime = Common.CurrentTime;
                    depositRow.reviewperson = request.Admin;
                    depositRow.fee = decimal.Parse(fee);
                    depositRow.rebate = decimal.Parse(rebate);
                    depositRow.merchantno = string.Concat("补单 - ", depositRow.merchantno);
                    depositRow.bonuspromocode = depositRowToCancel.bonuspromocode;

                    var depositInsertResult = connection.Insert(depositRow);
                    var depositCancelResult = connection.Update(depositRowToCancel);
                    var accountResult = connection.Update(accountRow);
                    var paymentRecordInsertResult = connection.Insert<ofPaymentRecord>(paymentRecord);

                    if (depositInsertResult > 0 && depositCancelResult && accountResult && paymentRecordInsertResult > 0)
                    {
                        scope.Complete();

                        if (depositRowToCancel.bonuspromocode > 0)
                        {
                            notifyChingAutoPromo_merchantno = depositRowToCancel.merchantno; ;
                            notifyChingAutoPromo = true;
                            notifyChingAutoPromo_Token = Common.Security.GetToken3(depositRowToCancel.account, 0, depositRowToCancel.ipaddress,
                                DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"), "/api/autopromo/AppDepositPromo", Guid.NewGuid().ToString());
                            notifyChingAutoPromo_Data = Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, dynamic>() {
                                { "orderNo", depositRowToCancel.merchantno },
                                { "account", depositRowToCancel.account },
                                { "amount", 0},
                                { "ipaddress", depositRowToCancel.ipaddress},
                            });
                        }


                        response = new ResponseModel<VerifyFailedDepositReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                            message = "",
                            Data = null
                        };
                    }
                    else
                        response = new ResponseModel<VerifyFailedDepositReturn>()
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
                    return new ResponseModel<VerifyFailedDepositReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "运行出错 " + errorStamp,
                        Data = null
                    };
                }

            }

            if (notifyChingAutoPromo)
            {
                try
                {

                    WebClient wcching = new WebClient();
                    wcching.Headers[HttpRequestHeader.ContentType] = "application/json";
                    wcching.Headers[HttpRequestHeader.Authorization] = notifyChingAutoPromo_Token;
                    wcching.Encoding = System.Text.Encoding.UTF8;
                    //Thread.Sleep(1000);
                    var result = wcching.UploadString(new Uri("http://awcmgt22-promoservice.azurewebsites.net/api/autopromo/AppDepositPromo"), notifyChingAutoPromo_Data);
                    _logger.LogInformation("ChingAutoPromo - [Notify] orderid:{0}, result:{1}", notifyChingAutoPromo_merchantno, result);
                }
                catch (Exception ex)
                {
                    _logger.LogError("ChingAutoPromo - [Exception] orderid:{0}, message:{1}", notifyChingAutoPromo_merchantno, ex.Message);
                }

            }

            string chingTriggerResult = Common.Misc.ChingApiTrigger(notifyChingAutoPromo_account, "/api/promo/AppDepositPointsFor",
                Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, dynamic>() {
                                { "orderNo", notifyChingAutoPromo_merchantno },
                                { "account", notifyChingAutoPromo_account },
                                { "amount", 0},
                                { "ipaddress", "168.95.1.1"},
                                { "miscType", 3}
                })
            );

            _logger.LogInformation("ChingTrigger - [Result] message:{0}", chingTriggerResult);

            return response;
        }
    }
}

