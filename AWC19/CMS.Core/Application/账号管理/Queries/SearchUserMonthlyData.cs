using AutoMapper;
using AWC2019.Core.Validation;
using CMS.Core.Application.提款管理;
using CMS.Core.Application.提款管理.Queries;
using Dapper.Contrib.Extensions;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShareEntity.Models;
using ShareEntity.OutputModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Core.Application.账号管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class SearchUserMonthlyData : IRequest<ResponseModel<SearchUserMonthlyDataReturn>>
    {
        /// <summary>
        /// 会员账号
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 当前登录账号，可空
        /// </summary>

        [JsonIgnore]
        public string Admin { get; set; }
    }

    /// <summary>
    /// 查询回应数据结构
    /// </summary>
    public class SearchUserMonthlyDataReturn
    {
        /// <summary>
        /// 会员账号
        /// </summary>
        public string account { get; set; }
       
        /// <summary>
        /// 当前总充值
        /// </summary>
        public decimal depositAmount { get; set; }
       
        /// <summary>
        /// 当前总提款
        /// </summary>
        public decimal withdrawAmount { get; set; }
        /// <summary>
        /// 当前总反水
        /// </summary>
        public decimal turnover { get; set; }

        /// <summary>
        /// 当前总红利
        /// </summary>
        public decimal bonus { get; set; }

    }



    /// <summary>
    /// API Model Validation
    /// </summary>
    public class SearchUserMonthlyDataValidator : AbstractValidator<SearchUserMonthlyData>
    {
        /// <summary>
        /// 
        /// </summary>
        public SearchUserMonthlyDataValidator()
        {
            RuleFor(x => x.account).Account();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class SearchUserMonthlyDataTextSql
    {
        /// <summary>
        /// 查询账号基本信息 SQL
        /// </summary>
        public const string SearcharUserMonthlyDataSql = @"select account,sum(p.depositAmount) depositAmount, sum(p.withdrawAmount) withdrawAmount, sum(p.turnover) turnover, sum(p.bonus+p.promo) bonus from ofPlayerDailyReportFG p with(nolock)
where financeGameDay>=@startTime and financeGameDay<@endTime and account=@account
group by account  ";


    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class SearchUserMonthlyDataHandler : BaseWithdraw<SearchUserMonthlyData, ResponseModel<SearchUserMonthlyDataReturn>>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        public SearchUserMonthlyDataHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected override ResponseModel<SearchUserMonthlyDataReturn> Handle(SearchUserMonthlyData request)
        {
            var currentDate = DateTime.UtcNow.AddHours(8);
            var startTime =  Convert.ToInt32(currentDate.ToString("yyyyMM01"));
            var endTime =  Convert.ToInt32(currentDate.AddMonths(1).ToString("yyyyMM01"));

            //开始切割金额
            var r = DapperBase.Query<SearchUserMonthlyDataReturn>(SearchUserMonthlyDataTextSql.SearcharUserMonthlyDataSql, 
                new { account = request.account, startTime= startTime, endTime=endTime },
                        commandTimeout: 1500, ConnectionString: CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING);
            if (r != null)
            {
                return new ResponseModel<SearchUserMonthlyDataReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "操作成功",
                    Data = r.First()
                };
            }
            else
            {
                return new ResponseModel<SearchUserMonthlyDataReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "操作失败",
                    Data = new SearchUserMonthlyDataReturn()
                };
            }

        }

    }
}
