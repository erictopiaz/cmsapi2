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
    public class ThirdpayNewUserSwitch : IRequest<ThirdpayNewUserSwitchReturn>
    {
        public long Id { get; set; }
        public bool OnOff { get; set; }
        public bool AppOnOff { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class ThirdpayNewUserSwitchReturn : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class ThirdpayNewUserSwitchValidator : AbstractValidator<ThirdpayNewUserSwitch>
    {
        public ThirdpayNewUserSwitchValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class ThirdpayNewUserSwitchHandler : BaseRequestHandler<ThirdpayNewUserSwitch, ThirdpayNewUserSwitchReturn>
    {
        public ThirdpayNewUserSwitchHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ThirdpayNewUserSwitchReturn Handle(ThirdpayNewUserSwitch request)
        {

            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var id = request.Id;
                var row = connection.Get<ofThirdpay_WithoutClass>(id);
                string miscJobj = row.misc;
                JObject jobj = new JObject();
                if (!string.IsNullOrEmpty(miscJobj))
                    jobj = JObject.Parse(miscJobj);
                else
                {
                    return new ThirdpayNewUserSwitchReturn()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = ""
                    };
                }
                jobj["newusers"] = request.OnOff ? 1: 0;

                miscJobj = jobj.ToString(Formatting.None);
                row.misc = miscJobj;
                bool updateresult = connection.Update(row);

                return new ThirdpayNewUserSwitchReturn()
                {
                    code = updateresult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }
        }
    }
}
