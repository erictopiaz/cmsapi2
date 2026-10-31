//using CMS.Data.ClientModel;
using ShareEntity.ClientModel;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.agent
{
    public class updateAgentBillPInputModel : IPInputModel
    {
        [CharAndDigital(12, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "sys_account_format_error")]
        public string adminAccount { set; get; }
        [CharAndDigitalAndUnderScore(20, MinimumLength = 6, IsAllowedEmpty = false, ErrorMessage = "agent_account_format_error")]
        public string account { set; get; }
        /// <summary>
        /// 是否可开下级代理
        /// 0：否，1：有
        /// </summary>
        public int allowCreateAgent { set; get; }
        /// <summary>
        /// 1:已经分配代理层级，0：无
        /// </summary>
        public int ty { set; get; } = 0;

        public Dictionary<string,decimal> billP { set; get; }


        [UrlValidate(50, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "url_format_error")]
        public string link1 { set; get; }
        [UrlValidate(50, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "url_format_error")]
        public string link2 { set; get; }
        [UrlValidate(50, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "url_format_error")]
        public string link3 { set; get; }

        [CharAndDigital(12, MinimumLength = 3, IsAllowedEmpty = true, ErrorMessage = "adcode_format_error")]
        public string adcode { set; get; }

        [CharAndDigital(12, MinimumLength = 3, IsAllowedEmpty = true, ErrorMessage = "parentAdcode_format_error")]
        public string parentAdcode { set; get; }
        /// <summary>
        /// 多账号
        /// 0：否，1：是
        /// </summary>
        public int isMultiAccount { set; get; }

        public int depositAllowed { set; get; }
        public int transferAllowed { set; get; }

        /// <summary>
        /// 代理是否开启了流水奖励
        /// 0：否，1：是
        /// </summary>
        public int betBonusAllowed { set; get; } = 0;
        /// <summary>
        /// 流水奖励的比例
        /// </summary>
        public decimal betBonusP { set; get; } = 0;
        /// <summary>
        /// 流水奖励上限
        /// </summary>
        public decimal betBonusMax { set; get; } = 0;

        [CharAndDigital(12, MinimumLength = 2, IsAllowedEmpty = true, ErrorMessage = "sys_account_format_error")]
        public string agentMarket { set; get; } = "";

        /// <summary>
        /// 结算优惠比例月
        /// </summary>
        public int billPromoMonth { set; get; } = 0;

    }

    



}
