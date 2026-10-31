using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.website
{
    public class QueryMerchantWhereModel
    {
        public long id { set; get; }
        /// <summary>
        /// 回复的内容
        /// </summary>
        public int days { get; set; }
        /// <summary>
        /// 是否开启查询商户的条件限制
        /// 1：启用，0：停用
        /// </summary>
        public int status { set; get; }

        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string Admin { get; set; } = null;
    }
}
