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
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using ShareEntity.OutputModels;
using Dapper.Contrib.Extensions;
using ShareEntity.Models;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class ManualPayQuery : BaseWithdrawQuery, IRequest<ResponseModel<PagingModel<BaseWithdrawReturn>>>
    {
        public string level { get; set; }
    }

    //public class ManualPayQueryReturn
    //{
    //    public long id { get; set; }
    //    public string account { get; set; }
    //    public string realname { get; set; }
    //    public decimal amount { get; set; }
    //    public string cardno { get; set; }
    //    public string cardaddress { get; set; }
    //    public string cardbank { get; set; }
    //    public DateTime requesttime { get; set; }
    //    public int status { get; set; }

    //    //public string requestip { get; set; }
    //    //public string requestaddress { get; set; }
    //    //public DateTime? reviewtime { get; set; }
    //    //public string reviewperson { get; set; }
    //    //public DateTime? paytime { get; set; }
    //    //public string payperson { get; set; }
    //    //public DateTime? canceltime { get; set; }
    //    //public string cancelperson { get; set; }
    //    //public string cancelreason { get; set; }
    //    //public decimal? balance { get; set; }
    //    //public string referrer { get; set; }
    //    //public string referrerreg { get; set; }
    //    public int level { get; set; }
    //    //public string affaccount { get; set; }
    //    public string checkperson { get; set; }
    //    public long TotalItems { get; set; }

    //}


    /// <summary>
    /// API Model Validation
    /// </summary>
    public class ManualPayQueryValidator : AbstractValidator<ManualPayQuery>
    {
        public ManualPayQueryValidator()
        {
            RuleFor(x => x.account).Account();
            RuleFor(x => x.cardno).CardNo();
            RuleFor(x => x.status).Status();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class ManualPayQueryHandler : BaseWithdraw<ManualPayQuery, ResponseModel<PagingModel<BaseWithdrawReturn>>>
    {
        public ManualPayQueryHandler(IMapper mapper, ILoggerFactory logger)
                : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<PagingModel<BaseWithdrawReturn>> Handle(ManualPayQuery request)
        {
            if (!request.currencys.Contains(request.currencycode))
            {
                return new ResponseModel<PagingModel<BaseWithdrawReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择货币查询"
                };
            }

            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT * , COUNT(*) OVER() as TotalItems " +
                "FROM ofWithdrawal /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY");


            // FILL UP THE WHERE CLAUSE
            if (request.starttime != null && request.endtime != null)
                builder.Where("requesttime BETWEEN @starttime AND @endtime", new { request.starttime, request.endtime });
            else if (request.starttime != null)
                builder.Where("requesttime >= @starttime", new { starttime = request.starttime });
            else
                builder.Where("requesttime >= @starttime", new { starttime = DateTime.UtcNow.AddHours(8).AddMonths(-2) });

            builder.Where("checkperson in @admin", new { admin = new List<string>() { "kafuph" } });
            if (request.status > 1)
                builder.Where("status = @status", new { status = request.status });
            else if(request.status == -1)
                builder.Where("status > 0");
            else
                builder.Where("status = 1");

            //var levels = request.level.ToLower() == "regular" ? BaseWithdraw.普通会员_和_万象会员


            switch (request.level.ToLower())
            {
                case "regular":
                    builder.Where("level in @levels", new { levels = BaseWithdraw.普通会员_和_万象会员 });
                    break;
                case "vip":
                    builder.Where("level not in @levels", new { levels = BaseWithdraw.普通会员_和_万象会员 });
                    break;
                default:
                    builder.Where("level = -1");
                    break;
            }

            //s

            //// FILL UP THE WHERE CLAUSE
            //if (!string.IsNullOrEmpty(request.account)) builder.Where("account LIKE @account", new { account = string.Concat("%", request.account, "%") });
            //if (!string.IsNullOrEmpty(request.cardno)) builder.Where("CardNo = @CardNo", new { request.cardno });
            //if (!string.IsNullOrEmpty(request.realname)) builder.Where("realname = @realname", new { request.realname });
            //if (!string.IsNullOrEmpty(request.affaccount)) builder.Where("affaccount = @affaccount", new { request.affaccount });
            //if (!string.IsNullOrEmpty(request.ipaddress)) builder.Where("ipaddress = @ipaddress", new { request.ipaddress });
            //if (!string.IsNullOrEmpty(request.location)) builder.Where("macaddress = @location", new { request.location });
            //if (request.status >= 0) builder.Where("status = @Status", new { request.status });
            ////if (request.VerifyTime != null) builder.Where("requesttime >= @VerifyTime", new { request.VerifyTime });
            //if (request.processingtime > 0) builder.Where("DATEDIFF(MINUTE, reviewtime,paytime) >= @processingtime", new { request.processingtime });

            //builder.Where("amount >= @BigVIP", new { BigVIP = 大额VIP余额 });
            //builder.Where("level IN @Levels", new { Levels = 普通会员_和_万象会员_和_白银VIP });



            // PAGING FUNCTION
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("requesttime DESC");

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var rows = connection.Query<BaseWithdrawReturn>(template.RawSql, template.Parameters);
                var totalItems = rows.Count() <= 0 ? 0 : rows.First().TotalItems;


                var adminAccess = connection.Get<ofManage>(request.Admin).access;
                ResponseModel<PagingModel<BaseWithdrawReturn>> searchlimitResponse;
                if (!isWithdrawNoSearchDateLimitAllowed(adminAccess, (DateTime)request.starttime, request.password, out searchlimitResponse))
                    return searchlimitResponse;


                // Apply the LevelName 
                if (rows.Count() > 0)
                    rows.ToList().ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x.misc))
                        {
                            var y = JObject.Parse(x.misc);
                            if (y.ContainsKey("paystat") && y.ContainsKey("paystatreason"))
                            {
                                x.paystat = y["paystat"].ToString() == "true";
                                x.paystatreason = y["paystatreason"].ToString();
                            }
                            if (y.ContainsKey("origamount"))
                                x.origamount = y["origamount"].ToString();
                            x.xfermanual = y.ContainsKey("xfermanual") ? y["xfermanual"].ToString() : "";
                        }

                        if (x.paytime != null)
                        {
                            x.minutesdiff = ((DateTime)x.paytime - x.reviewtime).TotalMinutes;
                            x.minutesdiff = Math.Floor(x.minutesdiff);
                        }
                        else
                            x.minutesdiff = -1;

                        x.levelname = CMS.Core.CachedData.LevelNameDict[x.level];
                        switch (x.status)
                        {
                            //case 0: x.cardno = "*****"; break;
                            case 1:
                                if (string.IsNullOrEmpty(x.checkperson))
                                {
                                    x.cardno = Common.Security.ObscureWithdrawCard(x.cardno);
                                    //x.amount = "*****";
                                }
                                else
                                {
                                    x.cardno = request.Admin == x.checkperson.ToLower() ? x.cardno : Common.Security.ObscureWithdrawCard(x.cardno);
                                    x.amount = request.Admin == x.checkperson.ToLower() ? x.amount : "*****";
                                }
                                break;
                            default:
                                x.cardno = "*****";// Common.Security.ObscureWithdrawCard(x.cardno);
                                x.amount = "*****";
                                break;
                        }

                    });



                var result = new ResponseModel<PagingModel<BaseWithdrawReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                    Data = new PagingModel<BaseWithdrawReturn>()
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

