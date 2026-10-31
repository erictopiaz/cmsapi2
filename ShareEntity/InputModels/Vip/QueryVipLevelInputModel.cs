//using CMS.Data.PageModel;
using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Vip
{
    /// <summary>
    /// vip 数据升级查询输入数据
    /// </summary>
    public class QueryVipLevelInputModel : PageBaseModel
    {
        [CharAndDigitalAndUnderScore(12, MinimumLength = 5, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string affaccount { set; get; }
        public int startTime { set; get; }
        public int endTime { set; get; }

        /// <summary>
        /// 当前客人是否可以升级了
        /// 0：降级，1：升级，3：保级
        /// </summary>
        public int status { set; get; } = -1;

        /// <summary>
        /// 0：降级， 1：升级，2：保级
        /// </summary>
        public int is_up { set; get; } = -1;
        /// <summary>
        /// vip 级别
        /// 升级到此级别的会员
        /// </summary>
        public int vipLevel { set; get; } = -1;

        /// <summary>
        /// 审核状态
        /// </summary>
        public int verifyStatus { set; get; } = -1;


    }


    public class VipLevelUpDownInputModel : AccountModel
    {
        public string ids { set; get; }

        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "reason_format_error")]
        public string cancelreason { set; get; }
    }

    /// <summary>
    /// vip 数据升级查询输入数据
    /// </summary>
    public class QueryVipDepositTimesDailyInputModel : PageBaseModel
    {
        [CharAndDigitalAndUnderScore(12, MinimumLength = 5, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string affaccount { set; get; }
        public int startTime { set; get; }
        public int endTime { set; get; }
        public int currencyType { set; get; }
        /// <summary>
        /// 品牌
        /// </summary>
        [MaxLength(6)]
        public string merchantAccount { set; get; }

    }

}
