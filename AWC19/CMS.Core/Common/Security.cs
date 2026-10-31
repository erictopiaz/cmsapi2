using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CMS.Core
{
    public static partial class Common
    {
        public static class Security
        {

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

            public static string ObscureString(string withdrawcard)
            {
                try
                {
                    if(string.IsNullOrEmpty(withdrawcard)) 
                        { return "****"; }
                    return string.Concat(withdrawcard.Substring(0, 2), "****", withdrawcard.Substring(withdrawcard.Length - 2, 2));
                }
                catch (Exception)
                {
                    return "****";
                }
            }
            public static string ObscureWithdrawCard(string withdrawcard)
            {
                try
                {
                    return string.Concat(withdrawcard.Substring(0, 4), "****", withdrawcard.Substring(withdrawcard.Length - 4, 4));
                }
                catch (Exception)
                {
                    return "****";
                }
            }
            public static string DESDeCode_Name(string? pToDecrypt)
            {
                string DES_Key = "8pi*aWc#";
                string jsonString = DESDeCode(pToDecrypt, DES_Key);
                var jsonDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);

                if (jsonDict.ContainsKey("Account")) return jsonDict["Account"];

                return "";

            }

            public static Dictionary<string, string> DESDeCode_NameJson(string? pToDecrypt) {
                string DES_Key = "8pi*aWc#";
                string jsonString = DESDeCode(pToDecrypt, DES_Key);
               return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);

                //if (jsonDict.ContainsKey("Account")) return jsonDict["Account"];
                //return "";

            }

            public static string QueryCurrencyTypes(IEnumerable<Claim> claims , string claimKey= "CurrencyTypes") {
                
                foreach (var claim in claims) {
                    if(claim.Type == claimKey) 
                        return claim.Value;
                }

                return "";

            }

            public static string DESDeCode(string pToDecrypt, string DES_Key)
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
                for (int x = 0; x < pToDecrypt.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }

                des.Key = ASCIIEncoding.ASCII.GetBytes(DES_Key);
                des.IV = ASCIIEncoding.ASCII.GetBytes(DES_Key);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                StringBuilder ret = new StringBuilder();

                return System.Text.Encoding.UTF8.GetString(ms.ToArray());
            }


            /// <summary>
            /// Double hashed use to store password 
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public static string DoubleMD5Hash(string input)
            {
                return MD5Hash(MD5Hash(input));
            }

            public static string MD5Hash(string input)
            {

                return string.Join("",
                  new MD5CryptoServiceProvider().ComputeHash(
                    new MemoryStream(Encoding.UTF8.GetBytes(input))).Select(x => x.ToString("X2")));
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


            public static string DESEnCode(string pToEncrypt, string DES_Key)
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.GetEncoding("UTF-8").GetBytes(pToEncrypt);

                des.Key = ASCIIEncoding.ASCII.GetBytes(DES_Key);
                des.IV = ASCIIEncoding.ASCII.GetBytes(DES_Key);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                StringBuilder ret = new StringBuilder();
                foreach (byte b in ms.ToArray())
                {
                    ret.AppendFormat("{0:X2}", b);
                }
                ret.ToString();
                return ret.ToString();
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
                   string calltime, string callaction, string loginToken)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    account = account,
                    callfrom = callfrom,
                    ipaddress = ipaddress,
                    calltime = calltime,
                    callaction = callaction,
                    pKey = loginToken
                });

                byte[] key = Encoding.UTF8.GetBytes("asldkjfvaoe84runvaofjna$awc.proapi");
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();//加密方式
                IJsonSerializer serializer = new JsonNetSerializer();//序列化Json
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();//base64加解密
                IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);//JWT编码
                var token = encoder.Encode(new
                {
                    awcSign = DESEnCode(json, "9bt*a$c#")
                }, key);//生成令牌

                return token;
            }


        }
    }
}
