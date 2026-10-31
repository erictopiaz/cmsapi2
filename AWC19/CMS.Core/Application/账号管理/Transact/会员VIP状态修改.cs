using AutoMapper;
using AWC2019.Core.Validation;
using Dapper;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShareEntity.OutputModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Core.Application.账号管理.Transact
{
    
    // <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 会员VIP状态修改 : IRequest<会员VIP状态修改Return>
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
        public int vipTypeAready { set; get; }

        /// <summary>
        /// 风控描述
        /// </summary>
        public string replyVIP { set; get; } = "";

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 会员VIP状态修改Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 会员VIP状态修改Validator : AbstractValidator<会员VIP状态修改>
    {
        public 会员VIP状态修改Validator()
        {
            RuleFor(x => x.account).Account();
        }
    }


    /// <summary>
    /// Request Handler
    /// </summary>
    public class 会员VIP状态修改Handler : BaseAccount<会员VIP状态修改, 会员VIP状态修改Return>
    {
        public 会员VIP状态修改Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override 会员VIP状态修改Return Handle(会员VIP状态修改 request)
        {

            string insertOrUpdateSql = string.Format(@"if(not EXISTS (SELECT top 1 * FROM ofUserExtention WHERE account=@account))
begin
INSERT INTO [dbo].[ofUserExtention] ([account],[vipTypeAready],[vipStatusRemark])
     VALUES(@account,@vipTypeAready,@vipStatusRemark)
end
else
begin
update ofUserExtention set vipTypeAready=@vipTypeAready,vipStatusRemark=@vipStatusRemark where account=@account
end");
            //var r_status = 0;
            //if (request.vipTypeAready > 0)
            //{
            //    r_status = 1;
            //}
            //else
            //{
            //    r_status = 0;
            //}
            var sqlArray = new List<string>();
            var param = new List<object>();
            request.replyVIP = string.IsNullOrEmpty(request.replyVIP) ? "" : request.replyVIP;
            var dp = new DynamicParameters();
            dp.Add("@account", request.account.Replace("'", "''"), DbType.AnsiString, ParameterDirection.Input, request.account.Length);
            dp.Add("@vipTypeAready", request.vipTypeAready, dbType: DbType.Int32, ParameterDirection.Input);
            //dp.Add("@r_status", r_status, dbType: DbType.Int32, ParameterDirection.Input);
            dp.Add("@vipStatusRemark", request.replyVIP.Replace("'", "''"), DbType.AnsiString, ParameterDirection.Input, request.replyVIP.Length);

            sqlArray.Add(insertOrUpdateSql);
            param.Add(dp);

            //sqlArray.Add("update ofUser set anomaly=@r_status where account=@account ");
            //param.Add(dp);

            var updateResult = DapperBase.ExecuteTransction(sqlArray, param: param, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);


            return new 会员VIP状态修改Return()
            {
                code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                message = updateResult ? "VIP状态修改-成功" : "VIP状态修改-失败"
            };

        }
    }
}
