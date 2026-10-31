using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.ClientModel
{
    /// <summary>
    /// 客户端ip 信息
    /// </summary>
    public class IPInputModel
    {
        [IpValidate(19, MinimumLength = 7, IsAllowedEmpty = true, ErrorMessage = "ip_formart_error")]
        public string IpAddress { set; get; }
        /// <summary>
        /// 客人IP对应的地理位置
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "client_mac_formart_error")]
        public string MacAddress { set; get; }
    }

}
