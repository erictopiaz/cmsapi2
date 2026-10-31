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
using ShareEntity.OutputModels;
using Microsoft.Extensions.Logging;

namespace CMS.Core.Application.存款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 第三方平台 : PagingModel, IRequest<ResponseModel<IEnumerable<第三方平台Return>>>
    {
    }

    public class 第三方平台Return
    {
        public long id { get; set; }
        public string name { get; set; } 

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 第三方平台Validator : AbstractValidator<第三方平台>
    {
        public 第三方平台Validator()
        {
            RuleFor(x => x.Page).Page();
            RuleFor(x => x.PageSize).PageSize();
            //RuleFor(x => x.CardNo).CardNo();
            //RuleFor(x => x.Bank).CharAndDigital(1, 10);
            //RuleFor(x => x.VerifyTime).Datetime();
            //RuleFor(x => x.Status).Status();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 第三方平台Handler : BaseWithdraw<第三方平台, ResponseModel<IEnumerable<第三方平台Return>>>
    {
        public 第三方平台Handler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<IEnumerable<第三方平台Return>> Handle(第三方平台 request)
        {
            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            var template = builder.AddTemplate("SELECT id,name FROM ofThirdpay WITH(NOLOCK)");


            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var rows = connection.Query<第三方平台Return>(template.RawSql, template.Parameters);

                var result = new ResponseModel<IEnumerable<第三方平台Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                    Data = rows
                };

                // TODO: Optimization here
                return result;
            }
        }
    }
}

