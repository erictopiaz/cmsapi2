//using CMS.Data.PageModel;                                                                                       
using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Bonus
{
    /// <summary>
    /// 当前红利优惠列表
    /// </summary>
    public class QueryBonusInputModel : PageBaseModel
    {
        [CharAndDigitalAndUnderScore(16, MinimumLength = 3, IsAllowedEmpty = true, ErrorMessage = "uid_format_error")]
        public string uid { set; get; }

        [MaxLength(30,ErrorMessage ="remark_length_gt_30")]
        public string remarks { set; get; }

        [CharAndDigitalAndUnderScore(12, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string affaccount { set; get; }

        [IpValidate(15, MinimumLength = 8, IsAllowedEmpty = true, ErrorMessage = "ip_format_error")]
        public string ipaddress { set; get; }

        public int status { set; get; } = -1;
        public int gametype { set; get; } = -1;
        /// <summary>
        /// 赠送金额类型
        /// 0：红利，1优惠
        /// </summary>
        public int bonusPromoType { set; get; } = -1;

        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string startTime { set; get; }

        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string endTime { set; get; }
        /// <summary>
        /// 货种类型
        /// </summary>
        public int currencyType { set; get; } = 156;
        public int walletType { set; get; } = -1;

        public string merchantAccount { set; get; } = "-1";
    }
}
