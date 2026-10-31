using LogLib.Helper;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using SmsLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UtilsLib;
using UtilsLib.RequestHelper;

namespace SmsLib.platforms
{
    /// <summary>
    /// 建周短信服务
    /// </summary>
    public class SmsJZ
    {
        private static JZMobileSvc.BusinessServiceClient jzMsgSvc = new JZMobileSvc.BusinessServiceClient();


        /// <summary>
        /// 短信平台返回，发送结果code
        /// 大于0 提交成功，该数字为本批次的任务ID，提交成功后请自行保存发送记录。
        /// </summary>
        public static readonly Dictionary<int, string> MessageErrorCode = new Dictionary<int, string>()
        {
            { -1, "余额不足" },
            { -2, "帐号或密码错误" },
            { -3, "连接服务商失败" },
            { -5, "其他错误，一般为网络问题，IP受限等" },
            { -6, "短信内容为空" },
            { -7, "目标号码为空" },
            { -9, "捕获未知异常" },
            { -10, "超过最大定时时间限制" },
            { -13, "没有权限使用该网关" },
            { -17, "没有提交权限，客户端帐号无法使用接口提交。或非绑定IP提交" },
            { -18, "提交参数名称不正确或确少参数" },

            { -19, "必须为POST提交" },
            { -20, "超速提交(非验证码类短信一般为每秒一次提交)" },
            { -21, "扩展参数不正确" },
            { -22, "Ip 被封停" },
            { -23, "国际短信号码必须“00”开头" },
            { -24, "userTaskID错误" },
            { -101, "发送短信处理异常" }
        };


        /// <summary>
        /// 呼叫JZ http api 发送短信
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password">md5 加密</param>
        /// <param name="phoneNumber"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static async Task<string> CallJZBatchApiHttp(string account, string password, string phoneNumber, string msg)
        {
            try
            {
                var myEncoding = Encoding.GetEncoding("utf-8");
                string param = HttpUtility.UrlEncode("account", myEncoding) +
                "=" + HttpUtility.UrlEncode(account, myEncoding) +
                "&" + HttpUtility.UrlEncode("password", myEncoding) +
                "=" + HttpUtility.UrlEncode(password, myEncoding) +
                "&" + HttpUtility.UrlEncode("destmobile", myEncoding) +
                "=" + HttpUtility.UrlEncode(phoneNumber, myEncoding) +
                "&" + HttpUtility.UrlEncode("msgText", myEncoding) +
                "=" + HttpUtility.UrlEncode(msg, myEncoding);
                
                var postBytes = Encoding.ASCII.GetBytes(param);

                var req = (HttpWebRequest)HttpWebRequest.Create("https://www.jianzhou.sh.cn/JianzhouSMSWSServer/http/sendBatchMessage");
                req.Method = "POST";
                req.ContentType ="application/x-www-form-urlencoded;charset=utf-8";
                req.ContentLength = postBytes.Length;
                //req.Timeout = 4000;

                using (var reqStream = req.GetRequestStream())
                {
                    await reqStream.WriteAsync(postBytes, 0, postBytes.Length);
                }
                var RecvStr = "";
                using (var WebRecv = req.GetResponse())
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
            catch (Exception)
            {
                return "-101";
            }
        }


        /// <summary>
        /// 呼叫JZ http api 发送短信 1
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="sampleCode"></param>
        /// <param name="account"></param>
        /// <param name="configData"></param>
        /// <param name="merchantAccount"></param>
        /// <param name="writeToFile"></param>
        /// <returns></returns>
        public static async Task<bool> SendJZhouMessageAsync(string phoneNumber, 
            string sampleCode, 
            string account,
            MessageConfigModel configData, 
            string merchantAccount,
            WriteToFile writeToFile=null)
        {
            #region 短信发送
            try
            {
                // 手机号码
                string msg = string.Format("验证码:{0}【{1}】", sampleCode, configData.signString);//string msg = string.Format("验证码:{0}【万象城体育】", sampleCode);
                if(merchantAccount.Equals("Boss",StringComparison.OrdinalIgnoreCase))
                    msg = string.Format("验证码:{0}【{1}】", sampleCode, configData.signString_boss);
                // 普通单发,注意前面必须为【】符号包含，置于头或者尾部。
                string json = string.Empty;
                var jz_account = configData.account;
                var jz_password = configData.password;
                if(merchantAccount.Equals("Boss", StringComparison.OrdinalIgnoreCase))
                {
                    jz_account = configData.account_boss;
                    jz_password = configData.password_boss;
                }
                #region 呼叫建周webservice
                //var response = await jzMsgSvc.sendBatchMessageAsync(jz_account, jz_password, phoneNumber, msg);
                //if (response.Body.sendBatchMessageReturn > 0)
                //{
                //    return true;
                //}
                #endregion

                #region 呼叫建周 http api
                var pwd = Common.GetMd5(jz_password).ToLower();
                var response = await SmsJZ.CallJZBatchApiHttp(account: jz_account, password: pwd, phoneNumber: phoneNumber, msg: msg);

                if (Convert.ToInt32(response) > 0)
                {
                    return true;
                }
                else
                {
                    var responseError = "未知错误!";
                    if (MessageErrorCode.ContainsKey(Convert.ToInt32(response)))
                        responseError = MessageErrorCode[Convert.ToInt32(response)];
                    if(writeToFile != null)
                    {
                        writeToFile.Done("SendJZhouMessageAsync_jz_response_error", "SendJZhouMessageAsync failed",
                            JsonConvert.SerializeObject(new
                            {
                                account = account,
                                response = response,
                                phoneNumber = phoneNumber,
                                msg = msg,
                                responseError = responseError
                            }), true);
                    }
                }
                #endregion

            }
            catch (Exception)
            {
                //return new ResponseResult() { code = "false", securityCode = sampleCode, account = account, message = "验证码传送失败e" };
            }
            return false;
            #endregion
        }

        /// <summary>
        /// 呼叫JZ http api 发送短信 2
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="sampleCode"></param>
        /// <param name="account"></param>
        /// <param name="configData"></param>
        /// <param name="merchantAccount"></param>
        /// <param name="writeToFile"></param>
        /// <returns></returns>
        public static async Task<bool> GetSendJZhouMessageCodeAsync(string phoneNumber,
            string sampleCode, 
            string account, 
            MessageConfigModel configData, 
            string merchantAccount,
            WriteToFile writeToFile = null)
        {
            #region 短信发送
            try
            {
                // 手机号码
                string msg = string.Format("验证码:{0}【{1}】", sampleCode, configData.signString);
                if (merchantAccount.Equals("Boss", StringComparison.OrdinalIgnoreCase))
                    msg = string.Format("验证码:{0}【{1}】", sampleCode, configData.signString_boss);
                //普通单发,注意前面必须为【】符号包含，置于头或者尾部。
                string json = string.Empty;
                var jz_account = configData.account;
                var jz_password = configData.password;
                if (merchantAccount.Equals("Boss", StringComparison.OrdinalIgnoreCase))
                {
                    jz_account = configData.account_boss;
                    jz_password = configData.password_boss;
                }
                #region 呼叫建周webservice
                //var response = await jzMsgSvc.sendBatchMessageAsync(jz_account, jz_password, phoneNumber, msg);
                //return response.Body.sendBatchMessageReturn>0 ;
                #endregion

                #region 呼叫建周 http api
                var pwd = Common.GetMd5(jz_password).ToLower();
                var response = await SmsJZ.CallJZBatchApiHttp(account: jz_account, password: pwd, phoneNumber: phoneNumber, msg: msg);

                if (Convert.ToInt32(response) > 0)
                {
                    return true;
                }
                else
                {
                    var responseError = "未知错误!";
                    if (MessageErrorCode.ContainsKey(Convert.ToInt32(response)))
                        responseError = MessageErrorCode[Convert.ToInt32(response)];
                    if (writeToFile != null)
                    {
                        writeToFile.Done("GetSendJZhouMessageCodeAsync_jz_response_error", "GetSendJZhouMessageCodeAsync failed",
                            JsonConvert.SerializeObject(new
                            {
                                account = account,
                                response = response,
                                phoneNumber = phoneNumber,
                                msg = msg,
                                responseError = responseError
                            }), true);
                    }
                }
                #endregion

            }
            catch (Exception)
            {
                //return new ResponseResult() { code = "false", securityCode = sampleCode, account = account, message = "验证码传送失败e" };
            }
            return false;
            #endregion
        }
       
        /// <summary>
        /// 国际短信发送  颂量
        /// </summary>
        /// <param name="configData">短信平台配置数据</param>
        /// <param name="phoneNumber">00+国家代码+手机号</param>
        /// <param name="sampleCode"></param>
        /// <param name="phoneCode"></param>
        /// <returns></returns>
        public static async Task<Tuple<bool, string>> SendIntlMessageAsync(MessageConfigModel configData, 
            string phoneNumber,
            string sampleCode,
            string phoneCode) 
        {
            #region 短信发送
            try {

                if (configData.plat == "ITNIO") {
                    phoneNumber = $"{phoneCode}{phoneNumber}";//国家代码+手机号
                    var apiUrl = configData.apiUrl;
                    var postData = JsonConvert.SerializeObject(new {
                        appId= configData.appId,
                        numbers= phoneNumber,
                        content= $"Your One Time Pin is {sampleCode}",
                        senderId=""
                    });
                    var timeStamp = Common.ToUnixTimestampSS(DateTime.UtcNow.AddHours(8));
                    var sign = Common.GetMd5($"{configData.Api_Key}{configData.Api_Secret}{timeStamp}").ToLower();
                    var headers = new Dictionary<string, string>() {
                        { "Sign", sign },
                        { "Timestamp", timeStamp.ToString() },
                        { "Api-Key", configData.Api_Key }
                    };

                    var response = await MRequest.RelayForm(apiUrl, "post", headers,
                        true, postData, "", "application/json;charset=UTF-8", isAllowedProxy: false, proxyUrl: "");
                    var r = JsonConvert.DeserializeObject<MessageResponeModel>(response);
                    if (r.status == "0")
                        return new Tuple<bool, string>(true, response);
                    else 
                    {
                        if (ITNIOErrorCode.errorCodes.ContainsKey(Convert.ToInt32(r.status)))
                            return new Tuple<bool, string>(false, string.Format(@"{0}:{1}", r.status, ITNIOErrorCode.errorCodes[Convert.ToInt32(r.status)]));
                        return new Tuple<bool, string>(false, response);
                    }
                }
                else if(configData.plat == "JIAN_ZHOU") {
                    phoneNumber = $"00{phoneCode}{phoneNumber}";//00+国家代码+手机号
                    // 手机号码
                    string msg = string.Format("Your One Time Pin is:{0}【BAYOBET】", sampleCode);
                    // 普通单发,注意前面必须为【】符号包含，置于头或者尾部。
                    string json = string.Empty;
                    var response = await jzMsgSvc.sendGjBatchMessageAsync(configData.account, configData.password,
                        sendDateTime: "", destmobile: phoneNumber, msgText: msg);
                    if (response.Body.@return > 0) {
                        return new Tuple<bool, string>(true, "");
                    }
                    else {
                        return new Tuple<bool, string>(false, JsonConvert.SerializeObject(response));
                    }
                }
                
            }
            catch (Exception ex) {
                return new Tuple<bool, string>(false, "unknown" + ex.Message);
            }
            return new Tuple<bool, string>(false, "unknown");
            #endregion
        }

        
    }


    /// <summary>
    /// 发送验证码结果信息
    /// </summary>
    public class ResponseResult
    {
        public string code { set; get; }
        public string message { set; get; }
        public string securityCode { set; get; }
        public string account { set; get; }
    }
}
