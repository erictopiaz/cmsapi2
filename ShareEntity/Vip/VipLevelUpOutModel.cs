using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.Vip
{
    /// <summary>
    /// vip 升级数据查询输出
    /// </summary>
    public class VipLevelUpOutModel
    {
        public long id { set; get; }
        public long uid { set; get; }
        public int month { set; get; }
        public string account { set; get; }
        public string affaccount { set; get; }
        public decimal totalDepositAmount { set; get; }
        public decimal totalWithdrawAmount { set; get; }
        public decimal totalBet { set; get; }
        public decimal lowStake { set; get; }
        public string levelName { set; get; }
        public string nextLevelName { set; get; }
        public string isUpDesc { set; get; }
        public int riskTypesAready { set; get; }
        /// <summary>
        /// 0：降级， 1：升级，2：保级
        /// </summary>
        private int _is_up;
        public int is_up {
            set
            {
                isUpDesc = value == 0 ? "降级" : value == 1 ? "升级" : value == 2 ? "保级" : "未知";
                _is_up = value;
            }
            get
            {
                return _is_up;
            }
        }
        public string statusDesc { set; get; }
        /// <summary>
        /// 审核状态
        /// </summary>
        private int _status;
        public int status {
            set
            {
                statusDesc = value == 0 ? "待审" : value == 1 ? "已审" : value == 2 ? "取消" : "未知";
                _status = value;
            }
            get
            {
                return _status;
            }
        }
        public string create_who { set; get; }
        public string verify_who { set; get; }
        public string remarks { set; get; }
        public string freezereason { set; get; }
        /// <summary>
        /// 被风控账号
        /// 1：已经被风控
        /// </summary>
        public int anomally { set; get; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public string verify_at { set; get; }
    }

    /// <summary>
    /// 各vip 每日存款次数
    /// </summary>
    public class VipDepositTimesDailyOutputModel
    {
        public long financeGameDay { set; get; }
        public int vip10001 { set; get; }
        public int vip10012 { set; get; }
        public int vip10007 { set; get; }
        public int vip10027 { set; get; }
        public int vip10028 { set; get; }
        public int vip10002 { set; get; }
        public int vip10003 { set; get; }
        public int vip10004 { set; get; }
        public int vip10025 { set; get; }
        public int vip10005 { set; get; }
        public string create_at { set; get; }
        public int totalTimes { set; get; }
        public int loginTimes { set; get; }

        #region 2022 12-21 新增

        /// <summary>
        /// 当天注册量
        /// </summary>
        public int regNumbersDaily { set; get; } = 0;
        /// <summary>
        /// pc 当天设备使会员数
        /// </summary>
        public int pc_used_Daily { set; get; } = 0;
        public int h5_used_Daily { set; get; } = 0;
        public int ios_used_Daily { set; get; } = 0;
        public int android_used_Daily { set; get; } = 0;

        /// <summary>
        /// pc当天新的会员使用数
        /// </summary>
        public int pc_add { set; get; } = 0;
        public int h5_add { set; get; } = 0;
        public int ios_add { set; get; } = 0;
        public int android_add { set; get; } = 0;

        #endregion
        /// <summary>
        /// 品牌
        /// </summary>
        public string merchantAccount { set; get; }
    }
}
