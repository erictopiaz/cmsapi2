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
    public class UpdateAccessPrivilegeCustom : IRequest<ResponseModel>
    {
        public string Account { get; set; }
        public string accessprivilegestring { get; set; }
        //[JsonIgnore]
        //public IEnumerable<string> access_privilege { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }


    }


    public class UpdateAccessPrivilegeCustomValidator : AbstractValidator<UpdateAccessPrivilegeCustom>
    {
        public UpdateAccessPrivilegeCustomValidator()
        {
            RuleFor(x => x.Account).Account();
        }
    }



    /// <summary>
    /// Request Handler
    /// </summary>
    public class UpdateAccessPrivilegeCustomHandler : BaseRequestHandler<UpdateAccessPrivilegeCustom, ResponseModel>
    {
        public UpdateAccessPrivilegeCustomHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }
        protected override ResponseModel Handle(UpdateAccessPrivilegeCustom request)
        {
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var accountRow = connection.Get<ofManage>(request.Account);

                var temp = request.accessprivilegestring;

                var access = accountRow.access;
                var accessDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(access);
                string currentAccess = accessDict["allowedpath"];
                List<string> currentAccessArray = new List<string>( currentAccess.Split('|'));
                if(!currentAccess.Contains(temp))
                    currentAccessArray.Add(temp);

                accessDict["allowedpath"] = string.Join('|', currentAccessArray);

                accountRow.access = Newtonsoft.Json.JsonConvert.SerializeObject(accessDict);
                var isSuccess = connection.Update(accountRow);

                return new ResponseModel()
                {
                    code = isSuccess ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = isSuccess ? "success" : "failed"
                };
            }
        }


    }
}

