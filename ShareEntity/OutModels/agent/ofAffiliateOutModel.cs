using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.agent
{

    public class ofAffiliateOutWithPasswordModel : ofAffiliateOutModel
    {
        public string password { get; set; }
        public long id { get; set; }
        public int currencyType { get; set; }
        public string a_merchantAccount { set; get; }
    }
    public partial class ofAffiliateOutModel
    {
        public string currency { get; set; }

        public string account { get; set; }
        public string parentAccount { set; get; }
        public string parentAdcode { set; get; }

        public string bbaccount { get; set; }

        public string realname { get; set; }

        public int? sex { get; set; }

        public string birthday { get; set; }

        public string country { get; set; }

        public string email { get; set; }

        public string telephone { get; set; }

        public string qq { get; set; }

        public string website { get; set; }

        public Dictionary<string, string> adlinks { set; get; }

        public string adlink { get; set; }

        public string adcode { get; set; }

        public string cardno { get; set; }
        public string cardname { get; set; }

        public string cardaddress { get; set; }

        public string ew_uuid { get; set; }

        public string pt_uuid { get; set; }

        public string sun_uuid { get; set; }

        public string remarks { get; set; }

        public int status { get; set; }

        public int verify { get; set; }

        public DateTime createtime { get; set; }

        public string createperson { get; set; }

        public decimal alloc { get; set; } = 0;

        public string verifyperson { get; set; }

        public DateTime? verifytime { get; set; }

        public decimal? balance { get; set; }
        public decimal? subTotalAmount { set; get; }
        public decimal? unSettleAmount { set; get; }
        public decimal? playerBonus { set; get; }
        public decimal? salaryBonus { set; get; }
        public decimal? festivalBonus { set; get; }

        public decimal? unavailable { get; set; }

        public string ipaddress { get; set; }

        public string macaddress { get; set; }

        public string lastipaddress { get; set; }

        public string lastmacaddress { get; set; }

        public int? adlinkhit { get; set; }

        public int? adcodelock { get; set; }

        public string commission { get; set; }

        public string upperlevel { get; set; }
        public string registerUrl { set; get; }
        
        public int agentId { set; get; }
        public billPModel pObject { set; get; }
        public string billP { set; get; }
        public int agentLevel { set; get; }
        public int isLeaf { set; get; }
        public int allowCreateAgent { set; get; }
        public string platformFeeP { set; get; }
        /// <summary>
        /// 修改账号信息标识
        /// </summary>
        public int mbr { set; get; }
        /// <summary>
        /// 修改代理设置标识
        /// </summary>
        public int msr { set; get; }
        /// <summary>
        /// 当前代理推广链接可编辑状态
        /// </summary>
        public int ad { set; get; }
        /// <summary>
        /// 多账号代理
        /// 0：否，1：是
        /// </summary>
        public int isMultiAccount { set; get; }

        #region 代理禁用商户ids
        public string disabledMids { set; get; }
        public List<int> mids { set; get; } = new List<int>();
        public List<object> disabledMidsKV { set; get; } = new List<object>();
        #endregion
        public int depositAllowed { set; get; }
        public int transferAllowed { set; get; }
        /// <summary>
        /// 市场负责人列表
        /// </summary>
        public List<object> agentMarkets { set; get; } = new List<object>() { new { value = "no", text = "无" } };
        /// <summary>
        /// 默认没有分配负责人
        /// </summary>
        public string agentMarket { set; get; } = "no";
        /// <summary>
        /// 结算优惠月
        /// </summary>
        public int billPromoMonth { set; get; } = 0;

        public int riskTypeAready { set; get; } = 0;
        public List<AffRiskModel> riskModels { set; get; } = new List<AffRiskModel>();
        /// <summary>
        /// 风控备注
        /// </summary>
        public string riskRemark { set; get; } = "";

    }


    /// <summary>
    /// 风控描述
    /// </summary>
    public class AffRiskModel
    {
        public int id { set; get; }
        public string label { set; get; }
    }

    public class billPModel
    {
        public decimal sp { set; get; } = 0;
        public decimal live { set; get; } = 0;
        public decimal es { set; get; } = 0;
        public decimal slot { set; get; } = 0;
        public decimal lottey { set; get; } = 0;
        public decimal all { set; get; } = 0;

        public decimal betBonusAllowed { set; get; } = 0;
        public decimal betBonusP { set; get; } = 0;
        public decimal betBonusMax { set; get; } = 0;
    }

    /// <summary>
    /// 代理登录返回数据
    /// </summary>
    //public class ofAffiliateOutModel
    //{
    //    public string account { get; set; }
    //    public string password { get; set; }
    //    public string realname { get; set; }

    //    public int? sex { get; set; }



    //    public string website { get; set; }

    //    public string adlink { get; set; }

    //    public string adcode { get; set; }


    //    public int status { get; set; }

    //    public int verify { get; set; }



    //    public decimal? balance { get; set; }

    //    public string lastipaddress { get; set; }

    //    public string lastmacaddress { get; set; }

    //    public int? adlinkhit { get; set; }

    //    public int? adcodelock { get; set; }
    //    public string billP { set; get; }
    //}


    public partial class ViewAffiliateOutModel
    {
        public long id { set; get; }
        public string account { get; set; }
        public string realname { get; set; }

        public string email { get; set; }

        public string telephone { get; set; }

        public string qq { get; set; }


        public string adcode { get; set; }

        public int status { get; set; }

        public int verify { get; set; }

        public DateTime createtime { get; set; }

        public string ipaddress { get; set; }

        public string registerUrl { set; get; }

    }
}
