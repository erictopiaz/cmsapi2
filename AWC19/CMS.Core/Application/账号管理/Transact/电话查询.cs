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
using ShareEntity.OutputModels;
using System.Net;
using Newtonsoft.Json.Linq;
using ShareEntity.Models;

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 电话查询 : IRequest<电话查询Return>
    {
        public string id { get; set; }
        public string Account { get; set; }
        public int CardIndex { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 电话查询Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 电话查询Validator : AbstractValidator<电话查询>
    {
        public 电话查询Validator()
        {
            RuleFor(x => x.Account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 电话查询Handler : BaseAccount<电话查询, 电话查询Return>
    {
        public 电话查询Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger)
        {
        }


        protected override 电话查询Return Handle(电话查询 request)
        {
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING)) {
                if (!string.IsNullOrEmpty(request.id)) {
                    var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                    request.Account = accountRow.account;
                }

                var userDetail = connection.Get<ofUser>(request.Account);
                string telephone = userDetail.telephone;
                if (string.IsNullOrEmpty(telephone))
                    return new 电话查询Return() {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "获取失败"
                    };

                WebClient wc = new WebClient();
                wc.Encoding = System.Text.Encoding.UTF8;

                string requestUrl = string.Format("https://api.ip138.com/mobile/?mobile={0}&datatype=jsonp&token=b2e0fe1793917a1fc56ce8cacf438e3e", telephone);
                string response = wc.DownloadString(requestUrl);
                /*
                    {
                        "ret": "ok",
                        "mobile": "19528127192",
                        "data": [
                            "河南",
                            "周口",
                            "移动",
                            "0394",
                            ""
                        ]
                    }
                 */
                var responseDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);
                if (!responseDict.ContainsKey("ret") && responseDict["ret"] != "ok") {
                    // Failed Fetch
                    _logger.LogError("TelephoneDetails - [FetchError] requesturl:{0}, response:{1}", requestUrl, response);
                    return new 电话查询Return() {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "获取失败"
                    };
                }

                if (!responseDict.ContainsKey("data")) {
                    return new 电话查询Return() {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "获取失败"
                    };
                }
                JArray x = JArray.FromObject(responseDict["data"]);
                string responseData = String.Join(' ', x);
                return new 电话查询Return() {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = responseData
                };
                
            }
        }
    }
}

