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
    public class 账号详细Update : IRequest<账号详细UpdateReturn>
    {
        public string id { get; set; }
        public string Account { get; set; }
        public int level { get; set; } = -1;
        public int sex { get; set; } = -1;
        public int status { get; set; } = -1;
        public string freezereason { get; set; }
        public string promodisabled { get; set; } = "";

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 账号详细UpdateReturn : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 账号详细UpdateValidator : AbstractValidator<账号详细Update>
    {
        public 账号详细UpdateValidator()
        {
            RuleFor(x => x.Account).Account();
            //RuleFor(x => x.Password).Password();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 账号详细UpdateHandler : BaseAccount<账号详细Update, 账号详细UpdateReturn>
    {
        public 账号详细UpdateHandler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override 账号详细UpdateReturn Handle(账号详细Update request)
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
                    return new 账号详细UpdateReturn()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "账号不存在"
                    };

                if (request.level > 0) userDetail.level = request.level;
                if (request.status >= 0) userDetail.status = request.status;
                if (request.sex >= 0) userDetail.sex = request.sex;

                if (!string.IsNullOrEmpty(request.freezereason)) userDetail.freezereason = request.freezereason;

                if (!string.IsNullOrEmpty(userDetail.misc))
                {
                    string misc = userDetail.misc;
                    var miscDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(misc);


                    if (!string.IsNullOrEmpty(request.promodisabled))
                    {
                        if (!request.promodisabled.StartsWith("[") && !request.promodisabled.StartsWith("]"))
                            request.promodisabled = string.Concat("[", request.promodisabled, "]");
                    }

                    // contains promodisabled
                    if (miscDict.ContainsKey("promodisabled"))
                        miscDict["promodisabled"] = request.promodisabled.ToString().ToLower();

                    // promodisabled == true 
                    else if (!string.IsNullOrEmpty(request.promodisabled) && request.promodisabled != "[]" && request.promodisabled != "null")
                        miscDict.Add("promodisabled", request.promodisabled);

                    string miscReconvert = Newtonsoft.Json.JsonConvert.SerializeObject(miscDict);
                    userDetail.misc = miscReconvert;
                }

                bool updateResult = connection.Update<ofUser>(userDetail);
                return new 账号详细UpdateReturn()
                {
                    code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = ""
                };
            }
        }
    }
}

