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
    public class PaymentGatewaySwitchV2 : IRequest<PaymentGatewaySwitchV2Return>
    {
        public string name { get; set; }
        public string group { get; set; }
        public bool OnOff { get; set; } 

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class PaymentGatewaySwitchV2Return: ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class PaymentGatewaySwitchV2Validator : AbstractValidator<PaymentGatewaySwitchV2>
    {
        public PaymentGatewaySwitchV2Validator()
        {
            //RuleFor(x => x.Id)
            //    .NotEmpty().WithMessage("ID不可为空")
            //    .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class PaymentGatewaySwitchV2Handler : BaseRequestHandler<PaymentGatewaySwitchV2, PaymentGatewaySwitchV2Return>
    {

        public PaymentGatewaySwitchV2Handler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override PaymentGatewaySwitchV2Return Handle(PaymentGatewaySwitchV2 request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));


            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var directpay_details = connection.Get<ofSettings>("PAYEMENT_DIRECTPAY_V2");
                JObject jobj = JObject.Parse(directpay_details.body);
                jobj["level_cat"][request.group]["payplatform"] = request.OnOff ? request.name : "";
                string jsonString = jobj.ToString(Formatting.None);
                directpay_details.body = jsonString;
                connection.Update(directpay_details);
                return new PaymentGatewaySwitchV2Return()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = jsonString
                };
            }

        }
    }
}

