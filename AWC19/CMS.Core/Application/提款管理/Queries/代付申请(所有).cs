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
using Dapper.Contrib.Extensions;
using ShareEntity.Models;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 代付列表所有Query : BaseWithdrawQuery, IRequest<ResponseModel<PagingModel<代付列表所有Return>>>
    {
        public string currency { get; set; }
        public string orderid { get; set; }
        public string platform { get; set; }
        public int transtype { get; set; }
        public int walletType { set; get; } = -1;
        public string? merchantAccount { set; get; } = "";

    }

    public class 代付列表所有Return : BaseWithdrawReturn
    {
        public string walletType { get; set; }
        public string merchantAccount { get; set; }
        public string platform { get; set; }
        public string result { get; set; }
        public string orderid { get; set; }
        public int statusCode { get; set; }
        public string customer_time { get; set; }
        public string transactionid { get; set; }
        public DateTime createtime { get; set; }
        public int transtype { get; set; }

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 代付列表所有QueryValidator : AbstractValidator<代付列表所有Query>
    {
        public 代付列表所有QueryValidator()
        {
            RuleFor(x => x.account).Account();
            RuleFor(x => x.cardno).CardNo();
            RuleFor(x => x.status).Status();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 代付列表所有Handler : BaseWithdraw<代付列表所有Query, ResponseModel<PagingModel<代付列表所有Return>>>
    {
        public 代付列表所有Handler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<PagingModel<代付列表所有Return>> Handle(代付列表所有Query request)
        {
            if (!request.currencys.Contains(request.currencycode))
            {
                return new ResponseModel<PagingModel<代付列表所有Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择货币查询"
                };
            }

            //// 创建DAPPER TEMPLATE 
            //var builder = new SqlBuilder();
            //var template = builder.AddTemplate("" +
            //    "SELECT * , COUNT(*) OVER() as TotalItems " +
            //    "FROM ofWithdrawal /**where**/ /**orderby**/ " +
            //    "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
            //    "FETCH NEXT @Next ROWS ONLY");



            //// FILL UP THE WHERE CLAUSE
            //if (request.starttime != null && request.endtime != null)
            //    builder.Where("requesttime BETWEEN @starttime AND @endtime", new { request.starttime, request.endtime });
            //else if (request.starttime != null)
            //    builder.Where("requesttime >= @starttime", new { starttime = request.starttime });
            //else
            //    builder.Where("requesttime >= @starttime", new { starttime = DateTime.UtcNow.AddHours(8).AddMonths(-2) });

            //// FILL UP THE WHERE CLAUSE
            //if (!string.IsNullOrEmpty(request.account)) builder.Where("account LIKE @account", new { account = string.Concat("%", request.account, "%") });
            //if (!string.IsNullOrEmpty(request.cardno)) builder.Where("CardNo = @CardNo", new { request.cardno });
            //if (!string.IsNullOrEmpty(request.realname)) builder.Where("realname = @realname", new { request.realname });
            //if (!string.IsNullOrEmpty(request.affaccount)) builder.Where("affaccount = @affaccount", new { request.affaccount });
            //if (!string.IsNullOrEmpty(request.ipaddress)) builder.Where("ipaddress = @ipaddress", new { request.ipaddress });
            //if (!string.IsNullOrEmpty(request.location)) builder.Where("macaddress = @location", new { request.location });
            //if (request.status >= 0) builder.Where("status = @Status", new { request.status });
            ////if (request.VerifyTime != null) builder.Where("requesttime >= @VerifyTime", new { request.VerifyTime });


            //builder.Where("amount <= @Maxamount", new { Maxamount = _DirectPayStartAmount });
            //builder.Where("level in @Levels", new { Levels = ALLUSER });

            //// PAGING FUNCTION
            //request.Page = request.Page == 0 ? 0 : request.Page - 1;
            //int offset = request.Page * request.PageSize;
            //builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            //builder.OrderBy("requesttime DESC");


            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            //var template = builder.AddTemplate("" +
            //    "SELECT transactionid,statuscode,level,checkperson, a.account AS account,a.realname AS realname,p.amount as amount,cardbank,p.cardno as cardno, requestip, reviewperson, reviewtime, paytime, payperson, result,orderid, platform,createtime,COUNT(*) OVER() as TotalItems  " +
            //    "FROM (SELECT * FROM ofWithdrawal WITH(NOLOCK) WHERE id IN (SELECT TransactionId FROM ofPaymentLog WITH(NOLOCK) /**where**/))a " +
            //    "INNER JOIN ofPaymentlog p ON a.id = p.TransactionId " +
            //    "/**orderby**/ " +
            //    "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
            //    "FETCH NEXT @Next ROWS ONLY");

            SqlBuilder.Template template;


            if (request.transtype == 1)
            {
                template = builder.AddTemplate("" +
                "SELECT b.*, e.walletType as walletType, e.merchantAccount as merchantAccount,d.*,c.*,e.id uid,e.misc ofuser_misc, e.withdrawalamount-e.depositamount user_winloss  FROM (" +
                "SELECT  misc='', transactionid,a.account AS account,a.realname AS realname,a.cardno AS cardno, a.amount AS amount, " +
                "p.reviewtime AS reviewtime, p.requesttime AS createtime,createtime as paytime,result,statuscode,a.bank as cardbank,orderid, a.platform AS platform,transtype,payadmin as payperson,a.payadmin as checkperson,COUNT(*) OVER() as TotalItems " +
                "FROM (SELECT a.*  " +
                "FROM ofPaymentLog a WITH(NOLOCK) where transtype = 1 order by a.Createtime desc " +
                "OFFSET 0 ROWS " +
                "FETCH NEXT 20 ROWS ONLY )b " +
                "LEFT JOIN ofaffwithdraw p ON b.TransactionId = CAST(p.id as varchar(50)))d " +
                "LEFT Join ofuserExtention c on c.account=b.account " +
                "LEFT Join ofUser e on e.account=b.account " +
                "ORDER BY b.paytime DESC");
                builder.Where("transtype=1");
            }
            else
            {
                template = builder.AddTemplate("" +
                "SELECT b.*, e.walletType as walletType, e.merchantAccount as merchantAccount ,c.*,e.id uid,e.misc ofuser_misc, e.withdrawalamount-e.depositamount user_winloss  FROM (" +
                "SELECT p.misc AS misc, transactionid,a.account AS account,a.realname AS realname,a.cardno AS cardno, a.amount AS amount, delayToTime, " +
                "p.reviewtime AS reviewtime, p.requesttime AS createtime,a.createtime as paytime," +
                "result,statuscode,a.bank as cardbank,orderid, a.platform AS platform,transtype,payadmin as payperson,a.payadmin as checkperson," +
                //"(SELECT COUNT(*) FROM ofPaymentLog WITH(NOLOCK)) as TotalItems " +
                "COUNT(*) OVER() as TotalItems " +
                "FROM ofPaymentLog a WITH(NOLOCK) " +
                "INNER JOIN ofWithdrawal p ON a.TransactionId = p.id " +
                "LEFT Join ofuser WITH(NOLOCK) on ofUser.account=a.account " +
                "/**where**/ " +
                "order by a.Createtime desc " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY)b " +
                "LEFT Join ofuserExtention c on c.account=b.account " +
                "LEFT Join ofUser e on e.account=b.account " +
                "ORDER BY b.paytime DESC" );

                builder.Where("transtype=0");

            }
            // FILL UP THE WHERE CLAUSE
            if (request.starttime != null && request.endtime != null)
                builder.Where("a.createtime BETWEEN @starttime AND @endtime", new { request.starttime, request.endtime });
            else if (request.starttime != null)
                builder.Where("a.createtime >= @starttime", new { starttime = request.starttime });
            else
                builder.Where("a.createtime >= @starttime", new { starttime = DateTime.UtcNow.AddHours(8).AddDays(-7) });


            builder.OrderBy("a.Createtime DESC");


            // FILL UP THE WHERE CLAUSE
            if (!string.IsNullOrEmpty(request.merchantAccount) && request.merchantAccount != "-1") { builder.Where("ofUser.merchantAccount = @merchantAccount", new { request.merchantAccount }); }
            if (request.walletType >= 0) { builder.Where("ofUser.walletType = @walletType", new { request.walletType }); }
            if (request.currencycode > 0) { builder.Where("p.currencycode = @currencycode", new { request.currencycode }); }
            if (!string.IsNullOrEmpty(request.account)) builder.Where("a.account LIKE @account", new { account = string.Concat("%", request.account, "%") });
            if (!string.IsNullOrEmpty(request.platform)) builder.Where("a.platform LIKE @platform", new { platform = string.Concat("%", request.platform, "%") });
            if (!string.IsNullOrEmpty(request.realname)) builder.Where("a.realname LIKE @realname", new { realname = string.Concat("%", request.realname, "%") });
            if (!string.IsNullOrEmpty(request.orderid)) builder.Where("a.orderid = @orderid", new { orderid = request.orderid});

            if (!string.IsNullOrEmpty(request.currency) && request.currency == "USDT") {
                builder.Where("a.bank IN ('USDT','USDT-ERC20','USDT-TRC20')");
            }

            //if (!string.IsNullOrEmpty(request.cardno)) builder.Where("CardNo = @CardNo", new { request.cardno });
            //if (!string.IsNullOrEmpty(request.realname)) builder.Where("realname = @realname", new { request.realname });
            //if (!string.IsNullOrEmpty(request.affaccount)) builder.Where("affaccount = @affaccount", new { request.affaccount });
            //if (!string.IsNullOrEmpty(request.ipaddress)) builder.Where("ipaddress = @ipaddress", new { request.ipaddress });
            //if (!string.IsNullOrEmpty(request.location)) builder.Where("macaddress = @location", new { request.location });
            //if (request.status >= 0) builder.Where("status = @Status", new { request.status });
            //if (request.VerifyTime != null) builder.Where("requesttime >= @VerifyTime", new { request.VerifyTime });


            //builder.Where("amount <= @Maxamount", new { Maxamount = _DirectPayStartAmount });
            //builder.Where("level in @Levels", new { Levels = ALLUSER });

            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            //builder.OrderBy("createtime DESC");


            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var adminAccess = connection.Get<ofManage>(request.Admin).access;
                ResponseModel<PagingModel<BaseWithdrawReturn>> searchlimitResponse;
                if (request.starttime != null && !isWithdrawNoSearchDateLimitAllowed(adminAccess, (DateTime)request.starttime, request.password, out searchlimitResponse))
                    return new ResponseModel<PagingModel<代付列表所有Return>>()
                    {
                        code = searchlimitResponse.code,
                        message = searchlimitResponse.message,
                    };


                var rows = connection.Query<代付列表所有Return>(template.RawSql, template.Parameters);
                var totalItems = rows.Count() <= 0 ? 0 : rows.First().TotalItems;


                // Apply the LevelName 
                if (rows.Count() > 0)
                    rows.ToList().ForEach(x =>
                    {
                        //if (!string.IsNullOrEmpty(x.misc))
                        //{
                        //    var y = JObject.Parse(x.misc);
                        //    x.paystat = y["paystat"].ToString() == "true";
                        //    x.paystatreason = y["paystatreason"].ToString();
                        //}
                        if (!string.IsNullOrEmpty(x.misc) && x.cardbank.Contains("USDT"))
                        {
                            var y = JObject.Parse(x.misc);
                            if (y.ContainsKey("usdt_xrate"))
                            {
                                x.usdt_xrate = y["usdt_xrate"].ToString();
                                x.usdt_equiv = (float.Parse(x.amount) / float.Parse(x.usdt_xrate)).ToString("0.000");
                            }
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




                        if (x.delaytotime != null)
                        {
                            var xx = ((DateTime)x.delaytotime - (DateTime)x.createtime).TotalHours;
                            if(xx >=0)
                                x.customer_time = string.Concat("0-",Math.Round(xx));
                        }
                        else
                            x.delaytotimediff = 99999999;


                        if (x.delaytotime != null && x.paytime != null)
                        {
                            x.delaytotimediff = ((DateTime)x.delaytotime - (DateTime)x.reviewtime).TotalMinutes;
                            x.delaytotimediff = Math.Floor(x.delaytotimediff);
                        }
                        else
                            x.delaytotimediff = 99999999;



                        if (x.paytime != null)
                        {
                            x.minutesdiff = (((DateTime)x.paytime) - ((DateTime)x.reviewtime)).TotalMinutes;
                            x.minutesdiff = Math.Floor(x.minutesdiff);
                        }
                        else
                            x.minutesdiff = -1;

                        if (x.statusCode == 1)
                        x.cardno = string.Format("{0}***{1}", x.cardno.Substring(0, 5), x.cardno.Substring(x.cardno.Length - 6, 6));
                        //x.levelname = CMS.Core.CachedData.LevelNameDict[x.level];
                        //x.cardno = Common.Security.ObscureWithdrawCard(x.cardno);
                        //switch (x.status)
                        //{
                        //    case 1:
                        //        if (string.IsNullOrEmpty(x.checkperson))
                        //        {
                        //            x.cardno = "*****";
                        //            x.amount = "*****";
                        //        }
                        //        else
                        //        {
                        //            x.cardno = request.Admin == x.checkperson.ToLower() ? x.cardno : Common.Security.ObscureWithdrawCard(x.cardno);
                        //            x.amount = request.Admin == x.checkperson.ToLower() ? x.amount : "*****";
                        //        }
                        //        break;
                        //    default:
                        //        x.cardno = "*****";
                        //        x.amount = "*****";
                        //        break;
                        //}

                    });



                var result = new ResponseModel<PagingModel<代付列表所有Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                    Data = new PagingModel<代付列表所有Return>()
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

