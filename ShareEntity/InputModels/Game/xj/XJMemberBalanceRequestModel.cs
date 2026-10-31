using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.xj
{
    public class XJMemberBalanceRequestModel
    {
        public string memberCode { get; set; }
    }

    public class XJMemberBalanceResponseModel
    {
        public decimal balance { get; set; }
        public string currencyCode { get; set; }
    }
    /// <summary>
    /// 请求注单参数
    /// </summary>
    public class XJWagersRequestModel
    {
        /// <summary>
        /// true:结算定单，false:未结算定单
        /// </summary>
        public bool isSettled { set; get; }
        /// <summary>
        /// 游戏类型
        /// </summary>
        public int gameType { set; get; }
        /// <summary>
        /// 注单开始时间
        /// yyyy-MM-dd HH:mm:ss.fffffff
        /// </summary>
        public string startTime { set; get; }
        /// <summary>
        /// 注意结束时间
        /// yyyy-MM-dd HH:mm:ss.fffffff
        /// </summary>
        public string endTime { set; get; }

    }


    public class XJWagersResponseModel
    {
        /// <summary>
        /// 会员账号
        /// </summary>
        public string memberCode { set; get; }
        /// <summary>
        /// 币种
        /// </summary>
        public string currencyCode { set; get; }
        /// <summary>
        /// 会员下注IP
        /// </summary>
        public string ipAddress { set; get; }
        /// <summary>
        /// 注单id 可能包括下注信息。如
        ///table，shoes，
        /// </summary>
        public string id { set; get; }
        /// <summary>
        /// 下注时间
        /// </summary>
        public string createTime { set; get; }
        /// <summary>
        /// 注单结算时间
        /// </summary>
        public string settleTime { set; get; }
        /// <summary>
        /// 注单状态
        /// 1 確認 - 會員下注確認（未結帳） 2 結帳 - 會員的投注結算
        /// 3 取消 - 會員投注取消（因遊戲取消）
        /// 4 無效 - 會員投注無效
        /// </summary>
        public int wagerStatus { set; get; }
        /// <summary>
        /// 下注总金额
        /// </summary>
        public decimal stake { set; get; }
        /// <summary>
        ///  从会员角度显示赢/输的金额
        ///  贏：（下注金額 + 贏的金額）
        ///  輸：金額是 0
        ///  平手：（取決於遊戲規則）
        /// </summary>
        public decimal returnAmount { set; get; }



        public decimal cashoutStake { set; get; }
        public decimal cashoutReturnAmount { set; get; }
        public decimal remainingStake { set; get; }
        public decimal remainingReturnAmount { set; get; }
        /// <summary>
        /// 来源装置
        /// 11：h5, 12:pc ,20:appandroid, 21:appios,31:nano,32:mini,41:download
        /// </summary>
        public int channel { set; get; }
        /// <summary>
        /// 会员的赔率类型
        /// 1:欧洲盘口， 2：香港盘口， 3：马来盘口， 4：印尼盘口
        /// </summary>
        public int oddsType { set; get; }
        /// <summary>
        /// 注单赔率
        /// </summary>
        public decimal odds { set; get; }
        public List<Bets> Bets { set; get; }
        public List<CashOut> cashout { set; get; }
    }

    public class Bets
    {
        /// <summary>
        /// 注单编号
        /// </summary>
        public string id { set; get; }
        /// <summary>
        /// 投注顺序
        /// </summary>
        public int seq { set; get; }
        /// <summary>
        /// 投注状态  查看5.12
        /// 0 初始,1 贏, 2 輸, 3 平局,4 取消
        /// </summary>
        public int betStatus { set; get; }
        /// <summary>
        /// 投注金额
        /// </summary>
        public decimal stake { set; get; }
        /// <summary>
        /// 會員方 贏/輸 的金額
        /// </summary>
        public decimal returnAmount { set; get; }
        /// <summary>
        /// 注单赔率
        /// </summary>
        public decimal odds { set; get; }
        /// <summary>
        /// 下注類型名稱
        /// 如：  Asian Handicap
        /// </summary>
        public string betType { set; get; }
        /// <summary>
        /// 注單的選擇
        /// 如  home 或，away
        /// </summary>
        public string selection { set; get; }
        /// <summary>
        /// 让分大小
        /// </summary>
        public decimal? handicap { set; get; }
        /// <summary>
        /// half time
        /// </summary>
        public string period { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string group { set; get; }
        /// <summary>
        /// 無效原因
        /// </summary>
        public string remark { set; get; }

        [JsonProperty(PropertyName="event")]
        public Event events { set;get; }
        /// <summary>
        /// 客队分数
        /// </summary>
        public string scoreAway { set; get; }
        /// <summary>
        /// 主队分数
        /// </summary>
        public string scoreHome { set; get; }
        public  int marketlineId { set; get; }
    }


    public class Event
    {
        /// <summary>
        /// 体育4000
        /// </summary>
        public int gameType { set; get; }
        /// <summary>
        /// 事件id
        /// </summary>
        public string id { set; get; }
        /// <summary>
        /// 事件名
        /// </summary>
        public string name { set; get; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public string startTime { set; get; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public string endTime { set; get; }
        /// <summary>
        /// 活动结果   AgileBet 为空
        /// </summary>
        public string results { set; get; }
        public string tag { set; get; }
        /// <summary>
        /// 遊戲名稱清單
        /// 如：Football
        /// </summary>
        public string eventType { set; get; }
    }
    public class CashOut
    {
        public string id { set; get; }
        /// <summary>
        /// Cashout 順序
        /// </summary>
        public int seq { set; get; }
        /// <summary>
        /// Cashout 建立時間
        /// </summary>
        public DateTime cashoutCreateTime { set; get; }
        /// <summary>
        /// 502:成功
        /// 503,504無效
        /// </summary>
        public int cashoutStatus { set; get; }
        /// <summary>
        /// Cashout 金額
        /// </summary>
        public decimal cashoutStake { set; get; }
        /// <summary>
        /// Display cashout 實際回返金額
        /// </summary>
        public decimal cashoutReturnAmount { set; get; }
    }


    public class XJWagersTableModel
    {
        /// <summary>
        /// 会员账号
        /// </summary>
        public string memberCode { set; get; }
        /// <summary>
        /// 币种
        /// </summary>
        public string currencyCode { set; get; }
        /// <summary>
        /// 会员下注IP
        /// </summary>
        public string ipAddress { set; get; }
        /// <summary>
        /// 注单id 可能包括下注信息。如
        ///table，shoes，
        /// </summary>
        public string wager_id { set; get; }
        /// <summary>
        /// 下注时间
        /// </summary>
        public string createTime { set; get; }
        /// <summary>
        /// 注单结算时间
        /// </summary>
        public string settleTime { set; get; }
        /// <summary>
        /// 注单状态
        /// 1 確認 - 會員下注確認（未結帳） 2 結帳 - 會員的投注結算
        /// 3 取消 - 會員投注取消（因遊戲取消）
        /// 4 無效 - 會員投注無效
        /// </summary>
        public int wagerStatus { set; get; }
        /// <summary>
        /// 下注总金额
        /// </summary>
        public decimal stake { set; get; }
        /// <summary>
        ///  从会员角度显示赢/输的金额
        ///  贏：（下注金額 + 贏的金額）
        ///  輸：金額是 0
        ///  平手：（取決於遊戲規則）
        /// </summary>
        public decimal returnAmount { set; get; }


        /// <summary>
        /// 来源装置
        /// 11：h5, 12:pc ,20:appandroid, 21:appios,31:nano,32:mini,41:download
        /// </summary>
        public int channel { set; get; }
        /// <summary>
        /// 会员的赔率类型
        /// 1:欧洲盘口， 2：香港盘口， 3：马来盘口， 4：印尼盘口
        /// </summary>
        public int oddsType { set; get; }
        /// <summary>
        /// 注单赔率
        /// </summary>
        public decimal odds { set; get; }
        public List<Bets> Bets { set; get; }
       
    }

}
