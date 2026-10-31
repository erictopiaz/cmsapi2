using System;


namespace ShareEntity.InputModels.Game.xj.Enum
{

    /// <summary>
    /// PRODUCT LIST
    /// </summary>
    public enum ProductEnum
    {
        /// <summary>
        /// Live casino game
        /// </summary>
        AgileDeal = 1,
        /// <summary>
        /// Lottery
        /// </summary>
        AgileNBG = 2,
        /// <summary>
        /// Slot game
        /// </summary>
        AgileSlot = 3,
        /// <summary>
        /// Sportsbook game
        /// </summary>
        AgileBet = 4,




    }


    /// <summary>
    /// Status between partner and keno
    /// </summary>
    [Serializable]
    public enum WagerResultStatus
    {
        Confirmed = 1,
        Settled = 2,
        Cancelled = 3,
        Void = 4
    }


    public enum DistributionChannel
    {
        /// <summary>
        /// NA
        /// </summary>
        NA = 0,
        /// <summary>
        /// WebSite
        /// </summary>
        WebSite = 10,
        /// <summary>
        /// Mobile Website
        /// </summary>
        MobileWebSite = 11,
        /// <summary>
        /// Web Flash (AgileSlot)
        /// </summary>
        WebFlash = 12,
        /// <summary>
        /// Mobile App - Android
        /// </summary>
        AndroidApp = 20,
        /// <summary>
        /// Mobile App - iOS
        /// </summary>
        iOSApp = 21,
        /// <summary>
        /// Nano
        /// </summary>
        Nano = 31,
        /// <summary>
        /// Mini
        /// </summary>
        Mini = 32,
        /// <summary>
        /// Download (AgileSlot/AgileDeal)
        /// </summary>
        Download = 41,
    }


    public struct DistributionChannel_S
    {


        //Website 桌面网页版
        //MobileWeb 手机版网页
        //iOS-WebView 嵌入于 iOS 移动应用中
        //Android-WebView 嵌入于 Android 移动应用中

        /// <summary>
        /// WebSite
        /// </summary>
        public const string WebSite = "Website";
        /// <summary>
        /// Mobile Web
        /// </summary>
        public const string MobileWeb = "MobileWeb";

        /// <summary>
        /// Mobile App - Android
        /// </summary>
        public const string AndroidWebView = "Android-WebView";
        /// <summary>
        /// Mobile App - iOS
        /// </summary>
        public const string iOSWebView = "iOS-WebView";
       
    }




    public enum OddsType
    {
        EuroOdds = 1,
        HongKongOdds = 2,
        MalayOdd = 3,
        IndonesianOdds = 4,
    }


    public enum BetStatus
    {
        Initial = 0,
        Win = 1,
        Loss = 2,
        Cancel = 4,
    }


    public enum GameType
    {
        //AgileDeal
        AgileDeal_All = 1000,
        AgileDeal_Baccarat = 1001,
        AgileDeal_Sicbo = 1101,
        AgileDeal_Roulette = 1201,

        //AgileNBG
        AgileNBG_All = 2000,
        AgileNBG_Keno_All = 2100,
        AgileNBG_Keno_Beijing = 2101,
        AgileNBG_Keno_Slovakia = 2102,
        AgileNBG_Keno_Australia = 2103,
        AgileNBG_Keno_WestCanada = 2104,
        AgileNBG_Keno_Canada = 2105,
        AgileNBG_Keno_Malta = 2106,
        AgileNBG_Lotto_All = 2300,
        AgileNBG_Lotto_ShangHai = 2310,
        AgileNBG_Lotto_ChongQing = 2320,
        AgileNBG_Lotto_JiangXi = 2330,
        AgileNBG_Lotto_TianJin = 2340,
        AgileNBG_Lotto_ChinaSWL3D = 2350,
        AgileNBG_Lotto_XinJiang = 2360,

        //AgileSlot
        AgileSlot_All = 3000,
        AgileSlot_ChineseChess = 3001,
        AgileSlot_ChineseChess3D = 3002,
        AgileSlot_XOY = 3003,
        AgileSlot_FruitPlanet = 3004,
        AgileSlot_KungFuCombat = 3005,
        AgileSlot_OceanParadise = 3007,
        AgileSlot_PandaAndDragoness = 3008,
        AgileSlot_ChineseZodiac = 3009,
        AgileSlot_TheDoggyJob = 3010,
        AgileSlot_ShanghaiLegend = 3011,
        AgileSlot_PrettySun = 3012,
        AgileSlot_IamTuhao = 3013,
        AgileSlot_MahjongKing = 3014,
        AgileSlot_NaughtyLandlord = 3015,
        AgileSlot_JourneyToTheWest = 3016,

        //AgileBet
        AgileBet_All = 4000,

    }


    public enum LaunchType
    {
        /// <summary>
        /// Normal Launch - Default
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Play for fun (AgileSolt use only)
        /// </summary>
        PlayForFun = 1
    }


}
