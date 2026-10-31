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
using CMS.Core.Application;
using ShareEntity.OutputModels;
using Microsoft.Extensions.Logging;
using Dapper.Contrib.Extensions;
using ShareEntity.Models;
using System.ComponentModel.DataAnnotations;
using ShareEntity.Global;

namespace CMS.Core.Application.系统管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class UpdateWebsiteNavSettings : IRequest<ResponseModel>
    {
        public WebSiteNavSettingsModel inputModel { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }

        [MaxLength(10)]
        public string merchantAccount { set; get; } = "";
        public int currencyType { get; set; } = 156;

    }


    public class UpdateWebsiteNavSettingsValidator : AbstractValidator<UpdateWebsiteNavSettings>
    {
        public UpdateWebsiteNavSettingsValidator()
        {
            //RuleFor(x => x.inputModel.android).();
        }
    }
    public class WebSiteNavSettings_Support
    {
        public string id { set; get; }
        public string icon { set; get; }
        public string title { set; get; }
        public string serveTime { set; get; }
        public string serveRange { set; get; }
        public string url { set; get; }

    }
    public class WebSiteNavSettingsSubModel
    {
        public string label { set; get; }
        public string url { set; get; }

    }
    public class WebSiteNavSettingsModel
    {
        public WebSiteNavSettingsSubModel[] sites { set; get; }
        public WebSiteNavSettingsSubModel[] ios { set; get; }
        public WebSiteNavSettingsSubModel[] android { set; get; }
        public WebSiteNavSettingsSubModel[] nav { set; get; }
        public WebSiteNavSettingsSubModel[] app { set; get; }
        public WebSiteNavSettingsSubModel[] csclient_android { set; get; }
        public WebSiteNavSettingsSubModel[] csclient_ios { set; get; }
        public WebSiteNavSettings_Support[] support { set; get; }
        public int currency { set; get; } = 156;
        public string mer { set; get; } = "";


    }


    /// <summary>
    /// Request Handler
    /// </summary>
    public class UpdateWebsiteNavSettingsHandler : BaseRequestHandler<UpdateWebsiteNavSettings, ResponseModel>
    {
        public UpdateWebsiteNavSettingsHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel Handle(UpdateWebsiteNavSettings request)
        {

            if (request.currencyType == 156 && request.merchantAccount.Equals("Boss", StringComparison.OrdinalIgnoreCase))
                return UpdateWebsiteNavSettingsINR(request);
            else if (request.currencyType == 156 && request.merchantAccount.Equals("Awc", StringComparison.OrdinalIgnoreCase))
                return UpdateWebsiteNavSettings(request);
            else
                return new ResponseModel() { code = ResponseCode.code.FAIL, message = "操作失败"};

        }

        private  ResponseModel UpdateWebsiteNavSettings(UpdateWebsiteNavSettings request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(request.inputModel);
                var ofsetting = connection.Get<ofSettings>("WEBSITE_NAV");
                ofsetting.body = jsonString;
                connection.Update(ofsetting);
                return new ResponseModel() { code = ResponseCode.code.SUCCESS, message = "操作成功" };
            }
        }

        private ResponseModel UpdateWebsiteNavSettingsINR(UpdateWebsiteNavSettings request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(request.inputModel);
                var name = string.Format(@"WEBSITE_NAV_{0}_{1}", request.merchantAccount, request.currencyType);
                var ofsetting = connection.Get<ofSettings>(name);
                ofsetting.body = jsonString;
                connection.Update(ofsetting);
                return new ResponseModel() { code = ResponseCode.code.SUCCESS, message = "操作成功" };
            }
        }
    }


}

