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
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class GameControl : IRequest<ResponseModel<IEnumerable<GameControlReturn>>>
    {
    }

    public class GameControlReturn : ofGameClass
    {

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class GameControlValidator : AbstractValidator<GameControl>
    {
        public GameControlValidator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class GameControlHandler : BaseRequestHandler<GameControl, ResponseModel<IEnumerable<GameControlReturn>>>
    {
        public GameControlHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<IEnumerable<GameControlReturn>> Handle(GameControl request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var result = connection.GetAll<GameControlReturn>();

                return new ResponseModel<IEnumerable<GameControlReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = result
                };
            }
        }
    }
}

