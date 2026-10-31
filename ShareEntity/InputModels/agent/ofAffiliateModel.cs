using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.agent
{
    /// <summary>
    /// 代理表结构
    /// </summary>
    public class ofAffiliateModel : RegisterAffiliateModel
    {

        public string country { get; set; }

        public string adlink { get; set; }

        public string cardno { get; set; }

        public string cardname { get; set; }

        public string cardaddress { get; set; }


        public string remarks { get; set; }

        public int status { get; set; }

        public int verify { get; set; }

        public decimal alloc { get; set; }

        public decimal? balance { get; set; }
        public int? adlinkhit { get; set; }

        public int? adcodelock { get; set; }
        public string commission { get; set; }
    }


    public class AffModel
    {
        public string account { set; get; }
        public int agentLevel { set; get; }
        public string parentAccount { set; get; }
        public decimal unSettleAmount { set; get; }
        public decimal subTotalAmount { set; get; }
        /// <summary>
        /// 人头奖励
        /// </summary>
        public decimal playerBonus { set; get; }
        /// <summary>
        /// 保底
        /// </summary>
        public decimal salaryBonus { set; get; }
        /// <summary>
        /// 节日奖励
        /// </summary>
        public decimal festivalBonus { set; get; }
        /// <summary>
        /// 是否为多账号
        /// 0：否，1：是
        /// </summary>
        public int isMultiAccount { set; get; }
        public decimal balance { set; get; }
        /// <summary>
        /// 结算备注
        /// </summary>
        public string remark { set; get; }

        /// <summary>
        /// 合营注册时间
        /// </summary>
        public string createtime { set; get; }
    }
}
