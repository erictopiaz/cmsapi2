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
using System.Net;
using System.Runtime.CompilerServices;
using PayLib.TYPay;
using LogLib.Helper;
using PayLib.AxPay;
using DBLibs.WXDapper;
using PayLib.Models;
using PayLib;
using System.Security.Policy;
using PayLib.MegaPay;
using static Thirdpay.Huobi.SDK.Model.Response.Wallet.GetWithdrawQuotaResponse.Quota;
using CMS.Core.Application.Account.Queries;
using Amazon.Runtime.Internal.Transform;
using StackExchange.Redis;
using PayLib.YiYuan;
using PayLib.SimplyPay;
using PayLib.HHPay;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class DirectPayRequestV2 : IRequest<ResponseModel<DirectPayRequestV2Return>>
    {
        public long Id { get; set; }
        public decimal fee { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }
        [JsonIgnore]
        public WriteToFile writeToFile { set; get; } = null;
    }

    public class DirectPayRequestV2Return
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class DirectPayRequestV2Validator : AbstractValidator<DirectPayRequestV2>
    {
        public DirectPayRequestV2Validator()
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
    public class DirectPayRequestV2Handler : BaseWithdraw<DirectPayRequestV2, ResponseModel<DirectPayRequestV2Return>>
    {

        private static object daifu_lock = new object();
        public DirectPayRequestV2Handler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<DirectPayRequestV2Return> Handle(DirectPayRequestV2 request)
        {
            if ((DateTime.Now - CachedData.QueryTimeDict["DirectPayRequest"]).TotalMilliseconds < 5000)
            {
                return new ResponseModel<DirectPayRequestV2Return>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "点击台快, 请慢慢来",
                    Data = null
                };
            }
            CachedData.QueryTimeDict["DirectPayRequest"] = DateTime.Now;


            var currencyCode = 0;
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var withdrawRow = connection.Get<ofWithdrawal>(request.Id);
                currencyCode = withdrawRow.currencycode;
            }

            if (currencyCode == 356)
            {
                lock (daifu_lock) {
                    return INR(request);
                }
                
            }
            else if (currencyCode == 156) {
                //继续向下执行
            }
            else {
                return new ResponseModel<DirectPayRequestV2Return>() {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "错误货币 : error currency code",
                    Data = null
                };
            }
            
            ofWithdrawal itemToProces;
            string targetPlatform = "";
            decimal feecent = 0;
            decimal feepertransaction = 0;



            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {

                var withdrawRow = connection.Get<ofWithdrawal>(request.Id);

                withdrawRow.cardbank = withdrawRow.cardbank == "上海浦东发展银行" ? "浦东发展银行" : withdrawRow.cardbank;
                withdrawRow.cardbank = withdrawRow.cardbank == "广发银行" ? "广东发展银行" : withdrawRow.cardbank;

                var directpay_details = connection.Get<ofSettings>("PAYEMENT_DIRECTPAY_V2");
                JObject PAYEMENT_DIRECTPAY_V2 = JObject.Parse(directpay_details.body);
                var payement_directpay_v2_regular_level = PAYEMENT_DIRECTPAY_V2["level_cat"]["regular_low"]["levels"].ToArray();
                var payement_directpay_v2_vip_level = PAYEMENT_DIRECTPAY_V2["level_cat"]["vip_low"]["levels"].ToArray();
                var payement_directpay_v2_vip3_level = PAYEMENT_DIRECTPAY_V2["level_cat"]["vip3_low"]["levels"].ToArray();

                itemToProces = withdrawRow;
                // Should be in paywait mode 
                if (withdrawRow.status != 1)
                    return new ResponseModel<DirectPayRequestV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "不可进行支付的状态",
                        Data = null
                    };

                // Check if the lockperson is the same as the cancelperson
                if (withdrawRow.checkperson.ToLower() != request.Admin.ToLower())
                    return new ResponseModel<DirectPayRequestV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "本人不是锁定的使用者",
                        Data = null
                    };




                // Check if already INSERTED in the paymentLog, So i'll just need to do a query instead
                var ofpaymentlogCount = connection.QueryFirstOrDefault<long?>("SELECT * FROM ofPaymentLog WITH(NOLOCK) WHERE Transactionid='" + withdrawRow.id.ToString() + "'");
                if (ofpaymentlogCount > 0)
                {
                    return new ResponseModel<DirectPayRequestV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "本申请已经代付完成",
                        Data = null
                    };
                }



                #region Extract the platform to be USED PER Group 普通 VIP　VIP３

                // PLATFORM
                var VIP3PaymentPlatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["vip3_low"]["payplatform"].ToString();
                var VIP3PaymentPlatform_top = PAYEMENT_DIRECTPAY_V2["level_cat"]["vip3_hi"]["payplatform"].ToString();
                var vipClientsPaymentPlatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["vip_low"]["payplatform"].ToString();
                var vipClientsPaymentPlatform_top = PAYEMENT_DIRECTPAY_V2["level_cat"]["vip_hi"]["payplatform"].ToString();
                var reguarClientsPaymentPlatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["regular_low"]["payplatform"].ToString();
                var regulartop_PaymentPlatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["regular_hi"]["payplatform"].ToString();
                var newuser_PaymentPlatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["newusr_low"]["payplatform"].ToString();
                var newusertop_PaymentPlatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["newusr_hi"]["payplatform"].ToString();

                // MAX AMOUNT
                var newusertop_maximumamount = PAYEMENT_DIRECTPAY_V2["level_cat"]["newusr_hi"]["maxamount"].ToString();
                var newuserlow_maximumamount = PAYEMENT_DIRECTPAY_V2["level_cat"]["newusr_low"]["maxamount"].ToString();
                var regularlow_maximumamount = PAYEMENT_DIRECTPAY_V2["level_cat"]["regular_low"]["maxamount"].ToString();
                var regularhi_maximumamount = PAYEMENT_DIRECTPAY_V2["level_cat"]["regular_hi"]["maxamount"].ToString();
                var vip3top_maximumamount = PAYEMENT_DIRECTPAY_V2["level_cat"]["vip3_hi"]["maxamount"].ToString();
                var vip3_maximumamount = PAYEMENT_DIRECTPAY_V2["level_cat"]["vip3_low"]["maxamount"].ToString();
                var viptop_maximumamount = PAYEMENT_DIRECTPAY_V2["level_cat"]["vip_hi"]["maxamount"].ToString();
                var vip_maximumamount = PAYEMENT_DIRECTPAY_V2["level_cat"]["vip_low"]["maxamount"].ToString();
                var default_usdt_maximumamount = PAYEMENT_DIRECTPAY_V2["level_cat"]["default_usdt"]["maxamount"].ToString();
                var default_usdt_payplatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["default_usdt"]["payplatform"].ToString();
                var usdt_low_maximumamount = PAYEMENT_DIRECTPAY_V2["level_cat"]["usdt_low"]["maxamount"].ToString();
                var usdt_low_payplatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["usdt_low"]["payplatform"].ToString();
                var usdt_hi_maximumamount = PAYEMENT_DIRECTPAY_V2["level_cat"]["usdt_hi"]["maxamount"].ToString();
                var usdt_hi_payplatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["usdt_hi"]["payplatform"].ToString();

                var cgp_payplatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["cgp"]["payplatform"].ToString();
                var gp_payplatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["gp"]["payplatform"].ToString();
                var gopay_platform = "GoPay";
                var eb_payplatform = "EB";
                var k豆_payplatform = "K豆";
                var wepay_payplatform = "WePay";

                var usdttrc_low_payplatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["usdttrc_low"]["payplatform"].ToString();
                var usdttrc_hi_payplatform = PAYEMENT_DIRECTPAY_V2["level_cat"]["usdttrc_hi"]["payplatform"].ToString();




                // 普通 替换
                if ((float)withdrawRow.amount > float.Parse(regularlow_maximumamount))
                    reguarClientsPaymentPlatform = regulartop_PaymentPlatform;

                // VIP 替换
                if ((float)withdrawRow.amount > float.Parse(vip_maximumamount))
                    vipClientsPaymentPlatform = vipClientsPaymentPlatform_top;

                // VIP3 替换
                if ((float)withdrawRow.amount > float.Parse(vip3_maximumamount))
                    VIP3PaymentPlatform = VIP3PaymentPlatform_top;

                var accountLevel = withdrawRow.level; // connection.Get<ofUser>(itemToProces.account).level;


                targetPlatform = payement_directpay_v2_vip_level.Contains(accountLevel.ToString()) ? vipClientsPaymentPlatform : reguarClientsPaymentPlatform;

                // ******白银特别处理*******
                targetPlatform = accountLevel.ToString() == "10027" ? vipClientsPaymentPlatform : targetPlatform;

                targetPlatform = payement_directpay_v2_vip3_level.Contains(accountLevel.ToString()) ? VIP3PaymentPlatform : targetPlatform;

                // NewUsers are extracted from RegularUser
                var accountRow = connection.Get<ofUser>(withdrawRow.account);
                DateTime monthBefore = DateTime.Parse(Common.CurrentTime.AddDays(-30).ToString("yyyy-MM-dd"));
                if (payement_directpay_v2_regular_level.Contains(accountLevel.ToString()))
                {
                    if (accountRow.createtime >= monthBefore)
                    {
                        targetPlatform = newuser_PaymentPlatform;
                        if ((float)withdrawRow.amount > float.Parse(newuserlow_maximumamount))
                            targetPlatform = newusertop_PaymentPlatform;
                    }
                }

                // MaxAmount Filter
                // VIP
                if (payement_directpay_v2_vip_level.Contains(accountLevel.ToString()) &&
                    (float)withdrawRow.amount > float.Parse(viptop_maximumamount))
                    targetPlatform = "";

                // VIP3
                if (payement_directpay_v2_vip3_level.Contains(accountLevel.ToString()) &&
                    (float)withdrawRow.amount > float.Parse(vip3top_maximumamount))
                    targetPlatform = "";

                // Regular
                if (payement_directpay_v2_regular_level.Contains(accountLevel.ToString()) &&
                    (float)withdrawRow.amount > float.Parse(regularhi_maximumamount))
                    targetPlatform = "";



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
                }


                if (withdrawRow.cardbank.ToLower() == "cgp" || withdrawRow.cardbank.ToLower() == "cgp-erc20" || withdrawRow.cardbank.ToLower() == "cgpay")
                {
                    targetPlatform = cgp_payplatform;
                    itemToProces.cardbank = "CGP";
                }

                if (withdrawRow.cardbank.ToLower() == "购宝" || withdrawRow.cardbank.ToLower() == "g币")
                {
                    targetPlatform = gp_payplatform;
                    itemToProces.cardbank = "GP";
                }


                if (withdrawRow.cardbank.ToLower() == "gop" )
                {
                    targetPlatform = gopay_platform;
                    itemToProces.cardbank = "GoPay";
                }




                if (withdrawRow.cardbank.ToLower() == "eb" || withdrawRow.cardbank.ToLower() == "ebpay")
                {
                    targetPlatform = eb_payplatform;
                    itemToProces.cardbank = "EB";
                }


                if (withdrawRow.cardbank.ToLower() == "k豆")
                {
                    targetPlatform = k豆_payplatform;
                    itemToProces.cardbank = "K豆";
                }

                if (withdrawRow.cardbank.ToLower() == "wp" || withdrawRow.cardbank.ToLower() == "wepay")
                {
                    targetPlatform = wepay_payplatform;
                    itemToProces.cardbank = "WePay";
                }

                if (string.IsNullOrEmpty(targetPlatform))
                {
                    _logger.LogError("NoPlatformSelected - [SendPayment] withdrawamount:{0}", withdrawRow.amount);
                    return new ResponseModel<DirectPayRequestV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "没可用的通道",
                        Data = null
                    };
                }
                #endregion


                // Get Platform fee percentage based on the targetPlatform
                JArray gatewaylist = JArray.Parse(PAYEMENT_DIRECTPAY_V2["payplatform"].ToString());
                for (int i = 0; i < gatewaylist.Count; i++)
                    if (gatewaylist[i]["key"].ToString() == targetPlatform)
                    {
                        feecent = decimal.Parse(gatewaylist[i]["feecent"].ToString());
                        feepertransaction = gatewaylist[i]["feepertransaction"] != null ? decimal.Parse(gatewaylist[i]["feepertransaction"].ToString()) : 0;
                    }


                //Get RemaingBalance through Platform
                string getbalance_message = "";
                float getbalance_amount = -1;


                float usdt_xrate = 6f;
                try
                {
                    var payment_crypto = connection.Get<ofSettings>("PAYMENT_CRYPTO").body;
                    usdt_xrate = float.Parse(JObject.Parse(payment_crypto)["usdt_xrate"].ToString()) + float.Parse(JObject.Parse(payment_crypto)["usdt_xrate_withdraw_topup"].ToString());
                }
                catch (Exception ex) { }

                bool getbalance_result = GetRemainingBalance(targetPlatform, withdrawRow.account, withdrawRow.cardbank, out getbalance_message, out getbalance_amount, withdrawRow.amount, usdt_xrate);

                if (!getbalance_result)
                {
                    _logger.LogError("{1} - [SendPayment] message:{0}",
                        getbalance_message, targetPlatform);
                    return new ResponseModel<DirectPayRequestV2Return>()
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
                    return new ResponseModel<DirectPayRequestV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = string.Format("【" + targetPlatform + "】金额不足 剩下 - RMB{0}", getbalance_amount.ToString("0.00")),
                        Data = null
                    };
                }

            }


            if (currencyCode == 356)
                targetPlatform = "INR";

            using (TransactionScope scope = new TransactionScope())
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var withdrawRow = connection.Get<ofWithdrawal>(request.Id);


                itemToProces = withdrawRow;
                // Should be in paywait mode 
                if (withdrawRow.status != 1)
                    return new ResponseModel<DirectPayRequestV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "不可进行支付的状态",
                        Data = null
                    };

                // Check if the lockperson is the same as the cancelperson
                if (withdrawRow.checkperson.ToLower() != request.Admin.ToLower())
                    return new ResponseModel<DirectPayRequestV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "本人不是锁定的使用者",
                        Data = null
                    };



                // Check if already INSERTED in the paymentLog, So i'll just need to do a query instead
                var ofpaymentlogCount = connection.QueryFirstOrDefault<long?>("SELECT * FROM ofPaymentLog WITH(NOLOCK) WHERE Transactionid='" + withdrawRow.id.ToString() + "'");

                if (ofpaymentlogCount > 0)
                {
                    return new ResponseModel<DirectPayRequestV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "本申请已经代付完成",
                        Data = null
                    };
                }


                // Verify BANKS
                string cardbank = withdrawRow.cardbank;
                if (withdrawRow.cardbank == "上海浦东发展银行") cardbank = "浦东发展银行";
                if (withdrawRow.cardbank == "广发银行") cardbank = "广东发展银行";

                if (withdrawRow.cardbank.ToLower() == "cgpay") cardbank = "CGP";
                if (withdrawRow.cardbank.ToLower() == "购宝") cardbank = "GP";
                if (withdrawRow.cardbank.ToLower() == "gopay") cardbank = "GoPay";
                if (withdrawRow.cardbank.ToLower() == "ebpay") cardbank = "EB";
                if (withdrawRow.cardbank.ToLower() == "k豆") cardbank = "K豆";

                // Verify USDT Wallet Address
                if (withdrawRow.cardbank == "USDT" || withdrawRow.cardbank == "USDT-ERC20")
                {
                    if (!withdrawRow.cardno.StartsWith("0x"))
                    {
                        return new ResponseModel<DirectPayRequestV2Return>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "清检查钱包地址",
                            Data = null
                        };
                    }
                }



                // UPDATE ofWithdrawal
                withdrawRow.fee = (feecent > 0 ? (withdrawRow.amount * (feecent / 100)) : 0) + feepertransaction;
                withdrawRow.payperson = request.Admin;
                withdrawRow.paytime = Common.CurrentTime;
                withdrawRow.status = 2;

                // UPATE ofUser
                var accountRow = connection.Get<ofUser>(withdrawRow.account);
                accountRow.amountsuspend = accountRow.amountsuspend - withdrawRow.amount;
                accountRow.withdrawalamount += withdrawRow.amount;
                accountRow.withdrawalnumber += 1;


                #region ofPaymentRecord
                var paymentRecord = new ofPaymentRecord()
                {
                    currency = currencyCode == 356 ? "INR" :"CNY",
                    account = withdrawRow.account,
                    createtime = Common.CurrentTime,
                    referenceguid = withdrawRow.referenceguid,
                    status = 1,
                    summaryType = 0,
                    transactiontype = 0,
                    preBalance = accountRow.amountactive,
                    bonus = withdrawRow.amount,
                    afterBalance = accountRow.amountactive + withdrawRow.amount,
                    summary = "提款",
                };
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
                    Bank = cardbank,
                    platform = currencyCode == 356 ? "INR" :targetPlatform,
                    payadmin = request.Admin,
                    TransId = withdrawRow.id,
                    misc = ""
                };

                connection.Insert<ofPaymentRecord>(paymentRecord);
                var isAccountUpdateSuccess = connection.Update(accountRow);
                var isWithdrawalSuccess = connection.Update(withdrawRow);
                var isPaymentLog = connection.Insert<ofPaymentLog>(paymentLog) > 0;


                if (isWithdrawalSuccess && isAccountUpdateSuccess && isPaymentLog)
                {
                    scope.Complete();

                    // Withdraw Promo Notify



                }
                else
                    return new ResponseModel<DirectPayRequestV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "资料更新失败",
                        Data = null
                    };

            }

            // Withdraw Promo for USDT Notify Ching
            if (itemToProces.cardbank.ToLower().Contains("usdt"))
            {

                try
                {
                    MemCache.REDIS_CONNSTRING = CMS.Data.Misc.REDIS_CONNECTIONSTRING;
                    MemCache.PushQueue(new JObject()
                    {
                        ["task"] = "withdrawautopromonotifyching",
                        ["account"] = itemToProces.account,
                        ["id"] = itemToProces.id
                    }.ToString(Formatting.None));

                }
                catch (Exception ex)
                {
                    _logger.LogError("ChingAutoPromo - [Exception] WithdrawID:{0}, message:{1}", itemToProces.id, ex.Message);
                }

            }

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                // Get the Playform

                var payment_crypto = connection.Get<ofSettings>("PAYMENT_CRYPTO").body;
                float usdt_xrate = 0;
                try
                {
                    usdt_xrate = float.Parse(JObject.Parse(payment_crypto)["usdt_xrate"].ToString()) + float.Parse(JObject.Parse(payment_crypto)["usdt_xrate_withdraw_topup"].ToString());

                    var y = JObject.Parse(itemToProces.misc);

                    if (y.ContainsKey("usdt_xrate"))
                        usdt_xrate = float.Parse(y["usdt_xrate"].ToString());

                    //if(y.ContainsKey("usdt_xrate_withdraw_topup"))
                    //    usdt_xrate = usdt_xrate + float.Parse(y["usdt_xrate_withdraw_topup"].ToString());

                }

                catch (Exception ex) { }

                var accountLevel = connection.Get<ofUser>(itemToProces.account).level;

                _logger.LogInformation("代付 - [{0}] account:{1}, amount:{2} withdrawId:{3}, level:{4}",
                                    targetPlatform, itemToProces.account, itemToProces.amount, itemToProces.id, accountLevel);
                bool payTaskQueue = false;
                try
                {


                    MemCache.REDIS_CONNSTRING = CMS.Data.Misc.REDIS_CONNECTIONSTRING;

                    switch (targetPlatform)
                    {
                        //case "金星": payTaskQueue = Payment金星(itemToProces); break;
                        case "壹圆USDT-TRC20": payTaskQueue = Payment壹圆USDTTRC20(itemToProces, usdt_xrate); break;
                        case "爱汇USDT-TRC20": payTaskQueue = Payment爱汇USDTTRC20(itemToProces, usdt_xrate); break;
                        
                        // 这个是跑 购宝
                        case "GP": payTaskQueue = PaymentGP(itemToProces); break;

                        // GoPay 就会跑 FunctionApp
                        case "CGP": payTaskQueue = PaymentCGP(itemToProces); break;
                        case "INR":
                            payTaskQueue = MemCache.PushQueue(new JObject()
                            {
                                ["task"] = "payout_inr",
                                ["id"] = itemToProces.id,
                                ["gw"] = "",
                                //["usdtxrate"] = usdt_xrate
                            }.ToString(Formatting.None), "paytaskq");
                            break;
                        default:
                            payTaskQueue = MemCache.PushQueue(new JObject()
                            {
                                ["task"] = "payout",
                                ["id"] = itemToProces.id,
                                ["gw"] = targetPlatform,
                                ["usdtxrate"] = usdt_xrate
                            }.ToString(Formatting.None), "paytaskq");

                            break;
                    }



                    #region MyRegion


                    if (itemToProces.wid == 1)
                    {
                        MemCache.PushQueue(new JObject()
                        {
                            ["task"] = "notifychingverifyfixedamountwdpromov2",
                            ["account"] = itemToProces.account,
                            ["id"] = itemToProces.referenceguid
                        }.ToString(Formatting.None));
                    }

                    #endregion



                }
                catch (Exception ex)
                {
                    _logger.LogError("代付 - [{0}] account:{1}, amount:{2} withdrawId:{3}, level:{4}, Exception:{5}",
                        targetPlatform, itemToProces.account, itemToProces.amount, itemToProces.id, accountLevel, ex.Message);
                }

                return new ResponseModel<DirectPayRequestV2Return>()
                {
                    code = payTaskQueue ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = string.Concat("账号:", itemToProces.account, " 通道:", targetPlatform, " 金额:", itemToProces.amount),
                    Data = null
                };
            }



        }
        /// <summary>
        /// inr 代付渠道 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private ResponseModel<DirectPayRequestV2Return> INR(DirectPayRequestV2 request)
        {
            ofWithdrawal itemToProcess;
            ofPaymentLog paymentLogRow;
            JObject targetPlatform_details;
            string targetPlatform;

            using (TransactionScope scope = new TransactionScope())
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                //decimal feecent;
                //decimal feepertransaction;

                // Payment Target Platform
                var directpay_details = connection.Get<ofSettings>("PAYEMENT_DIRECTPAY_V2_INR");
                JObject PAYEMENT_DIRECTPAY_V2 = JObject.Parse(directpay_details.body);
                targetPlatform = PAYEMENT_DIRECTPAY_V2.SelectToken("$.level_cat.all.payplatform").ToString();
                string usdtPlatform = PAYEMENT_DIRECTPAY_V2.SelectToken("$.level_cat.usdt.payplatform").ToString();
                targetPlatform_details = JObject.FromObject(PAYEMENT_DIRECTPAY_V2.SelectToken($"$.payplatform[?(@.key =='{targetPlatform}')]"));
                var withdrawRow = connection.Get<ofWithdrawal>(request.Id);
                itemToProcess = withdrawRow;

                // **** USDT
                if (withdrawRow.cardbank == "USDT-TRC20")
                    targetPlatform = usdtPlatform;

                #region Verify if eligible for this process
                // Should be in paywait mode 
                if (withdrawRow.status != 1)
                    return new ResponseModel<DirectPayRequestV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "不可进行支付的状态: error status for pay",
                        Data = null
                    };

                // Check if the lockperson is the same as the cancelperson
                if (withdrawRow.checkperson.ToLower() != request.Admin.ToLower())
                    return new ResponseModel<DirectPayRequestV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = $"本人不是锁定的使用者 : {withdrawRow.checkperson}",
                        Data = null
                    };

                // Check if already INSERTED in the paymentLog, So i'll just need to do a query instead
                var ofpaymentlogCount = connection.QueryFirstOrDefault<long?>("SELECT id FROM ofPaymentLog WITH(NOLOCK) WHERE Transactionid='" + withdrawRow.id.ToString() + "'");

                if (ofpaymentlogCount > 0)
                {
                    return new ResponseModel<DirectPayRequestV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "本申请已经代付完成 : Double Request ",
                        Data = null
                    };
                }
                #endregion

                #region Update AND Insert
                // UPDATE ofWithdrawal
                //withdrawRow.fee = (feecent > 0 ? (withdrawRow.amount * (feecent / 100)) : 0) + feepertransaction;
                withdrawRow.payperson = request.Admin;
                withdrawRow.paytime = Common.CurrentTime;
                withdrawRow.status = 2;

                // UPATE ofUser
                var accountRow = connection.Get<ofUser>(withdrawRow.account);
                accountRow.amountsuspend = accountRow.amountsuspend - withdrawRow.amount;
                accountRow.withdrawalamount += withdrawRow.amount;
                accountRow.withdrawalnumber += 1;


                #region ofPaymentRecord
                var paymentRecord = new ofPaymentRecord()
                {
                    currency = "INR",
                    account = withdrawRow.account,
                    createtime = Common.CurrentTime,
                    referenceguid = withdrawRow.referenceguid,
                    status = 1,
                    summaryType = 0,
                    transactiontype = 0,
                    preBalance = accountRow.amountactive,
                    bonus = withdrawRow.amount,
                    afterBalance = accountRow.amountactive + withdrawRow.amount,
                    summary = "提款",
                };
                #endregion


                paymentLogRow = new ofPaymentLog()
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
                    TransId = withdrawRow.id,
                    orderid = withdrawRow.referenceguid.ToLower(),
                    misc = ""
                };


                connection.Insert<ofPaymentRecord>(paymentRecord);
                var isAccountUpdateSuccess = connection.Update(accountRow);
                var isWithdrawalSuccess = connection.Update(withdrawRow);
                var isPaymentLog = connection.Insert<ofPaymentLog>(paymentLogRow) > 0;

                if (isWithdrawalSuccess && isAccountUpdateSuccess && isPaymentLog)
                {
                    scope.Complete();
                }
                else
                    return new ResponseModel<DirectPayRequestV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "资料更新失败",
                        Data = null
                    };
                #endregion


            }

            // **** Actual Payment
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                //var inr_dai_fu_exchange_body = connection.QueryFirstOrDefault<string>("SELECT body FROM ofSettings WHERE code = 'inr_dai_fu_exchange'");
                //string inr_dai_fu_exchange_body_current = JObject.Parse(inr_dai_fu_exchange_body)["current"].ToString();
                //var queryMerchantSql = "select currencyType,merchantid,merchantkey,returnaddress,requestaddress from ofThirdpay with(nolock) where id={0}";

                //string payment_crypto_inr_usdt_xrate = JObject.Parse(itemToProcess.misc)["inr_usdt_xrate"].ToString();
                //string payment_crypto_inr_usdt_xrate_withdraw_topup = JObject.Parse(itemToProcess.misc)["inr_usdt_xrate_withdraw_topup"].ToString();
                //decimal inr_usdt_xrate = decimal.Parse(payment_crypto_inr_usdt_xrate) + decimal.Parse(payment_crypto_inr_usdt_xrate_withdraw_topup);


                // *** USDT BYPASS PROCESS
                // *** ALTER amount to the equivalent USDT
                if (itemToProcess.cardbank.ToLower().StartsWith("usdt"))
                {
                    string payment_crypto_inr_usdt_xrate = JObject.Parse(itemToProcess.misc)["inr_usdt_xrate"].ToString();
                    string payment_crypto_inr_usdt_xrate_withdraw_topup = JObject.Parse(itemToProcess.misc)["inr_usdt_xrate_withdraw_topup"].ToString();
                    decimal inr_usdt_xrate = decimal.Parse(payment_crypto_inr_usdt_xrate) + decimal.Parse(payment_crypto_inr_usdt_xrate_withdraw_topup);

                    var accountName = itemToProcess.realname;
                    var payOrderID = itemToProcess.referenceguid;

                    var yyDaiFu = new YYTrc20();
                    yyDaiFu.merchantKey = "35d7074fba5f6c28615545fb0de71b67";
                    yyDaiFu.username = "bayo1000";// merchant.merchantid;
                    yyDaiFu.api_url = "https://awc3rdpay.azurewebsites.net/api/withdraw/RequestWithdraw";// merchant.requestaddress;

                    var postDict = new SortedDictionary<string, dynamic>() {
                            { "amount", itemToProcess.amount },
                            { "inr_usdt_xrate", inr_usdt_xrate},
                            { "wallet_address", itemToProcess.cardno},
                            { "accountName", accountName},
                            { "order_id", payOrderID},
                    };

                    var d = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(postDict));
                    var result = yyDaiFu.ReqDaiFuV(d, request.writeToFile).Result;
                    UpdatePaymentLog(itemToProcess.id, paymentLogRow.orderid, result.message, result.code == 1, "bayo1000");
                    if (result.code == 1) {
                        return new ResponseModel<DirectPayRequestV2Return>() {
                            code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                            message = "代付请求成功!（request direct pay success）",
                            Data = null
                        };
                    }
                    else {
                        return new ResponseModel<DirectPayRequestV2Return>() {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "代付请求失败，请和渠道确认代付结果!（request direct pay failed, check it with merhant pls）",
                            Data = null
                        };
                    }

                }
                else
                {
                    var inr_dai_fu_exchange_body = connection.QueryFirstOrDefault<string>("SELECT body FROM ofSettings WHERE code = 'inr_dai_fu_exchange'");
                    string inr_dai_fu_exchange_body_current = JObject.Parse(inr_dai_fu_exchange_body)["current"].ToString();
                    var queryMerchantSql = "select currencyType,merchantid,merchantkey,returnaddress,requestaddress from ofThirdpay with(nolock) where id={0}";

                    // *** INR PROCESS   bank daifu 
                    switch (inr_dai_fu_exchange_body_current)
                    {
                        case "0": 
                            {
                                MemCache.REDIS_CONNSTRING = CMS.Data.Misc.REDIS_CONNECTIONSTRING;
                                var payTaskQueue = MemCache.PushQueue(new JObject() {
                                    ["task"] = "payout_inr",
                                    ["id"] = itemToProcess.id,
                                    ["gw"] = "",
                                    //["usdtxrate"] = usdt_xrate
                                }.ToString(Formatting.None), "paytaskq");
                            }
                            break;
                        case "1":
                            {
                                //typay
                                var querySql = string.Format(queryMerchantSql, "10735");
                                var merchantDys = DapperBase.QueryAsync<OrderMerchantModel>(querySql, null,
                                    commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString).Result;
                                if (merchantDys != null && merchantDys.Count > 0)
                                {
                                    var merchant = merchantDys.First();
                                    var tyDaiFu = new TYPayINR();
                                    tyDaiFu.merchantKey = merchant.merchantkey;
                                    tyDaiFu.username = merchant.merchantid;
                                    tyDaiFu.api_url = merchant.requestaddress;
                                    var date = DateTime.UtcNow.AddHours(8);
                                    var time = UtilsLib.Common.ToUnixTimestampSS(date).ToString();
                                    var dfModel = new TYPayInrDaiFuRequestModel()
                                    {
                                        time = time,
                                        nonce = string.Format(@"a{0}", time),
                                        tid = paymentLogRow.orderid,
                                        money = itemToProcess.amount.ToString("0.00"),
                                        accountname = itemToProcess.realname,
                                        bankcardnumber = itemToProcess.cardno,
                                        ifsc = itemToProcess.cardaddress,
                                        callback_url = merchant.returnaddress, //ty这代收和代付 回调通知数据结构一样。
                                    };

                                    var d = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(dfModel));
                                    var result = tyDaiFu.ReqDaiFuV(d, request.writeToFile).Result;
                                    UpdatePaymentLog(itemToProcess.id, paymentLogRow.orderid, result.message, result.code == 1, "TYPay");
                                    if (result.code == 1)
                                    {
                                        return new ResponseModel<DirectPayRequestV2Return>()
                                        {
                                            code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                                            message = "代付请求成功!（request direct pay success）",
                                            Data = null
                                        };
                                    }
                                    else
                                    {
                                        return new ResponseModel<DirectPayRequestV2Return>()
                                        {
                                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                                            message = "代付请求失败，请和渠道确认代付结果!（request direct pay failed, check it with merhant pls）",
                                            Data = null
                                        };
                                    }

                                }
                                else
                                {

                                    return new ResponseModel<DirectPayRequestV2Return>()
                                    {
                                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                                        message = "代付请求发送失败（request direct pay failed）",
                                        Data = null
                                    };

                                }

                            }
                            break;
                        case "2":
                            {
                                //axpay
                                var querySql = string.Format(queryMerchantSql, "10745");
                                var merchantDys = DapperBase.QueryAsync<OrderMerchantModel>(querySql, null,
                                    commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString).Result;
                                if (merchantDys != null && merchantDys.Count > 0)
                                {
                                    var merchant = merchantDys.First();
                                    //var payBase = GetPayChanel.GetPayBaseById(account: model.account,
                                    //    thirdpayId: model.platform,
                                    //    merchantKey: merchant.merchantkey,
                                    //    merchantName: merchant.merchantid,
                                    //    notify_url: merchant.returnaddress,
                                    //    api_url: merchant.requestaddress);
                                    //return await payBase.ReqOrderV(model, writeToFile);

                                    //axpay 代付
                                    var axDaiFu = new AXPayInr();

                                    axDaiFu.merchantKey = merchant.merchantkey;
                                    axDaiFu.username = merchant.merchantid;
                                    axDaiFu.api_url = merchant.requestaddress;

                                    var date = DateTime.UtcNow.AddHours(8);

                                    var dfModel = new AXPayInrDaiFuRequestModel()
                                    {
                                        order_number = paymentLogRow.orderid,
                                        order_amount = ((int)itemToProcess.amount).ToString(),
                                        account = itemToProcess.cardno,
                                        name = itemToProcess.realname,
                                        ifsc = itemToProcess.cardaddress,
                                        notify_url = "https://awapis-thirdpay.azurewebsites.net/api/daifu/axDaiFuCallBack"
                                    };

                                    var d = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(dfModel));
                                    var result = axDaiFu.ReqDaiFuV(d, request.writeToFile).Result;
                                    UpdatePaymentLog(itemToProcess.id, paymentLogRow.orderid, result.message, result.code == 1, "AxPay");
                                    if (result.code == 1)
                                    {
                                        return new ResponseModel<DirectPayRequestV2Return>()
                                        {
                                            code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                                            message = "代付请求成功!（request direct pay success）",
                                            Data = null
                                        };
                                    }
                                    else
                                    {
                                        return new ResponseModel<DirectPayRequestV2Return>()
                                        {
                                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                                            message = "代付请求失败，请和渠道确认代付结果!（request direct pay failed, check it with merhant pls）",
                                            Data = null
                                        };
                                    }


                                }
                                else
                                {

                                    return new ResponseModel<DirectPayRequestV2Return>()
                                    {
                                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                                        message = "代付请求发送失败（request direct pay failed）",
                                        Data = null
                                    };

                                }

                            }
                            break;
                        case "3":
                            {
                                //axpay
                                var querySql = string.Format(queryMerchantSql, "10752");
                                var merchantDys = DapperBase.QueryAsync<OrderMerchantModel>(querySql, null,
                                    commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString).Result;
                                if (merchantDys != null && merchantDys.Count > 0)
                                {
                                    var merchant = merchantDys.First();

                                    //axpay 代付
                                    var axDaiFu = new MegaPayINR();

                                    axDaiFu.merchantKey = merchant.merchantkey;
                                    axDaiFu.username = merchant.merchantid;
                                    axDaiFu.api_url = merchant.requestaddress;

                                    var date = DateTime.UtcNow.AddHours(8);


                                    var daifuOrder = string.Format(@"df10752{0}", UtilsLib.Common.GuidToLong());// string.Format(@"df1075215404132934911610533");
                                    var bankname = string.IsNullOrEmpty(itemToProcess.cardbank) ? "bankname" : itemToProcess.cardbank;
                                    var dfModel = new MegaDaiFuReqModel()
                                    {
                                        morder = daifuOrder,
                                        amount = itemToProcess.amount.ToString("0.00"),
                                        ifsc = itemToProcess.cardaddress, //"FDRL0005555",
                                        bankname = bankname,              //银行名，如果没有 可以给定 bankname
                                        accname = itemToProcess.realname,//"hello9",
                                        accno = itemToProcess.cardno //"18031201728"
                                    };

                                    var d = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(dfModel));
                                    var result = axDaiFu.ReqDaiFuV(d, request.writeToFile).Result;
                                    //_logger.LogInformation("SendPayment - [GP] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                                    //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                                    UpdatePaymentLog(itemToProcess.id, daifuOrder, result.message, result.code == 1, "MegaPay");

                                    if (result.code == 1)
                                    {
                                        return new ResponseModel<DirectPayRequestV2Return>()
                                        {
                                            code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                                            message = "代付请求成功!（request direct pay success）",
                                            Data = null
                                        };
                                    }
                                    else
                                    {
                                        return new ResponseModel<DirectPayRequestV2Return>()
                                        {
                                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                                            message = "代付请求失败，请和渠道确认代付结果!（request direct pay failed, check it with merhant pls）",
                                            Data = null
                                        };
                                    }


                                }
                                else
                                {

                                    return new ResponseModel<DirectPayRequestV2Return>()
                                    {
                                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                                        message = "代付请求发送失败（request direct pay failed）",
                                        Data = null
                                    };

                                }

                            }
                            break;
                        case "4": {
                                
                                var querySql = string.Format(queryMerchantSql, "10767");
                                var merchantDys = DapperBase.QueryAsync<OrderMerchantModel>(querySql, null,
                                    commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString).Result;
                                if (merchantDys != null && merchantDys.Count > 0) {
                                    var merchant = merchantDys.First();

                                    //simplyPay 代付
                                    var simplyPayDaiFu = new SimplyPayINR();

                                    simplyPayDaiFu.merchantKey = merchant.merchantkey;
                                    simplyPayDaiFu.username = merchant.merchantid;
                                    simplyPayDaiFu.api_url = merchant.requestaddress;

                                    var daifuOrder = string.Format(@"df10767{0}", itemToProcess.referenceguid);
                                    var dfModel = new SimplyPayDaiFuReqModel() {
                                        morder = daifuOrder,
                                        amount = itemToProcess.amount.ToString("0.00"),
                                        ifsc = itemToProcess.cardaddress, //"FDRL0005555",
                                        accname = itemToProcess.realname,//"hello9",
                                        accno = itemToProcess.cardno //"18031201728"
                                    };

                                    var d = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(dfModel));
                                    var result = simplyPayDaiFu.ReqDaiFuV(d, request.writeToFile).Result;
                                    UpdatePaymentLog(itemToProcess.id, daifuOrder, result.message, result.code == 1, "SimplyPay");

                                    if (result.code == 1) {
                                        return new ResponseModel<DirectPayRequestV2Return>() {
                                            code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                                            message = "代付请求成功!（request direct pay success）",
                                            Data = null
                                        };
                                    }
                                    else {
                                        return new ResponseModel<DirectPayRequestV2Return>() {
                                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                                            message = "代付请求失败，请和渠道确认代付结果!（request direct pay failed, check it with merhant pls）",
                                            Data = null
                                        };
                                    }

                                }
                                else {

                                    return new ResponseModel<DirectPayRequestV2Return>() {
                                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                                        message = "代付请求发送失败（request direct pay failed）",
                                        Data = null
                                    };

                                }

                            }
                            break;
                        case "5":
                            {
                                //HHPay
                                var querySql = string.Format(queryMerchantSql, "10785");
                                var merchantDys = DapperBase.QueryAsync<OrderMerchantModel>(querySql, null,
                                    commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString).Result;
                                if (merchantDys != null && merchantDys.Count > 0)
                                {
                                    var merchant = merchantDys.First();
                                    //hhPay 代付
                                    var hhPayDaiFu = new HHPayINR();
                                    hhPayDaiFu.merchantKey =merchant.merchantkey;
                                    hhPayDaiFu.username = merchant.merchantid;
                                    hhPayDaiFu.api_url = merchant.requestaddress;

                                    var daifuOrder = string.Format(@"df10785{0}", itemToProcess.referenceguid);
                                    var dfModel = new HHPayDaiFuReqModel()
                                    {
                                        morder = daifuOrder,
                                        amount = itemToProcess.amount.ToString("0.00"),
                                        ifsc = itemToProcess.cardaddress, //"FDRL0005555",
                                        accname = itemToProcess.realname,//"hello9",
                                        accno = itemToProcess.cardno, //"18031201728"
                                        bankname = itemToProcess.cardbank //银行名
                                    };

                                    var d = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(dfModel));
                                    var result = hhPayDaiFu.ReqDaiFuV(d, request.writeToFile).Result;
                                    UpdatePaymentLog(itemToProcess.id, daifuOrder, result.message, result.code == 1, "HHPay");

                                    if (result.code == 1)
                                    {
                                        return new ResponseModel<DirectPayRequestV2Return>()
                                        {
                                            code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                                            message = "代付请求成功!（request direct pay success）",
                                            Data = null
                                        };
                                    }
                                    else
                                    {
                                        return new ResponseModel<DirectPayRequestV2Return>()
                                        {
                                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                                            message = "代付请求失败，请和渠道确认代付结果!（request direct pay failed, check it with merhant pls）",
                                            Data = null
                                        };
                                    }

                                }
                                else
                                {

                                    return new ResponseModel<DirectPayRequestV2Return>()
                                    {
                                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                                        message = "代付请求发送失败（request direct pay failed）",
                                        Data = null
                                    };

                                }

                            }
                            break;
                        default:
                            break;
                    }

                    return new ResponseModel<DirectPayRequestV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "",
                        Data = null
                    };
                }
            }


        }

        /// <summary>
        /// inr 代付渠道 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private ResponseModel<DirectPayRequestV2Return> INR_V(DirectPayRequestV2 request) {
            ofWithdrawal itemToProcess;
            ofPaymentLog paymentLogRow;
            JObject targetPlatform_details;
            string targetPlatform;

            var inr_dai_fu_exchange_body_current = "";
            
            using (TransactionScope scope = new TransactionScope())
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING)) {

                //var directpay_details = connection.Get<ofSettings>("PAYEMENT_DIRECTPAY_V2_INR");
                //JObject PAYEMENT_DIRECTPAY_V2 = JObject.Parse(directpay_details.body);
                //targetPlatform = PAYEMENT_DIRECTPAY_V2.SelectToken("$.level_cat.all.payplatform").ToString();
                //string usdtPlatform = PAYEMENT_DIRECTPAY_V2.SelectToken("$.level_cat.usdt.payplatform").ToString();
                //targetPlatform_details = JObject.FromObject(PAYEMENT_DIRECTPAY_V2.SelectToken($"$.payplatform[?(@.key =='{targetPlatform}')]"));

                var inr_dai_fu_exchange_body = connection.QueryFirstOrDefault<string>("SELECT body FROM ofSettings WHERE code = 'inr_dai_fu_exchange'");
                inr_dai_fu_exchange_body_current = JObject.Parse(inr_dai_fu_exchange_body)["current"].ToString();

                targetPlatform = inr_dai_fu_exchange_body_current.Equals("0") ? "EPay"
                    : inr_dai_fu_exchange_body_current.Equals("1") ? "TYPay"
                    : inr_dai_fu_exchange_body_current.Equals("2") ? "AXPay" : "";

                if(string.IsNullOrEmpty(targetPlatform))
                    return new ResponseModel<DirectPayRequestV2Return>() {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = $"错误代付渠道（error direct pay merchant）",
                        Data = null
                    };

                var withdrawRow = connection.Get<ofWithdrawal>(request.Id);
                itemToProcess = withdrawRow;


                #region Verify if eligible for this process
                // Should be in paywait mode 
                if (withdrawRow.status != 1)
                    return new ResponseModel<DirectPayRequestV2Return>() {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "不可进行支付的状态: error status for pay",
                        Data = null
                    };

                // Check if the lockperson is the same as the cancelperson
                if (withdrawRow.checkperson.ToLower() != request.Admin.ToLower())
                    return new ResponseModel<DirectPayRequestV2Return>() {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = $"本人不是锁定的使用者 : {withdrawRow.checkperson}",
                        Data = null
                    };

                // Check if already INSERTED in the paymentLog, So i'll just need to do a query instead
                var ofpaymentlogCount = connection.QueryFirstOrDefault<long?>("SELECT id FROM ofPaymentLog WITH(NOLOCK) WHERE Transactionid='" + withdrawRow.id.ToString() + "'");

                if (ofpaymentlogCount > 0) {
                    return new ResponseModel<DirectPayRequestV2Return>() {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "本申请已经代付完成 : Double Request ",
                        Data = null
                    };
                }
                #endregion

                #region Update AND Insert
                // UPDATE ofWithdrawal
                //withdrawRow.fee = (feecent > 0 ? (withdrawRow.amount * (feecent / 100)) : 0) + feepertransaction;
                withdrawRow.payperson = request.Admin;
                withdrawRow.paytime = Common.CurrentTime;
                withdrawRow.status = 2;

                // UPATE ofUser
                var accountRow = connection.Get<ofUser>(withdrawRow.account);
                accountRow.amountsuspend = accountRow.amountsuspend - withdrawRow.amount;
                accountRow.withdrawalamount += withdrawRow.amount;
                accountRow.withdrawalnumber += 1;


                #region ofPaymentRecord
                var paymentRecord = new ofPaymentRecord() {
                    currency = "INR",
                    account = withdrawRow.account,
                    createtime = Common.CurrentTime,
                    referenceguid = withdrawRow.referenceguid,
                    status = 1,
                    summaryType = 0,
                    transactiontype = 0,
                    preBalance = accountRow.amountactive,
                    bonus = withdrawRow.amount,
                    afterBalance = accountRow.amountactive + withdrawRow.amount,
                    summary = "提款",
                };
                #endregion


                paymentLogRow = new ofPaymentLog() {
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
                    TransId = withdrawRow.id,
                    orderid = withdrawRow.referenceguid.ToLower(),
                    misc = ""
                };


                connection.Insert<ofPaymentRecord>(paymentRecord);
                var isAccountUpdateSuccess = connection.Update(accountRow);
                var isWithdrawalSuccess = connection.Update(withdrawRow);
                var isPaymentLog = connection.Insert<ofPaymentLog>(paymentLogRow) > 0;

                if (isWithdrawalSuccess && isAccountUpdateSuccess && isPaymentLog) {
                    scope.Complete();
                }
                else
                    return new ResponseModel<DirectPayRequestV2Return>() {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "资料更新失败",
                        Data = null
                    };
                #endregion


            }

            // **** Actual Payment
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING)) {
                var queryMerchantSql = string.Format(@"select currencyType,merchantid,merchantkey,returnaddress,requestaddress from ofThirdpay with(nolock) where id={0}");
                switch (inr_dai_fu_exchange_body_current) {
                    case "0":
                        MemCache.REDIS_CONNSTRING = CMS.Data.Misc.REDIS_CONNECTIONSTRING;
                        var payTaskQueue = MemCache.PushQueue(new JObject() {
                            ["task"] = "payout_inr",
                            ["id"] = itemToProcess.id,
                            ["gw"] = "",
                            //["usdtxrate"] = usdt_xrate
                        }.ToString(Formatting.None), "paytaskq");
                        break;

                    case "1": {
                            //typay
                            var querySql = string.Format(queryMerchantSql, "10735");
                            var merchantDys = DapperBase.QueryAsync<OrderMerchantModel>(querySql, null,
                                commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString).Result;
                            if (merchantDys != null && merchantDys.Count > 0) {
                                var merchant = merchantDys.First();
                                var tyDaiFu = new TYPayINR();
                                tyDaiFu.merchantKey = merchant.merchantkey;
                                tyDaiFu.username = merchant.merchantid;
                                tyDaiFu.api_url = merchant.requestaddress;
                                var date = DateTime.UtcNow.AddHours(8);
                                var time = UtilsLib.Common.ToUnixTimestampSS(date).ToString();
                                var dfModel = new TYPayInrDaiFuRequestModel() {
                                    time = time,
                                    nonce = string.Format(@"a{0}", time),
                                    tid = paymentLogRow.orderid,
                                    money = itemToProcess.amount.ToString("0.00"),
                                    accountname = itemToProcess.realname,
                                    bankcardnumber = itemToProcess.cardno,
                                    ifsc = itemToProcess.cardaddress,
                                    callback_url = merchant.returnaddress, //ty这代收和代付 回调通知数据结构一样。
                                };

                                var d = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(dfModel));
                                var result = tyDaiFu.ReqDaiFuV(d, request.writeToFile).Result;
                                if (result.code == 1) {
                                    return new ResponseModel<DirectPayRequestV2Return>() {
                                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                                        message = "代付请求成功!（request direct pay success）",
                                        Data = null
                                    };
                                }
                                else {
                                    return new ResponseModel<DirectPayRequestV2Return>() {
                                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                                        message = "代付请求失败，请和渠道确认代付结果!（request direct pay failed, check it with merhant pls）",
                                        Data = null
                                    };
                                }

                            }
                            else {

                                return new ResponseModel<DirectPayRequestV2Return>() {
                                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                                    message = "代付请求发送失败（request direct pay failed）",
                                    Data = null
                                };

                            }

                        }
                        break;
                    case "2": {
                            //axpay
                            var querySql = string.Format(queryMerchantSql, "10745");
                            var merchantDys = DapperBase.QueryAsync<OrderMerchantModel>(querySql, null,
                                commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString).Result;
                            if (merchantDys != null && merchantDys.Count > 0) {
                                var merchant = merchantDys.First();
                                //var payBase = GetPayChanel.GetPayBaseById(account: model.account,
                                //    thirdpayId: model.platform,
                                //    merchantKey: merchant.merchantkey,
                                //    merchantName: merchant.merchantid,
                                //    notify_url: merchant.returnaddress,
                                //    api_url: merchant.requestaddress);
                                //return await payBase.ReqOrderV(model, writeToFile);

                                //axpay 代付
                                var axDaiFu = new AXPayInr();

                                axDaiFu.merchantKey = merchant.merchantkey;
                                axDaiFu.username = merchant.merchantid;
                                axDaiFu.api_url = merchant.requestaddress;

                                var date = DateTime.UtcNow.AddHours(8);
                                //merchant_id 是   string 商户号
                                //order_number 是   string 商户订单号；保证唯一性
                                //order_amount    是 string 订单金额 整数 ,单位卢比，不支持小数点
                                //type    是 string 支付类型填： BANK/UPI（任选其一）
                                //vpa 否   string upiid(当type= UPI时，必填)
                                //email 是   string 邮箱
                                //account 否   string 银行卡账号(当type= BANK时，必填)
                                //name 是   string 姓名，纯英文
                                //ifsc    否 string ifsc，”/^[A-Z]{4}0[A - Z0 - 9]{ 6}$/“; 判断规则(当type = BANK时，必填)
                                //phone 是	string	手机号 ，不用＋91
                                //notify_url	否	string	回调地址；正式账号可以代码里面传, 也可以商户后台自行添加
                                //sign	是	string	签名; 将以上字段 处理成json 格式，类似请求body的json串进行rsa加密，不需要排序

                                var dfModel = new AXPayInrDaiFuRequestModel() {
                                    order_number = paymentLogRow.orderid,
                                    order_amount = ((int)itemToProcess.amount).ToString(),
                                    account = itemToProcess.cardno,
                                    name = itemToProcess.realname,
                                    ifsc = itemToProcess.cardaddress,
                                    notify_url = "https://awapis-thirdpay.azurewebsites.net/api/daifu/axDaiFuCallBack"
                                };

                                var d = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(dfModel));
                                var result = axDaiFu.ReqDaiFuV(d, request.writeToFile).Result;
                                if (result.code == 1) {
                                    return new ResponseModel<DirectPayRequestV2Return>() {
                                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                                        message = "代付请求成功!（request direct pay success）",
                                        Data = null
                                    };
                                }
                                else {
                                    return new ResponseModel<DirectPayRequestV2Return>() {
                                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                                        message = "代付请求失败，请和渠道确认代付结果!（request direct pay failed, check it with merhant pls）",
                                        Data = null
                                    };
                                }


                            }
                            else {

                                return new ResponseModel<DirectPayRequestV2Return>() {
                                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                                    message = "代付请求发送失败（request direct pay failed）",
                                    Data = null
                                };

                            }

                        }
                        break;
                    default:
                        break;
                }

                return new ResponseModel<DirectPayRequestV2Return>() {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "",
                    Data = null
                };
            }


        }


        private bool PaymentYiYuanUSDT(SqlConnection connection, ofWithdrawal row, JObject platormDetails, ofPaymentLog paymentLog, float usdt_xrate)
        {
            // **** Initialize Pay Class
            var pay = new PayLib.YiYuan.YYTrc20()
            {
                username = platormDetails["mid"].ToString(),
                merchantKey = platormDetails["mkey"].ToString(),
                api_url = platormDetails["posturl"].ToString()
            };

            // **** POST the payment
            var result = pay.ReqDaiFuV(new Dictionary<string, string>() {
                { "usdt_xrate", usdt_xrate.ToString("0.00")},
                { "amount", row.amount.ToString("0.00")},
                { "order_id", paymentLog.orderid},
                { "wallet_address", row.cardno},
            }, null);


            // **** RESPONSE
            var resultResponse = result.Result;



            var paymentlgoInsertResult = connection.Execute("" +
                "UPDATE ofPaymentLog " +
                "SET " +
                "Result=@PAYMENTRESPONSE, " +
                "StatusCode=@PAYRESULT " +
                "WHERE TransactionId=@ID", (new
                {
                    ID = row.id,
                    PAYMENTRESPONSE = resultResponse.responseData,
                    PAYRESULT = resultResponse.code == 1 ? "1" : "0"
                }));


            return resultResponse.code == 1;
        }


        private bool PaymentBaoyinpayINR(SqlConnection connection, ofWithdrawal row, JObject platormDetails, ofPaymentLog paymentLog)
        {
            // **** Initialize Pay Class
            var pay = new PayLib.BaoinPay.BaoinPayINR()
            {
                username = platormDetails["mid"].ToString(),
                merchantKey = platormDetails["mkey"].ToString(),
                api_url = platormDetails["posturl"].ToString()
            };

            // **** POST the payment
            var result = pay.ReqDaiFuV(new Dictionary<string, string>()
            {
                { "callback_url", platormDetails["callback"].ToString() },
                { "bank_card_name", row.realname },
                { "bank_card_account", row.cardno },
                { "bank_card_remark", row.cardbank },
                { "total_amount", row.amount.ToString("0.00") },
                { "order_id", paymentLog.orderid},
            }, null);


            // **** RESPONSE
            var resultResponse = result.Result;



            var paymentlgoInsertResult = connection.Execute("" +
                "UPDATE ofPaymentLog " +
                "SET " +
                "Result=@PAYMENTRESPONSE, " +
                "StatusCode=@PAYRESULT " +
                "WHERE TransactionId=@ID", (new
                {
                    ID = row.id,
                    PAYMENTRESPONSE = resultResponse.responseData,
                    PAYRESULT = resultResponse.code == 1 ? "1" : "0"
                }));


            return resultResponse.code == 1;
        }
        private bool PaymentJDPayINR(SqlConnection connection, ofWithdrawal row, JObject platormDetails, ofPaymentLog paymentLog)
        {

            // **** Initialize Pay Class
            var pay = new PayLib.EPay.EPayINR()
            {
                username = platormDetails["mid"].ToString(),
                merchantKey = platormDetails["mkey"].ToString(),
                api_url = platormDetails["posturl"].ToString()
            };

            // **** POST the payment
            var PaymentMethod = row.cardbank.ToUpper().Equals("UPI") ? "UPI" : "Bank";
            var result = pay.ReqDaiFuV(new Dictionary<string, string>()
            {
                { "NotifyUrl", platormDetails["callback"].ToString() },
                { "BankAccountName", row.realname },
                { "BankAccountNumber", row.cardno },
                { "bank_card_remark", row.cardbank },
                { "OrderAmount", row.amount.ToString("0.00") },
                { "OrderId", paymentLog.orderid},
                { "PaymentMethod", PaymentMethod }, //必传
            }, null);


            //// **** Initialize Pay Class
            //var pay = new PayLib.JDPay.JDPayINR() {
            //    username = mid,
            //    merchantKey = mkey,
            //    api_url = postUrl
            //};

            //var PaymentMethod = cardbank.ToUpper().Equals("UPI") ? "UPI" : "Bank";

            //// **** POST the payment
            //var result = pay.ReqDaiFuV(new Dictionary<string, string>()
            //{
            //    { "NotifyUrl", callBackUrl },
            //    { "BankAccountName", realName },
            //    { "BankAccountNumber", cardno },//必传
            //    { "bank_card_remark", cardbank },
            //    { "OrderAmount", orderAmount.ToString("F2") },
            //    { "OrderId", orderId}
            //    { "PaymentMethod", PaymentMethod }, //必传
            //}, null);


            // **** RESPONSE
            var resultResponse = result.Result;



            //var paymentlgoInsertResult = connection.Execute("" +
            //    "UPDATE ofPaymentLog " +
            //    "SET " +
            //    "Result=@PAYMENTRESPONSE, " +
            //    "StatusCode=@PAYRESULT " +
            //    "WHERE TransactionId=@ID", (new {
            //        ID = row.id,
            //        PAYMENTRESPONSE = resultResponse.responseData,
            //        PAYRESULT = resultResponse.code == 1 ? "1" : "0"
            //    }));


            return resultResponse.code == 1;
        }


        private bool PaymentEPayINR(SqlConnection connection, ofWithdrawal row, JObject platormDetails, ofPaymentLog paymentLog)
            //private bool PaymentEPayINR(SqlConnection connection, string mid, string mkey, string postUrl, string callBackUrl,
            //string realName, string cardno, string cardbank, decimal orderAmount, string orderId, string withdrawRowID)
        {

            // **** Initialize Pay Class
            var pay = new PayLib.EPay.EPayINR()
            {
                username = platormDetails["mid"].ToString(),
                merchantKey = platormDetails["mkey"].ToString(),
                api_url = platormDetails["posturl"].ToString()
            };

            // **** POST the payment
            var PaymentMethod = row.cardbank.ToUpper().Equals("UPI") ? "UPI" : "Bank";
            var result = pay.ReqDaiFuV(new Dictionary<string, string>()
            {
                { "NotifyUrl", platormDetails["callback"].ToString() },
                { "BankAccountName", row.realname },
                { "BankAccountNumber", row.cardno },
                { "bank_card_remark", row.cardbank },
                { "OrderAmount", row.amount.ToString("0.00") },
                { "OrderId", paymentLog.orderid},
                { "PaymentMethod", PaymentMethod }, //必传
            }, null);


            // **** RESPONSE
            var resultResponse = result.Result;

            var paymentlgoInsertResult = connection.Execute("" +
                "UPDATE ofPaymentLog " +
                "SET " +
                "Result=@PAYMENTRESPONSE, " +
                "StatusCode=@PAYRESULT " +
                "WHERE TransactionId=@ID", (new
                {
                    ID = row.id,
                    PAYMENTRESPONSE = resultResponse.responseData,
                    PAYRESULT = resultResponse.code == 1 ? "1" : "0"
                }));


            return resultResponse.code == 1;
        }

        private bool Payment壹圆usdtINR(SqlConnection connection, ofWithdrawal row, JObject platormDetails, ofPaymentLog paymentLog)
        {
            // **** Initialize Pay Class
            var pay = new PayLib.BaoinPay.BaoinPayINR()
            {
                username = platormDetails["mid"].ToString(),
                merchantKey = platormDetails["mkey"].ToString(),
                api_url = platormDetails["posturl"].ToString()
            };

            // **** POST the payment
            var result = pay.ReqDaiFuV(new Dictionary<string, string>()
            {
                { "callback_url", platormDetails["callback"].ToString() },
                { "bank_card_name", row.realname },
                { "bank_card_account", row.cardno },
                { "bank_card_remark", row.cardbank },
                { "total_amount", row.amount.ToString("0.00") },
                { "order_id", paymentLog.orderid},
            }, null);


            // **** RESPONSE
            var resultResponse = result.Result;



            var paymentlgoInsertResult = connection.Execute("" +
                "UPDATE ofPaymentLog " +
                "SET " +
                "Result=@PAYMENTRESPONSE, " +
                "StatusCode=@PAYRESULT " +
                "WHERE TransactionId=@ID", (new
                {
                    ID = row.id,
                    PAYMENTRESPONSE = resultResponse.responseData,
                    PAYRESULT = resultResponse.code == 1 ? "1" : "0"
                }));


            return resultResponse.code == 1;
        }

        private bool GetRemainingBalance(string targetPlatform, string account, string cardbank, out string message, out float remainingBalance, decimal amountrequested, float usdt_xrate)
        {
            remainingBalance = 100000;
            message = "No Balance Query Available";
            return true;

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
                        ID = id.ToString(),
                        ORDERID = orderid,
                        PAYMENTRESPONSE = paymentResponse,
                        MISC = misc,
                        PAYRESULT = payResult ? "1" : "0"
                    }));
            }
            return paymentlgoInsertResult == 1;
        }




        private bool UpdatePaymentLog(long withdrawRowID, string orderid, string paymentResponse, bool payResult, string targetplatform)
        {
            int paymentlgoInsertResult = 0;
            try {
                

                using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING)) {

                    paymentlgoInsertResult = connection.Execute("" +
                        "UPDATE ofPaymentLog " +
                        "SET orderid=@ORDERID, " +
                        "Result=@PAYMENTRESPONSE, " +
                        "platform=@TARGETPLATFORM, " +
                        "StatusCode=@PAYRESULT " +
                        "WHERE TransactionId=@ID", (new {
                            ID = withdrawRowID.ToString(),
                            ORDERID = orderid,
                            TARGETPLATFORM = targetplatform,
                            PAYMENTRESPONSE = paymentResponse,
                            PAYRESULT = payResult ? "1" : "0"
                        }));
                }
                return paymentlgoInsertResult == 1;
            }
            catch (Exception) {

                return paymentlgoInsertResult == 1;
            }
        }


        private bool UpdatePaymentLog(long withdrawRowID, string orderid, string paymentResponse, bool payResult)
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
                        ID = withdrawRowID.ToString(),
                        ORDERID = orderid,
                        PAYMENTRESPONSE = paymentResponse,
                        PAYRESULT = payResult ? "1" : "0"
                    }));
            }
            return paymentlgoInsertResult == 1;
        }

        private bool Payment爱汇USDTTRC20(ofWithdrawal row, float usdt_xrate)
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
                _logger.LogInformation("    ] cny:{2}, usdt_xrate:{3} result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, usdt_xrate);
                return UpdatePaymentLog_USDT(row.id, orderid, stf.paymentResponse, payResult, usdt_xrate);
            }
            catch (Exception ex)
            {
                _logger.LogError("SendPayment - [爱汇USDT-TRC20] rowid:{1}, exception:{0}", ex.Message, row.id);
                return false;
            }
        }

        private bool PaymentCGP(ofWithdrawal row)
        {
            if (row.cardbank.ToLower() == "cgp" || row.cardbank.ToLower() == "cgpay")
            {


                string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
                string postUrl = "http://public.cgpay.io/api/v3/MerchantWithdraw";
                string returnUrl = "https://www.awcwin8.com";
                string _merchantID = "3b52434bf9962e03b11acb2ba8efe0a1";
                string _merhcnatKey = "a1636815de";


                var stf = new Thirdpay.CGP.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
                {
                    account = row.account,
                    returnUrl = returnUrl,
                    clientIPAddress = "1.2.2.2"
                };
                stf.proxy_address = "172.233.91.151";
                stf.proxy_port = 65531;

                try
                {
                    string orderid = stf.GenerateOrderID.Substring(0, 20);

                    float amountadjust = (float)row.amount;
                    //Random rand = new Random();
                    //float additional = rand.Next(1, 5);
                    //additional = additional / 100;
                    //amountadjust = amountadjust + additional;
                    var payResult = stf.SendPayment(orderid, amountadjust, Thirdpay.Common.BANKS.CGP, row.cardno, row.realname);
                    _logger.LogInformation("SendPayment - [CGP] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                    //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                    return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

                }
                catch (Exception ex)
                {
                    _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                    return false;
                }
            }
            else
                return false;
        }

        private bool PaymentGP(ofWithdrawal row)
        {
            //if (row.cardbank.ToLower() == "购宝")
            if (row.cardbank.ToLower() == "gp" || row.cardbank.ToLower() == "gopay" || row.cardbank.ToLower() == "g币")
            {


                string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
                //string postUrl = "http://public.cgpay.io/api/v3/MerchantWithdraw";
                string returnUrl = "https://www.awcwin8.com";
                //string _merchantID = "3b52434bf9962e03b11acb2ba8efe0a1";
                //string _merhcnatKey = "a1636815de";

                string _merchantID = "100423";
                string _merhcnatKey = "tZ70FwocFbOLCMo2Bej6l09x8plAo2pD";
                string postUrl = "https://api.fafa0858.com";

                var stf = new Thirdpay.购富.S(_merchantID, _merhcnatKey, callbackUrl, postUrl)
                {
                    account = row.account,
                    returnUrl = returnUrl,
                    clientIPAddress = "1.2.2.2"
                };

                stf.proxy_address = "172.233.91.151";
                stf.proxy_port = 65531;

                try
                {
                    string orderid = stf.GenerateOrderID.Substring(0, 20);

                    float amountadjust = (float)row.amount;
                    //Random rand = new Random();
                    //float additional = rand.Next(1, 5);
                    //additional = additional / 100;
                    //amountadjust = amountadjust + additional;
                    var payResult = stf.SendPayment(orderid, amountadjust, Thirdpay.Common.BANKS.CGP, row.cardno, row.realname);
                    _logger.LogInformation("SendPayment - [GP] orginalamount:{2}, adjustedamount:{3}, result:{1}, response:{0}", stf.paymentResponse, payResult.ToString(), row.amount, amountadjust);
                    //_logger.LogInformation("SendPayment - OrderID:{2}, Result:{1}, Response:{0}", stf.paymentResponse, payResult.ToString(), orderid);
                    return UpdatePaymentLog(row.id, orderid, stf.paymentResponse, payResult);

                }
                catch (Exception ex)
                {
                    _logger.LogError("SendPayment - RowID:{1}, Exception:{0}", ex.Message, row.id);
                    return false;
                }
            }
            else
                return false;

        }

        private bool Payment壹圆USDTTRC20(ofWithdrawal row, float usdt_xrate)
        {
            string returnUrl = "https://www.awcwin8.com";

            string callbackUrl = "http://pay.allpayzs.com/Payment/Callback/0/v2";
            string _merchantID = "awc1000";
            string _merhcnatKey = "d3b52c365c9b9e563a3127682fa42aeb";
            //string postUrl = "https://awc3rdpay.azurewebsites.net/api/withdraw/RequestWithdraw";
            string postUrl = "http://cmsw5o4x7-awc3rdpay.azurewebsites.net/api/withdraw/RequestWithdraw";

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

            //stf.proxy_address = "103.125.216.18";
            //stf.proxy_port = 8888;
            //if (row.cardbank == "邮政储蓄") row.cardbank = "邮政银行";
            //Thirdpay.Common.BANKS destBank;
            //if (!Enum.TryParse<Thirdpay.Common.BANKS>(row.cardbank, out destBank))
            //    return false;
            var destBank = Thirdpay.Common.BANKS.USDTTRC;
            try
            {
                string orderid = stf.GenerateOrderID.Substring(0, 20);

                float amountadjust = (float)row.amount;
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




    }
}

