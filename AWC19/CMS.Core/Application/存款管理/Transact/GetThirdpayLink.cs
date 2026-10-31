using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using ShareEntity.Models;
using AWC2019.Core.ViewModel;
using FluentValidation;
using System.Data.SqlClient;
using Dapper;
using AWC2019.Core.Validation;
using Newtonsoft.Json;
using CMS.Core.Application.提款管理;
using ShareEntity.OutputModels;
using System.Runtime.Serialization;
using System.Transactions;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using Dapper.Contrib.Extensions;
using System.Net;
//using RestSharp;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Ocsp;
using System.Web;
using Microsoft.IdentityModel.Tokens;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.存款管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class GetThirdpayLinkRequest : IRequest<ResponseModel<GetThirdpayLinkReturn>>
    {
        public string platform { get; set; }
        public float amount { get; set; }
        public string bank { get; set; }
        public string realname { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class GetThirdpayLinkReturn
    {
        public string link { get; set; }
        public string method { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class GetThirdpayLinkRequestValidator : AbstractValidator<GetThirdpayLinkRequest>
    {
        public GetThirdpayLinkRequestValidator()
        {
            RuleFor(x => x.platform).CharDigitalChinese(1,10);
            RuleFor(x => x.amount).FloatNonNegative();
            RuleFor(x => x.bank).CharDigitalChinese(1,12);
            RuleFor(x => x.realname).Realname();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class GetThirdpayLinkRequestHandler : BaseRequestHandler<GetThirdpayLinkRequest, ResponseModel<GetThirdpayLinkReturn>>
    {
        public GetThirdpayLinkRequestHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<GetThirdpayLinkReturn> Handle(GetThirdpayLinkRequest request)
        {
            WebClient wc = new WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;
            string link = string.Format("https://awfuncapp.azurewebsites.net/api/RequestPayment_V2?" +
                "name={0}&amount={1}&bankcode={2}&mainflag={3}&realname={4}",
                string.Concat("care", Guid.NewGuid().ToString().Replace("-","").Substring(0,5)), request.amount, request.bank, request.platform, request.realname);
            var result = wc.DownloadString(link);

            
            /*
                ["code"] = 0,
                ["data"] = new JObject()
                {
                    ["link"] = resultLink,
                    ["method"] = method
                },
                ["msg"] = "success"
             */

            var resultObj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);

                if (resultObj.ContainsKey("code") && resultObj["code"] == 0)
            {
                // Insert the request
                using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
                {
                    connection.Insert<ofDepositInternal>(new ofDepositInternal()
                    {
                        account = request.Admin,
                        depositmethod = "0",
                        amount = (decimal)request.amount,
                        requesttime = Common.CurrentTime,
                        merchantno = (string)resultObj["data"]["orderid"],
                        payplatform = long.Parse(request.platform),
                        bankname = request.bank,
                        status = 0
                    });
                }


                string linkString = resultObj["data"]["link"];
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(linkString);
                var encodeLink = System.Convert.ToBase64String(plainTextBytes);

                return new ResponseModel<GetThirdpayLinkReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "成功获取链接",
                    Data = new GetThirdpayLinkReturn()
                    {
                        link = resultObj["data"]["method"] == "JSON" ? linkString : encodeLink,
                        method = resultObj["data"]["method"]
                    }
                };
            }

            return new ResponseModel<GetThirdpayLinkReturn>()
            {
                code = ShareEntity.Global.ResponseCode.code.FAIL,
                message = "链接获取失败",
                Data = null
            };
        }

    }
}

