using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareEntity.OutModels.agent
{
    /// <summary>
    /// 代理财务数据
    /// 20260711  pp, iaes
    /// 20250205  bti
    /// </summary>
    public class FinanceReportBase: GameReportBase
    {
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
    /// 代理财务数据
    /// 缩小100倍输出 fb , jdb
    /// 20250205 bti
    /// </summary>
    public class FinanceReportBase100 : GameReportBase
    {
        //public long tid { set; get; }
        private string _account;
        /// <summary>
        /// 重写上层
        /// </summary>
        public string account {
            set
            {
                playerAccount = value;
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

        public string playerAccount {
            set;get;
        }


        /// <summary>
        /// 当前级别名称
        /// </summary>
        public string levels { set; get; }
        /// <summary>
        /// 级别代码
        /// </summary>
        public int levelnumber { set; get; }

        private string _createtime;
        public string createtime { 
            set {
                if(!string.IsNullOrEmpty(value)) {
                    _createtime = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss"); 
                } else { _createtime = ""; }
            }
            get { return _createtime; }
        }

        public string _lastLogin;
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public string lastLogin {
            set {
                if(!string.IsNullOrEmpty(value))
                    _lastLogin = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                else { _lastLogin = ""; }
            }
            get { return _lastLogin; }
        }

        private decimal _depositAmount;
        public decimal depositAmount {
            set
            {
                _depositAmount = value;// Math.Round(value / 100, 4);
            }
            get
            {
                return _depositAmount;
            }
        }
        public int depositTimes { set; get; }
        
        private decimal _withdrawAmount;
        public decimal withdrawAmount {
            set
            {
                _withdrawAmount = value;// Math.Round(value / 100, 4);
            }
            get
            {
                return _withdrawAmount;
            }
        }
        public int withdrawTimes { set; get; }

        private decimal _totalDepositFee;
        public decimal totalDepositFee {
            set
            {
                _totalDepositFee = value;// Math.Round(value / 100, 4);
            }
            get
            {
                return _totalDepositFee;
            }
        }
        private decimal _totalRealBankFee;
        public decimal totalRealBankFee
        {
            set
            {
                _totalRealBankFee = value;// Math.Round(value / 100, 4);
            }
            get
            {
                return _totalRealBankFee;
            }
        }
        private decimal _totalWithdrawFee;
        public decimal totalWithdrawFee {
            set
            {
                _totalWithdrawFee = value;// Math.Round(value / 100, 4);
            }
            get
            {
                return _totalWithdrawFee;
            }
        }
        private decimal _fee;
        public decimal fee {
            set
            {
                _fee = value;// Math.Round(value / 100, 4);
            }
            get
            {
                return _fee;
            }
        }

        private decimal _promo;
        public decimal promo {
            set
            {
                _promo = value;// Math.Round(value / 100, 4);
            }
            get
            {
                return _promo;
            }
        }
        private decimal _bonus;
        public decimal bonus {
            set
            {
                _bonus = value;// Math.Round(value / 100, 4);
            }
            get
            {
                return _bonus;
            }
        }
        private decimal _turnover;
        public decimal turnover {
            set
            {
                _turnover = value;// Math.Round(value / 100, 4);
            }
            get
            {
                return _turnover;
            }
        }

        public string wd
        {
            get
            {
                return (depositAmount - withdrawAmount).ToString("F2");
            }
        }
       
    }

    /// <summary>
    /// 代理财务数据
    /// 缩小100倍输出 fb , jdb
    /// </summary>
    public class FinanceReportBase100Intl : GameReportBaseIntl {
        private string _account;
        /// <summary>
        /// 重写上层
        /// </summary>
        public string account {
            set {
                playerAccount = value;
                if (!string.IsNullOrEmpty(value) && value.Count() > 5) {
                    _account = string.Format(@"{0}*", value.Substring(0, 5));
                }
                else {
                    _account = value;
                }
            }
            get { return _account; }
        }

        public string playerAccount {
            set; get;
        }


        /// <summary>
        /// 当前级别名称
        /// </summary>
        public string levels { set; get; }

        private string _createtime;
        public string createtime {
            set {
                if (!string.IsNullOrEmpty(value)) {
                    _createtime = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                }
                else { _createtime = ""; }
            }
            get { return _createtime; }
        }

        public string _lastLogin;
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public string lastLogin {
            set {
                if (!string.IsNullOrEmpty(value))
                    _lastLogin = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                else { _lastLogin = ""; }
            }
            get { return _lastLogin; }
        }

        private decimal _depositAmount;
        public decimal depositAmount {
            set {
                _depositAmount = value;// Math.Round(value / 100, 4);
            }
            get {
                return _depositAmount;
            }
        }
        public int depositTimes { set; get; }

        private decimal _withdrawAmount;
        public decimal withdrawAmount {
            set {
                _withdrawAmount = value;// Math.Round(value / 100, 4);
            }
            get {
                return _withdrawAmount;
            }
        }
        public int withdrawTimes { set; get; }

        private decimal _totalDepositFee;
        public decimal totalDepositFee {
            set {
                _totalDepositFee = value;// Math.Round(value / 100, 4);
            }
            get {
                return _totalDepositFee;
            }
        }
        private decimal _totalRealBankFee;
        public decimal totalRealBankFee {
            set {
                _totalRealBankFee = value;// Math.Round(value / 100, 4);
            }
            get {
                return _totalRealBankFee;
            }
        }
        private decimal _totalWithdrawFee;
        public decimal totalWithdrawFee {
            set {
                _totalWithdrawFee = value;// Math.Round(value / 100, 4);
            }
            get {
                return _totalWithdrawFee;
            }
        }
        private decimal _fee;
        public decimal fee {
            set {
                _fee = value;// Math.Round(value / 100, 4);
            }
            get {
                return _fee;
            }
        }

        private decimal _promo;
        public decimal promo {
            set {
                _promo = value;// Math.Round(value / 100, 4);
            }
            get {
                return _promo;
            }
        }
        private decimal _bonus;
        public decimal bonus {
            set {
                _bonus = value;// Math.Round(value / 100, 4);
            }
            get {
                return _bonus;
            }
        }
        private decimal _turnover;
        public decimal turnover {
            set {
                _turnover = value;// Math.Round(value / 100, 4);
            }
            get {
                return _turnover;
            }
        }

        public string wd {
            get {
                return (depositAmount - withdrawAmount).ToString("F2");
            }
        }

    }
}
