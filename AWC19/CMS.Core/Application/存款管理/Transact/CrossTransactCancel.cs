using AutoMapper;
using ShareEntity.Models;
using ShareEntity.OutputModels;
using Dapper;
using Dapper.Contrib.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Transactions;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.存款管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class CrossTransactCancel : IRequest<ResponseModel<CrossTransactCancelReturn>>
    {
        public long Id { get; set; }
        //public decimal fee { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class CrossTransactCancelReturn : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class CrossTransactCancelValidator : AbstractValidator<CrossTransactCancel>
    {
        public CrossTransactCancelValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class CrossTransactCancelHandler : BaseRequestHandler<CrossTransactCancel, ResponseModel<CrossTransactCancelReturn>>
    {
        public CrossTransactCancelHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<CrossTransactCancelReturn> Handle(CrossTransactCancel request)
        {
            using (TransactionScope scope = new TransactionScope())
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                try
                {
                    var crosstransactRow = connection.Get<ofCrossTransact>(request.Id);
                    if (crosstransactRow.status != 0)
                        return new ResponseModel<CrossTransactCancelReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "已经处理过了",
                            Data = null
                        };
                    crosstransactRow.reviewtime = Common.CurrentTime;
                    crosstransactRow.status = 2;
                    crosstransactRow.reviewperson = request.Admin;
                    var crosstransactResult = connection.Update(crosstransactRow);


                    // WITHDRAW Portion
                    var withdrawRow = connection.Get<ofWithdrawal>(crosstransactRow.withdrawid);
                    var deposit_id = connection.Query<long>("" +
                        "SELECT id " +
                        "FROM ofDeposit WITH(NOLOCK) " +
                        "WHERE merchantno = '" + crosstransactRow.orderid + "'");
                    var depositRow = connection.Get<ofDeposit>(deposit_id);

                    if (withdrawRow.account.ToLower() == depositRow.account.ToLower())
                        return new ResponseModel<CrossTransactCancelReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "用户同一个,不允许存提",
                            Data = null
                        };

                    // Should be in paywait mode 
                    if (withdrawRow.status != 1)
                        return new ResponseModel<CrossTransactCancelReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "不可进行支付的状态",
                            Data = null
                        };
                    withdrawRow.checkperson = null;




                    if (depositRow.status == 1)
                        return new ResponseModel<CrossTransactCancelReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "已经处理过了",
                            Data = null
                        };

                    //var fee = depositRow.fee;


                    //depositRow.status = 3;
                    //depositRow.reviewtime = Common.CurrentTime;
                    //depositRow.reviewperson = request.Admin;
                    //var depositResult = connection.Update(depositRow);
                    //withdrawRow.wid = 1;
                    var isWithdrawalSuccess = connection.Update(withdrawRow);
                    scope.Complete();

                    if (isWithdrawalSuccess)
                    {

                        return new ResponseModel<CrossTransactCancelReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                            message = "",
                            Data = null
                        };
                    }
                    else
                        return new ResponseModel<CrossTransactCancelReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                            message = "",
                            Data = null
                        };
                }
                catch (Exception ex)
                {
                    string errorStamp = Common.TimeStamp;
                    _logger.LogError("Exception:{0}, Stamp:{1}", ex.Message, errorStamp);
                    return new ResponseModel<CrossTransactCancelReturn>()
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

