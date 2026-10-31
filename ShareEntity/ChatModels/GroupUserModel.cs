using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.ChatModels
{
    /// <summary>
    /// 添加到聊天系统的会员数据，
    /// 这些会员，会有次数限制，专员线路时要输入账号
    /// </summary>
    public class GroupUserModel
    {
        /// <summary>
        /// 会员id
        /// </summary>
        public long id { set; get; }
        /// <summary>
        /// 商户号
        /// </summary>
        public int merchant_code { set; get; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string username { set; get; }
        /// <summary>
        /// 聊天次数限制
        /// </summary>
        public int chat_time { set; get; }
        /// <summary>
        /// 分配vip专员id
        /// </summary>
        public int group_id { set; get; }
    }
}
