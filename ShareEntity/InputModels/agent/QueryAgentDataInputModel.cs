using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.agent
{
    public class QueryAgentDataInputModel : AccountModel
    {
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string startTime { set; get; }
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string endTime { set; get; }

    }

    public class QueryAgentDataReportInputModel : PageBaseModel
    {
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string startTime { set; get; }
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string endTime { set; get; }


    }
}
