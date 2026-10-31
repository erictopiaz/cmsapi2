//using CMS.Data.PageModel;
using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.GameConfig
{
    public class QueryGameConfigInputModel : PageBaseModel
    {
        /// <summary>
        /// 游戏ID
        /// </summary>
        public int gameid { set; get; }
        /// <summary>
        /// 游戏分类
        /// </summary>
        public int gameclass { set; get; }
        /// <summary>
        /// 游戏名称
        /// </summary>
        [CharAndDigital(12, MinimumLength = 2, IsAllowedEmpty = true, ErrorMessage = "game_name_format_error")]
        public string gamename { set; get; }
        /// <summary>
        /// 当前启用状态
        /// </summary>
        public int gamestatus { set; get; }

        public int currencyType { set; get; } = 156;
    }
    /// <summary>
    /// 显示电子游戏列表
    /// </summary>
    public class QueryGameModelsInputModel : PageBaseModel
    {
       
    }
}
