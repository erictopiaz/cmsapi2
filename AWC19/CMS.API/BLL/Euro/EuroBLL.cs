using CMS.API.Models.Euro;
using CMS.Data;
using ShareEntity.InputModels.Euro;
using ShareEntity.Models;
using ShareEntity.OutModels.Bonus;
using ShareEntity.SqlCommandText;
using DBLibs;
using DBLibs.WXDapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;
using ShareEntity;

namespace CMS.API.BLL.Euro
{
    /// <summary>
    /// 欧洲杯竞猜，数据处理
    /// </summary>
    public class EuroBLL
    {
        /// <summary>
        /// 获取竞猜数据结果
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> ECupBetRecords(QueryEuroCupBilledInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = " ofEuroBetTeams a ";

            //string orderBy = inputModel.billSort == 0 ? " amount desc " : inputModel.billSort == 1 ? " amount asc " : " financegameday desc ";

            string orderBy = " a.create_at desc ";
            string keyName = " a.id ";

            if (!string.IsNullOrEmpty(inputModel.account))
            {
                whereSql = string.Format(@" a.account = '{0}' ", inputModel.account);
            }

            if (inputModel.status != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.b_status = {0} ", inputModel.status) :
                    string.Format(@" {0} and a.b_status = {1} ", whereSql, inputModel.status);

            if (inputModel.matchs != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.groupid = {0} ", inputModel.matchs) :
                    string.Format(@" {0} and a.groupid = {1} ", whereSql, inputModel.matchs);

            if (inputModel.euroname != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.euroname = {0} ", inputModel.euroname) :
                    string.Format(@" {0} and a.euroname = {1} ", whereSql, inputModel.euroname);

            //var leftjoinString = " left join ofEuroBetTeamsBilled b on a.account = b.account  and a.groupid=b.groupid and a.euroname=b.euroname ";
            var leftjoinString = "";
            var fields = string.Format(@" a.id,a.account,a.groupid,a.betTeams,a.euroname, a.b_status, a.create_at ,a.reward amount, a.betCount, a.verify_at, a.who_verify");



            return await DapperPaging<EuroCupBilledResponse>.QueryPage(whereSql, viewName, leftjoinString,
               fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, connectStringBase: DapperStringBase.ConnectionLogs);


            //            var pageModel = await DapperPaging<object>.QueryPage(whereSql, viewName, leftjoinString,
            //                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, connectStringBase: DapperStringBase.ConnectionLogs);


            //            ApiResultModel<PageModel<object>> apiResultModel = pageModel as ApiResultModel<PageModel<object>>;

            //            if (apiResultModel.code == "true"
            //                && apiResultModel.data.TotalRecord > 0) //获取到分页数据,且有输入时间且审核成功
            //            {
            //                //获取统计数据： 

            //                var tsql = string.Format(@"select sum(amount) totalReward from ofEuroBetTeamsBilled
            //where {0} ", whereSql);

            //                var result = await DapperBase.QueryAsync<BonusReport>(tsql,
            //                    null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionLogs);

            //                var code = (result != null && result.Count > 0);
            //                if (code)
            //                {
            //                    apiResultModel.data.totalData = result.First();
            //                }
            //            }
            //            return apiResultModel;

        }


        /// <summary>
        /// 结算记录审核
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> VerifyECupBilled(EuroUpdateInputModel inputModel)
        {
            #region 
            var loadBetTeamsByIdSql = string.Format(EuroSqlText.LoadBetTeamsByIdSql, inputModel.id);
            var betTeamModels = await DapperBase.QueryAsync<BetTeamModel>(loadBetTeamsByIdSql,
                  null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionLogs);
            #endregion

            try
            {
                if (betTeamModels != null && betTeamModels.Count == 1)
                {
                    var userModel = await DapperBase.QueryAsync<ofUser>(UserSqlCommandText.LoadUserDatailSql,
                        new { account = betTeamModels.First().account }, commandTimeout: 1500, ConnectionString: DapperStringBase.ConnectionString);
                    if (userModel == null || userModel.Count <= 0)
                    {
                        //推荐人信息核实-失败
                        return ApiResultModel<object>.GetResult("false", "账号有误", "account_error_failed");
                    }
                    if (User.ControllUser(userModel.First().misc, 1))
                        return ApiResultModel<object>.GetResult("false", "优惠限制", "account_error_failed");


                    var updateSql = string.Format(EuroSqlText.UpdateBetTeamsByIdSql, inputModel.id, 1,
                        DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"), inputModel.adminAccount);
                    if (await DapperBase.ExecuteTransctionAsync(updateSql,
                        null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionLogs))
                    {
                        var sqlArray = new List<string>();
                        var param = new List<object> { };
                        //var des = new Dictionary<int, string>()
                        //{
                        //    { 32, "32进16" },
                        //    { 24, "24进16" },
                        //    { 16, "16进8" },
                        //    { 8, "8进4" },
                        //    { 4, "排名" }
                        //};
                        var des = new Dictionary<int, string>()
                        {
                            { 48, "48晋级16" },
                            { 32, "32晋级8" },
                            { 16, "16晋级4" },
                            { 8, "冠亚季排名" }
                        };
                        //更新客人金额
                        var referenceguid = Common.GuidToLongString();
                        var pm = new
                        {
                            currency = "cny",
                            account = betTeamModels.First().account,
                            amount = 0,
                            reward = betTeamModels.First().reward,
                            remarks = string.Format(@"{0},[{1}]", des[betTeamModels.First().groupid], betTeamModels.First().euroname),
                            level = userModel.First().level,
                            bet = 3,
                            createperson = inputModel.adminAccount,
                            bonusPromoType = 0,
                            reviewtime = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"),
                            gametype = 100,
                            referrerreg = userModel.First().referrerreg,
                            referenceguid = referenceguid,
                            ipaddress = inputModel.ipAddress,
                            status = 1
                        };
                        sqlArray.Add(PromoSqlCommandText.CreateVipMonthPromoSql);
                        param.Add(pm);

                        //更新主账号余额
                        sqlArray.Add(UserSqlCommandText.UpdateBalanceASql);
                        param.Add(new { account = betTeamModels.First().account, amount = betTeamModels.First().reward });

                        var result = await DapperBase.ExecuteTransctionAsync(sqlArray,
                            param, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                        return ApiResultModel<object>.GetResult(
                            result ? "true" : "false",
                            result ? "审核-成功" : "审核-失败",
                            result ? "euro_billed_verify_success" : "euro_billed_verify_failed"
                        );
                    }
                    else
                    {
                        return ApiResultModel<object>.GetResult("false", "审核失败!", "verify_euro_failed");
                    }

                }
                else
                {
                    return ApiResultModel<object>.GetResult("false", "账号检测失败!", "account_info_check_failed");
                }
            }
            catch (Exception)
            {
                return ApiResultModel<object>.GetResult("false", "审核失败e!", "account_info_check_failed");
            }
        }

        /// <summary>
        /// 结算记录审核取消
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> CancelECupBilled(EuroUpdateInputModel inputModel)
        {
            var updateSql = string.Format(EuroSqlText.UpdateBetTeamsByIdSql, inputModel.id, 2,
                DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"), inputModel.adminAccount);
            if (await DapperBase.ExecuteTransctionAsync(updateSql,
                null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionLogs))
            {
               
                return ApiResultModel<object>.GetResult("true" ,"取消-成功","euro_billed_cancel_success");
            }
            else
            {
                return ApiResultModel<object>.GetResult("false", "取消失败!", "cancel_euro_failed");
            }
        }
    }
}
