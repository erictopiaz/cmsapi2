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
    public class DirectPayTypeSwitch : IRequest<DirectPayTypeSwitchReturn> {
        public int type { get; set; }
       
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class DirectPayTypeSwitchReturn : ResponseModel {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class DirectPayTypeSwitchValidator : AbstractValidator<DirectPayTypeSwitch> {
        public DirectPayTypeSwitchValidator() {
            //RuleFor(x => x.Id)
            //    .NotEmpty().WithMessage("ID不可为空")
            //    .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class DirectPayTypeSwitchHandler : BaseRequestHandler<DirectPayTypeSwitch, DirectPayTypeSwitchReturn> {

        public DirectPayTypeSwitchHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override DirectPayTypeSwitchReturn Handle(DirectPayTypeSwitch request) {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING)) {
                var directpay_details = connection.Get<ofSettings>("inr_dai_fu_exchange");
                JObject jobj = JObject.Parse(directpay_details.body);

                jobj["current"] = request.type;
                string jsonString = jobj.ToString(Formatting.None);
                directpay_details.body = jsonString;
                connection.Update(directpay_details);
                return new DirectPayTypeSwitchReturn() {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = jsonString
                };
            }

        }
    }
}
