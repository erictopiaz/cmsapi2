//using CMS.Data.PageModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.GameReport
{
    public class QueryGameReportInputModel
    {
        [CharAndDigital(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "player_account_format_error")]

        public string account { set; get; }
        public int gameId { set; get; }
        /// <summary>
        /// 0:盈利降序， 1：盈利升序, -1:时间排序
        /// </summary>
        public int sortType { set; get; }
        private string _startTime;
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string startTime
        {
            set
            {
                _startTime = string.IsNullOrEmpty(value) ? DateTime.UtcNow.AddHours(8).AddDays(-1).ToString("yyyy-MM-dd 00:00:00") : value;
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
                _endTime = string.IsNullOrEmpty(value) ? DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd 00:00:00") : value;
            }
            get
            {
                return _endTime;
            }
        }
        public int pageSize { set; get; }
        public int pageIndex { set; get; }
    }

    public class QueryGameReportBaseOutModel
    {
       
        public string begindate { set; get; }
        public string gamename2 { set; get; }
        public decimal totalBets { set; get; }
        public decimal winlost { set; get; }
        public string createtime { set; get; }
        public string macaddress { set; get; }
        public string cardaddress { set; get; }
        private string _withdrawcards;
        public string withdrawcards
        {
            set
            {
                try
                {

                    if (string.IsNullOrEmpty(value))
                        cardaddress = "";
                    var cardInfos = value.Split('|');
                    var addrs = string.Empty;
                    foreach (var c in cardInfos)
                    {
                        var address = c.Split(',')[2];
                        addrs = string.IsNullOrEmpty(addrs) ? address : string.Format(@"{0}__{1}", addrs, address);
                    }
                    cardaddress = addrs;
                }
                catch (Exception)
                {
                    cardaddress = "";
                }
                _withdrawcards = "";
            }
            get
            {
                return _withdrawcards;
            }
        }
        public decimal wdamount { set; get; }
    }
    /// <summary>
    /// 当账号为电话号码时，允许查看到电话号的账号
    /// </summary>
    public class QueryGameReportOutModel : QueryGameReportBaseOutModel
    {
        public string account { set; get; }

    }
   
}
