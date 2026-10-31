using CMS.API.Models;
using CMS.Data;
using DBLibs;
using DBLibs.WXDapper;
using Newtonsoft.Json;
using ShareEntity;
using ShareEntity.InputModels.Promo;
using ShareEntity.Models;
using ShareEntity.OutModels.Bonus;
using ShareEntity.OutModels.Promo;
using ShareEntity.OutModels.Referrals;
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
    public class SpreadBLL
    {
        /// <summary>
        /// 推荐记录
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> SpreadRecords(QuerySpreadInputModel inputModel,
            string token,
            string loginKey)
        {
            string whereSql = string.Empty;
            var viewName = " ofUser with(nolock) ";
            string orderBy = " id desc";
            string keyName = " id ";

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

            if (!string.IsNullOrEmpty(inputModel.account))
            {
                whereSql = string.Format(@" account in {0} ", accounts);
            }
            if (!string.IsNullOrEmpty(inputModel.affaccount))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" affaccount in {0} ", affaccounts) 
                    : string.Format(@" {0} and affaccount in {1} ", whereSql, affaccounts);

            if (inputModel.status != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" status = {0} ", inputModel.status) : string.Format(@" {0} and status = {1} ", whereSql, inputModel.status);
            
            if (!string.IsNullOrEmpty(inputModel.marcode))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" marcode = '{0}' ", inputModel.marcode) : string.Format(@" {0} and marcode = '{1}' ", whereSql, inputModel.marcode);
            
            if (inputModel.currencyType != -1) {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" currencyType = {0} ", inputModel.currencyType) : string.Format(@" {0} and currencyType = {1} ", whereSql, inputModel.currencyType);
            }

            whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" marcode IS not NULL and LEN(RTRIM(LTRIM(marcode))) != 0 ") : string.Format(@" {0} and marcode IS not NULL and LEN(RTRIM(LTRIM(marcode))) != 0 ", whereSql);
            if (!inputModel.merchantAccount.Equals("-1"))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" merchantAccount = '{0}' ", inputModel.merchantAccount)
                    : string.Format(@" {0} and merchantAccount = '{1}' ", whereSql, inputModel.merchantAccount);
            }

            if (inputModel.walletType != -1)
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" walletType = {0} ", inputModel.walletType)
                    : string.Format(@" {0} and walletType = {1} ", whereSql, inputModel.walletType);
            }

            var leftjoinString = "";
            var fields = " id as tid,account,createtime,depositnumber,depositamount,withdrawalamount,marcode, (withdrawalamount-depositamount) as wd ,status,walletType,merchantAccount ";

            return await DapperPaging<QuerySpreadOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex);
        }


        public static async Task<object> SpreadBilledRecords(QuerySpreadBilledInputModel inputModel,
            string token,
            string loginKey)
        {
            string whereSql = string.Empty;
            var viewName = " promoSpreadMonthBilled a with(nolock) ";
           
            string orderBy = inputModel.billSort == 0 ? " a.spreadSettleAmount desc " 
                : inputModel.billSort == 1 ? " a.spreadSettleAmount asc " : " a.financegameday desc ";

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


            if (string.IsNullOrEmpty(inputModel.merchantAccount))
                inputModel.merchantAccount = "-1";
            else
            {
                inputModel.merchantAccount = inputModel.merchantAccount.Replace("'", "''");
            }

            if (!string.IsNullOrEmpty(inputModel.account))
            {
                whereSql = string.Format(@" a.account in {0} ", accounts);
            }
            
            if (inputModel.status != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.status = {0} ", inputModel.status) : 
                    string.Format(@" {0} and a.status = {1} ", whereSql, inputModel.status);


            if (inputModel.startTime > 0 && inputModel.endTime > 0
               && inputModel.endTime >= inputModel.startTime)
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.financeGameDay >= {0} and a.financeGameDay <= {1}", inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and a.financeGameDay >= {1} and a.financeGameDay <= {2}", whereSql, inputModel.startTime, inputModel.endTime);
            }
            else
            {
                //默认为上月数据
                var curDate = DateTime.UtcNow.AddHours(8);
                var month = Convert.ToInt64(curDate.AddMonths(-1).ToString("yyyyMM"));
                inputModel.startTime = month;
                inputModel.endTime = month;
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.financeGameDay >= {0} and a.financeGameDay <= {1}", inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and a.financeGameDay >= {1} and a.financeGameDay <= {2}", whereSql, inputModel.startTime, inputModel.endTime);

            }

            if (inputModel.currencyType != -1) {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" u.currencyType = {0} ", inputModel.currencyType) 
                    : string.Format(@" {0} and u.currencyType = {1} ", whereSql, inputModel.currencyType);
            }
            if (!inputModel.merchantAccount.Equals("-1"))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" u.merchantAccount = '{0}' ", inputModel.merchantAccount)
                    : string.Format(@" {0} and u.merchantAccount = '{1}' ", whereSql, inputModel.merchantAccount);
            }

            if (inputModel.walletType != -1)
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" u.walletType = {0} ", inputModel.walletType)
                    : string.Format(@" {0} and u.walletType = {1} ", whereSql, inputModel.walletType);
            }

            var leftjoinString = " left join ofUser u with(nolock) on a.account=u.account " +
                " left join ofUserExtention ue with(nolock) on a.account=ue.account ";
            var fields = string.Format(@" a.id,u.id uid, u.merchantAccount,u.walletType, a.account,a.financegameday,a.depositAmount,
(pg_bet+jdb_bet+rsg_bet)  slot_bets,
			(ob_bet+imes_bet+rg_bet+tf_bet+obg_bet+imsp_bet+obcv_bet+fb_bet+bti_bet) es_bets,
			(lb_bet+kg3_bet+aw_bet+vr_bet+ky_bet+sw_bet) cp_bets,
			(bbin_bet+ag_bet+db_bet) live_bets , a.amount,a.spreadSettleAmount, a.status,a.reviewperson,a.reason,iif( u.anomaly is not null, u.anomaly,0) anomaly, ue.riskRemark, iif(ue.riskTypesAready is not null,ue.riskTypesAready,0) riskTypesAready ");


            var pageModel = await DapperPaging<object>.QueryPage(whereSql, viewName, leftjoinString,
                    fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, DapperStringBase.ConnectionStringReadOnly);

            ApiResultModel<PageModel<object>> apiResultModel = pageModel as ApiResultModel<PageModel<object>>;

            if (apiResultModel.code == "true"
                && apiResultModel.data.TotalRecord > 0 ) //获取到分页数据,且有输入时间且审核成功
            {
                //获取统计数据： 

                var tsql = string.Format(@"select sum(amount) totalReward from promoSpreadMonthBilled a
left join ofUser u with(nolock) on a.account=u.account
where {0} ", whereSql);

                var result = await DapperBase.QueryAsync<BonusReport>(tsql,
                    null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

                var code = (result != null && result.Count > 0);
                if (code)
                {
                    apiResultModel.data.totalData = result.First();
                }
            }
            return apiResultModel;

        }

        /// <summary>
        /// 推荐投注日结算
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="token"></param>
        /// <param name="loginKey"></param>
        /// <returns></returns>
        public static async Task<object> SpreadBilledRecordsINTL_Daily(QuerySpreadBilledIntlInputModel inputModel,
            string token,
            string loginKey) 
        {
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

            //inputModel.dataType = 0;//这里只查自身数据
            var viewName = " promoSpreadIntlDayBilled t ";
            string orderBy = " t.financeGameDay desc";
            string keyName = " t.financeGameDay ";
            var whereSql = string.Empty;
            var leftjoinString = string.Empty;
            var fields = string.Empty;
            var currentDate = DateTime.UtcNow.AddHours(8);
            if (inputModel.startTime < 0) {

                inputModel.startTime = Convert.ToInt64(currentDate.AddDays(-1).ToString("yyyyMMdd"));
                inputModel.endTime = Convert.ToInt64(currentDate.AddDays(-1).ToString("yyyyMMdd"));
            }

            if (inputModel.dataType == 0) {
                //只查询推荐人本人的
                if(!string.IsNullOrEmpty(inputModel.account)) {
                    whereSql = string.Format(@" u.account='{0}' ", inputModel.account);
                    whereSql = string.Format(@" {0} and t.financeGameDay>={1} and t.financeGameDay<={2} and t.referralType=1 ",
                        whereSql, inputModel.startTime, inputModel.endTime);
                }
                else {
                    whereSql = string.Format(@" t.financeGameDay>={0} and t.financeGameDay<={1} and t.referralType=1 ", 
                        inputModel.startTime, inputModel.endTime);
                }

                leftjoinString = string.Format(@" left join ofUser u with(nolock) on t.account = u.account ");
                fields = " t.account,u.createtime,u.depositamount, financeGameDay,  selfWaggered, waggered,selfRate, rate,selfCommission, commission, t.verifyTime ";
            }
            else {
                //推荐人下线 或 会员本人
                
                if (!string.IsNullOrEmpty(inputModel.account)) {
                    whereSql = string.Format(@" u.account='{0}' ", inputModel.account);
                }
                if (!string.IsNullOrEmpty(inputModel.marcode)) {
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" u.marcode='{0}' ", inputModel.marcode)
                        : string.Format(@" {0} and u.marcode='{1}' ", whereSql, inputModel.marcode);
                }

                if(string.IsNullOrEmpty(inputModel.account) 
                    && string.IsNullOrEmpty(inputModel.marcode))
                    return new AgentApiResultModel<object, object>() {
                        code = "false",
                        message = "请输入会员账号或推荐人账号",
                        message_code = "error_currency_search",
                        levels = "",
                    };

                whereSql = string.Format(@" {0} and t.financeGameDay>={1} and t.financeGameDay<={2} and t.referralType=0 ", whereSql, inputModel.startTime, inputModel.endTime);
                leftjoinString = string.Format(@" left join ofUser u with(nolock) on t.account = u.account ");
                fields = " t.account,u.createtime,u.depositamount,financeGameDay, selfWaggered, waggered,selfRate, rate,selfCommission, commission, t.verifyTime ";
            }

            var pageModel = await DapperPaging<ReferralPlayerModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, connectStringBase: DapperStringBase.ConnectionStringReadOnly);

            ApiResultModel<PageModel<ReferralPlayerModel>> apiResultModel = pageModel as ApiResultModel<PageModel<ReferralPlayerModel>>;

            if (apiResultModel.code == "true"
                && apiResultModel.data.TotalRecord > 0) //获取到分页数据,且有输入时间且审核成功
            {
                //获取统计数据： 
                var tsql = string.Format(@"select sum(selfCommission+commission) totalReward from promoSpreadIntlDayBilled t
left join ofUser u with(nolock) on t.account=u.account
where {0} ", whereSql);

                var result = await DapperBase.QueryAsync<BonusReport>(tsql,
                    null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);

                var code = (result != null && result.Count > 0);
                if (code) {
                    apiResultModel.data.totalData = result.First();
                }
            }
            return apiResultModel;


        }
        /// <summary>
        /// 推荐充值周结算
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="token"></param>
        /// <param name="loginKey"></param>
        /// <returns></returns>
        public static async Task<object> SpreadBilledRecordsINTL_Weekly(QuerySpreadBilledIntlInputModel inputModel,
            string token,
            string loginKey) {
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

            if (string.IsNullOrEmpty(inputModel.merchantAccount))
                inputModel.merchantAccount = "-1";
            else
            {
                inputModel.merchantAccount = inputModel.merchantAccount.Replace("'", "''");
            }


            //inputModel.dataType = 0;//这里只查自身数据
            var viewName = " promoSpreadIntlWeeklyBilled t ";
            string orderBy = " t.financeGameDay desc";
            string keyName = " t.financeGameDay ";
            var whereSql = string.Empty;
            var leftjoinString = string.Empty;
            var fields = string.Empty;
            var currentDate = DateTime.UtcNow.AddHours(8);
            if (inputModel.startTime < 0) {
                //最近6个月结算数据
                //inputModel.startTime = Convert.ToInt64(currentDate.AddMonths(-6).ToString("yyyyMMdd"));
                //inputModel.endTime = Convert.ToInt64(currentDate.ToString("yyyyMMdd"));

                //上周结算数据
                var dateRanage = Common.GetDateRanage(33);
                var m = JsonConvert.DeserializeObject<DateRanage>(JsonConvert.SerializeObject(dateRanage.Item2));
                inputModel.startTime = m.startTime;
                inputModel.endTime = m.endTime;
            }

            //只查询推荐人本人的
            if (!string.IsNullOrEmpty(inputModel.account)) {
                whereSql = string.Format(@" u.account='{0}' ", inputModel.account);
                whereSql = string.Format(@" {0} and t.financeGameDay>={1} and t.financeGameDay<={2} and t.referralType=1 ",
                    whereSql, inputModel.startTime, inputModel.endTime);
            }
            else {
                whereSql = string.Format(@" t.financeGameDay>={0} and t.financeGameDay<={1} and t.referralType=1 ",
                    inputModel.startTime, inputModel.endTime);
            }

            if (inputModel.currencyType != -1)
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" u.currencyType = {0} ", inputModel.currencyType) 
                    : string.Format(@" {0} and u.currencyType = {1} ", whereSql, inputModel.currencyType);
            }

            if (!inputModel.merchantAccount.Equals("-1"))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" u.merchantAccount = '{0}' ", inputModel.merchantAccount)
                    : string.Format(@" {0} and u.merchantAccount = '{1}' ", whereSql, inputModel.merchantAccount);
            }

            if (inputModel.walletType != -1)
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" u.walletType = {0} ", inputModel.walletType)
                    : string.Format(@" {0} and u.walletType = {1} ", whereSql, inputModel.walletType);
            }


            leftjoinString = string.Format(@" left join ofUser u with(nolock) on t.account = u.account ");
            fields = " t.account,t.financeGameDay,u.createtime,u.walletType,u.merchantAccount,u.depositamount totalDepositAmount,t.depositAmount,t.referrals,t.commission,t.activeNumbers, t.rate, t.status, t.create_at verifyTime ";



            var pageModel = await DapperPaging<ReferralPlayerModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, connectStringBase: DapperStringBase.ConnectionStringReadOnly);

            ApiResultModel<PageModel<ReferralPlayerModel>> apiResultModel = pageModel as ApiResultModel<PageModel<ReferralPlayerModel>>;

            if (apiResultModel.code == "true"
                && apiResultModel.data.TotalRecord > 0) //获取到分页数据,且有输入时间且审核成功
            {
                //获取统计数据： 
                var tsql = string.Format(@"select sum(commission) totalReward from promoSpreadIntlWeeklyBilled t
left join ofUser u with(nolock) on t.account=u.account
where {0} ", whereSql);

                var result = await DapperBase.QueryAsync<BonusReport>(tsql,
                    null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);

                var code = (result != null && result.Count > 0);
                if (code) {
                    apiResultModel.data.totalData = result.First();
                }
            }
            return apiResultModel;


        }

        public static async Task<object> CancelSpreadBilled(CacelSpreadBilledInputModel inputModel)
        {
            //理新当红利优惠状态
            var updateSpreadPromoSql = string.Format(@"update promoSpreadMonthBilled set status=@status, reviewperson=@reviewperson,reason=@reason,update_time=dateadd(hour,(8),getutcdate()) where status=0 and id=@id");
            var result = await DapperBase.ExecuteTransctionAsync(updateSpreadPromoSql,
                  new { status = 2, id = inputModel.id, reviewperson = inputModel.adminAccount, reason = inputModel.cancelreason }, 
                  commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            
            return new ApiResultModel<object>()
            {
                code = result ? "true" : "false",
                message = result ? "取消推荐结算成功" : "取消推荐结算失败",
                message_code = result ? "cancel_spread_billed_success" : "cancel_spread_billed_failed"
            };
        }
        /// <summary>
        /// 推荐优惠审核
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>

        public static async Task<object> VerifySpreadBilled(CacelSpreadBilledInputModel inputModel)
        {
            var model = await GetSpreadBilledModel(inputModel.id);
            if (!model.Item1)
                return model.Item3;

            var remarks = string.Format(@"推荐优惠月结{0}", model.Item2.financegameday);
            var promo = await CreateBonusAndVerifyAsync(model.Item2.account, 0, model.Item2.spreadSettleAmount,
               remarks, 0, -1, 3, inputModel.AdminIpAddress, inputModel.adminAccount, inputModel.id, rguid: Common.GuidToLongString());
           
            return ApiResultModel<object>.GetResult(promo.Item1 ? "true" : "false", promo.Item2,
                   promo.Item1 ? "verify_spread_billed_success" : "verify_spread_billed_failed");

        }




        private static async Task<Tuple<bool, string>> CreateBonusAndVerifyAsync(string account, decimal amount,
            decimal reward, string remarks, long promoCode,
            int autopromotype, decimal betTimes, string ipaddress, 
            string adminaccount, long id, int platformType = -1, int gametype = 100, string rguid = "")
        {
            try
            {
                var userModel = await DapperBase.QueryAsync<OfUserSaler>(UserSqlCommandText.LoadUserDatailSalerSql,
                    new { account = account },
                        commandTimeout: 1500, ConnectionString: DapperStringBase.ConnectionString);


                if (userModel != null && userModel.Count == 1)
                {
                    var sqlArray = new List<string>();
                    var param = new List<object>();

                    reward = reward > 888 ? 888 : reward;//6月开始，上限金额 为 888

                    //更新推荐优惠状态
                    sqlArray.Add(BonusSqlCommandText.UpdateSpreadBilledStatusSql);
                    param.Add(new { status = 1, id = id, reviewperson = adminaccount, reason = "", amount = reward });

                    var referenceguid = string.IsNullOrEmpty(rguid) ? Common.GuidToLongString() : rguid;
                    var salerAccount = string.IsNullOrEmpty(userModel.First().saler) ? "awcsystem" : userModel.First().saler;
                    var affaccount = string.IsNullOrEmpty(userModel.First().affaccount) ? "allwincity" : userModel.First().affaccount;
                    //更新主账号余额
                    sqlArray.Add(UserSqlCommandText.UpdateBalanceASql);
                    param.Add(new { account = account, amount = reward });

                    //设置优惠和设置状态
                    sqlArray.Add(BonusSqlCommandText.CreatePromoAndUpdateWithSalerSql);
                    param.Add(new
                    {
                        currency = "CNY",
                        account = account,
                        amount = amount,
                        reward = reward,//上限金额 为 888  
                        remarks = remarks,
                        bonusPromoCode = promoCode,
                        autopromotype = autopromotype,
                        platformType = platformType,
                        level = userModel.First().level,
                        bet = betTimes,
                        createperson = adminaccount,
                        gametype = gametype,
                        gamePercentType = 5,//其它
                        bonusPromoType = 0,//红利  ，1 优惠
                        referrerreg = userModel.First().referrerreg,
                        referenceguid = referenceguid,
                        ipaddress = ipaddress,
                        macaddress = "",
                        status = 1,
                        createTime = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"),
                        saler = salerAccount,
                        aaffaccount = affaccount
                    });


                    var result = await DapperBase.ExecuteTransctionAsync(sqlArray, param,
                           commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

                    return new Tuple<bool, string>(result, result ? "推荐结算申请-成功" : "推荐结算申请-失败");

                }
                else
                {
                    return new Tuple<bool, string>(false, "核实账号信息-失败");
                }
            }
            catch (Exception)
            {
                return new Tuple<bool, string>(false, "推荐结算申请-失败e");
            }

        }



        /// <summary>
        /// 核实并返回
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async static Task<Tuple<bool, PromoSpreadMonthBilled, ApiResultModel<object>>> GetSpreadBilledModel(int id)
        {
            var result = await DapperBase.QueryAsync<PromoSpreadMonthBilled>(BonusSqlCommandText.LoadSpreadBilledModelSql, new { id = id },
                 commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

            if (result != null && result.Count == 1)
            {
                if (User.ControllUser(result.First().misc, 1))
                    return new Tuple<bool, PromoSpreadMonthBilled, ApiResultModel<object>>(true, result.First(), new ApiResultModel<object>
                    {
                        code = "false",
                        message = "当前账号禁止优惠",
                        message_code = "user_promo_frobbiden_failed"
                    });
                result.First().affaccount = string.IsNullOrEmpty(result.First().affaccount) ? "allwincity" : result.First().affaccount;
                return new Tuple<bool, PromoSpreadMonthBilled, ApiResultModel<object>>(true, result.First(), null);

            }
            else
                return new Tuple<bool, PromoSpreadMonthBilled, ApiResultModel<object>>(true, result.First(), new ApiResultModel<object>
                {
                    code = "false",
                    message = "核实优惠红利-失败",
                    message_code = "verify_bonus_failed"
                });
        }
    }
}
