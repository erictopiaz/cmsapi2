using DBLibs.WXDapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareUrl
{
    public class UrlUtil
    {
        /// <summary>
        /// 存储优惠菜单的icon url
        /// </summary>
        private static Dictionary<DateTime, string> gameIconUrl = new Dictionary<DateTime, string>();
        /// <summary>
        /// 存储优惠菜单的icon url inr
        /// </summary>
        private static Dictionary<DateTime, string> gameIconUrl_inr = new Dictionary<DateTime, string>();
        /// <summary>
        /// 获取game icon url to cache
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetGameIconUrl()
        {
            if ((gameIconUrl.Count > 0 &&
                    (DateTime.UtcNow.AddHours(8) - gameIconUrl.First().Key).TotalSeconds > 30) ||
                    gameIconUrl.Count <= 0)
            {
                try
                {
                    var bodyObject = await DapperBase.ExecuteScalarValueAsync("select body from ofSettings where code='WEB_LINKS' and [status]=1",
                        null, null, 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);
                    if (bodyObject != null)
                    {
                        lock (gameIconUrl)
                        {
                            var webLinks = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WebLink>>(bodyObject.ToString());
                            if (gameIconUrl.Count > 0)
                                gameIconUrl.Clear();
                            foreach (var g in webLinks)
                            {
                                if (g.plat == "gameIconUrl")
                                    gameIconUrl.Add(DateTime.UtcNow.AddHours(8), g.path);
                            }
                        }
                    }
                    else return "";
                }
                catch (Exception)
                {
                    //可以记录异常
                    return "";
                }

            }
            return gameIconUrl.First().Value;
        }

        /// <summary>
        /// 获取game icon url to cache
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetGameIconUrl_INR()
        {
            if ((gameIconUrl_inr.Count > 0 &&
                    (DateTime.UtcNow.AddHours(8) - gameIconUrl_inr.First().Key).TotalSeconds > 60) ||
                    gameIconUrl_inr.Count <= 0)
            {
                try
                {
                    var bodyObject = await DapperBase.ExecuteScalarValueAsync("select body from ofSettings where code='WEB_LINKS_INR' and [status]=1",
                        null, null, 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);
                    if (bodyObject != null)
                    {
                        lock (gameIconUrl_inr)
                        {
                            var webLinks = JsonConvert.DeserializeObject<List<WebLink>>(bodyObject.ToString());
                            if (gameIconUrl_inr.Count > 0)
                                gameIconUrl_inr.Clear();
                            foreach (var g in webLinks)
                            {
                                if (g.plat == "gameIconUrl")
                                    gameIconUrl_inr.Add(DateTime.UtcNow.AddHours(8), g.path);
                            }
                        }
                    }
                    else return "";
                }
                catch (Exception)
                {
                    //可以记录异常
                    return "";
                }

            }
            return gameIconUrl_inr.First().Value;
        }
    }

    public class WebLink
    {
        public string plat { set; get; }
        public string path { set; get; }
        public int type { set; get; }
    }
}
