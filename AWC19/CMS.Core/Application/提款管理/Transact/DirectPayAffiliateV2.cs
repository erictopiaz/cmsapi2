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
    public class DirectPayAffiliateV2 : IRequest<ResponseModel<DirectPayAffiliateV2Return>>
    {
        public long Id { get; set; }

        public decimal fee { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class DirectPayAffiliateV2Return
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class DirectPayAffiliateV2Validator : AbstractValidator<DirectPayAffiliateV2>
    {
        public DirectPayAffiliateV2Validator()
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
    public class DirectPayAffiliateV2Handler : BaseWithdraw<DirectPayAffiliateV2, ResponseModel<DirectPayAffiliateV2Return>>
    {
        public DirectPayAffiliateV2Handler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<DirectPayAffiliateV2Return> Handle(DirectPayAffiliateV2 request)
        {
            lock (this)
            {
                if ((DateTime.Now - CachedData.QueryTimeDict["DirectPayRequest"]).TotalMilliseconds < 5000)

                {
                    return new ResponseModel<DirectPayAffiliateV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "点击台快, 请慢慢来",
                        Data = null
                    };
                }
                CachedData.QueryTimeDict["DirectPayRequest"] = DateTime.Now;

            }
            //return new ResponseModel<DirectPayAffiliateV2Return>()
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


                var directpay_details = connection.Get<ofSettings>("PAYEMENT_DIRECTPAY_V2");
                JObject PAYEMENT_DIRECTPAY_V2 = JObject.Parse(directpay_details.body);
                float maxDirectPay = float.Parse(PAYEMENT_DIRECTPAY_V2["level_cat"]["aff"]["maxamount"].ToString());
                targetPlatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["aff"]["payplatform"].ToString();
                var default_usdt_maximumamount = PAYEMENT_DIRECTPAY_V2["level_cat"]["default_usdt"]["maxamount"].ToString();
                var default_usdt_payplatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["default_usdt"]["payplatform"].ToString();
                var usdt_low_maximumamount = PAYEMENT_DIRECTPAY_V2["level_cat"]["usdt_low"]["maxamount"].ToString();
                var usdt_hi_maximumamount = PAYEMENT_DIRECTPAY_V2["level_cat"]["usdt_hi"]["maxamount"].ToString();
                var usdt_low_payplatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["usdt_low"]["payplatform"].ToString();
                var usdt_hi_payplatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["usdt_hi"]["payplatform"].ToString();

                var usdttrc_low_payplatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["usdttrc_low"]["payplatform"].ToString();
                var usdttrc_hi_payplatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["usdttrc_hi"]["payplatform"].ToString();


                // USDT PORTION
                if (withdrawRow.cardbank == "USDT" || withdrawRow.cardbank == "USDT-ERC20")
                {
                    targetPlatform = usdt_low_payplatform;
                    if ((float)withdrawRow.amount > float.Parse(usdt_low_maximumamount))
                        targetPlatform = usdt_hi_payplatform;

                    if ((float)withdrawRow.amount > float.Parse(usdt_hi_maximumamount))
                        targetPlatform = "";
                }



                if (withdrawRow.cardbank == "USDT-TRC20")
                {
                    targetPlatform = usdttrc_low_payplatform;
                    if ((float)withdrawRow.amount > float.Parse(usdt_low_maximumamount))
                        targetPlatform = usdttrc_hi_payplatform;

                    if ((float)withdrawRow.amount > float.Parse(usdt_hi_maximumamount))
                        targetPlatform = "";
                    //_logger.LogError("NoSupported Yet - [SendPayment] withdrawamount:{0}", withdrawRow.amount);
                    //return new ResponseModel<DirectPayAffiliateV2Return>()
                    //{
                    //    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    //    message = "TRC20 代付通道还没接好",
                    //    Data = null
                    //};
                }


                if (string.IsNullOrEmpty(targetPlatform))
                {
                    _logger.LogError("NoPlatformSelected - [SendPayment] withdrawamount:{0}", withdrawRow.amount);
                    return new ResponseModel<DirectPayAffiliateV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "没可用的通道",
                        Data = null
                    };
                }

                itemToProces = withdrawRow;
                // Should be in paywait mode 
                if (withdrawRow.status != 1)
                    return new ResponseModel<DirectPayAffiliateV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "不可进行支付的状态",
                        Data = null
                    };

                // Check MAX DirectPay Amount

                if (withdrawRow.amount > (decimal)maxDirectPay)
                {
                    _logger.LogError("代付 - [ExceedLimitAmount] max:{0}, requestedby:{2} requestedamount:{1}", maxDirectPay.ToString(), withdrawRow.amount, request.Admin.ToLower());
                    return new ResponseModel<DirectPayAffiliateV2Return>()
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
                    return new ResponseModel<DirectPayAffiliateV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "本申请已经代付完成",
                        Data = null
                    };
                }

                //targetPlatform = paymentdirectpaydetails_JOBJ["aff_payplatform"].ToString();

                // Get Platform fee percentage based on the targetPlatform
                JArray gatewaylist = JArray.Parse(PAYEMENT_DIRECTPAY_V2["payplatform"].ToString());
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
                catch (Exception) { }


                bool getbalance_result = GetRemainingBalance(targetPlatform, withdrawRow.account, withdrawRow.cardbank, out getbalance_message, out getbalance_amount, withdrawRow.amount, usdt_xrate);

                if (!getbalance_result)
                {
                    _logger.LogError("{1} - [SendPayment] message:{0}",
                        getbalance_message, targetPlatform);
                    return new ResponseModel<DirectPayAffiliateV2Return>()
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
                    return new ResponseModel<DirectPayAffiliateV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = string.Format("【" + targetPlatform + "】金额不足 剩下 - RMB{0}", getbalance_amount.ToString("0.00")),
                        Data = null
                    };
                }
            }




            using (TransactionScope scope = new TransactionScope())
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var withdrawRow = connection.Get<ofAffWithdraw>(request.Id);




                itemToProces = withdrawRow;
                // Should be in paywait mode 
                if (withdrawRow.status != 1)
                    return new ResponseModel<DirectPayAffiliateV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "不可进行支付的状态",
                        Data = null
                    };


                // Check if already INSERTED in the paymentLog, So i'll just need to do a query instead
                var ofpaymentlogCount = connection.QueryFirstOrDefault<long?>("SELECT * FROM ofPaymentLog WITH(NOLOCK) WHERE Transactionid=" + withdrawRow.id.ToString() + " AND transtype=1");

                if (ofpaymentlogCount > 0)
                {
                    return new ResponseModel<DirectPayAffiliateV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "本申请已经代付完成",
                        Data = null
                    };
                }


                // Verify BANKS
                if (withdrawRow.cardbank == "上海浦东发展银行") withdrawRow.cardbank = "浦东发展银行";
                if (withdrawRow.cardbank == "广发银行") withdrawRow.cardbank = "广东发展银行";



                // Verify USDT Wallet Address
                if (withdrawRow.cardbank == "USDT" || withdrawRow.cardbank == "USDT-ERC20")
                {
                    if (!withdrawRow.cardno.StartsWith("0x"))
                    {
                        return new ResponseModel<DirectPayAffiliateV2Return>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "清检查钱包地址",
                            Data = null
                        };
                    }
                }



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
                    return new ResponseModel<DirectPayAffiliateV2Return>()
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
                catch (Exception ex)
                {
                }

                _logger.LogInformation("代付 - [{0}] account:{1}, amount:{2} rowId:{3}",
                                    targetPlatform, itemToProces.account, itemToProces.amount, itemToProces.id);

                switch (targetPlatform)
                {
                    case "BitWin-USDT": isSuccess = PaymentBitWinUSDT(itemToProces, usdt_xrate); break;
                    case "爱汇USDT": isSuccess = Payment爱汇USDT(itemToProces, usdt_xrate); break;
                    case "爱汇USDT-TRC20": isSuccess = Payment爱汇USDTTRC20(itemToProces, usdt_xrate); break;
                    case "壹圆USDT": isSuccess = Payment壹圆USDT(itemToProces, usdt_xrate); break;
                    case "壹圆USDT-TRC20": isSuccess = Payment壹圆USDTTRC20(itemToProces, usdt_xrate); break;
                    case "GPAYUSDT-TRC20": isSuccess = PaymentGPay_USDTTRC(itemToProces, usdt_xrate); break;

                    case "OnePay": isSuccess = PaymentOnePay(itemToProces); break;
                    case "很美": isSuccess = Payment很美(itemToProces); break;
                    case "众宝私": isSuccess = Payment众宝私(itemToProces); break;
                    case "众宝": isSuccess = Payment众宝(itemToProces); break;
                    case "爱汇": isSuccess = Payment爱汇(itemToProces); break;
                    case "爱汇02": isSuccess = Payment爱汇02(itemToProces); break;
                    case "爱汇03": isSuccess = Payment爱汇03(itemToProces); break;
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
                    case "GPVIP": isSuccess = PaymentGPVIP(itemToProces); break;
                    case "依云付": isSuccess = Payment依云付(itemToProces); break;
                    case "优信付": isSuccess = Payment优信付(itemToProces); break;
                    case "天空": isSuccess = Payment天空(itemToProces); break;
                    case "天空49": isSuccess = Payment天空49(itemToProces); break;
                    case "万富": isSuccess = Payment万富(itemToProces); break;
                    case "JBP": isSuccess = PaymentJBP(itemToProces); break;
                    case "聚宝盆": isSuccess = Payment聚宝盆(itemToProces); break;
                    case "普宝盆": isSuccess = Payment普宝盆(itemToProces); break;
                    case "现在代付": isSuccess = PaymentMMP(itemToProces); break;
                    case "MMPB": isSuccess = PaymentMMPB(itemToProces); break;
                    case "币付": isSuccess = Payment币付(itemToProces); break;
                    case "机构代付": isSuccess = Payment机构代付(itemToProces); break;
                    case "云代付": isSuccess = Payment云代付(itemToProces); break;
                    case "新支付": isSuccess = Payment新支付(itemToProces); break;
                    case "新高付": isSuccess = Payment新高付(itemToProces); break;
                    case "新欧付": isSuccess = Payment新欧付(itemToProces); break;
                    case "欧付": isSuccess = Payment欧付(itemToProces); break;
                    case "爱汇06": isSuccess = Payment爱汇06(itemToProces); break;
                    case "爱汇07": isSuccess = Payment爱汇07(itemToProces); break;
                    case "龙腾": isSuccess = Payment龙腾(itemToProces); break;
                    case "富乐": isSuccess = Payment富乐(itemToProces); break;
                    case "壹圆": isSuccess = Payment壹圆(itemToProces, usdt_xrate); break;
                    case "佰圆": isSuccess = Payment佰圆(itemToProces, usdt_xrate); break;
                    case "仟圆": isSuccess = Payment仟圆(itemToProces, usdt_xrate); break;
                    case "富众": isSuccess = Payment富众(itemToProces); break;
                    case "古利": isSuccess = Payment古利(itemToProces); break;
                    case "新古利": isSuccess = Payment新古利(itemToProces); break;
                    case "旺寶": isSuccess = Payment旺寶(itemToProces); break;
                    case "Daddy": isSuccess = PaymentDaddy(itemToProces); break;
                    case "亚洲龙": isSuccess = Payment亚洲龙(itemToProces); break;
                    case "三河": isSuccess = Payment三河(itemToProces); break;
                    case "平安": isSuccess = Payment平安(itemToProces); break;
                    case "新爱付": isSuccess = Payment新爱付(itemToProces); break;
                    case "易新付": isSuccess = Payment易新付(itemToProces); break;
                    case "金星": isSuccess = Payment金星(itemToProces); break;
                    case "豪杰75": isSuccess = Payment豪杰75(itemToProces); break;
                    case "豪杰18": isSuccess = Payment豪杰18(itemToProces); break;
                    case "天地": isSuccess = Payment天地(itemToProces); break;
                    case "MPAY": isSuccess = PaymentMPAY(itemToProces); break;
                    case "阳光": isSuccess = Payment阳光(itemToProces); break;
                    case "顺兴": isSuccess = Payment顺兴(itemToProces); break;
                    case "壁虎": isSuccess = Payment壁虎(itemToProces); break;
                    case "火腾": isSuccess = Payment火腾(itemToProces); break;
                    case "高火腾": isSuccess = Payment高火腾(itemToProces); break;
                    case "普龙": isSuccess = Payment普龙(itemToProces); break;
                    case "幸福": isSuccess = Payment幸福(itemToProces); break;
                    case "幸运": isSuccess = Payment幸运(itemToProces); break;
                    case "奇点": isSuccess = Payment奇点(itemToProces); break;
                    case "金乐": isSuccess = Payment金乐(itemToProces); break;
                    case "聚富": isSuccess = Payment聚富(itemToProces); break;
                    case "质远": isSuccess = Payment质远(itemToProces); break;
                    case "青苹果": isSuccess = Payment青苹果(itemToProces); break;
                    case "金苹果": isSuccess = Payment金苹果(itemToProces); break;
                    case "新金苹果": isSuccess = Payment新金苹果(itemToProces); break;
                    case "新维鑫": isSuccess = Payment新维鑫(itemToProces); break;
                    case "维鑫": isSuccess = Payment维鑫(itemToProces); break;
                    case "新龙鞥": isSuccess = Payment新龙鞥(itemToProces); break;
                    case "WL": isSuccess = PaymentWL(itemToProces); break;
                    case "快乐": isSuccess = Payment快乐(itemToProces); break;
                    case "亿贝": isSuccess = Payment亿贝(itemToProces); break;
                    case "鑫鑫": isSuccess = Payment鑫鑫(itemToProces); break;
                    case "汇款": isSuccess = Payment汇款(itemToProces); break;
                    case "飞杨": isSuccess = Payment飞杨(itemToProces); break;
                    case "保盈": isSuccess = Payment保盈(itemToProces); break;
                    case "MP": isSuccess = PaymentMP(itemToProces); break;
                    case "万象式": isSuccess = Payment万象式(itemToProces); break;
                    case "新天空": isSuccess = Payment新天空(itemToProces); break;
                    case "DP": isSuccess = PaymentDP(itemToProces); break;
                    case "稳妥": isSuccess = Payment稳妥(itemToProces); break;
                    case "鑫城": isSuccess = Payment鑫城(itemToProces); break;
                    case "吉祥": isSuccess = Payment吉祥(itemToProces); break;
                    case "東邦": isSuccess = Payment東邦(itemToProces); break;
                    case "阳付": isSuccess = Payment阳付(itemToProces); break;
                    case "万象式二": isSuccess = Payment万象式二(itemToProces); break;
                    case "聚专富": isSuccess = Payment聚专富(itemToProces); break;
                    case "聚富小额": isSuccess = Payment聚富小额(itemToProces); break;
                    case "聚富95818": isSuccess = Payment聚富95818(itemToProces); break;
                    case "绅付": isSuccess = Payment绅付(itemToProces); break;
                    case "备付金": isSuccess = Payment备付金(itemToProces); break;
                    case "QP卡": isSuccess = PaymentQP卡(itemToProces); break;
                    case "捷凯": isSuccess = Payment捷凯(itemToProces); break;
                    case "鳄鱼": isSuccess = Payment鳄鱼(itemToProces); break;

                    default: isSuccess = false; break;
                }


                return new ResponseModel<DirectPayAffiliateV2Return>()
                {
                    code = isSuccess ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "",
                    Data = null
                };
            }

        }



        private bool Payment鳄鱼(ofAffWithdraw row)
        {
            string returnUrl = "https://www.awcwin8.com";

            string callbackUrl = "https://eocwpqz587487cl.m.pipedream.net";

            string _merchantID = "74228";
            string _merhcnatKey = "kzdDx4055Nt7Q8GY18154X911OpG821o1JF";
            string postUrl = "https://service.api.xbmn452h3vv.lmifu.xyz/InterfaceV5/CreateWithdrawOrder/";


            var stf = new Thirdpay.鳄鱼.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2",
            };

            stf.proxy_address = "103.125.216.18";
            stf.proxy_port = 8888;

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);

                _logger.LogInformation("SendPayment - [捷凯] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }





        private bool Payment捷凯(ofAffWithdraw row)
        {
            string returnUrl = "https://www.awcwin8.com";

            string callbackUrl = "https://eocwpqz587487cl.m.pipedream.net";

            //string _merchantID = "WXC888";
            //string _merhcnatKey = "3mMnlqRifNU28ke1tVLEdPSGKOpTrWFa|1pNBsxqTwVCUQj5d";
            //string postUrl = "https://bmyl.uas-gw.info/v4/withdrawal/WXC888/forward";

            string _merchantID = "74228";
            string _merhcnatKey = "kzdDx4055Nt7Q8GY18154X911OpG821o1JF";
            string postUrl = "https://service.api.xbmn452h3vv.lmifu.xyz/InterfaceV5/CreateWithdrawOrder/";

            //string postUrl = "https://bankapi.quq8.top/withdrawal/creatWithdrawal";

            var stf = new Thirdpay.捷凯.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2",
            };

            stf.proxy_address = "103.125.216.18";
            stf.proxy_port = 8888;

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);

                _logger.LogInformation("SendPayment - [捷凯] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }



        private bool PaymentQP卡(ofAffWithdraw row)
        {
            string returnUrl = "https://www.awcwin8.com";

            string callbackUrl = "https://eocwpqz587487cl.m.pipedream.net";

            string _merchantID = "WXC888";
            string _merhcnatKey = "3mMnlqRifNU28ke1tVLEdPSGKOpTrWFa|1pNBsxqTwVCUQj5d";
            string postUrl = "https://bmyl.uas-gw.info/v4/withdrawal/WXC888/forward";


            //string postUrl = "https://bankapi.quq8.top/withdrawal/creatWithdrawal";

            var stf = new Thirdpay.QP卡.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            stf.proxy_address = "103.125.216.18";
            stf.proxy_port = 8888;

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);

                _logger.LogInformation("SendPayment - [QP卡] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }




        private bool Payment备付金(ofAffWithdraw row)
        {
            string returnUrl = "https://www.awcwin8.com";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";

            string _merchantID = "1072233";
            string _merhcnatKey = "A5Ocaoqh9cwAsPg05J51tZQMuYXXeSSW";
            string postUrl = "https://bankapi.quq8.top/withdrawal/creatWithdrawal";

            var stf = new Thirdpay.备付金.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2",
            };

            stf.proxy_address = "103.125.216.18";
            stf.proxy_port = 8888;

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);

                _logger.LogInformation("SendPayment - [备付金] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool GetRemainingBalance(string targetPlatform, string account, string cardbank, out string message, out float remainingBalance, decimal amountrequested, float usdt_xrate)
        {
            cardbank = cardbank == "邮政储蓄" ? "邮政银行" : cardbank;
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



                //case "信付":
                //    var 信付 = new Thirdpay.信付.SPayment("1013796859",
                //        "abhsunngflzgoikxho",
                //        "http://pay.allpayzs.com/Payment/Callback/10234/v2",
                //        "https://newapi.xfuoo.com/balanceQuery")
                //    {
                //        account = account
                //    };
                //    // VerifyBank
                //    Thirdpay.Common.BANKS 信付destBank;
                //    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 信付destBank))
                //    {
                //        if (!信付.bankCode.ContainsKey(信付destBank))
                //        {
                //            _logger.LogCritical("信付 - [SendPayment] bank type doesn't exist, Bank:{0}", 信付destBank.ToString());
                //            message = string.Format("【信付】Bank Doesn't Exist, Bank:{0}", 信付destBank.ToString());
                //            return false;
                //        }
                //    }
                //    else
                //    {
                //        _logger.LogCritical("信付 - [SendPayment] bank type doesn't exist, Bank:{0}", 信付destBank.ToString());
                //        message = string.Format("【信付】Bank Doesn't Exist, Bank:{0}", 信付destBank.ToString());
                //        return false;

                //    }

                //    remainingBalance = 信付.remainingBalance;
                //    message = 信付.paymentResponse;


                //    break;


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
                        //if (依云付destBank == Thirdpay.Common.BANKS.兴业银行)
                        //{
                        //    _logger.LogCritical("依云付 - [SendPayment] bank type doesn't exist, Bank:{0}", 依云付destBank.ToString());
                        //    message = string.Format("【依云付】Bank Doesn't Exist, Bank:{0}", 依云付destBank.ToString());
                        //    return false;
                        //}
                    }

                    _logger.LogInformation("依云付 - [SendPayment] cardbank:{0}, 依云付destBank:{1}", cardbank, 依云付destBank.ToString());
                    remainingBalance = 100000;
                    break;

                case "旺寶":
                case "富众":
                case "壹圆":
                case "富乐":
                case "龙腾":
                case "新欧付":
                case "欧付":
                case "优信付":
                case "新支付":
                case "Daddy":
                case "JBP":
                case "诚信通":
                case "GPay":
                case "万富":
                case "OneTech":
                case "亚洲龙":
                case "三河":
                case "平安":
                case "新爱付":
                case "易新付":
                case "新高付":
                case "金星":
                case "火腾":
                case "高火腾":
                case "普龙":
                case "豪杰":
                case "天地":
                case "MPAY":
                case "阳光":
                case "奇点":
                case "壁虎":
                case "幸运":
                case "幸福":
                case "顺兴":
                case "金乐":
                case "聚富":
                case "质远":
                case "青苹果":
                case "金苹果":
                case "新金苹果":
                case "新维鑫":
                case "维鑫":
                case "壹圆USDT-TRC20":
                case "壹圆USDT":
                case "新龙鞥":
                case "WL":
                case "天空":
                case "天空49":
                case "信付":
                case "快乐":
                case "亿贝":
                case "鑫鑫":
                case "汇款":
                case "飞杨":
                case "保盈":
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
                        //if (壹快付destBank == Thirdpay.Common.BANKS.浦东发展银行 ||
                        //    壹快付destBank == Thirdpay.Common.BANKS.广东发展银行)
                        //{
                        //    _logger.LogCritical("壹快付 - [SendPayment] bank type doesn't exist, Bank:{0}", 壹快付destBank.ToString());
                        //    message = string.Format("【壹快付】Bank Doesn't Exist, Bank:{0}", 壹快付destBank.ToString());
                        //    return false;
                        //}
                    }
                    remainingBalance = 100000;
                    message = "No Balance Query Available";
                    break;
                case "BitWin-USDT":
                case "捷优代付":
                case "壹元":
                case "壹元公":
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

                case "爱汇USDT-TRC20":
                    #region Details

                    var 爱汇USDTTRC20 = new Thirdpay.爱汇.AhPayment("94951", "2ae47347-a166-4375-bc83-4c92cec71c0a", "", "https://api.ichatpay168.com/api/getbalance", Thirdpay.Common.BANKS.USDTTRC);
                    remainingBalance = 爱汇USDTTRC20.remainingBalance * usdt_xrate;
                    message = 爱汇USDTTRC20.paymentResponse;
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


                case "爱汇07":
                    #region Details
                    var 爱汇07 = new Thirdpay.爱汇.AhPayment("54603", "38979949-c978-45b4-a7d3-2107b964b979", "", "https://api.ichatpay168.com/api/getbalance");
                    // VerifyBank
                    Thirdpay.Common.BANKS 爱汇07destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 爱汇07destBank))
                    {
                        if (!爱汇07.bankCode.ContainsKey(爱汇07destBank))
                        {
                            _logger.LogCritical("爱汇07 - [SendPayment] bank type doesn't exist, Bank:{0}", 爱汇07destBank.ToString());
                            message = string.Format("【爱汇07】Bank Doesn't Exist, Bank:{0}", 爱汇07destBank.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogCritical("爱汇07 - [SendPayment] bank type doesn't exist, Bank:{0}", 爱汇07destBank.ToString());
                        message = string.Format("【爱汇07】Bank Doesn't Exist, Bank:{0}", 爱汇07destBank.ToString());
                        return false;
                    }

                    remainingBalance = 爱汇07.remainingBalance;
                    message = 爱汇07.paymentResponse;
                    #endregion
                    break;


                case "爱汇06":
                    #region Details
                    var 爱汇06 = new Thirdpay.爱汇.AhPayment("53077", "59aa5fc5-200d-4279-876f-1502fdd6974e", "", "https://api.ichatpay168.com/api/getbalance");
                    // VerifyBank
                    Thirdpay.Common.BANKS 爱汇06destBank;
                    if (Enum.TryParse<Thirdpay.Common.BANKS>(cardbank, out 爱汇06destBank))
                    {
                        if (!爱汇06.bankCode.ContainsKey(爱汇06destBank))
                        {
                            _logger.LogCritical("爱汇06 - [SendPayment] bank type doesn't exist, Bank:{0}", 爱汇06destBank.ToString());
                            message = string.Format("【爱汇06】Bank Doesn't Exist, Bank:{0}", 爱汇06destBank.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogCritical("爱汇06 - [SendPayment] bank type doesn't exist, Bank:{0}", 爱汇06destBank.ToString());
                        message = string.Format("【爱汇06】Bank Doesn't Exist, Bank:{0}", 爱汇06destBank.ToString());
                        return false;
                    }

                    remainingBalance = 爱汇06.remainingBalance;
                    message = 爱汇06.paymentResponse;
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
                    "WHERE TransactionId=@ID AND transtype=1", (new
                    {
                        ID = id.ToString(),
                        ORDERID = orderid,
                        PAYMENTRESPONSE = paymentResponse,
                        PAYRESULT = payResult ? "1" : "0"
                    }));
            }
            return paymentlgoInsertResult == 1;
        }





        private bool Payment壹圆USDT(ofAffWithdraw row, float usdt_xrate)
        {
            string returnUrl = "https://www.awcwin8.com";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string _merchantID = "awc1000";
            string _merhcnatKey = "d3b52c365c9b9e563a3127682fa42aeb";
            string postUrl = "https://awc3rdpay.azurewebsites.net/api/withdraw/RequestWithdraw";

            //var pay = new Thirdpay.壹圆.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            //{
            //    account = account,
            //    returnUrl = returnUrl,
            //    clientIPAddress = "168.95.1.1",
            //    realName = "测试",
            //    usdt_erc20_xrate = 6.5f
            //};

            var stf = new Thirdpay.壹圆.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2",
                usdt_erc20_xrate = usdt_xrate

            };


            Thirdpay.Common.BANKS destBank;
            string row_cardbank = row.cardbank;
            if (row_cardbank == "USDT" || row_cardbank == "USDT-ERC20")
                destBank = Thirdpay.Common.BANKS.USDTERC20;
            else if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;


            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);

                _logger.LogInformation("SendPayment - [壹圆USDT] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }





        private bool PaymentGPay_USDTTRC(ofAffWithdraw row, float usdt_xrate)
        {
            string _merchantID = "C1000143";
            string _merhcnatKey = "IyA9zv7mfrlnYHY3aYxSrtOpLduzhOsm";
            //string _merchantID = "C1000148";
            //string _merhcnatKey = "P4nlvoBuqjAtbyZyiRaDmpDUw1NH5tAw";
            string postUrl = "https://gapi.llque.com/api/v2/daifu/request";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10234/v2";

            string returnUrl = "https://www.awcwin8.com";
            var stf = new Thirdpay.GPay.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                usdt_erc20_xrate = usdt_xrate
            };

            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            if (destBank != Thirdpay.Common.BANKS.USDTTRC)
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
                _logger.LogInformation("SendPayment - [GPayUSDTTRC] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
            }
            catch (Exception ex)
            {
                string response = stf.paymentResponse ?? "noresponse";
                _logger.LogError("SendPayment - [GPayUSDTTRC] rowid:{1}, exception:{0}, paymentResponse:{1}", ex.Message, row.id, response);
                return false;
            }
        }



        private bool Payment壹圆USDTTRC20(ofAffWithdraw row, float usdt_xrate)
        {
            string returnUrl = "https://www.awcwin8.com";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string _merchantID = "awc1000";
            string _merhcnatKey = "d3b52c365c9b9e563a3127682fa42aeb";
            string postUrl = "https://awc3rdpay.azurewebsites.net/api/withdraw/RequestWithdraw";

            //var pay = new Thirdpay.壹圆.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            //{
            //    account = account,
            //    returnUrl = returnUrl,
            //    clientIPAddress = "168.95.1.1",
            //    realName = "测试",
            //    usdt_erc20_xrate = 6.5f
            //};

            var stf = new Thirdpay.壹圆.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2",
                usdt_erc20_xrate = usdt_xrate

            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);

                _logger.LogInformation("SendPayment - [壹圆USDTTRC20] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
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


        private bool Payment爱汇USDTTRC20(ofAffWithdraw row, float usdt_xrate)
        {
            // Convert to USDT Rate
            //float usdt_amount = ((float)row.amount / usdt_xrate);

            string postUrl = "https://api.ichatpay168.com/api/payout";
            string _merchantID = "94951";
            string _merchantKey = "2ae47347-a166-4375-bc83-4c92cec71c0a";

            string returnUrl = "https://www.awcwin8.com";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";
            var stf = new Thirdpay.爱汇.AhPayment(_merchantID, _merchantKey, callbackUrl, postUrl, Thirdpay.Common.BANKS.USDTTRC)
            {
                account = row.account,
                returnUrl = returnUrl,
                usdt_erc20_xrate = usdt_xrate
            };

            Thirdpay.Common.BANKS destBank = Thirdpay.Common.BANKS.USDTTRC;
            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [爱汇USDT-TRC20] cny:{2}, usdt_xrate:{3} result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, usdt_xrate);
                return UpdatePaymentLog_USDT(row.id, orderid, stf.paymentResponse, payResult, usdt_xrate);
            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [爱汇USDT-TRC20] rowid:{1}, exception:{0}", ex.Message, row.id);
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
            if (row_cardbank == "USDT" || row_cardbank == "USDT-ERC20")
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
                    "WHERE TransactionId=@ID AND transtype=1", (new
                    {
                        ID = id.ToString(),
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

        private bool Payment爱汇07(ofAffWithdraw row)
        {
            string postUrl = "https://api.ichatpay168.com/api/payout";
            string _merchantID = "54603";
            string _merchantKey = "38979949-c978-45b4-a7d3-2107b964b979";

            string returnUrl = "https://www.awcwin8.com";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
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
                _logger.LogInformation("SendPayment - [爱汇07] result:{1}, response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [爱汇07] rowid:{1}, exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment爱汇06(ofAffWithdraw row)
        {
            string postUrl = "https://api.ichatpay168.com/api/payout";
            string _merchantID = "53077";
            string _merchantKey = "59aa5fc5-200d-4279-876f-1502fdd6974e";

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
                _logger.LogInformation("SendPayment - [爱汇06] result:{1}, response:{0}", stf.paymentResponse, payResult.ToString());
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [爱汇06] rowid:{1}, exception:{0}", ex.Message, row.id);
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


        private bool Payment天空(ofAffWithdraw row)
        {
            string _merchantID = "6745";
            string _merhcnatKey = "" +
            "MIIJRAIBADANBgkqhkiG9w0BAQEFAASCCS4wggkqAgEAAoICAQDhKYqbbp5RXu9/" +
            "bFXvEmiBXg+wmdzHvGZ9l/BNElNLPbXTwGhEg+mHGywQSyp1R//zYTbs7ejQyFjA" +
            "cwmrTnJJ2k6zew74L95+NZ+XF8ugJHratK07Qk3ccGiHw7Mb4T9p1FcTubZIe6We" +
            "e4LunZVzX4/vowqKcSg8I6/d9cJ9ZazirGGC/CEiNiiY3pTLqBwNW5O66mRoZ2Ms" +
            "z23oeO9zH2h71tflujEvWTiZ+SoLMuns0Rf7WI0KrFOcUvYPvcjp8ICEeUyUQAN+" +
            "+RyONGB8UhOFMBfA6j/py66YqzSbnSPAoNMBg0YvM1+pq/A2yiJ/UVlyxWQi1pE4" +
            "c9JYIzvgtUqLZU06lzApvNXtV3j+vW0+nJlLSluvEEoagSximKZ2N+e4po99z0i1" +
            "/AVUPCl2Cvf+hnvWqBhw4bE4KoLA2KyNQytV/AaFP94fs8lr9LuoaL3oo9hVezKQ" +
            "cKpG/UvX2pcjnKWjS9aMNZBgfGSCvNZwDNEgFkDF4HOFTdcuuG8IxxT5b0G/v83s" +
            "LsM0I+FEAk0sAkqNTfMhkf+oERmg/sTZtsQCeLe6JkqVoopnZccdCU3fPIWeMJ2g" +
            "TtBiMqi3cb0Zb8n3fAkG0s/9pLFJOaHI3E3kjoAEoMSi/sJ0vNHyB5kcoUWh3yg7" +
            "kdtaoZRVTDekDXxRGgJD1XB5i+GxmQIDAQABAoICAQDEGaAEW0U7bUl9WudE/m1S" +
            "Lo/PqVGmC6nfvJKnLUyp/Sb9M1gcRB8BcmLgFfb2iiya1ZI2q8XXAIZ7pjVHo1za" +
            "gW2F16lj8pz+oslJmInaAauWC0HMmKSjVqtTgtfdckH+c9ca8OWXH0lRTBTuqQRe" +
            "pV4AwakOhhsataIX2FD5UbJPAP80S064Mx8MdN52uQUlhRqW8klcvJ6dilMY4yr4" +
            "wuQG50jFZXIs4LHxOBasBj3B6SHP4IGGSCvRvFddwlbXmIT124ccntvsnB2IpIlb" +
            "/UfO+eHWPK5JewGnKYT5+tQGkorziFd6CG5KodrmfQmqhTk4Qj5CMUntNsJ9zMH6" +
            "Auk3lDoXA0Ahy/sHe5LblElk+kDKb749XXxWcjmknxYZzMmcK3azEfCFXLt4CGlU" +
            "NO5HhN9dMTVwpkel81Mt87HHDCvfRMthPB1PdSWshMYjuQ6yNfbyLRDOW3VF1BAt" +
            "1NZA1vtikXXgmqpKRLIxuhfcmhvo37FQevbT4g7OliTqqZIrsPMVll7vBYthgdjB" +
            "R7r1qxptniAwjPENEWXmklU/8iAVQS2PhOpOfD8CVeZ/ve70FmZldayjDMqmwlcV" +
            "YEqpPhvHNNC6SwnQbkHCzspSj+VwJcgVppEc7YfH2/2Qx+u5jE/pM6bnQyLW9rqX" +
            "wJyCYsgeHVAitg0RjDsY+QKCAQEA8/iVcVHw010pM7ab+IOzk8t7HEa7B02mYTtK" +
            "N8SDwZsYZ1f8PeeJIyAGIhYKTZ964z4UF7z/fN80EUt3Jmx50+BAjWQO4MRhXGdh" +
            "Pirk5ZDfSEUFU41ONaaQFjBhD87PmwGkgzjjgZjA/YHnh044kHGdE1wUsEyWOoZC" +
            "AH8/ibLbIOttZBSxTzBIQAsnmXtVtqg4o3dAS56FlD21lNbZ9ti1ubtagaVoflrn" +
            "zpJcMCYyRzgtCsU0Bh4foczvFdimOyRMkaOPNyf900Wg6ZTFjYIBU1rqG+iqisQn" +
            "EBnGvdr36kdLwgpCXlAA09Bg0QgBxxampb/KFWDAjHF7hmWkdwKCAQEA7EONbRAF" +
            "ims9Q2S2rW2ECZO6iFxfseoMr410dyc8jFixxYfh/TLI7AvguWVp5u0ZSzgUTyhS" +
            "ERTmZYaAyPyEr1A9HHS6SiI0ymYSAXNIqSDcxm/HC68XE7smt7tZCTljeHaokBu2" +
            "7sHKLQ4ZvAaSdC8d1KsO/PUAazzup9BLFjZ7rbGJzhg1EWUc7dsit+cYrWsRm83j" +
            "ePGu4ucVbmN7nhoci8RnoBWEqIdwcUyYF1CJbgNaGeE6b3OPYWoESBFTmrjMipjg" +
            "/LJALMI40GD6nU8uR0dHpgQnXZk0JmV8nbsGnL9qfHa0OobkieqsfBGoKEJohXYb" +
            "g1VmlrSqyk4ubwKCAQEAvGzpPrRCg3wYLlCC9dUCBMvLMXOPT3yxW7cnAxtLh2P5" +
            "3RcfQdBpIi/BjOHo4zqg9pEjg5H84Lyiwhi+1XZCQVmZgZodw4rohy1w9Uq7rnHN" +
            "gUuNaZJhLR6PehOzYzofLWB/ZrmxPI51v1VtNBs9pJGa9zDO6bq87EZ1A11D/V2d" +
            "0DcELZ+tsbNgmdYDp46SDJiv5dBcYAr2sB9Y9rNZdES7Vc9Vf3v40F31Wp2/hZB6" +
            "lHbWktLr8y2g0J4uqXjCKjtGx9TC7557c4j7VmwIh9o4m+BDn3p6/OGyKcU+sbNE" +
            "djTKVfsGd0Di4rS1kbvVY6Yi3zDZKtmm3NFozcwC0QKCAQEAvngPMddywA6ISyaM" +
            "ZvAdPCNOg9QEicfoBjcBp3WISJTQbDCJrbgdZr6EorHxWGvYsl05cj9NOdwhicOP" +
            "W3jH9xW2zpQEAvPkny7+YRDxiPenRIqkavTyo8ODCSgONXOAj4MJjXPqXDz6oqv1" +
            "bzHVK7k4UVS+NovFlK6Ar+UqQ3QU7Qht0iqNzcI4OkEpkhzKNjRU88heFAizNtWY" +
            "329wHxtp2/uwlswCMGNP8OcQzertgDVPlAGRPHxrINt5LChbwCYvL6Tj7JQ/TFYf" +
            "u04TfHJzGp+KUXqTyc9SzYmPlxFJ2WAvlruTvIM3B0CbYUJS0HIJFV8L+ysEE4b/" +
            "ES1KHQKCAQBfEi1pHsdBm9N24ht+9HRTXhqZ8X7bGHWHKtJKEvP57ah3cKMJ34u0" +
            "R3p8LQKK+E7Ci463AIVZtsMjy0VnEy57ufQwCqmP98chNTOWm2Wn8qGYDThRaS4G" +
            "dJ59SXPZgF2YJiYFO+7vdgEcyQnfQvG13xk362xETVmhle0iBblhdcXZgkg0qhT7" +
            "NX+762j6VT95w0GPkvspjlBUeXAIaLPSwbH2mgai+g00jQWa4R8m5gZwGU3UTRpk" +
            "NFG71/3kPXsIy4P4hJHvhlqvxvCU7l0DfsIjWqiU4aBEgErr4JTYBQjY9PeICIZg" +
            "c0UMfQCi9E5QmKmfmo5mhrf0WhoI44qq" +
            "|459a0dc8169748c99af46261d9ba51c7";
            string postUrl = "https://service.cornerstone-pay.com";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10601/v2";
            string returnUrl = "https://www.awcwin8.com";


            var stf = new Thirdpay.天空.S(_merchantID, _merhcnatKey, callbackUrl, postUrl);

            stf.account = row.account;

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [天空] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool PaymentWL(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string returnUrl = "https://www.awcwin8.com";
            string _merchantID = "PF0027";
            string _merhcnatKey = "eyJhbGciOiJIUzI1NiJ9.eyJ1c2VySWQiOjAsInBsYXRmb3JtSWQiOjI3LCJhZ2VudElkIjowLCJ2ZXJzaW9uIjoxLCJwYXltZW50SWQiOjAsImlhdCI6MTYzNjQyMjA0NX0.uYaLwMy5xqFRCuIJ02ehvUlkkouPZdFqta7mFmefYX0|DV95IefpIeP3Frdz";
            string postUrl = "https://hbj168.club/gateway/api/v2/payouts";


            var stf = new Thirdpay.WT.S(_merchantID, _merhcnatKey, callbackUrl, postUrl);

            stf.account = row.account;

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [WL] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment新龙鞥(ofAffWithdraw row)
        {
            string postUrl = "https://gwy-mgt.go36pay.com";
            string _merchantID = "3FD211105095210M6078";
            string _merhcnatKey = "6e88e273df73c79407a78111c1100cc3|e00ea107f09240db0b27bbe940f3030f";
            string returnUrl = "https://www.awcwin8.com";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";

            var stf = new Thirdpay.龙腾.S2(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [新龙鞥] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }
        private bool Payment新维鑫(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string _merchantID = "2021123507";
            string _merhcnatKey = "ZvBQjzVTgiEdYPIUXDYAfWuErlKOAHnK";
            string postUrl = "https://wxp-pay.cc/Apipay";
            string returnUrl = "https://www.awcwin8.com";


            var stf = new Thirdpay.维鑫.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [新维鑫] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }




        private bool Payment万象式二(ofAffWithdraw row)
        {

            string _merchantID = "300148";
            string _merhcnatKey = "6kegt7ay4bg860ku57w1qo5tnukv0qb8";
            string postUrl = "https://api.jptxnewapidc.com/withdraw/create";

            string callbackUrl = "https://e24ey513fx-swp.azurewebsites.net/Payment/Callback/1/v2";
            //string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";

            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.万象式.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [万象式] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [万象式] RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment阳付(ofAffWithdraw row)
        {
            /*
             
             */
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";

            string _merchantID = "c4abf9ace5e64d22b1ef7862a26b04cf";
            string _merhcnatKey = "1c0347b56d704035b2215e14f586b923";
            string postUrl = "https://www.zipay268.com/transfer/apply";

            _merchantID = "20222423s0eaoo";
            _merhcnatKey = "8f4a5cd6bf8d4c4f2e2e09b8dd7b7192";
            postUrl = "https://dai.bzsmw.net/withdraw_v2/create_order";



            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.阳付.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [阳付] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment東邦(ofAffWithdraw row)
        {
            /*
             
             */
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";

            string _merchantID = "c4abf9ace5e64d22b1ef7862a26b04cf";
            string _merhcnatKey = "1c0347b56d704035b2215e14f586b923";
            string postUrl = "https://www.zipay268.com/transfer/apply";




            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.東邦.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [東邦] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }




        private bool Payment吉祥(ofAffWithdraw row)
        {
            /*
             
             */
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string _merchantID = "OKSH133427";
            string _merhcnatKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCNCUxIi/WaDY/Yr6RBHzd8GubBrJ8wW8rZMy4al2U6SydSz1WFEmKsqNsuXJ6pxGtIS9wTabMP0Av5hAV0bezFnBguo60t2xLoPLGPWBZCUYuETk4EPYrHe4E96c282lZiup20F/6EB7twOJQH4/IfsZdjiiVsFh+P6beMQ4xCQwIDAQAB";

            string postUrl = "";
            //postUrl = "http://zpay.website/pay/order/acp";
            postUrl = "https://a.qpay.cloud/pay/order/acp";


            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.幸运.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [吉祥] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }




        private bool Payment稳妥(ofAffWithdraw row)
        {
            string _merchantID = "PF0129";
            string _merhcnatKey = "a3iaNoQ4CzI4JQSi|eyJhbGciOiJIUzI1NiJ9.eyJ1c2VySWQiOjAsInBsYXRmb3JtSWQiOjEyOSwiYWdlbnRJZCI6MCwidmVyc2lvbiI6MSwicGF5bWVudElkIjowLCJpYXQiOjE2NDgwMDgwNzF9.icPOcoLt0mbjHCpTZpt5kJplJjlCsGS8dY0ZNUaLRn8";
            string postUrl = "https://hbj168.club/gateway/api/v1/payments";
            string returnUrl = "https://www.baidu.com";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";

            var stf = new Thirdpay.稳妥.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [DP] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment鑫城(ofAffWithdraw row)
        {
            string _merchantID = "PF0006";
            string _merhcnatKey = "pSSJCrC3jIvhIydr|eyJhbGciOiJIUzI1NiJ9.eyJ1c2VySWQiOjAsInBsYXRmb3JtSWQiOjYsImFnZW50SWQiOjAsInZlcnNpb24iOjEsInBheW1lbnRJZCI6MCwiaWF0IjoxNjQ5Mzk4MzY2fQ.IgaljdZaBcDp5JHnOC2Qq4doWTMGJ6YtC9A6LQLWv50";
            //string postUrl = "https://xincheng-pay.com/gateway/api/v2/payments";
            string postUrl = "https://xincheng-pay.com/gateway/api/v2/payouts";
            string returnUrl = "https://www.baidu.com";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";

            var stf = new Thirdpay.鑫城.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [鑫城] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool PaymentDP(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string _merchantID = "CID11101";
            string _merhcnatKey = "B5CNaF5uVkjQ6jf";
            string postUrl = "https://web.jf3092.com/paygate/withdraw.aspx";
            string returnUrl = "https://www.awcwin8.com";


            var stf = new Thirdpay.DP.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [DP] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool PaymentMP(ofAffWithdraw row)
        {


            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";

            string _merchantID = "105011205005";
            string _merhcnatKey = "ac7567be278edda66c7c75d8ee6f0451";
            string postUrl = "https://api.mpay.bet/gateway/withdrawal ";
            string returnUrl = "https://www.awcwin8.com";


            var stf = new Thirdpay.MP.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            stf.proxy_address = "103.125.216.18";
            stf.proxy_port = 8888;
            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [MP] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment保盈(ofAffWithdraw row)
        {
            string _merchantID = "1019540";
            string _merhcnatKey = "F42eDcnEeCk66f5DlNfDBsYkUno9T02D";
            string postUrl = "https://byapi.fastpay9.com/withdrawal/creatWithdrawal";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";

            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.保盈.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [顺兴] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment维鑫(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string _merchantID = "2021118278";
            string _merhcnatKey = "qKSOWkpsvQjmGdpzPTVOsrSAmLtiDSaU";
            string postUrl = "https://wxp-pay.cc/Apipay";
            string returnUrl = "https://www.awcwin8.com";


            var stf = new Thirdpay.维鑫.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [维鑫] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }
        private bool Payment金苹果(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string _merchantID = "";
            string _merhcnatKey = "";

            string postUrl = "http://api.ygtrade8.com/PaymentGetway/SinglePay";
            string returnUrl = "https://www.awcwin8.com";

            _merchantID = "AWS002";
            _merhcnatKey = "7ee7af84fe2740d69f49b525297cdabc";
            postUrl = "http://api.qpgtrade.com/PaymentGetway/SinglePay";


            var stf = new Thirdpay.阳光.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [青苹果] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }
        private bool Payment新金苹果(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string _merchantID = "";
            string _merhcnatKey = "";

            string postUrl = "http://api.feiyangtrade8.com/PaymentGetway/SinglePay";
            string returnUrl = "https://www.awcwin8.com";

            _merchantID = "AWS002";
            _merhcnatKey = "d4c466fc065280d03ca60627bed2855e";
            //postUrl = "http://api.qpgtrade.com/PaymentGetway/SinglePay";


            var stf = new Thirdpay.阳光.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [新金苹果] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment青苹果(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string _merchantID = "";
            string _merhcnatKey = "";

            string postUrl = "http://api.ygtrade8.com/PaymentGetway/SinglePay";
            string returnUrl = "https://www.awcwin8.com";

            _merchantID = "AWS001";
            _merhcnatKey = "3f4aecbdab8539faee9de57d7af079a3";
            postUrl = "http://api.qpgtrade.com/PaymentGetway/SinglePay";


            var stf = new Thirdpay.阳光.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [青苹果] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment质远(ofAffWithdraw row)
        {

            try
            {
                string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
                //string _merchantID = "110126";
                //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
                string _merchantID = "1154";
                string _merhcnatKey = "VIZCZRWILYVTFJD76JO1U30NWJ4E7KVXEZTPJNNEEAO4IREHSOGIM0K1SRSVMQKH74PBPVI2TDX2JNRREKVCKDDJBRIH0FV6QS5KQD8XRVNSQMH51GOIUNZKI1DH7PSS";

                string postUrl = "http://payapi.g4ds4jse.zypay.cc:6080/api/obpay/transfer";
                string returnUrl = "https://www.awcwin8.com";

                _logger.LogInformation("SendPayment - [质远] RowID:{0}, Initialization ", row.id);
                var stf = new Thirdpay.质远.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
                {
                    account = row.account,
                    returnUrl = returnUrl,
                    clientIPAddress = "1.2.2.2"
                };
                _logger.LogInformation("SendPayment - [质远] RowID:{0} Init Finished", row.id);

                if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
                Thirdpay.Common.BANKS destBank;
                if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                    return false;
                string orderid = stf.GenerateOrderID;

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                _logger.LogInformation("SendPayment - [质远] RowID:{0} Sending Payment", row.id);

                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [质远] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
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



        private bool PaymentBitWinUSDT(ofAffWithdraw row, float usdt_xrate)
        {
            //string postUrl = "https://api.bitwin.ai/api/v1/MerchantWithdraw";

            string _merchantID = "awc5";
            string postUrl = "https://api.bitwin.ai/api/v1/MerchantWithdraw";
            string _merchantkey = "1F9F157129FB24ACD878F76C52423C09";
            string returnUrl = "https://www.awcwin8.com";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/20192/v2";
            var stf = new Thirdpay.Bitwin.S(_merchantID, _merchantkey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2",
                usdt_erc20_xrate = usdt_xrate
            };

            Thirdpay.Common.BANKS destBank;
            string row_cardbank = row.cardbank;
            if (row_cardbank == "USDT" || row_cardbank == "USDT-ERC20")
                destBank = Thirdpay.Common.BANKS.USDTERC20;
            else if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID;
                var payResult = stf.SendPayment(orderid, (float)row.amount, destBank, row.cardno, "");

                _logger.LogInformation("SendPayment - [BitWinUSDT] cny:{2}, usdt_xrate:{3} result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, usdt_xrate);
                return UpdatePaymentLog_USDT(row.id, orderid, stf.paymentResponse, payResult, usdt_xrate);
            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [BitWinUSDT] rowid:{1}, exception:{0}", ex.Message, row.id);
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
            //string postUrl = "http://www.kunpengnb.com:8080/OnlinePay/wstPayOther";
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



        private bool Payment仟圆(ofAffWithdraw row, float usdt_xrate)
        {
            string returnUrl = "https://www.awcwin8.com";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string _merchantID = "awc3000";
            string _merhcnatKey = "b63c8b031a3c18b08d255584cd8f810b";
            string postUrl = "https://awc3rdpay.azurewebsites.net/api/withdraw/RequestWithdraw";

            //var pay = new Thirdpay.壹圆.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            //{
            //    account = account,
            //    returnUrl = returnUrl,
            //    clientIPAddress = "168.95.1.1",
            //    realName = "测试",
            //    usdt_erc20_xrate = 6.5f
            //};

            var stf = new Thirdpay.壹圆.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2",
                usdt_erc20_xrate = usdt_xrate

            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);

                _logger.LogInformation("SendPayment - [仟圆] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment佰圆(ofAffWithdraw row, float usdt_xrate)
        {
            string returnUrl = "https://www.awcwin8.com";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string _merchantID = "awc2000";
            string _merhcnatKey = "b0552cd09f105ca52cad60bb0bf191bf";
            string postUrl = "https://awc3rdpay.azurewebsites.net/api/withdraw/RequestWithdraw";

            //var pay = new Thirdpay.壹圆.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            //{
            //    account = account,
            //    returnUrl = returnUrl,
            //    clientIPAddress = "168.95.1.1",
            //    realName = "测试",
            //    usdt_erc20_xrate = 6.5f
            //};

            var stf = new Thirdpay.壹圆.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2",
                usdt_erc20_xrate = usdt_xrate

            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);

                _logger.LogInformation("SendPayment - [壹圆] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment万象式(ofAffWithdraw row)
        {

            string _merchantID = "201012";
            string _merhcnatKey = "65h684x8dimogkm2ti2k3zqfansy29x5";
            string postUrl = "http://api.jiantxnewapibac.com/withdraw/create";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";

            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.万象式.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [万象式] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [万象式] RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool PaymentMMP(ofAffWithdraw row)
        {
            string postURL = "https://api.mpay.bet/gateway/withdrawal";
            //string _merchantID = "105111300001";
            //string _merhcnatKey = "b47d04793cdbd721139c262040ffa3ff";
            string _merchantID = "105011205005";
            string _merhcnatKey = "ac7567be278edda66c7c75d8ee6f0451";

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

        private bool Payment绅付(ofAffWithdraw row)
        {

            try
            {
                string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
                //string _merchantID = "110126";
                //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
                string _merchantID = "MER20220408184640757480";
                string _merhcnatKey = "c8441e5d93a543b7aacf6e22ff1bbbbc";
                string postUrl = "	http://api.nbpay.info/settlement/pay";
                string returnUrl = "https://www.awcwin8.com";

                _logger.LogInformation("SendPayment - [绅付] RowID:{0}, Initialization ", row.id);
                var stf = new Thirdpay.绅付.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
                {
                    account = row.account,
                    returnUrl = returnUrl,
                    clientIPAddress = "1.2.2.2"
                };
                _logger.LogInformation("SendPayment - [绅付] RowID:{0} Init Finished", row.id);

                if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
                Thirdpay.Common.BANKS destBank;
                if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                    return false;
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                _logger.LogInformation("SendPayment - [绅付] RowID:{0} Sending Payment", row.id);

                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [绅付] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }



        private bool Payment聚富95818(ofAffWithdraw row)
        {
            try
            {
                /*
链接：jupay.pw
商户号：95806
登录名：wanxiang@qq.com	
密码：123456
密钥：a9d59393d8794dea891967dea8c07c0e
                 */
                string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
                //string _merchantID = "110126";
                //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
                string _merchantID = "95818";
                string _merhcnatKey = "3e3ac0ba7fd14d6cb3f4aab05be2101c";
                string postUrl = "http://api.jupay.pw/payforcustom.aspx";
                string returnUrl = "https://www.awcwin8.com";

                _logger.LogInformation("SendPayment - [聚富95818] RowID:{0}, Initialization ", row.id);
                var stf = new Thirdpay.聚富.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
                {
                    account = row.account,
                    returnUrl = returnUrl,
                    clientIPAddress = "1.2.2.2"
                };
                _logger.LogInformation("SendPayment - [聚富95818] RowID:{0} Init Finished", row.id);

                if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
                Thirdpay.Common.BANKS destBank;
                if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                    return false;
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                _logger.LogInformation("SendPayment - [聚富95818] RowID:{0} Sending Payment", row.id);

                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [聚富95818] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment聚富小额(ofAffWithdraw row)
        {

            try
            {
                /*
链接：jupay.pw
商户号：95806
登录名：wanxiang@qq.com	
密码：123456
密钥：a9d59393d8794dea891967dea8c07c0e
                 */
                string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
                //string _merchantID = "110126";
                //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
                string _merchantID = "95806";
                string _merhcnatKey = "a9d59393d8794dea891967dea8c07c0e";
                string postUrl = "http://api.jupay.pw/payforcustom.aspx";
                string returnUrl = "https://www.awcwin8.com";

                _logger.LogInformation("SendPayment - [聚富小额] RowID:{0}, Initialization ", row.id);
                var stf = new Thirdpay.聚富.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
                {
                    account = row.account,
                    returnUrl = returnUrl,
                    clientIPAddress = "1.2.2.2"
                };
                _logger.LogInformation("SendPayment - [聚富小额] RowID:{0} Init Finished", row.id);

                if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
                Thirdpay.Common.BANKS destBank;
                if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                    return false;
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                _logger.LogInformation("SendPayment - [聚富小额] RowID:{0} Sending Payment", row.id);

                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [聚富小额] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }




        private bool Payment聚专富(ofAffWithdraw row)
        {

            try
            {
                string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
                //string _merchantID = "110126";
                //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
                string _merchantID = "95804";
                string _merhcnatKey = "72d4c76fedab4f81bb051b94791e2f6b";
                string postUrl = "http://api.jupay.pw/payforcustom.aspx";
                string returnUrl = "https://www.awcwin8.com";

                _logger.LogInformation("SendPayment - [聚富] RowID:{0}, Initialization ", row.id);
                var stf = new Thirdpay.聚富.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
                {
                    account = row.account,
                    returnUrl = returnUrl,
                    clientIPAddress = "1.2.2.2"
                };
                _logger.LogInformation("SendPayment - [聚富] RowID:{0} Init Finished", row.id);

                if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
                Thirdpay.Common.BANKS destBank;
                if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                    return false;
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                _logger.LogInformation("SendPayment - [聚富] RowID:{0} Sending Payment", row.id);

                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [聚富] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
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
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string returnUrl = "https://www.awcwin8.com";

            string _merchantID = "AWS001";
            string _merhcnatKey = "903ccfbafa4be9a248cfe4dff379fd26";
            string postUrl = "http://api.youxintrade.com/PaymentGetway/SinglePay";

            var stf = new Thirdpay.阳光.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [壁虎] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        //private bool Payment信付(ofAffWithdraw row)
        //{

        //    string _merchantID = "1013796859";
        //    string _merhcnatKey = "abhsunngflzgoikxho";
        //    string postUrl = "https://newapi.xfuoo.com/agentPay";


        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
        //    string returnUrl = "https://www.awcwin8.com";

        //    var stf = new Thirdpay.信付.SPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
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

            postUrl = "https://v2.ezcash88.com/ezpay/";
            _merchantID = "832943417221559";
            _merchantKey = "xK287kmvbhw12YvnwRsx|74#u&8#XE7";


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


        private bool Payment三河(ofAffWithdraw row)
        {
            string _merchantID = "210568023";
            string _merhcnatKey = "1zqzte9ywnzta7dc3zl5xcvpouosi1rt";
            string postUrl = "https://www.sanhe16888.com//Payment_Dfpay_add.html";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.三河.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = row.requestip,
            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [三河] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment亚洲龙(ofAffWithdraw row)
        {
            string _merchantID = "90000121";
            string _merhcnatKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAlmVbpQE+ojwKxKQ+4UYCwIxmOyYgtWjuvgRerFzbTdahXncBxefoZ+8mfD7XmP1foMgxRxfa/ASdGalHFADyxnRerrjrha5H5CdkkiTAPLAZbPRTGxr6DGouxi+jH89slYEUTtK+qhjwHWLujkPehLVr2RiOhPKxaiwUThBajbYPuzRBppIc3yrz8VbyMKSzl4qk5B5c+ZVCyhNh2pVEZoZplst1c6nt2zsC3O0nx9GRKzcSb1Bg8F0VM7hRLdWyKvRCFvN047Uh0SZK5isxQXPD2Fj9ga6QI8ITDrB79i9B0/rrn3ciWSMZc31KCmjXKUJeQCtD5x+0udWqAnWUdwIDAQAB|MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCMtIdRp0WxB3IrMNgj/ABSPiz+DM9BCYOz9D6KIa+eSabt/fGhR7ZGJ4v/mCVafqATGEETDz7zmPzhjFQa5mCWQJMf2892jv/bdPHgRtCQ406x/VPK2yxvEtYokAD/6at8SkRCsu67bNvj8yk9WAsQ6XeuJnMikLs1ALMvGT5tnLFd0m+m6enD2tIVzG9R/hPXTPtkNSKnZo+DtRy9EFxjzl61+qz3EoIdESokijvybqp2IdCK7NKQ3KQNVnSebeLHHO78co6Hp0dS3M8vPTT2Oc+iNL7TuYao7UIrTq8w0/3UCSqIgSaKDoxOC0R6f7gvHbgdAnUb4ELQKAPnMdgnAgMBAAECggEAaKCxYxFsbEhqKB3JC3EPWBit4jfqQaVujvkXLCYCbkS8YYqNC+3EZ4RuwSAE3rb8F1bYxPB4UjG4EYwE2n/m4LOh83XDXpjVPv5GAg0GeEbetNH4Zgb2kp3CLCVw6Nt8Tg73lVVy2mIHXQjq6Je4mtnQyeoDgXd4DeC4XVR7LIvWd1I0iCVmppFEx+07VsHv3ENFRCwR4XL/s4lK9MrAV9mYoS3G2dtwk6m6fS17o5PmFW1Yop546Q64DhEcPMrYvKs7JeZQY49/tVpTMf1TtJLpTPd92BaShnzm5jWm9OTZ7w7fvr7TePsJDeceU/oz7GYN4TVXyQ7ahMTnMffLCQKBgQDGm9bv2iaidO1NZb9DjepNO8IZ0BzrI7M0/FtCZEW9NCfUHGQVwlpd2WBfMgfAZx4Iu7oTkYjTRSlM0R2DTjsCKhZTzf7AYHgJ+uI8aiFSafIadHKADq7KqktqmPhmMaNxwKxoe5BSQrLbnbxbeGq9aAnz9wkXftaB03mdXd6IWwKBgQC1XUAcZLGXpkxLDKyeCXbUndkSLDe+6n6BqrcJCpRUhpWrz7LMugFJ+zIucRgY9WzPrOhgSH3O6sBDSPiLCSEA5sz7ALi5gPShFH9Dm/AB3hgYGySQP2k5YYO6rHU4SKfLM+lhMLzaj/7cuyu35soLYSwTfsg1Ao/KDk74/8nZJQKBgQC7NtFv/NJoVxIN4Dhf5JmVPhxkmrF30YovCya3ObAnfs+AeVa60B7A0pSuh4mZC1rbXqIzqf8xL8OTnpdUW9zx2Fx9mPNnwkfFGJJD4sMQXcASwevg3HZVohs/G30jT+1C4B1ky/0qHaddr52LF8J+UmD3cMlINY7ciEzeU0reewKBgGL3Wa6ku8wW8PbbGa67V166K+OgwWKqrxGuyGZkoYBmWHICynyw21c1fbKeABpOgmvo3Hz90bkBnJyayxWDi8DkMOKfCC4h0lu7xZ5tD1DVkqxE03TrRU4MikdpKCXob7eQwj9IyyUxAlAMytR1WbdP9S9GNZXHqhMGL83sNocJAoGAV/nNO22ZowLV6AS9JxGb7DwE1XHKfYeh/wQ5Pm8Tci54atxaZ3F6SyqJCUMBvGkLP1S/AmvJ6288FT8T+hIhdf0O1jrdUJ8l17dTSZd7cy/l7jjbwd2JKif5JBzNVs52biy94yiTddeQv945UG1tHGQDH2LJFCpFvK71zs5JI0U=";
            string postUrl = "http://api.asia-pay8.com/api/agentPay/draw";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.亚洲龙.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = row.requestip,
            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [亚洲龙] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }



        private bool Payment古利(ofAffWithdraw row)
        {
            string _merchantID = "RCWO8940";
            string _merhcnatKey = "021ra4o5613sa910";
            string postUrl = "https://cashwork.luck-pay.com/jili/ap.do";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.古利.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = row.requestip,
            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [古利] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool PaymentDaddy(ofAffWithdraw row)
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


        private bool Payment金乐(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string _merchantID = "tv4Y82H864092aN22T8S8z5T88I3C872";
            string _merhcnatKey = "^f8q({<(!%&&10ME3985]45A%$3{I|S[9@5&~5_#*@d5V6hG~013M7~=g8ebZF~A";

            string postUrl = "";
            postUrl = "https://jinlepay.com/api/order/payment/createorder";

            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.金乐.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [金乐] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment聚富(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string _merchantID = "95685";
            string _merhcnatKey = "e1b148d166844711bdc64265c14d2571";
            string postUrl = "http://api.jupay.pw/payforcustom.aspx";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.聚富.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [聚富] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment旺寶(ofAffWithdraw row)
        {
            string _merchantID = "213f9f891303e60b629f1013";
            string _merhcnatKey = "f58e05cb5442de9c40ddb21a8020de03b1d6ad0df2ffe244e82db54f7d392271aa333cb0ad9e7cefc3b87694505a96ffb886a62c13e439af8f7dbb8e753afc6a06ddf39663487f678ed4e036afc11ce3e30da3c105b87b4c5bbc7f2fae29ba3d168547837d52fe5b0494c4efd97f37d4eaf73f096a1c0a6a08b050217b2ad53d";
            string postUrl = "https://api.wb55688.com/agent_pay";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.旺寶.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = row.requestip,
            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [古利] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentReponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentReponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment新古利(ofAffWithdraw row)
        {
            string _merchantID = "RCLM6866";
            string _merhcnatKey = "9k3wq37xtle449xh";
            string postUrl = "https://cashwork.luck-pay.com/jili/ap.do";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.古利.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = row.requestip,
            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [古利] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment壹圆(ofAffWithdraw row, float usdt_xrate)
        {
            string returnUrl = "https://www.awcwin8.com";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string _merchantID = "awc1000";
            string _merhcnatKey = "d3b52c365c9b9e563a3127682fa42aeb";
            string postUrl = "https://awc3rdpay.azurewebsites.net/api/withdraw/RequestWithdraw";

            //var pay = new Thirdpay.壹圆.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            //{
            //    account = account,
            //    returnUrl = returnUrl,
            //    clientIPAddress = "168.95.1.1",
            //    realName = "测试",
            //    usdt_erc20_xrate = 6.5f
            //};

            var stf = new Thirdpay.壹圆.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "168.95.1.2",
                usdt_erc20_xrate = usdt_xrate
            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                stf.clientIPAddress = "168.95.1.2";
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [壹圆] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment富众(ofAffWithdraw row)
        {
            string returnUrl = "https://www.awcwin8.com";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string _merchantID = "ZbC6tEBa";
            string _merhcnatKey = "nfjoV7qRzWFvNXAu";

            string postUrl = "https://ltezp.xyz/payfor/trans";

            var stf = new Thirdpay.龙腾.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"

            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [富众] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment富乐(ofAffWithdraw row)
        {
            string returnUrl = "https://www.awcwin8.com";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string _merchantID = "GXy5sBk0";
            string _merhcnatKey = "nk4hR2frwbzGqPgc";

            string postUrl = "https://ltezp.xyz/payfor/trans";

            var stf = new Thirdpay.龙腾.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"

            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [富众] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment龙腾(ofAffWithdraw row)
        {
            string returnUrl = "https://www.awcwin8.com";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string _merchantID = "36w50Tgx";
            string _merhcnatKey = "BaL64TSfCcPjlbQ8";

            string postUrl = "https://ltezp.xyz/payfor/trans";

            var stf = new Thirdpay.龙腾.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"

            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [龙腾] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment欧付(ofAffWithdraw row)
        {
            string returnUrl = "https://www.awcwin8.com";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "192956";
            //string _merhcnatKey = "BC56DC8Z9H3Q54PDEPL2UOJU|M8A3FQVOT2TY1K11KCIIFVPGU04WZE0S";

            string _merchantID = "192956";
            string _merhcnatKey = "38537DTQMP18Y4N0NM92QOCN|IR75EUOGRGJHY2OD0102DPG0TT2K9UKM";
            string postUrl = " https://api.kuaizhifu.xyz/gateway/trade/v1/remit";
            //string postUrl = " https://api.kuaizhifu.xyz/gateway/mer/v1/withdrawals";

            var stf = new Thirdpay.方舟.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"

            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [欧付] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment新欧付(ofAffWithdraw row)
        {
            string returnUrl = "https://www.awcwin8.com";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string _merchantID = "192956";
            string _merhcnatKey = "BC56DC8Z9H3Q54PDEPL2UOJU|M8A3FQVOT2TY1K11KCIIFVPGU04WZE0S";

            //string _merchantID = "192960";
            //string _merhcnatKey = "38537DTQMP18Y4N0NM92QOCN|IR75EUOGRGJHY2OD0102DPG0TT2K9UKM";
            //string _merhcnatKey = "73393DYJ1400TXQ6C8PKF066|5WVS8PCO0JYLACZF3684STPTFPMRC4PO";
            //string postUrl = " https://api.kuaizhifu.xyz/gateway/trade/v1/remit";
            string postUrl = " https://api.kuaizhifu.xyz/gateway/mer/v1/withdrawals";

            var stf = new Thirdpay.方舟.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"

            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [新欧付] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment新高付(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string _merchantID = "131455";
            string _merhcnatKey = "BB2B4B21-1D3B-E540-251C-CF29515F3528|1C5826E4-D4E7-69AE-6DED-CC52FE1250C3";

            string postUrl = "https://dxk.zz-dt.com/paycenter/withdraw/withdraw";
            postUrl = "https://pay.x-pay.in/paycenter/withdraw/withdraw";

            string returnUrl = "https://www.awcwin8.com";


            var stf = new Thirdpay.新支付.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [新高付] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment新支付(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10179/v2";
            string _merchantID = "110126";
            string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string postUrl = "https://dxk.zz-dt.com/paycenter/withdraw/withdraw";
            postUrl = "https://pay.x-pay.in/paycenter/withdraw/withdraw";

            string returnUrl = "https://www.awcwin8.com";


            var stf = new Thirdpay.新支付.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [新支付] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
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

        //private bool Payment天空(ofAffWithdraw row)
        //{
        //    string _merchantID = "6593";
        //    string postUrl = "https://api.wonderfulsky.com.cn/service";
        //    string _merhcnatKey =
        //        "MIIJRAIBADANBgkqhkiG9w0BAQEFAASCCS4wggkqAgEAAoICAQC47a/IROe9zYQx" +
        //        "zhcpzuhlvd93l+7jVW+r8V2mCqeeZaQyKKUWNptmm1K4Om3O4ORiEoOZfSZyEfqY" +
        //        "BQBlgKiUafKK95zbKU6zL6dL7vpVd8fWoC+66tXteqir8Ws62jjSn9Lo1KuVBzE5" +
        //        "kKFditQFMWlk3Zl8RkdKePvO1tgtgkknbXA8mXUHw6KZXUmk6xfl8Wa7pffiP5Uz" +
        //        "DM4Mb4SRHIicopA5pgVOQrEDNS+6MKlEiet65i3Gubvc8NATK0p1g/KcklauLByB" +
        //        "iA4xX8WbjoFhC7xTc4r2hOKCwS5YMvgjIQsESRHrW1mI1Nu3/DsrIcOsukaGTPyA" +
        //        "KdEED42HQFMS5OCkeWAb90m8LD7NvEMEqXcJE6PgxP9MozyGYKtmUNc1h9aMTT/O" +
        //        "iFmzWkkGRCUzluHuAn6cOHd0iZU8XZVeOiANwpa8hfIgCPZnISRWICLbe/LTakUU" +
        //        "9pMbFei1/sL8FgQXaJVh4+ni8w2QLdGlIhW4pNte1pxceQLUmHw+wTt/6E0KIWp3" +
        //        "p+zju2BdUVOHQs8sgyxs6Ynpq80hpUnjvBYPH6UFBWHGtt7mJPReAK2yOMJGIEjA" +
        //        "N8ieqFhST8hstfYIZOSkDVf8o9Ek1ZT0ujhu/iSnrAIJ2vUdK2QEBPFQvVKdVSqr" +
        //        "8n6K9BfK0ijhll7TuPpqRMH4ZZ1oTwIDAQABAoICAB6cOVyNC07/lcIouuzQsuck" +
        //        "ZrkFDSXUHof3i+jQGq69ZFBpKepjX5W8YBTelw6Ha0ZQijf93z9CephvO4GRqlXm" +
        //        "s58NHYmpbL18RdTr2cQL76g3G69077Q/eWv8qiMzu/QhQGgJy9z/M8azigJR2IaD" +
        //        "h06K370SZPqOzB+QsHfX+QPtuYZOYHDIkUGRj38c6vfE3Zxx/HYW0DuDAxhL3UWn" +
        //        "03UZ84WXVYl+wI4nX58SLvKJYdEWCL28QLxxn/+MSubo0F/aWT67TnyiaLcDg1Ic" +
        //        "ZI4Lw0CM/5OOq2+5SlKPHUOzVH+BGNY8aLWdhkEOHz0keZ89ti7Ez4bnZRGnAWX5" +
        //        "wLBq7wJ44dZXI1af8w9iIkUcXs8wrXWku1TbVjG4FXmMPxYH0apd6pSRry8C27rK" +
        //        "fRdNnQTUPhaqnYSEHyDcqYwBYUP+QfugYHFTyy+YF+4VwalVOZreDOVjW2GpPBev" +
        //        "wKeeAidM6yWQSNXRBzoIJELMQjyk2zzsmCvrR9Uu1ZTUM71tm6TleRsMeNpTfhdS" +
        //        "G1bMJ73OijwjxJ3sDv6z/0p2EHfQK8wV1ePthgjhCOtNQM75BJGOclaN2V0yrtQq" +
        //        "o6d7cws8exAkjioDIZey4ckeLfbyIW7rsLgPRoukizr/VtLgYoxUL5px3C8dc/xo" +
        //        "qFoBDVCNeZ6/NdFZdRABAoIBAQDkvO2HT/f/V5kJlmps3WCWWgRbb7NybknjiT7q" +
        //        "QzmzrRkX3G1eMZphuQDXDZkgZmlpXnrqcJR7KhuV5XuLDsrUYbkncaTCWaVjU99G" +
        //        "HlZ6RKYAUxq9a5uYnvsaIq1j74+L7xKGylqaOkOUWs4m2o3JRLqyB6ZJM+Zht5CX" +
        //        "c+QtSTNdogk/bThy3YsfV17UKR382zbhLKAk5fJUBZrfrphDdQelf7VkaS5RmQ91" +
        //        "pFhsSTDXyE9+6ECwuMLxws2aVeuXPkKlq9CEPX9aw0l1vShCKflHjqdN/2hQpzaX" +
        //        "W16QHOYUhIF3XvRN9uCVLwvAId2+yt5x2kNjzj68wCfHIV+PAoIBAQDO+BO49Anu" +
        //        "Of4vjHs0M8E54gikx6DHzTpSVbuhTZHQUPQSo+JuLrXk/a7RrkHS7f7hAlY/+ZlP" +
        //        "YRUyVHKJY5ajYwP3LqkzctnpUZBUrC+JPrFPxTO+dXg0ZFIdXW5XaCMl+2Jqv0al" +
        //        "JE+BD1CZrFRXwMOI20QIBdLkKPAMIvCjlSEO0a9D2JX/ZmX7p3uMxnkYXy76NlM0" +
        //        "jTZJ+5VjmiEZYhyHOeSqiPQddcuQSSDPqJ6qf4celQ1LOQcWEmTIQCm09Pg+e+a1" +
        //        "dcKJFj56E1p86rL0AaKu0KT+enQdHsXlf8PTTu2v7JBTkdgq75F6Hhgy+MH6DlfR" +
        //        "iMQ+RWOD5wtBAoIBAQCeElnryr3NfdHKqGH/vbUD8k1Uiwrtp330emI6Lh7wJk1T" +
        //        "CgeispDHu7vy4fjSRTPPl3VGEmrp+p15p08LODu/LZFwyr5beOwsikegA1Mrx8kY" +
        //        "LFYsVPMAM82/r/s3NtRyldKtx6V5M46ENYSFoT+euZpzcYneyLkLCnonz/T9vrp6" +
        //        "mIHfjTEXYDbGf/COmaOZnDx+vAInvakLm5v9vICJDZUtL+ZA79cMmMPwPpTEsIfK" +
        //        "dylIwGQEPIqr3D2NzJa0vFf+eY/t8znRlWdssD3kC3Gzm8fOxRRnuEanroLZAL+X" +
        //        "qkJ2Je6a+mMVdZ+fNUgyk6vJ7lXn4R3F/tz+ZffNAoIBAQCzJ/cakNTPzI+FZVt4" +
        //        "0pwemNTWnp9ElRg5EVF7zYO9CXZddFObAQAfGqFrjCYaij2pVLaIZm0ZU3ZfvM3w" +
        //        "s64//IQAQfnhOTf8SchgIzR63zPpU1D5OwI0h+q01J2hN9nFFRgaQzzJ0uanVzRz" +
        //        "8cKIu6oHG5aGQ1RMgEtalhwr3flhtIy8dA1TqbX54v6F24ATYsFtgf0AboRiAk9M" +
        //        "iqzAJjd/rtOL80UtAzyUx1iJqPK/r+isxmVdih3iM0XeOfxgUfVZUTNiPG+K7yBh" +
        //        "18LqgtcpdQj3vlA0oJdGZc70XGaM0bPaJzK/xG2UrRAG32ofJUn0RNor3PHS3YnJ" +
        //        "MgSBAoIBAQCc3AGKCc+1M2KtvsRVqecCJTy7xAArdB0TaxvxnWF4TXlxfrECvQMG" +
        //        "W1tfCIK2a5tdGlP47Wvj1w55t3tp2sHyUlD2OGkRyYIdeeAg8BDXWVJ0pmf/Lzih" +
        //        "dM1YAYijSvLdeyuxmHLp3Zvjq5QxqxZrSpMTSfbnP0xm5D4gMY2TZ8iA3Xj8oGeI" +
        //        "N2RaKp+nByBlGpdx1lHaDRrGShpJ+IvqPMAy84QiVffbmTq5oMz5hAKjOCHjBHpS" +
        //        "nD4BuZwBjRkzzoX+K3u+7/mS1efuWWkZd9P9tsVP4JqvxaT2hrZp0y90PWTPWgcM" +
        //        "9FpKNHyPXw6VmWqvRkqnZXKD7C8eHwEL|07192555F99A562538D6063E831D022F";



        //    //string postUrl = "https://api.youxintrade.com//PaymentGetway/SinglePay";
        //    //string _merchantID = "AWC001";
        //    //string _merhcnatKey = "1beaff1e5404f5b73dcbfebcb1703c60";
        //    string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10234/v2";

        //    string returnUrl = "https://www.awcwin8.com";
        //    var stf = new Thirdpay.天空.SPayment(_merchantID, _merhcnatKey, callbackUrl, postUrl)
        //    {
        //        account = row.account,
        //        returnUrl = returnUrl,
        //        clientIPAddress = "168.95.1.1"
        //    };

        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
        //        return false;

        //    try
        //    {
        //        string orderid = stf.GenerateOrderID;
        //        float amountadjust = (float)row.amount;
        //        var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
        //        _logger.LogInformation("SendPayment - [天空] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
        //        return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        string response = stf.paymentResponse ?? "noresponse";
        //        _logger.LogError("SendPayment - [天空] rowid:{1}, exception:{0}, paymentResponse:{1}", ex.Message, row.id, response);
        //        return false;
        //    }
        //}

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
            string _merhcnatKey = "10DBE1D8A94C0D63AD08367AF23D7FE0";
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

        private bool PaymentGPVIP(ofAffWithdraw row)
        {
            string _merchantID = "C1000146";
            string _merhcnatKey = "Fn87r6ar4fKN2UqlCWzF7bBsOjNX6wEa";
            //string _merhcnatKey = "vwSkx2HuDvhdRXg0fqh35sT9QhOhWo0Y";
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
                _logger.LogInformation("SendPayment - [GPVIP] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);
            }
            catch (Exception ex)
            {
                string response = stf.paymentResponse ?? "noresponse";
                _logger.LogError("SendPayment - [GPVIP] rowid:{1}, exception:{0}, paymentResponse:{1}", ex.Message, row.id, response);
                return false;
            }
        }

        private bool PaymentGPay(ofAffWithdraw row)
        {
            string _merchantID = "C1000143";
            string _merhcnatKey = "qYp3g8yQaqkPoiL5oSUrPeQT3yEXD2pw";
            //string _merchantID = "C1000148";
            //string _merhcnatKey = "P4nlvoBuqjAtbyZyiRaDmpDUw1NH5tAw";
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


        private bool Payment易新付(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string _merchantID = "";
            string _merhcnatKey = "";

            string postUrl = "";
            string returnUrl = "https://www.awcwin8.com";


            postUrl = "https://pay.x-pay.in/paycenter/withdraw/withdraw";
            _merchantID = "139378";
            _merhcnatKey = "A446E076-F435-CBA9-7129-ECA7AEBD3BA7|306A39E5-F6E1-0F6B-19FF-E6F7642F5043";


            var stf = new Thirdpay.新支付.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [新高付] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }



        private bool Payment新爱付(ofAffWithdraw row)
        {
            string _merchantID = "20210113170630191";
            string _merhcnatKey = "1d8e3cf6d1fe4aecbc12d4a8c6e2f852";
            string postUrl = "http://zpay.website/pay/order/acp";
            postUrl = "http://47.243.40.37/service/carry";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.爱爱付.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = row.requestip,
            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [新爱付] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment平安(ofAffWithdraw row)
        {
            string _merchantID = "OKSH407710";
            string _merhcnatKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDwDExLK4fXdlAE3exGSNYJs2MulTOr0FvKQh/OpzrBzxjHdqjWqsuu0d5tTPnrQVqfq7+5ojv0SVWToQMyLrbT4NYSbsu6yzHn9YUJ5Zmd5E6VDjk6cZ37tgGG+abXp98XHO+nJM2aGG54d4rxAtBJeM1btPyxY/loC+GpYuY8OQIDAQAB";
            string postUrl = "http://zpay.website/pay/order/acp";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.平安.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = row.requestip,
            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [平安] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }




        private bool Payment金星(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string _merchantID = "wxc888";
            string _merhcnatKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDVtl4Xb/u3U/ElrQQcghhzr8cfFOAcOsSCOblHeMUTe9vukBucmAW56v5ktaNcs+KzU48c3lZlfuvlqvCMC+Zixn/OE4SZd2A0goLFXFnw9vX3J2EsrUccM54G70DLB9VZLIdCqchkFPU0XUyXc2YopxNC9IY/5b89euQIptGUVQIDAQAB|C3AD125D8DDA4030A5A71D1672B79AC2";

            string postUrl = "";
            string returnUrl = "https://www.awcwin8.com";


            postUrl = "http://bestpays456.com:26358/deal/wit";
            //_merchantID = "139378";
            //_merhcnatKey = "A446E076-F435-CBA9-7129-ECA7AEBD3BA7|306A39E5-F6E1-0F6B-19FF-E6F7642F5043";


            var stf = new Thirdpay.金星.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [金星] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment火腾(ofAffWithdraw row)
        {
            string returnUrl = "https://www.awcwin8.com";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string _merchantID = "NC8QwkIG";
            string _merhcnatKey = "iZhoHEdmB7uFXYG9";

            string postUrl = "https://ltezp.xyz/payfor/trans";

            var stf = new Thirdpay.龙腾.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"

            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [普龙] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment普龙(ofAffWithdraw row)
        {
            string returnUrl = "https://www.awcwin8.com";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string _merchantID = "zPB7nu4R";
            string _merhcnatKey = "9MYUTPRstG1hJwiD";

            string postUrl = "https://ltezp.xyz/payfor/trans";

            var stf = new Thirdpay.龙腾.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"

            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [普龙] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment高火腾(ofAffWithdraw row)
        {
            string returnUrl = "https://www.awcwin8.com";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string _merchantID = "gCnUBXyV";
            string _merhcnatKey = "GRkM4VFpBUPuwrdz";

            string postUrl = "https://ltezp.xyz/payfor/trans";

            var stf = new Thirdpay.龙腾.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"

            };


            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [高火腾] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment奇点(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string _merchantID = "a5tkfii1";
            string _merhcnatKey = "ff4a2892833b6da32047157e320f15ad";

            string postUrl = "";
            postUrl = "https://pmapi.fairbetwin.com/v4/merchant/withdraw";

            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.奇点.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [奇点] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }



        private bool Payment顺兴(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string _merchantID = "";
            string _merhcnatKey = "";
            string postUrl = "http://api.youxintrade.com/PaymentGetway/SinglePay";

            _merchantID = "AWS001";
            _merhcnatKey = "5798c654e8c352722b98b2a2fc3a3887";
            postUrl = "http://api.sxtrade8.com/PaymentGetway/SinglePay";



            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.阳光.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [顺兴] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment飞杨(ofAffWithdraw row)
        {


            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string _merchantID = "";
            string _merhcnatKey = "";
            string postUrl = "http://api.youxintrade.com/PaymentGetway/SinglePay";

            _merchantID = "AWS001";
            _merhcnatKey = "bf188eab8fd7cd27525336b5ecd4b7bf";
            postUrl = "http://api.feiyangtrade8.com/PaymentGetway/SinglePay";



            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.阳光.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [顺兴] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment快乐(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            //string _merchantID = "OKSH989725";
            //string _merhcnatKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCOzKjv6lvXskXzZ7djiFjnFLwK3ir+rzqQjyBQuFEX3GHLlUbIe8N48GJbypWdo4RFGowralzB5kJaaGNnJGSCKlTQoT0ADKYVfnWYMYFSPwFbEYPk6y3yHhgWgqOh+8VKTmabxUS72l4biVaaGXLY1aOC+36iMJIu7A4R/hL5hQIDAQAB";
            string _merchantID = "OKSH739988";
            string _merhcnatKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCDIwvfedOVV1Gsi3GtCPQyyd9s7K76M8wC0LMPHT0W7dw7YvzGLz/08Yi2338YrKGuKHbcX9jRq1UjMJyuew+VKOUdNFWGlzVQv+eSGBIclrYI3ih2sSbGe+ukiMR8So1jukSO1BrIUSXxwRJxn+p0WQIwnQvQJ7ex352OzH4ElwIDAQAB";

            string postUrl = "";
            //postUrl = "http://zpay.website/pay/order/acp";
            postUrl = "https://a.qpay.cloud/pay/order/acp";


            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.幸运.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [幸运] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment汇款(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string _merchantID = "CID11201";
            string _merhcnatKey = "1esrTkGWh1jwuo1";
            string postUrl = "https://web.jf3092.com/paygate/withdraw.aspx";

            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.汇款.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [汇款] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }
        private bool Payment鑫鑫(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string _merchantID = "OKSH989725";
            string _merhcnatKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCOzKjv6lvXskXzZ7djiFjnFLwK3ir+rzqQjyBQuFEX3GHLlUbIe8N48GJbypWdo4RFGowralzB5kJaaGNnJGSCKlTQoT0ADKYVfnWYMYFSPwFbEYPk6y3yHhgWgqOh+8VKTmabxUS72l4biVaaGXLY1aOC+36iMJIu7A4R/hL5hQIDAQAB";


            _merchantID = "OKSH122291";
            _merhcnatKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCrdWCeHuzfrLaPF9uAohQYjDE3Vky90B/RSS6YGU8quw8dzk8oJq3MD0Dlc0fqF3ImoRejr0yqLLkYhfDx7rlXKJPcF1hVXs6zG8Kb7A8eZVwJCvqk6PQHgeqB2MQCmrPs1351dadyVtBx12uY8TX+OkVdZXRbxyJC9UAfxsLbHQIDAQAB";

            _merchantID = "OKSH586874";
            //_merhcnatKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCOzKjv6lvXskXzZ7djiFjnFLwK3ir+rzqQjyBQuFEX3GHLlUbIe8N48GJbypWdo4RFGowralzB5kJaaGNnJGSCKlTQoT0ADKYVfnWYMYFSPwFbEYPk6y3yHhgWgqOh+8VKTmabxUS72l4biVaaGXLY1aOC+36iMJIu7A4R/hL5hQIDAQAB";
            _merhcnatKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCBoCUAsk1hTWSPADj7UtupMx0F/oRcLY3GGa/1C57a7+4pdKekbP7UJhhwDtEMX72V9YOMvIbZKbR+919Dl+45JnMIDMv9tNsNLLxN9SSB5/RvXp3QYTuDGrFsBf5VGcdp+fNU6Uvmcxokw7E6r5Y+EXbS9DyBWKh6WOv6k8l8+wIDAQAB";


            string postUrl = "";

            postUrl = "https://a.qpay.cloud/pay/order/acp";

            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.幸运.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [鑫鑫] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment亿贝(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string _merchantID = "wxc66";
            string _merhcnatKey = "9538a5d642464715954c5df35a0b919e";
            //string postUrl = "https://zeocje.iun83pwe.club/app/DFPayFun";
            string postUrl = "https://api.iun83pwe.club/app/DFPayFun";

            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.亿贝.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [亿贝] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment幸福(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string _merchantID = "WXC001";
            string _merhcnatKey = "c51ce0249d1115bfabb6ebef96fa1b99";

            string postUrl = "http://api.youxintrade.com/PaymentGetway/SinglePay";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.阳光.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [幸福] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }
        private bool Payment幸运(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string _merchantID = "OKSH989725";
            string _merhcnatKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCOzKjv6lvXskXzZ7djiFjnFLwK3ir+rzqQjyBQuFEX3GHLlUbIe8N48GJbypWdo4RFGowralzB5kJaaGNnJGSCKlTQoT0ADKYVfnWYMYFSPwFbEYPk6y3yHhgWgqOh+8VKTmabxUS72l4biVaaGXLY1aOC+36iMJIu7A4R/hL5hQIDAQAB";

            string postUrl = "";
            postUrl = "http://zpay.website/pay/order/acp";

            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.幸运.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [幸运] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment壁虎(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string _merchantID = "WXC001";
            string _merhcnatKey = "e32eb6c6210bc7647cb56a89500c2922";

            string postUrl = "http://api.bifutrade.com/PaymentGetway/SinglePay";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.阳光.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [壁虎] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment阳光(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string _merchantID = "WXC001";
            string _merhcnatKey = "f5fd1cec891e73da74d2b8676c4ea515";

            string postUrl = "http://api.ygtrade8.com/PaymentGetway/SinglePay";
            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.阳光.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [阳光] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool PaymentMPAY(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            //string _merchantID = "110126";
            //string _merhcnatKey = "B44C47A0-5623-93E5-807A-2EA5E73E00D9|56B99F5A-37FD-8C30-F580-5DF44F48ACAA";
            string _merchantID = "131455";
            string _merhcnatKey = "BB2B4B21-1D3B-E540-251C-CF29515F3528|1C5826E4-D4E7-69AE-6DED-CC52FE1250C3";

            string postUrl = "https://dxk.zz-dt.com/paycenter/withdraw/withdraw";
            postUrl = "https://pay.x-pay.in/paycenter/withdraw/withdraw";
            string returnUrl = "https://www.awcwin8.com";

            _merchantID = "615226";
            _merhcnatKey = "C0CF16EE-FAEA-96DF-2AB4-2A548864BADD|3F609C87-95DF-51B7-24E9-920C59D91AC7";
            postUrl = "https://hyi.kangyuebt.com/paycenter/withdraw/withdraw";

            var stf = new Thirdpay.新支付.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [MPAY] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment天地(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string returnUrl = "https://www.awcwin8.com";

            string postUrl = "http://payapi.spgp6vwn.xdpay.cc:6080/api/obpay/transfer";
            string _merchantID = "1008";
            string _merhcnatKey = "4NQRYVCZFOKDUI4AJSEJE1QGUNYWSCNDXFBM8VZ0EWWYKSURURJTRVYIAV9NJI3FJFKMJOGDRMZI68IL1VEMF96XD45WJPYA5YAASDPFVWGMXRRW4FXDXRYUWJ0JB342";

            var stf = new Thirdpay.天地.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [天地] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }


        private bool Payment豪杰75(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string returnUrl = "https://www.awcwin8.com";

            string postUrl = "http://pay.heropays.com/api/trans/create_order";
            string _merchantID = "39803";
            string _merhcnatKey = "WFGFC0QHYJAUOZBNNVBQL5DRNSDEOPKJIA2GNGGF5V7SAAFKWX5BZP9NYRYOLDSCYAYKZ5TZGKAT9CFSBF5HCNJIGCWIMB8ILYOHF5JOZFK63GF3YL12PZNXATAHATXY|4b40dee8055e49b881bf46dc02b1d96b";

            // new account

            _merchantID = "40108";
            _merhcnatKey = "VBPM47BMP3UOQQ8M1UQLTWRWUEAHMYVPEFZBRFNFCUHFVZMUJOUB4CZYQVAB9VH11KDXMGCOZWJ9AXAZNSJSEPUMUPY6JT8HAQY6DD8KI3TJPTK4LRWPYATN3AMMRDVP";

            _merchantID = "40075";
            _merhcnatKey = "BQ1OVCUOFZ1BXUM5JIES2NDVQ1DX7X0QIOWW0608536AMUQL0XESRNXPI7WK9H8W0DYN6JFY6JNK2VXBMVWXBTWQ4L9N654QXCMHP7E4ERWJTDRL5TV7ZWP39FQCATIT|4b40dee8055e49b881bf46dc02b1d96b|MXBSsZc3Dzns";
            postUrl = "https://pay.heropays.com/api/trans/create_order";
            var stf = new Thirdpay.豪杰.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [豪杰] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }
        private bool Payment豪杰18(ofAffWithdraw row)
        {
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string returnUrl = "https://www.awcwin8.com";

            string postUrl = "http://pay.heropays.com/api/trans/create_order";
            string _merchantID = "39803";
            string _merhcnatKey = "WFGFC0QHYJAUOZBNNVBQL5DRNSDEOPKJIA2GNGGF5V7SAAFKWX5BZP9NYRYOLDSCYAYKZ5TZGKAT9CFSBF5HCNJIGCWIMB8ILYOHF5JOZFK63GF3YL12PZNXATAHATXY";

            // new account
            _merchantID = "40075";
            _merhcnatKey = "BQ1OVCUOFZ1BXUM5JIES2NDVQ1DX7X0QIOWW0608536AMUQL0XESRNXPI7WK9H8W0DYN6JFY6JNK2VXBMVWXBTWQ4L9N654QXCMHP7E4ERWJTDRL5TV7ZWP39FQCATIT";
            postUrl = "https://pay.heropays.com/api/trans/create_order";

            _merchantID = "40108";
            _merhcnatKey = "VBPM47BMP3UOQQ8M1UQLTWRWUEAHMYVPEFZBRFNFCUHFVZMUJOUB4CZYQVAB9VH11KDXMGCOZWJ9AXAZNSJSEPUMUPY6JT8HAQY6DD8KI3TJPTK4LRWPYATN3AMMRDVP|adff6cf12c9b4241b20614a5356f2a62|udDTBS25TxjH";

            var stf = new Thirdpay.豪杰.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [豪杰] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool Payment天空49(ofAffWithdraw row)
        {
            string _merchantID = "6749";
            string _merhcnatKey = "" +
"MIIJRAIBADANBgkqhkiG9w0BAQEFAASCCS4wggkqAgEAAoICAQDMUcqZJnF69dWi" +
"m5XZoq+TGORlT6kwO40pDdwlaGpEIbCBuFQhlEisTvKAoB/0tezvbxMkGfpA48xa" +
"5ubGNfCg3fnvXgTUUxCaxskQH7ntn0MiXTylQ24K5VCHdY2ELts6GaeakOien0y8" +
"aINKOAs8LDWCVsNBEihlJ4F6rGXqnrnfxHAXSHL3x7VshBoXtFBDEVtZ2C2gLG5G" +
"nngjPk/4khgHlMMbl+Vhi+DDSLotI7PTyYRwgrObqWb3dr/eVDSb4Hv6z3m69p3B" +
"cX1wh2BTE0hp/yXmqHXSB+HYoADdm4yEteO/V/1bLVN+Pcb/NRbIQhOivWQm1H9g" +
"sJzTzftSySbb0hU1Md5Ac/mMZUbyphgogQxZbWO4buQl9kvlvdhX6ZBsxIrxnuOI" +
"0YNszXR1BYnKMGksXw9gHCELzO7zBIbFiGnKQShqQcY6XxFd4vH7B+iZXD4voAY6" +
"YgHYoqsKpltnQngI0ZbYx74xb0zEROZORJhcEAilTvO6WFiBs75QSfk5gjvxsl0L" +
"eigSsnOAJWmt97rLqcgfGbQhMVnF9RGUkO0NFwoDKDGjx2jQVSpGMbi9/P/qYw1p" +
"aqSsxzn8zuUkAGr7WLPRpKLD2AxktFuduMwD01quQDClZi/vKhNWERNFFapwaJTe" +
"xXXLhHtpklBupWsvdT/3TsUNAkBX8QIDAQABAoICAQC0YeVurkzzlIPmiluSh5lg" +
"UVKayuiAp6KpQw4/jF59Bg0xiQ3lk2dRkGVx/2wW/GJQxIxui+yvSy3Idx/4j/h/" +
"3Vlh5Wx4E1mOe79Y19mrGrcHaK847D4Dqo4c+UBWGZP8SFf7EsH8AIGbuN1oMfBh" +
"UFpxVq4NDb3SCD28kuVrbHSHRIL9Wz382TintDzg/pT5kSJIQJ2PUoqNGCzSWJn7" +
"xucmEkTqYIDnTLnDTrrnTSTe7oNZXVq6K6W3BgTAgTDtCRQLyw4abM270blAFpSI" +
"43ZD9jl82O8j3Uebmy8IduJo67hE3qbTz0FuK5Y7C6Cwbf2t6Gy6B1LphyL9r1WY" +
"HgeMAEFwui2IC7jpIJB2AQ2tvsaubRCCOOKJm9vCsvQ7sFekCtS8a/hxtbPLZylo" +
"HBT7yUZXYaprx/ETxh0c8F5RYfvnRV6YcJ04fxnF/DTxMK73JeN1FVS+S8rt6blr" +
"I2CLBqZh4f4JcNsKqXoWPAPu6HCSRs44tyqbqKuecP/9AypfgoIKjxgYAlRJyUIN" +
"xmqnZ7pgF5adsrAbmVxf/iARUceo18iANPHfk5T/ZvGGSVOVKRL1YOyehqgvm9r8" +
"94Tb2YAybmhGoHMU8p0SDiVc3Jdx5Bn1VqbfW0bIdbHNVbTMiCCACao/8uWDBSqD" +
"xFI/MnbC/DAcyOZ1WAYsAQKCAQEA6c5zaXKpxKVs3YTHPcFNCTCYbvWUbEg2Rpba" +
"qPsawCNfiTAMc7lFETwkUW51esNccRyeYJvaUnj0GqjBpLuYl3R2u4HKQtz4cu/V" +
"X8S0jL14V+Tk0R2kYV3lcl9PQDArUw6k9SNa0d6guQGgb14CS5Fb4Oy2u3tB8EZ0" +
"/Sql4K6dij0ulHjQ/cQXHLqZ+1EZaW0vvbMYkbvw9tL5uGZ0yAVquNFQUipuImv0" +
"co5pWo7uW5fuy+4hAp1kGlSKedhY4Jtbi2JU/ObsGG/edaG40YUW+iNgpedAKkgl" +
"i2NPOU4ZOgJrOy0R+RqADc8k6esrUij9WRHUgLEpeH2PEVbAwQKCAQEA37bNEW8Y" +
"ltMqxbgoHJvHyk+IH0Wvw2BdzbgdJdVt+rviCMzGp0e3KQgH09YdayfsyI99ECFH" +
"IR0raP8BU/7LNKhJsXI8K6EVIoU+TZu9tj2DQEocAx9l6U3orsP98lDxB2cMQe+N" +
"42cg2w9lZwDuPJfUIxVOKRr+Weoiw4kLMgPBnoNo7uoiYJlUhrdZBRh8WcAknDP0" +
"aMd5fCeNSuQZeR01kwcn8oY7mbYJkXC8OPm/qwHQLy3+3HZOWTmv5GAXsUZCbDYG" +
"Hc8vG2RMfbYuqDg/NXcjSbcwpwYNC7rTcvGn0+/kN1IwYS01IoPcZhupOB+augC5" +
"pBDlD7n+aP8zMQKCAQEA32dKNf6/IswiasP9kQ+L6XUKojN7379cRTTNuFs/I0pp" +
"de94Zncn/KDn3vtZu7TCNr5uwscI5Hb4YCTR6vLpigZZkS8iMaNsrn5l9r1A+53r" +
"SKjIztuw/uw2WoOheSbNru42RpnCpPhBwKVCREeij8xT62aj8B6FUe4wyNsgZsDS" +
"fY2aXXnX0DK3FtdiXJYQXwFadbuUhWP8igvrWvxjJ+CtKe2u/uPWGoPmrjNPjZ03" +
"OkPBhBCEcDgtt08ChgxoWC16WoPZy1FnVK5H9URTEbhGzTpSlhOR5xTK8xYwVARA" +
"7IH1LjURkGF27Dr6/Zyc50ONWLdFwrYFd0EaHCo1wQKCAQAl4/Q/q61ifkmUS2if" +
"z9K69VV81nS6hqvcl/531fpWqVPTYbuTwZXIIVASFQXOjGiSX5szDC/b7/DWS7qR" +
"22nXdVll8I84ZIQL4L14r5QmNazxSpwL+1lrdoKedxuf2w23z0BIOqaxiVoLQERQ" +
"edvf72dCue0PET9bQ0i4DZIT2ocErgUr0gQYkF+8oXk2X7tQCDcrLbGLvZ1Z5ekM" +
"ci3OUaAtUtz1Or8FfcuJEqsOi15nPFtiYkv0UyVZ/2dHKu9YWyoM4emPSvQTk51Q" +
"/XZRj5VFHPShG6Hl6CR1hCMkKMHgJ5VjuUvwaZPCUMsXkD/M/a1bK5xFqYbibwIk" +
"DnVBAoIBAQCGJ3KIFqk94yuKSv7jbYf/xmh3hYKp2go0F8Mz8koqBlzkcvQIgmck" +
"ZfwHbBkx9sxEwLfSQYaveIWecsDgm9+U17u8BMJQeRKxaCiEepMW6fAvVuxa9aO5" +
"tG7qt41Zjn9mDboKhTeRH3Z8956Y0l2BNC0M9iJmoFSF5W96KQFNAoXKbIqDKDfG" +
"vxprfr0EsQGTHc9KVPPUsz5judfyTD85ULAC/QFcn6RMDG1WGrEPngrCoTJU9P3u" +
"WszdUClBkc0tjAxkKmm9jpqJ1TzrqTI61am97+MBufWO9tjd605HBGTJcWMSUjuA" +
"Qc2s+zR2+rFvJslllk9l12nQeqih15lt|ba26476c49cf55187e54737c79bb2449";
            string postUrl = "https://service.cornerstone-pay.com";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/10601/v2";
            string returnUrl = "https://www.awcwin8.com";


            var stf = new Thirdpay.天空.S(_merchantID, _merhcnatKey, callbackUrl, postUrl);
            stf.sendpayment_channel = "apipayout";
            stf.account = row.account;

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                //Random rand = new Random();
                //float additional = rand.Next(1, 5);
                //additional = additional / 100;
                //amountadjust = amountadjust + additional;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [天空] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                return false;
            }
        }



        private bool Payment新天空(ofAffWithdraw row)
        {

            string _merchantID = "6803";
            string postUrl = "https://service.cornerstone-pay.com";
            string _merhcnatKey = "" +
                "0ae72c7c11b5125c6c472f306b09b8a9|" +
                "MIIJQwIBADANBgkqhkiG9w0BAQEFAASCCS0wggkpAgEAAoICAQDPn/EE6nJXd95n" +
                "Z6N+VkPnVMtV6dVRCD78wHJToAGTgP3UWGwglK6SVYzvdrrVkWZ3QWLBkz1XLaiZ" +
                "6RnkJy9ZUUBWHXLfdB11jdcQXNnstREczusbKpWjWWQ0oSjbpnNdiY9AttNX6Srl" +
                "ace5hl/UzEiAz+CSCvX0+AFWOrZbthyXUZQhC4snSLrlu26bwNEJAKZOkK3Grj7h" +
                "Jb03FIg1IJ/G3f/u9k2T3+zyzA0N7La+uSX5sG+KUN+TJFiuab2ILt7DHTY6K6pP" +
                "huiA/tQLY0rQNfWhw4wpGZMJ6GfD3xWyGctV4NSO09sNPYicbxtsF9lUDVrPCtiB" +
                "hJWTTOiiDMB1ZiN0li6Y/M2a1NpzXmj0/r0xl6eOoX/gkLncU3jRJSajWUMDCvtn" +
                "4HY3X3PqH8Bshitrar7KCwVtgBSPcK0mQBsi5o0zwpWsT7VXyGWwbw9TmFx2HrcZ" +
                "ZvUoW431HmHAz1YloFx1pTNgqnnip1bJdTVRTb2g2tG2O44kkdJa70EzMIRlP79g" +
                "N0bo10xZMarrODS4qHRvOGvzoYGyZvnUZ/M0amhUDhKQ6F+rOGaA36JuyDPeQA2l" +
                "MaL+6E1V4CEISxKf/v51Wk1C1GJqYxLdkfKp1t+ibB5ocrWplruU20dGqcC/Fy0r" +
                "8fGfld4YXLKVVIlCyM1YOZimHbAx9wIDAQABAoICAHPiXqESxj0hz4XBp9RlC8N7" +
                "ZX6PZBu1UYq7U1Rvk1GkBojVzYwl0IYmr+IcT83SOPHJEfjfWLZm6pr1KxlX8aqV" +
                "miY2i55C7gpl36rnWE8xBNsidsET9K9WvoJNjUOD1RJZLBa/tJh1sRaZAvv/D0/z" +
                "sWgMHtkEuyE+p2hcntnzfGPNCbOkAMgtZPvCsMrIRSQUM77YK9Yf/UKnUMQEdr3I" +
                "dsWFtUrIOKaXrmYaFBM4kwtCxfpmSutVlmRHPiQjKsFu/7DBhjnTk1yN/Qm6sdml" +
                "UuWBqJbgUUPDXXmDOR9vTWm50e8d7SXHA5WRScWekhyz9v5d8dhrwkoQGXg8zf5a" +
                "PDIeoju5plibZmLrsoNYQx87TMW1iVho/DQiIRAHUFYXVEF9f8ULgAf+8POvqIbZ" +
                "vnKhkj08UM5PyxYJaeS/0MiKtJOgUvmdBQJvIX9c1zUDf6nl97GY/0PFMt8+kdwY" +
                "AWuU+1YYEgnbfADgJJd8unEzHDNxMV2ZmKNgJEDrmdgRAITrN5lJIeCwUrus+KNE" +
                "HAPE16C9VIsG/tVpcL/rzwrxBiANQqisEzMDO6gYiAv27As2WunJ1xLOwhN1qmWF" +
                "56dhxQNTsjUgssUoR1B91LVDR3UQcgrsD13l/7PGv79aCEXds8B1Y41ep7lAGyO1" +
                "uFshYBxwY8sqaxCzoTkZAoIBAQD9rMgZaj4M6rEhemmhY5gXL1r4meYzVlqzKj7O" +
                "D4vKt6TfXRpLF612IAD821RimMe0/StQqAdRYgdNPB2xdCemwzDKIXdHx9UxC/F+" +
                "mOZaEmPPysi2t7hpAq4+voybJ/9ZexlmWnV5PDRk51D63aby59cIedp7eE9krIQM" +
                "0Hcl1o1w7P+/bXc8B3fntE1lV6RERjglIKJifvQ5qU1ib08mkpvuq92lfMq7wSiG" +
                "iA3wkBAOGig/Zn9nWc04CRH6d0aS/8gux6QyHEsiDOUNcbUzjITd31Yr86pRjmb4" +
                "Ezjamy+chTsQnWgWkkE1GAOJZ9Zp5AxwSl5jADPcQQg/XvpbAoIBAQDRhxvLRNFP" +
                "WgwcayVfdg0qXBZJjEI6hX8UQV7ILcRrqoJAjMZ0kfw42oJ2rEYP2wahgqJs/6ic" +
                "aruAKxjAxuV07UCth2jjw/aQJd8KVOzI+T/954drcqYCAswHHNf4ky4WYaudCC4T" +
                "Ld/kAhka4XyJgwMnhx486oGnXxaWvfMvM+0pufB+1o21l6jN+HbRGVeuLjPgeeXZ" +
                "wJjOIqjgO7uMYwCRoEwH3sdNdj9cVGc6v9bCdRdwNycQEasvehg1u7kQIuLWflv5" +
                "+KxqGIgVQDEhA/EcZ6SZfp8IULYPYnCwkXo1FOUV2RAnsSViVIMEHCVDK7GIt4jV" +
                "BUtmq3HJZGGVAoIBAQDeu4GqRw2ae8rxMTFkIjVdThgbJc3pD45tt88TULUWVi6u" +
                "bGn0uyJ0EF+tpMqwE6Nt98i7df2O6HquxvTvZaSXGNVS9RVtcjlvaiQtWwUO2wzG" +
                "Wy/mtuN1WIQz1U1RwknDgIFpzE1PGJyMaEpDKrE2oHe/GnbKaHWVWKBNPBERqRPh" +
                "rkZUB0Jk883FJRu7urshs2ICv2e1AfFgW5X1svQVUy8U3dSsRCkht9TkYruSmHtw" +
                "Vmk2WcHNthypcdVWTLorweg7otE5qnwRtq67IjbOvze+NCXOS5ElMNqaQQ5va0jf" +
                "rl+RFBRblSkp/B7G2+R4b2K4iaPcW0YtwvAB7Fv3AoIBAQC76ys4YZ5ARwi5rDc2" +
                "qNAh/7lVeWAk0HorFn4Fl1alIaX0cs3C2vOVohSEjvm/WrdVv64hiCIXN3q29vAa" +
                "ZciB2A+8Esg2DxliKzUhNQ2xKisEc9T2vzq4bv3CG1Jv9MDO/gZGroF0Xh6Bhogx" +
                "na5uPnxjxUGPbNuf2x6NUAt9eFrqo4NYnuE1zrFtGbymyM3kwDdyyqElbSEfL+K6" +
                "SZhpzplCArxuCpZBmR42ltSG0muwuAevcyert80Kpbx05MqT+FXt/iJLzYYFGb9j" +
                "H9FwRPLIgQQVToUflhPTNHzRP4iVdwoHUEi28KAQ0q82zm91EQkHD2S/eSIpEPba" +
                "oxcJAoIBACB1LrBlHyyc9/3SahNR2FC1GEBBqWEu3t7TGeAtLad77IE5jbaK61r0" +
                "gDa7h6fO2V9tFfrPbYdiIlUlkTahDy/mbp8aABOno8GG5rLhzZ1pYv332Cc9Xwix" +
                "YmG8O+h3ixRgxyF5i2/NxsERW9ZwUEJMkp0/2xbe++dPxKczLZB78e72/vXgjYmb" +
                "GAxJf7Ame7hS/ZxU7eXIkLJvEpUu3KTbnzcgl5FpFJVKNkaqLWfBqX+x4hamhxaQ" +
                "fjmHdxwr9yF/bjuPfMh6no+zB14bmnYDC6kRqUKfS7BZFakHz5ShvRpcaZnHhu9h" +
                "XS1JE8538CqCC4x/i6IA4zn7QeAifWY=";

            //string callbackUrl = "https://e24ey513fx-swp.azurewebsites.net/Payment/Callback/1/v2";
            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";

            string returnUrl = "https://www.awcwin8.com";

            var stf = new Thirdpay.新天空.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
            {
                account = row.account,
                returnUrl = returnUrl,
                clientIPAddress = "1.2.2.2"
            };

            if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            Thirdpay.Common.BANKS destBank;
            if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
                return false;

            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
                var payResult = stf.SendPayment(orderid, amountadjust, destBank, row.cardno, row.realname);
                _logger.LogInformation("SendPayment - [万象式] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [万象式] RowID:{1}, Exception:{0}", ex.Message, row.id);
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

