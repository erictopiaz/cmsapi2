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
using Newtonsoft.Json.Linq;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class TransferToManualPay : IRequest<ResponseModel<TransferToManualPayReturn>>
    {
        public long Id { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class TransferToManualPayReturn
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class TransferToManualPayValidator : AbstractValidator<TransferToManualPay>
    {
        public TransferToManualPayValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class TransferToManualPayHandler : BaseWithdraw<TransferToManualPay, ResponseModel<TransferToManualPayReturn>>
    {
        public TransferToManualPayHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<TransferToManualPayReturn> Handle(TransferToManualPay request)
        {

            //var resultDetail = GetWithdrawDetails(request.Id);
            //if (resultDetail == null)
            //    return new ResponseModel<TransferToManualPayReturn>()
            //    {
            //        code = ShareEntity.Global.ResponseCode.code.FAIL,
            //        message = "",
            //        Data = null
            //    };

            //var transactingID = resultDetail.id;
            //var transactingAccount = resultDetail.account;
            //var transactingAmount = resultDetail.amount;
            //var transactingCheckperson = resultDetail.checkperson;
            //var transactingStatus = resultDetail.status;            // should be in pay wait mode (status=1)

            //// Should be in paywait mode 
            //if (transactingStatus != 1)
            //{
            //    return new ResponseModel<TransferToManualPayReturn>()
            //    {
            //        code = ShareEntity.Global.ResponseCode.code.FAIL,
            //        message = "不可进行支付的状态",
            //        Data = null
            //    };
            //}

            //// Check if the lockperson is the same as the cancelperson
            //if (transactingCheckperson != request.Admin)
            //{
            //    return new ResponseModel<TransferToManualPayReturn>()
            //    {
            //        code = ShareEntity.Global.ResponseCode.code.FAIL,
            //        message = "本人不是锁定的使用者",
            //        Data = null
            //    };
            //}

            using (TransactionScope scope = new TransactionScope())
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var withdrawRow = connection.Get<ofWithdrawal>(request.Id);

                // Should be in paywait mode 
                if (withdrawRow.status != 1)
                    return new ResponseModel<TransferToManualPayReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "不可进行支付的状态",
                        Data = null
                    };

                // Check if the lockperson is the same as the cancelperson
                if (withdrawRow.checkperson.ToLower() != request.Admin.ToLower())
                    return new ResponseModel<TransferToManualPayReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "本人不是锁定的使用者",
                        Data = null
                    };

                withdrawRow.checkperson = "kafuph";

                JObject miscObj = new JObject();
                if (string.IsNullOrEmpty(withdrawRow.misc))
                    miscObj["xfermanual"] = "kafuph";
                else
                { 
                    miscObj = JObject.Parse(withdrawRow.misc);
                    miscObj["xfermanual"] = "kafuph";
                }
                withdrawRow.misc = miscObj.ToString(Formatting.None);

                var updateResult= connection.Update(withdrawRow);

                scope.Complete();

                if (updateResult)
                    return new ResponseModel<TransferToManualPayReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "",
                        Data = null
                    };
                else
                    return new ResponseModel<TransferToManualPayReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "",
                        Data = null
                    };

            }

        }

    }
}

