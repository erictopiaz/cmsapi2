using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.agent
{
    public class CancelAgentBillInputModel : AccountModel
    {
        public long id { set; get; }

        [ChineseAndChar(50,0,IsAllowedEmpty =true,ErrorMessage ="remark_format_error")]
        public string cancelRemarks { set; get; }
    }
}
