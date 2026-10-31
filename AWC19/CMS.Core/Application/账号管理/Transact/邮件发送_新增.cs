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
    public class 邮件发送_新增 : IRequest<邮件发送_新增Return>
    {
        public string id{ get; set; }
        public string account { get; set; }
        public string template { get; set; }
        public string remark { get; set; }
        //[JsonIgnore]
        public string templatedata{ get; set; }
        [JsonIgnore]
        public string Admin { get; set; }

    }

    public class 邮件发送_新增Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 邮件发送_新增Validator : AbstractValidator<邮件发送_新增>
    {
        public 邮件发送_新增Validator()
        {
            RuleFor(x => x.account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 邮件发送_新增Handler : BaseAccount<邮件发送_新增, 邮件发送_新增Return>
    {
        public 邮件发送_新增Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger){}


        protected override 邮件发送_新增Return Handle(邮件发送_新增 request)
        {
            //_logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                //Check for the template over ride if needed
                if (string.IsNullOrEmpty(request.template)) {
                    var dbresult = connection.Get<ofSettings>("MAIL_TEMPLATE").body;
                    var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(dbresult);
                    if (responseObj["worldcuptmpl"][request.Admin.ToLower()] != null) {
                        request.template = responseObj["worldcuptmpl"][request.Admin.ToLower()];
                    }
                }


                // Verify the Accounts and Admins
                List<string> dataList = new List<string>(request.templatedata.Split('\n'));
                var templateDataDict = new Dictionary<string, string>();
                foreach (var item in dataList)
                {
                    var x = item.Trim();
                    x = x.StartsWith("#") ? x.Substring(1) : x;
                    if (!string.IsNullOrEmpty(x))
                    {
                        string name = x.Split('#')[0];
                        string namevalue = x.Split('#')[1];
                        templateDataDict.Add(name, namevalue);
                    }
                }

                request.templatedata = Newtonsoft.Json.JsonConvert.SerializeObject(templateDataDict, Formatting.None);



                if (!string.IsNullOrEmpty(request.id))
                {
                        var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                        request.account = accountRow.account;
                }
                var userDetail = connection.Get<ofUser>(request.account);
                string emailaddress = userDetail.email;


                //request.templatedata;
                

                var ofmail = new ofMail()
                {
                    account = userDetail.account,
                    email = userDetail.email,
                    createtime = Common.CurrentTime,
                    remark = request.remark,
                    status = 0,
                    template = request.template,
                    //sendtime = null,
                    templatedata = request.templatedata
                };

                var insertResult = connection.Insert<ofMail>(ofmail);

                return new 邮件发送_新增Return()
                {
                    code = insertResult > 0 ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }
        }
    }
}

