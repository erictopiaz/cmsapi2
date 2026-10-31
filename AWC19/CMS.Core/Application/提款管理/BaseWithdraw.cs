
using AutoMapper;
using CMS.Core.ViewModel;
using ShareEntity.Models;
using ShareEntity.OutputModels;
using Dapper;
using Dapper.Contrib.Extensions;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using CMS.Data.Models;

namespace CMS.Core.Application.提款管理
{
    public abstract class BaseWithdraw<TRequest, TResponse> : BaseRequestHandler<TRequest, TResponse>
         where TRequest : IRequest<TResponse>
    {
        //protected abstract TResponse Handle(TRequest request);
        //{ value: 10026, key: '核查级别' },
        //{ value: 10023, key: '爵士返水' },
        //{ value: 10020, key: '国王现金' },
        //{ value: 10022, key: '公爵返水' },
        //{ value: 10019, key: '公爵现金' },
        //{ value: 10018, key: '爵士现金' },
        //{ value: 10017, key: '骑士现金' },
        //{ value: 10021, key: '国王返水' },
        //{ value: 10024, key: '骑士返水' }

        //{ value: 10002, key: '黄金VIP' },
        //{ value: 10028, key: '白玉VIP' },
        protected readonly int[] VIP2LIST = { 10002, 10028 };

        //{ value: 10007, key: '普通玩家' },    普通会员_和_万象会员
        //{ value: 10027, key: '白银VIP' },     普通会员_和_万象会员
        //{ value: 10001, key: '万象会员' },    普通会员_和_万象会员
        protected readonly int[] 普通会员_和_万象会员 = { 10001, 10007, 10026,10012 };

        //{ value: 10007, key: '普通玩家' },    普通会员_和_万象会员
        //{ value: 10027, key: '白银VIP' },     普通会员_和_万象会员
        //{ value: 10001, key: '万象会员' },    普通会员_和_万象会员
        protected readonly int[] 普通会员_和_万象会员_和_白银VIP = { 10001, 10007, 10026, 10027 };


        //{ value: 10027, key: '白银VIP' }, 
        //{ value: 10028, key: '白玉VIP' },
        //{ value: 10002, key: '黄金VIP' },

        //{ value: 10011, key: '老虎机俱乐部' },
        protected readonly int[] VIPLIST = { 10027, 10028, 10002, 10011, 10012 };

        //{ value: 10003, key: '铂金VIP' },
        //{ value: 10004, key: '钻石VIP' },
        //{ value: 10005, key: '特邀VIP' },
        //{ value: 10025, key: '至尊VIP' },

        //{ value: 10012, key: '见习VIP' },
        //{ value: 10003, key: '铂金VIP' },
        // RequestUpdateTo  铂金，钻石，特邀，至尊
        protected readonly int[] VIP3LIST = { 10003, 10004, 10005, 10025 };

        //{ value: 10004, key: '钻石VIP' },
        //{ value: 10025, key: '至尊VIP' },
        //{ value: 10005, key: '特邀VIP' },
        //{ value: 10012, key: '见习VIP' },
        //{ value: 10003, key: '铂金VIP' },
        //{ value: 10002, key: '黄金VIP' },
        //{ value: 10028, key: '白玉VIP' },
        //{ value: 10027, key: '白银VIP' }, 
        //protected readonly int[] VIPALL = { 10004, 10025, 10005, 10012, 10003, 10002, 10028, 10027 };

        // 03/10/20 requested to update to 白玉，黄金，老虎机等级
        protected readonly int[] VIPALL = { 10028, 10002, 10011 };

        //{ value: 10004, key: '钻石VIP' },
        //{ value: 10025, key: '至尊VIP' },
        //{ value: 10005, key: '特邀VIP' },
        //{ value: 10012, key: '见习VIP' },
        //{ value: 10003, key: '铂金VIP' },
        //{ value: 10002, key: '黄金VIP' },
        //{ value: 10028, key: '白玉VIP' },
        //{ value: 10027, key: '白银VIP' }, 
        protected readonly int[] ALLUSER = { 10001, 10002, 10006, 10007, 10011, 10012, 10025, 10026, 10027, 10028 };


        protected readonly int[] VIP1LIST = { 10002, 10013, 10017, 10024, 10006 };
        protected readonly int[] 万象会员LIST = { 10001 };
        protected readonly int[] 普通会员LIST = { 10007, 10011, 10012, 10027 };
        protected readonly int[] 普通会员 = { 10007, 10011, 10012, 10027 };
        protected readonly int[] NON_VIP_CLIENT = { 10001, 10007, 10012, 10011, 10026, 10027 };

        protected readonly int[] VIP4LIST = { 10025, 10016, 10020, 10021, 10005 };
        protected readonly float 大额VIP余额 = 200000;

        /// <summary>
        /// 代付上线
        /// </summary>
        protected float _DirectPayStartAmount;
        protected float _DirectPayStartAmount_VIP;
        protected float _DirectPayStartAmount_VIP3;
        protected IWebHostEnvironment _env;
        public BaseWithdraw(string connectionString, IMapper mapper, ILoggerFactory logger) : base(connectionString, mapper, logger)
        {
            Populate_DirectPayStartAmount();

            //Populate_Levels();
            //
            // TODO: Should populate from DB instead of hard coded values
            //
        }

        public BaseWithdraw(string connectionString, IMapper mapper, ILoggerFactory logger, IWebHostEnvironment env) : base(connectionString, mapper, logger)
        {
            _env = env;
            Populate_DirectPayStartAmount();

            //Populate_Levels();
            //
            // TODO: Should populate from DB instead of hard coded values
            //
        }

        //public BaseWithdraw(string dbCconnectionString, IMapper mapper, ILoggerFactory logger) : base(dbCconnectionString, mapper, logger)
        //{
        //}

        private void Populate_DirectPayStartAmount()
        {
            //var builder = new SqlBuilder();
            //var template = builder.AddTemplate("SELECT * FROM ofSettings WITH(NOLOCK) /**where**/");
            //builder.Where("code = @code", new { code = "PAYMENT_MAXDIRECTPAY" });

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                //var result = connection.QueryFirst(template.RawSql, template.Parameters);
                //_DirectPayStartAmount = float.Parse(result.body);

                _DirectPayStartAmount = float.Parse(connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_TOP").body);
                _DirectPayStartAmount_VIP = float.Parse(connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_VIP_TOP").body);
                _DirectPayStartAmount_VIP3 = float.Parse(connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_VIP3_TOP").body);
            }

        }

        /// <summary>
        /// Check WithdrawSearch if it's beyond 31 days
        /// </summary>
        /// <param name="adminAccess"></param>
        /// <param name="starttime"></param>
        /// <param name="password"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public bool isWithdrawNoSearchDateLimitAllowed(string adminAccess, DateTime starttime, string password, out ResponseModel<PagingModel<BaseWithdrawReturn>> response)
        {
            if ((DateTime.UtcNow.AddHours(8) - (DateTime)starttime).TotalDays > 31)
            {
                if (!adminAccess.Contains("/api/Withdraw/NoSearchDateLimit"))
                {
                    response = new ResponseModel<PagingModel<BaseWithdrawReturn>>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "不允许搜索超过一个月",
                        Data = null
                    };
                    return false;
                }
                else if (password != CachedData.SEARCH_NOLIMIT_PASSWORD)
                {
                    response = new ResponseModel<PagingModel<BaseWithdrawReturn>>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "请输入正确密码",
                        Data = null
                    };
                    return false;
                }
            }

            response = null;
            return true;
        }


        /// <summary>
        /// Get Withdraw Details By ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected ofWithdrawal GetWithdrawDetails(long id)
        {

            //var builder = new SqlBuilder();
            //var template = builder.AddTemplate("" +
            //    "SELECT * " +
            //    "FROM ofWithdrawal WITH(NOLOCK) /**where**/ ");

            // FILL UP THE WHERE CLAUSE
            //builder.Where("status=1");
            //builder.Where("requesttime > @RequestTime", new { RequestTime = CMS.Core.Common.CurrentTime.AddDays(-100) });
            //builder.Where("id = @Id", new { Id = id });

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var details = connection.Get<ofWithdrawal>(id);
                return details;
                //var details = connection.QueryFirstOrDefault<ofWithdrawal>(template.RawSql, template.Parameters);
            }
        }





        // TODO:  Should Optimize this code
        public static List<string> _currentMonthAccountList;
        public static DateTime currentMonthAccountList_LastQuery;
        public List<string> currentMonthAccountList
        {
            get
            {
                DateTime monthBefore = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month - 2, 1);
                if (_currentMonthAccountList != null) return _currentMonthAccountList;

                if ((DateTime.UtcNow.AddHours(8) - currentMonthAccountList_LastQuery).TotalMinutes > 4)
                {
                    currentMonthAccountList_LastQuery = DateTime.UtcNow.AddHours(8);

                    var builder = new SqlBuilder();
                    var template = builder.AddTemplate("" +
                        "SELECT account " +
                        "FROM ofUser /**where**/");
                    builder.Where("createtime >= @Createtime", new { createtime = monthBefore });


                    using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
                    {
                        _currentMonthAccountList = connection.Query<string>(template.RawSql, template.Parameters).AsList();
                    }
                }
                return _currentMonthAccountList;
                //}
                //else
                //{
                //    currentMonthAccountList_LastQuery = DateTime.UtcNow.AddHours(8);
                //    var accountList = db.ofUsers
                //    .Where(p => p.createtime >= monthBefore)
                //    .Distinct()
                //    .Select(p => p.account).ToList();
                //    _currentMonthAccountList = accountList;
                //    return _currentMonthAccountList;
                //}
            }
        }
    }

    public class BaseWithdrawQuery : PagingModel
    {
        public string account { get; set; }
        public string cardno { get; set; }
        public string realname { get; set; }
        public string affaccount { get; set; }
        public string ipaddress { get; set; }
        public string password { get; set; }
        public string location { get; set; }
        public DateTime? starttime { get; set; }
        public DateTime? endtime { get; set; }
        public int processingtime { get; set; }
        public int status { get; set; } = -1;
        public int? paycategory { get; set; } = -1;
        public int delaytotime { get; set; } = -1;
        public int? conformedstatus { get; set; } = -1;
        [JsonIgnore]
        public string Admin { get; set; }
        [JsonIgnore]
        public List<int> currencys { get; set; }
        public int currencycode { get; set; }
    }


    public class BaseWithdrawReturn
    {
        public string walletType { get; set; }
        public string merchantAccount { get; set; }
        public string ofuser_misc { get; set; }
        public string account_tags { get; set; }
        public long id { get; set; }
        public long uid { get; set; }
        public string account { get; set; }
        public string realname { get; set; }
        public string amount { get; set; }
        public string origamount { get; set; }
        public string fee { get; set; }
        public string cardno { get; set; }
        public string cardaddress { get; set; }
        public string cardbank { get; set; }
        public DateTime requesttime { get; set; }
        public DateTime reviewtime { get; set; }
        public int status { get; set; }
        public DateTime? delaytotime { get; set; }
        public DateTime? paytime { get; set; }
        public DateTime? usercreatetime { get; set; }
        public string payperson { get; set; }
        public string payplatform { get; set; }
        public int level { get; set; }
        public string levelname { get; set; }
        //public string affaccount { get; set; }
        public string checkperson { get; set; }
        public string cancelreason { get; set; }
        public string cancelperson { get; set; }
        public string canceltime { get; set; }
        public string misc { get; set; }
        public string paymentgateway { get; set; }
        public double minutesdiff { get; set; }
        public double delaytotimediff { get; set; }
        public long TotalItems { get; set; }
        public long TotalFee { get; set; }
        public long totalmonthwithdraw { get; set; }
        public int conformedstatus { get; set; }
        public int currencycode { get; set; }
        public long totalmonthwithdrawcount { get; set; }
        /// <summary>
        /// 未到账状态
        /// </summary>
        public bool paystat { get; set; } = true;
        /// <summary>
        /// 未到账原因
        /// </summary>
        public string paystatreason { get; set; }
        public string usdt_xrate { get; set; }
        public string usdt_equiv { get; set; }
        public string xfermanual { get; set; }
        public string platform { get; set; }
        public float TotalAmount { get; set; }
        public int riskTypesAready { get; set; }
        public int wid{ get; set; }
        public int isTransferToManual{ get; set; }
        public float user_winloss { get; set; }

        public List<string> images { get; set;} = new List<string>();

    }
    public class BaseWithdraw : BaseActions
    {
        protected readonly int[] 万象会员LIST = { 10001 };
        protected readonly int[] 普通会员LIST = { 10007, 10011, 10012, 10027 };
        protected readonly int[] 普通会员 = { 10007, 10011, 10012, 10027 };
        public static readonly int[] 普通会员_和_万象会员 = { 10001, 10007, 10011, 10012, 10027 };
        protected readonly int[] NON_VIP_CLIENT = { 10001, 10007, 10012, 10011, 10026, 10027 };

        protected readonly int[] VIP1LIST = { 10002, 10013, 10017, 10024, 10006 };
        protected readonly int[] VIP2LIST = { 10003, 10014, 10018, 10023 };
        protected readonly int[] VIP3LIST = { 10004, 10015, 10019, 10022 };
        protected readonly int[] VIP4LIST = { 10025, 10016, 10020, 10021, 10005 };
        protected readonly float 大额VIP余额 = 100000;

        /// <summary>
        /// 代付上线
        /// </summary>
        protected float _DirectPayStartAmount;
        public BaseWithdraw(CMS.Data.MainDB context, IMapper mapper, ILogger logger) : base(context, mapper, logger)
        {
            Populate_DirectPayStartAmount();

            //Populate_Levels();
            //
            // TODO: Should populate from DB instead of hard coded values
            //
        }







        private void Populate_DirectPayStartAmount()
        {
            var builder = new SqlBuilder();
            var template = builder.AddTemplate("SELECT * FROM ofSettings WITH(NOLOCK) /**where**/");
            builder.Where("code = @code", new { code = "PAYMENT_MAXDIRECTPAY" });

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var result = connection.QueryFirst(template.RawSql, template.Parameters);
                _DirectPayStartAmount = float.Parse(result.body);
            }

        }

        /// <summary>
        /// Get Withdraw Details By ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected ofWithdrawal GetWithdrawDetails(long id)
        {

            //var builder = new SqlBuilder();
            //var template = builder.AddTemplate("" +
            //    "SELECT * " +
            //    "FROM ofWithdrawal WITH(NOLOCK) /**where**/ ");

            // FILL UP THE WHERE CLAUSE
            //builder.Where("status=1");
            //builder.Where("requesttime > @RequestTime", new { RequestTime = CMS.Core.Common.CurrentTime.AddDays(-100) });
            //builder.Where("id = @Id", new { Id = id });

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var details = connection.Get<ofWithdrawal>(id);
                return details;
                //var details = connection.QueryFirstOrDefault<ofWithdrawal>(template.RawSql, template.Parameters);
            }
        }

        // TODO:  Should Optimize this code
        public static List<string> _currentMonthAccountList;
        public static DateTime currentMonthAccountList_LastQuery;
        public List<string> currentMonthAccountList
        {
            get
            {
                DateTime monthBefore = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                //if (_currentMonthAccountList != null)
                //{
                if ((DateTime.UtcNow.AddHours(8) - currentMonthAccountList_LastQuery).TotalMinutes > 4)
                {
                    currentMonthAccountList_LastQuery = DateTime.UtcNow.AddHours(8);

                    var builder = new SqlBuilder();
                    var template = builder.AddTemplate("" +
                        "SELECT account " +
                        "FROM ofUser /**where**/");
                    builder.Where("createtime >= @Createtime", new { createtime = monthBefore });


                    using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
                    {
                        _currentMonthAccountList = connection.Query<string>(template.RawSql, template.Parameters).AsList();
                    }
                }
                return _currentMonthAccountList;
                //}
                //else
                //{
                //    currentMonthAccountList_LastQuery = DateTime.UtcNow.AddHours(8);
                //    var accountList = db.ofUsers
                //    .Where(p => p.createtime >= monthBefore)
                //    .Distinct()
                //    .Select(p => p.account).ToList();
                //    _currentMonthAccountList = accountList;
                //    return _currentMonthAccountList;
                //}
            }
        }
    }
}
