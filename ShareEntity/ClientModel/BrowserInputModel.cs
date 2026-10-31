using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.ClientModel
{
    /// <summary>
    /// 客户端浏览器信息
    /// </summary>
    public class BrowserInputModel
    {
        /// <summary>
        /// 客人所用的操作系统
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "client_os_fromart_error")]
        public virtual string Os { set; get; }
        /// <summary>
        /// 客人当前浏览器信息
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "client_browser_fromart_error")]
        public virtual string Browser { set; get; }
        /// <summary>
        /// 当前客人的使用的链接 
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "client_url_fromart_error")]
        public virtual string Url { set; get; }
        /// <summary>
        /// 客人来源网址
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "client_referrer_fromart_error")]
        public virtual string Referrer { set; get; }
    }
}
