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
    public class 电话更新 : IRequest<电话更新Return>
    {
        public string id { get; set; }
        public string Account { get; set; }
        public string telephone { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 电话更新Return : ResponseModel
    {
        public string prePhone { get; set; } = "";
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 电话更新Validator : AbstractValidator<电话更新>
    {
        public 电话更新Validator()
        {
            RuleFor(x => x.Account).Account();
            RuleFor(x => x.telephone).CharAndDigitalNullable(1, 18);
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 电话更新Handler : BaseAccount<电话更新, 电话更新Return>
    {
        public 电话更新Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger){}


        protected override 电话更新Return Handle(电话更新 request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));
            request.telephone = request.telephone.Trim();
            if(string.IsNullOrEmpty(request.telephone))
                return new 电话更新Return()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "不允许空白的电话"
                };
            var prePhone = "";
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                if (!string.IsNullOrEmpty(request.id))
                {
                        var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                        request.Account = accountRow.account;
                }
                var userDetail = connection.Get<ofUser>(request.Account);
                prePhone = userDetail.telephone;
                userDetail.telephone = request.telephone;
                bool updateResult = connection.Update<ofUser>(userDetail);
                return new 电话更新Return()
                {
                    code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    prePhone = prePhone
                };
            }
        }
    }
}

