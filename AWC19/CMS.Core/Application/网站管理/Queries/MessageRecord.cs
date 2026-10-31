using AutoMapper;
using AWC2019.Core.Validation;
using AWC2019.Core.ViewModel;
using CMS.Core;
using CMS.Core.Application.提款管理;
using CMS.Core.ViewModel;
using CMS.Data.Models;
using Dapper;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSwag.Annotations;
using ShareEntity.OutputModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtilsLib;
using static UtilsLib.lang.LangCodes;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class MessageRecord : PagingModel, IRequest<ResponseModel<PagingModel<MessageRecordReturn>>>
    {
        public List<int> currencys { get; set; }
        public int currencycode { get; set; }
        public string Account { get; set; }
        public string content { get; set; }
        /// <summary>
        /// 0 = 没开启
        /// 1 = 有开启
        /// </summary>
        public int Status { get; set; } = -1;
        public int Type { get; set; } = -1;
        //public int currencycode { get; set; } = 156;

        //public List<int> currencys { get; set; } = new List<int>();

    }

    public class MessageRecordReturn
    {
        public long id { get; set; }
        public string account { get; set; }
        public string createperson { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public DateTime createtime { get; set; }
        public int status { get; set; }
        public DateTime? readtime { get; set; }
        public int type { get; set; }
        public string typename { get; set; }
        internal long TotalItems { get; set; }
        /// <summary>
        /// 0:会员，1：代理
        /// </summary>
        public int accountType { set; get; }

    }


    public class MsgModel
    {
        //account, lastUpdateTime
        public string account { set; get; } = "";
        public string lastUpdateTime { set; get; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class MessageRecordValidator : AbstractValidator<MessageRecord>
    {
        public MessageRecordValidator()
        {
            RuleFor(x => x.Account).Account();
            RuleFor(x => x.content).CharDigitalChineseNullable(1,40);
            RuleFor(x => x.Page).Page();
            RuleFor(x => x.PageSize).PageSize();
            RuleFor(x => x.Status).Status();
            RuleFor(x => x.Type).Status();

        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class MessageRecordHandler : BaseRequestHandler<MessageRecord, ResponseModel<PagingModel<MessageRecordReturn>>>
    {
        public MessageRecordHandler(IMapper mapper, ILoggerFactory logger) 
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger){}


        protected override ResponseModel<PagingModel<MessageRecordReturn>> Handle(MessageRecord request)
        {
            if (!request.currencys.Contains(request.currencycode))
            {
                return new ResponseModel<PagingModel<MessageRecordReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择货币查询"
                };
            }

            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            bool withwhere = false;

            if (!request.currencys.Contains(request.currencycode)) {
                return new ResponseModel<PagingModel<MessageRecordReturn>>() {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择货币查询"
                };
            }

            // FILL UP THE WHERE CLAUSE
            if (!string.IsNullOrEmpty(request.Account)) {
                withwhere = true; 
                builder.Where("b.Account = @Account", new { Account = request.Account });
            }
            if (!string.IsNullOrEmpty(request.content)) builder.Where("b.content LIKE @REMARKS", new { remarks = string.Concat('%', request.content, '%') });

            if (request.Status >= 0) {
                withwhere = true;
                builder.Where("b.status=@STATUS", new { STATUS = request.Status });
            }

            if (request.currencycode > 0) { builder.Where("a.currencyType = @currencycode", new { currencycode = request.currencycode }); }

            if (request.Type >= 0) {
                withwhere = true;
                builder.Where("b.type=@TYPE", new { TYPE = request.Type });
            } 

            // PAGING FUNCTION
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("b.id DESC");

            SqlBuilder.Template template;
            if (withwhere)
            {
                template = builder.AddTemplate("" +
                "SELECT a.currencyType, b.* , COUNT(*) OVER() as TotalItems " +
                "FROM ofMessage b WITH(NOLOCK) " +
                "LEFT JOIN ofUser a WITH(NOLOCK) on a.account = b.account /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY");
            }
            else
                template = builder.AddTemplate("" +
                "SELECT a.currencyType, b.* , TotalItems=1000 " +
                "FROM ofMessage b WITH(NOLOCK) " +
                "LEFT JOIN ofUser a WITH(NOLOCK) on a.account = b.account /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY");



            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var rows = connection.Query<MessageRecordReturn>(template.RawSql, template.Parameters);
                long totalItems = (rows != null && rows.Count()>0) ? rows.First().TotalItems : 0;

                // Apply the LevelName 
                if (rows.Count() > 0)
                {
                    var accounts = string.Empty;
                    var ants = new List<string>();
                    foreach (var row in rows) {
                        if (ants.Contains(row.account.ToLower()))
                            continue;
                        accounts = string.IsNullOrEmpty(accounts)
                            ? string.Format(@"'{0}'",row.account) : string.Format(@"{0},'{1}'", accounts, row.account);
                        ants.Add(row.account.ToString());
                    }

                    #region 检测最后读取时间
                    var lastReadSql = string.Format(@"select account, lastUpdateTime from ofMessagesLastReadTime with(nolock) where account in ({0})", accounts);
                    var msgModels = connection.Query<MsgModel>(lastReadSql);
                    if (msgModels != null && msgModels.Count() > 0)
                    {
                        
                        //有检测到上次拉取全读接口
                        foreach (var msg in rows)
                        {
                            msg.typename = CMS.Core.CachedData.MessageTypeDict[msg.type];
                            var msgModel = msgModels.Where(x => x.account.Equals(msg.account,StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                            if(msgModel != null)
                            {
                                var lastUpdateTime = Convert.ToDateTime(msgModel.lastUpdateTime);
                                if (Convert.ToDateTime(msg.createtime) <= lastUpdateTime)
                                {
                                    msg.status = 1;
                                    msg.readtime = lastUpdateTime;
                                }
                                else
                                {
                                    msg.status = 0;
                                }
                            }
                            
                        }
                    }
                    else
                    {
                        rows.ToList().ForEach(x => x.typename = CMS.Core.CachedData.MessageTypeDict[x.type]);
                    }
                    #endregion

                    //rows.ToList().ForEach(x => x.typename = CMS.Core.CachedData.MessageTypeDict[x.type]);
                }

                return new ResponseModel<PagingModel<MessageRecordReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new PagingModel<MessageRecordReturn>()
                    {
                        TotalItems = totalItems,
                        Data = rows
                    }
                };
            }
        }
    }
}

