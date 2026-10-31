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

namespace CMS.Core.Application.账号管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 交易记录V2 : IRequest<ResponseModel<IEnumerable<交易记录V2Return>>>
    {
        public string id { get; set; }
        public string account { get; set; }
        public DateTime? startTime { get; set; }
        public DateTime endTime { get; set; }
    }

    public class 交易记录V2Return
    {
        /*
            网银	金额	提交时间	状态	存款银行	存款类型	收款人	流水号	审核人	
            第三方	金额	请求时间	订单状态	结束时间	商户订单号	银行订单号	支付平台		
            提款	金额	申请时间	状态	卡号	付款人	开户网点			
            优惠活动	金额	时间	状态	备注	新增人员	审核人员	游戏分类		
            红利优惠	赠送金额	存款金额	状态	优惠类型	投注倍数	申请时间	游戏分类		
            反水	返水	开始时间	状态	游戏分类	有效投注额	结余	结束时间	申请时间	
            余额调整	金额	时间	状态	类别	备注	创建人员	审核人员	游戏分类	
            游戏管理	转账金额	时间	状态	转账类型	左转前	左转后	右转前	右转后	优惠代码
         */
        public string TransactionType { get; set; }
        public long id { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? orderendtime { get; set; }
        public int Status { get; set; }

        public string a { get; set; }
        public string b { get; set; }
        public string c { get; set; }
        public string d { get; set; }
        public string e { get; set; }
        public string f { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 交易记录V2Validator : AbstractValidator<交易记录V2>
    {
        public 交易记录V2Validator()
        {
            RuleFor(x => x.account).Account();
            //RuleFor(x => x.id).DigitalNullable(1000, 100000000);
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 交易记录V2Handler : BaseRequestHandler<交易记录V2, ResponseModel<IEnumerable<交易记录V2Return>>>
    {
        public 交易记录V2Handler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }



        protected override ResponseModel<IEnumerable<交易记录V2Return>> Handle(交易记录V2 request)
        {


            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                if (!string.IsNullOrEmpty(request.id))
                {
                    var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                    request.account = accountRow.account;
                }

                if (request.startTime == null)
                {
                    var result = connection.QueryFirstOrDefault<DateTime?>("" +
                        "SELECT TOP 1 requesttime " +
                        "FROM ofwithdrawal WITH(NOLOCK) " +
                        "WHERE account=@ACCOUNT AND status = 2 " +
                        "ORDER BY ID DESC",
                        new { ACCOUNT = request.account });

                        request.startTime = result ?? request.startTime;
                }

                // 创建DAPPER TEMPLATE 
                var depositBuilder = new SqlBuilder();

                // This is using BETWEEN
                //var depositTemplate = depositBuilder.AddTemplate("" +
                //    "SELECT id, TransactionType = 'deposit',amount,status, " +
                //    "ISNULL(reviewtime, orderendtime) AS orderendtime, " +
                //    "ISNULL(depositmethod, payplatform) AS a, " +
                //    "ISNULL(merchantno, concat(receiveperson, ' - ', additional)) AS b " +
                //    "FROM ofDeposit WITH(NOLOCK) " +
                //    "WHERE (reviewtime BETWEEN @STARTTIME AND @ENDTIME OR orderendtime BETWEEN @STARTTIME AND @ENDTIME) " +
                //        "AND account=@ACCOUNT " +
                //        "AND status >= 0" +
                //    "ORDER BY id DESC");
                //// FILL UP THE WHERE CLAUSE
                //depositBuilder.AddParameters(new { ACCOUNT = request.account, STARTTIME = request.startTime, ENDTIME = request.endTime });

                // THIS is using GREATER THAN
                var depositTemplate = depositBuilder.AddTemplate("" +
                    "SELECT id, TransactionType = 'deposit',amount, " +
                    "ISNULL(reviewtime, orderendtime) AS orderendtime, " +
                    "status,fee as a,rebate as b,realname as c, " +
                    "ISNULL(depositmethod, payplatform) AS d, " +
                    "ISNULL(merchantno, concat(receiveperson, additional)) AS e " +
                    "FROM ofDeposit WITH(NOLOCK) " +
                    "WHERE (reviewtime >= @STARTTIME  OR orderendtime >= @STARTTIME) " +
                        "AND account=@ACCOUNT " +
                        "AND status = 1" +
                    "ORDER BY id DESC");
                // FILL UP THE WHERE CLAUSE
                depositBuilder.AddParameters(new { ACCOUNT = request.account, STARTTIME = request.startTime, ENDTIME = request.endTime });


                //depositBuilder.Where("account = @ACCOUNT", new { ACCOUNT = request.account });
                //depositBuilder.Where("status > 0");
                //depositBuilder.OrWhere("reviewtime BETWEEN @STARTTIME AND @ENDTIME",
                //    new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                //depositBuilder.OrWhere("orderendtime BETWEEN @STARTTIME AND @ENDTIME",
                //    new { STARTTIME = request.startTime, ENDTIME = request.endTime });



                // 创建DAPPER TEMPLATE 
                var withdrawBuilder = new SqlBuilder();
                var withdrawTemplate = withdrawBuilder.AddTemplate("" +
                    "SELECT id, TransactionType = 'withdraw',amount,status, " +
                    "requesttime AS a, " +
                    "ISNULL(ISNULL(paytime, canceltime),requesttime) AS orderendtime, " +
                    "ISNULL(ISNULL(payperson,cancelperson),requesttime) AS f, " +
                    "cancelreason AS b, cardno as c, concat('[',cardbank , '] ', cardaddress) as d, realname as e " +
                    "FROM ofwithdrawal WITH(NOLOCK) /**where**/ ORDER BY id DESC");
                // FILL UP THE WHERE CLAUSE
                withdrawBuilder.Where("requesttime >= @STARTTIME", new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                //withdrawBuilder.Where("requesttime BETWEEN @STARTTIME AND @ENDTIME", new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                withdrawBuilder.Where("account = @ACCOUNT", new { ACCOUNT = request.account });
                withdrawBuilder.Where("status >= 0");


                // 创建DAPPER TEMPLATE 
                var transferBuilder = new SqlBuilder();
                var transferTemplate = transferBuilder.AddTemplate("" +
                    "SELECT id, TransactionType = 'transfer',amount,status, " +
                    "endtime AS orderendtime, " +
                    "requesttime AS a, " +
                    "type AS b ," +
                    "requestip AS e ," +
                    "concat(leftamountbefore,' - ',leftamountafter) AS c ," +
                    "concat(rightamountbefore,' - ',rightamountafter) AS d " +
                    "FROM ofTransfer WITH(NOLOCK) /**where**/ ORDER BY id DESC");
                // FILL UP THE WHERE CLAUSE
                transferBuilder.Where("requesttime >= @STARTTIME", new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                //transferBuilder.Where("requesttime BETWEEN @STARTTIME AND @ENDTIME", new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                transferBuilder.Where("account = @ACCOUNT", new { ACCOUNT = request.account });
                transferBuilder.Where("status >= 0");



                // 创建DAPPER TEMPLATE 
                //var bonuspromoBuilder = new SqlBuilder();
                //var bonuspromoTemplate = bonuspromoBuilder.AddTemplate("" +
                //    "SELECT id, TransactionType = 'bonuspromo',reward AS amount,status, " +
                //    "reviewtime AS orderendtime, " +
                //    "remarks AS a, " +
                //    "bet AS b " +
                //    "FROM ofbonuspromo WITH(NOLOCK) /**where**/ ORDER BY id DESC");
                //// FILL UP THE WHERE CLAUSE
                //bonuspromoBuilder.Where("reviewtime BETWEEN @STARTTIME AND @ENDTIME",
                //    new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                //bonuspromoBuilder.Where("account = @ACCOUNT", new { ACCOUNT = request.account });
                //bonuspromoBuilder.Where("status >= 0");


                // THIS IS USING BETWEEN
                //var bonuspromoBuilder = new SqlBuilder();
                //var bonuspromoTemplate = bonuspromoBuilder.AddTemplate("" +
                //    "SELECT  TransactionType = 'bonuspromo', amount,a,b,status,orderendtime FROM(" +
                //    "SELECT reward AS amount, remarks AS a, bet AS b, status, reviewtime AS orderendtime " +
                //        "FROM ofBonusPromo WITH(NOLOCK) " +
                //        "WHERE reviewtime BETWEEN @STARTTIME AND @ENDTIME AND account=@ACCOUNT UNION ALL " +
                //    "SELECT amount, remarks AS a,0 AS b,status, reviewtime AS orderendtime " +
                //        "FROM ofPromotions WITH(NOLOCK) " +
                //        "WHERE reviewtime BETWEEN @STARTTIME AND @ENDTIME AND account=@ACCOUNT)z " +
                //        "ORDER BY orderendtime DESC");
                //// FILL UP THE WHERE CLAUSE
                //bonuspromoBuilder.Where("reviewtime BETWEEN @STARTTIME AND @ENDTIME",
                //    new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                //bonuspromoBuilder.Where("account = @ACCOUNT", new { ACCOUNT = request.account });
                //bonuspromoBuilder.Where("status >= 0");
                // THIS  IS USING GREATER THAN 
                //var bonuspromoBuilder = new SqlBuilder();
                //var bonuspromoTemplate = bonuspromoBuilder.AddTemplate("" +
                //    "SELECT  TransactionType = 'bonuspromo', amount,a,b,status,orderendtime FROM(" +
                //    "SELECT reward AS amount, remarks AS a, bet AS b, status, reviewtime AS orderendtime " +
                //        "FROM ofBonusPromo WITH(NOLOCK) " +
                //        "WHERE reviewtime >= @STARTTIME AND account=@ACCOUNT UNION ALL " +
                //    "SELECT amount, remarks AS a,0 AS b,status, reviewtime AS orderendtime " +
                //        "FROM ofPromotions WITH(NOLOCK) " +
                //        "WHERE reviewtime >= @STARTTIME AND account=@ACCOUNT)z " +
                //        "ORDER BY orderendtime DESC");
                //// FILL UP THE WHERE CLAUSE
                //bonuspromoBuilder.Where("reviewtime BETWEEN @STARTTIME AND @ENDTIME",
                //    new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                //bonuspromoBuilder.Where("account = @ACCOUNT", new { ACCOUNT = request.account });





                // 创建DAPPER TEMPLATE 
                var bonuspromoBuilder = new SqlBuilder();
                var bonuspromoTemplate = bonuspromoBuilder.AddTemplate("" +
                    "SELECT id, TransactionType = 'bonuspromo',amount,status, " +
                    "reviewtime AS orderendtime, " +
                    "remarks AS a ," +
                    "reward AS b ," +
                    "bet AS c ," +
                    "createperson AS d " +
                    "FROM ofBonusPromo WITH(NOLOCK) /**where**/ ORDER BY id DESC");
                // FILL UP THE WHERE CLAUSE
                bonuspromoBuilder.Where("reviewtime BETWEEN @STARTTIME AND @ENDTIME",
                    new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                bonuspromoBuilder.Where("account = @ACCOUNT", new { ACCOUNT = request.account });
                bonuspromoBuilder.Where("status >= 0");

                
                // 创建DAPPER TEMPLATE 
                var promotionsBuilder = new SqlBuilder();
                var promotionsTemplate = promotionsBuilder.AddTemplate("" +
                    "SELECT id, TransactionType = 'promo',amount,status, " +
                    "reviewtime AS orderendtime, " +
                    "remarks AS a ," +
                    "createperson AS b ," +
                    "gametype AS d ," +
                    "reviewperson AS c " +
                    "FROM ofPromotions WITH(NOLOCK) /**where**/ ORDER BY id DESC");
                // FILL UP THE WHERE CLAUSE
                promotionsBuilder.Where("reviewtime BETWEEN @STARTTIME AND @ENDTIME",
                    new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                promotionsBuilder.Where("account = @ACCOUNT", new { ACCOUNT = request.account });
                promotionsBuilder.Where("status >= 0");



                // 创建DAPPER TEMPLATE 
                var minusBuilder = new SqlBuilder();
                var minusTemplate = minusBuilder.AddTemplate("" +
                    "SELECT id, TransactionType = 'minus'," +
                    "CASE WHEN decreasetype=1 THEN amount ELSE -amount END AS amount," +
                    "status, " +
                    "reviewtime AS orderendtime, " +
                    "remarks AS a, " +
                    "amountbefore AS b, " +
                    "amountafter AS c ," +
                    "createperson AS d, " +
                    "gametype AS f, " +
                    "reviewperson AS e " +
                    "FROM ofminus WITH(NOLOCK) /**where**/ ORDER BY id DESC");
                // FILL UP THE WHERE CLAUSE
                minusBuilder.Where("reviewtime >= @STARTTIME", new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                //minusBuilder.Where("reviewtime BETWEEN @STARTTIME AND @ENDTIME", new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                minusBuilder.Where("account = @ACCOUNT", new { ACCOUNT = request.account });
                minusBuilder.Where("status >= 0");


                // 创建DAPPER TEMPLATE 
                var turnoverBuilder = new SqlBuilder();
                var turnoverTemplate = turnoverBuilder.AddTemplate("" +
                    "SELECT id, TransactionType = 'turnover'," +
                    "reward AS amount, " +
                    "status, " +
                    "reviewtime AS orderendtime, " +
                    "begindate AS a, " +
                    "enddate AS d, " +
                    "gametype AS b, " +
                    "totalbets AS c, " +
                    "leftbets AS e " +
                    "FROM ofturnover WITH(NOLOCK) /**where**/ ORDER BY id DESC");
                // FILL UP THE WHERE CLAUSE
                turnoverBuilder.Where("reviewtime >= @STARTTIME", new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                //turnoverBuilder.Where("reviewtime BETWEEN @STARTTIME AND @ENDTIME", new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                turnoverBuilder.Where("account = @ACCOUNT", new { ACCOUNT = request.account });
                turnoverBuilder.Where("status >= 0");

                var depositRows = connection.Query<交易记录V2Return>(depositTemplate.RawSql, depositTemplate.Parameters);
                if (depositRows.Count() > 0)
                    depositRows.ToList().ForEach(x => x.d = CMS.Core.CachedData.PayPlatformDict[x.d]);



                var withdrawRows = connection.Query<交易记录V2Return>(withdrawTemplate.RawSql, withdrawTemplate.Parameters);
                var transferRows = connection.Query<交易记录V2Return>(transferTemplate.RawSql, transferTemplate.Parameters);
                if (transferRows.Count() > 0)
                    transferRows.ToList().ForEach(x => x.b = CMS.Core.CachedData.GameCodeDict[int.Parse(x.b)]);


                var bonuspromoRows = connection.Query<交易记录V2Return>(bonuspromoTemplate.RawSql, bonuspromoTemplate.Parameters);

                var promotionsRows = connection.Query<交易记录V2Return>(promotionsTemplate.RawSql, promotionsTemplate.Parameters);
                //var promotionsRows = connection.Query<交易记录V2Return>(promotionsTemplate.RawSql, promotionsTemplate.Parameters);
                var minusRows = connection.Query<交易记录V2Return>(minusTemplate.RawSql,
                    minusTemplate.Parameters);
                if (minusRows.Count() > 0)
                    minusRows.ToList().ForEach(x => x.f = CMS.Core.CachedData.GameCodeDict[int.Parse(x.f ?? "100")]);



                var turnoverRows = connection.Query<交易记录V2Return>(turnoverTemplate.RawSql, turnoverTemplate.Parameters);
                if (turnoverRows.Count() > 0)
                    turnoverRows.ToList().ForEach(x => x.b = CMS.Core.CachedData.GameCodeDict[int.Parse(x.b)]);


                return new ResponseModel<IEnumerable<交易记录V2Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = depositRows
                    .Concat(withdrawRows)
                    .Concat(transferRows)
                    .Concat(bonuspromoRows)
                    .Concat(promotionsRows)
                    .Concat(minusRows)
                    .Concat(turnoverRows)
                };
            }
        }
    }
}
