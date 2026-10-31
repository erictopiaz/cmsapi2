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
using Microsoft.Extensions.Logging;
using ShareEntity.OutputModels;
using Dapper.Contrib.Extensions;
using ShareEntity.Models;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 提款金额查询 : IRequest<ResponseModel<提款金额查询Return>>
    {
        public long Id { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 提款金额查询Return
    {
        public decimal amount { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 提款金额查询Validator : AbstractValidator<提款金额查询>
    {
        public 提款金额查询Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 提款金额查询Handler : BaseWithdraw<提款金额查询, ResponseModel<提款金额查询Return>>
    {
        public 提款金额查询Handler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }
        //private bool Payment汇皇()
        //{
        //    string postUrl = "https://api.ichatpay168.com/api/payout";
        //    string _merchantID = "13286";
        //    string _merchantKey = "f11bb16e-f879-426e-b421-fb13fd39e032";
        //    string returnUrl = "https://www.awcwin8.com";
        //    string callbackUrl = "http://2pay.allpayzs.com/Payment/Callback/20192/v2";
        //    var stf = new Thirdpay.爱汇.AhPayment(_merchantID, _merchantKey, callbackUrl, postUrl)
        //    {
        //        account = "test",
        //        returnUrl = returnUrl,
        //        //publicOrPrivate = 1
        //    };

        //    Thirdpay.Common.BANKS destBank;
        //    if (!Enum.TryParse<Thirdpay.Common.BANKS>("工商银行", out destBank))
        //        return false;

        //    try
        //    {
        //        string orderid = stf.GenerateOrderID;

        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        protected override ResponseModel<提款金额查询Return> Handle(提款金额查询 request)
        {
            //Payment汇皇();
            //// 创建DAPPER TEMPLATE 
            //var builder = new SqlBuilder();
            //var template = builder.AddTemplate("" +
            //    "SELECT amount " +
            //    "FROM ofWithdrawal WITH(NOLOCK) /**where**/ ");

            //// FILL UP THE WHERE CLAUSE
            //builder.Where("id = @Id", new { Id = request.Id });

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                //var details = connection.QueryFirst<提款金额查询Return>(template.RawSql, template.Parameters);

                var withdrawRow = connection.Get<ofWithdrawal>(request.Id);
                return new ResponseModel<提款金额查询Return>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new 提款金额查询Return { 
                        amount = withdrawRow.amount
                    }
                };
            }

        }
    }



}

