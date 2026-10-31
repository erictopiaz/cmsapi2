using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{

    /// <summary>
    /// Modified ew_turnover to ea_turnvoer
    /// </summary>
    [Table("ofUserGroup")]
    public partial class ofUserGroupModified
    {
        public long id { get; set; }

        public int levelnumber { get; set; }

        public string levels { get; set; }

        public string description { get; set; }

        public DateTime time { get; set; }

        //public string _operator { get; set; }

        public decimal? ea_turnover { get; set; }
        public decimal? ea_turnovermax { get; set; }

        public decimal? ew_turnover { get; set; }
        public decimal? ew_turnovermax { get; set; }
        public decimal? pt_turnover { get; set; }

        public decimal? pt_turnovermax { get; set; }

        public decimal? pttg_turnover { get; set; }

        public decimal? pttg_turnovermax { get; set; }

        public decimal? sun_turnover { get; set; }

        public decimal? sun_turnovermax { get; set; }

        public decimal? keno_turnover { get; set; }

        public decimal? keno_turnovermax { get; set; }

        public decimal? hg_turnover { get; set; }

        public decimal? hg_turnovermax { get; set; }

        public decimal? gd_turnover { get; set; }

        public decimal? gd_turnovermax { get; set; }

        public decimal? bbin_turnover { get; set; }

        public decimal? bbin_turnovermax { get; set; }

        public decimal spb_turnover { get; set; }

        public decimal spb_turnovermax { get; set; }

        public decimal ttg_turnover { get; set; }

        public decimal ttg_turnovermax { get; set; }

        public decimal? lb_turnover { get; set; }

        public decimal? lb_turnovermax { get; set; }

        public decimal ag_turnover { get; set; }

        public decimal ag_turnovermax { get; set; }

        public decimal bonuspercent { get; set; }

        public int betnumber { get; set; }

        public decimal bonusmax { get; set; }

        public int reqlimit { get; set; }


        public string onestepset { get; set; }

        public string twostepset { get; set; }

        public decimal? lbssc_turnover { get; set; }

        public decimal? lbssc_turnovermax { get; set; }

        public decimal? bsg_turnover { get; set; }

        public decimal bsg_turnovermax { get; set; }

        public decimal? keno2_turnover { get; set; }

        public decimal? keno2_turnovermax { get; set; }

        public decimal? vr_turnover { get; set; }

        public decimal? vr_turnovermax { get; set; }

        public decimal? xj_turnover { get; set; }

        public decimal? xj_turnovermax { get; set; }

        public decimal? pg_turnover { get; set; }

        public decimal? pg_turnovermax { get; set; }

        public decimal? ds_turnover { get; set; }

        public decimal? ds_turnovermax { get; set; }

        public decimal? dses_turnover { get; set; }

        public decimal? dses_turnovermax { get; set; }

        public decimal? ob_turnover { get; set; }

        public decimal? ob_turnovermax { get; set; }
        public decimal? bti_turnover { get; set; }
        public decimal? bti_turnovermax { get; set; }
        public decimal? imes_turnover { get; set; }
        public decimal? imes_turnovermax { get; set; }
        public decimal? rg_turnover { get; set; }
        public decimal? rg_turnovermax { get; set; }
        public decimal? ky_turnover { get; set; }
        public decimal? ky_turnovermax { get; set; }
        public decimal? kg_turnover { get; set; }
        public decimal? kg_turnovermax { get; set; }
        public decimal? kg2_turnover { get; set; }
        public decimal? kg2_turnovermax { get; set; }
        public decimal? ysb_turnover { get; set; }
        public decimal? ysb_turnovermax { get; set; }
        
        public decimal? bg_turnover { get; set; }
        public decimal? bg_turnovermax { get; set; }
        public decimal? eb_turnover { get; set; }
        public decimal? eb_turnovermax { get; set; }
        public decimal? td_turnover { get; set; }
        public decimal? td_turnovermax { get; set; }
        
        public decimal? tf_turnover { get; set; }
        public decimal? tf_turnovermax { get; set; }
        public decimal? cq_turnover { get; set; }
        public decimal? cq_turnovermax { get; set; }
        public decimal? aw_turnover { get; set; }
        public decimal? aw_turnovermax { get; set; }
        
        public decimal? kg3_turnover { get; set; }
        public decimal? kg3_turnovermax { get; set; }
        public decimal? impk_turnover { get; set; }
        public decimal? impk_turnovermax { get; set; }

        public decimal? jdb_turnover { get; set; }
        public decimal? jdb_turnovermax { get; set; }

        public decimal? obg_turnover { get; set; }
        public decimal? obg_turnovermax { get; set; }

        public decimal? obcv_turnover { get; set; }
        public decimal? obcv_turnovermax { get; set; }
        public decimal? fb_turnover { get; set; }
        public decimal? fb_turnovermax { get; set; }

        public decimal? imsp_turnover { get; set; }
        public decimal? imsp_turnovermax { get; set; }
        
        public decimal? cr_turnover { get; set; }
        public decimal? cr_turnovermax { get; set; }

        public decimal? sw_turnover { get; set; }
        public decimal? sw_turnovermax { get; set; }
        public decimal? pm_turnover { get; set; }
        public decimal? pm_turnovermax { get; set; }
        public decimal? db_turnover { get; set; }
        public decimal? db_turnovermax { get; set; }
        public decimal? rsg_turnover { get; set; }
        public decimal? rsg_turnovermax { get; set; }
        public decimal? dbkeno_turnover { get; set; }
        public decimal? dbkeno_turnovermax { get; set; }

        public decimal? pp_turnover { get; set; }
        public decimal? pp_turnovermax { get; set; }



        public decimal? iaes_turnover { get; set; }
        public decimal? iaes_turnovermax { get; set; }


        public string? misc { get; set; }
        /// <summary>
        /// 印度货币 反水比例
        /// </summary>
        public string misc_inr { get; set; }
    }



    [Table("ofUserGroup")]
    public partial class ofUserGroup
    {
        public long id { get; set; }

        public int levelnumber { get; set; }

        public string levels { get; set; }

        public string description { get; set; }
        public string misc { get; set; }

        public DateTime time { get; set; }

        public decimal? ew_turnover { get; set; }

        public decimal? ew_turnovermax { get; set; }
        public decimal? ea_turnover { get; set; }

        public decimal? ea_turnovermax { get; set; }
        public decimal? pt_turnover { get; set; }

        public decimal? pt_turnovermax { get; set; }

        public decimal? pttg_turnover { get; set; }

        public decimal? pttg_turnovermax { get; set; }

        public decimal? sun_turnover { get; set; }

        public decimal? sun_turnovermax { get; set; }

        public decimal? keno_turnover { get; set; }

        public decimal? keno_turnovermax { get; set; }

        public decimal? hg_turnover { get; set; }

        public decimal? hg_turnovermax { get; set; }

        public decimal? gd_turnover { get; set; }

        public decimal? gd_turnovermax { get; set; }

        public decimal? bbin_turnover { get; set; }

        public decimal? bbin_turnovermax { get; set; }

        public decimal spb_turnover { get; set; }

        public decimal spb_turnovermax { get; set; }

        public decimal ttg_turnover { get; set; }

        public decimal ttg_turnovermax { get; set; }

        public decimal? lb_turnover { get; set; }

        public decimal? lb_turnovermax { get; set; }

        public decimal ag_turnover { get; set; }

        public decimal ag_turnovermax { get; set; }

        public decimal bonuspercent { get; set; }

        public int betnumber { get; set; }

        public decimal bonusmax { get; set; }

        public int reqlimit { get; set; }

       
        public string onestepset { get; set; }

        public string twostepset { get; set; }

        public decimal? lbssc_turnover { get; set; }

        public decimal? lbssc_turnovermax { get; set; }

        public decimal? bsg_turnover { get; set; }

        public decimal bsg_turnovermax { get; set; }

        public decimal? keno2_turnover { get; set; }

        public decimal? keno2_turnovermax { get; set; }

        public decimal? vr_turnover { get; set; }

        public decimal? vr_turnovermax { get; set; }

        public decimal? xj_turnover { get; set; }

        public decimal? xj_turnovermax { get; set; }

        public decimal? pg_turnover { get; set; }

        public decimal? pg_turnovermax { get; set; }

        public decimal? ds_turnover { get; set; }

        public decimal? ds_turnovermax { get; set; }

        public decimal? dses_turnover { get; set; }

        public decimal? dses_turnovermax { get; set; }

        public decimal? ob_turnover { get; set; }
        public decimal? ob_turnovermax { get; set; }
        public decimal? bti_turnover { get; set; }
        public decimal? bti_turnovermax { get; set; }
        public decimal? bg_turnover { get; set; }
        public decimal? bg_turnovermax { get; set; }

        public decimal? eb_turnover { get; set; }
        public decimal? eb_turnovermax { get; set; }
        public decimal? kg3_turnover { get; set; }
        public decimal? kg3_turnovermax { get; set; }
    }
}
