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
using Newtonsoft.Json.Linq;
using CMS.Core.Application.账号管理.Queries;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 等级Edit : 等级列表Return, IRequest<等级EditReturn>
    {
        [JsonIgnore]
        public string Admin { get; set; }
        public string? currencyCode { get; set; }

        public string currency { get; set; }
        

        public AmountRange usdtERCAmountRange { set; get; } = new AmountRange() { min = 3500, max = 100000 };
        public AmountRange usdtTRCAmountRange { set; get; } = new AmountRange() { min = 100, max = 100000 };
        public AmountRange inrAmountRange { set; get; } = new AmountRange() { min = 500, max = 200000 };


    }

    public class 等级EditReturn : ResponseModel
    {
    }

    public class MiscInrModel : 等级列表ReturnBase {
        public AmountRange usdtERCAmountRange { set; get; }
        public AmountRange usdtTRCAmountRange { set; get; } 
        public AmountRange inrAmountRange { set; get; }
    }


    public class AmountRange {
        public decimal min { set; get; }
        public decimal max { set; get; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 等级EditValidator : AbstractValidator<等级Edit>
    {
        public 等级EditValidator()
        {
            //TODO: Validation populate
            RuleFor(x => x.levelnumber)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 等级EditHandler : BaseRequestHandler<等级Edit, 等级EditReturn>
    {
        public 等级EditHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override 等级EditReturn Handle(等级Edit request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));



            if (request.currencyCode == "356")
            {
                return INR(request);
            }



            if (request.currencyCode == "586")
            {
                return PKR(request);
            }


            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var id = request.id;
                var row = connection.Get<ofUserGroupModified>(id);

                if (string.IsNullOrEmpty(request.currency))
                    return new 等级EditReturn()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = ""
                    };


                // INR SECTION
                if (request.currency == "inr")
                {
                    //string misc_inr;


                    var misc_inr = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                    var misc_inr_json = JObject.Parse(misc_inr);
                    misc_inr_json.Remove("misc");
                    misc_inr_json.Remove("misc_inr");
                    misc_inr_json.Remove("currency");
                    misc_inr = misc_inr_json.ToString(Formatting.None);
                    row.misc_inr = misc_inr;

                    var misc_inr_update = connection.Update(row);
                    return new 等级EditReturn()
                    {
                        code = misc_inr_update ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                        message = ""
                    };
                }


                // CNY SECTION 
                if (request.currency != "cny")
                    return new 等级EditReturn()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = ""
                    };

                row.ea_turnover = request.ew_turnover;
                row.ea_turnovermax = request.ew_turnovermax;

                row.ew_turnover = request.ew_turnover;
                row.ew_turnovermax = request.ew_turnovermax;
                row.pt_turnover = request.pt_turnover;
                row.pt_turnovermax = request.pt_turnovermax;
                row.keno_turnover = request.kg_turnover;
                row.keno_turnovermax = request.kg_turnovermax;
                row.bbin_turnover = request.bbin_turnover;
                row.bbin_turnovermax = request.bbin_turnovermax;
                row.spb_turnover = request.spb_turnover;
                row.spb_turnovermax = request.spb_turnovermax;
                row.ttg_turnover = request.ttg_turnover;
                row.ttg_turnovermax = request.ttg_turnovermax;
                row.lb_turnover = request.lb_turnover;
                row.lb_turnovermax = request.lb_turnovermax;
                row.ag_turnover = request.ag_turnover;
                row.ag_turnovermax = request.ag_turnovermax;
                row.keno2_turnover = request.kg2_turnover;
                row.keno2_turnovermax = request.kg2_turnovermax;
                row.vr_turnover = request.vr_turnover;
                row.vr_turnovermax = request.vr_turnovermax;
                row.xj_turnover = request.xj_turnover;
                row.xj_turnovermax = request.xj_turnovermax;
                row.pg_turnover = request.pg_turnover;
                row.pg_turnovermax = request.pg_turnovermax;
                row.ds_turnover = request.ds_turnover;
                row.ds_turnovermax = request.ds_turnovermax;
                row.dses_turnover = request.dses_turnover;
                row.dses_turnovermax = request.dses_turnovermax;
                row.ob_turnover = request.ob_turnover;
                row.ob_turnovermax = request.ob_turnovermax;

                row.bti_turnover = request.bti_turnover;
                row.bti_turnovermax = request.bti_turnovermax;

                row.imes_turnover = request.imes_turnover;
                row.imes_turnovermax = request.imes_turnovermax;
                row.rg_turnover = request.rg_turnover;
                row.rg_turnovermax = request.rg_turnovermax;
                row.ky_turnover = request.ky_turnover;
                row.ky_turnovermax = request.ky_turnovermax;
                row.ysb_turnover = request.ysb_turnover;
                row.ysb_turnovermax = request.ysb_turnovermax;
                row.kg2_turnover = request.kg2_turnover;
                row.kg2_turnovermax = request.kg2_turnovermax;
                row.kg_turnover = request.kg_turnover;
                row.kg_turnovermax = request.kg_turnovermax;
                row.bg_turnover = request.bg_turnover;
                row.bg_turnovermax = request.bg_turnovermax;
                row.eb_turnover = request.eb_turnover;
                row.eb_turnovermax = request.eb_turnovermax;
                row.td_turnover = request.td_turnover;
                row.td_turnovermax = request.td_turnovermax;
                row.tf_turnover = request.tf_turnover;
                row.tf_turnovermax = request.tf_turnovermax;
                row.cq_turnover = request.cq_turnover;
                row.cq_turnovermax = request.cq_turnovermax;
                row.aw_turnover = request.aw_turnover;
                row.aw_turnovermax = request.aw_turnovermax;
                row.kg3_turnover = request.kg3_turnover;
                row.kg3_turnovermax = request.kg3_turnovermax;
                row.impk_turnover = request.impk_turnover;
                row.impk_turnovermax = request.impk_turnovermax;
                row.jdb_turnover = request.jdb_turnover;
                row.jdb_turnovermax = request.jdb_turnovermax;
                row.obg_turnover = request.obg_turnover;
                row.obg_turnovermax = request.obg_turnovermax;
                row.fb_turnover = request.obg_turnover;
                row.fb_turnovermax = request.obg_turnovermax;
                row.sw_turnover = request.sw_turnover;
                row.sw_turnovermax = request.sw_turnovermax;
                row.imsp_turnover = request.imsp_turnover;
                row.imsp_turnovermax = request.imsp_turnovermax;
                row.obcv_turnover = request.obcv_turnover;
                row.obcv_turnovermax = request.obcv_turnovermax;
                row.cr_turnover = request.cr_turnover;
                row.cr_turnovermax = request.cr_turnovermax;
                row.pm_turnover = request.pm_turnover;
                row.pm_turnovermax = request.pm_turnovermax;
                row.db_turnover = request.db_turnover;
                row.db_turnovermax = request.db_turnovermax;
                row.rsg_turnover = request.rsg_turnover;
                row.rsg_turnovermax = request.rsg_turnovermax;

                row.dbkeno_turnover = request.dbkeno_turnover;
                row.dbkeno_turnovermax = request.dbkeno_turnovermax;

                row.pp_turnover = request.pp_turnover;
                row.pp_turnovermax = request.pp_turnovermax;

                row.iaes_turnover = request.iaes_turnover;
                row.iaes_turnovermax = request.iaes_turnovermax;



                var requestMiscDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(request.misc);
                var miscDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(row.misc);
                if (miscDict.ContainsKey("bonuspercentage"))
                    miscDict["bonuspercentage"] = request.bounspercentage.ToString();
                if (miscDict.ContainsKey("depositpercentage"))
                    miscDict["depositpercentage"] = request.depositpercentage.ToString();
                if (miscDict.ContainsKey("turnoverpercentage"))
                    miscDict["turnoverpercentage"] = request.turnoverpercentage.ToString();

                if (miscDict.ContainsKey("maxwithdrawperday"))
                    miscDict["maxwithdrawperday"] = request.maxwithdrawperday.ToString();
                else
                    miscDict.Add("maxwithdrawperday", request.maxwithdrawperday.ToString());

                if (miscDict.ContainsKey("maxwithdrawperdaypercard"))
                    miscDict["maxwithdrawperdaypercard"] = request.maxwithdrawperdaypercard.ToString();
                else
                    miscDict.Add("maxwithdrawperdaypercard", request.maxwithdrawperdaypercard.ToString());

                if (miscDict.ContainsKey("maxusdtrebateperday"))
                    miscDict["maxusdtrebateperday"] = request.maxusdtrebateperday.ToString();
                else
                    miscDict.Add("maxusdtrebateperday", request.maxusdtrebateperday.ToString());

                if (miscDict.ContainsKey("maxrebateperday"))
                    miscDict["maxrebateperday"] = request.maxrebateperday.ToString();
                else
                    miscDict.Add("maxrebateperday", request.maxrebateperday.ToString());


                miscDict["rmbAmountRange"] = requestMiscDict["rmbAmountRange"];
                miscDict["cgpAmountRange"] = requestMiscDict["cgpAmountRange"];
                miscDict["gpAmountRange"] = requestMiscDict["gpAmountRange"];
                miscDict["ebAmountRange"] = requestMiscDict["ebAmountRange"];
                miscDict["alipayAmountRange"] = requestMiscDict["alipayAmountRange"];
                miscDict["usdtERCAmountRange"] = requestMiscDict["usdtERCAmountRange"];
                miscDict["usdtTRCAmountRange"] = requestMiscDict["usdtTRCAmountRange"];

                //if (miscDict.ContainsKey("rmbAmountRange"))
                //{
                //    miscDict["rmbAmountRange"]["min"] = request.rmbAmountRange_min;
                //    miscDict["rmbAmountRange"]["max"] = request.rmbAmountRange_max;
                //}
                //else {
                //    miscDict.Add("rmbAmountRange", new JObject()
                //    {
                //        ["min"] = request.rmbAmountRange_min,
                //        ["max"] = request.rmbAmountRange_max,
                //    });

                //}


                //if (miscDict.ContainsKey("usdtERCAmountRange"))
                //{
                //    miscDict["usdtERCAmountRange"]["min"] = request.usdtERCAmountRange_min;
                //    miscDict["usdtERCAmountRange"]["max"] = request.usdtERCAmountRange_max;
                //}
                //else
                //{
                //    miscDict.Add("usdtERCAmountRange", new JObject()
                //    {
                //        ["min"] = request.usdtERCAmountRange_min,
                //        ["max"] = request.usdtERCAmountRange_max,
                //    });

                //}

                //if (miscDict.ContainsKey("usdtTRCAmountRange"))
                //{
                //    miscDict["usdtTRCAmountRange"]["min"] = request.usdtTRCAmountRange_min;
                //    miscDict["usdtTRCAmountRange"]["max"] = request.usdtTRCAmountRange_max;
                //}
                //else
                //{
                //    miscDict.Add("usdtTRCAmountRange", new JObject()
                //    {
                //        ["min"] = request.usdtTRCAmountRange_min,
                //        ["max"] = request.usdtTRCAmountRange_max,
                //    });

                //}




                row.misc = Newtonsoft.Json.JsonConvert.SerializeObject(miscDict);
                var updateResult = connection.Update(row);

                var ofsettings = connection.Get<ofSettings>("GROUP_LEVELURL");
                var ofsettingsDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(ofsettings.body);
                if (ofsettingsDict.ContainsKey(request.levelnumber.ToString()))
                {
                    ofsettingsDict[request.levelnumber.ToString()] = request.levelurl;
                }
                else
                {
                    ofsettingsDict.Add(request.levelnumber.ToString(), request.levelurl);
                }
                ofsettings.body = Newtonsoft.Json.JsonConvert.SerializeObject(ofsettingsDict, Formatting.None);
                connection.Update(ofsettings);


                return new 等级EditReturn()
                {
                    code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }
        }



        private 等级EditReturn PKR(等级Edit request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {

                #region 新
                var id = request.id;
                if (request.currencyCode != "586")
                    return new 等级EditReturn()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = ""
                    };

                //         public AmountRange usdtERCAmountRange { set; get; }
                //public AmountRange usdtTRCAmountRange { set; get; }
                //public AmountRange inrAmountRange { set; get; }
                var miModel = new MiscInrModel()
                {
                    usdtERCAmountRange = request.usdtERCAmountRange,
                    usdtTRCAmountRange = request.usdtTRCAmountRange,
                    inrAmountRange = request.inrAmountRange,
                    id = request.id,
                    levelnumber = request.levelnumber,
                    levels = request.levels,
                    onestepset = request.onestepset,
                    twostepset = request.twostepset,
                    description = request.description,
                    _9w_turnover = request._9w_turnover,
                    _9w_turnovermax = request._9w_turnovermax,
                    pg_turnover = request.pg_turnover,
                    pg_turnovermax = request.pg_turnovermax,
                    evo_turnover = request.evo_turnover,
                    evo_turnovermax = request.evo_turnovermax,
                    jdb_turnover = request.jdb_turnover,
                    jdb_turnovermax = request.jdb_turnovermax,
                    spribe_turnover = request.spribe_turnover,
                    spribe_turnovermax = request.spribe_turnovermax,
                    turbo_turnover = request.turbo_turnover,
                    turbo_turnovermax = request.turbo_turnovermax,
                    jl_turnover = request.jl_turnover,
                    jl_turnovermax = request.jl_turnovermax,

                    levelurl = request.levelurl,
                    turnoverpercentage = request.turnoverpercentage,
                    bounspercentage = request.bounspercentage,
                    depositpercentage = request.depositpercentage,
                    maxusdtrebateperday = request.maxusdtrebateperday,
                    maxrebateperday = request.maxrebateperday,
                    maxwithdrawperday = request.maxwithdrawperday,
                    maxwithdrawperdaypercard = request.maxwithdrawperdaypercard
                };

                var misc_inr = JsonConvert.SerializeObject(miModel);

                var updateSql = string.Format(@"update ofUserGroup set misc_pkr=N'{1}'  where id={0}", id, misc_inr);
                var updateResult = connection.Execute(updateSql);

                #endregion

                return new 等级EditReturn()
                {
                    code = updateResult > 0 ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };

            }
        }



        private 等级EditReturn INR(等级Edit request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                #region 旧
                //var id = request.id;
                //var row = connection.Get<ofUserGroupModified>(id);

                //if (string.IsNullOrEmpty(request.currency))
                //    return new 等级EditReturn()
                //    {
                //        code = ShareEntity.Global.ResponseCode.code.FAIL,
                //        message = ""
                //    };


                //var misc_inr = JsonConvert.SerializeObject(request);
                //var misc_inr_json = JObject.Parse(misc_inr);
                //misc_inr_json.Remove("misc");
                //misc_inr_json.Remove("misc_inr");
                //misc_inr_json.Remove("currency");
                //misc_inr = misc_inr_json.ToString(Formatting.None);
                //row.misc_inr = misc_inr;
                //var misc_inr_update = connection.Update(row);

                #endregion

                #region 新
                var id = request.id;
                if (request.currencyCode != "356")
                    return new 等级EditReturn() {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = ""
                    };

                //         public AmountRange usdtERCAmountRange { set; get; }
                //public AmountRange usdtTRCAmountRange { set; get; }
                //public AmountRange inrAmountRange { set; get; }
                var miModel = new MiscInrModel() {
                    usdtERCAmountRange = request.usdtERCAmountRange,
                    usdtTRCAmountRange = request.usdtTRCAmountRange,
                    inrAmountRange = request.inrAmountRange,
                    id = request.id,
                    levelnumber = request.levelnumber,
                    levels = request.levels,
                    onestepset = request.onestepset,
                    twostepset = request.twostepset,
                    description = request.description,
                    _9w_turnover = request._9w_turnover,
                    _9w_turnovermax = request._9w_turnovermax,
                    pg_turnover = request.pg_turnover,
                    pg_turnovermax = request.pg_turnovermax,
                    evo_turnover = request.evo_turnover,
                    evo_turnovermax = request.evo_turnovermax,
                    jdb_turnover = request.jdb_turnover,
                    jdb_turnovermax = request.jdb_turnovermax,
                    spribe_turnover = request.spribe_turnover,
                    spribe_turnovermax = request.spribe_turnovermax,
                    turbo_turnover = request.turbo_turnover,
                    turbo_turnovermax = request.turbo_turnovermax,
                    jl_turnover = request.jl_turnover,
                    jl_turnovermax = request.jl_turnovermax,

                    levelurl = request.levelurl,
                    turnoverpercentage = request.turnoverpercentage,
                    bounspercentage = request.bounspercentage,
                    depositpercentage = request.depositpercentage,
                    maxusdtrebateperday = request.maxusdtrebateperday,
                    maxrebateperday = request.maxrebateperday,
                    maxwithdrawperday = request.maxwithdrawperday,
                    maxwithdrawperdaypercard = request.maxwithdrawperdaypercard
                };

                var misc_inr = JsonConvert.SerializeObject(miModel);

                var updateSql = string.Format(@"update ofUserGroup set misc_inr=N'{1}'  where id={0}", id, misc_inr);
                var updateResult = connection.Execute(updateSql);

                #endregion

                return new 等级EditReturn()
                {
                    code = updateResult>0 ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };

            }
        }
    }
}

