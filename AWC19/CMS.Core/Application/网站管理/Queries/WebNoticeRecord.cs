using AutoMapper;
using AWC2019.Core.Validation;
using AWC2019.Core.ViewModel;
using CMS.Core;
using CMS.Core.Application.提款管理;
using CMS.Core.ViewModel;
using CMS.Data.Models;
using ShareEntity.OutputModels;
using Dapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSwag.Annotations;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.JWT;
using UtilsLib.Merchant;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class WebNoticeRecord : PagingModel, IRequest<ResponseModel<PagingModel<WebNoticeRecordReturn>>>
    {
        public string Content { get; set; }
        /// <summary>
        /// 0 = 没开启
        /// 1 = 有开启
        /// </summary>
        public int Status { get; set; } = -1;
        /// <summary>
        /// 0 = 普通公告
        /// 2 = 提示
        /// </summary>
        public int Type { get; set; } = -1;

        public int currencycode { get; set; } = 156;

        public List<int> currencys { get; set; } = new List<int>();
       
        public int walletType { set; get; } = -1;
        /// <summary>
        /// 品牌
        /// </summary>
        public string merchantAccount { set; get; } = "-1";
    }

    public class WebNoticeRecordReturn
    {

        public long id { get; set; }
        public string body { get; set; }
        public DateTime createtime { get; set; }
        public DateTime? expiredate { get; set; }
        public int status { get; set; }
        public int type { get; set; }
        public string typename { get; set; }
        internal long TotalItems { get; set; }
        public int currencycode { set; get; }
        /// <summary>
        /// 会员的品牌
        /// </summary>
        public string merchantAccount { set; get; } = "-1";
        public int walletType { set; get; } = -1;
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class WebNoticeRecordValidator : AbstractValidator<WebNoticeRecord>
    {
        public WebNoticeRecordValidator()
        {
            RuleFor(x => x.Status).Status();
            RuleFor(x => x.Type).Status();


        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class WebNoticeRecordHandler : BaseRequestHandler<WebNoticeRecord, ResponseModel<PagingModel<WebNoticeRecordReturn>>>
    {
        public WebNoticeRecordHandler(IMapper mapper, ILoggerFactory logger) 
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger){}


        protected override ResponseModel<PagingModel<WebNoticeRecordReturn>> Handle(WebNoticeRecord request)
        {


            if (!request.currencys.Contains(request.currencycode)) {
                return new ResponseModel<PagingModel<WebNoticeRecordReturn>>() {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择货币查询"
                };
            }

            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT *,n_merchantAccounts as merchantAccount,currencyType as currencycode , COUNT(*) OVER() as TotalItems " +
                "FROM ofNotice /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY");

            // FILL UP THE WHERE CLAUSE
            if (!string.IsNullOrEmpty(request.Content)) builder.Where("body LIKE @CONTENT", new { CONTENT = string.Concat("%", request.Content, "%") });
            if (request.Status >= 0) builder.Where("status=@STATUS", new { STATUS = request.Status });
            if (request.Type >= 0) builder.Where("type=@TYPE", new { TYPE = request.Type });
            if (request.currencycode >= 0) builder.Where("currencyType=@currencycode", new { currencycode = request.currencycode });
            if (request.currencycode >= 0) builder.Where("currencyType=@currencycode", new { currencycode = request.currencycode });

            if (!request.merchantAccount.Equals("-1") 
                && MerchantCodes.merhantMaps.ContainsKey(request.merchantAccount))
            {
                builder.Where("n_merchantAccounts=@merchantAccount", new { merchantAccount = request.merchantAccount });
            }


            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("createtime DESC, status DESC");

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var rows = connection.Query<WebNoticeRecordReturn>(template.RawSql, template.Parameters);
                long totalItems = rows.Count() == 0 ? 0 : rows.First().TotalItems;

                if (rows.Count() > 0)
                {
                    foreach (var row in rows)
                    {
                        row.merchantAccount = (string.IsNullOrEmpty(row.merchantAccount) || row.merchantAccount.Equals("-1")) ? "Awc" : row.merchantAccount;
                        row.typename = CMS.Core.CachedData.NoticeCodeDict[row.type];
                        row.walletType = MerchantCodes.merhantMaps.ContainsKey(row.merchantAccount)
                            ? MerchantCodes.merhantMaps[row.merchantAccount].walletType : -1;
                    }
                }
               
                return new ResponseModel<PagingModel<WebNoticeRecordReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new PagingModel<WebNoticeRecordReturn>()
                    {
                        TotalItems = totalItems,
                        Data = rows
                    }
                };
            }
        }
    }
}

