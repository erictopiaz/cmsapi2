using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Pay
{
    /// <summary>
    /// 渠道的配置参数misc
    /// </summary>
    public class ThirdPayMiscModel
    {
        public int mindepositcount { set; get; }
        public int requirebank { set; get; }
        public int requirenote { set; get; }
        /// <summary>
        /// 强制实名 必 注册名和申请充值名一致
        /// </summary>
        public int requirename { set; get; } = 0;
        /// <summary>
        /// 需要填写用户名（0：否，1：是）  配合会员详情里面去处理
        /// </summary>
        public int registeredname { set; get; }

        public int requirecardno { set; get; }
        /// <summary>
        /// 代理承担的手续费百分比
        /// </summary>
        public decimal feecent { set; get; } = 0;
        /// <summary>
        /// 会员要承担的手续费百分比
        /// </summary>
        public decimal self_feecent { set; get; } = 0;
        /// <summary>
        /// 会员是否要承担手续费 0：否，1：是
        /// </summary>
        public int deductfee { set; get; } = 0;
        /// <summary>
        /// 存款了多少次，后就会收取手续费用
        /// </summary>
        public int predeductcount { set; get; } = 0;


        /// <summary>
        /// 渠道可外层使用（目前只针对“万象城会员”且有一次存款成功的）可用
        /// 0：否，1：是
        /// </summary>
        public int newusers { set; get; }
        /// <summary>
        /// 点卡，选择面值金额
        /// 0:不让输入，1：可以输入
        /// </summary>
        public int allowedAmountInput { set; get; } = 1;

        /// <summary>
        /// 渠道返现比例
        /// </summary>
        public decimal rebate { set; get; } = 0;
        /// <summary>
        /// 通道打开要使用浏览器 跳出app 
        /// 0：否，1：是
        /// </summary>
        public int appnewwindow { set; get; } = 0;
        /// <summary>
        /// 下单过去的proxy
        /// </summary>
        public string proxy { set; get; }
        /// <summary>
        /// 支付UI交互方式 （URL,CRYPTO,BANK）
        /// </summary>
        public string method { set; get; }

    }
}
