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
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class DirectPayAffiliate : IRequest<ResponseModel<DirectPayAffiliateReturn>>
    {
        public long Id { get; set; }

        public decimal fee { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class DirectPayAffiliateReturn
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class DirectPayAffiliateValidator : AbstractValidator<DirectPayAffiliate>
    {
        public DirectPayAffiliateValidator()
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
    public class DirectPayAffiliateHandler : BaseWithdraw<DirectPayAffiliate, ResponseModel<DirectPayAffiliateReturn>>
    {
        public DirectPayAffiliateHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<DirectPayAffiliateReturn> Handle(DirectPayAffiliate request)
        {
            lock (this)
            {
                if ((DateTime.Now - CachedData.QueryTimeDict["DirectPayRequest"]).TotalMilliseconds < 5000)

                {
                    return new ResponseModel<DirectPayAffiliateReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "点击台快, 请慢慢来",
                        Data = null
                    };
                }
                CachedData.QueryTimeDict["DirectPayRequest"] = DateTime.Now;

            }
            //return new ResponseModel<DirectPayAffiliateReturn>()
            //{
            //    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
            //    message = "成功了请在一会儿在点",
            //    Data = null
            //};

            ofAffWithdraw itemToProces;
            string targetPlatform = "";
            decimal feecent = 0;
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var withdrawRow = connection.Get<ofAffWithdraw>(request.Id);
                var paymentdirectpaydetails = connection.Get<ofSettings>("PAYMENT_DIRECTPAYDETAILS");
                var paymentdirectpaydetails_JOBJ = JObject.Parse(paymentdirectpaydetails.body);



                if (withdrawRow.cardbank == "上海浦东发展银行") withdrawRow.cardbank = "浦东发展银行";
                if (withdrawRow.cardbank == "广发银行") withdrawRow.cardbank = "广东发展银行";

                itemToProces = withdrawRow;
                // Should be in paywait mode 
                if (withdrawRow.status != 1)
                    return new ResponseModel<DirectPayAffiliateReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "不可进行支付的状态",
                        Data = null
                    };
                // Check if the lockperson is the same as the cancelperson
                //if (withdrawRow.checkperson.ToLower() != request.Admin.ToLower())
                //    return new ResponseModel<DirectPayAffiliateReturn>()
                //    {
                //        code = ShareEntity.Global.ResponseCode.code.FAIL,
                //        message = "本人不是锁定的使用者",
                //        Data = null
                //    };


                // Check MAX DirectPay Amount
                float maxDirectPay = float.Parse(paymentdirectpaydetails_JOBJ["aff_maxamount"].ToString());

                if (withdrawRow.amount > (decimal)maxDirectPay)
                {
                    _logger.LogError("代付 - [ExceedLimitAmount] max:{0}, requestedby:{2} requestedamount:{1}", maxDirectPay.ToString(), withdrawRow.amount, request.Admin.ToLower());
                    return new ResponseModel<DirectPayAffiliateReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "不允许大于大夫金额限制",
                        Data = null
                    };
                }


                // Check if already INSERTED in the paymentLog, So i'll just need to do a query instead
                var ofpaymentlogCount = connection.QueryFirstOrDefault<long?>("SELECT * FROM ofPaymentLog WITH(NOLOCK) WHERE Transactionid=" + withdrawRow.id.ToString() + " AND transtype=1");
                if (ofpaymentlogCount > 0)
                {
                    return new ResponseModel<DirectPayAffiliateReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "本申请已经代付完成",
                        Data = null
                    };
                }

                targetPlatform = paymentdirectpaydetails_JOBJ["aff_payplatform"].ToString();

                // Get Platform fee percentage based on the targetPlatform
                var payment_directpay = connection.Get<ofSettings>("PAYMENT_DIRECTPAY");
                var gatewaylist = JArray.Parse(payment_directpay.body);
                for (int i = 0; i < gatewaylist.Count; i++)
                    if (gatewaylist[i]["key"].ToString() == targetPlatform)
                        feecent = decimal.Parse(gatewaylist[i]["feecent"].ToString());


                //Get RemaingBalance through Platform
                string getbalance_message = "";
                float getbalance_amount = -1;

                float usdt_xrate = 6f;
                try
                {
                    var payment_crypto = connection.Get<ofSettings>("PAYMENT_CRYPTO").body;
                    usdt_xrate = float.Parse(JObject.Parse(payment_crypto)["usdt_xrate"].ToString());
                }
                catch (Exception ex) { }


                bool getbalance_result = GetRemainingBalance(targetPlatform, withdrawRow.account, withdrawRow.cardbank, out getbalance_message, out getbalance_amount, withdrawRow.amount, usdt_xrate);

                if (!getbalance_result)
                {
                    _logger.LogError("{1} - [SendPayment] message:{0}",
                        getbalance_message, targetPlatform);
                    return new ResponseModel<DirectPayAffiliateReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = string.Format("【" + targetPlatform + "】message:{0}", getbalance_message),
                        Data = null
                    };
                }


                if (withdrawRow.amount > (decimal)getbalance_amount)
                {
                    _logger.LogError("{2} - [SendPayment] not enough balance, requested:{0}, remaining:{1}",
                        withdrawRow.amount, getbalance_amount, targetPlatform);
                    return new ResponseModel<DirectPayAffiliateReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = string.Format("【" + targetPlatform + "】金额不足 剩下 - RMB{0}", getbalance_amount.ToString("0.00")),
                        Data = null
                    };
                }


                // USDT PORTION
                if (withdrawRow.cardbank == "USDT")
                    targetPlatform = "爱汇USDT";


            }

            // 2 minutes before executing another 速付07 transaction
            const long minutesToWait = 120000;
            if (targetPlatform == "速付07")
            {
                if ((DateTime.Now - CachedData.QueryTimeDict["Payment速付07"]).TotalMilliseconds < minutesToWait)
                {
                    var result = minutesToWait - (DateTime.Now - CachedData.QueryTimeDict["Payment速付07"]).TotalMilliseconds;
                    result = result / 1000;
                    return new ResponseModel<DirectPayAffiliateReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "速付07 请过" + result.ToString("0") + "秒后在点击",
                        Data = null
                    };
                }
                CachedData.QueryTimeDict["Payment速付07"] = DateTime.Now;
            }







            using (TransactionScope scope = new TransactionScope())
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var withdrawRow = connection.Get<ofAffWithdraw>(request.Id);


                itemToProces = withdrawRow;
                // Should be in paywait mode 
                if (withdrawRow.status != 1)
                    return new ResponseModel<DirectPayAffiliateReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "不可进行支付的状态",
                        Data = null
                    };

                // Check if the lockperson is the same as the cancelperson
                //if (withdrawRow.checkperson.ToLower() != request.Admin.ToLower())
                //    return new ResponseModel<DirectPayAffiliateReturn>()
                //    {
                //        code = ShareEntity.Global.ResponseCode.code.FAIL,
                //        message = "本人不是锁定的使用者",
                //        Data = null
                //    };



                // Check if already INSERTED in the paymentLog, So i'll just need to do a query instead
                var ofpaymentlogCount = connection.QueryFirstOrDefault<long?>("SELECT * FROM ofPaymentLog WITH(NOLOCK) WHERE Transactionid=" + withdrawRow.id.ToString() + " AND transtype=1");

                if (ofpaymentlogCount > 0)
                {
                    return new ResponseModel<DirectPayAffiliateReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "本申请已经代付完成",
                        Data = null
                    };
                }


                // Verify BANKS
                if (withdrawRow.cardbank == "上海浦东发展银行") withdrawRow.cardbank = "浦东发展银行";
                if (withdrawRow.cardbank == "广发银行") withdrawRow.cardbank = "广东发展银行";


                // UPDATE ofWithdrawal
                withdrawRow.fee = feecent > 0 ? (withdrawRow.amount * (feecent / 100)) : 0;
                withdrawRow.payperson = request.Admin;
                withdrawRow.paytime = Common.CurrentTime;
                withdrawRow.status = 2;

                // UPATE ofUser
                //var accountRow = connection.Get<ofaff>(withdrawRow.account);
                //accountRow.amountsuspend = accountRow.amountsuspend - withdrawRow.amount;
                //accountRow.withdrawalamount += withdrawRow.amount;
                //accountRow.withdrawalnumber += 1;


                #region ofPaymentRecord
                //var paymentRecord = new ofPaymentRecord()
                //{
                //    currency = "CNY",
                //    account = withdrawRow.account,
                //    createtime = Common.CurrentTime,
                //    referenceguid = withdrawRow.referenceguid,
                //    status = 1,
                //    summaryType = 0,
                //    transactiontype = 0,
                //    preBalance = accountRow.amountactive,
                //    bonus = withdrawRow.amount,
                //    afterBalance = accountRow.amountactive + withdrawRow.amount,
                //    summary = "提款",
                //};
                #endregion


                var paymentLog = new ofPaymentLog()
                {
                    TransactionId = withdrawRow.id.ToString(),
                    Account = withdrawRow.account,
                    StatusCode = "0",
                    Realname = withdrawRow.realname,
                    Cardno = withdrawRow.cardno,
                    Amount = withdrawRow.amount,
                    Createtime = Common.CurrentTime,
                    Bank = withdrawRow.cardbank,
                    platform = targetPlatform,
                    payadmin = request.Admin,
                    transtype = 1
                };

                //connection.Insert<ofPaymentRecord>(paymentRecord);
                //var isAccountUpdateSuccess = connection.Update(accountRow);
                var isWithdrawalSuccess = connection.Update(withdrawRow);
                var isPaymentLog = connection.Insert<ofPaymentLog>(paymentLog) > 0;


                if (isWithdrawalSuccess && isPaymentLog)
                {
                    scope.Complete();
                }
                else
                    return new ResponseModel<DirectPayAffiliateReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "资料更新失败",
                        Data = null
                    };

            }


            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                bool isSuccess = false;

                var payment_crypto = connection.Get<ofSettings>("PAYMENT_CRYPTO").body;
                float usdt_xrate = 0;
                try
                {
                    usdt_xrate = float.Parse(JObject.Parse(payment_crypto)["usdt_xrate"].ToString());
                }
                catch (Exception ex) { 
                }

                _logger.LogInformation("代付 - [{0}] account:{1}, amount:{2} rowId:{3}",
                                    targetPlatform, itemToProces.account, itemToProces.amount, itemToProces.id);

                switch (targetPlatform)
                {
                    case "OnePay": isSuccess = PaymentOnePay(itemToProces); break;
                    case "很美": isSuccess = Payment很美(itemToProces); break;
                    case "众宝私": isSuccess = Payment众宝私(itemToProces); break;
                    case "众宝": isSuccess = Payment众宝(itemToProces); break;
                    case "爱汇": isSuccess = Payment爱汇(itemToProces); break;
                    case "爱汇02": isSuccess = Payment爱汇02(itemToProces); break;
                    case "爱汇03": isSuccess = Payment爱汇03(itemToProces); break;
                    case "爱汇USDT": isSuccess = Payment爱汇USDT(itemToProces, usdt_xrate); break;
                    case "汇皇": isSuccess = Payment汇皇(itemToProces); break;
                    case "信汇": isSuccess = Payment信汇(itemToProces); break;
                    case "纯代付": isSuccess = Payment纯代付(itemToProces); break;
                    case "纯代付02": isSuccess = Payment纯代付02(itemToProces); break;
                    case "纯代付03": isSuccess = Payment纯代付03(itemToProces); break;
                    case "纯代付04": isSuccess = Payment纯代付04(itemToProces); break;
                    case "小额代付": isSuccess = Payment小额代付(itemToProces); break;
                    case "聚合": isSuccess = Payment聚合(itemToProces); break;
                    case "聚合02": isSuccess = Payment聚合02(itemToProces); break;
                    case "XP": isSuccess = PaymentXP(itemToProces); break;
                    case "我来付": isSuccess = Payment我来付(itemToProces); break;
                    case "速付06": isSuccess = Payment速付06(itemToProces); break;
                    case "速付07": isSuccess = Payment速付07(itemToProces); break;
                    case "速付08": isSuccess = Payment速付08(itemToProces); break;
                    case "壹元": isSuccess = Payment壹元(itemToProces); break;
                    case "捷优代付": isSuccess = Payment捷优代付(itemToProces); break;
                    case "壹元公": isSuccess = Payment壹元公(itemToProces); break;
                    case "信付": isSuccess = Payment信付(itemToProces); break;
                    case "信付代付": isSuccess = Payment信付代付(itemToProces); break;
                    case "乐付": isSuccess = Payment乐付(itemToProces); break;
                    case "壹快付": isSuccess = Payment壹快付(itemToProces); break;
                    case "诚信通": isSuccess = Payment诚信通(itemToProces); break;
                    case "OneTech": isSuccess = PaymentOneTech(itemToProces); break;
                    case "GPay": isSuccess = PaymentGPay(itemToProces); break;
                    case "依云付": isSuccess = Payment依云付(itemToProces); break;
                    case "优信付": isSuccess = Payment优信付(itemToProces); break;
                    case "天空": isSuccess = Payment天空(itemToProces); break;
                    case "万富": isSuccess = Payment万富(itemToProces); break;
                    case "JBP": isSuccess = PaymentJBP(itemToProces); break;
                    case "聚宝盆": isSuccess = Payment聚宝盆(itemToProces); break;
                    case "普宝盆": isSuccess = Payment普宝盆(itemToProces); break;
                    case "现在代付": isSuccess = PaymentMMP(itemToProces); break;
                    case "MMPB": isSuccess = PaymentMMPB(itemToProces); break;
                    case "币付": isSuccess = Payment币付(itemToProces); break;
                    case "机构代付": isSuccess = Payment机构代付(itemToProces); break;
                    case "云代付": isSuccess = Payment云代付(itemToProces); break;
                    default: isSuccess = false; break;
                }


                return new ResponseModel<DirectPayAffiliateReturn>()
                {
                    code = isSuccess ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "",
                    Data = null
                };
            }

        }



        private bool GetRemainingBalance(string targetPlatform, string account, string cardbank, out string message, out float remainingBalance, decimal amountrequested, float usdt_xrate)
        {

            remainingBalance = -1;
            message = "";
            switch (targetPlatform)
            {
                case "乐付":
                    var 乐付 = new Thirdpay.乐付.SPayment("1596519637204", "271b480ea1a14ca7a19d89b0cfc86e38", "http://pay.allpayzs.com/Payment/Callback/10234/v",
                        "https://api.lftpay.cc/zz/queryMerchantBalance.jhtml");

                    // VerifyBank
                    Thirdpay.Common.BANKS 乐付destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 乐付destBank))
                    {
                        if (!乐付.bankCode.ContainsKey(乐付destBank))
                        {
                            _logger.LogCritical("乐付 - [SendPayment] bank type doesn't exist, Bank:{0}", 乐付destBank.ToString());
                            message = string.Format("【乐付】Bank Doesn't Exist, Bank:{0}", 乐付destBank.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogCritical("乐付 - [SendPayment] bank type doesn't exist, Bank:{0}", 乐付destBank.ToString());
                        message = string.Format("【乐付】Bank Doesn't Exist, Bank:{0}", 乐付destBank.ToString());
                        return false;

                    }

                    remainingBalance = 乐付.remainingBalance;
                    message = 乐付.paymentResponse;


                    break;



                case "信付":
                    var 信付 = new Thirdpay.信付.SPayment("1013796859",
                        "abhsunngflzgoikxho",
                        "http://pay.allpayzs.com/Payment/Callback/10234/v2",
                        "https://newapi.xfuoo.com/balanceQuery")
                    {
                        account = account
                    };
                    // VerifyBank
                    Thirdpay.Common.BANKS 信付destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 信付destBank))
                    {
                        if (!信付.bankCode.ContainsKey(信付destBank))
                        {
                            _logger.LogCritical("信付 - [SendPayment] bank type doesn't exist, Bank:{0}", 信付destBank.ToString());
                            message = string.Format("【信付】Bank Doesn't Exist, Bank:{0}", 信付destBank.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogCritical("信付 - [SendPayment] bank type doesn't exist, Bank:{0}", 信付destBank.ToString());
                        message = string.Format("【信付】Bank Doesn't Exist, Bank:{0}", 信付destBank.ToString());
                        return false;

                    }

                    remainingBalance = 信付.remainingBalance;
                    message = 信付.paymentResponse;


                    break;


                case "OnePay":
                    var onepay = new Thirdpay.OnePay.S("8KvijMjMQA",
                        "8coRe7rG6ffNwX5U533M",
                        "http://pay.allpayzs.com/Payment/Callback/10234/v2",
                        "https://api.onepayer.cn/getMoney.php")
                    {
                        account = account
                    };
                    // VerifyBank
                    Thirdpay.Common.BANKS OnePaydestBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out OnePaydestBank))
                    {
                        if (!onepay.bankCode.ContainsKey(OnePaydestBank))
                        {
                            _logger.LogCritical("OnePay - [SendPayment] bank type doesn't exist, Bank:{0}", OnePaydestBank.ToString());
                            message = string.Format("【OnePay】Bank Doesn't Exist, Bank:{0}", OnePaydestBank.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogCritical("OnePay - [SendPayment] bank type doesn't exist, Bank:{0}", OnePaydestBank.ToString());
                        message = string.Format("【OnePay】Bank Doesn't Exist, Bank:{0}", OnePaydestBank.ToString());
                        return false;

                    }

                    remainingBalance = onepay.remainingBalance;
                    message = onepay.paymentResponse;


                    break;


                case "速付06":
                    var 速付06 = new Thirdpay.速付.SPayment("100088010006", "dGHAvvxhULVHWemB0KcX0bv3_8J#SFzZvm&1QPIHV1m1SBqi&tQ6DBI=I8cmyr7@|&CLNFHYgFWmhlkIx=&x5f_ZH*hG", "http://www.google.com", "http://api.sssdyg.com/webapi/bank/findCreditCardByOfficeCodeSign")
                    {
                        account = account,
                        returnUrl = "http://www.google.com"
                    };
                    remainingBalance = 速付06.remainingBalance;
                    break;
                case "速付08":
                    var 速付08 = new Thirdpay.速付.SPayment("100088010008", "zeuJuYnj250iR6*Wx0EdjtFFjUvlKvro9A_Wj+qPNVM$8Nq5QJnmp^S3ylv0kK3E|j%ycTQ3dGSeUw1o_2!=9Ox!CqCP", "http://www.google.com", "http://api.sssdyg.com/webapi/bank/findCreditCardByOfficeCodeSign")
                    {
                        account = account,
                        returnUrl = "http://www.google.com"
                    };
                    remainingBalance = 速付08.remainingBalance;
                    break;
                case "速付07":
                    var 速付07 = new Thirdpay.速付.SPayment("100088010007", "awmE@lA$T*W5Isz$N$c%I05p8@ALC^u1smNu1EBva!Axpk_+dwovkJk8T4+sI74u|f3WJ&VMLOKB%5FfssEU_J!!7n@E", "http://www.google.com", "http://api.sssdyg.com/webapi/bank/findCreditCardByOfficeCodeSign")
                    {
                        account = account,
                        returnUrl = "http://www.google.com"
                    };
                    remainingBalance = 速付07.remainingBalance;
                    break;
                case "聚宝盆":
                case "普宝盆":
                    Thirdpay.Common.BANKS 聚宝盆destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 聚宝盆destBank))
                    {
                        if (聚宝盆destBank == Thirdpay.Common.BANKS.邮政储蓄 ||
                            聚宝盆destBank == Thirdpay.Common.BANKS.邮政银行 ||
                            聚宝盆destBank == Thirdpay.Common.BANKS.广东发展银行 ||
                            聚宝盆destBank == Thirdpay.Common.BANKS.浦东发展银行)
                        {
                            _logger.LogCritical("聚宝盆 - [SendPayment] bank type doesn't exist, Bank:{0}", 聚宝盆destBank.ToString());
                            message = string.Format("【聚宝盆】Bank Doesn't Exist, Bank:{0}", 聚宝盆destBank.ToString());
                            return false;
                        }
                    }

                    _logger.LogInformation("聚宝盆 - [SendPayment] cardbank:{0}, 聚宝盆destBank:{1}", cardbank, 聚宝盆destBank.ToString());
                    remainingBalance = 100000;
                    message = "No Balance Query Available";
                    break;

                case "依云付":
                    Thirdpay.Common.BANKS 依云付destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 依云付destBank))
                    {
                        if (依云付destBank == Thirdpay.Common.BANKS.兴业银行)
                        {
                            _logger.LogCritical("依云付 - [SendPayment] bank type doesn't exist, Bank:{0}", 依云付destBank.ToString());
                            message = string.Format("【依云付】Bank Doesn't Exist, Bank:{0}", 依云付destBank.ToString());
                            return false;
                        }
                    }

                    _logger.LogInformation("依云付 - [SendPayment] cardbank:{0}, 依云付destBank:{1}", cardbank, 依云付destBank.ToString());
                    remainingBalance = 100000;
                    break;

                case "优信付":
                case "JBP":
                case "诚信通":
                case "GPay":
                case "万富":
                case "OneTech":
                    remainingBalance = 100000;
                    //var avoidamount = new decimal[] {
                    //111,222,333,444,555,666,99, 199, 299, 399, 499, 599, 699, 799, 899, 999, 1000, 1999, 2999, 3999, 4999, 5999, 6999, 7999, 8999, 9999, 19999, 29999, 39999, 49999, 101, 102, 103, 104, 105, 106, 107, 108, 109, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 5000, 10000, 20000, 30000, 40000, 50000, 777, 888, 666, 10001, 10002, 10003, 10004, 10005, 10006, 10007, 10008, 10009, 11100, 12200, 13300, 14400, 15500, 16600, 17700, 18800, 19900, 9900, 29900, 39900, 49900, 10100, 10200, 10300, 10400, 10500, 10600, 10700, 10800, 10900};
                    //if (avoidamount.Contains(amountrequested))
                    //{
                    //    message = string.Format("【壹快付】不可提交铭感金额 {0}", amountrequested);
                    //    return false;
                    //}
                    //else
                    //{
                    //    remainingBalance = 100000;
                    //}
                    break;
                case "云代付":
                    var 云代付 = new Thirdpay.云付.S("5004", "AG7fphGu9mQYP2rYTssThIWT5QV5uFTEUrMXYcU2bLgn8ol5VY8UmS9g2CtS56zA", "http://www.google.com",
                        "https://shapi.txpay88.com/v1/dfapi/query_balance")
                    {
                        account = account,
                    };
                    remainingBalance = 云代付.remainingBalance;
                    break;
                case "现在代付":
                    var mmpThirdpay = new Thirdpay.MMP.S("105111300001", "b47d04793cdbd721139c262040ffa3ff", "", "https://api.mmpay.live/gateway/balance");
                    remainingBalance = mmpThirdpay.remainingBalance;
                    message = mmpThirdpay.paymentResponse;
                    break;

                case "MMPB":
                    //Thirdpay.Common.BANKS mmpbdestBank;
                    //if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out mmpbdestBank))
                    //{
                    //    if (mmpbdestBank == Thirdpay.Common.BANKS.邮政储蓄 ||
                    //        mmpbdestBank == Thirdpay.Common.BANKS.邮政银行)
                    //    {
                    //        _logger.LogCritical("MMPB - [SendPayment] bank type doesn't exist, Bank:{0}", mmpbdestBank.ToString());
                    //        message = string.Format("【MMPB】Bank Doesn't Exist, Bank:{0}", mmpbdestBank.ToString());
                    //        return false;
                    //    }
                    //}



                    var mmpbThirdpay = new Thirdpay.MMP.S("105111793002", "d1447fa68b23ce76f04dd8a9234919e9", "", "https://api.mmpay.live/gateway/balance");
                    remainingBalance = mmpbThirdpay.remainingBalance;
                    message = mmpbThirdpay.paymentResponse;
                    break;

                case "币付":
                    var 币付Thirdpay = new Thirdpay.币付.S("AWS001", "1e038815aaf3bbbbc56d1173213cfa72", "", "https://api.dayun2trade.com/PaymentGetway/SinglePayBalanceQuery");
                    //var 币付Thirdpay = new Thirdpay.币付.S("AWS001", "1e038815aaf3bbbbc56d1173213cfa72", "", "http://api.bifutrade.com/PaymentGetway/SinglePayBalanceQuery");
                    //var 币付Thirdpay = new Thirdpay.币付.S("AWS001", "2557c6b21cefd71bd8225420ebb9d124", "", "http://api.bifutrade.com/PaymentGetway/SinglePayBalanceQuery");
                    remainingBalance = 币付Thirdpay.remainingBalance;
                    message = 币付Thirdpay.paymentResponse;
                    break;

                case "信付代付":
                    var 信付Thirdpay = new Thirdpay.币付.S("AS001", "0e7b496f4e7e4886664d96814104f624", "", "http://api.youxintrade.com/PaymentGetway/SinglePayBalanceQuery");
                    remainingBalance = 信付Thirdpay.remainingBalance;
                    message = 信付Thirdpay.paymentResponse;
                    break;

                case "壹快付":
                    Thirdpay.Common.BANKS 壹快付destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 壹快付destBank))
                    {
                        if (壹快付destBank == Thirdpay.Common.BANKS.浦东发展银行 ||
                            壹快付destBank == Thirdpay.Common.BANKS.广东发展银行 ||
                            壹快付destBank == Thirdpay.Common.BANKS.邮政储蓄 ||
                            壹快付destBank == Thirdpay.Common.BANKS.邮政银行)
                        {
                            _logger.LogCritical("壹快付 - [SendPayment] bank type doesn't exist, Bank:{0}", 壹快付destBank.ToString());
                            message = string.Format("【壹快付】Bank Doesn't Exist, Bank:{0}", 壹快付destBank.ToString());
                            return false;
                        }
                    }
                    remainingBalance = 100000;
                    message = "No Balance Query Available";
                    break;
                case "捷优代付":
                case "壹元":
                case "壹元公":
                case "天空":
                    //string _merchantID = "MEMM000071JV";
                    //string _merhcnatKey = "";
                    //string postUrl = "https://trx.3rdpay.net/trx/rest/transfer/queryBalance";
                    //string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10234/v2";
                    //var XPpay = new Thirdpay.XPay.XPPayment("MEMM000071JV", "JFniRvgmDTHSuH/87SWZcA==|1iHqtnG2hFtUOr9K", callbackUrl, postUrl);
                    remainingBalance = 100000;
                    message = "No Balance Query Available";
                    break;

                case "XP":
                    string _merchantID = "MEMM000071JV";
                    string _merhcnatKey = "";
                    string postUrl = "https://trx.3rdpay.net/trx/rest/transfer/queryBalance";
                    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10234/v2";
                    var XPpay = new Thirdpay.XPay.XPPayment("MEMM000071JV", "JFniRvgmDTHSuH/87SWZcA==|1iHqtnG2hFtUOr9K", callbackUrl, postUrl);
                    remainingBalance = XPpay.remainingBalance;
                    message = XPpay.paymentResponse;
                    break;

                case "聚合":
                    #region Details
                    var 聚合pay = new Thirdpay.聚合.SPayment(
                        "360800000008200289",
                        "TxzdmETw|$x$#w#wMfikCR4wnKex2#YwN",
                        "http://pay.allpayzs.com/Payment/Callback/10234/v2",
                        "https://pay.fastpay365.com/gateway/index.json");
                    聚合pay.account = account;
                    remainingBalance = 聚合pay.remainingBalance;
                    message = 聚合pay.paymentResponse;
                    break;
                #endregion
                case "聚合02":
                    #region Details
                    var 聚合2pay = new Thirdpay.聚合.SPayment(
                        "360800000008200290",
                        "MJtccaW2|87Cd%yGRbRwzz@4*NfR$#AkE",
                        "http://pay.allpayzs.com/Payment/Callback/10234/v2",
                        "https://pay.fastpay365.com/gateway/index.json");
                    聚合2pay.account = account;
                    remainingBalance = 聚合2pay.remainingBalance;
                    message = 聚合2pay.paymentResponse;
                    break;
                #endregion
                case "爱汇":
                    #region Details
                    var 爱汇 = new Thirdpay.爱汇.AhPayment("13711", "64e1f042-61db-40a8-b2ff-95d3a60d2d89", "", "https://api.ichatpay168.com/api/getbalance");

                    // VerifyBank
                    Thirdpay.Common.BANKS 爱汇destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 爱汇destBank))
                    {
                        if (!爱汇.bankCode.ContainsKey(爱汇destBank))
                        {
                            _logger.LogCritical("爱汇 - [SendPayment] bank type doesn't exist, Bank:{0}", 爱汇destBank.ToString());
                            message = string.Format("【爱汇】Bank Doesn't Exist, Bank:{0}", 爱汇destBank.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogCritical("爱汇 - [SendPayment] bank type doesn't exist, Bank:{0}", 爱汇destBank.ToString());
                        message = string.Format("【爱汇】Bank Doesn't Exist, Bank:{0}", 爱汇destBank.ToString());
                        return false;
                    }

                    remainingBalance = 爱汇.remainingBalance;
                    message = 爱汇.paymentResponse;
                    #endregion
                    break;

                case "爱汇02":
                    #region Details
                    var 爱汇02 = new Thirdpay.爱汇.AhPayment("26591", "120dcb63-cdff-4195-8e06-d1bde7e8c7a4", "", "https://api.ichatpay168.com/api/getbalance");
                    // VerifyBank
                    Thirdpay.Common.BANKS 爱汇02destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 爱汇02destBank))
                    {
                        if (!爱汇02.bankCode.ContainsKey(爱汇02destBank))
                        {
                            _logger.LogCritical("爱汇02 - [SendPayment] bank type doesn't exist, Bank:{0}", 爱汇02destBank.ToString());
                            message = string.Format("【爱汇02】Bank Doesn't Exist, Bank:{0}", 爱汇02destBank.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogCritical("爱汇02 - [SendPayment] bank type doesn't exist, Bank:{0}", 爱汇02destBank.ToString());
                        message = string.Format("【爱汇02】Bank Doesn't Exist, Bank:{0}", 爱汇02destBank.ToString());
                        return false;
                    }

                    remainingBalance = 爱汇02.remainingBalance;
                    message = 爱汇02.paymentResponse;
                    #endregion
                    break;


                case "爱汇USDT":
                    #region Details

                    var 爱汇USDT = new Thirdpay.爱汇.AhPayment("67435", "99a9e535-f6b4-4b40-ad95-d1b034781f53", "", "https://api.ichatpay168.com/api/getbalance", Thirdpay.Common.BANKS.USDTERC20);
                    remainingBalance = 爱汇USDT.remainingBalance * usdt_xrate;
                    message = 爱汇USDT.paymentResponse;
                    #endregion
                    //remainingBalance = 200;
                    break;

                case "汇皇":
                    //remainingBalance = 20000;
                    //break;
                    #region Details
                    var 汇皇 = new Thirdpay.爱汇.AhPayment("13286", "f11bb16e-f879-426e-b421-fb13fd39e032", "", "https://api.ichatpay168.com/api/getbalance");
                    // VerifyBank
                    Thirdpay.Common.BANKS 汇皇destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 汇皇destBank))
                    {
                        if (!汇皇.bankCode.ContainsKey(汇皇destBank))
                        {
                            _logger.LogCritical("汇皇 - [SendPayment] bank type doesn't exist, Bank:{0}", 汇皇destBank.ToString());
                            message = string.Format("【汇皇】Bank Doesn't Exist, Bank:{0}", 汇皇destBank.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogCritical("汇皇 - [SendPayment] bank type doesn't exist, Bank:{0}", 汇皇destBank.ToString());
                        message = string.Format("【汇皇】Bank Doesn't Exist, Bank:{0}", 汇皇destBank.ToString());
                        return false;
                    }

                    remainingBalance = 汇皇.remainingBalance;
                    message = 汇皇.paymentResponse;
                    #endregion
                    break;

                case "爱汇03":
                    #region Details
                    var 爱汇03 = new Thirdpay.爱汇.AhPayment("90869", "6cf9cb99-6d6c-4a2a-80f9-bc3128147e2b", "", "https://api.ichatpay168.com/api/getbalance");
                    // VerifyBank
                    Thirdpay.Common.BANKS 爱汇03destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 爱汇03destBank))
                    {
                        if (!爱汇03.bankCode.ContainsKey(爱汇03destBank))
                        {
                            _logger.LogCritical("爱汇03 - [SendPayment] bank type doesn't exist, Bank:{0}", 爱汇03destBank.ToString());
                            message = string.Format("【爱汇03】Bank Doesn't Exist, Bank:{0}", 爱汇03destBank.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogCritical("爱汇03 - [SendPayment] bank type doesn't exist, Bank:{0}", 爱汇03destBank.ToString());
                        message = string.Format("【爱汇03】Bank Doesn't Exist, Bank:{0}", 爱汇03destBank.ToString());
                        return false;
                    }

                    remainingBalance = 爱汇03.remainingBalance;
                    message = 爱汇03.paymentResponse;
                    #endregion
                    break;

                case "信汇":
                    #region Details
                    var 信汇stf = new Thirdpay.纯代付.CDFPayment("882033353211706", "ADD6AEB7C9CFEF9AFE10AACABF40BA5C", "http://pay.allpayzs.com/Payment/Callback/10179/v2", "http://211.149.161.70:8080/OnlinePay/BalanceQuery");
                    信汇stf.account = account;

                    // VerifyBank
                    Thirdpay.Common.BANKS 信汇destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 信汇destBank))
                    {
                        if (!信汇stf.bankCode.ContainsKey(信汇destBank))
                        {
                            _logger.LogCritical("信汇 - [SendPayment] bank type doesn't exist, Bank:{0}", 信汇destBank.ToString());
                            message = string.Format("【信汇】Bank Doesn't Exist, Bank:{0}", 信汇destBank.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogCritical("信汇 - [SendPayment] bank type doesn't exist, Bank:{0}", 信汇destBank.ToString());
                        message = string.Format("【信汇】Bank Doesn't Exist, Bank:{0}", 信汇destBank.ToString());
                        return false;
                    }


                    remainingBalance = 信汇stf.remainingBalance;
                    message = 信汇stf.paymentResponse;
                    #endregion
                    break;


                case "纯代付":
                    #region Details
                    var 纯代付stf = new Thirdpay.纯代付.CDFPayment("882570270206985", "B19CED6C9EDF2ACD32AC5D2525ACB2C5", "http://pay.allpayzs.com/Payment/Callback/10179/v2", "http://45.137.11.103:8080/OnlinePay/BalanceQuery");
                    纯代付stf.account = account;

                    // VerifyBank
                    Thirdpay.Common.BANKS destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out destBank))
                    {
                        if (!纯代付stf.bankCode.ContainsKey(destBank))
                        {
                            _logger.LogCritical("纯代付 - [SendPayment] bank type doesn't exist, Bank:{0}", destBank.ToString());
                            message = string.Format("【纯代付】Bank Doesn't Exist, Bank:{0}", destBank.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogCritical("纯代付 - [SendPayment] bank type doesn't exist, Bank:{0}", destBank.ToString());
                        message = string.Format("【纯代付】Bank Doesn't Exist, Bank:{0}", destBank.ToString());
                        return false;
                    }


                    remainingBalance = 纯代付stf.remainingBalance;
                    message = 纯代付stf.paymentResponse;
                    #endregion
                    break;
                case "纯代付02":
                    #region Details
                    var 纯代付02stf = new Thirdpay.纯代付.CDFPayment("882570270200782", "AAE9BBF60A0FF0BE2EE5AB6AB3CFAD05", "http://pay.allpayzs.com/Payment/Callback/10179/v2", "http://45.137.11.103:8080/OnlinePay/BalanceQuery");
                    纯代付02stf.account = account;

                    // VerifyBank
                    Thirdpay.Common.BANKS 纯代付02destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 纯代付02destBank))
                    {
                        if (!纯代付02stf.bankCode.ContainsKey(纯代付02destBank))
                        {
                            _logger.LogCritical("纯代付02 - [SendPayment] bank type doesn't exist, Bank:{0}", 纯代付02destBank.ToString());
                            message = string.Format("【纯代付02】Bank Doesn't Exist, Bank:{0}", 纯代付02destBank.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogCritical("纯代付02 - [SendPayment] bank type doesn't exist, Bank:{0}", 纯代付02destBank.ToString());
                        message = string.Format("【纯代付02】Bank Doesn't Exist, Bank:{0}", 纯代付02destBank.ToString());
                        return false;
                    }

                    remainingBalance = 纯代付02stf.remainingBalance;
                    message = 纯代付02stf.paymentResponse;
                    #endregion
                    break;

                case "我来付":
                    #region Details

                    var 我来付stf = new Thirdpay.我来付.SPayment("WLFpay0426", "3f73698a3ff7dc6462f84e31ac084d96", "http://pay.allpayzs.com/Payment/Callback/10179/v2", "https://api.wellpays.com/sha256/balance");
                    我来付stf.account = account;

                    // VerifyBank
                    Thirdpay.Common.BANKS 我来付destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 我来付destBank))
                    {
                        if (!我来付stf.bankCode.ContainsKey(我来付destBank))
                        {
                            _logger.LogCritical("我来付 - [SendPayment] bank type doesn't exist, Bank:{0}", 我来付destBank.ToString());
                            message = string.Format("【我来付】Bank Doesn't Exist, Bank:{0}", 我来付destBank.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogCritical("纯代付03 - [SendPayment] bank type doesn't exist, Bank:{0}", 我来付destBank.ToString());
                        message = string.Format("【纯代付03】Bank Doesn't Exist, Bank:{0}", 我来付destBank.ToString());
                        return false;
                    }

                    remainingBalance = 我来付stf.remainingBalance;
                    message = 我来付stf.paymentResponse;
                    #endregion
                    break;
                case "纯代付03":
                    #region Details

                    var 纯代付03stf = new Thirdpay.纯代付.CDFPayment("882570270204421", "250CBBFEE6BBE31A051DC597ACDCCDCD", "http://pay.allpayzs.com/Payment/Callback/10179/v2", "http://45.137.11.103:8080/OnlinePay/BalanceQuery");
                    纯代付03stf.account = account;

                    // VerifyBank
                    Thirdpay.Common.BANKS 纯代付03destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 纯代付03destBank))
                    {
                        if (!纯代付03stf.bankCode.ContainsKey(纯代付03destBank))
                        {
                            _logger.LogCritical("纯代付03 - [SendPayment] bank type doesn't exist, Bank:{0}", 纯代付03destBank.ToString());
                            message = string.Format("【纯代付03】Bank Doesn't Exist, Bank:{0}", 纯代付03destBank.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogCritical("纯代付03 - [SendPayment] bank type doesn't exist, Bank:{0}", 纯代付03destBank.ToString());
                        message = string.Format("【纯代付03】Bank Doesn't Exist, Bank:{0}", 纯代付03destBank.ToString());
                        return false;
                    }

                    remainingBalance = 纯代付03stf.remainingBalance;
                    message = 纯代付03stf.paymentResponse;
                    #endregion
                    break;
                case "纯代付04":
                    #region Details

                    var 纯代付04stf = new Thirdpay.纯代付.CDFPayment("882570239941898", "DEAC35FBE3873EA5D5AF0CDEDD2F4AF3", "http://pay.allpayzs.com/Payment/Callback/10179/v2", "http://45.137.11.103:8080/OnlinePay/BalanceQuery");
                    纯代付04stf.account = account;

                    // VerifyBank
                    Thirdpay.Common.BANKS 纯代付04destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 纯代付04destBank))
                    {
                        if (!纯代付04stf.bankCode.ContainsKey(纯代付04destBank))
                        {
                            _logger.LogCritical("纯代付04 - [SendPayment] bank type doesn't exist, Bank:{0}", 纯代付04destBank.ToString());
                            message = string.Format("【纯代付04】Bank Doesn't Exist, Bank:{0}", 纯代付04destBank.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogCritical("纯代付04 - [SendPayment] bank type doesn't exist, Bank:{0}", 纯代付04destBank.ToString());
                        message = string.Format("【纯代付04】Bank Doesn't Exist, Bank:{0}", 纯代付04destBank.ToString());
                        return false;
                    }
                    remainingBalance = 纯代付04stf.remainingBalance;
                    message = 纯代付04stf.paymentResponse;
                    #endregion
                    break;
                case "小额代付":
                    #region Details

                    var 小额代付stf = new Thirdpay.纯代付.CDFPayment("882033353214364", "7F8FBC4FDDCCABB38AADFDBFABAA8E25", "http://pay.allpayzs.com/Payment/Callback/10179/v2", "http://www.kunpengnb.com:8080/OnlinePay/BalanceQuery");
                    小额代付stf.account = account;

                    // VerifyBank
                    Thirdpay.Common.BANKS 小额代付destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 小额代付destBank))
                    {
                        if (小额代付destBank == Thirdpay.Common.BANKS.邮政储蓄 ||
                            小额代付destBank == Thirdpay.Common.BANKS.邮政银行)
                        {
                            _logger.LogCritical("小额代付 - [SendPayment] bank type doesn't exist, Bank:{0}", 小额代付destBank.ToString());
                            message = string.Format("【小额代付】Bank Doesn't Exist, Bank:{0}", 小额代付destBank.ToString());
                            return false;
                        }

                        if (!小额代付stf.bankCode.ContainsKey(小额代付destBank))
                        {
                            _logger.LogCritical("小额代付 - [SendPayment] bank type doesn't exist, Bank:{0}", 小额代付destBank.ToString());
                            message = string.Format("【小额代付】Bank Doesn't Exist, Bank:{0}", 小额代付destBank.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogCritical("小额代付 - [SendPayment] bank type doesn't exist, Bank:{0}", 小额代付destBank.ToString());
                        message = string.Format("【小额代付】Bank Doesn't Exist, Bank:{0}", 小额代付destBank.ToString());
                        return false;
                    }
                    remainingBalance = 小额代付stf.remainingBalance;
                    message = 小额代付stf.paymentResponse;
                    #endregion
                    break;
                case "机构代付":
                    #region Details

                    var 机构代付stf = new Thirdpay.纯代付.CDFPayment("882033353211664", "C4AAD0CA6BECB5F60ACAC4184C5EECD2", "http://pay.allpayzs.com/Payment/Callback/10179/v2", "http://www.kunpengnb.com:8080/OnlinePay/BalanceQuery");
                    机构代付stf.account = account;

                    // VerifyBank
                    Thirdpay.Common.BANKS 机构代付destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 机构代付destBank))
                    {
                        //if (机构代付destBank == Thirdpay.Common.BANKS.邮政储蓄 ||
                        //    机构代付destBank == Thirdpay.Common.BANKS.邮政银行)
                        //{
                        //    _logger.LogCritical("机构代付 - [SendPayment] bank type doesn't exist, Bank:{0}", 机构代付destBank.ToString());
                        //    message = string.Format("【机构代付】Bank Doesn't Exist, Bank:{0}", 机构代付destBank.ToString());
                        //    return false;
                        //}

                        if (!机构代付stf.bankCode.ContainsKey(机构代付destBank))
                        {
                            _logger.LogCritical("机构代付 - [SendPayment] bank type doesn't exist, Bank:{0}", 机构代付destBank.ToString());
                            message = string.Format("【机构代付】Bank Doesn't Exist, Bank:{0}", 机构代付destBank.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogCritical("机构代付 - [SendPayment] bank type doesn't exist, Bank:{0}", 机构代付destBank.ToString());
                        message = string.Format("【机构代付】Bank Doesn't Exist, Bank:{0}", 机构代付destBank.ToString());
                        return false;
                    }
                    remainingBalance = 机构代付stf.remainingBalance;
                    message = 机构代付stf.paymentResponse;
                    #endregion
                    break;


                default:
                    break;
            }

            return remainingBalance >= 0;

        }



        private bool UpdatePaymentLog(long id, string orderid, string paymentResponse, bool payResult)
        {
            int paymentlgoInsertResult = 0;

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {

                paymentlgoInsertResult = connection.Execute("" +
                    "UPDATE ofPaymentLog " +
                    "SET orderid=@ORDERID, " +
                    "Result=@PAYMENTRESPONSE, " +
                    "StatusCode=@PAYRESULT " +
                    "WHERE TransactionId=@ID", (new
                    {
                        ID = id,
                        ORDERID = orderid,
                        PAYMENTRESPONSE = paymentResponse,
                        PAYRESULT = payResult ? "1" : "0"
                    }));
            }
            return paymentlgoInsertResult == 1;
        }
        private bool Payment爱汇02(ofAffWithdraw row)
        {
            string postUrl = "https://api.ichatpay168.com/api/payout";
            string _merchantID = "26591";
            string _merchantKey = "120dcb63-cdff-4195-8e06-d1bde7e8c7a4";

            //string _merchantID = "3mbw46m5";
            //string _merchantKey = "c4343b4f3fbc9b55466868142d884401";
            //string postUrl = "https://pmapi.jhf668.com/v4/merchant/withdrawv2";

            string returnUrl = "https://www.awcwin8.com";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";
            var stf = new Thirdpay.爱汇.AhPayment(_merchantID, _merchantKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                //publicOrPrivate = 1
            };

            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [爱汇] result:{1}, response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [爱汇] rowid:{1}, exception:{0}", ex.Message, row.id);
                return false;
            }
        }
        private bool Payment汇皇(ofAffWithdraw row)
        {
            string postUrl = "https://api.ichatpay168.com/api/payout";
            string _merchantID = "13286";
            string _merchantKey = "f11bb16e-f879-426e-b421-fb13fd39e032";

            //string _merchantID = "3mbw46m5";
            //string _merchantKey = "c4343b4f3fbc9b55466868142d884401";
            //string postUrl = "https://pmapi.jhf668.com/v4/merchant/withdrawv2";

            string returnUrl = "https://www.awcwin8.com";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";
            var stf = new Thirdpay.爱汇.AhPayment(_merchantID, _merchantKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                //publicOrPrivate = 1
            };

            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [汇皇] result:{1}, response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [汇皇] rowid:{1}, exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment爱汇USDT(ofAffWithdraw row, float usdt_xrate)
        {
            // Convert to USDT Rate
            //float usdt_amount = ((float)row.amount / usdt_xrate);

            string postUrl = "https://api.ichatpay168.com/api/payout";
            string _merchantID = "67435";
            string _merchantKey = "99a9e535-f6b4-4b40-ad95-d1b034781f53";

            string returnUrl = "https://www.awcwin8.com";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";
            var stf = new Thirdpay.爱汇.AhPayment(_merchantID, _merchantKey, callbackUrl, postUrl, Thirdpay.Common.BANKS.USDTERC20)
            {
                account = row.account,
                returnUrl = returnUrl,
                usdt_erc20_xrate = usdt_xrate
            };

            Thirdpay.Common.BANKS destBank;
            string row_cardbank = row.cardbank;
            if (row_cardbank == "USDT")
                destBank = Thirdpay.Common.BANKS.USDTERC20;
            else if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [爱汇USDT] result:{1}, response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog_USDT(row.id, orderid, stf.paymentResponse, payResult, usdt_xrate);
            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [爱汇USDT] rowid:{1}, exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool UpdatePaymentLog_USDT(long id, string orderid, string paymentResponse, bool payResult, float usdt_xrate)
        {
            int paymentlgoInsertResult = 0;

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                string misc = Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    { "usdt_xrate", usdt_xrate.ToString("0.00")}
                });
                paymentlgoInsertResult = connection.Execute("" +
                    "UPDATE ofPaymentLog " +
                    "SET orderid=@ORDERID, " +
                    "Result=@PAYMENTRESPONSE, " +
                    "StatusCode=@PAYRESULT, " +
                    "misc=@MISC " +
                    "WHERE TransactionId=@ID", (new
                    {
                        ID = id,
                        ORDERID = orderid,
                        PAYMENTRESPONSE = paymentResponse,
                        MISC = misc,
                        PAYRESULT = payResult ? "1" : "0"
                    }));
            }
            return paymentlgoInsertResult == 1;
        }

        private bool Payment爱汇03(ofAffWithdraw row)
        {
            string postUrl = "https://api.ichatpay168.com/api/payout";
            string _merchantID = "90869";
            string _merchantKey = "6cf9cb99-6d6c-4a2a-80f9-bc3128147e2b";

            //string _merchantID = "3mbw46m5";
            //string _merchantKey = "c4343b4f3fbc9b55466868142d884401";
            //string postUrl = "https://pmapi.jhf668.com/v4/merchant/withdrawv2";

            string returnUrl = "https://www.awcwin8.com";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";
            var stf = new Thirdpay.爱汇.AhPayment(_merchantID, _merchantKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                //publicOrPrivate = 1
            };

            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [爱汇] result:{1}, response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [爱汇] rowid:{1}, exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment爱汇(ofAffWithdraw row)
        {
            string postUrl = "https://api.ichatpay168.com/api/payout";
            string _merchantID = "13711";
            string _merchantKey = "64e1f042-61db-40a8-b2ff-95d3a60d2d89";

            //string _merchantID = "3mbw46m5";
            //string _merchantKey = "c4343b4f3fbc9b55466868142d884401";
            //string postUrl = "https://pmapi.jhf668.com/v4/merchant/withdrawv2";

            string returnUrl = "https://www.awcwin8.com";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";
            var stf = new Thirdpay.爱汇.AhPayment(_merchantID, _merchantKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                //publicOrPrivate = 1
            };

            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [爱汇] result:{1}, response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [爱汇] rowid:{1}, exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment捷优代付(ofAffWithdraw row)
        {
            string postUrl = "http://148.66.19.50/api/order/create";
            string _merchantID = "6";
            string _merhcnatKey = "b640621e33b64e16b7bc565ccd8b6205";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";

            //string postUrl = "http://23.98.37.163:8080/PAY";
            //string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.壹元.SPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl);

            stf.account = row.account;
            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }



        private bool Payment众宝(ofAffWithdraw row)
        {
            string _merchantID = "89274";
            string _merchantKey = "927a9ef65e564f979b83ade641ab8c60";
            string postUrl = "http://gateway.zbpay.cc";

            string returnUrl = "https://www.awcwin8.com";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";
            var stf = new Thirdpay.ZB支付.ZBv3(_merchantID, _merchantKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl
            };

            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [众宝] result:{1}, response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [众宝] rowid:{1}, exception:{0}", ex.Message, row.id);
                return false;
            }
        }
        private bool Payment众宝私(ofAffWithdraw row)
        {
            string _merchantID = "89315";
            string _merchantKey = "cce6df21b3e3423582b757d1632d4bbe";
            string postUrl = "http://gateway.zbpay.cc";

            string returnUrl = "https://www.awcwin8.com";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";
            var stf = new Thirdpay.ZB支付.ZBv3(_merchantID, _merchantKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                //publicOrPrivate = 1
            };

            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [众宝私] result:{1}, response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [众宝私] rowid:{1}, exception:{0}", ex.Message, row.id);
                return false;
            }
        }
        private bool Payment纯代付(ofAffWithdraw row)
        {
            string _merchantID = "882570270206985";
            //string _merhcnatKey = "B19CED6C9EDF2ACD32AC5D2525ACB2C5";
            string _merhcnatKey = "CC51EC2FDE2FCEAD95CF7E1CAFFDCFD3";
            string postUrl = "http://45.137.11.103:8080/OnlinePay/wstPayOther";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.纯代付.CDFPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment我来付(ofAffWithdraw row)
        {
            string _merchantID = "WLFpay0426";
            string _merhcnatKey = "3f73698a3ff7dc6462f84e31ac084d96";
            string postUrl = "https://api.wellpays.com/sha256/withdraw";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10234/v2";

            var stf = new Thirdpay.我来付.SPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl);
            stf.account = row.account;
            string returnUrl = "https://www.awcwin8.com";


            //if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }




        private bool PaymentXP(ofAffWithdraw row)
        {
            string _merchantID = "MEMM000071JV";
            string _merhcnatKey = "JFniRvgmDTHSuH/87SWZcA==|1iHqtnG2hFtUOr9K";
            string postUrl = "https://trx.3rdpay.net/trx/rest/transfer/order";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10234/v2";

            var stf = new Thirdpay.XPay.XPPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl);
            stf.account = row.account;
            string returnUrl = "https://www.awcwin8.com";


            //if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }




        private bool Payment聚合(ofAffWithdraw row)
        {
            var stf = new Thirdpay.聚合.SPayment(
                "360800000008200289",
                "TxzdmETw|$x$#w#wMfikCR4wnKex2#YwN",
                "http://pay.allpayzs.com/Payment/Callback/10234/v2",
                "https://pay.fastpay365.com/gateway/index.json");
            stf.account = row.account;


            string _merchantID = "882570270200782";
            string _merhcnatKey = "AAE9BBF60A0FF0BE2EE5AB6AB3CFAD05";
            string postUrl = "http://45.137.11.103:8080/OnlinePay/wstPayOther";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
            string returnUrl = "https://www.awcwin8.com";


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment聚合02(ofAffWithdraw row)
        {
            var stf = new Thirdpay.聚合.SPayment(
                "360800000008200290",
                "MJtccaW2|87Cd%yGRbRwzz@4*NfR$#AkE",
                "http://pay.allpayzs.com/Payment/Callback/10234/v2",
                "https://pay.fastpay365.com/gateway/index.json");
            stf.account = row.account;


            //string _merchantID = "882570270200782";
            //string _merhcnatKey = "AAE9BBF60A0FF0BE2EE5AB6AB3CFAD05";
            //string postUrl = "http://45.137.11.103:8080/OnlinePay/wstPayOther";
            //string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
            string returnUrl = "https://www.awcwin8.com";


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment纯代付03(ofAffWithdraw row)
        {
            string _merchantID = "882570270204421";
            string _merhcnatKey = "DDFBC5F28AEE48BADBACEEADFA0BCA00";
            //string _merhcnatKey = "250CBBFEE6BBE31A051DC597ACDCCDCD";
            string postUrl = "http://45.137.11.103:8080/OnlinePay/wstPayOther";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.纯代付.CDFPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment机构代付(ofAffWithdraw row)
        {
            string _merchantID = "882033353211664";
            string _merhcnatKey = "C4AAD0CA6BECB5F60ACAC4184C5EECD2";
            //string postUrl = "http://45.137.11.103:8080/OnlinePay/wstPayOther";
            string postUrl = "http://www.kunpengnb.com:8080/OnlinePay/wstPayOther";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
            string returnUrl = "https://www.awcwin8.com";


            var stf = new Thirdpay.纯代付.CDFPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                Random rand = new Random();
                float additional = rand.Next(1, 5);
                additional = additional / 100;
                amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [机构代付] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment小额代付(ofAffWithdraw row)
        {
            string _merchantID = "882033353214364";
            string _merhcnatKey = "7F8FBC4FDDCCABB38AADFDBFABAA8E25";
            //string postUrl = "http://45.137.11.103:8080/OnlinePay/wstPayOther";
            string postUrl = "http://www.kunpengnb.com:8080/OnlinePay/wstPayOther";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
            string returnUrl = "https://www.awcwin8.com";


            var stf = new Thirdpay.纯代付.CDFPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                Random rand = new Random();
                float additional = rand.Next(1, 5);
                additional = additional / 100;
                amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [小额代付] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment纯代付04(ofAffWithdraw row)
        {
            string _merchantID = "882570239941898";
            string _merhcnatKey = "DEAC35FBE3873EA5D5AF0CDEDD2F4AF3";
            string postUrl = "http://45.137.11.103:8080/OnlinePay/wstPayOther";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.纯代付.CDFPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment纯代付02(ofAffWithdraw row)
        {
            string _merchantID = "882570270200782";
            string _merhcnatKey = "8576FAFA18BAFAC9EAADECEEAC5DFDCB";
            //string _merhcnatKey = "AAE9BBF60A0FF0BE2EE5AB6AB3CFAD05";
            string postUrl = "http://45.137.11.103:8080/OnlinePay/wstPayOther";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.纯代付.CDFPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }



        private bool Payment很美(ofAffWithdraw row)
        {
            string returnUrl = "https://www.awcwin8.com";
            string _merchantID = "12205493";
            string _merchantKey = "82d5905a82afdde8900c16db86f271eb";
            string postUrl = "https://do.hmpay68.com/api";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";

            var stf = new Thirdpay.很美.HMPayment(_merchantID, _merchantKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl
            };

            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [很美] result:{1}, response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [很美] rowid:{1}, exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool PaymentOnePay(ofAffWithdraw row)
        {
            string _merchantID = "8KvijMjMQA";
            string _merhcnatKey = "8coRe7rG6ffNwX5U533M";
            string postUrl = "https://api.onepayer.cn/pay.php";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";

            var stf = new Thirdpay.OnePay.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account
            };
            string result = "";


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);

                _logger.LogInformation("SendPayment - [OrderDetails] orderid:{0}, amount:{1}, destbank:{2}, cardno:{3}, realname:{4}",
                    orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [Result] Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [Exception] RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }
        private bool Payment速付07(ofAffWithdraw row)
        {
            string _merchantID = "100088010007";
            string _merhcnatKey = "awmE@lA$T*W5Isz$N$c%I05p8@ALC^u1smNu1EBva!Axpk_+dwovkJk8T4+sI74u|f3WJ&VMLOKB%5FfssEU_J!!7n@E";
            string postUrl = "http://api.sssdyg.com/webapi/order/addOutOrder";
            //string postUrl = "http://23.98.37.163:8080/PAY";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.速付.SPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }
        private bool Payment速付08(ofAffWithdraw row)
        {
            /*
            商户名称：AWCTFGroup008
            商户Code:100088010008
            商户token：j%ycTQ3dGSeUw1o_2!=9Ox!CqCP
            商户key：zeuJuYnj250iR6*Wx0EdjtFFjUvlKvro9A_Wj+qPNVM$8Nq5QJnmp^S3ylv0kK3E 
            */
            string _merchantID = "100088010008";
            string _merhcnatKey = "zeuJuYnj250iR6*Wx0EdjtFFjUvlKvro9A_Wj+qPNVM$8Nq5QJnmp^S3ylv0kK3E|j%ycTQ3dGSeUw1o_2!=9Ox!CqCP";
            string postUrl = "http://api.sssdyg.com/webapi/order/addOutOrder";
            //string postUrl = "http://23.98.37.163:8080/PAY";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.速付.SPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment诚信通(ofAffWithdraw row)
        {
            string _merchantID = "504667682960834560";
            string _merhcnatKey = "f2f2175c98c791d2ee1cc83ea24ffdb9";
            string postUrl = "http://api.gatewaydata.net/api/gateway/proxyPay";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20179/v2";

            var stf = new Thirdpay.诚信通.SPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                returnUrl = "https://www.awcwin8.com",
                account = row.account
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment速付06(ofAffWithdraw row)
        {


            string _merchantID = "100088010006";
            string _merhcnatKey = "dGHAvvxhULVHWemB0KcX0bv3_8J#SFzZvm&1QPIHV1m1SBqi&tQ6DBI=I8cmyr7@|&CLNFHYgFWmhlkIx=&x5f_ZH*hG";
            string postUrl = "http://api.sssdyg.com/webapi/order/addOutOrder";
            //string postUrl = "http://23.98.37.163:8080/PAY";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.速付.SPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool PaymentJBP(ofAffWithdraw row)
        {

            string merchantID = "8";
            string merchantKey = "20171215AWCCWA";
            string _callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10045";
            string postURL = "https://api.jbp-pay.com/apply";

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                var stf = new Thirdpay.Daddy.JBP(merchantID, merchantKey, _callbackUrl, postURL)
                {
                    username = row.account
                };

                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool PaymentMMP(ofAffWithdraw row)
        {
            string postURL = "https://api.mmpay.live/gateway/withdrawal";
            string _merchantID = "105111300001";
            string _merhcnatKey = "b47d04793cdbd721139c262040ffa3ff";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/102101/v2";

            var stf = new Thirdpay.MMP.S(_merchantID, _merhcnatKey, callbackUrl, postURL);

            stf.account = row.account;
            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool PaymentMMPB(ofAffWithdraw row)
        {
            string postURL = "https://api.mmpay.live/gateway/withdrawal";
            string _merchantID = "105111793002";
            string _merhcnatKey = "d1447fa68b23ce76f04dd8a9234919e9";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/102101/v2";

            var stf = new Thirdpay.MMP.S(_merchantID, _merhcnatKey, callbackUrl, postURL);

            stf.account = row.account;
            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment普宝盆(ofAffWithdraw row)
        {
            string postURL = "https://pmapi.wikizh.com/v4/merchant/withdraw";
            string _merchantID = "wrkigbkd";
            string _merhcnatKey = "96ca2bdd7b2e66f23861845504dd6366"; string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";

            var stf = new Thirdpay.聚宝盆.JBP(_merchantID, _merhcnatKey, callbackUrl, postURL);

            stf.account = row.account;
            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }




        private bool Payment聚宝盆(ofAffWithdraw row)
        {
            string postURL = "https://pmapi.wikizh.com/v4/merchant/withdraw";
            string _merchantID = "jlu93k5c";
            string _merhcnatKey = "9f6faa68dcaa4fe9250ae64dcad12655"; string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";

            var stf = new Thirdpay.聚宝盆.JBP(_merchantID, _merhcnatKey, callbackUrl, postURL);

            stf.account = row.account;
            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment壹元公(ofAffWithdraw row)
        {
            string _merchantID = "8";
            string _merhcnatKey = "4c0214a16ac0c080";
            string postUrl = "http://180.178.43.210:10000/api/pay/order";
            //string postUrl = "http://23.98.37.163:8080/PAY";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.壹元.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment壹元(ofAffWithdraw row)
        {
            string postUrl = "http://148.66.19.50/api/order/create";
            string _merchantID = "3";
            string _merhcnatKey = "9475db73a69eb6a13d61058d28ac1dad";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";

            //string postUrl = "http://23.98.37.163:8080/PAY";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.壹元.SPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl);

            stf.account = row.account;
            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }



        //private bool Payment壹元(ofAffWithdraw row)
        //{
        //    string _merchantID = "8";
        //    string _merhcnatKey = "4c0214a16ac0c080";
        //    string postUrl = "http://180.178.43.210:10000/api/pay/order";
        //    //string postUrl = "http://23.98.37.163:8080/PAY";
        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
        //    string returnUrl = "https://www.awcwin8.com";

        //    var stf = new Thirdpay.壹元.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
        //    {
        //        account = row.account
        //    };

        //    if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}


        private bool Payment乐付(ofAffWithdraw row)
        {

            string _merchantID = "1596519637204";
            string _merhcnatKey = "271b480ea1a14ca7a19d89b0cfc86e38";
            string postUrl = "https://apic.lftpay.cc";


            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.乐付.SPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }




        private bool Payment信付(ofAffWithdraw row)
        {

            string _merchantID = "1013796859";
            string _merhcnatKey = "abhsunngflzgoikxho";
            string postUrl = "https://newapi.xfuoo.com/agentPay";


            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.信付.SPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }
        //private bool Payment速付(ofAffWithdraw row)
        //{
        //    string _merchantID = "100088010001";
        //    string _merhcnatKey = "F$Ult0#pJFK75&K1T4Dw^495MKJcafUBGX0ZM5U&ZNtyLq^6YoO0*&0m#endF&FI|#*r6L@QvsJfZa*_s=SBzU3C7vhH";
        //    string postUrl = "http://api.sssdyg.com/webapi/order/addOutOrder";
        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
        //    string returnUrl = "https://www.awcwin8.com";

        //    var stf = new Thirdpay.速付.SPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
        //    {
        //        account = row.account
        //    };

        //    if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}
        //private bool Payment速付02(ofAffWithdraw row)
        //{
        //    string _merchantID = "100088010002";
        //    string _merhcnatKey = "lz%7*2x$DhrZ4lu29ALlYDKPNtyrCyGHvoQJaRvQft&jhnIudFq!XLNPHmmu$%Ao|O9402fTExrVM%jgYmv6^_I4!hUV";
        //    string postUrl = "http://api.sssdyg.com/webapi/order/addOutOrder";
        //    //string postUrl = "http://23.98.37.163:8080/PAY";
        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
        //    string returnUrl = "https://www.awcwin8.com";

        //    var stf = new Thirdpay.速付.SPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
        //    {
        //        account = row.account
        //    };

        //    if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}



        //private bool PaymentDaddy(ofAffWithdraw row)
        //{

        //    string merchantID = "155";
        //    string merchantKey = "20171215AWCCWA";
        //    string _callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10045";
        //    string postURL = "http://www.dppayc001.com/mownecum_api/";

        //    if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        Thirdpay.Daddy.DaddyPay stf = new Thirdpay.Daddy.DaddyPay(merchantID, merchantKey, _callbackUrl, postURL)
        //        {
        //            username = row.account
        //        };

        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("RowID:{1}, Exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}
        //private bool Payment速龙(ofAffWithdraw row, bool 加急)
        //{
        //    string merchantID = "103888002001";
        //    string merchantKey = "MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBAI/X+RxrRuuQnWDVsz8JxhkJt0Ythef8J5BycYpgPcB/v0uIVeYivadjBGt/vUERQqwz0Ss1LuC7u4gg+hkkJ/1vkUJ9en18JfNbxpbW8uLkcn4Gq7FkthK8l5xHw4ffkBH7Bon3p1cKcMU85iwuJU0h1sS3j1eS/D1wxpmZYuDbAgMBAAECgYAiuhNsJwISMPGtxSnnGCb5OH1EtgzT+u5aUybSL3X5GV5Hj/yc+NospR0WDtrT8KTWChR/h/FvVCNsM/ntQ+tbSURHY3EeE5mYutvDomY/wh4gkVRz5sPod2wnuVmszu1wZjlbbsNldxUV0P35QMGs7vPePE0mp/qaAIQxlgOPgQJBAM4/DqJCBbn5AXCZMoBtPoToAcNK0lSqk3HI2i0j9ZMDSvPNg6Uj/xoCoJq0LcZD42Dq4CJO62CqYDl1Qw2dbvsCQQCyiy2M90UrD0P57Ow/UmGbzBFrU5V00SohqwOkGS2FLQj7vSf5isAQ4OCL/+3LFM5JNXXYJlMyRF7MwMjGAC+hAkEAghna5FXbgOXnl5y787F196bWvder+EalvegiYMlHNxajqEmi2oBrgn/Xyd8Q5PmcLF3dRycbaZ3wFy62PpdqEwJAXnftngyneGUXXoHRbp+0La8H+HW9OpYBaNcHkigQQcrWWfx3zj1v6n5R0UGVHELh8zJqK/f7RTmPv16x3LH0gQJBAK2K1HYMIsQTHnUlETk18uoevrY9NdmVSl9DnvYWxvpuZbUaO6QWJcpF9scTe5TMzsu0qZBVV6CfFV5suqr0/q8=";

        //    string postURL = "https://transfer.islpay.hk/transfer";
        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10052";

        //    if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        var stf = new Thirdpay.速龙.SulongPayment(merchantID, merchantKey, postURL, 加急)
        //        {
        //            returnUrl = "https://www.awcwin8.com"
        //        };
        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}
        //private bool Payment易付(ofAffWithdraw row)
        //{
        //    string postUrl = "https://api.onepay.solutions/v2/distribute/withdraw.html";
        //    string merchantID = "3577";
        //    string merchantKey = "MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBALQ1hfhkl/b4bgsWLDwWIuXJ/fSUHytm+5UwpQKvMSSuPFnRoDWjFpMThRlYrzkeSobcnCW3Xb0M9XGIXdWOO88++nm3vc8PP2/Do8kgqaPjcrUeZk8Xy5dhru+25dhUCu4QIsUut3EKUn9ctyW+/TWvT/xa+jQgbTjpdR4WntWDAgMBAAECgYBFo8Fn4gfPyuVLp3/koHFP96UKFQuOjDeAydGanfURxbtQwm91Ctc4cvymdN0R56DJSQuWj6vL2OwouZf5ZQO7RUKQHX5DU4iHoSnlW/7At+DEqWl+r7/6nOACahGzM/Bct+1iXMW3fjmjeFhbux7LR5x+p/VD8xdkPHeG4Co3kQJBAPittyplYLoalMekTs+ghzsagf4yXxVEOkieWEofXGjxEhgccNUIsDwYxbgII5inFidSuYDxus+AyAyTcARCXLUCQQC5g8E8zK/leFuF8GlLov3m8It9YOsA/E/Ony9PTHchd529apmMpba32s/2XAoj9z5glwEFyDooEq1FUl9ptIRXAkEA1gzZG7IVdzuIEuwYNlmL7VDc0cvT0j9fF8DMaEjkRyQyGfTpHkYTRDdigQzobu5rgD+Rky7/SLsl9sU150Sj/QJAWOxU1gXvUnkstbkVX9ITDn3C/yhTvTc0MgCPtt0aRYqGq+LLE2XjfaF/CcH4gmDjH32CWfkuF0NMluvzHEIRJQJBALBvJ2Tr+b9UWVacGkOwUxOfpV51mtirO6ElBXl5YJh3AQSqsxULNWLyfg7suBG6YpWBaKoZAaeRWt94OYpkIBk=|" +
        //        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDmwwFxxkClvsBzZUtE0CN4S7P0QWZxnpxn2De0zlqbjY6Put/8738SXYkGsuBIb5QZU3tDb/0hmON3zQ84BLexksP2iNqY1q1VSeY2NkV/QxrCUefUedTFsDU+ZcIB5JJ02m4fqpYtzYowtf5JrgjYHcyrO1IaX3NVITm9EPOMHQIDAQAB";

        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/12000";
        //    string returnUrl = "https://www.awcwin8.com";



        //    if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        var stf = new Thirdpay.易付卡.YfkPayment(merchantID, merchantKey, callbackUrl, postUrl)
        //        {
        //            account = row.account,
        //            returnUrl = "https://www.awcwin8.com",
        //            walletType = "1"
        //        };
        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}
        //private bool Payment易付今(ofAffWithdraw row)
        //{
        //    string postUrl = "https://api.onepay.solutions/v2/distribute/withdraw.html";
        //    string merchantID = "3577";
        //    string merchantKey = "MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBALQ1hfhkl/b4bgsWLDwWIuXJ/fSUHytm+5UwpQKvMSSuPFnRoDWjFpMThRlYrzkeSobcnCW3Xb0M9XGIXdWOO88++nm3vc8PP2/Do8kgqaPjcrUeZk8Xy5dhru+25dhUCu4QIsUut3EKUn9ctyW+/TWvT/xa+jQgbTjpdR4WntWDAgMBAAECgYBFo8Fn4gfPyuVLp3/koHFP96UKFQuOjDeAydGanfURxbtQwm91Ctc4cvymdN0R56DJSQuWj6vL2OwouZf5ZQO7RUKQHX5DU4iHoSnlW/7At+DEqWl+r7/6nOACahGzM/Bct+1iXMW3fjmjeFhbux7LR5x+p/VD8xdkPHeG4Co3kQJBAPittyplYLoalMekTs+ghzsagf4yXxVEOkieWEofXGjxEhgccNUIsDwYxbgII5inFidSuYDxus+AyAyTcARCXLUCQQC5g8E8zK/leFuF8GlLov3m8It9YOsA/E/Ony9PTHchd529apmMpba32s/2XAoj9z5glwEFyDooEq1FUl9ptIRXAkEA1gzZG7IVdzuIEuwYNlmL7VDc0cvT0j9fF8DMaEjkRyQyGfTpHkYTRDdigQzobu5rgD+Rky7/SLsl9sU150Sj/QJAWOxU1gXvUnkstbkVX9ITDn3C/yhTvTc0MgCPtt0aRYqGq+LLE2XjfaF/CcH4gmDjH32CWfkuF0NMluvzHEIRJQJBALBvJ2Tr+b9UWVacGkOwUxOfpV51mtirO6ElBXl5YJh3AQSqsxULNWLyfg7suBG6YpWBaKoZAaeRWt94OYpkIBk=|" +
        //        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDmwwFxxkClvsBzZUtE0CN4S7P0QWZxnpxn2De0zlqbjY6Put/8738SXYkGsuBIb5QZU3tDb/0hmON3zQ84BLexksP2iNqY1q1VSeY2NkV/QxrCUefUedTFsDU+ZcIB5JJ02m4fqpYtzYowtf5JrgjYHcyrO1IaX3NVITm9EPOMHQIDAQAB";

        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/12000";
        //    string returnUrl = "https://www.awcwin8.com";



        //    if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        var stf = new Thirdpay.易付卡.YfkPayment(merchantID, merchantKey, callbackUrl, postUrl)
        //        {
        //            account = row.account,
        //            returnUrl = "https://www.awcwin8.com",
        //            walletType = "2"
        //        };

        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}
        //private bool Payment四通(ofAffWithdraw row)
        //{
        //    string merchantID = "7ef808d85dd24d3495e1964d5fa167d9";
        //    string merchantKey = "6aa5aecd3194427e9fa2732fecd94bc6";
        //    string postUrl = "http://gateway.stonpay.xyz/b2cPay/initPay";
        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10055";
        //    string returnUrl = "https://www.awcwin8.com";
        //    postUrl = "http://gateway.stonpay.xyz/accountProxyPay/initPay";



        //    if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        var stf = new Thirdpay.四通.SitongPayment(merchantID, merchantKey, postUrl)
        //        {
        //            returnUrl = "https://www.awcwin8.com"
        //        };
        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}
        //private bool Payment易富通(ofAffWithdraw row)
        //{
        //    string returnUrl = "https://www.awcwin8.com";

        //    string _merchantID = "ERSH468080";
        //    string _merchantKey = "a8abd0dc023ef9590350a51205bb4679";
        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10235/v2";
        //    string postUrl = "http://47.106.170.223:9000/pay/order/acp";
        //    var stf = new Thirdpay.易富通.YFTPayment(_merchantID, _merchantKey, callbackUrl, postUrl)
        //    {
        //        account = row.account
        //    };

        //    if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}
        //private bool Payment速达(ofAffWithdraw row)
        //{
        //    string returnUrl = "https://www.awcwin8.com";

        //    string _merchantID = "allwincity_wd";
        //    string _merchantKey = "+YS098rFlwA=|Frz79Oys5vU=|95b8668b9f82414a9de5a75bac003345";
        //    string callbackUrl = "";
        //    //string _merchantID = "W79142909";
        //    //string _merchantKey = "nm7xanPkOLA=|0SorYr+n3jU=|95b8668b9f82414a9de5a75bac003345";
        //    string postUrl = "https://wd.sdsystem.hk/webapi/customer.asmx";
        //    var stf = new Thirdpay.速达.SudaPayment(row.account, _merchantID, _merchantKey, callbackUrl, postUrl);

        //    if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}
        //private bool Payment智联(ofAffWithdraw row)
        //{
        //    string returnUrl = "https://www.awcwin8.com";
        //    string _merchantID = "998800029";
        //    string _merchantKey = "U1Y8QVB1OJMM1WX4";
        //    string postUrl = "http://pay.cocozf.com/ctp_xa/view/server/aotori/propayTrans.php";
        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";
        //    var stf = new Thirdpay.智联.WxcPayment(_merchantID, _merchantKey, callbackUrl, postUrl)
        //    {
        //        account = row.account,
        //        returnUrl = returnUrl
        //    };

        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - [智联] result:{1}, response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - [智联] rowid:{1}, exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}
        //private bool Payment通用(ofAffWithdraw row)
        //{
        //    string _merchantID = "1707051910|J-80022";
        //    string _merchantKey = "jpu67WUR6FUFXMUpj9Canc1RQqov4iCC";
        //    string postUrl = "http://weivb1.zhithuachuang.com:8080/diy/everySelect/storedProcess/outPayAmt.jsp";


        //    string returnUrl = "https://www.awcwin8.com";
        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";
        //    var stf = new Thirdpay.通用.TongYongPayment(row.account, "", "", _merchantID, _merchantKey, callbackUrl, postUrl)
        //    {
        //        returnUrl = returnUrl
        //    };

        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - [通用] result:{1}, response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - [通用] rowid:{1}, exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}

        //private bool Payment金汇富(ofAffWithdraw row)
        //{
        //    string _merchantID = "mk8ft48u";
        //    string _merchantKey = "d502739d3ffa5589c59ab7a8171c9e62";

        //    string postUrl = "https://pmapi.jhf668.com/v4/merchant/withdrawv2";

        //    string returnUrl = "https://www.awcwin8.com";
        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";
        //    var stf = new Thirdpay.金汇富.JHFPayment(_merchantID, _merchantKey, callbackUrl, postUrl)
        //    {
        //        account = row.account,
        //        returnUrl = returnUrl,
        //        //publicOrPrivate = 1
        //    };

        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - [金汇富] result:{1}, response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - [金汇富] rowid:{1}, exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}
        //private bool Payment金汇富02(ofAffWithdraw row)
        //{
        //    string _merchantID = "3mbw46m5";
        //    string _merchantKey = "c4343b4f3fbc9b55466868142d884401";

        //    string postUrl = "https://pmapi.jhf668.com/v4/merchant/withdrawv2";

        //    string returnUrl = "https://www.awcwin8.com";
        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";
        //    var stf = new Thirdpay.金汇富.JHFPayment(_merchantID, _merchantKey, callbackUrl, postUrl)
        //    {
        //        account = row.account,
        //        returnUrl = returnUrl,
        //        //publicOrPrivate = 1
        //    };

        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - [金汇富02] result:{1}, response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - [金汇富02] rowid:{1}, exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}

        private bool Payment信汇(ofAffWithdraw row)
        {
            string _merchantID = "882033353211706";
            string _merhcnatKey = "ADD6AEB7C9CFEF9AFE10AACABF40BA5C";
            string postUrl = "http://211.149.161.70:8080/OnlinePay/wstPayOther";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.纯代付.CDFPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment壹快付(ofAffWithdraw row)
        {
            //string _merchantID = "463151377597383";
            ////string _merchantKey = "8a1s6a12GFxM25pQ0mn|74#u&8#XE7";
            //string _merchantKey = "Tv86LrVb3Qx4M9gFav|74#u&8#XE7"; // 2019-03-19 Updated

            string _merchantID = "697484537844653";
            string _merchantKey = "8uJnV2FgtPx6Sv7Le|74#u&8#XE7";

            string returnUrl = "https://www.awcwin8.com";
            string postUrl = "https://livepay.ykf38.com/ezpay/";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";
            var stf = new Thirdpay.壹快付.YKFPayment(_merchantID, _merchantKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl
            };

            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                float amountadjust = (float)row.amount;
                Random rand = new Random();
                float additional = rand.Next(1, 5);
                additional = additional / 100;
                amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [壹快付] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [壹快付] rowid:{1}, exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment云代付(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
            string _merchantID = "5004";
            string _merhcnatKey = "AG7fphGu9mQYP2rYTssThIWT5QV5uFTEUrMXYcU2bLgn8ol5VY8UmS9g2CtS56zA";
            string postUrl = "https://shapi.txpay88.com/v1/dfapi/add";


            var stf = new Thirdpay.云付.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                Random rand = new Random();
                float additional = rand.Next(1, 5);
                additional = additional / 100;
                amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [云付] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment信付代付(ofAffWithdraw row)
        {
            string _merchantID = "AS001";
            string _merhcnatKey = "0e7b496f4e7e4886664d96814104f624";
            string postUrl = "http://api.youxintrade.com//PaymentGetway/SinglePay";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";

            var stf = new Thirdpay.币付.S(_merchantID, _merhcnatKey, callbackUrl, postUrl);

            stf.account = row.account;
            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment币付(ofAffWithdraw row)
        {
            string _merchantID = "AWS001";
            string _merhcnatKey = "1e038815aaf3bbbbc56d1173213cfa72";
            string postUrl = "https://api.dayun2trade.com/PaymentGetway/SinglePay";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";

            var stf = new Thirdpay.币付.S(_merchantID, _merhcnatKey, callbackUrl, postUrl);

            stf.account = row.account;
            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment天空(ofAffWithdraw row)
        {
            string _merchantID = "6593";
            string postUrl = "https://api.wonderfulsky.com.cn/service";
            string _merhcnatKey =
                "MIIJRAIBADANBgkqhkiG9w0BAQEFAASCCS4wggkqAgEAAoICAQC47a/IROe9zYQx" +
                "zhcpzuhlvd93l+7jVW+r8V2mCqeeZaQyKKUWNptmm1K4Om3O4ORiEoOZfSZyEfqY" +
                "BQBlgKiUafKK95zbKU6zL6dL7vpVd8fWoC+66tXteqir8Ws62jjSn9Lo1KuVBzE5" +
                "kKFditQFMWlk3Zl8RkdKePvO1tgtgkknbXA8mXUHw6KZXUmk6xfl8Wa7pffiP5Uz" +
                "DM4Mb4SRHIicopA5pgVOQrEDNS+6MKlEiet65i3Gubvc8NATK0p1g/KcklauLByB" +
                "iA4xX8WbjoFhC7xTc4r2hOKCwS5YMvgjIQsESRHrW1mI1Nu3/DsrIcOsukaGTPyA" +
                "KdEED42HQFMS5OCkeWAb90m8LD7NvEMEqXcJE6PgxP9MozyGYKtmUNc1h9aMTT/O" +
                "iFmzWkkGRCUzluHuAn6cOHd0iZU8XZVeOiANwpa8hfIgCPZnISRWICLbe/LTakUU" +
                "9pMbFei1/sL8FgQXaJVh4+ni8w2QLdGlIhW4pNte1pxceQLUmHw+wTt/6E0KIWp3" +
                "p+zju2BdUVOHQs8sgyxs6Ynpq80hpUnjvBYPH6UFBWHGtt7mJPReAK2yOMJGIEjA" +
                "N8ieqFhST8hstfYIZOSkDVf8o9Ek1ZT0ujhu/iSnrAIJ2vUdK2QEBPFQvVKdVSqr" +
                "8n6K9BfK0ijhll7TuPpqRMH4ZZ1oTwIDAQABAoICAB6cOVyNC07/lcIouuzQsuck" +
                "ZrkFDSXUHof3i+jQGq69ZFBpKepjX5W8YBTelw6Ha0ZQijf93z9CephvO4GRqlXm" +
                "s58NHYmpbL18RdTr2cQL76g3G69077Q/eWv8qiMzu/QhQGgJy9z/M8azigJR2IaD" +
                "h06K370SZPqOzB+QsHfX+QPtuYZOYHDIkUGRj38c6vfE3Zxx/HYW0DuDAxhL3UWn" +
                "03UZ84WXVYl+wI4nX58SLvKJYdEWCL28QLxxn/+MSubo0F/aWT67TnyiaLcDg1Ic" +
                "ZI4Lw0CM/5OOq2+5SlKPHUOzVH+BGNY8aLWdhkEOHz0keZ89ti7Ez4bnZRGnAWX5" +
                "wLBq7wJ44dZXI1af8w9iIkUcXs8wrXWku1TbVjG4FXmMPxYH0apd6pSRry8C27rK" +
                "fRdNnQTUPhaqnYSEHyDcqYwBYUP+QfugYHFTyy+YF+4VwalVOZreDOVjW2GpPBev" +
                "wKeeAidM6yWQSNXRBzoIJELMQjyk2zzsmCvrR9Uu1ZTUM71tm6TleRsMeNpTfhdS" +
                "G1bMJ73OijwjxJ3sDv6z/0p2EHfQK8wV1ePthgjhCOtNQM75BJGOclaN2V0yrtQq" +
                "o6d7cws8exAkjioDIZey4ckeLfbyIW7rsLgPRoukizr/VtLgYoxUL5px3C8dc/xo" +
                "qFoBDVCNeZ6/NdFZdRABAoIBAQDkvO2HT/f/V5kJlmps3WCWWgRbb7NybknjiT7q" +
                "QzmzrRkX3G1eMZphuQDXDZkgZmlpXnrqcJR7KhuV5XuLDsrUYbkncaTCWaVjU99G" +
                "HlZ6RKYAUxq9a5uYnvsaIq1j74+L7xKGylqaOkOUWs4m2o3JRLqyB6ZJM+Zht5CX" +
                "c+QtSTNdogk/bThy3YsfV17UKR382zbhLKAk5fJUBZrfrphDdQelf7VkaS5RmQ91" +
                "pFhsSTDXyE9+6ECwuMLxws2aVeuXPkKlq9CEPX9aw0l1vShCKflHjqdN/2hQpzaX" +
                "W16QHOYUhIF3XvRN9uCVLwvAId2+yt5x2kNjzj68wCfHIV+PAoIBAQDO+BO49Anu" +
                "Of4vjHs0M8E54gikx6DHzTpSVbuhTZHQUPQSo+JuLrXk/a7RrkHS7f7hAlY/+ZlP" +
                "YRUyVHKJY5ajYwP3LqkzctnpUZBUrC+JPrFPxTO+dXg0ZFIdXW5XaCMl+2Jqv0al" +
                "JE+BD1CZrFRXwMOI20QIBdLkKPAMIvCjlSEO0a9D2JX/ZmX7p3uMxnkYXy76NlM0" +
                "jTZJ+5VjmiEZYhyHOeSqiPQddcuQSSDPqJ6qf4celQ1LOQcWEmTIQCm09Pg+e+a1" +
                "dcKJFj56E1p86rL0AaKu0KT+enQdHsXlf8PTTu2v7JBTkdgq75F6Hhgy+MH6DlfR" +
                "iMQ+RWOD5wtBAoIBAQCeElnryr3NfdHKqGH/vbUD8k1Uiwrtp330emI6Lh7wJk1T" +
                "CgeispDHu7vy4fjSRTPPl3VGEmrp+p15p08LODu/LZFwyr5beOwsikegA1Mrx8kY" +
                "LFYsVPMAM82/r/s3NtRyldKtx6V5M46ENYSFoT+euZpzcYneyLkLCnonz/T9vrp6" +
                "mIHfjTEXYDbGf/COmaOZnDx+vAInvakLm5v9vICJDZUtL+ZA79cMmMPwPpTEsIfK" +
                "dylIwGQEPIqr3D2NzJa0vFf+eY/t8znRlWdssD3kC3Gzm8fOxRRnuEanroLZAL+X" +
                "qkJ2Je6a+mMVdZ+fNUgyk6vJ7lXn4R3F/tz+ZffNAoIBAQCzJ/cakNTPzI+FZVt4" +
                "0pwemNTWnp9ElRg5EVF7zYO9CXZddFObAQAfGqFrjCYaij2pVLaIZm0ZU3ZfvM3w" +
                "s64//IQAQfnhOTf8SchgIzR63zPpU1D5OwI0h+q01J2hN9nFFRgaQzzJ0uanVzRz" +
                "8cKIu6oHG5aGQ1RMgEtalhwr3flhtIy8dA1TqbX54v6F24ATYsFtgf0AboRiAk9M" +
                "iqzAJjd/rtOL80UtAzyUx1iJqPK/r+isxmVdih3iM0XeOfxgUfVZUTNiPG+K7yBh" +
                "18LqgtcpdQj3vlA0oJdGZc70XGaM0bPaJzK/xG2UrRAG32ofJUn0RNor3PHS3YnJ" +
                "MgSBAoIBAQCc3AGKCc+1M2KtvsRVqecCJTy7xAArdB0TaxvxnWF4TXlxfrECvQMG" +
                "W1tfCIK2a5tdGlP47Wvj1w55t3tp2sHyUlD2OGkRyYIdeeAg8BDXWVJ0pmf/Lzih" +
                "dM1YAYijSvLdeyuxmHLp3Zvjq5QxqxZrSpMTSfbnP0xm5D4gMY2TZ8iA3Xj8oGeI" +
                "N2RaKp+nByBlGpdx1lHaDRrGShpJ+IvqPMAy84QiVffbmTq5oMz5hAKjOCHjBHpS" +
                "nD4BuZwBjRkzzoX+K3u+7/mS1efuWWkZd9P9tsVP4JqvxaT2hrZp0y90PWTPWgcM" +
                "9FpKNHyPXw6VmWqvRkqnZXKD7C8eHwEL|07192555F99A562538D6063E831D022F";



            //string postUrl = "https://api.youxintrade.com//PaymentGetway/SinglePay";
            //string _merchantID = "AWC001";
            //string _merhcnatKey = "1beaff1e5404f5b73dcbfebcb1703c60";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10234/v2";

            string returnUrl = "https://www.awcwin8.com";
            var stf = new Thirdpay.天空.SPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "168.95.1.1"
            };

            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [天空] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
            }
            catch (Exception ex)
            {
                string response = stf.paymentResponse ?? "noresponse";
                _logger.LogError("SendPayment - [天空] rowid:{1}, exception:{0}, paymentResponse:{1}", ex.Message, row.id, response);
                return false;
            }
        }

        private bool Payment优信付(ofAffWithdraw row)
        {
            string postUrl = "https://api.youxintrade.com//PaymentGetway/SinglePay";
            string _merchantID = "AWC001";
            string _merhcnatKey = "1beaff1e5404f5b73dcbfebcb1703c60";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10234/v2";

            string returnUrl = "https://www.awcwin8.com";
            var stf = new Thirdpay.优信付.SPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "168.95.1.1"
            };

            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [优信付] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
            }
            catch (Exception ex)
            {
                string response = stf.paymentResponse ?? "noresponse";
                _logger.LogError("SendPayment - [优信付] rowid:{1}, exception:{0}, paymentResponse:{1}", ex.Message, row.id, response);
                return false;
            }
        }
        private bool Payment依云付(ofAffWithdraw row)
        {
            string _merchantID = "800358";
            string _merhcnatKey = "126E0B01C6006873FCA0C2E6A6CDB4A5";
            string postUrl = "https://pay.evianpay.com/daifu";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10234/v2";

            string returnUrl = "https://www.awcwin8.com";
            var stf = new Thirdpay.依云付.SPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl
            };

            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [依云付] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
            }
            catch (Exception ex)
            {
                string response = stf.paymentResponse ?? "noresponse";
                _logger.LogError("SendPayment - [依云付] rowid:{1}, exception:{0}, paymentResponse:{1}", ex.Message, row.id, response);
                return false;
            }
        }


        private bool Payment万富(ofAffWithdraw row)
        {
            string _merchantID = "5012";
            string _merhcnatKey = "1ZHsjcxEJtqRZeu5iUiB134qu05DEPKkpEWYelZMQ7A8FX4spTP3aGOhT9b1MnH6";
            string postUrl = "https://shapi.wfpay88.com/v1/dfapi/add";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10234/v2";

            string returnUrl = "https://www.awcwin8.com";
            var stf = new Thirdpay.万富.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl
            };

            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [万富] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
            }
            catch (Exception ex)
            {
                string response = stf.paymentResponse ?? "noresponse";
                _logger.LogError("SendPayment - [万富] rowid:{1}, exception:{0}, paymentResponse:{1}", ex.Message, row.id, response);
                return false;
            }
        }


        private bool PaymentGPay(ofAffWithdraw row)
        {
            string _merchantID = "C1000148";
            string _merhcnatKey = "P4nlvoBuqjAtbyZyiRaDmpDUw1NH5tAw";
            string postUrl = "https://gapi.llque.com/api/v2/daifu/request";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10234/v2";

            string returnUrl = "https://www.awcwin8.com";
            var stf = new Thirdpay.GPay.SPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl
            };

            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [GPay] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
            }
            catch (Exception ex)
            {
                string response = stf.paymentResponse ?? "noresponse";
                _logger.LogError("SendPayment - [GPay] rowid:{1}, exception:{0}, paymentResponse:{1}", ex.Message, row.id, response);
                return false;
            }
        }

        private bool PaymentOneTech(ofAffWithdraw row)
        {
            //string _merchantID = "463151377597383";
            ////string _merchantKey = "8a1s6a12GFxM25pQ0mn|74#u&8#XE7";
            //string _merchantKey = "Tv86LrVb3Qx4M9gFav|74#u&8#XE7"; // 2019-03-19 Updated

            string _merchantID = "YB2008010";
            string _merchantKey = "d24801acf86461247d10b9c213a84f5c";
            string postUrl = "http://api.ybfu8.com/cashout";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10234/v2";

            string returnUrl = "https://www.awcwin8.com";
            var stf = new Thirdpay.OneTech.SPayment(_merchantID, _merchantKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl
            };

            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [OneTech] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [OneTech] rowid:{1}, exception:{0}", ex.Message, row.id);
                return false;
            }
        }
        //private bool Payment葡萄(ofAffWithdraw row)
        //{
        //    string returnUrl = "https://www.awcwin8.com";
        //    string _merchantID = "729013803";
        //    string _merchantKey = "20bf9782d26ff04ee34466dffda7a426";
        //    string postUrl = "http://order.myyoushang.top/api/pay/pay";
        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";
        //    var stf = new Thirdpay.葡萄.YSPayment(_merchantID, _merchantKey, callbackUrl, postUrl)
        //    {
        //        account = row.account,
        //        returnUrl = returnUrl
        //    };

        //    try
        //    {

        //        Thirdpay.Common.BANKS destBank;
        //        if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //            return false;

        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - [葡萄] result:{1}, response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - [葡萄] rowid:{1}, exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}
        //private bool Payment壹支付(ofAffWithdraw row)
        //{
        //    string returnUrl = "https://www.awcwin8.com";
        //    string _merchantID = "kDK2XDnFGT";
        //    string _merchantKey = "oGIyTFwA7XD9r8tBMDp1";
        //    string postUrl = "http://api.onepayer.cn/pay.php";
        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";
        //    var stf = new Thirdpay.壹支付.YZFPayment(row.account, _merchantID, _merchantKey, callbackUrl, postUrl)
        //    {
        //        returnUrl = returnUrl
        //    };

        //    try
        //    {

        //        Thirdpay.Common.BANKS destBank;
        //        if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //            return false;

        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - [壹支付] result:{1}, response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - [壹支付] rowid:{1}, exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}

        //private bool Payment富通(ofAffWithdraw row)
        //{
        //    string postUrl = "http://47.110.237.3/v2/WTWithdraw";
        //    string _merchantID = "1554433586|wxcy20190405";
        //    string _merhcnatKey = "117F4608EF1EAA4E4C3728B0CD3CACC5";
        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
        //    string returnUrl = "https://www.awcwin8.com";

        //    var stf = new Thirdpay.富通.FTPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
        //    {
        //        account = row.account
        //    };

        //    if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}

        //private bool Payment赢达(ofAffWithdraw row)
        //{
        //    string _merchantID = "73682|AWC";
        //    string _merchantKey = "ECBFCE6EF6D37A45C03F";
        //    string postUrl = "http://23.98.37.163:8080/PAY";
        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
        //    string returnUrl = "https://www.awcwin8.com";

        //    var stf = new Thirdpay.赢达.YingDaPayment(_merchantID, _merchantKey, callbackUrl, postUrl)
        //    {
        //        account = row.account
        //    };

        //    if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}

        //private bool PaymentBingo(ofAffWithdraw row)
        //{
        //    string merchantID = "542018040512058829|3820180105120579";
        //    string merchantKey = "SY5ASpH7Rs4EtJbARmc2pfdbnAWMhP74|CzB3YrYrHWzYYcXfCBrZBEQExMwjKB3m";
        //    string postURL = "https://www.bingopay.net/open-gateway/";
        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10052";

        //    if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        var stf = new Thirdpay.Bingo.BingoPay(merchantID, merchantKey, callbackUrl, postURL)
        //        {
        //            returnUrl = "https://www.awcwin8.com"
        //        };
        //        string orderid = stf.GenerateOrderID;
        //        var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}
        //#endif
    }
}

