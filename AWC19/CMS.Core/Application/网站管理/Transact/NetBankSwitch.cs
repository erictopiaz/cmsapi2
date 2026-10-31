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
    public class NetBankSwitch : IRequest<NetBankSwitchReturn>
    {
        public long Id { get; set; }
        public bool? OnOff { get; set; } 
        public bool? appOnOff { get; set; } 
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class NetBankSwitchReturn: ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class NetBankSwitchValidator : AbstractValidator<NetBankSwitch>
    {
        public NetBankSwitchValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class NetBankSwitchHandler : BaseRequestHandler<NetBankSwitch, NetBankSwitchReturn>
    {
        public NetBankSwitchHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override NetBankSwitchReturn Handle(NetBankSwitch request)
        {
            //var builder = new SqlBuilder();
            //var template = builder.AddTemplate("UPDATE ofNetBank SET status=@STATUS WHERE id=@ID");
            //builder.AddParameters(new { id = request.Id, status = request.OnOff ? 1 : 0 });
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                
                var id = request.Id;
                var row = connection.Get<ofNetbank_Brief>(id);
                if(request.OnOff != null) row.status = (bool)request.OnOff ? 1 : 0 ;
                if(request.appOnOff != null) row.appstatus = (bool)request.appOnOff ? 1 : 0 ;
                var x = connection.Update(row);

                return new NetBankSwitchReturn()
                {
                    code = x ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }

        }
    }
}

