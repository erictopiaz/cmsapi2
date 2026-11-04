using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using CMS.Data.Models;
using AWC2019.Core.ViewModel;
using FluentValidation;
using System.Data.SqlClient;
using Dapper;
using AWC2019.Core.Validation;
using Newtonsoft.Json;
using CMS.Core.Application.提款管理;
using ShareEntity.OutputModels;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using CMS.Core;
using CMS.Core.ViewModel;
using Dapper.Contrib.Extensions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using UtilsLib.JWT;
using UtilsLib;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class ThirdpayRecord : PagingModel, IRequest<ResponseModel<PagingModel<ThirdpayRecordReturn>>>
    {
        public string name { get; set; }

        public int status { get; set; }
        public int appstatus { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }

        public int currencyType { get; set; } = 0;
        public string adminAccount { set; get; } = "";

        public string token { set; get; } = "";
        public string loginkey { set; get; } = "";
        public int walletType { set; get; } = -1;
        public string merchantAccount { set; get; } = "-1";

    }

    public class ThirdpayRecordReturn
    {

        public bool regulargroup { get; set; }
        public bool vipgroup { get; set; }
        public bool vip3group { get; set; }
        public long id { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public decimal paymentcap { get; set; }
        public int status { get; set; }
        public int? sort { get; set; }
        public int? appstatus { get; set; }
        public int? minamount { get; set; }
        public int? maxamount { get; set; }
        public int? paycategory { get; set; }
        public double rebate { get; set; }
        public long alteramount { get; set; }
        public string levels { get; set; }
        public string fastpay { get; set; }
        public string fastdeposit { get; set; }
        internal string misc { get; set; }
        internal string queryMerchantWhere { get; set; }
        public float totalamountdeposit { get; set; }
        public float totaldepositpercent { get; set; }
        public int totalsuccess { get; set; }
        public int totalusdt { get; set; }
        public int totalfail { get; set; }
        public int dailyrequestlimit { get; set; }
        public int totaldepositcount { get; set; }
        public int isAgentUsed { get; set; }
        public int appnewwindow { get; set; }
        public string callbackip { get; set; }
        public string banks { get; set; }
        public double feecent { get; set; }
        public long mindepositcount { get; set; }
        public bool requirenote { get; set; }
        public bool requirebank { get; set; }
        public bool requirename { get; set; }
        public bool requirecardno { get; set; }
        public bool mustterminate { get; set; }
        public bool mustnotbedivisiblebyzero { get; set; }
        public bool deductfee { get; set; }
        public bool noduplicateamount { get; set; }
        public bool newusers { get; set; }
        public bool allowedamountinput { get; set; }
        public long classtemp { get; set; }
        [JsonIgnore]
        internal long TotalItems { get; set; }
        public long queryWhereModel_status { get; set; }
        public long queryWhereModel_day { get; set; }
        public bool registeredname { get; set; }
        public string merchantids { get; set; }
        public int merchantlevel { get; set; }
        public double self_feecent { get; set; }
        public long predeductcount { get; set; }
        /// <summary>
        /// 会员的品牌
        /// </summary>
        public string merchantAccount { set; get; } = "-1";
        public int walletType { set; get; } = -1;

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class ThirdpayRecordValidator : AbstractValidator<ThirdpayRecord>
    {
        public ThirdpayRecordValidator()
        {
            RuleFor(x => x.name).CharDigitalChineseNullable(1, 10);
            RuleFor(x => x.status).Status();
            RuleFor(x => x.appstatus).Status();
            RuleFor(x => x.currencyType).Status();
            //RuleFor(x => x.starttime).date();
            //RuleFor(x => x.endtime).Datetime();

        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class ThirdpayRecordHandler : BaseRequestHandler<ThirdpayRecord, ResponseModel<PagingModel<ThirdpayRecordReturn>>>
    {
        public ThirdpayRecordHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<PagingModel<ThirdpayRecordReturn>> Handle(ThirdpayRecord request)
        {

            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT *,t_walletType as walletType,t_merchantAccounts as merchantAccount ,class AS classtemp, COUNT(*) OVER() as TotalItems " +
                "FROM ofThirdpay WITH(NOLOCK) /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY");

            var currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(request.token, request.loginkey);
            if (!currencys.Contains(request.currencyType))
            {
                //货币类型有误
                return new ResponseModel<PagingModel<ThirdpayRecordReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择货币查询",
                    Data = new PagingModel<ThirdpayRecordReturn>()
                    {
                        Data = new List<ThirdpayRecordReturn>(),
                        TotalItems = 0
                    }
                };
            }


            // FILL UP THE WHERE CLAUSE
            if (request.status >= 0)
                builder.Where("status = @status", new { request.status });
            else
                builder.Where("status<2");
            if (request.appstatus >= 0)
                builder.Where("appstatus=@appstatus ", new { request.appstatus });

            if (request.currencyType >= 0)
            {
                builder.Where("currencyType = @currencyType", new { request.currencyType });
            }
            else
            {
                return new ResponseModel<PagingModel<ThirdpayRecordReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "无数据",
                    Data = new PagingModel<ThirdpayRecordReturn>()
                    {
                        Data = new List<ThirdpayRecordReturn>(),
                        TotalItems = 0
                    }
                };
            }

            if (!string.IsNullOrEmpty(request.name))
                builder.Where("name LIKE @NAMES", new { NAMES = string.Concat("%", request.name, "%") });

            if (!request.merchantAccount.Equals("-1"))
            {
                builder.Where("t_merchantAccounts LIKE @merchantAccount", new { merchantAccount = string.Concat("%", request.merchantAccount, "%") });

            }
            if (request.walletType != -1)
            {
                builder.Where("t_walletType in (@walletType,2)", new { walletType = request.walletType });

            }


            var depositBuilder = new SqlBuilder();
            //var depositTemplate = depositBuilder.AddTemplate("" +
            //    "SELECT payplatform AS id, SUM(amount) AS totalamountdeposit, COUNT(*) AS totalsuccess " +
            //    "FROM ofDeposit WITH(NOLOCK)  " +
            //    "/**where**/ GROUP BY payplatform");

            var depositTemplate = depositBuilder.AddTemplate("" +
                "SELECT payplatform AS id, " +
                "SUM(amount) AS totalamountdeposit, " +
                "SUM(amount / NULLIF(TRY_CAST(JSON_VALUE(CASE WHEN ISJSON(misc) = 1 THEN misc ELSE NULL END, '$.usdt_erc20_xrate') AS DECIMAL(18,8)), 0)) AS totalusdt, " +
                "COUNT(*) AS totalsuccess " +
                "FROM ofDeposit WITH(NOLOCK) " +
                "/**where**/ GROUP BY payplatform");

            var depositLogBuilder = new SqlBuilder();
            var depositLogTemplate = depositLogBuilder.AddTemplate("" +
                "SELECT payplatform AS id, COUNT(*) AS totalfail " +
                "FROM ofDepositLog WITH(NOLOCK)  " +
                "/**where**/ GROUP BY payplatform");

            var depositTotalBuilder = new SqlBuilder();
            var depositTotalTemplate = depositTotalBuilder.AddTemplate("" +
                "SELECT payplatform AS id, COUNT(*) AS totaldepositcount " +
                "FROM ofDeposit WITH(NOLOCK)  " +
                "/**where**/ GROUP BY payplatform");




            if (request.starttime.Year >= 2002)
            {
                depositBuilder.Where("requesttime >= @STARTTIME AND requesttime <= @ENDTIME",
                    new { STARTTIME = request.starttime, ENDTIME = request.endtime });
                depositBuilder.Where("deposittype = 0 AND status=1");

                depositTotalBuilder.Where("requesttime >= @STARTTIME AND requesttime <= @ENDTIME",
                    new { STARTTIME = request.starttime, ENDTIME = request.endtime });
                depositTotalBuilder.Where("deposittype=0");

                depositLogBuilder.Where("requesttime >= @STARTTIME AND requesttime <= @ENDTIME",
                    new { STARTTIME = request.starttime, ENDTIME = request.endtime });
            }
            else
            {
                depositBuilder.Where("requesttime >= @STARTTIME AND orderendtime >= @STARTTIME AND orderendtime <= @ENDTIME",
                    new { STARTTIME = Common.CurrentTime.ToString("yyyy-MM-dd"), ENDTIME = Common.CurrentTime.ToString("yyyy-MM-dd") + " 23:59:59" });
                depositBuilder.Where("deposittype = 0 AND status=1");

                depositTotalBuilder.Where("requesttime >= @STARTTIME AND requesttime <= @ENDTIME",
                    new { STARTTIME = Common.CurrentTime.ToString("yyyy-MM-dd"), ENDTIME = Common.CurrentTime.ToString("yyyy-MM-dd") + " 23:59:59" });
                depositTotalBuilder.Where("deposittype=0");

                depositLogBuilder.Where("requesttime >= @STARTTIME AND requesttime <= @ENDTIME",
                    new { STARTTIME = Common.CurrentTime.ToString("yyyy-MM-dd"), ENDTIME = Common.CurrentTime.ToString("yyyy-MM-dd") + " 23:59:59" });

            }


            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });

            //builder.OrderBy("id DESC");

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var depositRows = connection.Query<ThirdpayRecordReturn>(depositTemplate.RawSql, depositTemplate.Parameters);
                var totalamountDict = depositRows != null && depositRows.Count() > 0 ? depositRows.ToDictionary(x => x.id, x => x.totalamountdeposit) : new Dictionary<long, float>();
                var totalsuccessDict = depositRows != null && depositRows.Count() > 0 ? depositRows.ToDictionary(x => x.id, x => x.totalsuccess) : new Dictionary<long, int>();
                var totalusdtDict = depositRows != null && depositRows.Count() > 0 ? depositRows.ToDictionary(x => x.id, x => x.totalusdt) : new Dictionary<long, int>();

                var depositTotalRows = connection.Query<ThirdpayRecordReturn>(depositTotalTemplate.RawSql, depositTotalTemplate.Parameters);
                var totalcountDict = depositTotalRows != null && depositTotalRows.Count() > 0 ? depositTotalRows.ToDictionary(x => x.id, x => x.totaldepositcount) : new Dictionary<long, int>();

                var depositLogRows = connection.Query<ThirdpayRecordReturn>(depositLogTemplate.RawSql, depositLogTemplate.Parameters);
                var totalfailDict = depositLogRows != null && depositLogRows.Count() > 0 ? depositLogRows.ToDictionary(x => x.id, x => x.totalfail) : new Dictionary<long, int>();


                string idfirst = "";
                List<string> idarray = new List<string>();
                foreach (var item in totalamountDict)
                    idarray.Add(item.Key.ToString());


                if (idarray.Count() > 0)
                    builder.OrderBy("CASE WHEN id in (" + string.Join(',', idarray) + ") THEN '1' WHEN status = 1 THEN '0' ELSE id END");
                else
                    builder.OrderBy("CASE WHEN status = 1 THEN '0' ELSE id END");



                var rows = connection.Query<ThirdpayRecordReturn>(template.RawSql, template.Parameters);

                if (rows.Count() <= 0)
                {
                    return new ResponseModel<PagingModel<ThirdpayRecordReturn>>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "无数据",
                        Data = new PagingModel<ThirdpayRecordReturn>()
                        {
                            Data = rows,
                            TotalItems = 0
                        }
                    };
                }


                var totalItems = rows.First().TotalItems;

                var banklists = connection.Get<ofSettings>("PAYMENT_THIRDPAYBANK");
                var banklistsDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(banklists.body);
                foreach (var item in rows)
                {
                    try
                    {
                        var paytyperow = connection.Get<ofPayType>(item.classtemp);
                        if (banklistsDict.ContainsKey(item.id.ToString()) && !string.IsNullOrEmpty(banklistsDict[item.id.ToString()]))
                        {
                            item.banks = banklistsDict[item.id.ToString()].Replace("|", ",");
                        }
                        else
                            item.banks = "";

                        item.callbackip = paytyperow.callbackip;

                        if (item.levels.Length > 0)
                        {
                            var levels = item.levels.Split(',');
                            var levelname = new List<string>();
                            foreach (var item1 in levels)
                                if (!string.IsNullOrEmpty(item1))
                                    if (CMS.Core.CachedData.LevelNameDict.ContainsKey(int.Parse(item1)))
                                        levelname.Add(CMS.Core.CachedData.LevelNameDict[int.Parse(item1)]);
                            item.levels = string.Join(",", levelname);
                        }

                        if (totalamountDict.ContainsKey(item.id))
                        {
                            item.totalamountdeposit = totalamountDict[item.id];
                            item.totaldepositpercent = (item.totalamountdeposit / CachedData.MAX_DEPOSIT_PER_BANK) * 100;
                            item.totaldepositpercent = (float)Math.Floor(item.totaldepositpercent);
                            item.totalsuccess = (int)totalsuccessDict[item.id];
                            item.totalusdt = (int)totalusdtDict[item.id];
                        }

                        if (totalcountDict.ContainsKey(item.id))
                        {
                            item.totaldepositcount = (int)totalcountDict[item.id];
                        }

                        if (totalfailDict.ContainsKey(item.id))
                        {
                            item.totalfail = (int)totalfailDict[item.id];
                            item.totaldepositcount += item.totalfail;

                        }

                        int indexmarker = item.name.IndexOf('【');
                        if (indexmarker >= 0)
                        {
                            item.category = item.name.Substring(indexmarker);
                            if (!string.IsNullOrEmpty(item.misc))
                            {
                                var miscDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(item.misc);
                                var queryWhereModelDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(item.queryMerchantWhere);

                                item.feecent = miscDict.ContainsKey("feecent") ? miscDict["feecent"] : 0;
                                item.mindepositcount = miscDict.ContainsKey("mindepositcount") ? miscDict["mindepositcount"] : 0;
                                item.alteramount = miscDict.ContainsKey("alteramount") ? miscDict["alteramount"] : 0;
                                item.regulargroup = miscDict.ContainsKey("regulargroup") ? miscDict["regulargroup"] : false;
                                item.vipgroup = miscDict.ContainsKey("vipgroup") ? miscDict["vipgroup"] : false;
                                item.vip3group = miscDict.ContainsKey("vip3group") ? miscDict["vip3group"] : false;

                                item.requirebank = miscDict.ContainsKey("requirebank") ? miscDict["requirebank"] == 1 : false;
                                item.requirenote = miscDict.ContainsKey("requirenote") ? miscDict["requirenote"] == 1 : false;
                                item.requirecardno = miscDict.ContainsKey("requirecardno") ? miscDict["requirecardno"] == 1 : false;
                                item.requirename = miscDict.ContainsKey("requirename") ? miscDict["requirename"] == 1 : false;
                                item.mustterminate = miscDict.ContainsKey("mustterminate") ? miscDict["mustterminate"] == 1 : false;
                                item.mustnotbedivisiblebyzero = miscDict.ContainsKey("mustnotbedivisiblebyzero") ? miscDict["mustnotbedivisiblebyzero"] == 1 : false;
                                item.deductfee = miscDict.ContainsKey("deductfee") ? miscDict["deductfee"] == 1 : false;
                                item.noduplicateamount = miscDict.ContainsKey("noduplicateamount") ? miscDict["noduplicateamount"] == 1 : false;
                                item.newusers = miscDict.ContainsKey("newusers") ? miscDict["newusers"] == 1 : false;
                                item.allowedamountinput = miscDict.ContainsKey("allowedamountinput") ? miscDict["allowedamountinput"] == 1 : true;
                                item.registeredname = miscDict.ContainsKey("registeredname") ? miscDict["registeredname"] == 1 : true;
                                item.self_feecent = miscDict.ContainsKey("self_feecent") ? miscDict["self_feecent"] : 0;
                                item.predeductcount = miscDict.ContainsKey("predeductcount") ? miscDict["predeductcount"] : 0;
                                item.appnewwindow = miscDict.ContainsKey("appnewwindow") ? (int)miscDict["appnewwindow"] : 0;
                                item.dailyrequestlimit = miscDict.ContainsKey("dailyrequestlimit") ? (int)miscDict["dailyrequestlimit"] : 0;

                                item.queryWhereModel_day = queryWhereModelDict.ContainsKey("days") ? queryWhereModelDict["days"] : 0;
                                item.queryWhereModel_status = queryWhereModelDict.ContainsKey("status") ? queryWhereModelDict["status"] : 0;

                                double miscDictFeeCent = 0;
                                double miscDictRebate = 0;

                                if (miscDict.ContainsKey("feecent"))
                                    miscDictFeeCent = miscDict["feecent"] > 0 ? miscDict["feecent"] : 0;
                                if (miscDict.ContainsKey("rebate"))
                                    miscDictRebate = miscDict["rebate"] > 0 ? miscDict["rebate"] : 0;

                                item.feecent = miscDictFeeCent;
                                item.rebate = miscDictRebate;
                                item.mindepositcount = miscDict.ContainsKey("mindepositcount") ? miscDict["mindepositcount"] : 0;
                                item.alteramount = miscDict.ContainsKey("alteramount") ? miscDict["alteramount"] : 0;
                            }
                        }
                        else
                        {
                            item.category = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        string error = ex.Message;
                        _logger.LogInformation(ex.Message);
                    }
                }

                rows = rows.OrderByDescending(u => u.totaldepositpercent);

                return new ResponseModel<PagingModel<ThirdpayRecordReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new PagingModel<ThirdpayRecordReturn>()
                    {
                        Data = rows,
                        TotalItems = totalItems
                    }
                };
            }
        }
    }
}

