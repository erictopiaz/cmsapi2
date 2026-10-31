using System;
using System.Collections.Generic;
using System.Text;

namespace DBLibs.WXDapper
{
    /// <summary>
    /// 资料库链接字串实体
    /// </summary>
    public class DapperStringBase
    {
        /// <summary>
        /// 基本数据操作，数据库字串  正在使用
        /// </summary>
        public static string ConnectionString { get; set; }
        /// <summary>
        /// 只读库操作    正在使用
        /// </summary>
        public static string ConnectionStringReadOnly { set; get; }


        /// <summary>
        /// 系统资料操作,数据库字串
        /// </summary>
        public static string ConnectionStringSys { set; get; }

        /// <summary>
        /// 游戏记录数据库字串
        /// </summary>
        public static string ConnectionGameRecordString { set; get; }
        /// <summary>
        /// 日志数据库 
        /// </summary>
        public static string ConnectionLogs { set; get; }
        /// <summary>
        /// 收款系统数据库串串
        /// </summary>
        public static string ConnectionPayString { set; get; }
        /// <summary>
        /// 直揪库字串
        /// </summary>
        public static string ConnectionZBString { set; get; }
        /// <summary>
        /// mongo db 链接串串
        /// </summary>
        public static string ConnectionMongoString { set; get; }
        /// <summary>
        /// mongo db 集群 链接串串
        /// </summary>
        public static string ConnectionMongoClusterString { set; get; }

        /// <summary>
        /// MySql数据库字串
        /// </summary>
        public static string ConnectionMySqlString { set; get; }
        /// <summary>
        /// 游戏注单postgre数据库连续地址
        /// </summary>
        public static string ConnectionGamePostgreString { set; get; }
    }

    public class GameParams {
        public List<GameParamModel> gameParamModels { set; get; }
    }
    public class GameParamModel {
        public int gameid { set; get; }
        public string gamename { set; get; }
        public Dictionary<string,string> param { set; get; }
    }
    
}
