using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.Referrals {
    public class ReferralPlayerModel {
        /// <summary>
        /// 会员账号
        /// </summary>
        public string account { set; get; }

        private string _verifyTime;
        /// <summary>
        /// 审核结算时间
        /// </summary>
        public string verifyTime {
            set {
                if (!string.IsNullOrEmpty(value))
                    _verifyTime = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                else {
                    _verifyTime = "";
                }
            }
            get { return _verifyTime; }
        }
        private string _createtime;
        /// <summary>
        /// 会员创建时间
        /// </summary>
        public string createtime {
            set {  
                if (!string.IsNullOrEmpty(value))
                    _createtime = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                else {
                    _createtime = "";
                }
            }
            get { return _createtime; }
        }
        public long financeGameDay { set; get; }
        /// <summary>
        /// 会员总存款金额 
        /// </summary>
        public string totalDepositAmount { set; get; }
        /// <summary>
        /// 会员结算周存款金额
        /// </summary>
        public string depositAmount { set; get; }
        /// <summary>
        /// 结算周充值人数
        /// </summary>
        public int activeNumbers { set; get; }
        /// <summary>
        /// 结算周推荐人数
        /// </summary>
        public int referrals { set; get; }
        /// <summary>
        /// 会员投注
        /// </summary>
        public string waggered { set; get; }
        private decimal _rate;
        public decimal rate {
            set {
                _rate = value;
                rateDes = string.Format(@"{0}%", (value * 100).ToString("F2"));
            }
            get { return _rate; }
        }
        /// <summary>
        /// 结算比
        /// </summary>
        public string rateDes { set; get; }
        /// <summary>
        /// 结算佣金
        /// </summary>
        public string commission { set; get; }

        public string selfWaggered { set; get; }
        private decimal _selfRate;
        public decimal selfRate {
            set {
                _selfRate = value;
                selfRateDes = string.Format(@"{0}%", (value * 100).ToString("F2"));
            }
            get { return _selfRate; }
        }
        /// <summary>
        /// 结算比
        /// </summary>
        public string selfRateDes { set; get; }
        public string selfCommission { set; get; }

        /// <summary>
        /// 会员的品牌
        /// </summary>
        public string merchantAccount { set; get; } = "-1";
        public int walletType { set; get; } = -1;

    }
    //t.financeGameDay,u.createtime,u.depositamount totalDepositAmount,t.depositAmount,t.referrals,t.commission,t.activeNumbers, t.rate, t.status, t.create_at verifyTime
}
