using System;
using System.Collections.Generic;
using System.Text;
using static UtilsLib.lang.LangCodes;

namespace ShareEntity.InputModels.Game.BTI {
    public class BTIParamsModel {
        /// <summary>
        /// BTi 代理商用户的名 merchant ID
        /// </summary>
        public string AgentUserName { set; get; }
        /// <summary>
        /// BTi 代理商用户密码 password
        /// </summary>
        public string AgentPassword { set; get; }
        

        public string apiUrl { set; get; }

        public string loginUrl { set; get; }

        public string prefix { set; get; }
        public string gameName { set; get; } = "BTI";
        /// <summary>
        /// 反向 访问游戏api
        /// </summary>
        public string proxy { set; get; }
    }


    //AgentUserName 是 string (50) BTi 代理商用户的名 merchant ID
    //AgentPassword 是 string (50) BTi 代理商用户密码 password
    //MerchantCustomerCode 是 string (50) 玩家账号是唯一登入的，必须是无重复
    //(支持英文字母数字, 下划线, 划线, 电邮
    //符号 @ 或 英文的句点)其他语言符号不
    //支持。这个参数是每个接口用的唯一值
    //来锁定哪个玩家在 BTI。
    //LoginName 是 string (50) 玩家账号，不可重复(支持英文字母数
    //字, 下划线, 划线, 电邮符号 @ 或 英文的
    //句点)其他语言符号不支持。
    //CurrencyCode 是 string (3) 货币必须英文.ISO 4217 标准.如 RMB,
    //CNY
    //CountryCode 是 string (2) 国家必须英文 ISO 3166-1 标准.如 CN
    //City 是 string (50) 城市，可以中文
    //FirstName 是 string (100) 姓， 可以中文
    //LastName 是 string (100) 名，可以中文
    //Group1ID 是 Int 请放 0（新玩家）或 1 （普通玩家)
}
