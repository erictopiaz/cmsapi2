using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.YBSport
{
    public class YBSportAppLoginModel
    {
        public string domain { set; get; }
        /// <summary>
        /// 盘口api
        /// </summary>
        public string oddsApiUrl { set; get; }
        /// <summary>
        /// 代理ip 请使用代理ip去请盘口数据
        /// </summary>
        public string apiProxy { set; get; }
        public int porxyPort { set; get; }
        public string token { set; get; }
        /// <summary>
        /// 盘口api 不用加白
        /// </summary>
        public string apiDomain { set; get; }
        /// <summary>
        /// 图片domain
        /// </summary>
        public string imgDomain { set; get; }
    }

    public class YBResponseMode<T>
    {
        public bool status { set; get; }
        public string msg { set; get; }
        public string code { set; get; }
        public T data { set; get; }
        public long serverTime { set; get; }
    }

    public class YBBalance
    {
        public decimal balance { set; get; }
        public string userName { set; get; }
    }

    public class BetRecordResponse
    {
        public int pageSize { set; get; }
        public int totalCount { set; get; }
        public List<SportRecord> list { set; get; }
        public int pageNum { set; get; }
    }
    public class SportRecord
    {
        public string userName { set; get; }
        public string merchantCode { set; get; }
        public int orderStatus { set; get; }
        public int betCount { set; get; }
        public int seriesType { set; get; }
        public string seriesValue { set; get; }
        public string orderNo { set; get; }
        public long createTime { set; get; }

        public decimal orderAmount { set; get; }
        public long? settleTime { set; get; }
        public decimal? settleAmount { set; get; }
        public decimal? profitAmount { set; get; }
        /// <summary>
        /// 订单结算结果0-无结果 2-走水 3-输 4-赢 5-赢一半 6-输一半
        /// </summary>
        public int? outcome { set; get; }
        /// <summary>
        /// 结算次数
        /// </summary>
        public int? settleTimes { set; get; }

        public List<Palary> detailList { set; get; }

    }

    public class Palary
    {
        public string betNo { set; get; }
        public string playOptionsId { set; get; }
        public int matchId { set; get; }
        public decimal betAmount { set; get; }
        public string matchName { set; get; }
        public string matchInfo { set; get; }
        public int matchType { set; get; }
        public string marketType { set; get; }
        public int sportId { set; get; }
        public int playId { set; get; }
        public string sportName { set; get; }
        public string playOptionName { set; get; }
        public string playName { set; get; }
        public string marketValue { set; get; }
        public string oddsValue { set; get; }
        public string betResult { set; get; }
    }

    //"balance": 100.0000,
    //    "userName": "valar666"

//    {
//    "status": true,
//    "msg": "成功",
//    "code": "0000",
//    "data": {
//        "domain": "http://test-user-pc.sportxxxifbdxm2.com/#/home",
//        "token": "715b00a03c5502765b1c05e2301d2a4ffda0dad2"
//    },
//    "serverTime": 1587797016703
//}
}
