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
    public class DirectPaySettingsUpdate : IRequest<DirectPaySettingsUpdateReturn>
    {

        public string? currencyCode { get; set; } = "156";
        public string key { get; set; }
        public float feecent { get; set; }
        public int maxamount { get; set; } 
        public int[] level { get; set; }
    }

    public class DirectPaySettingsUpdateReturn: ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class DirectPaySettingsUpdateValidator : AbstractValidator<DirectPaySettingsUpdate>
    {
        public DirectPaySettingsUpdateValidator()
        {
            //RuleFor(x => x.Id)
            //    .NotEmpty().WithMessage("ID不可为空")
            //    .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class DirectPaySettingsUpdateHandler : BaseRequestHandler<DirectPaySettingsUpdate, DirectPaySettingsUpdateReturn>
    {

        public DirectPaySettingsUpdateHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override DirectPaySettingsUpdateReturn Handle(DirectPaySettingsUpdate request)
        {

            if (request.currencyCode == "356")
            {
                return INR(request);
            }

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var payment_directpay = connection.Get<ofSettings>("PAYMENT_DIRECTPAY");
                var gatewaylist = JArray.Parse(payment_directpay.body);
                for (int i = 0; i < gatewaylist.Count; i++)
                {
                    if (gatewaylist[i]["key"].ToString() == request.key)
                    {
                        gatewaylist[i]["feecent"] = request.feecent.ToString("0.00");
                    }
                }
                string gatewaylistupdated = Newtonsoft.Json.JsonConvert.SerializeObject(gatewaylist, Formatting.None);
                payment_directpay.body = gatewaylistupdated;
                var updateResult = connection.Update<ofSettings>(payment_directpay);

                bool updateresult = true;
                return new DirectPaySettingsUpdateReturn()
                {
                    code = updateresult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }

        }

        private DirectPaySettingsUpdateReturn INR(DirectPaySettingsUpdate request)
        {
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var payment_directpay = connection.Get<ofSettings>("PAYEMENT_DIRECTPAY_V2_INR");
                var gatewaylist = JArray.Parse(payment_directpay.body);
                for (int i = 0; i < gatewaylist.Count; i++)
                {
                    if (gatewaylist[i]["key"].ToString() == request.key)
                    {
                        gatewaylist[i]["feecent"] = request.feecent.ToString("0.00");
                    }
                }
                string gatewaylistupdated = Newtonsoft.Json.JsonConvert.SerializeObject(gatewaylist, Formatting.None);
                payment_directpay.body = gatewaylistupdated;
                var updateResult = connection.Update<ofSettings>(payment_directpay);

                bool updateresult = true;
                return new DirectPaySettingsUpdateReturn()
                {
                    code = updateresult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }
        }
    }
}

