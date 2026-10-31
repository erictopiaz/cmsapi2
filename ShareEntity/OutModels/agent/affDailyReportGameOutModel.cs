using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareEntity.OutModels.agent
{
    /// <summary>
    /// 20250205 ching 加 bti
    /// </summary>
    public partial class affDailyReportGameOutModel : GameReportBase
    {
       public long gameReportDay { set; get; }

    }
    /// <summary>
    /// 20250205 ching 加 bti
    /// </summary>
    public sealed class GameReportOutModel: GameReportBase
    {
        public decimal totalWinlost { set; get; }
        public decimal totalBet { set; get; }
    }
    /// <summary>
    /// 20260711 加 pp + iaes
    /// 20250205 ching 加 bti
    /// </summary>
    public partial class playersDailyReportGameOutModel : GameReportBase
    {
        public long gameReportDay { set; get; }
        public string affaccount { set; get; }
        #region 会员游戏统计报表
        public decimal totalBet { set; get; }
        public decimal totalWinlost { set; get; }
        public decimal totalTurnover { set; get; }
        #endregion
        public int anomaly { set; get; }
        public int riskTypesAready { set; get; }
        public string riskRemark { set; get; }
    }
    /// <summary>
    /// 缩小100倍
    /// </summary>
    public partial class playersDailyReportGameOutModel100 : GameReportBase100
    {
        private string _account;
        /// <summary>
        /// 会员账号
        /// </summary>
        public string account
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Count() > 5)
                {
                    _account = string.Format(@"{0}*", value.Substring(0, 5));
                }
                else
                {
                    _account = value;
                }
            }
            get { return _account; }
        }
        public long gameReportDay { set; get; }
        public string affaccount { set; get; }
        #region 会员游戏统计报表
        private decimal _totalBet;
        public decimal totalBet
        {
            set
            {
                _totalBet = Math.Round(value / 100, 4);
            }
            get
            {
                return _totalBet;
            }
        }
        private decimal _totalWinlost;
        public decimal totalWinlost
        {
            set
            {
                _totalWinlost = Math.Round(value / 100, 4);
            }
            get
            {
                return _totalWinlost;
            }
        }
        private decimal _totalTurnover;
        public decimal totalTurnover
        {
            set
            {
                _totalTurnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _totalTurnover;
            }
        }
        #endregion
    }
    public partial class playersMonthReportGameOutModel : GameReportBase
    {
        public long gameReportMonth { set; get; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class ExportPlayersMonthReportGameOutModel : GameBase
    {
        public string account { set; get; }
        public string affaccount { set; get; }
        public long gameReportMonth { set; get; }

    }

    /// <summary>
    /// 20250205 bti dbkeno
    /// 20260805 iaes+ pp 
    /// </summary>
    public class ExportPlayersDailyReportGameOutModel : GameBase
    {
        public string account { set; get; }
        public string affaccount { set; get; }
        public string parentAccount { set; get; }
        public long gameReportDay { set; get; }

    }


   


}
