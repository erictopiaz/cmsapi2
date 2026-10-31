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

namespace CMS.Core.Application.优惠管理.Queries
{
    /// <summary>
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 红包雨讯息 : IRequest<ResponseModel<红包雨讯息Return>>
    {
    }
    
    public class 红包雨讯息Return
    {
        public bool pc { get; set; }
        public bool app { get; set; }
        public long DrawTimeSpanSeconds { get; set; }
        public int ComputeType { get; set; }
        public List<string> DrawTimeList { get; set; }

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 红包雨讯息Validator : AbstractValidator<红包雨讯息>
    {
        public 红包雨讯息Validator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 红包雨讯息Handler : BaseRequestHandler<红包雨讯息, ResponseModel<红包雨讯息Return>>
    {
        public 红包雨讯息Handler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<红包雨讯息Return> Handle(红包雨讯息 request)
        {
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var dbresult = connection.Get<ofSettings>("PROMO_红包雨V5").body;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<红包雨讯息Return>(dbresult);

                var result = new ResponseModel<红包雨讯息Return>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                    Data = responseObj
                };

                // TODO: Optimization here
                return result;
            }
           
        }
    }
}

