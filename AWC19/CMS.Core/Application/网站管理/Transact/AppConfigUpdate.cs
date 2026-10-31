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
    public class AppConfigUpdate : IRequest<ResponseModel<AppConfigUpdateReturn>>
    {
        public string code { get; set; }
        public string value { get; set; }
    }

    public class AppConfigUpdateReturn : ResponseModel
    {

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class AppConfigUpdateValidator : AbstractValidator<AppConfigUpdate>
    {
        public AppConfigUpdateValidator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class AppConfigUpdateHandler : BaseRequestHandler<AppConfigUpdate, ResponseModel<AppConfigUpdateReturn>>
    {
        public AppConfigUpdateHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<AppConfigUpdateReturn> Handle(AppConfigUpdate request)
        {
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var jsonResult = connection.QueryFirstOrDefault<ofSettings>("SELECT * FROM ofSettings WHERE code='APP_SETTINGS'");
                if (jsonResult == null)
                {
                    _logger.LogError("APP_SETTINGS is blank");
                    return new ResponseModel<AppConfigUpdateReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "",
                        Data = null
                    };
                }

                var configsArray = Newtonsoft.Json.JsonConvert.DeserializeObject<AppConfigReturn.ConfigStruct[]>(jsonResult.body);
                for (int i = 0; i < configsArray.Length; i++)
                    if (configsArray[i].code == request.code)
                        configsArray[i].content = request.value;

                string contentString = Newtonsoft.Json.JsonConvert.SerializeObject(configsArray);
                jsonResult.body = contentString;
                var x = connection.Update(jsonResult);

                return new ResponseModel<AppConfigUpdateReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = null
                };
            }
        }
    }
}

