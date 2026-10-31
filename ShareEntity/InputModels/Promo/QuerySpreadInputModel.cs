//using CMS.Data.PageModel;
using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Promo
{
    public class QuerySpreadInputModel : PageBaseModel
    {
        [CharAndDigital(12, MinimumLength = 6, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string affaccount { set; get; }

        public string marcode { set; get; }

        //[CharAndDigital(12, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        //public string adminAccount { set; get; }

        public int status { set; get; } = -1;

        public int currencyType { set; get; } = 156;
        public int walletType { set; get; } = -1;
        public string merchantAccount { set; get; } = "-1";
    }

    /// <summary>
    /// 推荐结算记录查询
    /// </summary>
    public class QuerySpreadBilledInputModel : PageBaseModel
    {
        
        public int status { set; get; } = -1;
        public long startTime { set; get; } = -1;
        public long endTime { set; get; } = -1;

        /// <summary>
        /// 结算金额排序
        /// </summary>
        public int billSort { set; get; } = -1;
        public int currencyType { set; get; } = 156;
        public int walletType { set; get; } = -1;
        public string merchantAccount { set; get; } = "-1";
    }


    /// <summary>
    /// 推荐结算记录查询 INTL
    /// </summary>
    public class QuerySpreadBilledIntlInputModel : PageBaseModel {

        public int status { set; get; } = -1;
        public long startTime { set; get; } = -1;
        public long endTime { set; get; } = -1;

        /// <summary>
        /// 查询的数据类型，0：自身，1：团队
        /// </summary>
        public int dataType { set; get; } = 0;
        public int currencyType { set; get; } = 156;

        [CharAndDigitalAndUnderScore(20, MinimumLength = 3, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string marcode { set; get; }

        public int walletType { set; get; } = -1;
        public string merchantAccount { set; get; } = "-1";
    }

    public class CacelSpreadBilledInputModel : PageBaseModel
    {
        public int id { set; get; }

        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "reason_format_error")]
        public string cancelreason { set; get; }
    }


    public class PromoSpreadMonthBilled
    {
        public long id { set; get; }
        public long financegameday { set; get; }
        public string account { get; set; }

        /// <summary>
        /// 实际结算金额 每月上限888
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// 结算金额 6月结算开始有值,每月上限888(结算到amount)
        /// </summary>
        public decimal spreadSettleAmount { set; get; }
        public string misc { set; get; }
        public string affaccount { set; get; }
    }

}
