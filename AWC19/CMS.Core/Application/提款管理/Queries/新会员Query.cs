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
using Newtonsoft.Json.Linq;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 新会员Query : BaseWithdrawQuery, IRequest<ResponseModel<PagingModel<BaseWithdrawReturn>>>
    {
    }

    public class 新会员Return : BaseWithdrawReturnModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 新会员QueryValidator : AbstractValidator<新会员Query>
    {
        public 新会员QueryValidator()
        {

            RuleFor(x => x.account).Account();
            RuleFor(x => x.cardno).CardNo();
            RuleFor(x => x.realname).Realname();
            RuleFor(x => x.affaccount).Affiliate();
            RuleFor(x => x.ipaddress).IP();
            RuleFor(x => x.password).Password();
            RuleFor(x => x.location).CharDigitalChinese(1, 10);
            RuleFor(x => x.starttime).Datetime();
            RuleFor(x => x.endtime).Datetime();
            RuleFor(x => x.status).Status();
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
    public class 新会员Handler : BaseWithdraw<新会员Query, ResponseModel<PagingModel<BaseWithdrawReturn>>>
    {
        public 新会员Handler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<PagingModel<BaseWithdrawReturn>> Handle(新会员Query request)
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

            // FILL UP THE WHERE CLAUSE
            if (request.currencycode > 0) { builder.Where("currencycode = @currencycode", new { request.currencycode }); }
            if (!string.IsNullOrEmpty(request.account)) builder.Where("account LIKE @account", new { account = string.Concat("%", request.account, "%") });
            if (!string.IsNullOrEmpty(request.cardno)) builder.Where("CardNo = @CardNo", new { request.cardno });
            if (!string.IsNullOrEmpty(request.realname)) builder.Where("realname = @realname", new { request.realname });
            if (!string.IsNullOrEmpty(request.affaccount)) builder.Where("affaccount = @affaccount", new { request.affaccount });
            if (!string.IsNullOrEmpty(request.ipaddress)) builder.Where("ipaddress = @ipaddress", new { request.ipaddress });
            if (!string.IsNullOrEmpty(request.location)) builder.Where("macaddress = @location", new { request.location });
            if (request.status >= 0) builder.Where("status = @Status", new { request.status });
            //if (request.VerifyTime != null) builder.Where("requesttime >= @VerifyTime", new { request.VerifyTime });
            if (request.processingtime > 0) builder.Where("DATEDIFF(MINUTE, reviewtime,paytime) >= @processingtime", new { request.processingtime });

            DateTime monthBefore = DateTime.Parse(Common.CurrentTime.AddDays(-30).ToString("yyyy-MM-dd"));
            //DateTime monthBefore = DateTime.Parse(string.Format("{0}-01", Common.CurrentTime.ToString("yyyy-MM")));
            builder.Where("account IN (SELECT account FROM ofUser WITH(NOLOCK) WHERE createtime>=@ofuserstarttime)", new { ofuserstarttime = monthBefore });
            //builder.Where("account in @AccountList", new { AccountList = currentMonthAccountList });

            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("id DESC");



            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var rows = connection.Query<BaseWithdrawReturn>(template.RawSql, template.Parameters);
                var totalItems = rows.Count() <= 0 ? 0 : rows.First().TotalItems;
                // 搜索限制


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
                                    x.cardno = "*****";
                                    //x.amount = "*****";
                                }
                                else
                                {
                                    x.cardno = request.Admin == x.checkperson.ToLower() ? x.cardno : Common.Security.ObscureWithdrawCard(x.cardno);
                                    x.cardno = "*****";
                                    x.amount = request.Admin == x.checkperson.ToLower() ? x.amount : "*****";
                                }
                                break;
                            default:
                                x.cardno = "*****";
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

    //protected override ResponseModel<PagingModel<新会员Return>> Handle(新会员Query request)

    //    {

    //        // 创建DAPPER TEMPLATE 
    //        var builder = new SqlBuilder();
    //        var template = builder.AddTemplate("" +
    //            "SELECT * , COUNT(*) OVER() as TotalItems " +
    //            "FROM ofWithdrawal /**where**/ /**orderby**/ " +
    //            "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
    //            "FETCH NEXT @Next ROWS ONLY");

    //        // FILL UP THE WHERE CLAUSE
    //        if (!string.IsNullOrEmpty(request.account)) builder.Where("account LIKE @account", new { account = string.Concat("%", request.account, "%") });
    //        if (!string.IsNullOrEmpty(request.CardNo)) builder.Where("affaccount = @CardNo", new { request.CardNo });
    //        if (!string.IsNullOrEmpty(request.Bank)) builder.Where("Realname = @Bank", new { request.Bank });
    //        if (request.Status >= 0) builder.Where("status = @Status", new { request.Status });
    //        if (request.VerifyTime != null) builder.Where("requesttime >= @VerifyTime", new { request.VerifyTime });

    //        //builder.Where("amount > @Maxamount", new { Maxamount = _DirectPayStartAmount });
    //        builder.Where("account in @AccountList", new { AccountList = _currentMonthAccountList });

    //        // PAGING FUNCTION
    //        int offset = request.Page * request.PageSize;
    //        builder.AddParameters(new { Offset = offset, Next = request.PageSize });
    //        builder.OrderBy("requesttime DESC");

    //        using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
    //        {
    //            var rows = connection.Query<新会员Return>(template.RawSql, template.Parameters);
    //            var totalItems = rows == null ? 0 : rows.First().TotalItems;

    //            return new ResponseModel<PagingModel<新会员Return>>
    //            {
    //                code = ShareEntity.Global.ResponseCode.code.SUCCESS,
    //                message = "",
    //                Data = new PagingModel<新会员Return>()
    //                {
    //                    TotalItems = totalItems,
    //                    Data = rows
    //                }
    //            };
    //        }

    //    }


    //}
}

