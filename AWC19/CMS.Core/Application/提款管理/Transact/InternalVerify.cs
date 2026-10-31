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
    public class InternalVerify : IRequest<ResponseModel<InternalVerifyReturn>>
    {
        public long Id { get; set; }
        public string cancelreason { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class InternalVerifyReturn
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class InternalVerifyValidator : AbstractValidator<InternalVerify>
    {
        public InternalVerifyValidator()
        {
            RuleFor(x => x.cancelreason).CharDigitalChinese(1, 50);
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    public class InternalVerifyHandler : BaseWithdraw<InternalVerify, ResponseModel<InternalVerifyReturn>>
    {
        public InternalVerifyHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<InternalVerifyReturn> Handle(InternalVerify request)
        {
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var row = connection.Get<ofDepositInternal>(request.Id);
                if (row.status != 0) {
                    return new ResponseModel<InternalVerifyReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "取消失败",
                        Data = null
                    };
                }


                row.reviewtime = Common.CurrentTime;
                row.status = 2;
                connection.Update(row);

                return new ResponseModel<InternalVerifyReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "取消成功",
                    Data = null
                };
            }
        }

    }
}

