using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using QQWry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UtilsLib.IPHelper;
using UtilsLib.SqlInject;

namespace UtilsLib.RequestHelper
{
    public class HttpHelper
    {
        public static ClientModel GetClient(IHttpContextAccessor _accessor, 
            string clientIp = "127.0.0.1",
            string loction = "")
        {
            StringValues token = string.Empty;
            var ip = string.Empty;
            var macaddress = string.Empty;
            var xf = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(clientIp) 
                    || clientIp.Equals("127.255.255.255"))
                    clientIp = "127.0.0.1";

                var header = clientIp;

                if (_accessor.HttpContext.Request.Headers.ContainsKey("X-Real-IP") && clientIp.Equals("127.0.0.1"))
                {
                    header = _accessor.HttpContext.Request.Headers["X-Real-IP"].ToString();
                }

                if(_accessor.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                {
                    xf = _accessor.HttpContext.Request.Headers["X-Forwarded-For"].First().ToString();
                }

                if (_accessor.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For") 
                    && clientIp.Equals("127.0.0.1"))
                {
                    if (!string.IsNullOrEmpty(_accessor.HttpContext.Request.Headers["X-Forwarded-For"]))
                    { 
                        header = _accessor.HttpContext.Request.Headers["X-Forwarded-For"].First().Split(',')[0];
                    }
                }
                if (IPAddress.TryParse(header, out IPAddress f_ip))
                {
                    if(!string.IsNullOrEmpty(f_ip.ToString()))
                    {
                        ip = f_ip.ToString();
                    }
                    else
                        ip = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                }
                else
                    ip = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

                if (!IPAddress.TryParse(ip, out IPAddress ff_ip))
                    ip = "127.254.255.255";

                _accessor.HttpContext.Request.Headers.TryGetValue("Authorization", out token);
                
                macaddress = GetMacAddressV(ip, loction);
            }
            catch (Exception)
            {

            }
            return new ClientModel { ipaddress = ip, token = token, macaddress = macaddress, xf = xf };


        }

        /// <summary>
        /// 获取ip 没有mac 和 token
        /// </summary>
        /// <param name="_accessor"></param>
        /// <param name="clientIp"></param>
        /// <param name="loction"></param>
        /// <returns></returns>
        public static ClientModel GetClientWithOutMac(IHttpContextAccessor _accessor,
            string clientIp = "127.0.0.1",
            string loction = "")
        {
            StringValues token = string.Empty;
            var ip = string.Empty;
            var macaddress = string.Empty;
            var xf = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(clientIp)
                    || clientIp.Equals("127.255.255.255"))
                    clientIp = "127.0.0.1";

                var header = clientIp;

                if (_accessor.HttpContext.Request.Headers.ContainsKey("X-Real-IP") && clientIp.Equals("127.0.0.1"))
                {
                    header = _accessor.HttpContext.Request.Headers["X-Real-IP"].ToString();
                }

                if (_accessor.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                {
                    xf = _accessor.HttpContext.Request.Headers["X-Forwarded-For"].First().ToString();
                }

                if (_accessor.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For")
                    && clientIp.Equals("127.0.0.1"))
                {
                    if (!string.IsNullOrEmpty(_accessor.HttpContext.Request.Headers["X-Forwarded-For"]))
                    {
                        header = _accessor.HttpContext.Request.Headers["X-Forwarded-For"].First().Split(',')[0];
                    }
                }
                if (IPAddress.TryParse(header, out IPAddress f_ip))
                {
                    if (!string.IsNullOrEmpty(f_ip.ToString()))
                    {
                        ip = f_ip.ToString();
                    }
                    else
                        ip = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                }
                else
                    ip = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

                if (!IPAddress.TryParse(ip, out IPAddress ff_ip))
                    ip = "127.254.255.255";

                _accessor.HttpContext.Request.Headers.TryGetValue("Authorization", out token);


                macaddress = "";
            }
            catch (Exception)
            {

            }
            return new ClientModel { ipaddress = ip, token = token, macaddress = macaddress, xf = xf };

        }

        private static string GetMacAddress(string ip, string location)
        {
            try
            {
                var path = Path.Combine(Path.GetDirectoryName(location), "qqwry.dat");

               
                var ipScanner = new IpScanner
                {
                    DataPath = path,
                    IP = ip
                };
                var address = ipScanner.IPLocation();
                if (address.Contains("CZ88.NET"))
                    address = address.Replace("CZ88.NET", " ").Trim();
                return address;

            }
            catch (Exception)
            {
                return "";
            }
           
        }

        private static string GetMacAddressV(string ip, string location)
        {
            try
            {
                var config = new QQWryOptions()
                {
                    DbPath = System.Environment.CurrentDirectory + ("\\qqwry.dat")
                };

                var ipSearch = new QQWryIpSearch(config);
                var ipLocation = ipSearch.GetIpLocation(ip);
                return string.Format(@"{0}{1}", ipLocation.Country, ipLocation.Area);
            }
            catch (Exception)
            {
                return "";
            }

        }



        /// <summary>
        /// get 请求数
        /// </summary>
        /// <param name="Authorization">token</param>
        /// <param name="path">请求路径</param>
        /// <param name="paramString">get请求参数</param>
        /// <returns></returns>
        public static string GetData(string Authorization, string path, string paramString, string requestUrl)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    webClient.Headers.Add(HttpRequestHeader.Authorization, Authorization);
                    var url = string.Format(@"{0}{1}?{2}", requestUrl, path, paramString);
                    return webClient.DownloadString(url);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }
        /// <summary>
        /// post 数据请求
        /// </summary>
        /// <param name="Authorization"></param>
        /// <param name="path"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string PostData(string Authorization, string path, object o, string requestUrl)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    webClient.Headers.Add(HttpRequestHeader.Authorization, Authorization);
                    var url = string.Format(@"{0}{1}", requestUrl, path);
                    return webClient.UploadString(url,
                        "POST", JsonConvert.SerializeObject(o));
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }



    }

    public class ClientModel
    {
        public string ipaddress { set; get; } = "127.0.0.1";
        public string macaddress { set; get; } = "";
        /// <summary>
        /// 请求验证TOKEN
        /// </summary>
        public string token { set; get; }
        /// <summary>
        /// x-forward-
        /// </summary>
        public string xf { set; get; }
    }
}
