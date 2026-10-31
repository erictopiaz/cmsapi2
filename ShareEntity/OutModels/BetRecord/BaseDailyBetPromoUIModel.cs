using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.BetRecord
{
    /// <summary>
    /// 流水专题优惠ui
    /// </summary>
    public class BasePromoUIModel
    {
        /// <summary>
        /// 平台名描述
        /// </summary>
        public string title { get; set; } = "";
        /// <summary>
        /// 优惠名描述
        /// </summary>
        public string subTitle { get; set; } = "";
        /// <summary>
        /// 申请时间描述
        /// </summary>
        public string startTime { get; set; } = "";
        /// <summary>
        /// 数据统计时间
        /// </summary>
        public string reportTime { set; get; } = "";
        /// <summary>
        /// 奖励金额
        /// </summary>
        public string reward { set; get; } = "";
        /// <summary>
        /// 优惠代码
        /// </summary>
        public int promoCode { set; get; }
        /// <summary>
        /// 优惠UI 类型 0：注单，1：救援金
        /// </summary>
        public int promoType { set; get; }
        /// <summary>
        /// 0:领取，1：申请
        /// </summary>
        public int submitType { set; get; }
        /// <summary>
        /// 默认为0，  0：未领取状态，1：已经领取
        /// </summary>
        public int buttonStatus { set; get; } = 0;
        /// <summary>
        /// 流水倍数要求
        /// </summary>
        public string betTimes { set; get; } = "";
    }

    /// <summary>
    /// pg 每日流水彩金
    /// </summary>
    public class PGTaskLevelUIModel : BasePromoUIModel
    {
        /// <summary>
        /// 总投注金额
        /// </summary>
        public string totalBetAmount { get; set; } = "--";
        /// <summary>
        /// 关卡数据
        /// </summary>
        public string taskLevel { set; get; } = "--";

    }



    /// <summary>
    /// pg 每日流水彩金
    /// </summary>
    public class PGDailyBetUIModel: BasePromoUIModel
    {
        /// <summary>
        /// 总投注金额
        /// </summary>
        public string totalBetAmount { get; set; } = "--";

    }


    /// <summary>
    /// 包赔
    /// </summary>
    public class IndemnityUIModel : BasePromoUIModel
    {
        /// <summary>
        /// 总投注金额
        /// </summary>
        public string totalBetAmount { get; set; } = "--";

    }

    /// <summary>
    /// pg 每日注单挑战金
    /// </summary>
    public class PGDailyOrderUIModel : BasePromoUIModel
    {
        /// <summary>
        /// 注单总投注金额
        /// </summary>
        public string totalBetAmount { get; set; } = "--";
        /// <summary>
        /// 单数
        /// </summary>
        public string orderCount { set; get; } = "--";
        /// <summary>
        /// 平均每单投注
        /// </summary>
        public string avgValidAmount { set; get; } = "--";

    }

    /// <summary>
    /// pg 每日救援金
    /// </summary>
    public class PGDailyRescueUIModel : BasePromoUIModel
    {
        /// <summary>
        /// 注单总输赢
        /// </summary>
        public string totalWinlost { get; set; }
        /// <summary>
        /// 总优惠
        /// </summary>
        public string promo { set; get; }
        /// <summary>
        /// 总反水
        /// </summary>
        public string turnover { set; get; }

    }

    /// <summary>
    /// pg 幸运注单
    /// </summary>
    public class PGDailyLuckyOrderUIModel : BasePromoUIModel
    {
        /// <summary>
        /// 注单号
        /// </summary>
        public string orderNo { get; set; }

    }

    /// <summary>
    /// pg 幸运注单
    /// </summary>
    public class PGHighOddsOrderUIModel : BasePromoUIModel
    {
        /// <summary>
        /// 注单号
        /// </summary>
        public string orderNo { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        public string rewardTimes { get; set; } = "--";
    }


}
