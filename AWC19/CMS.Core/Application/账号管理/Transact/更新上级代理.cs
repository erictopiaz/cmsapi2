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
    //class 更新上级代理
    //{
    //}

    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 更新上级代理 : IRequest<更新上级代理Return>
    {
        public string Account { get; set; }

        public string affaccount { set; get; }
        /// <summary>
        /// 代理id
        /// </summary>
        public int accountid { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string freezereason { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 更新上级代理Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 更新上级代理Validator : AbstractValidator<更新上级代理>
    {
        public 更新上级代理Validator()
        {
            RuleFor(x => x.affaccount).AffiliateOld();
            RuleFor(x => x.Account).Account();


        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 更新上级代理Handler : BaseAccount<更新上级代理, 更新上级代理Return>
    {
        public 更新上级代理Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override 更新上级代理Return Handle(更新上级代理 request)
        {
            var sql = string.Format(@"update ofUser set affaccount=@affaccount,freezereason=CONCAT(freezereason,',',@freezereason) where id=@accountid");
            var dp = new DynamicParameters();
            dp.Add("@accountid", request.accountid, DbType.Int32, ParameterDirection.Input);
            dp.Add("@affaccount", request.affaccount, DbType.AnsiString, ParameterDirection.Input, 50);
            dp.Add("@freezereason", request.freezereason, dbType: DbType.AnsiString, ParameterDirection.Input, 50);

            var updateResult = DapperBase.ExecuteTransction(sql, param:dp, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
            
            return new 更新上级代理Return()
            {
                code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                message = updateResult ? "更新所属代理-成功" : "更新所属代理-失败"
            };
        }
    }



}
