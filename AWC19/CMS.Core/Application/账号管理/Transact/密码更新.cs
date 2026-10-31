using AutoMapper;
using AWC2019.Core.Validation;
using AWC2019.Core.ViewModel;
using CMS.Core.Application.账号管理;
using CMS.Data.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using ShareEntity.Models;
using ShareEntity.OutputModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 密码更新 : IRequest<密码更新Return>
    {
        public string id { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 密码更新Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 密码更新Validator : AbstractValidator<密码更新>
    {
        public 密码更新Validator()
        {
            RuleFor(x => x.Account).Account();
            RuleFor(x => x.Password).Password();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 密码更新Handler : BaseAccount<密码更新, 密码更新Return>
    {
        public 密码更新Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override 密码更新Return Handle(密码更新 request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                _logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(request));
                if (!string.IsNullOrEmpty(request.id))
                {
                    var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                    request.Account = accountRow.account;
                }


                var userDetail = connection.Get<ofUser>(request.Account);
                //userDetail.passkey = Common.Security.PasskeyEncrypt(request.Password);
                userDetail.password = Common.Security.DoubleMD5Hash(request.Password);

                bool updateResult = connection.Update<ofUser>(userDetail);
                return new 密码更新Return()
                {
                    code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = ""
                };
            }
        }
    }
}

