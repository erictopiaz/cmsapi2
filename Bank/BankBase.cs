using DBLibs.WXDapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    public class BankBase
    {
        /// <summary>
        /// 获取银行名
        /// </summary>
        /// <param name="id">银行id</param>
        /// <param name="currentcy_code">货币代码</param>
        /// <returns></returns>
        public static async Task<string> QueryBankName(int id, int currentcy_code)
        {
            try
            {
                var querySql = string.Format(@"select top 1 name from ofBankBase with(nolock)
where bid={0} and currencycode={1}", id, currentcy_code);
                var bankNameObjet = await DapperBase.ExecuteScalarValueAsync(querySql, null,
                    commandTimeout: 1500, ConnectionString: DapperStringBase.ConnectionStringReadOnly);
                if (bankNameObjet != null)
                {
                    return bankNameObjet.ToString() + ":" + id.ToString();
                }
                else
                {
                    return ":" + id.ToString();
                }
            }
            catch (Exception)
            {
                return ":" + id.ToString();
            }
            
        }
    }
}
