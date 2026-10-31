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
using CMS.Core.Application.提款管理;
using CMS.Core.ViewModel;
using CMS.Core.Application;
using ShareEntity.OutputModels;
using Microsoft.Extensions.Logging;
using Dapper.Contrib.Extensions;
using ShareEntity.Models;

namespace CMS.Core.Application.系统管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class UpdateAccessPrivilegeAllAddCustom : IRequest<ResponseModel>
    {
        public string Account { get; set; }
        public string accessprivilegestring { get; set; }
        //[JsonIgnore]
        //public IEnumerable<string> access_privilege { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }


    }


    public class UpdateAccessPrivilegeAllAddCustomValidator : AbstractValidator<UpdateAccessPrivilegeAllAddCustom>
    {
        public UpdateAccessPrivilegeAllAddCustomValidator()
        {
            RuleFor(x => x.Account).Account();
        }
    }



    /// <summary>
    /// Request Handler
    /// </summary>
    public class UpdateAccessPrivilegeAllAddCustomHandler : BaseRequestHandler<UpdateAccessPrivilegeAllAddCustom, ResponseModel>
    {
        public UpdateAccessPrivilegeAllAddCustomHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }
        protected override ResponseModel Handle(UpdateAccessPrivilegeAllAddCustom request)
        {
            var accountDict = new Dictionary<string, string>();
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var accountRows = connection.Query<ofManage>("SELECT account, access FROM ofManage WITH(NOLOCK) WHERE status = 1");
                accountDict = accountRows.ToDictionary(x => x.account, x => x.access);
            }

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                foreach (var item in accountDict)
                {
                    var accountRow = connection.Get<ofManage>(item.Key);
                    var temp = request.accessprivilegestring;
                    var access = accountRow.access;
                    var accessDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(access);
                    string currentAccess = accessDict["allowedpath"];
                    List<string> currentAccessArray = new List<string>(currentAccess.Split('|'));
                    if (!currentAccess.Contains(temp))
                        currentAccessArray.Add(temp);

                    accessDict["allowedpath"] = string.Join('|', currentAccessArray);

                    accountRow.access = Newtonsoft.Json.JsonConvert.SerializeObject(accessDict);
                    var result = connection.Update(accountRow);
                }
                var isSuccess = true;
                return new ResponseModel()
                {
                    code = isSuccess ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = isSuccess ? "success" : "failed"
                };
            }
        }


    }
}

