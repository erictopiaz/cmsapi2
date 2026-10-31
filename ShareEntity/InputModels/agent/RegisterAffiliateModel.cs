using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UtilsLib;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.agent
{
    /// <summary>
    /// 代理申请注册时，可提供的数据
    /// </summary>
    public class RegisterAffiliateModel
    {
        /// <summary>
        /// 币种
        /// </summary>
        [CharAndDigital(6, MinimumLength = 3, IsAllowedEmpty = true, ErrorMessage = "currency_format_error")]
        public string currency { get; set; } = "CNY";

        [CharAndDigitalAndUnderScore(12, MinimumLength = 6, IsAllowedEmpty = false, ErrorMessage = "account_format_error")]
        public string account { get; set; }

        [ChineseAndChar(10,0, MinimumLength = 2, IsAllowedEmpty = false, ErrorMessage = "name_format_error")]
        public string realname { get; set; }

        [CharAndDigital(12, MinimumLength = 6, IsAllowedEmpty = false, ErrorMessage = "password_format_error")]
        public string password { get; set; }

        public int? sex { get; set; } = 0;

        [DateTimeValidate(20,false,IsAllowedEmpty =false,ErrorMessage = "birthday_formart_error")]
        public string birthday { get; set; } = "1988-10-10";

        [EmailValidate(60,MinimumLength =10,IsAllowedEmpty =true,ErrorMessage ="email_format_error")]
        public string email { get; set; }

        [PhoneDigital(IsAllowedEmpty =true,ErrorMessage ="phone_format_error")]
        public string telephone { get; set; }

        [DigitalValidate(15, MinimumLength = 5, IsAllowedEmpty = true, ErrorMessage = "qq_format_error")]
        public string qq { get; set; }

        [UrlValidate(50, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "url_format_error")]
        public string website { get; set; }

        /// <summary>
        /// 推广码
        /// </summary>
        [CharAndDigital(10, MinimumLength = 5, IsAllowedEmpty = true, ErrorMessage = "code_format_error")]
        public string adcode { get; set; }

        [IpValidate(20,MinimumLength =10,IsAllowedEmpty =false,ErrorMessage ="ip_format_error")]
        public string ipaddress { get; set; }

        public string macaddress { get; set; }
        public string createperson { set; get; }
        public string host { set; get; }
        /// <summary>
        /// 注册地址的host
        /// </summary>
        private string _registerUrl;
        [UrlValidate(50, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "url_format_error")]
        public string registerUrl {
            set
            {
                host = Common.GetHostByUrl(value);
                _registerUrl = value;
            }
            get { return _registerUrl; }
        }
        /// <summary>
        /// http agent信息
        /// </summary>
        public string ha { set; get; }

    }
}
