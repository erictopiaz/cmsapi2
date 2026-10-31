using Microsoft.AspNetCore.Http.Headers;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Numeric;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UtilsLib.RequestHelper
{
    /// <summary>
    /// 公网请求数据
    /// </summary>
    public class MRequest
    {
        public static readonly string remote = "https://xxoo.com";
        public static string RelayPDPG(string method, 
            string header, string String_Send, string urlPath, string ContentType = "application/json", int timeOut=4000)
        {
            try
            {
                string RecvStr = "";
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                byte[] byteArray = Encoding.UTF8.GetBytes(String_Send);
                var requestUrlString = string.Format("{0}{1}", remote, urlPath);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(requestUrlString);
                if (header != "")
                {
                    WebReq.Headers.Add("Authorization", header);
                }
                WebReq.Method = method;
                WebReq.Accept = ContentType;
                WebReq.ContentType = ContentType; // "application/json";// "text/xml";
                WebReq.ContentLength = byteArray.Length;
                WebReq.Timeout = timeOut;
                if (string.Compare("get", method, true) != 0)
                {
                    using (var stream = WebReq.GetRequestStream()) {
                        stream.Write(byteArray, 0, byteArray.Length);
                    }
                }

                using (var WebRecv = (HttpWebResponse)WebReq.GetResponse())
                using (var reader = new StreamReader(WebRecv.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"))) {
                    string sLine = "";
                    while (sLine != null) {
                        sLine = reader.ReadLine();
                        if (sLine != null)
                            RecvStr += sLine;
                    }
                    RecvStr = RecvStr.Trim();
                    return RecvStr;
                }

                //HttpWebResponse WebRecv = (HttpWebResponse)WebReq.GetResponse();
                //Stream echostream = WebRecv.GetResponseStream();
                //StreamReader reader = new StreamReader(echostream, System.Text.Encoding.GetEncoding("UTF-8"));
                //string sLine = "";
                //while (sLine != null)
                //{
                //    sLine = reader.ReadLine();
                //    if (sLine != null)
                //        RecvStr += sLine;
                //}
                //RecvStr = RecvStr.Trim();
                //return RecvStr;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        /// <summary>
        /// http异步请求
        /// </summary>
        /// <param name="method"></param>
        /// <param name="header"></param>
        /// <param name="String_Send"></param>
        /// <param name="urlPath"></param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        public static async Task<string> RelayPDPGAsync(string method, 
            string header, string String_Send, string urlPath, string ContentType = "application/json", int timeOut=4000)
        {
            try
            {
                string RecvStr = "";
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                byte[] byteArray = Encoding.UTF8.GetBytes(String_Send);
                var requestUrlString = string.Format("{0}{1}", remote, urlPath);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(requestUrlString);
                if (header != "")
                {
                    WebReq.Headers.Add("Authorization", header);
                }
                WebReq.Method = method;
                WebReq.Accept = ContentType;
                WebReq.ContentType = ContentType; // "application/json";// "text/xml";
                WebReq.ContentLength = byteArray.Length;
                WebReq.Timeout = timeOut;
                if (string.Compare("get", method, true) != 0)
                {
                    using (var stream = WebReq.GetRequestStream()) {
                        await stream.WriteAsync(byteArray, 0, byteArray.Length);
                    }
                    
                }

                using (var WebRecv = WebReq.GetResponse())
                using (var reader = new StreamReader(WebRecv.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"))) {
                    string sLine = "";
                    while (sLine != null) {
                        sLine = await reader.ReadLineAsync();
                        if (sLine != null)
                            RecvStr += sLine;
                    }
                    RecvStr = RecvStr.Trim();
                    return RecvStr;
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        /// <summary>
        /// form-data  request post
        /// </summary>
        /// <param name="method"></param>
        /// <param name="remote"></param>
        /// <param name="formFields"></param>
        /// <returns></returns>
        public static string UploadFilesToRemoteUrl(string method,
            string remote, SortedDictionary<string, object> formFields, bool allowedProxy = false, int timeOut=4000)
        {
            try
            {
                string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(remote);
                if(allowedProxy)
                {
                    System.Net.WebProxy proxy = new WebProxy(new Uri("http://119.9.75.103:65531"));//指定的ip和端口
                    request.Proxy = proxy;
                }
                request.ContentType = "multipart/form-data; boundary=" +
                boundary;
                request.Method = method;// "POST";
                request.Timeout = timeOut;
                request.KeepAlive = true;
                Stream memStream = new System.IO.MemoryStream();
                var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                boundary + "\r\n");
                var endBoundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                boundary + "--");
                string formdataTemplate = "\r\n--" + boundary +
                "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
                if (formFields != null)
                {
                    foreach (string key in formFields.Keys)
                    {
                        string formitem = string.Format(formdataTemplate, key, formFields[key]);
                        byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                        memStream.Write(formitembytes, 0, formitembytes.Length);
                    }
                }

                memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                request.ContentLength = memStream.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    memStream.Position = 0;
                    byte[] tempBuffer = new byte[memStream.Length];
                    memStream.Read(tempBuffer, 0, tempBuffer.Length);
                    memStream.Close();
                    requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                }

                using (var response = request.GetResponse())
                {
                    using (var reader2 = new StreamReader(response.GetResponseStream())) {
                        var content = reader2.ReadToEnd();
                        return content;
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        #region 当前form-data
        public static async Task<string> UploadFilesToRemoteUrl(string method, string remote,
            string[] keys, object[] values, string Authorization = "",
            string ContentType = "multipart/form-data", bool allowedProxy = false, string proxyUrl= "http://119.9.75.103:65531", int timeOut=4000)
        {
            try
            {
                string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(remote);
                if (allowedProxy 
                    && !string.IsNullOrEmpty(proxyUrl))
                {
                    System.Net.WebProxy proxy = new WebProxy(new Uri(proxyUrl));//指定的ip和端口
                    request.Proxy = proxy;
                }
                request.ContentType = ContentType + "; boundary=" +
                                        boundary;
                if (!string.IsNullOrEmpty(Authorization))
                    request.Headers.Add("Authorization", "Basic " + Authorization);
                request.Method = method;// "POST";
                request.KeepAlive = true;
                request.Timeout = timeOut;

                Stream memStream = new System.IO.MemoryStream();
                var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                        boundary + "\r\n");
                var endBoundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                            boundary + "--");
                string formdataTemplate = "\r\n--" + boundary +
                                            "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
                if (keys.Length >= 0)
                {
                    for (int i = 0; i < keys.Length; i++)
                    {
                        string formitem = string.Format(formdataTemplate, keys[i], values[i]);
                        byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                        memStream.Write(formitembytes, 0, formitembytes.Length);
                    }
                }

                memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                request.ContentLength = memStream.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    memStream.Position = 0;
                    byte[] tempBuffer = new byte[memStream.Length];
                    memStream.Read(tempBuffer, 0, tempBuffer.Length);
                    memStream.Close();
                    requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                }

                using (var response = request.GetResponse())
                {
                    using (var reader2 = new StreamReader(response.GetResponseStream())) {
                        var content = await reader2.ReadToEndAsync();// reader2.ReadToEnd();
                        return content;
                    }
                }
            }
            catch (WebException ex)
            {
                return printResponse(ex.Response as HttpWebResponse);
            }
        }

        public static async Task<string> UploadFilesToRemoteUrlWithoutWebException(string method, string remote,
            string[] keys, object[] values, string Authorization = "",
            string ContentType = "multipart/form-data", bool allowedProxy = false, string proxyUrl = "http://119.9.75.103:65531", int timeOut=4000) {
            try {
                string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(remote);
                if (allowedProxy
                    && !string.IsNullOrEmpty(proxyUrl)) {
                    System.Net.WebProxy proxy = new WebProxy(new Uri(proxyUrl));//指定的ip和端口
                    request.Proxy = proxy;
                }
                request.ContentType = ContentType + "; boundary=" +
                                        boundary;
                if (!string.IsNullOrEmpty(Authorization))
                    request.Headers.Add("Authorization", "Basic " + Authorization);
                request.Method = method;// "POST";
                request.Timeout = timeOut;
                request.KeepAlive = true;

                Stream memStream = new System.IO.MemoryStream();
                var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                        boundary + "\r\n");
                var endBoundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                            boundary + "--");
                string formdataTemplate = "\r\n--" + boundary +
                                            "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
                if (keys.Length >= 0) {
                    for (int i = 0; i < keys.Length; i++) {
                        string formitem = string.Format(formdataTemplate, keys[i], values[i]);
                        byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                        memStream.Write(formitembytes, 0, formitembytes.Length);
                    }
                }

                memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                request.ContentLength = memStream.Length;

                using (var requestStream = request.GetRequestStream()) {
                    memStream.Position = 0;
                    byte[] tempBuffer = new byte[memStream.Length];
                    memStream.Read(tempBuffer, 0, tempBuffer.Length);
                    memStream.Close();
                    requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                }

                using (var response = request.GetResponse()) {
                    using (var reader2 = new StreamReader(response.GetResponseStream())) {
                        var content = await reader2.ReadToEndAsync();// reader2.ReadToEnd();
                        return content;
                    }
                }
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }




        public static string UploadFilesToRemoteUrlSync(string method, string remote, 
            string[] keys, object[] values, bool allowedProxy = false)
        {
            try
            {
                string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(remote);
                if (allowedProxy)
                {
                    System.Net.WebProxy proxy = new WebProxy(new Uri("http://119.9.75.103:65531"));//指定的ip和端口
                    request.Proxy = proxy;
                }
                request.ContentType = "multipart/form-data; boundary=" +
                                        boundary;
                request.Method = method;// "POST";
                request.Timeout = 4000;
                request.KeepAlive = true;
                Stream memStream = new System.IO.MemoryStream();
                var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                        boundary + "\r\n");
                var endBoundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                            boundary + "--");
                string formdataTemplate = "\r\n--" + boundary +
                                            "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
                if (keys.Length >= 0)
                {
                    for (int i = 0; i < keys.Length; i++)
                    {
                        string formitem = string.Format(formdataTemplate, keys[i], values[i]);
                        byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                        memStream.Write(formitembytes, 0, formitembytes.Length);
                    }
                }

                memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                request.ContentLength = memStream.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    memStream.Position = 0;
                    byte[] tempBuffer = new byte[memStream.Length];
                    memStream.Read(tempBuffer, 0, tempBuffer.Length);
                    memStream.Close();
                    requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                }

                using (var response = request.GetResponse())
                {
                    Stream stream2 = response.GetResponseStream();
                    StreamReader reader2 = new StreamReader(stream2);
                    var content = reader2.ReadToEnd();// reader2.ReadToEnd();
                    return content;
                }
            }
            catch (WebException ex)
            {
                return printResponse(ex.Response as HttpWebResponse);
            }
        }

        public static string printResponse(HttpWebResponse webResponse)
        {
            try {
                using (var resposneStream = webResponse.GetResponseStream())
                using (var streamReader = new StreamReader(resposneStream, Encoding.UTF8)) {
                    string responseData = streamReader.ReadToEnd();
                    return responseData;
                }
            }
            catch (Exception ex) {

                return ex.Message;
            }
        }
        #endregion

        public static string RelayPDPG_Url(string method, string remoteUrl, string header,
            string String_Send, string urlPath, string ContentType = "application/json")
        {
            try
            {
                string RecvStr = "";
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                byte[] byteArray = Encoding.UTF8.GetBytes(String_Send);
                var requestUrlString = string.Format("{0}{1}", remoteUrl, urlPath);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(requestUrlString);
                if (header != "")
                {
                    WebReq.Headers.Add("Authorization", header);
                }
                WebReq.Method = method;
                WebReq.Accept = ContentType;
                WebReq.ContentType = ContentType; // "application/json";// "text/xml";
                WebReq.ContentLength = byteArray.Length;
                WebReq.Timeout = 4000;
                if (string.Compare("get", method, true) != 0)
                {
                    using (var stream = WebReq.GetRequestStream()) {
                        stream.Write(byteArray, 0, byteArray.Length);
                    }
                }


                using (var WebRecv = (HttpWebResponse)WebReq.GetResponse())
                using (var reader = new StreamReader(WebRecv.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"))) {
                    string sLine = "";
                    while (sLine != null) {
                        sLine = reader.ReadLine();
                        if (sLine != null)
                            RecvStr += sLine;
                    }
                    RecvStr = RecvStr.Trim();
                    return RecvStr;
                }
            }
            catch (WebException ex)
            {
                //string status = null;
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                //if (response.StatusCode == HttpStatusCode.NotFound)
                //    status = "404";
                //if (response.StatusCode == HttpStatusCode.InternalServerError)
                //    status = "500";
                //if (response.StatusCode == HttpStatusCode.BadGateway)
                //    status = "502";
                return ((int)response.StatusCode).ToString();
            }
        }

        public static async Task<string> RelayLBForm(string remote, string method, string header, bool wait,
            string String_Send, string urlPath, string ContentType = "application/x-www-form-urlencoded",
            bool isAllowedProxy = true, string proxyUrl = "http://119.9.75.103:65531", int timeOut=4000)
        {
            try
            {
                string RecvStr = "";
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                byte[] byteArray = Encoding.UTF8.GetBytes(String_Send);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(remote);
                if (isAllowedProxy 
                    && !string.IsNullOrEmpty(proxyUrl))
                {
                    //new proxy  103.102.46.33
                    //old proxy 119.9.75.103
                    System.Net.WebProxy proxy = new WebProxy(new Uri(proxyUrl));//指定的ip和端口
                    WebReq.Proxy = proxy;

                }
                WebReq.Method = method;
                WebReq.ContentType = ContentType;
                WebReq.ContentLength = byteArray.Length;
                WebReq.Timeout = timeOut;//

                using (var stream = WebReq.GetRequestStream()) {
                    await stream.WriteAsync(byteArray, 0, byteArray.Length);
                }

                if (wait)
                {
                    using (var WebRecv = WebReq.GetResponse())
                    using (var reader = new StreamReader(WebRecv.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"))) {
                        string sLine = "";
                        while (sLine != null) {
                            sLine = await reader.ReadLineAsync();
                            if (sLine != null)
                                RecvStr += sLine;
                        }
                        RecvStr = RecvStr.Trim();
                        return RecvStr;
                    }
                }
                return RecvStr;
            }
            catch (WebException ex) {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                return ((int)response.StatusCode).ToString();
            }
        }



        public static async Task<string> RelayPostKeyValue(string url, string method, 
            Dictionary<string, string> dic, string ContentType = "application/x-www-form-urlencoded",
            bool isAllowedProxy = true, string proxyUrl = "http://119.9.75.103:65531", int Timeout=4000)
        {
            try
            {
                string result = string.Empty;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                if (isAllowedProxy 
                    && !string.IsNullOrEmpty(proxyUrl))
                {
                    //new proxy  103.102.46.33
                    //old proxy 119.9.75.103
                    System.Net.WebProxy proxy = new WebProxy(new Uri(proxyUrl));//指定的ip和端口
                    req.Proxy = proxy;

                }
                req.Method = method;
                req.ContentType = ContentType;
                req.Timeout = Timeout;

                #region 添加Post 参数
                StringBuilder builder = new StringBuilder();
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
                byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
                req.ContentLength = data.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    await reqStream.WriteAsync(data, 0, data.Length);
                }
                #endregion

                using (var resp = (HttpWebResponse)req.GetResponse())
                using (var reader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))//获取响应内容
                {
                    string sLine = "";
                    while (sLine != null)
                    {
                        sLine = await reader.ReadLineAsync();
                        if (sLine != null)
                            result += sLine;
                    }
                    result = result.Trim();
                }
                return result;
            }
            catch (WebException ex)
            {
                return printResponse(ex.Response as HttpWebResponse);
            }
        }

        /// <summary>
        /// 只处理post 请求 异步请求
        /// 如果isAllowedProxy= true 且 proxyUrl 有值，才会使用代理服务去请求游戏平台，
        /// 不然直接使用当前服务器的ip
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="wait"></param>
        /// <param name="String_Send"></param>
        /// <param name="urlPath"></param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        public static async Task<string> RelayForm(string remote, string method, Dictionary<string, string> headers, bool wait,
                string String_Send, string urlPath, string ContentType = "application/x-www-form-urlencoded", 
                bool isAllowedProxy = true, string proxyUrl = "http://119.9.75.103:65531", int timeOut=4000)
        {
            try
            {
                string RecvStr = "";
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                byte[] byteArray = Encoding.UTF8.GetBytes(String_Send);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(remote + urlPath);
                if(isAllowedProxy
                    && !string.IsNullOrEmpty(proxyUrl))
                {
                    System.Net.WebProxy proxy = new WebProxy(new Uri(proxyUrl));//指定的ip和端口
                    WebReq.Proxy = proxy;

                }

                WebReq.Method = method;
                if (headers != null && headers.Count > 0)
                {
                    foreach(var header in headers)
                    {
                        WebReq.Headers.Add(header.Key, header.Value);
                    }
                }
                WebReq.ContentType = ContentType;
                WebReq.ContentLength = byteArray.Length;
                WebReq.Timeout = timeOut;//

                using (Stream reqStream = WebReq.GetRequestStream()) {
                    await reqStream.WriteAsync(byteArray, 0, byteArray.Length);
                }

                if (wait)
                {
                    using (var WebRecv = WebReq.GetResponse())
                    using (var reader = new StreamReader(WebRecv.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"))) {
                        string sLine = "";
                        while (sLine != null) {
                            sLine = await reader.ReadLineAsync();
                            if (sLine != null)
                                RecvStr += sLine;
                        }
                        RecvStr = RecvStr.Trim();
                        return RecvStr;
                    }

                }
                return RecvStr;
            }
            catch (WebException ex)
            {
                return printResponse(ex.Response as HttpWebResponse);
            }
        }

        public static async Task<string> RelayFBForm(string remote, string method, Dictionary<string, string> headers, bool wait,
                string String_Send, string urlPath, string ContentType = "application/x-www-form-urlencoded",
                bool isAllowedProxy = true, string proxyUrl = "http://119.9.75.103:65531", int timeOut = 4000) {
            try {
                string RecvStr = "";
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                byte[] byteArray = Encoding.UTF8.GetBytes(String_Send);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(remote + urlPath);
                if (isAllowedProxy
                    && !string.IsNullOrEmpty(proxyUrl)) {
                    System.Net.WebProxy proxy = new WebProxy(new Uri(proxyUrl));//指定的ip和端口
                    WebReq.Proxy = proxy;

                }

                WebReq.Method = method;
                if (headers != null && headers.Count > 0) {
                    foreach (var header in headers) {
                        WebReq.Headers.Add(header.Key, header.Value);
                    }
                }
                WebReq.ContentType = ContentType;
                WebReq.ContentLength = byteArray.Length;
                WebReq.Timeout = timeOut;//

                using (Stream reqStream = WebReq.GetRequestStream()) {
                    await reqStream.WriteAsync(byteArray, 0, byteArray.Length);
                }

                if (wait) {
                    using (var WebRecv = await WebReq.GetResponseAsync())
                    using (var reader = new StreamReader(WebRecv.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"))) {
                        string sLine = "";
                        while (sLine != null) {
                            sLine = await reader.ReadLineAsync();
                            if (sLine != null)
                                RecvStr += sLine;
                        }
                        RecvStr = RecvStr.Trim();
                        return RecvStr;
                    }

                }
                return RecvStr;
            }
            catch (Exception ex) {

                return ex.Message;
            }
        }


        /// <summary>
        /// 只处理post 请求 异步请求
        /// 如果isAllowedProxy= true 且 proxyUrl 有值，才会使用代理服务去请求游戏平台，
        /// 不然直接使用当前服务器的ip
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="wait"></param>
        /// <param name="String_Send"></param>
        /// <param name="urlPath"></param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        public static async Task<string> RelayFormWithoutWebException(string remote, string method, Dictionary<string, string> headers, bool wait,
                string String_Send, string urlPath, string ContentType = "application/x-www-form-urlencoded",
                bool isAllowedProxy = true, string proxyUrl = "http://119.9.75.103:65531", int timeOut=4000) {
            try {
                string RecvStr = "";
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                byte[] byteArray = Encoding.UTF8.GetBytes(String_Send);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(remote + urlPath);
                if (isAllowedProxy
                    && !string.IsNullOrEmpty(proxyUrl)) {
                    System.Net.WebProxy proxy = new WebProxy(new Uri(proxyUrl));//指定的ip和端口
                    WebReq.Proxy = proxy;

                }

                WebReq.Method = method;
                if (headers != null && headers.Count > 0) {
                    foreach (var header in headers) {
                        WebReq.Headers.Add(header.Key, header.Value);
                    }
                }
                WebReq.ContentType = ContentType;
                WebReq.ContentLength = byteArray.Length;
                WebReq.Timeout = timeOut;//
                using (var stream = WebReq.GetRequestStream()) {
                    await stream.WriteAsync(byteArray, 0, byteArray.Length);
                }

                if (wait) {

                    using (var WebRecv = WebReq.GetResponse())
                    {
                        using (var reader = new StreamReader(WebRecv.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8")))
                        {
                            string sLine = "";
                            while (sLine != null)
                            {
                                sLine = await reader.ReadLineAsync();
                                if (sLine != null)
                                    RecvStr += sLine;
                            }
                            RecvStr = RecvStr.Trim();
                            return RecvStr;
                        }
                    }
                }
                return RecvStr;
            }
            catch (Exception ex) {
                return ex.Message; 
            }
        }


        /// <summary>
        /// 只处理post 请求 异步请求
        /// 如果isAllowedProxy= true 且 proxyUrl 有值，才会使用代理服务去请求游戏平台，
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="wait"></param>
        /// <param name="String_Send"></param>
        /// <param name="urlPath"></param>
        /// <param name="ContentType"></param>
        /// <param name="isAllowedProxy"></param>
        /// <param name="proxyUrl"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static async Task<string> UploadDataWithoutWebException(string remote, string method, Dictionary<string, string> headers, bool wait,
                string String_Send, string urlPath, string ContentType = "application/x-www-form-urlencoded",
                bool isAllowedProxy = true, string proxyUrl = "http://119.9.75.103:65531", int timeOut = 4000)
        {
            try
            {
                await Task.Delay(1);
                using (var client = new CustomWebClient())
                {
                    if (headers != null && headers.Count > 0)
                    {
                        foreach (var header in headers)
                        {
                            client.Headers.Add(header.Key, header.Value);
                        }
                    }

                    // 1. 设置超时 (例如 30 秒：30000)  
                    client.Timeout = timeOut;
                    if (isAllowedProxy
                        && !string.IsNullOrEmpty(proxyUrl))
                    {
                        // 2. 设置代理
                        var proxy = new WebProxy(proxyUrl);
                        // 如果代理需要用户名密码
                        // proxy.Credentials = new NetworkCredential("user", "password");
                        client.Proxy = proxy;
                    }
                    client.Headers.Add(HttpRequestHeader.ContentType, ContentType);
                    byte[] data = Encoding.UTF8.GetBytes(String_Send);
                    byte[] repData = client.UploadData(remote, "POST", data);
                    string content = Encoding.UTF8.GetString(repData);
                    return content;
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 只处理post 请求 异步请求 (返回响应数据的头，body , code)
        /// 如果isAllowedProxy= true 且 proxyUrl 有值，才会使用代理服务去请求游戏平台，
        /// 不然直接使用当前服务器的ip
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="wait"></param>
        /// <param name="String_Send"></param>
        /// <param name="urlPath"></param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        public static async Task<MYRequestResponse> RelayFormWithoutWebExceptionBHSC(string remote, string method, Dictionary<string, string> headers, bool wait,
                string String_Send, string urlPath, string ContentType = "application/x-www-form-urlencoded",
                bool isAllowedProxy = true, string proxyUrl = "http://119.9.75.103:65531", int timeOut = 4000)
        {
            try
            {
                string RecvStr = "";
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                byte[] byteArray = Encoding.UTF8.GetBytes(String_Send);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(remote + urlPath);
                if (isAllowedProxy
                    && !string.IsNullOrEmpty(proxyUrl))
                {
                    System.Net.WebProxy proxy = new WebProxy(new Uri(proxyUrl));//指定的ip和端口
                    WebReq.Proxy = proxy;

                }

                WebReq.Method = method;
                if (headers != null && headers.Count > 0)
                {
                    foreach (var header in headers)
                    {
                        WebReq.Headers.Add(header.Key, header.Value);
                    }
                }
                WebReq.ContentType = ContentType;
                WebReq.ContentLength = byteArray.Length;
                WebReq.Timeout = timeOut;//
                using (var stream = WebReq.GetRequestStream())
                {
                    await stream.WriteAsync(byteArray, 0, byteArray.Length);
                }

                var responseHeaders = new Dictionary<string, string>();

                if (wait)
                {

                    using (var WebRecv = WebReq.GetResponse())
                    {
                        if (WebRecv.Headers != null && WebRecv.Headers.Count > 0)
                        {
                            foreach (var key in WebRecv.Headers.AllKeys)
                            {
                                responseHeaders.Add(key, WebRecv.Headers[key]);
                            }
                        }
                        using (var reader = new StreamReader(WebRecv.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8")))
                        {
                            string sLine = "";
                            while (sLine != null)
                            {
                                sLine = await reader.ReadLineAsync();
                                if (sLine != null)
                                    RecvStr += sLine;
                            }
                            RecvStr = RecvStr.Trim();
                            return new MYRequestResponse() { httpCode=200, headers = responseHeaders, responseContent = RecvStr };
                        }
                    }
                }
                return new MYRequestResponse() { httpCode = 200, headers = responseHeaders, responseContent = RecvStr }; ;
            }
            catch (Exception ex)
            {
                //return ex.Message;
                return new MYRequestResponse() { httpCode = 200, headers = new Dictionary<string, string>(), responseContent = ex.Message };
            }
        }


        /// <summary>
        /// 只处理post 请求 同步请求
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="wait"></param>
        /// <param name="String_Send"></param>
        /// <param name="urlPath"></param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        public static  string RelayFormA(string remote, string method, Dictionary<string, string> headers, bool wait,
                string String_Send, string urlPath, string ContentType = "application/x-www-form-urlencoded",
                bool isAllowedProxy = true, string proxyUrl = "http://119.9.75.103:65531", int timeOut=4000) {
            try {
                string RecvStr = "";
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                byte[] byteArray = Encoding.UTF8.GetBytes(String_Send);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(remote + urlPath);
                if (isAllowedProxy) {
                    System.Net.WebProxy proxy = new WebProxy(new Uri(proxyUrl));//指定的ip和端口
                    WebReq.Proxy = proxy;

                }

                WebReq.Method = method;
                if (headers != null && headers.Count > 0) {
                    foreach (var header in headers) {
                        WebReq.Headers.Add(header.Key, header.Value);
                    }
                }
                WebReq.ContentType = ContentType;
                WebReq.ContentLength = byteArray.Length;
                WebReq.Timeout = timeOut;//
                using (var stream = WebReq.GetRequestStream()) {
                    stream.Write(byteArray, 0, byteArray.Length);
                }


                if (wait) {

                    using (var WebRecv = WebReq.GetResponse())
                    using (var reader = new StreamReader(WebRecv.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"))) {
                        string sLine = "";
                        while (sLine != null) {
                            sLine = reader.ReadLine();
                            if (sLine != null)
                                RecvStr += sLine;
                        }
                        RecvStr = RecvStr.Trim();
                        return RecvStr;
                    }

                }
                return RecvStr;
            }
            catch (WebException ex) {
                return printResponse(ex.Response as HttpWebResponse);
            }
        }

        public static  string RelayFormAWithoutWebException(string remote, string method, Dictionary<string, string> headers, bool wait,
                string String_Send, string urlPath, string ContentType = "application/x-www-form-urlencoded",
                bool isAllowedProxy = true, string proxyUrl = "http://119.9.75.103:65531", int timeOut=4000) {
            try {
                string RecvStr = "";
                ServicePointManager.ServerCertificateValidationCallback = 
                    new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);

                byte[] byteArray = Encoding.UTF8.GetBytes(String_Send);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(remote + urlPath);
                if (isAllowedProxy
                    && !string.IsNullOrEmpty(proxyUrl)) {
                    System.Net.WebProxy proxy = new WebProxy(new Uri(proxyUrl));//指定的ip和端口
                    WebReq.Proxy = proxy;

                }

                WebReq.Method = method;
                if (headers != null && headers.Count > 0) {
                    foreach (var header in headers) {
                        WebReq.Headers.Add(header.Key, header.Value);
                    }
                }
                WebReq.ContentType = ContentType;
                WebReq.ContentLength = byteArray.Length;
                WebReq.Timeout = timeOut;//

                using (var stream = WebReq.GetRequestStream()) {
                    stream.Write(byteArray, 0, byteArray.Length);
                }

                if (wait) {
                    using (var WebRecv = WebReq.GetResponse())
                    using (var reader = new StreamReader(WebRecv.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"))) {
                        string sLine = "";
                        while (sLine != null) {
                            sLine = reader.ReadLine();
                            if (sLine != null)
                                RecvStr += sLine;
                        }
                        RecvStr = RecvStr.Trim();
                        return RecvStr;
                    }
                }
                return RecvStr;
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="wait"></param>
        /// <param name="String_Send"></param>
        /// <param name="urlPath"></param>
        /// <param name="ContentType"></param>
        /// <param name="isAllowedProxy"></param>
        /// <param name="proxyUrl"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string,string>> QueryResponseCookieForm(string remote, string method, Dictionary<string, string> headers, bool wait,
                string String_Send, string urlPath, string ContentType = "application/x-www-form-urlencoded",
                bool isAllowedProxy = true, string proxyUrl = "http://119.9.75.103:65531", int timeOut=4000)
        {
            var cookies = new Dictionary<string, string>();
            await Task.Delay(1);
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(remote);
                if (isAllowedProxy)
                {
                    //new proxy  103.102.46.33
                    //old proxy 119.9.75.103
                    System.Net.WebProxy proxy = new WebProxy(new Uri(proxyUrl));//指定的ip和端口
                    request.Proxy = proxy;

                }
                if (headers != null && headers.Count > 0)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }
                request.Timeout = timeOut;
                request.CookieContainer = new CookieContainer();
                using (var response = (HttpWebResponse)(request.GetResponse()))
                {
                    
                    foreach (Cookie cook in response.Cookies)
                    {
                        cookies.Add(cook.Name, cook.Value);
                    }
                    cookies.Add("StatusCode", ((int)response.StatusCode).ToString());
                }
                return cookies;
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                cookies.Add("StatusCode", ((int)response.StatusCode).ToString());
                return cookies;
            }
        }
        /// <summary>
        /// get 请求返回cookies 和响应数据
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="wait"></param>
        /// <param name="String_Send"></param>
        /// <param name="urlPath"></param>
        /// <param name="ContentType"></param>
        /// <param name="isAllowedProxy"></param>
        /// <param name="proxyUrl"></param>
        /// <returns></returns>
        public static async Task<Tuple<Dictionary<string, object>,string>> QueryResponseWithCookieForm(string remote, string method, Dictionary<string, string> headers, bool wait,
                string String_Send, string urlPath, string ContentType = "application/x-www-form-urlencoded",
                bool isAllowedProxy = true, string proxyUrl = "http://119.9.75.103:65531", int timeOut=4000)
        {
            var cookies = new Dictionary<string, object>();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(remote);
                if (headers != null && headers.Count > 0)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                var jsonString = string.Empty;
                request.Timeout = timeOut;
                request.CookieContainer = new CookieContainer();
                using (var response = (HttpWebResponse)(request.GetResponse()))
                {
                    using (var receiveStream = response.GetResponseStream())
                    using (var readStream = new StreamReader(receiveStream, Encoding.UTF8)) {
                        jsonString = await readStream.ReadToEndAsync();

                        foreach (Cookie cook in response.Cookies) {
                            cookies.Add(cook.Name, cook);
                        }

                        for (int i = 0; i < response.Headers.Count; ++i) {
                            if (response.Headers.Keys[i].Equals("Authorization")) {
                                cookies.Add("Authorization", response.Headers[i]);
                                break;
                            }
                        }
                        cookies.Add("StatusCode", ((int)response.StatusCode).ToString());
                    }
                    
                }
                return new Tuple<Dictionary<string, object>, string>(cookies, jsonString);
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                cookies.Add("StatusCode", ((int)response.StatusCode).ToString());
                return new Tuple<Dictionary<string, object>, string>(cookies, "");
            }
        }
        
        private static string GetCookieValue(string cookie)
        {
            Regex regex = new Regex("=.*?;");
            Match value = regex.Match(cookie);
            string cookieValue = value.Groups[0].Value;
            return cookieValue.Substring(1, cookieValue.Length - 2);
        }

        private static string GetCookieName(string cookie)
        {
            Regex regex = new Regex("sulcmiswebpac.*?");
            Match value = regex.Match(cookie);
            return value.Groups[0].Value;
        }



        public static async Task<string> RelayBBinForm(string remote, string method, Dictionary<string, string> headers, bool wait,
                string String_Send, string urlPath, string ContentType = "application/x-www-form-urlencoded",
                bool isAllowedProxy = true, string proxyUrl = "http://119.9.75.103:65531", int timeOut = 4000)
        {
            try
            {
                string RecvStr = "";
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                byte[] byteArray = Encoding.UTF8.GetBytes(String_Send);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(remote);
                if (isAllowedProxy 
                    && !string.IsNullOrEmpty(proxyUrl)) {
                    //new proxy  103.102.46.33
                    //old proxy 119.9.75.103
                    System.Net.WebProxy proxy = new WebProxy(new Uri(proxyUrl));//指定的ip和端口
                    WebReq.Proxy = proxy;

                }

                WebReq.Method = method;
                WebReq.ContentType = ContentType;
                WebReq.ContentLength = byteArray.Length;
                WebReq.Timeout = timeOut;//

                using (var stream = WebReq.GetRequestStream()) {
                    await stream.WriteAsync(byteArray, 0, byteArray.Length);
                }
                if (wait)
                {
                    using (var WebRecv = WebReq.GetResponse())
                    using (var reader = new StreamReader(WebRecv.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"))) {
                        string sLine = "";
                        while (sLine != null) {
                            sLine = await reader.ReadLineAsync();
                            if (sLine != null)
                                RecvStr += sLine;
                        }
                        RecvStr = RecvStr.Trim();
                        return RecvStr;
                    }
                   
                }
                return RecvStr;
            }
            catch (WebException ex)
            {
                using (var resposneStream = ex.Response.GetResponseStream())
                using (var streamReader = new StreamReader(resposneStream, Encoding.UTF8)) {
                    string responseData = streamReader.ReadToEnd();
                    return responseData;
                }
            }
        }

        /// <summary>
        /// 只处理get请求
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="headers"></param>
        /// <param name="String_Send"></param>
        /// <param name="urlPath"></param>
        /// <param name="isAllowedProxy"></param>
        /// <param name="ContentType"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static async Task<string> Relay(string remoteUrl, Dictionary<string, string> headers,
            string String_Send, string urlPath, bool isAllowedProxy=true, 
            string ContentType = "application/json", string method="get", string proxyUrl = "http://119.9.75.103:65531", int timeOut=4000)
        {
            try
            {
                string RecvStr = "";
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                byte[] byteArray = Encoding.UTF8.GetBytes(String_Send);
                var requestUrlString = string.Format("{0}{1}", remoteUrl, urlPath);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(requestUrlString);
                if(isAllowedProxy 
                    && !string.IsNullOrEmpty(proxyUrl))
                {
                    //new proxy  103.102.46.33
                    //old proxy 119.9.75.103
                    System.Net.WebProxy proxy = new WebProxy(new Uri(proxyUrl));//指定的ip和端口
                    WebReq.Proxy = proxy;
                }
                if (headers != null && headers.Count > 0)
                {
                    foreach (var header in headers)
                    {
                        WebReq.Headers.Add(header.Key, header.Value);
                    }
                }
                WebReq.Method = method;
                WebReq.Accept = ContentType;
                WebReq.ContentType = ContentType;// "text/xml";
                WebReq.ContentLength = byteArray.Length;
                WebReq.Timeout = timeOut;
                if (string.Compare("get", method, true) != 0)
                {
                    using (var stream = WebReq.GetRequestStream()) {
                        await stream.WriteAsync(byteArray, 0, byteArray.Length);
                    }
                }

                using (var WebRecv = (HttpWebResponse)WebReq.GetResponse())
                using (var reader = new StreamReader(WebRecv.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"))) {
                    string sLine = "";
                    while (sLine != null) {
                        sLine = await reader.ReadLineAsync(); // reader.ReadLine();
                        if (sLine != null)
                            RecvStr += sLine;
                    }
                    RecvStr = RecvStr.Trim();
                    var code = (int)WebRecv.StatusCode;
                    return RecvStr;
                }

            }
            catch (WebException ex)
            {
                //string status = null;
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                //if (response.StatusCode == HttpStatusCode.NotFound)
                //    status = "404";
                //if (response.StatusCode == HttpStatusCode.InternalServerError)
                //    status = "500";
                //if (response.StatusCode == HttpStatusCode.BadGateway)
                //    status = "502";
                return ((int)response.StatusCode).ToString();
            }
        }



        /// <summary>
        /// 只处理get请求   WithoutWebException
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="headers"></param>
        /// <param name="String_Send"></param>
        /// <param name="urlPath"></param>
        /// <param name="isAllowedProxy"></param>
        /// <param name="ContentType"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static async Task<string> RelayWithoutWebException(string remoteUrl, Dictionary<string, string> headers,
            string String_Send, string urlPath, bool isAllowedProxy = true,
            string ContentType = "application/json", string method = "get", string proxyUrl = "http://119.9.75.103:65531", int timeOut = 4000) {
            try {
                string RecvStr = "";
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                byte[] byteArray = Encoding.UTF8.GetBytes(String_Send);
                var requestUrlString = string.Format("{0}{1}", remoteUrl, urlPath);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(requestUrlString);
                if (isAllowedProxy
                    && !string.IsNullOrEmpty(proxyUrl)) {
                    //new proxy  103.102.46.33
                    //old proxy 119.9.75.103
                    System.Net.WebProxy proxy = new WebProxy(new Uri(proxyUrl));//指定的ip和端口
                    WebReq.Proxy = proxy;
                }
                if (headers != null && headers.Count > 0) {
                    foreach (var header in headers) {
                        WebReq.Headers.Add(header.Key, header.Value);
                    }
                }
                WebReq.Method = method;
                WebReq.Accept = ContentType;
                WebReq.ContentType = ContentType;// "text/xml";
                WebReq.ContentLength = byteArray.Length;
                WebReq.Timeout = timeOut;
                if (string.Compare("get", method, true) != 0) {
                    using (var stream = WebReq.GetRequestStream()) {
                        await stream.WriteAsync(byteArray, 0, byteArray.Length);
                    }
                }

                using (var WebRecv = (HttpWebResponse)WebReq.GetResponse())
                using (var reader = new StreamReader(WebRecv.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"))) {
                    string sLine = "";
                    while (sLine != null) {
                        sLine = await reader.ReadLineAsync(); // reader.ReadLine();
                        if (sLine != null)
                            RecvStr += sLine;
                    }
                    RecvStr = RecvStr.Trim();
                    var code = (int)WebRecv.StatusCode;
                    return RecvStr;
                }

            }
            catch (Exception ex) {
                return ex.Message;
            }
        }





        public static async Task<MYRequestResponse> MyRelay(string remoteUrl, Dictionary<string, string> headers,
          string String_Send, string urlPath, bool isAllowedProxy = true,
          string ContentType = "application/json", string method = "get", string proxyUrl = "http://119.9.75.103:65531", int timeOut=4000) {
            try {
                string RecvStr = "";
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                byte[] byteArray = Encoding.UTF8.GetBytes(String_Send);
                var requestUrlString = string.Format("{0}{1}", remoteUrl, urlPath);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(requestUrlString);
                if (isAllowedProxy 
                    && !string.IsNullOrEmpty(proxyUrl)) {
                    //new proxy  103.102.46.33
                    //old proxy 119.9.75.103
                    System.Net.WebProxy proxy = new WebProxy(new Uri(proxyUrl));//指定的ip和端口
                    WebReq.Proxy = proxy;
                }
                if (headers != null && headers.Count > 0) {
                    foreach (var header in headers) {
                        WebReq.Headers.Add(header.Key, header.Value);
                    }
                }
                WebReq.Method = method;
                WebReq.Accept = ContentType;
                WebReq.ContentType = ContentType;// "text/xml";
                WebReq.ContentLength = byteArray.Length;
                WebReq.Timeout = timeOut;
                if (string.Compare("get", method, true) != 0) {
                    using (var stream = WebReq.GetRequestStream()) {
                        await stream.WriteAsync(byteArray, 0, byteArray.Length);
                    }
                }
                using (var WebRecv = (HttpWebResponse)WebReq.GetResponse())
                using (var reader = new StreamReader(WebRecv.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"))) {
                    string sLine = "";
                    while (sLine != null) {
                        sLine = await reader.ReadLineAsync(); // reader.ReadLine();
                        if (sLine != null)
                            RecvStr += sLine;
                    }
                    RecvStr = RecvStr.Trim();
                    return new MYRequestResponse() { httpCode = (int)WebRecv.StatusCode, responseContent = RecvStr };
                }

            }
            catch (WebException ex) {
                //string status = null;
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                //if (response.StatusCode == HttpStatusCode.NotFound)
                //    status = "404";
                //if (response.StatusCode == HttpStatusCode.InternalServerError)
                //    status = "500";
                //if (response.StatusCode == HttpStatusCode.BadGateway)
                //    status = "502";
                //return ((int)response.StatusCode).ToString();
                return new MYRequestResponse() { httpCode = (int)response.StatusCode, responseContent = "" };
            }
        }

        public static async Task<string> RelayTTGForm(string remote, string method, string header, bool wait,
            string String_Send, string urlPath, string ContentType = "application/x-www-form-urlencoded", 
            bool isAllowedProxy = true, string proxyUrl = "http://119.9.75.103:65531", int timeOut=4000)
        {
            try
            {
                string RecvStr = "";
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                byte[] byteArray = Encoding.UTF8.GetBytes(String_Send);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(remote);
                if (isAllowedProxy 
                    && !string.IsNullOrEmpty(proxyUrl))
                {
                    //new proxy  103.102.46.33
                    //old proxy 119.9.75.103
                    System.Net.WebProxy proxy = new WebProxy(new Uri(proxyUrl));//指定的ip和端口
                    WebReq.Proxy = proxy;
                }
                WebReq.Method = method;
                WebReq.ContentType = ContentType;
                WebReq.ContentLength = byteArray.Length;
                WebReq.Timeout = timeOut;// 
                using (var stream = WebReq.GetRequestStream()) {
                    await stream.WriteAsync(byteArray, 0, byteArray.Length);
                }
               
                if (wait)
                {
                    using (var WebRecv = WebReq.GetResponse())
                    using (var reader = new StreamReader(WebRecv.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"))) {
                        string sLine = "";
                        while (sLine != null) {
                            sLine = await reader.ReadLineAsync();
                            if (sLine != null)
                                RecvStr += sLine;
                        }
                        RecvStr = RecvStr.Trim();
                        return RecvStr;
                    }
                   
                }
                return RecvStr;
            }
            catch (WebException ex)
            {
                return printResponse(ex.Response as HttpWebResponse);
            }
        }
        public static async Task<string> RelayKYSend(string remote, string method, Dictionary<string, string> headers, bool wait,
                string String_Send, string urlPath, string ContentType = "application/x-www-form-urlencoded",
                bool isAllowedProxy = true, string proxyUrl = "http://119.9.75.103:65531", int timeOut=4000)
        {
            try
            {
                string RecvStr = "";
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(remote);
                if (isAllowedProxy 
                    && !string.IsNullOrEmpty(proxyUrl))
                {
                    //new proxy  103.102.46.33
                    //old proxy 119.9.75.103
                    System.Net.WebProxy proxy = new WebProxy(new Uri(proxyUrl));//指定的ip和端口
                    WebReq.Proxy = proxy;
                }
                WebReq.Method = method;
                WebReq.ContentType = ContentType;
                WebReq.Timeout = timeOut;// 150

                if (wait)
                {
                    using (var WebRecv = WebReq.GetResponse())
                    using (var reader = new StreamReader(WebRecv.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"))) {
                        string sLine = "";
                        while (sLine != null) {
                            sLine = await reader.ReadLineAsync();
                            if (sLine != null)
                                RecvStr += sLine;
                        }
                        RecvStr = RecvStr.Trim();
                        return RecvStr;
                    }
                    
                }
                return RecvStr;
            }
            catch (WebException ex)
            {
                return printResponse(ex.Response as HttpWebResponse);
            }
        }
        public static async Task<string> RelayTTGSend(string remote, string method, string header, bool wait,
            string String_Send, string urlPath, string ContentType = "application/x-www-form-urlencoded",
            bool isAllowedProxy = true, string proxyUrl = "http://119.9.75.103:65531", int timeOut=4000)
        {
            try
            {
                string RecvStr = "";
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(remote);
                if (isAllowedProxy 
                    && !string.IsNullOrEmpty(proxyUrl))
                {
                    //new proxy  103.102.46.33
                    //old proxy 119.9.75.103
                    System.Net.WebProxy proxy = new WebProxy(new Uri(proxyUrl));//指定的ip和端口
                    WebReq.Proxy = proxy;
                }
                WebReq.Method = method;
                WebReq.ContentType = ContentType;
                WebReq.Timeout = timeOut;// 

                if (wait)
                {
                    using (var WebRecv = WebReq.GetResponse())
                    using (var reader = new StreamReader(WebRecv.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"))) {
                        string sLine = "";
                        while (sLine != null) {
                            sLine = await reader.ReadLineAsync();
                            if (sLine != null)
                                RecvStr += sLine;
                        }
                        RecvStr = RecvStr.Trim();
                        return RecvStr;
                    }
                   
                }
                return RecvStr;
            }
            catch (WebException ex)
            {
                return printResponse(ex.Response as HttpWebResponse);
            }
        }
      
    }
}
