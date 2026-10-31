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

namespace CMS.Core.Application.电销管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 增加备注Query : IRequest<ResponseModel<增加备注Return>>
    {
        public string Admin { get; set; }
        public string account { get; set; }
        public string comment { get; set; }
        public int cstatus { set; get; } = -1;
        //public int sstatus { set; get; } = -1;
        public long id { set; get; }
    }

    public class 增加备注Return : ofCallLog
    {

        public long TotalItems { get; set; }

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 增加备注QueryValidator : AbstractValidator<增加备注Query>
    {
        public 增加备注QueryValidator()
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
    public class 增加备注Handler : BaseRequestHandler<增加备注Query, ResponseModel<增加备注Return>>
    {
        public 增加备注Handler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<增加备注Return> Handle(增加备注Query request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                int accountCounts = connection.QueryFirst<int>("SELECT COUNT(*) FROM ofUser WHERE account = @ACCOUNT",
                    new { ACCOUNT = request.account });
                if (accountCounts != 1)
                    return new ResponseModel<增加备注Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "有账号不存在, 请确认账号",
                    };


                //           var sqlStatement = string.Format(@"INSERT INTO [dbo].[ofCallLog]
                //      ([comment],[account],[telephone],[createperson],[status],[contactStatus],[calltime],[createtime])
                //VALUES
                //      (@comment,@account,@telephone,@createperson,@status,@contactStatus,@calltime,(select top 1 createtime from ofCallLog where account=@account and status=1 order by createtime desc))");


                //           var r = connection.Execute(sqlStatement, new
                //           {
                //               account = request.account,
                //               comment = request.comment,
                //               createperson = request.Admin,
                //               contactStatus = request.cstatus,
                //               status = 0,
                //               telephone = "",
                //               calltime = Common.CurrentTime,
                //           });

                long result = connection.Insert<ofCallLog>(new ofCallLog()
                {
                    account = request.account,
                    comment = request.comment,
                    createperson = request.Admin,
                    contactStatus = request.cstatus,
                    createtime = Common.CurrentTime,
                    status = 0,
                    telephone = "",
                    calltime = Common.CurrentTime

                });

                if (result > 0)
                return new ResponseModel<增加备注Return>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "增加成功",
                };
                else
                    return new ResponseModel<增加备注Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "增加失败",
                    };

            }
        }
    }
}
