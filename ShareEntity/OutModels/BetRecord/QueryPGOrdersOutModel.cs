using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ShareEntity.OutModels.BetRecord
{
    public class QueryPGOrdersOutModel
    {
        public string message { get; set; }
        /// <summary>
        /// 注单数
        /// </summary>
        public int orderCount { get; set; }
        /// <summary>
        /// 总有效投注金额 
        /// </summary>
        public decimal totalValidAmount { get; set; }
        /// <summary>
        /// 每单平均投注
        /// </summary>
        public decimal avgValidAmount { get; set; }
    }

    //{
    //  "message": null,
    //  "orderCount": 14,
    //  "totalValidAmount": 25.2,
    //  "avgValidAmount": 1.8
    //}


    public class QueryGameReportOutModel
    {
        public string message { get; set; }
        public List<GameReportOutModel> result { get; set; }
    }

    public class GameReportOutModel
    {
        /// <summary>
        /// 游戏id
        /// </summary>
        public int gameId { get; set; }
        /// <summary>
        /// 游戏平台名
        /// </summary>
        public string gameName { get; set; }
        /// <summary>
        /// 总投注
        /// </summary>
        public decimal totalBetAmount { get; set; }
        /// <summary>
        /// 总有效投注
        /// </summary>
        public decimal totalValidAmount { get;set; }
        /// <summary>
        /// 总派采
        /// </summary>
        public decimal totalPrizeAmount { get; set; }
        /// <summary>
        /// 总输赢
        /// </summary>
        public decimal winLoss { set; get; }
    }


    /// <summary>
    /// 幸运注单回应数据
    /// </summary>
    public class QueryPGHighOddsOrdersOutModel
    {
        public string message { get; set; }
        public decimal totalValidAmount { get; set; }
        /// <summary>
        /// 5 个固定集合（index0~4），每个 index 是一个注单列表：
        /// 0: 中奖赔率 ≥ 60 倍 
        /// 1: 中奖赔率 ≥ 120 倍 
        /// 2: 中奖赔率 ≥ 150 倍 
        /// 3: 中奖赔率 ≥ 200 倍 
        /// 4: 中奖赔率 ≥ 300 倍 
        /// 实际实现中会保证：同一个订单号只会出现在“最高赔率档”对应的 index 中。
        /// </summary>
        public List<List<QueryPgHighOddsOrderModel>> groups { get; set; }
    }
    /// <summary>
    /// 高中奖注单
    /// </summary>
    public class QueryPgHighOddsOrderModel
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string orderId { get; set; }
        /// <summary>
        /// 有效投注金额
        /// </summary>
        public decimal validAmount { get; set; }
        /// <summary>
        /// 派彩金额
        /// </summary>
        public decimal prizeAmount { get; set; }
        /// <summary>
        /// 实际中奖赔率
        /// </summary>
        public decimal oddsMultiple { set; get; }
        /// <summary>
        /// 注单时间
        /// </summary>
        public string orderTime { get; set; }
        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal reward { set; get; } = 0;
    }




    /// <summary>
    /// 幸运注单回应数据
    /// </summary>
    public class QueryPGLuckyOrdersOutModel
    {
        public string message { get; set; }
        public decimal totalValidAmount { get; set; }
        /// <summary>
        /// 8 个固定集合（index0~7），每个 index 是一个注单列表： 
        /// 0: 尾数 8 
        /// 1: 尾数 18 
        /// 2: 尾数 018 
        /// 3: 尾数 8888 
        /// 4: 尾数 88888 
        /// 5: 尾数 888888 
        /// 6: 尾数 8888888（保留位，暂未使用） 
        /// 7: 尾数 88888888 
        /// 实际实现中会保证：同一个订单号只会出现在“最长匹配尾数”的那个 index 里。
        /// </summary>
        public List<List<PGLuckyOrderModel>> groups { get; set; }
    }

    /// <summary>
    /// 注音详情
    /// </summary>
    public class PGLuckyOrderModel
    {
        /// <summary>
        /// 单号
        /// </summary>
        public string orderId { get; set; } = "";
        /// <summary>
        /// 有效投注金额 
        /// </summary>
        public decimal validAmount { get; set; }
        /// <summary>
        /// 下注时间
        /// </summary>
        public string orderTime  { get; set; }
        /// <summary>
        /// 定单对应的，奖励金额
        /// </summary>
        public decimal reward { set; get; } = 0;
    }

}
