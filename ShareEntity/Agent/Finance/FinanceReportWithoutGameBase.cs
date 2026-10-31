using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.Agent.Finance
{
    /// <summary>
    /// 代理财务数据
    /// 20250205  bti
    /// </summary>
    public class FinanceReportWithoutGameBase
    {
        /// <summary>
        /// 存款金额
        /// </summary>
        public decimal depositAmount { set; get; }
        public int depositTimes { set; get; }
        /// <summary>
        /// 提款金额
        /// </summary>
        public decimal withdrawAmount { set; get; }
        public int withdrawTimes { set; get; }

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
}
