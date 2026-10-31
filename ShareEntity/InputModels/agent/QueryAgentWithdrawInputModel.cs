//using CMS.Data.PageModel;
using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.agent
{
    public class QueryAgentWithdrawInputModel : PageBaseModel
    {
        [ChineseAndChar(12,0,IsAllowedEmpty =true,ErrorMessage ="name_format_error")]
        public string realname { set; get; }

        [BankCardNo(19,MinimumLength =16,IsAllowedEmpty =true,ErrorMessage ="cardno_format_error")]
        public string cardno { set; get; }
        public int status { set; get; } = -1;

        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string startTime { set; get; }

        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string endTime { set; get; }

        public int currencyType { set; get; } = 156;
    }
}
