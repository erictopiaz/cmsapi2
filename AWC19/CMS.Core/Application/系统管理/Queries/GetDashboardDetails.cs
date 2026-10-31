using AutoMapper;
using AWC2019.Core.Validation;
using CMS.Core.ViewModel;
using CMS.Data.Models;
using ShareEntity.OutputModels;
using Dapper;
using Dapper.Contrib.Extensions;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using UtilsLib.JWT;
using UtilsLib;
using System.Security.Cryptography.Xml;

namespace CMS.Core.Application.系统管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class GetDashboardDetails : PagingModel, IRequest<ResponseModel<List<ReportModel>>>
    {
        /// <summary>
        /// 市场管理人员账号（区分首页统计数据使用）
        /// </summary>
        [JsonIgnore]
        public string Admin { get; set; }

        public string loginKey { set; get; } = "";
        public string loginToken { set; get; } = "";
    }

    public class GetDashboardDetailsReturn
    {
        public int registercount { get; set; }
        public decimal depositamount { get; set; }
        public decimal withdrawamount { get; set; }
        public int initialdeposit { get; set; }
    }

    public class GetDashboardDetailsCurrencyReturn : GetDashboardDetailsReturn {
        public int registercountInr { get; set; }
        public decimal depositamountInr { get; set; }
        public decimal withdrawamountInr { get; set; }
        public int initialdepositInr { get; set; }
    }

    public class ReportModel {
        public string title { get; set; }
        public GetDashboardDetailsReturn report { set; get; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class GetDashboardDetailsValidator : AbstractValidator<GetDashboardDetails>
    {
        public GetDashboardDetailsValidator()
        {
            RuleFor(x => x.PageSize).PageSize();
        }
    }

    public class RightModel
    {
        public string account { set; get; }
        public string access { set; get; }
    }



    /// <summary>
    /// Request Handler
    /// </summary>
    public class GetDashboardDetailsHandler : BaseRequestHandler<GetDashboardDetails, ResponseModel<List<ReportModel>>>
    {
        public GetDashboardDetailsHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { 
        }


        protected override ResponseModel<List<ReportModel>> Handle(GetDashboardDetails request)
        {
            var currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(request.loginToken, request.loginKey);
           
            var currentData = DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd");
            var reportSql = string.Format(@"select * from (
select count(u.id) registercount from ofUser u with(nolock)
left join ofAffiliate a with(nolock) on u.affaccount = a.account
where u.currencyType = 156 and DateDiff(dd,u.createtime,DATEADD(hour,8,getutcdate()))=0 and a.verifyperson = '{0}'
) a,
(
select count(u.id) registercountInr from ofUser u with(nolock)
left join ofAffiliate a with(nolock) on u.affaccount = a.account
where u.currencyType = 356 and DateDiff(dd,u.createtime,DATEADD(hour,8,getutcdate()))=0 and a.verifyperson = '{0}'
) aa,
 (
 select iif(( sum(amount) IS not NULL and LEN(RTRIM(LTRIM(sum(amount)))) != 0), sum(amount),0) depositamount from ofDeposit a with(nolock)
 left join ofUser u with(nolock) on u.account = a.account
 left join ofAffiliate ag with(nolock) on u.affaccount = ag.account
 where requesttime >='2021-08-01' and DateDiff(dd,ISNULL(orderendtime,reviewtime),DATEADD(hour,8,getutcdate()))=0 and a.status=1 and u.currencyType=156 and ag.verifyperson='{0}'
 ) b,
 (
 select iif(( sum(amount) IS not NULL and LEN(RTRIM(LTRIM(sum(amount)))) != 0), sum(amount),0) depositamountInr from ofDeposit a with(nolock)
 left join ofUser u with(nolock) on u.account = a.account
 left join ofAffiliate ag with(nolock) on u.affaccount = ag.account
 where requesttime >='2021-08-01' and DateDiff(dd,ISNULL(orderendtime,reviewtime),DATEADD(hour,8,getutcdate()))=0 and a.status=1 and u.currencyType=356 and ag.verifyperson='{0}'
 ) bb,
 (
 select iif(( sum(amount) IS not NULL and LEN(RTRIM(LTRIM(sum(amount)))) != 0), sum(amount),0) withdrawamount from ofwithdrawal a with(nolock)
 left join ofUser u with(nolock) on u.account = a.account
 left join ofAffiliate ag with(nolock) on u.affaccount = ag.account
 where requesttime >='2021-08-01' and DateDiff(dd,a.paytime,DATEADD(hour,8,getutcdate()))=0  and a.status=2 and u.currencyType = 156 and ag.verifyperson='{0}'
 ) c,
 (
 select iif(( sum(amount) IS not NULL and LEN(RTRIM(LTRIM(sum(amount)))) != 0), sum(amount),0) withdrawamountInr from ofwithdrawal a with(nolock)
 left join ofUser u with(nolock) on u.account = a.account
 left join ofAffiliate ag with(nolock) on u.affaccount = ag.account
 where requesttime >='2021-08-01' and DateDiff(dd,a.paytime,DATEADD(hour,8,getutcdate()))=0  and a.status=2 and u.currencyType = 356 and ag.verifyperson='{0}'
 ) cc,
 (
	select count(*) initialdeposit from userFirstDepositDetail uf with(nolock) 
	left join ofUser u with(nolock) on u.account = uf.account
	left join ofAffiliate ag with(nolock) on uf.affaccount = ag.account
	where depositDay>={1} and depositDay<={1} and u.currencyType = 156 and ag.verifyperson='{0}'
 ) d,
 (
	select count(*) initialdepositInr from userFirstDepositDetail uf with(nolock) 
	left join ofUser u with(nolock) on u.account = uf.account
	left join ofAffiliate ag with(nolock) on uf.affaccount = ag.account
	where depositDay>={1} and depositDay<={1} and u.currencyType = 356 and ag.verifyperson='{0}'
 ) dd ", request.Admin, currentData);


            var accessSql = string.Format(@"select * from ofManage where account=@account and status = 1");
            var result = DapperBase.Query<RightModel>(accessSql, new { account = request.Admin }, commandType: CommandType.Text,
                        commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);
            var depositamount = 0.0m;
            var withdrawamount = 0.0m;
            var initialdeposit = 0;
            var registercount = 0;

            if (result != null && result.Count == 1 
                && result.First().access.IndexOf("/api/Admin/QueryCMSIndexAgentReport", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                //只能查看自己审核代理的统计数据
                var reportModels = new List<ReportModel>();
                //查询统计
                var sql = string.Format(@"select id from ofAffiliate where verifyperson='{0}'", request.Admin);
                if (DapperBase.ExecuteScalar(sql, param: null, commandTimeout: 15000,
                    ConnectionString: DapperStringBase.ConnectionString))//有审核过代理
                {

                    var reportResult = DapperBase.Query<GetDashboardDetailsCurrencyReturn>(reportSql, param: null, commandTimeout: 15000,
                        ConnectionString: DapperStringBase.ConnectionString);
                   
                    if (reportResult != null && reportResult.Count == 1)
                    {
                        if (currencys.Contains(156) && currencys.Count == 1) {

                            reportModels.Add(new ReportModel() {
                                title = "￥",
                                report = new GetDashboardDetailsReturn() {
                                    depositamount = reportResult.First().depositamount,
                                    withdrawamount = reportResult.First().withdrawamount,
                                    initialdeposit = reportResult.First().initialdeposit,
                                    registercount = reportResult.First().registercount,
                                }
                            });
                        }
                        else if (currencys.Contains(356) && currencys.Count == 1) {

                            reportModels.Add(new ReportModel() {
                                title = "₹",
                                report = new GetDashboardDetailsReturn() {
                                    depositamount = reportResult.First().depositamountInr,
                                    withdrawamount = reportResult.First().withdrawamountInr,
                                    initialdeposit = reportResult.First().initialdepositInr,
                                    registercount = reportResult.First().registercountInr,
                                }
                            });

                        }
                        else {
                            reportModels.Add(new ReportModel() {
                                title = "￥",
                                report = new GetDashboardDetailsReturn() {
                                    depositamount = reportResult.First().depositamount,
                                    withdrawamount = reportResult.First().withdrawamount,
                                    initialdeposit = reportResult.First().initialdeposit,
                                    registercount = reportResult.First().registercount,
                                }
                            });
                            reportModels.Add(new ReportModel() {
                                title = "₹",
                                report = new GetDashboardDetailsReturn() {
                                    depositamount = reportResult.First().depositamountInr,
                                    withdrawamount = reportResult.First().withdrawamountInr,
                                    initialdeposit = reportResult.First().initialdepositInr,
                                    registercount = reportResult.First().registercountInr,
                                }
                            });

                        }
                    }
                }

                return new ResponseModel<List<ReportModel>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = reportModels
                };
            }
            else
            {
                var allReportSql = string.Format(@"select * from (
select count(u.id) registercount from ofUser u with(nolock)
where u.currencyType=156 and DateDiff(dd,u.createtime,DATEADD(hour,8,getutcdate()))=0  
) a,
(
select count(u.id) registercountInr from ofUser u with(nolock)
where u.currencyType=356 and DateDiff(dd,u.createtime,DATEADD(hour,8,getutcdate()))=0  
) aa,
 (
 select iif(( sum(amount) IS not NULL and LEN(RTRIM(LTRIM(sum(amount)))) != 0), sum(amount),0) depositamount from ofDeposit a with(nolock)
 left join ofUser u with(nolock) on a.account=u.account
 where requesttime >='2024-03-01' and DateDiff(dd,ISNULL(orderendtime,reviewtime),DATEADD(hour,8,getutcdate()))=0 and a.status=1 and u.currencyType=156
 ) b,
 (
 select iif(( sum(amount) IS not NULL and LEN(RTRIM(LTRIM(sum(amount)))) != 0), sum(amount),0) depositamountInr from ofDeposit a with(nolock)
 left join ofUser u with(nolock) on a.account=u.account
 where requesttime >='2024-03-01' and DateDiff(dd,ISNULL(orderendtime,reviewtime),DATEADD(hour,8,getutcdate()))=0 and a.status=1 and u.currencyType=356
 ) bb,
 (
 select iif(( sum(amount) IS not NULL and LEN(RTRIM(LTRIM(sum(amount)))) != 0), sum(amount),0) withdrawamount from ofwithdrawal a with(nolock)
 left join ofUser u with(nolock) on a.account=u.account
 where requesttime >='2024-03-01' and DateDiff(dd,a.paytime,DATEADD(hour,8,getutcdate()))=0  and a.status=2 and u.currencyType=156
 ) c,
 (
 select iif(( sum(amount) IS not NULL and LEN(RTRIM(LTRIM(sum(amount)))) != 0), sum(amount),0) withdrawamountInr from ofwithdrawal a with(nolock)
 left join ofUser u with(nolock) on a.account=u.account
 where requesttime >='2024-03-01' and DateDiff(dd,a.paytime,DATEADD(hour,8,getutcdate()))=0  and a.status=2 and u.currencyType=356
 ) cc,
 (
  select count(uf.id) initialdeposit from userFirstDepositDetail uf with(nolock) 
  left join ofUser u with(nolock) on uf.account=u.account
  where depositDay>={0} and depositDay<={0} and u.currencyType=156
 ) d,
 (
  select count(uf.id) initialdepositInr from userFirstDepositDetail uf with(nolock) 
  left join ofUser u with(nolock) on uf.account=u.account
  where depositDay>={0} and depositDay<={0} and u.currencyType=356
 ) dd ", currentData);

                var reportResult = DapperBase.Query<GetDashboardDetailsCurrencyReturn>(allReportSql, param: null, commandTimeout: 15000,
                       ConnectionString: DapperStringBase.ConnectionString);

                if (reportResult != null && reportResult.Count == 1)
                {
                    depositamount = reportResult.First().depositamount;
                    withdrawamount = reportResult.First().withdrawamount;
                    initialdeposit = reportResult.First().initialdeposit;
                    registercount = reportResult.First().registercount;
                }

                var reportModels= new List<ReportModel>();

                if(currencys.Contains(156) && currencys.Count == 1) {

                    reportModels.Add(new ReportModel() {
                        title = "￥",
                        report = new GetDashboardDetailsReturn() {
                            depositamount = reportResult.First().depositamount,
                            withdrawamount = reportResult.First().withdrawamount,
                            initialdeposit = reportResult.First().initialdeposit,
                            registercount = reportResult.First().registercount,
                        }
                    });
                }
                else if (currencys.Contains(356) && currencys.Count == 1) {

                    reportModels.Add(new ReportModel() {
                        title = "₹",
                        report = new GetDashboardDetailsReturn() {
                            depositamount = reportResult.First().depositamountInr,
                            withdrawamount = reportResult.First().withdrawamountInr,
                            initialdeposit = reportResult.First().initialdepositInr,
                            registercount = reportResult.First().registercountInr,
                        }
                    });

                }
                else {
                    reportModels.Add(new ReportModel() {
                        title = "￥",
                        report = new GetDashboardDetailsReturn() {
                            depositamount = reportResult.First().depositamount,
                            withdrawamount = reportResult.First().withdrawamount,
                            initialdeposit = reportResult.First().initialdeposit,
                            registercount = reportResult.First().registercount,
                        }
                    });
                    reportModels.Add(new ReportModel() {
                        title = "₹",
                        report = new GetDashboardDetailsReturn() {
                            depositamount = reportResult.First().depositamountInr,
                            withdrawamount = reportResult.First().withdrawamountInr,
                            initialdeposit = reportResult.First().initialdepositInr,
                            registercount = reportResult.First().registercountInr,
                        }
                    });

                }

                return new ResponseModel<List<ReportModel>>() {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = reportModels
                };
               
                #region 老统计代码

                //using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
                //{
                //    var deposittodayfirst_id = connection.QueryFirstOrDefault<long>("SELECT Top 1 id FROM ofDeposit With(NOLOCK) WHERE requesttime>='" + Common.CurrentTime.ToString("yyyy-MM-dd") + "' ORDER BY ID ASC");
                //    var registercount = connection.QueryFirstOrDefault<int?>("SELECT COUNT(*) AS registertoday FROM ofUser With(NOLOCK) WHERE createtime>='" + Common.CurrentTime.ToString("yyyy-MM-dd") + "'");
                //    var depositamount = connection.QueryFirstOrDefault<decimal?>("SELECT SUM(amount) AS deposittoday FROM ofDeposit With(NOLOCK) WHERE id>=" + deposittodayfirst_id.ToString() + " AND orderendtime>='" + Common.CurrentTime.ToString("yyyy-MM-dd") + "' OR reviewtime>='" + Common.CurrentTime.ToString("yyyy-MM-dd") + "' AND status = 1");
                //    var withdrawamount = connection.QueryFirstOrDefault<decimal?>("SELECT SUM(amount) AS withdrawtoday FROM ofWithdrawal With(NOLOCK) WHERE paytime>='" + Common.CurrentTime.ToString("yyyy-MM-dd") + "' AND status = 2");

                //    var initialdeposit = connection.QueryFirst<int?>(
                //        "SELECT COUNT(*) " +
                //        "FROM (SELECT t.account, t.requesttime " +
                //        "FROM ( " +
                //        "SELECT DISTINCT account " +
                //        "FROM ofdeposit WITH(NOLOCK) " +
                //        "WHERE requesttime >='" + Common.CurrentTime.ToString("yyyy-MM-dd") + "' AND status =1) mo " +
                //        "CROSS APPLY ( " +
                //        "SELECT TOP 1 account,requesttime " +
                //        "FROM ofdeposit mi WITH(NOLOCK) " +
                //        "WHERE mi.account = mo.account " +
                //        "AND mi.status=1) t " +
                //        "WHERE t.requesttime >='" + Common.CurrentTime.ToString("yyyy-MM-dd") + "')y " +
                //        "INNER JOIN ofuser x WITH(NOLOCK) ON x.account= y.account ");


                //    return new ResponseModel<GetDashboardDetailsReturn>()
                //    {
                //        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                //        message = "",
                //        Data = new GetDashboardDetailsReturn()
                //        {
                //            depositamount = depositamount ?? 0,
                //            withdrawamount = withdrawamount ?? 0,
                //            initialdeposit = initialdeposit ?? 0,
                //            registercount = registercount ?? 0
                //        }
                //    };
                //}
                #endregion

            }

        }





    }

}
