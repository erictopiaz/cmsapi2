using AutoMapper;
using AWC2019.Core.Validation;
using ShareEntity.OutputModels;
using Dapper;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Org.BouncyCastle.Crypto.Tls;
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
    public class 关闭会员商户 : IRequest<关闭会员商户Return>
    {
        public string account { get; set; }
        public int id { set; get; }
        public string disableMids { set; get; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 关闭会员商户Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 关闭会员商户Validator : AbstractValidator<关闭会员商户>
    {
        public 关闭会员商户Validator()
        {
            RuleFor(x => x.account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 关闭会员商户Handler : BaseAccount<关闭会员商户, 关闭会员商户Return>
    {
        public 关闭会员商户Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override 关闭会员商户Return Handle(关闭会员商户 request)
        {
            if (string.IsNullOrEmpty(request.disableMids))
                return new 关闭会员商户Return() { 
                    code = ShareEntity.Global.ResponseCode.code.FAIL, 
                    message = "关闭会员商户-失败，请输入商户编号! 如多个商户请用逗号分开!" 
                };

            if(!request.disableMids.Contains("#"))
                return new 关闭会员商户Return() {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "关闭会员商户-失败,格式不正确，商户id#次数,商户id#次数"
                };

            string querySql = string.Format(@"SELECT * FROM ofUserExtention WHERE account=(select account from ofUser where id={0})", Convert.ToInt64(request.id));
            var queryResult = DapperBase.Query<QueryOutModel>(querySql, param: null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);

            var code = (queryResult != null && queryResult.Count == 1);
            var requestMidDict = new Dictionary<int, string>();
            var mids = request.disableMids.Split(',');
            foreach (var mid in mids)
            {
                var merchantId = 0;
                var limitTimes = 0;
                var m_id = mid.Split('#')[0];
                var l_times = mid.Split('#')[1];
                if (Int32.TryParse(m_id, out merchantId) 
                    && Int32.TryParse(l_times, out limitTimes))
                {
                    //if (!addMids.Contains(mid))
                    //    addMids.Add(mid);
                    if (!requestMidDict.ContainsKey(merchantId))
                        requestMidDict.Add(merchantId, mid);

                }
                else
                {
                    return new 关闭会员商户Return() { code = ShareEntity.Global.ResponseCode.code.FAIL, message = "关闭会员商户-失败，商户编号格式不正确：" + mid };
                }
            }

            if (code)//已经存在的会员
            {
                if(!string.IsNullOrEmpty(queryResult.First().disabledMids))
                {
                    var alreadMidDict = new Dictionary<int, string>();
                    //追加之前已经禁用的商户ids  
                    if (queryResult.First().disabledMids.Contains("#")) {
                        //新的禁用方式
                        var alreadyMids = JsonConvert.DeserializeObject<List<string>>(queryResult.First().disabledMids);
                        foreach (var a_mid in alreadyMids) {
                            var merchantId = a_mid.Split("#")[0];//已经被禁用的商户id
                            if (!alreadMidDict.ContainsKey(Convert.ToInt32(merchantId)))
                                alreadMidDict.Add(Convert.ToInt32(merchantId), a_mid);
                        }
                    }
                    else {
                        //之前只禁用商户没有次数限制的 
                        var alreadyMids = JsonConvert.DeserializeObject<List<int>>(queryResult.First().disabledMids);
                        foreach (var m in alreadyMids) {
                            if (!alreadMidDict.ContainsKey(m)) {
                                var value = string.Format(@"{0}#0", m);//如果被禁用 且次数是0 二示没有次数（永久禁用）
                                alreadMidDict.Add(m, value);
                            }
                        }
                    }

                    foreach (var item in alreadMidDict)
                    {
                        if (requestMidDict.ContainsKey(item.Key)) {
                            continue;
                        }
                        else {
                            requestMidDict.Add(item.Key, item.Value);
                        }
                    }

                }

            }
            var addMids = new List<string>();//[id#times,id#times]
            foreach (var m in requestMidDict) {
                addMids.Add(m.Value);
            }
            string insertOrUpdateSql = string.Format(@"if(not EXISTS (SELECT top 1 * FROM ofUserExtention WHERE account=@account))
begin
INSERT INTO [dbo].[ofUserExtention] ([account],[disabledMids])
     VALUES(@account,@disabledMids)
end
else
begin
update ofUserExtention set disabledMids=@disabledMids where account=@account
end");
            var merchantIds = JsonConvert.SerializeObject(addMids);
            var dp = new DynamicParameters();
            dp.Add("@account", request.account.Replace("'", "''"), DbType.AnsiString, ParameterDirection.Input, request.account.Length);
            dp.Add("@disabledMids", merchantIds, DbType.AnsiString, ParameterDirection.Input, merchantIds.Length);

            var updateResult = DapperBase.ExecuteTransction(insertOrUpdateSql, param: dp, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);

            return new 关闭会员商户Return()
            {
                code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                message = updateResult ? "关闭会员商户-成功" : "关闭会员商户-失败"
            };
        }
    }

    #region 旧
    //    public class 关闭会员商户Handler : BaseAccount<关闭会员商户, 关闭会员商户Return> {
    //        public 关闭会员商户Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

    //        protected override 关闭会员商户Return Handle(关闭会员商户 request) {
    //            if (string.IsNullOrEmpty(request.disableMids))
    //                return new 关闭会员商户Return() { code = ShareEntity.Global.ResponseCode.code.FAIL, message = "关闭会员商户-失败，请输入商户编号! 如多个商户请用逗号分开!" };

    //            string querySql = string.Format(@"SELECT * FROM ofUserExtention WHERE account=(select account from ofUser where id={0})", Convert.ToInt64(request.id));
    //            var queryResult = DapperBase.Query<QueryOutModel>(querySql, param: null, commandTimeout: 15000,
    //                ConnectionString: DapperStringBase.ConnectionString);
    //            var code = (queryResult != null && queryResult.Count == 1);

    //            var addMids = new List<int>();
    //            var mids = request.disableMids.Split(',');
    //            foreach (var mid in mids) {
    //                var m = 0;
    //                if (Int32.TryParse(mid, out m)) {
    //                    if (!addMids.Contains(Convert.ToInt32(mid)))
    //                        addMids.Add(Convert.ToInt32(mid));
    //                }
    //                else {
    //                    return new 关闭会员商户Return() { code = ShareEntity.Global.ResponseCode.code.FAIL, message = "关闭会员商户-失败，商户编号格式不正确：" + mid };
    //                }
    //            }

    //            if (code)//已经存在的会员
    //            {
    //                if (!string.IsNullOrEmpty(queryResult.First().disabledMids)) {
    //                    //追加之前已经禁用的商户ids
    //                    var alreadyMids = JsonConvert.DeserializeObject<List<int>>(queryResult.First().disabledMids);
    //                    foreach (var m in alreadyMids) {
    //                        if (!addMids.Contains(m))
    //                            addMids.Add(m);
    //                    }
    //                }

    //            }

    //            string insertOrUpdateSql = string.Format(@"if(not EXISTS (SELECT top 1 * FROM ofUserExtention WHERE account=@account))
    //begin
    //INSERT INTO [dbo].[ofUserExtention] ([account],[disabledMids])
    //     VALUES(@account,@disabledMids)
    //end
    //else
    //begin
    //update ofUserExtention set disabledMids=@disabledMids where account=@account
    //end");
    //            var merchantIds = JsonConvert.SerializeObject(addMids);
    //            var dp = new DynamicParameters();
    //            dp.Add("@account", request.account.Replace("'", "''"), DbType.AnsiString, ParameterDirection.Input, request.account.Length);
    //            dp.Add("@disabledMids", merchantIds, DbType.AnsiString, ParameterDirection.Input, merchantIds.Length);

    //            var updateResult = DapperBase.ExecuteTransction(insertOrUpdateSql, param: dp, commandTimeout: 15000,
    //                ConnectionString: DapperStringBase.ConnectionString);

    //            return new 关闭会员商户Return() {
    //                code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
    //                message = updateResult ? "关闭会员商户-成功" : "关闭会员商户-失败"
    //            };
    //        }
    //    }

    #endregion
}
