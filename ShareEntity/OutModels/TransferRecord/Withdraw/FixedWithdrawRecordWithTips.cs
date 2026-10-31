using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.TransferRecord.Withdraw
{
    public class FixedWithdrawRecordWithTips
    {
        public string topTips { set; get; }
        public List<string> bottomWarnings { set; get; }
        public List<FixedWithdrawRecord> records { set; get; }
    }

    public class FixedWithdrawRecordWithTip
    {
        public string topTips { set; get; }
        public List<string> bottomWarnings { set; get; }
        public FixedWithdrawRecord records { set; get; }
    }
}
