using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.agent
{
    /// <summary>
    /// 推广地址池
    /// </summary>
    public class AffSpreadLinkModel
    {
        /// <summary>
        /// 推广地址
        /// </summary>
        public string url { set; get; }
        /// <summary>
        /// 货币
        /// </summary>
        public int currencyType { set; get; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string mer { set; get; }
    }

}
