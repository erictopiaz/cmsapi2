using CMS.Data.Models;
using DBLibs;
using DBLibs.WXDapper;
using ShareEntity.InputModels.Promo;
using ShareEntity.Models;
using ShareEntity.OutModels.Promo;
using ShareEntity.SqlCommandText;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.JWT;
using UtilsLib.Merchant;

namespace CMS.API.BLL.Promo
{
    public class PromoMiscBLL
    {
        /// <summary>
        /// 优惠积分查询返回
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> PromoMiscRecords(QueryPromoMiscInputModel inputModel,
            string token,
            string loginKey)
        {
            string whereSql = string.Empty;
            var viewName = " ofPromoMisc a with(nolock) ";
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
            var accounts = string.IsNullOrEmpty(inputModel.account) ? "" : MerchantCodes.GetAccounts(inputModel.account);
            var affaccounts = string.IsNullOrEmpty(inputModel.affaccount) ? "" : MerchantCodes.GetAccounts(inputModel.affaccount);

            if (string.IsNullOrEmpty(inputModel.merchantAccount))
                inputModel.merchantAccount = "-1";
            else
            {
                inputModel.merchantAccount = inputModel.merchantAccount.Replace("'", "''");
            }


            if (!string.IsNullOrEmpty(inputModel.startTime)
               && !string.IsNullOrEmpty(inputModel.endTime)) {
                //
            }
            else {
                var currentDay = DateTime.UtcNow.AddHours(8);
                inputModel.endTime = currentDay.ToString("yyyy-MM-dd HH:mm:ss");
                inputModel.startTime = currentDay.AddDays(-7).ToString("yyyy-MM-dd 00:00:00");
            }
            whereSql = string.Format(@" a.createtime >= '{0}' and a.createtime < '{1}'", inputModel.startTime, inputModel.endTime);

            if (!string.IsNullOrEmpty(inputModel.account))
            {
                whereSql = string.Format(@" {0} and a.account in {1} ", whereSql, accounts);
            }
            if (!string.IsNullOrEmpty(inputModel.affaccount))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" b.affaccount in {0} ", affaccounts) 
                    : string.Format(@" {0} and b.affaccount in {1} ", whereSql, affaccounts);

            if (inputModel.status != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.status = {0} ", inputModel.status) : string.Format(@" {0} and a.status = {1} ", whereSql, inputModel.status);

            if (inputModel.type != -1 )
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.type = {0} ", inputModel.type) : string.Format(@" {0} and a.type = {1} ", whereSql, inputModel.type);


            if (inputModel.currencyType != -1) {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" b.currencyType = {0} ", inputModel.currencyType) : string.Format(@" {0} and b.currencyType = {1} ", whereSql, inputModel.currencyType);
            }
            if (!inputModel.merchantAccount.Equals("-1"))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" b.merchantAccount = '{0}' ", inputModel.merchantAccount)
                    : string.Format(@" {0} and b.merchantAccount = '{1}' ", whereSql, inputModel.merchantAccount);
            }

            if (inputModel.walletType != -1)
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" b.walletType = {0} ", inputModel.walletType)
                    : string.Format(@" {0} and b.walletType = {1} ", whereSql, inputModel.walletType);
            }

            var leftjoinString = " left join ofUser b with(nolock) on a.account = b.account  left join ofUserExtention ue with(nolock) on a.account = ue.account ";
            var fields = " a.id as tid,b.id uid,a.account,a.type,a.content as remark,a.createtime,a.points,a.left_points,a.createperson,a.status,a.rowstatus,b.affaccount,b.anomaly,b.walletType,b.merchantAccount,ue.riskRemark,ue.riskTypesAready ";

            return await DapperPaging<QueryPromoMiscOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex);
        }
        /// <summary>
        /// 创建积分
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> CreatePromoMisc(CreatePromoMiscInputModel inputModel)
        {

            var defModel = MerchantCodes.GetDefaultModel(inputModel.merchantAccount, inputModel.currencycode);
            if (defModel == null)
                return ApiResultModel<object>.GetResult("false", "请选择正确的品牌+货币", "error merchant account or currency");

            var userModels = await DapperBase.QueryAsync<ofUserx>(UserSqlCommandText.LoadUserDatailSql_mer,
                new
                {
                    account = inputModel.account,
                    currencycode = inputModel.currencycode,
                    merchantAccount = inputModel.merchantAccount
                }, commandTimeout: 1500, ConnectionString: DapperStringBase.ConnectionString);

            if (userModels != null && userModels.Count == 1) {
                var um = userModels.First();
                if (um.currencyType != inputModel.currencycode
                    || !um.merchantAccount.Equals(inputModel.merchantAccount, StringComparison.OrdinalIgnoreCase))
                {
                    return ApiResultModel<object>.GetResult("false", "请选择正确的品牌+货币", "error merchant account or currency");
                }

                if (inputModel.points > 0 && !string.IsNullOrEmpty(inputModel.account)
                    && !string.IsNullOrEmpty(inputModel.content))
                {
                    if (inputModel.points <= 0)
                        return ApiResultModel<object>.GetResult("false", "新增或扣除的积分-只能为正数!", "no_points_must_int_failed");
                    if (inputModel.type == 7)
                    {
                        if (string.IsNullOrEmpty(inputModel.remarks))
                            return ApiResultModel<object>.GetResult("false", "请填写备注信息!", "fill_remarks_failed");
                        else
                            inputModel.content = inputModel.remarks;
                    }

                    inputModel.createtime = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");
                    inputModel.createperson = inputModel.adminAccount;
                    inputModel.points = inputModel.status == 0 ? inputModel.points : -1 * inputModel.points;
                    inputModel.rowstatus = 0;
                    if (inputModel.status != 0 && inputModel.status != 1)
                        return ApiResultModel<object>.GetResult("false", "请选择积分调整类型", "no_select_points_type_failed");


                    var result = await DapperBase.ExecuteTransctionAsync(PromoSqlCommandText.CreatePromoMiscSql_V,
                            new
                            {
                                account = inputModel.account,
                                type = inputModel.type,
                                content = inputModel.content,
                                createtime = inputModel.createtime,
                                points = inputModel.points,
                                status = inputModel.status,
                                createperson = inputModel.createperson,
                                rowstatus = inputModel.rowstatus,
                                left_points = 0,
                                pm_f_uid = um.uid
                            }, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                    return new ApiResultModel<object>
                    {
                        code = result ? "true" : "false",
                        message = result ? "积分创建-成功" : "积分创建-失败",
                        message_code = result ? "promo_points_create_success" : "promo_points_create_failed"
                    };
                }
                else
                {
                    return new ApiResultModel<object>
                    {
                        code = "false",
                        message = "优惠积分创建-失败[请确认提交信息是否正确]",
                        message_code = "promo_points_create_failed"
                    };
                }
            }
            else
            {
                return new ApiResultModel<object>
                {
                    code = "false",
                    message = "优惠积分创建-失败[账号确认有误或品牌选择有误]",
                    message_code = "account check failed"
                };
            }
            
        }
        /// <summary>
        /// 审核积分
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> VerifyPromoMisc(VerifyPromoMiscInputModel inputModel)
        {
            var model = await DapperBase.QueryAsync<ofPromoMisc>(PromoSqlCommandText.LoadPromoMiscByIdSql,
                        new { tid = inputModel.tid }, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            if (model != null && model.Count > 0)
            {
                var UpdatePromoMiscByIdSql = string.Format(PromoSqlCommandText.UpdatePromoMiscByIdSql, model.First().points);
                var sqlArray = new List<string> { UpdatePromoMiscByIdSql, UserSqlCommandText.UpdatePointsASql };
                var param = new List<object> {
                    new { rowstatus = 1, tid = inputModel.tid, reviewperson=inputModel.adminAccount,account=model.First().account },
                    new {bonuspoints=model.First().points, account=model.First().account}
                };
                var result = await DapperBase.ExecuteTransctionAsync(sqlArray,
                    param, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                return new ApiResultModel<object>
                {
                    code = result ? "true" : "false",
                    message = result ? "积分审核-成功" : "积分审核-失败",
                    message_code = result ? "promo_points_verify_success" : "promo_points_verify_failed"
                };
            }
            else
            {
                return new ApiResultModel<object>
                {
                    code = "false",
                    message = "优惠积分创建-失败[请确认提交信息是否正确]",
                    message_code = "promo_points_create_failed"
                };
            }
        }

        /// <summary>
        /// 微调积分
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> CancelPromoMisc(VerifyPromoMiscInputModel inputModel)
        {
            var model = await DapperBase.QueryAsync<ofPromoMisc>(PromoSqlCommandText.LoadPromoMiscByIdSql,
                        new { tid = inputModel.tid }, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            if (model != null && model.Count > 0)
            {
                var UpdatePromoMiscByIdSql = string.Format(PromoSqlCommandText.UpdatePromoMiscByIdSql, 0);

                var result = await DapperBase.ExecuteTransctionAsync(UpdatePromoMiscByIdSql,
                        new { tid = inputModel.tid, rowstatus = 2, reviewperson = inputModel.adminAccount, account = model.First().account }, 
                        commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                return new ApiResultModel<object>
                {
                    code = result ? "true" : "false",
                    message = result ? "积分取消-成功" : "积分取消-失败",
                    message_code = result ? "promo_points_cancel_success" : "promo_points_cancel_failed"
                };
            }
            else
            {
                return new ApiResultModel<object>
                {
                    code = "false",
                    message = "优惠积分创建-失败[请确认提交信息是否正确]",
                    message_code = "promo_points_create_failed"
                };
            }
                
        }
    }
}
