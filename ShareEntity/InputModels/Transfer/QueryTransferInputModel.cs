//using CMS.Data.PageModel;
using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Transfer
{
    /// <summary>
    /// 转账列表查询输入
    /// </summary>
    public class QueryTransferInputModel : PageBaseModel
    {
        /// <summary>
        /// 转账方式
        /// </summary>
        public int transferCode { set; get; }
        /// <summary>
        /// 转账状态
        /// </summary>
        public int transferStatus { set; get; }
        /// <summary>
        /// 转账IP
        /// </summary>

        [IpValidate(15, MinimumLength = 8, IsAllowedEmpty = true, ErrorMessage = "ip_format_error")]
        public string ipaddress { set; get; }
        /// <summary>
        /// 转账开始时间
        /// </summary>
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage ="time_format_error")]
        public string startTime { set; get; }
        /// <summary>
        /// 转账结束时间
        /// </summary>
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string endTime { set; get; }

        /// <summary>
        /// 代理账号
        /// </summary>
        [CharAndDigital(12, MinimumLength = 6, IsAllowedEmpty = true, ErrorMessage = "agent_account_format_error")]
        public string agentAccount { set; get; }
    }
}
