using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UtilsLib.RequestHelper
{
    public static class HttpClientExt
    {
        private static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static async Task<ApiClient<T>> Post<T>(this IHttpClientFactory httpClientFactory,string baseUrl, string url, object data)
        {
            string _url = $"{baseUrl}{url}";
            string json = JsonConvert.SerializeObject(data, _jsonSettings);
            HttpClient httpClient = httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(2);
            using (HttpContent requestContent = new StringContent(json, Encoding.UTF8, "application/json"))
            using (HttpResponseMessage response = await httpClient.PostAsync(_url, requestContent))
            using (HttpContent responseContent = response.Content)
            {
                string result = await responseContent.ReadAsStringAsync();
                ApiClient<T> obj = new ApiClient<T>()
                {
                    HttpStatusCode = response.StatusCode,
                    ContentType = responseContent.Headers.ContentType.MediaType
                };

                try
                {
                    JsonConvert.PopulateObject(result, obj);
                    setError(obj, result);
                    return obj;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static async Task<ApiClient<T>> PatchAsync<T>(this IHttpClientFactory httpClientFactory, string baseUrl, string url, object data)
        {
            string _url = $"{baseUrl}{url}";
            string json = JsonConvert.SerializeObject(data, _jsonSettings);
            HttpClient httpClient = httpClientFactory.CreateClient();
            using (HttpContent requestContent = new StringContent(json, Encoding.UTF8, "application/json"))
            using (HttpResponseMessage response = await httpClient.PatchAsync(_url, requestContent))
            using (HttpContent responseContent = response.Content)
            {
                string result = await responseContent.ReadAsStringAsync();
                ApiClient<T> obj = new ApiClient<T>()
                {
                    HttpStatusCode = response.StatusCode,
                    ContentType = responseContent.Headers.ContentType.MediaType
                };

                try
                {
                    JsonConvert.PopulateObject(result, obj);
                    setError(obj, result);
                    return obj;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="baseUrl"></param>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        /// <param name="token"></param>
        /// <param name="timeOut">超时时间单位:秒，默认4秒</param>
        /// <returns></returns>
        public static async Task<string> PostAsync(this IHttpClientFactory httpClientFactory, 
            string baseUrl, string url, object data, 
            string contentType = "application/json", string token="", int timeOut=4)
        {
            try
            {
                string _url = $"{baseUrl}{url}";
                string json = JsonConvert.SerializeObject(data, _jsonSettings);
                HttpClient httpClient = httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(timeOut);
                var request = new HttpRequestMessage(HttpMethod.Post, _url);
               
                if (!string.IsNullOrEmpty(token))
                    request.Headers.Add("Authorization", token);

                using (var requestContent = new StringContent(json, Encoding.UTF8, contentType))
                {
                    request.Content = requestContent;
                    using (HttpResponseMessage response = await httpClient.SendAsync(request))
                    using (HttpContent responseContent = response.Content)
                    {
                        string result = await responseContent.ReadAsStringAsync();
                        return result;

                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                    return "请求超时";
                else
                    return "网络请求异常";
            }
            catch (Exception)
            {
                return "网络请求异常";
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="baseUrl"></param>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        /// <param name="token"></param>
        /// <param name="timeOut">超时时间单位:秒，默认4秒</param>
        /// <returns></returns>
        public static async Task<string> PostWithProxyAsync(this IHttpClientFactory httpClientFactory,
            string baseUrl, string url, object data,
            string contentType = "application/json", string token = "", int timeOut = 4, string proxyUrl= "http://119.9.75.103:8888")
        {
            try
            {
                string _url = $"{baseUrl}{url}";
                string json = JsonConvert.SerializeObject(data, _jsonSettings);
                var proxyHttpClientHandler = new HttpClientHandler
                {
                    Proxy = new WebProxy(proxyUrl),
                    UseProxy = true,
                };
                using(var httpClient = new HttpClient(proxyHttpClientHandler))
                {
                    httpClient.BaseAddress = new Uri(_url);
                    httpClient.Timeout = TimeSpan.FromSeconds(timeOut);
                    var request = new HttpRequestMessage(HttpMethod.Post, _url);

                    if (!string.IsNullOrEmpty(token))
                        request.Headers.Add("Authorization", token);

                    using (var requestContent = new StringContent(json, Encoding.UTF8, contentType))
                    {
                        request.Content = requestContent;
                        using (HttpResponseMessage response = await httpClient.SendAsync(request))
                        using (HttpContent responseContent = response.Content)
                        {
                            string result = await responseContent.ReadAsStringAsync();
                            return result;

                        }
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                    return "请求超时";
                else
                    return "网络请求异常";
            }
            catch (Exception)
            {
                return "网络请求异常";
            }

        }

        public static async Task<string> PostWithHeadersAsync(this IHttpClientFactory httpClientFactory,
            string baseUrl, string url, object data,
            string contentType = "application/json", Dictionary<string, string> headers = null)
        {
            try
            {
                string _url = $"{baseUrl}{url}";
                string json = JsonConvert.SerializeObject(data, _jsonSettings);
                HttpClient httpClient = httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(4);

                var request = new HttpRequestMessage(HttpMethod.Post, _url);
                if (headers != null)
                {
                    foreach(var d in headers)
                    {
                        request.Headers.Add(d.Key, d.Value);
                    }
                }

                using (var requestContent = new StringContent(json, Encoding.UTF8, contentType))
                {
                    request.Content = requestContent;
                    using (HttpResponseMessage response = await httpClient.SendAsync(request))
                    using (HttpContent responseContent = response.Content)
                    {
                        string result = await responseContent.ReadAsStringAsync();
                        return result;

                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                    return "请求超时";
                else
                    return "网络请求异常";
            }
            catch (Exception)
            {
                return "网络请求异常";
            }

        }


        public static async Task<string> PostStringAsync(this IHttpClientFactory httpClientFactory,
            string baseUrl, string url, string data,
            string contentType = "application/json", int timeOut=4)
        {
            try
            {
                string _url = $"{baseUrl}{url}";
                HttpClient httpClient = httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(timeOut);
                var request = new HttpRequestMessage(HttpMethod.Post, _url);
                using (var requestContent = new StringContent(data.ToString(), Encoding.UTF8, contentType))
                {
                    request.Content = requestContent;
                    using (HttpResponseMessage response = await httpClient.SendAsync(request))
                    using (HttpContent responseContent = response.Content)
                    {
                        string result = await responseContent.ReadAsStringAsync();
                        return result;

                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                    return "请求超时";
                else
                    return "网络请求异常";
            }
            catch (Exception)
            {
                return "网络请求异常";
            }
        }
        /// <summary>
        /// post  strring&string 使用proxy
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="baseUrl"></param>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static async Task<string> PostStringWithProxyAsync(this IHttpClientFactory httpClientFactory,
            string baseUrl, string url, string data,
            string contentType = "application/json", int timeOut =4, string proxyUrl= "http://119.9.75.103:8888")
        {
            try
            {
                string _url = $"{baseUrl}{url}";
                var proxyHttpClientHandler = new HttpClientHandler
                {
                    Proxy = new WebProxy(proxyUrl),
                    UseProxy = true,
                };
                
                using (var httpClient = new HttpClient(proxyHttpClientHandler))
                {
                    httpClient.BaseAddress = new Uri(_url);
                    httpClient.Timeout = TimeSpan.FromSeconds(timeOut);
                    var request = new HttpRequestMessage(HttpMethod.Post, _url);
                    using (var requestContent = new StringContent(data.ToString(), Encoding.UTF8, contentType))
                    {
                        request.Content = requestContent;
                        using (HttpResponseMessage response = await httpClient.SendAsync(request))
                        using (HttpContent responseContent = response.Content)
                        {
                            string result = await responseContent.ReadAsStringAsync();
                            return result;
                        }
                    }
                }

                
            }
            catch (OperationCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                    return "请求超时";
                else
                    return "网络请求异常";
            }
            catch (Exception)
            {
                return "网络请求异常";
            }
        }

        public static async Task<string> GetAsync(this IHttpClientFactory httpClientFactory, string token,
            string baseUrl, 
            string url, int Timeout=3)
        {
            try
            {
                string _url = $"{baseUrl}{url}";
                HttpClient httpClient = httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(Timeout);
                var request = new HttpRequestMessage(HttpMethod.Get, _url);
                request.Headers.Add("Authorization", token);
                using (HttpResponseMessage response = await httpClient.SendAsync(request))
                using (HttpContent responseContent = response.Content)
                {
                    string result = await responseContent.ReadAsStringAsync();
                    return result;
                }
            }
            catch (OperationCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                    return "请求超时";
                else
                    return "网络请求异常";
            }
            catch (Exception)
            {
                return "网络请求异常";
            }
        }
       

        static void setError<T>(ApiClient<T> obj, string json)
        {
            if (obj.Code != (int)Code.SUCCESS)
            {
                var erro = JsonConvert.DeserializeObject<dynamic>(json);
                if (erro.Data != null)
                    obj.ErrorData = erro.Data;
                else if (erro.data != null)
                    obj.ErrorData = erro.data;
                else if (erro.result != null)
                    obj.ErrorData = erro.result;
            }
        }
    }

    public class ApiClient<T>
    {
        public int Code { set; get; }
        public string ErrorData { set; get; }
        public HttpStatusCode HttpStatusCode { set; get; }
        public string ContentType { set; get; }
    }

    enum Code
    {
        SUCCESS=0,
        FAILED
    }

}