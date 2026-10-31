using AutoMapper;
using ShareEntity.Models;
using ShareEntity.OutputModels;
using ShareEntity.SqlCommandText;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilsLib.lang;

namespace CMS.Core.Application.提款管理.Queries {
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class TransferToFix : IRequest<ResponseModel<TransferToFixReturn>> {
        public long rowid { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class TransferToFixReturn {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class TransferToFixValidator : AbstractValidator<TransferToFix> {
        public TransferToFixValidator() {
            RuleFor(x => x.rowid)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class TransferToFixHandler : BaseWithdraw<TransferToFix, ResponseModel<TransferToFixReturn>> {
        public TransferToFixHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<TransferToFixReturn> Handle(TransferToFix request) {

            lock (this) {

                var queryWithdrawOrderSql = string.Format(@"select * from ofWithdrawal where id=@rowid");
                var withdrawRows = DapperBase.Query<ofWithdrawal>(queryWithdrawOrderSql, param: request, commandTimeout: 15000,
                    ConnectionString: DapperStringBase.ConnectionString);
                if(withdrawRows != null && withdrawRows.Count == 1) {
                    var w_order = withdrawRows.First();
                    var querySql = string.Format(@"update ofWithdrawal set wid=3, isTransferToFix=1 where id=@rowid and wid=1");
                    if(w_order.wid == 19) {
                        querySql = string.Format(@"update ofWithdrawal set wid=4, isTransferToFix=1 where id=@rowid and wid=19");
                    }
                    else if (w_order.wid == 20) {
                        querySql = string.Format(@"update ofWithdrawal set wid=5, isTransferToFix=1 where id=@rowid and wid=20");
                    }

                    if (DapperBase.Execute(querySql, param: request, commandTimeout: 15000,
                        ConnectionString: DapperStringBase.ConnectionString)) {
                        return new ResponseModel<TransferToFixReturn>() {
                            code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                            message = "转定额操作成功",
                            Data = null
                        };
                    }
                    else {
                        return new ResponseModel<TransferToFixReturn>() {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "转定额操作失败",
                            Data = null
                        };
                    }
                }
                else {
                    return new ResponseModel<TransferToFixReturn>() {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = $"多定单,error {request.rowid}",
                        Data = null
                    };
                }
                
            }

        }

    }
}
