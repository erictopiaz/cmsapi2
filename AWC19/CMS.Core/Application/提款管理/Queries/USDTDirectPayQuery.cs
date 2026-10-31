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
using UtilsLib.lang;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using ShareEntity.Models;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class USDTDirectPayQuery : BaseWithdrawQuery, IRequest<ResponseModel<PagingModel<USDTReturn>>>
    {
        public int walletType { set; get; } = -1;
        public string? merchantAccount { set; get; } = "";
    }

    public class USDTReturn : BaseWithdrawReturn
    {
        public string walletType { get; set; }
        public string merchantAccount { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class USDTDirectPayQueryValidator : AbstractValidator<USDTDirectPayQuery>
    {
        public USDTDirectPayQueryValidator()
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
    public class USDTDirectPayQueryHandler : BaseWithdraw<USDTDirectPayQuery, ResponseModel<PagingModel<USDTReturn>>>
    {
        public USDTDirectPayQueryHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<PagingModel<USDTReturn>> Handle(USDTDirectPayQuery request)
        {
            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT  a.*,c.*,d.id uid, d.walletType as walletType, d.merchantAccount as merchantAccount ,d.misc ofuser_misc, d.withdrawalamount-d.depositamount user_winloss  FROM (" +
                "SELECT ofWithdrawal.*, ofUser.walletType, ofUser.merchantAccount, COUNT(*) OVER() as TotalItems " +
                "FROM ofWithdrawal " +
                "JOIN ofUser WITH(NOLOCK) ON ofWithdrawal.account = ofuser.account " +
                "/**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY)a " +
                "LEFT Join ofuserExtention c on c.account=a.account " +
                "LEFT Join ofUser d on d.account=a.account");



            // FILL UP THE WHERE CLAUSE
            if (request.starttime != null && request.endtime != null && request.status == 2)
            {
                template = builder.AddTemplate("" +
                "SELECT  a.*,c.*,d.misc ofuser_misc,d.id uid, d.walletType as walletType, d.merchantAccount as merchantAccount , d.withdrawalamount-d.depositamount user_winloss  FROM (" +
                "SELECT ofWithdrawal.*, ofUser.walletType, ofUser.merchantAccount , COUNT(*) OVER() as TotalItems, SUM(amount) OVER() as TotalAmount " +
                   "FROM ofWithdrawal " +
                   "JOIN ofUser WITH(NOLOCK) ON ofWithdrawal.account = ofuser.account " +
                   "/**where**/ /**orderby**/ " +
                   "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY)a " +
                "LEFT Join ofuserExtention c on c.account=a.account " +
                "LEFT Join ofUser d on d.account=a.account");

                builder.Where("requesttime BETWEEN @starttime AND @endtime", new { request.starttime, request.endtime });
            }
            else if (request.starttime != null && request.endtime != null)
                builder.Where("requesttime BETWEEN @starttime AND @endtime", new { request.starttime, request.endtime });
            else if (request.starttime != null)
                builder.Where("requesttime >= @starttime", new { starttime = request.starttime });
            else
                builder.Where("requesttime >= @starttime", new { starttime = DateTime.UtcNow.AddHours(8).AddMonths(-2) });

            // FILL UP THE WHERE CLAUSE
            if (!string.IsNullOrEmpty(request.account)) builder.Where("ofWithdrawal.account LIKE @account", new { account = string.Concat("%", request.account, "%") });
            if (!string.IsNullOrEmpty(request.cardno)) builder.Where("CardNo = @CardNo", new { request.cardno });
            if (!string.IsNullOrEmpty(request.realname)) builder.Where("ofWithdrawal.realname = @realname", new { request.realname });
            if (!string.IsNullOrEmpty(request.affaccount)) builder.Where("affaccount = @affaccount", new { request.affaccount });
            if (!string.IsNullOrEmpty(request.ipaddress)) builder.Where("ofWithdrawal.ipaddress = @ipaddress", new { request.ipaddress });
            if (!string.IsNullOrEmpty(request.location)) builder.Where("macaddress = @location", new { request.location });
            if (request.status >= 0) builder.Where("ofWithdrawal.status = @Status", new { request.status });
            if (request.currencycode >= 0) builder.Where("ofWithdrawal.currencycode = @currencycode", new { request.currencycode });
            if (!string.IsNullOrEmpty(request.merchantAccount) && request.merchantAccount != "-1") { builder.Where("ofUser.merchantAccount = @merchantAccount", new { request.merchantAccount }); }
            if (request.walletType >= 0) { builder.Where("ofUser.walletType = @walletType", new { request.walletType }); }

            if (request.processingtime > 0)
                builder.Where("DATEDIFF(MINUTE, reviewtime,paytime) >= @processingtime", new { request.processingtime });

            #region ching 添加 20241029 日，新加支付宝和微信提款查询  （如果不是支付宝或微信，就只查询usdt的代付）
            if (request.paycategory == 19) //支付宝
                builder.Where("wid in (4, @paycategory)", new { paycategory = request.paycategory });
            else if (request.paycategory == 20) //微信
                builder.Where("wid in (5, @paycategory)", new { paycategory = request.paycategory });
            else if (request.paycategory == -2)
                builder.Where("wid in (19,20,4,5)");//返回 支付宝和微信 代付单
            else
                builder.Where("cardbank in ('USDT', 'USDT-ERC20', 'USDT-TRC20')");
            #endregion

            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("requesttime DESC");



            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var adminAccess = connection.Get<ofManage>(request.Admin).access;
                ResponseModel<PagingModel<BaseWithdrawReturn>> searchlimitResponse;
                if (!isWithdrawNoSearchDateLimitAllowed(adminAccess, (DateTime)request.starttime, request.password, out searchlimitResponse))
                {
                    var response = new ResponseModel<PagingModel<USDTReturn>>()
                    {
                        code = searchlimitResponse.code,
                        message = searchlimitResponse.message,
                        Data = null
                    };
                }
                   

                var rows = connection.Query<USDTReturn>(template.RawSql, template.Parameters);
                var totalItems = rows.Count() <= 0 ? 0 : rows.First().TotalItems;

                var item_1 = 0f;
                if (rows != null && rows.Count() > 0
                    && request.starttime != null && request.endtime != null) {
                    item_1 = rows.First().TotalAmount;
                }

                // Apply the LevelName 
                if (rows.Count() > 0)
                    rows.ToList().ForEach(x =>
                    {
                        x.TotalAmount = 0;
                        // Change USDT to USDT-ERC20
                        x.cardbank = x.cardbank == "USDT" ? "USDT-ERC20" : x.cardbank;

                        if (!string.IsNullOrEmpty(x.misc))
                        {
                            var y = JObject.Parse(x.misc);
                            if (y.ContainsKey("paystat") && y.ContainsKey("paystatreason"))
                            {
                                x.paystat = y["paystat"].ToString() == "true";
                                x.paystatreason = y["paystatreason"].ToString();
                            }

                            if (y.ContainsKey("usdt_xrate")) {

                                if (x.currencycode == 356)
                                {

                                    x.usdt_xrate = y["inr_usdt_xrate"].ToString();
                                    x.usdt_equiv = (float.Parse(x.amount) / float.Parse(x.usdt_xrate)).ToString("0.000");
                                }
                                else {
                                    x.usdt_xrate = y["usdt_xrate"].ToString();
                                    x.usdt_equiv = (float.Parse(x.amount) / float.Parse(x.usdt_xrate)).ToString("0.000");

                                }
                            }



                            if (y.ContainsKey("origamount"))
                                x.origamount = y["origamount"].ToString();
                                x.xfermanual = y.ContainsKey("xfermanual") ? y["xfermanual"].ToString() : "";
                        }

                        if (x.delaytotime != null)
                        {
                            x.delaytotimediff = ((DateTime)x.delaytotime - x.reviewtime).TotalMinutes;
                            x.delaytotimediff = Math.Floor(x.delaytotimediff);
                        }
                        else { 
                            x.delaytotime = x.requesttime.AddMinutes(30);
                            x.delaytotimediff = ((DateTime)x.delaytotime - x.reviewtime).TotalMinutes;
                            x.delaytotimediff = Math.Floor(x.delaytotimediff);
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
                                // ***** ERIC 要求给显示 2025-10-24 ***** 
                                //if (x.user_winloss < 0)
                                //    x.account_tags = "***";
                                //else
                                x.account_tags = y["tags"].ToString();
                            }
                        }


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

                        #region 合成支付宝和微信的收款码地址
                        if (x.wid == 19 || x.wid == 20 || x.wid == 4 || x.wid == 5) {
                            var qrUrl = string.Format(@"https://j33.app/payqr/{0}.png", x.cardno);
                            x.images.Add(qrUrl);
                        }
                        #endregion

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
                                    x.amount = request.Admin == x.checkperson.ToLower() ? x.amount : "*****";
                                    //x.cardno = "*****";
                                }
                                break;
                            default:
                                x.cardno = "*****";
                                x.amount = "*****";
                                break;
                        }

                    });

                if (rows != null && rows.Count() > 0
                   && request.starttime != null && request.endtime != null) {
                    rows.First().TotalAmount = item_1;
                }

                var result = new ResponseModel<PagingModel<USDTReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                    Data = new PagingModel<USDTReturn>()
                    {
                        TotalItems = totalItems,
                        Data = rows,
                    }
                };

                // TODO: Optimization here
                return result;
            }
        }
    }
}

