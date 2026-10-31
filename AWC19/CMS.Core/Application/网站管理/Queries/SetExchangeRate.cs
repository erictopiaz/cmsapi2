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
    public class SetExchangeRate : IRequest<ResponseModel<SetExchangeRateReturn>>
    {
        public string? currencyCode { get; set; } = "156";
        public float usdt_deposit_xrate { get; set; }
        public float usdt_withdraw_xrate { get; set; }
        public float usdt_deposit_topup { get; set; }
        public float usdt_withdraw_topup { get; set; }
        //public float usdt_market_xrate { get; set; }
    }

    public class SetExchangeRateReturn
    {
        public float usdt_deposit_xrate { get; set; }
        public float usdt_withdraw_xrate { get; set; }
        public float usdt_market_xrate { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class SetExchangeRateValidator : AbstractValidator<SetExchangeRate>
    {
        public SetExchangeRateValidator()
        {
            RuleFor(x => x.usdt_deposit_topup).LessThan(2).WithMessage("deposit topup too high");
            RuleFor(x => x.usdt_withdraw_topup).LessThan(2).WithMessage("withdraw topup too high");
            RuleFor(x => x.usdt_deposit_topup).GreaterThan(-2).WithMessage("deposit topup too low");
            RuleFor(x => x.usdt_withdraw_topup).GreaterThan(-2).WithMessage("withdraw topup too low");

        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class SetExchangeRateHandler : BaseRequestHandler<SetExchangeRate, ResponseModel<SetExchangeRateReturn>>
    {
        public SetExchangeRateHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        private ResponseModel<SetExchangeRateReturn> INR(SetExchangeRate request) {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {

                var payment_crypto = connection.Get<ofSettings>("PAYMENT_CRYPTO");
                //var payment_crypto_temp = connection.Get<ofSettings>("PAYMENT_CRYPTO_TEMP");
                var payment_crypto_jobj = JObject.Parse(payment_crypto.body);
                //var jobj = JObject.Parse(payment_crypto_temp.body);
                float market_xrate = float.Parse(payment_crypto_jobj["usdt_xrate"].ToString());
                payment_crypto_jobj["inr_usdt_xrate_withdraw_topup"] = request.usdt_withdraw_topup;
                payment_crypto_jobj["inr_usdt_xrate_deposit_topup"] = request.usdt_deposit_topup;
                payment_crypto.body = payment_crypto_jobj.ToString(Formatting.None);
                connection.Update(payment_crypto);
                return new ResponseModel<SetExchangeRateReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = ""
                };
            }

        }

        protected override ResponseModel<SetExchangeRateReturn> Handle(SetExchangeRate request)
        {
            if (request.currencyCode == "356") {
                return INR(request);
            }

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {

                var payment_crypto = connection.Get<ofSettings>("PAYMENT_CRYPTO");
                //var payment_crypto_temp = connection.Get<ofSettings>("PAYMENT_CRYPTO_TEMP");
                var payment_crypto_jobj = JObject.Parse(payment_crypto.body);
                //var jobj = JObject.Parse(payment_crypto_temp.body);
                float market_xrate = float.Parse(payment_crypto_jobj["usdt_xrate"].ToString());
                payment_crypto_jobj["usdt_xrate_withdraw_topup"] = request.usdt_withdraw_topup;
                payment_crypto_jobj["usdt_xrate_deposit_topup"] = request.usdt_deposit_topup;
                payment_crypto.body = payment_crypto_jobj.ToString(Formatting.None);
                connection.Update(payment_crypto);
                return new ResponseModel<SetExchangeRateReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = ""
                };
            }
        }
    }
}