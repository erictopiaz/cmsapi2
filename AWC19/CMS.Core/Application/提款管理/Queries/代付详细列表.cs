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
using Microsoft.Extensions.Logging;
using ShareEntity.OutputModels;
using Dapper.Contrib.Extensions;
using ShareEntity.Models;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 代付详细列表 : PagingModel, IRequest<ResponseModel<PagingModel<代付详细列表Return>>>
    {
        public string Account { get; set; }
        public string realname { get; set; }
        public DateTime? CreateTime { get; set; }
        [JsonIgnore]
        public string password { get; set; }
        public string Admin { get; set; }
        [JsonIgnore]
        public List<int> currencys { get; set; }
        public int currencycode { get; set; }
    }

    public class 代付详细列表Return
    {
        public int Id { get; set; }

        public string TransactionId { get; set; }
        public string Account { get; set; }
        public string Realname { get; set; }
        public string cardno { get; set; }
        public decimal Amount { get; set; }
        public DateTime Createtime { get; set; }
        public string Result { get; set; }
        public string StatusCode { get; set; }
        public string StatusMsg { get; set; }
        public string Bank { get; set; }
        public string orderid { get; set; }
        public long TotalItems { get; set; }
        public int conformedstatus { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 代付详细列表Validator : AbstractValidator<代付详细列表>
    {
        public 代付详细列表Validator()
        {
            RuleFor(x => x.Account).Account();
            RuleFor(x => x.CreateTime).Datetime();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 代付详细列表Handler : BaseWithdraw<代付详细列表, ResponseModel<PagingModel<代付详细列表Return>>>
    {
        public 代付详细列表Handler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<PagingModel<代付详细列表Return>> Handle(代付详细列表 request)
        {
            if (!request.currencys.Contains(request.currencycode))
            {
                return new ResponseModel<PagingModel<代付详细列表Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择货币查询"
                };
            }
            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT * , COUNT(*) OVER() as TotalItems " +
                "FROM ofPaymentLog /**where**/ " +
                "ORDER BY Createtime DESC " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY");

            // FILL UP THE WHERE CLAUSE
            if (request.currencycode > 0) { builder.Where("currencycode = @currencycode", new { request.currencycode }); }
            if (!string.IsNullOrEmpty(request.Account)) builder.Where("account LIKE @account", new { account = string.Concat("%", request.Account, "%") });
            if (!string.IsNullOrEmpty(request.realname)) builder.Where("realname LIKE @realname", new { realname = string.Concat("%", request.realname, "%") });
            if (request.CreateTime != null) builder.Where("createtime >= @Createtime", new { request.CreateTime });

            // PAGING FUNCTION
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            //builder.OrderBy("createtime DESC");

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                
                var adminAccess = connection.Get<ofManage>(request.Admin).access;
                ResponseModel<PagingModel<BaseWithdrawReturn>> searchlimitResponse;
                if (!isWithdrawNoSearchDateLimitAllowed(adminAccess, (DateTime)request.CreateTime, request.password, out searchlimitResponse))
                    return new ResponseModel<PagingModel<代付详细列表Return>>()
                    {
                        code = searchlimitResponse.code,
                        message = searchlimitResponse.message,
                    };


                var rows = connection.Query<代付详细列表Return>(template.RawSql, template.Parameters);

                rows.ToList().ForEach(x =>
                {
                    x.cardno = string.Format("{0}******{1}", x.cardno.Substring(0, 6), x.cardno.Substring(x.cardno.Length - 6, 6));
                    x.cardno = "******";
                });

                var totalItems = rows == null ? 0 : rows.First().TotalItems;
                return new ResponseModel<PagingModel<代付详细列表Return>>
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new PagingModel<代付详细列表Return>()
                    {
                        TotalItems = totalItems,
                        Data = rows
                    }
                };
            }
        }
    }
}

