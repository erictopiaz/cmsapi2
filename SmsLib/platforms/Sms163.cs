using SmsLib.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmsLib.platforms
{
    /// <summary>
    /// 163短信平台
    /// </summary>
    public class Sms163
    {


        //public static String genSignature(String secretKey, Dictionary<String, String> parameters)
        //{
        //    parameters = parameters.OrderBy(o => o.Key, StringComparer.Ordinal).ToDictionary(o => o.Key, p => p.Value);

        //    StringBuilder builder = new StringBuilder();
        //    foreach (KeyValuePair<String, String> kv in parameters)
        //    {
        //        builder.Append(kv.Key).Append(kv.Value);
        //    }
        //    builder.Append(secretKey);
        //    String tmp = builder.ToString();
        //    MD5 md5 = new MD5CryptoServiceProvider();
        //    byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(tmp));
        //    builder.Clear();
        //    foreach (byte b in result)
        //    {
        //        builder.Append(b.ToString("x2").ToLower());
        //    }
        //    return builder.ToString();
        //}


        private static readonly string URI_SEND_SMS = "https://sms.dun.163.com/v2/sendsms";

        // SECRET_ID 和 SECRET_KEY 是产品密钥。可以登录易盾官网找到自己的凭证信息。请妥善保管，避免泄露。
        private static readonly string SECRET_ID = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
        private static readonly string SECRET_KEY = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

        /// <summary>
        /// 示例：发送一条验证码短信
        /// </summary>
        public static void SendOtp()
        {
            // 这是你的 国内验证码短信 业务的ID。可以登录易盾官网查看此业务ID。
            var businessId = "6761056e270e4513b5a7a57feb541ea5";
            // 这是你事先创建好的模板，且已通过审核。
            var templateId = "xxxxx";
            // 这是你自己的业务系统生成的验证码。如果你希望由易盾生成验证码，并通过验证码校验接口来验证，请参考 OptVerifyDemo。
            var code = "123456";
            // 这是收信方号码。如，134开头的号码一般是中国移动的号码。
            var to = "13143907796";

            // 此处假设你的模板中只有验证码这一个变量，且变量名为 code。如，模板内容为 “您的验证码为${code}，5分钟内有效，请勿泄露。”
            var variables = new Dictionary<string, string>
            {
                ["code"] = code
            };

            var paramDict = CreateParam(businessId, templateId, variables, to);
            var response = Send(paramDict);

            Console.WriteLine("response: " + response);
        }

        /// <summary>
        /// 示例：发送一条通知短信
        /// </summary>
        public static void SendArn()
        {
            // 这是你的 国内通知类短信 业务的ID。可以登录易盾官网查看此业务ID。
            var businessId = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            // 这是你事先创建好的模板，且已通过审核。
            var templateId = "xxxxx";
            // 这是收信方号码。如，134开头的号码一般是中国移动的号码。
            var to = "xxxxxxxxxxx";

            // 此处假设你的模板中只有orderId这一个变量。如，模板内容为 “您的订单已发货，订单号为 ${orderId}。”
            var variables = new Dictionary<string, string>
            {
                ["orderId"] = "1020304050"
            };

            var paramDict = CreateParam(businessId, templateId, variables, to);
            var response = Send(paramDict);

            Console.WriteLine("response: " + response);
        }

        /// <summary>
        /// 示例：发送一条营销短信
        /// </summary>
        public static void sendMkt()
        {
            // 这是你的 国内营销类短信 业务的ID。可以登录易盾官网查看此业务ID。
            var businessId = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            // 这是你事先创建好的模板，且已通过审核。
            var templateId = "xxxxx";
            // 这是收信方号码。如，134开头的号码一般是中国移动的号码。
            var to = "xxxxxxxxxxx";

            // 此处假设你的模板没有变量。如，模板内容为 “即日起至12月底，每日登录APP签到，就有机会参与新年抽奖活动。”
            var variables = new Dictionary<string, string>();

            var paramDict = CreateParam(businessId, templateId, variables, to);
            var response = Send(paramDict);

            Console.WriteLine("response: " + response);
        }

        /// <summary>
        /// 示例：发送一条国际验证码短信
        /// </summary>
        public static void SendInternationalOtp()
        {
            // 这是你的 国际验证码短信 业务的ID。可以登录易盾官网查看此业务ID。
            var businessId = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            // 这是你事先创建好的模板，且已通过审核。
            var templateId = "xxxxx";
            // 这是你自己的业务系统生成的验证码。如果你希望由易盾生成验证码，并通过验证码校验接口来验证，请参考 OptVerifyDemo。
            var code = "123456";
            // 这是收信方号码。不包含国际电话区号。
            var to = "xxxxxxxxxxx";
            // 这是收信方的国际电话区号。如，美国是1，英国是44，法国是33，俄罗斯是79。
            var countryCallingCode = "1";

            // 此处假设你的模板中只有验证码这一个变量，且变量名为 code。如，模板内容为 “Your verification code is ${code}, valid for 5 minutes.”
            var variables = new Dictionary<string, string>
            {
                ["code"] = code
            };

            var paramDict = CreateParamForInternational(businessId, templateId, variables, to, countryCallingCode);
            var response = Send(paramDict);

            Console.WriteLine("response: " + response);
        }

        private static SendResponse Send(IDictionary<string, string> paramDict)
        {
            return RequestUtils.PostForEntity<SendResponse>(URI_SEND_SMS, paramDict);
        }
        private static IDictionary<string, string> CreateParam(
            string businessId, string templateId, IDictionary<string, string> variables, string to)
        {
            return CreateSendParam(businessId, templateId, variables, to, null);
        }

        private static IDictionary<string, string> CreateParamForInternational(
            string businessId, string templateId, IDictionary<string, string> variables, string to, string countryCallingCode)
        {
            return CreateSendParam(businessId, templateId, variables, to, countryCallingCode);
        }

        private static IDictionary<string, string> CreateSendParam(
            string businessId, string templateId, IDictionary<string, string> variables, string to, string countryCallingCode)
        {
            var paramDict = new Dictionary<string, string>()
            {
                ["nonce"] = ParamUtils.CreateNonce(),
                ["timestamp"] = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(),
                ["version"] = "v2",

                ["secretId"] = SECRET_ID,
                ["businessId"] = businessId,

                ["templateId"] = templateId,
                ["mobile"] = to,

                ["paramType"] = "json",
                ["params"] = ParamUtils.SerializeVariables(variables)
            };

            // 如果要发送国际短信，则需要指明国际电话区号。如果不是国际短信，则不要指定此参数
            if (!string.IsNullOrWhiteSpace(countryCallingCode))
            {
                paramDict["internationalCode"] = countryCallingCode;
            }


            // 在最后一步生成此次请求的签名
            paramDict["signature"] = ParamUtils.GenSignature(SECRET_KEY, paramDict);

            return paramDict;
        }
    }


    public class SendResponse
    {
        public int code { get; set; }
        public string msg { get; set; }
        public Data data { get; set; }

        public override string ToString()
        {
            return "code=" + code + ", "
                  + "msg=" + msg + ", "
                  + "data=" + data;
        }

        public class Data
        {
            public int result { get; set; }
            public string requestId { get; set; }

            public override string ToString()
            {
                return "result=" + result + ", "
                        + "requestId=" + requestId;
            }
        }
    }
}
