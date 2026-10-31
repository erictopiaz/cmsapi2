using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static UtilsLib.RSA1024.RSAForJava;

namespace UtilsLib
{
    /// <summary>
    /// 常转换工具操作
    /// </summary>
    public static class Common
    {
        

        #region  uid 生成短字串字串
        //自定义进制(0、O没有加入,容易混淆；同时排除X,用X补位)
        private static char[] r = new char[] { 'Q', 'W', 'E', '8', 'A', 'S', '2', 'D', 'Z', '9', 'C', '7', 'P', '5', 'I', 'K', '3', 'M', 'J', 'U', 'F', 'R', '4', 'V', 'Y', 'L', 'T', 'N', '6', 'B', 'G', 'H' };
        //不能与自定义进制有重复
        private static char b = 'X';
        //进制长度
        private static int binLen = r.Length;
        //生成的邀请码长度
        private static int length = 6;
        public static string Encode(long id)
        {
            char[] buf = new char[32];
            int charPos = 32;

            while ((id / binLen) > 0)
            {
                int ind = (int)(id % binLen);
                buf[--charPos] = r[ind];
                id /= binLen;
            }
            buf[--charPos] = r[(int)(id % binLen)];
            String str = new String(buf, charPos, (32 - charPos));
            //不够长度的自动随机补全
            if (str.Length < length)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(b);
                System.Random rnd = new System.Random();
                for (int i = 1; i < length - str.Length; i++)
                {
                    sb.Append(r[rnd.Next(binLen)]);
                }
                str += sb.ToString();
            }
            return str;
        }

        public static string Decode(string code)
        {
            char[] chs = code.ToArray();
            long res = 0L;
            for (int i = 0; i < chs.Length; i++)
            {
                int ind = 0;
                for (int j = 0; j < binLen; j++)
                {
                    if (chs[i] == r[j])
                    {
                        ind = j;
                        break;
                    }
                }
                if (chs[i] == b)
                {
                    break;
                }
                if (i > 0)
                {
                    res = res * binLen + ind;
                }
                else
                {
                    res = ind;
                }
            }
            return res.ToString();
        }
        #endregion
        /// <summary>
        /// 短字串处理
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string[] ShortString(string url)
        {
            //可以自定义生成MD5加密字符传前的混合KEY
            string key = "Awc8m*d";
            //要使用生成URL的字符
            string[] chars = new string[]
            {
                "a", "b", "c", "d", "e", "f", "g", "h",
                "i", "j", "k", "l", "m", "n", "o", "p",
                "q", "r", "s", "t", "u", "v", "w", "x",
                "y", "z", "0", "1", "2", "3", "4", "5",
                "6", "7", "8", "9", "A", "B", "C", "D",
                "E", "F", "G", "H", "I", "J", "K", "L",
                "M", "N", "O", "P", "Q", "R", "S", "T",
                "U", "V", "W", "X", "Y", "Z"
            };
            //对传入网址进行MD5加密
            string hex = GetMd5(key + url);
            string[] resUrl = new string[4];
            for (int i = 0; i < 4; i++)
            {
                //把加密字符按照8位一组16进制与0x3FFFFFFF进行位与运算
                int hexint = 0x3FFFFFFF & Convert.ToInt32("0x" + hex.Substring(i * 8, 8), 16);
                string outChars = string.Empty;
                for (int j = 0; j < 6; j++)
                {
                    //把得到的值与0x0000003D进行位与运算，取得字符数组chars索引
                    int index = 0x0000003D & hexint;
                    //把取得的字符相加
                    outChars += chars[index];
                    //每次循环按位右移5位
                    hexint = hexint >> 5;
                }
                //把字符串存入对应索引的输出数组
                resUrl[i] = outChars;
            }
            return resUrl;
        }

        /// <summary>
        /// 检测输入字串中是否还有电话号码
        /// </summary>
        /// <param name="input_str"></param>
        /// <returns></returns>
        public static bool IsIncludedTel(string input_str)
        {
            var regex = new Regex(@"([1][3456789]\d{9})");
            //Match集合，匹配成功的字符串集合
            var collection = regex.Matches(input_str);
            return collection.Count > 0;
        }

        /// <summary>
        /// 字串部份隐藏
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string HideSensitiveInfo(string info)
        {
            if (String.IsNullOrEmpty(info))
            {
                return "";
            }
            if (info.Length < 4)
                return info;
            var pre = info.Substring(0, 2);
            var last = info.Substring(2, info.Length-2);
            var len = last.Length / 2;
            var stars = string.Empty;
            for (var i = 0; i < len; i++)
            {
                stars = string.IsNullOrEmpty(stars) ? string.Format(@"*") : string.Format(@"{0}*", stars);
            }

            return string.Format(@"{0}{1}{2}", pre, stars, last.Substring(len, last.Length - len));
        }
        /// <summary>
        /// 检测 ip 是否为ipv6
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIPV6(string ip) {
            try {
                if (String.IsNullOrEmpty(ip)) {
                    return false;
                }
                System.Net.IPAddress address;
                var isIP = System.Net.IPAddress.TryParse(ip, out address);

                var bIsIPv6 = (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6) ? true : false;
                return bIsIPv6;
            }
            catch (Exception) {

                return false;
            }
        }
        /// <summary>
        /// 检测 ip 是否为ipv4
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIPV4(string ip) {
            try {
                if (String.IsNullOrEmpty(ip)) {
                    return false;
                }
                System.Net.IPAddress address;
                var isIP = System.Net.IPAddress.TryParse(ip, out address);
                var bIsIPv4 = (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) ? true : false;
                return bIsIPv4;
            }
            catch (Exception) {

                return false;
            }
        }
        /// <summary>
        /// 隐藏姓名
        /// 姓****
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string HideSensitiveName(string name, int startIndex=1)
        {
            if (String.IsNullOrEmpty(name))
            {
                return "";
            }
            if (name.Length < 2)
                return name;
            var pre = name.Substring(0, startIndex);
            var last = name.Substring(startIndex, name.Length - startIndex);
            var len = last.Length;
            var stars = string.Empty;
            for (var i = 0; i < len; i++)
            {
                stars = string.IsNullOrEmpty(stars) ? string.Format(@"*") : string.Format(@"{0}*", stars);
            }

            return string.Format(@"{0}{1}", pre, stars);
        }
        /// <summary>
        /// 默认密钥向量
        /// </summary>
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        ///   时间转时间戳转
        /// </summary> 
        public static long ToUnixTimestamp()
        {
            DateTime startTime = new DateTime(1970, 1, 1, 8, 0, 0, 0);
            return (long)(DateTime.UtcNow.AddHours(8) - startTime).TotalMilliseconds; // 相差M秒数
        }
        public static long ToUnixTimestamp(string dateTime)
        {
            var dt = Convert.ToDateTime(dateTime);
            DateTime startTime = new DateTime(1970, 1, 1, 8, 0, 0, 0);
            return (long)(dt - startTime).TotalMilliseconds; // 相差M秒数
        }


        /// <summary>
        /// 根据当前时间获取转为UnixTime
        /// 13 位
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long ToUnixTimestamp(DateTime dt)
        {
            DateTime startTime = new DateTime(1970, 1, 1, 8, 0, 0, 0);
            return (long)(dt - startTime).TotalMilliseconds; // 相差M秒数
        }
        public static DateTime FromUnixTimestamp(long UnixTimestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 8, 0, 0, 0);
            return origin.AddMilliseconds(UnixTimestamp);

        }
        /// <summary>
        /// 10位
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long ToUnixTimestampSS(DateTime dt)
        {
            DateTime startTime = new DateTime(1970, 1, 1, 8, 0, 0, 0);
            return (long)(dt - startTime).TotalSeconds; // 相差秒数
        }
        public static DateTime FromUnixTimestampSS(long UnixTimestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 8, 0, 0, 0);
            return origin.AddSeconds(UnixTimestamp);

        }

        /// <summary>
        /// 时间字串转到时间
        /// </summary>
        /// <param name="dateString">如："2025-06-13 15:13:41 +08:00"</param>
        /// <param name="formats">是dateString 对应的时间字串格式</param>
        /// <returns></returns>
        public static DateTime FromDateStringToDateTime(string dateString, string formats= "yyyy-MM-dd HH:mm:ss +08:00")
        {
            string[] f = { formats };
            return DateTime.ParseExact(dateString, f, new CultureInfo("en-US"), DateTimeStyles.None);
        }

        /// <summary>
        /// 时间字串转到时间戳 13位
        /// </summary>
        /// <param name="dateString">如："2025-06-13 15:13:41 +08:00"</param>
        /// <param name="formats">是dateString 对应的时间字串格式</param>
        /// <returns></returns>
        public static long FromDateStringToUnixTimestamp(string dateString, string formats = "yyyy-MM-dd HH:mm:ss +08:00")
        {
            string[] f = { formats };
            return ToUnixTimestamp(DateTime.ParseExact(dateString, f, new CultureInfo("en-US"), DateTimeStyles.None));
        }



        /// <summary>
        /// 时间差天
        /// 和当前时间相差多少天
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int SubDay(DateTime dateTime)
        {
            TimeSpan current = new TimeSpan(DateTime.UtcNow.AddHours(8).Ticks);
            TimeSpan ts2 = new TimeSpan(dateTime.Ticks);
            TimeSpan ts = current.Subtract(ts2).Duration();
            return ts.Days;
        }
        /// <summary>
        /// 当前月有多少天
        /// </summary>
        /// <returns></returns>
        public static int DaysInMonth() {
            return System.Threading.Thread.CurrentThread.CurrentUICulture.Calendar.GetDaysInMonth(DateTime.UtcNow.AddHours(8).Year, DateTime.UtcNow.AddHours(8).Month);
        }


        public static string BuildQuerystring(Dictionary<string, string> querystringParams)
        {
            List<string> paramList = new List<string>();
            foreach (var parameter in querystringParams)
            {
                paramList.Add(parameter.Key + "=" + parameter.Value);
            }
            return "?" + string.Join("&", paramList);
        }


        public static string GenerateRNGCode(int length)
        {
            string strCode = string.Empty;
            //验证码的字符集，去掉了一些容易混淆的字符
            char[] character = { '0', '1', '2', '3', '4', '5', '6', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'i', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

            System.Random rnd = new System.Random(GetRandomSeed());

            //生成验证码字符串
            for (int i = 0; i < length; i++)
            {
                strCode += character[rnd.Next(character.Length)];
            }
            return strCode;
        }
        /// <summary>
        /// 数字串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateDightCode(int length)
        {
            string strCode = string.Empty;
            //验证码的字符集，去掉了一些容易混淆的字符
            char[] character = { '0', '1', '2', '3', '4', '5', '6', '8', '9'};
            System.Random rnd = new System.Random(GetRandomSeed());

            //生成验证码字符串
            for (int i = 0; i < length; i++)
            {
                strCode += character[rnd.Next(character.Length)];
            }
            return strCode;
        }
        private static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
        public static string ReferenceNoEx()
        {
            System.Random random = new System.Random(GetRandomSeed());
            string refno = "";
            for (int i = 0; i < 4; i++)
            {
                refno += random.Next(9).ToString();
            }
            refno = DateTime.UtcNow.AddHours(8).ToString("yyMMddHHmmss") + refno;
            return refno;
        }

        /// <summary>
        /// 解析网址
        /// </summary>
        /// <param name="urlString">网址</param>
        /// <returns>返回host</returns>
        public static string GetHostByUrl(string urlString)
        {
            if (string.IsNullOrEmpty(urlString))
                return string.Empty;
            try
            {
                Uri uri = new Uri(urlString.ToString());
                return uri.Host;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 解析网址
        /// </summary>
        /// <param name="urlString">网址</param>
        /// <returns>返回host:port</returns>
        public static string GetHostAndPortByUrl(string urlString)
        {
            if (string.IsNullOrEmpty(urlString))
                return string.Empty;
            try
            {
                Uri uri = new Uri(urlString.ToString());
                if (uri.Port != 80 && uri.Port != 443)
                {
                    return string.Format(@"{0}:{1}", uri.Host, uri.Port);
                }
                else
                {
                    return uri.Host;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// 获取指定url 中的参数据值
        /// </summary>
        /// <param name="urlString"></param>
        /// <param name="key"></param>
        /// <returns>返回key对应的value</returns>
        public static string GetKeyValueByUrl(string urlString, string key)
        {
            if (string.IsNullOrEmpty(urlString))
                return string.Empty;
            try
            {
                var uri = new Uri(urlString.ToString());
                var queryString = uri.Query;
                var paramDict = System.Web.HttpUtility.ParseQueryString(queryString);
                return paramDict[key];
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }
        /// <summary>
        /// 获取guid long字串
        /// </summary>
        /// <returns></returns>
        public static string GuidToLongString()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            long guidL = BitConverter.ToInt64(buffer, 0);
            return guidL.ToString();
        }
        public static long GuidToLong()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }
        /// <summary>
        /// 获取guid long字串
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static string GuidToLongString(string inputStr)
        {
            byte[] buffer = Guid.Parse(inputStr).ToByteArray();
            long guidL = BitConverter.ToInt64(buffer, 0);
            return guidL.ToString();
        }

        ///// <summary>
        ///// 微软提供的SHA256加密算法
        ///// </summary>
        ///// <param name="strInput">加密码源字串</param>
        ///// <returns></returns>
        //public static string SHA256(this string strInput)
        //{
        //    return Crypto.SHA256(strInput);
        //}


        /// <summary>
        ///  HMAC-SHA256
        /// </summary>
        /// <param name="text">加密码源字串</param>
        /// <param name="key">加密key</param>
        /// <returns></returns>
        public static String HmacSha256(string text, string key) {
            // change according to your needs, an UTF8Encoding
            // could be more suitable in certain situations
            ASCIIEncoding encoding = new ASCIIEncoding();

            Byte[] textBytes = encoding.GetBytes(text);
            Byte[] keyBytes = encoding.GetBytes(key);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
        /// <summary>
        /// 字串转到 Hex
        /// </summary>
        /// <param name="inputHex"></param>
        /// <returns></returns>
        public static byte[] ConvertFromStringToHex(string inputHex)
        {
            inputHex = inputHex.Replace("-", "");

            byte[] resultantArray = new byte[inputHex.Length / 2];
            for (int i = 0; i < resultantArray.Length; i++)
            {
                resultantArray[i] = Convert.ToByte(inputHex.Substring(i * 2, 2), 16);
            }
            return resultantArray;
        }

        /// <summary>
        ///  加密HMAC-SHA256 Base64 out
        /// </summary>
        /// <param name="text">加密码源字串</param>
        /// <param name="key">加密key</param>
        /// <returns></returns>
        public static String HmacSha256_Base64(string text, string key)
        {
            // change according to your needs, an UTF8Encoding
            // could be more suitable in certain situations
            var encoding = new ASCIIEncoding();
            Byte[] textBytes = encoding.GetBytes(text);
            Byte[] keyBytes = encoding.GetBytes(key);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);
            return Convert.ToBase64String(hashBytes);
           
        }

        //UTF-8 的 byte 数组后，使用 sha256 算法进行编码，再将编码后的数组转码为 16 进制的字符串
        public static string ComputeSHA256(string s) {
            string hash = String.Empty;
            // 初始化一个 SHA256 哈希对象
            using (SHA256 sha256 = SHA256.Create()) {

                //UTF - 8 的 byte 数组 sha256 算法进行编码
                byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));
                // 编码后的数组转码为 16 进制的字符串
                foreach (byte b in hashValue) {
                    hash += $"{b:X2}";
                }
            }

            return hash.ToLower();
        }


        /// <summary>
        /// 生成BASE64编码
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static string ToBase64String(this string strInput)
        {
            if (string.IsNullOrEmpty(strInput))
                return string.Empty;
            byte[] bytes = System.Text.Encoding.GetEncoding("utf-8").GetBytes(strInput);
            //編成 Base64 字串
            return Convert.ToBase64String(bytes); ;
        }
        /// <summary>
        /// base64解密
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static string Base64ToString(this string strInput)
        {
            if (string.IsNullOrEmpty(strInput))
                return string.Empty;
            return Encoding.GetEncoding("utf-8").GetString(Convert.FromBase64String(strInput));
        }
        /// <summary>
        /// 字串安全过虑
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static string StringFilter(this string strInput) {
            if (string.IsNullOrEmpty(strInput))
                return string.Empty;
            return strInput.Replace("'", "''");
        }
        /// <summary>
        /// 获取输入串的MD5加密串串,默认大写
        /// 可以根据自己需求转为大小或小写
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static string GetMd5(string inputStr)
        {
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(inputStr));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            return sb.ToString().ToUpper();
        }
        /// <summary>
        /// 二次MD5
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static string GetDBMd5(string inputStr)
        {
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(inputStr));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            return GetMd5(sb.ToString().ToUpper());
        }

        public static string PasskeyEncrypt(string encryptString)
        {
            string encryptKey = "commonex";
            byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        public static string PasskeyDecrypt(string decryptString)
        {
            string decryptKey = "commonex";
            byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }


        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="text"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string DESEncrypt(string text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(text);
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);// 密匙
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);// 初始化向量
            MemoryStream ms = new MemoryStream();
            des.Mode = CipherMode.ECB;
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            var retB = Convert.ToBase64String(ms.ToArray());
            return retB;

        }

        #region DES加密字符串 和解密 key iv
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DESEncryption(string plainText, string key, string iv) {
            using (var des = DES.Create()) {
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;
                des.Key = Encoding.UTF8.GetBytes(key);
                des.IV = Encoding.UTF8.GetBytes(iv);

                byte[] plainByteArray = Encoding.UTF8.GetBytes(plainText);
                byte[] cipherByteArray = des.CreateEncryptor().TransformFinalBlock(plainByteArray, 0,
                plainByteArray.Length);

                string cipherText = Convert.ToBase64String(cipherByteArray);
                return cipherText;
            }
        }
        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DESDecrypt(string text, string key, string iv) {
            using (var des = DES.Create()) {
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;
                des.Key = Encoding.UTF8.GetBytes(key);
                des.IV = Encoding.UTF8.GetBytes(iv);

                byte[] plainByteArray = Convert.FromBase64String(text);
                byte[] cipherByteArray = des.CreateDecryptor().TransformFinalBlock(plainByteArray, 0,
                plainByteArray.Length);

                string cipherText = Encoding.UTF8.GetString(cipherByteArray);
                return cipherText;
            }
        }

        #endregion

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="toEncrypt">被加密的明文</param>
        /// <returns>密文</returns>
        public static string AESEncrypt(string toEncrypt, string AES_KEY, string AES_IV)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(AES_KEY);
            byte[] ivArray = UTF8Encoding.UTF8.GetBytes(AES_IV);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            using (var rm = new RijndaelManaged())
            {
                rm.Key = keyArray;
                rm.IV = ivArray;
                rm.Mode = CipherMode.CBC;
                ICryptoTransform it = rm.CreateEncryptor();
                byte[] rst = it.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Convert.ToBase64String(rst, 0, rst.Length);
            }
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="toDecrypt"></param>
        /// <param name="AES_KEY"></param>
        /// <param name="AES_IV"></param>
        /// <returns></returns>
        public static string AESDecrypt(string toDecrypt, string AES_KEY, string AES_IV)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(AES_KEY);
            byte[] ivArray = UTF8Encoding.UTF8.GetBytes(AES_IV);
            byte[] toDecryptArray = Convert.FromBase64String(toDecrypt);
            using (var rm = new RijndaelManaged())
            {
                rm.Key = keyArray;
                rm.IV = ivArray;
                rm.Mode = CipherMode.CBC;
                ICryptoTransform it = rm.CreateDecryptor();
                byte[] rst = it.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);
                return UTF8Encoding.UTF8.GetString(rst);
            }
        }

        /// <summary>
        /// AES-128-ECB
        /// 加密
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string EncryptAES(string toEncrypt, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        /// <summary>
        /// AES-128-ECB
        /// 解密
        /// </summary>
        /// <param name="toDecrypt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DecryptAES(string toDecrypt, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);

                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch (Exception)
            {
                return decryptString;
            }
        }
        //public static string PasswordEncrypt(string password, string format)
        //{
        //    return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(password, format);
        //}
        private static DESCryptoServiceProvider key = new DESCryptoServiceProvider();
        public static string EncryptAES(string text, string key, string iv)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length) len = keyBytes.Length;
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            byte[] ivBytes1 = Encoding.UTF8.GetBytes(iv);
            byte[] keyBytes1 = new byte[16];
            int len1 = ivBytes1.Length;
            if (len1 > keyBytes1.Length) len1 = keyBytes1.Length;
            Array.Copy(ivBytes1, keyBytes1, len1);
            rijndaelCipher.IV = ivBytes1;
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(text);
            byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0,
            plainText.Length);
            return Convert.ToBase64String(cipherBytes);
        }
        public static string DecryptAES(string text, string key, string iv)
        {
            try
            {
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                rijndaelCipher.KeySize = 128;
                rijndaelCipher.BlockSize = 128;
                byte[] encryptedData = Convert.FromBase64String(text);
                byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
                byte[] keyBytes = new byte[16];
                int len = pwdBytes.Length;
                if (len > keyBytes.Length) len = keyBytes.Length;
                Array.Copy(pwdBytes, keyBytes, len);
                rijndaelCipher.Key = keyBytes;
                byte[] ivBytes1 = Encoding.UTF8.GetBytes(iv);
                byte[] keyBytes1 = new byte[16];
                int len1 = ivBytes1.Length;
                if (len1 > keyBytes1.Length) len1 = keyBytes1.Length;
                Array.Copy(ivBytes1, keyBytes1, len1);
                rijndaelCipher.IV = keyBytes1;
                ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
                byte[] plainText = transform.TransformFinalBlock(encryptedData, 0,
                encryptedData.Length);
                return Encoding.UTF8.GetString(plainText);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static string GetIV()
        {
            return GetIV(16);
        }
        public static string GetIV(int len)
        {
            RNGCryptoServiceProvider rngc = new RNGCryptoServiceProvider();
            Byte[] buff = new Byte[len / 2];
            rngc.GetBytes(buff);
            StringBuilder sb = new StringBuilder();
            foreach (Byte b in buff)
            {
                sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
        }
        #region DESEnCode DES加密
        public static string DESEnCode(string pToEncrypt, string DES_Key)
        {
            using (var des = new DESCryptoServiceProvider()) {
                byte[] inputByteArray = Encoding.GetEncoding("UTF-8").GetBytes(pToEncrypt);

                des.Key = ASCIIEncoding.ASCII.GetBytes(DES_Key);
                des.IV = ASCIIEncoding.ASCII.GetBytes(DES_Key);
                using (var ms = new MemoryStream()) {
                    using (var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write)) {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();

                        StringBuilder ret = new StringBuilder();
                        foreach (byte b in ms.ToArray()) {
                            ret.AppendFormat("{0:X2}", b);
                        }
                        ret.ToString();
                        return ret.ToString();
                    }
                }
            }

        }
        #endregion

        #region DESDeCode DES解密
        public static string DESDeCode(string pToDecrypt, string DES_Key)
        {
            using (var des = new DESCryptoServiceProvider()) {
                byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
                for (int x = 0; x < pToDecrypt.Length / 2; x++) {
                    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }

                des.Key = ASCIIEncoding.ASCII.GetBytes(DES_Key);
                des.IV = ASCIIEncoding.ASCII.GetBytes(DES_Key);
                using (var ms = new MemoryStream()) {
                    using (var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write)) {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        StringBuilder ret = new StringBuilder();
                        return System.Text.Encoding.UTF8.GetString(ms.ToArray());
                    }
                }  
            }

            //DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            //byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            //for (int x = 0; x < pToDecrypt.Length / 2; x++)
            //{
            //    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
            //    inputByteArray[x] = (byte)i;
            //}

            //des.Key = ASCIIEncoding.ASCII.GetBytes(DES_Key);
            //des.IV = ASCIIEncoding.ASCII.GetBytes(DES_Key);
            //MemoryStream ms = new MemoryStream();
            //CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            //cs.Write(inputByteArray, 0, inputByteArray.Length);
            //cs.FlushFinalBlock();

            //StringBuilder ret = new StringBuilder();

            //return System.Text.Encoding.UTF8.GetString(ms.ToArray());
        }
        #endregion

        #region AES加密解密 AES256 cbc pkcs7padding  
        /// <summary>
        /// EncryptAES256 cbc pkcs7padding
        /// </summary>
        /// <param name="text">Raw content text.</param>
        /// <param name="key">The key which is used for en/decryption for both side.</param>
        /// <param name="iv">The initialization vector for the symmetric algorithm.</param>
        /// <returns></returns>
        public static string EncryptAES256(string text, string key, string iv)
        {
            var sourceBytes = System.Text.Encoding.UTF8.GetBytes(text);
            using (var aes = new System.Security.Cryptography.RijndaelManaged()) {
                aes.Mode = System.Security.Cryptography.CipherMode.CBC;
                aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Key = System.Text.Encoding.UTF8.GetBytes(key);
                if (!string.IsNullOrEmpty(iv))
                    aes.IV = System.Text.Encoding.UTF8.GetBytes(iv);
                using (var transform = aes.CreateEncryptor()) {
                    return System.Convert.ToBase64String(transform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length));
                }
            }
                
           
        }

        /// <summary>
        /// DecryptAES256
        /// </summary>
        /// <param name="text">Encrypted text.</param>
        /// <param name="key">The key which is used for en/decryption for both side.</param>
        /// <param name="iv">The initialization vector for the symmetric algorithm.</param>
        /// <returns></returns>
        public static string DecryptAES256(string encryptData, string key, string iv)
        {
            var encryptBytes = System.Convert.FromBase64String(encryptData);
            var aes = new System.Security.Cryptography.RijndaelManaged();
            aes.Mode = System.Security.Cryptography.CipherMode.CBC;
            aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Key = System.Text.Encoding.UTF8.GetBytes(key);
            if (!string.IsNullOrEmpty(iv))
                aes.IV = System.Text.Encoding.UTF8.GetBytes(iv);
            var transform = aes.CreateDecryptor();
            return System.Text.Encoding.UTF8.GetString(transform.TransformFinalBlock(encryptBytes, 0, encryptBytes.Length));
        }

        #endregion


        #region AES加密解密 AES / ECB / PKCS5PADDING

        private static AesManaged CreateAes(string secrectKey, string initVector)
        {
            var aes = new AesManaged();
            aes.Key = System.Text.Encoding.UTF8.GetBytes(secrectKey); //UTF8-Encoding
            if(!string.IsNullOrEmpty(initVector))
                aes.IV = System.Text.Encoding.UTF8.GetBytes(initVector);//UT8-Encoding
            return aes;
        }

        public static string EncryptAesEcbPKCS5(string text, string secrectKey, string initVector)
        {
            using (AesManaged aes = CreateAes(secrectKey, initVector))
            {
                ICryptoTransform encryptor = aes.CreateEncryptor();
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(text);
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }
        public static String doEncryptAESPKCS7(String plainText, String key)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(Encrypt(plainBytes, getRijndaelManaged(key)));
        }

        public static String doDecryptAESPKCS7(String encryptedText, String key)
        {
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            return Encoding.UTF8.GetString(Decrypt(encryptedBytes, getRijndaelManaged(key)));
        }
        private static RijndaelManaged getRijndaelManaged(String secretKey)
        {
            var keyBytes = new byte[16];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }

        private static byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        private static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }


        public static string DecryptAesEcbPKCS5(string text, string secrectKey, string initVector)
        {
            using (var aes = CreateAes(secrectKey, initVector))
            {
                ICryptoTransform decryptor = aes.CreateDecryptor();
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(text)))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cs))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }

            }
        }

        #endregion


        public static string ObscureString(string withdrawcard)
        {
           
            try
            {
                if (string.IsNullOrEmpty(withdrawcard))
                    return "";
                return string.Concat(withdrawcard.Substring(0, 2), "****", withdrawcard.Substring(withdrawcard.Length - 2, 2));
            }
            catch (Exception)
            {
                return "***";
            }
        }

        public static string sha1ToBase64(string inputString)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            var passwordBytes = Encoding.UTF8.GetBytes(inputString);
            var passwordHash = sha1.ComputeHash(passwordBytes);
            return Convert.ToBase64String(passwordHash);
        }


        //商户私钥签名
        public static string RSASign(string signStr, string privateKey)
        {
            try
            {
                //私钥转换成C#专用私钥
                privateKey = RSAPrivateKeyJava2DotNet(privateKey);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                //RSAParameters para = new RSAParameters();
                rsa.FromXmlString(privateKey);
                byte[] signBytes = rsa.SignData(UTF8Encoding.UTF8.GetBytes(signStr), "md5");
                return Convert.ToBase64String(signBytes);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //RSA私钥格式转换
        private static string RSAPrivateKeyJava2DotNet(string privateKey)
        {
            RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));
            return string.Format(
                "<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned())
            );
        }

        //使用公钥验签
        public static bool ValidateRsaSign(string plainText, string publicKey, string signedData)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                //RSAParameters para = new RSAParameters();
                rsa.FromXmlString(publicKey);
                return rsa.VerifyData(UTF8Encoding.UTF8.GetBytes(plainText), "md5", Convert.FromBase64String(signedData));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //公钥格式转换
        public static string RSAPublicKeyJava2DotNet(string publicKey)
        {
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
            return string.Format(
                "<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned())
            );
        }


        /// <summary>
        /// 获取时间区间
        /// 1：今天， 2：昨天，3：当周,33:上周，4：本月，5：上月 ， 6：不使用时间快捷
        /// </summary>
        /// <param name="dateStatus"></param>
        /// <returns></returns>
        public static Tuple<bool, object> GetDateRanage(int dateStatus)
        {
            var d = DateTime.UtcNow.AddHours(8);
            if (dateStatus == 1)
            {
                var startTime = Convert.ToInt32(d.ToString("yyyyMMdd"));
                return new Tuple<bool, object>(true, new { startTime = startTime, endTime = startTime, startTimeS = d.ToString("yyyy-MM-dd 00:00:00"), endTimeS = d.AddDays(1).ToString("yyyy-MM-dd 00:00:00") });
            }
            else if (dateStatus == 2)
            {
                var startTime = Convert.ToInt32(d.AddDays(-1).ToString("yyyyMMdd"));
                return new Tuple<bool, object>(true, new { startTime = startTime, endTime = startTime, startTimeS = d.AddDays(-1).ToString("yyyy-MM-dd 00:00:00"), endTimeS = d.ToString("yyyy-MM-dd 00:00:00") });
            }
            else if (dateStatus == 3)
            {
                int week = Convert.ToInt32(d.DayOfWeek);
                week = week == 0 ? 7 : week;

                //获取上周星期一/星期天
                //var start_time_last_week = d.AddDays(1 - week - 7);//上周星期一
                //var end_time_last_week = d.AddDays(7 - week - 7);//上周星期天


                //获取本周星期一/星期天
                var st = d.AddDays(1 - week);//本周星期一
                var et = d.AddDays(7 - week);//本周星期天


                //获取下周星期一/星期天
                //var start_time_next_week = d.AddDays(1 - week + 7);//下周星期一
                //var end_time_next_week = d.AddDays(7 - week + 7);//下周星期天


                //var st = d.AddDays(-(int)d.DayOfWeek + 1);
                //var et = d.AddDays(7 - (int)d.DayOfWeek);

                var startTime = Convert.ToInt32(st.ToString("yyyyMMdd"));
                var endTime = Convert.ToInt32(et.ToString("yyyyMMdd"));
                return new Tuple<bool, object>(true, new { startTime = startTime, endTime = endTime, startTimeS = st.ToString("yyyy-MM-dd 00:00:00"), endTimeS = et.AddDays(1).ToString("yyyy-MM-dd 00:00:00") });
            }
            else if (dateStatus == 33)
            {
                int week = Convert.ToInt32(d.DayOfWeek);
                week = week == 0 ? 7 : week;

                //获取上周星期一/星期天
                var st = d.AddDays(1 - week - 7);//上周星期一
                var et = d.AddDays(7 - week - 7);//上周星期天


                //var st = d.AddDays(-(int)d.DayOfWeek -6);
                //var et = d.AddDays(-(int)d.DayOfWeek);

                var startTime = Convert.ToInt32(st.ToString("yyyyMMdd"));
                var endTime = Convert.ToInt32(et.ToString("yyyyMMdd"));
                return new Tuple<bool, object>(true, new { startTime = startTime, endTime = endTime, startTimeS = st.ToString("yyyy-MM-dd 00:00:00"), endTimeS = et.AddDays(1).ToString("yyyy-MM-dd 00:00:00") });
            }
            else if (dateStatus == 4)
            {
                var st = d.AddDays(-(int)d.Day + 1);
                var et = d.AddMonths(1).AddDays(-d.AddMonths(1).Day + 1).AddDays(-1);
                var startTime = Convert.ToInt32(st.ToString("yyyyMMdd"));
                var endTime = Convert.ToInt32(et.ToString("yyyyMMdd"));
                return new Tuple<bool, object>(true, new { startTime = startTime, endTime = endTime, startTimeS = st.ToString("yyyy-MM-dd 00:00:00"), endTimeS = et.AddDays(1).ToString("yyyy-MM-dd 00:00:00") });
            }
            else if (dateStatus == 5)
            {
                var m = d.AddMonths(-1);
                var st = m.AddDays(-(int)m.Day + 1);
                var et = m.AddMonths(1).AddDays(-m.AddMonths(1).Day + 1).AddDays(-1);
                var startTime = Convert.ToInt32(st.ToString("yyyyMMdd"));
                var endTime = Convert.ToInt32(et.ToString("yyyyMMdd"));
                return new Tuple<bool, object>(true, new { startTime = startTime, endTime = endTime, startTimeS = st.ToString("yyyy-MM-dd 00:00:00"), endTimeS = et.AddDays(1).ToString("yyyy-MM-dd 00:00:00") });
            }
            return new Tuple<bool, object>(false, new { startTime = 0, endTime = 0 });
        }
        /// <summary>
        /// 给定日期获取周几
        /// 会返回对应时区的周几
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string GetWeekDayName(DateTime dateTime)
        {
            string[] Day = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            return Day[Convert.ToInt32(dateTime.DayOfWeek.ToString("d"))].ToString();
        }
        /// <summary>
        /// 给定日期获取周几
        /// 会返回对应时区的周几 
        /// item1:星期*    从 星期日 开始， 
        /// item2: index  从  0   开始
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Tuple<string,int> GetWeekDayNameAndIndex(DateTime dateTime)
        {
            string[] Day = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            return new Tuple<string, int>(Day[Convert.ToInt32(dateTime.DayOfWeek.ToString("d"))].ToString(), Convert.ToInt32(dateTime.DayOfWeek.ToString("d")));
        }

        /// <summary>
        /// ip 转整型数据
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <returns></returns>
        public static int GetIntFromIP(string ipaddress)
        {
            try
            {
                IPAddress IPAddr = IPAddress.Parse(ipaddress);
                return IPAddr.GetHashCode();
            }
            catch (Exception)
            {
                IPAddress IPAddr = IPAddress.Parse("127.0.0.1");
                return IPAddr.GetHashCode();
            }
            
            ////网上的代码是得到字节组再转换成int
            //byte[] byt = IPAddr.GetAddressBytes();
            //int intIP = System.BitConverter.ToInt32(byt, 0);
            //Console.WriteLine("字节转换结果：{0}", intIP);
        }
        /// <summary>
        /// long 转到ip
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static string GetIPFromInt(long address)
        {
            return IPAddress.Parse(address.ToString()).ToString();
        }
        /// <summary>
        /// long 转到16进制
        /// </summary>
        /// <param name="nubmer"></param>
        /// <returns></returns>
        public static string longConvertToHex(long nubmer)
        {
            return nubmer.ToString("X");
        }

        /// <summary>
        /// 反加key=value&
        /// </summary>
        /// <param name="m_values"></param>
        /// <returns></returns>
        public static string ToUrl(SortedDictionary<string,object> m_values)
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    //Log.Error(this.GetType().ToString(), "WxPayData内部含有值为null的字段!");
                    //throw new WxPayException("WxPayData内部含有值为null的字段!");
                }

                if (pair.Key != "sign" && pair.Value.ToString() != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }
        /// <summary>
        /// 加密返回小写md5
        /// </summary>
        /// <param name="m_values"></param>
        /// <param name="merchantKey"></param>
        /// <returns></returns>
        public static string MakeSign(SortedDictionary<string, object> m_values,string merchantKey)
        {
            //转url格式
            string str = ToUrl(m_values);
            //在string后加入API KEY
            str += "&key=" + merchantKey;
            //所有字符转为大写
            return GetMd5(str).ToLower();
        }

        /// <summary>
        /// Verify If the string is a valid JSON String
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Url Encode
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static string UrlEncode(string strInput)
        {
            return System.Net.WebUtility.UrlEncode(strInput);
        }
        /// <summary>
        /// Url UrlDecode
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static string UrlDecode(string strInput) {
            return System.Net.WebUtility.UrlDecode(strInput);
        }


        #region  更新赋值,前者赋值给后者 泛型+反射
        /// <summary>
        /// 更新实体赋值,前者赋值给后者 泛型+反射
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="pModel"></param>
        /// <returns></returns>
        public static T ConvertModel<T, P>(P pModel) {
            T ret = System.Activator.CreateInstance<T>();

            List<PropertyInfo> p_pis = pModel.GetType().GetProperties().ToList();
            PropertyInfo[] t_pis = typeof(T).GetProperties();

            foreach (PropertyInfo pi in t_pis) {
                //可写入数据
                if (pi.CanWrite) {
                    //忽略大小写
                    var name = p_pis.Find(s => s.Name.ToLower() == pi.Name.ToLower());
                    if (name != null && pi.PropertyType.Name == name.PropertyType.Name) {
                        pi.SetValue(ret, name.GetValue(pModel, null), null);
                    }

                }
            }

            return ret;
        }

        #endregion
        /// <summary>
        /// 保留2位小数 不四舍五入
        /// </summary>
        /// <param name="d_Input"></param>
        /// <returns></returns>
        public static decimal ConvertF2Decimal(decimal d_Input) {
            if(d_Input == 0) 
                return Convert.ToDecimal(0.ToString("F2"));
            return ((int)(d_Input * 100)) / 100.0m;
        }
        /// <summary>
        /// 查询 年月 如：202405
        /// </summary>
        /// <returns></returns>
        public static string MonthToLong() {
            var currentDate = DateTime.UtcNow.AddHours(8);
            if (Convert.ToInt64(currentDate.ToString("yyyyMM")) > 202405)
                return currentDate.ToString("yyyyMM");
            else
                return "";

        }
        /// <summary>
        /// 字串数据转，sql拼接方式
        /// 如：new List<string>() { "foo", "boo"} 转换为  'foo','boo'
        /// </summary>
        /// <param name="strings">字串List</param>
        /// <returns></returns>
        public static string ToStringJoinSql(IList<string> strings) {
            
            return string.Join(",", strings.Select(s => "'" + s + "'"));

        }


        /// <summary>
        /// Ching Security Method
        /// </summary>
        /// <param name="account"></param>
        /// <param name="callfrom"></param>
        /// <param name="ipaddress"></param>
        /// <param name="calltime"></param>
        /// <param name="callaction"></param>
        /// <param name="loginToken"></param>
        /// <returns></returns>
        public static string GetToken3(string account, int callfrom, string ipaddress,
               string calltime, string callaction, string loginToken, string jwtEncoderKey, string desEncodeKey)
        {
            var json = JsonConvert.SerializeObject(new
            {
                account = account,
                callfrom = callfrom,
                ipaddress = ipaddress,
                calltime = calltime,
                callaction = callaction,
                pKey = loginToken
            });

            byte[] key = Encoding.UTF8.GetBytes(jwtEncoderKey);
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();//加密方式
            IJsonSerializer serializer = new JsonNetSerializer();//序列化Json
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();//base64加解密
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);//JWT编码
            var token = encoder.Encode(new
            {
                awcSign = UtilsLib.Common.DESEnCode(json, desEncodeKey)
            }, key);//生成令牌

            return token;
        }
    }
}
