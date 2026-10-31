//using CMS.Data.PageModel;
using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Account
{
    public class QueryUserLogInputModel : PageBaseModel
    {
        /// <summary>
        /// 账号类型
        /// </summary>
        public int accountType { set; get; }

        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string startTime { set; get; }
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string endTime { set; get; }

    }
    /// <summary>
    /// 会员注单记录查询 
    /// </summary>
    public class QueryUserBetLogInputModel : PageBaseModel
    {

        public int gameId { set; get; }
        private string _startTime;
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string startTime
        {
            set
            {
                _startTime = string.IsNullOrEmpty(value) ? DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd 00:00:00") : value;
            }
            get
            {
                return _startTime;
            }
        }
        private string _endTime;
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string endTime
        {
            set
            {
                _endTime = string.IsNullOrEmpty(value) ? DateTime.UtcNow.AddHours(8).AddDays(1).ToString("yyyy-MM-dd 00:00:00") : value;
            }
            get
            {
                return _endTime;
            }
        }
        /// <summary>
        /// 1:结算 ，0：未结算 , -1:全部
        /// </summary>
        public int settled { set; get; } = 1;


        /// <summary>
        /// 货种类型
        /// </summary>
        public int currencyType { set; get; } = 356;
        /// <summary>
        /// 注单号
        /// </summary>
        [MaxLength(50)]
        public string betId { set; get; } = "";

    }


    /// <summary>
    /// 会员注单记录查询 
    /// </summary>
    public class QueryOrdersByPlatInputModel : PageBaseModel {

        /// <summary>
        /// 游戏id: 1.PG 2.JL 3.TURBO 4.JDB 5.EVO 6.9W
        ///    7.fb 8.pa 9.tf 10.imes  11.imsp
        /// </summary>
        public int gameId { set; get; }
        private string _startTime;
        /// <summary>
        /// 开始时间
        /// </summary>
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string startTime {
            set {
                _startTime = string.IsNullOrEmpty(value) 
                    ? DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-ddT00:00:00+08:00") 
                    : Convert.ToDateTime(value).ToString("yyyy-MM-ddT00:00:00+08:00");
            }
            get {
                return _startTime;
            }
        }
        private string _endTime;
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string endTime {
            set {
                _endTime = string.IsNullOrEmpty(value) 
                    ? DateTime.UtcNow.AddHours(8).AddDays(1).ToString("yyyy-MM-ddT00:00:00+08:00") 
                    : Convert.ToDateTime(value).ToString("yyyy-MM-ddT00:00:00+08:00");
            }
            get {
                return _endTime;
            }
        }

        /// <summary>
        /// 1:结算 ，0：未结算, -1:全部
        /// </summary>
        public int settled { set; get; } = -1;


        /// <summary>
        /// 货种类型
        /// </summary>
        public int currencyType { set; get; } = 356;
        /// <summary>
        /// 注单id
        /// </summary>
        [MaxLength(50)]
        public string betId { set; get; } = "";
    }


    /// <summary>
    /// 会员注单报表
    /// </summary>
    public class QueryOrdersReportInputModel : PageBaseVModel
    {
        private string _startTime;
        /// <summary>
        /// 开始时间
        /// </summary>
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string startTime
        {
            set
            {
                _startTime = string.IsNullOrEmpty(value) ? DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd 00:00:00") : value;
            }
            get
            {
                return _startTime;
            }
        }
        private string _endTime;
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string endTime
        {
            set
            {
                _endTime = string.IsNullOrEmpty(value) ? DateTime.UtcNow.AddHours(8).AddDays(1).ToString("yyyy-MM-dd 00:00:00") : value;
            }
            get
            {
                return _endTime;
            }
        }

        /// <summary>
        /// 0.注单创建时间 1.注单结算时间
        /// </summary>
        public int TimeType { set; get; } = -1;
        /// <summary>
        /// 最低总投注金额（目前用来查询最低总投注金额>=这个值的会员）
        /// </summary>
        public decimal betAmount { set; get; } = 0;
        /// <summary>
        /// 货种类型
        /// </summary>
        public int currencyType { set; get; } = 356;

    }

}
