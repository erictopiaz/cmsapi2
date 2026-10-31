using AutoMapper;
using AWC2019.Core.Validation;
using CMS.Core.Application.账号管理;
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

namespace CMS.Core.Application.优惠管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 配置存款优惠 : IRequest<配置存款优惠Return>
    {
        public string account { get; set; }
        public List<int> specialPromo { set; get; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 配置存款优惠Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 配置存款优惠Validator : AbstractValidator<配置存款优惠>
    {
        public 配置存款优惠Validator()
        {
            RuleFor(x => x.account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 配置存款优惠Handler : BaseAccount<配置存款优惠, 配置存款优惠Return>
    {
        public 配置存款优惠Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override 配置存款优惠Return Handle(配置存款优惠 request)
        {
            string insertOrUpdateSql = string.Format(@"if(not EXISTS (SELECT top 1 * FROM ofUserExtention WHERE account=@account))
begin
INSERT INTO [dbo].[ofUserExtention] ([account],[specialDepositPromo],[sp_status])
     VALUES(@account,@specialDepositPromo,@sp_status)
end
else
begin
update ofUserExtention set specialDepositPromo=@specialDepositPromo,sp_status=@sp_status where account=@account
end");
            var sp_status = 0;
            if (request.specialPromo != null && request.specialPromo.Count >0)
            {
                sp_status = 1;
            }
            else
            {
                request.specialPromo = new List<int>();
                sp_status = 0;
            }
            var specialPromo = JsonConvert.SerializeObject(request.specialPromo);
            var dp = new DynamicParameters();
            dp.Add("@account", request.account.Replace("'", "''"), DbType.AnsiString, ParameterDirection.Input, request.account.Length);
            dp.Add("@specialDepositPromo", specialPromo, DbType.AnsiString, ParameterDirection.Input, specialPromo.Length);
            dp.Add("@sp_status", sp_status, dbType: DbType.Int32, ParameterDirection.Input);

            var updateResult = DapperBase.ExecuteTransction(insertOrUpdateSql, param: dp, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);

            return new 配置存款优惠Return()
            {
                code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                message = updateResult ? "配置存款优惠-成功" : "配置存款优惠-失败"
            };

        }
    }
}
