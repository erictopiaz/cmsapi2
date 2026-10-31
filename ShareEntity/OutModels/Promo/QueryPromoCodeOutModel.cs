using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.Promo
{
    /// <summary>
    /// 优惠代码查询返回
    /// </summary>
    public class QueryPromoCodeOutModel
    {
        public long tid { set; get; }
        public string account { set; get; }
        public string promocode { set; get; }
        public decimal amount { set; get; }
        public decimal bet { set; get; }
        public decimal lowamount { set; get; }
        public string remark { set; get; }
        public string createtime { set; get; }
        public string expiredate { set; get; }
        public string statusDesc { set; get; }
        private int _status;
        public int status
        {
            set
            {
                statusDesc = value == 0 ? "等待审核" : value == 1 ? "审核成功" : "取消审核";
                _status = value;
            }
            get
            {
                return _status;
            }
        }
    }
}
