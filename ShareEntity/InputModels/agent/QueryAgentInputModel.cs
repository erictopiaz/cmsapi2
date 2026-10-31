using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.agent
{
    public class QueryAgentInputModel : PageBaseModel
    {
        /// <summary>
        /// 代理账号状态
        /// </summary>
        public int status { set; get; } = -1;
        public int parentId { set; get; } = -1;
        public int verify { set; get; } = -1;

        [EmailValidate(60, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "email_format_error")]
        public string email { get; set; }

        [ChineseAndChar(10, 0, MinimumLength = 2, IsAllowedEmpty = true, ErrorMessage = "name_format_error")]
        public string realname { get; set; }

        [DigitalValidate(15, MinimumLength = 5, IsAllowedEmpty = true, ErrorMessage = "qq_format_error")]
        public string qq { get; set; }

        [UrlValidate(50, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "url_format_error")]
        public string registerUrl { get; set; }
        [UrlValidate(50, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "adlink_format_error")]
        public string adlink { get; set; }

        [CharAndDigital(10, MinimumLength = 3, IsAllowedEmpty = true, ErrorMessage = "code_format_error")]
        public string adcode { get; set; }

        [PhoneDigital(IsAllowedEmpty =true,ErrorMessage ="phone_format_error")]
        public string telephone { set; get; }

        public int currencyType { get; set; } = 156;

        public int walletType { set; get; } = -1;
        public string merchantAccount { set; get; } = "-1";
    }
}
