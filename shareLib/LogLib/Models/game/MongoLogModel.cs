using System;
using System.Collections.Generic;
using System.Text;

namespace LogLib.Models.game {
    /// <summary>
    /// 游戏日志数据
    /// </summary>
    public class MongoLogModel {
        public string action_name { set; get; }
        /// <summary>
        /// 1: 游戏
        /// </summary>
        public int logType { set; get; }
        public string account { set; get; }
        public double usedTime { set; get; } = 0;
        public long create_at { set; get; }
        public string create_at_str { set; get; }

        public object requestModel { set; get; }
        public object responseModel { set; get; } 
    }
}
