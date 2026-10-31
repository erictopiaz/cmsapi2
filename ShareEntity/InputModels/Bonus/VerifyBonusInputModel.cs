using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Bonus
{
    /// <summary>
    /// 单个审核
    /// </summary>
    public class VerifyBonusInputModel : AccountModel
    {
        public int tid { set; get; }

        [SqlInjectAttibute(IsAllowedEmpty =true,ErrorMessage = "reason_format_error")]
        public string cancelreason { set; get; }
    }
    /// <summary>
    /// 批量操作
    /// </summary>
    public class VerifyBonusBatInputModel : AccountModel
    {
        public string tids { set; get; }

        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "reason_format_error")]
        public string cancelreason { set; get; }
    }
}
