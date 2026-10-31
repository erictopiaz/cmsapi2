
using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.agent
{
    public class QueryAgentBilledInputModel : PageBaseModel
    {
        /// <summary>
        /// 0: 人工结算
        /// 1：自动结算
        /// </summary>
        public int afpaymentlivetype { set; get; } = -1;
        public long startTime { set; get; } = -1;
        public long endTime { set; get; } = -1;
        public decimal proportion { set; get; } = -1;

        [ChineseAndChar(20,3, MinimumLength = 2, IsAllowedEmpty = true, ErrorMessage = "备注格式有误")]
        public string remark { set; get; }
        /// <summary>
        /// 金额金额的排序
        /// </summary>
        public int billSort { set; get; } = -1;
        /// <summary>
        /// 结算类型
        /// 0: 月结， 1：人头奖励， 2：保底， 3：节日红包
        /// </summary>
        public int settleType { set; get; }
        public int verifyStatus { set; get; } = -1;

        /// <summary>
        /// 市场审核人员账号
        /// </summary>
        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "审核账号格式有误")]
        public string verifyperson { set; get; }

        public int currencyType { set; get; } = 156;
        /// <summary>
        /// 注册时间， 0：注册时间不满6个月
        /// </summary>
        public int regTimeWhere { set; get; } = -1;
    }
}
