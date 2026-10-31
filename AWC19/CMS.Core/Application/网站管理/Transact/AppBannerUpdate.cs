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
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using CMS.Core;
using CMS.Core.ViewModel;
using Dapper.Contrib.Extensions;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class AppBannerUpdate : IRequest<ResponseModel<AppBannerUpdateReturn>>
    {
        public string code { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }

    public class AppBannerUpdateReturn : ResponseModel
    {

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class AppBannerUpdateValidator : AbstractValidator<AppBannerUpdate>
    {
        public AppBannerUpdateValidator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class AppBannerUpdateHandler : BaseRequestHandler<AppBannerUpdate, ResponseModel<AppBannerUpdateReturn>>
    {
        public AppBannerUpdateHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<AppBannerUpdateReturn> Handle(AppBannerUpdate request)
        {
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var jsonResult = connection.Get<ofSettings>("APP_BANNER");
                if (jsonResult == null)
                {
                    _logger.LogError("APP_BANNER is blank");
                    return new ResponseModel<AppBannerUpdateReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "",
                        Data = null
                    };
                }

                var configsArray = Newtonsoft.Json.JsonConvert.DeserializeObject<AppBannerReturn.ConfigStruct[]>(jsonResult.body);
                for (int i = 0; i < configsArray.Length; i++)
                    if (configsArray[i].plat == request.code &&
                        configsArray[i].type == request.type)
                    { 
                        configsArray[i].path = request.value;
                        //configsArray[i].type = request.type;
                    }

                string contentString = Newtonsoft.Json.JsonConvert.SerializeObject(configsArray);
                jsonResult.body = contentString;
                var x = connection.Update(jsonResult);
                //var x = false;
                return new ResponseModel<AppBannerUpdateReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = null
                };
            }
        }
    }
}

