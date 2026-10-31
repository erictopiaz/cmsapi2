using AutoMapper;
using Dapper;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using Newtonsoft.Json;
using ShareEntity.OutputModels;
using AWC2019.Core.Validation;
using MongoDB.Driver;

namespace CMS.Core.Application.账号管理.Transact {
   
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 定额提款限制更新 : IRequest<定额提款限制更新Return> {
        public int id { get; set; }
        public string account { get; set; }
        /// <summary>
        /// 是否可以参与，定额的提款：0：可以，1：禁止参与
        /// </summary>
        public int isLimitedfixWithdrawal { get; set; }
        /// <summary>
        /// 0:网银定额，1：支付宝定额，2：微信定额  
        /// 默认 0
        /// </summary>
        public int limitType { get; set; } = 0;

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 定额提款限制更新Return : ResponseModel {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 定额提款限制更新Validator : AbstractValidator<定额提款限制更新> {
        public 定额提款限制更新Validator() {
            RuleFor(x => x.account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 定额提款限制更新Handler : BaseAccount<定额提款限制更新, 定额提款限制更新Return> {
        public 定额提款限制更新Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) {
        }


        protected override 定额提款限制更新Return Handle(定额提款限制更新 request) {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

           if(request.limitType == 1) {
                return UpdateLimitedAliFixWithdrawal(request);
            }
            else if (request.limitType == 2)
            {
                return UpdateLimitedWXFixWithdrawal(request);
                //return new 定额提款限制更新Return()
                //{
                //    code = ShareEntity.Global.ResponseCode.code.FAIL,
                //    message = $"更新-失败, error limitType:暂末开放!"
                //};

            }
            else if(request.limitType == 0) {
                return UpdateLimitedFixWithdrawal(request);
            }
            else {
                return new 定额提款限制更新Return() {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = $"更新-失败, error limitType:{request.limitType}"
                };
            }

        }
        /// <summary>
        /// 微信定额 参与权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private 定额提款限制更新Return UpdateLimitedWXFixWithdrawal(定额提款限制更新 request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {

                if (request.id > 0)
                {
                    var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                    request.account = accountRow.account;
                }
            }

            string insertOrUpdateSql = string.Format(@"if(not EXISTS (SELECT top 1 * FROM ofUserExtention WHERE account=@account))
begin
INSERT INTO [dbo].[ofUserExtention] ([account],[isLimitedWXFixWithdrawal])
     VALUES(@account,@isLimitedWXFixWithdrawal)
end
else
begin
update ofUserExtention set isLimitedWXFixWithdrawal=@isLimitedWXFixWithdrawal where account=@account
end");
            var sqlArray = new List<string>();
            var param = new List<object>();
            var dp = new DynamicParameters();
            dp.Add("@account", request.account.Replace("'", "''"), DbType.AnsiString, ParameterDirection.Input, request.account.Length);
            dp.Add("@isLimitedWXFixWithdrawal", request.isLimitedfixWithdrawal, dbType: DbType.Int32, ParameterDirection.Input);

            sqlArray.Add(insertOrUpdateSql);
            param.Add(dp);

            var updateResult = DapperBase.ExecuteTransction(sqlArray, param: param, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);


            return new 定额提款限制更新Return()
            {
                code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                message = updateResult ? "更新-成功" : "更新-失败"
            };


        }
        /// <summary>
        /// 支付宝定额 参与权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private  定额提款限制更新Return UpdateLimitedAliFixWithdrawal(定额提款限制更新 request) {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING)) {

                if (request.id > 0) {
                    var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                    request.account = accountRow.account;
                }
            }

            string insertOrUpdateSql = string.Format(@"if(not EXISTS (SELECT top 1 * FROM ofUserExtention WHERE account=@account))
begin
INSERT INTO [dbo].[ofUserExtention] ([account],[isLimitedAliFixWithdrawal])
     VALUES(@account,@isLimitedAliFixWithdrawal)
end
else
begin
update ofUserExtention set isLimitedAliFixWithdrawal=@isLimitedAliFixWithdrawal where account=@account
end");
            var sqlArray = new List<string>();
            var param = new List<object>();
            var dp = new DynamicParameters();
            dp.Add("@account", request.account.Replace("'", "''"), DbType.AnsiString, ParameterDirection.Input, request.account.Length);
            dp.Add("@isLimitedAliFixWithdrawal", request.isLimitedfixWithdrawal, dbType: DbType.Int32, ParameterDirection.Input);

            sqlArray.Add(insertOrUpdateSql);
            param.Add(dp);

            var updateResult = DapperBase.ExecuteTransction(sqlArray, param: param, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);


            return new 定额提款限制更新Return() {
                code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                message = updateResult ? "更新-成功" : "更新-失败"
            };


        }
        /// <summary>
        /// 网银定额 参与权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private 定额提款限制更新Return UpdateLimitedFixWithdrawal(定额提款限制更新 request) {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING)) {

                if (request.id > 0) {
                    var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                    request.account = accountRow.account;
                }
            }

            string insertOrUpdateSql = string.Format(@"if(not EXISTS (SELECT top 1 * FROM ofUserExtention WHERE account=@account))
begin
INSERT INTO [dbo].[ofUserExtention] ([account],[isLimitedfixWithdrawal])
     VALUES(@account,@isLimitedfixWithdrawal)
end
else
begin
update ofUserExtention set isLimitedfixWithdrawal=@isLimitedfixWithdrawal where account=@account
end");
            var sqlArray = new List<string>();
            var param = new List<object>();
            var dp = new DynamicParameters();
            dp.Add("@account", request.account.Replace("'", "''"), DbType.AnsiString, ParameterDirection.Input, request.account.Length);
            dp.Add("@isLimitedfixWithdrawal", request.isLimitedfixWithdrawal, dbType: DbType.Int32, ParameterDirection.Input);

            sqlArray.Add(insertOrUpdateSql);
            param.Add(dp);

            var updateResult = DapperBase.ExecuteTransction(sqlArray, param: param, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);


            return new 定额提款限制更新Return() {
                code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                message = updateResult ? "更新-成功" : "更新-失败"
            };


        }
    }
}
