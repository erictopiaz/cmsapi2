using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.OfSetting
{
    /// <summary>
    /// app 优惠的跳转地址
    /// </summary>
    public class PromoJumpUrlModel
    {
        /// <summary>
        /// 旧地址
        /// </summary>
        /// <summary>
        /// 来源网址
        /// </summary>
        [UrlValidate(150, MinimumLength = 10, IsAllowedEmpty = false, ErrorMessage = "referrer rule error")]
        public string oldUrl {  get; set; }
        /// <summary>
        /// 新地址
        /// </summary>
        /// <summary>
        /// 来源网址
        /// </summary>
        [UrlValidate(150, MinimumLength = 10, IsAllowedEmpty = false, ErrorMessage = "referrer rule error")]
        public string newUrl { get; set; }
    }


    /// <summary>
    /// app api地址
    /// </summary>
    public class ApiUrlModel
    {
        /// <summary>
        /// 旧地址
        /// </summary>
        /// <summary>
        /// 来源网址
        /// </summary>
        public string oldUrl { get; set; } = "";
        /// <summary>
        /// 新地址
        /// </summary>
        /// <summary>
        /// 来源网址
        /// </summary>
        public string newUrl { get; set; } = "";
    }
}
