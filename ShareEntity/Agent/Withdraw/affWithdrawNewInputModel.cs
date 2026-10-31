using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.Agent.Withdraw
{
    public class affWithdrawNewInputModel
    {
        /// <summary>
        /// 代理账号
        /// </summary>
        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = false, ErrorMessage = "agent_account_format_error")]
        public string account { get; set; }
        /// <summary>
        /// 代理密码
        /// </summary>
        [CharAndDigital(20, MinimumLength = 6, IsAllowedEmpty = false, ErrorMessage = "agent_password_format_error")]
        public string password { get; set; }

        /// <summary>
        /// 代理银行卡号
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true)]
        public string cardno { get; set; }
        /// <summary>
        /// 代理取款金额
        /// </summary>
        public decimal? amount { get; set; }

        /// <summary>
        /// 代理取款IP
        /// </summary>
        [IpValidate(19, MinimumLength = 7, IsAllowedEmpty = true, ErrorMessage = "ip_formart_error")]
        public string requestip { get; set; }

        [SqlInjectAttibute(IsAllowedEmpty = true)]
        public string requestaddress { get; set; }
    }
}
