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
using ShareEntity.OutputModels;
using System.Runtime.Serialization;
using System.Transactions;
using Microsoft.Extensions.Logging;
using CMS.Core;
using NSwag.Annotations;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class ThirdpayGroupSwitch : IRequest<ThirdpayGroupSwitchReturn>
    {
        public long id { get; set; }
        public bool onoff { get; set; } 
        public string group{ get; set; } 
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class ThirdpayGroupSwitchReturn: ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class ThirdpayGroupSwitchValidator : AbstractValidator<ThirdpayGroupSwitch>
    {
        public ThirdpayGroupSwitchValidator()
        {
            RuleFor(x => x.id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
            RuleFor(x => x.group).CharAndDigital(1, 20);
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class ThirdpayGroupSwitchHandler : BaseRequestHandler<ThirdpayGroupSwitch, ThirdpayGroupSwitchReturn>
    {
        public ThirdpayGroupSwitchHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ThirdpayGroupSwitchReturn Handle(ThirdpayGroupSwitch request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var id = request.id;
                var row = connection.Get<ofThirdpay_WithoutClass>(id);
                string miscJobj = row.misc;
                JObject jobj=new JObject();
                if (!string.IsNullOrEmpty(miscJobj))
                    jobj = JObject.Parse(miscJobj);

                List<string> levelList = new List<string>( row.levels.Split(','));

                switch (request.group)
                {
                    case "regulargroup":
                        levelList.Remove("10001");
                        levelList.Remove("10007");
                        levelList.Remove("10026");
                        levelList.Remove("10027");
                        if (request.onoff) {
                            levelList.Add("10001");
                            levelList.Add("10007");
                            levelList.Add("10026");
                            levelList.Add("10027");
                        }
                        break;
                    case "vipgroup":
                        levelList.Remove("10028");
                        levelList.Remove("10002");
                        levelList.Remove("10011");
                        if (request.onoff)
                        {
                            levelList.Add("10028");
                            levelList.Add("10002");
                            levelList.Add("10011");
                        }
                        break;
                    case "vip3group":
                        levelList.Remove("10003");
                        levelList.Remove("10005");
                        levelList.Remove("10004");
                        levelList.Remove("10025");
                        if (request.onoff)
                        {
                            levelList.Add("10003");
                            levelList.Add("10005");
                            levelList.Add("10004");
                            levelList.Add("10025");
                        }
                        break;
                    case "agentgroup":
                        row.isAgentUsed = request.onoff ? 1: 0;
                        bool updateresult_agent = connection.Update(row);

                        return new ThirdpayGroupSwitchReturn()
                        {
                            code = updateresult_agent ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                            message = ""
                        };
                        break;
                    default:
                        return null;
                        break;
                }
                jobj[request.group] = request.onoff;
                row.levels = string.Join(',', levelList);

                

                miscJobj = jobj.ToString(Formatting.None);
                row.misc = miscJobj;
                bool updateresult = connection.Update(row);
                
                return new ThirdpayGroupSwitchReturn()
                {
                    code = updateresult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }

        }
    }
}

