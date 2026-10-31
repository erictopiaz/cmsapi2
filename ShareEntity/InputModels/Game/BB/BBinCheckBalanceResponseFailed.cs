using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.BB
{
    public class BBinCheckBalanceResponseFailed
    {
        public bool result { set; get; }
        public BBinResultDataFailed data { set; get; }
    }
    /// <summary>
    /// 当前账号取余额操作失败返回
    /// </summary>
    public class BBinResultDataFailed
    {
        public string code { set; get; }
        public string message { set; get; }
    }

}
