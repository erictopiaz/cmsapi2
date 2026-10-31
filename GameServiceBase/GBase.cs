using DBLibs.WXDapper;
using LogLib.Helper;
using ShareEntity.InputModels.Game;
using ShareEntity.Models;
using ShareEntity.SqlCommandText;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.RequestHelper;

namespace GameServiceBase
{
    public class GBase
    {
        public static readonly Dictionary<string, int> gameTransIDS_AwcNew = new Dictionary<string, int>()
        {
            //{"DB", 173 },
             #region 体育
             {"OB", 153 },
             {"CR", 169 },
             {"OBCV", 166 },
             {"IMSP", 167 },
             {"OBG", 165 },
             {"PM", 165 }, //前身为 OBG
             {"FB", 171 },
             {"BTI", 180 },
            { "XJ", 145},
            #endregion
            
            {"LB",139 },
            {"TTG", 137 },
            {"BBIN", 134 },
            {"BG",156},
            {"KG3", 162},
            {"VR", 144 },

            {"TF",159 },
            {"IMES",150 },

            {"PG", 146 },
            {"JDB", 164 },

            {"KY",155 },
            {"AW", 160 },
            //{"AG",140 }  //去 vin 那
        };



        /// <summary>
        /// 获取动太操作类对象
        /// </summary>
        /// <param name="account"></param>
        /// <param name="gameid"></param>
        /// <param name="writeToFile"></param>
        /// <param name="lang"></param>
        /// <param name="gameName"></param>
        /// <param name="ipaddress"></param>
        /// <returns></returns>
        public static BaseGame GetDynamicObjectByGameId(
            string account,
            int gameid,
            int lang,
            WriteToFile writeToFile,
            string gameName = "",
            string ipaddress = "127.0.0.1")
        {
            BaseGame gameBase = null;
            switch (gameName.ToUpper())
            {
                case "TT":
                case "TTG": { gameBase = new GameLibs.TTG.TTG(writeToFile); } break;//全
                case "BBIN": { gameBase = new GameLibs.BB.BB(writeToFile); } break;//全
                case "OB": { gameBase = new OB(writeToFile); } break;//全
                case "AG": { gameBase = new GameLibs.AG.AG(writeToFile); } break;//全
                case "KG3": { gameBase = new KG3(writeToFile); } break; //全
                case "LB": { gameBase = new GameLibs.LB.LB(writeToFile); } break;//全
                case "PG": { gameBase = new GameLibs.PG.PG(writeToFile); } break;//全
                case "KY": { gameBase = new GameLibs.KY.KY(writeToFile); } break;//全
                case "VR": { gameBase = new GameLibs.VR.VR(writeToFile); } break;//全
                case "IMES": { gameBase = new ESports(writeToFile); } break;//全
                case "BG": { gameBase = new GameLibs.BG.BG(writeToFile); } break;//全
                case "TF": { gameBase = new GameLibs.TF.TF(writeToFile); } break;//全
                case "AW": { gameBase = new GameLibs.AW.AW(writeToFile); } break;
                case "PM": { gameBase = new GameLibs.OBG.OBG(writeToFile); } break;//pm 前身为obg
                case "OBG": { gameBase = new GameLibs.OBG.OBG(writeToFile); } break;
                case "BTI": { gameBase = new GameLibs.BTI.BTI(writeToFile); } break;
                case "OBCV": { gameBase = new OBCV(writeToFile); } break;
                case "IMSP": { gameBase = new IMSport(writeToFile); } break;
                case "CR": { gameBase = new CRGame(writeToFile); } break;
                case "JDB": { gameBase = new GameLibs.JDB.JDB(writeToFile); } break;
                case "FB": { gameBase = new GameLibs.FB.FB(writeToFile); } break;
                case "EVO": { gameBase = new EVO(writeToFile); } break;
                case "DB": { gameBase = new PMCasino(writeToFile); } break; //pm 真人
                case "SPRIBE": { gameBase = new GameLibs.Spribe.Spribe(writeToFile); } break;
                case "9W": { gameBase = new _9W(writeToFile); } break;
                case "TURBO": { gameBase = new GameLibs.Turbo.Turbo(writeToFile); } break;
                default:
                    break;
            }
            if (gameBase != null)
            {
                gameBase.account = account;
                gameBase.gameId = gameid;
                gameBase.ipaddress = ipaddress;
                gameBase.lang = lang;

            }
            return gameBase;
        }
    }






    public enum PLATFORMACTIONS
    {

        GetBalance,
        /// <summary>
        /// from house to platform
        /// </summary>
        TransferIn,
        /// <summary>
        /// from platform to house
        /// </summary>
        TransferOut,
        /// <summary>
        /// website login authentication
        /// </summary>
        WebAuthenticate,
        /// <summary>
        /// desktop software login authentication
        /// </summary>
        ClientAuthenticate
    }

    public class BaseGame
    {
        public static Dictionary<int, QueryGameParamsInputModel> dictGameParam = new Dictionary<int, QueryGameParamsInputModel>();

        public static Dictionary<DateTime, Dictionary<int, QueryGameParamsInputModel>> dictGameParamCache
            = new Dictionary<DateTime, Dictionary<int, QueryGameParamsInputModel>>();

        public static List<IGrouping<string, QueryGameParamsInputModel>> dictMerchantGameParamCache
            = new List<IGrouping<string, QueryGameParamsInputModel>>();

        private static Dictionary<string, string> dictGameType = new Dictionary<string, string>();
        public string apiResponse = "";

        /// <summary>
        /// 获取账号余额
        /// </summary>
        /// <returns></returns>
        public virtual async Task<object> BalanceQueryAsync()
        {
            await Task.Run(() => { });
            return null;
        }
        protected string BuildPostData(Dictionary<string, string> d)
        {
            string s = "";
            for (int i = 0; i < d.Count; i++)
            {
                var item = d.ElementAt(i);
                var key = item.Key;
                var val = item.Value;
                s += String.Format("{0}={1}", key, val);
                if (i != d.Count - 1)
                    s += "&";
            }
            return s;
        }
        /// <summary>
        /// 获取平台token
        /// </summary>
        /// <returns></returns>
        public virtual async Task<string> GetAgentTokenAsync()
        {
            await Task.Run(() => { });
            return null;
        }


        // public abstract float balance { get; }
        /// <summary>
        /// 获取所有游戏平台账号余额
        /// </summary>
        /// <returns></returns>
        public static async Task<object> BalanceAllQueryAsync(IHttpClientFactory clientFactory, string token)
        {
            var list = new List<string>
            {
                "/Balance/AWC",
                "/Balance/AG",
                "/Balance/EA",
                "/Balance/KG",
                "/Balance/KG2",
                "/Balance/LB",
                "/Balance/PT",
                "/Balance/TTG",
                "/Balance/VR",
                "/Balance/XJ",
                "/Balance/PG",
                "/Balance/YSB",
                "/Balance/GG",
                "/Balance/IMES",
                "/Balance/BBIN",
                "/Balance/OB",
                "/Balance/RG",
                "/Balance/KY",
                "/Balance/BG",
                "/Balance/EB",
                "/Balance/TD",
                "/Balance/TF",
                "/Balance/AW",
                "/Balance/CQ"
            };
            var currentTime = DateTime.UtcNow.AddHours(8);
            var tasks = list.Select(item => GetBalanceAsync(item, clientFactory, token));
            var results = await Task.WhenAll(tasks);
            var resultCollection = new List<BalanceResponseModel>();
            foreach (var r in results)
            {
                if (!string.IsNullOrEmpty(r))
                {
                    var br = Newtonsoft.Json.JsonConvert.DeserializeObject<BalanceResponseModel>(r);
                    resultCollection.Add(br);
                }
            }
            return resultCollection;
        }
        private static async Task<string> GetBalanceAsync(string path, IHttpClientFactory clientFactory, string token)
        {
            var jsonString = await clientFactory.GetAsync(token, "https://apps.awccbk.com", path);
            return jsonString;
        }


        public static async Task<object> BalanceAllPostAsync(IHttpClientFactory clientFactory, string account)
        {
            var list = new List<string>
            {
                "AWC",
                "AG",
                "EA",
                "KG",
                "KG2",
                "LB",
                "PT",
                "TTG",
                "VR",
                "XJ",
                "PG",
                "YSB",
                "GG",
                "IMES",
                "BBIN",
                "OB",
                "RG",
                "KY",
                "BG",
                "EB",
                "TD",
                "TF",
                "AW",
                "CQ"
            };
            var currentTime = DateTime.UtcNow.AddHours(8);
            var tasks = list.Select(item => PostBalanceAsync(item, clientFactory, account));
            var results = await Task.WhenAll(tasks);
            var resultCollection = new List<BalanceModel>();
            foreach (var r in results)
            {
                if (!string.IsNullOrEmpty(r))
                {
                    var br = Newtonsoft.Json.JsonConvert.DeserializeObject<BalanceModel>(r);
                    resultCollection.Add(br);
                }
            }
            return resultCollection;
        }
        public static async Task<object> BalanceAllGetAsync(IHttpClientFactory clientFactory, string account)
        {
            var list = new List<string>
            {
                "AWC",
                "AG",
                "LB",
                "PT",
                "TTG",
                "VR",
                "XJ",
                "PG",
                "IMES",
                "BBIN",
                "OB",
                "KY",
                "BG",
                "EB",
                "TD",
                "TF",
                "AW",
                "CQ",
                "KG3"
            };
            var usingList = new List<string>();
            var querySql = string.Format(GameSqlCommandText.QueryTransfePlatSql, account);
            var result = await DapperBase.QueryAsync<string>(querySql, null,
                    commandTimeout: 1500, ConnectionString: DapperStringBase.ConnectionString);
            foreach (var p in result)
            {
                if (list.Contains(p))
                    usingList.Add(p);
            }
            if (result.Count > 0)
                result.Add("AWC");
            var currentTime = DateTime.UtcNow.AddHours(8);
            var resultCollection = new List<BalanceResponseModel>();
            //var resultCollection = new List<string>();
            var tasks = usingList.Select(item => GetBalanceNewAsync(item, clientFactory, account));
            var results = await Task.WhenAll(tasks);
            foreach (var r in results)
            {
                if (!string.IsNullOrEmpty(r))
                {
                    var br = Newtonsoft.Json.JsonConvert.DeserializeObject<BalanceResponseModel>(r);
                    resultCollection.Add(br);
                    //resultCollection.Add(r);
                }
            }
            return resultCollection;
        }
        private static async Task<string> PostBalanceAsync(string path, IHttpClientFactory clientFactory, string account)
        {

            var postData = new { account = account, gameid = 153, ipAddress = "127.0.0.1" };

            var jsonString = await clientFactory.PostAsync("https://awcmgt20-game-stg.azurewebsites.net",
                "/api/Game/QueryBalance", postData);
            return jsonString;

        }

        private static async Task<string> GetBalanceNewAsync(string path, IHttpClientFactory clientFactory, string account)
        {
            var urlPath = string.Format(@"/Balance/NoAuth/{0}?account={1}", path, account);
            var currentTime = DateTime.UtcNow.AddHours(8);
            var jsonString = string.Empty;
           
            {
                jsonString = await clientFactory.GetAsync("1212", "http://awc-webapp-staging-18bf-swap.azurewebsites.net",
                    urlPath);
                var m = Newtonsoft.Json.JsonConvert.DeserializeObject<BalanceResponseModel>(jsonString);
                m.data.utime = DateTime.UtcNow.AddHours(8).Subtract(currentTime).TotalMilliseconds.ToString();
                jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(m);

            }

            return jsonString; // string.Format(@"{0}", jsonString, ts.TotalMilliseconds);
        }

        /// <summary>
        /// 创建账号
        /// </summary>
        /// <param name="lang">语系 : 1 英文, 2 简体中文,3 繁体中文,4 泰文,5 越南文</param>
        /// <returns></returns>
        public virtual async Task<bool> CreateAccountAsync(int lang = 2)
        {
            await Task.Run(() => { });
            return true;
        }
        /// <summary>
        /// 创建账号
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> CreateAccountAsync()
        {
            await Task.Run(() => { });
            return true;
        }

        /// <summary>
        /// 创建账号,返回结果和原因
        /// </summary>
        /// <returns></returns>
        public virtual async Task<Tuple<bool, string>> CreateAccountResultAsync()
        {
            await Task.Run(() => { });
            return new Tuple<bool, string>(true, "");
        }
        /// <summary>
        /// 创建账号,返回结果和原因
        /// </summary>
        /// <param name="lang">语系 : 1 英文, 2 简体中文,3 繁体中文,4 泰文,5 越南文</param>
        /// <returns></returns>
        //public virtual async Task<Tuple<bool, string>> CreateAccountResultAsync(int lang = 2)
        //{
        //    await Task.Run(() => { });
        //    return new Tuple<bool, string>(true, "");
        //}

        /// <summary>
        /// 存款到游戏
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<bool> TransferDepositAsync(string amount, string trf_id)//GameAccountOperationModel model)
        {
            await Task.Run(() => { });
            return true;
        }

        /// <summary>
        /// 存款到游戏 (false, 1)退还金额
        /// 转账时会有多状态操作,方便确认要不要给客人退回余额
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<Tuple<bool, int>> TransferDepositManyStatusAsync(string amount, string trf_id)//GameAccountOperationModel model)
        {
            await Task.Run(() => { });
            return new Tuple<bool, int>(true, -1);
        }
        /// <summary>
        /// 存款到游戏 (false, 1)退还金额, 金额字串
        /// 转账时会有多状态操作,方便确认要不要给客人退回余额
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<TransOutResultModel> TransferDepositManyStatusVAsync(string amount, string trf_id)//GameAccountOperationModel model)
        {
            await Task.Run(() => { });
            return new TransOutResultModel();
        }


        /// <summary>
        /// 获取转入详情，返回转账前后余额 
        /// pg 有直接返回平台操作前后金额
        /// </summary>
        /// <param name="trf_id"></param>
        /// <param name="trans_status">转账状态是否成功，0：失败，1：成功</param>
        /// <param name="amount">转账金额, 如果平台转账定单可以查询到转账前后金额，这个金额无效，按平台定单查询为准</param>
        /// <param name="balanceAmountBefore">游戏平台返回的操作前余额</param>
        /// <param name="balanceAmount">游戏平台返回的操作后余额</param>
        /// <returns></returns>
        public virtual async Task<TransOutResultModel> TransferInDetailAsync(string trf_id, int trans_status,
            float amount, float balanceAmountBefore, float balanceAmount)
        {
            await Task.Run(() => { });
            return new TransOutResultModel();
        }

        /// <summary>
        /// 获取转出详情，返回转账前后余额   
        /// pg 有直接返回平台操作前后金额
        /// </summary>
        /// <param name="amount">转账金额</param>
        /// <param name="trf_id"></param>
        /// <param name="trans_status">转账状态是否成功，0：失败，1：成功</param>
        /// <param name="insertSql">左转前后数据脚本</param>
        /// <param name="pas">左转前后数据脚本参数</param>
        /// <param name="balanceAmountBefore">游戏平台返回的操作前余额</param>
        /// <param name="balanceAmount">游戏平台返回的操作后余额</param>
        /// <returns></returns>
        public virtual async Task<TransOutResultModel> TransferOutDetailAsync(float amount, string trf_id, int trans_status, string insertSql,
            TransferCreateModel pas, float balanceAmountBefore, float balanceAmount)
        {
            await Task.Run(() => { });
            return new TransOutResultModel();
        }


        /// <summary>
        /// 从游戏取款
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<bool> TransferWithdrawAsync(string amount, string trf_id)//GameAccountOperationModel model)
        {
            await Task.Run(() => { });
            return true;
        }

        /// <summary>
        /// 从游戏取款 (true, 0) 时给客人加点
        /// 转账时会有多状态操作，方便确认要不要给客人退回余额
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<Tuple<bool, int>> TransferWithdrawManyStatusAsync(string amount, string trf_id)//GameAccountOperationModel model)
        {
            await Task.Run(() => { });
            return new Tuple<bool, int>(true, -1);
        }

        /// <summary>
        /// 从游戏取款 (true, 0) 时给客人加点
        /// 转账时会有多状态操作，方便确认要不要给客人退回余额
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<TransOutResultModel> TransferWithdrawManyStatusVAsync(string amount, string trf_id)//GameAccountOperationModel model)
        {
            await Task.Run(() => { });
            return new TransOutResultModel();
        }
        public virtual async Task<object> LoginAsync(GameLoginBaseInputModel inputLoginModel)
        {
            await Task.Run(() => { });
            return string.Empty;
        }
        public virtual async Task<object> GetWidgetUrl(OBWidgetUrlInputModel inputLoginModel)
        {
            await Task.Run(() => { });
            return string.Empty;
        }
        public virtual async Task<object> LoginByGameIdAsync(GameLoginBaseInputModel inputLoginModel)
        {
            await Task.Run(() => { });
            return string.Empty;
        }
        /// <summary>
        /// 游戏大厅登录
        /// </summary>
        /// <param name="inputLoginModel"></param>
        /// <returns></returns>
        public virtual async Task<object> LoginLobbyAsync(GameLoginBaseInputModel inputLoginModel)
        {
            await Task.Run(() => { });
            return string.Empty;
        }
        public virtual async Task<object> LoginAsyncV
            (GameLoginBaseInputModel inputLoginModel)
        {
            await Task.Run(() => { });
            return string.Empty;
        }
        public virtual async Task<object> LoginAppAsync
            (GameLoginBaseInputModel inputLoginModel)
        {
            await Task.Run(() => { });
            return string.Empty;
        }
        public virtual async Task<object> LoginAppRefreshAsync
           (GameLoginBaseInputModel inputLoginModel)
        {
            await Task.Run(() => { });
            return string.Empty;
        }
        public virtual async Task<object> LoginAppAsync
            (GameLoginBaseInputModel inputLoginModel, IHttpClientFactory clientFactory)
        {
            await Task.Run(() => { });
            return string.Empty;
        }
        public virtual async Task<object> UpdatePasswordAsync(UpdateGamePasswordInputModel model)
        {
            await Task.Run(() => { });
            return string.Empty;
        }
        /// <summary>
        /// pc版游戏登录认证
        /// </summary>
        /// <returns></returns>
        //public virtual async Task<object> GameWebLoginCallBack(GameCallBackModel model)
        //{
        //    await Task.Run(() => { });
        //    return string.Empty;
        //}
        public virtual object GameWebLoginCallBack(GameCallBackModel model)
        {
            return string.Empty;
        }


        public string account { set; get; }
        /// <summary>
        /// 品牌:决定游戏参数
        /// </summary>
        public string merchantAccount { set; get; }
        /// <summary>
        /// 会员当前钱包余额
        /// </summary>
        public decimal wallet { set; get; } = 0;
        /// <summary>
        /// 会员账号对应 的id
        /// </summary>
        public long userId { set; get; }
        /// <summary>
        /// 1 英文, 2 简体中文,3 繁体中文,4 泰文,5 越南文, 7 印度
        /// </summary>
        public int lang { set; get; } = 2;
        /// <summary>
        /// 会员货币种类
        /// </summary>
        public int currencyType { set; get; } = 156;
        /// <summary>
        /// 1:全额提取，0：指定金额
        /// 目前仅 9W 使用
        /// </summary>
        public int withdrawType { set; get; } = 0;
        /// <summary>
        /// 转账金额反校验
        /// </summary>
        public float transferAmount { set; get; } = 0;
        /// <summary>
        /// 转账开始时间
        /// </summary>
        public long startTransferTime { set; get; } = 0;
        public string transfer { set; get; } = "";
        public string agaccount { set; get; } = "";
        public string bbaffaccount { set; get; } = "";
        public string account_id { set; get; }//账号id
        /// <summary>
        /// 游戏余额读取的过期时间, 默认4s
        /// </summary>
        public int balanceTimeOut { set; get; } = 8000;
        public int gameId { set; get; }
        public string ipaddress { set; get; }
        /// <summary>
        /// 默认不使用
        /// </summary>
        public int useProxyUrl { set; get; } = 0;
        /// <summary>
        /// Retreive gamename based on the ID
        /// </summary>
        /// <param name="gameid"></param>
        /// <returns></returns>
        public string GameName(string gameid)
        {
            switch (gameid)
            {
                case "128": return "EA";
                case "129": return "PT";
                case "130": return "SUN馆网站";
                case "100": return "万象城娱乐";
                case "131": return "KG";
                case "134": return "BBIN";
                case "135": return "SPB";
                case "136": return "PT老虎机";
                case "137": return "TT";
                case "138": return "PT";
                case "139": return "UED";
                case "140": return "AG";
                case "141": return "LBSSC";
                case "142": return "BSG";
                case "143": return "KG2";
                case "144": return "VR";
                case "145": return "XJ";
                case "146": return "PG";
                case "147": return "SPT";
                case "148": return "YSB";
                case "149": return "GG";
                case "150": return "IMES";
                case "151": return "DS";
                case "152": return "DSES";
                default: return gameid;
            }
        }
        public struct gamidAndName
        {
            public int gameid;
            public string gamename;
        }
        public async Task<string> GetGameNameForTransfer(string gameid)
        {
            string gamename = string.Empty;
            if (!dictGameType.ContainsKey(gameid))
            {
                var result = await DapperBase.QueryAsync<gamidAndName>(GameSqlCommandText.GetActiveGameidAndName, null,
                    commandTimeout: 1500, ConnectionString: DapperStringBase.ConnectionString);
                if (result.Count > 0)
                {
                    lock (dictGameType)
                    {
                        if (!dictGameType.ContainsKey(gameid))
                        {
                            foreach (var gidn in result)
                            {
                                dictGameType.Add(gidn.gameid.ToString(), gidn.gamename);
                            }
                        }
                    }
                }
            }
            return gamename = dictGameType[gameid] == null ? "" : dictGameType[gameid].ToString();

        }
        /// <summary>
        /// 获取游戏参数
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="agentAccount"></param>
        /// <param name="online">1:正式和测试的游戏都可获取取，0：只能获取到正式环境游戏</param>
        /// <returns></returns>
        public async Task<bool> LoadGameParams_old(string agentAccount = "allwincity", int online = 1)
        {
            if (dictGameParam.ContainsKey(gameId))
            {
                return true;
            }
            else
            {
                var querySql = online == 0 ? GameSqlCommandText.QueryGameParamsSql : GameSqlCommandText.QueryAllGameParamsSql;
                var result = await DapperBase.QueryAsync<QueryGameParamsInputModel>(querySql,
                    new { gameid = gameId, agentAccount = agentAccount },
                    commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                if (result != null && result.Count > 0)
                {

                    lock (dictGameParam)
                    {
                        if (!dictGameParam.ContainsKey(gameId))
                        {
                            dictGameParam.Add(gameId, result.First());
                            return true;
                        }
                    }
                }
                return false;
            }
        }




        public async Task<bool> LoadGameParamsCache(string agentAccount = "allwincity", int online = 1)
        {
            await Task.Delay(1);
            if (dictGameParamCache.Count == 0)
            {
                lock (dictGameParamCache)
                {
                    if (dictGameParamCache.Count > 0)
                        return true;
                    //online = 1;
                    online = 0;
                    var querySql = online == 0 ? GameSqlCommandText.QueryGameParamsSql : GameSqlCommandText.QueryAllGameParamsSql;
                    var result = DapperBase.Query<QueryGameParamsInputModel>(querySql,
                        new { gameid = gameId, agentAccount = agentAccount },
                        commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                    if (result != null && result.Count > 0)
                    {

                        if (dictGameParamCache.Count > 0)
                        {
                            dictGameParamCache.Clear();
                        }
                        var gs = new Dictionary<int, QueryGameParamsInputModel>();
                        foreach (var g in result)
                        {
                            gs.Add(g.gameid, g);
                        }
                        dictGameParamCache.Add(DateTime.UtcNow.AddHours(8), gs);
                        return dictGameParamCache.Count > 0;
                    }
                }

                return false;
            }
            else
            {
                return true;
            }

        }

        /// <summary>
        /// 缓存游戏数据到内存 品牌
        /// </summary>
        /// <param name="agentAccount"></param>
        /// <param name="online"></param>
        /// <returns></returns>
        public async Task<bool> LoadGameParamsCacheV(string agentAccount = "allwincity", int online = 1)
        {
            await Task.Delay(1);
            if (dictMerchantGameParamCache.Count == 0)
            {
                lock (dictMerchantGameParamCache)
                {
                    if (dictMerchantGameParamCache.Count > 0)
                        return true;
                    //online = 1;  //正式+测试
                    online = 0; //正式
                    var querySql = online == 0 ? GameSqlCommandText.QueryMerchantGameParamsSql : GameSqlCommandText.QueryAllMerchantGameParamsSql;
                    var result = DapperBase.Query<QueryGameParamsInputModel>(querySql,
                        new { gameid = gameId, agentAccount = agentAccount },
                        commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                    if (result != null && result.Count > 0)
                    {
                        var groupByYearQuery = (from gp in result
                                                group gp by gp.g_merchantAccounts into newGroup
                                                orderby newGroup.Key
                                                select newGroup).ToList();

                        if (dictMerchantGameParamCache.Count > 0)
                        {
                            dictMerchantGameParamCache.Clear();
                        }
                        dictMerchantGameParamCache.AddRange(groupByYearQuery);
                        return dictMerchantGameParamCache.Count > 0;

                    }
                }

                return false;
            }
            else
            {
                return true;
            }

        }



        /// <summary>
        /// 客人是否被限制游戏
        /// </summary>
        /// <param name="account">会员账号</param>
        /// <param name="gamename">会用小写比较</param>
        /// <returns></returns>
        public static async Task<bool> AllowedGameAsync(string account, string gamename)
        {

            var querySql = string.Format(@"select misc from ofUser with(nolock) where account='{0}' ", account);
            var result = await DapperBase.ExecuteScalarValueAsync(querySql, null,
                    commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

            if (result != null)
            {
                if (result.ToString().Contains("gamedisabledstat"))
                {
                    var m = Newtonsoft.Json.JsonConvert.DeserializeObject<MiscModel>(result.ToString());
                    if (string.IsNullOrEmpty(m.gamedisabledstat))
                        return false;
                    var ps = m.gamedisabledstat.Split('|');
                    foreach (var p in ps)
                    {
                        if (p.ToLower().Equals(gamename.ToLower()))
                            return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 查询开启的游戏列表
        /// </summary>
        /// <param name="account">会员账号</param>
        /// <param name="gamename">会用小写比较</param>
        /// <returns></returns>
        public static async Task<List<ofGameClass>> GetGameList()
        {

            var querySql = string.Format(@"select gameid,gamename2,g_merchantAccounts,currencyTypes from ofGameClassMerchant
where class=1 and useable=1 and online=1 and gameTypeFK>-1");
            var result = await DapperBase.QueryAsync<ofGameClass>(querySql, null,
                    commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

            if (result != null && result.Count > 0)
            {
                return result.ToList();
            }
            else
            {
                return new List<ofGameClass>();
            }
        }




        public bool IsNumber(string str, ref string amount)
        {
            try
            {
                double str_tmp = Convert.ToDouble(str);
                double amount_tmp = Convert.ToDouble(amount);

                if (str_tmp + amount_tmp < 0)
                {
                    return false;
                }
                amount = Convert.ToDouble(str_tmp + amount_tmp).ToString("F2");
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool IsNumber(string str)
        {
            try
            {
                str = Convert.ToDouble(str).ToString();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 向游戏平台存款取消操作
        /// </summary>
        /// <param name="_helper"></param>
        /// <param name="Amount"></param>
        /// <param name="Account"></param>
        /// <param name="data"></param>
        /// <param name="referenceno"></param>
        /// <returns></returns>
        public async Task<bool> CancelTransfer(string Amount, string Account, string data, string referenceno)
        {
            StringBuilder SqlStr_0 = new StringBuilder();
            StringBuilder SqlStr = new StringBuilder();

            SqlStr_0.Length = 0;
            SqlStr_0.AppendFormat(@"update ouaccount set amount=amount+{0} where account='{1}'", Amount,
                Account);
            data = Common.EncryptDES(data, "apgamebk");
            SqlStr.Length = 0;
            SqlStr.AppendFormat(
                @"update gitransfer set status=2,process=1,apidata=apidata+'{0}',endtime=DATEADD(hh,8,GETUTCDATE()),errordesc='{1}' where elementid='{2}'",
                data, "", referenceno);

            var sqlArray = new List<string> { SqlStr_0.ToString(), SqlStr.ToString() };
            var param = new List<object> { null, null };
            return await DapperBase.ExecuteTransctionAsync(sqlArray, param, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
        }

        #region

        /// <summary>
        /// 向游戏平台存款取消操作
        /// </summary>
        /// <param name="Amount"></param>
        /// <param name="Account"></param>
        /// <param name="data"></param>
        /// <param name="referenceno"></param>
        /// <param name="trans_r_status">0:等处理，1：转账完成， 2：转账失败</param>
        /// <returns></returns>
        public async Task<bool> CancelTransferV(float Amount, string Account, string data, string referenceno, int trans_r_status)
        {
            try
            {
                var apidata = string.IsNullOrEmpty(data) ? "" : data.Replace("'", "''");
                var updateAccountSql = string.Format(@"update ofUser set amountactive= amountactive + {0} where account='{1}'", Amount, Account);
                var transferFinisedSql = string.Format(@"INSERT INTO [dbo].[ofTransferFinished]
           ([f_tid],[rightamountbefore],[rightamountafter],[trans_r_status],[responsedata])
     VALUES
           ('{0}', 0, 0, {1}, '{2}')", referenceno, trans_r_status, apidata);

                var sqlArray = new List<string> { updateAccountSql, transferFinisedSql };
                var param = new List<object> { null, null };
                return await DapperBase.ExecuteTransctionAsync(sqlArray, param, commandTimeout: 15000,
                    ConnectionString: DapperStringBase.ConnectionString);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 游戏平台存点， 成功后更新平台的存点前后的金额
        /// </summary>
        /// <param name="rightamountbefore">前余额</param>
        /// <param name="rightamountafter">后余额</param>
        /// <param name="trans_r_status">转账状态</param>
        /// <param name="data">回应数据</param>
        /// <param name="referenceno">和游戏平台关联的业务id</param>
        /// <returns></returns>
        public async Task<bool> UpdateTransferDepositFinishedV(
            float rightamountbefore,
            float rightamountafter,
            int trans_r_status,
            string data,
            string referenceno)
        {
            var transferFinisedSql = string.Format(@"INSERT INTO [dbo].[ofTransferFinished]
           ([f_tid],[rightamountbefore],[rightamountafter],[trans_r_status],[responsedata])
     VALUES
           ('{0}', {1}, {2}, {3}, '{4}')", referenceno, rightamountbefore, rightamountafter, trans_r_status, data);
            var sqlArray = new List<string> { transferFinisedSql };
            var param = new List<object> { null };
            return await DapperBase.ExecuteTransctionAsync(sqlArray, param, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
        }

        /// <summary>
        /// 从游戏取点成功，更新定单状态和客人钱包
        /// </summary>
        /// <param name="amount">提取点数</param>
        /// <param name="leftamountbefore">游戏平台提取点数据前点数</param>
        /// <param name="leftamountafter">游戏平台提取点数后点数</param>
        /// <param name="data">平台api 返回数据</param>
        /// <param name="referenceno">转点记录key</param>
        /// <returns></returns>
        public async Task<bool> UpdateTransferWithdrawFinishedV(
            float amount,
            float leftamountbefore,
            float leftamountafter, string data,
            string referenceno,
            string insertSql,
            object param
            )
        {
            var apidata = string.IsNullOrEmpty(data) ? "" : data.Replace("'", "''");
            var updateAccountSql = string.Format(@"update ofUser set amountactive= amountactive + {0} where account='{1}'", amount, account);

            var transferFinisedSql = string.Format(@"INSERT INTO [dbo].[ofTransferFinished]
           ([f_tid],[rightamountbefore],[rightamountafter],[trans_r_status],[responsedata])
     VALUES
           ('{0}', (select amountactive from ofUser where account='{1}'), (select amountactive-{2} from ofUser where account='{1}'), 1, '{3}')",
           referenceno, account, amount, data);

            var updateTransferCreatedSql = string.Format(@"update ofTransferCreated set leftamountbefore={0}, leftamountafter={1} where tid='{2}'",
                leftamountbefore, leftamountafter, referenceno);

            var sqlArray = new List<string> { updateAccountSql, updateTransferCreatedSql, transferFinisedSql };
            var pas = new List<object> { null, null, null };
            return await DapperBase.ExecuteTransctionAsync(sqlArray, pas, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
        }

        public async Task<bool> UpdateTransferWithdrawFinishedV(
            float amount,
            string data,
            string referenceno,
            int trans_status,
            string insertSql,
            TransferCreateModel param
            )
        {
            var apidata = string.IsNullOrEmpty(data) ? "" : data.Replace("'", "''");
            var updateAccountSql = string.Format(@"update ofUser set amountactive= amountactive + {0} where account='{1}'", amount, account);

            var transferFinisedSql = string.Format(@"INSERT INTO [dbo].[ofTransferFinished]
           ([f_tid],[rightamountbefore],[rightamountafter],[trans_r_status],[responsedata])
     VALUES
           ('{0}', (select amountactive from ofUser where account='{1}'), (select amountactive+{2} from ofUser where account='{1}'), {3}, '{4}')",
           referenceno, account, amount, trans_status, data);

            var sqlArray = new List<string> { transferFinisedSql, updateAccountSql };
            var pas = new List<object> { null, null };
            if (!string.IsNullOrEmpty(insertSql))
            {
                sqlArray.Add(insertSql);
                pas.Add(param);
            }
            return await DapperBase.ExecuteTransctionAsync(sqlArray, pas, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
        }
        #endregion


        /// <summary>
        /// 转账成功后更新游戏定单状态
        /// </summary>
        /// <param name="trf_id"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTransferDepositFinished(string trf_id)
        {
            var SqlStr = string.Format(@"update ofTransferNew set status=1,process=1,apidata=apidata+'{0}',endtime=DATEADD(hh,8,GETUTCDATE()) where elementid='{1}'",
                                  "", trf_id);
            var sqlArray = new List<string> { SqlStr.ToString() };
            var param = new List<object> { null };
            return await DapperBase.ExecuteTransctionAsync(sqlArray, param, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
        }

        /// <summary>
        /// 从游戏取点成功，更新定单状态和客人钱包
        /// </summary>
        /// <param name="trf_id"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTransferWithdrawFinished(string trf_id, string account, string amount)
        {

            var SqlStr = string.Format(@"update ofTransferNew set status=1,process=1,apidata=apidata+'{0}',endtime=DATEADD(hh,8,GETUTCDATE()),rightamountbefore=(select amountactive from ofUser where account='{1}'),rightamountafter=(select amountactive+{2} from ofUser where account='{1}')  where elementid='{3}'",
                "", account, amount, trf_id);
            var SqlStr_0 = string.Format(@"update ofUser set amountactive=amountactive+{0} where account='{1}'", amount, account);
            var sqlArray = new List<string> { SqlStr.ToString(), SqlStr_0.ToString() };
            var param = new List<object> { null, null };
            return await DapperBase.ExecuteTransctionAsync(sqlArray, param, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
        }
        /// <summary>
        /// 从游戏平台取款时，取消当前记录
        /// </summary>
        /// <param name="_helper"></param>
        /// <param name="Amount"></param>
        /// <param name="Account"></param>
        /// <param name="data"></param>
        /// <param name="referenceno"></param>
        /// <returns></returns>
        public async Task<bool> CancelTransferNotReturnAmount(string Amount, string Account, string data, string referenceno)
        {
            StringBuilder SqlStr = new StringBuilder();
            data = Common.EncryptDES(data, "apgamebk");
            SqlStr.Length = 0;
            SqlStr.AppendFormat(
                @"update gitransfer set status=2,process=1,apidata=apidata+'{0}',endtime=DATEADD(hh,8,GETUTCDATE()),errordesc='{1}',rightamountbefore=(select amount from ouAccount where account='{2}'),rightamountafter=(select amount+{3} from ouAccount where account='{2}')  where elementid='{4}'",
                data, "", Account, Amount, referenceno);
            var sqlArray = new List<string> { SqlStr.ToString() };
            var param = new List<object> { null };
            return await DapperBase.ExecuteTransctionAsync(sqlArray, param, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
        }

        public async Task<string> GetPasswordByPassKey(string key)
        {
            var result = await DapperBase.ExecuteScalarValueAsync(GameSqlCommandText.GetPasskey, new { account = account }, null, 15000,
                ConnectionString: DapperStringBase.ConnectionString);
            return result == null ? "" : Common.DecryptDES(result.ToString(), key);
        }

        /// <summary>
        /// 获取当前游戏平台的下注记录
        /// </summary>
        /// <returns></returns>
        public virtual async Task<object> GetGameBetRecordsAsync(GameBetRecordParams model)
        {
            await Task.Run(() => { });
            return string.Empty;
        }

        /// <summary>
        /// 获取当前游戏平台的下注记录 原数据输出
        /// </summary>
        /// <returns></returns>
        public virtual async Task<object> GetGameBetRecordsSRCAsync(GameBetRecordParams model)
        {
            await Task.Run(() => { });
            return string.Empty;
        }


        public virtual object GetGameBetRecords(GameBetRecordParams model)
        {
            return string.Empty;
        }




    }


    public class BalanceResponseModel
    {
        public int code { set; get; }
        public string message { set; get; }
        public Balance data { set; get; }
    }
    public class Balance
    {
        /// <summary>
        /// 游戏平台名
        /// </summary>
        public string type { set; get; }
        /// <summary>
        /// 游戏余额
        /// </summary>
        public decimal balance { set; get; } = 0;
        /// <summary>
        /// 自定义游戏平台名，方便客人确认
        /// </summary>
        public string customerName { set; get; }

        /// <summary>
        /// 响应时间
        /// </summary>
        public string utime { set; get; }

    }


    public class BalanceModel
    {
        public int code { set; get; }
        public string message { set; get; }
        public string data { set; get; }
    }

    public class BalanceResponseNewModel
    {
        public string code { set; get; }
        public string message { set; get; }
        public BalanceNew data { set; get; }
    }
    public class BalanceNew
    {
        public string gameName { set; get; }
        public decimal balance { set; get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class TransOutResultModel
    {
        //bool, int, string, float, float, string
        public bool success { set; get; } = false;
        public int needType { set; get; } = 0;
        public string message { set; get; }
        public float beforeAmount { set; get; } = 0;
        public float afterAmount { set; get; } = 0;
        public string responseData { set; get; }
        /// <summary>
        /// 如果提款时是全额提款
        /// 目前仅 9W 使用
        /// </summary>
        public float withdrawBalance { set; get; } = 0;
    }


    public class TransferCreateModel
    {
        public string currency { set; get; }
        public string account { set; get; }
        public float amount { set; get; }
        public int type { set; get; }
        public string endtime { set; get; }
        public string promotioncode { set; get; }
        public float leftamountbefore { set; get; }
        public float leftamountafter { set; get; }
        public float rightamountbefore { set; get; }
        public float rightamountafter { set; get; }
        public int status { set; get; }
        public string acode { set; get; }
        public string elementid { set; get; }
        public string referrerno { set; get; }
        public string paymentid { set; get; }
        public string errordesc { set; get; }
        public string requestip { set; get; }
        public string requestaddress { set; get; }
        public string referrer { set; get; }
        public string referrerreg { set; get; }
        public int level { set; get; }
        public string referenceguid { set; get; }
        /// <summary>
        /// 转账平台返回结果
        /// </summary>
        public string xmlapi { set; get; }
        public int transtatus { set; get; } = 0;
        public int process { set; get; } = 1;
    }
}
