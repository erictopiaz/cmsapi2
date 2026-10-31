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
using CMS.Core.Application.账号管理;
using Microsoft.Extensions.Logging;
using Dapper.Contrib.Extensions;
using ShareEntity.OutputModels;
using DBLibs.WXDapper;
using System.Data;


namespace CMS.Core.Application.账号管理.Transact
{
   
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 更新会员加码比例 : IRequest<更新会员加码比例Return>
    {
        /// <summary>
        /// 根据会员id 获取到会员账号
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 会员id
        /// </summary>
        public long id { get; set; }
        public int b_status { set; get; }
        public object betCodeDetail { set; get; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 更新会员加码比例Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 更新会员加码比例Validator : AbstractValidator<更新会员加码比例>
    {
        public 更新会员加码比例Validator()
        {
            RuleFor(x => x.account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 更新会员加码比例Handler : BaseAccount<更新会员加码比例, 更新会员加码比例Return>
    {
        public 更新会员加码比例Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override 更新会员加码比例Return Handle(更新会员加码比例 request)
        {
            string insertOrUpdateSql = string.Format(@"if(not EXISTS (SELECT top 1 * FROM ofUserExtention WHERE account=@account))
begin
INSERT INTO [dbo].[ofUserExtention] ([account],[betCodeDetail],[b_status])
     VALUES(@account,@betCodeDetail, @b_status)
end
else
begin
update ofUserExtention set betCodeDetail=@betCodeDetail,b_status=@b_status where account=@account
end");
            var betCodeDetail = request.betCodeDetail.ToString().Replace("'", "''");
            var dp = new DynamicParameters();
            dp.Add("@account", request.account.Replace("'","''"), DbType.AnsiString, ParameterDirection.Input, request.account.Length);
            dp.Add("@betCodeDetail", betCodeDetail, DbType.AnsiString, ParameterDirection.Input, betCodeDetail.Length);
            dp.Add("@b_status", request.b_status, dbType: DbType.Int32, ParameterDirection.Input);

            var updateResult = DapperBase.ExecuteTransction(insertOrUpdateSql, param:dp, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
            
            return new 更新会员加码比例Return()
            {
                code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                message = updateResult ? "更新加码比例-成功" : "更新加码比例-失败"
            };
        }
    }



}
