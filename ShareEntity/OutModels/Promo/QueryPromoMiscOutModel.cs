using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.Promo
{
    public class QueryPromoMiscOutModel
    {
        public long tid { set; get; }
        public string account { set; get; }
        public decimal points { set; get; }
        public decimal left_points { set; get; }
        public string remark { set; get; }
        public string createtime { set; get; }
        public string statusDesc { set; get; }
        private int _status;
        public int status
        {
            set
            {
                statusDesc = value == 0 ? "新增" : "扣除";
                _status = value;
            }
            get
            {
                return _status;
            }
        }
        public string rowstatusDesc { set; get; }
        private int _rowstatus;
        public int rowstatus
        {
            set
            {
                rowstatusDesc = value == 0 ? "等待审核" : value == 1 ? "审核成功" : "取消审核";
                _rowstatus = value;
            }
            get
            {
                return _rowstatus;
            }
        }
        public string createperson { set; get; }
        public string anomaly { set; get; }
        public int riskTypesAready { set; get; }
        public string riskRemark { set; get; }
        public long uid { set; get; }
        /// <summary>
        /// 会员的品牌
        /// </summary>
        public string merchantAccount { set; get; } = "-1";
        public int walletType { set; get; } = -1;

    }
}
