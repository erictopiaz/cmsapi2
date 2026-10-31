using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.Agent.Finance
{
    public class AgentReportQueryInputModel : PageBaseModel
    {
        public long startTime { set; get; } = -1;
        public long endTime { set; get; } = -1;


        /// <summary>
        /// 0: 当前顶级
        /// 1：下级所有代理
        /// 2: 下一级代理
        /// 3: 当前代理和当前代理所有子代理
        /// </summary>
        public int queryType { set; get; } = -1;
        public int parentId { set; get; } = 0;
        /// <summary>
        /// 结算记录金额排序
        /// </summary>
        public int amountSort { set; get; } = -1;


        #region 用来统计活跃客人
        /// <summary>
        /// 存款金额
        /// </summary>
        public decimal depositAmount { set; get; } = -1;
        /// <summary>
        /// 存款金额 max
        /// </summary>
        public decimal depositAmountMax { set; get; } = -1;
        /// <summary>
        /// 投注额
        /// </summary>
        public decimal totalBet { set; get; } = -1;
        /// <summary>
        /// 投注额
        /// </summary>
        public decimal totalBetMax { set; get; } = -1;

        #endregion

        /// <summary>
        /// 市场审核人员  2021-10-12 add by ching 
        /// </summary>
        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string verifyperson { set; get; }
        /// <summary>
        /// 默认为cny 
        /// </summary>
        public int currencyType { set; get; } = 156;
        public int walletType { set; get; } = -1;
        public string merchantAccount { set; get; } = "-1";
    }


}
