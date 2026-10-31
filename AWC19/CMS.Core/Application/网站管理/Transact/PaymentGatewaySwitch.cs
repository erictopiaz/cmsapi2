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
using ShareEntity.OutputModels;
using System.Runtime.Serialization;
using System.Transactions;
using Microsoft.Extensions.Logging;
using CMS.Core;
using NSwag.Annotations;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class PaymentGatewaySwitch : IRequest<PaymentGatewaySwitchReturn>
    {
        public string name { get; set; }
        public string group { get; set; }
        public bool OnOff { get; set; } 

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class PaymentGatewaySwitchReturn: ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class PaymentGatewaySwitchValidator : AbstractValidator<PaymentGatewaySwitch>
    {
        public PaymentGatewaySwitchValidator()
        {
            RuleFor(x => x.name).CharDigitalChineseNullable(1, 10);
            RuleFor(x => x.group).CharDigitalChineseNullable(1, 10);
            //RuleFor(x => x.Id)
            //    .NotEmpty().WithMessage("ID不可为空")
            //    .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class PaymentGatewaySwitchHandler : BaseRequestHandler<PaymentGatewaySwitch, PaymentGatewaySwitchReturn>
    {

        public PaymentGatewaySwitchHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override PaymentGatewaySwitchReturn Handle(PaymentGatewaySwitch request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {

                bool updateresult = false;
                switch (request.group)
                {
                    case "newuser":
                        var newuserplatform = connection.Get<ofSettings>("PAYMENT_PLATFORM_NEWUSER");
                        if (newuserplatform.body == request.name && request.OnOff == false)
                            newuserplatform.body = "";
                        else
                            newuserplatform.body = request.name;
                        updateresult  = connection.Update<ofSettings>(newuserplatform);
                        break;
                    case "newuser_top":
                        var newuserplatform_top = connection.Get<ofSettings>("PAYMENT_PLATFORM_NEWUSER_TOP");
                        if (newuserplatform_top.body == request.name && request.OnOff == false)
                            newuserplatform_top.body = "";
                        else
                            newuserplatform_top.body = request.name;
                        updateresult = connection.Update<ofSettings>(newuserplatform_top);
                        break;
                    case "regular_low":
                        var platform = connection.Get<ofSettings>("PAYMENT_PLATFORM");
                        if (platform.body == request.name && request.OnOff == false)
                            platform.body = "";
                        else
                            platform.body = request.name;
                        updateresult  = connection.Update<ofSettings>(platform);
                        break;
                    case "regular_top":
                        var regular_top_platform = connection.Get<ofSettings>("PAYMENT_PLATFORM_TOP");
                        if (regular_top_platform.body == request.name && request.OnOff == false)
                            regular_top_platform.body = "";
                        else
                            regular_top_platform.body = request.name;
                        updateresult = connection.Update<ofSettings>(regular_top_platform);
                        break;
                    case "vip":
                        var platformvip = connection.Get<ofSettings>("PAYMENT_PLATFORM_VIP");
                        if (platformvip.body == request.name && request.OnOff == false)
                            platformvip.body = "";
                        else
                            platformvip.body = request.name;
                        updateresult = connection.Update<ofSettings>(platformvip);
                        break;
                    case "vip_low":
                        var platformviplow = connection.Get<ofSettings>("PAYMENT_PLATFORM_VIP");
                        if (platformviplow.body == request.name && request.OnOff == false)
                            platformviplow.body = "";
                        else
                            platformviplow.body = request.name;
                        updateresult = connection.Update<ofSettings>(platformviplow);
                        break;
                    case "vip_top":
                        var platformviptop = connection.Get<ofSettings>("PAYMENT_PLATFORM_VIP_TOP");
                        if (platformviptop.body == request.name && request.OnOff == false)
                            platformviptop.body = "";
                        else
                            platformviptop.body = request.name;
                        updateresult = connection.Update<ofSettings>(platformviptop);
                        break;
                    case "vip3_low":
                        var platformvip3low = connection.Get<ofSettings>("PAYMENT_PLATFORM_VIP3");
                        if (platformvip3low.body == request.name && request.OnOff == false)
                            platformvip3low.body = "";
                        else
                            platformvip3low.body = request.name;
                        updateresult = connection.Update<ofSettings>(platformvip3low);
                        break;
                    case "vip3_top":
                        var platformvip3top = connection.Get<ofSettings>("PAYMENT_PLATFORM_VIP3_TOP");
                        if (platformvip3top.body == request.name && request.OnOff == false)
                            platformvip3top.body = "";
                        else
                            platformvip3top.body = request.name;
                        updateresult = connection.Update<ofSettings>(platformvip3top);
                        break;

                    case "vip3":
                        var platformvip3 = connection.Get<ofSettings>("PAYMENT_PLATFORM_VIP3");
                        if (platformvip3.body == request.name && request.OnOff == false)
                            platformvip3.body = "";
                        else
                            platformvip3.body = request.name;
                        updateresult = connection.Update<ofSettings>(platformvip3);
                        break;
                    case "aff":
                        var paymentdirectpaydetails = connection.Get<ofSettings>("PAYMENT_DIRECTPAYDETAILS");
                        var paymentdirectpaydetails_JOBJ = JObject.Parse(paymentdirectpaydetails.body);

                        if (paymentdirectpaydetails_JOBJ["aff_payplatform"].ToString() == request.name && request.OnOff == false)
                            paymentdirectpaydetails_JOBJ["aff_payplatform"] = "";
                        else
                            paymentdirectpaydetails_JOBJ["aff_payplatform"] = request.name;

                        paymentdirectpaydetails.body = paymentdirectpaydetails_JOBJ.ToString(Formatting.None);
                        updateresult = connection.Update<ofSettings>(paymentdirectpaydetails);
                        break;

                    default:
                        break;
                }

                return new PaymentGatewaySwitchReturn()
                {
                    code = updateresult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }

        }
    }
}

