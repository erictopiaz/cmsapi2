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
    public class DirectPaySettingsUpdateV2 : IRequest<DirectPaySettingsUpdateV2Return>
    {

        public string? currencyCode { get; set; } = "156";
        public string key { get; set; }
        public float feecent { get; set; }
        public int feepertransaction { get; set; }
        public int maxamount { get; set; } 
        public string banks { get; set; } 
        public int[] level { get; set; }
    }

    public class DirectPaySettingsUpdateV2Return: ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class DirectPaySettingsUpdateV2Validator : AbstractValidator<DirectPaySettingsUpdateV2>
    {
        public DirectPaySettingsUpdateV2Validator()
        {
            //RuleFor(x => x.Id)
            //    .NotEmpty().WithMessage("ID不可为空")
            //    .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class DirectPaySettingsUpdateV2Handler : BaseRequestHandler<DirectPaySettingsUpdateV2, DirectPaySettingsUpdateV2Return>
    {

        public DirectPaySettingsUpdateV2Handler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override DirectPaySettingsUpdateV2Return Handle(DirectPaySettingsUpdateV2 request)
        {
            if (request.currencyCode == "356")
            {
                return INR(request);
            }

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var directpay_details = connection.Get<ofSettings>("PAYEMENT_DIRECTPAY_V2");
                JObject jobj = JObject.Parse(directpay_details.body);
                JArray jarr = (JArray)jobj["payplatform"];
                for (int i = 0; i < jarr.Count(); i++)
                {
                    if (jobj["payplatform"][i]["key"].ToString() == request.key)
                    {
                        jobj["payplatform"][i]["feecent"] = request.feecent;
                        jobj["payplatform"][i]["feepertransaction"] = request.feepertransaction;
                        jobj["payplatform"][i]["banks"] = JArray.FromObject(request.banks.Split(','));
                    }
                }


                //jobj["level_cat"][request.group]["payplatform"] = request.OnOff ? request.name : "";
                    string jsonString = jobj.ToString(Formatting.None);
                directpay_details.body = jsonString;

                bool result  = connection.Update(directpay_details);

                if(result)
                    return new DirectPaySettingsUpdateV2Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "更新成功"
                    };
                else
                    return new DirectPaySettingsUpdateV2Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "更新失败"
                    };
            }

        }

        private DirectPaySettingsUpdateV2Return INR(DirectPaySettingsUpdateV2 request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var directpay_details = connection.Get<ofSettings>("PAYEMENT_DIRECTPAY_V2_INR");
                JObject jobj = JObject.Parse(directpay_details.body);
                JArray jarr = (JArray)jobj["payplatform"];
                for (int i = 0; i < jarr.Count(); i++)
                {
                    if (jobj["payplatform"][i]["key"].ToString() == request.key)
                    {
                        jobj["payplatform"][i]["feecent"] = request.feecent;
                        jobj["payplatform"][i]["feepertransaction"] = request.feepertransaction;
                        jobj["payplatform"][i]["banks"] = JArray.FromObject(request.banks.Split(','));
                    }
                }


                //jobj["level_cat"][request.group]["payplatform"] = request.OnOff ? request.name : "";
                string jsonString = jobj.ToString(Formatting.None);
                directpay_details.body = jsonString;

                bool result = connection.Update(directpay_details);

                if (result)
                    return new DirectPaySettingsUpdateV2Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "更新成功"
                    };
                else
                    return new DirectPaySettingsUpdateV2Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "更新失败"
                    };
            }

        }
    }
}

