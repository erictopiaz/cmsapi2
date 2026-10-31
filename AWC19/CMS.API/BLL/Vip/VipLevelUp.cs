using CMS.API.BLL.Promo;
using ShareEntity.InputModels.Vip;
using ShareEntity.SqlCommandText;
using DBLibs;
using DBLibs.WXDapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;
using ShareEntity.Vip;

namespace CMS.API.BLL.Vip
{
    public class VipLevelUp
    {
        /// <summary>
        /// 级别关系升级奖金
        /// 88	188	288	588	888	1888	5888
        /// </summary>
        private static readonly Dictionary<int, decimal> upLevelRewardKV = new Dictionary<int, decimal>()
        {
            { 10027, 88 },
            { 10028, 188  },
            { 10002, 288  },
            { 10003, 588  },
            { 10004, 888  },
            { 10025, 1888  }
        };
        private static readonly Dictionary<int, decimal> upLevelRewards = new Dictionary<int, decimal>()
        {
            { 1, 88 },
            { 2, 188  },
            { 3, 288  },
            { 4, 588  },
            { 5, 888  },
            { 6, 1888  }
        };
        /// <summary>
        /// 会员等级排序
        /// </summary>
        private static Dictionary<int, int> levelSort = new Dictionary<int, int>()
        {
            { 10027, 1 },
            { 10028, 2 },
            { 10002, 3 },
            { 10003, 4 },
            { 10004, 5 },
            { 10025, 6 }
        };
        /// <summary>
        /// 排序会员等级
        /// </summary>
        private static Dictionary<int, int> sortLevel = new Dictionary<int, int>()
        {
            { 1,10027 },
            { 2,10028 },
            { 3,10002 },
            { 4,10003 },
            { 5,10004 },
            { 6,10025 }
        };
        /// <summary>
        /// 会员等级名
        /// </summary>
        private static Dictionary<int, string> levelName = new Dictionary<int, string>()
        {
            { 10007, "普通会员" },
            { 10001, "万象会员" },
            { 10027, "白银VIP" },
            { 10028, "白玉VIP" },
            { 10002, "黄金VIP" },
            { 10003, "铂金VIP" },
            { 10004, "钻石VIP" },
            { 10025, "至尊VIP" }
        };

        #region vip 升级处理
        /// <summary>
        /// 生成优惠或红利
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> VipLevelMonthRecords(QueryVipLevelInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = " lastMonthVipData a ";
            string orderBy = " a.month desc";
            string keyName = " a.month,a.account ";


            if (!string.IsNullOrEmpty(inputModel.account))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.account = '{0}' ", inputModel.account) : string.Format(@" {0} and a.account = '{1}' ", whereSql, inputModel.account);
            }
            if (!string.IsNullOrEmpty(inputModel.affaccount))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" b.affaccount = '{0}' ", inputModel.affaccount) : string.Format(@" {0} and b.affaccount = '{1}' ", whereSql, inputModel.affaccount);

            if (inputModel.status != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.status = {0} ", inputModel.status) : string.Format(@" {0} and a.status = {1} ", whereSql, inputModel.status);

            if (inputModel.is_up != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.is_up = {0} ", inputModel.is_up) : string.Format(@" {0} and a.is_up = {1} ", whereSql, inputModel.is_up);

            if (inputModel.vipLevel != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.nextLevel = {0} ", inputModel.vipLevel) : string.Format(@" {0} and a.nextLevel = {1} ", whereSql, inputModel.vipLevel);


            if (inputModel.startTime>-1
                && inputModel.endTime>-1)
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" month >= {0} and month <= {1} ", inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and month >= {1} and month <= {2} ", whereSql, inputModel.startTime, inputModel.endTime);
            }


            var leftjoinString = string.Format(@" left join ofUser b on a.account = b.account ");
            leftjoinString += string.Format(@" left join ofUserExtention d on a.account = d.account ");
            var fields = " d.riskTypesAready,a.*,b.id uid,b.affaccount,b.freezereason freezereason,iif(b.anomaly is not null,b.anomaly,0) anomaly ";

            return await DapperPaging<VipLevelUpOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex);
        }

        /// <summary>
        /// 获取越级升级奖金
        /// </summary>
        /// <param name="startSortId"></param>
        /// <param name="endSortId"></param>
        /// <returns></returns>
        private static decimal QueryTotalReward(int startSortId, int endSortId)
        {
            var reward = 0m;
            foreach (var r in upLevelRewards)
            {
                if (r.Key <= endSortId && r.Key > startSortId)
                    reward = reward + r.Value;
            }
            return reward;
        }
        /// <summary>
        /// 获取升级奖金,如果有越级会给到最高级别的奖金
        /// </summary>
        /// <param name="endSortId"></param>
        /// <returns></returns>
        private static decimal QueryMaxLevelTotalReward(int endSortId)
        {
            return upLevelRewards.ContainsKey(endSortId) ? upLevelRewards[endSortId] : 0;
        }

        /// <summary>
        /// vip 升级操作
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> VipLevelUpTo(VipLevelUpDownInputModel inputModel)
        {
            var tids = GetTids(inputModel.ids);
            if (tids.Item1)
            {
                var whereSql = string.Empty;
                foreach (var m in tids.Item2)
                {
                    if (string.IsNullOrEmpty(whereSql))
                        whereSql = string.Format(@"{0}", m);
                    else
                        whereSql = string.Format(@"{0},{1}", whereSql, m);
                }
                if (!string.IsNullOrEmpty(whereSql))
                    whereSql = string.Format(@"({0})", whereSql);


                #region 找出指定id的实体  VipUpModel
                var LoadVipMonthModelByIdSql = string.Format(PromoSqlCommandText.LoadVipMonthModelByIdSql, whereSql);
                var vipUpModels = await DapperBase.QueryAsync<VipUpModel>(LoadVipMonthModelByIdSql,
                      null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                #endregion

                if(vipUpModels!= null && vipUpModels.Count > 0)
                {
                    var sqlArray = new List<string>();
                    var param = new List<object> { };
                    foreach(var m in vipUpModels)
                    {
                        if (m.is_up != 1)
                            return ApiResultModel<object>.GetResult("false", "目前,仅支持升级操作!", "time_support_up_level_failed");

                        var startSortId = levelSort.ContainsKey(m.level) ? levelSort[m.level] : 0;
                        var endSortId = levelSort[m.nextLevel];
                        var reward = QueryMaxLevelTotalReward(endSortId);// QueryTotalReward(startSortId,endSortId); // upLevelRewardKV[m.nextLevel];

                        //vip升级定单状态更新
                        var UpdateOfTurnOverStatus = string.Format(PromoSqlCommandText.UpdateVipMonthStatus, string.Format(@"({0})", m.id));
                        sqlArray.Add(UpdateOfTurnOverStatus);
                        param.Add(new
                        {
                            status = 1,
                            reviewperson = inputModel.adminAccount,
                            reviewtime = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss")
                        });
                        //vip升级 优惠列表记录
                        var referenceguid = Common.GuidToLongString();
                        var fromVipName = levelName.ContainsKey(m.level) ? levelName[m.level] : "vip";
                        var toVipName = levelName.ContainsKey(m.nextLevel) ? levelName[m.nextLevel] : "vip";
                        var pm = new PromoModel()
                        {
                            account = m.account,
                            amount = 0,
                            reward = reward,
                            remarks = string.Format(@"VIP升级从{0}到{1}",fromVipName,toVipName),
                            level = m.nextLevel,
                            bet = 1,
                            createperson = "***",
                            bonusPromoType = 0,
                            reviewtime = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"),
                            gametype = 100,
                            referrerreg = m.referrerreg,
                            referenceguid = referenceguid,
                            ipaddress = "127.0.0.1",
                            status = 1
                        };
                        sqlArray.Add(PromoSqlCommandText.CreateVipMonthPromoSql);
                        param.Add(pm);

                        //更新主账号余额
                        sqlArray.Add(UserSqlCommandText.UpdateBalanceWithLevelSql);
                        param.Add(new { account = m.account, amount = reward, nextLevel = m.nextLevel });

                    }

                    var result = await DapperBase.ExecuteTransctionAsync(sqlArray,
                        param, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                    return ApiResultModel<object>.GetResult(
                        result ? "true" : "false",
                        result ? "升级审核-成功" : "升级审核-失败",
                        result ? "up_level_verify_success" : "up_level_verify_failed"
                    );

                }
                else
                {
                    return ApiResultModel<object>.GetResult("false", "账号不存在或账号级别已经变动!", "account_info_check_failed");
                }

            }
            return new ApiResultModel<object>()
            {
                code = "false",
                message = "升级审核-失败[ids]",
                message_code = "account_up_level_failed",

            };
        }

        /// <summary>
        /// vip升级数据取消
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> VipLevelCancel(VipLevelUpDownInputModel inputModel)
        {
            var tids = GetTids(inputModel.ids);
            if (tids.Item1)
            {
                var whereSql = string.Empty;
                foreach (var m in tids.Item2)
                {
                    if (string.IsNullOrEmpty(whereSql))
                        whereSql = string.Format(@"{0}", m);
                    else
                        whereSql = string.Format(@"{0},{1}", whereSql, m);
                }
                if (!string.IsNullOrEmpty(whereSql))
                    whereSql = string.Format(@"({0})", whereSql);

                //vip定单状态取消
                var UpdateOfTurnOverStatus = string.Format(PromoSqlCommandText.UpdateVipUpLevelStatusCancel, whereSql);
                //status=@status, remarks=@cancelperson,verify_who=@cancelreason,verify_at=@canceltime
                var result = await DapperBase.ExecuteTransctionAsync(UpdateOfTurnOverStatus,
                      new { status = 2, cancelperson = inputModel.adminAccount, 
                          cancelreason = inputModel.cancelreason, canceltime = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss") }, 
                      commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

                return new ApiResultModel<object>()
                {
                    code = result ? "true" : "false",
                    message = result ? "取消vip升级-成功" : "取消vip升级-失败",
                    message_code = result ? "cancel_vip_up_level_success" : "cancel_vip_up_level_failed"
                };
            }
            return new ApiResultModel<object>()
            {
                code = "false",
                message = "vip升级失败[ids]",
                message_code = "cancel_vip_up_level_failed",

            };
        }

        #endregion

        //VipDepositTimesDailyRecords

        #region vip 降级处理 
        /// <summary>
        /// 降级记录
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> VipLevelDownMonthRecords(QueryVipLevelInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = " lastMonthVipDownData a ";
            string orderBy = " a.month desc";
            string keyName = " a.month,a.account ";


            if (!string.IsNullOrEmpty(inputModel.account))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.account = '{0}' ", inputModel.account) : string.Format(@" {0} and a.account = '{1}' ", whereSql, inputModel.account);
            }
            if (!string.IsNullOrEmpty(inputModel.affaccount))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" b.affaccount = '{0}' ", inputModel.affaccount) : string.Format(@" {0} and b.affaccount = '{1}' ", whereSql, inputModel.affaccount);

            if (inputModel.status != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.status = {0} ", inputModel.status) : string.Format(@" {0} and a.status = {1} ", whereSql, inputModel.status);

            if (inputModel.is_up != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.is_up = {0} ", inputModel.is_up) : string.Format(@" {0} and a.is_up = {1} ", whereSql, inputModel.is_up);

            if (inputModel.vipLevel != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.nextLevel = {0} ", inputModel.vipLevel) : string.Format(@" {0} and a.nextLevel = {1} ", whereSql, inputModel.vipLevel);


            if (inputModel.startTime > -1
                && inputModel.endTime > -1)
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" month >= {0} and month <= {1} ", inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and month >= {1} and month <= {2} ", whereSql, inputModel.startTime, inputModel.endTime);
            }


            var leftjoinString = string.Format(@" left join ofUser b on a.account = b.account ");
            var fields = " a.*,b.affaccount,b.freezereason freezereason,iif(b.anomaly is not null,b.anomaly,0) anomaly ";

            return await DapperPaging<VipLevelUpOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex);
        }

        
        /// <summary>
        /// vip 降级操作
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> VipLevelDownTo(VipLevelUpDownInputModel inputModel)
        {
            var tids = GetTids(inputModel.ids);
            if (tids.Item1)
            {
                var whereSql = string.Empty;
                foreach (var m in tids.Item2)
                {
                    if (string.IsNullOrEmpty(whereSql))
                        whereSql = string.Format(@"{0}", m);
                    else
                        whereSql = string.Format(@"{0},{1}", whereSql, m);
                }
                if (!string.IsNullOrEmpty(whereSql))
                    whereSql = string.Format(@"({0})", whereSql);


                #region 找出指定id的实体  VipUpModel
                var LoadVipMonthModelByIdSql = string.Format(PromoSqlCommandText.LoadVipDownMonthModelByIdSql, whereSql);
                var vipUpModels = await DapperBase.QueryAsync<VipUpModel>(LoadVipMonthModelByIdSql,
                      null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                #endregion

                var lowestLevelAccounts = string.Empty;
               
                if (vipUpModels != null && vipUpModels.Count > 0)
                {
                    if(vipUpModels.Count == 1 
                        && vipUpModels.First().level == 10027)
                    {
                        return ApiResultModel<object>.GetResult("false", "降级失败：当前会员级别为最低级别!", "lowest_down_level_failed");
                    }

                    var sqlArray = new List<string>();
                    var param = new List<object> { };
                    foreach (var m in vipUpModels)
                    {
                        if (m.is_up != 0) 
                            return ApiResultModel<object>.GetResult("false", "目前,仅支持【降级】操作!", "time_support_down_level_failed");
                        if (!levelSort.ContainsKey(m.level))
                            return ApiResultModel<object>.GetResult("false", "当前会员为非VIP会员!", "account_not_vip_failed");

                        if(m.level == 10027)//白银会员，不让被降级了
                        {
                            lowestLevelAccounts = string.IsNullOrEmpty(lowestLevelAccounts) 
                                ? m.account : string.Format(@"{0},{1}", lowestLevelAccounts, m.account);
                            continue;
                        }

                        var toLevel = m.nextLevel;
                        var reward = 0;

                        //vip升级定单状态更新
                        var UpdateOfTurnOverStatus = string.Format(PromoSqlCommandText.UpdateVipDownMonthStatus, string.Format(@"({0})", m.id));
                        sqlArray.Add(UpdateOfTurnOverStatus);
                        param.Add(new
                        {
                            status = 1,
                            reviewperson = inputModel.adminAccount,
                            reviewtime = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss")
                        });
                        
                        //更新主账号余额
                        sqlArray.Add(UserSqlCommandText.UpdateBalanceWithLevelSql);
                        param.Add(new { account = m.account, amount = reward, nextLevel = toLevel });

                    }

                    var result = await DapperBase.ExecuteTransctionAsync(sqlArray,
                        param, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

                    return ApiResultModel<object>.GetResult(
                        result ? "true" : "false",
                        result ? "降级审核-成功" : "降级审核-失败",
                        result ? "down_level_verify_success" : "down_level_verify_failed",
                        data: string.IsNullOrEmpty(lowestLevelAccounts) ? "" : string.Format("已经是最低级别的账号[{0}],不会被降级!", lowestLevelAccounts)
                    );

                }
                else
                {
                    return ApiResultModel<object>.GetResult("false", "账号不存在或账号级别已经变动!", "account_info_check_failed");
                }

            }
            return new ApiResultModel<object>()
            {
                code = "false",
                message = "降级审核-失败[ids]",
                message_code = "account_down_level_failed",

            };
        }

        /// <summary>
        /// vip降级数据取消
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> VipLevelDownCancel(VipLevelUpDownInputModel inputModel)
        {
            var tids = GetTids(inputModel.ids);
            if (tids.Item1)
            {
                var whereSql = string.Empty;
                foreach (var m in tids.Item2)
                {
                    if (string.IsNullOrEmpty(whereSql))
                        whereSql = string.Format(@"{0}", m);
                    else
                        whereSql = string.Format(@"{0},{1}", whereSql, m);
                }
                if (!string.IsNullOrEmpty(whereSql))
                    whereSql = string.Format(@"({0})", whereSql);

                //vip定单状态取消
                var UpdateOfTurnOverStatus = string.Format(PromoSqlCommandText.UpdateVipDownLevelStatusCancel, whereSql);
                var result = await DapperBase.ExecuteTransctionAsync(UpdateOfTurnOverStatus,
                      new
                      {
                          status = 2,
                          cancelperson = inputModel.adminAccount,
                          cancelreason = inputModel.cancelreason,
                          canceltime = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss")
                      },
                      commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

                return new ApiResultModel<object>()
                {
                    code = result ? "true" : "false",
                    message = result ? "取消vip降级-成功" : "取消vip降级-失败",
                    message_code = result ? "cancel_vip_down_level_success" : "cancel_vip_down_level_failed"
                };
            }
            return new ApiResultModel<object>()
            {
                code = "false",
                message = "vip降级失败[ids]",
                message_code = "cancel_vip_down_level_failed",

            };
        }

        #endregion


        #region
        /// <summary>
        /// 降级记录
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> VipDepositTimesDailyRecords(QueryVipDepositTimesDailyInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = " ofVipDepositTimesDaily a ";
            string orderBy = " a.financeGameDay desc";
            string keyName = " a.id ";


            if (inputModel.startTime > -1
                && inputModel.endTime > -1)
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" financeGameDay >= {0} and financeGameDay <= {1} ", inputModel.startTime, inputModel.endTime) 
                    : string.Format(@" {0} and financeGameDay >= {1} and financeGameDay <= {2} ", whereSql, inputModel.startTime, inputModel.endTime);
            }

            whereSql = string.Format(@" {0} and currencyType={1}", whereSql, inputModel.currencyType);
            if(!inputModel.merchantAccount.Equals("-1"))
                whereSql = string.Format(@" {0} and a.merchantAccount='{1}'", whereSql, inputModel.merchantAccount);


            var leftjoinString = string.Format(@" ");
            var fields = " * ";

            return await DapperPaging<VipDepositTimesDailyOutputModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex,DapperStringBase.ConnectionLogs);
        }
        #endregion

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
    }


    public class VipUpModel
    {
        public long id { set; get; }
        public int level { set; get; }
        public int nextLevel { set; get; }
        public string account { set; get; }
        /// <summary>
        /// 0:降级，1：升级，2：保级
        /// </summary>
        public int is_up { set; get; }
        public string referrerreg { set; get; }
    }

    /// <summary>
    /// 红利优惠模型
    /// </summary>
    public class PromoModel
    {
        public string currency { set; get; } = "CNY";
        public string account { set; get; }
        public decimal amount { set; get; }
        public decimal reward { set; get; }
        public string remarks { set; get; }
        public int level { set; get; }
        public decimal bet { set; get; }
        public string createperson { set; get; } = "***";

        public int bonusPromoType { set; get; }
        public string referrerreg { set; get; }
        public string referenceguid { set; get; }
        public string ipaddress { set; get; }
        public int gametype { set; get; }
        public int status { set; get; }
        public string reviewtime { set; get; }

    }
}
