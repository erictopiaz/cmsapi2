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
using CMS.Core.Application.提款管理;
using ShareEntity.OutputModels;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using CMS.Core;
using CMS.Core.ViewModel;
using Dapper.Contrib.Extensions;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class SetDirectPayMaxAmountV2 : IRequest<ResponseModel<SetDirectPayMaxAmountV2Return>>
    {
        public float vip_low { get; set; }
        public float vip_hi { get; set; }
        public float vip3_low { get; set; }
        public float vip3_hi { get; set; }
        public float regular_low { get; set; }
        public float regular_hi { get; set; }
        public float newusr_low { get; set; }
        public float newusr_hi { get; set; }
        public float usdt_low { get; set; }
        public float usdt_hi { get; set; }
        public float aff { get; set; } = 0f;
        public string category { get; set; } = "bank";

    }

    public class SetDirectPayMaxAmountV2Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class SetDirectPayMaxAmountV2Validator : AbstractValidator<SetDirectPayMaxAmountV2>
    {
        public SetDirectPayMaxAmountV2Validator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class SetDirectPayMaxAmountV2Handler : BaseRequestHandler<SetDirectPayMaxAmountV2, ResponseModel<SetDirectPayMaxAmountV2Return>>
    {
        public SetDirectPayMaxAmountV2Handler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<SetDirectPayMaxAmountV2Return> Handle(SetDirectPayMaxAmountV2 request)
        {
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var directpay_details = connection.Get<ofSettings>("PAYEMENT_DIRECTPAY_V2");
                JObject jobj = JObject.Parse(directpay_details.body);
                //JObject level_cat = jobj["level_cat"];

                switch (request.category)
                {
                    case "bank":
                        jobj["level_cat"]["newusr_low"]["maxamount"] = request.newusr_low.ToString("0");
                        jobj["level_cat"]["newusr_hi"]["maxamount"] = request.newusr_hi.ToString("0");

                        jobj["level_cat"]["vip_low"]["maxamount"] = request.vip_low.ToString("0");
                        jobj["level_cat"]["vip_hi"]["maxamount"] = request.vip_hi.ToString("0");

                        jobj["level_cat"]["vip3_low"]["maxamount"] = request.vip3_low.ToString("0");
                        jobj["level_cat"]["vip3_hi"]["maxamount"] = request.vip3_hi.ToString("0");

                        jobj["level_cat"]["regular_low"]["maxamount"] = request.regular_low.ToString("0");
                        jobj["level_cat"]["regular_hi"]["maxamount"] = request.regular_hi.ToString("0");

                        jobj["level_cat"]["aff"]["maxamount"] = request.aff.ToString("0");
                        break;
                    case "usdt":

                        jobj["level_cat"]["usdt_low"]["maxamount"] = request.usdt_low.ToString("0");
                        jobj["level_cat"]["usdt_hi"]["maxamount"] = request.usdt_hi.ToString("0");

                        break;
                        
                    default:
                        break;
                }
                //for (int i = 0; i < jarr.Count(); i++)
                //{
                //    if (jobj["payplatform"][i]["key"].ToString() == request.key)
                //    {
                //        jobj["payplatform"][i]["feecent"] = request.feecent;

                //    }
                //}
                directpay_details.body = jobj.ToString(Formatting.None);

                var result = connection.Update(directpay_details);
                if (result)
                    return new ResponseModel<SetDirectPayMaxAmountV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "更新成功"
                    };
                else {
                    return new ResponseModel<SetDirectPayMaxAmountV2Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "更新失败"
                    };
                }
                return null;

            }
        }
    }
}

