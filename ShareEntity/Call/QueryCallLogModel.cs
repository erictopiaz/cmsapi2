//using CMS.Data.PageModel;
using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.Call
{
    /// <summary>
    /// 电销记录查询输入
    /// </summary>
    public class QueryCallLogModel: PageBaseModel
    {
        /// <summary>
        /// 备注状态 0:已经备注，1：已分配
        /// </summary>
        public int status { set; get; } = -1;
        /// <summary>
        /// 通话状态  0：未接通，1：接通, -1:分配状态
        /// </summary>
        public int contactStatus { set; get; } = -2;
        /// <summary>
        /// 负责人
        /// </summary>
        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "person_format_error")]
        public string createperson { get; set; }

        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string starttime { set; get; }
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string endtime { set; get; }
    }
}
