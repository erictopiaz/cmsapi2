using AutoMapper;
using CMS.Core.Application.Account.Queries;
using CMS.Core.ViewModel;
using CMS.Data.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using ShareEntity.Models;
using ShareEntity.OutputModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CMS.Core.Application.账号管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 账户存款列表 : PagingModel, IRequest<ResponseModel<PagingModel<账户存款列表Return>>>
    {
        public string account { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public string Admin { get; set; }
        public int currencycode { get; set; } = 156;

        //public DateTime? endtime { get; set; }
    }

    public class 账户存款列表Return
    {
        public long id { get; set; }
        public int? accounttype { get; set; }
        public string account { get; set; }
        //public string nickname { get; set; }
        //public string realname { get; set; }
        //public int sex { get; set; }
        //public string birthday { get; set; }
        public int status { get; set; }
        public int level { get; set; }
        public DateTime createtime { get; set; }
        public string referrerreg { get; set; }
        public string telephone { get; set; }
        public decimal amountactive { get; set; }
        public decimal amountsuspend { get; set; }
        public string ipaddress { get; set; }
        //public string macaddress { get; set; }
        public int depositnumber { get; set; }
        public decimal depositamount { get; set; }
        public int withdrawalnumber { get; set; }
        public decimal withdrawalamount { get; set; }
        public int loginnumber { get; set; }
        public string affaccount { get; set; }
        //public string freezereason { get; set; }
        public decimal bonuspoints { get; set; }
        public decimal winloss { get; set; }
        public string marcode { get; set; }
        public DateTime? lastdeposittime{ get; set; }
        public DateTime? transtatus { get; set; }
        public string withdrawcards { get; set; }
        //public string misc { get; set; }
        public int riskTypesAready { set; get; }
        internal long TotalItems { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 账户存款列表Validator : AbstractValidator<账户存款列表>
    {
        public 账户存款列表Validator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 账户存款列表Handler : BaseRequestHandler<账户存款列表, ResponseModel<PagingModel<账户存款列表Return>>>
    {
        public 账户存款列表Handler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<PagingModel<账户存款列表Return>> Handle(账户存款列表 request)
        {
            // Get Account Created based on the start Date

            // Get All Successful Deposit on that date onwards

            //TODO: Do This after everything else

            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            SqlBuilder.Template template;
            template = builder.AddTemplate("" +
                "SELECT * FROM (SELECT accounttype, telephone,account, status, level, createtime, referrerreg, amountactive, amountsuspend, ipaddress, depositnumber, depositamount, " +
                "withdrawalnumber, withdrawalamount, loginnumber, affaccount, bonuspoints, marcode, transtatus, withdrawcards , (depositamount-withdrawalamount) AS winloss," +
                "(SELECT COUNT(*) FROM ofUser With(NOLOCK) /**where**/) as TotalItems " +
                "FROM ofUser WITH(NOLOCK) /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY)mo " +
                "CROSS APPLY (SELECT TOP 1 requesttime as lastdeposittime " +
                "FROM ofDeposit mi WITH(NOLOCK) WHERE mi.account = mo.account AND status = 1 ORDER BY id DESC)a " +
                "left join ofUserExtention ue WITH(NOLOCK) on mo.account=ue.account");

            // FILL UP THE WHERE CLAUSE
            //builder.Where("depositnumber > 0");

            if (!string.IsNullOrEmpty(request.account)) builder.Where("ofUser.account LIKE @account", new { account = string.Concat("%", request.account, "%") });

            //if (request.startdate == null || ((DateTime)request.startdate).Year < 2008)
            //{
            //    request.startdate = Common.CurrentTime.AddDays(-90);
            //    request.enddate = Common.CurrentTime;
            //}
            //if (request.enddate == null)
            //    request.enddate = Common.CurrentTime;

            //builder.Where("createtime BETWEEN @STARTDATE AND @ENDDATE", new { STARTDATE=request.startdate, ENDDATE=request.enddate});
            builder.Where("depositnumber > 0 and ofUser.currencyType=@currencycode", new {
                currencycode=request.currencycode
            });


            // PAGING FUNCTION
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("winloss DESC");

            #region ching add 确认查询到的会员列表
            var accounts = new List<string>();
            #endregion
            var rows = new List<账户存款列表Return>();
            var totalItems = 0L;
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                rows = connection.Query<账户存款列表Return>(template.RawSql, template.Parameters).ToList();
                totalItems = rows.Count() == 0 ? 0 : rows.First().TotalItems;

                var adminAccess = connection.Get<ofManage>(request.Admin).access;

                bool showTelephone = false;
                bool showEmail = false;
                if (adminAccess.Contains("/api/Account/AllowToSeePhone")) showTelephone = true;
                if (adminAccess.Contains("/api/Account/AllowToSeeEmail")) showEmail = true;

                // Apply the LevelName 
                if (rows.Count() > 0)
                    rows.ToList().ForEach(
                        x =>
                        {
                            if (!showTelephone)
                            {
                                x.telephone = x.depositamount == 0 ? x.telephone : "****";
                                if (UtilsLib.Common.IsIncludedTel(x.account))
                                {
                                    x.account = Common.Security.ObscureString(x.account);
                                }
                            }
                            //x.levelname = CMS.Core.CachedData.LevelNameDict[x.level];

                            accounts.Add(x.account);//添加到会员列表
                        });



                //return new ResponseModel<PagingModel<账户存款列表Return>>()
                //{
                //    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                //    message = "",
                //    Data = new PagingModel<账户存款列表Return>()
                //    {
                //        Data = rows,
                //        TotalItems = totalItems
                //    }
                //};
            }


            #region 查询统计 会员总存和总提 20251021
            if (accounts.Count() > 0)
            {
                var queryWDReportModel = AccountBase.QueryUserWDReport(accounts);
                if (queryWDReportModel != null && queryWDReportModel.Count() > 0)
                {
                    foreach (var wd in queryWDReportModel)
                    {
                        foreach (var r in rows)
                        {
                            if (wd.account.Equals(r.account, StringComparison.OrdinalIgnoreCase))
                            {
                                r.depositamount = wd.depositamount;
                                r.depositnumber = wd.depositnumber;
                                r.withdrawalnumber = wd.withdrawalnumber;
                                r.withdrawalamount = wd.withdrawalamount;
                                r.winloss = r.withdrawalamount - r.depositamount; //ching add 20251217
                               
                            }
                        }
                    }
                }
            }

            #endregion
            return new ResponseModel<PagingModel<账户存款列表Return>>()
            {
                code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                message = "",
                Data = new PagingModel<账户存款列表Return>()
                {
                    Data = rows,
                    TotalItems = totalItems
                }
            };

        }
    }
}
