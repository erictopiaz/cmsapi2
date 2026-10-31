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
using CMS.Core.ViewModel;
using CMS.Core.Application;
using ShareEntity.OutputModels;
using Microsoft.Extensions.Logging;
using Dapper.Contrib.Extensions;
using ShareEntity.Models;

namespace CMS.Core.Application.系统管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class ToggleGameStatus : IRequest<ResponseModel>
    {
        public bool gamestatus { get; set; }
        public string gameid { get; set; }
        public long id { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }

        public string merchantAccount { get; set; } = "";

    }


    public class ToggleGameStatusValidator : AbstractValidator<ToggleGameStatus>
    {
        public ToggleGameStatusValidator()
        {
            RuleFor(x => x.gameid).CharAndDigital(2, 5);
        }
    }



    /// <summary>
    /// Request Handler
    /// </summary>
    public class ToggleGameStatusHandler : BaseRequestHandler<ToggleGameStatus, ResponseModel>
    {
        public ToggleGameStatusHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel Handle(ToggleGameStatus request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                if(request.merchantAccount.Equals("Boss", StringComparison.OrdinalIgnoreCase))
                {
                    var row = connection.Get<ofGameClassMerchantUpdate>(request.id);
                    row.gamestatus = request.gamestatus ? 1 : 0;
                    //row.useable = request.gamestatus ? 1 : 0;

                    var isSuccess = connection.Update<ofGameClassMerchantUpdate>(row);

                    return new ResponseModel()
                    {
                        code = isSuccess ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                        message = isSuccess ? "success" : "failed"
                    };
                }
                else
                {
                    var row = connection.Get<ofGameClassUpdate>(request.gameid);
                    row.gamestatus = request.gamestatus ? 1 : 0;
                    row.useable = request.gamestatus ? 1 : 0;

                    var isSuccess = connection.Update<ofGameClassUpdate>(row);

                    return new ResponseModel()
                    {
                        code = isSuccess ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                        message = isSuccess ? "success" : "failed"
                    };
                }
                    
            }
        }


    }
}

