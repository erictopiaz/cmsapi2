using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.Resize
{
    /// <summary>
    /// 微调返回
    /// </summary>
    public class QueryResizeOutModel
    {
        public long tid { set; get; }
        public string account { set; get; }
        public decimal amount { set; get; }
        public string remarks { set; get; }
        public string createtime { set; get; }
        public string reviewtime { set; get; }
        public string createperson { set; get; }
        public string reviewperson { set; get; }
        public string cancelreason { set; get; }
        public string statusDesc { set; get; }
        private int _status;
        public int status {
            set
            {
                statusDesc = value == 1 ? "审核成功" : value == 0 ? "等待审核" : "取消审核";
                _status = value;
            }
            get
            {
                return _status;
            }
        }
        public string decreasetypeDesc { set; get; }
        private int _decreasetype;
        /// <summary>
        /// 操作类型
        /// 0：扣
        /// 1： 新增
        /// 2: 冻结
        /// </summary>
        public int decreasetype {
            set
            {
                decreasetypeDesc = value == 1 ? "新增" : value == 2 ? "冻结": "扣除";
                _decreasetype = value;
            }
            get
            {
                return _decreasetype;
            }
        }
        public string amountTypeDesc { set; get; }
        private int _amounttype;
        /// <summary>
        /// 扣除的钱包
        /// </summary>
        public int amountType
        {
            set
            {
                amountTypeDesc = value == 0 ? "可用余额" : "累积余额";
                _amounttype = value;
            }
            get
            {
                return _amounttype;
            }
        }
        /// <summary>
        /// 代理id
        /// </summary>
        public long aid { set; get; }
        public int anomaly { set; get; }
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
