using CMS.API.Models;
using CMS.Core.Application.系统管理.Transact;
using DBLibs;
using DBLibs.WXDapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using Org.BouncyCastle.Crypto;
using ShareEntity.Global;
using ShareEntity.InputModels.OfSetting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.JWT;

namespace CMS.API.BLL.Sys
{
    /// <summary>
    /// 参设置
    /// </summary>
    public class OfSettingBLL
    {
        #region 网站参数配置 updateWebsiteSetting

        /// <summary>
        /// 查询网站配置参数
        /// </summary>
        /// <returns></returns> 
        public static async Task<object> QueryWebSiteNavSetting()
        {

            var updateSql = string.Format(@"select body from ofSettings where code='WEBSITE_NAV'");

            var body = await DapperBase.ExecuteScalarValueAsync(updateSql, null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
            var code = body != null;
            return JsonConvert.DeserializeObject<WebSiteNavSettingsModel>(body.ToString());
        }

        /// <summary>
        /// 查询网站配置参数
        /// </summary>
        /// <param name="merchantAccount"></param>
        /// <param name="currencyType"></param>
        /// <returns></returns>
        public static async Task<object> QueryWebSiteNavSettingINR(string merchantAccount, int currencyType)
        {
            var querySql = string.Format(@"select body from ofSettings with(nolock) where code='WEBSITE_NAV_{0}_{1}'", merchantAccount, currencyType);
           
            var body = await DapperBase.ExecuteScalarValueAsync(querySql, null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
            var code = body != null;
            return JsonConvert.DeserializeObject<WebSiteNavSettingsModel>(body.ToString());
        }

        /// <summary>
        /// 更新网站参数配置
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> updateWebSiteSetting(WebSiteSettingsModels inputModel)
        {
            inputModel.website.icon = "kf_pic1.png";
            inputModel.website1.icon = "kf_pic2.png";
            inputModel.website2.icon = "kf_pic1.png";
            inputModel.website3.icon = "kf_pic2.png";
            inputModel.website2.isNeedAppendToken = 1;
            inputModel.website3.isNeedAppendToken = 1;
            var settings = new Dictionary<string, object>()
            {
                { "website", inputModel.website},
                { "website1" , inputModel.website1},
                { "website2" , inputModel.website2},
                { "website3" , inputModel.website3},
                { "qq", inputModel.qq }
            };

            var updateSql = string.Format(@"update ofSettings set body=N'{0}',status=1 where code='WEBSITE_SETTINGS_ALL' and id=10041 ",
                Newtonsoft.Json.JsonConvert.SerializeObject(settings));

            var updateResult = await DapperBase.ExecuteTransctionAsync(updateSql, null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);

            return ApiResponseModel<object>.GetResult(
                updateResult ? 1 : 0,
                updateResult ? "网站参数配置-成功" : "网站参数配置-失败",
                updateResult ? "wb_settings_success" : "wb_settings_failed");
        }

        /// <summary>
        /// 查询网站配置参数
        /// </summary>
        /// <returns></returns>
        public static async Task<object> QueryWebSiteSetting()
        {

            var updateSql = string.Format(@"select body from ofSettings
where code='WEBSITE_SETTINGS_ALL' and id=10041");

            var body = await DapperBase.ExecuteScalarValueAsync(updateSql, null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
            var code = body != null;
            return ApiResponseModel<object>.GetResult(
                code ? 1 : 0,
                code ? "获取网站配置-成功" : "获取网站配置-失败",
                code ? "query_wb_settings_success" : "query_wb_settings_failed",
                code ? JsonConvert.DeserializeObject<WebSiteSettingsModels>(body.ToString()) : null);
        }

        #endregion

        #region 合营参数配置
        /// <summary>
        /// 查询合营推广地址host参数配置
        /// </summary>
        /// <returns></returns>
        public static async Task<object> QueryAgentRegisterHostSetting()
        {
            var queryAgentHostSql = string.Format(@"select body from ofSettings where code='agent_default_hosts_new'");
            var hosts = await DapperBase.ExecuteScalarValueAsync(queryAgentHostSql, null,
                commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            var hostDictionary = JsonConvert.DeserializeObject<AgentRegisterHostModel>(hosts.ToString());
            //AgentRegisterHostModel
            var code = hosts != null;
            return ApiResponseModel<object>.GetResult(
                code ? 1 : 0,
                code ? "获取合营注册配置-成功" : "获取合营注册配置-失败",
                code ? "query_agent_spread_success" : "query_agent_spread_failed",
                code ? hostDictionary : null);
        }

        /// <summary>
        /// 更新合营推广地址host参数配置
        /// </summary>
        /// <returns></returns>
        public static async Task<object> UpdateAgentRegisterHostSetting(AgentRegisterHostModel model)
        {
            var bayoHost = model.Bayo.Replace("'", "''");
            var bossHost = model.Boss.Replace("'", "''");
            var awcHost = model.Awc.Replace("'", "''");
            var dict = new Dictionary<string, object>()
            {
                {"Bayo", bayoHost },
                {"Boss", bossHost },
                {"Awc", awcHost }
            };
            var body = JsonConvert.SerializeObject(dict);
            var updateAgentHostSql = string.Format(@"update ofSettings set body=N'{0}' where code='agent_default_hosts_new'", body);
            
            var r = await DapperBase.ExecuteTransctionAsync(updateAgentHostSql, null,
                commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

            return ApiResponseModel<object>.GetResult(
                 r ? 1 : 0,
                 r ? "成功" : "失败",
                 r ? "update_agent_spread_success" : "update_agent_spread_failed");
        }

        //

        /// <summary>
        /// 更新接口地址列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<object> UpdateApiUrlModelSetting(ApiUrlModel model)
        {
            var oldUrl = model.oldUrl.Replace("'", "''").Trim();
            var newUrl = model.newUrl.Replace("'", "''").Trim();

            var queryApiUrlSql = string.Format(@"select body from ofSettings where code='app_call_api_lists'");
            var apiUrlObj = await DapperBase.ExecuteScalarValueAsync(queryApiUrlSql, null,
                commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            if (model.oldUrl.Equals(apiUrlObj.ToString()))
            {
                var updatePromoUrlSql = string.Format(@"update ofSettings set body=N'{0}' where code='app_call_api_lists'", newUrl);
                var r = await DapperBase.ExecuteTransctionAsync(updatePromoUrlSql, null,
                    commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

                return ApiResponseModel<object>.GetResult(
                     r ? 1 : 0,
                     r ? "成功" : "失败",
                     r ? "update_api_url_success" : "update_api_url_failed");
            }
            else
            {
                return ApiResponseModel<object>.GetResult(
                 0,
                 "未知旧地址",
                 "not found old url");
            }

        }


        /// <summary>
        /// 更新优惠的跳转url
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<object> UpdatePromoJumpUrlSetting(PromoJumpUrlModel model)
        {
            var oldUrl = model.oldUrl.Replace("'", "''").Trim();
            var newUrl = model.newUrl.Replace("'", "''").Trim();

            var queryPromoUrlSql = string.Format(@"select top 1 id from ofPromotionContent where promoDescPageUrl='{0}'", model.oldUrl);
            if( !(await DapperBase.ExecuteScalarAsync(queryPromoUrlSql, null,
                commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString))){
                return ApiResponseModel<object>.GetResult(
                 0,
                 "未知旧地址",
                 "not found old url");
            }

            var updatePromoUrlSql = string.Format(@"update ofPromotionContent set promoDescPageUrl='{0}' where promoDescPageUrl='{1}'", newUrl, oldUrl);
            var r = await DapperBase.ExecuteTransctionAsync(updatePromoUrlSql, null,
                commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

            return ApiResponseModel<object>.GetResult(
                 r ? 1 : 0,
                 r ? "成功" : "失败",
                 r ? "update_promo_url_success" : "update_promo_url_failed");
        }

        #endregion




        #region app参数配置
        /// <summary>
        /// 更新APP参数配置
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> UpdateAppSetting(AppSettingsModel inputModel)
        {
            var settings = new Dictionary<string, AppSetting>()
            {
                { "quanzhan_ios", inputModel.quanzhan_ios},
                { "quanzhan_android" , inputModel.quanzhan_android},
                { "sport_ios", inputModel.sport_ios },
                { "sport_android", inputModel.sport_android },
                { "agent_ios", inputModel.agent_ios },
                { "agent_android", inputModel.agent_android },
                { "keno_ios", inputModel.keno_ios },
                { "keno_android", inputModel.keno_android },
                { "nav_iphone", inputModel.nav_iphone },
                { "nav_android", inputModel.nav_android },
                { "kf_iphone", inputModel.kf_iphone },
                { "kf_android", inputModel.kf_android }
            };

            foreach(var m in settings)
            {
                if (m.Key.Contains("ios"))
                    m.Value.appType = 0;
                else
                    m.Value.appType = 1;
            }


            var updateSql = string.Format(@"update ofSettings set body=N'{0}',status=1 where code='APP_SETTINGS_ALL' and id=10040 ",
                Newtonsoft.Json.JsonConvert.SerializeObject(settings));

            var updateResult = await DapperBase.ExecuteTransctionAsync(updateSql, null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);

            return ApiResponseModel<object>.GetResult(
                updateResult ? 1 : 0,
                updateResult ? "App配置-成功" : "App配置-失败",
                updateResult ? "app_settings_success" : "app_settings_failed");
        }
        /// <summary>
        /// 查询app配置参数
        /// </summary>
        /// <returns></returns>
        public static async Task<object> QueryAppSetting()
        {
           
            var updateSql = string.Format(@"select body from ofSettings
where code='APP_SETTINGS_ALL' and id=10040");

            var body = await DapperBase.ExecuteScalarValueAsync(updateSql, null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
            var code = body != null;
            return ApiResponseModel<object>.GetResult(
                code ? 1 : 0,
                code ? "获取App配置-成功" : "获取App配置-失败",
                code ? "query_app_settings_success" : "query_app_settings_failed",
                code ? JsonConvert.DeserializeObject<AppSettingsModel>(body.ToString()) : null);
        }

        #endregion

        
        #region app intl 参数配置
        /// <summary>
        /// 更新APP参数配置
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> UpdateAppSettingIntl(AppSettingsModel inputModel) {
            var settings = new Dictionary<string, AppSetting>()
            {
                { "quanzhan_ios", inputModel.quanzhan_ios},
                { "quanzhan_android" , inputModel.quanzhan_android},
                { "sport_ios", inputModel.sport_ios },
                { "sport_android", inputModel.sport_android },
                { "agent_ios", inputModel.agent_ios },
                { "agent_android", inputModel.agent_android },
                { "keno_ios", inputModel.keno_ios },
                { "keno_android", inputModel.keno_android },
                { "nav_iphone", inputModel.nav_iphone },
                { "nav_android", inputModel.nav_android },
                { "kf_iphone", inputModel.kf_iphone },
                { "kf_android", inputModel.kf_android }
            };

            foreach (var m in settings) {
                if (m.Key.Contains("ios"))
                    m.Value.appType = 0;
                else
                    m.Value.appType = 1;

                if (!string.IsNullOrEmpty(m.Value.content)) {
                    var contents = m.Value.content.Split("###");
                    var langs = new List<LangModel>() {
                            new LangModel(){ lang = 1, body =contents.Count()>1?contents[1]:""},
                            new LangModel(){ lang = 2, body =contents.Count()>0?contents[0]:""},
                            new LangModel(){ lang = 5, body =contents.Count()>2?contents[2]:""},
                            new LangModel(){ lang = 7, body =contents.Count()>3?contents[3]:""}
                        };
                    m.Value.contentLangs = JsonConvert.SerializeObject(langs);
                }
                else {
                    var langs = new List<LangModel>() {
                            new LangModel(){ lang = 1, body =""},
                            new LangModel(){ lang = 2, body =""},
                            new LangModel(){ lang = 5, body =""},
                            new LangModel(){ lang = 7, body =""}
                        };
                    m.Value.contentLangs = JsonConvert.SerializeObject(langs);
                }

            }
            //var querySql = string.Format(@"select body from ofSettings with(nolock) where code='APP_SETTINGS_ALL_USD' and id=10077");
            //var body = await DapperBase.ExecuteScalarValueAsync(querySql, null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            //var versionModel = JsonConvert.DeserializeObject<AppSettingsModel>(body.ToString());
            //if (!inputModel.quanzhan_android.version.Equals(versionModel.quanzhan_android.version)) {
            //    inputModel.quanzhan_android.lasetVersion = versionModel.quanzhan_android.version;
            //    settings["quanzhan_android"] = inputModel.quanzhan_android;
            //}
            //if (!inputModel.quanzhan_ios.version.Equals(versionModel.quanzhan_ios.version)) {
            //    inputModel.quanzhan_ios.lasetVersion = versionModel.quanzhan_ios.version;
            //    settings["quanzhan_ios"] = inputModel.quanzhan_ios;
            //}


            var updateSql = string.Format(@"update ofSettings set body=N'{0}',status=1 where code='APP_SETTINGS_ALL_USD' and id=10077 ",
                Newtonsoft.Json.JsonConvert.SerializeObject(settings));

            var updateResult = await DapperBase.ExecuteTransctionAsync(updateSql, null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);

            return ApiResponseModel<object>.GetResult(
                updateResult ? 1 : 0,
                updateResult ? "App配置-成功" : "App配置-失败",
                updateResult ? "app_settings_success" : "app_settings_failed");
        }
        /// <summary>
        /// 查询app配置参数
        /// </summary>
        /// <returns></returns>
        public static async Task<object> QueryAppSettingIntl() {

            var updateSql = string.Format(@"select body from ofSettings
where code='APP_SETTINGS_ALL_USD' and id=10077");

            var body = await DapperBase.ExecuteScalarValueAsync(updateSql, null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
            var code = body != null;
            return ApiResponseModel<object>.GetResult(
                code ? 1 : 0,
                code ? "获取App配置-成功" : "获取App配置-失败",
                code ? "query_app_settings_success" : "query_app_settings_failed",
                code ? JsonConvert.DeserializeObject<AppSettingsModel>(body.ToString()) : null);
        }

        #endregion

        #region app Boss 参数配置
        /// <summary>
        /// 更新APP参数配置
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> UpdateAppSettingBoss(AppSettingsModel inputModel)
        {
            var settings = new Dictionary<string, AppSetting>()
            {
                { "quanzhan_ios", inputModel.quanzhan_ios},
                { "quanzhan_android" , inputModel.quanzhan_android},
                { "sport_ios", inputModel.sport_ios },
                { "sport_android", inputModel.sport_android },
                { "agent_ios", inputModel.agent_ios },
                { "agent_android", inputModel.agent_android },
                { "keno_ios", inputModel.keno_ios },
                { "keno_android", inputModel.keno_android },
                { "nav_iphone", inputModel.nav_iphone },
                { "nav_android", inputModel.nav_android },
                { "kf_iphone", inputModel.kf_iphone },
                { "kf_android", inputModel.kf_android }
            };

            foreach (var m in settings)
            {
                if (m.Key.Contains("ios"))
                    m.Value.appType = 0;
                else
                    m.Value.appType = 1;

                if (!string.IsNullOrEmpty(m.Value.content))
                {
                    var contents = m.Value.content.Split("###");
                    var langs = new List<LangModel>() {
                            new LangModel(){ lang = 1, body =contents.Count()>1?contents[1]:""},
                            new LangModel(){ lang = 2, body =contents.Count()>0?contents[0]:""},
                            new LangModel(){ lang = 5, body =contents.Count()>2?contents[2]:""},
                            new LangModel(){ lang = 7, body =contents.Count()>3?contents[3]:""}
                        };
                    m.Value.contentLangs = JsonConvert.SerializeObject(langs);
                }
                else
                {
                    var langs = new List<LangModel>() {
                            new LangModel(){ lang = 1, body =""},
                            new LangModel(){ lang = 2, body =""},
                            new LangModel(){ lang = 5, body =""},
                            new LangModel(){ lang = 7, body =""}
                        };
                    m.Value.contentLangs = JsonConvert.SerializeObject(langs);
                }

            }
           
            var updateSql = string.Format(@"update ofSettings set body=N'{0}',status=1 where code='APP_SETTINGS_ALL_BOSS_CNY' ",
                Newtonsoft.Json.JsonConvert.SerializeObject(settings));

            var updateResult = await DapperBase.ExecuteTransctionAsync(updateSql, null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);

            return ApiResponseModel<object>.GetResult(
                updateResult ? 1 : 0,
                updateResult ? "App配置-成功" : "App配置-失败",
                updateResult ? "app_settings_success" : "app_settings_failed");
        }
        /// <summary>
        /// 查询app配置参数
        /// </summary>
        /// <returns></returns>
        public static async Task<object> QueryAppSettingBoss()
        {

            var updateSql = string.Format(@"select body from ofSettings with(nolock)
where code='APP_SETTINGS_ALL_BOSS_CNY'");

            var body = await DapperBase.ExecuteScalarValueAsync(updateSql, null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
            var code = body != null;
            return ApiResponseModel<object>.GetResult(
                code ? 1 : 0,
                code ? "获取App配置-成功" : "获取App配置-失败",
                code ? "query_app_settings_success" : "query_app_settings_failed",
                code ? JsonConvert.DeserializeObject<AppSettingsModel>(body.ToString()) : null);
        }

        #endregion




        #region app首页游戏参数配置
        /// <summary>
        /// 更新APP首页游戏参数配置
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> UpdateAppIndexSetting(AppIndexGameSettingModels inputModel)
        {
      
            var settings = new Dictionary<string, object>()
            {
                { "sportGroup", inputModel.sportGroup},
                { "pokerGroup" , inputModel.pokerGroup},
                { "esportGroup", inputModel.esportGroup },
                { "liveGroup1", inputModel.liveGroup1 },
                { "liveGroup2", inputModel.liveGroup2 },
                { "kenoGroup1", inputModel.kenoGroup1 },
                { "kenoGroup2", inputModel.kenoGroup2 },
                { "kenoGroup3", inputModel.kenoGroup3 }
            };

            var updateSql = string.Format(@"update ofSettings set body=N'{0}',status=1 where code='APP_INDEX_GAME_SETTINGS_ALL' and id=10042 ",
                Newtonsoft.Json.JsonConvert.SerializeObject(settings));

            var updateResult = await DapperBase.ExecuteTransctionAsync(updateSql, null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);

            return ApiResponseModel<object>.GetResult(
                updateResult ? 1 : 0,
                updateResult ? "App首页游戏配置-成功" : "App首页游戏配置-失败",
                updateResult ? "app_index_settings_success" : "app_index_settings_failed");
        }
        /// <summary>
        /// 查询app首页游戏配置参数
        /// </summary>
        /// <returns></returns>
        public static async Task<object> QueryAppIndexSetting()
        {

            var updateSql = string.Format(@"select body from ofSettings
where code='APP_INDEX_GAME_SETTINGS_ALL' and id=10042");

            var body = await DapperBase.ExecuteScalarValueAsync(updateSql, null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
            var code = body != null;
            return ApiResponseModel<object>.GetResult(
                code ? 1 : 0,
                code ? "获取App首页游戏配置-成功" : "获取App首页游戏配置-失败",
                code ? "query_app_index_settings_success" : "query_app_index_settings_failed",
                code ? JsonConvert.DeserializeObject<AppIndexGameSettingModels>(body.ToString()) : null);
        }

        #endregion


        #region app老虎机游戏参数配置
        /// <summary>
        /// 更新APP老机游戏参数配置
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> UpdateAppSlotSetting(AppSlotGameSettingModels inputModel)
        {

            var settings = new Dictionary<string, object>()
            {
                { "ptGroup", inputModel.ptGroup},
                { "pgGroup" , inputModel.pgGroup},
                { "ttgGroup", inputModel.ttgGroup },
                { "cq9Group", inputModel.cq9Group },
                { "agfGroup", inputModel.agfGroup }
            };

            var updateSql = string.Format(@"update ofSettings set body=N'{0}',status=1 where code='APP_SLOT_GAME_SETTINGS_ALL' and id=10043 ",
                Newtonsoft.Json.JsonConvert.SerializeObject(settings));

            var updateResult = await DapperBase.ExecuteTransctionAsync(updateSql, null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);

            return ApiResponseModel<object>.GetResult(
                updateResult ? 1 : 0,
                updateResult ? "App老虎机游戏配置-成功" : "App老虎机游戏配置-失败",
                updateResult ? "app_Slot_settings_success" : "app_Slot_settings_failed");
        }
        /// <summary>
        /// 查询app老虎机游戏配置参数
        /// </summary>
        /// <returns></returns>
        public static async Task<object> QueryAppSlotSetting()
        {

            var updateSql = string.Format(@"select body from ofSettings
where code='APP_SLOT_GAME_SETTINGS_ALL' and id=10043");

            var body = await DapperBase.ExecuteScalarValueAsync(updateSql, null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
            var code = body != null;
            return ApiResponseModel<object>.GetResult(
                code ? 1 : 0,
                code ? "获取App老虎机游戏配置-成功" : "获取App老虎机游戏配置-失败",
                code ? "query_app_Slot_settings_success" : "query_app_Slot_settings_failed",
                code ? JsonConvert.DeserializeObject<AppSlotGameSettingModels>(body.ToString()) : null);
        }

        #endregion


        #region app游戏登录模式
        /// <summary>
        /// 更新APP游戏登录模式
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> UpdateGameUrlMode(AppGameUrlModeSettingModels inputModel)
        {

            //var updateSql = string.Format(@"update ofSettings set body=N'{0}',status=1 where code='APP_GAME_URL_MODE_SETTINGS_ALL' and id=10044 ",
            //    Newtonsoft.Json.JsonConvert.SerializeObject(inputModel.selectedBoxes));

            var updateSql = string.Format(@"update ofUser set appGameUrlMode=N'{0}' where account='{1}' ",
                JsonConvert.SerializeObject(inputModel.selectedBoxes), inputModel.account);

            var updateResult = await DapperBase.ExecuteTransctionAsync(updateSql, null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);

            return ApiResponseModel<object>.GetResult(
                updateResult ? 1 : 0,
                updateResult ? "App游戏登录模式配置-成功" : "App游戏登录模式配置-失败",
                updateResult ? "app_Slot_settings_success" : "app_Slot_settings_failed");

        }
        /// <summary>
        /// 查询app游戏登录模式
        /// </summary>
        /// <returns></returns>
        public static async Task<object> QueryGameUrlMode(QueryAppGameUrlModeInputModels model)
        {

            //            var updateSql = string.Format(@"select body from ofSettings
            //where code='APP_GAME_URL_MODE_SETTINGS_ALL' and id=10044");

            string querySql = string.IsNullOrEmpty(model.account) ? string.Format(@"SELECT appGameUrlMode FROM ofUser WHERE id={0}", Convert.ToInt64(model.id)) :
                string.Format(@"SELECT appGameUrlMode FROM ofUser WHERE account='{0}'", model.account);

            var body = await DapperBase.ExecuteScalarValueAsync(querySql, param: null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);


            var gameListSql = string.Format(@"select gamename2,gameid from ofGameClass
where class=1 and useable=1 ");

            //GameUrlModeSelectList
            var sls = await DapperBase.QueryAsync<GameUrlModeSelectList>(gameListSql, null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);

            var code = body != null;
            return ApiResponseModel<object>.GetResult(
                1, "获取App游戏登录模式配置-成功", "query_app_Slot_settings_success",
                code ? new { gameList = sls, selectedBoxes = JsonConvert.DeserializeObject<List<int>>(body.ToString()) } :
                new { gameList = sls, selectedBoxes = new List<int>() });
        }

        #endregion


        #region 加载系统开启的币种

        public static async Task<object> LoadCurrency(string currencys) {
            try {

                var currencySql = string.Format(@"select cu.currencyname,cu.currencycode,cu.currencyLang from ofCurrency cu with(nolock)
where cu.online=1");
                var currencyTypes = JsonConvert.DeserializeObject<List<int>>(currencys);
                var currencyModels = await DapperBase.QueryAsync<CurrencyModel>(currencySql,
                    null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);
                var accessCurrencys = currencyModels.Where(item => currencyTypes.Any(e => item.currencycode == e));

                return ApiResponseModel<object>.GetResult(1 ,"成功" ,"app_Slot_settings_failed", accessCurrencys);
            }
            catch (Exception) {
                return ApiResponseModel<object>.GetResult(1, "失败-e", "app_Slot_settings_failed", new List<CurrencyModel>());
            }

            
        }
        /// <summary>
        /// 返回货币+品牌
        /// </summary>
        /// <param name="currencys"></param>
        /// <returns></returns>
        public static async Task<object> LoadCurrencyV(string currencys)
        {
            try
            {

                var currencySql = string.Format(@"select cu.currencyname,cu.currencycode,cu.currencyLang from ofCurrency cu with(nolock)
where cu.online=1");
                var currencyTypes = JsonConvert.DeserializeObject<List<int>>(currencys);
                var currencyModels = await DapperBase.QueryAsync<CurrencyModel>(currencySql,
                    null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);
                var accessCurrencys = currencyModels.Where(item => currencyTypes.Any(e => item.currencycode == e));

                return ApiResponseModel<object>.GetResult(1, "成功", "app_Slot_settings_failed", new BaseSearchModel()
                {
                    currencyModels = accessCurrencys.ToList()
                });
            }
            catch (Exception)
            {
                return ApiResponseModel<object>.GetResult(1, "失败-e", "app_Slot_settings_failed", new BaseSearchModel());
            }

        }

        #endregion
    }
}
