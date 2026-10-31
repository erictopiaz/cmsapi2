using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib;

namespace ShareEntity.OutModels.agent
{
    /// <summary>
    ///  20260711  pp, iaes
    /// 20250205  bti
    /// </summary>
    public class affDailyReportFinanceOutModel : FinanceReportBase
    {
        public long financeDay { set; get; }
        #region 后加需求 统计（游戏平台输赢和下注记录）
        /// <summary>
        /// 输赢
        /// </summary>
        public decimal winlost { set; get; }
        /// <summary>
        /// 投注流水
        /// </summary>
        public decimal bets { set; get; }
        #endregion

        public decimal? obg_bet { get; set; } = 0;
        public decimal? obg_winlost { get; set; } = 0;


    }

    /// <summary>
    /// 20260711 ching 加  pp iaes
    /// 20250205  bti
    /// </summary>
    public class AffDailyReportFinanceOutModel : GameReportBase_
    {
        public string account {  set; get; }
        public long financeDay { set; get; }
        #region 后加需求 统计（游戏平台输赢和下注记录）
        /// <summary>
        /// 输赢
        /// </summary>
        public decimal winlost { set; get; }
        /// <summary>
        /// 投注流水
        /// </summary>
        public decimal bets { set; get; }
        #endregion

        /// <summary>
        /// 存款金额
        /// </summary>
        public decimal depositAmount { set; get; }
        /// <summary>
        /// 充值次数
        /// </summary>
        public int depositTimes { set; get; }
        /// <summary>
        /// 提款金额
        /// </summary>
        public decimal withdrawAmount { set; get; }
        /// <summary>
        /// 提款次数
        /// </summary>
        public int withdrawTimes { set; get; }
        /// <summary>
        /// 优惠
        /// </summary>

        public decimal promo { set; get; }
        /// <summary>
        /// 红利
        /// </summary>
        public decimal bonus { set; get; }
        /// <summary>
        /// 存款反点
        /// </summary>
        public decimal rebate { set; get; }
        /// <summary>
        /// 反水
        /// </summary>
        public decimal turnover { set; get; }
        /// <summary>
        /// 存减提 赢利
        /// </summary>
        public string wd
        {
            get
            {
                return (depositAmount - withdrawAmount).ToString("F2");
            }
        }

        public decimal fee { set; get; }
        /// <summary>
        /// 银行卡收款手续费用
        /// </summary>
        public decimal totalRealBankFee { set; get; }
        /// <summary>
        /// 三方存款手续费用 
        /// </summary>
        public decimal totalDepositFee { set; get; }
        /// <summary>
        /// 提款手续费用
        /// </summary>
        public decimal totalWithdrawFee { set; get; }

    }

    /// <summary>
    /// 20250205  bti
    /// 20260711  加  pp + iaes
    /// </summary>
    public class playersDailyReportFinanceOutModel : FinanceReportBase
    {
        public string affaccount { set; get; }
        public long financeDay { set; get; }
        /// <summary>
        /// 投注
        /// </summary>
        public decimal bets { set; get; }
        /// <summary>
        /// 输赢
        /// </summary>
        public decimal winlost { set; get; }
        /// <summary>
        /// vip 名称
        /// </summary>
        public string levels { set; get; }
        public int level { set; get; }
        public int anomaly { set; get; }
        public int riskTypesAready { set; get; }
        public string riskRemark { set; get; }
        /// <summary>
        /// 会员id
        /// </summary>
        public long uid { set; get; } = 0;
        /// <summary>
        /// 会员的品牌
        /// </summary>
        public string merchantAccount { set; get; } = "-1";
        public int walletType { set; get; } = -1;

        private string _createtime;
        public string createtime
        {
            set
            {
                _createtime = string.IsNullOrEmpty(value) ? "": Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            get { return _createtime; }
        }
    }

    /// <summary>
    /// 会员账号+前缀
    /// 20260711  加  pp + iaes
    /// </summary>
    public class playersDailyReportFinanceOutModel_Account_prefix: playersDailyReportFinanceOutModel
    {
        public string account { set; get; }
    }





    /// <summary>
    /// 缩小100倍输出
    /// 20250205 bti
    /// </summary>
    public class playersDailyReportFinanceOutModel100 : FinanceReportBase100
    {
        public string affaccount { set; get; }
        public long financeDay { set; get; }
        private decimal _bets;
        public decimal bets
        {
            set
            {
                _bets = value;// Math.Round(value / 100, 4);
            }
            get
            {
                return _bets;
            }
        }
        private decimal _winlost;
        public decimal winlost
        {
            set
            {
                _winlost = value;// Math.Round(value / 100, 4);
            }
            get
            {
                return _winlost;
            }
        }
      
        
    }

    public class playersDailyReportFinanceOutModel100Intl : FinanceReportBase100Intl {
        public string affaccount { set; get; }
        public long financeDay { set; get; }
        private decimal _bets;
        public decimal bets {
            set {
                _bets = value;// Math.Round(value / 100, 4);
            }
            get {
                return _bets;
            }
        }
        private decimal _winlost;
        public decimal winlost {
            set {
                _winlost = value;// Math.Round(value / 100, 4);
            }
            get {
                return _winlost;
            }
        }


    }
    public class playersMonthReportFinanceOutModel : FinanceReportBase
    {
        public string affaccount { set; get; }
        public long financeMonth { set; get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExportPlayersMonthReportFinanceOutModel
    {
        public string account { set; get; }
        public string affaccount { set; get; }
        public long financeMonth { set; get; }
        public decimal depositAmount { set; get; }
        //public int depositTimes { set; get; }
        //public decimal depositFee { set; get; }
        public decimal withdrawAmount { set; get; }
        //public int withdrawTimes { set; get; }
        //public decimal withdrawFee { set; get; }
        public decimal promo { set; get; }
        public decimal bonus { set; get; }
    }
    public class ExportPlayersDailyReportFinanceOutModel
    {
        public string account { set; get; }
        public string affaccount { set; get; }
        public string parentAccount { set; get; }
        public long financeDay { set; get; }
        public decimal depositAmount { set; get; }
        public decimal withdrawAmount { set; get; }
        public decimal promo { set; get; }
        public decimal bonus { set; get; }
        public decimal turnover { set; get; }
        public decimal winlost { set; get; }
        public decimal bets { set; get; }
    }



    public class ExportTurnOverOutModel
    {
        public long tid { set; get; }
        public string account { set; get; }
        public decimal effectivebets { set; get; }
        public decimal reward { set; get; }
        public decimal winlost { set; get; }
        public decimal leftbets { set; get; }
        public string begindate { set; get; }
        public string enddate { set; get; }
        public string requesttime { set; get; }
        public int gametype { set; get; }
        public string gamename { set; get; }
        public string statusDesc { set; get; }
        private int _status;
        public int status
        {
            set
            {
                statusDesc = value == 1 ? "审核通过" : value == 0 ? "等待审核" : "取消审核";
                _status = value;
            }
            get
            {
                return _status;
            }
        }
        public string cancelreason { set; get; }
        public string cancelperson { set; get; }
        public string reviewperson { set; get; }
        public string affaccount { set; get; }
        public string calcDesc { set; get; }
        private decimal _calc;
        public decimal calc
        {
            set
            {
                calcDesc = string.Format(@"{0}‰", (value * 1000).ToString("F2"));
                _calc = value;
            }
            get
            {
                return _calc;
            }
        }
        public int turnOverType { set; get; }
    }
}
