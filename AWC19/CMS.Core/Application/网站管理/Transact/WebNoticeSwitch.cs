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
    public class WebNoticeSwitch : IRequest<WebNoticeSwitchReturn>
    {
        public long Id { get; set; }
        public bool OnOff { get; set; } 
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class WebNoticeSwitchReturn: ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class WebNoticeSwitchValidator : AbstractValidator<WebNoticeSwitch>
    {
        public WebNoticeSwitchValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class WebNoticeSwitchHandler : BaseRequestHandler<WebNoticeSwitch, WebNoticeSwitchReturn>
    {
        public WebNoticeSwitchHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override WebNoticeSwitchReturn Handle(WebNoticeSwitch request)
        {
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var id = request.Id;
                var notice = connection.Get<ofNotice>(id);
                notice.status = request.OnOff ? 1 : 0 ;
                var x = connection.Update(notice);

                return new WebNoticeSwitchReturn()
                {
                    code = x ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }

        }
    }
}

