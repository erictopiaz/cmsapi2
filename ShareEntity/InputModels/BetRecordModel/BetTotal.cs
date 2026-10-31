using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.BetRecordModel
{
    /// <summary>
    /// 注单首页统计数据
    /// </summary>
    public class BetTotal
    {
        /// <summary>
        /// 投注额
        /// </summary>
        public decimal betAmount { set; get; }
        /// <summary>
        /// 游戏输赢
        /// </summary>
        public decimal winlost { set; get; }
        /// <summary>
        /// 有效投注额
        /// </summary>
        public decimal stake { set; get; }
        /// <summary>
        /// 会员账号
        /// </summary>
        public string playerAccount { set; get; }
        /// <summary>
        /// 游戏平
        /// </summary>
        public string gameName { set; get; }
        /// <summary>
        /// 游戏id
        /// </summary>
        public int gameId { set; get; }
        /// <summary>
        /// 游戏icon url
        /// </summary>
        public string iconUrl { set; get; }
    }
}
