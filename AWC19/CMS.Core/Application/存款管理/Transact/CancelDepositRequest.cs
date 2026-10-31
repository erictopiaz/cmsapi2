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
using ShareEntity.OutputModels;
using System.Runtime.Serialization;
using System.Transactions;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using Dapper.Contrib.Extensions;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.存款管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class CancelDepositRequest : IRequest<ResponseModel<CancelDepositReturn>>
    {
        public long Id { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class CancelDepositReturn :ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class CancelDepositRequestValidator : AbstractValidator<CancelDepositRequest>
    {
        public CancelDepositRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class CancelDepositRequestHandler : BaseRequestHandler<CancelDepositRequest, ResponseModel<CancelDepositReturn>>
    {
        public CancelDepositRequestHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<CancelDepositReturn> Handle(CancelDepositRequest request)
        {

            using (TransactionScope scope = new TransactionScope())
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                try
                {
                    var depositRow = connection.Get<ofDeposit>(request.Id);
                    // 已经处理过了
                    if (depositRow.status != 0)
                        return new ResponseModel<CancelDepositReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "已经处理过了",
                            Data = null
                        };


                    depositRow.status = 3;
                    depositRow.reviewtime = Common.CurrentTime;
                    depositRow.reviewperson = request.Admin;
                    var depositResult = connection.Update(depositRow);
                    scope.Complete();

                    if (depositResult)
                        return new ResponseModel<CancelDepositReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                            message = "",
                            Data = null
                        };
                    else
                    {
                        return new ResponseModel<CancelDepositReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "取消失败",
                            Data = null
                        };
                    }
                }
                catch (Exception ex)
                {
                    string errorStamp = Common.TimeStamp;
                    _logger.LogError("Exception:{0}, Stamp:{1}", ex.Message, errorStamp);
                    return new ResponseModel<CancelDepositReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "运行出错 " + errorStamp,
                        Data = null
                    };
                }

            }

        }

    }
}

