using AutoMapper;
using CMS.Data.Models;
using ShareEntity.OutputModels;
using Dapper.Contrib.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using ShareEntity.Models;

namespace CMS.Core.Application.网站管理.Queries {
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class PaymentGatewaySwitchV2INR : IRequest<PaymentGatewaySwitchV2INRReturn> {
        public string name { get; set; }
        public string group { get; set; }
        public bool OnOff { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class PaymentGatewaySwitchV2INRReturn : ResponseModel {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class PaymentGatewaySwitchV2INRValidator : AbstractValidator<PaymentGatewaySwitchV2INR> {
        public PaymentGatewaySwitchV2INRValidator() {
            //RuleFor(x => x.Id)
            //    .NotEmpty().WithMessage("ID不可为空")
            //    .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class PaymentGatewaySwitchV2INRHandler : BaseRequestHandler<PaymentGatewaySwitchV2INR, PaymentGatewaySwitchV2INRReturn> {

        public PaymentGatewaySwitchV2INRHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override PaymentGatewaySwitchV2INRReturn Handle(PaymentGatewaySwitchV2INR request) {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING)) {
                var directpay_details = connection.Get<ofSettings>("PAYEMENT_DIRECTPAY_V2_INR");
                JObject jobj = JObject.Parse(directpay_details.body);
                jobj["level_cat"][request.group]["payplatform"] = request.OnOff ? request.name : "";
                string jsonString = jobj.ToString(Formatting.None);
                directpay_details.body = jsonString;
                connection.Update(directpay_details);
                return new PaymentGatewaySwitchV2INRReturn() {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = jsonString
                };
            }

        }
    }
}
