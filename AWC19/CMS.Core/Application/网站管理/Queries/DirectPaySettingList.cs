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
using ShareEntity.OutputModels;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using CMS.Core;
using CMS.Core.ViewModel;
using Dapper.Contrib.Extensions;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class DirectPaySettingList : IRequest<ResponseModel<DirectPaySettingListReturn>>
    {
    }

    public class DirectPaySettingListReturn
    {
        public string details{ get; set; }


    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class DirectPaySettingListValidator : AbstractValidator<DirectPaySettingList>
    {
        public DirectPaySettingListValidator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class DirectPaySettingListHandler : BaseRequestHandler<DirectPaySettingList, ResponseModel<DirectPaySettingListReturn>>
    {
        public DirectPaySettingListHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<DirectPaySettingListReturn> Handle(DirectPaySettingList request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                //string jsonString = "{\"payplatform\":[{\"key\":\"云代付\",\"feecent\":0.7,\"banks\":[\"工商银行，农业银行\"],\"cat_en\":[\"newusr_low\",\"newusr_hi\"],\"decomm\":false,\"maxamount\":10000},{\"key\":\"机构代付\",\"feecent\":0.7,\"banks\":[\"工商银行，农业银行\"],\"cat_en\":[\"newusr_low\",\"newusr_hi\"],\"decomm\":false,\"maxamount\":10000}],\"level_cat\":{\"newusr_low\":{\"name\":\"新玩家(低)\",\"levels\":[\"10006\",\"10007\"],\"payplatform\":\"云代付\",\"desc\":\"一个余额内的普通玩家\"},\"newusr_hi\":{\"name\":\"新玩家(高)\",\"levels\":[\"10006\",\"10007\"]}}}";
                //JObject jobj = JObject.Parse(jsonString);
                //JObject jobj_payplatform = jobj.ToObject(JObject);

                var directpay_details = connection.Get<ofSettings>("PAYEMENT_DIRECTPAY_V2").body;

                if(true)
                return new ResponseModel<DirectPaySettingListReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new DirectPaySettingListReturn()
                    {
                        details = directpay_details
                        //details = "{\"payplatform\":[{\"key\":\"云代付\",\"feecent\":\"0.70\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"机构代付\",\"feecent\":\"0.85\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"信付代付\",\"feecent\":\"0.85\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"普宝盆\",\"feecent\":\"0.00\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"捷优代付\",\"feecent\":\"0.85\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"信汇\",\"feecent\":\"0.80\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"币付\",\"feecent\":\"0.00\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"现在代付\",\"feecent\":\"0.00\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"MMPB\",\"feecent\":\"0.00\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"壹快付\",\"feecent\":\"0.70\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"小额代付\",\"feecent\":\"0.80\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"汇皇\",\"feecent\":\"0.00\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"依云付\",\"feecent\":\"1.20\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"爱汇\",\"feecent\":\"0.65\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"爱汇02\",\"feecent\":\"0.65\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"爱汇03\",\"feecent\":\"0.65\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"爱汇05\",\"feecent\":\"0.65\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"壹元\",\"feecent\":\"0.70\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"壹元公\",\"feecent\":\"0.70\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"聚宝盆\",\"feecent\":\"0.00\",\"maxamount\":\"5000\",\"decomm\":false},{\"key\":\"爱汇USDT\",\"currency\":\"USDT\",\"feecent\":\"1.00\",\"maxamount\":\"5000\",\"status\":true,\"decomm\":false},{\"key\":\"JBP\",\"feecent\":\"0.00\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"GPay\",\"feecent\":\"0.00\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"很美\",\"feecent\":\"0.00\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"诚信通\",\"feecent\":\"0.00\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"OneTech\",\"feecent\":\"0.00\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"乐付\",\"feecent\":\"0.00\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"信付\",\"feecent\":\"0.70\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"OnePay\",\"feecent\":\"0.00\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"速付06\",\"feecent\":\"1.00\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"速付07\",\"feecent\":\"1.00\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"速付08\",\"feecent\":\"1.00\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"众宝\",\"feecent\":\"1.0\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"众宝私\",\"feecent\":\"1.0\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"速付\",\"feecent\":\"0.00\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"纯代付\",\"feecent\":\"0.50\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"纯代付02\",\"feecent\":\"0.50\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"纯代付03\",\"feecent\":\"0.50\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"纯代付04\",\"feecent\":\"0.50\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"聚合\",\"feecent\":\"0.50\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"聚合02\",\"feecent\":\"0.50\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"XP\",\"feecent\":\"0.60\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"我来付\",\"feecent\":\"0.60\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"优信付\",\"feecent\":\"0.70\",\"maxamount\":\"5000\",\"decomm\":true},{\"key\":\"天空\",\"feecent\":\"0.01\",\"maxamount\":\"5000\",\"decomm\":true}],\"qwe\":{\"newusr_low\":{\"name\":\"新玩家(低)\",\"levels\":[\"10006\",\"10007\"],\"payplatform\":\"云代付\",\"desc\":\"一个余额内的普通玩家\",\"minamount\":\"1\",\"maxamount\":\"2999\"},\"newusr_hi\":{\"name\":\"新玩家(高)\",\"desc\":\"一个余额内的普通玩家\",\"minamount\":\"3000\",\"maxamount\":\"49999\",\"levels\":[\"10006\",\"10007\"]},\"regular_low\":{\"name\":\"普通(低)\",\"desc\":\"一个余额内的普通玩家\",\"minamount\":\"1\",\"maxamount\":\"2999\",\"levels\":[\"10006\",\"10007\"]},\"regular_hi\":{\"name\":\"普通(高)\",\"desc\":\"一个余额内的普通玩家\",\"minamount\":\"3000\",\"maxamount\":\"49999\",\"levels\":[\"10006\",\"10007\"]},\"vip_low\":{\"name\":\"VIP(低)\",\"desc\":\"一个余额内的普通玩家\",\"minamount\":\"1\",\"maxamount\":\"2999\",\"levels\":[\"10028\",\"10002\",\"10011\"]},\"vip_hi\":{\"name\":\"VIP(高)\",\"desc\":\"一个余额内的普通玩家\",\"minamount\":\"3000\",\"maxamount\":\"49999\",\"levels\":[\"10028\",\"10002\",\"10011\"]},\"vip3_low\":{\"name\":\"VIP3(低)\",\"desc\":\"一个余额内的普通玩家\",\"minamount\":\"1\",\"maxamount\":\"4999\",\"levels\":[\"10003\",\"10004\",\"10005\",\"10025\"]},\"vip3_hi\":{\"name\":\"VIP3(高)\",\"minamount\":\"5000\",\"maxamount\":\"49999\",\"levels\":[\"10003\",\"10004\",\"10005\",\"10025\"]},\"aff\":{\"name\":\"代理\",\"levels\":[\"10006\",\"10007\"]}}}"
                    }
                };



                var vip = connection.Get<ofSettings>("PAYMENT_DIRECTPAY").body;

                return new ResponseModel<DirectPaySettingListReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new DirectPaySettingListReturn()
                    {
                        details = vip
                    }
                };
            }
        }
    }
}

