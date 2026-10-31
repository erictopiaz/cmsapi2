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
    public class UpdatePassword : IRequest<ResponseModel>
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }

    }


    public class UpdatePasswordValidator : AbstractValidator<UpdatePassword>
    {
        public UpdatePasswordValidator()
        {
            RuleFor(x => x.OldPassword).MinimumLength(6);
            RuleFor(x => x.NewPassword).MinimumLength(6);
            RuleFor(x => x.OldPassword).MaximumLength(20);
            RuleFor(x => x.NewPassword).MaximumLength(20);
        }
    }



    /// <summary>
    /// Request Handler
    /// </summary>
    public class UpdatePasswordHandler : BaseRequestHandler<UpdatePassword, ResponseModel>
    {
        public UpdatePasswordHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel Handle(UpdatePassword request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var row = connection.Get<ofManage>(request.Admin);
                
                // Verify OLD password
                if(Common.Security.DoubleMD5Hash(request.OldPassword).ToLower() != row.password.ToLower())
                    return new ResponseModel()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "密码错误" 
                    };

                string newPassword = Common.Security.DoubleMD5Hash(request.NewPassword);
                row.password = newPassword;

                var isSuccess = connection.Update(row);

                return new ResponseModel()
                {
                    code = isSuccess ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = isSuccess ? "密码更新成功" : "更新失败"
                };

                //string existingAccess = "";
                //var accessDict = new Dictionary<string, dynamic>();
                //if (!string.IsNullOrEmpty(row.access))
                //{
                //    accessDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(row.access);
                //    if (accessDict.ContainsKey("allowedpath"))
                //        existingAccess = accessDict["allowedpath"];
                //    else
                //        accessDict.Add("allowedpath", "");
                //}
                //else accessDict.Add("allowedpath", "");

                ////var existingAccess = row.access;
                ////List<string> privilegeList = new List<string>();


                //var existingAccessList = new List<string>(existingAccess.Split('|'));

                //// Removal
                //if (!request.status)
                //{
                //    if (string.IsNullOrEmpty(existingAccess))
                //        return new ResponseModel()
                //        {
                //            code = ShareEntity.Global.ResponseCode.code.FAIL,
                //            message = "不存在的命令 " + request.command
                //        };
                //    if (!existingAccess.Contains(request.command))
                //        return new ResponseModel()
                //        {
                //            code = ShareEntity.Global.ResponseCode.code.FAIL,
                //            message = "不存在的命令 " + request.command
                //        };
                //    existingAccessList.Remove(request.command);
                //}

                //// Adding
                //else
                //{
                //    if (!request.ApiList.ContainsKey(request.command))
                //        return new ResponseModel()
                //        {
                //            code = ShareEntity.Global.ResponseCode.code.FAIL,
                //            message = "不存在的命令 " + request.command
                //        };
                //    if (existingAccessList.Contains(request.command))
                //        return new ResponseModel()
                //        {
                //            code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                //            message = "命令已经存在 " + request.command
                //        };

                //    existingAccessList.Add(request.command);
                //}



                //// Check if Command Exist
                //existingAccessList = existingAccessList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                //row.access = String.Join("|", existingAccessList.ToArray());
                //accessDict["allowedpath"] = String.Join("|", existingAccessList.ToArray());
                ////var accessDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(access);
                ////accessDict["allowedpath"] = string.Join('|', request.access_privilege);
                //row.access = Newtonsoft.Json.JsonConvert.SerializeObject(accessDict);
                //var isSuccess = connection.Update<ofManage>(row);

                ////var accountRow = connection.Get<ofManage>(request.Account);
                ////var access = accountRow.access;





                //bool updateResult = connection.Update<ofManage>(row);


                //var isSuccess = connection.Execute("" +
                //    "UPDATE ofManage " +
                //    "SET access=@access  " +
                //    "WHERE account = @account ", 
                //    new { account = request.Account, access = String.Join("|", existingAccessList.ToArray())});

                //return new ResponseModel()
                //{
                //    code = isSuccess ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                //    message = isSuccess ? "success" : "failed"
                //};
            }
        }


    }
}

