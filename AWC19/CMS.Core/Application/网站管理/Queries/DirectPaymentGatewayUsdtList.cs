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
    public class DirectPaymentGatewayUsdtList : IRequest<ResponseModel<IEnumerable<DirectPaymentGatewayUsdtListReturn>>>
    {
    }

    public class DirectPaymentGatewayUsdtListReturn
    {
        public string name { get; set; }
        public bool status { get; set; }
        //    public bool statusregular{ get; set; }
        //    public bool statusregulartop{ get; set; }
        //    public bool statusvip{ get; set; }
        //    public bool statusvip3{ get; set; }
        //    public bool statusviptop { get; set; }
        //    public bool statusvip3top { get; set; }
        //    public bool statusnewuser { get; set; }
        //    public bool statusnewuser_top { get; set; }
        //    public bool statusaff { get; set; }
        //}

        /// <summary>
        /// API Model Validation
        /// </summary>
        public class DirectPaymentGatewayUsdtListValidator : AbstractValidator<DirectPaymentGatewayUsdtList>
        {
            public DirectPaymentGatewayUsdtListValidator()
            {
            }
        }

        /// <summary>
        /// Request Handler
        /// </summary>
        public class DirectPaymentGatewayUsdtListHandler : BaseRequestHandler<DirectPaymentGatewayUsdtList, ResponseModel<IEnumerable<DirectPaymentGatewayUsdtListReturn>>>
        {
            public DirectPaymentGatewayUsdtListHandler(IMapper mapper, ILoggerFactory logger)
                : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


            protected override ResponseModel<IEnumerable<DirectPaymentGatewayUsdtListReturn>> Handle(DirectPaymentGatewayUsdtList request)
            {

                using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
                {
                    var payment_directpay = connection.Get<ofSettings>("PAYMENT_DIRECTPAY").body;
                    JArray obj = JArray.Parse(payment_directpay);
                    var retVal = new List<DirectPaymentGatewayUsdtListReturn>();
                    foreach (JObject item in obj)
                    {
                        if (item.ContainsKey("currency") && item.ContainsKey("status"))
                        {
                            retVal.Add(new DirectPaymentGatewayUsdtListReturn
                            {
                                name = (string)item["key"],
                                status = (bool)item["status"],
                            });
                        }
                    }

                    return new ResponseModel<IEnumerable<DirectPaymentGatewayUsdtListReturn>>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "",
                        Data = retVal
                    };
                }
            }
        }
    }
}

