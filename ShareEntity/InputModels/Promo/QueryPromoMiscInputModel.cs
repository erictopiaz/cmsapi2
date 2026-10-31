//using CMS.Data.PageModel;
using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Promo
{
    public class QueryPromoMiscInputModel : PageBaseModel
    {
        [CharAndDigital(12, MinimumLength = 6, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string affaccount { set; get; }

        //[CharAndDigital(12, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        //public string adminAccount { set; get; }

        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string startTime { set; get; }
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string endTime { set; get; }
        /// <summary>
        /// 0 = 每天签到
        ///6 = 连续几天的签到
        ///3	存款积分
        ///1	手机银行卡绑定
        ///5	手机验证积分,
        ///90   红包雨积分
        /// </summary>
        public int type { set; get; } = -1;

        public int status { set; get; } = -1;

        public int currencyType { set; get; } = 156;
        public int walletType { set; get; } = -1;
        public string merchantAccount { set; get; } = "-1";
    }
}
