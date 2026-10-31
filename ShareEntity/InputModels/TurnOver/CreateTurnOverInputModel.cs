using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.TurnOver
{
    public class CreateTurnOverInputModel : AccountModel
    {
        public int gameType { set; get; }
        [DateTimeValidate(19, true, IsAllowedEmpty = false, ErrorMessage = "time_format_error")]
        public string startTime { set; get; }
        [DateTimeValidate(19, true, IsAllowedEmpty = false, ErrorMessage = "time_format_error")]
        public string endTime { set; get; }
        [ChineseAndChar(50, 3, IsAllowedEmpty = false, ErrorMessage = "remark_format_error")]
        public string remarks { set; get; }
        [SqlInjectAttibute(IsAllowedEmpty = false, ErrorMessage = "content_format_error")]
        public string content { set; get; }
        public string gamename { set; get; }
        /// <summary>
        /// 平台指定的游戏加返水
        /// 目前有：0： 默认， 1：SSC， 2： PK10 ， 3： HK49， 4： BC   
        /// </summary>
        public int platformGame { set; get; }
        /// <summary>
        /// 0:体育， 1：真人， 2：彩票， 3：电竞， 4：电子，5：其它
        /// </summary>
        public int platformType { set; get; } = -1;

        /// <summary>
        /// 添加反水时，选择货币种类 默认为cny
        /// </summary>
        public int currencyType { set; get; } = 156;

        /// <summary>
        /// 品牌
        /// </summary>
        public string merchantAccount { set; get; } = "-1";

    }
}
