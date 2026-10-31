using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.agent
{
    public class QueryaffBindInputModel
    {
        /// <summary>
        /// 代理账号
        /// </summary>
        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = false, ErrorMessage = "agent_account_format_error")]
        public string account { get; set; }

        [IpValidate(50, MinimumLength = 7, IsAllowedEmpty = true, ErrorMessage = "ip_formart_error")]
        public string requestip { get; set; }

        [SqlInjectAttibute(IsAllowedEmpty = true)]
        public string requestaddress { get; set; }

        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "admin_account_format_error")]
        public string adminAccount { get; set; }

        /// <summary>
        /// 0:银行卡，1：虚拟卡,100:全部卡类型
        /// </summary>
        public int bankType { set; get; } = 100;
        /// <summary>
        /// 1:android，2：ios,   3:h5  , 4: pc , 5: 体育ios,6: 体育android
        /// </summary>
        public int device { set; get; }
    }
    
    /// <summary>
    /// 根据卡号查询结果
    /// </summary>
    public class QueryAffCardInputModel
    {
        /// <summary>
        /// 代理账号
        /// </summary>
        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = false, ErrorMessage = "agent_account_format_error")]
        public string account { get; set; }
       
        /// <summary>
        /// 代理银行卡号
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true)]
        public string cardno { get; set; }
       
        /// <summary>
        /// 代理取款IP
        /// </summary>
        [IpValidate(19, MinimumLength = 7, IsAllowedEmpty = true, ErrorMessage = "ip_formart_error")]
        public string requestip { get; set; }

        [SqlInjectAttibute(IsAllowedEmpty = true)]
        public string requestaddress { get; set; }
    }


    public class affBindInputModel
    {
        /// <summary>
        /// 代理账号
        /// </summary>
        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = false, ErrorMessage = "agent_account_format_error")]
        public string account { get; set; }

        /// <summary>
        /// 代理密码
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true)]
        public string password { get; set; }
        /// <summary>
        /// 代理姓名
        /// </summary>
        [ChineseAndChar(10, 0, IsAllowedEmpty = false, MinimumLength = 2, ErrorMessage = "agent_realname_format_error")]
        public string owner { get; set; }

        /// <summary>
        /// 代理银行卡号
        /// </summary>
        [CharAndDigital(50, IsAllowedEmpty = true, MinimumLength = 16, ErrorMessage = "agent_withdrawal_account_format_error")]
        public string cardno { get; set; }
        /// <summary>
        /// 代理银行卡名
        /// </summary>
        [ChineseAndChar(12, 3, IsAllowedEmpty = false, MinimumLength = 1, ErrorMessage = "agent_cardname_format_error")]
        public string bankName { get; set; }
        /// <summary>
        /// 代理开户行地址
        /// </summary>
        [ChineseAndChar(50, 4, IsAllowedEmpty = true, MinimumLength = 2, ErrorMessage = "agent_cardaddress_format_error")]
        public string bankAddress { get; set; }
        /// <summary>
        /// 银行代码
        /// </summary>
        [ChineseAndChar(12, 2, IsAllowedEmpty = true, MinimumLength = 1, ErrorMessage = "agent_bankCode_format_error")]
        public string bankCode { get; set; }
        /// <summary>
        /// 代理取款IP
        /// </summary>
        [IpValidate(50, MinimumLength = 7, IsAllowedEmpty = true, ErrorMessage = "ip_formart_error")]
        public string requestip { get; set; }

        [SqlInjectAttibute(IsAllowedEmpty = true)]
        public string requestaddress { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>

        [SqlInjectAttibute(IsAllowedEmpty = true)]
        public string remarks { get; set; }
        /// <summary>
        /// 姓名拼音
        /// </summary>

        [CharAndDigital(25, IsAllowedEmpty = true, MinimumLength = 4, ErrorMessage = "agent_realname_pinyin_format_error")]
        public string ownerPY { set; get; }
        /// <summary>
        /// swift code
        /// </summary>

        [CharAndDigital(25, IsAllowedEmpty = true, MinimumLength = 4, ErrorMessage = "agent_bank_swift_format_error")]
        public string swift { set; get; }

        /// <summary>
        /// 0:银行卡号，1：虚拟卡号
        /// </summary>
        public int bankType { set; get; } = 0;


        /// <summary>
        /// 代理姓名
        /// </summary>
        [ChineseAndChar(10, 0, IsAllowedEmpty = true, MinimumLength = 2, ErrorMessage = "agent_realname_format_error")]
        public string realname { get; set; }

        /// <summary>
        /// 代理银行卡名
        /// </summary>
        [ChineseAndChar(12, 0, IsAllowedEmpty = true, MinimumLength = 2, ErrorMessage = "agent_cardname_format_error")]
        public string cardbank { get; set; }
        /// <summary>
        /// 代理开户行地址
        /// </summary>
        [ChineseAndChar(20, 0, IsAllowedEmpty = true, MinimumLength = 2, ErrorMessage = "agent_cardaddress_format_error")]
        public string cardaddress { get; set; }

    }

    public class affBindInputFromCMSModel : affBindInputModel
    {
        /// <summary>
        /// 代理账号
        /// </summary>
        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "agent_account_format_error")]
        public string adminAccount { get; set; }
        
    }

    public class affRemoveBindInputModel
    {
        /// <summary>
        /// 代理账号
        /// </summary>
        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = false, ErrorMessage = "agent_account_format_error")]
        public string account { get; set; }
        /// <summary>
        /// 代理密码
        /// </summary>
        [CharAndDigital(20, MinimumLength = 6, IsAllowedEmpty = true, ErrorMessage = "agent_password_format_error")]
        public string password { get; set; }

        /// <summary>
        /// 代理银行卡号
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true)]
        public string cardno { get; set; }
        /// <summary>
        /// 代理银行卡名 中、英文数字或_
        /// </summary>
        [ChineseAndCharAndUnderScore(16,MinimumLength =3,IsAllowedEmpty =true,ErrorMessage = "bankName格式有误")]
        public string bankName { get; set; }
        /// <summary>
        /// 代理开户行地址
        /// </summary>
        [ChineseAndChar(50, 4, IsAllowedEmpty = true, MinimumLength = 2, ErrorMessage = "agent_cardaddress_format_error")]
        public string bankAddress { get; set; }

        /// <summary>
        /// 代理取款IP
        /// </summary>
        [IpValidate(50, MinimumLength = 7, IsAllowedEmpty = true, ErrorMessage = "ip_formart_error")]
        public string requestip { get; set; }

        [SqlInjectAttibute(IsAllowedEmpty = true)]
        public string requestaddress { get; set; }
    }

    public class CardBase
    {
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string cardno { set; get; } = "";
        /// <summary>
        /// 银行名
        /// </summary>
        public string bankName { set; get; } = "";
        /// <summary>
        /// 银行地址
        /// </summary>
        public string bankAddress { set; get; } = "";
        /// <summary>
        /// 卡主
        /// </summary>
        public string owner { set; get; } = "";
        public string icon { set; get; } = "";
        /// <summary>
        /// 默认银行代码
        /// </summary>
        public string bankCode { set; get; } = "";

        /// <summary>
        /// 0:银行卡，1：虚拟卡
        /// </summary>
        public int bankType { set; get; }


        public string remarks { set; get; } = "";
        public string ownerPY { set; get; } = "";
        public string swift { set; get; } = "";
    }

    public class AffCardBase:CardBase
    {
        public string verifyperson { set; get; }
    }


    ///// <summary>
    ///// 绑定的银行卡信息
    ///// </summary>
    //public class ofBankCards : CardBase
    //{
    //    /// <summary>
    //    /// 账号
    //    /// </summary>
    //    public string account { set; get; }

    //    /// <summary>
    //    /// 谁绑卡
    //    /// </summary>
    //    public string who_bind { set; get; } = "";
    //    /// <summary>
    //    /// 默认绑定后可以使用
    //    /// </summary>
    //    public int cardStatus { set; get; } = 1;

    //    /// <summary>
    //    /// 默认银行代码
    //    /// </summary>
    //    public string bankCode { set; get; } = "";

    //    /// <summary>
    //    /// 账号类型 0:会员账号，1：代理账号
    //    /// </summary>
    //    public int accountType { set; get; } = 1;

    //    public string requestip { set; get; } = "";
    //    /// <summary>
    //    /// 0:银行卡，1：虚拟卡
    //    /// </summary>
    //    public int bankType { set; get; }
    //    /// <summary>
    //    /// 代理的审查人员
    //    /// </summary>
    //    public string verifyperson { set; get; }

    //}
}
