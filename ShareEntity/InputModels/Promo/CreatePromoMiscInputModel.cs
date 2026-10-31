using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Promo
{
    public class CreatePromoMiscInputModel : AccountModel
    {
        public int type { set; get; } = -1;
        /// 0新增
        /// <summary>
        /// 1扣除
        /// </summary>
        public int status { set; get; } = -1;
        public decimal points { set; get; } = 0;

        [ChineseAndChar(12,3, MinimumLength = 2, IsAllowedEmpty = false, ErrorMessage = "content_format_error")]
        public string content { set; get; }

        public int rowstatus { set; get; } = 0;

        public string createtime { set; get; }
        public string createperson { set; get; }

        [ChineseAndChar(25, 3, MinimumLength = 2, IsAllowedEmpty = true, ErrorMessage = "content_format_error")]
        public string remarks { set; get; }

        /// <summary>
        /// 会员的品牌
        /// </summary>
       // [CharAndDigital(6, MinimumLength = 0, IsAllowedEmpty = true, ErrorMessage = "merchant_account_error")]
        public string merchantAccount { set; get; } = "Awc";
        public int currencycode { set; get; } = 156;

    }
}
