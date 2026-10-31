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
    public class 邮件发送_导入 : IRequest<邮件发送_导入Return>
    {
        //[JsonIgnore]
        public string templatedata { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }

    }

    public class 邮件发送_导入Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 邮件发送_导入Validator : AbstractValidator<邮件发送_导入>
    {
        public 邮件发送_导入Validator()
        {
            //RuleFor(x => x.Account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 邮件发送_导入Handler : BaseAccount<邮件发送_导入, 邮件发送_导入Return>
    {
        public 邮件发送_导入Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override 邮件发送_导入Return Handle(邮件发送_导入 request)
        {
            //_logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                //Check for the template over ride if needed
                string templateID = "";
                var dbresult = connection.Get<ofSettings>("MAIL_TEMPLATE").body;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(dbresult);
                if (responseObj["worldcuptmpl"][request.Admin.ToLower()] != null)
                {
                    templateID = responseObj["worldcuptmpl"][request.Admin.ToLower()];
                }

                //Added Variable 
                string extra_hotline = "";
                if (responseObj["hotline"][request.Admin.ToLower()] != null)
                {
                    extra_hotline = responseObj["hotline"][request.Admin.ToLower()];
                }


                // Verify the Accounts and Admins
                List<string> dataList = new List<string>(request.templatedata.Split('\n'));
                var templateDataDict = new Dictionary<string, string>();

                var mailingList = new List<ofMail>();
                foreach (var item in dataList)
                {
                    var x = item.Trim();
                    x = x.StartsWith("#") ? x.Substring(1) : x;
                    if (!string.IsNullOrEmpty(x))
                    {
                        string account = x.Split('#')[0];
                        string templateid = string.IsNullOrEmpty(x.Split('#')[1]) ? templateID : x.Split('#')[1];
                        string templatedata = x.Split('#')[2];
                        string remarks = x.Split('#')[3];
                        string status = x.Split('#')[4];
                        string marketing_or_default = x.Split('#').Length > 5 ? x.Split('#')[5] : "0";
                        //string namevalue = x.Split('#')[5];

                        // adding a default value
                        var templatedataJson = JObject.Parse(templatedata);

                        if (templatedataJson.ContainsKey("hotline")) templatedataJson["hotline"] = extra_hotline;
                        else templatedataJson.Add("hotline", extra_hotline);

                        templatedata = templatedataJson.ToString(Formatting.None);
                        //var cmd = new SqlCommand("SELECT account FROM ofuser WITH(NOLOCK) WHERE account=@ACCOUNT");
                        //cmd.Parameters.AddWithValue("ACCOUNT", account);
                        //var accountRow = connection.QuerySingle<ofUser>(cmd.CommandText, cmd.Parameters);
                        var accountRow = connection.Get<ofUser>(account);
                        if (accountRow == null) {
                            return new 邮件发送_导入Return()
                            {
                                code = ShareEntity.Global.ResponseCode.code.FAIL,
                                message = string.Concat(account, "不存在的账号")
                            };
                        }
                        string email = accountRow.email;

                        mailingList.Add(new ofMail()
                        {
                            account = account,
                            email = email,
                            template = templateID,
                            templatedata = templatedata,
                            remark = remarks,
                            status = status == "1" ? 2 : 0,
                            createtime = Common.CurrentTime, 
                            createdby = request.Admin,
                            mailtype = int.Parse(marketing_or_default)
                        });
                    }
                }
                // #账号#模板ID#参数#备注#直接发送胡哦增加


                var insertResult = connection.Insert(mailingList);

                MemCache.REDIS_CONNSTRING = CMS.Data.Misc.REDIS_CONNECTIONSTRING;
                MemCache.PushQueue(new JObject()
                {
                    ["task"] = "sendmail",
                }.ToString(Formatting.None));
                return new 邮件发送_导入Return()
                {
                    code = insertResult > 0 ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }
        }
    }
}

