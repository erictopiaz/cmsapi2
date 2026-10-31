using GamePlatformBillFee.models;
using ShareEntity.OutModels.agent;
using System;
using System.Collections.Generic;
using System.Text;

namespace GamePlatformBillFee
{
    /// <summary>
    /// cny 156 结算平台费用 比例
    /// </summary>
    public class CNYFee
    {
        /// <summary>
        /// 纯利润
        ///  20260711  pp, iaes
        /// </summary>
        /// <param name="aFMontBillReportModel"></param>
        /// <returns></returns>
        public static decimal getBillData(AFMontBillReportCnyModel m)
        {
            var platFee = getTotalPlatformFee(m);
            platFee = platFee >= 0 ? platFee : 0;
            return m.totalWinlost - m.totalTurnover - platFee - m.totalBonus - m.totalFee;
        }

        /// <summary>
        /// 获取总平台费用
        ///  20260711  pp, iaes
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static decimal getTotalPlatformFee(AFMontBillReportCnyModel m)
        {
            return getPlatformFee(m.ea_winlost, "ea", m.account)
                + getPlatformFee(m.kg_winlost, "kg", m.account)
                //+ getPlatformFee(m.sb_winlost, "sb")
                + getPlatformFee(m.bbin_winlost, "bbin", m.account)
                + getPlatformFee(m.kg2_winlost, "kg2", m.account)
                + getPlatformFee(m.pt_winlost, "pt", m.account)
                + getPlatformFee(m.ttg_winlost, "ttg", m.account)
                //+ getPlatformFee(m.gg_winlost, "gg", m.account)
                + getPlatformFee(m.ag_winlost, "ag", m.account)
                + getPlatformFee(m.vr_winlost, "vr", m.account)
                + getPlatformFee(m.xj_winlost, "xj", m.account)
                + getPlatformFee(m.imes_winlost, "imes", m.account)
                + getPlatformFee(m.ob_winlost, "ob", m.account)
                + getPlatformFee(m.ysb_winlost, "ysb", m.account)
                + getPlatformFee(m.pg_winlost, "pg", m.account)
                + getPlatformFee(m.lb_winlost, "lb", m.account)
                + getPlatformFee(m.ky_winlost, "ky", m.account)
                + getPlatformFee(m.rg_winlost, "rg", m.account)
                + getPlatformFee(m.dscp_winlost, "dscp", m.account)
                //+ getPlatformFee(m.dses_winlost, "dses")
                + getPlatformFee(m.bg_winlost, "bg", m.account)
                + getPlatformFee(m.eb_winlost, "eb", m.account)
                + getPlatformFee(m.td_winlost, "td", m.account)
                + getPlatformFee(m.tf_winlost, "tf", m.account)
                + getPlatformFee(m.aw_winlost, "aw", m.account)
                + getPlatformFee(m.cq_winlost, "cq", m.account)
                + getPlatformFee(m.obg_winlost, "obg", m.account) 
                + getPlatformFee(m.obcv_winlost, "obcv", m.account)
                + getPlatformFee(m.imsp_winlost, "imsp", m.account)
                + getPlatformFee(m.sw_winlost, "sw", m.account)
                + getPlatformFee(m.jdb_winlost, "jdb", m.account)
                + getPlatformFee(m.cr_winlost, "cr", m.account)
                + getPlatformFee(m.fb_winlost, "fb", m.account)
                + getPlatformFee(m.db_winlost, "db", m.account)
                + getPlatformFee(m.dbkeno_winlost, "dbkeno", m.account)
                + getPlatformFee(m.rsg_winlost, "rsg", m.account) + getPlatformFee(m.bti_winlost, "bti", m.account)
                + getPlatformFee(m.pp_winlost, "pp", m.account)
                + getPlatformFee(m.iaes_winlost, "iaes", m.account);
        }

        /// <summary>
        /// 获取总平台费用
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static decimal getTotalPlatformFeeV(affDailyReportFinanceOutModel m)
        {
            return //getPlatformFee(m.ea_winlost, "ea", m.account)
                //+ getPlatformFee(m.kg_winlost, "kg", m.account)
                //+ getPlatformFee(m.sb_winlost, "sb")
                + getPlatformFee(m.bbin_winlost, "bbin", m.account)
                + getPlatformFee(m.kg3_winlost, "kg3", m.account)
                //+ getPlatformFee(m.pt_winlost, "pt", m.account)
                + getPlatformFee(m.ttg_winlost, "ttg", m.account)
                //+ getPlatformFee(m.gg_winlost, "gg", m.account)
                + getPlatformFee(m.ag_winlost, "ag", m.account)
                + getPlatformFee(m.vr_winlost, "vr", m.account)
                + getPlatformFee(m.xj_winlost, "xj", m.account)
                + getPlatformFee(m.imes_winlost, "imes", m.account)
                + getPlatformFee(m.ob_winlost, "ob", m.account)
                //+ getPlatformFee(m.ysb_winlost, "ysb", m.account)
                + getPlatformFee(m.pg_winlost, "pg", m.account)
                + getPlatformFee(m.lb_winlost, "lb", m.account)
                + getPlatformFee(m.ky_winlost, "ky", m.account)
                //+ getPlatformFee(m.rg_winlost, "rg", m.account)
                //+ getPlatformFee(m.dscp_winlost, "dscp", m.account)
                //+ getPlatformFee(m.dses_winlost, "dses")
                //+ getPlatformFee(m.bg_winlost, "bg", m.account)
                //+ getPlatformFee(m.eb_winlost, "eb", m.account)
                //+ getPlatformFee(m.td_winlost, "td", m.account)
                + getPlatformFee(m.tf_winlost, "tf", m.account)
                + getPlatformFee(m.aw_winlost, "aw", m.account)
                //+ getPlatformFee(m.cq_winlost, "cq", m.account)
                + getPlatformFee(m.obg_winlost, "obg", m.account) 
                + getPlatformFee(m.obcv_winlost, "obcv", m.account)
                + getPlatformFee(m.imsp_winlost, "imsp", m.account)
                + getPlatformFee(m.dbkeno_winlost, "dbkeno", m.account)
                //+ getPlatformFee(m.sw_winlost, "sw", m.account)
                + getPlatformFee(m.jdb_winlost, "jdb", m.account)
                + getPlatformFee(m.cr_winlost, "cr", m.account)
                + getPlatformFee(m.fb_winlost, "fb", m.account)
                + getPlatformFee(m.db_winlost, "db", m.account)
                + getPlatformFee(m.rsg_winlost, "rsg", m.account) + getPlatformFee(m.bti_winlost, "bti", m.account)
                + getPlatformFee(m.pp_winlost, "pp", m.account)
                + getPlatformFee(m.iaes_winlost, "iaes", m.account);
        }
        private static decimal getPlatformFee(decimal? winlost, string plat, string account)
        {
            //输赢为负时，平台费用为0的账号
            // var accounts = new List<string>() { "miledson889", "miledson8888", "miledson888", "miledson88", "miledson86", "miledson" };
            var w = (winlost > 0 || winlost != null) ? (decimal)winlost : 0;
            //if (!accounts.Contains(account.ToLower()) && w <=0 )//特定账号，输赢为负时，平台费用为0
            //    return 0;
            if (GameFee.platfeeMap.ContainsKey(plat))
                return w * GameFee.platfeeMap[plat];
            else
                return w * 0;
        }
       
        /// <summary>
        /// 获取代理的结算金额30%   35%   40%  45%  50% 55%
        /// v1: 小10w  5活跃  35% 
        /// v2: 小30w  10活跃  40%
        /// v3: 小60w  25活跃  45%
        /// v4: 小100w  50活跃  50%
        /// v5: 大100w  80活跃  55%
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="activePlayers"></param>
        /// <returns></returns>
        public static Tuple<decimal, decimal, bool> GetBilledAmount(decimal amount, int activePlayers)
        {
            var p = 0.3000m;
            var toUnSettleAmount = false;
            if (amount > 1000000 && activePlayers >= 80)
                p = 0.5500m;
            else if (amount > 600000 && amount <= 1000000 && activePlayers >= 50)
                p = 0.5000m;
            else if (amount > 300000 && amount <= 600000 && activePlayers >= 25)
                p = 0.4500m;
            else if (amount > 100000 && amount<= 300000 && activePlayers >= 10)
                p = 0.4000m;
            else if (amount >= 1000 && amount <= 100000 && activePlayers >= 5)
                p = 0.3500m;

            if (amount < 1000 || activePlayers < 5)//结算到累积池内
                toUnSettleAmount = true;

            var billAmount = amount * p;
            return new Tuple<decimal, decimal, bool>(billAmount, p, toUnSettleAmount);

        }


    }
}
