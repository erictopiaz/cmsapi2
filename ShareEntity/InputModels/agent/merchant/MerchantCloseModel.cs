using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.agent.merchant
{
    /// <summary>
    /// 禁用商户输入数据
    /// </summary>
    public class MerchantCloseModel: BaseAgentModel
    {
        /// <summary>
        /// 要禁用的商户ids 豆号分开
        /// </summary>
        public string disableMids { set; get; }
    }
    /// <summary>
    /// 要启用的商户ids 豆号分开
    /// </summary>
    public class MerchantOpenModel : BaseAgentModel
    {
        /// <summary>
        /// 要启用的商户ids 豆号分开
        /// </summary>
        public List<int> openMids { set; get; }
    }
}
