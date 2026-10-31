using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game {


    /// <summary>
    /// 获取游戏首页配置
    /// </summary>
    public class GameSetting : ICloneable {
        /// <summary>
        /// 游戏名
        /// </summary>
        public string game { set; get; }
        /// <summary>
        /// 游戏分类id
        /// </summary>
        public int gameType { set; get; }
        public int linkTolobby { set; get; }
        /// <summary>
        /// 游戏分类名称
        /// </summary>
        public string gameTypeName { set; get; }

        private string _gameTypeNames;
        /// <summary>
        /// 分类名语语系列表
        /// </summary>
        public string gameTypeNames {
            set {

                if (!string.IsNullOrEmpty(value)) {
                    _gameTypeNames = value;
                    var langDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(value);
                    if (langDic != null) {
                        foreach (var lang in langDic) {
                            menusNames.Add(new GameMenusName() { lang = lang.Key, name = lang.Value });
                        }
                    }
                }
            }
            get { return _gameTypeNames; }
        }
        /// <summary>
        /// 分类名语语系列表
        /// </summary>
        public List<GameMenusName> menusNames { set; get; } = new List<GameMenusName>();
        /// <summary>
        /// 游戏描述主标题
        /// </summary>
        public string gTitle { set; get; }
        /// <summary>
        /// 游戏描述，第二标题
        /// </summary>
        public string gTitleSec { set; get; }
        /// <summary>
        /// 游戏背影图
        /// </summary>
        public string gamePicUrl { set; get; }
        /// <summary>
        /// 游戏背影图 选择中的后的图片
        /// </summary>
        public string gamePicUrl_S { set; get; }
        /// <summary>
        /// 图片拼接的path
        /// </summary>
        public string gamePicUrl_Path { set; get; }
        /// <summary>
        /// 图片名+版本号
        /// </summary>
        public string picName { set; get; }
        /// <summary>
        /// 平台分组排序
        /// </summary>
        public int sortId { set; get; }
        /// <summary>
        /// 游戏相关图片版本
        /// </summary>
        public string picVersion { set; get; }

        /// <summary>
        /// 前端暂不使用
        /// </summary>
        public int useable { set; get; }
        /// <summary>
        /// 前端暂不使用
        /// </summary>
        public int online { set; get; }
        /// <summary>
        /// 游戏支持的币种
        /// </summary>
        public string currencyTypes { set; get; }

        public int transferSort { set; get; }
        /// <summary>
        /// 当前游戏支持的钱包类型
        /// 1：转账钱包，0：单一钱包，2：二个都支持
        /// </summary>
        public int g_walletType { set; get; }
        /// <summary>
        /// 支持的品牌名
        /// </summary>
        public string g_merchantAccounts { set; get; }
        public object Clone() {
            return this.MemberwiseClone();
        }
    }

    public class GameMenusName {
        public int lang { set; get; }
        public string name { set; get; }
    }

    /// <summary>
    /// 游戏相关图片版本
    /// </summary>
    public class GamePicV {
        /// <summary>
        /// 首页游戏图版
        /// </summary>
        public string homegameV { set; get; }
    }

    #region 输出模型，转到项目内 ，这里暂时注释掉

    ///// <summary>
    ///// 首页游戏导航语语系翻译
    ///// </summary>
    //public class GameMenuSetting {
    //    /// <summary>
    //    /// 游戏分类名
    //    /// </summary>
    //    public string name { set; get; }
    //    /// <summary>
    //    /// 游戏分类id
    //    /// </summary>
    //    public int gametype { set; get; }
    //    /// <summary>
    //    /// 游戏分类图片地址
    //    /// </summary>
    //    public string gamePicUrl { set; get; }

    //    /// <summary>
    //    /// 链接到自定义的lobby  0：否，1:是，默认为0
    //    /// </summary>
    //    public int linkTolobby { set; get; } = 0;

    //    /// <summary>
    //    /// 游戏分类图片路径
    //    /// </summary>
    //    public string gamePicUrl_Path { set; get; }
    //    /// <summary>
    //    /// 游戏分类高亮图片路径
    //    /// </summary>
    //    public string gamePicUrl_S_Path { set; get; }
    //    /// <summary>
    //    /// 游戏分类图片名
    //    /// </summary>
    //    public string picName { set; get; }
    //    /// <summary>
    //    /// 游戏分类名，语系集合
    //    /// </summary>
    //    public string names {
    //        set {
    //            if (!string.IsNullOrEmpty(value)) {
    //                var langDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(value);
    //                if (langDic != null) {
    //                    foreach (var lang in langDic) {
    //                        menusNames.Add(new GameMenusName() { lang = lang.Key, name = lang.Value });
    //                    }
    //                }
    //            }
    //        }
    //        get { return ""; }
    //    }
    //    /// <summary>
    //    ///  游戏分类名，语系集合
    //    /// </summary>
    //    public List<GameMenusName> menusNames { set; get; } = new List<GameMenusName>();
    //}


    ///// <summary>
    ///// 首面游戏侧栏描述
    ///// </summary>
    //public class SideMenuModel {
    //    /// <summary>
    //    /// 游戏分类名
    //    /// </summary>
    //    public string name { set; get; }
    //    /// <summary>
    //    /// 游戏分类id
    //    /// </summary>
    //    public string gametype { set; get; }
    //    /// <summary>
    //    /// 选中高亮logo
    //    /// </summary>
    //    public string gamePicUrl { set; get; }
    //    /// <summary>
    //    /// 没有选中，灰色
    //    /// </summary>
    //    public string gamePicUrl_0 { set; get; }

    //    /// <summary>
    //    /// 是否显示分类下面的所有（0：否，1：是）
    //    /// 当前分类的所有平台，（如果不空，则要处理显示的逻辑）
    //    /// 注：根据gametype 查显示所有当前的分类下的游戏平台
    //    /// </summary>
    //    public int isNeedAll { set; get; } = 0;
    //}
    ///// <summary>
    ///// app 首页游戏设置描述
    ///// </summary>
    //public class GaemUiModel {
    //    /// <summary>
    //    /// 首面游戏侧栏描述
    //    /// </summary>
    //    public List<SideMenuModel> sideMenus { set; get; }
    //    /// <summary>
    //    /// 游戏平台
    //    /// </summary>
    //    public List<GameSetting> plats { set; get; }
        
    //}

    #endregion
}
