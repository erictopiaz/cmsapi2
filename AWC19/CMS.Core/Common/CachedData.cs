using CMS.Data.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using Org.BouncyCastle.Asn1.Ocsp;
using ShareEntity.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CMS.Core
{
    public class CachedData
    {
        public const float MAX_DEPOSIT_PER_BANK = 200000;
        public const string SEARCH_NOLIMIT_PASSWORD = "4r9i320";
        public const int 第三方超一个月免输入密码_TotalMinutes = 60;
        public static int currencyType { set; get; } = 156;//

        // TODO: Should come from "using Microsoft.Extensions.Caching.Memory;"
        // For REFACTORING  

        private static string[] _gamename;
        public static string[] GameName
        {
            get
            {

                if (_gamename != null) 
                    return _gamename;

                var builder = new SqlBuilder();
                var template = builder.AddTemplate("" +
                    "SELECT gamename " +
                    "FROM ofGameClass WITH(NOLOCK) /**where**/ ");

                // FILL UP THE WHERE CLAUSE
                builder.Where("class = 1");
                builder.Where("useable = 1");

                builder.Where("gamename NOT IN @EXCLUDENAME", new
                {
                    EXCLUDENAME = new List<string>() {
                    "SPB", "主钱包", "万象城娱乐","PT老虎机","SPT", "BSG"
                }
                });

                using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
                {
                    var result = connection.Query<string>(template.RawSql, template.Parameters);
                    var resultArray = result.ToArray();
                    for (int i = 0; i < resultArray.Count(); i++)
                        resultArray[i] = resultArray[i].ToUpper() == "UED" ? "LB" : resultArray[i];

                    _gamename = resultArray;
                    return resultArray;


                }
            }
        }

        public static List<string> GetGameNames(int currencyType) {

            var querySql = string.Format(@"select gamename2 from ofgameclass
where class=1 and useable=1 and currencyTypes like '%{0}%' ", currencyType);

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING)) {
                var result = connection.Query<string>(querySql);
                var resultArray = result.ToArray();
                for (int i = 0; i < resultArray.Count(); i++)
                    resultArray[i] = resultArray[i].ToUpper() == "UED" ? "LB" : resultArray[i];

                return resultArray.ToList();
            }
        }
       
        private static Dictionary<int, string> _levelnamedict;
        private string _AccessString;
        public static Dictionary<int, string> LevelNameDict
        {
            get
            {
                if (_levelnamedict != null) return _levelnamedict;


                // 创建DAPPER TEMPLATE 
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("" +
                    "SELECT levelnumber,levels FROM ofUserGroup WITH(NOLOCK) ORDER BY levelnumber");

                using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
                {
                    var myDictionary = connection.Query(template.RawSql)
                        .ToDictionary(x => (int)x.levelnumber, x => (string)x.levels);

                    _levelnamedict = myDictionary;
                    return _levelnamedict;
                }
            }
        }

        public static string AccessString(string admin) {
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING)) {
                return connection.Get<ofManage>(admin).access;

            }
        }


        public static Dictionary<string, DateTime> QueryTimeDict = new Dictionary<string, DateTime>() {
            { "DirectPayRequest", DateTime.Now},
            { "ResetBalanceToZero", DateTime.Now},
            { "Payment速付06", DateTime.Now - TimeSpan.FromMinutes(3)},
            { "Payment速付07", DateTime.Now - TimeSpan.FromMinutes(3)},
            { "Payment速付08", DateTime.Now - TimeSpan.FromMinutes(3)},
            { "第三方超一个月免输入密码", DateTime.Now - TimeSpan.FromMinutes(30)}
        };

        public static Dictionary<string, DateTime> 第三方超一个月免输入密码 = new Dictionary<string, DateTime>() {};

        private static Dictionary<string, string> _payplatformdict;
        private static DateTime _payplatformdict_lastquery;
        public static Dictionary<string, string> PayPlatformDict
        {
            get
            {
                if (_payplatformdict != null)
                {
                    if((DateTime.Now - _payplatformdict_lastquery).TotalSeconds < 60)
                        return _payplatformdict;
                }
                _payplatformdict_lastquery = DateTime.Now;
                // 创建DAPPER TEMPLATE 
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("" +
                    "SELECT id,name FROM ofThirdpay WITH(NOLOCK) ORDER BY id");

                using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
                {
                    var myDictionary = connection.Query(template.RawSql)
                        .ToDictionary(x => (string)x.id.ToString(), x => (string)x.name);

                    if (!myDictionary.ContainsKey("10277")) myDictionary.Add("10277", "和平-支付宝");
                    if (!myDictionary.ContainsKey("10278")) myDictionary.Add("10278", "和平-微信");

                    _payplatformdict = myDictionary;
                    _payplatformdict.Add("ATM汇款", "ATM汇款");
                    _payplatformdict.Add("网银转账", "网银转账");
                    _payplatformdict.Add("支付宝转账", "支付宝转账");
                    _payplatformdict.Add("ATM自动", "ATM自动");
                    return _payplatformdict;
                }
            }
        }



        private static Dictionary<int, string> _gamecodedict;
        public static Dictionary<int, string> GameCodeDict
        {
            get
            {
                #region delete
                //if (_gamecodedict != null) 
                //    return _gamecodedict;
                #endregion
                var querySql = string.Format(@"SELECT gameid,gamename2 FROM ofGameClass WITH(NOLOCK) where currencyTypes like '%{0}%' ORDER BY id", currencyType);
                // 创建DAPPER TEMPLATE 
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("" + querySql);
                using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
                {
                    var myDictionary = connection.Query(template.RawSql)
                        .ToDictionary(x => (int)x.gameid, x => (string)x.gamename2);

                    _gamecodedict = myDictionary;
                    _gamecodedict.Add(-1, "不存在的游戏类型");
                    _gamecodedict.Add(0, "不存在的游戏类型");
                    _gamecodedict.Add(4042, "这个是优惠的东西");
                    return _gamecodedict;
                }
            }
        }


        private static Dictionary<int, string> _currencytypedict;
        public static Dictionary<int, string> CurrencyTypeDict
        {
            get
            {
                if (_currencytypedict != null) return _currencytypedict;

                // 创建DAPPER TEMPLATE 
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("" +
                    "SELECT currencycode,currencyname FROM ofCurrency WITH(NOLOCK)");
                using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
                {
                    var myDictionary = connection.Query(template.RawSql)
                        .ToDictionary(x => (int)x.currencycode, x => (string)x.currencyname);

                    _currencytypedict = myDictionary;
                    return _currencytypedict;
                }
            }
        }




        private static Dictionary<int, string> _bonusPromoAgainst;
        public static Dictionary<int, string> BonusPromoAgainstDict
        {
            get
            {
                if (_bonusPromoAgainst != null) return _bonusPromoAgainst;

                // 创建DAPPER TEMPLATE 
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("" +
                    "SELECT * FROM ofBonusPromoAgainst WITH(NOLOCK) ORDER BY id");
                using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
                {
                    var table = connection.Query(template.RawSql);
                    //.ToDictionary(x => (int)x.bonusPromoCode, x => (string)x.bonusPromoName);
                    var myDictionary = new Dictionary<int, string>();
                    foreach (var item in table)
                        if (!myDictionary.ContainsKey(item.bonusPromoCode))
                            myDictionary.Add(item.bonusPromoCode, item.bonusPromoName);
                    if (!myDictionary.ContainsKey(0)) myDictionary.Add(0, "其他");
                    _bonusPromoAgainst = myDictionary;

                    return _bonusPromoAgainst;
                }
            }
        }



        public static Dictionary<int, string> NoticeCodeDict = new Dictionary<int, string>() {
            { -1, "错误"},
            { 0, "普通公告"},
            { 1, "公告"},
            { 2, "合营公告"},
            { 100, "提示公告"}
        };


        public static Dictionary<int, string> NetBankTypeDict = new Dictionary<int, string>() {
            { -1, "未知"},
            { 0, "网银转账"},
            { 1, "ATM汇款"},
            { 2, "支付宝转账"},
        };

        public static Dictionary<int, string> LogTypeDict = new Dictionary<int, string>() {
            {0,"登入成功" },
            {1,"密码错误" },
            {2,"锁住" },
            {3,"自动升级成普通玩家" }};

        public static List<string> PaymentGatewayList = new List<string>() {
            //"JBP",
            //"GPay",
            //"很美",
            //"诚信通",
            "云代付",
            "机构代付",
            "信付代付",
            "普宝盆",
            "捷优代付",
            "信汇",
            "币付",
            "现在代付",
            "MMPB",
            "壹快付",
            "小额代付",
            "汇皇",
            //"OneTech",
            //"乐付",
            //"信付",
            //"OnePay",
            //"速付06",
            //"速付07",
            //"速付08",
            //"众宝",
            //"众宝私",
            "依云付",
            "爱汇",
            "爱汇02",
            "爱汇03",
            "爱汇05",
            //"纯代付",
            //"纯代付02",
            //"纯代付03",
            //"纯代付04",
            //"聚合",
            //"聚合02",
            //"XP",
            //"我来付",
            "壹元",
            "壹元公",
            //"优信付",
            //"天空",
            //"万富",
            "聚宝盆",
            //"速龙普通",
            //"速龙加急",
            //"槟果",
            //"daddy",
            //"四通",
            //"速达",
            //"富通",
            //"易富通",
            //"赢达",
            //"葡萄",
            //"易付",
            //"智联",
            //"通用",
            //"壹支付",
            //"速付",
            //"速付02",
            //"金汇富",

            //"金汇富02"
        };


        public static Dictionary<string, string> IPTranslation = new Dictionary<string, string>() {
            { "222.127.7.178, 103.102.45.87", "PBCOM【ETPI线】"},
            { "222.127.7.178", "PBCOM【ETPI线】"},
            { "180.232.108.130, 103.102.45.87", "PBCOM【GLOBE线】"},
            { "180.232.108.130", "PBCOM【GLOBE线】"},
            { "61.221.26.51, 103.102.45.87", "客服台湾"},
            { "61.221.26.51", "客服台湾"},
            { "207.46.140.177, 103.102.45.87", "VPN [Azure]"},
            { "207.46.140.177", "VPN [Azure]"},
            { "119.9.119.147", "VPN [Rack]"},
            { "103.125.216.250, 103.102.45.87", "VPN [VPSSVR-Japan"},
        };






        private static Dictionary<string, string> _gameBalanceTransferDict;
        public static Dictionary<string, string> GameBalanceTransferDict
        {
            get
            {
                if (_gameBalanceTransferDict != null) return _gameBalanceTransferDict;

                // 创建DAPPER TEMPLATE 
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("" +
                    "SELECT CONCAT(plat,SUBSTRING(LTRIM(STR(gameid)),0,2)) AS gamename, gameid " +
                    "FROM ofGameClass WITH(NOLOCK) " +
                    "WHERE gameid>2000 and class = 0 AND plat IS NOT NULL");

                using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
                {
                    var table = connection.Query(template.RawSql);
                    //.ToDictionary(x => (int)x.bonusPromoCode, x => (string)x.bonusPromoName);
                    var myDictionary = new Dictionary<string, string>();
                    foreach (var item in table)
                        if (!myDictionary.ContainsKey(item.gamename))
                            myDictionary.Add("" + item.gamename, "" + item.gameid);
                    _gameBalanceTransferDict = myDictionary;

                    return _gameBalanceTransferDict;
                }
            }
        }
        public static string gameBalanceTransfer(string gameId, int direction)
        {

            string x = string.Concat(gameId, direction == 0 ? "2" : "3");

            if (GameBalanceTransferDict.ContainsKey(x))
                return GameBalanceTransferDict[x];
            return "";
            //switch (gameId)
            //{
            //    case "EA": transferId = direction == 0 ? "2026" : "3026"; break;
            //    case "AG": transferId = direction == 0 ? "2037" : "3037"; break;
            //    case "BBIN": transferId = direction == 0 ? "2032" : "3032"; break;
            //    case "PT": transferId = direction == 0 ? "2035" : "3035"; break;
            //    case "SPB": transferId = direction == 0 ? "2033" : "3033"; break;
            //    case "188": transferId = direction == 0 ? "2033" : "3033"; break;
            //    case "TT": transferId = direction == 0 ? "2034" : "3034"; break;
            //    case "TTG": transferId = direction == 0 ? "2034" : "3034"; break;
            //    case "BSG": transferId = direction == 0 ? "2039" : "3039"; break;
            //    case "SSC": transferId = direction == 0 ? "2038" : "3038"; break;
            //    case "KG": transferId = direction == 0 ? "2029" : "3029"; break;
            //    case "KG2": transferId = direction == 0 ? "2040" : "3040"; break;
            //    case "UED": transferId = direction == 0 ? "2036" : "3036"; break;
            //    case "LB": transferId = direction == 0 ? "2036" : "3036"; break;
            //    case "VR": transferId = direction == 0 ? "2041" : "3041"; break;
            //    case "XJ": transferId = direction == 0 ? "2042" : "3042"; break;
            //    case "PG": transferId = direction == 0 ? "2043" : "3043"; break;
            //    case "SPT": transferId = direction == 0 ? "2044" : "3044"; break;
            //    case "YSB": transferId = direction == 0 ? "2045" : "3045"; break;
            //    case "GG": transferId = direction == 0 ? "2046" : "3046"; break;
            //    case "IMES": transferId = direction == 0 ? "2047" : "3047"; break;
            //    case "DSCP": transferId = direction == 0 ? "2048" : "3048"; break;
            //    case "DSES": transferId = direction == 0 ? "2049" : "3049"; break;
            //    case "OB": transferId = direction == 0 ? "2050" : "3050"; break;
            //    case "RG": transferId = direction == 0 ? "2051" : "3051"; break;
            //    case "KY": transferId = direction == 0 ? "2052" : "3052"; break;
            //    case "BG": transferId = direction == 0 ? "2053" : "3053"; break;
            //    case "EB": transferId = direction == 0 ? "2054" : "3054"; break;
            //    case "TD": transferId = direction == 0 ? "2055" : "3055"; break;
            //    case "TF": transferId = direction == 0 ? "2056" : "3056"; break;
            //    case "AW": transferId = direction == 0 ? "2057" : "3057"; break;
            //    case "CQ": transferId = direction == 0 ? "2058" : "3058"; break;
            //    case "KG3": transferId = direction == 0 ? "2059" : "3059"; break;
            //}
            //return transferId;
        }




        public static Dictionary<int, string> MessageTypeDict = new Dictionary<int, string>() {
            { 0, "留言回复"},
            { 1, "优惠活动"},
            { 2, "游戏比赛"},
            { 3, "紧急通知"},
            { 4, "其他"},
            { 5, "SMS短信"},
            { 6, "留言回复"}
        };

        public static Dictionary<int, string> ValidateCodeTypeDict = new Dictionary<int, string>() {
            { 0, "手机"},
            { 1, "邮箱"},
            { 2, "3"},
            { 3, "绑定卡"},
            { 4, "重置密码"},
            { 5, "手机验证优惠"},
            { 6, "6"},
            { 7, "7"},
            { 10, "注册验证"},
            { 16, "会员回归手机验证"},
            { 15, "登录验证码"},
            { 100, "提示会员确认到账" },
            { 101, "设定提款密码" },
            { 102, "更改提款密码" }
        };


        public static Dictionary<string, string> OtherRoutes = new Dictionary<string, string>()
            {
                {"/api/Withdraw/NoSearchDateLimit","不限制日期范围"},
                {"/api/Account/ShowNoDepositPhone","显示未存款手机【*】"},
                {"/api/Account/AllowToSeePhone","显示会员手机【*】"},
                {"/api/Withdraw/HidesBankCard","隐私银行卡"},
                {"/api/Account/AllowToSeeEmail","显示会员邮箱【*】"},
                {"/api/Account/AllowToSeeVerifyCode","显示手机验证码"},
                {"/api/Account/AllowToSeeWalletAddress","显示会员钱包地址【*】"},
                {"/api/Call/AllowQuerySalerSelf","查询电销自身会员"},
                {"/api/Agent/PlayersReportList","查询会员报表"},
                {"/api/Agent/LoadAgentPlayer","会员记录查询 "},
                {"/api/Agent/LoadAgentList","合营注册查询"},
                {"/api/Agent/LoadAgentById","合营详情查看"},
                {"/api/Agent/AgentUpdateBaseInfo","合营账号信息修改"},
                {"/api/Agent/UpdateAgentBillP","合营设置修改"},
                {"/api/Agent/AgentUpdatePassword","合营密码修改"},
                {"/api/Agent/UpdateRiskAff","修改合营风控状态"},
                {"/api/Agent/AgentVerify","合营审核"},
                {"/api/Agent/AgentDisableOrEnable","合营启用或停用"},
                {"/api/Agent/QueryAgentData","推广报表查询"},
                {"/api/Agent/FinanceReportList","合营财务报表查询"},
                { "/api/Admin/QueryAgent","管理员查直属代理"},
                {"/api/agent/FinanceReportListByVerifyPerson", "合营财务报表列表【自审核的代理】" }, //20210125 ching 新增

                {"/api/Agent/AgentAutoBillDailyNoLevelRecords","合营日佣金报表" },  //20200726 ching 新增
                {"/api/Agent/CheckFinanceReportListRight","查看代理信息【非自审核代理权限】" },  //20210125 ching 新增

                {"/api/Agent/RemoveUnSettleAmount","未结算金额清零" },  //20200826 ching 新增
                {"/api/Agent/AgentReSettle","代理重新结算" },//20201107 ching 新增
                {"/api/agent/AgentBetWinlostByGame", "代理游戏数据详情" }, //20201105 ching 新增
                {"/api/agent/LoadAgentBindBankInfo", "获取代理已绑银行" }, //20201111 ching 新增
                {"/api/agent/LoadAgentBindBankInfoByCard", "获取代理单卡信息" }, //20201111 ching 新增
                {"/api/agent/BindCard", "代理绑定银行卡信息" }, //20201111 ching 新增
                {"/api/agent/UpdateBindCard", "代理更新银行卡信息" }, //20201111 ching 新增
                {"/api/agent/RemoveBindCard", "代理移除银行卡信息" }, //20201111 ching 新增
                {"/api/agent/QueryShowBindCard", "查看代理已绑银行卡信息" }, //20201111 ching 新增
                {"/api/Agent/PlayersFinanceReportList","合营财务报表直属会员查询"},
                {"/api/Agent/GameBetWinlostList","合营游戏数据查询"},
                {"/api/Agent/PlayerGameBetWinlostList","合营游戏数据直属会员查询"},
                {"/api/Agent/AgentBillNoLevelRecords","佣金结算记录查询"},
                {"/api/Agent/CheckAgentBillNoLevelRecords","佣金结算记录查询【非自审核代理权限】"},//20210410 ching 新增
                {"/api/Agent/CreateAgentBill","佣金结算创建"},
                {"/api/Export/LABILL","佣金结算导出"},
                {"/api/Agent/LoadAgentBillById","佣金结算详情"},
                {"/api/Agent/UpdateAgentMonthSettle","佣金结算编辑"},
                {"/api/Agent/CancelAgentBill","佣金结算取消"},
                {"/api/Agent/AgentBillVerify","佣金结算审核"},
                {"/api/Agent/AgentWithdrawRecords","提款记录查询"},
                { "/api/Agent/AgentWithdrawRecordsAll","查看所有代理提款记录"},//20210908---ching ----add  
                {"/api/Agent/AgentWithdrawRecordsAccess","提款记录查询【密码查询2月前】"},
                {"/api/Agent/CancelAgentWithdraw","提款取消"},
                {"/api/Agent/VerifyAgentWithdraw","提款审核"},
                {"/api/Agent/PayAgentWithdraw","提款支付"},
                {"/api/Bonus/BonusRecords","优惠红利记录查询"},
                {"/api/Bonus/CreateBonus","优惠红利新增"},
                {"/api/Bonus/CreateBonusBat","优惠红利导入"},
                {"/api/Bonus/LoadBonusById","优惠红利详情"},
                {"/api/Bonus/UpdateBonus","优惠红利编辑"},
                {"/api/Bonus/CancelBonus","优惠红利取消"},
                {"/api/Bonus/VerifyBonus","优惠红利审核"},
                {"/api/TurnOver/TurnOverRecords","返水记录查询"},
                {"/api/TurnOver/CreateTurnOver","返水记录新增"},
                {"/api/TurnOver/VerifyTurnOver","返水审核"},
                {"/api/TurnOver/CancelTurnOver","返水取消"},
                {"/api/Export/LATURNOVER","返水导出"},
                {"/api/Resize/ResizeRecords","微调记录查询"},
                {"/api/Resize/CreateResize","微调新增"},
                {"/api/Resize/CancelResize","微调取消"},
                {"/api/Resize/VerifyResize","微调审核"},
                {"/api/Promo/PromoMiscRecords","积分记录查询"},
                {"/api/Promo/CreatePromoMisc","积分新增"},
                {"/api/Promo/CancelPromoMisc","积分取消"},
                {"/api/Promo/VerifyPromoMisc","积分审核"},
                {"/api/Promo/SpreadRecords","推荐记录查询"},
                {"/api/Promo/SpreadBilledRecords","推荐结算记录查询"},
                {"/api/Promo/SpreadBilledRecordsINTL","推荐结算记录查询(INTL)"},
                {"/api/Promo/CancelSpreadBilled","推荐结算取消"},
                {"/api/Promo/VerifySpreadBilled","推荐结算审核"},
                {"/api/Promo/ECupBetRecords","欧洲杯竞猜数据"},

                {"/api/Export/LAPFD","合营财务报表导出"},
                {"/api/Export/LAPGD","会员游戏数据导出"},
                {"/api/Export/LAGD","代理游戏数据导出"},
                {"/api/Export/LAWDF","提款导出"},
                {"/api/Agent/PlayerBetWinlostByGame","会员游戏数据详情" },
                {"/api/Agent/LoadPlayersRegister","注册会员人数查询" },
                { "/api/Agent/LoadFisrtDepositPlayers","首存人数查询"},
                { "/api/Admin/ModifyVeriedAgent","管理员修改已审核代理"},
                { "/api/Admin/ViewVeriedAgentBaseInfo","管理员查看已审核代理基本信息"},
                {"/api/System/OldCMSAccess","旧后台权限"},
                { "/api/Admin/EditAgentAdlinks","管理员编辑代理推广链接"},
                { "/api/bet/QueryUserBetLog","查询账号平台注单"},
                { "/api/Agent/AccessBankCardNo","查看显示提款卡号"},
                { "/api/Agent/LoadPointsRecords","查看代理账变记录"},

                { "/api/Agent/ResizeAgentRecords","查看代理微调记录"},
                { "/api/Agent/CreateAgentResize","创建代理微调"},
                { "/api/Agent/VerifyAgentResize","审核微调记录"},
                { "/api/Agent/CancelAgentResize","取消微调记录"},

                { "/api/Agent/QueryResizeRecordsAll","查看所有代理微调记录"},//20210615---ching ----add  
                { "/api/Agent/CreateResizeAll","创建所有代理微调"},//20210615---ching ----add
                { "/api/Agent/VerifyResizeAll","审核所有代理微调记录"},//20210615---ching ----add
                { "/api/Agent/CancelResizeAll","取消所有代理微调记录"},//20210615---ching ----add

                {"/api/Agent/BonusAgentRecords","代理优惠红利记录查询"},
                { "/api/Agent/CreateAgentBonus","创建代理优惠红利"},
                { "/api/Agent/CancelAgentBonus","取消代理优惠红利"},
                { "/api/Agent/VerifyAgentBonus","审核代理优惠红利"},
                { "/api/Agent/UpdateAgentBonus","更新代理优惠红利"},

                {"/api/Agent/PlayerBonusRecords","下线会员优惠红利记录查询"},
                { "/api/Agent/CreatePlayerBonus","创建下线会员优惠红利"},
                {"/api/Agent/LoadPlayerBonusById","下线会员优惠红利详情"},
                { "/api/Agent/UpdatePlayerBonus","更新下线会员优惠红利"},
                { "/api/Agent/VerifyPlayerBonus","审核下线会员优惠红利"},
                { "/api/Agent/CancelPlayerBonus","取消下线会员优惠红利"},

                { "/api/Admin/QueryCMSIndexAgentReport","首页直属代理统计数据"},
                {"/api/Agent/CheckAllPlayerBonusRecords","下线会员优惠红利查询【非自审核代理下线会员权限】"},

                {"/api/Agent/DisableMids","关闭代理指定商户"},//20221016 ching 新增
                {"/api/Agent/EnableMids","开启代理商户"},//20221016 ching 新增

            };

    }
}
