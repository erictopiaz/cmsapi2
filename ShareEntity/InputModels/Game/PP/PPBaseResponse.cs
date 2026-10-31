using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.PP
{
    public class PPBaseResponse
    {
        public int error {  get; set; }
        public string description { get; set; }
        public string playerId { get; set; } = "";
    }


    //{
    // "error": "0",
    // "description": "OK",
    // "playerId": 6749178
    //}

    public class PPBalanceResponse: PPBaseResponse
    {
        public decimal balance { get; set; } = 0;
    }


    public class PPLoginResponse : PPBaseResponse
    {
        public string gameURL { get; set; } = "";
    }


}
