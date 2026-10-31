using Amazon.Runtime.Internal;
using LogLib.Helper;
using Newtonsoft.Json;
using ShareEntity.Game.BetOrderModels;
using ShareEntity.InputModels.Account;
using ShareEntity.OutModels.BetRecord;
using ShareEntity.OutputModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.RequestHelper;

namespace GameOrderShare
{
    public class GOrdersBLL
    {
        /// <summary>
        /// 根据平台id ,拉取注单数据
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public static async Task<object> QueryOrdersByPostgre(QueryOrdersByPlatInputModel inputModel,
            int gameId, WriteToFile writeToFile)
        {
            var remoteUrl = "";
            var data = "";
            try
            {
                var Settled = inputModel.settled == 1 ? "&Settled=true" : inputModel.settled == 0 ? "&Settled=false" : "&Settled=";
                var getString = string.Format(@"PageIndex={0}&PageSize={1}&GameId={6}&StartTime={2}&StopTime={3}&TimeType=0&Currency={4}&UserName={5}{7}&AgentName={8}",
                    inputModel.pageIndex,
                    inputModel.pageSize,
                    Common.UrlEncode(inputModel.startTime),
                    Common.UrlEncode(inputModel.endTime),
                    inputModel.currencyType,
                    inputModel.account, gameId, Settled, inputModel.parentAccount);

                remoteUrl = string.Format(@"http://103.102.47.69:19071/api/Order/QueryOrderRecords?{0}", getString);

                data = await MRequest.Relay(remoteUrl, headers: null, "", "",
                    isAllowedProxy: true, ContentType: "application/json; charset=utf-8", method: "get", proxyUrl: "", timeOut: 4000);
                if (writeToFile != null)
                {
                    writeToFile.Done("QueryOrdersByPostgre_request", "QueryOrdersByPostgre ",
                        string.Format(@"remoteUrl = {0}   responseData={1}", remoteUrl, data), true);
                }
                var responseResult = JsonConvert.DeserializeObject<BetResponeModel<List<BetModel>, PageInfo, TotalData, PageTotalData>>(data);
                if (string.IsNullOrEmpty(responseResult.message))
                {
                    //查询成功
                    if (responseResult.result.Count > 0)
                    {
                        return new ApiResultModel<BetResponeModel<List<BetModel>, PageInfo, TotalData, PageTotalData>>
                        {
                            code = "true",
                            message = "查询成功",
                            message_code = "found data",
                            data = responseResult,
                        };
                    }
                    else
                    {
                        return new ApiResultModel<BetResponeModel<List<BetModel>, PageInfo, TotalData, PageTotalData>>
                        {
                            code = "true",
                            message = $"无数据",
                            message_code = "not found data",
                            data=responseResult,
                        };
                    }
                }
                else
                {
                    return new ApiResultModel<BetResponeModel<List<BetModel>, PageInfo, TotalData, PageTotalData>>
                    {
                        code = "false",
                        message = $"{responseResult.message}",
                        message_code = responseResult.message,
                        data = new BetResponeModel<List<BetModel>, PageInfo, TotalData, PageTotalData>()
                        {
                            message = "",
                            result = new List<BetModel>(),
                            pageInfo = new PageInfo(),
                            totalData = new TotalData(),
                            pageTotalData = new PageTotalData()
                        },
                    };
                }

            }
            catch (Exception e)
            {
                if (writeToFile != null)
                {
                    writeToFile.Done("GOrdersBLL_QueryOrdersByPostgre_ex", "QueryOrdersByPostgre ",
                        string.Format(@"requestdata = {0}, responseData = {1},   e.message={2}", remoteUrl, data, e.StackTrace), true);
                }
                return new ApiResultModel<BetResponeModel<List<BetModel>, PageInfo, TotalData, PageTotalData>>
                {
                    code = "false",
                    message = "游戏加载数据_失败E",
                    message_code = "query orders ex...",
                    data = new BetResponeModel<List<BetModel>, PageInfo, TotalData, PageTotalData>()
                    {
                        message = "",
                        result = new List<BetModel>(),
                        pageInfo = new PageInfo(),
                        totalData = new TotalData(),
                        pageTotalData = new PageTotalData()
                    }
                };
            }
        }

        /// <summary>
        /// 根据平台id ,拉取注单数据 (会员账号有前缀)
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public static async Task<object> QueryOrdersByPostgrePrefix(QueryOrdersByPlatInputModel inputModel,
            int gameId, WriteToFile writeToFile)
        {
            var remoteUrl = "";
            var data = "";
            try
            {
                var Settled = inputModel.settled == 1 ? "&Settled=true" : inputModel.settled == 0 ? "&Settled=false" : "&Settled=";
                var getString = string.Format(@"PageIndex={0}&PageSize={1}&GameId={6}&StartTime={2}&StopTime={3}&TimeType=0&Currency={4}&UserName={5}{7}&AgentName={8}&Id={9}",
                    inputModel.pageIndex,
                    inputModel.pageSize,
                    Common.UrlEncode(inputModel.startTime),
                    Common.UrlEncode(inputModel.endTime),
                    inputModel.currencyType,
                    inputModel.account, gameId, Settled, inputModel.parentAccount, inputModel.betId);

                remoteUrl = string.Format(@"http://103.102.47.69:19071/api/Order/QueryOrderRecords?{0}", getString);

                data = await MRequest.Relay(remoteUrl, headers: null, "", "",
                    isAllowedProxy: true, ContentType: "application/json; charset=utf-8", method: "get", proxyUrl: "", timeOut: 4000);
                if (writeToFile != null)
                {
                    writeToFile.Done("QueryOrdersByPostgrePrefix_request", "QueryOrdersByPostgrePrefix ",
                        string.Format(@"remoteUrl = {0}   responseData={1}", remoteUrl, data), true);
                }
                var responseResult = JsonConvert.DeserializeObject<BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalData, PageTotalData>>(data);
                if (string.IsNullOrEmpty(responseResult.message))
                {
                    //查询成功
                    if (responseResult.result.Count > 0)
                    {
                        return new ApiResultModel<BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalData, PageTotalData>>
                        {
                            code = "true",
                            message = "查询成功",
                            message_code = "found data",
                            data = responseResult,
                        };
                    }
                    else
                    {
                        return new ApiResultModel<BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalData, PageTotalData>>
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
                    return new ApiResultModel<BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalData, PageTotalData>>
                    {
                        code = "false",
                        message = $"{responseResult.message}",
                        message_code = responseResult.message,
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
            catch (Exception e)
            {
                if (writeToFile != null)
                {
                    writeToFile.Done("GOrdersBLL_QueryOrdersByPostgrePrefix_ex", "QueryOrdersByPostgrePrefix ",
                        string.Format(@"requestdata = {0}, responseData = {1},   e.message={2}", remoteUrl, data, e.StackTrace), true);
                }
                return new ApiResultModel<BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalData, PageTotalData>>
                {
                    code = "false",
                    message = "游戏加载数据_失败E",
                    message_code = "query orders ex...",
                    data = new BetResponeModel<List<BetWithPrefixModel>, PageInfo, TotalData, PageTotalData>()
                    {
                        message = "",
                        result = new List<BetWithPrefixModel>(),
                        pageInfo = new PageInfo(),
                        totalData = new TotalData(),
                        pageTotalData = new PageTotalData()
                    }
                };
            }
        }

        /// <summary>
        /// 根据平台id ,拉取注单数据
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public static async Task<object> QueryOrdersByPostgreV(QueryOrdersByPlatInputModel inputModel,
            int gameId, WriteToFile writeToFile)
        {
            try
            {
                var Settled = inputModel.settled == 1 ? "&Settled=true" : inputModel.settled == 0 ? "&Settled=false" : "";
                var getString = string.Format(@"PageIndex={0}&PageSize={1}&GameId={6}&StartTime={2}&StopTime={3}&TimeType=0&Currency={4}&UserName={5}{7}&AgentName={8}",
                    inputModel.pageIndex,
                    inputModel.pageSize,
                    Common.UrlEncode(inputModel.startTime),
                    Common.UrlEncode(inputModel.endTime),
                    inputModel.currencyType,
                    inputModel.account, gameId, Settled, inputModel.parentAccount);

                var remoteUrl = string.Format(@"http://103.102.47.69:19071/api/Order/QueryOrderRecords?{0}", getString);

                var data = await MRequest.Relay(remoteUrl, headers: null, "", "",
                    isAllowedProxy: true, ContentType: "application/json; charset=utf-8", method: "get", proxyUrl: "", timeOut: 4000);

                writeToFile.Done("QueryOrdersByPostgre_request", "QueryOrdersByPostgre ",
                    string.Format(@"remoteUrl = {0}   responseData={1}", remoteUrl, data), true);
                var responseResult = JsonConvert.DeserializeObject<BetResponeModel<List<BetModel>, PageInfo, TotalData, PageTotalData>>(data);
                if (string.IsNullOrEmpty(responseResult.message))
                {
                    //查询成功
                    if (responseResult.result.Count > 0)
                    {
                        return new ApiResponseModel<BetResponeModel<List<BetModel>, PageInfo, TotalData, PageTotalData>>
                        {
                            code = 1,
                            message = "查询成功",
                            message_code = "found data",
                            data = responseResult,
                        };
                    }
                    else
                    {
                        return new ApiResponseModel<object>
                        {
                            code = 1,
                            message = $"无数据",
                            message_code = "not found data",
                            data = responseResult,
                        };
                    }
                }
                else
                {
                    return new ApiResponseModel<object>
                    {
                        code = 0,
                        message = $"{responseResult.message}",
                        message_code = responseResult.message,
                        data = new BetResponeModel<List<BetModel>, PageInfo, TotalData, PageTotalData>()
                        {
                            message = "",
                            result = new List<BetModel>(),
                            pageInfo = new PageInfo(),
                            totalData = new TotalData(),
                            pageTotalData = new PageTotalData()
                        },
                    };
                }

            }
            catch (Exception)
            {
                return new ApiResponseModel<object>
                {
                    code = 0,
                    message = "游戏加载数据_失败E",
                    message_code = "query orders ex...",
                    data = new BetResponeModel<List<BetModel>, PageInfo, TotalData, PageTotalData>()
                    {
                        message = "",
                        result = new List<BetModel>(),
                        pageInfo = new PageInfo(),
                        totalData = new TotalData(),
                        pageTotalData = new PageTotalData()
                    }
                };
            }
        }

        
        /// <summary>
        /// 获取会员游戏报表
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="writeToFile"></param>
        /// <returns></returns>
        public static async Task<ApiResultModel<BetResponeModel<List<BetReportModel>, PageInfo, TotalData, PageTotalData>>> QueryOrderReportPostgre(QueryOrdersReportInputModel inputModel,
            WriteToFile writeToFile)
        {
            try
            {
                
                var getString = string.Format(@"UserName={0}&StartTime={1}&StopTime={2}&TimeType={3}&Currency={4}&AgentName={5}",
                   inputModel.account,
                   Common.UrlEncode(inputModel.startTime),
                   Common.UrlEncode(inputModel.endTime),
                   inputModel.TimeType,
                   inputModel.currencyType,inputModel.parentAccount);

                var remoteUrl = string.Format(@"http://103.102.47.69:19071/api/Order/QueryOrderReport?{0}", getString);

                var data = await MRequest.Relay(remoteUrl, headers: null, "", "",
                    isAllowedProxy: true, ContentType: "application/json; charset=utf-8", method: "get", proxyUrl: "", timeOut: 4000);

                writeToFile.Done("QueryOrderReportPostgre_request", "QueryOrderReportPostgre ",
                    string.Format(@"remoteUrl = {0}   responseData={1}", remoteUrl, data), true);
                var responseResult = JsonConvert.DeserializeObject<BetResponeModel<List<BetReportModel>, PageInfo, TotalData, PageTotalData>>(data);
                if (string.IsNullOrEmpty(responseResult.message))
                {
                    //查询成功
                    if (responseResult.result.Count > 0)
                    {
                        return new ApiResultModel<BetResponeModel<List<BetReportModel>, PageInfo, TotalData, PageTotalData>>
                        {
                            code = "true",
                            message = "查询成功",
                            message_code = "found data",
                            data = responseResult,
                        };
                    }
                    else
                    {
                        return new ApiResultModel<BetResponeModel<List<BetReportModel>, PageInfo, TotalData, PageTotalData>>
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
                    return new ApiResultModel<BetResponeModel<List<BetReportModel>, PageInfo, TotalData, PageTotalData>>
                    {
                        code = "false",
                        message = $"{responseResult.message}",
                        message_code = responseResult.message,
                        data = new BetResponeModel<List<BetReportModel>, PageInfo, TotalData, PageTotalData>()
                        {
                            message = "",
                            result = new List<BetReportModel>(),
                            pageInfo = new PageInfo(),
                            totalData = new TotalData(),
                            pageTotalData = new PageTotalData()
                        },
                    };
                }

            }
            catch (Exception e)
            {
                writeToFile.Done("GOrdersBLL_QueryOrderReportPostgre_ex", "QueryOrderReportPostgre",
                    string.Format(@"requestdata = {0}   e.message={1}",
                    JsonConvert.SerializeObject(inputModel), e.StackTrace), true);
                return new ApiResultModel<BetResponeModel<List<BetReportModel>, PageInfo, TotalData, PageTotalData>>
                {
                    code = "false",
                    message = "游戏加载游戏数据，处理异常",
                    message_code = "query orders ex...",
                    data = new BetResponeModel<List<BetReportModel>, PageInfo, TotalData, PageTotalData>()
                    {
                        message = "",
                        result = new List<BetReportModel>(),
                        pageInfo = new PageInfo(),
                        totalData = new TotalData(),
                        pageTotalData = new PageTotalData()
                    }
                };
            }
        }

        /// <summary>
        /// 获取会员游戏报表平台总投注
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="writeToFile"></param>
        /// <returns></returns>
        public static async Task<ApiResultModel<BetResponeModel<List<UserBetReportModel>>>> QueryUsersByAgentWhereBetAmount(QueryOrdersReportInputModel inputModel,
            WriteToFile writeToFile)
        {
            try
            {

                var getString = string.Format(@"AgentName={0}&BetAmount={1}&StartTime={2}&StopTime={3}",
                   inputModel.parentAccount, inputModel.betAmount,
                   Common.UrlEncode(inputModel.startTime),
                   Common.UrlEncode(inputModel.endTime));
                var remoteUrl = string.Format(@"http://103.102.47.69:19071/api/Order/QueryUsersByAgentWhereBetAmount?{0}", getString);

                var data = await MRequest.Relay(remoteUrl, headers: null, "", "",
                    isAllowedProxy: true, ContentType: "application/json; charset=utf-8", method: "get", proxyUrl: "", timeOut: 4000);

                writeToFile.Done("QueryUsersByAgentWhereBetAmount_request", "QueryUsersByAgentWhereBetAmount ",
                    string.Format(@"remoteUrl = {0}   responseData={1}", remoteUrl, data), true);
                var responseResult = JsonConvert.DeserializeObject<BetResponeModel<List<UserBetReportModel>>>(data);
                if (string.IsNullOrEmpty(responseResult.message))
                {
                    //查询成功
                    if (responseResult.result.Count > 0)
                    {
                        return new ApiResultModel<BetResponeModel<List<UserBetReportModel>>>
                        {
                            code = "true",
                            message = "查询成功",
                            message_code = "found data",
                            data = responseResult,
                        };
                    }
                    else
                    {
                        return new ApiResultModel<BetResponeModel<List<UserBetReportModel>>>
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
                    return new ApiResultModel<BetResponeModel<List<UserBetReportModel>>>
                    {
                        code = "false",
                        message = $"{responseResult.message}",
                        message_code = responseResult.message,
                        data = new BetResponeModel<List<UserBetReportModel>>()
                        {
                            message = "",
                            result = new List<UserBetReportModel>()
                        },
                    };
                }

            }
            catch (Exception e)
            {
                writeToFile.Done("GOrdersBLL_QueryUsersByAgentWhereBetAmount_ex", "QueryUsersByAgentWhereBetAmount",
                    string.Format(@"requestdata = {0}   e.message={1}",
                    JsonConvert.SerializeObject(inputModel), e.StackTrace), true);
                return new ApiResultModel<BetResponeModel<List<UserBetReportModel>>>
                {
                    code = "false",
                    message = "游戏加载游戏数据，处理异常",
                    message_code = "query orders ex...",
                    data = new BetResponeModel<List<UserBetReportModel>>()
                    {
                        message = "",
                        result = new List<UserBetReportModel>()
                    }
                };
            }
        }



        /// <summary>
        /// 查询游戏 平台列表
        /// </summary>
        /// <param name="writeToFile"></param>
        /// <returns></returns>
        public static async Task<object> QueryGameListPostgre(WriteToFile writeToFile)
        {
            try
            {
                var remoteUrl = string.Format(@"http://103.102.47.69:19071/api/Game/QueryGameList");

                var data = await MRequest.Relay(remoteUrl, headers: null, "", "",
                    isAllowedProxy: true, ContentType: "application/json; charset=utf-8", method: "get", proxyUrl: "", timeOut: 4000);

                writeToFile.Done("QueryGameListPostgre_request", "QueryGameListPostgre ",
                    string.Format(@"remoteUrl = {0}   responseData={1}", remoteUrl, data), true);

                var responseResult = JsonConvert.DeserializeObject<BetResponeModel<List<GameModel>>>(data);
                if (string.IsNullOrEmpty(responseResult.message))
                {
                    //查询成功
                    if (responseResult.result.Count > 0)
                    {
                        return new ApiResultModel<List<GameModel>>
                        {
                            code = "true",
                            message = "查询成功",
                            message_code = "found data",
                            data = responseResult.result,
                        };
                    }
                    else
                    {
                        return new ApiResultModel<List<GameModel>>
                        {
                            code = "true",
                            message = $"无数据",
                            message_code = "not found data",
                            data = new List<GameModel>(),
                        };
                    }
                }
                else
                {
                    return new ApiResultModel<object>
                    {
                        code = "false",
                        message = $"{responseResult.message}",
                        message_code = responseResult.message,
                        data = new List<GameModel>()
                    };
                }

            }
            catch (Exception)
            {
                return new ApiResultModel<object>
                {
                    code = "false",
                    message = "游戏平台，处理失败E",
                    message_code = "query game list ex...",
                    data = new List<GameModel>()
                };
            }
        }
        
        /// <summary>
        /// 查询游戏 平台列表
        /// </summary>
        /// <param name="writeToFile"></param>
        /// <returns></returns>
        public static async Task<object> QueryGameListPostgreV(WriteToFile writeToFile)
        {
            try
            {
                var remoteUrl = string.Format(@"http://103.102.47.69:19071/api/Game/QueryGameList");

                var data = await MRequest.Relay(remoteUrl, headers: null, "", "",
                    isAllowedProxy: true, ContentType: "application/json; charset=utf-8", method: "get", proxyUrl: "", timeOut: 4000);

                writeToFile.Done("QueryGameListPostgre_request", "QueryGameListPostgre ",
                    string.Format(@"remoteUrl = {0}   responseData={1}", remoteUrl, data), true);

                var responseResult = JsonConvert.DeserializeObject<BetResponeModel<List<GameModel>>>(data);
                if (string.IsNullOrEmpty(responseResult.message))
                {
                    //查询成功
                    if (responseResult.result.Count > 0)
                    {
                        return new ApiResponseModel<List<GameModel>>
                        {
                            code = 1,
                            message = "查询成功",
                            message_code = "found data",
                            data = responseResult.result,
                        };
                    }
                    else
                    {
                        return new ApiResponseModel<List<GameModel>>
                        {
                            code = 0,
                            message = $"无数据",
                            message_code = "not found data",
                            data = new List<GameModel>(),
                        };
                    }
                }
                else
                {
                    return new ApiResponseModel<object>
                    {
                        code = 0,
                        message = $"{responseResult.message}",
                        message_code = responseResult.message,
                        data = new List<GameModel>()
                    };
                }

            }
            catch (Exception)
            {
                return new ApiResponseModel<object>
                {
                    code = 0,
                    message = "游戏平台，处理失败E",
                    message_code = "query game list ex...",
                    data = new List<GameModel>()
                };
            }
        }



        /// <summary>
        /// 查询会员列表（多个会员），总投注数据
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="gameId">暂时无使用</param>
        /// <param name="writeToFile"></param>
        /// <returns></returns>
        public static async Task<BetResponeModel<List<QueryReportsByUsersModel>>> QueryReportsByUsers(QueryReportsByUsersInputModel inputModel,
            int gameId, WriteToFile writeToFile)
        {
            var responseData = string.Empty;
            try
            {
                var remoteUrl = string.Format(@"http://103.102.47.69:19071/api/Order/QueryReportsByUsers");
                var postJson = JsonConvert.SerializeObject(inputModel);
                responseData = await MRequest.RelayFormWithoutWebException(remoteUrl, "post", null, true, postJson, "",
                           ContentType: " application/json-patch+json", isAllowedProxy: false, proxyUrl: "", timeOut:5000);
                if (writeToFile != null)
                {
                    writeToFile.Done("QueryReportsByUsers_request", "QueryReportsByUsers ",
                        string.Format(@"remoteUrl = {0} , requestModel= {1},  responseData={2}",
                        remoteUrl, JsonConvert.SerializeObject(inputModel), responseData), true);
                }
                var responseResult = JsonConvert.DeserializeObject<BetResponeModel<List<QueryReportsByUsersModel>>>(responseData);
                return responseResult;
                //if (string.IsNullOrEmpty(responseResult.message))
                //{
                //    //查询成功
                //    if (responseResult.result.Count > 0)
                //    {
                //        return new ApiResponseModel<BetResponeModel<List<QueryReportsByUsersModel>>>
                //        {
                //            code = 1,
                //            message = "查询成功",
                //            message_code = "found data",
                //            data = responseResult,
                //        };
                //    }
                //    else
                //    {
                //        return new ApiResponseModel<BetResponeModel<List<QueryReportsByUsersModel>>>
                //        {
                //            code = 1,
                //            message = $"无数据",
                //            message_code = "not found data",
                //            data = responseResult,
                //        };
                //    }
                //}
                //else
                //{
                //    return new ApiResponseModel<BetResponeModel<List<QueryReportsByUsersModel>>>
                //    {
                //        code = 0,
                //        message = $"{responseResult.message}",
                //        message_code = responseResult.message,
                //        data = new BetResponeModel<List<QueryReportsByUsersModel>>()
                //        {
                //            message = "",
                //            result = new List<QueryReportsByUsersModel>(),
                //        },
                //    };
                //}

            }
            catch (Exception e)
            {
                //return new ApiResponseModel<BetResponeModel<List<QueryReportsByUsersModel>>>
                //{
                //    code = 0,
                //    message = "游戏加载数据_失败E",
                //    message_code = "query orders ex...",
                //    data = new BetResponeModel<List<QueryReportsByUsersModel>>()
                //    {
                //        message = "",
                //        result = new List<QueryReportsByUsersModel>()
                //    }
                //};
                if (writeToFile != null) {
                    writeToFile.Done("QueryReportsByUsers_request_ex", "QueryReportsByUsers ex ",
                        string.Format(@"requestModel = {0}   e.StackTrace={1}", JsonConvert.SerializeObject(inputModel), e.StackTrace), true);
                }
                return new BetResponeModel<List<QueryReportsByUsersModel>>()
                {
                    message = "处理异常",
                    result = new List<QueryReportsByUsersModel>()
                };
            }
        }



        /// <summary>
        ///  拉取平台，有效投注 （目前固定为pg平台）
        /// </summary>
        /// <param name="account"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="gameId"></param>
        /// <param name="writeToFile"></param>
        /// <returns></returns>
        public static async Task<ApiResponseModel<object>> QueryValidAmountPostgreV(string account,
            string startTime,
            string endTime,
            IList<int> gameIdList,
            int gameId=1,
            WriteToFile writeToFile=null)
        {
            try
            {
                var gameIdString = "";
                if (gameIdList.Count > 0)
                {
                    //GameIds=10&GameIds=12&GameIds=22
                    foreach (var gid in gameIdList)
                    {
                        gameIdString = string.IsNullOrEmpty(gameIdString)
                            ? string.Format("&GameIds={0}", gid)
                            : string.Format("{0}&GameIds={1}", gameIdString, gid);
                    }
                }
                var getString = string.Format(@"UserName={0}&StartTime={1}&StopTime={2}{3}",
                    account,
                    Common.UrlEncode(startTime),
                    Common.UrlEncode(endTime), gameIdString);

                var remoteUrl = string.Format(@"http://103.102.47.69:19071/api/Order/QueryPgValidAmount?{0}", getString);

                var data = await MRequest.Relay(remoteUrl, headers: null, "", "",
                    isAllowedProxy: true, ContentType: "application/json; charset=utf-8", method: "get", proxyUrl: "", timeOut: 4000);

                writeToFile.Done("QueryValidAmountPostgreV_request", "QueryValidAmountPostgreV ",
                    string.Format(@"remoteUrl = {0}   responseData={1}", remoteUrl, data), true);

                var responseResult = JsonConvert.DeserializeObject<dynamic>(data);
                if (string.IsNullOrEmpty((string)responseResult.message))
                {
                    //查询成功
                    return new ApiResponseModel<object>
                    {
                        code = 1,
                        message = "查询成功",
                        message_code = "success",
                        data = (decimal)responseResult.validAmount,
                    };
                }
                else
                {
                    return new ApiResponseModel<object>
                    {
                        code = 0,
                        message = $"{responseResult.message}",
                        message_code = "query_bet_amount_failed",
                        data = 0,
                    };
                }

            }
            catch (Exception ex)
            {
                writeToFile.Done("QueryValidAmountPostgreV_request_ex", "QueryValidAmountPostgreV ",
                   string.Format(@"account = {0}   ex.StackTrace={1}", account, ex.StackTrace), true);
                return new ApiResponseModel<object>
                {
                    code = 0,
                    message = "游戏加载数据,处理异常",
                    message_code = "exception",
                    data = 0
                };
            }
        }

        /// <summary>
        ///  拉取平台，有效投注和总投注次数 （目前固定为pg平台）
        /// </summary>
        /// <param name="account"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="gameId"></param>
        /// <param name="writeToFile"></param>
        /// <returns></returns>
        public static async Task<ApiResponseModel<QueryPGOrdersOutModel>> QueryOrderDailyBonusV(string account,
            string startTime,
            string endTime,
            IList<int> gameIdList,
            int gameId = 1,
            WriteToFile writeToFile = null)
        {
            try
            {
                var gameIdString = "";
                if (gameIdList.Count > 0)
                {
                    //GameIds=10&GameIds=12&GameIds=22
                    foreach (var gid in gameIdList)
                    {
                        gameIdString = string.IsNullOrEmpty(gameIdString)
                            ? string.Format("&GameIds={0}", gid)
                            : string.Format("{0}&GameIds={1}", gameIdString, gid);
                    }
                }

                var getString = string.Format(@"UserName={0}&StartTime={1}&StopTime={2}{3}",
                    account,
                    Common.UrlEncode(startTime),
                    Common.UrlEncode(endTime), gameIdString);
                var remoteUrl = string.Format(@"http://103.102.47.69:19071/api/Order/QueryPgOrderChallenge?{0}", getString);

                var data = await MRequest.Relay(remoteUrl, headers: null, "", "",
                    isAllowedProxy: true, ContentType: "application/json; charset=utf-8", method: "get", proxyUrl: "", timeOut: 4000);

                writeToFile.Done("QueryOrderDailyBonusV_request", "QueryOrderDailyBonusV ",
                    string.Format(@"remoteUrl = {0}   responseData={1}", remoteUrl, data), true);

                var responseResult = JsonConvert.DeserializeObject<QueryPGOrdersOutModel>(data);

                //{
                //  "message": null,
                //  "orderCount": 14,
                //  "totalValidAmount": 25.2,
                //  "avgValidAmount": 1.8
                //}

                if (string.IsNullOrEmpty(responseResult.message))
                {
                    //查询成功
                    return new ApiResponseModel<QueryPGOrdersOutModel>
                    {
                        code = 1,
                        message = "查询成功",
                        message_code = "success",
                        data = responseResult,
                    };
                }
                else
                {
                    return new ApiResponseModel<QueryPGOrdersOutModel>
                    {
                        code = 0,
                        message = $"{responseResult.message}",
                        message_code = "failed"
                    };
                }

            }
            catch (Exception)
            {
                return new ApiResponseModel<QueryPGOrdersOutModel>
                {
                    code = 0,
                    message = "游戏加载数据_失败E",
                    message_code = "query_exception"
                };
            }
        }

        /// <summary>
        ///  拉取平台，有效投注和总投注次数 （目前固定为pg,jdb,ky）
        /// </summary>
        /// <param name="account"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="gameId"></param>
        /// <param name="writeToFile"></param>
        /// <returns></returns>
        public static async Task<ApiResponseModel<QueryGameReportOutModel>> QueryGameRescueReportV(string account,
            string startTime,
            string endTime,
            List<int> gameIdList,
            int gameId = 1,
            WriteToFile writeToFile = null)
        {
            try
            {
                var gameIdString = "";
                if(gameIdList.Count > 0)
                {
                    //GameIds=10&GameIds=12&GameIds=22
                    foreach (var gid in gameIdList)
                    {
                        gameIdString = string.IsNullOrEmpty(gameIdString)
                            ? string.Format("&GameIds={0}", gid)
                            : string.Format("{0}&GameIds={1}", gameIdString, gid);
                    }
                }
                var getString = string.Format(@"UserName={0}&StartTime={1}&StopTime={2}{3}",
                    account,
                    Common.UrlEncode(startTime),
                    Common.UrlEncode(endTime), gameIdString);

                

                var remoteUrl = string.Format(@"http://103.102.47.69:19071/api/Order/QueryRescueReport?{0}", getString);

                var data = await MRequest.Relay(remoteUrl, headers: null, "", "",
                    isAllowedProxy: true, ContentType: "application/json; charset=utf-8", method: "get", proxyUrl: "", timeOut: 4000);

                writeToFile.Done("QueryGameReportV_request", "QueryGameReportV ",
                    string.Format(@"remoteUrl = {0}   responseData={1}", remoteUrl, data), true);

                var responseResult = JsonConvert.DeserializeObject<QueryGameReportOutModel>(data);


                if (string.IsNullOrEmpty(responseResult.message))
                {
                    //查询成功
                    return new ApiResponseModel<QueryGameReportOutModel>
                    {
                        code = 1,
                        message = "查询成功",
                        message_code = "success",
                        data = responseResult,
                    };
                }
                else
                {
                    return new ApiResponseModel<QueryGameReportOutModel>
                    {
                        code = 0,
                        message = $"{responseResult.message}",
                        message_code = "failed"
                    };
                }

            }
            catch (Exception)
            {
                return new ApiResponseModel<QueryGameReportOutModel>
                {
                    code = 0,
                    message = "游戏加载数据_失败E",
                    message_code = "query_exception"
                };
            }
        }



        /// <summary>
        /// 幸运注单
        /// </summary>
        /// <param name="account"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="gameId"></param>
        /// <param name="writeToFile"></param>
        /// <returns></returns>
        public static async Task<ApiResponseModel<QueryPGLuckyOrdersOutModel>> QueryPGLuckyOrders(string account,
            string startTime,
            string endTime,
            int gameId = 1,
            WriteToFile writeToFile = null)
        {
            try
            {
                var getString = string.Format(@"UserName={0}&StartTime={1}&StopTime={2}",
                    account,
                    Common.UrlEncode(startTime),
                    Common.UrlEncode(endTime));

                var remoteUrl = string.Format(@"http://103.102.47.69:19071/api/Order/QueryPgLuckyOrders?{0}", getString);

                var data = await MRequest.Relay(remoteUrl, headers: null, "", "",
                    isAllowedProxy: true, ContentType: "application/json; charset=utf-8", method: "get", proxyUrl: "", timeOut: 4000);

                writeToFile.Done("QueryPGLuckyOrders_request", "QueryPGLuckyOrders ",
                    string.Format(@"remoteUrl = {0}   responseData={1}", remoteUrl, data), true);

                var responseResult = JsonConvert.DeserializeObject<QueryPGLuckyOrdersOutModel>(data);


                if (string.IsNullOrEmpty(responseResult.message))
                {
                    //查询成功
                    return new ApiResponseModel<QueryPGLuckyOrdersOutModel>
                    {
                        code = 1,
                        message = "查询成功",
                        message_code = "success",
                        data = responseResult,
                    };
                }
                else
                {
                    return new ApiResponseModel<QueryPGLuckyOrdersOutModel>
                    {
                        code = 0,
                        message = $"{responseResult.message}",
                        message_code = "failed"
                    };
                }

            }
            catch (Exception)
            {
                return new ApiResponseModel<QueryPGLuckyOrdersOutModel>
                {
                    code = 0,
                    message = "游戏加载数据_失败E",
                    message_code = "query_exception"
                };
            }
        }


        /// <summary>
        /// 高爆额外彩金
        /// </summary>
        /// <param name="account"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="gameId"></param>
        /// <param name="writeToFile"></param>
        /// <returns></returns>
        public static async Task<ApiResponseModel<QueryPGHighOddsOrdersOutModel>> QueryHighOddsOrders(string account,
            string startTime,
            string endTime,
            IList<int> gameIdList,
            int gameId = 1,
            WriteToFile writeToFile = null)
        {
            try
            {
                var gameIdString = "";
                if (gameIdList.Count > 0)
                {
                    foreach (var gid in gameIdList)
                    {
                        gameIdString = string.IsNullOrEmpty(gameIdString)
                            ? string.Format("&GameIds={0}", gid)
                            : string.Format("{0}&GameIds={1}", gameIdString, gid);
                    }
                }
                var getString = string.Format(@"UserName={0}&StartTime={1}&StopTime={2}{3}",
                    account,
                    Common.UrlEncode(startTime),
                    Common.UrlEncode(endTime), gameIdString);

                var remoteUrl = string.Format(@"http://103.102.47.69:19071/api/Order/QueryPgHighOddsOrders?{0}", getString);

                var data = await MRequest.Relay(remoteUrl, headers: null, "", "",
                    isAllowedProxy: true, ContentType: "application/json; charset=utf-8", method: "get", proxyUrl: "", timeOut: 4000);

                writeToFile.Done("QueryPGLuckyOrders_request", "QueryPGLuckyOrders ",
                    string.Format(@"remoteUrl = {0}   responseData={1}", remoteUrl, data), true);

                var responseResult = JsonConvert.DeserializeObject<QueryPGHighOddsOrdersOutModel>(data);


                if (string.IsNullOrEmpty(responseResult.message))
                {
                    //查询成功
                    return new ApiResponseModel<QueryPGHighOddsOrdersOutModel>
                    {
                        code = 1,
                        message = "查询成功",
                        message_code = "success",
                        data = responseResult,
                    };
                }
                else
                {
                    return new ApiResponseModel<QueryPGHighOddsOrdersOutModel>
                    {
                        code = 0,
                        message = $"{responseResult.message}",
                        message_code = "failed"
                    };
                }

            }
            catch (Exception)
            {
                return new ApiResponseModel<QueryPGHighOddsOrdersOutModel>
                {
                    code = 0,
                    message = "游戏加载数据_失败E",
                    message_code = "query_exception"
                };
            }
        }
    }
}
