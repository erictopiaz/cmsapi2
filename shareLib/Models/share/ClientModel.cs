using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace Models.share
{
    public partial class ClientModel
    {
        /// <summary>
        /// 客人IP
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "请检查输入字符格式")]
        [IpValidate(18, MinimumLength = 7, IsAllowedEmpty = true, ErrorMessage = "ip要求7到18个字符")]
        public virtual string IpAddress { set; get; }
        /// <summary>
        /// 客人IP对应的地理位置
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "请检查输入字符格式")]
        public virtual string MacAddress { set; get; }
        /// <summary>
        /// 客人所用的操作系统
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "请检查输入字符格式")]
        public virtual string Os { set; get; }
        /// <summary>
        /// 客人当前浏览器信息
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "请检查输入字符格式")]
        public virtual string Browser { set; get; }
        /// <summary>
        /// 当前客人的使用的链接 
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "请检查输入字符格式")]
        [UrlValidate(60,IsAllowedEmpty =true,ErrorMessage ="当前的URL格式有误,请确认")]
        public virtual string Url { set; get; }
        /// <summary>
        /// 客人来源网址
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "请检查输入字符格式")]
        [UrlValidate(60, IsAllowedEmpty = true, ErrorMessage = "当前的URL格式有误,请确认")]
        public virtual string Referrer { set; get; }
    }
}
