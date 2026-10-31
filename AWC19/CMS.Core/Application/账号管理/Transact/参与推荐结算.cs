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
using System.Data.SqlClient;
using System.Text;

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 参与推荐结算 : IRequest<参与推荐结算Return>
    {
        public int id { get; set; }
        public string account { get; set; }
        /// <summary>
        /// 是否设置了实名验证
        /// </summary>
        public int isLimitSpreadBilled { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 参与推荐结算Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 参与推荐结算Validator : AbstractValidator<参与推荐结算>
    {
        public 参与推荐结算Validator()
        {
            RuleFor(x => x.account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 参与推荐结算Handler : BaseAccount<参与推荐结算, 参与推荐结算Return>
    {
        public 参与推荐结算Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger)
        {
        }


        protected override 参与推荐结算Return Handle(参与推荐结算 request)
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
INSERT INTO [dbo].[ofUserExtention] ([account],[isLimitSpreadBilled])
     VALUES(@account,@isLimitSpreadBilled)
end
else
begin
update ofUserExtention set isLimitSpreadBilled=@isLimitSpreadBilled where account=@account
end");
            var sqlArray = new List<string>();
            var param = new List<object>();
            var dp = new DynamicParameters();
            dp.Add("@account", request.account.Replace("'", "''"), DbType.AnsiString, ParameterDirection.Input, request.account.Length);
            dp.Add("@isLimitSpreadBilled", request.isLimitSpreadBilled, dbType: DbType.Int32, ParameterDirection.Input);

            sqlArray.Add(insertOrUpdateSql);
            param.Add(dp);

            var updateResult = DapperBase.ExecuteTransction(sqlArray, param: param, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);


            return new 参与推荐结算Return()
            {
                code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                message = updateResult ? "更新参与推荐结算-成功" : "更新参与推荐结算-失败"
            };


        }
    }
}
