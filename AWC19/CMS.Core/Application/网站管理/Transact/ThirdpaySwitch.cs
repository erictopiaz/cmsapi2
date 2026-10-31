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
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class ThirdpaySwitch : IRequest<ThirdpaySwitchReturn>
    {
        public long Id { get; set; }
        public bool OnOff { get; set; } 
        public bool AppOnOff { get; set; } 
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class ThirdpaySwitchReturn: ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class ThirdpaySwitchValidator : AbstractValidator<ThirdpaySwitch>
    {
        public ThirdpaySwitchValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class ThirdpaySwitchHandler : BaseRequestHandler<ThirdpaySwitch, ThirdpaySwitchReturn>
    {
        public ThirdpaySwitchHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ThirdpaySwitchReturn Handle(ThirdpaySwitch request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var id = request.Id;
                var row = connection.Get<ofThirdpay_WithoutClass>(id);
                row.status = request.OnOff ? 1 : 0 ;
                row.appstatus = request.AppOnOff ? 1 : 0;
                var x = connection.Update(row);

                return new ThirdpaySwitchReturn()
                {
                    code = x ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }

        }
    }
}

