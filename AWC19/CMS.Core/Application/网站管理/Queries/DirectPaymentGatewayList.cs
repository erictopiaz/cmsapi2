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
    public class DirectPaymentGatewayList : IRequest<ResponseModel<IEnumerable<DirectPaymentGatewayListReturn>>>
    {
    }

    public class DirectPaymentGatewayListReturn
    {
        public string name { get; set; }
        public bool statusregular{ get; set; }
        public bool statusregulartop{ get; set; }
        public bool statusvip{ get; set; }
        public bool statusvip3{ get; set; }
        public bool statusviptop { get; set; }
        public bool statusvip3top { get; set; }
        public bool statusnewuser { get; set; }
        public bool statusnewuser_top { get; set; }
        public bool statusaff { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class DirectPaymentGatewayListValidator : AbstractValidator<DirectPaymentGatewayList>
    {
        public DirectPaymentGatewayListValidator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class DirectPaymentGatewayListHandler : BaseRequestHandler<DirectPaymentGatewayList, ResponseModel<IEnumerable<DirectPaymentGatewayListReturn>>>
    {
        public DirectPaymentGatewayListHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<IEnumerable<DirectPaymentGatewayListReturn>> Handle(DirectPaymentGatewayList request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var vip3 = connection.Get<ofSettings>("PAYMENT_PLATFORM_VIP3").body;
                var vip = connection.Get<ofSettings>("PAYMENT_PLATFORM_VIP").body;
                var vip3top = connection.Get<ofSettings>("PAYMENT_PLATFORM_VIP3_TOP").body;
                var viptop = connection.Get<ofSettings>("PAYMENT_PLATFORM_VIP_TOP").body;
                var regular = connection.Get<ofSettings>("PAYMENT_PLATFORM").body;
                var regulartop = connection.Get<ofSettings>("PAYMENT_PLATFORM_TOP").body;
                var newuser = connection.Get<ofSettings>("PAYMENT_PLATFORM_NEWUSER").body;
                var newuser_top = connection.Get<ofSettings>("PAYMENT_PLATFORM_NEWUSER_TOP").body;

                var paymentdirectpaydetails = connection.Get<ofSettings>("PAYMENT_DIRECTPAYDETAILS");
                var paymentdirectpaydetails_JOBJ = JObject.Parse(paymentdirectpaydetails.body);
                var aff = paymentdirectpaydetails_JOBJ["aff_payplatform"].ToString();


                var retVal = new List<DirectPaymentGatewayListReturn>();
                foreach (var item in CachedData.PaymentGatewayList) {
                        retVal.Add(new DirectPaymentGatewayListReturn {
                            name = item,
                            statusregular = item == regular,
                            statusregulartop = item == regulartop,
                            statusvip = item == vip,
                            statusvip3 = item == vip3,
                            statusviptop = item == viptop,
                            statusvip3top = item == vip3top,
                            statusnewuser = item == newuser,
                            statusnewuser_top = item == newuser_top, 
                            statusaff = item == aff
                        });
                }

                return new ResponseModel<IEnumerable<DirectPaymentGatewayListReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = retVal
                };
            }
        }
    }
}

