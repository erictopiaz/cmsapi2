using System;
using System.Collections.Generic;
using System.Text;

namespace LogLib.Config
{
    /// <summary>
    /// 文件日志操作实例
    /// </summary>
    public sealed class FileOption
    {
        /// <summary>
        /// 日志文件的路径
        /// </summary>
        public string FilePath { set; get; }
    }
}
