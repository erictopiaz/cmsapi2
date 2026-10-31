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
using Org.BouncyCastle.Ocsp;
using Newtonsoft.Json.Linq;
using ShareEntity.Models;

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 邮件发送_改状态 : IRequest<邮件发送_改状态Return>
    {
        public string id { get; set; }

        public int status {get;set;}
    }

    public class 邮件发送_改状态Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 邮件发送_改状态Validator : AbstractValidator<邮件发送_改状态>
    {
        public 邮件发送_改状态Validator()
        {
            //RuleFor(x => x.Account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 邮件发送_改状态Handler : BaseAccount<邮件发送_改状态, 邮件发送_改状态Return>
    {
        public 邮件发送_改状态Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger){}


        protected override 邮件发送_改状态Return Handle(邮件发送_改状态 request)
        {
            //_logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var mail = connection.Get<ofMail>(request.id);

                if(mail.status != 0)
                    return new 邮件发送_改状态Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "无法改状态"
                    };

                mail.status = request.status;



                if (request.status == 3)
                    mail.canceltime = Common.CurrentTime;


                var updateResult = connection.Update<ofMail>(mail);


                if (request.status == 2) {
                    MemCache.REDIS_CONNSTRING = CMS.Data.Misc.REDIS_CONNECTIONSTRING;
                    MemCache.PushQueue(new JObject()
                    {
                        ["task"] = "sendmail",
                    }.ToString(Formatting.None));

                }

                return new 邮件发送_改状态Return()
                {
                    code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }
        }
    }
}

