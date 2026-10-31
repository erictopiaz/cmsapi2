//using CMS.Data.ClientModel;
using ShareEntity.ClientModel;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Game
{
    public class GameLoginBaseInputModel : IPInputModel {
        /// <summary>
        /// 游戏账号
        /// </summary>
        [CharAndDigitalAndUnderScore(16, MinimumLength = 6, IsAllowedEmpty = true, ErrorMessage = "账号格式有误")]
        public string account { set; get; }
        /// <summary>
        /// 游戏id
        /// </summary>
        public int gameid { set; get; }
        /// <summary>
        /// 语系 : 1 英文, 2 简体中文,3 繁体中文,4 泰文,5 越南文
        /// </summary>
        public int lang { set; get; }

        /// <summary>
        /// pt游戏登录参数:平台游戏代码
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "game_gamecode_format_error")]
        public string gamecode { set; get; }

        /// <summary>
        /// qt游戏登录参数:"demo"; "real";
        /// mg: 1:demo,0:real
        /// sw: fun,real
        /// pg,bg: 1:real,2:demo
        /// </summary>
        [CharAndDigital(16, MinimumLength = 1, IsAllowedEmpty = true, ErrorMessage = "game_gamemodel_format_error")]
        public string gamemodel { set; get; } = "";

        /// <summary>
        /// 1:android，2：ios,   3:h5  , 4: pc , 5: 体育ios,6: 体育android
        /// </summary>
        public int device { set; get; }

        /// <summary>
        /// 游戏线路，1：默认线路(线路1), 2:线路 2   (目前lb、aw 有多条线路使用)
        /// </summary>
        public int gamelane { set; get; } = 1;
        /// <summary>
        /// 游戏中名从前台缓存中获取
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "game_name_format_error")]
        public string gamechinesename { set; get; }
        /// <summary>
        /// 游戏中名从前台缓存中获取图片
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "game_pic_name_format_error")]
        public string gamepic { set; get; }

        /// <summary>
        /// bbin ---- 0: 电子,1： 真人, 2： 体育, 3： 电竞, 4： 彩票
        /// ag -- gametype=2 真人国际厅，gametype=6捕鱼王,gametype=13 h5版国际厅
        /// im  --- 0:ESPORTSBULL_ESPORTS   , 1  : IM_BG
        /// </summary>
        public int gameType { set; get; }
        /// <summary>
        /// ttg
        /// </summary>
        [CharAndDigital(50, MinimumLength = 2, IsAllowedEmpty = true, ErrorMessage = "game_ttg_name_format_error")]
        public string gameName { set; get; }

        /// <summary>
        /// ttg
        /// </summary>
        [DigitalValidate(6, MinimumLength = 1, IsAllowedEmpty = true, ErrorMessage = "game_ttg_gametype_format_error")]
        public string ttgGameType { set; get; }

        /// <summary>
        /// 货币类型
        /// </summary>
        public int currencyType { get; set; } = 156;
        /// <summary>
        /// ag 登录数据
        /// </summary>
        public string agaccount { set; get; } = "";

        /// <summary>
        /// 仅pg 过度使用，0：旧方式登录url，1：新方式登录 html源始码,  默认为0
        /// </summary>
        public int temp { set; get; } = 0;
        /// <summary>
        /// 品牌 2025-05-31
        /// </summary>
        public string merchantAccount { set; get; } = "";
    }
}