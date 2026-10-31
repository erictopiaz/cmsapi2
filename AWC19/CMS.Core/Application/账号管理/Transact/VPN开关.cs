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
using ShareEntity.Models;

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class VPN开关 : IRequest<VPN开关Return>
    {
        public string id { get; set; }
        public string Account { get; set; }
        public int status { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class VPN开关Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class VPN开关Validator : AbstractValidator<VPN开关>
    {
        public VPN开关Validator()
        {
            RuleFor(x => x.Account).Account();
            RuleFor(x => x.id).DigitalNullable(1000, 100000000);
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class VPN开关Handler : BaseAccount<VPN开关, VPN开关Return>
    {
        public VPN开关Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger){}


        protected override VPN开关Return Handle(VPN开关 request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                string account = "";
                if (!string.IsNullOrEmpty(request.id))
                {
                    var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                    account = accountRow.account;
                }
                else
                {
                    return new VPN开关Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "账号不存在"
                    };
                }
                var userDetail = connection.Get<ofUser>(account);

                if (!string.IsNullOrEmpty(userDetail.misc))
                {
                    string misc = userDetail.misc;
                    var miscDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(misc);

                    if (miscDict.ContainsKey("allowopenvpn"))
                        miscDict["allowopenvpn"] = request.status.ToString() == "1" ? "1" : "0";
                    else
                        miscDict.Add("allowopenvpn", request.status.ToString() == "1" ? "1" : "0");

                    string miscReconvert = Newtonsoft.Json.JsonConvert.SerializeObject(miscDict);
                    userDetail.misc = miscReconvert;
                }
                else
                {
                    string miscReconvert = Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, string>() {
                        { "allowopenvpn", request.status.ToString() == "1" ? "1" : "0" }
                    });
                    userDetail.misc = miscReconvert;
                }

                bool updateResult = connection.Update<ofUser>(userDetail);

                return new VPN开关Return()
                {
                    code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "更新成功"
                };
            }
        }
    }
}

