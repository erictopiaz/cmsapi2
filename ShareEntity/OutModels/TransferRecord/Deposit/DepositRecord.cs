using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.TransferRecord.Deposit
{
    
    /// <summary>
    /// 充值记录
    /// </summary>
    public class DepositRecord : RecordBase
    {
        /// <summary>
        /// 定单号
        /// </summary>
        public string merchantno { set; get; }
        /// <summary>
        /// 存款通道
        /// </summary>
        public string name { set; get; }

        public int payplatform { set; get; }

        private int _status;
        public int status
        {
            set
            {
                statusDesc = value == 0 ? "等待处理" : value == 1 ? "处理完成" : "失败";
                _status = value;
            }
            get { return _status; }
        }

        /// <summary>
        /// 手续费
        /// </summary>
        public decimal fee { set; get; }
        /// <summary>
        /// 反点
        /// </summary>
        public decimal rebate { set; get; }

        /// <summary>
        /// 虚拟币金额和对应的汇率
        /// </summary>
        public DepositMiscModel cryptoModel { set; get; }
        private string _msic;
        public string misc
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    cryptoModel = new DepositMiscModel();
                }
                else
                {
                    cryptoModel = Newtonsoft.Json.JsonConvert.DeserializeObject<DepositMiscModel>(value);
                }
                _msic = "";
            }
            get { return _msic; }
        }
        /// <summary>
        /// 当前货币符号
        /// </summary>
        public string iconUrl { set; get; } = "";
        /// <summary>
        /// 0或1  0：没有汇率，1：有汇率
        /// 默认为0 , 如果1，显示对应虚拟金额和对应的汇率
        /// </summary>
        public int isSrypto { set; get; } = 0;

        private int _paycategory;
        /// <summary>
        /// 渠道类别
        /// </summary>
        public int paycategory
        {
            set
            {
                _paycategory = value;
                isSrypto = (value == 7 || value == 13) ? 1 : 0;//7:虚拟币或 13 是c2c钱包时
            }
            get { return _paycategory; }
        }
    }

}
