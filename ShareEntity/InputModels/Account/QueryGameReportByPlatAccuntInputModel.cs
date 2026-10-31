using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Account
{
    /// <summary>
    /// 游戏平台日报查询输入条件（boss）
    /// </summary>
    public class QueryGameReportByPlatAccuntInputModel: PageBaseModel
    {
        /// <summary>
        /// 开始时间 2026-02-01 00:00:00
        /// </summary>
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string startTime { set; get; }

        /// <summary>
        /// 结束时间 2026-02-02 00:00:00
        /// </summary>
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string endTime { set; get; }
        /// <summary>
        /// 时间类型 0.注单时间 1.结算时间
        /// 默认为 1
        /// </summary>
        public int timeType { set; get; } = 1;
        /// <summary>
        /// 报表时区 0.北京时间 1.美东时间 
        /// 默认为 0 
        /// </summary>
        public int timeZone { set; get; } = 0;

        /// <summary>
        /// 默认 999 所有
        /// 1.PG  2.JL  3.TURBO  4.JDB  5.EVO  6.9W  7.Spribe  8.FB  9.PA  10.IMES  11.IMSP  12.TF  13.LB  14.KY  15.XJ  16.DB真人 17.DB彩票
        /// </summary>
        public int gameId { set; get; } = 999;
    }
}
