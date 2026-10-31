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
using ShareEntity.Models;

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 卡查询 : IRequest<卡查询Return>
    {
        public string id { get; set; }
        public string Account { get; set; }
        public int CardIndex { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 卡查询Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 卡查询Validator : AbstractValidator<卡查询>
    {
        public 卡查询Validator()
        {
            RuleFor(x => x.Account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 卡查询Handler : BaseAccount<卡查询, 卡查询Return>
    {
        public 卡查询Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger)
        {
        }


        protected override 卡查询Return Handle(卡查询 request)
        {
            string cardtosearch = "";
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                if (!string.IsNullOrEmpty(request.id))
                {
                    var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                    request.Account = accountRow.account;
                }
                var userDetail = connection.Get<ofUser>(request.Account);
                string withdrawCards = userDetail.withdrawcards;
                if (string.IsNullOrEmpty(withdrawCards))
                    return new 卡查询Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "没绑定的卡"
                    };

                var withdrawCardList = new List<string>(withdrawCards.Split('|'));
                if (withdrawCardList.Count - 1 < request.CardIndex)
                {
                    return new 卡查询Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "超过范围"
                    };

                }

                /*
                REQUEST
                    http://vip.kazhiguo.com/bank/appkey=3ff96d5fc60dba46e327973e5ed2cb69&bankcard=6222083602009479277           
                
                RESPONSE
                    {"result":
                        {   "card":"6222083602009479277",
                            "bin":"622208",
                            "bank_id":"01020000",
                            "bank_name":"\u4e2d\u56fd\u5de5\u5546\u94f6\u884c",
                            "bank_name_short":"\u5de5\u884c",
                            "province":"\u5e7f\u4e1c\u7701",
                            "city":"\u5e7f\u5dde",
                            "address":"\u5e7f\u4e1c\u7701 - \u5e7f\u5dde",
                            "bank_code":"icbc",
                            "card_name":"\u7406\u8d22\u91d1\u5361",
                            "card_type":"\u501f\u8bb0\u5361",
                            "tel":"95588",
                            "url":"www.icbc.com.cn",
                            "status":"2"},
                       "error_code":0
                     }

                 */

                cardtosearch = withdrawCardList[request.CardIndex];
                /*
                建设银行,6217000900001977995,延吉市开发区分行
                 */
                cardtosearch = cardtosearch.Split(',')[1];
                var card = connection.Get<ofStatus>(cardtosearch);

                if (card != null)
                {
                    return new 卡查询Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = card.body
                    };
                }


            }

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {

                WebClient wc = new WebClient();
                wc.Encoding = System.Text.Encoding.UTF8;
                string requestUrl = string.Concat("http://api.kazhiguo.com/api/bankcard/apikey=5e89596adbac529fc1d1dc9377f4979f&bankcard=", cardtosearch);
                //string requestUrl = string.Concat("http://vip.kazhiguo.com/bank/appkey=3ff96d5fc60dba46e327973e5ed2cb69&bankcard=", cardtosearch);
                string response = wc.DownloadString(requestUrl);
                var responseDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);
                if (!responseDict.ContainsKey("error_code") && responseDict["error_code"] != 0)
                {
                    // Failed Fetch
                    _logger.LogError("WithdrawCardDetails - [FetchError] requesturl:{0}, response:{1}", requestUrl, response);
                    return new 卡查询Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "获取失败"
                    };
                }

                if (!responseDict.ContainsKey("result"))
                {
                    return new 卡查询Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "获取失败"
                    };
                }
                string cardaddress = responseDict["result"]["address"];
                cardaddress = string.Concat(responseDict["result"]["bank_name"],"-",cardaddress);

                // Insert Into ofStatus
                SqlCommand cmd = new SqlCommand("" +
                    "INSERT INTO ofStatus(identifier,idtype,status,body,createtime,createperson) " +
                    "VALUES(@IDENTIFIER,@IDTYPE,@STATUS,@BODY,@CREATETIME,@CREATEPERSON");
                cmd.Parameters.AddWithValue("@IDENTIFIER", cardtosearch);
                cmd.Parameters.AddWithValue("@IDTYPE", 0);
                cmd.Parameters.AddWithValue("@STATUS", 1);
                cmd.Parameters.AddWithValue("@BODY", cardaddress);
                cmd.Parameters.AddWithValue("@CREATETIME", Common.CurrentTime);
                cmd.Parameters.AddWithValue("@CREATEPERSON", request.Admin);

                ofStatus tobeinserted = new ofStatus()
                {
                    identifier = cardtosearch,
                    idtype = 0,
                    status = 1,
                    body = cardaddress,
                    createtime = Common.CurrentTime,
                    createperson = request.Admin
                };

                var insertResult = connection.Insert<ofStatus>(tobeinserted);
                if (insertResult > 0)
                {
                    return new 卡查询Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = cardaddress
                    };
                }

                return new 卡查询Return()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "获取失败"
                };
            }
        }
    }
}

