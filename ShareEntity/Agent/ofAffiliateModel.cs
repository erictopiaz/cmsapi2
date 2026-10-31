using ShareEntity.InputModels.agent;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.Agent
{
    public class ofAffiliateModel : ofAffiliateLoginOutModel
    {
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 代理是否可以给会员渠道充值，0：否，1：有
        /// </summary>
        public int depositAllowed { set; get; }
        /// <summary>
        /// 代理是否有划款权限，0：无，1：有
        /// </summary>
        public int transferAllowed { set; get; }

        /// <summary>
        /// 优惠结算月
        /// </summary>
        public int billPromoMonth { set; get; }
        /// <summary>
        /// 优惠比例
        /// </summary>
        public string billPromoPDesc { set; get; }
        private decimal _billPromoP;
        public decimal billPromoP
        {
            set
            {
                _billPromoP = value;
                billPromoPDesc = value == 0 ? "--" : string.Format(@"{0}%", (value * 100).ToString("0"));
            }
            get { return _billPromoP; }
        }
    }






    public class ofAffiliateLoginOutModel
    {
        /// <summary>
        /// 币种
        /// </summary>
        public string currency { get; set; }
        /// <summary>
        /// 代理账号
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        //public string password { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string realname { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int? sex { get; set; }
        /// <summary>
        /// 是否被启用
        /// 0：示启用，1： 启用
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 是否被审核
        /// 0：示审核，1： 已审核
        /// </summary>
        public int verify { get; set; }

        public string birthday { get; set; }

        public string country { get; set; }

        /// <summary>
        /// 注册网址
        /// </summary>
        public string website { get; set; }
        /// <summary>
        /// 推广域名
        /// 目前以空格分割多个
        /// </summary>
        public string adlink { get; set; }
        /// <summary>
        /// 代理推广代码
        /// </summary>
        public string adcode { get; set; }

        /// <summary>
        /// 结算占成
        /// </summary>
        public decimal alloc { get; set; }

        public decimal balance { get; set; }
        /// <summary>
        /// 本月结算金额
        /// </summary>
        private decimal _curBilledAmount;
        public decimal curBilledAmount
        {
            set
            {
                _curBilledAmount = value;// Math.Round(value / 100, 4);
            }
            get
            {
                return _curBilledAmount;
            }
        }
        /// <summary>
        /// 当前代理可使用的金额
        /// </summary>
        public decimal activeAmount { set; get; }
        /// <summary>
        /// 总佣金，可能为负
        /// </summary>
        public decimal totalAmount { set; get; }
        public decimal subTotalAmount { set; get; }
        /// <summary>
        /// 历史金额，未结算金额累积
        /// </summary>
        private decimal _unSettleAmount;
        public decimal unSettleAmount
        {
            set
            {
                _unSettleAmount = value;// Math.Round(value / 100, 4);
            }
            get
            {
                return _unSettleAmount;
            }
        }

        public decimal? unavailable { get; set; }

        /// <summary>
        /// 注册ip
        /// </summary>
        public string ipaddress { get; set; }
        /// <summary>
        /// 注册地址
        /// </summary>
        public string macaddress { get; set; }
        /// <summary>
        /// 上次登 录ip
        /// </summary>
        public string lastipaddress { get; set; }
        /// <summary>
        /// 上次登 录地址
        /// </summary>
        public string lastmacaddress { get; set; }

        public string commission { get; set; }
        public string registerUrl { set; get; }
        private BillPModel _billPS;
        /// <summary>
        /// 结算占成 
        /// </summary>
        public object billPS
        {
            set
            {
                try
                {
                    // _billPS = new BillPModel();//统一默认
                    _billPS = Newtonsoft.Json.JsonConvert.DeserializeObject<BillPModel>(value.ToString());
                }
                catch (Exception)
                {
                    _billPS = new BillPModel();//统一默认
                }

            }
            get
            {
                return _billPS;
            }
        }

        public List<AgentLevel> levels { set; get; }
        public int agentLevel { set; get; }
        /// <summary>
        /// 是否为多账号
        /// 1： 是，0：否
        /// </summary>
        public int isMultiAccount { set; get; }

        public int currencyType { set; get; }
        public string a_merchantAccount { set; get; }
        public string accessLang { set; get; }
        public int lang { set; get; }
    }

    public class BillPModel
    {
        public decimal sp { set; get; }
        public decimal live { set; get; }
        public decimal es { set; get; }
        public decimal slot { set; get; }
        public decimal lottey { set; get; }
        public decimal all { set; get; }
    }
    public class AgentLevel
    {
        public string agentAccount { set; get; }
        public string agentLevel { set; get; }
        public string parentAccount { set; get; } = "";
    }
}
