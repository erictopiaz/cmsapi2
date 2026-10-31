using ShareEntity.OutModels.TransferRecord.Deposit;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.TransferRecord.Withdraw
{
    
    /// <summary>
    /// 提款记录
    /// </summary>
    public class WithdrawRecord : RecordBase
    {
        /// <summary>
        /// 提款银行
        /// </summary>
        public string cardbank { set; get; }

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
        /// 提款单号
        /// </summary>
        public string withdrawno { set; get; } = "";

        /// <summary>
        /// 失败备注
        /// </summary>
        public string cancelreason { set; get; } = "";
        /// <summary>
        /// 提款数据集
        /// </summary>
        public string misc { set; get; }
        /// <summary>
        /// 如果不空串，显示当前汇率
        /// </summary>
        public string inrToUsdtRate { set; get; } = "";
        /// <summary>
        /// 兑换后的金额
        /// </summary>
        public string amountTo { set; get; } = "";

        /// <summary>
        /// 虚拟币金额和对应的汇率
        /// </summary>
        public DepositMiscModel cryptoModel { set; get; }
        /// <summary>
        /// 0或1  0：没有汇率，1：有汇率
        /// 默认为0 , 如果1，显示对应虚拟金额和对应的汇率
        /// </summary>
        public int isSrypto { set; get; } = 0;
        /// <summary>
        /// 当前 货币icon
        /// </summary>
        public string iconUrl { set; get; } = "";

        public int category { set; get; }
        public int bid { set; get; }
    }

}
