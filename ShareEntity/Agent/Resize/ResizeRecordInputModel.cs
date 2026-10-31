using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.Agent.Resize
{
    /// <summary>
    /// 公司奖励查询输入数据
    /// </summary>
    public class ResizeRecordInputModel: PageBaseModel
    {
        /// <summary>
        /// 开始时间 yyyy-MM-dd 00:00:00
        /// </summary>
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "开始时格式有误")]
        public string startTime { set; get; }
        /// <summary>
        /// 开始时间 yyyy-MM-dd 00:00:00
        /// </summary>
        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "结束时间格式有误")]
        public string endTime { set; get; }

        /// <summary>
        /// 0：等待处理，1：处理完成，2：取消
        /// </summary>
        public int status { set; get; } = -1;
    }
}
