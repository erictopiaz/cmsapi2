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
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class DirectPayRequestTest : IRequest<ResponseModel<DirectPayRequestTestReturn>>
    {
        public long Id { get; set; }
        public decimal fee { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class DirectPayRequestTestReturn
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class DirectPayRequestTestValidator : AbstractValidator<DirectPayRequestTest>
    {
        public DirectPayRequestTestValidator()
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
    public class DirectPayRequestTestHandler : BaseWithdraw<DirectPayRequestTest, ResponseModel<DirectPayRequestTestReturn>>
    {
        public DirectPayRequestTestHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<DirectPayRequestTestReturn> Handle(DirectPayRequestTest request)
        {
            var isSuccess = Payment纯代付("care111", 2f, "6217003380011770341", "廖武蜜", "建设银行");
            return new ResponseModel<DirectPayRequestTestReturn>()
            {
                code = isSuccess ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                message = "",
                Data = null
            };
        }


        private bool UpdatePaymentLog(long id, string orderid, string paymentResponse, bool payResult) {
            int  paymentlgoInsertResult = 0;
            
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
        private bool Payment爱汇02(ofWithdrawal row)
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
        private bool Payment爱汇(string account, float amount, string cardno, string realname, string cardbank)
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
                account = account,
                returnUrl = returnUrl,
                //publicOrPrivate = 1
            };

            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)amount, destBank, cardno, realname);
                _logger.LogInformation("SendPayment - [爱汇] result:{1}, response:{0}", stf.paymentResponse, payResult.ToString());
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [爱汇] exception:{0}", ex.Message);
                return false;
            }
        }
        private bool Payment众宝(ofWithdrawal row)
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
        private bool Payment众宝私(ofWithdrawal row)
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
        private bool Payment纯代付(string account, float amount, string cardno, string realname, string cardbank)
            {
            string _merchantID = "882570270206985";
            string _merhcnatKey = "B19CED6C9EDF2ACD32AC5D2525ACB2C5";
            string postUrl = "http://58.82.235.62:8080/OnlinePay/wstPayOther";
            //string _merchantID = "100088010001";
            //string _merhcnatKey = "F$Ult0#pJFK75&K1T4Dw^495MKJcafUBGX0ZM5U&ZNtyLq^6YoO0*&0m#endF&FI|#*r6L@QvsJfZa*_s=SBzU3C7vhH";
            //string postUrl = "http://api.sssdyg.com/webapi/order/addOutOrder";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.纯代付.CDFPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = account
            };

            if (cardbank == "邮政储蓄") cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)amount, destBank, cardno, realname);
                _logger.LogInformation("SendPayment - [纯代付] Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString());
                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [纯代付] Exception:{0}", ex.Message);
                return false;
            }
        }
        private bool Payment很美(ofWithdrawal row)
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




        //private bool Payment速付(ofWithdrawal row)
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
        //private bool Payment速付02(ofWithdrawal row)
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
        //private bool PaymentJBP(ofWithdrawal row)
        //{

        //    string merchantID = "8";
        //    string merchantKey = "20171215AWCCWA";
        //    string _callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10045";
        //    string postURL = "https://api.jbp-pay.com/apply";

        //    if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        var stf = new Thirdpay.Daddy.JBP(merchantID, merchantKey, _callbackUrl, postURL)
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
        //private bool PaymentDaddy(ofWithdrawal row)
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
        //private bool Payment速龙(ofWithdrawal row, bool 加急)
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
        //private bool Payment易付(ofWithdrawal row)
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
        //private bool Payment易付今(ofWithdrawal row)
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
        //private bool Payment四通(ofWithdrawal row)
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
        //private bool Payment易富通(ofWithdrawal row)
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
        //private bool Payment速达(ofWithdrawal row)
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
        //private bool Payment智联(ofWithdrawal row)
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
        //private bool Payment通用(ofWithdrawal row)
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

        //private bool Payment金汇富(ofWithdrawal row)
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
        //private bool Payment金汇富02(ofWithdrawal row)
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
        //private bool Payment壹快付(ofWithdrawal row)
        //{
        //    string _merchantID = "463151377597383";
        //    //string _merchantKey = "8a1s6a12GFxM25pQ0mn|74#u&8#XE7";
        //    string _merchantKey = "Tv86LrVb3Qx4M9gFav|74#u&8#XE7"; // 2019-03-19 Updated
        //    string returnUrl = "https://www.awcwin8.com";
        //    string postUrl = "https://livepay.ykf38.com/ezpay/";
        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";
        //    var stf = new Thirdpay.壹快付.YKFPayment(_merchantID, _merchantKey, callbackUrl, postUrl)
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
        //        _logger.LogInformation("SendPayment - [壹快付] result:{1}, response:{0}", stf.paymentResponse, payResult.ToString());
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("SendPayment - [壹快付] rowid:{1}, exception:{0}", ex.Message, row.id);
        //        return false;
        //    }
        //}
        //private bool Payment葡萄(ofWithdrawal row)
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
        //private bool Payment壹支付(ofWithdrawal row)
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

        //private bool Payment富通(ofWithdrawal row)
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

        //private bool Payment赢达(ofWithdrawal row)
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

        //private bool PaymentBingo(ofWithdrawal row)
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

