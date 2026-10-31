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
using CMS.Core.ViewModel;
using CMS.Core.Application;
using ShareEntity.OutputModels;
using Microsoft.Extensions.Logging;
using Dapper.Contrib.Extensions;
using ShareEntity.Models;

namespace CMS.Core.Application.系统管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class ToggleUserStatus : IRequest<ResponseModel>
    {
        public string account { get; set; }
        public bool status { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }

    }


    public class ToggleUserStatusValidator : AbstractValidator<ToggleUserStatus>
    {
        public ToggleUserStatusValidator()
        {
            RuleFor(x => x.account).Account();
        }
    }



    /// <summary>
    /// Request Handler
    /// </summary>
    public class ToggleUserStatusHandler : BaseRequestHandler<ToggleUserStatus, ResponseModel>
    {
        public ToggleUserStatusHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel Handle(ToggleUserStatus request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var row = connection.Get<ofManage>(request.account);
                row.status = request.status ? 1 : 0 ;
                var isSuccess = connection.Update(row);

                return new ResponseModel()
                {
                    code = isSuccess ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = isSuccess ? "success" : "failed"
                };
            }
        }


    }
}

