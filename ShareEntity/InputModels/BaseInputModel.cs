using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels
{
    
    public class BaseInputModel
    {
        /// <summary>
        /// 账号
        /// </summary>
        [CharAndDigitalAndUnderScore(20, MinimumLength = 5, IsAllowedEmpty = true, ErrorMessage = "account error")]
        public string account { set; get; }

        /// <summary>
        /// 代理账号
        /// </summary>
        [CharAndDigitalAndUnderScore(16, MinimumLength = 6, IsAllowedEmpty = true, ErrorMessage = "agent account error")]
        public string affaccount { set; get; }

        /// <summary>
        /// 请求的IP 地址, 可空
        /// </summary>
        [IpValidate(50, MinimumLength = 8, IsAllowedEmpty = true, ErrorMessage = "ip error")]
        public string ipaddress { set; get; }
        /// <summary>
        /// ip 所在位置
        /// </summary>
        [ChineseAndChar(50, 3, MinimumLength = 2, IsAllowedEmpty = true, ErrorMessage = "mac_format_error")]
        public string macaddress { set; get; }

        /// <summary>
        /// 1:android，2：ios,   3:h5  , 4: pc , 5: 体育ios,6: 体育android
        /// </summary>
        public int device { set; get; } = 0;
        /// <summary>
        /// 货币代码
        /// </summary>
        public int currencyType { set; get; } = 156;
        /// <summary>
        /// 品牌名 2025-05-31 添加
        /// </summary>
        public string merchantAccount { set; get; } = "";
        /// <summary>
        /// 钱包类型 2025-05-31 添加
        /// </summary>
        public int walletType { set; get; } = 1;


    }

    

}
