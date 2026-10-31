using DBLibs.WXDapper;
using ShareEntity.Models;
using ShareEntity.SqlCommandText;
using ShareEntity.webSvcSqlCommandText;
using ShareSql.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.Merchant;

namespace PromoShare
{
    public class InsertPromo
    {
        /// <summary>
        /// 生成红利记录
        /// 1: 标记当前客人是分配到哪位电销服务
        /// 2: 20210329  设备id添加
        /// 3: 20241206 更新账号金额结点，如果金额 小或等10 (外币+加Boss品牌)
        /// 3: 20250822 更新账号金额结点，如果金额 小或等10 (Boss品牌)
        /// </summary>
        /// <param name="account"></param>
        /// <param name="amount"></param>
        /// <param name="reward"></param>
        /// <param name="remarks"></param>
        /// <param name="promoCode"></param>
        /// <param name="autopromotype"></param>
        /// <param name="betTimes"></param>
        /// <param name="ipaddress"></param>
        /// <param name="platformType"></param>
        /// <param name="gametype"></param>
        /// <param name="rguid">外部传入guid</param>
        /// <param name="deviceId"></param>
        /// <param name="createTime">生成优惠时间</param>
        /// <param name="isAutoPromo"></param>
        /// <returns></returns>
        public static async Task<Tuple<bool, string>> CreateBonusAndVerifyAsync(string account, decimal amount,
            decimal reward, string remarks, long promoCode,
            int autopromotype, decimal betTimes, string ipaddress, int platformType = -1,
            int gametype = 100, string rguid = "", int deviceId = 0, string createTime = "", int isAutoPromo = 0)
        {
            try
            {
                var userModels = await DapperBase.QueryAsync<OfUserSaler>(UserSqlCommandText.LoadUserDatailSalerSql,
                    new { account = account }, commandTimeout: 1500, ConnectionString: DapperStringBase.ConnectionString);

                if (userModels != null && userModels.Count == 1)
                {
                    var sqlArray = new List<string>();
                    var param = new List<object>();
                    var um = userModels.First();
                    var defModel = MerchantCodes.GetDefaultModel(um.merchantAccount, um.currencyType);

                    var referenceguid = string.IsNullOrEmpty(rguid) ? Common.GuidToLongString() : rguid;

                    var affaccountTemp = um.currencyType == 840 ? "winzones"
                        : um.currencyType == 156 ? "allwincity"
                        : um.currencyType == 356 ? "omg356" : "";

                    var currency = um.currencyType == 840 ? "USD"
                        : um.currencyType == 156 ? "CNY"
                        : um.currencyType == 356 ? "INR" : "";
                    if (currency == "")
                        return new Tuple<bool, string>(false, "error currency");

                    var salerAccount = string.IsNullOrEmpty(um.saler) ? "awcsystem" : um.saler;
                    var affaccount = string.IsNullOrEmpty(um.affaccount) ? affaccountTemp : um.affaccount;
                    var reviewtime = string.IsNullOrEmpty(createTime) ? DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss") : createTime;
                    //更新主账号余额
                    sqlArray.Add(UserSqlCommandText.UpdateBalanceASql);
                    param.Add(new { account = account, amount = reward });


                    //设置优惠和设置状态
                    sqlArray.Add(PromoSqlCommandText.CreatePromoAndUpdateWithSalerWithReviewTimeSql_V);
                    param.Add(new
                    {
                        currency = defModel.currency,
                        account = account,
                        amount = amount,
                        reward = reward,
                        remarks = remarks,
                        bonusPromoCode = promoCode,
                        autopromotype = autopromotype,
                        platformType = platformType,
                        level = um.level,
                        bet = betTimes,
                        createperson = account,//抽奖人账号
                        gametype = gametype,
                        gamePercentType = 5,//其它
                        bonusPromoType = 0,//红利  ，1 优惠
                        referrerreg = um.referrerreg,
                        referenceguid = referenceguid,
                        ipaddress = ipaddress,
                        macaddress = "",
                        status = 1,
                        createTime = reviewtime,
                        reviewtime = reviewtime,
                        saler = salerAccount,
                        aaffaccount = affaccount,
                        device_id = deviceId, //设备id
                        isAutoPromo = isAutoPromo,
                        p_f_uid = um.id
                    });

                    #region 记录充值账号时余额结点 如果我余额<=10才做记录 2024-11-28
                    if (!defModel.merchantAccount.Equals("Awc", StringComparison.OrdinalIgnoreCase))
                    {
                        var tpSqls = await AmoutRecord.ConfirmBalanceAsync(account, defModel.merchantAccount, defModel.currencyType,
                            referenceguid: referenceguid, r_type: 1, currentDate: reviewtime, remark: $"前端红利优惠到账,remarks:{remarks}");
                        if (!string.IsNullOrEmpty(tpSqls.Item1))
                        {
                            sqlArray.Add(tpSqls.Item1);
                            param.Add(tpSqls.Item2);
                        }
                    }

                    #endregion

                    var result = await DapperBase.ExecuteTransctionAsync(sqlArray, param,
                           commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

                    return new Tuple<bool, string>(result, result ? "优惠申请-成功" : "优惠申请-失败");

                }
                else
                {
                    return new Tuple<bool, string>(false, "核实账号信息-失败");
                }
            }
            catch (Exception)
            {
                return new Tuple<bool, string>(false, "优惠申请-失败e");
            }

        }


        /// <summary>
        /// 生成红利记录，并给客人加点 合营给下级会员发放优惠
        /// 1: 标记当前客人是分配到哪位电销服务
        /// 2: 20210329  设备id添加
        /// 3: 20241206 更新账号金额结点，如果金额 小或等10 (外币+加Boss品牌)
        /// 3: 20250822 更新账号金额结点，如果金额 小或等10 (Boss品牌)
        /// </summary>
        /// <param name="account"></param>
        /// <param name="amount"></param>
        /// <param name="reward"></param>
        /// <param name="remarks"></param>
        /// <param name="promoCode"></param>
        /// <param name="autopromotype"></param>
        /// <param name="betTimes"></param>
        /// <param name="ipaddress"></param>
        /// <param name="platformType"></param>
        /// <param name="gametype"></param>
        /// <param name="rguid">外部传入guid</param>
        /// <param name="deviceId"></param>
        /// <param name="createTime">生成优惠时间</param>
        /// <param name="isAutoPromo"></param>
        /// <returns></returns>
        public static async Task<Tuple<bool, string>> AgentCreateBonusAndVerifyAsync(string account, decimal amount,
            decimal reward, string remarks, long promoCode,
            int autopromotype, decimal betTimes, string ipaddress, int platformType = -1,
            int gametype = 100, string rguid = "", int deviceId = 0, string createTime = "", int isAutoPromo = 0)
        {
            try
            {
                var userModels = await DapperBase.QueryAsync<OfUserSaler>(UserSqlCommandText.LoadUserDatailSalerSql,
                    new { account = account }, commandTimeout: 1500, ConnectionString: DapperStringBase.ConnectionString);

                if (userModels != null && userModels.Count == 1)
                {
                    var sqlArray = new List<string>();
                    var param = new List<object>();
                    var um = userModels.First();
                    var defModel = MerchantCodes.GetDefaultModel(um.merchantAccount, um.currencyType);

                    var referenceguid = string.IsNullOrEmpty(rguid) ? Common.GuidToLongString() : rguid;

                    var affaccountTemp = um.currencyType == 840 ? "winzones"
                        : um.currencyType == 156 ? "allwincity"
                        : um.currencyType == 356 ? "omg356" : "";

                    var currency = um.currencyType == 840 ? "USD"
                        : um.currencyType == 156 ? "CNY"
                        : um.currencyType == 356 ? "INR" : "";
                    if (currency == "")
                        return new Tuple<bool, string>(false, "error currency");

                    var salerAccount = string.IsNullOrEmpty(um.saler) ? "awcsystem" : um.saler;
                    var affaccount =  um.affaccount;
                    var reviewtime = string.IsNullOrEmpty(createTime) ? DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss") : createTime;
                    //更新主账号余额
                    sqlArray.Add(UserSqlCommandText.UpdateBalanceASql);
                    param.Add(new { account = account, amount = reward });


                    //设置优惠和设置状态
                    sqlArray.Add(PromoSqlCommandText.CreatePromoAndUpdateWithSalerWithReviewTimeSql_V);
                    param.Add(new
                    {
                        currency = defModel.currency,
                        account = account,
                        amount = amount,
                        reward = reward,
                        remarks = remarks,
                        bonusPromoCode = promoCode,
                        autopromotype = autopromotype,
                        platformType = platformType,
                        level = um.level,
                        bet = betTimes,
                        createperson = affaccount,//上级账号
                        gametype = gametype,
                        gamePercentType = 5,//其它
                        bonusPromoType = 0,//红利  ，1 优惠
                        referrerreg = um.referrerreg,
                        referenceguid = referenceguid,
                        ipaddress = ipaddress,
                        macaddress = "",
                        status = 1,
                        createTime = reviewtime,
                        reviewtime = reviewtime,
                        saler = salerAccount,
                        aaffaccount = affaccount,
                        device_id = deviceId, //设备id
                        isAutoPromo = isAutoPromo,
                        p_f_uid = um.id
                    });

                    #region 记录充值账号时余额结点 如果我余额<=10才做记录 2024-11-28
                    if (!defModel.merchantAccount.Equals("Awc", StringComparison.OrdinalIgnoreCase))
                    {
                        var tpSqls = await AmoutRecord.ConfirmBalanceAsync(account, defModel.merchantAccount, defModel.currencyType,
                            referenceguid: referenceguid, r_type: 1, currentDate: reviewtime, remark: $"前端红利优惠到账,remarks:{remarks}");
                        if (!string.IsNullOrEmpty(tpSqls.Item1))
                        {
                            sqlArray.Add(tpSqls.Item1);
                            param.Add(tpSqls.Item2);
                        }
                    }

                    #endregion

                    #region 代理账变生成
                    var guid = Common.GuidToLongString();
                    var summary = string.Format(@"发放优惠扣除[{0}]", account);
                    var amountChange = AgentCommandSql.AmountChange(account: affaccount, summary: summary,
                        reward: (-reward).ToString(), transactiontype: 0, summaryType: 1007, guid, currency: defModel.currency);
                    if (amountChange.Item1)
                    {
                        sqlArray.Add(amountChange.Item2);
                        param.Add(amountChange.Item3);
                    }
                    else
                        return new Tuple<bool, string>(false, "扣款失败");
                    #endregion

                    #region 更新代理钱包之前况
                    var beforeSql = string.Format(@"update ofPaymentRecord
set balancePre=(select balance from ofAffiliate where account='{0}'),subTotalAmountPre=(select subTotalAmount from ofAffiliate where account='{0}')
where referenceguid='{1}' ", affaccount, guid);

                    sqlArray.Add(beforeSql);
                    param.Add(null);
                    #endregion

                    #region 更新代理 可用金额
                    sqlArray.Add(wsAgentSqlCommandText.UpdateAgentAmountSql);
                    param.Add(new { amount = reward, account = affaccount });
                    #endregion

                    #region 更新代理钱包之后状况
                    var afterSql = string.Format(@"update ofPaymentRecord
set balanceAfter=(select balance from ofAffiliate where account='{0}'),subTotalAmountAfter=(select subTotalAmount from ofAffiliate where account='{0}'), unSettleAmountAfter=(select unSettleAmount from ofAffiliate where account='{0}')
where referenceguid='{1}' ", affaccount, guid);

                    sqlArray.Add(afterSql);
                    param.Add(null);
                    #endregion

                    var result = await DapperBase.ExecuteTransctionAsync(sqlArray, param,
                           commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

                    return new Tuple<bool, string>(result, result ? "优惠申请-成功" : "优惠申请-失败");

                }
                else
                {
                    return new Tuple<bool, string>(false, "核实账号信息-失败");
                }
            }
            catch (Exception)
            {
                return new Tuple<bool, string>(false, "优惠申请-失败e");
            }

        }

    }
}
