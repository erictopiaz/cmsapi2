using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.agent
{
    public class BaseAgentModel
    {
        /// <summary>
        /// 代理账号
        /// </summary>
        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = false, ErrorMessage = "agent_account_format_error")]
        public string account { get; set; }

        [IpValidate(50, MinimumLength = 7, IsAllowedEmpty = true, ErrorMessage = "ip_formart_error")]
        public string requestip { get; set; }

        [SqlInjectAttibute(IsAllowedEmpty = true)]
        public string requestaddress { get; set; }

        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "admin_account_format_error")]
        public string adminAccount { get; set; }
    }
}
