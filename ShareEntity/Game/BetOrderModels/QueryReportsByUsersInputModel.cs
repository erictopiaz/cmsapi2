using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.Game.BetOrderModels
{
    public class QueryReportsByUsersInputModel
    {
        /// <summary>
        /// 账户列表
        /// </summary>
        public List<string> Users { get; set; }
        public string agentName { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string startTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string stopTime { get; set; }
    }
}
