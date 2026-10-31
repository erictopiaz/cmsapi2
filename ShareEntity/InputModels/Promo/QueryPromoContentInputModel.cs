//using CMS.Data.PageModel;
using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Promo
{
    public class QueryPromoContentInputModel : PageBaseModel
    {
        [CharAndDigitalAndUnderScore(12, MinimumLength = 6, IsAllowedEmpty = true, ErrorMessage = "agent_account_format_error")]
        public string affaccount { set; get; }


        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "remark_format_error")]
        public string remark { set; get; }

        //[CharAndDigital(12, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        //public string adminAccount { set; get; }

        [CharAndDigitalAndUnderScore(12, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "sys_account_format_error")]
        public string createcustomer { set; get; }

        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string startTime { set; get; }

        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string endTime { set; get; }
        public int status { set; get; } = -1;
        public int promotype { set; get; } = -1;
        public int promocatalog { set; get; } = -1;
        /// <summary>
        /// 是否为存款优惠
        /// </summary>
        public int isDepositListPromo { set; get; } = -1;
        public int currencyType { set; get; } = 156;

        public int walletType { set; get; } = -1;
        public string merchantAccount { set; get; } = "-1";

    }
}
