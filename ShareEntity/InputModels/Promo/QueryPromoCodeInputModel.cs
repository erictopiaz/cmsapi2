using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Promo
{
    /// <summary>
    /// 优惠代码查询输入
    /// </summary>
    public class QueryPromoCodeInputModel : PageBaseModel
    {
        [CharAndDigital(12, MinimumLength = 6, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string affaccount { set; get; }

        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "remark_format_error")]
        public string remark { set; get; }

        //[CharAndDigital(12, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        //public string adminAccount { set; get; }

        [CharAndDigital(15, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "promocode_format_error")]
        public string promocode { set; get; }

        public int status { set; get; } = -1;
        /// <summary>
        /// 游戏平台类型： 
        /// 0: 体育
        /// 1：真人
        /// 2：彩票
        /// 3：电竞
        /// 4：电子
        /// 5：其它  
        /// </summary>
        public int platformType { set; get; } = -1;
    }
}
