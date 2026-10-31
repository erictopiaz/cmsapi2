using ShareEntity.ClientModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Game
{
    public class OBWidgetUrlInputModel : IPInputModel
    {
        /// <summary>
        /// 会员账号
        /// </summary>
        [CharAndDigitalAndUnderScore(20, MinimumLength = 5, IsAllowedEmpty = true, ErrorMessage = "账号格式有误")]
        public string account { set; get; }
        
        /// <summary>
        /// 1:android，2：ios,   3:h5  , 4: pc , 5: 体育ios,6: 体育android
        /// </summary>
        public int device { set; get; } = 0;
        /// <summary>
        /// 可带入下列其中一个：en、cs、ch、id、vn、th
        /// </summary>
        [MaxLength(3)]
        public string languageDesc { get; set; } = "";
        /// <summary>
        /// 默认为 00000000
        /// </summary>
        public string widgetId { get; set; } = "00000000";
        /// <summary>
        /// 填入开启快捷投注网站的完整链接，用于减少跨网域风险，例如 https://www.google.com
        /// </summary>
        public string domain { set; get; } = "";
        /// <summary>
        /// 赔率类型表
        /// 1 Malay Odds(马来盘),2 China Odds(中国盘),3 Decimal Odds(欧洲盘),4 Indo Odds(印度尼西亚盘),5 American Odds(美国盘)
        /// </summary>
        public string odds_type { set; get; } = "";

        /// <summary>
        /// 可不填写
        /// </summary>
        public int gameid { set; get; } = -1;
        /// <summary>
        /// 可不填写
        /// </summary>
        public int currencyType { set; get; } = 0;
        /// <summary>
        /// 可不填写
        /// </summary>
        public string merchantAccount { set; get; } = "";
        /// <summary>
        /// 会员id , 可不填写
        /// </summary>
        public long id { set; get; } = 0;
    }

}
