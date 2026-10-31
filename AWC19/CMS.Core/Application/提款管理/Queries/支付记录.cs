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

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 支付记录Query : BaseWithdrawQuery, IRequest<ResponseModel<PagingModel<支付记录Return>>>
    {
        public string platform { get; set; }
    }

    public class 支付记录Return : BaseWithdrawReturn
    {
        public string platform { get; set; }
        public string result { get; set; }
        public string orderid { get; set; }
        public int statusCode { get; set; }
        public float totalAmount { get; set; }
        public DateTime createtime { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 支付记录QueryValidator : AbstractValidator<支付记录Query>
    {
        public 支付记录QueryValidator()
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
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 支付记录Handler : BaseWithdraw<支付记录Query, ResponseModel<PagingModel<支付记录Return>>>
    {
        public 支付记录Handler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<PagingModel<支付记录Return>> Handle(支付记录Query request)
        {

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
            var template = builder.AddTemplate("" +
                "SELECT (SELECT SUM(amount) From ofPaymentLog WITH(NOLOCK) /**where**/)as TotalAmount, statuscode,level,checkperson, a.account AS account,a.realname AS realname,p.amount as amount,cardbank, requestip, reviewperson, reviewtime, paytime, payperson, result,orderid, platform,createtime,COUNT(*) OVER() as TotalItems  " +
                "FROM (SELECT * FROM ofWithdrawal WITH(NOLOCK) WHERE id IN (SELECT TransactionId FROM ofPaymentLog WITH(NOLOCK) /**where**/))a " +
                "INNER JOIN ofPaymentlog p ON a.id = p.TransactionId " +
                "/**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY");




            // FILL UP THE WHERE CLAUSE
            if (request.starttime != null && request.endtime != null)
                builder.Where("createtime BETWEEN @starttime AND @endtime", new { request.starttime, request.endtime });
            else if (request.starttime != null)
                builder.Where("createtime >= @starttime", new { starttime = request.starttime });
            else
                builder.Where("createtime >= @starttime", new { starttime = DateTime.UtcNow.AddHours(8).AddDays(-7) });

            builder.OrderBy("CREATETIME DESC");
            // FILL UP THE WHERE CLAUSE
            if (!string.IsNullOrEmpty(request.platform)) builder.Where("platform = @platform", new { platform = request.platform });
            if (!string.IsNullOrEmpty(request.account)) builder.Where("account LIKE @account", new { account = string.Concat("%", request.account, "%") });
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
                var rows = connection.Query<支付记录Return>(template.RawSql, template.Parameters);
                var totalItems = rows.Count() <= 0 ? 0 : rows.First().TotalItems;

                // Apply the LevelName 
                if (rows.Count() > 0)
                    rows.ToList().ForEach(x =>
                    {
                        if (x.paytime != null)
                        {
                            x.minutesdiff = ((DateTime)x.paytime - x.requesttime).TotalMinutes;
                            x.minutesdiff = Math.Floor(x.minutesdiff);
                        }
                        else
                            x.minutesdiff = -1;

                        //if(x.statusCode == 1)
                        //x.cardno = string.Format("{0}***{1}", x.cardno.Substring(0, 5), x.cardno.Substring(x.cardno.Length - 6, 6));
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



                var result = new ResponseModel<PagingModel<支付记录Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                    Data = new PagingModel<支付记录Return>()
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

