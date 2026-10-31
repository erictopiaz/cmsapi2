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
using CMS.Core.Application.账号管理;
using Microsoft.Extensions.Logging;
using Dapper.Contrib.Extensions;
using ShareEntity.OutputModels;
using Org.BouncyCastle.Ocsp;
using ShareEntity.Models;

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 邮箱更新 : IRequest<邮箱更新Return>
    {
        public string id { get; set; }
        public string Account { get; set; }
        public string email { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 邮箱更新Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 邮箱更新Validator : AbstractValidator<邮箱更新>
    {
        public 邮箱更新Validator()
        {
            RuleFor(x => x.Account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 邮箱更新Handler : BaseAccount<邮箱更新, 邮箱更新Return>
    {
        public 邮箱更新Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger){}


        protected override 邮箱更新Return Handle(邮箱更新 request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                if (!string.IsNullOrEmpty(request.id))
                {
                        var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                        request.Account = accountRow.account;
                }
                var userDetail = connection.Get<ofUser>(request.Account);
                userDetail.email = request.email;

                bool updateResult = connection.Update<ofUser>(userDetail);
                return new 邮箱更新Return()
                {
                    code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = ""
                };
            }
        }
    }
}

