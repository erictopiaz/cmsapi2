using AutoMapper;
using AWC2019.Core.Validation;
using AWC2019.Core.ViewModel;
using Ching.DistributedTransaction.EFModel;
using CMS.Core.Application.账号管理;
using CMS.Data.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using DBLibs.WXDapper;
using FluentValidation;
using GameLibs;
using GameLibs._9Wickets;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using LogLib.Helper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Utilities.Net;
using ShareEntity.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.lang;
using UtilsLib.Merchant;

namespace CMS.Core.Application.账号管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 获取游戏金额 : IRequest<获取游戏金额Return>
    {
        public string id { get; set; }
        public string Account { get; set; }
        public string GameCode { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }

        public int currencyType { set; get; } = 156;
        public WriteToFile writeToFile { get; set; } = null;

        public string callSec { set; get; } = "";
        public string apiUrl { set; get; } = "";
        public string jwtEncoderKey { set; get; } = "";
        public string desEncodeKey { set; get; } = "";
    }

    public class 获取游戏金额Return : ShareEntity.OutputModels.ResponseModel<float>
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 获取游戏金额Validator : AbstractValidator<获取游戏金额>
    {
        public 获取游戏金额Validator()
        {
            //RuleFor(x => x.id).DigitalNullable(1000,100000000);
            RuleFor(x => x.Account).Account();
            RuleFor(x => x.GameCode).CharAndDigitalNullable(1, 10);
        }
    }



    /// <summary>
    /// Request Handler
    /// </summary>
    public class 获取游戏金额Handler : BaseAccount<获取游戏金额, 获取游戏金额Return>
    {
        public 获取游戏金额Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger)
        {
        }

        private string SendRequest(string API, string[] parameters)
        {


            string getparameters = "";
            switch (API)
            {
                case "PTGetBalance":
                case "SportsBookBalanceQuery":
                case "ByQueryBalance":
                case "QueryEwBalance": getparameters = string.Format("account={0}", parameters[0]); break;
                case "TTG_BALANCEQUERY": getparameters = string.Format("uid={0}", parameters[0]); break;
                case "KenoBalanceQuery":
                case "Keno2BalanceQuery": getparameters = string.Format("account={0}&ipaddress=&macaddress=", parameters[0]); break;
                case "AGBalanceQuery":
                case "UedBalanceQquery": getparameters = string.Format("account={0}&ipaddress=", parameters[0]); break;
                //case "TTG_BALANCEQUERY": getparameters = string.Format("uid={0}", parameters[0]); break;
                default:
                    for (int i = 0; i < parameters.Length; i++)
                        parameters[i] = "args=" + parameters[i];
                    getparameters = string.Join('&', parameters);
                    break;
            }

            string requestString = string.Concat("http://awcsvc-ea.azurewebsites.net", "/command.asmx/", API, "?", getparameters);
            WebClient wc = new WebClient();
            try
            {
                var result = wc.DownloadString(requestString);
                result = result.Substring(result.IndexOf('>') + 1);
                result = result.Substring(result.IndexOf('>') + 1);
                result = result.Substring(0, result.IndexOf('<'));
                return result;
            }
            catch (Exception ex)
            {
                return "-1";
            }

        }



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


        /// <summary>
        /// 根据游戏代码+游戏id 获取游戏余额
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="gameId"></param>
        /// <param name="currencyType"></param>
        /// <param name="account"></param>
        /// <param name="agaccount"></param>
        /// <param name="uid"></param>
        /// <param name="merchantAccount">品牌</param>
        /// <param name="writeToFile"></param>
        /// <param name="callSec"></param>
        /// <param name="apiUrl"></param>
        /// <param name="jwtEncoderKey"></param>
        /// <param name="desEncodeKey"></param>
        /// <returns></returns>
        private string SendRequestVINTL(string gameCode, int gameId,
            int currencyType, string account, string agaccount, long uid, 
            WriteToFile writeToFile, string merchantAccount, string callSec="",string apiUrl="", string jwtEncoderKey="", string desEncodeKey="") 
        {
            try {

                var model = new {
                    gameCode = gameCode,
                    gameId = gameId,
                    currencyType = currencyType,
                    account = account,
                    agaccount = agaccount,
                    uid = uid,
                    merchantAccount =merchantAccount
                };

                //转账钱包余额查询
                var secBody = UtilsLib.Common.DESEnCode(JsonConvert.SerializeObject(model), callSec);
                secBody = UtilsLib.Common.ToBase64String(secBody);
                var token = GetToken3(account, 4, "127.0.0.1", "2024-11-09 00:00:00",
                    "/api/Game/BalanceFromCMS", "", jwtEncoderKey: jwtEncoderKey, desEncodeKey: desEncodeKey);
                var Url = string.Format(@"{0}/api/Game/BalanceFromCMS?secBody={1}", apiUrl, secBody);
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, Url))
                    {
                        request.Headers.Add("Authorization", token);
                        var response = client.SendAsync(request).Result;
                        response.EnsureSuccessStatusCode();
                        var balanceResult = response.Content.ReadAsStringAsync().Result;
                        var balanceModel = JsonConvert.DeserializeObject<dynamic>(balanceResult);
                        return balanceModel.data;
                    }
                }

            }
            catch (Exception) {
                return "-1";
            }

        }


        protected override 获取游戏金额Return Handle(获取游戏金额 request)
        {
            var agaccount2 = "";
            var merchantAccount = "";
            
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var queryAccountSql = string.Format(@"SELECT id,account,currencyType,agaccount2,merchantAccount FROM ofuser WITH(NOLOCK) WHERE id={0}", request.id);
                if (string.IsNullOrEmpty(request.id))
                {
                    queryAccountSql = string.Format(@"SELECT id,account,currencyType,agaccount2,merchantAccount FROM ofuser WITH(NOLOCK) WHERE account='{0}'", request.Account);
                }
                var accountRow = connection.Query<ofUserx>(queryAccountSql);
                if (accountRow != null && accountRow.ToList().Count > 0)
                {
                    var user = accountRow.First();
                    request.Account = user.account;
                    request.currencyType = user.currencyType;
                    agaccount2 = user.agaccount2;
                    merchantAccount = user.merchantAccount;
                    request.id = user.id.ToString();
                }
                else
                {
                    return new 获取游戏金额Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        Data = -1,
                        message = "error request id "
                    };
                }

            }


            float balance = -1f;
            string retVal = "";

            if (request.currencyType == 156) {
                request.GameCode = request.GameCode.ToUpper().Equals("TT") ? "TTG" : request.GameCode;
                var gameid = CachedData.GameCodeDict.FirstOrDefault(
                    x => x.Value.ToLower() == request.GameCode.ToLower()).Key;


                var defModel = MerchantCodes.GetDefaultModel(merchantAccount, request.currencyType);
                if (defModel.walletType == 0)
                {
                    using (var context = new AmountRecordDbContext(DapperStringBase.ConnectionGamePostgreString))
                    {
                        // 查询要更新的账户，并锁定该记录
                        var user = context.UserGameWalletPostgres
                            .FromSqlRaw($"SELECT * FROM \"UserGameWallet\" WHERE account = '{request.Account.ToLower()}' FOR UPDATE")
                            .FirstOrDefault();
                        if (user != null)
                        {
                            retVal = user.wallet.ToString();
                        }
                        else
                        {
                            return new 获取游戏金额Return()
                            {
                                code = ShareEntity.Global.ResponseCode.code.FAIL,
                                Data = -1,
                                message = "会员未启动游戏"
                            };
                        }
                    }
                    if (float.TryParse(retVal, out balance))
                        return new 获取游戏金额Return()
                        {
                            code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                            Data = balance,
                            message = "success"
                        };
                    else
                        return new 获取游戏金额Return()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,//ching modidy from success to failed
                            Data = -1,
                            message = ""
                        };
                }


                if (!request.GameCode.ToUpper().Equals("AG")) {
                    retVal = SendRequestVINTL(request.GameCode,
                        gameid,
                        request.currencyType,
                        request.Account,
                        agaccount: agaccount2,
                        Convert.ToInt64(request.id)
                        , request.writeToFile, merchantAccount: merchantAccount,
                        request.callSec, request.apiUrl, request.jwtEncoderKey, request.desEncodeKey);
                }
                else {

                    switch (request.GameCode.ToLower()) {
                        case "ag": retVal = SendRequest("AGBalanceQuery", new string[] { request.Account }); break;
                        default:
                            request.GameCode = "";
                            retVal = "";
                            break;
                    }

                }

            }
            else if(request.currencyType == 356) {
                //356 目前仅有  9w:176
                retVal = SendRequestVINTL(request.GameCode, gameId:176, request.currencyType, 
                    request.Account, 
                    agaccount:"", 
                    uid: Convert.ToInt64(request.id), 
                    request.writeToFile, merchantAccount:merchantAccount);
            }
            else {
                return new 获取游戏金额Return() {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,//ching modidy from success to failed
                    Data = -1,
                    message = "error currency "
                };
            }

            if (float.TryParse(retVal, out balance))
                return new 获取游戏金额Return()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    Data = balance,
                    message = "success"
                };
            else
                return new 获取游戏金额Return()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,//ching modidy from success to failed
                    Data = -1,
                    message = ""
                };
        }

        
    }

}

