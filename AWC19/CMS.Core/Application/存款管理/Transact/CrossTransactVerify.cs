using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using DBLibs.WXDapper;
using FluentValidation;
using LogLib.Helper;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using ShareEntity.Models;
using ShareEntity.OutputModels;
using ShareEntity.SqlCommandText;
using SmsLib.Models;
using SmsLib.platforms;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.存款管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class CrossTransactVerify : IRequest<ResponseModel<CrossTransactVerifyReturn>>
    {
        public long Id { get; set; }
        //public decimal fee { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }

    }

    public class CrossTransactVerifyReturn : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class CrossTransactVerifyValidator : AbstractValidator<CrossTransactVerify>
    {
        public CrossTransactVerifyValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class CrossTransactVerifyHandler : BaseRequestHandler<CrossTransactVerify, ResponseModel<CrossTransactVerifyReturn>>
    {
        private string SendRequest(string API, string[] parameters)
        {
            string getparameters = "";
            for (int i = 0; i < parameters.Length; i++)
                parameters[i] = "args=" + parameters[i];
            getparameters = string.Join('&', parameters);
            //string requestString = string.Concat("http://awcsvc-ea-newcmsstating.azurewebsites.net", "/command.asmx/", API, "?", getparameters);
            string requestString = string.Concat("http://awcsvc-ea.azurewebsites.net", "/command.asmx/", API, "?", getparameters);
            WebClient wc = new WebClient();
            try
            {
                var result = wc.DownloadString(requestString);
                result = result.Substring(result.IndexOf('>') + 1);
                result = result.Substring(result.IndexOf('>') + 1);
                result = result.Substring(0, result.IndexOf('<'));
                return result;
            }
            catch (Exception)
            {
                return "-1";
            }

        }

        /// <summary>
        /// 互转审核后，发送短信通知
        /// </summary>
        /// <param name="telephone">电话号</param>
        /// <param name="merchantAccount">品牌</param>
        /// <param name="account">会员账号</param>
        /// <param name="sqlConnectString"></param>
        /// <returns></returns>
        private async Task<bool> SendWithdrawMessageAsync(string telephone, string merchantAccount, string account, string sqlConnectString)
        {
            try
            {
                var QueryMessageConfigSql = @"select body from ofSettings where code='intl_mobilemessage_configs'";
                //signString
                var configObj = await DapperBase.ExecuteScalarValueAsync(QueryMessageConfigSql, null,
                                commandTimeout: 15000, ConnectionString: sqlConnectString);
                var configData = JsonConvert.DeserializeObject<MessageConfigModel>(configObj.ToString());
                var sendCode = UtilsLib.Common.GenerateDightCode(6);
                var responseCode = false;
                responseCode = await SmsJZ.GetSendJZhouMessageCodeAsync(telephone, sampleCode: sendCode, "",
                    configData, merchantAccount: merchantAccount);
                if (responseCode)
                {
                    //发送成功后，写入验证码查询表格
                    var recordSendCode = await DapperBase.ExecuteTransctionAsync(MessageTextSql.InserPhoneCodeSql,
                                    new
                                    {
                                        account = account,
                                        type = 100,
                                        validate = sendCode,
                                        ipaddress = "127.255.255.255",
                                        macaddress = "后台生成IP",
                                        body = telephone,
                                        accountType = 0,//0:会员，1：合营
                                        phCode = 86
                                    },
                                    commandTimeout: 1500, ConnectionString: sqlConnectString);
                    if (!recordSendCode)
                    {
                        WriteToFile.TxtWriteTo("/DepositController/CrossTransactVerify_recordSendCode_failed",
                        "CrossTransactVerify failed",
                         JsonConvert.SerializeObject(new
                         {
                             telephone = telephone,
                             merchantAccount = merchantAccount,
                             account = account,
                             responseCode = responseCode,
                             sendCode = sendCode
                         }), true);
                    }
                }
                else
                {
                    WriteToFile.TxtWriteTo("/DepositController/CrossTransactVerify_responseCode_failed",
                    "CrossTransactVerify failed",
                 JsonConvert.SerializeObject(new
                 {
                     telephone = telephone,
                     merchantAccount = merchantAccount,
                     account = account,
                     responseCode = responseCode,
                     sendCode = sendCode
                 }), true);

                }

                return responseCode;
            }
            catch (Exception e)
            {
                WriteToFile.TxtWriteTo("/DepositController/CrossTransactVerify_SendWithdrawMessageAsync_ex", 
                    "CrossTransactVerify ex",
                 JsonConvert.SerializeObject(new
                 {
                     telephone = telephone,
                     merchantAccount = merchantAccount,
                     account = account,
                     exStackTrace = e.StackTrace
                 }), true);
                return false;
            }
            
        }

        public CrossTransactVerifyHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<CrossTransactVerifyReturn> Handle(CrossTransactVerify request)
        {
            var sqlConnectString = CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING;
            using (TransactionScope scope = new TransactionScope())
            using (SqlConnection connection = new SqlConnection(sqlConnectString))
            {
                try
                {
                    var crosstransactRow = connection.Get<ofCrossTransact>(request.Id);
                    if (crosstransactRow.status != 0)
                        return new ResponseModel<CrossTransactVerifyReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "已经处理过了",
                            Data = null
                        };
                    crosstransactRow.reviewtime = Common.CurrentTime;
                    crosstransactRow.status = 1;
                    crosstransactRow.reviewperson = request.Admin;
                    var crosstransactResult = connection.Update(crosstransactRow);

                    // WITHDRAW Portion
                    var withdrawRow = connection.Get<ofWithdrawal>(crosstransactRow.withdrawid);
                    var deposit_id = connection.Query<long>("" +
                        "SELECT id " +
                        "FROM ofDeposit WITH(NOLOCK) " +
                        "WHERE merchantno = '" + crosstransactRow.orderid + "'");
                    var depositRow = connection.Get<ofDeposit>(deposit_id);


                    if (depositRow.status == 2)
                        return new ResponseModel<CrossTransactVerifyReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "存款已经取消了 - (不允许审核。 只能取消)",
                            Data = null
                        };

                    //if (depositRow.status >0)
                    //    return new ResponseModel<CrossTransactVerifyReturn>()
                    //    {
                    //        code = ShareEntity.Global.ResponseCode.code.FAIL,
                    //        message = "存款已经处理了, 不允许审核。 只能取消",
                    //        Data = null
                    //    };


                    if (withdrawRow.account.ToLower() == depositRow.account.ToLower())
                        return new ResponseModel<CrossTransactVerifyReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "用户同一个,不允许存提",
                            Data = null
                        };

                    // Should be in paywait mode 
                    if (withdrawRow.status != 1)
                        return new ResponseModel<CrossTransactVerifyReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "不可进行支付的状态",
                            Data = null
                        };

                    // Update Goes HERE
                    withdrawRow.fee = 0;
                    withdrawRow.payperson = request.Admin;
                    withdrawRow.paytime = Common.CurrentTime;
                    withdrawRow.status = 2;

                    var withdrawAccountRow = connection.Get<ofUser>(withdrawRow.account);
                    withdrawAccountRow.amountsuspend = withdrawAccountRow.amountsuspend - withdrawRow.amount;
                    withdrawAccountRow.withdrawalamount += withdrawRow.amount;
                    withdrawAccountRow.withdrawalnumber += 1;

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
                        preBalance = withdrawAccountRow.amountactive,
                        bonus = withdrawRow.amount,
                        afterBalance = withdrawAccountRow.amountactive + withdrawRow.amount,
                        summary = "提款",
                    };
                    #endregion
                    connection.Insert<ofPaymentRecord>(paymentRecord);




                    var isWithdrawAccountUpdateSuccess = connection.Update(withdrawAccountRow);
                    var isWithdrawalSuccess = connection.Update(withdrawRow);

                    if (isWithdrawAccountUpdateSuccess && isWithdrawalSuccess)
                    {
                        scope.Complete();

                        SendRequest("API", new string[] {
                            "ThirdpayCompleteWithAmountVerify",
                                depositRow.merchantno,
                                "",
                                depositRow.amount.ToString("0.00"),
                                "0" });

                        if (withdrawRow.delayToTime != null &&
                           Common.CurrentTime.AddMinutes(30) >= withdrawRow.delayToTime)
                        {
                            // notify ching
                            //MemCache.REDIS_CONNSTRING = CMS.Data.Misc.REDIS_CONNECTIONSTRING;
                            //MemCache.PushQueue(new JObject()
                            //{
                            //    ["task"] = "withdrawfixedpromonotifyching",
                            //    ["orderno"] = withdrawRow.id.ToString(),
                            //    ["account"] = withdrawRow.account
                            //}.ToString(Formatting.None));
                            MemCache.REDIS_CONNSTRING = CMS.Data.Misc.REDIS_CONNECTIONSTRING;
                            MemCache.PushQueue(new JObject()
                            {
                                ["task"] = "notifychingverifyfixedamountwdpromov2",
                                ["id"] = withdrawRow.referenceguid.ToString(),
                                ["account"] = withdrawRow.account
                            }.ToString(Formatting.None));
                        }

                        #region 异步短信发送，不必担心发送结果（提款审核后，提示会员可能到账） ，目前仅 cny 
                        if (withdrawAccountRow.currencyType == 156)
                        {
                           SendWithdrawMessageAsync(telephone: withdrawAccountRow.telephone,
                                merchantAccount: withdrawAccountRow.merchantAccount,
                                account: withdrawAccountRow.account, sqlConnectString: sqlConnectString);
                        }
                        #endregion

                        return new ResponseModel<CrossTransactVerifyReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                            message = "",
                            Data = null
                        };
                    }
                    else {
                        return new ResponseModel<CrossTransactVerifyReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "",
                            Data = null
                        };
                    }

                    #region trash

                    //// Gets the FEE and REBATE
                    //var thirdpayRow = connection.Get<ofThirdpay>("10530");
                    //var fee = depositRow.fee.ToString();
                    //string misc = thirdpayRow.misc;
                    //decimal rebatePercentage = 0;
                    //string rebate = "0";
                    //// COMPUTE FOR THE FEE AND REBATE
                    //decimal feerate = 0;
                    //try
                    //{
                    //    if (!string.IsNullOrEmpty(misc))
                    //    {
                    //        var miscDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(misc);
                    //        decimal feecent = 0;
                    //        if (miscDict.ContainsKey("feecent") && decimal.TryParse(miscDict["feecent"], out feecent))
                    //        {
                    //            feerate = feecent;
                    //            if (feecent >= 5)
                    //            {
                    //                _logger.LogCritical("FeeComputation - [Exceeded 5%] feecent:{0}%, merchantno:{1}",
                    //                    feecent.ToString("0.00"),
                    //                    depositRow.merchantno);
                    //            }
                    //            else if (feecent <= -5)
                    //            {
                    //                _logger.LogCritical("FeeComputation - [Below -5] feecent:{0}%, merchantno:{1}",
                    //                    feecent.ToString("0.00"),
                    //                    depositRow.merchantno);
                    //            }
                    //            else
                    //            {
                    //                decimal feeFloat = depositRow.amount * (feecent / 100);
                    //                _logger.LogCritical("FeeComputation - [Compute] feecent:{0}%, merchantno:{1}, fee:{2}",
                    //                    feecent.ToString("0.00"),
                    //                    depositRow.merchantno,
                    //                    feeFloat.ToString("0.00"));

                    //                // Verify Fee
                    //                fee = feeFloat.ToString("0.00");
                    //                //fee = "0";
                    //            }

                    //        }

                    //        if (miscDict.ContainsKey("rebate") && decimal.TryParse(miscDict["rebate"], out rebatePercentage))
                    //        {
                    //            if (rebatePercentage >= 5)
                    //            {
                    //                _logger.LogError("RebateComputation - [Exceeded 5%] rebate:{0}%, merchantno:{1}",
                    //                    rebatePercentage.ToString("0.00"),
                    //                    depositRow.merchantno);
                    //            }
                    //            else if (rebatePercentage < 0)
                    //            {
                    //                _logger.LogError("RebateComputation - [Below 0] rebate:{0}%, merchantno:{1}",
                    //                    rebatePercentage.ToString("0.00"),
                    //                    depositRow.merchantno);
                    //            }
                    //            else
                    //            {
                    //                rebate = (depositRow.amount * (rebatePercentage / 100)).ToString("0.00");
                    //                _logger.LogError("RebateComputation - [Compute] rebate:{0}%, merchantno:{1}, computedrebate:{2}",
                    //                    rebatePercentage.ToString("0.00"), depositRow.merchantno, rebate);
                    //            }

                    //        }
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    _logger.LogCritical("FeeComputation, RebateComputation - [Exception] message :{0}", ex.Message);
                    //    fee = "0";
                    //    rebate = "0";
                    //}




                    //if (depositRow.status != 0)
                    //    return new ResponseModel<CrossTransactVerifyReturn>()
                    //    {
                    //        code = ShareEntity.Global.ResponseCode.code.FAIL,
                    //        message = "已经处理过了",
                    //        Data = null
                    //    };

                    //var rebate_float = decimal.Parse(rebate);
                    //var fee_float = decimal.Parse(fee);
                    //var depositAccountRow = connection.Get<ofUser>(depositRow.account);

                    //#region ofPaymentRecord
                    //var depositPaymentRecord = new ofPaymentRecord()
                    //{
                    //    currency = "CNY",
                    //    account = depositRow.account,
                    //    createtime = Common.CurrentTime,
                    //    referenceguid = depositRow.referenceguid,
                    //    transactiontype = 1,
                    //    status = 1,
                    //    summaryType = 1,
                    //    preBalance = depositAccountRow.amountactive,
                    //    bonus = depositRow.amount + rebate_float,
                    //    afterBalance = depositAccountRow.amountactive + depositRow.amount + rebate_float,
                    //    summary = "存款",
                    //};
                    //connection.Insert<ofPaymentRecord>(paymentRecord);
                    //#endregion

                    //depositAccountRow.amountactive += depositRow.amount + rebate_float;
                    //depositAccountRow.depositamount += depositRow.amount;
                    //depositAccountRow.depositnumber = depositAccountRow.depositnumber + 1;

                    //depositRow.status = 1;
                    //depositRow.reviewtime = Common.CurrentTime;
                    //depositRow.orderendtime = Common.CurrentTime;
                    //depositRow.fee = fee_float;
                    //depositRow.rebate = rebate_float;

                    //depositRow.reviewperson = request.Admin;
                    //var depositResult = connection.Update(depositRow);
                    //var accountResult = connection.Update(depositAccountRow);
                    //scope.Complete();

                    //if (depositResult && accountResult)
                    //{

                    //    if (depositRow.bonuspromocode > 0)
                    //    {
                    //        try
                    //        {
                    //            var promoApiToken = Common.Security.GetToken3(depositRow.account, 0, depositRow.ipaddress,
                    //                DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"), "/api/autopromo/AppDepositPromo", Guid.NewGuid().ToString());


                    //            WebClient wcching = new WebClient();
                    //            wcching.Headers[HttpRequestHeader.ContentType] = "application/json";
                    //            wcching.Headers[HttpRequestHeader.Authorization] = promoApiToken;
                    //            wcching.Encoding = System.Text.Encoding.UTF8;
                    //            var result = wcching.UploadString(new Uri("http://awcmgt22-promoservice.azurewebsites.net/api/autopromo/AppDepositPromo"),
                    //                        Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, dynamic>() {
                    //                    { "orderNo", depositRow.merchantno },
                    //                    { "account", depositRow.account },
                    //                    { "amount", 0},
                    //                    { "ipaddress", depositRow.ipaddress},
                    //                        })
                    //                    );
                    //            _logger.LogInformation("ChingAutoPromo - [Notify] orderid:{0}, result:{1}", depositRow.merchantno, result);

                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            _logger.LogError("ChingAutoPromo - [Exception] orderid:{0}, message:{1}", depositRow.merchantno, ex.Message);
                    //        }
                    //    }

                    //    return new ResponseModel<CrossTransactVerifyReturn>()
                    //    {
                    //        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    //        message = "",
                    //        Data = null
                    //    };
                    //}
                    //else
                    //    return new ResponseModel<CrossTransactVerifyReturn>()
                    //    {
                    //        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    //        message = "",
                    //        Data = null
                    //    }; 
                    #endregion
                }
                catch (Exception ex)
                {
                    string errorStamp = Common.TimeStamp;
                    _logger.LogError("Exception:{0}, Stamp:{1}", ex.Message, errorStamp);
                    return new ResponseModel<CrossTransactVerifyReturn>()
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

