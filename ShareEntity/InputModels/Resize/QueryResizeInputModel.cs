//using CMS.Data.PageModel;
using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Resize
{
    public class QueryResizeInputModel : PageBaseModel
    {
        [CharAndDigital(12, MinimumLength = 6, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string affaccount { set; get; }

        [SqlInjectAttibute(IsAllowedEmpty =true, ErrorMessage ="remark_format_error")]
        public string remarks { set; get; }

        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "remark_format_error")]
        public string remark { set; get; }

        //[CharAndDigital(12, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        //public string adminAccount { set; get; }
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "开始时格式有误")]
        public string startTime { set; get; }
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "结束时间格式有误")]
        public string endTime { set; get; }
        public int gameType { set; get; } = -1;
        public int status { set; get; } = -1;
       public int decreasetype { set; get; } = -1;
        /// <summary>
        /// 游戏平台类型： 
        /// 0: 体育
        /// 1：真人
        /// 2：彩票
        /// 3：电竞
        /// 4：电子
        /// 5：其它  
        /// </summary>
        public int platformType { set; get; } = -1;

        public int currencyType { set; get; } = 156;
        //查询输入
        public int walletType { set; get; } = -1;
        public string merchantAccount { set; get; } = "-1";
    }
}
