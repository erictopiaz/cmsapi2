//using CMS.Data.ClientModel;
using ShareEntity.ClientModel;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Transfer
{
    public class CancleTransferInputModel : IPInputModel
    {
        public long transferId { set; get; }

        [CharAndDigital(12, MinimumLength = 6, IsAllowedEmpty = false, ErrorMessage = "agent_account_format_error")]
        public string adminAccount { set; get; }
    }
}
