//using CMS.Data.ClientModel;
using ShareEntity.ClientModel;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.agent
{
    public class VerifyAgentInputModel : IPInputModel
    {
        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = false, ErrorMessage = "account_format_error")]
        public string account { set; get; }

        [CharAndDigital(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "sys_account_format_error")]
        public string adminAccount { set; get; }
    }
}
