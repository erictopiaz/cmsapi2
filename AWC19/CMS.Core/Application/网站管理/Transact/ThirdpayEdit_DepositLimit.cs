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
using ShareEntity.OutputModels;
using System.Runtime.Serialization;
using System.Transactions;
using Microsoft.Extensions.Logging;
using CMS.Core;
using NSwag.Annotations;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class ThirdpayEdit_DepositLimitOnly : IRequest<ThirdpayEdit_DepositLimitOnlyReturn>
    {
        public long id { get; set; }
        public int? minamount { get; set; }
        public int? maxamount { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class ThirdpayEdit_DepositLimitOnlyReturn : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class ThirdpayEdit_DepositLimitOnlyValidator : AbstractValidator<ThirdpayEdit_DepositLimitOnly>
    {
        public ThirdpayEdit_DepositLimitOnlyValidator()
        {
            //TODO: Validation populate
            RuleFor(x => x.id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class ThirdpayEdit_DepositLimitOnlyHandler : BaseRequestHandler<ThirdpayEdit_DepositLimitOnly, ThirdpayEdit_DepositLimitOnlyReturn>
    {
        public ThirdpayEdit_DepositLimitOnlyHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ThirdpayEdit_DepositLimitOnlyReturn Handle(ThirdpayEdit_DepositLimitOnly request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));



            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {

                var bankbaseList = connection.GetAll<ofBankBase>();

                var id = request.id;
                var row_justtogetclass = connection.Get<ofThirdpay>(id);
                var row = connection.Get<ofThirdpay_WithoutClass>(id);

                row.minamount = request.minamount;
                row.maxamount = request.maxamount;


                var ofThirdpay_updateresult = connection.Update(row);

                return new ThirdpayEdit_DepositLimitOnlyReturn()
                {
                    code = ofThirdpay_updateresult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }
        }
    }
}

