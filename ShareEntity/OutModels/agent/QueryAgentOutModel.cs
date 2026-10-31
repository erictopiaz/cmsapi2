using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.agent
{
    public class QueryAgentOutModel
    {
        public long tid { set; get; }
        public string account { set; get; }
        public string birthday { set; get; }
        public string website { set; get; }
        public string adcode { set; get; }
        public string registerUrl { set; get; }
        public string ipaddress { set; get; }
        public string verifyperson { set; get; }
        public string statusDesc { set; get; }
        private int _status;
        public int status
        {
            set
            {
                statusDesc = value == 1 ? "已启用" : "未启用";
                _status = value;
            }
            get
            {
                return _status;
            }
        }
        public int id { set; get; }
        public int pId { set; get; }
        public string agentLevelDesc { set; get; }
        private string _isLeaf;
        public string isLeaf { 
            set
            {
                agentLevelDesc = value.Equals("1") ? "有" : "无";
                _isLeaf = value.Equals("1") ? "1" : "0";
            }
            get
            {
                return _isLeaf;
            }
        }
        public string adlink { set; get; }
        public string verifyDesc { set; get; }
        private int _verify;
        public int verify
        {
            set
            {
                verifyDesc = value == 1 ? "审核通过" : value == 2 ? "已作废" : "等待审核";
                _verify = value;
            }
            get
            {
                return _verify;
            }
        }
        public string createtime { set; get; }
        public string parentAccount { set; get; }
        public string lastLoginDate { set; get; }

        public string currency { set; get; }
        /// <summary>
        /// 会员的品牌
        /// </summary>
        public string merchantAccount { set; get; } = "-1";
        public int walletType { set; get; } = -1;

    }
}
