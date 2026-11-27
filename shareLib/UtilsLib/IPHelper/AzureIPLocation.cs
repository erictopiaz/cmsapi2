using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UtilsLib.RequestHelper;

namespace UtilsLib.IPHelper
{
    /// <summary>
    /// azure 查询ip位置区域
    /// </summary>
    public class AzureIPLocation
    {
        /// <summary>
        /// 查询指定会员，时间段内的充值 （到账时间为准）
        /// </summary>
        /// <param name="lists">会账号</param>
        /// <returns>true: 黑名单了，false:不在黑名单列表</returns>
        public static async Task<IPLocationResponseModel> QeuryIPLocation(string account,
            string remoteUrl, 
            Dictionary<string, string> headers,
            string proxyUrl= "http://172.233.91.151:65531")
        {
            
            var data = await MRequest.Relay(remoteUrl, headers: headers, "", "",
                    isAllowedProxy: true, ContentType: "application/json; charset=utf-8", method: "get", proxyUrl: proxyUrl, timeOut: 4000);
            if(string.IsNullOrEmpty(data))
                return null;
            else
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IPLocationResponseModel>(data);
            }
        }
    }

    public class IPLocationResponseModel
    {
        public CountryRegion countryRegion { set; get; }
        public string ipAddress { set; get; }
        public IPError error { set; get; }
    }

    public class CountryRegion
    {
        public string isoCode {  get; set; }
    }

    public class IPError
    {
        public string code {  set; get; }
        public string message { set; get; }
        public string target { set; get; }
    }

//      "error": {
//    "code": "Unauthorized",
//    "message": "Bearer realm=\"https://atlas.microsoft.com/\", SharedKey realm=\"https://atlas.microsoft.com/\"",
//    "target": "WWW-Authenticate",
//    "details": []
//}



//    {
//  "countryRegion": {
//    "isoCode": "US"
//  },
//  "ipAddress": "2001:4898:80e8:b::189"
//}

}
