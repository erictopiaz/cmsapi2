using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.Agent.Finance
{
    /// <summary>
    /// 新加： pp iaes  20260711
    /// 20250205  bti
    /// </summary>
    public class AgentPlayersReportFinanceOutModel
    {
        private string _account;
        /// <summary>
        /// 会员账号
        /// </summary>
        public string account
        {
            set
            {
                if (value.ToLower().StartsWith("boaa")
                    || value.ToLower().StartsWith("bocc"))
                {
                    _account = value.Remove(0, 4);
                }
                else
                {
                    _account = value;
                }
            }
            get { return _account; }
        }

        private string _affaccount;
        /// <summary>
        /// 上级代理
        /// </summary>
        public string affaccount
        {
            set
            {
                if (value.ToLower().StartsWith("boaa")
                    || value.ToLower().StartsWith("bocc"))
                {
                    _affaccount = value.Remove(0, 4);
                }
                else
                {
                    _affaccount = value;
                }
            }
            get { return _affaccount; }
        }

        public long financeDay { set; get; }
        /// <summary>
        /// 投注
        /// </summary>
        public decimal bets { set; get; } = 0;
        /// <summary>
        /// 输赢
        /// </summary>
        public decimal winlost { set; get; } = 0;
        /// <summary>
        /// vip 名称
        /// </summary>
        public string levels { set; get; }
        public int level { set; get; }
        public int anomaly { set; get; }
        public int riskTypesAready { set; get; }

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
        /// <summary>
        /// 注册时间
        /// </summary>
        public string createtime
        {
            set
            {
                _createtime = string.IsNullOrEmpty(value) ? "" : Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            get { return _createtime; }
        }
        /// <summary>
        /// 最后登录时间，暂无
        /// </summary>
        public string lastLoginTime { set; get; } = "";
        /// <summary>
        /// 默认国籍
        /// </summary>
        public string citizenship { set; get; } = "中国";





        /// <summary>
        /// 存款金额
        /// </summary>
        public decimal depositAmount { set; get; } = 0;
        public int depositTimes { set; get; } = 0;
        /// <summary>
        /// 提款金额
        /// </summary>
        public decimal withdrawAmount { set; get; } = 0;
        public int withdrawTimes { set; get; } = 0;

        public decimal promo { set; get; } = 0;
        /// <summary>
        /// 红利
        /// </summary>
        public decimal bonus { set; get; } = 0;
        /// <summary>
        /// 存款反点
        /// </summary>
        public decimal rebate { set; get; } = 0;
        /// <summary>
        /// 反水
        /// </summary>
        public decimal turnover { set; get; } = 0;

        public string wd
        {
            get
            {
                return (depositAmount - withdrawAmount).ToString("F2");
            }
        }
        /// <summary>
        /// 手续费用
        /// </summary>
        public decimal fee { set; get; } = 0;
        /// <summary>
        /// 银行卡收款手续费用
        /// </summary>
        public decimal totalRealBankFee { set; get; } = 0;
        /// <summary>
        /// 三方存款手续费用 
        /// </summary>
        public decimal totalDepositFee { set; get; } = 0;
        /// <summary>
        /// 提款手续费用
        /// </summary>
        public decimal totalWithdrawFee { set; get; } = 0;


    }

}
