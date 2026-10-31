using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib;

namespace ShareEntity.Agent.Finance
{
    /// <summary>
    /// 20250205  bti
    /// </summary>
    public class playersReportFinanceOutModel: FinanceReportWithoutGameBase
    {
        private string _account;
        /// <summary>
        /// 会员账号
        /// </summary>
        public string account
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _account = Common.HideSensitiveInfo(value);
                }
                else
                {
                    _account = "";
                }

            }
            get { return _account; }
        }
        public string affaccount { set; get; }
        public long financeDay { set; get; }
        /// <summary>
        /// 投注
        /// </summary>
        public decimal bets { set; get; }
        /// <summary>
        /// 输赢
        /// </summary>
        public decimal winlost { set; get; }
        /// <summary>
        /// vip 名称
        /// </summary>
        public string levels { set; get; }
        public int level { set; get; }
        public int anomaly { set; get; }
        public int riskTypesAready { set; get; }

        /// <summary>
        /// 会员id
        /// </summary>
        public long uid { set; get; } = 0;
        /// <summary>
        /// 会员的品牌
        /// </summary>
        public string merchantAccount { set; get; } = "-1";
        public int walletType { set; get; } = -1;

        private string _createtime;
        /// <summary>
        /// 注册时间
        /// </summary>
        public string createtime
        {
            set
            {
                _createtime = string.IsNullOrEmpty(value) ? "" : Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            get { return _createtime; }
        }
        /// <summary>
        /// 最后登录时间，暂无
        /// </summary>
        public string lastLoginTime { set; get; } = "";
        /// <summary>
        /// 默认国籍
        /// </summary>
        public string citizenship { set; get; } = "中国";

    }

}
