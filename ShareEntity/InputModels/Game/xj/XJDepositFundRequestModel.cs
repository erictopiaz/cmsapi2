using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.xj
{
    public class XJDepositFundRequestModel
    {
        public string memberCode { get; set; }
        public string balance { get; set; }
        public string currencyCode { get; set; }

        public string refId { get; set; }
    }

    public class XJDepositFundResponseModel
    {
        public string transferId { get; set; }

        public decimal totalBalance { get; set; }
    }
}
