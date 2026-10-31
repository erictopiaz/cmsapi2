using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Resize
{
    public class VerifyResizeInputModel : AccountModel
    {
        public long tid { set; get; }

        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "cancelreason_format_error")]
        public string cancelreason { set; get; }
    }
}
