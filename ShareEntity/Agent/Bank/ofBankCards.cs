using ShareEntity.InputModels.agent;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.Agent.Bank
{
    /// <summary>
    /// 绑定的银行卡信息
    /// </summary>
    public class ofBankCards : CardBase
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string account { set; get; }

        /// <summary>
        /// 谁绑卡
        /// </summary>
        public string who_bind { set; get; } = "";
        /// <summary>
        /// 默认绑定后可以使用
        /// </summary>
        public int cardStatus { set; get; } = 1;

        ///// <summary>
        ///// 默认银行代码
        ///// </summary>
        //public string bankCode { set; get; } = "";

        /// <summary>
        /// 账号类型 0:会员账号，1：代理账号
        /// </summary>
        public int accountType { set; get; } = 1;

        public string requestip { set; get; } = "";



        /// <summary>
        /// 代理姓名
        /// </summary>
        public string realname { set; get; }
        /// <summary>
        /// 0:rmb, 1:usdt, 2:btc
        /// </summary>
        public int category { set; get; }

        /// <summary>
        /// 代理的审查人员
        /// </summary>
        public string verifyperson { set; get; }
    }

}
