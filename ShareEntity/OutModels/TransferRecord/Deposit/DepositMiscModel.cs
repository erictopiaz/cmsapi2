using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.TransferRecord.Deposit
{
    /// <summary>
    /// 充值虚拟币金额和对应的汇率
    /// </summary>
    public class DepositMiscModel
    {
        /// <summary>
        /// 充值虚拟币金额
        /// </summary>
        public string cryptoAmount { set; get; } = "0.00";
        /// <summary>
        /// 汇率
        /// </summary>
        public string cryptoRate { set; get; } = "";

    }
}
