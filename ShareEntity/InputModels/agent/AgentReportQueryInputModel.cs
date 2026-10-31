using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.agent
{
    /// <summary>
    /// 代理权限查询
    /// </summary>
    public class AgentRightInputModel
    {
        public int queryType { set; get; } = 0;
        /// <summary>
        /// 代理账号
        /// </summary>
        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = false, ErrorMessage = "account_format_error")]
        public string account { set; get; }
        /// <summary>
        /// 管理员账号
        /// </summary>
        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "admin_account_format_error")]
        public string adminAccount { set; get; }

    }

    /// <summary>
    /// 当前记录，有多少行
    /// </summary>
    public class RecordsCount
    {
        public int cnt { set; get; }
    }
}
