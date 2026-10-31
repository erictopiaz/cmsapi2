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
using System.Linq;
using System.Text;

namespace CMS.Core.Application.账号管理.Transact {
   
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 更新会员充值实名 : IRequest<更新会员充值实名Return> {
        public string account { get; set; }
        public int id { set; get; }
        public string realNames { set; get; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 更新会员充值实名Return : ResponseModel {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 更新会员充值实名Validator : AbstractValidator<更新会员充值实名> {
        public 更新会员充值实名Validator() {
            RuleFor(x => x.account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 更新会员充值实名Handler : BaseAccount<更新会员充值实名, 更新会员充值实名Return> {
        public 更新会员充值实名Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override 更新会员充值实名Return Handle(更新会员充值实名 request) {

            if (string.IsNullOrEmpty(request.realNames))
                return new 更新会员充值实名Return() {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "更新会员充值实名-失败，请输入实名列表! 如多个实名请用#分开!"
                };

            string querySql = string.Format(@"SELECT * FROM ofUserExtention WHERE account=(select account from ofUser where id={0})", Convert.ToInt64(request.id));
            var queryResult = DapperBase.Query<QueryOutModel>(querySql, param: null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);

            var code = (queryResult != null && queryResult.Count == 1);
            
            var requestNames = request.realNames.Split('#');//新修改的姓名列表
            //if (code)//已经存在的会员
            //{
            //    if (!string.IsNullOrEmpty(queryResult.First().realNames)) {

            //        //已经存在的实名 
            //        var alreadRealNames = JsonConvert.DeserializeObject<List<string>>(queryResult.First().realNames);

            //        foreach (var name in alreadRealNames) {
            //            //存放到更新列表data
            //            if(!updateRealNames.Contains(name))
            //                updateRealNames.Add(name);
            //        }

            //    }

            //}

            //foreach (var name in requestNames) {
            //    //存放到更新列表data
            //    if (!updateRealNames.Contains(name))
            //        updateRealNames.Add(name);
            //}

            string insertOrUpdateSql = string.Format(@"if(not EXISTS (SELECT top 1 * FROM ofUserExtention WHERE account=@account))
begin
INSERT INTO [dbo].[ofUserExtention] ([account],[realNames])
     VALUES(@account,@realNames)
end
else
begin
update ofUserExtention set realNames=@realNames where account=@account
end");
            var depositRealNames = JsonConvert.SerializeObject(requestNames);
            var dp = new DynamicParameters();
            dp.Add("@account", request.account.Replace("'", "''"), DbType.AnsiString, ParameterDirection.Input, request.account.Length);
            dp.Add("@realNames", depositRealNames, DbType.AnsiString, ParameterDirection.Input, depositRealNames.Length);

            var updateResult = DapperBase.ExecuteTransction(insertOrUpdateSql, param: dp, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);

            return new 更新会员充值实名Return() {
                code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                message = updateResult ? "更新会员充值实名-成功" : "更新会员充值实名-失败"
            };
        }
    }



}
