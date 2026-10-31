using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibs
{
    /// <summary>
    /// 读取游戏余额输出
    /// </summary>
    public class BalanceOutPutModel
    {
        /// <summary>
        /// 0 : 失败 ，1: 正常
        /// </summary>
        public int code { set; get; }
        /// <summary>
        /// 游戏代码
        /// </summary>
        public int gameId { set; get; }
        /// <summary>
        /// 游戏平台名
        /// </summary>
        public string gameName { set; get; }
        /// <summary>
        /// 游戏余额
        /// </summary>
        public object balance { set; get; }
    }
}
