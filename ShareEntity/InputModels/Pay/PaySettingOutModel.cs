using ShareEntity.InputModels.website;
using ShareEntity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Pay
{

    public class PaySettingModel : Merchant
    {
        public int dkey { set; get; }
        public string dvalue { set; get; }
        /// <summary>
        /// 存dvalue 名称对应的语系
        /// </summary>
        public string dvalues { set; get; }
        public string icon { set; get; }
        public string navilogo { set; get; }
        public string title { set; get; }
        public string selectTitle { set; get; }
        /// <summary>
        /// 推荐使用
        /// 1：推荐
        /// </summary>
        public int recommend { set; get; }

    }


    public class Merchant
    {
        public long id { set; get; }
        public decimal minamount { set; get; }
        public decimal maxamount { set; get; }
        public string method { set; get; }
        public string name { set; get; }
        /// <summary>
        /// 存name 名称对应的语系
        /// </summary>
        public string names { set; get; }
        public int sort { set; get; }
        public int status { set; get; }
        public int appstatus { set; get; }
        public string levels { set; get; }
        public string fastpay { set; get; }
        public int paycategory { set; get; }
        public int sortid { set; get; }
        /// <summary>
        /// 银行列表
        /// </summary>
        public string banklist { set; get; }

        public ThirdPayMiscModel miscModel { set; get; }
        private string _misc;
        public string misc
        {
            set
            {
                miscModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ThirdPayMiscModel>(value);
            }
            get
            {
                return _misc;
            }
        }
        public QueryMerchantWhereModel queryMerchantWhereModel { set; get; }
        private string _queryMerchantWhere;
        public string queryMerchantWhere
        {
            set
            {
                queryMerchantWhereModel = Newtonsoft.Json.JsonConvert.DeserializeObject<QueryMerchantWhereModel>(value);
            }
            get
            {
                return _queryMerchantWhere;
            }
        }
        /// <summary>
        /// 是否要上传存款证名
        /// 0：无，1：需要
        /// </summary>
        public int isNeedUploadPayment { set; get; } = 0;

        #region 人工存款通道
        /// <summary>
        /// 0:上线，1：所有开启状态（方便测试渠道）
        /// </summary>
        public int online { set; get; } = 0;
        /// <summary>
        /// 当前通道是否要联系客服处理
        /// 1：是，0：否
        /// </summary>
        public int isNeedService { set; get; } = 0;
        #endregion

        #region 商户访问级别要求
        /// <summary>
        /// 高级别商户，要使用低级别商户多少次才给开放
        /// </summary>
        public int merchantLevel { set; get; }
        /// <summary>
        /// 低级别商户列表
        /// </summary>
        public string merchantids { set; get; }

        /// <summary>
        /// 当前会员是否有权限使用高级商户
        /// 0：无， 1：有
        /// </summary>
        public int allowedUseMerchantLevel { set; get; } = 0;
        #endregion

        /// <summary>
        /// 商户的收款币种
        /// </summary>
        public int currencyType { set; get; }
        /// <summary>
        /// 钱包类型
        /// </summary>
        public int t_walletType { set; get; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string t_merchantAccounts { set; get; }
    }


    public class PaySettingOutModel
    {
        /// <summary>
        /// 支付类型名
        /// </summary>
        public string dvalue { set; get; }
        /// <summary>
        /// 提款分类id
        /// </summary>
        public long dkey { set; get; }
        /// <summary>
        /// 支付类型（推荐logo）
        /// </summary>
        public string icon { set; get; }
        /// <summary>
        /// 支付类型logo
        /// </summary>
        public string navilogo { set; get; }
        /// <summary>
        /// 推荐使用,是否是推荐的支付类型 （0：否，1：是）
        /// </summary>
        public int recommend { set; get; }
        /// <summary>
        /// 支付类型描述
        /// </summary>
        public string title { set; get; }
        /// <summary>
        /// 选择列表title
        /// </summary>
        public string selectTitle { set; get; }
        /// <summary>
        /// 商户信息
        /// </summary>
        public List<merchant> merchants { set; get; }
        /// <summary>
        /// 优惠列表
        /// </summary>
        public List<PayPromo> promos { set; get; }
        /// <summary>
        /// 已经绑定的银行卡，提款时使用
        /// </summary>
        public List<BankCard> banks { set; get; }
        /// <summary>
        /// 存提页面注意事项
        /// </summary>
        public TipsModel tipsModel { set; get; }
        /// <summary>
        /// 提款页金额限制
        /// </summary>
        public OfUserGroupModel withdrawAmountRange { set; get; }
        /// <summary>
        /// 分类绑卡时使用：0：银行卡，1：虚拟币
        /// </summary>
        public int category { set; get; }
        /// <summary>
        /// 申请优惠存款金额和总存款金额，
        /// 二者比例和百分比
        /// </summary>
        public DepositPromoLimitModel depositPromoLimitModel { set; get; }
    }


    public class OfUserGroupModel
    {

        /// <summary>
        /// 每日最高提款累积
        /// </summary>
        public decimal maxwithdrawperday { set; get; } = 0;
        /// <summary>
        /// 每日单卡最高提款累积
        /// </summary>
        public decimal maxwithdrawperdaypercard { set; get; } = 0;

        /// <summary>
        /// 单笔最低下限
        /// </summary>
        public decimal minAmountPerOrder { set; get; } = 100;
        /// <summary>
        /// 单笔最高上限
        /// </summary>
        public decimal maxAmountPerOrder { set; get; } = 100000;


    }


    public class TipsModel
    {
        public int cardNumbers { set; get; }
        public string withdraw { set; get; }
        public string service { set; get; }
        /// <summary>
        /// 存款时的注意事项
        /// </summary>
        public List<string> warning { set; get; }
        public List<TipsLink> tipsLinks { set; get; }
    }

    /// <summary>
    /// 存款页面通道提示信息，如果有跳转到新页面提示信息
    /// </summary>
    public class TipsLink
    {
        public string title { set; get; }
        public string url { set; get; }
        /// <summary>
        /// 0:弹框， 1：跳转到新页面   默认为1
        /// </summary>
        public int linkModel { set; get; } = 1;
        /// <summary>
        /// 当前分类名：  存款指南   或  推荐交易所
        /// </summary>
        public string typeDesc { set; get; }
        /// <summary>
        /// 支付链接的icon
        /// </summary>
        public string iconUrl { set; get; } = "";
    }


    public class BankCard
    {
        public string name { set; get; }
        public string cardno { set; get; }
        public string qr_url { set; get; } = "";
        /// <summary>
        /// 卡号加密
        /// </summary>
        public string cardsec { set; get; }
        public string address { set; get; }
        public string icon { set; get; }
        public int withdrawType { set; get; }
        /// <summary>
        /// 人名
        /// </summary>
        public string cardName { set; get; } = "";

        /// <summary>
        /// 单笔最低限额
        /// </summary>
        public decimal min { set; get; }
        /// <summary>
        /// 单笔最高限额
        /// </summary>
        public decimal max { set; get; }

    }


    public class PayPromo
    {
        /// <summary>
        /// 优惠标识
        /// </summary>
        public int s_value { set; get; }
        /// <summary>
        /// 优惠描述
        /// </summary>
        public string s_text { set; get; }
        /// <summary>
        /// 0: 线上优惠， 1：线下优惠
        /// </summary>
        public int offline { set; get; } = 0;

    }


    public class BankListModel
    {

        public string id { set; get; }
        public string name { set; get; }
        public string url { set; get; }
        public int category { set; get; }


        //  {
        //  "id": "37",
        //  "name": "郑州银行",
        //  "url": "https://www.cdnimg9.com/img/pc/bank/bk_37.png",
        //  "category": 0
        //},
    }

    /// <summary>
    /// 提款快捷金额
    /// </summary>
    public class FastPayModel
    {
        /// <summary>
        /// 快捷金额 豆号分割 如：  300,500,700
        /// </summary>
        public string fastpay { set; get; } = "";
        /// <summary>
        /// 0:金额充足不需要快捷金额，1：需要快捷金额，小额提款
        /// </summary>
        public int status { set; get; } = 0;
        public int min { set; get; }
        public int max { set; get; }
    }

    /// <summary>
    /// 商户基本数据
    /// </summary>
    public class merchant
    {
        /// <summary>
        /// 渠道id
        /// </summary>
        public long id { set; get; }
        /// <summary>
        /// 渠道名
        /// </summary>
        public string name { set; get; }
        /// <summary>
        /// 最低单笔金额
        /// </summary>
        public decimal min { set; get; }
        /// <summary>
        /// 最高单笔金额
        /// </summary>
        public decimal max { set; get; }
        public string method { set; get; }
        /// <summary>
        /// 银行列表，如果不 为空，就需要选择银行列表的显示
        /// </summary>
        public List<BankModel> banklist { set; get; }
        /// <summary>
        /// 需要选择银行列表（0：否，1：是）
        /// </summary>
        public int requirebank { set; get; }
        /// <summary>
        /// 需要填写卡号（0：否，1：是）
        /// </summary>
        public int requirecardno { set; get; }
        /// <summary>
        /// 需要填写用户名（0：否，1：是）
        /// </summary>
        public int requirename { set; get; }
        /// <summary>
        /// 需要填写备注（0：否，1：是）
        /// </summary>
        public int requirenote { set; get; }
        /// <summary>
        /// 点卡，选择面值金额
        /// </summary>
        public int allowedAmountInput { set; get; }
        /// <summary>
        /// 手续费计算百分比
        /// </summary>
        public decimal feecent { set; get; }
        /// <summary>
        /// 是否需要提示手续费
        /// </summary>
        public int deductfee { set; get; }

        public string fastpay { set; get; }
        /// <summary>
        /// 存提页面注意事项
        /// </summary>
        public TipsModel tipsModel { set; get; }
        /// <summary>
        /// 是否要上传存款证名
        /// 0：无，1：需要
        /// </summary>
        public int isNeedUploadPayment { set; get; } = 0;
        /// <summary>
        /// 当前通道是否要联系客服处理
        /// 1：是，0：否
        /// </summary>
        public int isNeedService { set; get; } = 0;
        /// <summary>
        /// 是否要拉取动固定金额，定额存款优惠 默认为0, 1:是，0否
        /// </summary>
        public int isNeedPullFixAmount { set; get; } = 0;// 
        /// <summary>
        /// 渠道返现的百分比 
        /// </summary>
        public decimal rebate { set; get; } = 0;
        /// <summary>
        /// 通道打开要使用浏览器 跳出app 
        /// 0：否，1：是
        /// </summary>
        public int appnewwindow { set; get; } = 0;

        /// <summary>
        /// 当前渠道分类id  :
        /// 1 银行卡,9	极速网银
        /// </summary>
        public int paycategory { set; get; }

    }

    /// <summary>
    /// 银行列表
    /// </summary>
    public class BankModel
    {
        /// <summary>
        /// 银行id
        /// </summary>
        public string id { set; get; }
        /// <summary>
        /// 银行名
        /// </summary>
        public string name { set; get; }
        /// <summary>
        /// 银行logo
        /// </summary>
        public string icon { set; get; }
        public string url { set; get; }
        public int category { set; get; }
        public int withdrawType { set; get; }
        public int status { set; get; }
    }



    public class AmountRange
    {
        public decimal min { set; get; }
        public decimal max { set; get; }
    }


    public class OfUserGroupExtentionModel : OfUserGroupModel
    {
        public AmountRange rmbAmountRange { set; get; }
        public AmountRange inrAmountRange { set; get; }
        public AmountRange usdtERCAmountRange { set; get; }
        public AmountRange usdtTRCAmountRange { set; get; }
        public AmountRange cgpAmountRange { set; get; }
        public AmountRange gpAmountRange { set; get; }
        public AmountRange gopAmountRange { set; get; }
        public AmountRange ebAmountRange { set; get; }

        public AmountRange alipayAmountRange { set; get; }

        public AmountRange kdouAmountRange { set; get; }
        public AmountRange wePayAmountRange { set; get; }
    }

    public class WithdrawRespnoseModel
    {
        /// <summary>
        /// 银行卡列表,绑卡使用
        /// </summary>
        public List<BankModel> banklist { set; get; }
        /// <summary>
        /// 提款页面Ui 配置信息
        /// </summary>
        public List<PaySettingOutModel> withdrawBankInfo { set; get; }

        public List<OTCXRate> rates { set; get; }
        /// <summary>
        /// 是否要用快捷提款金额（小额提款时使用）
        /// </summary>
        public FastPayModel fastPay { set; get; }

        /// <summary>
        /// 是否要用快捷提款金额(支付宝定额提款小额) 
        /// </summary>
        public FastPayModel aliFastPay { set; get; }
    }

    public class OTCXRateSetting
    {
        /// <summary>
        /// 币种类别
        /// </summary>
        public int currencyType { set; get; } = 156;
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal usdt_xrate { set; get; }
        /// <summary>
        /// 浮动汇率
        /// </summary>
        public decimal usdt_xrate_withdraw_topup { set; get; }
        /// <summary>
        /// 汇率兑换关系 1: 当前金额乘rate, 0: 当前金额除rate 即可获取到大概的转换金额
        /// </summary>
        public int exchange { set; get; }

    }

    /// <summary>
    /// 娄字货币的汇率
    /// </summary>
    public class OTCXRate
    {
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal rate { set; get; }
        /// <summary>
        /// 提款类型 2：usdt, 3:btc  将作废
        /// </summary>
        public int withdrawType { set; get; }

        /// <summary>
        /// 提款类型 0: rmb, 1：usdt, 2:btc, 3:inr , 4: 4 :CGP
        /// </summary>
        public int category { set; get; }

        /// <summary>
        /// 汇率兑换关系 
        /// 1: 当前金额乘rate, 
        /// 0: 当前金额除rate 即可获取到大概的转换金额
        /// </summary>
        public int exchange { set; get; }

        /// <summary>
        /// 币种类别
        /// </summary>
        public int currencyType { set; get; } = 156;
        /// <summary>
        /// category 类别对应的提款手续费用
        /// </summary>
        public decimal fee { set; get; } = 0;
    }


}
