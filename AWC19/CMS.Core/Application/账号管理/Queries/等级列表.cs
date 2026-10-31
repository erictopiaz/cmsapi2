using Amazon.Runtime.Internal.Transform;
using AutoMapper;
using AWC2019.Core.Validation;
using CMS.Core.ViewModel;
using CMS.Data.Models;
using ShareEntity.OutputModels;
using Dapper;
using Dapper.Contrib.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Thirdpay.Crypto;
using ShareEntity.Models;

namespace CMS.Core.Application.账号管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 等级列表 : PagingModel, IRequest<ResponseModel<IEnumerable<object>>>
    {
        public string currency { get; set; }
        public string currencyCode { get; set; }
    }

    public class 等级列表Return : 等级列表ReturnBase 
    {
        //public long id { get; set; }
        //public int levelnumber { get; set; }
        //public string levels { get; set; }
        //public string onestepset { get; set; }
        //public string twostepset { get; set; }
        //public string description { get; set; }

        public decimal? ew_turnover { get; set; }
        public decimal? ew_turnovermax { get; set; }
        public decimal? ea_turnover { get; set; }
        public decimal? ea_turnovermax { get; set; }
        public decimal? pt_turnover { get; set; }
        public decimal? pt_turnovermax { get; set; }
        public decimal? keno_turnover { get; set; }
        public decimal? keno_turnovermax { get; set; }
        public decimal? bbin_turnover { get; set; }
        public decimal? bbin_turnovermax { get; set; }
        public decimal spb_turnover { get; set; }
        public decimal spb_turnovermax { get; set; }
        public decimal ttg_turnover { get; set; }
        public decimal ttg_turnovermax { get; set; }
        public decimal? lb_turnover { get; set; }
        public decimal? lb_turnovermax { get; set; }
        public decimal ag_turnover { get; set; }
        public decimal ag_turnovermax { get; set; }
        public decimal? keno2_turnover { get; set; }
        public decimal? keno2_turnovermax { get; set; }
        public decimal? vr_turnover { get; set; }
        public decimal? vr_turnovermax { get; set; }
        public decimal? xj_turnover { get; set; }
        public decimal? xj_turnovermax { get; set; }

        //public decimal? pg_turnover { get; set; }
        //public decimal? pg_turnovermax { get; set; }

        public decimal? ds_turnover { get; set; }
        public decimal? ds_turnovermax { get; set; }
        public decimal? dses_turnover { get; set; }
        public decimal? dses_turnovermax { get; set; }
        public decimal? ob_turnover { get; set; }
        public decimal? ob_turnovermax { get; set; }
        public decimal? bti_turnover { get; set; }
        public decimal? bti_turnovermax { get; set; }

        public decimal? ysb_turnover { get; set; }
        public decimal? ysb_turnovermax { get; set; }
        public decimal? rg_turnover { get; set; }
        public decimal? rg_turnovermax { get; set; }
        public decimal? gg_turnover { get; set; }
        public decimal? gg_turnovermax { get; set; }
        public decimal? ky_turnover { get; set; }
        public decimal? ky_turnovermax { get; set; }
        public decimal? imes_turnover { get; set; }
        public decimal? imes_turnovermax { get; set; }

        public decimal? kg_turnover { get; set; }
        public decimal? kg_turnovermax { get; set; }
        public decimal? kg2_turnover { get; set; }
        public decimal? kg2_turnovermax { get; set; }

        public decimal? eb_turnover { get; set; }
        public decimal? eb_turnovermax { get; set; }

        public decimal? bg_turnover { get; set; }
        public decimal? bg_turnovermax { get; set; }
        public decimal? td_turnover { get; set; }
        public decimal? td_turnovermax { get; set; }
        public decimal? tf_turnover { get; set; }
        public decimal? tf_turnovermax { get; set; }
        public decimal? cq_turnover { get; set; }
        public decimal? cq_turnovermax { get; set; }
        public decimal? aw_turnover { get; set; }
        public decimal? aw_turnovermax { get; set; }
        public decimal? kg3_turnover { get; set; }
        public decimal? kg3_turnovermax { get; set; }
        public decimal? impk_turnover { get; set; }
        public decimal? impk_turnovermax { get; set; }
        
        public decimal? obg_turnovermax { get; set; }
        public decimal? obg_turnover { get; set; }
        public decimal? imsp_turnovermax { get; set; }
        public decimal? imsp_turnover { get; set; }
        public decimal? obcv_turnovermax { get; set; }
        public decimal? obcv_turnover { get; set; }
        public decimal? fb_turnovermax { get; set; }
        public decimal? fb_turnover { get; set; }
        public decimal? sw_turnovermax { get; set; }
        public decimal? sw_turnover { get; set; }
        public decimal? cr_turnovermax { get; set; }
        public decimal? cr_turnover { get; set; }
        public decimal? pm_turnovermax { get; set; }
        public decimal? pm_turnover { get; set; }
        public decimal? db_turnovermax { get; set; }
        public decimal? db_turnover { get; set; }

        public decimal? dbkeno_turnovermax { get; set; }
        public decimal? dbkeno_turnover { get; set; }
        public decimal? rsg_turnovermax { get; set; }
        public decimal? rsg_turnover { get; set; }

        public decimal? pp_turnovermax { get; set; }
        public decimal? pp_turnover { get; set; }

        public decimal? iaes_turnovermax { get; set; }
        public decimal? iaes_turnover { get; set; }

        //public decimal? evo_turnovermax { get; set; }
        //public decimal? evo_turnover { get; set; }
        //public decimal? jdb_turnovermax { get; set; }
        //public decimal? jdb_turnover { get; set; }
        //public decimal? spribe_turnovermax { get; set; }
        //public decimal? spribe_turnover { get; set; }

        //public string? levelurl { get; set; }
        //public decimal turnoverpercentage { get; set; }
        //public decimal bounspercentage { get; set; }
        //public decimal depositpercentage { get; set; }
        //public decimal maxusdtrebateperday { get; set; }
        //public decimal maxrebateperday { get; set; }
        //public decimal maxwithdrawperday { get; set; }
        //public decimal maxwithdrawperdaypercard { get; set; }
        //public string? misc { get; set; }
        //public string? misc_inr { get; set; }
    }


    public class 等级列表ReturnBase {
        public long id { get; set; }

        public int levelnumber { get; set; }
        public string levels { get; set; }
        public string onestepset { get; set; }
        public string twostepset { get; set; }
        public string description { get; set; }



        public decimal? _9w_turnover { get; set; }
        public decimal? _9w_turnovermax { get; set; }

        public decimal? pg_turnover { get; set; }
        public decimal? pg_turnovermax { get; set; }
        
        public decimal? evo_turnovermax { get; set; }
        public decimal? evo_turnover { get; set; }
        public decimal? jdb_turnovermax { get; set; }
        public decimal? jdb_turnover { get; set; }
        public decimal? spribe_turnovermax { get; set; }
        public decimal? spribe_turnover { get; set; }

        public decimal? turbo_turnovermax { get; set; }
        public decimal? turbo_turnover { get; set; }

        public decimal? jl_turnovermax { get; set; }
        public decimal? jl_turnover { get; set; }

        public string? levelurl { get; set; }
        public decimal turnoverpercentage { get; set; }
        public decimal bounspercentage { get; set; }
        public decimal depositpercentage { get; set; }
        public decimal maxusdtrebateperday { get; set; }
        public decimal maxrebateperday { get; set; }
        public decimal maxwithdrawperday { get; set; }
        public decimal maxwithdrawperdaypercard { get; set; }



        public string? misc { get; set; }
        public string? misc_inr { get; set; }
        public string? misc_pkr { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 等级列表Validator : AbstractValidator<等级列表>
    {
        public 等级列表Validator()
        {
            RuleFor(x => x.currency).CharAndDigitalNullable(1, 10);
            RuleFor(x => x.currencyCode).CharAndDigitalNullable(1, 10);
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 等级列表Handler : BaseRequestHandler<等级列表, ResponseModel<IEnumerable<object>>>
    {
        public 等级列表Handler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        //private ResponseModel<IEnumerable<等级列表Return>> INR_Level() { 

        //}

        protected override ResponseModel<IEnumerable<object>> Handle(等级列表 request)
        {
            if (request.currencyCode == "356") {
                return INR();
            }

            if (request.currencyCode == "586")
            {
                return PKR();
            }
            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT * " +
                "FROM ofUserGroup WITH(NOLOCK) /**where**/");

            var levellist = new List<string>() {
                "白玉VIP",
                "白银VIP",
                "核查级别",
                "见习VIP",
                //"老虎机俱乐部",
                "普通会员",
                //"忠实玩家",
                "特邀VIP",
                "钻石VIP",
                "铂金VIP",
                "黄金VIP",
                "至尊VIP",
                "万象会员"
            };
            builder.Where("levels IN @LEVELS", new { LEVELS = levellist });

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var rows = connection.Query<等级列表Return>(template.RawSql, template.Parameters);


                // Format OSS and TSS

                var rowsSettings = connection.QueryFirst<string>("" +
                    "SELECT body " +
                    "FROM ofSettings WITH(NOLOCK) " +
                    "WHERE code = 'GROUP_LEVELURL'");
                var rowSettingsDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(rowsSettings);

                foreach (var item in rows)
                {
                    item.levelurl = rowSettingsDict.ContainsKey(item.levelnumber.ToString()) ? rowSettingsDict[item.levelnumber.ToString()] : "";
                    item.ew_turnover = item.ew_turnover ?? item.ea_turnover;
                    item.ew_turnovermax = item.ew_turnovermax ?? item.ea_turnovermax;

                    var oss = item.onestepset;
                    oss = oss.Contains("|") ? oss.Split('|')[1] : oss;
                    var ossArray = oss.Contains(",") ? oss.Split(',') : null;
                    item.onestepset = string.Format("{0}送/{1}限/{2}倍", ossArray[1], ossArray[2], ossArray[3]);

                    var tss = item.twostepset;
                    var tssarray = tss.Contains("|") ? tss.Split('|') : new string[] { tss };
                    tss = tssarray[tssarray.Length - 1];
                    var tssArray = tss.Contains(",") ? tss.Split(',') : null;
                    item.twostepset = string.Format("{0}送/{1}限/{2}倍", tssArray[1], tssArray[2], tssArray[3]);

                    
                    var miscJson = Newtonsoft.Json.Linq.JObject.Parse(item.misc);

                    item.bounspercentage = decimal.Parse(
                        Newtonsoft.Json.Linq.JObject.Parse(item.misc)["bonuspercentage"].ToString());

                    item.turnoverpercentage = decimal.Parse(
                        Newtonsoft.Json.Linq.JObject.Parse(item.misc)["turnoverpercentage"].ToString());

                    item.depositpercentage = decimal.Parse(
                        Newtonsoft.Json.Linq.JObject.Parse(item.misc)["depositpercentage"].ToString());

                    item.maxwithdrawperday = miscJson.ContainsKey("maxwithdrawperday") ?
                        decimal.Parse(miscJson["maxwithdrawperday"].ToString()) : 0;
                    
                    item.maxwithdrawperdaypercard = miscJson.ContainsKey("maxwithdrawperdaypercard") ?
                        decimal.Parse(miscJson["maxwithdrawperdaypercard"].ToString()) : 0;
                    
                    item.maxrebateperday = miscJson.ContainsKey("maxrebateperday") ?
                        decimal.Parse(miscJson["maxrebateperday"].ToString()) : 0;
                    
                    item.maxusdtrebateperday = miscJson.ContainsKey("maxusdtrebateperday") ?
                        decimal.Parse(miscJson["maxusdtrebateperday"].ToString()) : 0;

                    if (!miscJson.ContainsKey("gpAmountRange")) {
                        miscJson["gpAmountRange"] = miscJson["cgpAmountRange"];
                    }
                    if (!miscJson.ContainsKey("ebAmountRange"))
                    {
                        miscJson["ebAmountRange"] = miscJson["ebAmountRange"];
                    }
                    if (!miscJson.ContainsKey("alipayAmountRange"))
                    {
                        miscJson["alipayAmountRange"] = miscJson["alipayAmountRange"];
                    }

                    item.misc = Newtonsoft.Json.JsonConvert.SerializeObject(miscJson);
                }



                if (request.currency == "inr")
                {
                    List<等级列表Return> usergroup_inr = new List<等级列表Return>();
                    foreach (var item in rows)
                    {
                        var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<等级列表Return>(item.misc_inr);
                            temp.id = item.id;
                        
                        temp.levelnumber = item.levelnumber;
                        temp.levels = item.levels;
                        //temp.misc = item.misc;
                        temp.levelurl = item.levelurl;
                        usergroup_inr.Add(temp);
                    }
                    return new ResponseModel<IEnumerable<object>>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "",
                        Data = usergroup_inr
                    };
                }


                return new ResponseModel<IEnumerable<object>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = rows
                };
            }
        }


        private ResponseModel<IEnumerable<object>> PKR()
        {

            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT * " +
                "FROM ofUserGroup WITH(NOLOCK) /**where**/");

            builder.Where(" id>0 and ugCurrencyTypes LIKE '%586%' ");

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var rows = connection.Query<等级列表Return>(template.RawSql, template.Parameters);

                foreach (var row in rows)
                {
                    if (VipLevelINTL.levelIntlName.ContainsKey(row.levelnumber))
                        row.levels = VipLevelINTL.levelIntlName[row.levelnumber];
                }

                foreach (var item in rows)
                {
                    var misc_pkr_Dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.misc_pkr);
                    item.levelurl = misc_pkr_Dict.ContainsKey("levelurl") ? misc_pkr_Dict["levelurl"].ToString() : "";
                    item.ew_turnover = item.ew_turnover ?? item.ea_turnover;
                    item.ew_turnovermax = item.ew_turnovermax ?? item.ea_turnovermax;

                    var oss = item.onestepset;
                    oss = oss.Contains("|") ? oss.Split('|')[1] : oss;
                    var ossArray = oss.Contains(",") ? oss.Split(',') : null;
                    item.onestepset = string.Format("{0}送/{1}限/{2}倍", ossArray[1], ossArray[2], ossArray[3]);

                    var tss = item.twostepset;
                    var tssarray = tss.Contains("|") ? tss.Split('|') : new string[] { tss };
                    tss = tssarray[tssarray.Length - 1];
                    var tssArray = tss.Contains(",") ? tss.Split(',') : null;
                    item.twostepset = string.Format("{0}送/{1}限/{2}倍", tssArray[1], tssArray[2], tssArray[3]);


                    var miscJson = Newtonsoft.Json.Linq.JObject.Parse(item.misc);

                    item.bounspercentage = decimal.Parse(
                        Newtonsoft.Json.Linq.JObject.Parse(item.misc)["bonuspercentage"].ToString());

                    item.turnoverpercentage = decimal.Parse(
                        Newtonsoft.Json.Linq.JObject.Parse(item.misc)["turnoverpercentage"].ToString());

                    item.depositpercentage = decimal.Parse(
                        Newtonsoft.Json.Linq.JObject.Parse(item.misc)["depositpercentage"].ToString());

                    item.maxwithdrawperday = miscJson.ContainsKey("maxwithdrawperday") ?
                        decimal.Parse(miscJson["maxwithdrawperday"].ToString()) : 0;

                    item.maxwithdrawperdaypercard = miscJson.ContainsKey("maxwithdrawperdaypercard") ?
                        decimal.Parse(miscJson["maxwithdrawperdaypercard"].ToString()) : 0;

                    item.maxrebateperday = miscJson.ContainsKey("maxrebateperday") ?
                        decimal.Parse(miscJson["maxrebateperday"].ToString()) : 0;

                    item.maxusdtrebateperday = miscJson.ContainsKey("maxusdtrebateperday") ?
                        decimal.Parse(miscJson["maxusdtrebateperday"].ToString()) : 0;

                    if (!miscJson.ContainsKey("gpAmountRange"))
                    {
                        miscJson["gpAmountRange"] = miscJson["cgpAmountRange"];
                    }
                    if (!miscJson.ContainsKey("ebAmountRange"))
                    {
                        miscJson["ebAmountRange"] = miscJson["ebAmountRange"];
                    }
                    if (!miscJson.ContainsKey("alipayAmountRange"))
                    {
                        miscJson["alipayAmountRange"] = miscJson["alipayAmountRange"];
                    }

                    item.misc = Newtonsoft.Json.JsonConvert.SerializeObject(miscJson);
                }




                List<等级列表ReturnBase> usergroup_inr = new List<等级列表ReturnBase>();
                foreach (var item in rows)
                {
                    var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<等级列表ReturnBase>(item.misc_pkr);
                    temp.id = item.id;

                    temp.levelnumber = item.levelnumber;
                    temp.levels = item.levels;
                    temp.levelurl = item.levelurl;
                    usergroup_inr.Add(temp);
                }
                return new ResponseModel<IEnumerable<object>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = usergroup_inr
                };

            }
        }

        private ResponseModel<IEnumerable<object>> INR() {

            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT * " +
                "FROM ofUserGroup WITH(NOLOCK) /**where**/");

            builder.Where(" id>0 and ugCurrencyTypes LIKE '%356%' ");

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING)) {
                var rows = connection.Query<等级列表Return>(template.RawSql, template.Parameters);

                foreach (var row in rows) {
                    if(VipLevelINTL.levelIntlName.ContainsKey(row.levelnumber))
                        row.levels = VipLevelINTL.levelIntlName[row.levelnumber];
                }

                foreach (var item in rows) {
                    var misc_inr_Dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.misc_inr);
                    item.levelurl = misc_inr_Dict.ContainsKey("levelurl") ? misc_inr_Dict["levelurl"].ToString() : "";
                    item.ew_turnover = item.ew_turnover ?? item.ea_turnover;
                    item.ew_turnovermax = item.ew_turnovermax ?? item.ea_turnovermax;

                    var oss = item.onestepset;
                    oss = oss.Contains("|") ? oss.Split('|')[1] : oss;
                    var ossArray = oss.Contains(",") ? oss.Split(',') : null;
                    item.onestepset = string.Format("{0}送/{1}限/{2}倍", ossArray[1], ossArray[2], ossArray[3]);

                    var tss = item.twostepset;
                    var tssarray = tss.Contains("|") ? tss.Split('|') : new string[] { tss };
                    tss = tssarray[tssarray.Length - 1];
                    var tssArray = tss.Contains(",") ? tss.Split(',') : null;
                    item.twostepset = string.Format("{0}送/{1}限/{2}倍", tssArray[1], tssArray[2], tssArray[3]);


                    var miscJson = Newtonsoft.Json.Linq.JObject.Parse(item.misc);

                    item.bounspercentage = decimal.Parse(
                        Newtonsoft.Json.Linq.JObject.Parse(item.misc)["bonuspercentage"].ToString());

                    item.turnoverpercentage = decimal.Parse(
                        Newtonsoft.Json.Linq.JObject.Parse(item.misc)["turnoverpercentage"].ToString());

                    item.depositpercentage = decimal.Parse(
                        Newtonsoft.Json.Linq.JObject.Parse(item.misc)["depositpercentage"].ToString());

                    item.maxwithdrawperday = miscJson.ContainsKey("maxwithdrawperday") ?
                        decimal.Parse(miscJson["maxwithdrawperday"].ToString()) : 0;

                    item.maxwithdrawperdaypercard = miscJson.ContainsKey("maxwithdrawperdaypercard") ?
                        decimal.Parse(miscJson["maxwithdrawperdaypercard"].ToString()) : 0;

                    item.maxrebateperday = miscJson.ContainsKey("maxrebateperday") ?
                        decimal.Parse(miscJson["maxrebateperday"].ToString()) : 0;

                    item.maxusdtrebateperday = miscJson.ContainsKey("maxusdtrebateperday") ?
                        decimal.Parse(miscJson["maxusdtrebateperday"].ToString()) : 0;

                    if (!miscJson.ContainsKey("gpAmountRange")) {
                        miscJson["gpAmountRange"] = miscJson["cgpAmountRange"];
                    }
                    if (!miscJson.ContainsKey("ebAmountRange")) {
                        miscJson["ebAmountRange"] = miscJson["ebAmountRange"];
                    }
                    if (!miscJson.ContainsKey("alipayAmountRange")) {
                        miscJson["alipayAmountRange"] = miscJson["alipayAmountRange"];
                    }

                    item.misc = Newtonsoft.Json.JsonConvert.SerializeObject(miscJson);
                }




                List<等级列表ReturnBase> usergroup_inr = new List<等级列表ReturnBase>();
                foreach (var item in rows) {
                    var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<等级列表ReturnBase>(item.misc_inr);
                    temp.id = item.id;

                    temp.levelnumber = item.levelnumber;
                    temp.levels = item.levels;
                    temp.levelurl = item.levelurl;
                    usergroup_inr.Add(temp);
                }
                return new ResponseModel<IEnumerable<object>>() {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = usergroup_inr
                };

            }
        }
    }
}
