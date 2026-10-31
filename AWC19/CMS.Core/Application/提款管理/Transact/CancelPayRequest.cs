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
using CMS.Core;
using NSwag.Annotations;
using Dapper.Contrib.Extensions;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class CancelPayRequest : IRequest<ResponseModel<CancelPayRequestReturn>>
    {
        public long Id { get; set; }
        public string cancelType { get; set; } 
        public string cancelreason { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class CancelPayRequestReturn
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class CancelPayRequestValidator : AbstractValidator<CancelPayRequest>
    {
        public CancelPayRequestValidator()
        {
            //RuleFor(x => x.cancelreason).CharDigitalChineseNullable(1, 80);
            //RuleFor(x => x.cancelType).CharDigitalChineseNullable(1, 10);

            //RuleFor(x => x.Id)
            //    .NotEmpty().WithMessage("ID不可为空")
            //    .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    public class CancelPayRequestHandler : BaseWithdraw<CancelPayRequest, ResponseModel<CancelPayRequestReturn>>
    {
        public CancelPayRequestHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<CancelPayRequestReturn> Handle(CancelPayRequest request)
        {
            var resultDetail = GetWithdrawDetails(request.Id);
            if (resultDetail == null)
                return new ResponseModel<CancelPayRequestReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "",
                    Data = null
                };
            var transactingID = resultDetail.id;
            var transactingAccount = resultDetail.account;
            var transactingAmount = resultDetail.amount;
            var transactingCheckperson = resultDetail.checkperson;
            var transactingStatus = resultDetail.status;            // should be in pay wait mode (status=1)

            // Should be in paywait mode 
            if (transactingStatus > 1)
            {
                return new ResponseModel<CancelPayRequestReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "不可进行支付的状态",
                    Data = null
                };
            }

            // Check if the lockperson is the same as the cancelperson
            if (transactingStatus == 1 && transactingCheckperson != request.Admin)
            {
                return new ResponseModel<CancelPayRequestReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "本人不是锁定的使用者",
                    Data = null
                };
            }

            String withdrawMisc = null;

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var withdrawRow = connection.Get<ofWithdrawal>(transactingID);
                withdrawMisc = withdrawRow.misc;

                if (!string.IsNullOrEmpty(withdrawRow.misc))
                {
                    string misc = withdrawRow.misc;
                    var miscDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(misc);

                    // contains promodisabled
                    if (miscDict.ContainsKey("cancelType"))
                        miscDict["cancelType"] = request.cancelType;

                    // promodisabled == true 
                    else if (!string.IsNullOrEmpty(request.cancelType))
                        miscDict.Add("cancelType", request.cancelType);

                    if (string.IsNullOrEmpty(request.cancelType))
                        miscDict.Remove("cancelType");

                    string miscReconvert = Newtonsoft.Json.JsonConvert.SerializeObject(miscDict);
                    withdrawMisc = miscReconvert;
                }
                else {
                    if (!string.IsNullOrEmpty(request.cancelType))
                    {
                        withdrawMisc = Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, string>() {
                            { "cancelType",  request.cancelType}
                        });
                    }
                    else {
                        withdrawMisc = Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, string>() {
                            { "cancelType",  "提款取消"}
                        });
                    }
                }
            }


                using (TransactionScope scope = new TransactionScope())
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {


                // Updateing Withdrawal
                var isWithdrawalSuccess = connection.Execute("" +
                    "UPDATE OFWITHDRAWAL " +
                    "SET status=3 ,canceltime=@CANCELTIME,cancelreason=@CANCELREASON,cancelperson=@CHECKPERSON, misc=@MISC " +
                    "WHERE id=@ID AND status<=1", new
                    {
                        ID = transactingID,
                        CHECKPERSON = request.Admin,
                        CANCELREASON = request.cancelreason,
                        CANCELTIME = Common.CurrentTime, 
                        MISC = withdrawMisc
                    });

                // Updateing Account Balance
                var isAccountUpdateSuccess = connection.Execute("" +
                    "UPDATE OFUSER " +
                    "SET amountactive=amountactive+@AMOUNT,amountsuspend=amountsuspend-@AMOUNT " +
                    "WHERE account=@ACCOUNT", new
                    {
                        ACCOUNT = transactingAccount,
                        AMOUNT = transactingAmount,
                    });

                scope.Complete();

                if (isWithdrawalSuccess == 1 && isAccountUpdateSuccess == 1)
                    return new ResponseModel<CancelPayRequestReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "",
                        Data = null
                    };
                else
                    return new ResponseModel<CancelPayRequestReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "",
                        Data = null
                    };

            }

        }

    }
}

