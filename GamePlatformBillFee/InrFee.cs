using GamePlatformBillFee.models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GamePlatformBillFee
{
    /// <summary>
    /// inr 356 结算平台费用 比例
    /// </summary>
    public class InrFee
    {
        /// <summary>
        /// 纯利润
        /// </summary>
        /// <returns></returns>
        public static decimal getBillData(AFMontBillReportInrModel m)
        {
            var platFee = getTotalPlatformFee(m);
            platFee = platFee >= 0 ? platFee : 0;//总手续费负数时，手续费为0
            return m.totalWinlost - m.totalTurnover - platFee - m.totalBonus - m.totalFee;
        }
        /// <summary>
        /// 获取总平台费用
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static decimal getTotalPlatformFee(AFMontBillReportInrModel m)
        {
            return getPlatformFee(m.pg_winlost, "pg", m.account)
                + getPlatformFee(m.jdb_winlost, "jdb", m.account)
                + getPlatformFee(m.evo_winlost, "evo", m.account)
                + getPlatformFee(m.spribe_winlost, "spribe", m.account)
                + getPlatformFee(m.turbo_winlost, "turbo", m.account)
                + getPlatformFee(m.b9w_winlost, "9w", m.account)
                + getPlatformFee(m.jl_winlost, "jl", m.account);
        }
        private static decimal getPlatformFee(decimal? winlost, string plat, string account)
        {
            //输赢为负时，平台费用为0的账号
            var w = winlost ?? 0;
            if (platfeeMap.ContainsKey(plat))
                return w * platfeeMap[plat];
            else
                return w * 0;
        }
        /// <summary>
        /// 平台结算平台费用比例
        /// </summary>
        private static Dictionary<string, decimal> platfeeMap = new Dictionary<string, decimal>()
        {
            { "pg", 0.07m },
            { "jdb", 0.07m },
            { "evo", 0.07m },
            { "turbo", 0.07m },
            { "spribe", 0.07m },
            { "9w", 0.07m },
            { "b9w", 0.07m },
            { "jl", 0.07m }

        };


        /// <summary>
        /// 获取代理的结算金额30%   35%   40%  45%  50%
        /// v1: 小1M  5活跃  30% 
        /// v2: 小3M  10活跃  35%
        /// v3: 小6M  25活跃  40%
        /// v4: 小10M  50活跃  45%
        /// v5: 大10M  80活跃  50%
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="activePlayers"></param>
        /// <returns></returns>
        public static Tuple<decimal, decimal, bool> GetBilledAmount(decimal amount, int activePlayers)
        {
            var p = 0.3000m;
            var toUnSettleAmount = false;
            if (amount >= 100000000 && activePlayers >= 80)
                p = 0.5000m;
            else if (amount >= 10000000 && activePlayers >= 50)
                p = 0.4500m;
            else if (amount >= 6000000 && activePlayers >= 25)
                p = 0.4000m;
            else if (amount >= 3000000 && activePlayers >= 10)
                p = 0.3500m;

            if (amount < 1000000 || activePlayers < 5)//结算到累积池内
                toUnSettleAmount = true;

            var billAmount = amount * p;
            return new Tuple<decimal, decimal, bool>(billAmount, p, toUnSettleAmount);

        }
    }
}
