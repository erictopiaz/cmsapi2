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
    public class GetManageGroup : IRequest<ResponseModel<IEnumerable<string>>> { }



    public class GetManageGroupHandler : CMS.Core.BaseRequestHandler<GetManageGroup, ResponseModel<IEnumerable<string>>>
    {
        public GetManageGroupHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<IEnumerable<string>> Handle(GetManageGroup request)
        {
            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT code " +
                "FROM ofManageGroup WITH(NOLOCK) /**where**/ ");

            // FILL UP THE WHERE CLAUSE
            builder.Where("class = 1");
            builder.Where("gamestatus = 1");
            builder.Where("code NOT IN @EXCLUDENAME", new
                {
                    EXCLUDENAME = new List<string>() {
                    "manager"
                }
            });

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var result = connection.Query<string>(template.RawSql, template.Parameters);
                var resultArray = result.ToArray();

                var responseData = new ResponseModel<IEnumerable<string>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = resultArray
                };

                return responseData;
            }
        }

    }
}

