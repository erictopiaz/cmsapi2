
using AutoMapper;
using AWC2019.Core.Validation;
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
using System.Text.Json.Serialization;

namespace CMS.Core.Application.游戏管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 游戏转账搜索 : PagingModel, IRequest<ResponseModel<PagingModel<游戏转账搜索Return>>>
    {
        //账户 游戏类型   状态{成功/失败/处理中 代理 IP 开始时间，结束时间  {查询}

        public string account { get; set; }
        public string type { get; set; }
        public int typedirection { get; set; }
        public int status { get; set; } = -1;
        //public string affaccount { get; set; }
        public string ipaddress { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }

        [JsonIgnore]
        public List<int> currencys { get; set; }
        public int currencycode { get; set; }
    }

    public class 游戏转账搜索Return
    {
        //账号 转账金额    时间 转账类型    左转前 左转后 右转前 右转后 优惠代码 状态  管理
        public string account { get; set; }
        public decimal amount { get; set; }
        public DateTime requesttime { get; set; }
        public int type { get; set; }
        public string typename { get; set; }
        public decimal leftamountbefore { get; set; }
        public decimal leftamountafter { get; set; }
        public decimal rightamountbefore { get; set; }
        public decimal rightamountafter { get; set; }
        public decimal TotalAmount { get; set; }
        public int status { get; set; }
        internal long TotalItems { get; set; }
        //public long id { get; set; }
        //public string currency { get; set; }
        //public string promotioncode { get; set; }
        //public DateTime? endtime { get; set; }
        //public string acode { get; set; }
        //public string elementid { get; set; }
        //public string referrerno { get; set; }
        //public string paymentid { get; set; }
        //public string errordesc { get; set; }
        //public string requestip { get; set; }
        //public string requestaddress { get; set; }
        //public string referrer { get; set; }
        //public string referrerreg { get; set; }
        //public int level { get; set; }
        //public string referenceguid { get; set; }
        //public string xmlapi { get; set; }
        //public string _operator { get; set; }
        //public int? transtatus { get; set; }
        //public int? process { get; set; }
        //public int? alert { get; set; }
        //public string reviewperson { get; set; }
        //public DateTime? reviewtime { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 游戏转账搜索Validator : AbstractValidator<游戏转账搜索>
    {
        public 游戏转账搜索Validator()
        {
            RuleFor(x => x.account).Account();
            RuleFor(x => x.ipaddress).IP();
            RuleFor(x => x.type).CharAndDigitalNullable(1,10);

            //RuleFor(x => x.PageSize).PageSize();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 游戏转账搜索Handler : BaseRequestHandler<游戏转账搜索, ResponseModel<PagingModel<游戏转账搜索Return>>>
    {
        public 游戏转账搜索Handler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<PagingModel<游戏转账搜索Return>> Handle(游戏转账搜索 request)
        {
            if (!request.currencys.Contains(request.currencycode))
            {
                return new ResponseModel<PagingModel<游戏转账搜索Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择货币查询"
                };
            }

            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            bool withWhere = false;
            bool withTimeRange = false;

            if (request.starttime.Year > 2010 && request.endtime.Year > 2010)
            {

                if (request.status == 1)
                    builder.Where("a.requesttime >= @starttime AND a.requesttime <= @endtime", new { request.starttime, request.endtime });
                else
                    builder.Where("a.requesttime BETWEEN @starttime AND @endtime", new { request.starttime, request.endtime });

                withWhere = true;
                withTimeRange = true;
            }
            else if (request.starttime.Year > 2010)
            {
                withWhere = true;
                withTimeRange = request.status == 1;
                builder.Where("a.requesttime >= @starttime", new
                {
                    starttime = request.starttime
                });
            }
            else // Limit to 3 Months
                builder.Where("a.requesttime >= @starttime", new { starttime = Common.CurrentTime.AddDays(-7) });


            if (!string.IsNullOrEmpty(request.ipaddress)) { withWhere = true; builder.Where("ipaddress = @ipaddress", new { request.ipaddress }); }

            if (!string.IsNullOrEmpty(request.account)) { withWhere = true; builder.Where("a.account = @ACCOUNT", new { request.account }); }

            if (request.currencycode > 0) { builder.Where("f.currencytype = @currencycode", new { request.currencycode }); }

            if (request.status == -1) { }
            else if(request.status==1)
                builder.Where("a.Status = @Status", new { request.status });
            else
                builder.Where("a.Status <> 1");

            string gameType = CachedData.gameBalanceTransfer(request.type, request.typedirection);
            if (!string.IsNullOrEmpty(gameType)) {

                if (request.typedirection < 0)
                {
                    var xx = new List<string>();
                    xx.Add(CachedData.gameBalanceTransfer(request.type, 0));
                    xx.Add(CachedData.gameBalanceTransfer(request.type, 1));
                    builder.Where("type IN @type", new { type = xx });

                }
                else
                    builder.Where("type = @type", new { type = int.Parse(gameType) });
            }

            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("requesttime DESC");

            SqlBuilder.Template template;

            #region

            if (withWhere && withTimeRange)
                template = builder.AddTemplate("" +
                    "SELECT f.currencytype,a.account,amount,a.requesttime,a.type,a.leftamountbefore,a.leftamountafter,a.rightamountbefore,a.rightamountafter, trans_r_status as status , " +
                    "COUNT(*) OVER() as TotalItems, " +
                    "SUM(amount) OVER() as TotalAmount " +
                    "FROM ofTransfer a WITH(NOLOCK) " +
                    "LEFT JOIN ofUser f WITH(NOLOCK) ON f.account = a.account " +
                    "LEFT JOIN ofTransferFinished b ON a.elementid = b.f_tid  /**where**/ /**orderby**/ " +
                    "OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY");

            else if (withWhere)
                template = builder.AddTemplate("" +
                    "SELECT f.currencytype,a.account,amount,a.requesttime,a.type,a.leftamountbefore,a.leftamountafter,a.rightamountbefore,a.rightamountafter, trans_r_status as status , " +
                    "COUNT(*) OVER() as TotalItems " +
                    "FROM ofTransfer a WITH(NOLOCK) " +
                    "LEFT JOIN ofUser f WITH(NOLOCK) ON f.account = a.account " +
                    "LEFT JOIN ofTransferFinished b ON a.elementid = b.f_tid  /**where**/ /**orderby**/ " +
                    "OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY");

            else
                template = builder.AddTemplate("" +
                    "SELECT f.currencytype,a.account,amount,a.requesttime,a.type,a.leftamountbefore,a.leftamountafter,a.rightamountbefore,a.rightamountafter, trans_r_status as status , " +
                    "TotalItems=1000 " +
                    "FROM ofTransfer a WITH(NOLOCK) " +
                    "LEFT JOIN ofUser f WITH(NOLOCK) ON f.account = a.account " +
                    "LEFT JOIN ofTransferFinished b ON a.elementid = b.f_tid  /**where**/ /**orderby**/ " +
                    "OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY");
            #endregion

            builder.OrderBy("a.id DESC");

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var rows = connection.Query<游戏转账搜索Return>(template.RawSql, template.Parameters);

                long totalItems = 0;
                CMS.Core.CachedData.currencyType = request.currencycode;
                if (rows.Count() > 0)
                {
                    totalItems = rows.First().TotalItems;
                    // Apply the LevelName 
                    //if (rows.Count() > 0)
                    //    rows.ToList().ForEach(x => x.typename = CMS.Core.CachedData.GameCodeDict[x.type]);
                    if (rows.Count() > 0)
                    {
                        foreach (var row in rows)
                        {
                            row.typename = CMS.Core.CachedData.GameCodeDict[row.type];
                            if(row.type>3000)
                            {
                                row.rightamountafter = row.rightamountbefore + row.amount;
                            }
                        }
                    }
                        
                }



                var result = new ResponseModel<PagingModel<游戏转账搜索Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                    Data = new PagingModel<游戏转账搜索Return>()
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
