using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.API.Models.Call
{
    /// <summary>
    /// 备注记录
    /// </summary>
    public class CallLogModel
    {
        // t1.account, t1.comment,t1.createtime,t1.createperson,t1.contactStatus,
        /// <summary>
        /// 会员账号
        /// </summary>
        public string account { set; get; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string createperson { set; get; }
        /// <summary>
        /// 备注数据
        /// </summary>
        public string comment { set; get; }
        /// <summary>
        /// 备注状态  0：未通，1：接通， -1：分配置
        /// </summary>
        public int contactStatus { set; get; }
        /// <summary>
        /// 备注或分配时间
        /// </summary>
        public string createtime { set; get; }
    }
    /// <summary>
    /// 备注记录统计输出
    /// </summary>
    public class CallLogOutModel:CallLogModel
    {
        /// <summary>
        /// 统计时间内的存款金额
        /// </summary>
        public decimal amount { set; get; }
        /// <summary>
        /// 备注记录的分配时间
        /// </summary>
        public string deleverTime { set; get; }
        /// <summary>
        /// 从注册到当前，最后一次存款时间
        /// </summary>
        public string reviewtime { set; get; }
        /// <summary>
        /// VIP名称
        /// </summary>
        public string levels { set; get; }

        public long uid { set; get; }
    }
}
