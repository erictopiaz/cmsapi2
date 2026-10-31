using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.Transfer
{
    /// <summary>
    /// 转账记录查询输出结果
    /// </summary>
    public class QueryTransferOutModel 
    {
        public long id { set; get; }
        public string account { set; get; }
        public string amount { set; get; }
        public string requesttime { set; get; }
        public int gameid { set; get; }

        public string gamename { set; get; }
        public string leftamountbefore { set; get; }
        public string leftamountafter { set; get; }
        public string rightamountbefore { set; get; }
        public string rightamountafter { set; get; }
        public string promotioncode { set; get; }
        public string status { set; get; }
        public string affaccount { set; get; }
    }

    /// <summary>
    /// 查看转账详情
    /// </summary>
    public class ViewTransferOutModel : QueryTransferOutModel
    {

    }
}
