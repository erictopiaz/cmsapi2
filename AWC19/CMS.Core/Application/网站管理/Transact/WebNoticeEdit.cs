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
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using UtilsLib.Merchant;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class WebNoticeEdit : IRequest<WebNoticeEditReturn>
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public int currencycode { get; set; } = 156;
        public DateTime? ExpireDate{ get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string merchantAccount { get; set; } = "-1";
    }

    public class WebNoticeEditReturn: ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class WebNoticeEditValidator : AbstractValidator<WebNoticeEdit>
    {
        public WebNoticeEditValidator()
        {
            //RuleFor(x => x.Id)
            //    .NotEmpty().WithMessage("ID不可为空")
            //    .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class WebNoticeEditHandler : BaseRequestHandler<WebNoticeEdit, WebNoticeEditReturn>
    {
        public WebNoticeEditHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override WebNoticeEditReturn Handle(WebNoticeEdit request)
        {

            // 创建DAPPER TEMPLATE 
            //var builder = new SqlBuilder();
            //var template = builder.AddTemplate("" +
            //    "UPDATE ofNotice " +
            //    "SET body=@BODY, expiredate=@EXPIREDATE " +
            //    "WHERE id=@ID", new { 
            //        BODY = request.Content, 
            //        EXPIREDATE = request.ExpireDate,
            //        ID = request.Id
            //    });

            #region 解析出多语系

            var contents = request.Content.Split("###");//0：中，1:英，2：越南，3：印度
            var langModels = new List<LangModel>() {
                new LangModel() {
                    lang = 1,
                    body = contents.Count()>1 ? contents[1] : "",
                },
                new LangModel() {
                    lang = 2,
                    body = contents.Count()>0 ? contents[0] : "",
                },
                new LangModel() {
                    lang = 5,
                    body = contents.Count()>2 ? contents[2] : "",
                },
                new LangModel() {
                    lang = 7,
                    body = contents.Count()>3 ? contents[3] : "",
                },
            };

            #endregion

            if (!MerchantCodes.merhantMaps.ContainsKey(request.merchantAccount))
            {
                return new WebNoticeEditReturn()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择品牌"
                };
            }


            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {

                var merModel = MerchantCodes.merhantMaps[request.merchantAccount];


                var id = request.Id;
                var notice = connection.Get<ofNotice>(id);
                notice.body = request.Content;
                notice.expiredate = request.ExpireDate;
                notice.currencyType = merModel.currencyType;
                notice.bodyLangs = JsonConvert.SerializeObject(langModels);
                notice.n_merchantAccounts = merModel.merchantAccount;
                var updateResult = connection.Update(notice);

                return new WebNoticeEditReturn()
                {
                    code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }
        }
    }
}

