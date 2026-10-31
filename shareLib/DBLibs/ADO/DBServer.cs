using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DBLibs.ADO
{
    public class DBServer
    {
        /// <summary>
        /// DB HANDLE
        /// </summary>
        public SqlConnection SqlConn = null;
      
        
        //public int TotalRecord = 0;

        /// <summary>
        /// total pages
        /// </summary>
        public int TotalPages = 0;
        /// <summary>
        /// page index
        /// </summary>
        public int PageIndex = 1;
       

        public static async Task<bool> CreateTable(string ResConnectionStr, string tableSql)
        {
            using (SqlConnection sqlconn = new SqlConnection(ResConnectionStr))
            {
                sqlconn.Open();
                using (SqlCommand command =
                    new SqlCommand(tableSql, sqlconn))
                {
                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
        
        public static bool BulkInsertResult<T>(ref string ErrorStr, string TableName, IList<T> dataList, string ResConnectionStr)
        {
            try
            {

                if (dataList == null || dataList.Count <= 0)
                    ErrorStr = "请求返回数据为空!";
                else
                    ErrorStr = string.Format(@"本次请求返回数据为---{0}----条", dataList.Count.ToString());
                DataTable dt = dataList.ToDataTable();

                using (SqlConnection sqlconn = new SqlConnection(ResConnectionStr))
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlconn,
                    SqlBulkCopyOptions.TableLock |
                    SqlBulkCopyOptions.FireTriggers |
                    SqlBulkCopyOptions.UseInternalTransaction,
                    null))
                    {
                        bulkCopy.DestinationTableName = TableName;
                        bulkCopy.WriteToServer(dt);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorStr = ex.Message;
            }
            return false;

        }

       



    }
}
