
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UtilsLib;

namespace ShareEntity.Models
{



    /// <summary>
    /// 
    /// </summary>
    public partial class ofUserx:ofUser
    {
        /// <summary>
        /// affaccount 的上一级代理
        /// </summary>
        public string parentAccount { get; set; }

        public int phoneCode { get; set; }

        public string wallet { set; get; }

        public long id { get; set; }
        public long uid { get; set; }
    }

    [Table("ofUser")]
    public partial class ofUser
    {
        

        [Required]
        [StringLength(50)]
        public string currency { get; set; }

        public int regdevice { set; get; } = 0;

        public int? accounttype { get; set; }

        [ExplicitKey]
        [Required]
        [StringLength(50)]
        public string account { get; set; }

        [Required]
        [StringLength(50)]
        public string password { get; set; }

        [StringLength(50)]
        public string passkey { get; set; }

        [Required]
        [StringLength(50)]
        public string nickname { get; set; }

        [Required]
        [StringLength(150)]
        public string realname { get; set; }

        public int sex { get; set; }

        [Required]
        [StringLength(50)]
        public string birthday { get; set; }

        [Required]
        [StringLength(150)]
        public string email { get; set; }

        [StringLength(150)]
        public string telephone { get; set; }



        [StringLength(150)]
        public string qq { get; set; }

        [StringLength(150)]
        public string yahoo { get; set; }

        [StringLength(150)]
        public string msn { get; set; }

        [Required]
        [StringLength(150)]
        public string country { get; set; }

        public int status { get; set; }
        /// <summary>
        /// 1:表示账号被风控
        /// </summary>
        public int anomaly { get; set; }

        public int level { get; set; }

        public DateTime createtime { get; set; }

        [Required]
        [StringLength(150)]
        public string referrer { get; set; }

        [Required]
        [StringLength(150)]
        public string referrerreg { get; set; }

        public decimal amountactive { get; set; }

        public decimal? amount_ew { get; set; }

        public decimal? amount_hg { get; set; }

        public decimal amount_gd { get; set; }

        public decimal? amount_pt { get; set; }

        public decimal? amount_sun { get; set; }

        public decimal? amount_ph { get; set; }

        public decimal? amount_ssc { get; set; }

        public decimal? amount_keno { get; set; }

        public decimal? amount_poker { get; set; }

        public decimal? amount_bbin { get; set; }

        public decimal amountsuspend { get; set; }

        [Required]
        [StringLength(150)]
        public string ipaddress { get; set; }

        [Required]
        [StringLength(250)]
        public string macaddress { get; set; }

        public int depositnumber { get; set; }

        public decimal depositamount { get; set; }

        public int withdrawalnumber { get; set; }

        public decimal withdrawalamount { get; set; }

        public int loginnumber { get; set; }

        [StringLength(50)]
        public string affaccount { get; set; }
        [Required]
        [StringLength(50)]
        public string bbaffaccount { get; set; }

        [StringLength(500)]
        public string freezereason { get; set; }

        public decimal bonuspoints { get; set; }

        public int eavendorid { get; set; }

        [StringLength(250)]
        public string eaacode { get; set; }

        [StringLength(50)]
        public string favgame { get; set; }

        public int? acceptmail { get; set; }

        [StringLength(50)]
        public string marcode { get; set; }

        public int? eaactive { get; set; }

        public int? ptactive { get; set; }

        public int pttactive { get; set; }

        public int? sunactive { get; set; }

        public int? kenoactive { get; set; }

        public int? hgactive { get; set; }

        public int? gdactive { get; set; }

        public int? ewstrict { get; set; }

        public int? byactive { get; set; }

        public int? tyactive { get; set; }

        public int cwactive { get; set; }

        public int uedactive { get; set; }

        [StringLength(50)]
        public string sunaccount { get; set; }

        [StringLength(50)]
        public string ptaccount { get; set; }

        [StringLength(50)]
        public string pttaccount { get; set; }

        [StringLength(50)]
        public string bbinaccount { get; set; }

        [StringLength(250)]
        public string kgbase { get; set; }

        public DateTime? transtatus { get; set; }

        [StringLength(50)]
        public string token { get; set; }

        [StringLength(50)]
        public string token_clientip { get; set; }

        public DateTime? token_timesmtp { get; set; }

        public int trynumber { get; set; }

        [StringLength(500)]
        public string validatecode { get; set; }

        public int validatestatus { get; set; }

        public int validatepasswordstatus { get; set; }

        public int? agactive { get; set; }

        [StringLength(250)]
        public string agaccount { get; set; }
        public string agaccount2 { get; set; }

        public int lbsscactive { get; set; }

        [StringLength(250)]
        public string lbsscaccount { get; set; }

        public string withdrawcards { get; set; }

        [StringLength(500)]
        public string gamecontrol { get; set; }

        public decimal bsgamount { get; set; }

        public int bsgactive { get; set; }

        public int keno2active { get; set; }

        public string misc { get; set; }
        /// <summary>
        /// 会员当前币种
        /// </summary>
        public int currencyType { set; get; } = 156;
        /// <summary>
        /// 品牌 2025-05-31
        /// </summary>
        public string merchantAccount { set; get; } = "";
        /// <summary>
        /// 1:转账钱包，0：单一钱包
        /// </summary>
        public int walletType { set; get; }

    }

    [Table("ofUser")]
    public class ofUser_WithID : ofUser
    { 
        public long id { get; set; }
       
        public int walletType { set; get; }
    }
    
    /// <summary>
    /// 确认当前客人和是不是电销带过来的
    /// </summary>
    public class OfUserSaler : ofUser
    {
        /// <summary>
        /// 0816 新增
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 电销账号
        /// </summary>
        public string saler { set; get; }
        /// <summary>
        /// 用户等级列表中的，misc
        /// </summary>
        public string umisc { get; set; }
        public int riskTypesAready { set; get; }
        /// <summary>
        /// 会员游戏钱包金额
        /// </summary>
        public decimal wallet { set; get; } = 0.0m;


    }

    public class ofUserExtention : OfUserSaler
    {
        /// <summary>
        /// 提款密码
        /// </summary>
        public string payPassword { set; get; } = "";
        public string? betCodeDetail { set; get; }
        public int? b_status { set; get; }
        public DateTime? lockdate{ set; get; }
        /// <summary>
        /// 单卡提款最低限
        /// </summary>
        public int withdrawMinAmount { set; get; }
        /// <summary>
        /// 单卡提款最高限
        /// </summary>
        public int withdrawMaxAmount { set; get; }
        /// <summary>
        /// inr数据
        /// </summary>
        public string misc_inr { set; get; }
        /// <summary>
        /// 是否可以参与，定额的提款：0：可以，1：禁止参与
        /// </summary>
        public int isLimitedfixWithdrawal { set; get; }
        /// <summary>
        /// 是否可以参与，定额支付宝的提款：0：可以，1：禁止参与
        /// </summary>
        public int isLimitedAliFixWithdrawal { set; get; } = 0;
        /// <summary>
        /// 是否可以参与，定额微信的提款：0：可以，1：禁止参与
        /// </summary>
        public int isLimitedWXFixWithdrawal { set; get; } = 0;
        /// <summary>
        /// 限制会员定额提款的金额配置区间（网银定额，支付宝定额），默认为0
        /// </summary>
        public int fixWAmountLimitLevel { set; get; } = 0;
    }


    public class misc
    {
        /// <summary>
        /// 优惠风控
        /// 1：优惠红利， 2：积分  ， 3：反水， 4： 存款反点， 5：红包雨，6：周周挑战
        /// </summary>
        public string promodisabled { set; get; }
        /// <summary>
        /// 游戏平台已经禁用
        /// </summary>
        public string gamedisabledstat { set; get; }
    }

    /// <summary>
    /// 登录响应数据
    /// </summary>
    //public class LoginResponse
    //{
        
    //    private string _account;
    //    /// <summary>
    //    /// 会员账号
    //    /// </summary>
    //    public string account {
    //        set
    //        {
    //            string encrypted = Common.EncryptDES(value, "commonex");
    //            friendcode = Common.ToBase64String(encrypted);
    //            _account = value;
    //        }
    //        get { return _account; }
    //    }
    //    /// <summary>
    //    /// 尝试登录的次数
    //    /// </summary>
    //    public int trynumber { set; get; }
    //    /// <summary>
    //    /// 会员姓名
    //    /// </summary>
    //    public string realname { set; get; }
    //    /// <summary>
    //    /// 姓名 0：女， 1：男
    //    /// </summary>
    //    public int sex { set; get; }
    //    /// <summary>
    //    /// 会员生日
    //    /// </summary>
    //    public string birthday { set; get; }
    //    private string _email;
    //    /// <summary>
    //    /// 会员邮箱
    //    /// </summary>
    //    public string email {
    //        set
    //        {
    //            if (!string.IsNullOrEmpty(value))
    //            {
    //                _email = string.Format(@"{0}*****{1}", value.Substring(0, 3), value.Substring(value.Length - 3, 3));
    //            }
    //        }
    //        get { return _email; }
    //    }
    //    private string _telephone;
    //    /// <summary>
    //    /// 会员电话
    //    /// </summary>
    //    public string telephone {
    //        set
    //        {
    //            if(!string.IsNullOrEmpty(value))
    //            {
    //                _telephone = string.Format(@"{0}*****{1}", value.Substring(0, 3), value.Substring(value.Length - 3, 3));
    //            }
    //        }
    //        get { return _telephone; }
    //    }
    //    /// <summary>
    //    /// 会员qq
    //    /// </summary>
    //    public string qq { set; get; }
    //    /// <summary>
    //    /// 会员当前状态 1：正常
    //    /// </summary>
    //    public int status { set; get; }
    //    /// <summary>
    //    /// 会员级别代码
    //    /// </summary>
    //    public string level { set; get; }
    //    /// <summary>
    //    /// 会员级别名称
    //    /// </summary>
    //    public string level_id { set; get; }
    //    /// <summary>
    //    /// 会员下一级别代码
    //    /// </summary>
    //    public int nextLevelNumber { set; get; }
    //    /// <summary>
    //    /// 会员下一级别名称
    //    /// </summary>
    //    public string nextLevels { set; get; }
    //    /// <summary>
    //    /// 当前级别logo
    //    /// </summary>
    //    public string currentVipLogo { set; get; }
    //    /// <summary>
    //    /// 下一级别logo
    //    /// </summary>
    //    public string nextVipLogo { set; get; }
    //    /// <summary>
    //    /// logoUrl
    //    /// </summary>
    //    public string logoUrl { set; get; }
    //    /// <summary>
    //    /// 会员有可用金额
    //    /// </summary>
    //    public decimal amountactive { set; get; }
    //    /// <summary>
    //    /// 会员总存款金额
    //    /// </summary>
    //    public decimal depositamount { set; get; }
    //    /// <summary>
    //    /// 会员积分
    //    /// </summary>
    //    public decimal bonuspoints { set; get; }
    //    /// <summary>
    //    /// 会员绑定的卡列表
    //    /// </summary>
    //    public List<object> bindedCards { set; get; } = new List<object>();

    //    private string _withdrawcards;

    //    public string withdrawcards {
    //        set
    //        {
    //            if(!string.IsNullOrEmpty(value))
    //            {
    //                var cs = value.Split('|');
    //                foreach(var item in cs)
    //                {
    //                    var cardArray = item.Split(',');
    //                    var cardno = string.Format(@"************{0}", cardArray[1].Substring(cardArray[1].Length - 4, 4));
    //                    var card = new { a = cardArray[0], b = cardno, c = cardArray[2] };
    //                    bindedCards.Add(card);
    //                }
    //            }
    //            _withdrawcards = "";
    //        }
    //        get { return _withdrawcards; }
    //    }
    //    /// <summary>
    //    /// 会员注册时间
    //    /// </summary>
    //    public string createtime { set; get; }
    //    public string misc {
    //        set
    //        {
    //            if(!string.IsNullOrEmpty(value))
    //            {
    //                var m = Newtonsoft.Json.JsonConvert.DeserializeObject<MiscModel>(value);
    //                bonusbalance = m.bonusbalance;
    //                prevlogin = m.prevlogin;
    //                phoneverify = m.phoneverify;
    //                recommendcode = m.recommendcode;
    //                withdrawpromo18 = m.withdrawpromo18;
    //                fonverpro2020 = m.fonverpro2020;
    //                gamedisabledstat = m.gamedisabledstat;
    //            }
    //        }
    //        get { return ""; }
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public decimal bonusbalance { set; get; }
    //    /// <summary>
    //    /// 会员上级登录时间
    //    /// </summary>
    //    public string prevlogin { set; get; }
    //    /// <summary>
    //    /// 会员电话审核状态 1：已经绑定，0：未绑定
    //    /// </summary>
    //    public int phoneverify { set; get; }
    //    public string recommendcode { set; get; }
    //    public string levelurl { set; get; } = "";
    //    public string withdrawpromo18 { set; get; } = "";
    //    public string fonverpro2020 { set; get; }
    //    public string gamedisabledstat { set; get; }
    //    /// <summary>
    //    /// 会员冻结金额
    //    /// </summary>
    //    public decimal amountsuspend { set; get; }
    //    /// <summary>
    //    /// 会员本月存款金额
    //    /// </summary>
    //    public decimal monthdeposit { set; get; }
    //    /// <summary>
    //    /// 会员本月投注额
    //    /// </summary>
    //    public decimal monthlybets { set; get; }
    //    /// <summary>
    //    /// 会员推荐好友代码
    //    /// </summary>
    //    public string friendcode { set; get; }
    //    /// <summary>
    //    /// 本月签到天数
    //    /// </summary>
    //    public int signCount { set; get; }

    //}

    //public class MiscModel
    //{
    //    public decimal bonusbalance { set; get; }
    //    public string prevlogin { set; get; }
    //    public int phoneverify { set; get; }
    //    public string recommendcode { set; get; }
    //    public string withdrawpromo18 { set; get; }
    //    public string fonverpro2020 {set;get;}
    //    public string gamedisabledstat { set; get; }
    //}
}