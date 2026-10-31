using AutoMapper;
using AWC2019.Core.Validation;
using CMS.Core.Application.Account.Queries;
using CMS.Core.ViewModel;
using CMS.Data.Models;
using ShareEntity.OutputModels;
using Dapper;
using Dapper.Contrib.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ShareEntity.Models;

namespace CMS.Core.Application.账号管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 存款账号 : PagingModel, IRequest<ResponseModel<PagingModel<存款账号Return>>>
    {
        public string account { get; set; }
        public string uid { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public int currencycode { get; set; } = 156;
        public string Admin {  get; set; }
    }

    public class 存款账号Return
    {
        public long id { get; set; }
        public int? accounttype { get; set; }
        public string account { get; set; }
        
        public int status { get; set; }
        public int level { get; set; }
        public DateTime createtime { get; set; }
        public string referrerreg { get; set; }
        public decimal amountactive { get; set; }
        public decimal amountsuspend { get; set; }
        public string ipaddress { get; set; }
        
        public int depositnumber { get; set; }
        public decimal depositamount { get; set; }
        public int withdrawalnumber { get; set; }
        public decimal withdrawalamount { get; set; }
        public int loginnumber { get; set; }
        public string affaccount { get; set; }
       
        public decimal bonuspoints { get; set; }
        public string marcode { get; set; }
        public DateTime? transtatus { get; set; }
        public string withdrawcards { get; set; }
        
        internal long TotalItems { get; set; }

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 存款账号Validator : AbstractValidator<存款账号>
    {
        public 存款账号Validator()
        {
            RuleFor(x => x.account).Account();
            RuleFor(x => x.uid).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 存款账号Handler : BaseRequestHandler<存款账号, ResponseModel<PagingModel<存款账号Return>>>
    {
        public 存款账号Handler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<PagingModel<存款账号Return>> Handle(存款账号 request)
        {
            // Get Account Created based on the start Date

            // Get All Successful Deposit on that date onwards

            //TODO: Do This after everything else

            if (!string.IsNullOrEmpty(request.uid)) {
                var account = AccountBase.QueryAccountById(Convert.ToInt64(request.uid));
                request.account = string.IsNullOrEmpty(account) ? request.account : account;
            }

            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT * , COUNT(*) OVER() as TotalItems " +
                "FROM ofUser WITH(NOLOCK) /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY");

            // FILL UP THE WHERE CLAUSE
            builder.Where("depositnumber > 0 and currencyType = @currencycode", new { currencycode = request.currencycode});

            if (!string.IsNullOrEmpty(request.account)) builder.Where("account LIKE @account", new { account = string.Concat("%", request.account, "%") });

            if (request.startdate == null || ((DateTime)request.startdate).Year < 2008)
            {
                request.startdate = Common.CurrentTime.AddDays(-90);
                request.enddate = Common.CurrentTime;
            }
            if (request.enddate == null)
                request.enddate = Common.CurrentTime;

            builder.Where("createtime BETWEEN @STARTDATE AND @ENDDATE", new { STARTDATE=request.startdate, ENDDATE=request.enddate});
            builder.Where("depositnumber > 0");


            // PAGING FUNCTION
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("id DESC");

            #region ching add 确认查询到的会员列表
            var accounts = new List<string>();
            #endregion
            var rows = new List<存款账号Return>();
            var totalItems = 0L;
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                rows = connection.Query<存款账号Return>(template.RawSql, template.Parameters).ToList();
                totalItems = rows.Count() == 0 ? 0 : rows.First().TotalItems;
                var adminAccess = connection.Get<ofManage>(request.Admin).access;
                bool showTelephone = false;
                if (adminAccess.Contains("/api/Account/AllowToSeePhone")) showTelephone = true;
                if (rows.Count() > 0)
                    rows.ToList().ForEach(x => {
                        if (!showTelephone)
                        {
                            if (UtilsLib.Common.IsIncludedTel(x.account))
                                x.account = Common.Security.ObscureString(x.account);
                        }
                        accounts.Add(x.account);//添加到会员列表
                    });
                
                //return new ResponseModel<PagingModel<存款账号Return>>()
                //{
                //    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                //    message = "",
                //    Data = new PagingModel<存款账号Return>()
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
                                break;
                            }
                        }
                    }
                }
            }

            #endregion


            return new ResponseModel<PagingModel<存款账号Return>>()
            {
                code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                message = "",
                Data = new PagingModel<存款账号Return>()
                {
                    Data = rows,
                    TotalItems = totalItems
                }
            };




        }
    }
}
