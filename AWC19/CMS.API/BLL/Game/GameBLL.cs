using Amazon.Runtime.Internal.Transform;
using DBLibs;
using DBLibs.WXDapper;
using GameOrderShare;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using ShareEntity.InputModels.Account;
using ShareEntity.InputModels.BetRecordModel;
using ShareEntity.InputModels.GameConfig;
using ShareEntity.InputModels.GameReport;
using ShareEntity.InputModels.Transfer;
using ShareEntity.Models;
using ShareEntity.OutModels.GameConfig;
using ShareEntity.OutputModels;
using ShareEntity.SqlCommandText;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.JWT;
using UtilsLib.lang;
using UtilsLib.RequestHelper;

namespace CMS.API.BLL.Game
{
    /// <summary>
    /// 游戏参数列表，
    /// </summary>
    public class GameBLL
    {
        /// <summary>
        /// 游戏分类列表查询
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> LoadGameConfig(QueryGameConfigInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = " ofGameClass ";
            string orderBy = " id ";
            string keyName = " id ";

            if (inputModel.gameid > 0 && inputModel.gameid < 10000)
                whereSql = string.Format(@" gameid = {0}", inputModel.gameid);
            if (inputModel.gameclass == 0 || inputModel.gameclass == 1)
                whereSql = string.IsNullOrEmpty(whereSql) ?  string.Format(@" class = {0}", inputModel.gameclass) : string.Format(@" {0} and class = {1}", whereSql, inputModel.gameclass);
            if (!string.IsNullOrEmpty(inputModel.gamename))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" gamename2 = N'{0}'", inputModel.gamename) : string.Format(@" {0} and gamename2 = N'{1}'", whereSql, inputModel.gamename);

            if (inputModel.gamestatus == 0 )
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" gamestatus = {0}", inputModel.gamestatus) : string.Format(@" {0} and gamestatus = {1}", whereSql, inputModel.gamestatus);
            
            if (inputModel.gamestatus == 1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" useable = {0}", inputModel.gamestatus) : string.Format(@" {0} and useable = {1}", whereSql, inputModel.gamestatus);
            
            if(inputModel.gamestatus == 2) //游戏平台加上主钱包
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" (useable = {0} or gameid=100) ", 1) : string.Format(@" {0} and (useable = {1} or gameid=100) ", whereSql, 1);

            if(inputModel.currencyType != 156) {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" currencyTypes like '%356%'") : string.Format(@" {0} and currencyTypes like '%356%'", whereSql);
            }

            var leftjoinString = "";
            var fields = " gameid,gamename2 as gamename ,gamestatus,class as gameclass, params as param,createtime,operator as who_create ";

            return await DapperPaging<QueryGameConfigOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex);
        }
        /// <summary>
        /// 游戏参数列表
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> LoadGameConfigParams(QueryGameConfigInputModel inputModel)
        {
            string whereSql = "";
            var viewName = " ofGameClass ";
            string orderBy = " id ";
            string keyName = " id ";

            whereSql = string.Format(@" class = {0}", inputModel.gameclass);

            if (inputModel.gamestatus == 0 || inputModel.gamestatus == 1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" gamestatus = {0}", inputModel.gamestatus) : string.Format(@" {0} and gamestatus = {1}", whereSql, inputModel.gamestatus);


            var leftjoinString = "";
            var fields = " gameid,gamename,gamestatus, params as param ";
            return await DapperPaging<QueryGameConfigParamsOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex);
        }
        /// <summary>
        /// 更新游戏分类状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<object> UpdateGameConfigStatus(UpdateGameConfigInputModel model)
        {
            var result = await DapperBase.ExecuteAsync(GameSqlCommandText.UpdateGameConfigSql,new { gamestatus = model.gameStatus, id = model.id },
                commandTimeout:15000, ConnectionString:DapperStringBase.ConnectionString);
            return new ApiResultModel<object>
            {
                code = result ? "true" : "false",
                message = result ? "更新游戏分类状态-成功" : "更新游戏分类状态-失败",
                message_code = result ? "update_game_config_status_success" : "update_game_config_status_failed"
            };
        }

        /// <summary>
        /// 游戏盈利报表
        /// 风控使用
        /// </summary>
        /// <param name="model"></param>
        /// <param name="adminAccount"></param>
        /// <returns></returns>

        public static async Task<object> LoadGameReport(QueryGameReportInputModel model, 
            string adminAccount)
        {
            string whereSql = "";
            var viewName = " ofTurnover t ";
            string orderBy = model.sortType == -1 ? "t.begindate desc " : model.sortType == 0 ? " t.winlost asc " : " t.winlost desc";
            string keyName = " t.id ";

            if (!string.IsNullOrEmpty(model.account))
                whereSql = string.Format(@" t.account = '{0}' ", model.account) ;

            if (model.gameId > 0)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" t.gametype = {0}", model.gameId) : string.Format(@" {0} and t.gametype = {1}", whereSql, model.gameId);

            whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" begindate >= '{0}' and begindate<='{1}' ", model.startTime, model.endTime)
                : string.Format(@" {0} and begindate >= '{1}' and begindate<='{2}' ", whereSql, model.startTime, model.endTime);


            var leftjoinString = string.Format(@" left join ofUser u on t.account=u.account
left join ofGameClass g on t.gametype = g.gameid  ");
            var fields = " t.account,t.begindate, g.gamename2,t.totalbets, -t.winlost winlost ,u.createtime, u.macaddress,u.withdrawcards, (withdrawalamount-depositamount) wdamount ";

            var pageModel = await DapperPaging<QueryGameReportOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                    fields, orderBy, keyName, model.pageSize, model.pageIndex);

            ApiResultModel<PageModel<QueryGameReportOutModel>> apiResultModel = pageModel as ApiResultModel<PageModel<QueryGameReportOutModel>>;
            if (await CheckRight.LoadAllowedPathByAccount(adminAccount, "/api/Account/AllowToSeePhone"))
            {
                foreach (var m in apiResultModel.data.modelList)
                {
                    if (isTel(m.account))//是电话
                    {
                        m.account = string.Format(@"{0}****{1}", m.account.Substring(0, 3), m.account.Substring(m.account.Length - 4, 4));
                    }
                }

            }
            if (apiResultModel.code == "true"
                && apiResultModel.data.TotalRecord > 0 ) //获取到分页数据,   结算后的才支统计返回
            {
                //获取统计数据： 

                var tsql = string.Format(@"select sum(t.totalbets) totalbets, sum(-t.winlost) totalWinlost ,sum((withdrawalamount-depositamount)) totalWDamount
from ofTurnover t
left join ofUser u on t.account=u.account
left join ofGameClass g on t.gametype = g.gameid 
where {0} ", whereSql);
                
                var result = await DapperBase.QueryAsync<GameReport>(tsql,
               null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);

                var code = (result != null && result.Count > 0);
                if (code)
                {
                    apiResultModel.data.totalData = result.First();
                }
            }
            return apiResultModel;
        }

        /// <summary>
        /// 获取电子游戏列表
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> LoadGameModels(QueryGameModelsInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = " ofGameSlots ";
            string orderBy = " id desc ";
            string keyName = " id ";

            //if (inputModel.gameid > 0 && inputModel.gameid < 10000)
            //    whereSql = string.Format(@" gameid = {0}", inputModel.gameid);
            //if (inputModel.gameclass == 0 || inputModel.gameclass == 1)
            //    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" class = {0}", inputModel.gameclass) : string.Format(@" {0} and class = {1}", whereSql, inputModel.gameclass);
            //if (!string.IsNullOrEmpty(inputModel.gamename))
            //    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" gamename2 = N'{0}'", inputModel.gamename) : string.Format(@" {0} and gamename2 = N'{1}'", whereSql, inputModel.gamename);

            //if (inputModel.gamestatus == 0)
            //    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" gamestatus = {0}", inputModel.gamestatus) : string.Format(@" {0} and gamestatus = {1}", whereSql, inputModel.gamestatus);

            //if (inputModel.gamestatus == 1)
            //    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" useable = {0}", inputModel.gamestatus) : string.Format(@" {0} and useable = {1}", whereSql, inputModel.gamestatus);

            //if (inputModel.gamestatus == 2) //游戏平台加上主钱包
            //    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" (useable = {0} or gameid=100) ", 1) : string.Format(@" {0} and (useable = {1} or gameid=100) ", whereSql, 1);


            var leftjoinString = "";
            var fields = " * ";

            return await DapperPaging<object>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex);
        }


        /// <summary>
        /// 获取平台注单记录
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="config">获取配置文件信息</param>
        /// <returns></returns>
        public static async Task<ApiResponseModel<PageTupleModel<T,K>>> QueryUserBetRecords<T,K>(QueryUserBetLogInputModel inputModel, 
            IConfiguration config)
        {
            try
            {
                if(inputModel.gameId != 169 )
                {
                    return ApiResponseModel<PageTupleModel<T, K>>.GetResult(0, "未开启平台!", "query_page_failed");
                }
                var token = "";
                //转到游戏平台服务获取
                var betApiToken = GetJWTAutToken(inputModel.account, 4, inputModel.AdminIpAddress,
                        DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"), "/api/bet/PlayersGameRecords",
                        token, jwtKey: config["jwt:jwtKey"], pkeySec: config["jwt:pKeySec"]);
                var send_data = Newtonsoft.Json.JsonConvert.SerializeObject(inputModel);
                var headers = new Dictionary<string, string>()
                {
                    {"Authorization",betApiToken }
                };

                var body = await MRequest.RelayForm("http://awcmgt23-gamebetrecord.azurewebsites.net", "POST", headers, true,
                    send_data, "/api/bet/PlayersGameRecords", "application/json", isAllowedProxy: false);

                return JsonConvert.DeserializeObject<ApiResponseModel<PageTupleModel<T,K>>>(body);
            }
            catch (Exception)
            {
                return new ApiResponseModel<PageTupleModel<T,K>>()
                {
                    code = 0,
                    message = "游戏加载数据",
                    message_code = "query_page_failed",
                    data = new PageTupleModel<T,K>
                    {
                        TotalRecord = 0,
                        TotalPages = 0,
                        PageIndex = 0,
                        modelList = new List<T>()
                    }
                };
            }
        }


        /// <summary>
        /// 获取平台注单记录
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="config">获取配置文件信息</param>
        /// <returns></returns>
        public static async Task<ApiResponseModel<BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalData, PageTotalData>>> QueryUserBetRecordsV(QueryUserBetLogInputModel inputModel,
            IConfiguration config)
        {
            try
            {
            //gameid: 1.PG  2.JL  3.TURBO
            //4.JDB  5.EVO  6.9W  7.Spribe
            //8.FB  9.PA  10.IMES
            //11.IMSP  12.TF  13.LB
            //14.KY  15.XJ  16.DBZ 17.DBC
                var gameIDs = new Dictionary<int, int>()
                {
                    { 171, 8},//fb
                    { 140, 9 },//pa
                    { 150, 10}, //imes
                    { 167, 11 }, //imsp
                    { 159, 12 }, //tf
                    { 139, 13 },// lb
                    { 155, 14 }, // ky
                    { 145, 15 }, //xj
                    { 173, 16 }, //db 真人
                    { 181, 17 }, //dbkeno
                    { 146, 1 }, //pg
                    { 164, 4 }, //jdb
                    { -1, 999 }, //所有平台
                };
                if (!gameIDs.ContainsKey(inputModel.gameId))
                {
                    return new ApiResponseModel<BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalData, PageTotalData>>()
                    {
                        code = 0,
                        message = $"未开启平台",
                        message_code = "query_page_failed",
                        data = new BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalData, PageTotalData>()
                        {
                            message = "",
                            result = new List<BetWithPrefixModel>(),
                            pageInfo = new PageInfo(),
                            totalData = new TotalData(),
                            pageTotalData = new PageTotalData()
                        },
                    };
                }

                if (string.IsNullOrEmpty(inputModel.account) 
                    && string.IsNullOrEmpty(inputModel.parentAccount))
                    return new ApiResponseModel<BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalData, PageTotalData>>()
                    {
                        code = 0,
                        message = $"请输入会员账号或代理账号",
                        message_code = "query_page_failed",
                        data = new BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalData, PageTotalData>()
                        {
                            message = "",
                            result = new List<BetWithPrefixModel>(),
                            pageInfo = new PageInfo(),
                            totalData = new TotalData(),
                            pageTotalData = new PageTotalData()
                        },
                    };


                var currentDate = DateTime.UtcNow.AddHours(8);
                var model = new QueryOrdersByPlatInputModel()
                {
                    gameId = gameIDs[inputModel.gameId], // 999,//所有游戏
                    pageIndex = inputModel.pageIndex,
                    pageSize = inputModel.pageSize,
                    account = inputModel.account, // "boaaching0088",
                    parentAccount = inputModel.parentAccount,
                    settled = inputModel.settled, // -1,
                    currencyType = 156,
                    betId = inputModel.betId,
                    startTime = inputModel.startTime, // currentDate.AddMonths(-1).ToString("yyyy-MM-01 00:00:00"),// "2026-01-01 00:00:00",
                    endTime = Convert.ToDateTime(inputModel.endTime).AddDays(1).ToString("yyyy-MM-dd 00:00:00")// currentDate.AddDays(1).ToString("yyyy-MM-dd 00:00:00") // "2026-01-07 00:00:00"
                };
                var s = await GOrdersBLL.QueryOrdersByPostgrePrefix(inputModel: model, gameId: gameIDs[inputModel.gameId], writeToFile: null);
                var r = s as ApiResultModel<BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalData, PageTotalData>>;
                if (r.rCode == 1
                    && r.data.result != null 
                    && r.data.result.Count > 0)
                {
                    return new ApiResponseModel<BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalData, PageTotalData>>()
                    {
                        code = 1,
                        message = $"查询成功",
                        message_code = "success",
                        data = new BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalData, PageTotalData>()
                        {
                            message = "",
                            result = r.data.result,
                            pageInfo = r.data.pageInfo,
                            totalData = r.data.totalData,
                            pageTotalData = r.data.pageTotalData
                        },
                    };
                }
                else
                {
                    return new ApiResponseModel<BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalData, PageTotalData>>()
                    {
                        code = 0,
                        message = $"无数据",
                        message_code = "not found bet orders",
                        data = new BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalData, PageTotalData>()
                        {
                            message = "",
                            result = new List<BetWithPrefixModel>(),
                            pageInfo = new PageInfo(),
                            totalData = new TotalData(),
                            pageTotalData = new PageTotalData()
                        },
                    };
                }

            }
            catch (Exception)
            {
                return new ApiResponseModel<BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalData, PageTotalData>>()
                {
                    code = 0,
                    message = $"数据处理异常",
                    message_code = "query bet order ex",
                    data = new BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalData, PageTotalData>()
                    {
                        message = "",
                        result = new List<BetWithPrefixModel>(),
                        pageInfo = new PageInfo(),
                        totalData = new TotalData(),
                        pageTotalData = new PageTotalData()
                    },
                };


            }
        }

        /// <summary>
        /// 获取平台 外站游戏 平台账变 
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="token"></param>
        /// <param name="loginKey"></param>
        /// <returns></returns>
        public static async Task<ApiResponseModel<PageModel<GameResizeModel>>> QueryBetAmountResize(QueryUserBetLogInputModel inputModel,
            string token, 
            string loginKey) 
        {
            try {
                string whereSql = string.Empty;
                var viewName = " GameTranferUnionLog a with(nolock) ";
                string orderBy = " a.create_at desc";
                string keyName = " a.actionId ";

                inputModel.adminAccount = JwtHelper.GetName(token, loginKey);
                var currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(token, loginKey);
                if (!currencys.Contains(inputModel.currencyType)) {
                    return new ApiResponseModel<PageModel<GameResizeModel>>() {
                        code = 0,
                        message = "请选择货币查询",
                        message_code = "error_currency_search"
                    };
                }

                var gameIds = new Dictionary<int, string>() {
                    { 176, "9W" },
                    { 175, "Spribe" },
                    { 172, "EVO" },
                    { 164, "JDB" },
                    { 146, "PG" },
                    { 177, "Turbo" },
                    { 178, "JL" }
                };

                var platform = gameIds.ContainsKey(inputModel.gameId) ? gameIds[inputModel.gameId] : "";

                if (!string.IsNullOrEmpty(inputModel.account)) {
                    whereSql = string.Format(@" a.account = '{0}' ", inputModel.account);
                }

               
                if (!string.IsNullOrEmpty(inputModel.startTime)
                    && !string.IsNullOrEmpty(inputModel.endTime)) {
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.create_at >= '{0}' and a.create_at <= '{1}'",
                        inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and a.create_at >= '{1}' and a.create_at <= '{2}'", whereSql, inputModel.startTime, inputModel.endTime);
                }
                else {
                    var currentDay = DateTime.UtcNow.AddHours(8);
                    inputModel.startTime = currentDay.ToString("yyyy-MM-dd 00:00:00");
                    inputModel.endTime = currentDay.ToString("yyyy-MM-dd 23:59:59");
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.create_at >= '{0}' and a.create_at <= '{1}'", inputModel.startTime, inputModel.endTime)
                        : string.Format(@" {0} and a.create_at >= '{1}' and a.create_at <= '{2}'", whereSql, inputModel.startTime, inputModel.endTime);

                }

                if (!string.IsNullOrEmpty(platform))
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.platform = '{0}' ", platform) : string.Format(@" {0} and a.platform = '{1}' ", whereSql, platform);


                var leftjoinString = string.Format(@"");
                var fields = " account , actionName,platform,amount,amountType, create_at ";
                return await DapperPaging<GameResizeModel>.QueryPageV1(whereSql, viewName, leftjoinString,
                        fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, DapperStringBase.ConnectionStringReadOnly);


            }
            catch (Exception) {
                return new ApiResponseModel<PageModel<GameResizeModel>> {
                    code = 0,
                    message = "游戏加载数据",
                    message_code = "query_page_failed"
                };
            }
        }








        private static bool isTel(string phone)
        {
            string[] RxStr = new string[] { "^(11[0-9]{9})$", "^(12[0-9]{9})$", "^(13[0-9]{9})$", "^(14[0-9]{9})$", "^(15[0-9]{9})$", "^(16[0-9]{9})$", "^(17[0-9]{9})$", "^(18[0-9]{9})$", "^(19[0-9]{9})$" };
            int Len = RxStr.Length;
            for (int i = 0; i < Len; i++)
            {
                Regex sRx = new Regex(RxStr[i], RegexOptions.IgnoreCase);
                if (sRx.IsMatch(phone, 0))
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetJWTAutToken(string account, int callfrom, string ipaddress,
           string calltime, string callaction, string loginToken, string jwtKey = "", string pkeySec = "")
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                account = account,
                callfrom = callfrom,
                ipaddress = ipaddress,
                calltime = calltime,
                callaction = callaction,
                pKey = loginToken
            });

            byte[] key = Encoding.UTF8.GetBytes(jwtKey);
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();//加密方式
            IJsonSerializer serializer = new JsonNetSerializer();//序列化Json
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();//base64加解密
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);//JWT编码
            var token = encoder.Encode(new
            {
                awcSign = Common.DESEnCode(json, pkeySec)
            }, key);//生成令牌

            return token;
        }
    }

    public class GameReport
    {
        /// <summary>
        /// 游戏平台的总输赢
        /// </summary>
        public decimal totalWinlost { set; get; }
        /// <summary>
        /// 客人的总提-总存
        /// </summary>
        public decimal totalWDamount { set; get; }
        /// <summary>
        /// 总投注
        /// </summary>
        public decimal totalbets { set; get; }
    }
}
