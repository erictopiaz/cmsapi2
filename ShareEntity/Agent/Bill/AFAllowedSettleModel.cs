using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.Agent.Bill
{
    /// <summary>
    /// 合营和对应的活跃会员数
    /// </summary>
    public class AFAllowedSettleModel
    {
        private string _affaccount;
        /// <summary>
        /// 代理账号
        /// </summary>
        public string affaccount
        {
            set { _affaccount = value.ToLower(); }
            get { return _affaccount; }
        }
        /// <summary>
        /// 活跃会员数
        /// </summary>
        public int cnt { set; get; }
    }
}
