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
using Microsoft.Extensions.Logging;
using ShareEntity.OutputModels;
using Dapper.Contrib.Extensions;
using ShareEntity.Models;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 提款卡号查询 : IRequest<ResponseModel<提款卡号查询Return>>
    {
        public string Id { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 提款卡号查询Return
    {
        public string cardno { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 提款卡号查询Validator : AbstractValidator<提款卡号查询>
    {
        public 提款卡号查询Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空");
            //.GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 提款卡号查询Handler : BaseWithdraw<提款卡号查询, ResponseModel<提款卡号查询Return>>
    {
        public 提款卡号查询Handler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<提款卡号查询Return> Handle(提款卡号查询 request)
        {
            //// 创建DAPPER TEMPLATE 
            //var builder = new SqlBuilder();
            //var template = builder.AddTemplate("" +
            //    "SELECT amount " +
            //    "FROM ofWithdrawal WITH(NOLOCK) /**where**/ ");

            //// FILL UP THE WHERE CLAUSE
            //builder.Where("id = @Id", new { Id = request.Id });

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                //var details = connection.QueryFirst<提款卡号查询Return>(template.RawSql, template.Parameters);

                var adminAccess = connection.Get<ofManage>(request.Admin).access;

                bool show = false;
                bool hidesbankcard = false;
                if (adminAccess.Contains("/api/Withdraw/HidesBankCard")) hidesbankcard = true;


                var withdrawRow = connection.Get<ofWithdrawal>(request.Id);

                if (!withdrawRow.cardbank.ToLower().Contains("usdt") && hidesbankcard)
                {

                    return new ResponseModel<提款卡号查询Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "没权限显示银行卡",
                        Data = new 提款卡号查询Return
                        {
                            cardno = withdrawRow.cardno
                        }
                    };
                }

                return new ResponseModel<提款卡号查询Return>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new 提款卡号查询Return
                    {
                        cardno = withdrawRow.cardno
                    }
                };
            }
        }
    }
}

