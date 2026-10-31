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
    public class GetDiffUserAccessMenu : IRequest<ResponseModel<GetDiffUserAccessMenuReturn>>
    {
        public string Account { get; set; }

    }

    public class GetDiffUserAccessMenuValidator : AbstractValidator<GetDiffUserAccessMenu>
    {
        public GetDiffUserAccessMenuValidator()
        {

            RuleFor(x => x.Account).Account();
        }
    }



    /// <summary>
    /// Request Handler
    /// </summary>
    //public class GetDiffUserAccessPrivilegeHandler : BaseActions, IRequestHandler<GetDiffUserAccessPrivilege, ResponseModel<IEnumerable<string>>>
    //{
    //    public GetDiffUserAccessPrivilegeHandler(CMS.Data.MainDB context, IMapper mapper, ILogger logger) : base(context, mapper, logger) { }
    //    public Task<ResponseModel<IEnumerable<string>>> Handle(GetDiffUserAccessPrivilege request, CancellationToken cancellationToken)
    //    {
    public class GetDiffUserAccessMenuReturn
    {
        public IEnumerable<GetApiAccessListReturn> allowedpath { get; set; }
        public IEnumerable<GetUserMenuReturn> menu { get; set; }
    }

    public class GetDiffUserAccessPrivilegeHandler : CMS.Core.BaseRequestHandler<GetDiffUserAccessMenu, ResponseModel<GetDiffUserAccessMenuReturn>>
    {
        public GetDiffUserAccessPrivilegeHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<GetDiffUserAccessMenuReturn> Handle(GetDiffUserAccessMenu request)
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
                var result = connection.QueryFirstOrDefault<string>(template.RawSql, template.Parameters);
                var accessallowedpath = new List<string>();
                var accessmenu = new List<string>();
                var menu = new List<GetUserMenuReturn>();

                if (!string.IsNullOrEmpty(result))
                {
                    var access = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(result);
                    accessallowedpath.AddRange(access["allowedpath"].Split('|'));

                    accessmenu.AddRange(access["menu"].Split('|'));
                    
                    foreach (var item in CMS.Core.Application.系统管理.Queries.GetUserMenuHandler.MENULIST)
                    {
                        string _group = "";
                        if (item.Key.IndexOf('/', 1) > 0)
                            _group = item.Key.Substring(1, item.Key.IndexOf('/', 1));
                        else
                            _group = item.Key.Substring(1);
                        _group = _group.Replace("/", "");
                        // Extract GROUP
                        menu.Add(new GetUserMenuReturn()
                        {
                            menukey = item.Key,
                            menuname = item.Value,
                            group = _group,
                            status = accessmenu.Contains(item.Key)
                        });
                    }
                }



                var responseData = new ResponseModel<GetDiffUserAccessMenuReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new GetDiffUserAccessMenuReturn() {
                        //allowedpath = accessallowedpath, 
                        menu = menu
                    }
                };
                return responseData;
            }
        }


    }
}

