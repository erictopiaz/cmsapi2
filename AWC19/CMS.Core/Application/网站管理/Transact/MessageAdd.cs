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
using System.Transactions;
using Microsoft.Extensions.Logging;
using CMS.Core;
using NSwag.Annotations;
using Dapper.Contrib.Extensions;
using ShareEntity.Global;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class MessageAdd : IRequest<MessageAddReturn>
    {
        /// <summary>
        /// 0 = 留言讯息
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// Comma Separated accounts
        /// </summary>
        public string AccountList { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        [JsonIgnore]
        public string Admin { get; set; } = null;
    }

    public class MessageAddReturn : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class MessageAddValidator : AbstractValidator<MessageAdd>
    {
        public MessageAddValidator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class MessageAddHandler : BaseRequestHandler<MessageAdd, MessageAddReturn>
    {
        public MessageAddHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override MessageAddReturn Handle(MessageAdd request)
        {
            var accountList = new List<string>(request.AccountList.Split(',')).Distinct();
            int submittedAccountCount = accountList.Count();


            var builder = new SqlBuilder();
            SqlBuilder.Template template;
            if (request.Type == 1) { 
                template = builder.AddTemplate("" +
                "SELECT COUNT(*) " +
                "FROM ofAffiliate WITH(NOLOCK) /**where**/");
                builder.Where("account in @ACCOUNTLIST", new { ACCOUNTLIST = accountList });
            }
            else {
                template = builder.AddTemplate("" +
                "SELECT COUNT(*) " +
                "FROM ofUser WITH(NOLOCK) /**where**/");
                builder.Where("account in @ACCOUNTLIST", new { ACCOUNTLIST = accountList });
            }
       

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                // VerifyAllAccounts
                var accountsCount = connection.QueryFirst<int>(template.RawSql, template.Parameters);
                if (submittedAccountCount != accountsCount)
                    return new MessageAddReturn()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "有错误账号"
                    };
                //var titleLang = request.Title.Split("###");
                //var contentLang = request.Content.Split("###");//0：中，1:英，2：越南，3：印度
                //if (titleLang.Length != contentLang.Length)
                //    return new MessageAddReturn() {
                //        code = ShareEntity.Global.ResponseCode.code.FAIL,
                //        message = "标头和内容不对应!!"
                //    };

                #region 生成多语系消息体
                
                //var messageLangModels = new List<MessageLangModel>() 
                //{
                //        new MessageLangModel(){ 
                //            lang=1, messageModel = new MessageModel() 
                //            {
                //                title = titleLang.Count()>1 ? titleLang[1] : "",
                //                content = contentLang[1]
                //            }
                //        },
                //        new MessageLangModel(){
                //            lang=2, messageModel = new MessageModel()
                //            {
                //                title = titleLang.Count()>0 ? titleLang[0] : "",
                //                content = contentLang[0]
                //            }
                //        },
                //        new MessageLangModel(){
                //            lang=5, messageModel = new MessageModel()
                //            {
                //                title = titleLang.Count()>2 ? titleLang[2] : "",
                //                content = contentLang[2]
                //            }
                //        },
                //        new MessageLangModel(){
                //            lang=7, messageModel = new MessageModel()
                //            {
                //                title = titleLang.Count()>3 ? titleLang[3] : "",
                //                content = contentLang[3]
                //            }
                //        },
                //};
                //var messageLangs = JsonConvert.SerializeObject(messageLangModels);
                #endregion

                List<ofMessage> messageList = new List<ofMessage>();
                foreach (var item in accountList)
                {
                    
                    messageList.Add(new ofMessage()
                    {
                        account = item,
                        createperson = request.Admin,
                        content = request.Content,
                        title = request.Title,
                        createtime = Core.Common.CurrentTime,
                        location = "0,1",
                        readtime = null,
                        status = 0,
                        type = request.Type,
                        visible = 1,
                        messageLangs = JsonConvert.SerializeObject(new List<MessageLangModel>())    //new List<MessageLangModel>()
                        //accountType = request.Type
                    });
                }

                var insertResult = connection.Insert(messageList);

                return new MessageAddReturn()
                {
                    code = /*x > 0 ? ShareEntity.Global.ResponseCode.code.SUCCESS :*/ShareEntity.Global.ResponseCode.code.FAIL,
                    message = insertResult.ToString()
                };
            }
        }
    }
}

