using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using ShareEntity.Models;
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

namespace CMS.Core.Application.优惠管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 红利优惠列表 : PagingModel, IRequest<ResponseModel<PagingModel<红利优惠列表Return>>>
    {
        public string account { get; set; }
        public string remarks { get; set; }
        public int? gametype { get; set; }
        public int status { get; set; } = -1;
        public string marcode { get; set; }
        public DateTime? starttime { get; set; }
        public DateTime? endtime { get; set; }
        public string ipaddress { get; set; }
        public string macaddress { get; set; }
        public string affaccount { get; set; }
    }

    public class 红利优惠列表Return
    {
        public long id { get; set; }
        //public string currency { get; set; }
        public string account { get; set; }
        public decimal amount { get; set; }
        public decimal reward { get; set; }
        public DateTime createtime { get; set; }
        public string remarks { get; set; }
        public int level { get; set; }
        public string levelname { get; set; }
        public int bet { get; set; }
        public string createperson { get; set; }
        public string reviewperson { get; set; }
        public DateTime? reviewtime { get; set; }
        public int? gametype { get; set; }
        public string gametypename { get; set; }
        public string referrer { get; set; }
        public string referrerreg { get; set; }
        //public string referenceguid { get; set; }
        public int status { get; set; }
        public int visible { get; set; }
        public string ipaddress { get; set; }
        public string macaddress { get; set; }
        //public int alert { get; set; }
        public string reason { get; set; }
        public long TotalItems { get; set; }

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 红利优惠列表QueryValidator : AbstractValidator<红利优惠列表>
    {
        public 红利优惠列表QueryValidator()
        {
            //RuleFor(x => x.Account).Account();
            //RuleFor(x => x.CardNo).CardNo();
            //RuleFor(x => x.Bank).CharAndDigital(1, 10);
            //RuleFor(x => x.VerifyTime).Datetime();
            //RuleFor(x => x.Status).Status();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 红利优惠列表Handler : BaseRequestHandler<红利优惠列表, ResponseModel<PagingModel<红利优惠列表Return>>>
    {
        public 红利优惠列表Handler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<PagingModel<红利优惠列表Return>> Handle(红利优惠列表 request)
        {
            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT * , COUNT(*) OVER() as TotalItems " +
                "FROM ofPromotions WITH(NOLOCK) /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY");

            //var template = builder.AddTemplate("" +
            //"SELECT  TransactionType = 'bonuspromo', amount,a=remarks,b,c,status,orderendtime FROM(" +
            //"SELECT reward AS amount, remarks, bet AS b, c=amount,status, reviewtime AS orderendtime " +
            //    "FROM ofBonusPromo WITH(NOLOCK) " +
            //    "WHERE reviewtime >= @STARTTIME AND account=@ACCOUNT UNION ALL " +
            //"SELECT amount, remarks, b=0,c=0,status, reviewtime AS orderendtime " +
            //    "FROM ofPromotions WITH(NOLOCK) " +
            //    "WHERE reviewtime >= @STARTTIME AND account=@ACCOUNT)z " +
            //    "ORDER BY orderendtime DESC");

            //            public string account { get; set; }
            //public string remarks { get; set; }
            //public int? gametype { get; set; }
            //public int status { get; set; } = -1;
            //public string marcode { get; set; }
            //public DateTime starttime { get; set; }
            //public DateTime endtime { get; set; }
            //public string ipaddress { get; set; }
            //public string macaddress { get; set; }
            //public string affaccount { get; set; }



            // FILL UP THE WHERE CLAUSE
            // UNCOMMENT if need to Limit to 3Months
            if (request.starttime == null /*|| request.starttime < DateTime.UtcNow.AddHours(8).AddMonths(-3)*/)
                builder.Where("createtime >= @REQUESTTIME", new { REQUESTTIME = DateTime.UtcNow.AddHours(8).AddMonths(-3) });
            else
                builder.Where("createtime >= @REQUESTTIME", new { REQUESTTIME = request.starttime });


            // Others
            if (!string.IsNullOrEmpty(request.account)) builder.Where("account = @account", new { request.account });
            //if (!string.IsNullOrEmpty(request.gametype)) builder.Where("gametype = @gametype", new { request.gametype });
            if (request.gametype > 0) builder.Where("gametype = @gametype", new { request.gametype });
            if (!string.IsNullOrEmpty(request.macaddress)) builder.Where("macaddress = @macaddress", new { request.macaddress });
            if (request.status >= 0) builder.Where("Status = @Status", new { request.status });
            if (!string.IsNullOrEmpty(request.remarks)) builder.Where("remarks LIKE @REMARKS", new { remarks = string.Concat('%', request.remarks, '%') });

            // PAGING FUNCTION
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("createtime DESC");


            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var rows = connection.Query<红利优惠列表Return>(template.RawSql, template.Parameters);
                long totalItems = 0;
                if (rows.Count() > 0)
                {
                    totalItems = rows == null ? 0 : rows.First().TotalItems;

                    rows.ToList().ForEach(x =>
                    {
                        x.levelname = CMS.Core.CachedData.LevelNameDict[x.level];
                        x.gametypename = CMS.Core.CachedData.GameCodeDict[x.gametype ?? 0];
                    });
                }


                var result = new ResponseModel<PagingModel<红利优惠列表Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                    Data = new PagingModel<红利优惠列表Return>()
                    {
                        TotalItems = totalItems,
                        Data = rows
                    }
                };

                // TODO: Optimization here
                return result;
            }
        }
    }
}

