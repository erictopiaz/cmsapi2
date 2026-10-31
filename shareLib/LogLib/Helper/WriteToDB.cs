using DBLibs.WXDapper;
using LogLib.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LogLib.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class WriteToDB
    {
        public void Done(LogModel logModel)
        {
            Task.Run(async () => await Task.WhenAll(WriteAllToDB(logModel)).ConfigureAwait(false));

        }
        private static IEnumerable<Task> WriteAllToDB(LogModel logModel)
        {
            var taskList = new List<Task>();
            try
            {
                taskList.Add(DBWrite(logModel));
            }
            catch (Exception)
            {

            }
            return taskList;
        }
        /// <summary>
        /// 写日志文件
        /// </summary>
        /// <param name="logType">当前日志是哪个种类</param>
        /// <param name="title">日志标题</param>
        /// <param name="body">日志内容</param>
        /// <param name="append">是否追加文件</param>
        /// <returns></returns>
        private static async Task DBWrite(LogModel model)
        {
            try
            {
                var logSql = @"INSERT INTO [dbo].[ofAccountLogs]
           ([logno]
           ,[account]
           ,[accounttype]
           ,[logtype]
           ,[logmessage]
           ,[ipaddress]
           ,[relationAccount]
           ,[logresult]
           ,[actiontype],[logoDetail])
     VALUES
           (@logno,@account,@accounttype,@logtype,@logmessage,@ipaddress,@relationAccount,@logresult,@actiontype,@logoDetail)";
        await DapperBase.ExecuteTransctionAsync(logSql, model, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionLogs);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
