using AutoMapper;
using ShareEntity.OutputModels;
using Dapper;
using DBLibs.WXDapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CMS.Core.Application.网站管理.Transact
{
    
    /// <summary>
    /// 客人留言回复
    /// </summary>
    public class LMessageReply : IRequest<LMessageReplyReturn>
    {
        public long id { set; get; }
        /// <summary>
        /// 回复的内容
        /// </summary>
        [JsonRequired]
        public string reply { get; set; }

        [JsonIgnore]
        public string Admin { get; set; } = null;
    }

    public class LMessageReplyReturn : ResponseModel
    {
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class LMessageReplyHandler : BaseRequestHandler<LMessageReply, LMessageReplyReturn>
    {
        public LMessageReplyHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override LMessageReplyReturn Handle(LMessageReply request)
        {
            var updateSql = string.Format(@"update ofLMessage set replyMessage=@reply,replyPerson=@Admin,replyStatus=1 where id=@id");
            request.reply = request.reply.Replace("'", "''");
            var updateResult = DapperBase.ExecuteTransction(updateSql, param: request, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionLogs);

            return new LMessageReplyReturn()
            {
                code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                message = updateResult ? "回复留言-成功" : "回复留言-失败"
            };
        }
    }



    
}
