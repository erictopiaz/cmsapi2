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
using Newtonsoft.Json.Linq;
using Dapper.Contrib.Extensions;
using UtilsLib.lang;
using CMS.Core.Application.Account.Queries;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class AllRecord : BaseWithdrawQuery, IRequest<ResponseModel<PagingModel<AllRecordReturn>>>
    {
        public int walletType { set; get; } = -1;
        public string? merchantAccount { set; get; } = "";
        public int levelgroup { get; set; }
        public string cardaddress { get; set; }

        public string uid { get; set; }

        public string withdrawamount { get; set; }
        public int? paycategory { get; set; } = -1;
    }

    public class AllRecordReturn : BaseWithdrawReturn
    {
        public string walletType { get; set; }
        public string merchantAccount { get; set; }
        //public decimal TotalFee { get; set; }
        public float TotalUSDT { get; set; }
        public double minutesdiff { get; set; }
        public string reviewperson { get; set; }
        public bool xferriskmgt { get; set; }
        public string delayToTimeInHours { get; set; }
        public string requestip { get; set; }
        public string requestaddress { get; set; }

        public long haveconformed { get; set; }
        /// <summary>
        /// 提款分类： 3：极速提款
        /// </summary>
        public int crosstransact { set; get; }
        public int pendingconform { set; get; }
        public int wid { set; get; }
        /// <summary>
        /// 极速提款确认到账
        /// </summary>
        public decimal reward { set; get; }
        public decimal selfreward { set; get; }
        /// <summary>
        /// 是否被回撤过定单(次数)
        /// </summary>
        public int alreadyReturnTimes { set; get; } = 0;
        public string preReturnAmount { set; get; } = "";

        /// <summary>
        /// ofUserMisc
        /// </summary>
        public string userMisc { set; get; }
        /// <summary>
        /// 付款标记，从 userMisc中解析出来
        /// </summary>
        public string tags { set; get; }

    }


    /// <summary>
    /// API Model Validation
    /// </summary>
    public class AllRecordValidator : AbstractValidator<AllRecord>
    {
        public AllRecordValidator()
        {
            RuleFor(x => x.account).Account();
            RuleFor(x => x.uid).Account();
            //RuleFor(x => x.cardno).CardNo();
            RuleFor(x => x.status).Status();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class AllRecordHandler : BaseWithdraw<AllRecord, ResponseModel<PagingModel<AllRecordReturn>>>
    {
        public AllRecordHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        
        protected override ResponseModel<PagingModel<AllRecordReturn>> Handle(AllRecord request)
        {
            request.cardno = string.IsNullOrEmpty(request.cardno) ? "" : request.cardno.Replace("'", "''");

            if (!request.currencys.Contains(request.currencycode))
            {
                return new ResponseModel<PagingModel<AllRecordReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择货币查询"
                };
            }

            if (!string.IsNullOrEmpty(request.uid)) {
                var account = AccountBase.QueryAccountById(Convert.ToInt64(request.uid));
                request.account = string.IsNullOrEmpty(account) ? request.account : account;
            }

            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();

            bool withwhere = false;
            bool withTimeRange = false;

            // FILL UP THE WHERE CLAUSE
            if (request.starttime != null && request.endtime != null)
            {
                if (request.status == 2)
                {
                    //Prior to Kay Request
                    builder.Where("paytime >= @starttime", new { request.starttime });
                    builder.Where("paytime <= @endtime", new { request.endtime });

                    //builder.Where("requesttime >= @starttime", new { request.starttime });
                    //builder.Where("requesttime <= @endtime", new { request.endtime });
                }
                else
                {
                    builder.Where("requesttime BETWEEN @starttime AND @endtime", new { request.starttime, request.endtime });
                }

                withTimeRange = request.status == 1;
                withTimeRange |= request.status == 2;
                withwhere = true;
            }
            else if (request.starttime != null)
            {
                builder.Where("requesttime >= @starttime", new { starttime = request.starttime });
                withTimeRange = request.status == 1;
                withTimeRange |= request.status == 2;
                withwhere = true;
            }
            else if (request.starttime == null && request.endtime == null)
            {
                builder.Where("requesttime >= @starttime", new { starttime = Common.CurrentTime.AddDays(-15)});
            }
            // FILL UP THE WHERE CLAUSE


            if (!string.IsNullOrEmpty(request.account))
            {
                //withwhere = true; builder.Where("W.Account = @Account", new { request.account });
                builder.Where("W.Account LIKE @account", new { account = string.Concat("%", request.account, "%") });
            }


            if (!string.IsNullOrEmpty(request.cardno))
            {
                withwhere = true; builder.Where("CardNo = @CardNo", new { request.cardno });
            }

            //确认当前管理员账号---是否只能查看直所开代理的会员账号
            var adminAccess = CachedData.AccessString(request.Admin);
            if (adminAccess.IndexOf("/api/Admin/QueryAgent", StringComparison.OrdinalIgnoreCase) >= 0)//账号已经被控了，只能看到自己开的代理和代理下线
            {
                withwhere = true; builder.Where("ofWithdrawal.affaccount in (select account from ofAffiliate where verifyperson=@admin)", 
                    new { admin = request.Admin });

            }

            if (request.currencycode > 0) { builder.Where("currencycode = @currencycode", new { request.currencycode }); }
            if (!string.IsNullOrEmpty(request.realname)) { withwhere = true; builder.Where("realname = @realname", new { request.realname }); }
            if (!string.IsNullOrEmpty(request.affaccount)) { withwhere = true; builder.Where("ccc.affaccount = @affaccount", new { request.affaccount }); }
            if (!string.IsNullOrEmpty(request.ipaddress)) { withwhere = true; builder.Where("ipaddress = @ipaddress", new { request.ipaddress }); }
            if (!string.IsNullOrEmpty(request.location)) { withwhere = true; builder.Where("macaddress = @location", new { request.location }); }
            if (request.conformedstatus >=0) { withwhere = true; builder.Where("conformedstatus = @conformedstatus", new { request.conformedstatus }); }
            if (!string.IsNullOrEmpty(request.merchantAccount) && request.merchantAccount != "-1") { builder.Where("ccc.merchantAccount = @merchantAccount", new { request.merchantAccount }); }
            if (request.walletType >= 0) { builder.Where("ccc.walletType = @walletType", new { request.walletType }); }
            if (!string.IsNullOrEmpty(request.withdrawamount)) {
                int minAmount = 0;
                int maxAmount = 0;
                if (request.withdrawamount.Contains("-"))
                {
                    var withdrawAmountArray = request.withdrawamount.Split("-");
                    if (int.TryParse(withdrawAmountArray[0].Trim(), out minAmount))
                    {
                        if (int.TryParse(withdrawAmountArray[1].Trim(), out maxAmount))
                        {
                            withwhere = true;
                            builder.Where("amount BETWEEN @MINAMOUNT AND @MAXAMOUNT", new { MINAMOUNT = minAmount, MAXAMOUNT = maxAmount });
                        }
                    }

                }
                else {
                    if (int.TryParse(request.withdrawamount, out minAmount))
                    {
                        withwhere = true;
                        builder.Where("amount <= @MINAMOUNT", new { MINAMOUNT = minAmount });
                    }
                }
                
            }

            if (!string.IsNullOrEmpty(request.cardaddress)) { withwhere = true; builder.Where("cardaddress LIKE @cardaddress", new { cardaddress = string.Concat("%",request.cardaddress, "%") }); }
            if (request.status >= 0) { withwhere = true; builder.Where("w.status = @Status", new { request.status }); }
            if (request.levelgroup == 0)
            {
                //withwhere = true;
                builder.Where("w.level in @Levels", new { Levels = 普通会员_和_万象会员 });
            }
            if (request.levelgroup == 1)
            {
                //withwhere = true;
                builder.Where("w.level not in @Levels", new { Levels = 普通会员_和_万象会员 });
            }


            if (request.paycategory >=0 ) { 
                withwhere = true;
                switch (request.paycategory)
                {
                    case 0:
                        builder.Where("wid = 3"); break;
                    case 1:
                        builder.Where("wid = 1"); break;
                    case 2:
                        builder.Where("wid = 2");
                        builder.Where("cardbank IN ('EB','EBpay')"); break;
                    case 3:
                        builder.Where("wid = 2");
                        builder.Where("cardbank IN ('CGP','CGPay')"); break;
                    case 4:
                        builder.Where("wid = 2");
                        builder.Where("cardbank LIKE 'GoPay%'"); break;
                    case 7:
                        builder.Where("wid = 2");
                        builder.Where("cardbank LIKE 'USDT%'"); break;
                    case 19:
                        builder.Where("wid in (4,19)"); break; //支付宝
                    case 20:
                        builder.Where("wid in (5,20)"); break; //微信
                    default: break;
                }
            }


            //if (request.VerifyTime != null) builder.Where("requesttime >= @VerifyTime", new { request.VerifyTime });

            string _pendingconfirmstart =  Common.CurrentTime.AddDays(-7).ToString("yyyy-MM-dd");
            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize, pendingconfirmstart= _pendingconfirmstart });
            builder.OrderBy("requesttime DESC");


            SqlBuilder.Template template;



            if (withwhere && withTimeRange && request.paycategory == 7)
            {
                template = builder.AddTemplate("" +
                "SELECT ISNULL(crosstransact,0) as crosstransact,ISNULL(bbb.selfreward ,0) as selfreward, ISNULL(r.haveconformed ,0) as haveconformed,z.platform as payplatform, pendingconform,a.*,c.*,cc.id as uid,cc.misc as userMisc,iif(( b.reward IS not NULL and LEN(RTRIM(LTRIM(b.reward))) != 0), b.reward,0) reward FROM(" +
                "SELECT w.* ,ccc.walletType, ccc.merchantAccount, " +

                //{ "usdt_xrate":6.73,"usdt_xrate_withdraw_topup":"0.05","usdt_xrate_deposit_topup":"0.05","huobi_usdt_xrate":"-1","cryptocompare_usdt_xrate":"7.433","orig_usdt_xrate":"6.68"}

                    "sum(ROUND(amount / CAST(JSON_VALUE(w.misc,'$.usdt_xrate') AS float),2)) OVER() as TotalUSDT, " +
                    "COUNT(*) OVER() as TotalItems, " +
                    "SUM(amount) OVER() as TotalAmount, " +
                    "SUM(fee) OVER() as TotalFee " +
                "FROM ofWithdrawal w WITH(NOLOCK) " +
                "LEFT Join ofuser ccc WITH(NOLOCK) on ccc.account= w.account " +
                "/**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY)a " +
                "LEFT Join ofuser cc WITH(NOLOCK) on cc.account=a.account " +
                "LEFT Join ofuserExtention c WITH(NOLOCK) on c.account=a.account " +
                "outer apply (select sum(reward) as reward from ofBonuspromo bb WITH(NOLOCK) WHERE a.referenceguid = bb.referenceguid) b " +
                "outer apply (select sum(reward) as selfreward from ofBonuspromoself bbs WITH(NOLOCK) WHERE a.account = bbs.account) bbb " +
                "outer apply (select top 1 id as haveconformed from ofWithdrawal ww WITH(NOLOCK) WHERE a.account = ww.account and status =2 and conformedStatus=1) r " +
                "left join ofPaymentLog z WITH(NOLOCK) on z.TransId=a.id " +
                "outer apply (select count(*) as pendingconform from ofCrossTransact aoc left join ofwithdrawal aow on aoc.withdrawid = aow.id where aoc.createtime > @pendingconfirmstart AND aoc.account = a.account and aow.conformedStatus = 0) rr " +
                //"outer apply (select Count(*) as pendingconform from ofWithdrawal ow1 WITH(NOLOCK) where ow1.requesttime > (SELECT TOP 1 requesttime FROM ofWithdrawal ow WITH(NOLOCK) WHERE requesttime > @pendingconfirmstart AND ow.account = a.account and ow.status = 2 and ow.wid = 3 and ow.conformedStatus = 1 order by requesttime desc) AND ow1.account = a.account AND ow1.WID = 3 ) rr " +
                "outer apply (select top 1 id as crosstransact from ofcrosstransact where account = a.account and status = 1) dd");
            }
            else if (withwhere && withTimeRange)
            {
                template = builder.AddTemplate("" +
                "SELECT  ISNULL(crosstransact,0) as crosstransact,ISNULL(bbb.selfreward ,0) as selfreward,ISNULL(r.haveconformed ,0) as haveconformed, z.platform as payplatform,pendingconform,a.*,c.*,cc.id as uid,cc.misc as userMisc,iif(( b.reward IS not NULL and LEN(RTRIM(LTRIM(b.reward))) != 0), b.reward,0) reward FROM(" +
                "SELECT w.* ,ccc.walletType, ccc.merchantAccount, " +
                    "COUNT(*) OVER() as TotalItems, " +
                    "SUM(amount) OVER() as TotalAmount, " +
                    "SUM(fee) OVER() as TotalFee " +
                "FROM ofWithdrawal w WITH(NOLOCK) " +
                "LEFT Join ofuser ccc WITH(NOLOCK) on ccc.account= w.account " +
                "/**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY)a " +
                "LEFT Join ofuser cc WITH(NOLOCK) on cc.account=a.account " +
                "LEFT Join ofuserExtention c  WITH(NOLOCK) on c.account=a.account " +
                "outer apply (select sum(reward) as reward from ofBonuspromo bb WITH(NOLOCK) WHERE a.referenceguid = bb.referenceguid) b " +
                "outer apply (select sum(reward) as selfreward from ofBonuspromoself bbs WITH(NOLOCK) WHERE a.account = bbs.account) bbb " +
                "outer apply (select top 1 id as haveconformed from ofWithdrawal ww WITH(NOLOCK) WHERE a.account = ww.account and status =2 and conformedStatus=1) r " +
                "left join ofPaymentLog z WITH(NOLOCK) on z.TransId=a.id " +
                "outer apply (select count(*) as pendingconform from ofCrossTransact aoc left join ofwithdrawal aow on aoc.withdrawid = aow.id where aoc.createtime > @pendingconfirmstart AND aoc.account = a.account and aow.conformedStatus = 0) rr " +

                //"outer apply (select Count(*) as pendingconform from ofWithdrawal ow1 WITH(NOLOCK) where ow1.requesttime > (SELECT TOP 1 requesttime FROM ofWithdrawal ow WITH(NOLOCK) WHERE requesttime > @pendingconfirmstart AND ow.account = a.account and ow.status = 2 and ow.wid = 3 and ow.conformedStatus = 1 order by requesttime desc) AND ow1.account = a.account AND ow1.WID = 3 ) rr " +
                "outer apply (select top 1 id as crosstransact from ofcrosstransact where account = a.account and status = 1) dd");                //"outer apply(select Count(*) as pendingconform from ofWithdrawal ow1 WITH(NOLOCK) where ow1.account = a.account AND ow1.WID = 3 AND ow1.requesttime > (SELECT TOP 1 requesttime FROM ofWithdrawal ow WITH(NOLOCK) WHERE ow.account = a.account and ow.status = 2 and ow.wid = 3 and ow.conformedStatus = 1 order by requesttime desc)) rr");
            }
            else if (withwhere)
            {   
                template = builder.AddTemplate("" +
                "SELECT  ISNULL(crosstransact,0) as crosstransact, ISNULL(bbb.selfreward ,0) as selfreward,ISNULL(r.haveconformed ,0) as haveconformed,z.platform as payplatform,pendingconform,a.*,c.*,cc.id as uid,cc.misc as userMisc,iif(( b.reward IS not NULL and LEN(RTRIM(LTRIM(b.reward))) != 0), b.reward,0) reward FROM(" +
                "SELECT w.* ,ccc.walletType, ccc.merchantAccount, COUNT(*) OVER() as TotalItems " +
                "FROM ofWithdrawal w WITH(NOLOCK) " +
                "LEFT Join ofuser ccc WITH(NOLOCK) on ccc.account= w.account " +
                "/**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY)a " +
                "LEFT Join ofuser cc WITH(NOLOCK) on cc.account=a.account " +
                "LEFT Join ofuserExtention c WITH(NOLOCK) on c.account=a.account " +
                "outer apply (select sum(reward) as reward from ofBonuspromo bb WITH(NOLOCK) WHERE a.referenceguid = bb.referenceguid) b " +
                "outer apply (select sum(reward) as selfreward from ofBonuspromoself bbs WITH(NOLOCK) WHERE a.account = bbs.account) bbb " +
                "outer apply (select top 1 id as haveconformed from ofWithdrawal ww WITH(NOLOCK) WHERE a.account = ww.account and status =2 and conformedStatus=1) r " +
                "left join ofPaymentLog z WITH(NOLOCK) on z.TransId=a.id " +
                "outer apply (select count(*) as pendingconform from ofCrossTransact aoc left join ofwithdrawal aow on aoc.withdrawid = aow.id where aoc.createtime > @pendingconfirmstart AND aoc.account = a.account and aow.conformedStatus = 0) rr " +

                //"outer apply (select Count(*) as pendingconform from ofWithdrawal ow1 WITH(NOLOCK) where ow1.requesttime > (SELECT TOP 1 requesttime FROM ofWithdrawal ow WITH(NOLOCK) WHERE requesttime > @pendingconfirmstart AND ow.account = a.account and ow.status = 2 and ow.wid = 3 and ow.conformedStatus = 1 order by requesttime desc) AND ow1.account = a.account AND ow1.WID = 3 ) rr " +
                "outer apply (select top 1 id as crosstransact from ofcrosstransact where account = a.account and status = 1) dd");
                //"outer apply(select Count(*) as pendingconform from ofWithdrawal ow1 WITH(NOLOCK) where ow1.account = a.account AND ow1.WID = 3 AND ow1.requesttime > (SELECT TOP 1 requesttime FROM ofWithdrawal ow WITH(NOLOCK) WHERE ow.account = a.account and ow.status = 2 and ow.wid = 3 and ow.conformedStatus = 1 order by requesttime desc)) rr");; ;
            }
            else
            {
                template = builder.AddTemplate("" +
                "SELECT ISNULL(crosstransact,0) as crosstransact, ISNULL(bbb.selfreward ,0) as selfreward,ISNULL(r.haveconformed ,0) as haveconformed,z.platform as payplatform,pendingconform,a.*,c.*,cc.id as uid,iif(( b.reward IS not NULL and LEN(RTRIM(LTRIM(b.reward))) != 0), b.reward,0) reward FROM(" +
                "SELECT w.* ,ccc.walletType as walletType, ccc.merchantAccount as merchantAccount, TotalItems =1000 " +
                "FROM ofWithdrawal w WITH(NOLOCK) " +
                "LEFT Join ofuser ccc WITH(NOLOCK) on ccc.account= w.account " +
                "/**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY)a " +
                "LEFT Join ofuser cc WITH(NOLOCK) on cc.account=a.account " +
                "LEFT Join ofuserExtention c WITH(NOLOCK) on c.account=a.account " +
                "outer apply (select sum(reward) as reward from ofBonuspromo bb WITH(NOLOCK) WHERE a.referenceguid = bb.referenceguid) b " +
                "outer apply (select sum(reward) as selfreward from ofBonuspromoself bbs WITH(NOLOCK) WHERE a.account = bbs.account) bbb " +
                "outer apply (select top 1 id as haveconformed from ofWithdrawal ww WITH(NOLOCK) WHERE a.account = ww.account and status =2 and conformedStatus=1) r " +
                "left join ofPaymentLog z WITH(NOLOCK) on z.TransId=a.id " +
                "outer apply (select count(*) as pendingconform from ofCrossTransact aoc left join ofwithdrawal aow on aoc.withdrawid = aow.id where aoc.createtime > @pendingconfirmstart AND aoc.account = a.account and aow.conformedStatus = 0) rr " +

                //"outer apply (select Count(*) as pendingconform from ofWithdrawal ow1 WITH(NOLOCK) where ow1.requesttime > (SELECT TOP 1 requesttime FROM ofWithdrawal ow WITH(NOLOCK) WHERE requesttime > @pendingconfirmstart AND ow.account = a.account and ow.status = 2 and ow.wid = 3 and ow.conformedStatus = 1 order by requesttime desc) AND ow1.account = a.account AND ow1.WID = 3 ) rr " +
                "outer apply (select top 1 id as crosstransact from ofcrosstransact where account = a.account and status = 1) dd");
            }


            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                // 搜索限制
                //var adminAccess = connection.Get<ofManage>(request.Admin).access;
                ResponseModel<PagingModel<BaseWithdrawReturn>> searchlimitResponse;
                if (request.starttime !=null && 
                    !isWithdrawNoSearchDateLimitAllowed(adminAccess, (DateTime)request.starttime, request.password, out searchlimitResponse))
                    return new ResponseModel<PagingModel<AllRecordReturn>>()
                    {
                        code = searchlimitResponse.code,
                        message = searchlimitResponse.message,
                    };

                


                var rows = connection.Query<AllRecordReturn>(template.RawSql, template.Parameters);
                var totalItems = rows.Count() <= 0 ? 0 : rows.First().TotalItems;
                if (rows.Count() > 0)
                    rows.ToList().ForEach(x => Common.Security.ObscureWithdrawCard(x.cardno));

                // Apply the LevelName 
                if (rows.Count() > 0)
                    rows.ToList().ForEach(x =>
                    {
                        var w_misc = JsonConvert.DeserializeObject<Dictionary<string, object>>(x.misc);
                        var dkey = "preRuturnAmount";//当前订单原始金额，撤单后不变
                        if (w_misc.ContainsKey(dkey)) {
                            x.preReturnAmount = w_misc[dkey].ToString();
                        }

                        if (!string.IsNullOrEmpty(x.userMisc)) {
                            var u_misc = JsonConvert.DeserializeObject<Dictionary<string, object>>(x.userMisc);
                            x.tags = u_misc.ContainsKey("tags") && u_misc["tags"] != null ? u_misc["tags"].ToString() : "";
                            x.userMisc = "";
                        }
                        else {
                            x.tags = "";
                        }


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

                            if (y.ContainsKey("origamount"))
                                x.origamount = y["origamount"].ToString();
                            x.xfermanual = y.ContainsKey("xfermanual") ? y["xfermanual"].ToString() : "";
                            x.xferriskmgt = y.ContainsKey("xferriskmgt") ? (bool)y["xferriskmgt"] : false;
                        }


                        if (x.crosstransact == 0) {
                            x.haveconformed = 1;
                        }

                        if (x.delaytotime != null)
                        {
                            var _temp = ((DateTime)x.delaytotime - x.requesttime).TotalHours;
                            x.delayToTimeInHours = string.Concat("0-", Math.Round(_temp), "H");
                            if (_temp <= 3) x.delayToTimeInHours = "0-3H";
                            if (_temp <= 2) x.delayToTimeInHours = "0-2H";
                            if (_temp <= 1) x.delayToTimeInHours = "0-1H";
                            if (_temp < 0.6) x.delayToTimeInHours = "0-30M";
                            //x.minutesdiff = Math.Floor(x.minutesdiff);
                        }

                        if (x.reviewtime != null && x.reviewtime.Year > 2000)
                        {
                            x.minutesdiff = (x.reviewtime - x.requesttime).TotalMinutes;
                            x.minutesdiff = Math.Floor(x.minutesdiff);
                        }
                        else if (!string.IsNullOrEmpty(x.canceltime))
                        {
                            x.minutesdiff = (DateTime.Parse(x.canceltime) - x.requesttime).TotalMinutes;
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

                        x.cardno = Common.Security.ObscureWithdrawCard(x.cardno);
                        switch (x.status)
                        {
                            case 1:
                                if (string.IsNullOrEmpty(x.checkperson))
                                {
                                    //x.cardno = "*****";
                                    //x.amount = "*****";
                                }
                                else
                                {
                                    //x.cardno = request.Admin == x.checkperson.ToLower() ? x.cardno : "*****";
                                    //x.amount = request.Admin == x.checkperson.ToLower() ? x.amount : "*****";
                                }
                                break;
                            default:
                                //x.cardno = "*****";
                                //x.amount = "*****";
                                break;
                        }

                    });

               


                var result = new ResponseModel<PagingModel<AllRecordReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                    Data = new PagingModel<AllRecordReturn>()
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

