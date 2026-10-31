using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.agent
{
    public class QueryAgentDataOutModel
    {
        /// <summary>
        /// 注册人员数量
        /// </summary>
        public int registerNumber { set; get; }
        /// <summary>
        /// 首存人员数量
        /// </summary>
        public int firstDepositNumber { set; get; }

       /// <summary>
       /// 总存款
       /// </summary>
        public decimal depositTotal { set; get; }
        /// <summary>
        /// 总提款
        /// </summary>
        public decimal withdrawTotal { set; get; }
        /// <summary>
        /// 访问数量
        /// </summary>
        public int visterTimes { set; get; }
        public int depositUserTotal { set; get; }

        public string levelDesc { set; get; }
        private int _isLeaf;
        /// <summary>
        /// 1: 有子
        /// 0：无子
        /// </summary>
        public int isLeaf
        {
            set
            {
                levelDesc = value == 1 ? "双击展开" : "";
                _isLeaf = value;
            }
            get
            {
                return _isLeaf;
            }
        }
        public string account { set; get; }
        public int tid { set; get; }
        public int reportDay { set; get; }
    }
}
