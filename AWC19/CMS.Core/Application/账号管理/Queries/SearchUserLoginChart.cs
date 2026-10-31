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
    public class SearchUserLoginChart : IRequest<ResponseModel<List<SearchUserLoginChartReturn>>>
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
    public class SearchUserLoginChartReturn
    {
        /// <summary>
        /// 登录设备  1:android，2：ios,   3:h5  , 4: pc 
        /// </summary>
        public int l_device { get; set; }

        /// <summary>
        /// 登录次数
        /// </summary>
        public int times { get; set; } = 0;

    }



    /// <summary>
    /// API Model Validation
    /// </summary>
    public class SearchUserLoginChartValidator : AbstractValidator<SearchUserLoginChart>
    {
        /// <summary>
        /// 
        /// </summary>
        public SearchUserLoginChartValidator()
        {
            RuleFor(x => x.account).Account();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class SearchUserLoginChartTextSql
    {
        /// <summary>
        /// 查询账号基本信息 SQL
        /// </summary>
        public const string SearcharUserLoginChartSql = @"select l_device, count(id) times from ofLog with(nolock)
where account=@account and event_type=11
group by l_device  ";


    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class SearchUserLoginChartHandler : BaseWithdraw<SearchUserLoginChart, ResponseModel<List<SearchUserLoginChartReturn>>>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        public SearchUserLoginChartHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected override ResponseModel<List<SearchUserLoginChartReturn>> Handle(SearchUserLoginChart request)
        {
            var currentDate = DateTime.UtcNow.AddHours(8);

            
            var r = DapperBase.Query<SearchUserLoginChartReturn>(SearchUserLoginChartTextSql.SearcharUserLoginChartSql,
                new { account = request.account },
                        commandTimeout: 1500, ConnectionString: CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING);
            if (r != null)
            {
                return new ResponseModel<List<SearchUserLoginChartReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "操作成功",
                    Data = r.ToList()
                };
            }
            else
            {
                return new ResponseModel<List<SearchUserLoginChartReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "操作失败",
                    Data = new List<SearchUserLoginChartReturn>()
                };
            }

        }

    }
}
