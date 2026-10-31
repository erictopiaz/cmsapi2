using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.lang;

namespace ShareEntity.Models {

    /// <summary>
    /// vip 等级列表
    /// </summary>
    public class VipLevelINTL {

        /// <summary>
        /// 货币和语系对应 浏览器的CultureInfo
        /// 1 英文, 2 简体中文,3 繁体中文,4 泰文,5 越南文, 6 马来文， 7：印度
        /// 会员注册时 默认  10001 
        /// </summary>
        public static Dictionary<int, string> levelIntlName = new Dictionary<int, string>() {
                { 10001, "VIP 0" },
                { 10027, "VIP 1" },
                { 10028, "VIP 2" },
                { 10002, "VIP 3" },
                { 10003, "VIP 4" },
                { 10004, "VIP 5" },
            {10025, "VIP 6"},
            {10005, "VIP 7"}
        };

    }
}
