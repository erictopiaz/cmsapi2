using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.agent
{
    /// <summary>
    /// 合营绑定 推广链接代码
    /// </summary>
    public class AffBindSpreadUrlInputModel
    {
        /// <summary>
        /// 合营推广地址
        /// </summary>
        [UrlValidate(150, MinimumLength = 10, IsAllowedEmpty = false, ErrorMessage = "referrer rule error")]
        public string spreadUrl { get; set; }
        
        /// <summary>
        /// updateType0：解绑, 1: 绑定
        /// </summary>
        public int updateType { get; set; }

        /// <summary>
        /// 请求的IP 地址, 可空
        /// </summary>
        [IpValidate(50, MinimumLength = 8, IsAllowedEmpty = true, ErrorMessage = "IP地址有误")]
        public string ipaddress { set; get; }
    }
}
