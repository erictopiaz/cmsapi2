//using CMS.Data.PageModel;
using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.TurnOver
{
    /// <summary>
    /// 查询反水
    /// </summary>
    public class QueryTurnOverInputModel : PageBaseModel
    {
        [CharAndDigitalAndUnderScore(16, MinimumLength = 5, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string affaccount { set; get; }

        [CharAndDigitalAndUnderScore(16, MinimumLength = 3, IsAllowedEmpty = true, ErrorMessage = "uid_format_error")]
        public string uid { set; get; }

        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string startTime { set; get; }
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string endTime { set; get; }
        public int gameType { set; get; } = -1;
        public int status { set; get; } = -1;
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
        /// <summary>
        /// 排序条件
        /// 0：输赢  ,1:投注额， 2：创建时间
        /// </summary>
        public int sortKey { set; get; }
        /// <summary>
        /// 是否额外返水
        /// 0：返水，1：额外返水，-1：全部
        /// </summary>
        public int eturnover { set; get; } = -1;
        /// <summary>
        /// 平台对应的游戏
        /// { 1,"ssc" },{ 2,"pk10" },{ 3,"hk49" },{ 4,"bc" },{ 5,"电子" },{ 6,"捕鱼" }
        /// </summary>
        public int platformGame { set; get; } = -1;
        /// <summary>
        /// 货币种类
        /// </summary>
        public int currencyType { set; get; } = 156;
        public int walletType { set; get; } = -1;
        public string merchantAccount { set; get; } = "-1";
    }

}
