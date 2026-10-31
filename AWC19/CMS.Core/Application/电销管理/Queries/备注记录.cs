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
using ShareEntity.Models;

namespace CMS.Core.Application.电销管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 备注记录Query : PagingModel, IRequest<ResponseModel<PagingModel<备注记录Return>>>
    {
        public string account { get; set; }
        public string createperson { get; set; }
        public int status { get; set; } = -1;
        public int contactStatus { get; set; } = -1;
        public string comment { get; set; }
        public string Admin { get; set; }
        public DateTime? starttime { get; set; }
        public DateTime? endtime { get; set; }
    }

    public class 备注记录Return : ofCallLog
    {
        public string topaccount { get; set; }
        public long TotalItems { get; set; }
        public bool anomally { get; set; }
        public string misc { get; set; }

        public long uid { get; set; }

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 备注记录QueryValidator : AbstractValidator<备注记录Query>
    {
        public 备注记录QueryValidator()
        {

            //RuleFor(x => x.account).Account();
            //RuleFor(x => x.createperson).Account();
            //RuleFor(x => x.status).Status();
            //RuleFor(x => x.contactStatus).Status();
            //RuleFor(x => x.comment).CharDigitalChinese(1,100);
            //RuleFor(x => x.starttime).Datetime();
            //RuleFor(x => x.endtime).Datetime();

            //RuleFor(x => x.Admin).Admin();
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
    public class 备注记录Handler : BaseRequestHandler<备注记录Query, ResponseModel<PagingModel<备注记录Return>>>
    {
        public 备注记录Handler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<PagingModel<备注记录Return>> Handle(备注记录Query request)
        {
            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();

            ////FILL UP THE WHERE CLAUSE
            if (request.starttime != null && request.endtime != null)
                builder.Where("createtime BETWEEN @starttime AND @endtime", new { request.starttime, request.endtime });
            else if (request.starttime != null)
                builder.Where("createtime >= @starttime", new { starttime = request.starttime });

            // FILL UP THE WHERE CLAUSE
            if (!string.IsNullOrEmpty(request.account))
            {
                request.Page = 0;
                builder.Where("Account = @Account", new { request.account });
            }

            // FILL UP THE WHERE CLAUSE
            if (!string.IsNullOrEmpty(request.comment))
            {
                request.Page = 0;
                builder.Where("comment LIKE @comment", new
                {
                    comment = string.Concat("%",request.comment, "%")
                });
            }

            // FILL UP THE WHERE CLAUSE
            if (!string.IsNullOrEmpty(request.createperson))
            {
                //request.Page = 0;
                builder.Where("createperson = @createperson", new { request.createperson });
            }

            // FILL UP THE WHERE CLAUSE
            if (request.contactStatus >=0 )
            {
                //request.Page = 0;
                builder.Where("contactStatus = @contactStatus", new { request.contactStatus});
            }

            // FILL UP THE WHERE CLAUSE
            switch (request.status)
            {
                case 0:
                    builder.Where("status = @STATUS", new { request.status }); break;
                default:
                    break;
            }

            //if (request.status == 1) {
            //    //builder.Where("comment LIKE '分配'");
            //}

            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("ID DESC");
           
            //builder.Where("status=0");
            SqlBuilder.Template template;

            if (request.status == 1)
                template = builder.AddTemplate("" +
                    " SELECT a.TotalItems AS TotalItems, a.account AS account, a.createtime AS createtime,a.createperson AS createperson,a.comment AS comment,ofUser.id uid, ofuser.misc as misc FROM (" +
                    " SELECT * FROM (SELECT COUNT(*) OVER() as TotalItems, topaccount as account,createtime,createperson,comment FROM " +
                    " (SELECT DISTINCT account FROM ofcalllog WITH(NOLOCK) WHERE status=1 AND comment LIKE '分配')mo " +
                    " CROSS APPLY (SELECT TOP 1 account AS topaccount, createperson, createtime,comment " +
                    " FROM ofcalllog mi WITH(NOLOCK)" +
                    " WHERE mi.account = mo.account order by id DESC)t /**where**/)a order by createtime" +
                    " OFFSET @offset ROWS " +
                    " FETCH NEXT @Next ROWS ONLY)a JOIN ofUser WITH(NOLOCK) ON a.account=ofUser.account ");
            else
            {
                template = builder.AddTemplate("" +
                    "SELECT a.account AS account, a.createtime AS createtime,a.createperson AS createperson,a.comment AS comment,ofUser.id uid, ofuser.misc as misc, TotalItems FROM (SELECT COUNT(*) OVER() as TotalItems, * FROM ofcalllog WITH(NOLOCK) /**where**/ /**orderby**/ " +
                    "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                    "FETCH NEXT @Next ROWS ONLY)a JOIN ofUser WITH(NOLOCK) ON a.account=ofUser.account ");
            }

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {

                var rows = connection.Query<备注记录Return>(template.RawSql, template.Parameters);

                var totalItems = rows.Count() == 0 ? 0 : rows.First().TotalItems;

                // Apply the LevelName 
                if (rows.Count() > 0)
                    rows.ToList().ForEach(
                        x =>
                        {
                            x.anomally = false;
                            if (!string.IsNullOrEmpty(x.misc))
                            {
                                var xDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(x.misc);
                                if (xDict.ContainsKey("promodisabled"))
                                {
                                    xDict["promodisabled"] = xDict["promodisabled"].Replace("[", "");
                                    xDict["promodisabled"] = xDict["promodisabled"].Replace("]", "");
                                    if (!string.IsNullOrEmpty(xDict["promodisabled"]))
                                    {
                                        x.anomally = true;
                                    }
                                }
                            }

                        });


                var result = new ResponseModel<PagingModel<备注记录Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                    Data = new PagingModel<备注记录Return>()
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

