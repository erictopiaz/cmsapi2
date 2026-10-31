using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel.JL {
    public class JL_OrderLog_settle  {
        /// <summary>
        /// 同一注单重送请求时会与前次不同
        /// </summary>
        public string reqId { set; get; }

        /// <summary>
        /// 货币名称
        /// </summary>
        public string currency { set; get; } = "INR";
        /// <summary>
        ///  游戏代码
        /// </summary>
        public int game { set; get; }
        /// <summary>
        /// 下注/结算行为唯一识别值。等同于交易序号
        /// </summary>
        [Key]
        public long round { set; get; }
        /// <summary>
        /// 注单结账时间戳
        /// </summary>
        public long wagersTime { set; get; }

        public DateTime wagersTimeSrc { set; get; }

        /// <summary>
        /// 押注金额
        /// </summary>
        public decimal betAmount { set; get; }
        public decimal winloseAmount { set; get; }
        /// <summary>
        /// 牌局唯一值(所有下注及结算共享同一值, 等同 §3.1.3 WagersId)
        /// </summary>
        public long sessionId { set; get; } = -1;
        /// <summary>
        ///  注单类型:1=下注 2=结算
        /// </summary>
        public int type { set; get; } = -1;

        /// <summary>
        /// 玩家账号唯一值。结算(type= 2) 时必带; 下注(type= 1) 时依营运商需求带入
        /// </summary>
        public string userId { set; get; } = "";
        /// <summary>
        /// 有效投注金额, 结算才会带入(请参考 Turnover (有效投注额) 一节)
        /// </summary>
        public decimal turnover { set; get; } = 0;
        /// <summary>
        /// 预扣金额(仅用于有预先扣款机制的游戏，请参考Preserve (预扣保证金))
        /// </summary>
        public decimal preserve { set; get; } = 0;

        /// <summary>
        /// 玩家装置信息(依营运商需求带入; 营运商须在 Login §2.1.1 /LoginWithoutRedirect §2.1.2 带入)
        /// </summary>
        public string platform { set; get; } = "";
        /// <summary>
        /// 牌局全部下注总和(依营运商需求带入; 只在结算时
        /// </summary>
        public decimal sessionTotalBet { set; get; } = 0;
        /// <summary>
        /// 注单类型(依营运商需求带入; 请参考附录 C)
        /// </summary>
        public int statementType { set; get; } = -1;
        /// <summary>
        /// 游戏类型(依营运商需求带入; 请参考附录 D)
        /// </summary>
        public int gameCategory { set; get; } = -1;

        #region 电子和鱼机 

        /// <summary>
        /// 1. 鱼机游戏大单号(依营运商需求带入) 或 2. 离线开奖的触发局局号
        /// </summary>
        public long transactionId { set; get; } = -1;

        #endregion

    }
}
