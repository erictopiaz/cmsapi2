//using CMS.Data.InputModels.Promo;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.Promo
{
    public class QueryPromoContentOutModel
    {
        public long id { set; get; }
        public string title { set; get; }
        public string titleSec { set; get; }
        public string picUrl { set; get; }
        public string startTime { set; get; }
        public string endTime { set; get; }
        public string statusDesc { set; get; }
        private int _status;
        public int status
        {
            set
            {
                statusDesc = value == 0 ? "停用" : "启用";
                _status = value;
            }
            get
            {
                return _status;
            }
        }

        public string createcustomer { set; get; }
        public string promoCatalogDesc { set; get; }
        private int _promoCatalog { set; get; }
        public int promoCatalog {
            set
            {
                promoCatalogDesc = value == 0 ? "普通优惠" : "自助优惠";
                _promoCatalog = value;

            }
            get
            {
                return _promoCatalog;
            }
        }
        private string _gamename2;
        public string gamename2 {
            set
            {
                _gamename2 = string.IsNullOrEmpty(value) ? "未指定平台" : value;
            }
            get
            {
                return _gamename2;
            }
        }

        /// <summary>
        /// 会员的品牌
        /// </summary>
        public string merchantAccount { set; get; } = "-1";
        public int walletType { set; get; } = -1;
    }
}
