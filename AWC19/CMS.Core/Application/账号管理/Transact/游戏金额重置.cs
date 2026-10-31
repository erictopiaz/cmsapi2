using AutoMapper;
using AWC2019.Core.Validation;
using AWC2019.Core.ViewModel;
using Ching.DistributedTransaction.EFModel;
using Ching.DistributedTransaction.EFModel.TableModel;
using CMS.Core.Application.账号管理;
using CMS.Core.Application.账号管理.Queries;
using CMS.Data.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using DBLibs.WXDapper;
using FluentValidation;
using GameLibs;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using LogLib.Helper;
using LogLib.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities.Net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.Merchant;
using static GameLibs.TTG.TTGBLL;
using static UtilsLib.lang.LangCodes;

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 游戏金额重置 : IRequest<游戏金额重置Return>
    {
        public string id { get; set; }
        public string Account { get; set; }
        public string GameCode { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }

        public WriteToFile writeToFile { get; set; } = null;
        public string callSec { set; get; } = "";
        public string apiUrl { set; get; } = "";
        public string jwtEncoderKey { get; set; } = "";
        public string desEncodeKey { set; get; } = "";
    }

    public class 游戏金额重置Return : ShareEntity.OutputModels.ResponseModel<float>
    {
        public string  account { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 游戏金额重置Validator : AbstractValidator<游戏金额重置>
    {
        public 游戏金额重置Validator()
        {
            RuleFor(x => x.Account).Account();
            RuleFor(x => x.GameCode).CharAndDigitalNullable(1,10);
        }
    }



    /// <summary>
    /// Request Handler
    /// </summary>
    public class 游戏金额重置Handler : BaseAccount<游戏金额重置, 游戏金额重置Return>
    {
        public 游戏金额重置Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger)
        {
        }


        #region 
       
        protected override 游戏金额重置Return Handle(游戏金额重置 request) {

            var agaccount = "";
            var currencyType = 0;
            var uid = 0L;
            var merchantAccount = "";
            if (!string.IsNullOrEmpty(request.id)) {
                using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING)) {
                    var accountRow = connection.QuerySingle("SELECT account, currencyType, agaccount2, id as uid, merchantAccount FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                    request.Account = accountRow.account;
                    agaccount = accountRow.agaccount2;
                    currencyType = accountRow.currencyType;
                    uid = accountRow.uid;
                    merchantAccount = accountRow.merchantAccount;
                }
            }


            if ((DateTime.UtcNow.AddHours(8) - CachedData.QueryTimeDict["ResetBalanceToZero"]).TotalMilliseconds < 5000) {
                return new 游戏金额重置Return() {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = $"金额重置失败{request.GameCode}：操作频繁",
                    account = request.Account
                };
            }


            CachedData.QueryTimeDict["ResetBalanceToZero"] = DateTime.UtcNow.AddHours(8);

            var defModel = MerchantCodes.GetDefaultModel(merchantAccount, currencyType);
            if(defModel.walletType == 0)
            {
                return ResetWalletToZero(request.Account, merchantAccount);
            }


            if (!GameService.gameTransIDS_AwcNew.ContainsKey(request.GameCode.ToUpper())) {
                return new 游戏金额重置Return() {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = $"金额重置失败{request.GameCode}：【错误的游戏代码】",
                    account = request.Account
                };
            }

            var gameId = GameService.gameTransIDS_AwcNew[request.GameCode.ToUpper()];
            var gameBase = GameService.GetDynamicObjectByGameId(account: request.Account,
                gameid: gameId,
                lang: 2,
                writeToFile: request.writeToFile,
                gameName: request.GameCode.ToUpper());

            var reqBalance = BalanceQueryAsync(request.GameCode,gameId,currencyType:currencyType,request.Account,agaccount:agaccount,
                uid: uid,
                merchantAccount:merchantAccount,
                writeToFile: request.writeToFile, 
                callSec:request.callSec,
                apiUrl:request.apiUrl,
                jwtEncoderKey:request.jwtEncoderKey, desEncodeKey:request.desEncodeKey).Result;
            
            decimal balance = -1m;
            if (decimal.TryParse(reqBalance, out balance)) {
                ////
            }
            else
                return new 游戏金额重置Return()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = $"金额重置失败{request.GameCode}：【余额确认失败】,balance:{reqBalance}",
                    account = request.Account
                };

            if (balance>= 0)
            {
                return new 游戏金额重置Return()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = $"金额重置失败{request.GameCode}:【余额为正，当前金额为：{balance}】",
                    account = request.Account

                };
            }
            else
            {
                var trf_id = string.Format(@"{0}", UtilsLib.Common.GuidToLongString());
                var amount = balance * -1;
                if (amount < 0.01m)
                {
                    amount = 0.01m;
                }
                var r = TransferDepositManyStatusVAsync(request.GameCode, gameId, amount: amount, currencyType: currencyType,
                    request.Account, agaccount: agaccount,
                uid: uid, writeToFile: request.writeToFile,
                callSec: request.callSec,
                apiUrl: request.apiUrl,
                jwtEncoderKey: request.jwtEncoderKey, desEncodeKey: request.desEncodeKey).Result;
                if (r.code.ToString().Equals("1"))
                {
                    return new 游戏金额重置Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = $"金额重置成功{request.GameCode},补贴金额为：{amount}",
                        account = request.Account
                    };
                }
                else
                {
                    return new 游戏金额重置Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = $"金额重置失败{request.GameCode}，{r.message}",
                        account = request.Account
                    };
                }
            }

        }
        #endregion
        private 游戏金额重置Return ResetWalletToZero(string Account, string merchantAccount)
        {
            using (var context = new AmountRecordDbContext(DapperStringBase.ConnectionGamePostgreString))
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    // 查询要更新的账户，并锁定该记录
                    var user = context.UserGameWalletPostgres
                        .FromSqlRaw($"SELECT * FROM \"UserGameWallet\" WHERE account = '{Account.ToLower()}' FOR UPDATE ") // 锁住
                        .FirstOrDefault();
                    if (user != null)
                    {
                        //找到
                        if (user.wallet > 0)
                        {
                            return new 游戏金额重置Return()
                            {
                                code = ShareEntity.Global.ResponseCode.code.FAIL,
                                message = $"金额重置失败:【余额为正，当前金额为：{user.wallet}】",
                                account = Account
                            };
                        }
                        else
                        {
                            var amount = user.wallet * -1;
                            var transferNo = UtilsLib.Common.GuidToLong();
                            var md5Src = string.Format(@"{0},{1},{2}", transferNo, "reset", "wallet");
                            var actionId = $"wallet{UtilsLib.Common.GetMd5(md5Src)}";
                            var gModel = new GameTranferUnionLogPostgre()
                            {
                                account = Account.ToLower(),
                                action_id = actionId,
                                tid = $"wallet{transferNo.ToString()}",
                                create_at = DateTime.UtcNow.AddHours(8),
                                action_name = "inter_add",
                                platform = "wallet",
                                amount = amount,
                                amount_type = 1,
                                g_currency_type = user.currency_type,
                                affaccount = user.affaccount,
                                merchant_account = user.merchant_account
                            };
                            user.wallet = 0;
                            context.Add(gModel);
                            var returnValue_1 = context.SaveChanges();
                            if (returnValue_1 > 0)
                            {
                                trans.Commit();

                                return new 游戏金额重置Return()
                                {
                                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                                    message = $"金额重置成功,补贴金额为：{amount}",
                                    account = Account
                                };
                            }
                            else
                            {
                                return new 游戏金额重置Return()
                                {
                                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                                    message = $"金额重置失败!",
                                    account = Account
                                };
                            }
                        }
                    }
                    else
                    {
                        return new 游戏金额重置Return()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = $"游戏账号确认失败!",
                            account = Account
                        };
                    }
                }
            }


        }

        #region 透过游戏服务读取游戏余额
        private async Task<string> BalanceQueryAsync(string gameCode, int gameId,
            int currencyType, string account, string agaccount, long uid, string merchantAccount,
            WriteToFile writeToFile=null, string callSec = "", string apiUrl = "", 
            string jwtEncoderKey = "", string desEncodeKey = "")
        {
            try
            {
                var model = new
                {
                    gameCode = gameCode,
                    gameId = gameId,
                    currencyType = currencyType,
                    account = account,
                    agaccount = agaccount,
                    uid = uid,
                    merchantAccount = merchantAccount
                };

                var secBody = UtilsLib.Common.DESEnCode(JsonConvert.SerializeObject(model), callSec);
                secBody = UtilsLib.Common.ToBase64String(secBody);
                var token = GetToken3(account, 4, "127.0.0.1", "2024-11-09 00:00:00",
                    "/api/Game/BalanceFromCMS", "",jwtEncoderKey: jwtEncoderKey, desEncodeKey: desEncodeKey);
                var Url = string.Format(@"{0}/api/Game/BalanceFromCMS?secBody={1}", apiUrl, secBody);
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, Url))
                    {
                        request.Headers.Add("Authorization", token);
                        var response = await client.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                        var balanceResult = await response.Content.ReadAsStringAsync();
                        var balanceModel = JsonConvert.DeserializeObject<dynamic>(balanceResult);
                        return balanceModel.data;
                    }
                }
                
            }
            catch (Exception)
            {
                return "exception";
            }

        }
        #endregion

        /// <summary>
        /// Ching Security Method
        /// </summary>
        /// <param name="account"></param>
        /// <param name="callfrom"></param>
        /// <param name="ipaddress"></param>
        /// <param name="calltime"></param>
        /// <param name="callaction"></param>
        /// <param name="loginToken"></param>
        /// <returns></returns>
        private string GetToken3(string account, int callfrom, string ipaddress,
               string calltime, string callaction, string loginToken, string jwtEncoderKey, string desEncodeKey)
        {
            var json = JsonConvert.SerializeObject(new
            {
                account = account,
                callfrom = callfrom,
                ipaddress = ipaddress,
                calltime = calltime,
                callaction = callaction,
                pKey = loginToken
            });

            byte[] key = Encoding.UTF8.GetBytes(jwtEncoderKey);
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();//加密方式
            IJsonSerializer serializer = new JsonNetSerializer();//序列化Json
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();//base64加解密
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);//JWT编码
            var token = encoder.Encode(new
            {
                awcSign = UtilsLib.Common.DESEnCode(json, desEncodeKey)
            }, key);//生成令牌

            return token;
        }


        #region 透过游戏服务转入到游戏平台
        private async Task<ApiResponseModel<object>> TransferDepositManyStatusVAsync(string gameCode, int gameId, decimal amount,
            int currencyType, string account, string agaccount, long uid,
            WriteToFile writeToFile = null, string callSec = "", string apiUrl = "",
            string jwtEncoderKey = "", string desEncodeKey = "")
        {
            try
            {
                //{"amount":1300.0,"gamename":"IMES","direction":true,"withdrawType":0,
                //"currencyType":0,"account":"lcy199203",
                //"ipaddress":"182.147.154.135","macaddress":"","device":3,"lang":2} 
                var model = new
                {
                    amount = amount,
                    gamename = gameCode,
                    direction = false,
                    gameId = gameId,
                    withdrawType = 0,
                    currencyType = currencyType,
                    account = account,
                    agaccount = agaccount,
                    device = 4,
                    lang = 2,
                    ipaddress = "127.0.0.1",
                    macaddress = "",
                    uid = uid
                };
                var secBody = UtilsLib.Common.DESEnCode(JsonConvert.SerializeObject(model), callSec);
                secBody = UtilsLib.Common.ToBase64String(secBody);
                var token = GetToken3(account, 4, "127.0.0.1", "2024-11-09 00:00:00",
                    "/api/Game/TransferFromCMS", "", jwtEncoderKey: jwtEncoderKey, desEncodeKey: desEncodeKey);
                var Url = string.Format(@"{0}/api/Game/TransferFromCMS?secBody={1}", apiUrl, secBody);
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, Url))
                    {
                        request.Headers.Add("Authorization", token);
                        var response = await client.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                        var balanceResult = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<ApiResponseModel<object>>(balanceResult);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ApiResponseModel<object>
                {
                    code = 0,
                    message = "游戏转账,处理异常",
                    message_code = "game_1006"
                };
            }

        }
        #endregion
    }

}

