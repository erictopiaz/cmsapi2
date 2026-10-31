using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using CMS.Data.Models;
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
using Newtonsoft.Json.Linq;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class ManualPayRequest : IRequest<ResponseModel<ManualPayRequestReturn>>
    {
        public string paymentgateway { get; set; }
        public long Id { get; set; }
        public decimal fee { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class ManualPayRequestReturn
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class ManualPayRequestValidator : AbstractValidator<ManualPayRequest>
    {
        public ManualPayRequestValidator()
        {
            RuleFor(x => x.fee).DecimalNonNegative();
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class ManualPayRequestHandler : BaseWithdraw<ManualPayRequest, ResponseModel<ManualPayRequestReturn>>
    {
        public ManualPayRequestHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<ManualPayRequestReturn> Handle(ManualPayRequest request)
        {

            //var resultDetail = GetWithdrawDetails(request.Id);
            //if (resultDetail == null)
            //    return new ResponseModel<ManualPayRequestReturn>()
            //    {
            //        code = ShareEntity.Global.ResponseCode.code.FAIL,
            //        message = "",
            //        Data = null
            //    };

            //var transactingID = resultDetail.id;
            //var transactingAccount = resultDetail.account;
            //var transactingAmount = resultDetail.amount;
            //var transactingCheckperson = resultDetail.checkperson;
            //var transactingStatus = resultDetail.status;            // should be in pay wait mode (status=1)

            //// Should be in paywait mode 
            //if (transactingStatus != 1)
            //{
            //    return new ResponseModel<ManualPayRequestReturn>()
            //    {
            //        code = ShareEntity.Global.ResponseCode.code.FAIL,
            //        message = "不可进行支付的状态",
            //        Data = null
            //    };
            //}

            //// Check if the lockperson is the same as the cancelperson
            //if (transactingCheckperson != request.Admin)
            //{
            //    return new ResponseModel<ManualPayRequestReturn>()
            //    {
            //        code = ShareEntity.Global.ResponseCode.code.FAIL,
            //        message = "本人不是锁定的使用者",
            //        Data = null
            //    };
            //}

            using (TransactionScope scope = new TransactionScope())
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {




                var withdrawRow = connection.Get<ofWithdrawal>(request.Id);


                // Get the fee per transaction and the fee percentage if the paymentgateway is 支付宝-微信

                #region AUTO Compute For Fees
                decimal autoFee = 0;
                string targetPlatform = "支付宝-微信";

                if (request.paymentgateway == targetPlatform)
                {
                    var directpay_details = connection.Get<ofSettings>("PAYEMENT_DIRECTPAY_V2");
                    JObject PAYEMENT_DIRECTPAY_V2 = JObject.Parse(directpay_details.body);

                    // Get Platform fee percentage based on the targetPlatform
                    JArray gatewaylist = JArray.Parse(PAYEMENT_DIRECTPAY_V2["payplatform"].ToString());
                    for (int i = 0; i < gatewaylist.Count; i++)
                        if (gatewaylist[i]["key"].ToString() == targetPlatform)
                        {
                            var feecent = decimal.Parse(gatewaylist[i]["feecent"].ToString());
                            var feepertransaction = gatewaylist[i]["feepertransaction"] !=null ? decimal.Parse(gatewaylist[i]["feepertransaction"].ToString()) : 0;
                            autoFee = (feecent > 0 ? (withdrawRow.amount * (feecent / 100)) : 0) + feepertransaction;
                            request.fee = autoFee;
                        }

                }
                #endregion

                // Should be in paywait mode 
                if (withdrawRow.status != 1)
                    return new ResponseModel<ManualPayRequestReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "不可进行支付的状态",
                        Data = null
                    };

                // Check if the lockperson is the same as the cancelperson
                if (withdrawRow.checkperson.ToLower() != request.Admin.ToLower())
                    return new ResponseModel<ManualPayRequestReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "本人不是锁定的使用者",
                        Data = null
                    };

                // Update Goes HERE
                withdrawRow.fee = request.fee;
                withdrawRow.payperson = request.Admin;
                withdrawRow.paytime = Common.CurrentTime;
                withdrawRow.status = 2;
                string withdraw_misc = withdrawRow.misc;
                JObject miscJobj = JObject.Parse(withdraw_misc);
                miscJobj.Add("gw", request.paymentgateway);
                withdraw_misc = miscJobj.ToString(Formatting.None);
                withdrawRow.misc = withdraw_misc;

                var accountRow = connection.Get<ofUser>(withdrawRow.account);
                accountRow.amountsuspend = accountRow.amountsuspend - withdrawRow.amount;
                accountRow.withdrawalamount += withdrawRow.amount;
                accountRow.withdrawalnumber += 1;


                #region ofPaymentRecord
                var paymentRecord = new ofPaymentRecord()
                {
                    currency = "CNY",
                    account = withdrawRow.account,
                    createtime = Common.CurrentTime,
                    referenceguid = withdrawRow.referenceguid,
                    status = 1,
                    summaryType = 0,
                    transactiontype = 0,
                    preBalance = accountRow.amountactive,
                    bonus = withdrawRow.amount + request.fee,
                    afterBalance = accountRow.amountactive + withdrawRow.amount + request.fee,
                    summary = "提款",
                };
                #endregion

                var paymentLog = new ofPaymentLog()
                {
                    TransactionId = withdrawRow.id.ToString(),
                    Account = withdrawRow.account,
                    StatusCode = "1",
                    Realname = withdrawRow.realname,
                    Cardno = withdrawRow.cardno,
                    Amount = withdrawRow.amount,
                    Createtime = Common.CurrentTime,
                    Bank = withdrawRow.cardbank,
                    platform = request.paymentgateway,
                    payadmin = request.Admin,
                    TransId = withdrawRow.id,
                    Result = "手动处理的",
                    misc = ""
                };

                
                connection.Insert<ofPaymentRecord>(paymentRecord);
                connection.Insert<ofPaymentLog>(paymentLog);
                var isAccountUpdateSuccess = connection.Update(accountRow);
                var isWithdrawalSuccess = connection.Update(withdrawRow);

                scope.Complete();

                if (isWithdrawalSuccess && isAccountUpdateSuccess)
                {
                    if (withdrawRow.cardbank.ToLower().Contains("usdt"))
                    {
                        MemCache.REDIS_CONNSTRING = CMS.Data.Misc.REDIS_CONNECTIONSTRING;
                        MemCache.PushQueue(new JObject()
                        {
                            ["task"] = "withdrawautopromonotifyching",
                            ["account"] = withdrawRow.account,
                            ["id"] = withdrawRow.id
                        }.ToString(Formatting.None));
                        //try
                        //{
                        //    var promoApiToken = Common.Security.GetToken3(withdrawRow.account, 0, "77.66.77.66",
                        //        DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"), "/api/autopromo/AppWithdrawPromo", Guid.NewGuid().ToString());
                        //    /*
                        //    {
                        //      "wid": 0,
                        //      "account": "string",
                        //      "ipaddress": "string",
                        //      "macaddress": "string",
                        //      "promoType": 0,
                        //      "device": 0
                        //    }
                        //     */

                        //    WebClient wcching = new WebClient();
                        //    wcching.Headers[HttpRequestHeader.ContentType] = "application/json";
                        //    wcching.Headers[HttpRequestHeader.Authorization] = promoApiToken;
                        //    wcching.Encoding = System.Text.Encoding.UTF8;
                        //    var result = wcching.UploadString(new Uri("http://awcmgt22-promoservice.azurewebsites.net/api/autopromo/AppWithdrawPromo"),
                        //                Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, dynamic>() {
                        //                    { "wid", withdrawRow.id },
                        //                    { "account", withdrawRow.account },
                        //                    { "ipaddress", "77.66.77.66"},
                        //                    { "macaddress", "none"},
                        //                    { "promoType",10},
                        //                    { "device", 0},
                        //                })
                        //            );
                        //    _logger.LogInformation("ChingAutoPromo - [Notify] WithdrawID:{0}, result:{1}", withdrawRow.id, result);

                        //}
                        //catch (Exception ex)
                        //{
                        //    _logger.LogError("ChingAutoPromo - [Exception] WithdrawID:{0}, message:{1}", withdrawRow.id, ex.Message);
                        //}

                    }



                    if (withdrawRow.wid == 1)
                    {
                        MemCache.PushQueue(new JObject()
                        {
                            ["task"] = "notifychingverifyfixedamountwdpromov2",
                            ["account"] = withdrawRow.account,
                            ["id"] = withdrawRow.referenceguid
                        }.ToString(Formatting.None));
                    }

                    return new ResponseModel<ManualPayRequestReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "",
                        Data = null
                    };
                }
                else
                    return new ResponseModel<ManualPayRequestReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "",
                        Data = null
                    };

            }

        }

    }
}

