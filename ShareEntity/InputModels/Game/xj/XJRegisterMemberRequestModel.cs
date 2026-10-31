using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.xj
{
    public class XJRegisterMemberRequestModel
    {
        public string memberCode { get; set; }

        public string currencyCode { get; set; }

        public string countryCode { get; set; }

        public int memberStatus { get; set; }

        public string displayName { get; set; }

        public int oddsType { get; set; }
    }
    public class XJRegisterMemberResponeModel
    {
    }
}
