using AutoMapper;
using AWC2019.Core.Validation;
using Ching.DistributedTransaction.EFModel;
using Ching.DistributedTransaction.EFModel.TableModel;
using CMS.Core.ViewModel;
using CMS.Data.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ShareEntity.Models;
using ShareEntity.OutputModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using UtilsLib.Merchant;
using UtilsLib.ValidationAttr;
using static CMS.Core.Common;

namespace CMS.Core.Application.账号管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 交易记录 : IRequest<ResponseModel<IEnumerable<交易记录Return>>>
    {
        public long id { get; set; }
        public string account { get; set; }


        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }




    }

    public class 交易记录Return
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
        public string g { get; set; }
        public string h { get; set; }
        public string i { get; set; }
        public string j { get; set; }
        public string k { get; set; }
        public string l { get; set; }
        public string m { get; set; }
        public string n { get; set; }
        public decimal rightamountbefore { set; get; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 交易记录Validator : AbstractValidator<交易记录>
    {
        public 交易记录Validator()
        {
            RuleFor(x => x.account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 交易记录Handler : BaseRequestHandler<交易记录, ResponseModel<IEnumerable<交易记录Return>>>
    {
        public 交易记录Handler(IMapper mapper, ILoggerFactory logger, IWebHostEnvironment env)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger, env) { }



        protected override ResponseModel<IEnumerable<交易记录Return>> Handle(交易记录 request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                string id = request.id.ToString();
                SqlCommand qmd = new SqlCommand();

                if (request.id > 0) {
                    qmd = new SqlCommand("SELECT account, currencyType, merchantAccount FROM ofuser WITH(NOLOCK) WHERE id = @ID");
                    qmd.Parameters.AddWithValue("@ID", request.id);
                }
                else
                {
                    qmd = new SqlCommand("SELECT TOP 1 account, currencyType, merchantAccount FROM ofuser WITH(NOLOCK) WHERE account = @ACCOUNT");
                    qmd.Parameters.AddWithValue("@ACCOUNT", request.account);
                }

                var sql = qmd.CommandText;
                var parameters = qmd.Parameters.Cast<SqlParameter>()
                    .ToDictionary(p => p.ParameterName, p => p.Value);
                var accountRow = connection.QueryFirst<dynamic>(sql, parameters);

                //var accountRow = connection.QueryFirst<dynamic>(qmd.ToString(),qmd.pa);
                //var accountRow = connection.QueryFirst<dynamic>(qmd.ToString(),qmd.pa);

                request.account = accountRow.account;
                CMS.Core.CachedData.currencyType = (int)accountRow.currencyType;
                var defModel = MerchantCodes.GetDefaultModel((string)accountRow.merchantAccount, (int)accountRow.currencyType);
                if (request.startTime == null)
                {
                    var firstWithdrawRow = connection.QueryFirstOrDefault<ofWithdrawalEx>("" +
                       "SELECT TOP 1 requesttime " +
                       "FROM ofwithdrawal WITH(NOLOCK) " +
                       "WHERE account=@ACCOUNT AND status = 2 " +
                       "ORDER BY ID DESC",
                       new { ACCOUNT = request.account });

                    request.startTime = firstWithdrawRow != null ? firstWithdrawRow.requesttime : Convert.ToDateTime(CurrentTime.ToString("yyyy-MM-dd 00:00:00"));
                    request.endTime = CurrentTime;
                }

                if (request.startTime == null) {
                    request.startTime = CurrentTime.AddDays(-3);
                    request.endTime = CurrentTime;
                }


                // 创建DAPPER TEMPLATE 
                var depositBuilder = new SqlBuilder();

               

                // THIS is using GREATER THAN
                var depositTemplate = depositBuilder.AddTemplate("" +
                    "SELECT id, TransactionType = 'deposit',amount,ipaddress AS d, status, " +
                    "ISNULL(reviewtime, orderendtime) AS orderendtime, " +
                    "ISNULL(depositmethod, payplatform) AS a, " +
                    "ISNULL(merchantno, concat(receiveperson, ' - ', additional)) AS b, " +
                    "fee as c, " +
                    "rebate as f, " +
                    "deposittype as g, " +
                    "ipaddress as j, " +
                    "misc as k , " +
                    "realname as h " +
                    "FROM ofDeposit WITH(NOLOCK) " +
                    "WHERE (reviewtime >= @STARTTIME  OR orderendtime >= @STARTTIME) " +
                        "AND account=@ACCOUNT " +
                        "AND status >= 0 " +
                        "AND payplatform not in (10530,10572,10783,10772) " +
                    "ORDER BY id DESC");
                // FILL UP THE WHERE CLAUSE
                depositBuilder.AddParameters(new { ACCOUNT = request.account, STARTTIME = request.startTime, ENDTIME = request.endTime });


                var depositCombinedBuilder = new SqlBuilder();

                // THIS is using GREATER THAN
                var depositCombinedTemplate = depositCombinedBuilder.AddTemplate("" +
                    "SELECT id, TransactionType = 'depositcombined',amount,ipaddress AS d, status, " +
                    "ISNULL(reviewtime, orderendtime) AS orderendtime, " +
                    "ISNULL(depositmethod, payplatform) AS a, " +
                    "ISNULL(merchantno, concat(receiveperson, ' - ', additional)) AS b, " +
                    "fee as c, " +
                    "rebate as f, " +
                    "deposittype as g, " +
                    "ipaddress as j, " +
                    "misc as k , " +
                    "realname as h " +
                    "FROM ofDeposit WITH(NOLOCK) " +
                    "WHERE (reviewtime >= @STARTTIME  OR orderendtime >= @STARTTIME) " +
                        "AND account=@ACCOUNT " +
                        "AND status >= 0 " +
                        //"AND payplatform not in (10530,10572) " +
                    "ORDER BY id DESC");
                // FILL UP THE WHERE CLAUSE
                depositCombinedBuilder.AddParameters(new { ACCOUNT = request.account, STARTTIME = request.startTime, ENDTIME = request.endTime });



                var crossBuilder = new SqlBuilder();
                var crossTemplate = crossBuilder.AddTemplate("" +
                    "SELECT dp.id, TransactionType = 'cross',dp.amount,dp.ipaddress AS d, dp.status, " +
                    "ISNULL(dp.reviewtime, dp.orderendtime) AS orderendtime, " +
                    "ISNULL(dp.depositmethod, dp.payplatform) AS a, " +
                    "ISNULL(dp.merchantno, concat(dp.receiveperson, ' - ', dp.additional)) AS b, " +
                    "dp.fee as c, " +
                    "dp.rebate as f, " +
                    "dp.deposittype as g, " +
                    "dp.ipaddress as j, " +
                    "dp.misc as k , " +
                    "wd.conformedstatus as h, " +
                    "wd.requesttime as n, " +
                    "ISNULL(wd.paytime,bp.createtime) as m, " +
                    "dp.fixPromoP l FROM " +
                    "(SELECT * FROM ofDeposit WITH(NOLOCK) " +
                    "WHERE (reviewtime >= @STARTTIME  OR orderendtime >= @STARTTIME) " +
                        "AND account=@ACCOUNT " +
                        "AND status >= 0 " +
                        "AND payplatform in (10530,10572,10783,10772) " +
                    "ORDER BY id DESC OFFSET 0 rows)dp " +
                    "LEFT JOIN ofcrosstransact ct WITH(NOLOCK) ON dp.merchantno = ct.orderid " +
                    "LEFT JOIN ofwithdrawal wd WITH(NOLOCK) ON ct.withdrawid = wd.id " +
                    "outer apply (select top 1 createtime from ofBonuspromo bb WHERE bb.referenceguid = wd.referenceguid)bp " );
                // FILL UP THE WHERE CLAUSE
                crossBuilder.AddParameters(new { ACCOUNT = request.account, STARTTIME = request.startTime, ENDTIME = request.endTime });


              


                // 创建DAPPER TEMPLATE 
                var withdrawBuilder = new SqlBuilder();
                var withdrawTemplate = withdrawBuilder.AddTemplate("" +
                    "SELECT id, TransactionType = 'withdraw',amount,status, " +
                    "ISNULL(ISNULL(paytime, canceltime), reviewtime) AS orderendtime, " +
                    "ISNULL(canceltime, reviewtime) AS i, " +
                    "requesttime AS a, " +
                    "realname AS h, " +
                    "misc as j, " +
                    "ISNULL(ISNULL(payperson,cancelperson),reviewperson) AS b, " +
                    "cancelreason AS c, " +
                    "concat('[',cardbank , '] ', cardaddress) as g," +
                    "cardno AS f, " +
                    "requestip AS d " +
                    "FROM ofwithdrawal WITH(NOLOCK) /**where**/ ORDER BY id DESC");
                // FILL UP THE WHERE CLAUSE
                withdrawBuilder.Where("requesttime BETWEEN @STARTTIME AND @ENDTIME", new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                //withdrawBuilder.Where("requesttime BETWEEN @STARTTIME AND @ENDTIME", new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                withdrawBuilder.Where("account = @ACCOUNT", new { ACCOUNT = request.account });
                withdrawBuilder.Where("status >= 0");


                // 创建DAPPER TEMPLATE 
                var transferBuilder = new SqlBuilder();
                var transferTemplate = transferBuilder.AddTemplate("" +
                    "SELECT a.id,a.TransactionType,a.amount,a.rightamountbefore,a.status,a.elementid,a.orderendtime,a.a,a.b,a.c,a.d,a.e,balanceAfterTransfer.balanceJson as g FROM (SELECT TOP 1000000 id, TransactionType = 'transfer',amount,status,elementid, " +
                    "endtime AS orderendtime, " +
                    "requesttime AS a, " +
                    "type AS b ," +
                    "requestip AS e," +
                    "concat(leftamountbefore,' - ',leftamountafter) AS c , " +
                    "concat(rightamountbefore,' - ',rightamountafter) AS d, " +
                    "rightamountbefore " + //ching add 2025-03-20
                    "FROM ofTransfer WITH(NOLOCK) " +
                    //"LEFT JOIN balanceAfterTransfer ON elementid=balanceAfterTransfer.transferGuid " +
                    "/**where**/ ORDER BY id DESC)a " +
                    "LEFT JOIN balanceAfterTransfer ON a.elementid=balanceAfterTransfer.transferGuid " +
                    "ORDER BY a.id DESC");
                // FILL UP THE WHERE CLAUSE
                transferBuilder.Where("requesttime BETWEEN @STARTTIME AND @ENDTIME", new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                //transferBuilder.Where("requesttime BETWEEN @STARTTIME AND @ENDTIME", new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                transferBuilder.Where("ofTransfer.account = @ACCOUNT", new { ACCOUNT = request.account });
                transferBuilder.Where("status >= 0");



               
                // THIS  IS USING GREATER THAN 
                var bonuspromoBuilder = new SqlBuilder();
                var bonuspromoTemplate = bonuspromoBuilder.AddTemplate("" +
                    "SELECT  TransactionType = 'bonuspromo', amount,a,b,c,d,e,status,orderendtime FROM(" +
                    "SELECT reward AS amount, remarks AS a, bet AS b, c=amount,status, reviewtime AS orderendtime," +
                    "d='红利', e=bonusPromoCode " +
                        "FROM ofBonusPromo WITH(NOLOCK) " +
                        "WHERE reviewtime >= @STARTTIME AND account=@ACCOUNT UNION ALL " +
                    "SELECT amount, remarks AS a, b=0,c=0,status, reviewtime AS orderendtime, " +
                    "d='优惠', e='' " +
                        "FROM ofPromotions WITH(NOLOCK) " +
                        "WHERE reviewtime >= @STARTTIME AND account=@ACCOUNT)z " +
                        "ORDER BY orderendtime DESC");
                // FILL UP THE WHERE CLAUSE
                bonuspromoBuilder.Where("reviewtime BETWEEN @STARTTIME AND @ENDTIME",
                    new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                bonuspromoBuilder.Where("account = @ACCOUNT", new { ACCOUNT = request.account });


                var bonuspromoselfBuilder = new SqlBuilder();
                var bonuspromoselfTemplate = bonuspromoBuilder.AddTemplate("" +
                    "SELECT  TransactionType = 'bonuspromoself', amount,a,b,c,d,e,status,orderendtime FROM(" +
                    "SELECT reward AS amount, remarks AS a, bet AS b, c=amount,status, " +
                    "reviewtime AS orderendtime," +
                    "d='福利', e=bonusPromoCode " +
                        "FROM ofBonusPromoSelf WITH(NOLOCK) " +
                        "WHERE reviewtime >= @STARTTIME AND account=@ACCOUNT)z " +
                        "ORDER BY orderendtime DESC");
                // FILL UP THE WHERE CLAUSE
                bonuspromoselfBuilder.Where("reviewtime BETWEEN @STARTTIME AND @ENDTIME",
                    new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                bonuspromoselfBuilder.Where("account = @ACCOUNT", new { ACCOUNT = request.account });




                // 创建DAPPER TEMPLATE 
                var minusBuilder = new SqlBuilder();
                var minusTemplate = minusBuilder.AddTemplate("" +
                    "SELECT id, TransactionType = 'minus'," +
                    "CASE WHEN decreasetype=1 THEN amount ELSE -amount END AS amount," +
                    "status, " +
                    "reviewtime AS orderendtime, " +
                    "remarks AS a, " +
                    "amountbefore AS b, " +
                    "amountafter AS c " +
                    "FROM ofminus WITH(NOLOCK) /**where**/ ORDER BY id DESC");
                // FILL UP THE WHERE CLAUSE
                minusBuilder.Where("reviewtime BETWEEN @STARTTIME AND @ENDTIME", new { STARTTIME = request.startTime, ENDTIME = request.endTime });
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
                    "gametype AS b, " +
                    "totalbets AS c " +
                    "FROM ofturnover WITH(NOLOCK) /**where**/ ORDER BY id DESC");
                // FILL UP THE WHERE CLAUSE
                turnoverBuilder.Where("reviewtime BETWEEN @STARTTIME AND @ENDTIME", new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                //turnoverBuilder.Where("reviewtime BETWEEN @STARTTIME AND @ENDTIME", new { STARTTIME = request.startTime, ENDTIME = request.endTime });
                turnoverBuilder.Where("account = @ACCOUNT", new { ACCOUNT = request.account });
                turnoverBuilder.Where("status >= 0");

                string registered_realname = connection.QueryFirst<string>("SELECT realname FROM ofUser WITH(NOLOCK) WHERE account='" + request.account + "'");

                var depositRows = connection.Query<交易记录Return>(depositTemplate.RawSql, depositTemplate.Parameters);
                if (depositRows.Count() > 0) {
                    var payPlatformDict = CMS.Core.CachedData.PayPlatformDict;
                    depositRows.ToList().ForEach(x => {
                        if (!string.IsNullOrEmpty(x.k)) {
                            var json_xk = JObject.Parse(x.k);
                            if (json_xk.ContainsKey("posted_realname") &&
                                registered_realname.Trim() != json_xk["posted_realname"].ToString().Trim())
                                x.k = string.Concat(json_xk["posted_realname"].ToString()).Trim();
                            //x.k = string.Concat(registered_realname, "/", json_xk["posted_realname"]);
                            else
                                x.k = "";
                        }
                        x.a = payPlatformDict.ContainsKey(x.a) ? payPlatformDict[x.a] : "NA";
                        x.e = Network.GetLocation(x.d, _env.ContentRootPath + "//qqwry.dat");
                        x.j = string.Concat(x.j, "【", x.e, "】");
                    });
                }
                var depositCombinedRows = connection.Query<交易记录Return>(depositCombinedTemplate.RawSql, depositCombinedTemplate.Parameters);
                if (depositCombinedRows.Count() > 0) {
                    var payPlatformDict = CMS.Core.CachedData.PayPlatformDict;
                    depositCombinedRows.ToList().ForEach(x => {
                        if (!string.IsNullOrEmpty(x.k)) {
                            var json_xk = JObject.Parse(x.k);
                            if (json_xk.ContainsKey("posted_realname") &&
                                registered_realname.Trim() != json_xk["posted_realname"].ToString().Trim())
                                x.k = string.Concat(json_xk["posted_realname"].ToString()).Trim();
                            //x.k = string.Concat(registered_realname, "/", json_xk["posted_realname"]);
                            else
                                x.k = "";
                        }
                        x.a = payPlatformDict.ContainsKey(x.a) ? payPlatformDict[x.a]:"NA";
                        x.e = Network.GetLocation(x.d, _env.ContentRootPath + "//qqwry.dat");
                        x.j = string.Concat(x.j, "【", x.e, "】");
                    });
                }
                var crossRows = connection.Query<交易记录Return>(crossTemplate.RawSql, crossTemplate.Parameters);
                if (crossRows.Count() > 0) {
                    var payPlatformDict = CMS.Core.CachedData.PayPlatformDict;
                    crossRows.ToList().ForEach(x => {
                        if (!string.IsNullOrEmpty(x.k)) {
                            var json_xk = JObject.Parse(x.k);
                            if (json_xk.ContainsKey("posted_realname") &&
                                registered_realname.Trim() != json_xk["posted_realname"].ToString().Trim())
                                x.k = string.Concat(json_xk["posted_realname"].ToString()).Trim();
                            //x.k = string.Concat(registered_realname, "/", json_xk["posted_realname"]);
                            else
                                x.k = "";
                        }
                        if (!string.IsNullOrEmpty(x.m)) {
                            x.m = (DateTime.Parse(x.m) - DateTime.Parse(x.n)).TotalMinutes.ToString("0");
                        }
                        x.l = float.Parse(x.l).ToString("0");
                        x.a = payPlatformDict.ContainsKey(x.a) ? payPlatformDict[x.a] : "NA";
                        x.e = Network.GetLocation(x.d, _env.ContentRootPath + "//qqwry.dat");
                        x.j = string.Concat(x.j, "【", x.e, "】");
                    });
                }


                var withdrawRows = connection.Query<交易记录Return>(withdrawTemplate.RawSql, withdrawTemplate.Parameters);
                if (withdrawRows.Count() > 0)
                    withdrawRows.ToList().ForEach(x =>
                    {
                        x.e = Network.GetLocation(x.d, _env.ContentRootPath + "//qqwry.dat");
                        x.f = Common.Security.ObscureWithdrawCard(x.f);
                        if (!string.IsNullOrEmpty(x.j))
                        {
                            var misc = JObject.Parse(x.j);
                            x.j = misc.ContainsKey("cancelType") ? misc["cancelType"].ToString() : "提款取消";
                        }
                        else
                        {
                            x.j = "提款取消";
                           }
                    });


                #region 转账记录

                var transferRows = new List<交易记录Return>();
                if (defModel.merchantAccount.Equals("Awc"))
                {
                    transferRows = connection.Query<交易记录Return>(transferTemplate.RawSql, transferTemplate.Parameters).ToList();
                    if (transferRows.Count() > 0)
                    {
                        #region 2024-07-08 ching 修改
                        var gameCodeDict = CMS.Core.CachedData.GameCodeDict;
                        #endregion

                        transferRows.ToList().ForEach(x =>
                        {
                            if (int.Parse(x.b) > 3000)
                            {
                                x.d = $"{x.rightamountbefore} - {x.rightamountbefore + x.Amount}";
                            }

                            x.b = gameCodeDict.ContainsKey(int.Parse(x.b)) ? gameCodeDict[int.Parse(x.b)] : "NA";
                            x.f = Network.GetLocation(x.e, _env.ContentRootPath + "//qqwry.dat");
                            x.j = string.Concat(x.e, "【", Network.GetLocation(x.e, _env.ContentRootPath + "//qqwry.dat"), "】");


                            List<string> balanceList = new List<string>();
                            if (!string.IsNullOrEmpty(x.g))
                            {
                                var jarr = Newtonsoft.Json.Linq.JArray.Parse(x.g);
                                foreach (Newtonsoft.Json.Linq.JObject item in jarr)
                                {
                                    try
                                    {
                                        float bal = float.Parse(item["data"]["balance"].ToString());
                                        if (bal > 0)
                                            balanceList.Add(string.Concat(item["data"]["type"].ToString().ToUpper(), "=",
                                             item["data"]["balance"].ToString()));
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                                x.g = string.Join('\n', balanceList);
                            }


                        });
                    }
                }
                else
                {
                    using (var context = new AmountRecordDbContext(DapperStringBase.ConnectionGamePostgreString))
                    {
                        //transfer
                        // 查询要更新的账户，并锁定该记录
                        var startTime = request.startTime?.ToString("yyyy-MM-dd HH:mm:ss");
                        var users = context.GameWalletTranferPostgres
                            .FromSqlRaw($"SELECT * FROM \"public\".\"game_wallet_transaction\" where user_name='{request.account}' and transfer_type=100 and addtime>'{startTime}' order by id desc ").ToList();

                        if (users != null && users.Count > 0)
                        {
                            foreach (var user in users){
                                var r_mark = user.remarks;
                                var ip = "";
                                var amountactive = "";
                                var l_befor = "";
                                var l_after = "";
                                var r_befor = "";
                                var r_after = "";
                                if (r_mark.Contains("#"))
                                {
                                    var rs = r_mark.Split('#');
                                    ip = rs[2];
                                    amountactive = rs[1];
                                    r_mark = rs[0];
                                }

                                if(user.change_type == 1)
                                {
                                    var gameName = GameIds.gameNames.ContainsKey(user.game_id)
                                        ? GameIds.gameNames[user.game_id] : "游戏钱包";
                                    //游戏平台加分
                                    l_befor = string.IsNullOrEmpty(amountactive) ? "NA" : amountactive;
                                    l_after = string.IsNullOrEmpty(amountactive) ? "NA" : (Convert.ToDecimal(amountactive) - user.amount ).ToString();
                                    r_befor = (user.balance - user.amount).ToString();
                                    r_after = user.balance.ToString();
                                    r_mark = $"主帐号 >> {gameName}";
                                }
                                else
                                {
                                    //游戏平台减分，转到主钱包
                                    l_befor = (user.balance + user.amount).ToString(); 
                                    l_after = user.balance.ToString(); 
                                    r_befor = string.IsNullOrEmpty(amountactive) ? "NA" : amountactive;
                                    r_after = string.IsNullOrEmpty(amountactive) ? "NA" : (user.amount + Convert.ToDecimal(amountactive)).ToString();
                                    r_mark = "游戏钱包 >> 主帐号";
                                }
                                transferRows.Add(new 交易记录Return()
                                {
                                    TransactionType = "transfer",
                                    id = user.id,
                                    Amount = user.amount,
                                    orderendtime = Convert.ToDateTime(user.addtime.ToString("yyyy-MM-dd HH:mm:ss")),
                                    Status = 1,
                                    a = user.addtime.ToString("yyyy-MM-dd HH:mm:ss"),
                                    b = r_mark, //"主帐号 >> IMSP",
                                    c = $"{l_befor} - {l_after}",//"52000.00 - 51500.00", // 左前，左后
                                    d = $"{r_befor} - {r_after}",//"0.00 - 500.00",  //右前 ，右后
                                    e = ip, // "",
                                    f = Network.GetLocation(ip, _env.ContentRootPath + "//qqwry.dat"),// "贵州省贵阳市联通",
                                    j = "",
                                    rightamountbefore = 0
                                });
                            }
                        }
                       
                    }

                }


                #endregion 

                var bonuspromoRows = connection.Query<交易记录Return>(bonuspromoTemplate.RawSql, bonuspromoTemplate.Parameters);
                #region 2024-07-08 ching 修改
                if (bonuspromoRows.Count() > 0) {
                    var bonusPromoAgainstDict = CMS.Core.CachedData.BonusPromoAgainstDict;
                    bonuspromoRows.ToList().ForEach(x => 
                    x.a = string.IsNullOrEmpty(x.a) ? (bonusPromoAgainstDict.ContainsKey(int.Parse(x.e)) ? bonusPromoAgainstDict[int.Parse(x.e)] :"NA") : x.a);
                }
                ////改前
                //if (bonuspromoRows.Count() > 0)
                //    bonuspromoRows.ToList().ForEach(x => x.a = string.IsNullOrEmpty(x.a) ? CMS.Core.CachedData.BonusPromoAgainstDict[int.Parse(x.e)] : x.a);

                #endregion



                var bonuspromoselfRows = connection.Query<交易记录Return>(bonuspromoselfTemplate.RawSql, bonuspromoselfTemplate.Parameters);
              
                var minusRows = connection.Query<交易记录Return>(minusTemplate.RawSql,
                    minusTemplate.Parameters);
                var turnoverRows = connection.Query<交易记录Return>(turnoverTemplate.RawSql, turnoverTemplate.Parameters);
                if (turnoverRows.Count() > 0) {
                    turnoverRows.ToList().ForEach(x => x.b = CMS.Core.CachedData.GameCodeDict[int.Parse(x.b)]);
                }

                #region 绑卡验证码 最近30天 的记录
                var verifyCodeSql = string.Format(@"select transactionType='verifyCode', validate a, account, type n, createtime orderendtime,validatetime l, status from ofAccountValidate with(nolock)
where account='{0}' and type=3 and createtime>='{1}'
order by id desc", request.account, CurrentTime.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss"));
                var verifyCodeRows = connection.Query<交易记录Return>(verifyCodeSql);
                if(verifyCodeRows.Count() > 0)
                {
                    verifyCodeRows.ToList().ForEach(x =>
                    {
                        if (x.n.Equals("3"))
                            x.m = "绑卡验证码";
                        x.j = x.Status == 1 ? "验证成功" : "未验证";
                    });
                }
                #endregion



                return new ResponseModel<IEnumerable<交易记录Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = depositRows
                    .Concat(withdrawRows)
                    .Concat(transferRows)
                    .Concat(crossRows)
                    .Concat(depositCombinedRows)
                    .Concat(bonuspromoRows)
                    .Concat(bonuspromoselfRows)
                    //.Concat(promotionsRows)
                    .Concat(minusRows)
                    .Concat(turnoverRows)
                    .Concat(verifyCodeRows)
                };
            }
        }
    }
}
