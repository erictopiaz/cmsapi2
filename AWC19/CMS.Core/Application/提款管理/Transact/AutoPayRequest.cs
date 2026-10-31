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
using System.Transactions;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class AutoPayRequest : IRequest<ResponseModel<AutoPayRequestReturn>>
    {
        public long Id { get; set; }

        [SwaggerIgnore]
        public string Admin { get; set; }
    }

    public class AutoPayRequestReturn
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class AutoPayRequestValidator : AbstractValidator<AutoPayRequest>
    {
        public AutoPayRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class AutoPayRequestHandler : BaseWithdraw, IRequestHandler<AutoPayRequest, ResponseModel<AutoPayRequestReturn>>
    {
        public AutoPayRequestHandler(CMS.Data.MainDB context, IMapper mapper, ILogger logger) : base(context, mapper, logger) { }
        public Task<ResponseModel<AutoPayRequestReturn>> Handle(AutoPayRequest request, CancellationToken cancellationToken)
        {

            var resultDetail = GetWithdrawDetails(request.Id);
            if (resultDetail == null) return Task.FromResult(
                new ResponseModel<AutoPayRequestReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "",
                    Data = null
                });

            var transactingID = resultDetail.id;
            var transactingAccount = resultDetail.account;
            var transactingAmount = resultDetail.amount;
            var transactingCheckperson = resultDetail.checkperson;
            var transactingStatus = resultDetail.status;            // should be in pay wait mode (status=1)

            // Should be in paywait mode 
            if (transactingStatus != 1) {
                return Task.FromResult(
                    new ResponseModel<AutoPayRequestReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "不可进行支付的状态",
                        Data = null
                    });
            }

            // Check if the lockperson is the same as the cancelperson
            if (transactingCheckperson != request.Admin)
            {
                return Task.FromResult(
                new ResponseModel<AutoPayRequestReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "本人不是锁定的使用者",
                    Data = null
                });
            }

            using (TransactionScope scope = new TransactionScope())
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                // Updateing Withdrawal
                var isWithdrawalSuccess = connection.Execute("" +
                    "UPDATE ofWithdrawal " +
                    "SET status=@STATUS,paytime=@PAYTIME,payperson=@PAYPERSON,fee=@FEE " +
                    "WHERE id=@ID AND status=1 ", (new
                    {
                        ID = transactingID,
                        CHECKPERSON = request.Admin
                    }));

                // Updateing Account Balance
                var isAccountUpdateSuccess = connection.Execute("" +
                    "UPDATE ofUser " +
                    "SET amountsuspend=amountsuspend-@AMOUNT," +
                            "withdrawalnumber=withdrawalnumber+1," +
                            "withdrawalamount=withdrawalamount+@AMOUNT " +
                    "WHERE account=@ACCOUNT ", (new
                    {
                        ACCOUNT = transactingAccount,
                        AMOUNT = transactingAmount,
                    }));

                scope.Complete();

                if (isWithdrawalSuccess == 1 && isAccountUpdateSuccess == 1)
                    return Task.FromResult(new ResponseModel<AutoPayRequestReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "",
                        Data = null
                    });
                else
                    return Task.FromResult(new ResponseModel<AutoPayRequestReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "",
                        Data = null
                    });

            }

        }

    }
}

