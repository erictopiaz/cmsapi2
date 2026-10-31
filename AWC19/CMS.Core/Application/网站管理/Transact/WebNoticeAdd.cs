using AutoMapper;
using AWC2019.Core.Validation;
using AWC2019.Core.ViewModel;
using CMS.Core;
using ShareEntity.Global;
using CMS.Data.Models;
using ShareEntity.OutputModels;
using Dapper;
using Dapper.Contrib.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSwag.Annotations;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using UtilsLib;
using UtilsLib.JWT;
using UtilsLib.Merchant;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class WebNoticeAdd : IRequest<WebNoticeAddReturn>
    {
        public string Content { get; set; }
        public DateTime? ExpireDate{ get; set; }

        /// <summary>
        /// 0 = 普通公告
        /// 2 = 代理，
        /// 100 提示
        /// </summary>
        public int Type { get; set; } = -1;
        [JsonIgnore]
        public string Admin { get; set; } = null;
        public int currencycode { get; set; } = 156;
        /// <summary>
        /// 品牌
        /// </summary>
        public string merchantAccount { get; set; } = "-1";
    }

    public class WebNoticeAddReturn: ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class WebNoticeAddValidator : AbstractValidator<WebNoticeAdd>
    {
        public WebNoticeAddValidator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class WebNoticeAddHandler : BaseRequestHandler<WebNoticeAdd, WebNoticeAddReturn>
    {
        public WebNoticeAddHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override WebNoticeAddReturn Handle(WebNoticeAdd request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            #region 解析出多语系

            //var contents = request.Content.Split("###");//0：中，1:英，2：越南，3：印度
            //var langModels = new List<LangModel>() {
            //    new LangModel() {
            //        lang = 1,
            //        body = contents.Count()>1 ? contents[1] : "",
            //    },
            //    new LangModel() {
            //        lang = 2,
            //        body = contents.Count()>0 ? contents[0] : "",
            //    },
            //    new LangModel() {
            //        lang = 5,
            //        body = contents.Count()>2 ? contents[2] : "",
            //    },
            //    new LangModel() {
            //        lang = 7,
            //        body = contents.Count()>3 ? contents[3] : "",
            //    },
            //};

            #endregion

            if (!MerchantCodes.merhantMaps.ContainsKey(request.merchantAccount))
            {
                return new WebNoticeAddReturn()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择品牌"
                };
            }
            

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var x = connection.Insert(new ofNotice()
                {
                    body = request.Content,
                    createperson = request.Admin,
                    createtime = Core.Common.CurrentTime,
                    expiredate = request.ExpireDate,
                    currencyType = request.currencycode,
                    status = 0,
                    type = request.Type < 0 ? 0 : request.Type,
                    bodyLangs = JsonConvert.SerializeObject(new List<LangModel>()),
                    n_merchantAccounts = request.merchantAccount

                });
                return new WebNoticeAddReturn()
                {
                    code = x > 0 ? ShareEntity.Global.ResponseCode.code.SUCCESS :ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }
        }
    }
}

