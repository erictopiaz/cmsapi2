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
using static CMS.Core.Common;
using Microsoft.AspNetCore.Hosting;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 提款详细Query : IRequest<ResponseModel<提款详细Return>>
    {
        public long Id { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 提款详细Return
    {
        public long id { get; set; }
        public string account { get; set; }
        public string realname { get; set; }
        public decimal amount { get; set; }
        public string cardno { get; set; }
        public string cardaddress { get; set; }
        public string cardbank { get; set; }
        public DateTime requesttime { get; set; }
        public int status { get; set; }
        public string requestip { get; set; }
        public string requestaddress { get; set; }
        public DateTime? reviewtime { get; set; }
        public string reviewperson { get; set; }
        public DateTime? paytime { get; set; }
        public string payperson { get; set; }
        public DateTime? canceltime { get; set; }
        public string cancelperson { get; set; }
        public string cancelreason { get; set; }
        //public decimal? balance { get; set; }
        //public string referrer { get; set; }
        //public string referrerreg { get; set; }
        public int level { get; set; }
        //public string affaccount { get; set; }
        public string checkperson { get; set; }
        public string firstdeposit { get; set; }

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 提款详细QueryValidator : AbstractValidator<提款详细Query>
    {
        public 提款详细QueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 提款详细Handler : BaseWithdraw<提款详细Query, ResponseModel<提款详细Return>>
    {
        public 提款详细Handler(IMapper mapper, ILoggerFactory logger, IWebHostEnvironment env)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger, env) { }

        protected override ResponseModel<提款详细Return> Handle(提款详细Query request)
        {
            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT * " +
                "FROM ofWithdrawal WITH(NOLOCK) /**where**/ ");

            // FILL UP THE WHERE CLAUSE
            builder.Where("id = @Id", new { Id = request.Id });



            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var details = connection.QueryFirst<提款详细Return>(template.RawSql, template.Parameters);
                var firstDeposit = connection.QueryFirstOrDefault<DateTime>("SELECT TOP 1 requesttime FROM ofdeposit with(nolock) WHERE account = @ACCOUNT and status = 1 ORDER by ID DESC", new { ACCOUNT = details.account });
        
                details.cardno = "***";
                details.requestaddress = Network.GetLocation(details.requestip, _env.ContentRootPath + "//qqwry.dat");
                details.firstdeposit = firstDeposit == null ? "没首存" : firstDeposit.ToString("yyyy-MM-dd HH:mm"); 
                return new ResponseModel<提款详细Return>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = details
                };
            }
        }
    }
}

