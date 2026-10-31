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
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Http;

namespace CMS.Core.Application.存款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 内充列表 : PagingModel, IRequest<ResponseModel<PagingModel<内充Return>>>
    {
        //账户 存款姓名    收款姓名 存款类型   代理 IP 开始时间 结束时间 { 查询/导出 }

        public string account { get; set; }
        public string depositname { get; set; }
        public string receivename { get; set; }
        public string deposittype { get; set; }
        public string depositmethod { get; set; }
        public string affaccount { get; set; }
        public string ipaddress { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public int status { get; set; } = -1;
    }

    public class 内充Return : ofDepositInternal
    {
        public string payplatformname { get; set; }
        public long TotalItems { get; set; }
        public decimal TotalFee { get; set; }
        public decimal TotalAmount { get; set; }

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 内充QueryValidator : AbstractValidator<内充列表>
    {
        public 内充QueryValidator()
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
    public class 内充Handler : BaseWithdraw<内充列表, ResponseModel<PagingModel<内充Return>>>
    {
        public 内充Handler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<PagingModel<内充Return>> Handle(内充列表 request)
        {
            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            bool withWhere = false;
            bool withTimeRange = false;

            //// FILL UP THE WHERE CLAUSE
            if (request.starttime != null && request.endtime != null) { 
                if(request.starttime.Year > 2019 && request.endtime.Year > 2019)
                builder.Where("requesttime >= @starttime AND requesttime <= @endtime", new { request.starttime, request.endtime });
                withTimeRange = true;
            }

            //else
            //    builder.Where("requesttime BETWEEN @starttime AND @endtime", new { request.starttime, request.endtime });

            if (request.status >= 0) { 
                builder.Where("Status = @Status", new { request.status });
                withWhere = true;
            }

            //if (request.starttime.Year > 2010 && request.endtime.Year > 2010)
            //{

            //    withWhere = true;
            //    withTimeRange = request.status == 1;
            //}
            //else if (request.starttime.Year > 2010)
            //{
            //    withWhere = true;
            //    withTimeRange = request.status == 1;
            //    builder.Where("requesttime >= @starttime", new { starttime = request.starttime });
            //}
            //else // Limit to 3 Months
            //    builder.Where("requesttime >= @starttime", new { starttime = Common.CurrentTime.AddDays(-7) });


            //builder.Where("deposittype > 0");
            //if (!string.IsNullOrEmpty(request.account)) builder.Where("account LIKE @account", new { account = string.Concat("%", request.account, "%") });
            //if (!string.IsNullOrEmpty(request.depositname)) { withWhere = true; builder.Where("realname = @DepositName", new { request.depositname }); }
            //if (!string.IsNullOrEmpty(request.receivename)) { withWhere = true; builder.Where("receiveperson = @ReceiveName", new { request.receivename }); }
            //if (!string.IsNullOrEmpty(request.affaccount)) { withWhere = true; builder.Where("affaccount = @affaccount", new { request.affaccount }); }
            //if (!string.IsNullOrEmpty(request.ipaddress)) { withWhere = true; builder.Where("ipaddress = @ipaddress", new { request.ipaddress }); }
            //if (!string.IsNullOrEmpty(request.deposittype)) { withWhere = true; builder.Where("deposittype = @deposittype", new { request.deposittype }); }
            //if (!string.IsNullOrEmpty(request.depositmethod)) { withWhere = true; builder.Where("depositmethod = @depositmethod", new { request.depositmethod }); }
            //if (request.status >= 0)
            //    builder.Where("Status = @Status", new { request.status });
            //else
            //    builder.Where("Status < 2");

            //// PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("requesttime DESC");

            SqlBuilder.Template template;
            //if (withWhere && withTimeRange)
            //    template = builder.AddTemplate("" +
            //    "SELECT a.*, ofUser.level " +
            //    "FROM (SELECT * , " +
            //        "COUNT(*) OVER() as TotalItems, " +
            //        "SUM(amount) OVER() as TotalAmount, " +
            //        "SUM(fee) OVER() as TotalFee " +
            //    "FROM ofDeposit WITH(NOLOCK) /**where**/ /**orderby**/ " +
            //    "OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY)a " +
            //    "INNER JOIN ofuser WITH(NOLOCK) ON a.account = ofuser.account");

            //else if (withWhere)
            //    template = builder.AddTemplate("" +
            //    "SELECT a.*, ofUser.level " +
            //    "FROM (SELECT * , COUNT(*) OVER() as TotalItems FROM ofDeposit WITH(NOLOCK) /**where**/ /**orderby**/ " +
            //    "OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY)a " +
            //    "INNER JOIN ofuser WITH(NOLOCK) ON a.account = ofuser.account");

            //else
            //    template = builder.AddTemplate("" +
            //    "SELECT a.*, ofUser.level " +
            //    "FROM (SELECT * , TotalItems=10000 FROM ofDeposit WITH(NOLOCK) /**where**/ /**orderby**/ " +
            //    "OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY)a " +
            //    "INNER JOIN ofuser WITH(NOLOCK) ON a.account = ofuser.account");

            if (withWhere)
                template = builder.AddTemplate("" +
                    "SELECT *, COUNT(*) OVER() as TotalItems, SUM(amount) OVER() as TotalAmount FROM ofDepositInternal WITH(NOLOCK) /**where**/ /**orderby**/" +
                    "OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY");
            else
                template = builder.AddTemplate("" +
                    "SELECT *, COUNT(*) OVER() as TotalItems FROM ofDepositInternal WITH(NOLOCK) /**orderby**/" +
                    "OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY");


            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                //var rows = connection.GetAll<内充Return>();
                var rows = connection.Query<内充Return>(template.RawSql, template.Parameters);

                rows.ToList().ForEach(x =>
                {
                    x.payplatformname = CMS.Core.CachedData.PayPlatformDict[x.payplatform.ToString()];
                });

                long totalItems = 0;
                if (rows.Count() > 0)
                    totalItems = rows.First().TotalItems;



                var result = new ResponseModel<PagingModel<内充Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                    Data = new PagingModel<内充Return>()
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

