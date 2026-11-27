using Dapper;
using DBLibs.WXDapper;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using UtilsLib.IPHelper;

namespace IPShareLib
{
    /// <summary>
    /// ip地址管理
    /// </summary>
    public class IPManage
    {
        /// <summary>
        /// 检测IP对应的区域是否可以访问 api
        /// </summary>
        /// <param name="account"></param>
        /// <param name="remoteUrl"></param>
        /// <param name="ip"></param>
        /// <param name="sub_key"></param>
        /// <returns></returns>
        public static async Task<Tuple<bool, string>> AccessIPLocation(string account,
           string remoteUrl,
           string ip,
           string sub_key)
        {
            var check_ip = await AccessMangerIP(ip);
            if (check_ip)
                return new Tuple<bool, string>(true, "cms 白名单IP");
            else
            {
                var loc = await QeuryIPLocation(account: "", remoteUrl: remoteUrl, ip: ip, sub_key: sub_key);
                var allowedLoctions = await AccessMangerLocation();
                foreach (var allowed in allowedLoctions) {
                    if (allowed.Equals(loc, StringComparison.OrdinalIgnoreCase))
                        return new Tuple<bool, string>(true, "允许区域, ip:{ip}, 区域:{loc}");
                }
                return new Tuple<bool, string>(false, $"区域限制,ip:{ip}, 区域:{loc}");
            }
                
        }



        /// <summary>
        /// 查询当前ip区域
        /// </summary>
        /// <param name="account"></param>
        /// <param name="remoteUrl"></param>
        /// <param name="ip"></param>
        /// <param name="sub_key"></param>
        /// <returns></returns>
        public static async Task<string> QeuryIPLocation(string  account, 
            string remoteUrl, 
            string ip,
            string sub_key)
        {
            remoteUrl = string.Format(remoteUrl, ip);
            var headers = new Dictionary<string, string>() {
                    {"subscription-key",sub_key }
                };

            var s = await AzureIPLocation.QeuryIPLocation(account: "", remoteUrl: remoteUrl, headers: headers);
            if (s != null)
            {
                if (s.countryRegion != null
                    && !string.IsNullOrEmpty(s.countryRegion.isoCode))
                {
                    return s.countryRegion.isoCode;
                }
            }

            return "";
        }

        /// <summary>
        /// 返回可访问的区域
        /// </summary>
        /// <returns></returns>
        public static async Task<List<string>> AccessMangerLocation()
        {
            using (SqlConnection connection = new SqlConnection(DapperStringBase.ConnectionStringReadOnly))
            {
                var queryMapIPSql = string.Format(@"select body from ofSettings with(nolock)
where code='access_ip_location'");

                var locs = await connection.QueryAsync<string>(queryMapIPSql);
                if (locs != null && locs.Count() > 0)
                {
                    //白名单验证通过
                    var locations = locs.First().Split('#').ToList();
                    return locations;
                }
                else
                {
                    return new List<string>();
                }

            }
        }


        /// <summary>
        /// 检测ip 会法的 vpn
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static async Task<bool> AccessMangerIP(string ip)
        {
            using (SqlConnection connection = new SqlConnection(DapperStringBase.ConnectionStringReadOnly))
            {
                var queryMapIPSql = string.Format(@"select account from ofManageAccessApiIPS with(nolock)
where ipaddress=@ip");

                var accounts = await connection.QueryAsync<string>(queryMapIPSql,
                    new
                    {
                        ip = ip
                    });
                if (accounts != null && accounts.Count() >  0)
                {
                    //白名单验证通过
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }
    }
}
