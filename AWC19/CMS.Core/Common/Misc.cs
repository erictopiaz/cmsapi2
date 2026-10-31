using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace CMS.Core
{
    public static partial class Common
    {

        public class Misc
        {

            public static Dictionary<string, string> FromJsonToDict(string json) {
                try
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            public static string ChingApiTrigger(string account, string api, string jsonData)
            {
                string fqdn = "";
                try
                {
                    var promoApiToken = Common.Security.GetToken3(account, 4, "127.255.255.255",
                        DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"), api, Guid.NewGuid().ToString());


                    WebClient wcching = new WebClient();
                    wcching.Headers[HttpRequestHeader.ContentType] = "application/json";
                    wcching.Headers[HttpRequestHeader.Authorization] = promoApiToken;
                    wcching.Encoding = System.Text.Encoding.UTF8;
                    fqdn = string.Concat("http://awcmgt22-promoservice.azurewebsites.net" + api);
                    var result = wcching.UploadString(new Uri(fqdn),
                        jsonData);

                    return result;
                }
                catch (WebException ex)
                {
                    //var result = ex.Response.GetResponseStream();
                    using (Stream stream = ex.Response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        String responseString = reader.ReadToEnd();
                        return responseString;
                    }
                }



            }

        }

    }
}
