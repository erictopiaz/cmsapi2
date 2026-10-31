//using CMS.Data.ClientModel;
using ShareEntity.ClientModel;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels
{
    public class AccountModel : IPInputModel
    {
        [CharAndDigitalAndUnderScore(22, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string account { set; get; } 

        [CharAndDigitalAndUnderScore(12, MinimumLength =4, IsAllowedEmpty = true, ErrorMessage = "sys_account_format_error")]
        public string adminAccount { set; get; }

        [IpValidate(20, MinimumLength = 9, IsAllowedEmpty = true, ErrorMessage = "ip_format_error")]
        public string AdminIpAddress { set; get; }

        [CharAndDigitalAndUnderScore(12, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "sys_parentAccount_format_error")]
        public string parentAccount { set; get; }
    }
}
