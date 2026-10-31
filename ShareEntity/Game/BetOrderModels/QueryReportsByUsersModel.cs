using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.Game.BetOrderModels
{
    /// <summary>
    /// 会员总投注
    /// </summary>
    public class QueryReportsByUsersModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 总投注
        /// </summary>
        public decimal totalBetAmount { get; set; }
        /// <summary>
        /// 有效投注
        /// </summary>
        public decimal betAmount { get; set; }
        /// <summary>
        /// 派奖金额
        /// </summary>
        public decimal prizeAmount { get; set; }
        /// <summary>
        /// 输赢
        /// </summary>
        public decimal winLost { get; set; }
    }



}
