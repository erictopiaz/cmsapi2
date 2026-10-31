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
using Dapper.Contrib.Extensions;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class ApproveRequest : IRequest<ResponseModel<ApproveRequestReturn>>
    {
        public long Id { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class ApproveRequestReturn
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class ApproveRequestValidator : AbstractValidator<ApproveRequest>
    {
        public ApproveRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class ApproveRequestHandler : BaseWithdraw<ApproveRequest, ResponseModel<ApproveRequestReturn>>
    {
        public ApproveRequestHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<ApproveRequestReturn> Handle(ApproveRequest request)
        {
            using (TransactionScope scope = new TransactionScope())
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var withdrawRow = connection.Get<ofWithdrawal>(request.Id);

                // Should be in paywait mode 
                if (withdrawRow.status != 0)
                    return new ResponseModel<ApproveRequestReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "已经处理过了",
                        Data = null
                    };

                // Update Goes HERE
                withdrawRow.reviewperson = request.Admin;
                withdrawRow.reviewtime = Common.CurrentTime;
                withdrawRow.status = 1;

                var isWithdrawalSuccess = connection.Update(withdrawRow);

                scope.Complete();

                if (isWithdrawalSuccess)
                    return new ResponseModel<ApproveRequestReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "",
                        Data = null
                    };
                else
                    return new ResponseModel<ApproveRequestReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "",
                        Data = null
                    };

            }

        }

    }
}

