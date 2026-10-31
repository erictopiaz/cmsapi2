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
using Newtonsoft.Json.Linq;
using CMS.Data;
using ShareEntity.Models;

namespace CMS.Core.Application.电销管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 电销任务Query : PagingModel, IRequest<ResponseModel<PagingModel<电销任务Return>>>
    {
        public string account { get; set; }
        public string Admin { get; set; }
        public DateTime? starttime { get; set; }
        public DateTime? endtime { get; set; }
    }

    public class 电销任务Return : ofCallLog
    {
        public DateTime? assignedtime { get; set; }
        public string topaccount { get; set; }
        public long TotalItems { get; set; }

        public DateTime? transtatus { get; set; }
        public DateTime? deposittime { get; set; }
        public string affaccount { get; set; }
        public decimal depositamount { get; set; }
        public int depositnumber { get; set; }
        public string ipaddress { get; set; }
        public string macaddress { get; set; }
        public string referrerreg { get; set; }
        public int level { get; set; }
        //public string levelname { get; set; }
        public DateTime createtime { get; set; }
        public DateTime registertime { get; set; }
        public DateTime? lastdeposittime { get; set; }
        public int status { get; set; }
        public string account { get; set; }
        public string realname { get; set; }
        public string birthday { get; set; }
        public string telephone { get; set; }
        public string levelname { get; set; }


    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 电销任务QueryValidator : AbstractValidator<电销任务Query>
    {
        public 电销任务QueryValidator()
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
    public class 电销任务Handler : BaseRequestHandler<电销任务Query, ResponseModel<PagingModel<电销任务Return>>>
    {
        public 电销任务Handler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<PagingModel<电销任务Return>> Handle(电销任务Query request)
        {
            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();

            //// FILL UP THE WHERE CLAUSE
            //if (request.starttime != null && request.endtime != null)
            //    builder.Where("createtime BETWEEN @starttime AND @endtime", new { request.starttime, request.endtime });
            //else if (request.starttime != null)
            //    builder.Where("createtime >= @starttime", new { starttime = request.starttime });

            // FILL UP THE WHERE CLAUSE
            if (!string.IsNullOrEmpty(request.account))
            {
                request.Page = 0;
                builder.Where("Account = @Account", new { request.account });
            }

            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("assignedtime DESC");
            builder.Where("createperson = @CREATEPERSON", new
            {
                CREATEPERSON = request.Admin
                //CREATEPERSON = "tina"
            });

            SqlBuilder.Template template;
            template = builder.AddTemplate("" +
                "SELECT * FROM (SELECT TotalItems, topaccount as account , assignedtime, createperson,telephone,level,referrerreg,realname,status,ofuser.createtime as registertime,depositamount," +
                "depositnumber,transtatus,affaccount,ipaddress,macaddress,calltime " +
                "FROM (SELECT COUNT(*) OVER() as TotalItems, topaccount,assignedtime, createperson,calltime FROM " +
                "(SELECT DISTINCT account FROM ofcalllog WITH(NOLOCK) WHERE status=1 and CONVERT(VARCHAR, comment)='分配')mo " +
                "CROSS APPLY (SELECT TOP 1 account AS topaccount, createperson, createtime as assignedtime,calltime " +
                "FROM ofcalllog mi WITH(NOLOCK)" +
                "WHERE mi.account = mo.account order by id DESC)t /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +
                "FETCH NEXT @Next ROWS ONLY)b " +
                "left join ofuser on b.topaccount = ofuser.account)z " +
                "OUTER APPLY (SELECT TOP 1 account, ISNULL(orderendtime,reviewtime) AS deposittime FROM ofdeposit di WITH(NOLOCK) WHERE di.account = z.account ORDER by ID DESC )c");

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var rows = connection.Query<电销任务Return>(template.RawSql, template.Parameters);
                var totalItems = rows.Count() == 0 ? 0 : rows.First().TotalItems;
                var accountList = new List<string>();
                if (rows.Count() > 0)
                {

                    rows.ToList().ForEach(x =>
                    {
                        accountList.Add(x.account);
                        x.levelname = CMS.Core.CachedData.LevelNameDict[x.level];
                        x.createtime = x.calltime != null ? (DateTime)x.calltime : x.createtime;
                    });


                    // Hide the phone number if there's a comment already  
                    /*
                        SELECT * 
                        FROM	(SELECT distinct account FROM ofcalllog WITH(NOLOCK) 
                                WHERE account in ('zjq345144888','yumi035','k8782153','cdwjll')) mo
                        CROSS APPLY 
                                (SELECT TOP 1 *
                                FROM ofcalllog WITH(NOLOCK) WHERE ofcalllog.account = mo.account and status = 0 order by id desc) mi
                     */
                    var hidePhoneBuilder = new SqlBuilder();
                    SqlBuilder.Template hidePhoneTemplate;
                    hidePhoneBuilder.AddParameters(new { CREATEPERSON = request.Admin });
                    hidePhoneBuilder.Where("account = @ACCOUNTLIST", new
                    {
                        ACCOUNTLIST = accountList
                    });
                    hidePhoneTemplate = hidePhoneBuilder.AddTemplate("" +
                        "SELECT * " +
                        "FROM (SELECT distinct account FROM ofcalllog WITH(NOLOCK) " +
                        "WHERE account in @ACCOUNTLIST )mo " +
                        "CROSS APPLY " +
                        "(SELECT TOP 1 * " +
                        "FROM ofcalllog WITH(NOLOCK) " +
                            "WHERE ofcalllog.account=mo.account AND status=1 AND createperson=@CREATEPERSON ORDER BY ID DESC )mi");
                    var toHide = connection.Query<电销任务Return>(hidePhoneTemplate.RawSql, hidePhoneTemplate.Parameters);

                    rows.ToList().ForEach(x =>
                    {
                        toHide.ToList().ForEach(y =>
                        {
                            if (x.account.ToLower() == y.account.ToLower() &&
                                y.createtime < x.assignedtime) 
                                x.telephone = Common.Security.ObscureString(x.telephone);
                            x.id = y.id;
                        });
                    });
                }



                var result = new ResponseModel<PagingModel<电销任务Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                    Data = new PagingModel<电销任务Return>()
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

