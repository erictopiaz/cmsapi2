using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.TransferRecord
{
    /// <summary>
    /// 交易记录基本类型 
    /// requesttime "yyyy-MM-dd HH:mm:ss"
    /// </summary>
    public class RecordBase
    {
        private string _requesttime;
        /// <summary>
        /// 申请时间
        /// </summary>
        public string requesttime {
            set
            {
                _requesttime = string.IsNullOrEmpty(value) ? "" : Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            get { return _requesttime; }
        }
        private string _paytime;
        public string paytime
        {
            set
            {
                _paytime = string.IsNullOrEmpty(value) ? "" : Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            get { return _paytime; }
        }
        private string _endtime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public string endtime
        {
            set
            {
                _endtime = string.IsNullOrEmpty(value) ? "" : Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            get { return _endtime; }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public string statusDesc { set; get; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal amount { set; get; }


        private string _account;
        /// <summary>
        /// 会员账号
        /// </summary>
        public string account
        {
            set
            {
                if (value.ToLower().StartsWith("boaa")
                    || value.ToLower().StartsWith("bocc"))
                {
                    _account = value.Remove(0, 4);
                }
            }
            get { return _account; }
        }
    }

}
