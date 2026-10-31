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
using log4net.Core;
using Newtonsoft.Json.Linq;
using Dapper.Contrib.Extensions;
using UtilsLib.lang;
using ShareEntity.Models;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 新会员DirectPayQuery : BaseWithdrawQuery, IRequest<ResponseModel<PagingModel<BaseWithdrawReturn>>>
    {
        public int walletType { set; get; } = -1;
        public string? merchantAccount { set; get; } = "";
    }

    public class 新会员DirectPayReturn : BaseWithdrawReturnModel
    {
        public string walletType { get; set; }
        public string merchantAccount { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 新会员DirectPayQueryValidator : AbstractValidator<新会员DirectPayQuery>
    {
        public 新会员DirectPayQueryValidator()
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
    public class 新会员DirectPayHandler : BaseWithdraw<新会员DirectPayQuery, ResponseModel<PagingModel<BaseWithdrawReturn>>>
    {
        public 新会员DirectPayHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<PagingModel<BaseWithdrawReturn>> Handle(新会员DirectPayQuery request)
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

            //var template = builder.AddTemplate("" +
            //    "SELECT * , COUNT(*) OVER() as TotalItems " +
            //    "FROM ofWithdrawal /**where**/ /**orderby**/ " +
            //    "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
            //    "FETCH NEXT @Next ROWS ONLY");

            //var template = builder.AddTemplate("" +
            //    "SELECT ofuser.account,amount, ofuser.createtime, COUNT(*) OVER() as TotalItems " +
            //    "FROM ofWithdrawal WITH(NOLOCK) " +
            //    "JOIN ofUser WITH(NOLOCK) ON ofWithdrawal.account = ofuser.account   " +
            //    "/**where**/ /**orderby**/ " +
            //    "OFFSET @Offset ROWS " +
            //    "FETCH NEXT @Next ROWS ONLY ");

            bool withCompute = false;

            // FILL UP THE WHERE CLAUSE
            if (request.starttime != null && request.endtime != null) { 
                builder.Where("requesttime BETWEEN @starttime AND @endtime", new { request.starttime, request.endtime });
                if (request.status == 2) withCompute = true;
            }
            else if (request.starttime != null)
                builder.Where("requesttime >= @starttime", new { starttime = request.starttime });
            else
                builder.Where("requesttime >= @starttime", new { starttime = DateTime.UtcNow.AddHours(8).AddMonths(-2) });

            // FILL UP THE WHERE CLAUSE
            if (request.currencycode > 0) { builder.Where("ofwithdrawal.currencycode = @currencycode", new { request.currencycode }); }
            if (!string.IsNullOrEmpty(request.account)) builder.Where("ofwithdrawal.account LIKE @account", new { account = string.Concat("%", request.account, "%") });
            if (!string.IsNullOrEmpty(request.cardno)) builder.Where("CardNo = @CardNo", new { request.cardno });
            if (!string.IsNullOrEmpty(request.realname)) builder.Where("ofwithdrawal.realname = @realname", new { request.realname });
            if (!string.IsNullOrEmpty(request.affaccount)) builder.Where("ofwithdrawal.affaccount = @affaccount", new { request.affaccount });
            if (!string.IsNullOrEmpty(request.ipaddress)) builder.Where("ofwithdrawal.ipaddress = @ipaddress", new { request.ipaddress });
            if (!string.IsNullOrEmpty(request.location)) builder.Where("ofwithdrawal.macaddress = @location", new { request.location });
            if (request.status >= 0) builder.Where("ofwithdrawal.status = @Status", new { request.status });
            //if (request.VerifyTime != null) builder.Where("requesttime >= @VerifyTime", new { request.VerifyTime });
            if (request.processingtime > 0) builder.Where("DATEDIFF(MINUTE, reviewtime,paytime) >= @processingtime", new { request.processingtime });
            if (!string.IsNullOrEmpty(request.merchantAccount) && request.merchantAccount != "-1") { builder.Where("ofUser.merchantAccount = @merchantAccount", new { request.merchantAccount }); }
            if (request.walletType >= 0) { builder.Where("ofUser.walletType = @walletType", new { request.walletType }); }


            DateTime monthBefore = DateTime.Parse(Common.CurrentTime.AddDays(-30).ToString("yyyy-MM-dd"));
            //DateTime monthBefore = DateTime.Parse(string.Format("{0}-01", Common.CurrentTime.ToString("yyyy-MM")));


            //builder.Where("account IN (SELECT account FROM ofUser WITH(NOLOCK) WHERE createtime>=@starttime)", new { starttime = monthBefore });
            //builder.Where("level in @Levels", new { Levels = 普通会员_和_万象会员 });
            //builder.Where("(ofwithdrawal.level in @Levels OR (ofwithdrawal.level=10027 AND ofuser.createtime >= '2020-06-01' AND ofuser.createtime < @starttime))",
            //    new { Levels = 普通会员_和_万象会员, starttime = monthBefore });
            builder.Where("(ofwithdrawal.level IN @Levels AND ofuser.createtime >= @starttime)",
                 new { Levels = 普通会员_和_万象会员, starttime = monthBefore });



            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("id DESC");




            SqlBuilder.Template template;



            template = builder.AddTemplate("" +
                "SELECT  walletType, merchantAccount,x.*,a.*,c.*,d.id as uid,d.misc ofuser_misc, d.withdrawalamount-d.depositamount user_winloss FROM (SELECT ofwithdrawal.misc misc, ofwithdrawal.id id, ofwithdrawal.account account,ofwithdrawal.realname realname, amount, fee, cardno, cardaddress, cardbank, requesttime, reviewtime, ofwithdrawal.status status, paytime, payperson, ofwithdrawal.level level, ofwithdrawal.affaccount affaccount, checkperson, cancelreason, cancelperson, canceltime,ofUser.createtime usercreatetime, COUNT(*) OVER() as TotalItems " +
                "FROM ofWithdrawal WITH(NOLOCK) " +
                "JOIN ofUser WITH(NOLOCK) ON ofWithdrawal.account = ofuser.account " +
                " /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +
                "FETCH NEXT @Next ROWS ONLY)a " +
                "LEFT Join ofuserExtention c on c.account=a.account " +
                "LEFT Join ofUser d on d.account=a.account " +
                "OUTER APPLY (SELECT sum(amount) as totalmonthwithdraw, count(*) as totalmonthwithdrawcount FROM ofwithdrawal z WHERE requesttime >= DATEADD(day,-30,GETDATE())  AND z.account = a.account AND status=2)x");

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {


                var adminAccess = connection.Get<ofManage>(request.Admin).access;
                ResponseModel<PagingModel<BaseWithdrawReturn>> searchlimitResponse;
                if (request.starttime!=null && !isWithdrawNoSearchDateLimitAllowed(adminAccess, (DateTime)request.starttime, request.password, out searchlimitResponse))
                    return searchlimitResponse;


                var rows = connection.Query<BaseWithdrawReturn>(template.RawSql, template.Parameters);
                var totalItems = rows.Count() <= 0 ? 0 : rows.First().TotalItems;

                float totalAmount = 0;
                //if (withCompute) {
                //    totalAmount = connection.QueryFirstOrDefault<float>("" +
                //        "SELECT SUM(amount) " +
                //        "FROM ofWithdrawal WITH(NOLOCK) " +
                //        "JOIN ofUser WITH(NOLOCK) ON ofWithdrawal.account = ofuser.account " +
                //        " /**where**/", template.Parameters);
                //}


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


                        //if (!string.IsNullOrEmpty(x.ofuser_misc))
                        //{
                        //    var y = JObject.Parse(x.ofuser_misc);
                        //    if (y.ContainsKey("tags"))
                        //    {
                        //        x.account_tags = y["tags"].ToString();
                        //    }
                        //}
                        if (!string.IsNullOrEmpty(x.ofuser_misc))
                        {
                            var y = JObject.Parse(x.ofuser_misc);
                            if (y.ContainsKey("tags"))
                            {
                                if (x.user_winloss < 0)
                                    x.account_tags = "***";
                                else
                                    x.account_tags = y["tags"].ToString();
                            }
                        }


                        //x.TotalAmount = totalAmount;
                        if (x.paytime != null)
                        {
                            x.minutesdiff = ((DateTime)x.paytime - x.reviewtime).TotalMinutes;
                            x.minutesdiff = Math.Floor(x.minutesdiff);
                        }
                        else
                            x.minutesdiff = -1;

                        #region 根据当前货币查询 显示不同的货币等级
                        if (request.currencycode == (int)LangCodes.CurrencyCode.INR
                        || request.currencycode == (int)LangCodes.CurrencyCode.USD) {
                            if (VipLevelINTL.levelIntlName.ContainsKey(x.level))
                                x.levelname = VipLevelINTL.levelIntlName[x.level];
                        }
                        else {
                            x.levelname = CMS.Core.CachedData.LevelNameDict[x.level];
                        }
                        #endregion
                        //x.levelname = CMS.Core.CachedData.LevelNameDict[x.level];
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

    
}

