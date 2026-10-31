using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Promo
{
    /// <summary>
    /// 创建优惠活动内容和读取优惠活动内容
    /// </summary>
    public class CreatePromoContentInputModel : AccountModel
    {
        private string _title;
        public string title { 
            set {
                _title = string.IsNullOrEmpty(value) ? "" : value.Replace("'", "''");
            }
            get { return _title; }
        }
        private string _titlesec;
        public string titlesec {
            set {
                _titlesec = string.IsNullOrEmpty(value) ? "" : value.Replace("'", "''");
            }
            get { return _titlesec; }
        }
        private string _promoDetail;
        public string promoDetail {
            set {
                _promoDetail = string.IsNullOrEmpty(value) ? "" : value.Replace("'", "''");
            }
            get { return _promoDetail; }
        }
        private string _promoMember;
        public string promoMember {
            set {
                _promoMember = string.IsNullOrEmpty(value) ? "" : value.Replace("'", "''");
            }
            get { return _promoMember; }
        }
        private string _promoGames;
        public string promoGames {
            set {
                _promoGames = string.IsNullOrEmpty(value) ? "" : value.Replace("'", "''");
            }
            get { return _promoGames; }
        }
        private string _howApplication;
        public string howApplication {
            set {
                _howApplication = string.IsNullOrEmpty(value) ? "" : value.Replace("'", "''");
            }
            get { return _howApplication; }
        }
        
        [UrlValidate(100,MinimumLength =15,IsAllowedEmpty =true,ErrorMessage ="url_format_error")]
        public string picUrl { set; get; }

        private string _picBanner;
        public string picBanner {
            set {
                _picBanner = string.IsNullOrEmpty(value) ? "" : value.Replace("'", "''");
            }
            get { return _picBanner; }
        }
        private string _promobgPic;
        /// <summary>
        /// 优惠内页图
        /// </summary>
        public string promobgPic {
            set {
                _promobgPic = string.IsNullOrEmpty(value) ? "" : value.Replace("'", "''");
            }
            get { return _promobgPic; }
        }

        private string _picPromo;
        public string picPromo {
            set {
                _picPromo = string.IsNullOrEmpty(value) ? "" : value.Replace("'", "''");
            }
            get { return _picPromo; }
        }

        private string _startTime;
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string startTime 
        {  
            set
            {
                _startTime = string.IsNullOrEmpty(value) ? "" : Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            get
            {
                return _startTime;
            }

        }
        private string _endTime;
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string endTime {
            set
            {
                _endTime = string.IsNullOrEmpty(value) ? "" : Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            get
            {
                return _endTime;
            }
        }
        public int bannerStatus { set; get; }

        public int status { set; get; }
        public int promotype { set; get; }
        public int promocatalog { set; get; }
        public int autopromotype { set; get; }
        public int cstatus { set; get; }
        public int device { set; get; }
        /// <summary>
        /// 降序
        /// </summary>
        public int psort { set; get; }
        /// <summary>
        ///  降序
        /// </summary>
        public int bsort { set; get; }
        /// <summary>
        /// 是否要显示优惠的内容，
        /// 这里目前只为那些，只有BANNER 没有优惠内容使用的标识
        /// </summary>
        public int isDisplayPromoContent { set; get; }
        /// <summary>
        /// 游戏平台
        /// </summary>
        public int gametype { set; get; } = -1;

        private string _bannerPromoPath;
        /// <summary>
        /// 首页banner跳转到活动的path
        /// </summary>
        public string bannerPromoPath {
            set {
                _bannerPromoPath = string.IsNullOrEmpty(value) ? "" : value.Replace("'", "''");
            }
            get { return _bannerPromoPath; }
        }

        /// <summary>
        /// 当前优惠活动在网站上的位置
        /// </summary>
        public int promoPos { set; get; }
        /// <summary>
        /// 当前banner类型
        /// 0：普通   1：专题
        /// </summary>
        public int bannerPromoType { set; get; }
        private string _startTimeDesc;
        public string startTimeDesc {
            set {
                _startTimeDesc = string.IsNullOrEmpty(value) ? "" : value.Replace("'", "''");
            }
            get { return _startTimeDesc; }
        }
        private string _body;
        public string body {
            set {
                _body = string.IsNullOrEmpty(value) ? "" : value.Replace("'", "''");
            }
            get { return _body; }
        }

        [CharAndDigitalAndUnderScore(12, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "sys_account_format_error")]
        public string createcustomer { set; get; }

        [CharAndDigitalAndUnderScore(12, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "agent_account_format_error")]
        public string affaccount { set; get; } = "allwincity";
        public long id { set; get; } = 0;
        /// <summary>
        /// 存款优惠列表
        /// 0：非存款优惠列表
        /// 1：存款优惠列表
        /// 2：指定会员存款优惠
        /// </summary>
        public int isDepositListPromo { set; get; } = 0;
        public string vips { set; get; }
        public List<int> vipLevels { set; get; } = new List<int>();

        public int currencyType { set; get; } = -1;

        /// <summary>
        /// 品牌
        /// </summary>
        public string p_merchantAccounts { set; get; } = "-1";
        public int p_walletType { set; get; } = -1;

    }


    /// <summary>
    /// 1月25号-2月1号抽奖活动
    /// </summary>
    public class FreeAmountPromoModel 
    {
        public string startTime { set; get; }

       
        public string endTime { set; get; }
       
        public int status { set; get; }
       
        public int device { set; get; }

        public string title { set; get; }
       
    }

}
