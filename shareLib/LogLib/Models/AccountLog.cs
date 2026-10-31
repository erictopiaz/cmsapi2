using Models.share;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogLib.Models
{
    /// <summary>
    /// 系统内部日志
    /// </summary>
    public class AccountLog : ClientModel
    {
        /// <summary>
        /// 代理账号
        /// </summary>
        public string agent { set; get; }
        /// <summary>
        /// 公司账号
        /// </summary>
        public string company { set; get; }
        /// <summary>
        /// 用户账号
        /// </summary>
        public string account { set; get; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public string createtime { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { set; get; }
        /// <summary>
        /// 日志归类
        /// </summary>
        public int logEvent { set; get; }
    }
}
