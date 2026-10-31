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
    public class DirectPayMaxAmountList : IRequest<ResponseModel<DirectPayMaxAmountListReturn>>
    {
    }

    public class DirectPayMaxAmountListReturn
    {
        public string regularamount{ get; set; }
        public string newuseramount { get; set; }
        public string vipamount { get; set; }
        public string vip3amount { get; set; }
        public string affamount { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class DirectPayMaxAmountListValidator : AbstractValidator<DirectPayMaxAmountList>
    {
        public DirectPayMaxAmountListValidator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class DirectPayMaxAmountListHandler : BaseRequestHandler<DirectPayMaxAmountList, ResponseModel<DirectPayMaxAmountListReturn>>
    {
        public DirectPayMaxAmountListHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<DirectPayMaxAmountListReturn> Handle(DirectPayMaxAmountList request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var viplow = connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_VIP").body;
                var viptop = connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_VIP_TOP").body;
                
                var vip3low = connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_VIP3").body;
                var vip3top = connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_VIP3_TOP").body;

                var regularlow = connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY").body;
                var regulartop = connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_TOP").body;
                
                var newuserlow = connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_NEWUSER").body;
                var newusertop = connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_NEWUSER_TOP").body;


                var paymentdirectpaydetails = connection.Get<ofSettings>("PAYMENT_DIRECTPAYDETAILS");
                var paymentdirectpaydetails_JOBJ = JObject.Parse(paymentdirectpaydetails.body);
                var affamount = paymentdirectpaydetails_JOBJ["aff_maxamount"].ToString();

                return new ResponseModel<DirectPayMaxAmountListReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new DirectPayMaxAmountListReturn()
                    {
                        newuseramount = string.Concat(newuserlow, ",", newusertop),
                        regularamount = string.Concat(regularlow,",", regulartop),
                        vipamount = string.Concat(viplow, ",", viptop),
                        vip3amount = string.Concat(vip3low, ",", vip3top),
                        affamount = affamount
                        //vipamount = float.Parse(vip),
                        //vip3amount = float.Parse(vip3)
                    }
                };
            }
        }
    }
}

