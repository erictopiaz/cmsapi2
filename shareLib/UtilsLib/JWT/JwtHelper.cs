using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Sockets;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;

namespace UtilsLib.JWT
{
    /// <summary>
    /// jwt token 生成和解密
    /// </summary>
    public class JwtHelper
    {

        #region jwt 低内存解密

        private static TokenValidationParameters validations = new TokenValidationParameters() { IssuerSigningKey=null };
        private static TokenValidationParameters setV(string secret) {
            return new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };
        }

        /// <summary>
        /// app,pc 请求验证
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secret"></param>
        /// <param name="DES_Key"></param>
        /// <param name="needDesKey">是否要des key 解密，true:是，false:否,  默认为true</param>
        /// <returns></returns>
        public static TokenModel GetRquestAuthNew(string token, string secret, string DES_Key, bool needDesKey=true) {
            try {
                token = token.Replace("Bearer ", "");
                token = token.Replace("bearer ", "");
                lock (validations) {
                    if(validations.IssuerSigningKey == null) {
                        validations = setV(secret);
                    }
                }
                var handler = new JwtSecurityTokenHandler();
                var claims = handler.ValidateToken(token, validations, out var tokenSecure);
                var keys = claims.Identities.ToList();
                string awcSign = claims.Claims.Where(c => c.Type == "awcSign")
    .Select(x => x.Value).FirstOrDefault();
                var json = needDesKey ? Common.DESDeCode(awcSign, DES_Key) : awcSign;
                var tokenModel = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(json);
                return tokenModel;
            }
            catch (Exception) {
                return null;
            }

        }


        /// <summary>
        /// app,pc 请求验证
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secret"></param>
        /// <param name="DES_Key"></param>
        /// <param name="needDesKey">是否要des key 解密，true:是，false:否,  默认为true</param>
        /// <returns></returns>
        public static Tuple<string,string> GetRquestAuthUserData(string token, 
            string secret, 
            string DES_Key, 
            bool needDesKey = true)
        {
            try
            {
                token = token.Replace("Bearer ", "");
                token = token.Replace("bearer ", "");
                lock (validations)
                {
                    if (validations.IssuerSigningKey == null)
                    {
                        validations = setV(secret);
                    }
                }
                var handler = new JwtSecurityTokenHandler();
                var claims = handler.ValidateToken(token, validations, out var tokenSecure);

                var keys = claims.Identities.ToList();
                string accountJson = claims.Claims.Where(c => c.Type == ClaimTypes.Name)
    .Select(x => x.Value).FirstOrDefault();
                string userData = claims.Claims.Where(c => c.Type == ClaimTypes.UserData)
    .Select(x => x.Value).FirstOrDefault();
                var json = Common.DESDeCode(accountJson, DES_Key);
                return new Tuple<string,string>(json, userData);
            }
            catch (Exception)
            {
                return new Tuple<string, string>("", "");
            }

        }

        #endregion


        /// <summary>
        /// 获取token 指定 claim
        /// </summary>
        /// <param name="secert"></param>
        /// <param name="authHeader"> token </param>
        /// <param name="claimType">指定 claim</param>
        /// <returns></returns>
        public static string DecodeToken(string secert, string authHeader, string claimType)
        {
            var handler = new JwtSecurityTokenHandler();
            authHeader = authHeader.Replace("bearer ", "");
            var jsonToken = handler.ReadToken(authHeader);
            var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;

            return tokenS.Claims.First(claim => claim.Type == claimType).Value;
        }

        /// <summary>
        /// 生成TOKEN
        /// </summary>
        /// <param name="Account"></param>
        /// <param name="access"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string CreateToken(string Account, string access, string ip="127.255.255.255")
        {
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("asldkjfvaoe84runvaofjna");
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, Account.ToUpper()),
                    new Claim(ClaimTypes.UserData, access)
                }),
                Expires = DateTime.UtcNow.AddDays(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
            });
            return tokenHandler.WriteToken(token);
        }

        public static string GetName(string token, string secret)
        {
            try
            {
                token = token.Replace("bearer ", "");
                var key = Encoding.ASCII.GetBytes(secret);
                var handler = new JwtSecurityTokenHandler();
                var validations = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                var claims = handler.ValidateToken(token, validations, out var tokenSecure);
                var keys = claims.Identities.ToList();
                var json = Common.DESDeCode(claims.Identity.Name, "8pi*aWc#");
                var tokenModel = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(json);
                return tokenModel.account;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static CMSTokenModel GetCMSTokenModel(string token, string secret) {
            try {
                token = token.Replace("bearer ", "");
                token = token.Replace("Bearer ", "");
                var key = Encoding.ASCII.GetBytes(secret);
                var handler = new JwtSecurityTokenHandler();
                var validations = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                var claims = handler.ValidateToken(token, validations, out var tokenSecure);
                //var keys = claims.Identities.ToList();
                var json = Common.DESDeCode(claims.Identity.Name, "8pi*aWc#");
                var tokenModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CMSTokenModel>(json);
                return tokenModel;
            }
            catch (Exception) {
                return new CMSTokenModel();
            }
        }

        public static IList<int> DESDeCode_ClaimValueByClaimKey(string token, string secret, string claimKey = "CurrencyTypes") {
            try {
                token = token.Replace("bearer ", "");
                var key = Encoding.ASCII.GetBytes(secret);
                var handler = new JwtSecurityTokenHandler();
                var validations = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                var claims = handler.ValidateToken(token, validations, out var tokenSecure);
                foreach (var claim in claims.Claims) {
                    if (claim.Type == claimKey)
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(claim.Value);
                }
                return new List<int>() ;
            }
            catch (Exception) {
                return new List<int>(); 
            }

        }

        public static IList<int> DESDeCode_ClaimValueByClaimKey(ClaimsPrincipal claims, string claimKey = "CurrencyTypes") {
            try {
               
                foreach (var claim in claims.Claims) {
                    if (claim.Type == claimKey)
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(claim.Value);
                }
                return new List<int>();
            }
            catch (Exception) {
                return new List<int>();
            }

        }


    


       

        /// <summary>
        /// 自定义playload 获取TOKEN
        /// </summary>
        /// <param name="playload"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public static string GetTokenByCustomPlayload(Dictionary<string, object> playload, string secretKey)
        {
            string token = String.Empty;

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            //设置过期时间
            DateTime time = DateTime.Now.AddMinutes(120);
            //playload.exp = DateTimeHelper.GetTimeStamp(time).ToString();
            //var dataString = Newtonsoft.Json.JsonConvert.SerializeObject(new { account_name = "xxx" });
            //var json = new JsonSerializer().Deserialize(new JsonTextReader(new StringReader(dataString)));

            //var dict = new Dictionary<string, object>()
            //{
            //    { "method", "account.balance"},
            //    { "app_key", "268d521b1dfc7701" },
            //    {  "data", new { account_name="xxx"} }
            //};
            ////获取私钥
            //string secret = "85519a02c7663398d557b68566a27ac0";
            ////将Token保存在缓存中
            return encoder.Encode(playload, secretKey);
        }

    }

    /// <summary>
    /// 登录账号信息和访问权限
    /// </summary>
    public class AaccountAccessTokenModel
    {
        /// <summary>
        /// 登录的账号信息
        /// </summary>
        public LoginData account { get; set; } 
        /// <summary>
        /// 访问权限
        /// </summary>
        public string access { set; get; }
    }
    public class LoginData
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }

    public class CMSTokenModel : TokenModel {
        /// <summary>
        /// 市场人员管理 代理权限
        /// </summary>
        public int allowedToManageAgent { get; set; }
    }
    public class TokenModel
    {
        /// <summary>
        /// 登录账号
        /// </summary>
        public string account { get; set; } = "";
        /// <summary>
        /// 登录ip
        /// </summary>
        public string AdminIpAddress { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public string loginTime { set; get; }
        /// <summary>
        /// 1:android，2：ios,   3:h5  , 4: pc , 5: 体育ios,6: 体育android
        /// </summary>
        public int callfrom { set; get; } = 0;
        /// <summary>
        /// 用来区分不同的图片资源
        /// 1:原生android，2：原生ios,   3:h5  , 4: pc , 5: 体育ios,6: 体育android, 7: flutter ios, 8:flutter android
        /// </summary>
        public int sourceId { set; get; } = -1;
        /// <summary>
        /// 如获取不到IP 请固定为：127.255.255.255
        /// </summary>
        public string ipaddress { set; get; }
        /// <summary>
        /// 当前调取的时间 (北京时间)  “yyyy-MM-dd HH:mm:ss”
        /// </summary>
        public string calltime { set; get; }
        /// <summary>
        /// 当前请求的path   如当前请求地址为 http://{api-domain}/api/Shared/AgentLogin      ,   callaction应为：/api/Shared/AgentLogin
        /// </summary>
        public string callaction { set; get; }
        /// <summary>
        /// 自定义加密码数据
        /// 登录后，生成的token
        /// </summary>
        public string pkey { set; get; }
        /// <summary>
        /// 客户端版本号  
        /// </summary>
        public string clientVer { set; get; }
        /// <summary>
        /// 1 英文, 2 简体中文,3 繁体中文,4 泰文,5 越南文, 6 马来文， 7：印度
        /// </summary>
        public int lang { set; get; } = 2;
        /// <summary>
        /// 0：非外站，1 ：外站
        /// </summary>
        public int intl { set; get; } = 0;
        /// <summary>
        /// 会员每次登录后唯一的guid ，后端生成
        /// </summary>
        public string guid { set; get; } = "";
        /// <summary>
        /// pc/h5 操作的域名
        /// </summary>
        public string host { set; get; } = "";
        /// <summary>
        /// 货币 默认156  cny 
        /// 156:cny , 356:inr
        /// </summary>
        public int currencyType { set; get; } = 156;
        /// <summary>
        /// 商户信息 可能会根据 不同市场时使用这个参数
        /// </summary>
        public string merchantAccount { set; get; } = "Awc";
        /// <summary>
        /// 商户的推广码（区分app是从哪个代理的apk 过来的）
        /// </summary>
        public string merchantCode { set; get; } = "";

        /// <summary>
        /// 当前设备所在地区 大写
        /// </summary>
        public string gec { set; get; }

        /// <summary>
        /// 检测验证码，是否开启成功 0：前没没有加载成功，1：前加载成功 (默认为1)
        /// 加载成功后端就要验证
        /// </summary>
        public int neCaptchaValidate { set; get; } = 1;
        /// <summary>
        /// 设备指纹
        /// </summary>
        public string fingerid { set; get; } = "";
        /// <summary>
        /// 会员钱包类型  1：转账鲆包，0：单一钱包
        /// </summary>
        public int walletType { set; get; } = 1;

        /// <summary>
        /// 品牌 
        /// </summary>
        public string appDeviceBrand { set; get; } = "";
        /// <summary>
        /// 手机型号  
        /// </summary>
        public string appDeviceModel { set; get; } = "";
        /// <summary>
        /// 系统
        /// </summary>
        public string devType { set; get; } = "";
    }
}
