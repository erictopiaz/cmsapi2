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
using ShareEntity.OutputModels;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using CMS.Core;
using CMS.Core.ViewModel;
using Dapper.Contrib.Extensions;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class ValidateCodeRecord : PagingModel, IRequest<ResponseModel<PagingModel<ValidateCodeRecordReturn>>>
    {
        public string account { get; set; }
        public int status { get; set; } = -1;
        public string body { get; set; }
        public string validate { get; set; }
        public int type { get; set; } = -1;
        public string admin { set; get; }

    }

    public class ValidateCodeRecordReturn
    {
        public long id { get; set; }
        public string account { get; set; }
        public int type { get; set; }
        public string typename { get; set; }
        public string validate { get; set; }
        public DateTime createtime { get; set; }
        public DateTime updatetime { get; set; }
        public DateTime? validatetime { get; set; }
        public int status { get; set; }
        public string ipaddress { get; set; }
        public string macaddress { get; set; }
        public string body { get; set; }
        public long TotalItems { get; set; }

        public string remark { get; set; } 
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class ValidateCodeRecordValidator : AbstractValidator<ValidateCodeRecord>
    {
        public ValidateCodeRecordValidator()
        {
            RuleFor(x => x.account).Account();
            RuleFor(x => x.status).Status();
            RuleFor(x => x.type).Status();
            RuleFor(x => x.validate).CharAndDigitalNullable(1,10);
            //RuleFor(x => x.body).CharAndDigitalNullable(1,10);

        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class ValidateCodeRecordHandler : BaseRequestHandler<ValidateCodeRecord, ResponseModel<PagingModel<ValidateCodeRecordReturn>>>
    {
        public ValidateCodeRecordHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<PagingModel<ValidateCodeRecordReturn>> Handle(ValidateCodeRecord request)
        {

            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT * , COUNT(*) OVER() as TotalItems " +
                "FROM ofAccountValidate WITH(NOLOCK) /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY");

            // FILL UP THE WHERE CLAUSE
            if (!string.IsNullOrEmpty(request.account)) builder.Where("account = @account", new { request.account });
            if (!string.IsNullOrEmpty(request.validate)) builder.Where("validate = @validate", new { request.validate });
            if (request.status >= 0) builder.Where("status = @status", new { request.status });
            if (request.type >= 0)
                builder.Where("type = @type", new { request.type });
            else
                builder.Where("type > -1000", new { request.type });

            if (!string.IsNullOrEmpty(request.body)) builder.Where("body = @body", new { request.body });

            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("createtime DESC");

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var rows = connection.Query<ValidateCodeRecordReturn>(template.RawSql, template.Parameters);
                var adminAccess = connection.Get<ofManage>(request.admin).access;

                bool showTelephone = false;
                bool showVerifyCode = false;
                if (adminAccess.Contains("/api/Account/AllowToSeePhone")) 
                    showTelephone = true;

                if (adminAccess.Contains("/api/Account/AllowToSeeVerifyCode", StringComparison.OrdinalIgnoreCase))
                    showVerifyCode = true;

                long totalItems = 0;
                // Apply the LevelName 
                if (rows.Count() > 0)
                {
                    totalItems = rows.First().TotalItems;
                    rows.ToList().ForEach(x =>
                    {
                        if (!showTelephone)
                        {
                            if(UtilsLib.Common.IsIncludedTel(x.account) || x.account == x.body)
                                x.account = Common.Security.ObscureString(x.account);
                        }
                        x.typename = CMS.Core.CachedData.ValidateCodeTypeDict[x.type];
                        if (x.body.Length > 5)
                            x.body = string.Format("{0}*****{1}",
                                x.body.Substring(0, 2),
                                x.body.Substring(x.body.Length - 2, 2));
                        if (showVerifyCode == false)
                        {
                            x.validate = string.Format("{0}**{1}",
                                   x.validate.Substring(0, 2),
                                   x.validate.Substring(x.validate.Length - 2, 2));
                        }

                    });
                }


                return new ResponseModel<PagingModel<ValidateCodeRecordReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new PagingModel<ValidateCodeRecordReturn>()
                    {
                        Data = rows,
                        TotalItems = totalItems
                    }
                };
            }
        }
    }
}

