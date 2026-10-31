//using CMS.Data.Global;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.OfSetting
{
    /// <summary>
    /// app 参数配置输入
    /// </summary>
    public class AppSettingsModel : AccountModel
    {
      
        public AppSetting quanzhan_ios { set; get; }
        public AppSetting quanzhan_android { set; get; }
        public AppSetting sport_ios { set; get; }
        public AppSetting sport_android { set; get; }
        public AppSetting agent_ios { set; get; }
        public AppSetting agent_android { set; get; }
        public AppSetting keno_ios { set; get; }
        public AppSetting keno_android { set; get; }
        public AppSetting nav_iphone { set; get; }
        public AppSetting nav_android { set; get; }
        public AppSetting kf_iphone { set; get; }
        public AppSetting kf_android { set; get; }

    }

    

    public class AppSetting
    {
        /// <summary>
        /// 当前版本号
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "version_format_error")]
        public string version { set; get; } = "";
        
        /// <summary>
        /// 上一版本号
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "version_format_error")]
        public string lastVersion { set; get; } = "";
        public decimal size { set; get; } = 0m;

        /// <summary>
        /// 内容的多语系用###分开
        /// 0：中，1：英文，2：越南, 3:印度
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "content_format_error")]
        public string content { set; get; }

        public string contentLangs { set; get; } = "[]";
        /// <summary>
        /// 下载页面，供客人选择下载app
        /// </summary>
        [UrlValidate(150, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "downloadUrl_format_error")]
        public string downloadUrl { set; get; } = "";
        /// <summary>
        /// 下载包地址
        /// </summary>
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "version_format_error")]
        public string apps { set; get; }

        /// <summary>
        /// 0:ios  ,1:android
        /// </summary>
        public int appType { set; get; }

    }


    public class WebSiteSettingsModels : AccountModel
    {
        public WebSiteSettingsModel website { set; get; }
        public WebSiteSettingsModel website1 { set; get; }
        public WebSiteSettingsModel website2 { set; get; }
        public WebSiteSettingsModel website3 { set; get; }
        public Contact qq { set; get; }
    }
    /// <summary>
    /// 合营注册地址
    /// </summary>
    public class AgentRegisterHostModel
    {
        public string Bayo { set; get; }
        public string Awc { set; get; }
        public string Boss { set; get; }
    }

    public class Contact
    {

        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "contact_format_error")]
        public string contacts { set; get; }
    }


    public class WebSiteSettingsModel
    {
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "services_format_error")]
        public string url { set; get; } = "";
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "title_format_error")]
        public string title { set; get; } = "";
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "desc_format_error")]
        public string serveRange { set; get; } = "";

        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "desc_format_error")]
        public string serveTime { set; get; } = "";
       
        public int id { set; get; }
        public string icon { set; get; } = "";
        public int status { set; get; }
        /// <summary>
        /// 是否要配置token
        /// </summary>
        public int isNeedAppendToken { set; get; } = 0;

    }

    public class AppIndexGameSettingModels : AccountModel
    {
        public AppIndexGameSettingModel sportGroup { set; get; }//体育分组
        public AppIndexGameSettingModel pokerGroup { set; get; }//棋牌分组
        public AppIndexGameSettingModel esportGroup { set; get; }//电竞分组
        public AppIndexGameSettingModel liveGroup1 { set; get; }//真人分组1
        public AppIndexGameSettingModel liveGroup2 { set; get; }//真人分组2
        public AppIndexGameSettingModel kenoGroup1 { set; get; }//彩票分组1
        public AppIndexGameSettingModel kenoGroup2 { set; get; }//彩票分组2
        public AppIndexGameSettingModel kenoGroup3 { set; get; }//彩票分组3
    }

    /// <summary>
    /// app 首页游戏配置
    /// </summary>
    public class AppIndexGameSettingModel
    {
        [UrlValidate(150, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "l_pic_format_error")]
        public string l_pic { set; get; } = "";
        [UrlValidate(150, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "s_pic_format_error")]
        public string s_pic { set; get; } = "";
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "title_format_error")]
        public string title { set; get; } = "";
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "titleSec_format_error")]
        public string titleSec { set; get; } = "";
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "gameName1_format_error")]
        public string gameName1 { set; get; } = "";
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "gameName2_format_error")]
        public string gameName2 { set; get; } = "";
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "gameName3_format_error")]
        public string gameName3 { set; get; } = "";

        [UrlValidate(150, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "gameLogo1_format_error")]
        public string gameLogo1 { set; get; } = "";
        [UrlValidate(150, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "gameLogo2_format_error")]
        public string gameLogo2 { set; get; } = "";
        [UrlValidate(150, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "gameLogo3_format_error")]
        public string gameLogo3 { set; get; } = "";

        [UrlValidate(150, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "downloadUrl1_format_error")]
        public string downloadUrl1 { set; get; } = "";
        [UrlValidate(150, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "downloadUrl2_format_error")]
        public string downloadUrl2 { set; get; } = "";
        [UrlValidate(150, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "downloadUrl3_format_error")]
        public string downloadUrl3 { set; get; } = "";

        [UrlValidate(150, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "androiddownloadUrl1_format_error")]
        public string androiddownloadUrl1 { set; get; } = "";
        [UrlValidate(150, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "androiddownloadUrl2_format_error")]
        public string androiddownloadUrl2 { set; get; } = "";
        [UrlValidate(150, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "androiddownloadUrl3_format_error")]
        public string androiddownloadUrl3 { set; get; } = "";

        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "groupName_format_error")]
        public string groupName { set; get; }

        /// <summary>
        /// 组内排序
        /// </summary>
        public int groupInsideSort { set; get; } = 0;
        /// <summary>
        /// 组排序
        /// </summary>
        public int groupSort { set; get; } = 0;
    }

    public class AppSlotGameSettingModel: AppIndexGameSettingModel
    {
        public int useLobby { set; get; } = -1;

        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "gameCode1_format_error")]
        public string gameCode1 { set; get; } = "";
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "gameCode2_format_error")]
        public string gameCode2 { set; get; } = "";
        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "gameCode3_format_error")]
        public string gameCode3 { set; get; } = "";

    }

    public class AppSlotGameSettingModels : AccountModel
    {
        public AppSlotGameSettingModel ptGroup { set; get; }
        public AppSlotGameSettingModel pgGroup { set; get; }
        public AppSlotGameSettingModel ttgGroup { set; get; }
        public AppSlotGameSettingModel cq9Group { set; get; }
        public AppSlotGameSettingModel agfGroup { set; get; }
    }


    public class AppGameUrlModeSettingModels : AccountModel
    {
        public List<int> selectedBoxes { set; get; }
    }

    public class GameUrlModeSelectList
    {
        public int gameid { set; get; }
        public string gameName2 { set; get; }
    }


    public class QueryAppGameUrlModeInputModels : AccountModel
    {
        public long id { set; get; }
    }

}
