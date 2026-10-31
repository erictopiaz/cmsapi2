using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.agent
{
    public class affMonthReportFinanceOutModel : FinanceReportBase
    {
        /// <summary>
        /// 报表月份
        /// </summary>
        public long financeMonth { set; get; }
    }
}
