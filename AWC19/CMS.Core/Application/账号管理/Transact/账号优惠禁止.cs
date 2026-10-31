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
using ShareEntity.InputModels.Promo;
using ShareEntity.Models;

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 账号优惠禁止 : IRequest<账号优惠禁止Return>
    {
        public string id { get; set; }
        public string Account { get; set; }
        public string promodisabled { get; set; } = "";

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 账号优惠禁止Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 账号优惠禁止Validator : AbstractValidator<账号优惠禁止>
    {
        public 账号优惠禁止Validator()
        {
            RuleFor(x => x.Account).Account();
            //RuleFor(x => x.Password).Password();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 账号优惠禁止Handler : BaseAccount<账号优惠禁止, 账号优惠禁止Return>
    {
        public 账号优惠禁止Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override 账号优惠禁止Return Handle(账号优惠禁止 request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                if (!string.IsNullOrEmpty(request.id))
                {
                    var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                    request.Account = accountRow.account;
                }

                var userDetail = connection.Get<ofUser>(request.Account);
                if (userDetail == null)
                    return new 账号优惠禁止Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "账号不存在"
                    };

                if (!string.IsNullOrEmpty(userDetail.misc))
                {
                    string misc = userDetail.misc;
                    var miscDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(misc);

                    // contains promodisabled
                    if (miscDict.ContainsKey("promodisabled"))
                    {
                        miscDict["promodisabled"] = request.promodisabled.ToString().ToLower();
                        if (!string.IsNullOrEmpty(request.promodisabled) && request.promodisabled != "[]")
                            userDetail.anomaly = 1;
                        else
                            userDetail.anomaly = !miscDict.ContainsKey("gamedisabledstat") ? 0 : 1;
                    }

                    // promodisabled == true 
                    else if (!string.IsNullOrEmpty(request.promodisabled) && request.promodisabled != "[]" && request.promodisabled != "null") { 
                        miscDict.Add("promodisabled", request.promodisabled);
                        userDetail.anomaly = 1;
                    }

                    if (string.IsNullOrEmpty(request.promodisabled) || request.promodisabled == "[]") { 
                        miscDict.Remove("promodisabled");
                        userDetail.anomaly = !miscDict.ContainsKey("gamedisabledstat") ? 0 : 1;

                    }

                    string miscReconvert = Newtonsoft.Json.JsonConvert.SerializeObject(miscDict);
                    userDetail.misc = miscReconvert;
                }
                else
                {
                    //ching add 2022 01 17
                    var value = string.Format(@"[{0}]", request.promodisabled);
                    var misc = new Dictionary<string, string>()
                    {
                        { "promodisabled", value }
                    };
                    userDetail.misc = Newtonsoft.Json.JsonConvert.SerializeObject(misc);
                }

                bool updateResult = connection.Update<ofUser>(userDetail);
                return new 账号优惠禁止Return()
                {
                    code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = ""
                };
            }
        }
    }
}

