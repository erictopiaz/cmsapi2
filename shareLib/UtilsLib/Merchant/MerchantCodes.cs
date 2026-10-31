using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilsLib.Merchant
{
    /// <summary>
    /// 品牌和对应的钱包类型
    /// </summary>
    public class MerchantCodes
    {
        /// <summary>
        /// 货币和语系对应 浏览器的CultureInfo
        /// 1 英文, 2 简体中文,3 繁体中文,4 泰文,5 越南文, 6 马来文， 7：印度
        /// </summary>
        public static Dictionary<string, MerchantModel> merhantMaps = new Dictionary<string, MerchantModel>() {
            { "Awc", new MerchantModel(){ merchantAccount="Awc",walletType=1, currencyType=156,currency="CNY", default_affaccount="allwincity", intl = 0, lang=2} },
            { "Bayo", new MerchantModel(){ merchantAccount="Bayo",walletType=0, currencyType=356,currency="INR", default_affaccount="omg356", intl = 1, lang=1} },
            { "omg356", new MerchantModel(){ merchantAccount="Bayo",walletType=0, currencyType=356,currency="INR", default_affaccount="omg356", intl = 1, lang=1} },
            { "Boss", new MerchantModel(){ merchantAccount="Boss",walletType=0, currencyType = 156,currency="CNY", default_affaccount="boaaboss156", intl = 1, lang=2}}
        };

        private static Dictionary<string, List<DefaultModel>> merhantMaps_V = new Dictionary<string, List<DefaultModel>>() {
            { "Awc", new List<DefaultModel>(){
                            new DefaultModel(){ merchantAccount="Awc", prefix="", walletType=1, currencyType=156,currency="CNY", default_affaccount="allwincity", intl = 0, lang=2, citizenship="中国" }
                        }
            },
            { "Bayo", new List<DefaultModel>(){
                            new DefaultModel(){ merchantAccount="Bayo", prefix="", walletType=0, currencyType=356,currency="INR", default_affaccount="omg356", intl = 1, lang=1, citizenship="" }
                        }
            },
            { "omg356", new List<DefaultModel>(){
                        new DefaultModel() { merchantAccount="Bayo", prefix="", walletType = 0, currencyType = 356, currency = "INR", default_affaccount = "omg356", intl = 1, lang = 1 , citizenship="" }
                    }
            },
            { "Boss", new List<DefaultModel>(){
                        new DefaultModel() { merchantAccount="Boss", prefix="bocc", walletType = 0, currencyType = 356, currency = "INR", default_affaccount = "boccboss356", intl = 1, lang = 1, citizenship="" },
                        new DefaultModel() { merchantAccount="Boss", prefix="boaa", walletType = 0, currencyType = 156,currency="CNY", default_affaccount="boaaboss156", intl = 1, lang=2, citizenship="中国" }
                    }
            }
        };


        public static DefaultModel GetDefaultModel(string merchantAccount, int currencyType)
        {
            if (merchantAccount.Equals("Bayo",StringComparison.OrdinalIgnoreCase) 
                || merchantAccount.Equals("omg356", StringComparison.OrdinalIgnoreCase))
            {
                //兼容 Bayo没有 加密token中没有currecyType ,
                //后面升级后，取消这里操作
                return merhantMaps_V[merchantAccount].First(); 
            }
            if (!merhantMaps_V.ContainsKey(merchantAccount))
                return null;
            var defModels = merhantMaps_V[merchantAccount].Where(x => x.currencyType == currencyType);
            return (defModels != null && defModels.Count() > 0) ? defModels.FirstOrDefault() : null;
        }

        /// <summary>
        /// 获取账号集合，方便 sql查询时account in 
        /// 会员账号或代理账号
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static string GetAccounts(string account)
        {
            if (account.StartsWith("boaa") || account.StartsWith("bocc"))
            {
                account = account.Remove(0, 4);
            }

            return string.Format(@"('{0}','boaa{0}','bocc{0}')", account);
        }
    }


    public class MerchantModel
    {

        /// 品牌名
        /// </summary>
        public string merchantAccount { set; get; }
        /// <summary>
        /// 钱包类型 1：转账钱包，0：单一钱包
        /// </summary>
        public int walletType { set; get; }
        /// <summary>
        /// 货币类型 156：cny, 356 inr, 
        /// </summary>
        public int currencyType { set; get; }
        public string currency { set; get; }
        /// <summary>
        /// 0:非外站，1：外站 （会员账号又是合营账号【如果代理激活】）
        /// </summary>
        public int intl { set; get; }
        /// <summary>
        ///  1 英文, 2 简体中文,3 繁体中文,4 泰文,5 越南文, 6 马来文， 7：印度
        /// </summary>
        public int lang { set; get; }
        /// <summary>
        /// 品牌默认的代理账号
        /// </summary>
        public string default_affaccount { set; get; } = "";

    }

    public class DefaultModel
    {
        /// <summary>
        /// 品牌名
        /// </summary>
        public string merchantAccount { set; get; }
        /// <summary>
        /// 钱包类型 1：转账钱包，0：单一钱包
        /// </summary>
        public int walletType { set; get; }
        /// <summary>
        /// 货币类型 156：cny, 356 inr, 
        /// </summary>
        public int currencyType { set; get; }
        public string currency { set; get; }
        /// <summary>
        /// 0:非外站，1：外站 （会员账号又是合营账号【如果代理激活】）
        /// </summary>
        public int intl { set; get; }
        /// <summary>
        ///  1 英文, 2 简体中文,3 繁体中文,4 泰文,5 越南文, 6 马来文， 7：印度
        /// </summary>
        public int lang { set; get; }
        /// <summary>
        /// 品牌默认的代理账号
        /// </summary>
        public string default_affaccount { set; get; } = "";

        /// <summary>
        /// 会员账号前缀 awc 无前缀
        /// </summary>
        public string prefix { set; get; } = "";
        public string citizenship { set; get; } = "";
    }
}
