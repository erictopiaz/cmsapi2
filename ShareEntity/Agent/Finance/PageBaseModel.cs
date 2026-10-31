using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.Agent.Finance
{
    /// <summary>
    /// 分页基本元素
    /// </summary>
    public class PageBaseModel
    {
        [CharAndDigitalAndUnderScore(20, MinimumLength = 3, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string account { set; get; }

        [CharAndDigitalAndUnderScore(20, MinimumLength = 3, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string parentAccount { set; get; }

        [CharAndDigitalAndUnderScore(16, MinimumLength = 3, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string adminAccount { set; get; }

        [CharAndDigitalAndUnderScore(20, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "密码格式有误")]
        public string password { set; get; }

        /// <summary>
        /// 管理员ip
        /// </summary>
        [IpValidate(50, MinimumLength = 9, IsAllowedEmpty = true, ErrorMessage = "ip_format_error")]
        public string AdminIpAddress { set; get; }

        /// <summary>
        /// 客人的IP
        /// </summary>
        [IpValidate(50, MinimumLength = 9, IsAllowedEmpty = true, ErrorMessage = "ip_format_error")]
        public string UserIpAddress { set; get; }

        public int pageSize { set; get; }
        public int pageIndex { set; get; }
        /// <summary>
        /// 会员级别
        /// </summary>
        public int level { set; get; } = -1;
    }

    /// <summary>
    /// 分页基本元素
    /// </summary>
    public class PageBaseVModel
    {
        [CharAndDigitalAndUnderScore(20, MinimumLength = 3, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string account { set; get; }

        [CharAndDigitalAndUnderScore(20, MinimumLength = 3, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string parentAccount { set; get; }

        /// <summary>
        /// 客人的IP
        /// </summary>
        [IpValidate(50, MinimumLength = 9, IsAllowedEmpty = true, ErrorMessage = "ip_format_error")]
        public string ipAddress { set; get; }

        public int pageSize { set; get; }
        public int pageIndex { set; get; }
    }
}
