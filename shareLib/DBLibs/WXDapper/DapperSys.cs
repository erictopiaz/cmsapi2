using DBLibs.Config;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DBLibs.WXDapper
{
    /// <summary>
    /// 系统配置库
    /// </summary>
    public class DapperSys : DapperBase
    {
        /// <summary>
        /// 重写父类方法取得
        /// 实例的数据库链接字串集
        /// </summary>
        //protected override DapperStringBase _options { set; get; }
        //public DapperSys(IOptions<DapperStringBase> options)
        //{
        //    this._options = options.Value;
        //}
        //public string GetCurrentConnectionString()
        //{
        //    return _options.ConnectionStringSys;
        //}
    }
}
