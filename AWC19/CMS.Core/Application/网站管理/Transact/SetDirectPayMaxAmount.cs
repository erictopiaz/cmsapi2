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
    public class SetDirectPayMaxAmount : IRequest<ResponseModel<SetDirectPayMaxAmountReturn>>
    {

        public string? currencyCode { get; set; } = "156";
        public string vip { get; set; }
        public string regular { get; set; }
        public string newuser { get; set; }
        public string vip3 { get; set; }
        public string aff { get; set; }
    }

    public class SetDirectPayMaxAmountReturn: ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class SetDirectPayMaxAmountValidator : AbstractValidator<SetDirectPayMaxAmount>
    {
        public SetDirectPayMaxAmountValidator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class SetDirectPayMaxAmountHandler : BaseRequestHandler<SetDirectPayMaxAmount, ResponseModel<SetDirectPayMaxAmountReturn>>
    {
        public SetDirectPayMaxAmountHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<SetDirectPayMaxAmountReturn> Handle(SetDirectPayMaxAmount request)
        {

            if (request.currencyCode == "356")
            {
                return INR(request);
            }


            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                //var vip = connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_VIP");
                //vip.body = request.vip.ToString("0");
                //var vipresult = connection.Update(vip);

                //var vip3 = connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_VIP3");
                //vip3.body = request.vip3.ToString("0");
                //var vip3result = connection.Update(vip3);
                //bool vip3result = false;
                //bool vipresult = false;
                //bool regularresult = false;


                if (!string.IsNullOrEmpty(request.aff))
                {
                    var paymentdirectpaydetails = connection.Get<ofSettings>("PAYMENT_DIRECTPAYDETAILS");
                    var paymentdirectpaydetails_JOBJ = JObject.Parse(paymentdirectpaydetails.body);
                    paymentdirectpaydetails_JOBJ["aff_maxamount"] = request.aff;
                    paymentdirectpaydetails.body = paymentdirectpaydetails_JOBJ.ToString(Formatting.None);
                    var affresult = connection.Update(paymentdirectpaydetails);
                }

                if (request.vip3.Contains(',') && request.vip3.Split(',').Length == 2)
                {
                    // check if the first is greater than the second
                    float vip3_low = 0;
                    float vip3_top = 0;

                    if (float.TryParse(request.vip3.Split(',')[0], out vip3_low) &&
                        float.TryParse(request.vip3.Split(',')[1], out vip3_top))
                    {
                        if (vip3_low < vip3_top)
                        {
                            var vip3low = connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_vip3");
                            vip3low.body = vip3_low.ToString("0");
                            var vip3lowresult = connection.Update(vip3low);

                            var vip3top = connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_vip3_TOP");
                            vip3top.body = vip3_top.ToString("0");
                            var vip3topresult = connection.Update(vip3top);

                            //if (vip3lowresult && vip3topresult)
                            //{
                            //    return new ResponseModel<SetDirectPayMaxAmountReturn>()
                            //    {
                            //        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                            //        message = "",
                            //    };
                            //}

                        }
                    }

                }


                if (request.vip.Contains(',') && request.vip.Split(',').Length == 2)
                {
                    // check if the first is greater than the second
                    float vip_low = 0;
                    float vip_top = 0;

                    if (float.TryParse(request.vip.Split(',')[0], out vip_low) &&
                        float.TryParse(request.vip.Split(',')[1], out vip_top))
                    {
                        if (vip_low < vip_top)
                        {
                            var viplow = connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_VIP");
                            viplow.body = vip_low.ToString("0");
                            var viplowresult = connection.Update(viplow);

                            var viptop = connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_VIP_TOP");
                            viptop.body = vip_top.ToString("0");
                            var viptopresult = connection.Update(viptop);

                            //if (viplowresult && viptopresult && viplowresult && vip3result)
                            //{
                            //    return new ResponseModel<SetDirectPayMaxAmountReturn>()
                            //    {
                            //        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                            //        message = "",
                            //    };
                            //}

                        }
                    }

                }


                if (request.regular.Contains(',') && request.regular.Split(',').Length == 2) {
                    // check if the first is greater than the second
                    float regular_low = 0;
                    float regular_top = 0;

                    if (float.TryParse(request.regular.Split(',')[0], out regular_low) &&
                        float.TryParse(request.regular.Split(',')[1], out regular_top)){
                        if (regular_low < regular_top) { 
                            var regularlow = connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY");
                            regularlow.body = regular_low.ToString("0");
                            var regularlowresult = connection.Update(regularlow);

                            var regulartop = connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_TOP");
                            regulartop.body = regular_top.ToString("0");
                            var regulartopresult = connection.Update(regulartop);

                            //if (vipresult && regulartopresult && regularlowresult && vip3result)
                            //{
                            //    return new ResponseModel<SetDirectPayMaxAmountReturn>()
                            //    {
                            //        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                            //        message = "",
                            //    };
                            //}

                        }
                    }

                }

                if (request.newuser.Contains(',') && request.newuser.Split(',').Length == 2) {
                    // check if the first is greater than the second
                    float regular_low = 0;
                    float regular_top = 0;

                    if (float.TryParse(request.newuser.Split(',')[0], out regular_low) &&
                        float.TryParse(request.newuser.Split(',')[1], out regular_top)){
                        if (regular_low < regular_top) { 
                            var regularlow = connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_NEWUSER");
                            regularlow.body = regular_low.ToString("0");
                            var regularlowresult = connection.Update(regularlow);

                            var regulartop = connection.Get<ofSettings>("PAYMENT_MAXDIRECTPAY_NEWUSER_TOP");
                            regulartop.body = regular_top.ToString("0");
                            var regulartopresult = connection.Update(regulartop);

                            //if (vipresult && regulartopresult && regularlowresult && vip3result)
                            //{
                            //    return new ResponseModel<SetDirectPayMaxAmountReturn>()
                            //    {
                            //        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                            //        message = "",
                            //    };
                            //}

                        }
                    }

                }

                //if (vipresult && regularresult && vip3result) {
                //    return new ResponseModel<SetDirectPayMaxAmountReturn>()
                //    {
                //        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                //        message = "",
                //    };
                //}
                return new ResponseModel<SetDirectPayMaxAmountReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                };

                //return new ResponseModel<SetDirectPayMaxAmountReturn>()
                //{
                //    code = ShareEntity.Global.ResponseCode.code.FAIL,
                //    message = "",
                //};

            }
        }

        private ResponseModel<SetDirectPayMaxAmountReturn> INR(SetDirectPayMaxAmount request)
        {
            return new ResponseModel<SetDirectPayMaxAmountReturn>()
            {
                code = ShareEntity.Global.ResponseCode.code.FAIL,
                message = "Not Available for INR Currency"
            };
        }
    }
}

