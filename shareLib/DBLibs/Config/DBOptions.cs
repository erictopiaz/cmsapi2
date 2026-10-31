using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DBLibs.Config
{
    /// <summary>
    /// 数据库链接字串
    /// </summary>
    public class DBOptions
    {
        /// <summary>
        /// 连接字串
        /// </summary>
        public string ConnectionString { set; get; }

        /// <summary>
        /// 当前加密码或解密KEY
        /// </summary>
        [DefaultValue("123456")]
        public string DesKey { set; get; }
        /// <summary>
        /// total record
        /// </summary>
        [DefaultValue(0)]
        public int TotalRecord { set; get; }
        /// <summary>
        /// total pages
        /// </summary>
        [DefaultValue(0)]
        public int TotalPages { set; get; }
        /// <summary>
        /// page index
        /// </summary>
        [DefaultValue(1)]
        public int PageIndex { set; get; }
    }
}
