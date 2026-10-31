using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models {






    [Table("ofDeposit")]
    public class ofDeposit {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string currency { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        [StringLength(50)]
        public string realname { get; set; }

        public decimal amount { get; set; }

        public decimal fee { get; set; }
        public decimal rebate { get; set; }
        public decimal feerate { get; set; }
        public decimal feemax { get; set; }

        public int? deposittype { get; set; }

        [StringLength(50)]
        public string depositmethod { get; set; }

        [StringLength(50)]
        public string additional { get; set; }

        [StringLength(50)]
        public string receiveperson { get; set; }

        [StringLength(50)]
        public string receivecardno { get; set; }

        public DateTime? deposittime { get; set; }

        public DateTime requesttime { get; set; }

        [StringLength(150)]
        public string bankname { get; set; }

        public DateTime? reviewtime { get; set; }

        [StringLength(50)]
        public string reviewperson { get; set; }

        [StringLength(50)]
        public string merchantno { get; set; }

        [StringLength(50)]
        public string bankorderno { get; set; }

        public DateTime? orderendtime { get; set; }

        public int status { get; set; }

        [StringLength(150)]
        public string referrer { get; set; }

        [StringLength(150)]
        public string referrerreg { get; set; }

        [StringLength(150)]
        public string ipaddress { get; set; }

        [StringLength(250)]
        public string macaddress { get; set; }

        [StringLength(250)]
        public string supplement { get; set; }

        [StringLength(150)]
        public string referenceguid { get; set; }

        public long payplatform { get; set; }

        public int alert { get; set; }

        [StringLength(50)]
        public string affaccount { get; set; }
        [StringLength(50)]
        public string saler { get; set; }

        public int joinbonus { get; set; }

        public decimal bonusamount { get; set; }

        public int bonusbet { get; set; }
        public int bonuspromocode { get; set; }
        public string misc { get; set; }

        public int currencycode { get; set; }
        public int alreadyResizeTimes { get; set; }

    }

    public class ofDeopsitEx : ofDeposit {
        public long uid { get; set; }
        public decimal self_fee { get; set; }
        public decimal self_feerate { get; set; }
        public string t_misc { get; set; }
        public int paycategory { get; set; }

        /// <summary>
        /// 支付渠道名
        /// </summary>
        public string payplatformname { get; set; }
        public string merchantkey { get; set; }
        public string callbackip { get; set; } = "";
        public string merchantid { get; set; }
        /// <summary>
        /// 会员注册设备
        /// </summary>
        public int regdevice { set; get; }

        public int currencyType { get; set; }
        public int depositnumber { get; set; }

    }

    /// <summary>
    /// 存送优惠存款比例限制输出数据
    /// </summary>
    public partial class DepositPromoLimitModel {
        public decimal promoDepositTotal { get; set; } = 0;
        public decimal totoalDeposit { get; set; } = 0;
        /// <summary>
        /// 申请存款优的存款金额占比总存款比例
        /// </summary>
        public decimal p { get; set; } = 0;
        public string pDesc { get; set; } = "";
        /// <summary>
        /// 最高   存款优惠的存款金额点比总存款
        /// </summary>
        public decimal pMax { get; set; } = 0;
        public string pMaxDesc { get; set; } = "";
    }
}