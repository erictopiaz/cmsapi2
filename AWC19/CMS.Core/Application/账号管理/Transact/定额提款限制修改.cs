using AutoMapper;
using AWC2019.Core.Validation;
using ShareEntity.OutputModels;
using Dapper;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json.Serialization;

namespace CMS.Core.Application.账号管理.Transact {
  
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 定额提款限制修改 : IRequest<定额提款限制修改Return> {
        /// <summary>
        /// 会员账号根据id 获取账号
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 会员id
        /// </summary>
        public long id { set; get; }
        /// <summary>
        /// 配置定额限制级别
        /// </summary>
        public int fixWAmountLimitLevel { set; get; }


        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 定额提款限制修改Return : ResponseModel {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 定额提款限制修改Validator : AbstractValidator<定额提款限制修改> {
        public 定额提款限制修改Validator() {
            RuleFor(x => x.account).Account();
        }
    }


    /// <summary>
    /// Request Handler
    /// </summary>
    public class 定额提款限制修改Handler : BaseAccount<定额提款限制修改, 定额提款限制修改Return> {
        public 定额提款限制修改Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override 定额提款限制修改Return Handle(定额提款限制修改 request) {

            string insertOrUpdateSql = string.Format(@"if(not EXISTS (SELECT top 1 * FROM ofUserExtention WHERE account=@account))
begin
INSERT INTO [dbo].[ofUserExtention] ([account],[fixWAmountLimitLevel])
     VALUES(@account,@fixWAmountLimitLevel)
end
else
begin
update ofUserExtention set fixWAmountLimitLevel=@fixWAmountLimitLevel where account=@account
end");

            var sqlArray = new List<string>();
            var param = new List<object>();
            var dp = new DynamicParameters();
            dp.Add("@account", request.account.Replace("'", "''"), DbType.AnsiString, ParameterDirection.Input, request.account.Length);
            dp.Add("@fixWAmountLimitLevel", request.fixWAmountLimitLevel, dbType: DbType.Int32, ParameterDirection.Input);

            sqlArray.Add(insertOrUpdateSql);
            param.Add(dp);

            var updateResult = DapperBase.ExecuteTransction(sqlArray, param: param, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);


            return new 定额提款限制修改Return() {
                code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                message = updateResult ? "定额提款限制修改-成功" : "定额提款限制修改-失败"
            };

        }
    }
}
