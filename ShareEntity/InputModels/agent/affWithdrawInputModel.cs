using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.agent
{
    /// <summary>
    /// 代理提款输入
    /// </summary>
    public class affWithdrawInputModel
    {
        /// <summary>
        /// 代理账号
        /// </summary>
        [CharAndDigital(12, MinimumLength = 6, IsAllowedEmpty = false, ErrorMessage = "agent_account_format_error")]
        public string account { get; set; }
        /// <summary>
        /// 代理密码
        /// </summary>
        [CharAndDigital(12, MinimumLength = 6, IsAllowedEmpty = false, ErrorMessage = "agent_password_format_error")]
        public string password { get; set; }
        /// <summary>
        /// 代理姓名
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true)]
        public string realname { get; set; }

        /// <summary>
        /// 代理银行卡号
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true)]
        public string cardno { get; set; }
        [SqlInjectAttibute(IsAllowedEmpty = true)]
        public string cardbank { get; set; }
        /// <summary>
        /// 代理开户行地址
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true)]
        public string cardaddress { get; set; }
        /// <summary>
        /// 代理取款金额
        /// </summary>
        public decimal? amount { get; set; }
        /// <summary>
        /// 代理取款IP
        /// </summary>
        [IpValidate(20, MinimumLength = 8, IsAllowedEmpty = true, ErrorMessage = "ip_format_error")]
        public string requestip { get; set; }

        [SqlInjectAttibute(IsAllowedEmpty = true)]
        public string requestaddress { get; set; }

        public decimal rate { get; set; } = 1;
        /// <summary>
        /// 提款货币类型： 0：rmb ,1:usdt-trc20
        /// 默认为0
        /// </summary>
        public int bankType { set; get; } = 0;
    }



    
}
