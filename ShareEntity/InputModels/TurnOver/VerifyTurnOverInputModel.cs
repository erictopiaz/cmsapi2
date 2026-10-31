using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.TurnOver
{
    public class VerifyTurnOverInputModel : AccountModel
    {
        public string ids { set; get; }

        [SqlInjectAttibute(IsAllowedEmpty = true, ErrorMessage = "reason_format_error")]
        public string cancelreason { set; get; }
    }
}
