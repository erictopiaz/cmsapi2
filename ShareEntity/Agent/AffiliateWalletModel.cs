using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.Agent
{
    public class AffiliateWalletModel
    {
        /// <summary>
        /// 可用余额
        /// </summary>
        public decimal balance { get; set; }
        /// <summary>
        /// 累积金额
        /// </summary>
        public decimal unSettleAmount { get; set; }
    }
}
