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

namespace CMS.Core.Application.系统管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class GetAccessPrivilege : IRequest<ResponseModel<IEnumerable<string>>>
    {
        public string Account { get; set; }

    }


    public class GetAccessPrivilegeValidator : AbstractValidator<GetAccessPrivilege>
    {
        public GetAccessPrivilegeValidator()
        {

            RuleFor(x => x.Account).Account();
        }
    }


    /// <summary>
    /// Request Handler
    /// </summary>
    //public class GetAccessPrivilegeHandler : BaseActions, IRequestHandler<GetAccessPrivilege, ResponseModel<IEnumerable<string>>>
    //{
    //    public GetAccessPrivilegeHandler(CMS.Data.MainDB context, IMapper mapper, ILogger logger) : base(context, mapper, logger) { }
    //    public Task<ResponseModel<IEnumerable<string>>> Handle(GetAccessPrivilege request, CancellationToken cancellationToken)
    //    {


    public class GetAccessPrivilegeHandler : CMS.Core.BaseRequestHandler<GetAccessPrivilege, ResponseModel<IEnumerable<string>>>
    {
        public GetAccessPrivilegeHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<IEnumerable<string>> Handle(GetAccessPrivilege request)
        {

            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT access " +
                "FROM ofManage /**where**/ ");

            // FILL UP THE WHERE CLAUSE
            if (!string.IsNullOrEmpty(request.Account)) builder.Where("Account = @Account", new { request.Account });
            builder.Where("status = 1");

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var result  = connection.QueryFirstOrDefault<string>(template.RawSql, template.Parameters);
                var data = new List<string>();

                if (!string.IsNullOrEmpty(result))
                {
                    var access = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(result);
                    data.AddRange(access["allowedpath"].Split('|'));
                }


                var responseData = new ResponseModel<IEnumerable<string>>() {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = data
                };
                return responseData;
            }
        }


    }
}

