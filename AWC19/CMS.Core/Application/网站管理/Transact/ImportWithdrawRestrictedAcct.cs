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
using ShareEntity.OutputModels;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using CMS.Core;
using CMS.Core.ViewModel;
using Dapper.Contrib.Extensions;
using System.Text.RegularExpressions;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class ImportWithdrawRestrictedAcct : IRequest<ResponseModel<ImportWithdrawRestrictedAcctReturn>>
    {
        public string accounts { get; set; }
    }

    public class ImportWithdrawRestrictedAcctReturn : ResponseModel
    {

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class ImportWithdrawRestrictedAcctValidator : AbstractValidator<ImportWithdrawRestrictedAcct>
    {
        public ImportWithdrawRestrictedAcctValidator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class ImportWithdrawRestrictedAcctHandler : BaseRequestHandler<ImportWithdrawRestrictedAcct, ResponseModel<ImportWithdrawRestrictedAcctReturn>>
    {
        public ImportWithdrawRestrictedAcctHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<ImportWithdrawRestrictedAcctReturn> Handle(ImportWithdrawRestrictedAcct request)
        {
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var jsonResult = connection.Get<ofSettings>("withdraw_restricted_unreachable");

                // Filter out the accounts
                request.accounts = request.accounts.ToLower();
                var accountList = request.accounts.Split('\n').ToList();
                accountList.ForEach(x => {
                    x = x.Trim();
                    //x = x.Remove('\n');
                }
                );
                Regex reg = new Regex("^[a-z0-9_]*$");

                foreach (var item in accountList)
                {
                    if (!reg.IsMatch(item))
                    {
                        return new ResponseModel<ImportWithdrawRestrictedAcctReturn>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = string.Format("请检查 {0} 账号", item),
                            Data = null
                        };
                    }
                }

                var cmd = new SqlCommand("" +
                    "SELECT COUNT(*) FROM ofUser WITH(NOLOCK) " +
                    "/**where**/");
                var sqlBuilder = new SqlBuilder();
                SqlBuilder.Template sqlTemplate;
                sqlBuilder.Where("account IN @ACCOUNTLIST", new
                {
                    ACCOUNTLIST = accountList
                });
                sqlTemplate = sqlBuilder.AddTemplate("" +
                    "SELECT COUNT(*) FROM ofUser WITH(NOLOCK) " +
                    "/**where**/");
                var existAccountActualCount = connection.QueryFirstOrDefault<long>(sqlTemplate.RawSql, sqlTemplate.Parameters);

                if(existAccountActualCount != accountList.Count)
                {
                    return new ResponseModel<ImportWithdrawRestrictedAcctReturn>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = string.Format("存在数量:{0}账号, 提交数量:{1}账号", existAccountActualCount.ToString(), accountList.Count()),
                        Data = null
                    };
                }

                string listString = string.Join(',', accountList);
                jsonResult.body = listString;
                connection.Update<ofSettings>(jsonResult);
                return new ResponseModel<ImportWithdrawRestrictedAcctReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = null
                };
            }
        }
    }
}

