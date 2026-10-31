using AutoMapper;
using CMS.Data.Models;
using ShareEntity.OutputModels;
using Dapper.Contrib.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using ShareEntity.Models;

namespace CMS.Core.Application.网站管理.Queries {
   
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class DirectPaySettingListINR : IRequest<ResponseModel<DirectPaySettingListINRReturn>> {
    }

    public class DirectPaySettingListINRReturn {
        public string details { get; set; }
        public int directPayType { get; set; }

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class DirectPaySettingListINRValidator : AbstractValidator<DirectPaySettingListINR> {
        public DirectPaySettingListINRValidator() {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class DirectPaySettingListINRHandler : BaseRequestHandler<DirectPaySettingListINR, ResponseModel<DirectPaySettingListINRReturn>> {
        public DirectPaySettingListINRHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<DirectPaySettingListINRReturn> Handle(DirectPaySettingListINR request) {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING)) {
                
                var directpay_details = connection.Get<ofSettings>("PAYEMENT_DIRECTPAY_V2_INR").body;
                var directPayTypebody = connection.Get<ofSettings>("inr_dai_fu_exchange");
                JObject jobj = JObject.Parse(directPayTypebody.body);

                var directPayType = jobj["current"];
                if (true)
                    return new ResponseModel<DirectPaySettingListINRReturn>() {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "",
                        Data = new DirectPaySettingListINRReturn() {
                            details = directpay_details,
                            directPayType = (int)directPayType,
                        }
                    };


            }
        }
    }
}
