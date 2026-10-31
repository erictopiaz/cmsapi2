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
using Dapper.Contrib.Extensions;
using UtilsLib.lang;
using ShareEntity.Models;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class RiskMgmtRecord : BaseWithdrawQuery, IRequest<ResponseModel<PagingModel<RiskMgmtRecordReturn>>>
    {
        public int levelgroup { get; set; }
        public int walletType { set; get; } = -1;
        public string? merchantAccount { set; get; } = "";
    }

    public class RiskMgmtRecordReturn : BaseWithdrawReturn
    {
        public string walletType { get; set; }
        public string merchantAccount { get; set; }
        //public decimal TotalFee { get; set; }
        //public decimal TotalAmount { get; set; }
        public double minutesdiff { get; set; }
        public string reviewperson { get; set; }
        public bool xferriskmgt { get; set; }



    }


    /// <summary>
    /// API Model Validation
    /// </summary>
    public class RiskMgmtRecordValidator : AbstractValidator<RiskMgmtRecord>
    {
        public RiskMgmtRecordValidator()
        {
            RuleFor(x => x.account).Account();
            RuleFor(x => x.cardno).CardNo();
            RuleFor(x => x.status).Status();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class RiskMgmtRecordHandler : BaseWithdraw<RiskMgmtRecord, ResponseModel<PagingModel<RiskMgmtRecordReturn>>>
    {
        public RiskMgmtRecordHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<PagingModel<RiskMgmtRecordReturn>> Handle(RiskMgmtRecord request)
        {
            if (!request.currencys.Contains(request.currencycode))
            {
                return new ResponseModel<PagingModel<RiskMgmtRecordReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择货币查询"
                };
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

            // FILL UP THE WHERE CLAUSE
            if (!string.IsNullOrEmpty(request.merchantAccount) && request.merchantAccount != "-1") { builder.Where("ofUser.merchantAccount = @merchantAccount", new { request.merchantAccount }); }
            if (request.walletType >= 0) { builder.Where("ofUser.walletType = @walletType", new { request.walletType }); }
            if (request.currencycode > 0) { builder.Where("ofwithdrawal.currencycode = @currencycode", new { request.currencycode }); }
            if (!string.IsNullOrEmpty(request.account)) { withwhere = true; builder.Where("ofwithdrawal.Account = @Account", new { request.account }); }
            if (!string.IsNullOrEmpty(request.cardno))
            {
                withwhere = true; builder.Where("CardNo = @CardNo", new { request.cardno });
            }

            if (!string.IsNullOrEmpty(request.realname)) { withwhere = true; builder.Where("realname = @realname", new { request.realname }); }
            if (!string.IsNullOrEmpty(request.affaccount)) { withwhere = true; builder.Where("affaccount = @affaccount", new { request.affaccount }); }
            if (!string.IsNullOrEmpty(request.ipaddress)) { withwhere = true; builder.Where("ipaddress = @ipaddress", new { request.ipaddress }); }
            if (!string.IsNullOrEmpty(request.location)) { withwhere = true; builder.Where("macaddress = @location", new { request.location }); }
            if (request.status >= 0) { withwhere = true; builder.Where("ofwithdrawal.status = @Status", new { request.status }); }
            if (request.levelgroup == 0)
            {
                //withwhere = true;
                builder.Where("ofwithdrawal.level in @Levels", new { Levels = 普通会员_和_万象会员 });
            }
            if (request.levelgroup == 1)
            {
                //withwhere = true;
                builder.Where("ofwithdrawal.level not in @Levels", new { Levels = 普通会员_和_万象会员 });
            }

            builder.Where("ofwithdrawal.misc LIKE '%xferriskmgt%'");

            //if (request.VerifyTime != null) builder.Where("requesttime >= @VerifyTime", new { request.VerifyTime });

            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("id DESC");

            SqlBuilder.Template template;
            //var template = new SqlBuilder();
            if (withwhere && withTimeRange)
            {
                template = builder.AddTemplate("" +
                    "select a.*, u.id as uid , u.walletType as walletType, u.merchantAccount as merchantAccount from" +
                "(SELECT ofWithdrawal.*, ofUser.walletType, ofUser.merchantAccount, " +
                    "COUNT(*) OVER() as TotalItems, " +
                    "SUM(amount) OVER() as TotalAmount, " +
                    "SUM(fee) OVER() as TotalFee " +
                "FROM ofWithdrawal WITH(NOLOCK) " +
                "JOIN ofUser WITH(NOLOCK) ON ofWithdrawal.account = ofuser.account " +
                "/**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY) a " +
                "left join ofUser u WITH(nolock) on u.account= a.account ");
            }
            else if (withwhere)
            {
                template = builder.AddTemplate("" +
                    " select a.*, u.id as uid, u.walletType as walletType, u.merchantAccount as merchantAccount  from " +
                "(SELECT ofWithdrawal.*, COUNT(*) OVER() as TotalItems " +
                "FROM ofWithdrawal WITH(NOLOCK) " +
                "JOIN ofUser WITH(NOLOCK) ON ofWithdrawal.account = ofuser.account " +
                "/**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY) a " +
                "left join ofUser u WITH(nolock) on u.account= a.account ");
            }
            else
            {
                template = builder.AddTemplate("" +
                    "select a.*, u.id as uid, u.walletType as walletType, u.merchantAccount as merchantAccount  from " +
                "( SELECT  ofWithdrawal.*, TotalItems =1000 " +
                "FROM ofWithdrawal WITH(NOLOCK) " +
                "JOIN ofUser WITH(NOLOCK) ON ofWithdrawal.account = ofuser.account " +
                "/**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY ) a " +
                "left join ofUser u WITH(nolock) on a.account= u.account ");


                //template = builder.AddTemplate("" +
                //   " SELECT *, TotalItems = 1000 " +
                //"FROM ofWithdrawal WITH(NOLOCK) /**where**/ /**orderby**/ " +
                //"OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                //"FETCH NEXT @Next ROWS ONLY");
            }




            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var adminAccess = connection.Get<ofManage>(request.Admin).access;
                ResponseModel<PagingModel<BaseWithdrawReturn>> searchlimitResponse;
                if (request.starttime != null && !isWithdrawNoSearchDateLimitAllowed(adminAccess, (DateTime)request.starttime, request.password, out searchlimitResponse))
                    return new ResponseModel<PagingModel<RiskMgmtRecordReturn>>()
                    {
                        code = searchlimitResponse.code,
                        message = searchlimitResponse.message,
                    };

                var rows = connection.Query<RiskMgmtRecordReturn>(template.RawSql, template.Parameters);
                var totalItems = rows.Count() <= 0 ? 0 : rows.First().TotalItems;
                if (rows.Count() > 0)
                    rows.ToList().ForEach(x => Common.Security.ObscureWithdrawCard(x.cardno));

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
                            x.xferriskmgt = y.ContainsKey("xferriskmgt") ? (bool)y["xferriskmgt"] : false;

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
                                    x.cardno = "*****";
                                    //x.cardno = "*****";
                                    //x.amount = "*****";
                                }
                                else
                                {
                                    x.cardno = "*****";
                                    //x.cardno = request.Admin == x.checkperson.ToLower() ? x.cardno : "*****";
                                    //x.amount = request.Admin == x.checkperson.ToLower() ? x.amount : "*****";
                                }
                                break;
                            default:
                                x.cardno = "*****";
                                //x.amount = "*****";
                                break;
                        }

                    });



                var result = new ResponseModel<PagingModel<RiskMgmtRecordReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                    Data = new PagingModel<RiskMgmtRecordReturn>()
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

