using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.Bonus
{
    public class QueryBonusOutModel
    {
        public long tid { set; get; }
        public string account { set; get; }
        public string affaccount { set; get; }

        public decimal Profit { set; get; }


        public string remarks { set; get; }
        public decimal amount { set; get; }
        public decimal reward { set; get; }
        public decimal bet { set; get; }
        public int level { set; get; }
        public string levels { set; get; }
        public int gametype { set; get; }
        public string createtime { set; get; }
        public string expiredTime { set; get; }

        public string statusDesc { set; get; }
        private int _status;
        public int status
        {
            set
            {
                statusDesc = value == 0 ? "等待审核" : value == 1 ? "审核通过" : "取消审核";
                _status = value;
            }
            get
            {
                return _status;
            }
        }
        public int anomaly { set; get; }
        public int riskTypesAready { set; get; }
        public string riskRemark { set; get; }
        public string referenceguid { set; get; }
        public string createperson { set; get; }
        public string reviewperson { set; get; }
        public string ipaddress { set; get; }
        public string reason { set; get; }
        public string reviewtime { set; get; }

        public string bonusPromoTypeDesc { set; get; }
        private int _bonusPromoType;
        public int bonusPromoType
        {
            set
            {
                bonusPromoTypeDesc = value == 0 ? "红利" : "优惠";
                _bonusPromoType = value;
            }
            get { return _bonusPromoType; }
        }
        public string bonusPromoCodeDesc { set; get; }
        private int _bonusPromoCode;
        public int? bonusPromoCode
        {
            set
            {
                try
                {
                    bonusPromoCodeDesc = (value == 0 || value == null) ? "" : BonusPromoDictionary.bonusPromoCodeName.ContainsKey(value.ToString()) ? BonusPromoDictionary.bonusPromoCodeName[value.ToString()] : "";
                    _bonusPromoCode = value == null ? 0 : Convert.ToInt32(value);
                }
                catch (Exception)
                {
                    _bonusPromoCode = 0 ;
                    bonusPromoCodeDesc = "";
                }
                
            }
            get
            {
                return _bonusPromoCode;
            }
        }
        /// <summary>
        /// 代理id
        /// </summary>
        public long aid { set; get; }
        /// <summary>
        /// 会员id
        /// </summary>
        public long uid { set; get; }
        /// <summary>
        /// 会员的品牌
        /// </summary>
        public string merchantAccount { set; get; } = "-1";
        public int walletType { set; get; } = -1;
        public int currencyType { set; get; } = -1;
    }

    public class QueryPGPromoOutModel
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string account { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { set; get; }
        /// <summary>
        /// 充值金额
        /// </summary>
        public decimal amount { set; get; }
        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal reward { set; get; }
        /// <summary>
        /// 倍数
        /// </summary>
        public decimal bet { set; get; }
        private string _createtime;
        /// <summary>
        /// 申请时间
        /// </summary>
        public string createtime
        {
            set {  _createtime = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss"); }
            get { return _createtime; }
        }
        
        public string statusDesc { set; get; }
        private int _status;
        public int status
        {
            set
            {
                statusDesc = value == 0 ? "处理中" : value == 1 ? "完成" : "失败";
                _status = value;
            }
            get
            {
                return _status;
            }
        }
        private string _reviewtime;
        /// <summary>
        /// 处理时间
        /// </summary>
        public string reviewtime
        {
            set {
                if(!string.IsNullOrEmpty(value))
                    _reviewtime = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss"); 
            }
            get { return _reviewtime; }
        }

      
    }



    /// <summary>
    /// cms 福利中心数据
    /// </summary>
    public class QuerySelfBonusOutModel
    {
        public long tid { set; get; }
        public string account { set; get; }
        public string affaccount { set; get; }

        public decimal Profit { set; get; }


        public string remarks { set; get; }
        public decimal amount { set; get; }
        public decimal reward { set; get; }
        public decimal bet { set; get; }
        public int level { set; get; }
        public string levels { set; get; }
        public int gametype { set; get; }
        private string _createtime;
        public string createtime
        {
            set
            {
                _createtime = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            get
            {
                return _createtime;
            }
        }
        private string _expiredTime;
        public string expiredTime {
            set
            {
                _expiredTime = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            get
            {
                return _expiredTime;
            }
        }

        public string statusDesc { set; get; }
        private int _status;
        public int status
        {
            set
            {
                statusDesc = value == 0 ? "未领取" : value == 1 ? "已领取" : value == 2 ? "过期" : value == 3 ? "被取消": "等待审核";
                _status = value;
            }
            get
            {
                return _status;
            }
        }
        public int anomaly { set; get; }
        public int riskTypesAready { set; get; }
        public string riskRemark { set; get; }
        public string referenceguid { set; get; }
        public string createperson { set; get; }
        public string reviewperson { set; get; }
        public string ipaddress { set; get; }
        public string reason { set; get; }
        public string reviewtime { set; get; }

        public string bonusPromoTypeDesc { set; get; }
        private int _bonusPromoType;
        public int bonusPromoType
        {
            set
            {
                bonusPromoTypeDesc = value == 0 ? "红利" : "优惠";
                _bonusPromoType = value;
            }
            get { return _bonusPromoType; }
        }
        public string bonusPromoCodeDesc { set; get; }
        private int _bonusPromoCode;
        public int? bonusPromoCode
        {
            set
            {
                try
                {
                    bonusPromoCodeDesc = (value == 0 || value == null) ? "" : BonusPromoDictionary.bonusPromoCodeName.ContainsKey(value.ToString()) ? BonusPromoDictionary.bonusPromoCodeName[value.ToString()] : "";
                    _bonusPromoCode = value == null ? 0 : Convert.ToInt32(value);
                }
                catch (Exception)
                {
                    _bonusPromoCode = 0;
                    bonusPromoCodeDesc = "";
                }

            }
            get
            {
                return _bonusPromoCode;
            }
        }
        /// <summary>
        /// 代理id
        /// </summary>
        public long aid { set; get; }
        public long uid { set; get; }
        /// <summary>
        /// 会员的品牌
        /// </summary>
        public string merchantAccount { set; get; } = "-1";
        public int walletType { set; get; } = -1;
        public int currencyType { set; get; } = -1;
    }

    public class SelfBonusSelectModel
    {
        public int value { set; get; }
        public string text { set; get; }
    }
    public class ExportBonusOutModel
    {
       

        public string account { set; get; }
        public string affaccount { set; get; }
        public string parentAccount { set; get; }

        public string remarks { set; get; }
        public decimal amount { set; get; }
        public decimal reward { set; get; }
        public decimal bet { set; get; }
        public int level { set; get; }
        public string levels { set; get; }

        public string gamename { set; get; }
        public int gametype { set; get; }
        public string createtime { set; get; }

        public string statusDesc { set; get; }
        private int _status;
        public int status
        {
            set
            {
                statusDesc = value == 0 ? "等待审核" : value == 1 ? "审核通过" : "取消审核";
                _status = value;
            }
            get
            {
                return _status;
            }
        }

        public string referenceguid { set; get; }
        public string createperson { set; get; }
        public string reviewperson { set; get; }
        public string ipaddress { set; get; }
        public string reason { set; get; }
        public string reviewtime { set; get; }

        public string bonusPromoTypeDesc { set; get; }
        private int _bonusPromoType;
        public int bonusPromoType
        {
            set
            {
                bonusPromoTypeDesc = value == 0 ? "红利" : "优惠";
                _bonusPromoType = value;
            }
            get { return _bonusPromoType; }
        }
    }

    public class BonusPromoDictionary
    {
        public static Dictionary<string, string> bonusPromoCodeName = new Dictionary<string, string>()
        {
            { "1000", "首存优惠" },
                    { "1001", "体育线下首存优惠" },
                    { "1002", "体育线下日存优惠" },
                    { "1003", "彩票线下首存优惠" },
                    { "1004", "彩票线下日存优惠" },
                    { "1005", "真人线下日存优惠" },
                    { "1006", "推荐优惠" }
        };
    }

    public class BonusReport
    {
        /// <summary>
        /// 优惠或红利赠送的总金额
        /// </summary>
        public decimal totalReward { set; get; }
       
    }
}
