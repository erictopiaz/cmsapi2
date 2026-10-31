//using CMS.Data.ClientModel;
using ShareEntity.ClientModel;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.agent
{
    public class updateAgentBaseInfoInputModel : IPInputModel
    {
        [CharAndDigital(12, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "sys_account_format_error")]
        public string adminAccount { set; get; }
        [CharAndDigitalAndUnderScore(25, MinimumLength = 6, IsAllowedEmpty = false, ErrorMessage = "agent_account_format_error")]
        public string account { set; get; }

        //[CharAndDigital(12, MinimumLength = 5, IsAllowedEmpty = true, ErrorMessage = "adcode_format_error")]
        //public string adcode { set; get; }

        [UrlValidate(50, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "adlink_format_error")]
        public string adlink { set; get; }

        //[UrlValidate(50, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "url_format_error")]
        //public string link1 { set; get; }
        //[UrlValidate(50, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "url_format_error")]
        //public string link2 { set; get; }
        //[UrlValidate(50, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "url_format_error")]
        //public string link3 { set; get; }

        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "remarks_format_error")]
        public string remarks { set; get; }
        public int verify { set; get; }
        public int status { set; get; }

        [DigitalValidate(13, IsAllowedEmpty = true, MinimumLength = 5, ErrorMessage = "qq_format_error")]
        public string qq { set; get; }

        [EmailValidate(60, MinimumLength = 6, IsAllowedEmpty = true, ErrorMessage = "email_format_error")]
        public string email { set; get; }

        [PhoneDigital(IsAllowedEmpty = true, ErrorMessage = "phone_format_error")]
        public string telephone { set; get; }
        public int allowCreateAgent { set; get; }
        [ChineseAndChar(12, 0, IsAllowedEmpty = false, ErrorMessage = "realname_format_error")]
        public string realname { set; get; }
        public int sex { set; get; } = -1;
        [DateTimeValidate(19, false, MinimumLength = 10, ErrorMessage = "birthday_format_error")]
        public string birthday { set; get; }
    }



    public class RemoveUnSettleAmountModel : IPInputModel
    {
        [CharAndDigital(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "sys_account_format_error")]
        public string adminAccount { set; get; }
        [CharAndDigitalAndUnderScore(16, MinimumLength = 6, IsAllowedEmpty = false, ErrorMessage = "agent_account_format_error")]
        public string account { set; get; }

    }
}
