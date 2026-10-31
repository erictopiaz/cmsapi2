using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace UtilsLib
{
    /// <summary>
    /// 统一返回格式
    /// 处理结果更新为rcode ，code 为object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResultModel<T>
    {
        //public string _code;
        ///// <summary>
        ///// true or false
        ///// </summary>
        //public string code {
        //    set
        //    {
        //        _code = value;
        //        rCode = value.Equals("true", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
        //    }
        //    get { return _code; }
        //}

        public object _code;
        /// <summary>
        /// true or false ，  1或0
        /// </summary>
        public object code
        {
            set
            {
                _code = value;
                rCode = (value.ToString().Equals("true", StringComparison.OrdinalIgnoreCase)
                    || value.ToString().Equals("1", StringComparison.OrdinalIgnoreCase)) ? 1 : 0;
            }
            get { return _code; }
        }

        /// <summary>
        /// 代理处理结果
        /// 1 成功
        /// 0 失败
        /// </summary>
        public int rCode { set; get; }

        /// <summary>
        /// 返回结果描述
        /// </summary>
        public string message { set; get; }
        /// <summary>
        /// 返回结果描述代码
        /// </summary>
        public string message_code { set; get; }

        public List<ReponseMsgLangModel> msgLangs { set; get; }

        public int responseCode { set; get; }
        /// <summary>
        /// 返回数据结构
        /// </summary>
        public T data { set; get; }


        public static ApiResultModel<T> GetResult(object code, string message, string message_code, 
            T data=default(T), int responseCode = 200,
            List<ReponseMsgLangModel> msgLangs = null)
        {
            return new ApiResultModel<T>
            {
                code = code,
                message = message,
                message_code = message_code,
                responseCode = responseCode,
                data = data,
                msgLangs = msgLangs != null ? msgLangs : new List<ReponseMsgLangModel>()
            };
        }
    }

   

    /// <summary>
    /// 请求响应数据 
    /// 处理结果更新为rcode ，code 为object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponseModel<T>
    {
        //private int _code;
        ///// <summary>
        ///// 1 成功
        ///// 0 失败
        ///// </summary>
        //public int code { 
        //    set { 
        //        _code = value;
        //        rCode = value;
        //    } 
        //    get { return _code; }
        //}

        private object _code;
        /// <summary>
        /// true or false  或   1或0
        /// </summary>
        public object code
        {
            set
            {
                _code = value;
                rCode = (value.ToString().Equals("true", StringComparison.OrdinalIgnoreCase)
                    || value.ToString().Equals("1", StringComparison.OrdinalIgnoreCase)) ? 1 : 0;
            }
            get { return _code; }
        }

        /// <summary>
        /// 代理处理结果
        /// 1 成功
        /// 0 失败
        /// </summary>
        public int rCode { set; get; }

        /// <summary>
        /// 返回结果描述
        /// </summary>
        public string message { set; get; }
        /// <summary>
        /// 返回结果描述代码
        /// </summary>
        public string message_code { set; get; }
        /// <summary>
        /// 1503:游戏维护中
        /// </summary>
        public int responseCode { set; get; } = 200;
        public string used_time { set; get; }
        /// <summary>
        /// 返回数据结构
        /// </summary>
        public T data { set; get; }

        public List<ReponseMsgLangModel> msgLangs { set; get; } 

        public static ApiResponseModel<T> GetResult(object code, string message, string message_code, 
            T data = default(T), int responseCode = 200, string used_time="", 
            List<ReponseMsgLangModel> msgLangs = null)
        {
            return new ApiResponseModel<T>
            {
                code = code,
                message = message,
                message_code = message_code,
                responseCode = responseCode,
                data = data,
                used_time = used_time,
                msgLangs = msgLangs != null ? msgLangs : new List<ReponseMsgLangModel>()
            };
        }
    }

    /// <summary>
    /// 返回消息的语系
    /// </summary>
    public class ReponseMsgLangModel {
        public int lang { set; get; }
        public string body { set; get; }
    }

    public class AgentApiResultModel<T,K> : ApiResultModel<T>
    {
        /// <summary>
        /// 代理层级使用
        /// 返水账号的串串如：a,b,c
        /// </summary>
        public string levels { set; get; }
        /// <summary>
        /// 统计数据
        /// </summary>
        public K total { set; get; }
    }

    public class AgentApiReponseModel<T, K> : ApiResponseModel<T>
    {
        /// <summary>
        /// 代理层级使用
        /// 返水账号的串串如：a,b,c
        /// </summary>
        public string levels { set; get; }
        /// <summary>
        /// 统计数据
        /// </summary>
        public K total { set; get; }
    }

    public class PageModel<T>
    {
        /// <summary>
        /// 总记录数据
        /// </summary>
        public int TotalRecord { set; get; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { set; get; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { set; get; }
        /// <summary>
        /// 统计数据：时间段内的 查询结果总的统计数据
        /// </summary>
        public object totalData { set; get; }
        /// <summary>
        /// 分页数据
        /// </summary>
        public List<T> modelList { set; get; }
        
    }
    public class PageTupleModel<T,K>
    {
        /// <summary>
        /// 总记录数据
        /// </summary>
        public int TotalRecord { set; get; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { set; get; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { set; get; }
        /// <summary>
        /// 统计数据：时间段内的 查询结果总的统计数据
        /// </summary>
        public K totalData { set; get; }
        /// <summary>
        /// 分页数据
        /// </summary>
        public List<T> modelList { set; get; }

    }

    /// <summary>
    /// API响应代码
    /// </summary>
    public static class ApiResponseCode
    {
        /// <summary>
        /// 失败
        /// </summary>
        public const int FAIL = 0;
        /// <summary>
        /// 成功
        /// </summary>
        public const int SUCCESS = 1;
    }
}
