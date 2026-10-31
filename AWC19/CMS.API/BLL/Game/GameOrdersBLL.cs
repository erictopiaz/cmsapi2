using DBLibs;
using DBLibs.WXDapper;
using LogLib.Helper;
using MongoDB.Driver;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using ShareEntity.InputModels.Account;
using ShareEntity.InputModels.BetRecordModel;
using ShareEntity.InputModels.Game.ttg;
using ShareEntity.Models;
using ShareEntity.OutputModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.JWT;
using UtilsLib.RequestHelper;

namespace CMS.API.BLL.Game {
    public class GameOrdersBLL {

        private static readonly Dictionary<int, int> gameIDs = new Dictionary<int, int>()
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
                    { 183, 18 }, //pp
                    { 182, 22 }, //iaes
                    { -1, 999 }, //所有平台
                };

        private static readonly Dictionary<int, string> gameIDMapToNames = new Dictionary<int, string>()
                {
                    { 171, "FB"},//fb
                    { 140, "PA" },//pa
                    { 150, "IMES"}, //imes
                    { 167, "IMSP" }, //imsp
                    { 159, "TF" }, //tf
                    { 139, "LB" },// lb
                    { 155, "KY" }, // ky
                    { 145, "XJ" }, //xj
                    { 173, "DB" }, //db 真人
                    { 181, "DBKeno" }, //dbkeno
                    { 146, "PG" }, //pg
                    { 164, "JDB" }, //jdb
                    { 176, "9W" },
                    { 175, "Spribe" },
                    { 172, "EVO" },
                    { 177, "Turbo" },
                    { 178, "JL" }
                };

        private static Dictionary<int, ofGameSlotModel> gameSlotModels = new Dictionary<int, ofGameSlotModel>();

        /// <summary>
        /// 拉取 游戏日报表
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="writeToFile"></param>
        /// <param name="mapGameId">gameid 映射, true：映射，false ：否</param>
        /// <returns></returns>
        public static async Task<object> QueryDailyGameReportByPlatAccunt(QueryGameReportByPlatAccuntInputModel inputModel,
            WriteToFile writeToFile,
            bool mapGameId = false)
        {
            var remoteUrl = "";
            var data = "";
            try
            {
                if (mapGameId)
                {
                    inputModel.gameId = gameIDs.ContainsKey(inputModel.gameId) ? gameIDs[inputModel.gameId] : 999;
                }
                if(string.IsNullOrEmpty(inputModel.startTime) || string.IsNullOrEmpty(inputModel.endTime))
                {
                    var currentDate = DateTime.UtcNow.AddHours(8);
                    inputModel.startTime = currentDate.ToString("yyyy-MM-dd 00:00:00");
                    inputModel.endTime = currentDate.ToString("yyyy-MM-dd 00:00:00");
                }

                var endTime = Convert.ToDateTime(inputModel.endTime).AddDays(1);
                if(inputModel.timeZone == 1)
                {
                    //美东时间
                    inputModel.startTime = Convert.ToDateTime(inputModel.startTime).ToString("yyyy-MM-ddTHH:mm:ss-04:00");
                    inputModel.endTime = endTime.ToString("yyyy-MM-ddTHH:mm:ss-04:00");
                }
                else
                {
                    //北京时间
                    inputModel.startTime = Convert.ToDateTime(inputModel.startTime).ToString("yyyy-MM-ddTHH:mm:ss+08:00");
                    inputModel.endTime = endTime.ToString("yyyy-MM-ddTHH:mm:ss+08:00");
                }

                var getString = string.Format(@"PageIndex={0}&PageSize={1}&StartTime={2}&StopTime={3}&UserName={4}&UserParents={5}&GameId={6}&TimeType={7}&TimeZone={8}",
                    inputModel.pageIndex,
                    inputModel.pageSize,
                    Common.UrlEncode(inputModel.startTime),
                    Common.UrlEncode(inputModel.endTime),
                    inputModel.account,
                    inputModel.parentAccount,
                    inputModel.gameId,
                    inputModel.timeType,
                    inputModel.timeZone);

                remoteUrl = string.Format(@"http://103.102.47.69:19071/api/Order/QueryDayReport?{0}", getString);
                data = await MRequest.Relay(remoteUrl, headers: null, "", "",
                    isAllowedProxy: true, ContentType: "application/json; charset=utf-8", method: "get", proxyUrl: "", timeOut: 4000);
                if (writeToFile != null)
                {
                    writeToFile.Done("QueryDailyGameReportByPlatAccunt_request", "QueryDailyGameReportByPlatAccunt ",
                        string.Format(@"remoteUrl = {0}   responseData={1}", remoteUrl, data), true);
                }
                var responseResult = JsonConvert.DeserializeObject<BetResponeModel<List<DailyGameReportModel>, PageInfo, TotalDataV, PageTotalData>>(data);
                if (string.IsNullOrEmpty(responseResult.message))
                {
                    //查询成功
                    if (responseResult.result.Count > 0)
                    {
                        return new ApiResultModel<BetResponeModel<List<DailyGameReportModel>, PageInfo, TotalDataV, PageTotalData>>
                        {
                            code = "true",
                            message = "查询成功",
                            message_code = "found data",
                            data = responseResult,
                        };
                    }
                    else
                    {
                        return new ApiResultModel<BetResponeModel<List<DailyGameReportModel>, PageInfo, TotalDataV, PageTotalData>>
                        {
                            code = "true",
                            message = $"无数据",
                            message_code = "not found data",
                            data = responseResult,
                        };
                    }
                }
                else
                {
                    return new ApiResultModel<BetResponeModel<List<DailyGameReportModel>, PageInfo, TotalDataV, PageTotalData>>
                    {
                        code = "false",
                        message = $"{responseResult.message}",
                        message_code = responseResult.message,
                        data = new BetResponeModel<List<DailyGameReportModel>, PageInfo, TotalDataV, PageTotalData>()
                        {
                            message = "",
                            result = new List<DailyGameReportModel>(),
                            pageInfo = new PageInfo(),
                            totalData = new TotalDataV(),
                            pageTotalData = new PageTotalData()
                        },
                    };
                }

            }
            catch (Exception e)
            {
                if (writeToFile != null)
                {
                    writeToFile.Done("GOrdersBLL_QueryOrdersByPostgrePrefix_ex", "QueryOrdersByPostgrePrefix ",
                        string.Format(@"requestdata = {0}, responseData = {1},   e.message={2}", remoteUrl, data, e.StackTrace), true);
                }
                return new ApiResultModel<BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalDataV, PageTotalData>>
                {
                    code = "false",
                    message = "游戏加载数据_失败E",
                    message_code = "query orders ex...",
                    data = new BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalDataV, PageTotalData>()
                    {
                        message = "",
                        result = new List<BetWithPrefixModel>(),
                        pageInfo = new PageInfo(),
                        totalData = new TotalDataV(),
                        pageTotalData = new PageTotalData()
                    }
                };
            }
        }




        /// <summary>
        /// 获取平台 外站游戏 平台注单 
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="token"></param>
        /// <param name="loginKey"></param>
        /// <returns></returns>
        public static async Task<object> QueryOrdersByPlat(QueryOrdersByPlatInputModel inputModel,
            string token,
            string loginKey) {
            try {

                var currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(token, loginKey);
                if (!currencys.Contains(inputModel.currencyType)) {
                    return new ApiResponseModel<object>() {
                        code = 0,
                        message = "请选择货币查询",
                        message_code = "error_currency_search"
                    };
                }
                //var gameIds = new Dictionary<int, string>() {
                //    { 176, "9W" },
                //    { 175, "Spribe" },
                //    { 172, "EVO" },
                //    { 164, "JDB" },
                //    { 146, "PG" },
                //    { 177, "Turbo" },
                //    { 178, "JL" }
                //};
                
                if(!gameIDMapToNames.ContainsKey(inputModel.gameId))
                    return new ApiResponseModel<object> {
                        code = 0,
                        message = "错误的游戏平台【erro game plat code】",
                        message_code = "query_page_failed"
                    };

                if(inputModel.gameId == 146) {
                    //return await QueryOrdersByPG(inputModel, token, loginKey);
                    return await QueryOrdersByPostgre(inputModel, token, loginKey,gameId:1);
                }
                else if (inputModel.gameId == 175) {
                    //return await QueryOrdersBySpribe(inputModel, token, loginKey);
                    return await QueryOrdersByPostgre(inputModel, token, loginKey, gameId: 7);
                }
                else if (inputModel.gameId == 164) {
                    return await QueryOrdersByJDB(inputModel, token, loginKey);
                }
                else if (inputModel.gameId == 172) {
                    return await QueryOrdersByEVO(inputModel, token, loginKey);
                }
                else if (inputModel.gameId == 177) {
                    return await QueryOrdersByTurbo(inputModel, token, loginKey);
                }
                else if (inputModel.gameId == 178) {
                    //return await QueryOrdersByJL(inputModel, token, loginKey);
                    return await QueryOrdersByPostgre(inputModel, token, loginKey,gameId:2);
                }
                else {
                    return new ApiResponseModel<object> {
                        code = 0,
                        message = "游戏平台暂无数据【game plat no data】",
                        message_code = "query_page_failed"
                    };
                }

            }
            catch (Exception) {
                return new ApiResponseModel<object> {
                    code = 0,
                    message = "游戏加载数据",
                    message_code = "query_page_failed"
                };
            }
        }

        private static async Task<object> QueryOrdersByPG(QueryOrdersByPlatInputModel inputModel,
            string token,
            string loginKey) 
        {
            try {
                string whereSql = string.Empty;
                var viewName = " PG_BetOrderLog_wd a ";
                string orderBy = " a.create_time_d desc";
                string keyName = " a.bet_id ";

                if (!string.IsNullOrEmpty(inputModel.account)) {
                    whereSql = string.Format(@" a.player_name = '{0}' ", inputModel.account);
                }


                if (!string.IsNullOrEmpty(inputModel.startTime)
                    && !string.IsNullOrEmpty(inputModel.endTime)) {
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.create_time_d >= '{0}' and a.create_time_d < '{1}'",
                        inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and a.create_time_d >= '{1}' and a.create_time_d < '{2}'", whereSql, inputModel.startTime, inputModel.endTime);
                }
                else {
                    var currentDay = DateTime.UtcNow.AddHours(8);
                    inputModel.startTime = currentDay.ToString("yyyy-MM-dd 00:00:00");
                    inputModel.endTime = currentDay.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.create_time_d >= '{0}' and a.create_time_d < '{1}'", inputModel.startTime, inputModel.endTime)
                        : string.Format(@" {0} and a.create_time_d >= '{1}' and a.create_time_d < '{2}'", whereSql, inputModel.startTime, inputModel.endTime);

                }

                whereSql = string.Format(@" {0} and g.plat='{1}' ", whereSql, "pg");
                var leftjoinString = string.Format(@" left join ofGameSlots g with(nolock) on a.game_id = g.code ");
                var fields = " player_name account, bet_id orderno, bet_amount, win_amount, transfer_amount, [platform], create_time_d bet_time, updated_time_d bet_time_end,g.en,g.cn, plat='pg' ";


                //return await DapperPaging<PGBetRcordModel>.QueryPageV1(whereSql, viewName, leftjoinString,
                //        fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, DapperStringBase.ConnectionGameRecordString);

                var apiResultModel =  await DapperPaging<PGBetRcordModel>.QueryPageV1(whereSql, viewName, leftjoinString,
                        fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, DapperStringBase.ConnectionGameRecordString);

                if (apiResultModel.code.ToString().Equals("1")
                    && apiResultModel.data.TotalRecord > 0 && !string.IsNullOrEmpty(inputModel.startTime)
                    && !string.IsNullOrEmpty(inputModel.endTime)) //获取到分页数据,且有输入时间
                {
                    //获取统计数据： 
                    var tsql = string.Format(@"select sum(bet_amount) totalBet,sum(win_amount) totalWinAmount, sum(transfer_amount) totalWinlost, count(player_name) rowss  from PG_BetOrderLog_wd a with(nolock)
left join ofGameSlots g with(nolock) on a.game_id = g.code
where {0} ", whereSql);

                    var result = await DapperBase.QueryAsync<OrderReportModel>(tsql,
                        null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionGameRecordString);
                    var code = (result != null && result.Count > 0);
                    if (code) {
                        apiResultModel.data.totalData = result.First();
                    }
                }
                else {
                    apiResultModel.data.totalData = new OrderReportModel();
                }
                return apiResultModel;

            }
            catch (Exception) {
                return new ApiResponseModel<object> {
                    code = 0,
                    message = "游戏加载数据",
                    message_code = "query_page_failed"
                };
            }
        }

        private static async Task<object> QueryOrdersByPostgre(QueryOrdersByPlatInputModel inputModel,
            string token,
            string loginKey,
            int gameId) 
        {
            var data = string.Empty;
            try {
                var getString = string.Format(@"PageIndex={0}&PageSize={1}&GameId={5}&StartTime={2}&StopTime={3}&TimeType=0&UserName={4}",
                    inputModel.pageIndex,
                    inputModel.pageSize,
                    Common.UrlEncode(inputModel.startTime),
                    Common.UrlEncode(inputModel.endTime),
                    inputModel.account, gameId);
                var remoteUrl = string.Format(@"http://103.102.47.69:19071/api/Order/QueryOrderRecords?{0}", getString);
                data = await MRequest.Relay(remoteUrl, headers:null, "", "", 
                    isAllowedProxy: true,ContentType: "application/json; charset=utf-8", method:"get", proxyUrl: "", timeOut: 4000);

                var responseResult = JsonConvert.DeserializeObject<BetResponeModel<List<BetModel>, PageInfo, TotalData,PageInfo>>(data);
                if (string.IsNullOrEmpty(responseResult.message)) {
                    //查询成功
                    if(responseResult.result.Count > 0) {
                        return new ApiResponseModel<BetResponeModel<List<BetModel>, PageInfo, TotalData, PageInfo>> {
                            code = 1,
                            message = "查询成功",
                            message_code = "found data",
                            data = responseResult,
                        };
                    }
                    else {
                        return new ApiResponseModel<object> {
                            code = 0,
                            message = $"无数据",
                            message_code = "not found data"
                        };
                    }
                }
                else {
                    return new ApiResponseModel<object> {
                        code = 0,
                        message = $"游戏加载数据, data:{responseResult.message}",
                        message_code = responseResult.message
                    };
                }
                
            }
            catch (Exception) {
                return new ApiResponseModel<object> {
                    code = 0,
                    message = "游戏加载数据",
                    message_code = "query_page_failed"
                };
            }
        }

        private static async Task<object> QueryOrdersBySpribe(QueryOrdersByPlatInputModel inputModel,
            string token,
            string loginKey) {
            try {
                string whereSql = string.Empty;
                var viewName = " Spribe_BetOrderLog_withdraw w ";
                string orderBy = " w.create_time desc";
                string keyName = " w.provider_tx_id ";

                if (!string.IsNullOrEmpty(inputModel.account)) {
                    whereSql = string.Format(@" w.user_id = '{0}' ", inputModel.account);
                }


                if (!string.IsNullOrEmpty(inputModel.startTime)
                    && !string.IsNullOrEmpty(inputModel.endTime)) {
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" w.create_time >= '{0}' and w.create_time < '{1}'",
                        inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and w.create_time >= '{1}' and w.create_time < '{2}'", whereSql, inputModel.startTime, inputModel.endTime);
                }
                else {
                    var currentDay = DateTime.UtcNow.AddHours(8);
                    inputModel.startTime = currentDay.ToString("yyyy-MM-dd 00:00:00");
                    inputModel.endTime = currentDay.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" w.create_time >= '{0}' and w.create_time < '{1}'", inputModel.startTime, inputModel.endTime)
                        : string.Format(@" {0} and w.create_time >= '{1}' and w.create_time < '{2}'", whereSql, inputModel.startTime, inputModel.endTime);

                }

                //whereSql = string.Format(@" {0} and g.plat='{1}' ", whereSql, "pg");

                //whereSql = string.Format(@" {0} and d.provider_tx_id is not null ", whereSql);//已经结算的注单

                var leftjoinString = string.Format(@" left join Spribe_BetOrderLog_deposit d with(nolock) on w.provider_tx_id = d.withdraw_provider_tx_id
left join Spribe_BetOrderLog_rollback r with(nolock) on w.provider_tx_id = r.rollback_provider_tx_id ");

                var fields = " w.user_id account, w.provider_tx_id orderno, w.amount bet_amount, d.amount win_amount,(d.amount-w.amount) transfer_amount,w.game,w.platform, w.create_time bet_time ,d.create_time bet_time_end,r.create_time cancel_time, plat='Spribe' ";

                var apiResultModel = await DapperPaging<SpribeBetRcordModel>.QueryPageV1(whereSql, viewName, leftjoinString,
                        fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, DapperStringBase.ConnectionGameRecordString);

                if (apiResultModel.code.ToString().Equals("1")
                    && apiResultModel.data.TotalRecord > 0 && !string.IsNullOrEmpty(inputModel.startTime)
                    && !string.IsNullOrEmpty(inputModel.endTime)) //获取到分页数据,且有输入时间
                {
                    //获取统计数据： 只统计完成且没有取消的注单
                    var tsql = string.Format(@"select sum(w.amount) totalBet,sum(d.amount) totalWinAmount, sum(d.amount-w.amount) totalWinlost, count(*) rowss
from Spribe_BetOrderLog_withdraw w with(nolock)
left join Spribe_BetOrderLog_deposit d with(nolock) on w.provider_tx_id = d.withdraw_provider_tx_id
left join Spribe_BetOrderLog_rollback r with(nolock) on w.provider_tx_id = r.rollback_provider_tx_id
where {0} and r.rollback_provider_tx_id is null ", whereSql); 

                    var result = await DapperBase.QueryAsync<OrderReportModel>(tsql,
                        null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionGameRecordString);
                    var code = (result != null && result.Count > 0);
                    if (code) {
                        apiResultModel.data.totalData = result.First();
                    }
                }
                else {
                    apiResultModel.data.totalData = new OrderReportModel();
                }
                return apiResultModel;

            }
            catch (Exception) {
                return new ApiResponseModel<object> {
                    code = 0,
                    message = "游戏加载数据",
                    message_code = "query_page_failed"
                };
            }
        }

        private static async Task<object> QueryOrdersByJDB(QueryOrdersByPlatInputModel inputModel,
            string token,
            string loginKey) {
            try {
                string whereSql = string.Empty;
                var viewName = " JDB_BetOrderLog_wd wd ";
                string orderBy = " wd.bet_time desc";
                string keyName = " wd.transferId ";

                if (!string.IsNullOrEmpty(inputModel.account)) {
                    whereSql = string.Format(@" wd.uid = '{0}' ", inputModel.account);
                }


                if (!string.IsNullOrEmpty(inputModel.startTime)
                    && !string.IsNullOrEmpty(inputModel.endTime)) {
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" wd.bet_time >= '{0}' and wd.bet_time < '{1}'",
                        inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and wd.bet_time >= '{1}' and wd.bet_time < '{2}'", whereSql, inputModel.startTime, inputModel.endTime);
                }
                else {
                    var currentDay = DateTime.UtcNow.AddHours(8);
                    inputModel.startTime = currentDay.ToString("yyyy-MM-dd 00:00:00");
                    inputModel.endTime = currentDay.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" wd.bet_time >= '{0}' and wd.bet_time < '{1}'", inputModel.startTime, inputModel.endTime)
                        : string.Format(@" {0} and wd.bet_time >= '{1}' and wd.bet_time < '{2}'", whereSql, inputModel.startTime, inputModel.endTime);

                }

                whereSql = string.Format(@" {0} and g.plat='{1}' ", whereSql, "jdb");

                var leftjoinString = string.Format(@" left join JDB_BetOrderLog_rollback r with(nolock) on r.transferId = wd.transferId
left join ofGameSlots g with(nolock) on CAST(wd.mType AS varchar) = g.code ");

                var fields = " wd.uid account,wd.transferId orderno, -wd.bet bet_amount, win win_amount, netWin transfer_amount, wd.bet_time, wd.lastModifyTime bet_time_end, g.en,g.cn,cancel_time, plat='JDB' ";

                var apiResultModel = await DapperPaging<JDBBetRcordModel>.QueryPageV1(whereSql, viewName, leftjoinString,
                        fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, DapperStringBase.ConnectionGameRecordString);

                if (apiResultModel.code.ToString().Equals("1")
                    && apiResultModel.data.TotalRecord > 0 && !string.IsNullOrEmpty(inputModel.startTime)
                    && !string.IsNullOrEmpty(inputModel.endTime)) //获取到分页数据,且有输入时间
                {
                    //获取统计数据： 只统计完成且没有取消的注单
                    var tsql = string.Format(@"select sum(-wd.bet) totalBet,sum(win) totalWinAmount, sum(netWin) totalWinlost, count(*) rowss
from JDB_BetOrderLog_wd wd with(nolock)
left join JDB_BetOrderLog_rollback r with(nolock) on r.transferId = wd.transferId
left join ofGameSlots g with(nolock) on CAST(wd.mType AS varchar) = g.code
where {0} and r.transferId is null ", whereSql);

                    var result = await DapperBase.QueryAsync<OrderReportModel>(tsql,
                        null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionGameRecordString);
                    var code = (result != null && result.Count > 0);
                    if (code) {
                        apiResultModel.data.totalData = result.First();
                    }
                }
                else {
                    apiResultModel.data.totalData = new OrderReportModel();
                }
                return apiResultModel;

            }
            catch (Exception) {
                return new ApiResponseModel<object> {
                    code = 0,
                    message = "游戏加载数据",
                    message_code = "query_page_failed"
                };
            }
        }

        private static async Task<object> QueryOrdersByEVO(QueryOrdersByPlatInputModel inputModel,
            string token,
            string loginKey) {
            try {
                string whereSql = string.Empty;
                var viewName = " EVO_BetOrderLog_withdraw w ";
                string orderBy = " w.create_time desc";
                string keyName = " w.transactionRefId ";

                if (!string.IsNullOrEmpty(inputModel.account)) {
                    whereSql = string.Format(@" w.userId = '{0}' ", inputModel.account);
                }


                if (!string.IsNullOrEmpty(inputModel.startTime)
                    && !string.IsNullOrEmpty(inputModel.endTime)) {
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" w.create_time >= '{0}' and w.create_time < '{1}'",
                        inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and w.create_time >= '{1}' and w.create_time < '{2}'", whereSql, inputModel.startTime, inputModel.endTime);
                }
                else {
                    var currentDay = DateTime.UtcNow.AddHours(8);
                    inputModel.startTime = currentDay.ToString("yyyy-MM-dd 00:00:00");
                    inputModel.endTime = currentDay.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" w.create_time >= '{0}' and w.create_time < '{1}'", inputModel.startTime, inputModel.endTime)
                        : string.Format(@" {0} and w.create_time >= '{1}' and w.create_time < '{2}'", whereSql, inputModel.startTime, inputModel.endTime);

                }

                //whereSql = string.Format(@" {0} and g.plat='{1}' ", whereSql, "jdb");

                var leftjoinString = string.Format(@" left join EVO_BetOrderLog_deposit d with(nolock) on d.transactionRefId = w.transactionRefId
left join EVO_BetOrderLog_rollback r with(nolock) on r.transactionRefId = w.transactionRefId ");

                var fields = " w.userId account,w.transactionRefId orderno,w.transactionAmount bet_amount, d.transactionAmount win_Amount,(d.transactionAmount-w.transactionAmount) transfer_amount,w.gameType game,w.create_time bet_time,d.create_time bet_time_end,r.create_time cancel_time, plat='EVO' ";

                var apiResultModel = await DapperPaging<EVOBetRcordModel>.QueryPageV1(whereSql, viewName, leftjoinString,
                        fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, DapperStringBase.ConnectionGameRecordString);

                if (apiResultModel.code.ToString().Equals("1")
                    && apiResultModel.data.TotalRecord > 0 && !string.IsNullOrEmpty(inputModel.startTime)
                    && !string.IsNullOrEmpty(inputModel.endTime)) //获取到分页数据,且有输入时间
                {
                    //获取统计数据： 只统计完成且没有取消的注单
                    var tsql = string.Format(@"select sum(w.transactionAmount) totalBet,sum(d.transactionAmount) totalWinAmount, sum(d.transactionAmount-w.transactionAmount) totalWinlost, count(*) rowss
from EVO_BetOrderLog_withdraw w with(nolock)
left join EVO_BetOrderLog_deposit d with(nolock) on d.transactionRefId = w.transactionRefId
left join EVO_BetOrderLog_rollback r with(nolock) on r.transactionRefId = w.transactionRefId
where {0} and r.transactionRefId is null ", whereSql);

                    var result = await DapperBase.QueryAsync<OrderReportModel>(tsql,
                        null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionGameRecordString);
                    var code = (result != null && result.Count > 0);
                    if (code) {
                        apiResultModel.data.totalData = result.First();
                    }
                }
                else {
                    apiResultModel.data.totalData = new OrderReportModel();
                }
                return apiResultModel;

            }
            catch (Exception) {
                return new ApiResponseModel<object> {
                    code = 0,
                    message = "游戏加载数据",
                    message_code = "query_page_failed"
                };
            }
        }

        private static async Task<object> QueryOrdersByTurbo(QueryOrdersByPlatInputModel inputModel,
            string token,
            string loginKey) {
            try {
                string whereSql = string.Empty;
                var viewName = " Turbo_BetOrderLog_withdraw w ";
                string orderBy = " w.bet_time desc";
                string keyName = " w.roundid ";

                if (!string.IsNullOrEmpty(inputModel.account)) {
                    whereSql = string.Format(@" w.account = '{0}' ", inputModel.account);
                }


                if (!string.IsNullOrEmpty(inputModel.startTime)
                    && !string.IsNullOrEmpty(inputModel.endTime)) {
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" w.bet_time >= '{0}' and w.bet_time < '{1}'",
                        inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and w.bet_time >= '{1}' and w.bet_time < '{2}'", whereSql, inputModel.startTime, inputModel.endTime);
                }
                else {
                    var currentDay = DateTime.UtcNow.AddHours(8);
                    inputModel.startTime = currentDay.ToString("yyyy-MM-dd 00:00:00");
                    inputModel.endTime = currentDay.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" w.bet_time >= '{0}' and w.bet_time < '{1}'", inputModel.startTime, inputModel.endTime)
                        : string.Format(@" {0} and w.bet_time >= '{1}' and w.bet_time < '{2}'", whereSql, inputModel.startTime, inputModel.endTime);

                }

                //whereSql = string.Format(@" {0} and g.plat='{1}' ", whereSql, "jdb");

                var leftjoinString = string.Format(@" left join Turbo_BetOrderLog_deposit d with(nolock) on d.roundid = w.roundid
left join Turbo_BetOrderLog_rollback r with(nolock) on r.roundid = w.roundid ");

                var fields = " w.account,w.roundid orderno,w.money bet_amount, d.money win_Amount,(d.money-w.money) transfer_amount,w.game_name game,w.bet_time bet_time,d.update_time bet_time_end,r.cancel_time, plat='Turbo' ";

                var apiResultModel = await DapperPaging<TurboBetRcordModel>.QueryPageV1(whereSql, viewName, leftjoinString,
                        fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, DapperStringBase.ConnectionGameRecordString);

                if (apiResultModel.code.ToString().Equals("1")
                    && apiResultModel.data.TotalRecord > 0 && !string.IsNullOrEmpty(inputModel.startTime)
                    && !string.IsNullOrEmpty(inputModel.endTime)) //获取到分页数据,且有输入时间
                {
                    //获取统计数据： 只统计完成且没有取消的注单
                    var tsql = string.Format(@"select sum(w.money) totalBet,sum(d.money) totalWinAmount, sum(d.money-w.money) totalWinlost, count(*) rowss
from Turbo_BetOrderLog_withdraw w with(nolock)
left join Turbo_BetOrderLog_deposit d with(nolock) on d.roundid = w.roundid
left join Turbo_BetOrderLog_rollback r with(nolock) on r.roundid = w.roundid
where {0} and r.roundid is null ", whereSql);

                    var result = await DapperBase.QueryAsync<OrderReportModel>(tsql,
                        null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionGameRecordString);
                    var code = (result != null && result.Count > 0);
                    if (code) {
                        apiResultModel.data.totalData = result.First();
                    }
                }
                else {
                    apiResultModel.data.totalData = new OrderReportModel();
                }
                return apiResultModel;

            }
            catch (Exception) {
                return new ApiResponseModel<object> {
                    code = 0,
                    message = "游戏加载数据",
                    message_code = "query_page_failed"
                };
            }
        }

        private static async Task<object> QueryOrdersByJL(QueryOrdersByPlatInputModel inputModel,
            string token,
            string loginKey) {
            try {
                string whereSql = string.Empty;
                var viewName = " JL_OrderLog_withdraw w ";
                string orderBy = " w.id desc";
                string keyName = " w.id ";

                if (!string.IsNullOrEmpty(inputModel.account)) {
                    whereSql = string.Format(@" w.userId = '{0}' ", inputModel.account);
                }

                if (!string.IsNullOrEmpty(inputModel.startTime)
                    && !string.IsNullOrEmpty(inputModel.endTime)) {
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" w.wagersTimeSrc >= '{0}' and w.wagersTimeSrc < '{1}'",
                        inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and w.wagersTimeSrc >= '{1}' and w.wagersTimeSrc < '{2}'", whereSql, inputModel.startTime, inputModel.endTime);
                }
                else {
                    var currentDay = DateTime.UtcNow.AddHours(8);
                    inputModel.startTime = currentDay.ToString("yyyy-MM-dd 00:00:00");
                    inputModel.endTime = currentDay.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" w.wagersTimeSrc >= '{0}' and w.wagersTimeSrc < '{1}'", inputModel.startTime, inputModel.endTime)
                        : string.Format(@" {0} and w.wagersTimeSrc >= '{1}' and w.wagersTimeSrc < '{2}'", whereSql, inputModel.startTime, inputModel.endTime);

                }

                var leftjoinString = string.Format(@" left join JL_OrderLog_settle s with(nolock) on s.sessionId = w.sessionId
left join JL_OrderLog_Cancel c with(nolock) on c.reqId = w.reqId ");

                var fields = string.Format(@"  w.userId account, w.round orderno,w.game gid,w.sessionId, w.betAmount+iif(s.betAmount is NULL,0,s.betAmount) bet_amount ,
w.winloseAmount + iif(s.winloseAmount is NULL, 0, s.winloseAmount ) win_Amount, 
(w.winloseAmount + iif(s.winloseAmount is NULL, 0, s.winloseAmount ) - w.betAmount -iif(s.betAmount is NULL,0,s.betAmount) ) transfer_amount
, w.wagersTimeSrc bet_time, s.wagersTimeSrc bet_time_end, c.create_at,plat='JL',
iif(c.create_at is NULL, 0, 1) isC ");

                var apiResultModel = await DapperPaging<JLBetRcordModel>.QueryPageV1(whereSql, viewName, leftjoinString,
                        fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, DapperStringBase.ConnectionGameRecordString);

                if (apiResultModel.code.ToString().Equals("1")
                    && apiResultModel.data.TotalRecord > 0 && !string.IsNullOrEmpty(inputModel.startTime)
                    && !string.IsNullOrEmpty(inputModel.endTime)) //获取到分页数据,且有输入时间
                {
                    lock (gameSlotModels) {
                        if(gameSlotModels.Count <= 0) {
                            var queryGameList = string.Format(@"select en,cn,code from ofGameSlots
where plat='JL' and stat = 1 and dev='all'");
                            var games = DapperBase.Query<ofGameSlotModel>(queryGameList,
                                null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);
                            foreach(var g in games.ToList()) {
                                var gcode = Convert.ToInt32(g.code);
                                if(!gameSlotModels.ContainsKey(gcode))
                                    gameSlotModels.Add(Convert.ToInt32(gcode), g);
                            }
                        }
                    }
                    

                    #region 处理注单数据，
                    foreach (var m in apiResultModel.data.modelList) {

                        if (gameSlotModels.ContainsKey(m.gid))
                            m.game = gameSlotModels[m.gid].en;

                        if(m.sessionId == -1 && m.isC == 0) {
                            //电子，一单（下注和结算） 且注单没有取消
                            m.bet_time_end = m.bet_time; //下注结算时间相同
                        }
                        else if(m.sessionId > -1 && m.isC == 0) {
                            //下注和结算 2单 且注单没有取消

                        }
                    }
                    #endregion

                    //获取统计数据： 只统计完成且没有取消的注单
                    var tsql = string.Format(@"
select sum(w.betAmount+iif(s.betAmount is NULL, 0, s.betAmount)) totalBet, sum(w.winloseAmount+iif(s.winloseAmount is NULL, 0, s.winloseAmount)) totalWinAmount ,  
sum(w.winloseAmount+iif(s.winloseAmount is NULL, 0, s.winloseAmount)-w.betAmount-iif(s.betAmount is NULL, 0, s.betAmount)) totalWinlost , count(*) rowss
from JL_OrderLog_withdraw w with(nolock)
left join JL_OrderLog_settle s with(nolock) on s.sessionId = w.sessionId
left join JL_OrderLog_Cancel c with(nolock) on c.reqId = w.reqId
where {0} and c.create_at is null ", whereSql);

                    var result = await DapperBase.QueryAsync<OrderReportModel>(tsql,
                        null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionGameRecordString);
                    var code = (result != null && result.Count > 0);
                    if (code) {
                        apiResultModel.data.totalData = result.First();
                    }
                }
                else {
                    apiResultModel.data.totalData = new OrderReportModel();
                }
                return apiResultModel;

            }
            catch (Exception) {
                return new ApiResponseModel<object> {
                    code = 0,
                    message = "游戏加载数据",
                    message_code = "query_page_failed"
                };
            }
        }
    }
}
