using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DBLibs.WXDapper
{
    /// <summary>
    /// ado 操作类
    /// </summary>
    public sealed class DapperHelper : DapperBase
    {
        /// <summary>
        /// 重写父类方法取得
        /// 实例的数据库链接字串集
        /// </summary>
        //protected static DapperStringBase _options { set; get; }
        //public DapperHelper(IOptions<DapperStringBase> options)
        //{
        //    _options = options.Value;
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public static string GetCurrentConnectionString()
        //{
        //    return _options.ConnectionString;
        //}
    }
}
