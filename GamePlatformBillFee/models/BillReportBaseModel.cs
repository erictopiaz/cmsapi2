using System;
using System.Collections.Generic;
using System.Text;

namespace GamePlatformBillFee.models
{
    public class BillReportBaseModel
    {
        private string _account;
        public string account
        {
            set { _account = value.ToLower(); }
            get { return _account; }
        }
        public int agentLevel { set; get; }
        public string parentAccount { set; get; }
        public decimal totalBonus { set; get; }
        public decimal totalFee { set; get; }
        public decimal totalTurnover { set; get; }
        public decimal totalBet { set; get; }
        public decimal totalWinlost { set; get; }
        public decimal totalDeposit_fee { set; get; }
        public decimal totalWithdraw_fee { set; get; }
        public decimal totalDepositAmount { set; get; }
        public decimal totalWithdrawAmount { set; get; }

        /// <summary>
        /// 历史未达到结算条件的金额
        /// </summary>
        public decimal unSettleAmount { set; get; }
        /// <summary>
        /// 结算比例列表
        /// </summary>
        public string billP { set; get; }
    }
}
