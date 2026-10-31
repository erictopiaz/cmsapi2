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
using ShareEntity.Models;

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 姓名更新 : IRequest<姓名更新Return>
    {
        public string id { get; set; }
        public string Account { get; set; }
        public string realname { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 姓名更新Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 姓名更新Validator : AbstractValidator<姓名更新>
    {
        public 姓名更新Validator()
        {
            RuleFor(x => x.Account).Account();
            RuleFor(x => x.realname).Realname();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 姓名更新Handler : BaseAccount<姓名更新, 姓名更新Return>
    {
        public 姓名更新Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger){}


        protected override 姓名更新Return Handle(姓名更新 request)
        {
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                if (!string.IsNullOrEmpty(request.id))
                {
                        var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                        request.Account = accountRow.account;
                }
                var userDetail = connection.Get<ofUser>(request.Account);
                userDetail.realname = request.realname;
                bool updateResult = connection.Update<ofUser>(userDetail);
                return new 姓名更新Return()
                {
                    code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = ""
                };
            }
        }
    }
}

