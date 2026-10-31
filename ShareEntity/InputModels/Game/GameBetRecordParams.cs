using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Game
{
    public class GameBetRecordParams
    {
        /// <summary>
        /// 会员账号
        /// </summary>
        [CharAndDigital(12, MinimumLength = 6, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string account { set; get; }
        public int gameid { set; get; } = 0;
        /// <summary>
        /// 游戏平台名称 必填
        /// </summary>
        public string gamename { set; get; }

        #region ky 指定时间段内查询所有客个的下注信息

        /// <summary>
        /// ky 注单开始时间
        /// </summary>
        [DateTimeValidate(19, true, IsAllowedEmpty = true)]
        public string ky_startTime { set; get; }
        /// <summary>
        /// ky 注单结束时间
        /// </summary>
        [DateTimeValidate(19, true, IsAllowedEmpty = true)]
        public string ky_endTime { set; get; }
        #endregion

        #region ttg 请求指定时间内的游戏报表数据，如总投注和总输赢
        /// <summary>
        /// ttg   lb 共用日期
        /// </summary>
        [DigitalValidate(8, MinimumLength = 8, IsAllowedEmpty = true, ErrorMessage = "请求开始日期yyyyMMDD")]
        public string startDate { set; get; }
        /// <summary>
        /// ttg  lb 共用日期
        /// </summary>
        [DigitalValidate(8, MinimumLength = 8, IsAllowedEmpty = true, ErrorMessage = "请求结束日期yyyyMMDD")]
        public string endDate { set; get; }
        [DigitalValidate(2, MinimumLength = 1, IsAllowedEmpty = true, ErrorMessage = "请求当天的小时")]
        public string startDateHour { set; get; }
        [DigitalValidate(2, MinimumLength = 1, IsAllowedEmpty = true, ErrorMessage = "请求当天的小时")]
        public string endDateHour { set; get; }
        [DigitalValidate(2, MinimumLength = 1, IsAllowedEmpty = true, ErrorMessage = "请求当天的小时内的分数")]
        public string startDateMinute { set; get; }
        [DigitalValidate(2, MinimumLength = 1, IsAllowedEmpty = true, ErrorMessage = "请求当天的小时内的分数")]
        public string endDateMinute { set; get; }

        #endregion

        #region pg 指定时间内获取的游戏下注记录
        /// <summary>
        /// 当这数据小5000时就算取完了
        /// pg   pt bbin 共用
        /// </summary>
        public int pageSize { set; get; }
        /// <summary>
        /// pg   pt bbin  共用
        /// </summary>
        public int pageIndex { set; get; }
        /// <summary>
        /// pg betType => 投注模式 (1：金钱；2：试玩；3：锦标赛）
        /// </summary>
        public int betType { set; get; }
        /// <summary>
        /// pg 根据这个rowVersion返回下注记录
        /// </summary>
        public long pg_row_version { set; get; }
        /// <summary>
        /// pg 输入时间段,会自动转为時間戳
        /// </summary>
        public string pg_startTime { set; get; }
        /// <summary>
        /// pg 输入时间段,会自动转为時間戳
        /// </summary>
        public string pg_endTime { set; get; }

        #endregion

        #region skywind 指定时间内获取有效下注记录
        /// <summary>
        /// 记录row no
        /// </summary>
        public int offset { set; get; }
        /// <summary>
        /// 分页大小
        /// </summary>
        public int limit { set; get; }
        /// <summary>
        /// 查询开始时间
        /// </summary>
        public DateTime firstTs__gt { set; get; }
        /// <summary>
        /// 查询结束时间
        /// </summary>
        public DateTime firstTs__lt { set; get; }
        #endregion

        #region Mg  指定rowid取后面最多2W条记录
        public Int64 rowId { set; get; }
        public string startingAfter { set; get; }//mg 新版API 
        #endregion

        #region pt 指定时间段内查询所有客个的下注信息
        /// <summary>
        /// pt  bbin  lb共用
        /// </summary>
        [DateTimeValidate(29, true, IsAllowedEmpty = true)]
        public string startDatePt { set; get; }
        /// <summary>
        /// pt bbin lb共用
        /// </summary>
        [DateTimeValidate(29, true, IsAllowedEmpty = true)]
        public string endDatePt { set; get; }
        public int showInfo { set; get; }
        #endregion

        #region bbin 指定日期和时间查询客个下注信息
        /// <summary>
        /// bbin 游戏种类(1：BB 体育、3：BB 视讯、5：BB 电子、12：BB 彩票) 
        /// 目前只有真人视讯，默认为 3
        /// </summary>
        public int gamekind { set; get; } = 3;
        /// <summary>
        /// bbin 传入的日期  如 yyyy-MM-dd
        /// </summary>
        public string bbin_rounddate { set; get; }
        /// <summary>
        /// bbin 传入的开始时间 如  "HH:mm:ss"
        /// </summary>
        public string bbin_starttime { set; get; }
        /// <summary>
        /// bbin 传入的结束时间 如  "HH:mm:ss"
        /// </summary>
        public string bbin_endtime { set; get; }

        #endregion

        #region lb 指寂静日期内所有客人投注记录
        public string status { set; get; }
        #endregion

        #region  cq9 
        [DateTimeValidate(19, true, IsAllowedEmpty = true)]
        public string starttimeCQ9 { set; get; }
        [DateTimeValidate(19, true, IsAllowedEmpty = true)]
        public string endtimeCQ9 { set; get; }
        #endregion

        #region XJ
        /// <summary>
        /// 是否结算1：已结算，0：未结算
        /// </summary>
        public int xj_isSettled { set; get; }
        /// <summary>
        /// 请求注单的开始时间
        /// yyyy-MM-ddTHH:mm:ss.fffffff
        /// </summary>
        public string xj_starttime { set; get; }
        /// <summary>
        /// 请求注单的结束时间
        /// yyyy-MM-ddTHH:mm:ss.fffffff
        /// </summary>
        public string xj_endtime { set; get; }
        /// <summary>
        /// 4000 体育
        /// </summary>
        public int xj_gameType { set; get; }
        #endregion

        #region SB
        /// <summary>
        /// 注单的最版本号
        /// 初始值为0  下次请求时，请传上次返回什中的last_version_key
        /// 每次返回500条数据
        /// </summary>
        public int last_version_key { set; get; } = 0;
        #endregion

        #region AG
        /// <summary>
        /// 请求注单的开始时间
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string ag_starttime { set; get; }
        /// <summary>
        /// 请求注单的结束时间
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string ag_endtime { set; get; }
        /// <summary>
        /// 0:fish  1:live
        /// </summary>
        public int ag_type { set; get; } = 1;
        #endregion

        #region lb 指定日期内所有客人投注记录
        /// <summary>
        /// 请求注单的开始时间
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string lb_starttime { set; get; }
        /// <summary>
        /// 请求注单的结束时间
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string lb_endtime { set; get; }
        /// <summary>
        /// all ： 全部   settled: 已完成   unsettled： 未完成   cancel：已取消
        /// </summary>
        public string lb_settleStatus { set; get; }
        #endregion

        #region kg 指定日期内所有客人投注记录  返回记录只有结算记录
        /// <summary>
        /// 请求注单的开始时间
        /// yyyy-MM-dd HH:mm:ss   当前时间向前推10分钟内的数据的
        /// </summary>
        public string kg_starttime { set; get; }

        #endregion

        #region vr 指定日期内所有客人投注记录  返回记录只有结算记录
        /// <summary>
        /// vr 请求注单的开始时间 会自动转为平台时间格式
        /// </summary>
        public string vr_starttime { set; get; }
        /// <summary>
        /// vr 请求注单的结束时间 会自动转为平台时间格式
        /// </summary>
        public string vr_endtime { set; get; }

        #endregion
        #region imes 指定日期内所有客人投注记录  返回记录只有结算记录
        /// <summary>
        /// 请求注单的开始时间
        /// yyyy-MM-dd HH:mm:ss  
        /// </summary>
        public string imes_starttime { set; get; }
        public string imes_endtime { set; get; }
        #endregion
        #region rg 指定日期内所有客人投注记录  返回记录只有结算记录
        /// <summary>
        /// 请求注单的开始时间
        /// yyyy-MM-dd HH:mm:ss  
        /// </summary>
        public string rg_starttime { set; get; }
        public string rg_endtime { set; get; }
        #endregion

        #region bg 指定时间内所有客人的投注记录
        public string bg_starttime { set; get; }
        public string bg_endtime { set; get; }
        #endregion

        #region eb 指定时间内所有客人的投注记录
        public string eb_starttime { set; get; }
        public string eb_endtime { set; get; }
        #endregion

        #region td 指定所有客人的投注记录
        public long beginId { set; get; }
        #endregion

        #region tf 指定所有客人的投注记录
        /// <summary>
        /// tf 请求注单的开始时间 会自动转为平台时间格式
        /// </summary>
        public string tf_starttime { set; get; }
        /// <summary>
        /// tf 请求注单的结束时间 会自动转为平台时间格式
        /// </summary>
        public string tf_endtime { set; get; }
        #endregion

        #region impk 指定所有客人的投注记录
        public string impk_starttime { set; get; }
        public string impk_endtime { set; get; }
        #endregion

        #region aw 指定所有客人的投注记录
        /// <summary>
        /// aw 注单开始id
        /// </summary>
        public long startId { set; get; }
        /// <summary>
        /// aw 注单开始时间
        /// </summary>
        public string aw_starttime { set; get; }
        //public string aw_endtime { set; get; }
        #endregion

        #region kg3 指定所有客人的投注记录
        public string kg3_starttime { set; get; }
        public string kg3_endtime { set; get; }
        #endregion

        #region jdb 所有客人投注记录
        /// <summary>
        /// jdb 平台注单时间
        /// </summary>
        public string jdb_starttime { set; get; }
        /// <summary>
        /// jdb 平台注单结束时间
        /// </summary>
        public string jdb_endtime { set; get; }

        #endregion

        #region obg 体育 所有客人投注记录 后改名为  PM 了。
        /// <summary>
        /// obg pm 注单请求时间 会自动转为平台时间格式
        /// </summary>
        public string obg_sport_starttime { set; get; }
        /// <summary>
        /// obg pm 注单请求结束时间 会自动转为平台时间格式
        /// </summary>
        public string obg_sport_endtime { set; get; }

        #endregion

        #region cr 体育 所有客人投注记录

        public string cr_sport_starttime { set; get; }
        public string cr_sport_endtime { set; get; }

        /// <summary>
        /// (settle = 0) dateStart dateEnd 為下注時間
        /// (settle = 1) dateStart dateEnd 為結算時間
        /// </summary>
        public int cr_settle { set; get; }

        #endregion

        #region fb 体育 注单拉取
        /// <summary>
        /// fb 拉取json 文件ids 列表，0：否，1：是
        /// fb 拉取注单，是先拉到注单的json文件的ids 然后再根据文件id 获取 json 文件
        /// </summary>
        public int pullIds { set; get; } = 0;
        /// <summary>
        /// fb 拉取单个文件的id
        /// </summary>
        public long fileId { set; get; } = 0;
        /// <summary>
        /// fb 文件id 拉取的开始时间
        /// </summary>
        public string fb_startTime { set; get; }
        /// <summary>
        /// fb 文件id 拉取的结束时间
        /// </summary>
        public string fb_endtime { set; get; }
        #endregion

        #region evo 真人注单拉取
        /// <summary>
        /// evo 注单开始时间
        /// </summary>
        public string evo_starttime { set; get; }
        /// <summary>
        /// evo 注单结束时间
        /// </summary>
        public string evo_endtime { set; get; }
        #endregion

        /// <summary>
        /// 请求的IP 地址, 可空
        /// </summary>
        [IpValidate(50, MinimumLength = 8, IsAllowedEmpty = true, ErrorMessage = "IP地址有误")]
        public string ipaddress { set; get; }
    }

}
