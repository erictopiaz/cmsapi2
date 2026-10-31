using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.agent
{
    public class VerifyAffWithdrawInputModel : AccountModel
    {
        public long id { set; get; }
        /// <summary>
        /// 0: 等审核
        /// 1：等支付
        /// 2： 支付完成
        /// 3： 取消
        /// </summary>
        public int status { set; get; }

        [SqlInjectAttibute(IsAllowedEmpty =true,ErrorMessage = "remark_format_error")]
        public string cancelreason { set; get; }
    }
}
