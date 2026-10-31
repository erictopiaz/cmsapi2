using AutoMapper;
using AWC2019.Core.Validation;
using ShareEntity.OutputModels;
using Dapper;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CMS.Core.Application.账号管理.Transact {
    
    

    public class 银行卡单笔提款区间限制 : IRequest<银行卡单笔提款区间限制Return> {
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
        public int minAmount { set; get; } = 0;
        public int maxAmount { set; get; } = 0;


        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 银行卡单笔提款区间限制Return : ResponseModel {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 银行卡单笔提款区间限制Validator : AbstractValidator<银行卡单笔提款区间限制> {
        public 银行卡单笔提款区间限制Validator() {
            RuleFor(x => x.account).Account();
        }
    }


    /// <summary>
    /// Request Handler
    /// </summary>
    public class 银行卡单笔提款区间限制Handler : BaseAccount<银行卡单笔提款区间限制, 银行卡单笔提款区间限制Return> {
        public 银行卡单笔提款区间限制Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override 银行卡单笔提款区间限制Return Handle(银行卡单笔提款区间限制 request) {

            if(string.IsNullOrEmpty(request.account)) {
                var queryAccountSql = string.Format(@"select account from ofUser where id={0}", request.id);
                var accountObj = DapperBase.ExecuteScalarValueAsync(queryAccountSql, param: null, commandTimeout: 15000,
                    ConnectionString: DapperStringBase.ConnectionString);
                request.account = accountObj.ToString();

            }
            string insertOrUpdateSql = string.Format(@"if(not EXISTS (SELECT top 1 * FROM ofUserExtention WHERE account=@account))
begin
INSERT INTO [dbo].[ofUserExtention] ([account],[withdrawMinAmount],[withdrawMaxAmount])
     VALUES(@account,@withdrawMinAmount,@withdrawMaxAmount)
end
else
begin
update ofUserExtention set withdrawMinAmount=@withdrawMinAmount,withdrawMaxAmount=@withdrawMaxAmount where account=@account
end");

            var sqlArray = new List<string>();
            var param = new List<object>();
            var dp = new DynamicParameters();
            dp.Add("@account", request.account.Replace("'", "''"), DbType.AnsiString, ParameterDirection.Input, request.account.Length);
            dp.Add("@withdrawMinAmount", request.minAmount, dbType: DbType.Int32, ParameterDirection.Input);
            dp.Add("@withdrawMaxAmount", request.maxAmount, dbType: DbType.Int32, ParameterDirection.Input);

            sqlArray.Add(insertOrUpdateSql);
            param.Add(dp);

            var updateResult = DapperBase.ExecuteTransction(sqlArray, param: param, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);


            return new 银行卡单笔提款区间限制Return() {
                code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                message = updateResult ? "修改-成功" : "修改-失败"
            };

        }
    }
}
