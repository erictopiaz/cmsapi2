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
using Newtonsoft.Json.Linq;
using ShareEntity.Models;

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 更新标记 : IRequest<更新标记Return>
    {
        public long id { get; set; }
        public string account { get; set; }
        public string tags { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 更新标记Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 更新标记Validator : AbstractValidator<更新标记>
    {
        public 更新标记Validator()
        {
            RuleFor(x => x.account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 更新标记Handler : BaseAccount<更新标记, 更新标记Return>
    {
        public 更新标记Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger)
        {
        }


        protected override 更新标记Return Handle(更新标记 request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {

                if (request.id > 0)
                {
                    var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                    request.account = accountRow.account;
                }
                var userDetail = connection.Get<ofUser>(request.account);



                //string account = request.account;
                //if (!string.IsNullOrEmpty(request.id))
                //{
                //    var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                //    account = accountRow.account;
                //}

                if (userDetail == null) 
                    return new 更新标记Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "账号不存在"
                    };

                //var userDetail = connection.Get<ofUser>(account);

                if (!string.IsNullOrEmpty(userDetail.misc))
                {
                    string misc = userDetail.misc;
                    var miscDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(misc);

                    if (miscDict.ContainsKey("tags"))
                        miscDict["tags"] = request.tags;
                    else
                        miscDict.Add("tags", request.tags);

                    string miscReconvert = Newtonsoft.Json.JsonConvert.SerializeObject(miscDict);
                    userDetail.misc = miscReconvert;
                }
                else { 
                    string miscReconvert = Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, string>() {
                        { "tags", request.tags }
                    });
                    userDetail.misc = miscReconvert;
                }

                bool updateResult = connection.Update<ofUser>(userDetail);

                return new 更新标记Return()
                {
                    code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "更新成功"
                };
            }
        }
    }
}

