using AutoMapper;
using CMS.Core.ViewModel;
using ShareEntity.Models;
using ShareEntity.OutputModels;
using ShareEntity.webSvcSqlCommandText;
using Dapper;
using Dapper.Contrib.Extensions;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using UtilsLib;
using static Dapper.SqlBuilder;
using CMS.Core.Application.Account.Queries;
using System.ComponentModel.DataAnnotations;

namespace CMS.Core.Application.账号管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 首存账号 : PagingModel, IRequest<ResponseModel<PagingModel<首存账号Return>>>
    {
        public List<int> currencys { get; set; }
        public int currencycode { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }

        public string uid { get; set; }
        [MaxLength(30)]
        public string affaccount { set; get; } = "";
    }

    public class 首存账号Return
    {
        public long id { get; set; }
       
        public string account { get; set; }
        public string affaccount { get; set; } = "";
        public string telephone { get; set; }
        public string realname { get; set; }
        public int status { get; set; }
        public string misc { get; set; }
        public string anomally{ get; set; }
        public int level { get; set; }
        public DateTime requesttime { get; set; }
        public int depositnumber { get; set; }
        public decimal depositamount { get; set; }
        public int withdrawalnumber { get; set; }
        public int device { get; set; }
        public decimal withdrawalamount { get; set; }
        public decimal winloss { get; set; }
        public DateTime? transtatus { get; set; }
        public string levelname { get; set; }
        internal long TotalItems { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 首存账号Validator : AbstractValidator<首存账号>
    {
        public 首存账号Validator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 首存账号Handler : BaseRequestHandler<首存账号, ResponseModel<PagingModel<首存账号Return>>>
    {
        public 首存账号Handler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<PagingModel<首存账号Return>> Handle(首存账号 request)
        {
            if (!request.currencys.Contains(request.currencycode))
            {
                return new ResponseModel<PagingModel<首存账号Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择货币查询"
                };
            }


            // 创建DAPPER TEMPLATE 
            if (request.startDate == null || ((DateTime)request.startDate).Year < 2008)
                request.startDate = Common.CurrentTime;
            if (request.endDate == null)
                request.endDate = ((DateTime)request.startDate).AddDays(1);


            var affaccount_sql = string.IsNullOrEmpty(request.affaccount)
                ? "" : string.Format(@" and x.affaccount='{0}'", request.affaccount);

            #region 记录总行数统计
            var rowCountSql = string.Format(@"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.requesttime desc)AS Row, * from (
	SELECT  x.telephone,x.realname,x.status,x.misc,x.withdrawalamount,x.withdrawalnumber,x.withdrawalamount-x.depositamount AS winloss,
            x.level, y.account AS account, y.requesttime AS requesttime,x.depositnumber as depositnumber, x.depositamount as depositamount, y.lastdeposit as transtatus 
            FROM (SELECT t.account, t.requesttime, j.lastdeposit
            FROM (
            SELECT DISTINCT account 
            FROM ofdeposit WITH(NOLOCK) 
            WHERE requesttime BETWEEN @STARTDATE AND @ENDDATE AND status =1 AND currencycode = @CURRENCYCODE) mo
            CROSS APPLY (
                SELECT TOP 1 account,requesttime 
                FROM ofdeposit mi WITH(NOLOCK) 
                WHERE mi.account = mo.account 
                AND mi.status=1) t 
            CROSS APPLY (
                SELECT TOP 1 account,requesttime as lastdeposit
                FROM ofdeposit mj WITH(NOLOCK) 
                WHERE mj.account = mo.account 
                AND mj.status=1 ORDER BY ID DESC) j 
            WHERE t.requesttime BETWEEN @STARTDATE AND @ENDDATE)y 
            INNER JOIN ofuser x WITH(NOLOCK) ON x.account= y.account
            where 1=1 {0}
 ) T
) 
TT
OPTION (RECOMPILE)", affaccount_sql);
            

            var param = new DynamicParameters();
            param.Add("@STARTDATE", ((DateTime)request.startDate).ToString("yyyy-MM-dd"));
            param.Add("@ENDDATE", ((DateTime)request.endDate).ToString("yyyy-MM-dd HH:mm:ss"));
            param.Add("@CURRENCYCODE", (int)request.currencycode);
            
            #endregion

            #region 查询指定记录
            var rowSql = string.Format(@"SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.requesttime desc)AS Row, * from 
(
	SELECT  x.id,x.telephone,x.realname,x.status,x.misc,x.withdrawalamount,x.affaccount,x.withdrawalnumber,x.withdrawalamount-x.depositamount AS winloss,
            x.level, y.account AS account, y.requesttime AS requesttime,x.depositnumber as depositnumber, x.depositamount as depositamount, y.lastdeposit as transtatus , y.device
            FROM (SELECT t.account, t.requesttime , j.lastdeposit, t.device
            FROM (
            SELECT DISTINCT account 
            FROM ofdeposit WITH(NOLOCK) 
            WHERE requesttime BETWEEN @STARTDATE AND @ENDDATE AND status =1 AND currencycode=@CURRENCYCODE) mo
            CROSS APPLY (
           SELECT TOP 1 account,requesttime, device 
            FROM ofdeposit mi WITH(NOLOCK) 
            WHERE mi.account = mo.account 
            AND mi.status=1) t 
            CROSS APPLY (
                SELECT TOP 1 account,requesttime as lastdeposit
                FROM ofdeposit mj WITH(NOLOCK) 
                WHERE mj.account = mo.account 
                AND mj.status=1 ORDER BY ID DESC) j 
            WHERE t.requesttime BETWEEN @STARTDATE AND @ENDDATE)y 
            INNER JOIN ofuser x WITH(NOLOCK) ON x.account= y.account
            where 1=1 {0}
 ) T
) 
TT
 WHERE TT.Row between @startIndex and @endIndex 
OPTION (RECOMPILE) ", affaccount_sql);

            #endregion

            #region 查询统计 会员总存和总提 20251021
            //var accounts = new List<string>();
            //if (accounts.Count() > 0)
            //{
            //    var queryWDReportModel = AccountBase.QueryUserWDReport(accounts);
            //    if (queryWDReportModel != null && queryWDReportModel.Count() > 0)
            //    {
            //        foreach (var wd in queryWDReportModel)
            //        {
            //            foreach (var r in rows)
            //            {
            //                if (wd.account.Equals(r.account, StringComparison.OrdinalIgnoreCase))
            //                {
            //                    r.depositamount = wd.depositamount;
            //                    r.depositnumber = wd.depositnumber;
            //                    r.withdrawalnumber = wd.withdrawalnumber;
            //                    r.withdrawalamount = wd.withdrawalamount;
            //                    r.winloss = r.withdrawalamount - r.depositamount; //ching add 20251217

            //                }
            //            }
            //        }
            //    }
            //}

            #endregion



            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                bool showTelephone = false;
                var adminAccess = connection.Get<ofManage>(request.Admin).access;


                if (adminAccess.Contains("/api/Account/AllowToSeePhone"))
                    showTelephone = true;
                var count = connection.Query<RecordsCount>(rowCountSql, param);

                if (count != null && count.First().cnt > 0)//有记录，查询出来
                {
                    var startIndex = (request.Page-1) * request.PageSize + 1;
                    var endIndex = request.Page * request.PageSize;
                    var totalPages = count.First().cnt % request.PageSize > 0 ? count.First().cnt / request.PageSize + 1 : count.First().cnt / request.PageSize;

                    param.Add("@startIndex", startIndex, dbType: DbType.Int32);
                    param.Add("@endIndex", endIndex, dbType: DbType.Int32);

                    var rows = connection.Query<首存账号Return>(rowSql, param);

                    // Apply the Winloss 
                    if (rows.Count() > 0) {
                        rows.ToList().ForEach(
                            x => {

                                Dictionary<string, string> tempMisc = new Dictionary<string, string>();
                                if (x.misc != null)
                                    tempMisc = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(x.misc);

                                if (tempMisc != null) {
                                    x.anomally = "";
                                    if ((tempMisc.ContainsKey("gamedisabledstat") && !string.IsNullOrEmpty(tempMisc["gamedisabledstat"])) ||
                                        (tempMisc.ContainsKey("promodisabled") && tempMisc["promodisabled"] != "[]")) {
                                        x.anomally = "风控";
                                    }
                                }

                                x.levelname = CMS.Core.CachedData.LevelNameDict[x.level];
                                if (!showTelephone) x.telephone = "****";
                                if (!showTelephone)
                                {
                                    if(UtilsLib.Common.IsIncludedTel(x.account))
                                        x.account = Common.Security.ObscureString(x.account);
                                }
                                //x.winloss = x.depositamount - x.withdrawalamount;
                            });
                    }

                    //var totalItems = rows.Count();// == 0 ? 0 : rows.First().TotalItems;
                    return new ResponseModel<PagingModel<首存账号Return>>() {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "",
                        Data = new PagingModel<首存账号Return>() {
                            Data = rows,
                            TotalItems = count.First().cnt
                        }
                    };
                }
                else {

                    return new ResponseModel<PagingModel<首存账号Return>>() {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "",
                        Data = new PagingModel<首存账号Return>() {
                            Data = new List<首存账号Return>(),
                            TotalItems = 0
                        }
                    };
                }

            }
        }


        #region 备份
//        protected override ResponseModel<PagingModel<首存账号Return>> Handle_old(首存账号 request)
//        {
//            if (!request.currencys.Contains(request.currencycode))
//            {
//                return new ResponseModel<PagingModel<首存账号Return>>()
//                {
//                    code = ShareEntity.Global.ResponseCode.code.FAIL,
//                    message = "请选择货币查询"
//                };
//            }


//            // 创建DAPPER TEMPLATE 
//            if (request.startDate == null || ((DateTime)request.startDate).Year < 2008)
//                request.startDate = Common.CurrentTime;
//            if (request.endDate == null)
//                request.endDate = ((DateTime)request.startDate).AddDays(1);

//            #region 记录总行数统计
//            var rowCountSql = string.Format(@"SELECT count(*) cnt FROM 
//( 
//SELECT ROW_NUMBER() OVER 
//(order by T.requesttime desc)AS Row, * from (
//	SELECT  x.telephone,x.realname,x.status,x.misc,x.withdrawalamount,x.withdrawalnumber,x.withdrawalamount-x.depositamount AS winloss,
//            x.level, y.account AS account, y.requesttime AS requesttime,x.depositnumber as depositnumber, x.depositamount as depositamount, y.lastdeposit as transtatus 
//            FROM (SELECT t.account, t.requesttime, j.lastdeposit
//            FROM (
//            SELECT DISTINCT account 
//            FROM ofdeposit WITH(NOLOCK) 
//            WHERE requesttime BETWEEN @STARTDATE AND @ENDDATE AND status =1 AND currencycode = @CURRENCYCODE) mo
//            CROSS APPLY (
//                SELECT TOP 1 account,requesttime 
//                FROM ofdeposit mi WITH(NOLOCK) 
//                WHERE mi.account = mo.account 
//                AND mi.status=1) t 
//            CROSS APPLY (
//                SELECT TOP 1 account,requesttime as lastdeposit
//                FROM ofdeposit mj WITH(NOLOCK) 
//                WHERE mj.account = mo.account 
//                AND mj.status=1 ORDER BY ID DESC) j 
//            WHERE t.requesttime BETWEEN @STARTDATE AND @ENDDATE)y 
//            INNER JOIN ofuser x WITH(NOLOCK) ON x.account= y.account
// ) T
//) 
//TT
//OPTION (RECOMPILE)");

//            var param = new DynamicParameters();
//            param.Add("@STARTDATE", ((DateTime)request.startDate).ToString("yyyy-MM-dd"));
//            param.Add("@ENDDATE", ((DateTime)request.endDate).ToString("yyyy-MM-dd HH:mm:ss"));
//            param.Add("@CURRENCYCODE", (int)request.currencycode);

//            #endregion

//            #region 查询指定记录
//            var rowSql = string.Format(@"SELECT TT.* FROM 
//( 
//SELECT ROW_NUMBER() OVER 
//(order by T.requesttime desc)AS Row, * from 
//(
//	SELECT  x.id,x.telephone,x.realname,x.status,x.misc,x.withdrawalamount,x.affaccount,x.withdrawalnumber,x.withdrawalamount-x.depositamount AS winloss,
//            x.level, y.account AS account, y.requesttime AS requesttime,x.depositnumber as depositnumber, x.depositamount as depositamount, y.lastdeposit as transtatus , y.device
//            FROM (SELECT t.account, t.requesttime , j.lastdeposit, t.device
//            FROM (
//            SELECT DISTINCT account 
//            FROM ofdeposit WITH(NOLOCK) 
//            WHERE requesttime BETWEEN @STARTDATE AND @ENDDATE AND status =1 AND currencycode=@CURRENCYCODE) mo
//            CROSS APPLY (
//           SELECT TOP 1 account,requesttime, device 
//            FROM ofdeposit mi WITH(NOLOCK) 
//            WHERE mi.account = mo.account 
//            AND mi.status=1) t 
//            CROSS APPLY (
//                SELECT TOP 1 account,requesttime as lastdeposit
//                FROM ofdeposit mj WITH(NOLOCK) 
//                WHERE mj.account = mo.account 
//                AND mj.status=1 ORDER BY ID DESC) j 
//            WHERE t.requesttime BETWEEN @STARTDATE AND @ENDDATE)y 
//            INNER JOIN ofuser x WITH(NOLOCK) ON x.account= y.account
// ) T
//) 
//TT
// WHERE TT.Row between @startIndex and @endIndex 
//OPTION (RECOMPILE) ");

//            #endregion

//            #region 查询统计 会员总存和总提 20251021
//            //var accounts = new List<string>();
//            //if (accounts.Count() > 0)
//            //{
//            //    var queryWDReportModel = AccountBase.QueryUserWDReport(accounts);
//            //    if (queryWDReportModel != null && queryWDReportModel.Count() > 0)
//            //    {
//            //        foreach (var wd in queryWDReportModel)
//            //        {
//            //            foreach (var r in rows)
//            //            {
//            //                if (wd.account.Equals(r.account, StringComparison.OrdinalIgnoreCase))
//            //                {
//            //                    r.depositamount = wd.depositamount;
//            //                    r.depositnumber = wd.depositnumber;
//            //                    r.withdrawalnumber = wd.withdrawalnumber;
//            //                    r.withdrawalamount = wd.withdrawalamount;
//            //                    r.winloss = r.withdrawalamount - r.depositamount; //ching add 20251217

//            //                }
//            //            }
//            //        }
//            //    }
//            //}

//            #endregion



//            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
//            {
//                bool showTelephone = false;
//                var adminAccess = connection.Get<ofManage>(request.Admin).access;


//                if (adminAccess.Contains("/api/Account/AllowToSeePhone")) showTelephone = true;
//                var count = connection.Query<RecordsCount>(rowCountSql, param);

//                if (count != null && count.First().cnt > 0)//有记录，查询出来
//                {
//                    var startIndex = (request.Page - 1) * request.PageSize + 1;
//                    var endIndex = request.Page * request.PageSize;
//                    var totalPages = count.First().cnt % request.PageSize > 0 ? count.First().cnt / request.PageSize + 1 : count.First().cnt / request.PageSize;

//                    param.Add("@startIndex", startIndex, dbType: DbType.Int32);
//                    param.Add("@endIndex", endIndex, dbType: DbType.Int32);

//                    var rows = connection.Query<首存账号Return>(rowSql, param);

//                    // Apply the Winloss 
//                    if (rows.Count() > 0)
//                    {
//                        rows.ToList().ForEach(
//                            x => {

//                                Dictionary<string, string> tempMisc = new Dictionary<string, string>();
//                                if (x.misc != null)
//                                    tempMisc = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(x.misc);

//                                if (tempMisc != null)
//                                {
//                                    x.anomally = "";
//                                    if ((tempMisc.ContainsKey("gamedisabledstat") && !string.IsNullOrEmpty(tempMisc["gamedisabledstat"])) ||
//                                        (tempMisc.ContainsKey("promodisabled") && tempMisc["promodisabled"] != "[]"))
//                                    {
//                                        x.anomally = "风控";
//                                    }
//                                }

//                                x.levelname = CMS.Core.CachedData.LevelNameDict[x.level];
//                                if (!showTelephone) x.telephone = "****";
//                                if (!showTelephone)
//                                {
//                                    if (UtilsLib.Common.IsIncludedTel(x.account))
//                                        x.account = Common.Security.ObscureString(x.account);
//                                }
//                                //x.winloss = x.depositamount - x.withdrawalamount;
//                            });
//                    }

//                    //var totalItems = rows.Count();// == 0 ? 0 : rows.First().TotalItems;
//                    return new ResponseModel<PagingModel<首存账号Return>>()
//                    {
//                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
//                        message = "",
//                        Data = new PagingModel<首存账号Return>()
//                        {
//                            Data = rows,
//                            TotalItems = count.First().cnt
//                        }
//                    };
//                }
//                else
//                {

//                    return new ResponseModel<PagingModel<首存账号Return>>()
//                    {
//                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
//                        message = "",
//                        Data = new PagingModel<首存账号Return>()
//                        {
//                            Data = new List<首存账号Return>(),
//                            TotalItems = 0
//                        }
//                    };
//                }

//            }
//        }

        #endregion

    }


    /// <summary>
    /// 当前记录，有多少行
    /// </summary>
    public class RecordsCount {
        public int cnt { set; get; }
    }
}
