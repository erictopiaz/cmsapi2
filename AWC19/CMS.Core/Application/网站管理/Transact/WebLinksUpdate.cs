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
    public class WebLinksUpdate : IRequest<ResponseModel<WebLinksUpdateReturn>>
    {
        public string code { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }

    public class WebLinksUpdateReturn : ResponseModel
    {

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class WebLinksUpdateValidator : AbstractValidator<WebLinksUpdate>
    {
        public WebLinksUpdateValidator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class WebLinksUpdateHandler : BaseRequestHandler<WebLinksUpdate, ResponseModel<WebLinksUpdateReturn>>
    {
        public WebLinksUpdateHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<WebLinksUpdateReturn> Handle(WebLinksUpdate request)
        {
            //return null;
            //_logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var jsonResult = connection.Get<ofSettings>("WEB_LINKS");
                if (jsonResult == null)
                {
                    _logger.LogError("WEB_LINKS is blank");
                    return new ResponseModel<WebLinksUpdateReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "",
                        Data = null
                    };
                }

                var configsArray = Newtonsoft.Json.JsonConvert.DeserializeObject<WebLinksReturn.ConfigStruct[]>(jsonResult.body);
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
                return new ResponseModel<WebLinksUpdateReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = null
                };
            }
        }
    }
}

