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

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 会员开启商户 : IRequest<会员开启商户Return>
    {
        public string account { get; set; }
        public int id { set; get; }
        public List<int> openMids { set; get; }


        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 会员开启商户Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 会员开启商户Validator : AbstractValidator<会员开启商户>
    {
        public 会员开启商户Validator()
        {
            RuleFor(x => x.account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 会员开启商户Handler : BaseAccount<会员开启商户, 会员开启商户Return>
    {
        public 会员开启商户Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override 会员开启商户Return Handle(会员开启商户 request)
        {
            if (request.openMids != null && request.openMids.Count > 0)
            {
                //
            }
            else
                return new 会员开启商户Return() { code = ShareEntity.Global.ResponseCode.code.FAIL, message = "会员开启商户-失败，请选择商户! " };


            //string querySql = (string.IsNullOrEmpty(request.account) || request.id>0) ? string.Format(@"SELECT * FROM ofUserExtention WHERE account=(select account from ofUser where id={0})", Convert.ToInt64(request.id)) :
            //    string.Format(@"SELECT * FROM ofUserExtention WHERE account=(select account from ofUser where account='{0}')", request.account);

            string querySql = string.Format(@"SELECT * FROM ofUserExtention WHERE account=(select account from ofUser where id={0})", Convert.ToInt64(request.id));

            var queryResult = DapperBase.Query<QueryOutModel>(querySql, param: null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
            var code = (queryResult != null && queryResult.Count == 1);
            
            var addMids = new List<string>();
            if (code)
            {
                request.account = queryResult.First().account.ToString();
                if (!string.IsNullOrEmpty(queryResult.First().disabledMids))
                {
                    if (queryResult.First().disabledMids.Contains("#")) {//新的禁用商户格式 [id#times]
                        var alreadyMids = JsonConvert.DeserializeObject<List<string>>(queryResult.First().disabledMids);
                        foreach (var m in alreadyMids) {
                            var mid = Convert.ToInt32(m.Split('#')[0]);
                            if (!request.openMids.Contains(mid))
                                addMids.Add(m);
                        }
                    }
                    else {
                        //追加之前已经禁用的商户ids
                        var alreadyMids = JsonConvert.DeserializeObject<List<int>>(queryResult.First().disabledMids);
                        foreach (var m in alreadyMids) {
                            if (!request.openMids.Contains(m)) {
                                var item = string.Format(@"{0}#0", m);
                                addMids.Add(item);
                            }
                               
                        }
                    }

                    
                }

            }
            else {
                var queryAccountSql = string.Format(@"select account from ofUser where id={0}", Convert.ToInt64(request.id));
                var accountObj = DapperBase.ExecuteScalarValue(queryAccountSql, param: null, commandTimeout: 15000,
                    ConnectionString: DapperStringBase.ConnectionString);
                request.account = accountObj.ToString();
            }
            var merchantIds = JsonConvert.SerializeObject(addMids);
            string insertOrUpdateSql = string.Format(@"if(not EXISTS (SELECT top 1 * FROM ofUserExtention WHERE account='{0}'))
begin
INSERT INTO [dbo].[ofUserExtention] ([account],[disabledMids])
     VALUES('{0}',N'{1}')
end
else
begin
update ofUserExtention set disabledMids=N'{1}' where account='{0}'
end", request.account, merchantIds);
            //var merchantIds = JsonConvert.SerializeObject(addMids);
            //var dp = new DynamicParameters();
            //dp.Add("@account", request.account.Replace("'", "''"), DbType.AnsiString, ParameterDirection.Input, request.account.Length);
            //dp.Add("@disabledMids", merchantIds, DbType.String, ParameterDirection.Input, merchantIds.Length);

            var updateResult = DapperBase.ExecuteTransction(insertOrUpdateSql, param: null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);

            return new 会员开启商户Return()
            {
                code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                message = updateResult ? "会员开启商户-成功" : "会员开启商户-失败"
            };
        }
    }
}
