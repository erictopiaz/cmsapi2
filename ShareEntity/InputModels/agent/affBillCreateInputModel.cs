using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.agent
{
    /// <summary>
    /// 管理员创建结算
    /// </summary>
    public class affBillCreateInputModel : AccountModel
    {
        public long billedTime { set; get; } = -1;
        public decimal amount { get; set; }
        public decimal totalSettleAmount { set; get; }
        public decimal subTotalAmount { set; get; } = 0;
        public decimal unSettleAmount { set; get; } = 0;
        public decimal unSettleAmountPre { set; get; } = 0;
        public decimal unSettleAmountAfter { set; get; } = 0;
        /// <summary>
        /// 是否达到到结算条件
        /// </summary>
        public int canSettle { set; get; } = 0;

        [ChineseAndChar(20,3,IsAllowedEmpty =true,ErrorMessage ="agent_bill_remark_format_error")]
        public string remark { get; set; }

        [CharAndDigital(12, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "sys_account_format_error")]
        public string createcustomer { get; set; } = "sysAdmin";

        /// <summary>
        /// 0: 人工结算
        /// 1：自动结算
        /// </summary>
        public int afpaymentlivetype { set; get; } = 0;

        public decimal grossamount { set; get; }
        public decimal platformfee { set; get; }
        /// <summary>
        /// 结算比例
        /// </summary>
        public decimal proportion { set; get; } = 0.0m;
        public decimal turnover { set; get; }
        public decimal promotion { set; get; }
        public decimal bonus { set; get; }
        public decimal depositfee { set; get; }
        public decimal withdrawfee { set; get; }
        /// <summary>
        /// 代理自身纯利
        /// </summary>
        public decimal profit { set; get; } = 0;
        /// <summary>
        /// 多账号代理自身纯利+分账号的纯利
        /// </summary>
        public decimal totalProfit { set; get; } = 0;
        /// <summary>
        /// 活跃会员数
        /// </summary>
        public int totalActivePlayer { set; get; }

        public DateTime createtime { set; get; }
        /// <summary>
        /// 结算类型
        /// 0: 月结， 1：人头奖励， 2：保底， 3：节日红包， 4：违规操作， 5：流水奖励
        /// </summary>
        public int settleType { set; get; }

        public int isMultiAccount { set; get; } = 0;
        /// <summary>
        /// 流水比例，结算时给到的。
        /// </summary>
        public decimal betBonus { set; get; } = 0;
    }

    public class affBillModifyInputModel : affBillCreateInputModel
    {
        [DigitalValidate(30, MinimumLength = 2, IsAllowedEmpty = false, ErrorMessage = "tid_format_error")]
        public string tid { set; get; }
    }
}
