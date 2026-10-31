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
using Newtonsoft.Json.Linq;
using Dapper.Contrib.Extensions;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class PaymentNotReceivedUpdate : IRequest<ResponseModel<PaymentNotReceivedUpdateReturn>>
    {
        public long Id { get; set; }
        public string reason { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class PaymentNotReceivedUpdateReturn
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class PaymentNotReceivedUpdateValidator : AbstractValidator<PaymentNotReceivedUpdate>
    {
        public PaymentNotReceivedUpdateValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    public class PaymentNotReceivedUpdateHandler : BaseWithdraw<PaymentNotReceivedUpdate, ResponseModel<PaymentNotReceivedUpdateReturn>>
    {
        public PaymentNotReceivedUpdateHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<PaymentNotReceivedUpdateReturn> Handle(PaymentNotReceivedUpdate request)
        {
            var resultDetail = GetWithdrawDetails(request.Id);
            if (resultDetail == null)
                return new ResponseModel<PaymentNotReceivedUpdateReturn>()
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
            if (transactingStatus != 2)
            {
                return new ResponseModel<PaymentNotReceivedUpdateReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "只能调整成功状态",
                    Data = null
                };
            }

            string misc = resultDetail.misc;

            //// Check if the lockperson is the same as the cancelperson
            //if (transactingStatus == 1 && transactingCheckperson != request.Admin)
            //{
            //    return new ResponseModel<PaymentNotReceivedUpdateReturn>()
            //    {
            //        code = ShareEntity.Global.ResponseCode.code.FAIL,
            //        message = "本人不是锁定的使用者",
            //        Data = null
            //    };
            //}

            using (TransactionScope scope = new TransactionScope())
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                JObject obj = new JObject();

                if (string.IsNullOrEmpty(misc))
                {

                    obj = new JObject()
                    {
                        ["paystat"] = false,
                        ["paystatadmin"] = request.Admin,
                        ["paystatreason"] = request.reason
                    };
                }
                else {
                    obj = JObject.Parse(misc);
                    obj.Add("paystat", false);
                    obj.Add("paystatadmin", request.Admin);
                    obj.Add("paystatreason", request.reason);
                }



                // Updating Withdrawal
                var isWithdrawalSuccess = connection.Execute("" +
                    "UPDATE OFWITHDRAWAL " +
                    "SET misc=@MISC " +
                    "WHERE id=@ID AND status=2", new
                    {
                        ID = transactingID,
                        MISC = obj.ToString(Formatting.None)
                    });
                string remarks = string.Concat("提款补单【", resultDetail.requesttime.ToString("yyyy-MM-dd HH:mm:ss"), "】");
                var insertMinus = connection.Execute("" +
                    "INSERT INTO ofMinus(currency,account,amount,amountbefore,amountafter,createtime,createperson,reviewtime," +
                    "reviewperson, remarks, status, referrer, referrerreg, level, referenceguid, gametype, cancelreason, decreasetype, appendtype, orderno) VALUES(" +
                    "'CNY',@ACCOUNT,@AMOUNT,'0.00','0.00',@CREATETIME,@ADMIN,NULL,NULL,@REMARKS,'0','',@REFERRER,'0',@GUID,NULL,NULL,'1',NULL,NULL)", new
                    {
                        ACCOUNT = resultDetail.account,
                        AMOUNT = resultDetail.amount,
                        CREATETIME = Common.CurrentTime,
                        ADMIN = request.Admin,
                        REFERRER = resultDetail.referrer,
                        GUID = resultDetail.referenceguid, 
                        REMARKS = remarks
                    });

                scope.Complete();

                if (isWithdrawalSuccess == 1 && insertMinus == 1)
                    return new ResponseModel<PaymentNotReceivedUpdateReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "",
                        Data = null
                    };
                else
                    return new ResponseModel<PaymentNotReceivedUpdateReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "",
                        Data = null
                    };

            }

        }

    }
}

