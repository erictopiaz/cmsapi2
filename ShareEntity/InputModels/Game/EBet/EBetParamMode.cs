using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.EBet
{
    public class EBetParamMode
    {
        //{ "apiurl":"http://awcbet.ebet.im:8888",
        //"Channel_Id":"1406",
        //"Public_Key":"",
        //"Private_Key":"",
        //"prefix":"",
        //"gamestatus":1}
        public string Channel_Id { set; get; }
        public string Public_Key { set; get; }
        public string apiUrl { set; get; }
        public string apiGameUrl { set; get; }
        public string Private_Key { set; get; }
        public string prefix { set; get; }
        public int gamestatus { set; get; }
        public string gameName { set; get; } = "EB";
    }
}
