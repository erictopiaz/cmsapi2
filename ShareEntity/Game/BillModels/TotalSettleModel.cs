using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.Game.BillModels
{
    /// <summary>
    /// 合营app“历史结算”列表的总统计
    /// </summary>
    public class TotalSettleModel
    {
        /// <summary>
        /// 总佣金
        /// </summary>
        public decimal totalAmount { set; get; } = 0.0m;
        /// <summary>
        /// 总累计
        /// </summary>
        public decimal totalUnSettleAmount { set; get; } = 0.0m;
        /// <summary>
        /// 总抽点
        /// </summary>
        public decimal totalSubAmount { set; get; } = 0.0m;
    }
}
