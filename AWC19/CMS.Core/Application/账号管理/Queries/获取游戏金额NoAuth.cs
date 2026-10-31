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
using CMS.Core.Application.账号管理;
using Microsoft.Extensions.Logging;
using Dapper.Contrib.Extensions;
using System.Net;

namespace CMS.Core.Application.账号管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 获取游戏金额NoAuth : IRequest<获取游戏金额NoAuthReturn>
    {
        public string id { get; set; }
        public string Account { get; set; }
        public string GameCode { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 获取游戏金额NoAuthReturn : ShareEntity.OutputModels.ResponseModel<float>
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 获取游戏金额NoAuthValidator : AbstractValidator<获取游戏金额NoAuth>
    {
        public 获取游戏金额NoAuthValidator()
        {
            //RuleFor(x => x.Account).Account();
            RuleFor(x => x.Account).Account();
            RuleFor(x => x.GameCode).CharAndDigitalNullable(1,10);
        }
    }



    /// <summary>
    /// Request Handler
    /// </summary>
    public class 获取游戏金额NoAuthHandler : BaseAccount<获取游戏金额NoAuth, 获取游戏金额NoAuthReturn>
    {
        public 获取游戏金额NoAuthHandler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger)
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

            //string requestString = string.Concat("http://awcsvc-ea-newcmsstating.azurewebsites.net", "/command.asmx/", API, "?", getparameters);
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
        protected override 获取游戏金额NoAuthReturn Handle(获取游戏金额NoAuth request)
        {
            if (!string.IsNullOrEmpty(request.id))
            {
                using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
                {
                    var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                    request.Account = accountRow.account;
                }
            }

            var gameid = CachedData.GameCodeDict.FirstOrDefault(
                x => x.Value.ToLower() == request.GameCode.ToLower()).Key;
            float balance = -1f;
            string retVal = "";
            switch (request.GameCode.ToLower())
            {
                //case "pt": retVal = SendRequest("PTGetBalance", new string[] { request.Account }); break;
                case "spb":
                case "188": retVal = SendRequest("SportsBookBalanceQuery", new string[] { request.Account }); break;
                case "ttg": retVal = SendRequest("TTG_BALANCEQUERY", new string[] { request.Account }); break;
                case "ued": retVal = SendRequest("UedBalanceQquery", new string[] { request.Account }); break;
                case "lb": retVal = SendRequest("UedBalanceQquery", new string[] { request.Account }); break;
                case "bbin": retVal = SendRequest("ByQueryBalance", new string[] { request.Account }); break;
                case "ag": retVal = SendRequest("AGBalanceQuery", new string[] { request.Account }); break;
                //case "kg": retVal = SendRequest("KenoBalanceQuery", new string[] { request.Account }); break;
                //case "kg2": retVal = SendRequest("Keno2BalanceQuery", new string[] { request.Account }); break;
                case "ea": retVal = SendRequest("QueryEwBalance", new string[] { request.Account }); break;
                case "xj": retVal = SendRequest("API", new string[] { "XJ_GetBalance", request.Account }); break;
                case "vr": retVal = SendRequest("API", new string[] { "VR_GetBalance", request.Account }); break;
                case "pg": retVal = SendRequest("API", new string[] { "PG_GetBalance", request.Account }); break;
                case "gg": retVal = SendRequest("API", new string[] { "GG_GetBalance", request.Account }); break;
                case "spt": retVal = SendRequest("API", new string[] { "SPT_GetBalance", request.Account }); break;
                case "ysb": retVal = SendRequest("API", new string[] { "YSB_GetBalance", request.Account }); break;
                case "imes": retVal = SendRequest("API", new string[] { "IMES_GetBalance", request.Account }); break;
                case "pt": retVal = SendRequest("API", new string[] { "IMPT_GetBalance", request.Account }); break;
                case "dscp": retVal = SendRequest("API", new string[] { "DS_GetBalance", request.Account }); break;
                case "dses": retVal = SendRequest("API", new string[] { "DSES_GetBalance", request.Account }); break;
                case "ob": retVal = SendRequest("API", new string[] { "OB_GetBalance", request.Account }); break;
                case "rg": retVal = SendRequest("API", new string[] { "RG_GetBalance", request.Account }); break;
                case "ky": retVal = SendRequest("API", new string[] { "KY_GetBalance", request.Account }); break;
                case "bg": retVal = SendRequest("API", new string[] { "BG_GetBalance", request.Account }); break;
                case "eb": retVal = SendRequest("API", new string[] { "EB_GetBalance", request.Account }); break;
                case "td": retVal = SendRequest("API", new string[] { "TD_GetBalance", request.Account }); break;
                case "aw": retVal = SendRequest("API", new string[] { "AW_GetBalance", request.Account }); break;
                case "tf": retVal = SendRequest("API", new string[] { "TF_GetBalance", request.Account }); break;
                case "pm":
                case "obg": retVal = SendRequest("API", new string[] { "OBG_GetBalance", request.Account }); break;
                case "imsp": retVal = SendRequest("API", new string[] { "IMSP_GetBalance", request.Account }); break;
                case "obcv": retVal = SendRequest("API", new string[] { "OBCV_GetBalance", request.Account }); break;
                case "sw": retVal = SendRequest("API", new string[] { "SW_GetBalance", request.Account }); break;
                case "cr": retVal = SendRequest("API", new string[] { "CR_GetBalance", request.Account }); break;
                case "kg":
                case "kg2":
                case "kg3": retVal = SendRequest("API", new string[] { "KG3_GetBalance", request.Account }); break;
                case "balance":
                case "main":
                case "awc":
                default:
                    request.GameCode = "";
                    retVal = "";
                    break;
            }
            if (float.TryParse(retVal, out balance))
                return new 获取游戏金额NoAuthReturn()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    Data = balance,
                    message = ""
                };
            else
                return new 获取游戏金额NoAuthReturn()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    Data = -1,
                    message = ""
                };
        }
    }

}

