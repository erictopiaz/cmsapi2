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
using Newtonsoft.Json.Linq;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class WebVariablesUpdate : IRequest<ResponseModel<WebVariablesUpdateReturn>>
    {
        public string jsondata { get; set; }
    }
    public class WebVariablesUpdateReturn : ResponseModel
    {

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class WebVariablesUpdateValidator : AbstractValidator<WebVariablesUpdate>
    {
        public WebVariablesUpdateValidator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class WebVariablesUpdateHandler : BaseRequestHandler<WebVariablesUpdate, ResponseModel<WebVariablesUpdateReturn>>
    {
        public WebVariablesUpdateHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<WebVariablesUpdateReturn> Handle(WebVariablesUpdate request)
        {
            //return null;
            //_logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var jsonResult = connection.Get<ofSettings>("WEB_VARIABLES");
                string toWrite = request.jsondata;
                JObject jobj = JObject.Parse(toWrite);
                jsonResult.body = jobj.ToString(Formatting.None);
                var resultupdate = connection.Update(jsonResult);
                return new ResponseModel<WebVariablesUpdateReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = null
                };
            }
        }
    }
}

