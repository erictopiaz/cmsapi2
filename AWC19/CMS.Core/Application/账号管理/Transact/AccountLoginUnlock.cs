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

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class AccountLoginUnlock : IRequest<AccountLoginUnlockReturn>
    {
        public long id { get; set; }
        public string Account { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class AccountLoginUnlockReturn : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class AccountLoginUnlockValidator : AbstractValidator<AccountLoginUnlock>
    {
        public AccountLoginUnlockValidator()
        {
            RuleFor(x => x.Account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class AccountLoginUnlockHandler : BaseAccount<AccountLoginUnlock, AccountLoginUnlockReturn>
    {
        public AccountLoginUnlockHandler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger){}


        protected override AccountLoginUnlockReturn Handle(AccountLoginUnlock request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {

                if (request.id > 0)
                {
                    var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                    request.Account = accountRow.account;
                }
                //if (miscDict.ContainsKey("lockdate"))
                //    miscDict["lockdate"] = DateTime.UtcNow.AddHours(6).ToString("yyyy-MM-dd HH:mm:ss");
                //string resultMisc = Newtonsoft.Json.JsonConvert.SerializeObject(miscDict, Formatting.None);
                //Assignment goes here
                //userDetail.misc = resultMisc;
                //userDetail.trynumber = 0;
                //string misc = userDetail.misc;
                //var miscDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(misc);
                var cmd = new SqlCommand("" +
                    "UPDATE ofUserExtention " +
                    "SET lockdate = @LOCKDATE " +
                    "WHERE account = @ACCOUNT");
                //cmd.Parameters.AddWithValue("@LOCKDATE", DateTime.UtcNow.AddHours(6).ToString("yyyy-MM-dd HH:mm:ss"));
                //cmd.Parameters.AddWithValue("@ACCOUNT", request.Account);
                var result = connection.Execute(
                    "UPDATE ofUserExtention " +
                    "SET lockdate = @LOCKDATE " +
                    "WHERE account = @ACCOUNT"
                    , new { 
                        LOCKDATE = DateTime.UtcNow.AddHours(6).ToString("yyyy-MM-dd HH:mm:ss"),
                        ACCOUNT = request.Account
                    });

                return new AccountLoginUnlockReturn()
                {
                    code = result == 1 ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = ""
                };
            }
        }
    }
}

