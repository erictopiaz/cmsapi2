//using CMS.Data.PageModel;
using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Saler
{
    /// <summary>
    /// 销售报表查询输入数据
    /// </summary>
    public class SalerReportInputModel : PageBaseModel
    {
        [CharAndDigitalAndUnderScore(16, MinimumLength = 3, IsAllowedEmpty = true, ErrorMessage = "saler_format_error")]
        public string saler { set; get; }

        public int startTime { set; get; } = -1;
        public int endTime { set; get; } = -1;

    }

    /// <summary>
    /// 记录查询输出
    /// </summary>
    public class SalerReportOutModel : SalerReport
    {
        public string account { set; get; }
        public string saler { set; get; }
        public int financeGameDay { set; get; }
        //public decimal depositAmount { set; get; }
        //public decimal withdrawAmount { set; get; }
        //public decimal promo { set; get; }
        //public decimal bonus { set; get; }
    }

    public class SalerReport
    {
        public decimal depositAmount { set; get; }
        public decimal withdrawAmount { set; get; }
        public decimal promo { set; get; }
        public decimal bonus { set; get; }
    }
}
