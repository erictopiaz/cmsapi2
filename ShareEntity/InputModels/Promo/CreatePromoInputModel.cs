using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Promo
{
    /// <summary>
    /// 新增优惠或红利
    /// </summary>
    public class CreatePromoInputModel :AccountModel
    {
        /// <summary>
        /// 0:红利
        /// 1：优惠
        /// </summary>
        public int bonusPromoType { set; get; }
        /// <summary>
        /// 优惠代码
        /// </summary>
        public int bonusPromoCode { set; get; } = 0;
        /// <summary>
        /// 游戏平台
        /// </summary>
        public int gametype { set; get; }
        /// <summary>
        /// 存款金额
        /// </summary>
        public decimal amount { set; get; }
        /// <summary>
        /// 赠送金额
        /// </summary>
        public decimal reward { set; get; }
        /// <summary>
        /// 投注倍数
        /// </summary>
        public int bet { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty =true, ErrorMessage = "remark_format_error")]
        public string remarks { set; get; }
        /// <summary>
        /// 0: 体育
        /// 1：真人
        /// 2：彩票
        /// 3：电竞
        /// 4：电子
        /// 5：其它  
        /// </summary>
        public int gamePercentType { set; get; }

        /// <summary>
        /// 批量新增时使用
        /// </summary>
        public string content { set; get; }

        /// <summary>
        /// 货币代码
        /// </summary>
        public int currencycode { set; get; } = 156;
        /// <summary>
        /// 品牌
        /// </summary>
        public string merchantAccount { set; get; } = "Awc";
       
    }

    /// <summary>
    /// 福利中心 新增优惠或红利
    /// </summary>
    public class CreateSelfPromoInputModel : AccountModel
    {
        /// <summary>
        /// 0:红利
        /// 1：优惠
        /// </summary>
        public int bonusPromoType { set; get; }
        public int bonusPromoCode { set; get; } = 0;
        /// <summary>
        /// 游戏平台
        /// </summary>
        public int gametype { set; get; }
        /// <summary>
        /// 存款金额
        /// </summary>
        public decimal amount { set; get; }
        /// <summary>
        /// 赠送金额
        /// </summary>
        public decimal reward { set; get; }
        /// <summary>
        /// 投注倍数
        /// </summary>
        public int bet { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "remark_format_error")]
        public string remarks { set; get; }
        /// <summary>
        /// 0: 体育
        /// 1：真人
        /// 2：彩票
        /// 3：电竞
        /// 4：电子
        /// 5：其它  
        /// </summary>
        public int gamePercentType { set; get; }

        /// <summary>
        /// 批量新增时使用
        /// </summary>
        public string content { set; get; }

        [DateTimeValidate(19, true, IsAllowedEmpty = false, ErrorMessage = "time_format_error")]
        public string startTime { set; get; }

        [DateTimeValidate(19, true, IsAllowedEmpty = false, ErrorMessage = "time_format_error")]
        public string endTime { set; get; }

        /// <summary>
        /// 货币代码
        /// </summary>
        public int currencycode { set; get; } = 156;
        /// <summary>
        /// 品牌
        /// </summary>
        public string merchantAccount { set; get; } = "Awc";

    }

    public class UpdatePromoInputModel : CreatePromoInputModel
    {
        
        public int tid { set; get; }

        //[ChineseAndCharMidScore(35, MinimumLength = 16, IsAllowedEmpty = false, ErrorMessage = "rid_format_error")]
        //public string gid { set; get; }
    }

    public class UpdateSelfPromoInputModel : CreateSelfPromoInputModel
    {

        public int tid { set; get; }

    }

    public class BonusData
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string account { set; get; }
        /// <summary>
        /// 存款金额
        /// </summary>
        public decimal amount { set; get; }
        /// <summary>
        /// 赠送金额 
        /// </summary>
        public decimal reward { set; get; }
        public decimal bets { set; get; }
    }
}
