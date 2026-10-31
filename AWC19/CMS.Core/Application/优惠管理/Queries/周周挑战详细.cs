using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using ShareEntity.Models;
using AWC2019.Core.ViewModel;
using FluentValidation;
using System.Data.SqlClient;
using Dapper;
using AWC2019.Core.Validation;
using Newtonsoft.Json;
using CMS.Core.Application.提款管理;
using CMS.Core.ViewModel;
using ShareEntity.OutputModels;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CMS.Core.Application.优惠管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 周周挑战详细 : PagingModel, IRequest<ResponseModel<周周挑战详细Return>>
    {
        public string account { get; set; }
    }

    public class 周周挑战详细Return
    {
        public float amount { get; set; }
        public float bet { get; set; }
        public float deposit { get; set; }

    }



    /// <summary>
    /// Request Handler
    /// </summary>
    public class 周周挑战详细Handler : BaseRequestHandler<周周挑战详细, ResponseModel<周周挑战详细Return>>
    {
        public 周周挑战详细Handler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<周周挑战详细Return> Handle(周周挑战详细 request)
        {
            string[] parameters = { "WebAPI_周周存送details", request.account, "" };
            for (int i = 0; i < parameters.Length; i++)
                parameters[i] = "args=" + parameters[i];
            string getparameters = string.Join('&', parameters);

            string requestString = string.Concat("http://awcsvc-ea.azurewebsites.net", "/command.asmx/", "API", "?", getparameters);
            WebClient wc = new WebClient();
            try
            {
                var result = wc.DownloadString(requestString);
                result = result.Substring(result.IndexOf('>') + 1);
                result = result.Substring(result.IndexOf('>') + 1);
                result = result.Substring(0, result.IndexOf('<'));

                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

                return new ResponseModel<周周挑战详细Return>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                    Data = new 周周挑战详细Return()
                    {
                        amount = float.Parse(obj["amount"]),
                        bet = float.Parse(obj["bets"]),
                        deposit = float.Parse(obj["deposit"])
                    },
                };

                //return new ResponseModel<周周挑战详细Return>()
                //{
                //    code = 0,
                //    Data = new 周周挑战详细Return()
                //    {
                //        amount = 0,
                //        bet = 0,
                //        deposit = 2
                //    },
                //    message = "test"
                //};
            }
            catch (Exception)
            {
                return new ResponseModel<周周挑战详细Return>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                    Data = new 周周挑战详细Return()
                    {
                        amount = 0,
                        bet = 0,
                        deposit = 0
                    },
                };
            }
        }
    }
}

