using CMS.API.BLL.Export;
using DBLibs.WXDapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.API.BLL
{
    /// <summary>
    /// 根据给定的PATH ，查看是否有权限操作
    /// </summary>
    public class CheckRight
    {
        public static async Task<bool> LoadAllowedPathByAccount(string account, string requestPath)
        {
            try
            {
                var accessSql = string.Format(@"select * from ofManage where account=@account and status = 1");
                var result = await DapperBase.QueryAsync<RightModel>(accessSql, new { account = account }, commandType: CommandType.Text,
                            commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);

                if (result != null && result.Count == 1 && result.First().access.IndexOf(requestPath, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
