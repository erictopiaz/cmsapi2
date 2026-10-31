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
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class WebLinks : PagingModel, IRequest<ResponseModel<WebLinksReturn>>
    {
        //public string Content { get; set; }
        ///// <summary>
        ///// 0 = 没开启
        ///// 1 = 有开启
        ///// </summary>
        //public int Status { get; set; } = -1;
        ///// <summary>
        ///// 0 = 普通公告
        ///// 2 = 提示
        ///// </summary>
        //public int Type { get; set; } = -1;
    }

    public class WebLinksReturn
    { 
        public ConfigStruct[] config { get; set; }
        public struct ConfigStruct
        {
            public string plat { get; set; }
            public string path { get; set; }
            public string type { get; set; }
        }

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class WebLinksValidator : AbstractValidator<WebLinks>
    {
        public WebLinksValidator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class WebLinksHandler : BaseRequestHandler<WebLinks, ResponseModel<WebLinksReturn>>
    {
        public WebLinksHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<WebLinksReturn> Handle(WebLinks request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var jsonResult = connection.Get<ofSettings>("WEB_LINKS");
                if (jsonResult == null)
                {
                    _logger.LogError("WEB_LINKS is blank");
                    return new ResponseModel<WebLinksReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "",
                        Data = null
                    };
                }

                var configsArray = Newtonsoft.Json.JsonConvert.DeserializeObject<WebLinksReturn.ConfigStruct[]>(jsonResult.body);
                return new ResponseModel<WebLinksReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new WebLinksReturn() { 
                           config = configsArray
                    } 
                };
            }
        }
    }
}

