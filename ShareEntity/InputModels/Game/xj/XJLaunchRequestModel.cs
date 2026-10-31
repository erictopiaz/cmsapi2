using ShareEntity.InputModels.Game.xj.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.xj
{
    public class XJLaunchRequestModel
    {

        public string token { get; set; }
        public GameType gameType { get; set; }
        public DistributionChannel channel { get; set; }
        public string lobbyUrl { get; set; }

        public string logoutUrl { get; set; }
        public LaunchType launchType { get; set; }

        public string langCode { get; set; }
    }

    public class XJAPILaunchModel
    {

        public string token { get; set; }
        public object gameType { get; set; }
        public object channel { get; set; }
        public string lobbyUrl { get; set; }

        public string logoutUrl { get; set; }

        public int launchType { get; set; }

        public string langCode { get; set; }
    }

    public class XJLaunchResponseModel
    {

        public string memberCode { get; set; }

        public string currencyCode { get; set; }

        public string timeZone { get; set; }

        public int memberStatus { get; set; }

        public string merchantId { get; set; }

        public string countryCode { get; set; }

        public int oddsType { get; set; }
    }
}
