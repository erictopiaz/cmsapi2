using ShareEntity.InputModels.Promo;
using ShareEntity.Models;
using ShareEntity.OutModels.Promo;
using ShareEntity.SqlCommandText;
using DBLibs;
using DBLibs.WXDapper;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.JWT;
using UtilsLib.Merchant;

namespace CMS.API.BLL.Promo
{
    public class PromoBLL
    {

        /// <summary>
        /// 加载当前的自助优惠的分类菜单
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> AutoPromoTypes(QueryAutoPromoTypesInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = " ofAutoPromoNaviMenu ";
            string orderBy = " sortid asc ";
            string keyName = " promoMenuCode ";

            whereSql = string.Format(@" status = {0}", inputModel.status);
            
            var leftjoinString = "";
            var fields = " promoMenuCode, promoMenuName ";

            return await DapperPaging<QueryAutoPromoTypesOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex);
        }

        /// <summary>
        /// 优惠页面内容创建
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> CreatePromoContent(CreatePromoContentInputModel inputModel)
        {
            if (inputModel.currencyType == -1)
                return ApiResultModel<object>.GetResult("false", "请选择货币", "error_currencyType");

            if (!MerchantCodes.merhantMaps.ContainsKey(inputModel.p_merchantAccounts))
            {
                return ApiResultModel<object>.GetResult("false", "请选择品牌", "error_currencyType");
            }
            var merModel = MerchantCodes.merhantMaps[inputModel.p_merchantAccounts];

            var models = await DapperBase.QueryAsync<CreatePromoContentInputModel>(PromoSqlCommandText.LoadPromoContentByIdSql,
                new { tid = inputModel.id }, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

            inputModel.vips = Newtonsoft.Json.JsonConvert.SerializeObject(inputModel.vipLevels);
            inputModel.p_walletType = merModel.walletType;
            var sql = (models != null && models.Count > 0) ? PromoSqlCommandText.UpdatePromoContentSql_V : PromoSqlCommandText.CreatePromoContentSql_V;
           
            var result = await DapperBase.ExecuteTransctionAsync(sql,
                    inputModel, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

            return new ApiResultModel<object>
            {
                code = result?"true":"false",
                message = result ? "优惠页面内容创建-成功" : "优惠页面内容创建-失败",
                message_code = result ? "promo_content_create_success" : "promo_content_create_failed"
            };
        }
        /// <summary>
        /// 获取指定优惠
        /// </summary>
        /// <param name="tid">优惠的ID</param>
        /// <returns></returns>
        public static async Task<object> LoadPromoContentById(long tid)
        {
            var models = await DapperBase.QueryAsync<CreatePromoContentInputModel>(PromoSqlCommandText.LoadPromoContentByIdSql,
                    new { tid = tid }, commandTimeout: 15000, 
                    ConnectionString: DapperStringBase.ConnectionStringReadOnly);
            var result = (models != null && models.Count > 0);
            return new ApiResultModel<object>
            {
                code = result ? "true":"false",
                message = result ? "获取优惠页面内容-成功" : "获取优惠页面内容-失败",
                message_code = result ? "promo_content_load_success" : "promo_content_load_failed",
                data = result ? models.First() : null
            };
        }
        /// <summary>
        /// 优惠活动内容查询
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> PromoContentRecords(QueryPromoContentInputModel inputModel,
            string token,
            string loginKey)
        {
            string whereSql = string.Empty;
            var viewName = " ofPromotionContent a ";
            string orderBy = " a.id desc";
            string keyName = " a.id ";

            inputModel.adminAccount = JwtHelper.GetName(token, loginKey);
            var currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(token, loginKey);
            if (!currencys.Contains(inputModel.currencyType)) {
                return new AgentApiResultModel<object, object>() {
                    code = "false",
                    message = "请选择货币查询",
                    message_code = "error_currency_search",
                    levels = "",
                };
            }

            if (!string.IsNullOrEmpty(inputModel.affaccount))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.affaccount = '{0}' ", inputModel.affaccount) : string.Format(@" {0} and a.affaccount = '{1}' ", whereSql, inputModel.affaccount);
            
            if (!string.IsNullOrEmpty(inputModel.createcustomer))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.createcustomer = '{0}' ", inputModel.createcustomer) : string.Format(@" {0} and a.createcustomer = '{1}' ", whereSql, inputModel.createcustomer);

            if (inputModel.status != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.status = {0} ", inputModel.status) : string.Format(@" {0} and a.status = {1} ", whereSql, inputModel.status);
            
            if (inputModel.promotype != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.promotype = {0} ", inputModel.promotype) : string.Format(@" {0} and a.promotype = {1} ", whereSql, inputModel.promotype);

            if (inputModel.promocatalog != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.promocatalog = {0} ", inputModel.promocatalog) : string.Format(@" {0} and a.promocatalog = {1} ", whereSql, inputModel.promocatalog);

            if (inputModel.isDepositListPromo != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.isDepositListPromo = {0} ", inputModel.isDepositListPromo) : string.Format(@" {0} and a.isDepositListPromo = {1} ", whereSql, inputModel.isDepositListPromo);


            if (!string.IsNullOrEmpty(inputModel.startTime)
               && !string.IsNullOrEmpty(inputModel.endTime))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" endtime >= '{0}' and endtime < '{1}'", inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and endtime >= '{1}' and endtime < '{2}'", whereSql, inputModel.startTime, inputModel.endTime);
            }

            if (inputModel.currencyType != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.currencyType = {0} ", inputModel.currencyType) 
                    : string.Format(@" {0} and a.currencyType = {1} ", whereSql, inputModel.currencyType);
            
            if (!inputModel.merchantAccount.Equals("-1"))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.p_merchantAccounts like '%{0}%' ", inputModel.merchantAccount)
                    : string.Format(@" {0} and a.p_merchantAccounts like '%{1}%' ", whereSql, inputModel.merchantAccount);
            }

            if (inputModel.walletType != -1)
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.p_walletType in ({0},2) ", inputModel.walletType)
                    : string.Format(@" {0} and a.p_walletType in ({1},2) ", whereSql, inputModel.walletType);
            }

            var leftjoinString = " left join ofGameClass g on a.gametype = g.gameid";
            var fields = "  a.*,a.p_merchantAccounts merchantAccount,a.p_walletType walletType,g.gamename2 ";

            return await DapperPaging<QueryPromoContentOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex);
        }
    }
}
