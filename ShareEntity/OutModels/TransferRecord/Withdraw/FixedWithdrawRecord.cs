using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.TransferRecord.Withdraw
{
    
    /// <summary>
    /// 极速提款记录
    /// </summary>
    public class FixedWithdrawRecord : RecordBase
    {
        /// <summary>
        /// 提款单号
        /// </summary>
        public string worderno { set; get; }

        /// <summary>
        /// 提款人名
        /// </summary>
        public string realname { set; get; }

        /// <summary>
        /// 提款银行
        /// </summary>
        public string cardbank { set; get; }
        private string _cardno;
        /// <summary>
        /// 卡号
        /// </summary>
        public string cardno
        {
            set
            {
                _cardno = string.IsNullOrEmpty(value) ? "" : string.Format(@"********{0}", value.Substring(value.Length - 5, 5));
            }
            get { return _cardno; }
        }

        private int _status;
        public int status
        {
            set
            {
                statusDesc = value == 0 ? "等待处理" : value == 1 ? "等待支付" : value == 2 ? "支付完成" : "失败";
                _status = value;
            }
            get { return _status; }
        }
        /// <summary>
        /// 失败备注
        /// </summary>
        public string cancelreason { set; get; } = "";

        private decimal _reward;
        /// <summary>
        /// 确认到账红利金额
        /// </summary>
        public decimal? reward
        {
            set
            {
                _reward = value == null ? 0 : (decimal)value;
            }
            get { return _reward; }
        }
        /// <summary>
        /// 是否转人工  0：否，1：是
        /// </summary>
        public int isTransferToManual { set; get; } = 0;

        /// <summary>
        /// 是否确认过，0：无，1：是
        /// </summary>
        public int conformedStatus { set; get; }

        /// <summary>
        /// 3:定额网银 或 4：定额支付宝
        /// </summary>
        public string widDesc { set; get; }
        private int _wid;
        /// <summary>
        /// 3:定额网银，4：定额支付宝
        /// </summary>
        public int wid
        {
            set
            {
                _wid = value;
                widDesc = value == 3 ? "定额网银" : "定额支付宝";
            }
            get { return _wid; }
        }
        public string reward_p { set; get; } = "0.2%";
    }
}
