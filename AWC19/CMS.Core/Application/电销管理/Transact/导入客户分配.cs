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
using ShareEntity.OutputModels;
using Microsoft.Extensions.Logging;
using Dapper.Contrib.Extensions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Xml.Schema;
using ShareEntity.Models;

namespace CMS.Core.Application.电销管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 导入客户分配Query : IRequest<ResponseModel<导入客户分配Return>>
    {
        public string Admin { get; set; }
        public string accountassignment { get; set; }
    }

    public class 导入客户分配Return : ofCallLog
    {

        public long TotalItems { get; set; }

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 导入客户分配QueryValidator : AbstractValidator<导入客户分配Query>
    {
        public 导入客户分配QueryValidator()
        {
            //RuleFor(x => x.Account).Account();
            //RuleFor(x => x.CardNo).CardNo();
            //RuleFor(x => x.Bank).CharAndDigital(1, 10);
            //RuleFor(x => x.VerifyTime).Datetime();
            //RuleFor(x => x.Status).Status();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 导入客户分配Handler : BaseRequestHandler<导入客户分配Query, ResponseModel<导入客户分配Return>>
    {
        public 导入客户分配Handler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<导入客户分配Return> Handle(导入客户分配Query request)
        {
            // Verify the Accounts and Admins
            List<string> assignmentList = new List<string>(request.accountassignment.Split('\n'));
            List<string[]> singleAssignment = new List<string[]>();

            List<string> accountList = new List<string>();
            List<string> adminList = new List<string>();
            foreach (var item in assignmentList)
            {
                var x = item.Trim();
                x = x.StartsWith("#") ? x.Substring(1) : x;

                if (!string.IsNullOrEmpty(x))
                {
                    singleAssignment.Add(x.Split('#'));
                    accountList.Add(x.Split('#')[0]);
                    if (!adminList.Contains(x.Split('#')[1]))
                        adminList.Add(x.Split('#')[1]);
                }
            }





            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                int accountCounts = connection.QueryFirst<int>("SELECT COUNT(*) FROM ofUser WHERE account IN @ACCOUNT",
                    new { ACCOUNT = accountList });
                if (accountCounts != accountList.Count())
                    return new ResponseModel<导入客户分配Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "有账号不存在, 请确认账号",
                    };


                int adminCounts = connection.QueryFirst<int>("SELECT COUNT(*) FROM ofManage WHERE account IN @ACCOUNT",
                    new { ACCOUNT = adminList });
                if (adminCounts != adminList.Count())
                    return new ResponseModel<导入客户分配Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "有后台账号不存在, 请确认后台账号",
                    };


                foreach (var item in singleAssignment)
                {
                    connection.Insert<ofCallLog>(new ofCallLog()
                    {
                        account = item[0],
                        comment = "分配",
                        createperson = item[1],
                        createtime = Common.CurrentTime,
                        status = 1,
                        telephone = "",
                        calltime = Common.CurrentTime
                    });
                }



                var result = new ResponseModel<导入客户分配Return>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                };

                // TODO: Optimization here
                return result;
            }
        }
    }
}

