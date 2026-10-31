using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Resize
{
    public class CreateResizeInputModel : AccountModel
    {
        [CharAndDigital(6, MinimumLength = 2, IsAllowedEmpty = true, ErrorMessage = "currency_format_error")]
        public string currency { set; get; } = "CNY";
        public decimal amount { set; get; }

        [CharAndDigital(12, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "sys_account_format_error")]
        public string createperson { set; get; } = "admin";

        [SqlInjectLenLimitAttibute(50,MinimumLength =2, IsAllowedEmpty = true, ErrorMessage = "remarks_format_error")]
        public string remarks { set; get; }
        /// <summary>
        /// 0:扣除， 1：新增，  2：冻结
        /// </summary>
        public int decreasetype { set; get; }

        [CharAndDigital(22, MinimumLength = 18, IsAllowedEmpty = true, ErrorMessage = "referenceguid_format_error")]
        public string referenceguid { set; get; }

        public string referrer { set; get; } = "";

        public int appendType { set; get; } = -1;

        [ChineseAndCharMidScore(50, MinimumLength = 20, IsAllowedEmpty = true, ErrorMessage = "orderNo_format_error")]
        public string orderNo { set; get; } = "";

        /// <summary>
        /// 账号类型
        /// 0:会员，1：代理
        /// </summary>
        public int accountType { set; get; } = 0;
        /// <summary>
        /// 调整金额类
        /// 0:可用余额
        /// 1：累积金额
        /// </summary>
        public int amountType { set; get; } = 0;
        /// <summary>
        /// 代理奖励 0， 默认为：-1
        /// </summary>
        public int bonusType { set; get; } = -1;
        /// <summary>
        /// 会员的品牌
        /// </summary>
        public string merchantAccount { set; get; } = "Awc";
        public int walletType { set; get; } = -1;
        public int currencycode { set; get; } = 156;

        public long r_f_uid { set; get; } = -1;

    }
}
