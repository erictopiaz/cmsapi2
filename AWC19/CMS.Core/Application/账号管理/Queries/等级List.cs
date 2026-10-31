using AutoMapper;
using CMS.Core.ViewModel;
using CMS.Data.Models;
using ShareEntity.OutputModels;
using Dapper;
using Dapper.Contrib.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CMS.Core.Application.账号管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 等级List : PagingModel, IRequest<ResponseModel<IEnumerable<等级ListReturn>>>
    {
    }

    public class 等级ListReturn
    {
        public int levelnumber { get; set; }
        public string levels { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 等级ListValidator : AbstractValidator<等级List>
    {
        public 等级ListValidator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 等级ListHandler : BaseRequestHandler<等级List, ResponseModel<IEnumerable<等级ListReturn>>>
    {
        public 等级ListHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<IEnumerable<等级ListReturn>> Handle(等级List request)
        {
            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT * " +
                "FROM ofUserGroup WITH(NOLOCK)");

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var rows = connection.Query<等级ListReturn>(template.RawSql, template.Parameters);
                var rowsSettings = connection.Query<string>("" +
                    "SELECT body " +
                    "FROM ofSettings WITH(NOLOCK) " +
                    "WHERE code = 'APP_SETTINGS'");


                return new ResponseModel<IEnumerable<等级ListReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = rows
                };
            }
        }
    }
}
