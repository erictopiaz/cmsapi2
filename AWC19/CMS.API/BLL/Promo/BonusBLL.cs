using CMS.API.Models;
using CMS.Core.Application.Account.Queries;
using CMS.Core.Application.存款管理.Transact;
using CMS.Data;
using DBLibs;
using DBLibs.WXDapper;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using PromoShare;
using ShareEntity;
using ShareEntity.InputModels.Bonus;
using ShareEntity.InputModels.Promo;
using ShareEntity.Models;
using ShareEntity.OutModels.Bonus;
using ShareEntity.SqlCommandText;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.JWT;
using UtilsLib.lang;
using UtilsLib.Merchant;
using UtilsLib.SqlInject;

namespace CMS.API.BLL.Promo
{
    public class BonusBLL
    {
        /// <summary>
        /// 生成优惠或红利
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="token"></param>
        /// <param name="loginKey"></param>
        /// <returns></returns>
        public static async Task<object> BonusRecordsAsync(QueryBonusInputModel inputModel,
            string token,
            string loginKey)
        {
            string whereSql = string.Empty;
            var viewName = " ofBonusPromo a with(nolock) ";
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


            if (!string.IsNullOrEmpty(inputModel.uid)) {
                var account = AccountBase.QueryAccountById(Convert.ToInt64(inputModel.uid));
                inputModel.account = string.IsNullOrEmpty(account) ? inputModel.account : account;
            }


            if (!string.IsNullOrEmpty(inputModel.account))
            {
                whereSql = string.Format(@" a.account in {0} ", accounts);
            }

            if (inputModel.status != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.status = {0} ", inputModel.status) : string.Format(@" {0} and a.status = {1} ", whereSql, inputModel.status);

            if (inputModel.gametype != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.gametype = {0} ", inputModel.gametype) : string.Format(@" {0} and a.gametype = {1} ", whereSql, inputModel.gametype);
            
            if (inputModel.bonusPromoType != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.bonusPromoType = {0} ", inputModel.bonusPromoType) : string.Format(@" {0} and a.bonusPromoType = {1} ", whereSql, inputModel.bonusPromoType);

           
            if (!string.IsNullOrEmpty(inputModel.startTime)
                && !string.IsNullOrEmpty(inputModel.endTime))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.reviewtime >= '{0}' and a.reviewtime <= '{1}'",
                    inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and a.reviewtime >= '{1}' and a.reviewtime <= '{2}'", 
                    whereSql, inputModel.startTime, inputModel.endTime);
            }
            else {
                var currentDay = DateTime.UtcNow.AddHours(8);
                inputModel.startTime = currentDay.AddDays(-7).ToString("yyyy-MM-01 00:00:00");
                inputModel.endTime = currentDay.ToString("yyyy-MM-dd 23:59:59");
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.createtime >= '{0}' and a.createtime <= '{1}'", inputModel.startTime, inputModel.endTime) 
                    : string.Format(@" {0} and a.createtime >= '{1}' and a.createtime <= '{2}'", whereSql, inputModel.startTime, inputModel.endTime);

            }
            
            if (!string.IsNullOrEmpty(inputModel.remarks))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" remarks like '%{0}%' ", inputModel.remarks) 
                    : string.Format(@" {0} and remarks like '%{1}%' ", whereSql, inputModel.remarks);
            }

            whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.accountType = 0 ") : string.Format(@" {0} and a.accountType = 0 ", whereSql);

            if (!string.IsNullOrEmpty(inputModel.affaccount))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" u.affaccount in {0} ", affaccounts) : string.Format(@" {0} and u.affaccount in {1} ", whereSql, affaccounts);

            if (inputModel.currencyType != -1) {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" u.currencyType = {0} ", inputModel.currencyType) : string.Format(@" {0} and u.currencyType = {1} ", whereSql, inputModel.currencyType);
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

            var leftjoinString = string.Format(@" left join ofUser u with(nolock) on a.account = u.account  
left join ofUserExtention ue with(nolock) on a.account = ue.account
            left join ofUserGroup b with(nolock) on a.level = b.levelnumber ");
            var fields = " a.id as tid,a.account,a.amount,a.reward,a.remarks,a.createtime,a.bet,a.level,a.gametype,a.bonusPromoType,a.status,a.referenceguid,a.createperson,a.reviewperson,a.reviewtime,a.reason,a.ipaddress,a.bonusPromoCode,b.levels,(u.withdrawalamount-u.depositamount) Profit,u.id uid,u.anomaly,u.walletType,u.merchantAccount,ue.riskRemark,ue.riskTypesAready ";
           
            var currentTime = DateTime.UtcNow.AddHours(8);

            var apiResultModel = await DapperPaging<QueryBonusOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                    fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, DapperStringBase.ConnectionStringReadOnly);

            #region ching add 确认查询到的会员列表
            var _accounts = new List<string>();
            #endregion

            TimeSpan ts = DateTime.UtcNow.AddHours(8).Subtract(currentTime);
            if (inputModel.currencyType == (int)LangCodes.CurrencyCode.INR
                || inputModel.currencyType == (int)LangCodes.CurrencyCode.USD) {
                if (apiResultModel.data.modelList.Count > 0) {
                    foreach (var m in apiResultModel.data.modelList) {
                        if (VipLevelINTL.levelIntlName.ContainsKey(m.level))
                            m.levels = VipLevelINTL.levelIntlName[m.level];
                    }
                }
            }
            if(apiResultModel.code.Equals("true") 
                && apiResultModel.data.TotalRecord > 0)
            {
                foreach (var m in apiResultModel.data.modelList)
                {
                    if (_accounts.Contains(m.account.ToLower()))
                        continue;
                    else
                    {
                        _accounts.Add(m.account.ToLower());
                    }
                   
                }
                if (_accounts.Count > 0) {
                    var queryWDReportModel = AccountBase.QueryUserWDReport(_accounts);
                    if (queryWDReportModel != null && queryWDReportModel.Count() > 0)
                    {
                        foreach (var wd in queryWDReportModel)
                        {
                            foreach (var r in apiResultModel.data.modelList)
                            {
                                if (wd.account.Equals(r.account, StringComparison.OrdinalIgnoreCase))
                                {
                                    r.Profit = wd.withdrawalamount - wd.depositamount;
                                   
                                }
                            }
                        }
                    }
                }

            }

            if (apiResultModel.code.Equals("true")                
                && inputModel.currencyType != -1
                && apiResultModel.data.TotalRecord > 0 && !string.IsNullOrEmpty(inputModel.startTime)
                && !string.IsNullOrEmpty(inputModel.endTime)) //获取到分页数据,且有输入时间
            {
                //获取统计数据： 

                var tsql = string.Format(@"select sum(a.reward) totalReward
from ofBonusPromo a with(nolock)
left join ofUserGroup b with(nolock) on a.level = b.levelnumber  
left join ofUser u with(nolock) on a.account = u.account
where {0} ", whereSql);

                var result = await DapperBase.QueryAsync<BonusReport>(tsql,
                    null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);
                TimeSpan ts2 = DateTime.UtcNow.AddHours(8).Subtract(currentTime);

                var code = (result != null && result.Count > 0);
                if (code)
                {
                    apiResultModel.data.totalData = result.First();
                }
            }
            return apiResultModel;


        }


        /// <summary>
        /// 加载优惠或红利
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        public static async Task<object> LoadBonusById(long tid)
        {
            var models = await DapperBase.QueryAsync<QueryBonusOutModel>(BonusSqlCommandText.LoadBonusModelSql,
                new { aid = tid.ToString() }, commandTimeout: 15000, 
                ConnectionString: DapperStringBase.ConnectionStringReadOnly);
            bool code = (models != null && models.Count > 0);
            return new ApiResultModel<object>
            {
                code = code ? "true" : "false",
                message = code ? "优惠红利加载-成功" : "优惠红利加载-失败",
                message_code = code ? "bonus_promo_load_success" : "bonus_promo_load_failed",
                data = code ? models.First() : null
            };
        }
        /// <summary>
        /// 生成优惠或红利
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> CreateBonusAsync(CreatePromoInputModel inputModel, string token,
            string loginKey)
        {

            var userModel = await DapperBase.QueryAsync<OfUserSaler>(UserSqlCommandText.LoadUserDatailSalerSql, new { account = inputModel.account },
                    commandTimeout: 1500, ConnectionString: DapperStringBase.ConnectionString);
            var referenceguid = Common.GuidToLongString();
            var result = false;
            //货币限制
            var currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(token, loginKey);
            if (!currencys.Contains(userModel.First().currencyType)) {
                return new ApiResultModel<object>() {
                    code = "false",
                    message = "货币限制",
                    message_code = "error_currency_search"
                };
            }
            if(userModel.First().currencyType != inputModel.currencycode 
                || !userModel.First().merchantAccount.Equals(inputModel.merchantAccount, StringComparison.OrdinalIgnoreCase))
            {
                return ApiResultModel<object>.GetResult("false", "请选择正确的品牌+货币", "error merchant account or currency");
            }

            if(userModel.First().currencyType == 356 
                && inputModel.bonusPromoCode != 10171)
            {
                return ApiResultModel<object>.GetResult("false", "失败：请选择优惠项目（select promo item pls ?）", "user_promo_amount_limit_failed");
            }

            var autopromotype = -1;
            var platformType = -1;
            if (inputModel.bonusPromoCode == 10055 )
            {
                autopromotype = 2;
                platformType = 0;
            }
            else if (inputModel.bonusPromoCode == 1011110082)//体育幸运注单
            {
                inputModel.remarks = string.Format(@"App注册首存送18[{0}]", inputModel.remarks);
            }
            else if (inputModel.bonusPromoCode == 10006)//体育幸运注单
            {
                autopromotype = 999;
                inputModel.remarks = string.Format(@"体育幸运注单号[{0}]", inputModel.remarks);
            }
            else if(inputModel.bonusPromoCode == 1004200001 // VIP周存投注优惠
                || inputModel.bonusPromoCode == 1004200002)//线下  10%VIP周存送
            {
                autopromotype = 0;
                inputModel.remarks = string.Format(@"{0}",
                    inputModel.bonusPromoCode == 1004200001 ?
                    string.Format(@"VIP周存投注优惠[{0}]",inputModel.remarks) : string.Format(@"10%VIP周存送[{0}]",inputModel.remarks));


                var dateRanage = Common.GetDateRanage(33);//上周的时间区间
                var m = JsonConvert.DeserializeObject<DateRanage>(JsonConvert.SerializeObject(dateRanage.Item2));
                var endTime = m.endTimeS;

                //检测这周是否有参加过相斥的活动，如果有，直接返回失败
                var promoCodes = new List<long>() { 1004200001, 1004200002 };

                if(await PromoBaseBLL.CheckPromoByMultiPromoCode(account: inputModel.account, autopromotype:0, startTime: endTime, promoCodes: promoCodes))
                    return ApiResultModel<object>.GetResult("false", "失败：重复申请", "user_promo_amount_limit_failed");

                if(inputModel.bonusPromoCode == 1004200001)
                {
                    if(userModel.First().riskTypesAready != 0 
                        && userModel.First().riskTypesAready != 6)
                        return ApiResultModel<object>.GetResult("false", "失败：风控状态，不符合申请条件", "user_promo_amount_limit_failed");
                }

            }
            else if (inputModel.bonusPromoCode == 10104)//体育线下存款优惠，每月一次  
            {
                autopromotype = 0;
                inputModel.remarks = string.Format(@"体育线下存款优惠10%[{0}]", inputModel.remarks);
            }
            else if (inputModel.bonusPromoCode == 10042)//vip 周存优惠，每周一次  
            {
                autopromotype = 0;
            }
            else if (inputModel.bonusPromoCode == 10093)//vip 周存体育优惠，每周一次
            {
                autopromotype = 0;
                inputModel.remarks = string.Format(@"vip体育周存优惠[{0}]", inputModel.remarks);
            }
            else if (inputModel.bonusPromoCode == 10094)//vip 周存彩票棋牌优惠，每周一次  VIP彩票棋牌周存
            {
                autopromotype = 0;
                inputModel.remarks = string.Format(@"VIP彩票周存[{0}]", inputModel.remarks);
            }
            else if (inputModel.bonusPromoCode == 10056)
            {
                autopromotype = 2;
                platformType = 1;
            }
            else if (inputModel.bonusPromoCode == 10015 && inputModel.gametype == 138)//pt
            {
                autopromotype = 2;
                platformType = 6;
            }
            else if (inputModel.bonusPromoCode == 10015 && inputModel.gametype == 137)//ttg
            {
                autopromotype = 2;
                platformType = 7;
            }
            else if (inputModel.bonusPromoCode == 10015 && inputModel.gametype == 146)//pg
            {
                autopromotype = 2;
                platformType = 8;
            }
            else if (inputModel.bonusPromoCode == 10075)//AW存款彩金
            {
                autopromotype = 0;
                inputModel.remarks = string.Format(@"AW存款彩金[{0}]", inputModel.remarks);
                if(inputModel.reward > 3888)
                    return ApiResultModel<object>.GetResult("false", "失败：赠送金额超过上限金额[3888]", "user_promo_amount_limit_failed");
            }
            else if(inputModel.bonusPromoCode == 10171) {
                //100% Deposit Bonus'  外站 inr
                inputModel.remarks = string.Format(@"{0}", string.IsNullOrEmpty(inputModel.remarks)? "100% Deposit Bonus": inputModel.remarks);
            }

            if (userModel != null && userModel.Count == 1)
            {
                if (User.ControllUser(userModel.First().misc, 1))
                    return ApiResultModel<object>.GetResult("false", "当前账号禁止优惠", "user_promo_frobbiden_failed");
                var salerAccount = string.IsNullOrEmpty(userModel.First().saler) ? "awcsystem" : userModel.First().saler;
                var affaccount = string.IsNullOrEmpty(userModel.First().affaccount) ? "allwinctiy" : userModel.First().affaccount;

                result = await DapperBase.ExecuteTransctionAsync(PromoSqlCommandText.CreatePromoWithSalerSql_V,
                    new
                    {
                        currency = "CNY",
                        account = inputModel.account,
                        amount = inputModel.amount,
                        reward = inputModel.reward,
                        remarks = inputModel.remarks,
                        bonusPromoCode = inputModel.bonusPromoCode,
                        level = userModel.First().level,
                        bet = inputModel.bet,
                        createperson = inputModel.adminAccount,
                        gametype = inputModel.gametype,
                        gamePercentType = inputModel.gamePercentType,
                        bonusPromoType = inputModel.bonusPromoType,
                        referrerreg = userModel.First().referrerreg,
                        referenceguid = referenceguid,
                        ipaddress = inputModel.IpAddress,
                        macaddress = inputModel.MacAddress,
                        autopromotype= autopromotype,
                        platformType = platformType,
                        accountType = 0,
                        saler = salerAccount,
                        aaffaccount = affaccount,
                        p_f_uid = userModel.First().id
                    }, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

            }
            return new ApiResultModel<object>
            {
                code = result ? "true" : "false",
                message = result ? "优惠红利创建-成功" : "优惠红利创建-失败",
                message_code = result ? "bonus_promo_create_success" : "bonus_promo_create_failed"
            };
        }
       
        /// <summary>
        /// 批量生成优惠或红利
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> CreateBonusBatAsync(CreatePromoInputModel inputModel)
        {
            var models = CalcBonusData(inputModel.content);
            if(models.Item1)//输入数据格式正确
            {

                #region 获取所有几传入的所有账号  -----  这里有点重
                var whereSql = string.Empty;
                foreach (var m in models.Item2)
                {
                    if (string.IsNullOrEmpty(whereSql))
                        whereSql = string.Format(@"'{0}'", m.Key);
                    else
                        whereSql = string.Format(@"{0},'{1}'", whereSql, m.Key);
                }
                if (!string.IsNullOrEmpty(whereSql))
                    whereSql = string.Format(@"({0})", whereSql);
                #endregion

                var LoadUserDatailBatSql = string.Format(UserSqlCommandText.LoadUserDatailBatSalerSql, whereSql);

                var userBonusModels = await DapperBase.QueryAsync<OfUserSaler>(LoadUserDatailBatSql, null,
                    commandTimeout: 1500, ConnectionString: DapperStringBase.ConnectionString);

                foreach(var u in userBonusModels)
                {
                    if (u.currencyType != inputModel.currencycode
                        || !u.merchantAccount.Equals(inputModel.merchantAccount, StringComparison.OrdinalIgnoreCase))
                    {
                        return ApiResultModel<object>.GetResult("false", "请选择正确的品牌+货币", "error merchant account or currency");
                    }
                }

                var errorAccounts = string.Empty;//不存在的账号列表
                var controllUsers = string.Empty;//被风控的账号
                if (userBonusModels.Count != models.Item2.Count)
                {
                    //不存在的账号
                    var query = models.Item2.Where(p => !userBonusModels.Any(g => p.Key.ToLower() == g.account.ToLower()));
                    foreach (var item in query)
                    {
                        errorAccounts = string.IsNullOrEmpty(errorAccounts) ? item.Key : string.Format(@"{0},{1}", errorAccounts, item.Key);
                    }
                }
                var insertModels = new List<object>();
                //对合法账号加红利或优惠
                foreach (var m in userBonusModels)
                {
                    var account = m.account.ToLower();
                    if(User.ControllUser(m.misc,1)) //被风控的账号
                    {
                        controllUsers = string.IsNullOrEmpty(controllUsers) ? account : string.Format(@"{0},{1}", controllUsers, account);
                        continue;
                    }
                    if (!models.Item2.ContainsKey(account) || models.Item2[account] == null)
                        continue;
                    var affaccount = string.IsNullOrEmpty(m.affaccount) ? "allwincity" : m.affaccount;
                    var saler = string.IsNullOrEmpty(m.saler) ? "awcsystem" : m.saler;
                    var item = models.Item2[account];
                    insertModels.Add(new
                    {
                        currency = "CNY",
                        account = account,
                        amount = item.amount,
                        reward = item.reward,
                        remarks = inputModel.remarks,
                        bonusPromoCode = inputModel.bonusPromoCode,
                        level = m.level,
                        bet = item.bets,
                        createperson = inputModel.adminAccount,
                        gametype = inputModel.gametype,
                        gamePercentType = inputModel.gamePercentType,
                        bonusPromoType = inputModel.bonusPromoType,
                        referrerreg = m.referrerreg,
                        referenceguid = Common.GuidToLongString(),
                        ipaddress = inputModel.IpAddress,
                        macaddress = inputModel.MacAddress,
                        autopromotype = 0,//系统默认
                        platformType = -1,//系统默认
                        accountType = 0, //系统默认
                        saler = saler,
                        aaffaccount = affaccount,
                        p_f_uid = m.id
                    });

                }


                var insertResult = await DapperBase.ExecuteTransctionMultipleAsync(PromoSqlCommandText.CreatePromoWithSalerSql_V, 
                    insertModels, commandTimeout: 15000,
                    ConnectionString: DapperStringBase.ConnectionString);

                return new ApiResultModel<turnoverResult>()
                {
                    code = insertResult ? "true" : "false",
                    message = "批量添加红利或优惠结束,当前新增:" + insertModels.Count + "个记录",
                    message_code = "calc_bonus_promo_finished",
                    data = new turnoverResult { errorAccounts = errorAccounts,controllUsers= controllUsers }
                };
            }
            else
            {
                return new ApiResultModel<object>()
                {
                    code = "false",
                    message = models.Item3,
                    message_code = "calc_bonus_bat_failed",

                };

            }

        }

        /// <summary>
        /// 编辑红利和优惠
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> UpdateBonusAsync(UpdatePromoInputModel inputModel)
        {
            var userModel = await DapperBase.QueryAsync<ofUser>(UserSqlCommandText.LoadUserDatailSql, new { account = inputModel.account },
                    commandTimeout: 1500, ConnectionString: DapperStringBase.ConnectionString);

            if (userModel.First().currencyType != inputModel.currencycode
               || !userModel.First().merchantAccount.Equals(inputModel.merchantAccount, StringComparison.OrdinalIgnoreCase))
            {
                return ApiResultModel<object>.GetResult("false", "请选择正确的品牌+货币", "error merchant account or currency");
            }


            var autopromotype = -1;
            var platformType = -1;
            if (inputModel.bonusPromoCode == 10055)
            {
                autopromotype = 2;
                platformType = 0;
            }
            else if (inputModel.bonusPromoCode == 10042)
            {
                autopromotype = 0;
            }
            else if (inputModel.bonusPromoCode == 1011110059)//群英回归优惠
            {
                autopromotype = 0;
            }
            else if (inputModel.bonusPromoCode == 10104)//体育线下存款优惠，每月一次  
            {
                autopromotype = 0;
                inputModel.remarks = string.Format(@"体育线下存款优惠10%[{0}]", inputModel.remarks);
            }
            else if (inputModel.bonusPromoCode == 10056)
            {
                autopromotype = 2;
                platformType = 1;
            }
            else if (inputModel.bonusPromoCode == 10015 && inputModel.gametype == 138)//pt
            {
                autopromotype = 2;
                platformType = 6;
            }
            else if (inputModel.bonusPromoCode == 10015 && inputModel.gametype == 137)//ttg
            {
                autopromotype = 2;
                platformType = 7;
            }
            else if (inputModel.bonusPromoCode == 10015 && inputModel.gametype == 146)//pg
            {
                autopromotype = 2;
                platformType = 8;
            }
            else if (inputModel.bonusPromoCode == 10006)//体育幸运注单
            {
                autopromotype = 999;
            }

            var result = await DapperBase.ExecuteTransctionAsync(PromoSqlCommandText.UpdatePromoNewSql,
                    new
                    {
                        tid = inputModel.tid,
                        account = inputModel.account,
                        amount = inputModel.amount,
                        reward = inputModel.reward,
                        remarks = inputModel.remarks,
                        level = userModel.First().level,
                        bet = inputModel.bet,
                        createperson = inputModel.adminAccount,
                        gametype = inputModel.gametype,
                        gamePercentType = inputModel.gamePercentType,
                        bonusPromoType = inputModel.bonusPromoType,
                        referrerreg = userModel.First().referrerreg,
                        ipaddress = inputModel.IpAddress,
                        macaddress = inputModel.MacAddress,
                        autopromotype = autopromotype,
                        platformType = platformType
                    }, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString); ;

            return new ApiResultModel<object>
            {
                code = result ? "true" : "false",
                message = result ? "优惠红利编辑-成功" : "优惠红利编辑-失败",
                message_code = result ? "bonus_promo_modify_success" : "bonus_promo_modify_failed"
            };
        }

        /// <summary>
        /// 红利优惠审核
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> VerifyBonusAsync(VerifyBonusInputModel inputModel, string postgreSql="")
        {
            var model = await GetOrderModel(inputModel.tid);
            if (!model.Item1)
                return model.Item3;
            var currentDate = DateTime.UtcNow.AddHours(8);
            var dd =  Convert.ToInt32(model.Item2.createtime.ToString("yyyyMMdd"));
            var MM = Convert.ToInt32(model.Item2.createtime.ToString("yyyyMM"));
            var pomoModel = model.Item2;
            var defModel = MerchantCodes.GetDefaultModel(pomoModel.merchantAccount, pomoModel.currencyType);
            var sqlArray = new List<string>();
            var param = new List<object>();
            //理新当红利优惠状态
            sqlArray.Add(BonusSqlCommandText.UpdateBonusStatusSql_with_Datetime);
            param.Add(new { status = 1, id = pomoModel.id, reviewperson = inputModel.adminAccount, 
                reviewtime=currentDate.ToString("yyyy-MM-dd HH:mm:ss"), reason= inputModel.cancelreason });
            //账变申请 
            var preBalance = string.Format(UserSqlCommandText.preBalance, pomoModel.account);
            var afterBalance = string.Format(UserSqlCommandText.afterBalanceA, pomoModel.reward, pomoModel.account);
            var CreatePaymentSql = string.Format(PaymentSqlCommandText.CreatePaymentSql, preBalance, afterBalance);
            sqlArray.Add(CreatePaymentSql);
            param.Add(new
            {
                currency = defModel.currency, // "CNY",
                account = pomoModel.account,
                transactiontype = 1,
                summary = pomoModel.remarks,
                referenceguid = pomoModel.referenceguid,
                bonus = pomoModel.reward,
                status = 1,
                summaryType = pomoModel.bonusPromoType == 0 ? 2 : 3
            }); ;
            //更新主账号余额
            sqlArray.Add(UserSqlCommandText.UpdateBalanceASql);
            param.Add(new { account = pomoModel.account, amount = pomoModel.reward });

            #region 记录充值账号时余额结点 如果我余额<=10才做记录 2024-11-28
            if (!defModel.merchantAccount.Equals("Awc", StringComparison.OrdinalIgnoreCase))
            {
                var tpSqls = await AmoutRecord.ConfirmBalanceAsync(pomoModel.account, defModel.merchantAccount, defModel.currencyType,
                    pomoModel.referenceguid, r_type: 1, currentDate: currentDate.ToString("yyyy-MM-dd HH:mm:ss"), remark: "后台红利优惠到账");
                if (!string.IsNullOrEmpty(tpSqls.Item1))
                {
                    sqlArray.Add(tpSqls.Item1);
                    param.Add(tpSqls.Item2);
                }
            }

            #endregion
            var result = await DapperBase.ExecuteTransctionAsync(sqlArray, param,
                 commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

            return new ApiResultModel<object>
            {
                code = result ? "true" : "false",
                message = result ? "审核优惠红利-成功" : "审核优惠红利-失败",
                message_code = result ? "verify_bonus_promo_success" : "verify_bonus_promo_failed"
            };

        }
        /// <summary>
        /// 红利优惠取消审核
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> CancelBonusAsync(VerifyBonusInputModel inputModel)
        {
            //优惠红利定单状态取消
            var model = await GetOrderModel(inputModel.tid);
            if (!model.Item1)
                return model.Item3;
            //理新当红利优惠状态
            var result = await DapperBase.ExecuteTransctionAsync(BonusSqlCommandText.UpdateBonusStatusSql,
                  new { status = 2, id = model.Item2.id, reviewperson = inputModel.adminAccount, reason=inputModel.cancelreason }, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            return new ApiResultModel<object>()
            {
                code = result ? "true" : "false",
                message = result ? "取消优惠红利成功" : "取消优惠红利失败",
                message_code = result ? "cancel_bonus_promo_success" : "cancel_bonus_promo_failed"
            };
        }

        #region 福利中心


        /// <summary>
        /// 福利中心 红利优惠审核
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> VerifySelfBonusAsync(VerifyBonusInputModel inputModel)
        {
            var model = await GetSelfOrderModel(inputModel.tid);
            if (!model.Item1)
                return model.Item3;
            
            var sqlArray = new List<string>();
            var param = new List<object>();
            //理新当红利优惠状态
            sqlArray.Add(BonusSqlCommandText.VerifySelfBonusStatusSql);
            param.Add(new { status = 0, id = model.Item2.id, reviewperson = inputModel.adminAccount });
            
            var result = await DapperBase.ExecuteTransctionAsync(sqlArray, param,
                 commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

            return new ApiResultModel<object>
            {
                code = result ? "true" : "false",
                message = result ? "审核成功,等待会员领取..." : "审核优惠红利-失败",
                message_code = result ? "verify_bonus_promo_success" : "verify_bonus_promo_failed"
            };

        }

        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> VerifySelfBonusBatAsync(VerifyBonusBatInputModel inputModel)
        {
            var tids = GetTids(inputModel.tids);
            if (tids.Item1)
            {
                var sqlArray = new List<string>();
                var param = new List<object>();
                foreach (var m in tids.Item2)
                {
                    //理新当红利优惠状态
                    var updateSql = string.Format(BonusSqlCommandText.VerifySelfBonusStatusSqlV, inputModel.adminAccount, m);
                    sqlArray.Add(updateSql);
                    param.Add(null);
                }
                var result = await DapperBase.ExecuteTransctionAsync(sqlArray, param,
                     commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                return new ApiResultModel<object>
                {
                    code = result ? "true" : "false",
                    message = result ? "审核成功,等待会员领取..." : "审核优惠红利-失败",
                    message_code = result ? "verify_bonus_promo_success" : "verify_bonus_promo_failed"
                };
            }
            else
            {
                return new ApiResultModel<object>()
                {
                    code = "false",
                    message = "审核失败[ids]",
                    message_code = "verify_selfbonus_failed",

                };
            }


        }

        /// <summary>
        /// 根据选择的id集合
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private static Tuple<bool, List<long>> GetTids(string ids)
        {
            if (string.IsNullOrEmpty(ids))
                return new Tuple<bool, List<long>>(false, null);
            try
            {
                var idList = ids.Split(",");
                var list = new List<long>();
                foreach (var item in idList)
                    list.Add(Convert.ToInt64(item));
                return new Tuple<bool, List<long>>(true, list);

            }
            catch (Exception)
            {
                return new Tuple<bool, List<long>>(false, null);
            }

        }

        /// <summary>
        /// 福利中心 红利审核取消
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> CancelSelfBonusAsync(VerifyBonusInputModel inputModel)
        {
            //优惠红利定单状态取消
            var model = await GetSelfOrderModel(inputModel.tid);
            if (!model.Item1)
                return model.Item3;
            //理新当红利优惠状态
            var result = await DapperBase.ExecuteTransctionAsync(BonusSqlCommandText.CancelSelfBonusStatusSql,
                  new { status = 3, id = model.Item2.id, reviewperson = inputModel.adminAccount, reason = inputModel.cancelreason }, 
                  commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            return new ApiResultModel<object>()
            {
                code = result ? "true" : "false",
                message = result ? "取消优惠红利成功" : "取消优惠红利失败",
                message_code = result ? "cancel_bonus_promo_success" : "cancel_bonus_promo_failed"
            };
        }

        /// <summary>
        /// 福利中心 加载优惠或红利
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        public static async Task<object> LoadSelfBonusById(long tid)
        {
            var models = await DapperBase.QueryAsync<QuerySelfBonusOutModel>(BonusSqlCommandText.LoadSelfBonusModelSql,
                new { aid = tid.ToString() }, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionStringReadOnly);
            bool code = (models != null && models.Count > 0);
            return new ApiResultModel<object>
            {
                code = code ? "true" : "false",
                message = code ? "优惠红利加载-成功" : "优惠红利加载-失败",
                message_code = code ? "bonus_promo_load_success" : "bonus_promo_load_failed",
                data = code ? models.First() : null
            };
        }


        /// <summary>
        /// 福利中心，项目选择列表
        /// </summary>
        /// <returns></returns>
        public static async Task<object> SelfBonusList()
        {
            
            var models = await DapperBase.ExecuteScalarValueAsync(BonusSqlCommandText.SelfBonusList,
                null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);

            var kvs = JsonConvert.DeserializeObject<List<SelfBonusSelectModel>>(models.ToString());
            
            return new ApiResultModel<object>
            {
                code = "true" ,
                message = "优惠红利加载-成功",
                message_code ="bonus_promo_load_success" ,
                data = kvs
            };
        }


        /// <summary>
        /// 福利中心 记录查询
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> SelfBonusRecordsAsync(QueryBonusInputModel inputModel,
            string token,
            string loginKey)
        {
            string whereSql = string.Empty;
            var viewName = " ofBonusPromoSelf a with(nolock) ";
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


            if (!string.IsNullOrEmpty(inputModel.account))
            {
                whereSql = string.Format(@" a.account in {0} ", accounts);
            }

            

            if (!string.IsNullOrEmpty(inputModel.affaccount))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" u.affaccount in {0} ", affaccounts) 
                    : string.Format(@" {0} and u.affaccount in {1} ", whereSql, affaccounts);

            if (inputModel.status != -1)
            {
                var currentDate = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");
                //0:进行中,待领取，1：已领取，2：已失效
                if (inputModel.status == 0)
                {
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.createtime <= '{0}' and a.expiredTime >='{0}' and a.status=0 ", currentDate) 
                        : string.Format(@" {0} and a.createtime <= '{1}' and a.expiredTime >='{1}' and a.status=0 ", whereSql, currentDate);
                }
                else if (inputModel.status == 1)
                {
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.status = {0} ", inputModel.status)
                        : string.Format(@" {0} and a.status = {1} ", whereSql, inputModel.status);

                }
                else if (inputModel.status == 2)
                {
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.expiredTime < '{0}' and a.status=0 ", currentDate) 
                        : string.Format(@" {0} and a.expiredTime < '{1}' and a.status=0 ", whereSql, currentDate);
                }
                else if (inputModel.status == 3)//被取消
                {
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.status = {0} ", inputModel.status) 
                        : string.Format(@" {0} and a.status = {1} ", whereSql, inputModel.status);
                }
                else if (inputModel.status == 4)//等待审核
                {
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.status = {0} ", inputModel.status)
                        : string.Format(@" {0} and a.status = {1} ", whereSql, inputModel.status);
                }
            }

            if (inputModel.gametype != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.gametype = {0} ", inputModel.gametype) : string.Format(@" {0} and a.gametype = {1} ", whereSql, inputModel.gametype);

            if (inputModel.bonusPromoType != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.bonusPromoType = {0} ", inputModel.bonusPromoType) : string.Format(@" {0} and a.bonusPromoType = {1} ", whereSql, inputModel.bonusPromoType);

            if (string.IsNullOrEmpty(inputModel.startTime)
                && string.IsNullOrEmpty(inputModel.endTime))
            {
                var currentDay = DateTime.UtcNow.AddHours(8);
                var startTime = currentDay.ToString("yyyy-MM-01 00:00:00");
                var endTime = currentDay.ToString("yyyy-MM-dd 23:59:59");
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.createtime >= '{0}' and a.createtime <= '{1}'", startTime, endTime) : string.Format(@" {0} and a.createtime >= '{1}' and a.createtime <= '{2}'", whereSql, startTime, endTime);

            }

            if (!string.IsNullOrEmpty(inputModel.startTime)
                && !string.IsNullOrEmpty(inputModel.endTime))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.createtime >= '{0}' and a.createtime <= '{1}'",
                    inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and a.createtime >= '{1}' and a.createtime <= '{2}'", whereSql, inputModel.startTime, inputModel.endTime);
            }

            if (!string.IsNullOrEmpty(inputModel.remarks))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" remarks like '%{0}%' ", inputModel.remarks) : string.Format(@" {0} and remarks like '%{1}%' ", whereSql, inputModel.remarks);
            }

            if (inputModel.currencyType != -1) {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" u.currencyType = {0} ", inputModel.currencyType) : string.Format(@" {0} and u.currencyType = {1} ", whereSql, inputModel.currencyType);
            }

            whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.accountType = 0 ") : string.Format(@" {0} and a.accountType = 0 ", whereSql);
            
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

            var leftjoinString = string.Format(@" left join ofUser u with(nolock) on a.account = u.account  
left join ofUserExtention ue with(nolock) on a.account = ue.account
            left join ofUserGroup b with(nolock) on a.level = b.levelnumber ");
            var fields = " a.id as tid,a.account,a.amount,a.reward,a.remarks,a.createtime,a.expiredTime,a.bet,a.level,a.gametype,a.bonusPromoType,a.status,a.referenceguid,a.createperson,a.reviewperson,a.reviewtime,a.reason,a.ipaddress,a.bonusPromoCode,b.levels,(u.withdrawalamount-u.depositamount) Profit,u.id uid,u.anomaly,u.walletType,u.currencyType,u.merchantAccount,ue.riskRemark,ue.riskTypesAready ";

            var apiResultModel = await DapperPaging<QuerySelfBonusOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                    fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, DapperStringBase.ConnectionStringReadOnly);

            if (inputModel.currencyType == (int)LangCodes.CurrencyCode.INR
                || inputModel.currencyType == (int)LangCodes.CurrencyCode.USD) {
                if (apiResultModel.data.modelList.Count > 0) {
                    foreach (var m in apiResultModel.data.modelList) {
                        if (VipLevelINTL.levelIntlName.ContainsKey(m.level))
                            m.levels = VipLevelINTL.levelIntlName[m.level];
                    }
                }
            }
            if (apiResultModel.code == "true"
                && apiResultModel.data.TotalRecord > 0) //获取到分页数据,且有输入时间
            {
                var currentDate = DateTime.UtcNow.AddHours(8);

                foreach (var m in apiResultModel.data.modelList)
                {
                    if (m.status == 1)
                    {
                        m.statusDesc = "已领取";
                    }
                    else if (m.status == 3)
                    {
                        m.statusDesc = "被取消";
                    }
                    else if (m.status == 4)
                    {
                        m.statusDesc = "等待审核";
                    }
                    else
                    {
                        if (currentDate >= Convert.ToDateTime(m.createtime)
                            && currentDate <= Convert.ToDateTime(m.expiredTime)
                            && m.status == 0)
                        {
                            m.statusDesc = "未领取";
                            m.status = 0;
                        }
                        else
                        {
                            m.statusDesc = "过期";
                            m.status = 2;
                        }
                    }
                }

                if (inputModel.currencyType != -1
                    && !string.IsNullOrEmpty(inputModel.startTime)
                    && !string.IsNullOrEmpty(inputModel.endTime))//输入时间会有统计
                {
                    //获取统计数据： 

                    var tsql = string.Format(@"select sum(a.reward) totalReward
from ofBonusPromoSelf a
left join ofUserGroup b on a.level = b.levelnumber  
left join ofUser u on a.account = u.account
where {0} ", whereSql);

                    var result = await DapperBase.QueryAsync<BonusReport>(tsql,
                        null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);

                    var code = (result != null && result.Count > 0);
                    if (code)
                    {
                        apiResultModel.data.totalData = result.First();
                    }
                }


            }
            return apiResultModel;


        }

        /// <summary>
        /// 福利中心 批量生成优惠或红利
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> CreateSelfBonusBatAsync(CreateSelfPromoInputModel inputModel)
        {
            var models = CalcBonusData(inputModel.content);

            if (models.Item1)//输入数据格式正确
            {

                #region 获取所有几传入的所有账号  -----  这里有点重
                var whereSql = string.Empty;
                foreach (var m in models.Item2)
                {
                    if (string.IsNullOrEmpty(whereSql))
                        whereSql = string.Format(@"'{0}'", m.Key);
                    else
                        whereSql = string.Format(@"{0},'{1}'", whereSql, m.Key);
                }
                if (!string.IsNullOrEmpty(whereSql))
                    whereSql = string.Format(@"({0})", whereSql);
                #endregion

                #region 如果没有选择优惠列表，会被强制返回
                var selectModels = await DapperBase.ExecuteScalarValueAsync(BonusSqlCommandText.SelfBonusList,
                null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);
                var kvs = JsonConvert.DeserializeObject<List<SelfBonusSelectModel>>(selectModels.ToString());
                var selfBonusDict = new Dictionary<int, string>();
                //{
                //    { 1, "生日礼金" },
                //    { 2, "节日礼金" },
                //    { 3, "免费筹码"},
                //    { 4, "红包"},
                //    { 5, "电销礼金"}
                //};

                foreach (var kv in kvs)
                {
                    if(kv.value != 0)
                        selfBonusDict.Add(kv.value, kv.text);
                }

                if (!selfBonusDict.ContainsKey(inputModel.bonusPromoCode))
                    return ApiResultModel<object>.GetResult("false", "请选择优惠列表", "user_promo_frobbiden_failed");

                #endregion
                var remarks = string.IsNullOrEmpty(inputModel.remarks) ? "" : string.Format(@"[{0}]", inputModel.remarks);
                inputModel.remarks = string.Format(@"{0}{1}", 
                    selfBonusDict[inputModel.bonusPromoCode], remarks );


                var LoadUserDatailBatSql = string.Format(UserSqlCommandText.LoadUserDatailBatSalerSql, whereSql);

                var userBonusModels = await DapperBase.QueryAsync<OfUserSaler>(LoadUserDatailBatSql, null,
                    commandTimeout: 1500, ConnectionString: DapperStringBase.ConnectionString);

                var errorAccounts = string.Empty;//不存在的账号列表
                var controllUsers = string.Empty;//被风控的账号
                if (userBonusModels.Count != models.Item2.Count)
                {
                    //不存在的账号
                    var query = models.Item2.Where(p => !userBonusModels.Any(g => p.Key.ToLower() == g.account.ToLower()));
                    foreach (var item in query)
                    {
                        errorAccounts = string.IsNullOrEmpty(errorAccounts) ? item.Key : string.Format(@"{0},{1}", errorAccounts, item.Key);
                    }
                }
                var insertModels = new List<object>();
                //对合法账号加红利或优惠
                foreach (var m in userBonusModels)
                {
                    if (m.currencyType != inputModel.currencycode
                        || !m.merchantAccount.Equals(inputModel.merchantAccount, StringComparison.OrdinalIgnoreCase))
                    {
                        return ApiResultModel<object>.GetResult("false", "请选择正确的品牌+货币", "error merchant account or currency");
                    }

                    var account = m.account.ToLower();
                    if (User.ControllUser(m.misc, 1)) //被风控的账号
                    {
                        controllUsers = string.IsNullOrEmpty(controllUsers) ? account : string.Format(@"{0},{1}", controllUsers, account);
                        continue;
                    }
                    if (!models.Item2.ContainsKey(account) || models.Item2[account] == null)
                        continue;
                    var affaccount = string.IsNullOrEmpty(m.affaccount) ? "allwincity" : m.affaccount;
                    var saler = string.IsNullOrEmpty(m.saler) ? "awcsystem" : m.saler;
                    var item = models.Item2[account];
                    insertModels.Add(new
                    {
                        currency = "CNY",
                        account = account,
                        amount = item.amount,
                        reward = item.reward,
                        remarks = inputModel.remarks,
                        createtime = inputModel.startTime,
                        expiredTime = inputModel.endTime,
                        status = 4,//福利中心，刚添加状态为4，等审核确认，确认后，会员才可以领取
                        bonusPromoCode = inputModel.bonusPromoCode,
                        level = m.level,
                        bet = item.bets,
                        createperson = inputModel.adminAccount,
                        gametype = inputModel.gametype,
                        gamePercentType = inputModel.gamePercentType,
                        bonusPromoType = inputModel.bonusPromoType,
                        referrerreg = m.referrerreg,
                        referenceguid = Common.GuidToLongString(),
                        ipaddress = inputModel.IpAddress,
                        macaddress = inputModel.MacAddress,
                        autopromotype = 0,//系统默认
                        platformType = -1,//系统默认
                        accountType = 0, //系统默认
                        saler = saler,
                        aaffaccount = affaccount,
                        ps_f_uid = m.id
                    });

                }


                var insertResult = await DapperBase.ExecuteTransctionMultipleAsync(PromoSqlCommandText.CreateSelfPromoWithSalerSql_V,
                    insertModels, commandTimeout: 15000,
                    ConnectionString: DapperStringBase.ConnectionString);

                return new ApiResultModel<turnoverResult>()
                {
                    code = insertResult ? "true" : "false",
                    message = "批量添加红利或优惠结束,当前新增:" + insertModels.Count + "个记录",
                    message_code = "calc_bonus_promo_finished",
                    data = new turnoverResult { errorAccounts = errorAccounts, controllUsers = controllUsers }
                };
            }
            else
            {
                return new ApiResultModel<object>()
                {
                    code = "false",
                    message = models.Item3,
                    message_code = "calc_bonus_bat_failed",

                };

            }

        }


        /// <summary>
        /// 福利中心 生成优惠或红利
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> CreateSelfBonusAsync(CreateSelfPromoInputModel inputModel)
        {
            var userModel = await DapperBase.QueryAsync<OfUserSaler>(UserSqlCommandText.LoadUserDatailSalerSql, 
                new { account = inputModel.account }, commandTimeout: 1500, ConnectionString: DapperStringBase.ConnectionString);
            var referenceguid = Common.GuidToLongString();
            var result = false;

            var selectModels = await DapperBase.ExecuteScalarValueAsync(BonusSqlCommandText.SelfBonusList,
                 null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);
            var kvs = JsonConvert.DeserializeObject<List<SelfBonusSelectModel>>(selectModels.ToString());
            var selfBonusDict = new Dictionary<int, string>();
            //{
            //    { 1, "生日礼金" },
            //    { 2, "节日礼金" },
            //    { 3, "免费筹码"},
            //    { 4, "红包"},
            //    { 5, "电销礼金"}
            //};

            foreach (var kv in kvs)
            {
                if (kv.value != 0)
                    selfBonusDict.Add(kv.value, kv.text);
            }

            var autopromotype = -1;
            var platformType = -1;

            if(!selfBonusDict.ContainsKey(inputModel.bonusPromoCode))
                return ApiResultModel<object>.GetResult("false", "请选择优惠列表", "user_promo_frobbiden_failed");


            var remarks = string.IsNullOrEmpty(inputModel.remarks) ? "" : string.Format(@"[{0}]", inputModel.remarks);

            inputModel.remarks = string.Format(@"{0}{1}",
                selfBonusDict[inputModel.bonusPromoCode], remarks);

            if (userModel != null && userModel.Count == 1)
            {
                if (userModel.First().currencyType != inputModel.currencycode
                    || !userModel.First().merchantAccount.Equals(inputModel.merchantAccount, StringComparison.OrdinalIgnoreCase))
                {
                    return ApiResultModel<object>.GetResult("false", "请选择正确的品牌+货币", "error merchant account or currency");
                }


                if (User.ControllUser(userModel.First().misc, 1))
                    return ApiResultModel<object>.GetResult("false", "当前账号禁止优惠", "user_promo_frobbiden_failed");
                var salerAccount = string.IsNullOrEmpty(userModel.First().saler) ? "awcsystem" : userModel.First().saler;
                var affaccount = string.IsNullOrEmpty(userModel.First().affaccount) ? "allwinctiy" : userModel.First().affaccount;
                result = await DapperBase.ExecuteTransctionAsync(PromoSqlCommandText.CreateSelfPromoWithSalerSql_V,
                    new
                    {
                        currency = "CNY",
                        account = inputModel.account,
                        amount = inputModel.amount,
                        reward = inputModel.reward,
                        createtime = inputModel.startTime,
                        expiredTime = inputModel.endTime,
                        status = 4,//福利中心，刚添加状态为4，等审核确认，确认后，会员才可以领取
                        remarks = inputModel.remarks,
                        bonusPromoCode = inputModel.bonusPromoCode,
                        level = userModel.First().level,
                        bet = inputModel.bet,
                        createperson = inputModel.adminAccount,
                        gametype = inputModel.gametype,
                        gamePercentType = inputModel.gamePercentType,
                        bonusPromoType = inputModel.bonusPromoType,
                        referrerreg = userModel.First().referrerreg,
                        referenceguid = referenceguid,
                        ipaddress = inputModel.IpAddress,
                        macaddress = inputModel.MacAddress,
                        autopromotype = autopromotype,
                        platformType = platformType,
                        accountType = 0,
                        saler = salerAccount,
                        aaffaccount = affaccount,
                        ps_f_uid = userModel.First().id
                    }, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

            }
            return new ApiResultModel<object>
            {
                code = result ? "true" : "false",
                message = result ? "优惠红利创建-成功" : "优惠红利创建-失败",
                message_code = result ? "bonus_promo_create_success" : "bonus_promo_create_failed"
            };
        }

        /// <summary>
        /// 编辑红利和优惠
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> UpdateSelfBonusAsync(UpdateSelfPromoInputModel inputModel)
        {
            var userModel = await DapperBase.QueryAsync<ofUser>(UserSqlCommandText.LoadUserDatailSql, new { account = inputModel.account },
                    commandTimeout: 1500, ConnectionString: DapperStringBase.ConnectionString);
            var autopromotype = -1;
            var platformType = -1;

            if (userModel.First().currencyType != inputModel.currencycode
               || !userModel.First().merchantAccount.Equals(inputModel.merchantAccount, StringComparison.OrdinalIgnoreCase))
            {
                return ApiResultModel<object>.GetResult("false", "请选择正确的品牌+货币", "error merchant account or currency");
            }

            var result = await DapperBase.ExecuteTransctionAsync(PromoSqlCommandText.UpdateSelfPromoNewSql,
                    new
                    {
                        tid = inputModel.tid,
                        account = inputModel.account,
                        amount = inputModel.amount,
                        reward = inputModel.reward,
                        createtime = inputModel.startTime,
                        expiredTime = inputModel.endTime,
                        remarks = inputModel.remarks,
                        level = userModel.First().level,
                        bet = inputModel.bet,
                        createperson = inputModel.adminAccount,
                        gametype = inputModel.gametype,
                        gamePercentType = inputModel.gamePercentType,
                        bonusPromoType = inputModel.bonusPromoType,
                        referrerreg = userModel.First().referrerreg,
                        ipaddress = inputModel.IpAddress,
                        macaddress = inputModel.MacAddress,
                        autopromotype = autopromotype,
                        platformType = platformType
                    }, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString); ;

            return new ApiResultModel<object>
            {
                code = result ? "true" : "false",
                message = result ? "优惠红利编辑-成功" : "优惠红利编辑-失败",
                message_code = result ? "bonus_promo_modify_success" : "bonus_promo_modify_failed"
            };
        }


        /// <summary>
        /// 福利中心  记录核实并返回
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
        private async static Task<Tuple<bool, ofBonusPromo, ApiResultModel<object>>> GetSelfOrderModel(long aid)
        {
            var result = await DapperBase.QueryAsync<ofBonusPromo>(BonusSqlCommandText.LoadSelfBonusModelSql, new { aid = aid },
                 commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

            if (result != null && result.Count == 1)
            {
                if (User.ControllUser(result.First().misc, 1))
                    return new Tuple<bool, ofBonusPromo, ApiResultModel<object>>(true, result.First(), new ApiResultModel<object>
                    {
                        code = "false",
                        message = "当前账号禁止优惠",
                        message_code = "user_promo_frobbiden_failed"
                    });
                result.First().affaccount = string.IsNullOrEmpty(result.First().affaccount) ? "allwincity" : result.First().affaccount;
                return new Tuple<bool, ofBonusPromo, ApiResultModel<object>>(true, result.First(), null);

            }
            else
                return new Tuple<bool, ofBonusPromo, ApiResultModel<object>>(true, result.First(), new ApiResultModel<object>
                {
                    code = "false",
                    message = "核实优惠红利-失败",
                    message_code = "verify_bonus_failed"
                });
        }




        #endregion

        /// <summary>
        /// 核实并返回
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
        private async static Task<Tuple<bool, ofBonusPromo, ApiResultModel<object>>> GetOrderModel(int aid)
        {
            var result = await DapperBase.QueryAsync<ofBonusPromo>(BonusSqlCommandText.LoadBonusModelSql, new { aid = aid },
                 commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

            if (result != null && result.Count == 1 )
            {
                if (User.ControllUser(result.First().misc, 1))
                    return new Tuple<bool, ofBonusPromo, ApiResultModel<object>>(true, result.First(), new ApiResultModel<object>
                    {
                        code = "false",
                        message = "当前账号禁止优惠",
                        message_code = "user_promo_frobbiden_failed"
                    });
                result.First().affaccount = string.IsNullOrEmpty(result.First().affaccount) ? "allwincity" : result.First().affaccount;
                return new Tuple<bool, ofBonusPromo, ApiResultModel<object>>(true, result.First(), null);

            }
            else
                return new Tuple<bool, ofBonusPromo, ApiResultModel<object>>(true, result.First(), new ApiResultModel<object>
                {
                    code = "false",
                    message = "核实优惠红利-失败",
                    message_code = "verify_bonus_failed"
                });
        }


        /// <summary>
        /// 获取红利或优惠数据模型
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static Tuple<bool, Dictionary<string, BonusData>, string> CalcBonusData(string content)
        {
            try
            {
                var arrays = content.Split("#");
                for (int o = 0; o < arrays.Length; o++)
                    arrays[o] = arrays[o].Trim();

                if (arrays.Length <= 1 || (arrays.Length - 1) % 4 != 0)
                {
                    //数据格式有误
                    return new Tuple<bool, Dictionary<string, BonusData>, string>(false, null, "数据格式有误");
                }
                var models = new Dictionary<string, BonusData>();
                for (int i = 1; i < arrays.Length; i += 4)
                {
                    var account = arrays[i];
                    var amount = arrays[i + 1].Trim();
                    var reward = arrays[i + 2].Trim();
                    var bets = arrays[i + 3].Trim();
                    decimal amt, bet,rd;
                    if (!ExpressSqlInject.CharAndDigital(account, 4, 15) ||
                        !ExpressSqlInject.DecimalValidate(amount, out amt) ||
                        !ExpressSqlInject.DecimalValidate(bets, out bet) ||
                        !ExpressSqlInject.DecimalValidate(reward, out rd))
                        return new Tuple<bool, Dictionary<string, BonusData>, string>(false, null, "数据校验有误");
                    else
                    {
                        if(models.ContainsKey(account.ToLower()))
                            return new Tuple<bool, Dictionary<string, BonusData>, string>(false, null, string.Format(@"{0}重复",account));
                        else
                            models.Add(account.ToLower(), new BonusData { account = account.ToLower(), amount = amt, reward = rd, bets = bet });
                    }
                }
                return new Tuple<bool, Dictionary<string, BonusData>, string>(true, models, "");
            }
            catch (Exception)
            {
                return new Tuple<bool, Dictionary<string, BonusData>, string>(false, null,"导入数据异常");
            }

        }


        /// <summary>
        /// 获取pt返水数据模型
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static Tuple<bool, Dictionary<string, TurnOverData>> CalcAccountTurnOverData(string content)
        {
            try
            {
                var arrays = content.Split("#");
                for (int o = 0; o < arrays.Length; o++)
                    arrays[o] = arrays[o].Trim();

                if (arrays.Length <= 1 || (arrays.Length - 1) % 3 != 0)
                {
                    //数据格式有误
                    return new Tuple<bool, Dictionary<string, TurnOverData>>(false, null);
                }
                // pt 特别处理 pt 游戏只有 这边客人账号的pt account

                var whereSql = string.Empty;
                for (int i = 1; i < arrays.Length; i += 3)
                {
                    var pttaccount = arrays[i];
                    if (ExpressSqlInject.CharAndDigital(pttaccount, 4, 18))
                    {
                        if (string.IsNullOrEmpty(whereSql))
                            whereSql = string.Format(@"'{0}'", pttaccount);
                        else
                            whereSql = string.Format(@"{0},'{1}'", whereSql, pttaccount);
                    }
                }
                if (!string.IsNullOrEmpty(whereSql))
                    whereSql = string.Format(@"({0})", whereSql);
                //从读库取出当前加返水的人员信息
                var LoadUserTurnOverPercentSql = string.Format(PromoSqlCommandText.LoadPtUserTurnOverPercentByAccountSql, whereSql);

                var turnOverModels = DapperBase.Query<TurnOverModel>(LoadUserTurnOverPercentSql,
                      null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                Dictionary<string, string> ptUser = new Dictionary<string, string>();

                foreach (var b in turnOverModels)
                {

                    if (ptUser.ContainsKey(b.pttaccount.ToLower()))
                        ptUser.Remove(b.pttaccount.ToLower());
                    ptUser.Add(b.pttaccount.ToLower(), b.account);
                }

                var models = new Dictionary<string, TurnOverData>();
                for (int i = 1; i < arrays.Length; i += 3)
                {
                    var ant = arrays[i];
                    var account = ptUser[ant.ToLower()];
                    var effectbet = arrays[i + 1].Trim();
                    var winlost = arrays[i + 2].Trim();
                    decimal eb, wl;
                    if (!ExpressSqlInject.CharAndDigital(account, 4, 18) ||
                        !ExpressSqlInject.DecimalValidate(effectbet, out eb) ||
                        !ExpressSqlInject.DecimalValidate(winlost, out wl))
                        return new Tuple<bool, Dictionary<string, TurnOverData>>(false, null);
                    else
                        models.Add(account.ToLower(), new TurnOverData { account = account.ToLower(), effectivebets = eb, winlost = wl });
                }

                return new Tuple<bool, Dictionary<string, TurnOverData>>(true, models);
            }
            catch (Exception)
            {
                return new Tuple<bool, Dictionary<string, TurnOverData>>(false, null);
            }

        }
    }

    
}
