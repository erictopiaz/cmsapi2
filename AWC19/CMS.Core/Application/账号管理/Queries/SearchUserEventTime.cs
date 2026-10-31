using AutoMapper;
using AWC2019.Core.Validation;
using CMS.Core.Application.提款管理;
using CMS.Core.Application.提款管理.Queries;
using Dapper;
using Dapper.Contrib.Extensions;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Linq;
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
    public class SearchUserEventTime : IRequest<ResponseModel<List<SearchUserEventTimeReturn>>>
    {
        /// <summary>
        /// 会员账号
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 当前提款的id
        /// </summary>
        public long wid { get; set; } = 0;
        /// <summary>
        /// 开始时间
        /// </summary>
        public string startTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string endTime { get; set; } 
        /// <summary>
        /// 当前登录账号，可空
        /// </summary>

        [JsonIgnore]
        public string Admin { get; set; }
    }

    /// <summary>
    /// 查询回应数据结构
    /// </summary>
    public class SearchUserEventTimeReturn
    {
        /// <summary>
        /// 会员账号
        /// </summary>
        public string account { get; set; }

        /// <summary>
        /// 当前总充值
        /// </summary>
        public decimal amount { get; set; }

        /// <summary>
        /// 当前总提款
        /// </summary>
        public decimal balance { get; set; }
        /// <summary>
        /// 当前总反水
        /// </summary>
        public string paytime { get; set; }
        /// <summary>
        /// 事件操作类型
        /// </summary>
        public int optype { get; set; }

        /// <summary>
        /// 事件操作类型 描述 0:充值，1:提款， 2:优惠红利，3:反水
        /// </summary>
        public string n { get; set; }

    }



    /// <summary>
    /// API Model Validation
    /// </summary>
    public class SearchUserEventTimeValidator : AbstractValidator<SearchUserEventTime>
    {
        /// <summary>
        /// 
        /// </summary>
        public SearchUserEventTimeValidator()
        {
            RuleFor(x => x.account).Account();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class SearchUserEventTimeTextSql
    {
        /// <summary>
        /// 查询账号基本信息 SQL
        /// </summary>
        public const string SearcharUserEventTimeSql = @"select top 100 * from (
select account, amount,balance,paytime, optype=1, n='提款' from ofWithdrawal with(nolock)
where account=@account and requesttime>='2026-06-01 00:00:00' and paytime >= @startTime and paytime < @endTime and status=2
union all
select account, amount, balance = 0, orderendtime paytime ,optype=0, n='充值' from ofDeposit with(nolock)
where account=@account and requesttime>='2026-06-01 00:00:00' and ISNULL(orderendtime,reviewtime) >= @startTime and ISNULL(orderendtime,reviewtime)< @endTime and status=1
union all
select account, amount, balance = 0, reviewtime paytime, optype=2, n='优惠红利' from ofBonusPromo with(nolock)
where account=@account and createtime>='2026-06-01 00:00:00' and reviewtime >= @startTime and reviewtime < @endTime and status=1
union all
select  account, reward, balance=0, reviewtime paytime,optype=3, n='反水' from ofTurnover with(nolock)
where account=@account and reviewtime>=@startTime and reviewtime< @endTime and status=1
)a order by a.paytime desc  ";

        /// <summary>
        /// 查询，提款信息
        /// </summary>
        public const string queyrWithdrawSql = @"select top 1 * from ofwithdrawal with(nolock)
where account=@account and status=2 and id<@wid
order by id desc";


        /// <summary>
        /// 查询会员信息
        /// </summary>
        public const string queyrUserSql = @"select account, createtime from ofUser with(nolock)
where account=@account ";

    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class SearchUserEventTimeHandler : BaseWithdraw<SearchUserEventTime, ResponseModel<List<SearchUserEventTimeReturn>>>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        public SearchUserEventTimeHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected override ResponseModel<List<SearchUserEventTimeReturn>> Handle(SearchUserEventTime request)
        {
            var currentDate = DateTime.UtcNow.AddHours(8);
            var startTime = string.Empty;
            var endTime = string.Empty;

            if (!string.IsNullOrEmpty(request.startTime)
                && !string.IsNullOrEmpty(request.endTime))
            {
                startTime = Convert.ToDateTime(request.startTime).ToString("yyyy-MM-dd HH:mm:ss");
                endTime = Convert.ToDateTime(request.endTime).ToString("yyyy-MM-dd HH:mm:ss"); ;
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
                {
                    var withdrawRow = connection.Get<ofWithdrawal>(request.wid);
                    endTime = withdrawRow.paytime?.ToString("yyyy-MM-dd HH:mm:ss");
                    var withdrawRowLow = connection.Query<ofWithdrawal>(SearchUserEventTimeTextSql.queyrWithdrawSql,
                        new { account = request.account, wid = request.wid });
                    if (withdrawRowLow != null) {
                        startTime = withdrawRowLow.First().paytime?.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        //开始时间，为注册时间
                        var userModels = connection.Query<ofUser>(SearchUserEventTimeTextSql.queyrUserSql,
                            new { account = request.account });
                        startTime = userModels.First().createtime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
            }

            if (string.IsNullOrEmpty(startTime)
                || string.IsNullOrEmpty(endTime))
            {
                return new ResponseModel<List<SearchUserEventTimeReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "操作失败",
                    Data = new List<SearchUserEventTimeReturn>()
                };
            }

            var r = DapperBase.Query<SearchUserEventTimeReturn>(SearchUserEventTimeTextSql.SearcharUserEventTimeSql,
                new { account = request.account, startTime = startTime, endTime = endTime },
                        commandTimeout: 1500, ConnectionString: CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING);
            if (r != null && r.Count > 0)
            {
                return new ResponseModel<List<SearchUserEventTimeReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "操作成功",
                    Data = r.ToList()
                };
            }
            else
            {
                return new ResponseModel<List<SearchUserEventTimeReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "操作失败",
                    Data = new List<SearchUserEventTimeReturn>()
                };
            }

        }

    }
}
