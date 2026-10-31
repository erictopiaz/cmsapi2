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

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 会员风控状态修改 : IRequest<会员风控状态修改Return>
    {
        /// <summary>
        /// 会员账号根据id 获取账号
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 会员id
        /// </summary>
        public long id { set; get; }
        /// <summary>
        /// 配置好的风控状态
        /// </summary>
        public int riskTypeAready { set; get; }

        /// <summary>
        /// 风控描述
        /// </summary>
        public string reply { set; get; } = "";

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 会员风控状态修改Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 会员风控状态修改Validator : AbstractValidator<会员风控状态修改>
    {
        public 会员风控状态修改Validator()
        {
            RuleFor(x => x.account).Account();
        }
    }

    
    /// <summary>
    /// Request Handler
    /// </summary>
    public class 会员风控状态修改Handler : BaseAccount<会员风控状态修改, 会员风控状态修改Return>
    {
        public 会员风控状态修改Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override 会员风控状态修改Return Handle(会员风控状态修改 request)
        {

            string insertOrUpdateSql = string.Format(@"if(not EXISTS (SELECT top 1 * FROM ofUserExtention WHERE account=@account))
begin
INSERT INTO [dbo].[ofUserExtention] ([account],[riskTypesAready],[r_status],[riskRemark])
     VALUES(@account,@riskTypesAready,@r_status,@riskRemark)
end
else
begin
update ofUserExtention set riskTypesAready=@riskTypesAready,r_status=@r_status,riskRemark=@riskRemark where account=@account
end");
            var r_status = 0;
            if (request.riskTypeAready >0 )
            {
                r_status = 1;
            }
            else
            {
                r_status = 0;
            }
            var sqlArray = new List<string>();
            var param = new List<object>();
            request.reply = string.IsNullOrEmpty(request.reply) ? "" : request.reply;
            var dp = new DynamicParameters();
            dp.Add("@account", request.account.Replace("'", "''"), DbType.AnsiString, ParameterDirection.Input, request.account.Length);
            dp.Add("@riskTypesAready", request.riskTypeAready, dbType: DbType.Int32, ParameterDirection.Input);
            dp.Add("@r_status", r_status, dbType: DbType.Int32, ParameterDirection.Input);
            dp.Add("@riskRemark", request.reply.Replace("'", "''"), DbType.AnsiString, ParameterDirection.Input, request.reply.Length);

            sqlArray.Add(insertOrUpdateSql);
            param.Add(dp);

            sqlArray.Add("update ofUser set anomaly=@r_status where account=@account ");
            param.Add(dp);

            var updateResult = DapperBase.ExecuteTransction(sqlArray, param: param, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);


            return new 会员风控状态修改Return()
            {
                code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                message = updateResult ? "风控状态修改-成功" : "风控状态修改-失败"
            };

        }
    }
}
