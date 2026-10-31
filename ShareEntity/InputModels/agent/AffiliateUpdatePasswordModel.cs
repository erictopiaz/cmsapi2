using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.agent
{
    /// <summary>
    /// 代理修改密码数据模型
    /// </summary>
    public sealed class AffiliateUpdatePasswordModel : AffiliateBaseModel
    {

        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "sys_account_format_error")]
        public string adminAccount { set; get; }
        /// <summary>
        /// 新密码不可与旧密码相同
        /// </summary>
        /// <returns></returns>
        public bool CheckNewPassword()
        {
            if (string.Compare(password, oldPassword) != 0 )
                return false;
            else
                return true;
        }
      
    }
   
    /// <summary>
    /// 代理登录
    /// </summary>
    public sealed class AffiliateLoginModel : AffiliateBaseModel
    { 
       
    }

    public class AffiliateBaseModel
    {
        [CharAndDigitalAndUnderScore(16, MinimumLength = 6, IsAllowedEmpty = false, ErrorMessage = "agent_account_format_error")]
        public string account { set; get; }

        [CharAndDigital(20, MinimumLength = 6, IsAllowedEmpty = false, ErrorMessage = "agent_password_format_error")]
        public string password { set; get; }

        public string oldPassword { set; get; }
        [IpValidate(19, MinimumLength = 8, IsAllowedEmpty = true, ErrorMessage = "ip_format_error")]
        public string ipaddress { set; get; }
        [ChineseAndChar(15,3,MinimumLength =2, IsAllowedEmpty = true, ErrorMessage = "mac_format_error")]
        public string macaddress { set; get; }
       
    }


    /// <summary>
    /// 修改合营风控级别
    /// </summary>
    public sealed class AffiliateUpdateRiskModel 
    {
        [CharAndDigitalAndUnderScore(16, MinimumLength = 6, IsAllowedEmpty = false, ErrorMessage = "agent_account_format_error")]
        public string account { set; get; }

       
        public string ipaddress { set; get; }
        [ChineseAndChar(15, 3, MinimumLength = 2, IsAllowedEmpty = true, ErrorMessage = "mac_format_error")]
        public string macaddress { set; get; }
        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "sys_account_format_error")]
        public string adminAccount { set; get; }
        /// <summary>
        /// 风控级别
        /// </summary>
        public int riskTypeAready { set; get; }
        /// <summary>
        /// 风控描述
        /// </summary>
        public string reply { set; get; } = "";
        
    }
}
