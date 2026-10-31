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
using System.Transactions;
using Microsoft.Extensions.Logging;
using CMS.Core;
using NSwag.Annotations;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 红包雨设置Update : IRequest<红包雨设置UpdateReturn>
    {
        public bool pc { get; set; }
        public bool app { get; set; }
        public long drawTimeSpanSeconds { get; set; }
        public int computeType { get; set; }
        public List<string> drawTimeList { get; set; }
    }

    public class 红包雨设置UpdateReturn: ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 红包雨设置UpdateValidator : AbstractValidator<红包雨设置Update>
    {
        public 红包雨设置UpdateValidator()
        {
            //RuleFor(x => x.Id)
            //    .NotEmpty().WithMessage("ID不可为空")
            //    .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 红包雨设置UpdateHandler : BaseRequestHandler<红包雨设置Update, 红包雨设置UpdateReturn>
    {

        public 红包雨设置UpdateHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override 红包雨设置UpdateReturn Handle(红包雨设置Update request)
        {
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var dbresult = connection.Get<ofSettings>("PROMO_红包雨V5");

                List<string> drawTimeStringList = new List<string>();
                for (int i = 0; i < request.drawTimeList.Count; i++)
                {
                    DateTime temp;
                    if(DateTime.TryParse(request.drawTimeList[i], out temp))
                        drawTimeStringList.Add(request.drawTimeList[i]);
                }
                request.drawTimeList = drawTimeStringList;
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                dbresult.body = json;
                connection.Update(dbresult);

                //bool updateresult = true;
                return new 红包雨设置UpdateReturn()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }

        }
    }
}

