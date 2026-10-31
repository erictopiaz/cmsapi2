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
using MongoDB.Driver;
using ShareEntity.Models;

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 更新存款条件 : IRequest<更新存款条件Return>
    {
        public long id { get; set; }
        public string account { get; set; }
        public string[] depositrules { get; set; }
        /// <summary>
        /// 0:网银定额限制提交，1：支付宝定额限制提交，2：微信定额限制提交
        /// 默认为 0
        /// </summary>
        public int limitType { get; set; } = 0;
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 更新存款条件Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 更新存款条件Validator : AbstractValidator<更新存款条件>
    {
        public 更新存款条件Validator()
        {
            RuleFor(x => x.account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 更新存款条件Handler : BaseAccount<更新存款条件, 更新存款条件Return>
    {
        public 更新存款条件Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger)
        {
        }


        protected override 更新存款条件Return Handle(更新存款条件 request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));
            lock(this) {
                if (request.limitType == 1) {
                    return AlipayDepositRules(request);
                }
                else if (request.limitType == 2)
                {
                    return WXDepositRules(request);
                    //return new 更新存款条件Return()
                    //{
                    //    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    //    message = $"更新成功,error limitType 暂末开放!!"
                    //};
                }
                else if (request.limitType == 0) {
                    return BankDepositRules(request);
                }
                else {
                    return new 更新存款条件Return() {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = $"更新成功,error limitType {request.limitType}"
                    };
                }
            }
            
        }

        private 更新存款条件Return BankDepositRules(更新存款条件 request) {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING)) {
                string account = request.account;
                if (request.id > 0) {
                    var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                    account = accountRow.account;
                }

                var userDetail = connection.Get<ofUser>(account);

                if (!string.IsNullOrEmpty(userDetail.misc)) {
                    string misc = userDetail.misc;
                    var miscDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(misc);

                    if (miscDict.ContainsKey("depositrules"))
                        miscDict["depositrules"] = string.Join(",", request.depositrules);
                    else
                        miscDict.Add("depositrules", string.Join(",", request.depositrules));

                    string miscReconvert = Newtonsoft.Json.JsonConvert.SerializeObject(miscDict);
                    userDetail.misc = miscReconvert;
                }
                else {
                    string miscReconvert = Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, string>() {
                        { "depositrules", string.Join(",", request.depositrules) }
                    });
                    userDetail.misc = miscReconvert;
                }

                bool updateResult = connection.Update<ofUser>(userDetail);
                _logger.LogInformation("UpdateDepositRules - account:{0}, result:{1}",
                    request.account,
                    updateResult.ToString());

                return new 更新存款条件Return() {
                    code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "更新成功"
                };
            }

        }
        private 更新存款条件Return AlipayDepositRules(更新存款条件 request) {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING)) {
                string account = request.account;
                if (request.id > 0) {
                    var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                    account = accountRow.account;
                }

                var userDetail = connection.Get<ofUser>(account);
                if (request.depositrules.Length >= 2) {
                    return new 更新存款条件Return() {
                        code =  ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "更新失败，请选择限定条件"
                    };
                }
                if (!string.IsNullOrEmpty(userDetail.misc)) {
                    string misc = userDetail.misc;
                    var miscDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(misc);
                    var rules = request.depositrules.Length > 0 ?request.depositrules.First():"";
                    if (miscDict.ContainsKey("alipaydepositrules"))
                        miscDict["alipaydepositrules"] = rules;
                    else
                        miscDict.Add("alipaydepositrules", rules);

                    string miscReconvert = Newtonsoft.Json.JsonConvert.SerializeObject(miscDict);
                    userDetail.misc = miscReconvert;
                }
                else {
                    string miscReconvert = Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, string>() {
                        { "alipaydepositrules", string.Join(",", request.depositrules) }
                    });
                    userDetail.misc = miscReconvert;
                }

                bool updateResult = connection.Update<ofUser>(userDetail);
                _logger.LogInformation("UpdateAlipayDepositRules - account:{0}, result:{1}",
                    request.account,
                    updateResult.ToString());

                return new 更新存款条件Return() {
                    code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "更新成功"
                };
            }

        }
        private 更新存款条件Return WXDepositRules(更新存款条件 request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                string account = request.account;
                if (request.id > 0)
                {
                    var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                    account = accountRow.account;
                }

                var userDetail = connection.Get<ofUser>(account);
                if (request.depositrules.Length >= 2)
                {
                    return new 更新存款条件Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "更新失败，请选择限定条件"
                    };
                }
                if (!string.IsNullOrEmpty(userDetail.misc))
                {
                    string misc = userDetail.misc;
                    var miscDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(misc);
                    var rules = request.depositrules.Length > 0 ? request.depositrules.First() : "";
                    if (miscDict.ContainsKey("wxdepositrules"))
                        miscDict["wxdepositrules"] = rules;
                    else
                        miscDict.Add("wxdepositrules", rules);

                    string miscReconvert = Newtonsoft.Json.JsonConvert.SerializeObject(miscDict);
                    userDetail.misc = miscReconvert;
                }
                else
                {
                    string miscReconvert = Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, string>() {
                        { "wxdepositrules", string.Join(",", request.depositrules) }
                    });
                    userDetail.misc = miscReconvert;
                }

                bool updateResult = connection.Update<ofUser>(userDetail);
                _logger.LogInformation("UpdateAlipayDepositRules - account:{0}, result:{1}",
                    request.account,
                    updateResult.ToString());

                return new 更新存款条件Return()
                {
                    code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "更新成功"
                };
            }

        }
    }
}

