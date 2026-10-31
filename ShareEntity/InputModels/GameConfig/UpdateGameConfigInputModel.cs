using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.GameConfig
{
    public class UpdateGameConfigInputModel
    {
        public long id { set; get; }
        public int gameStatus { set; get; }

        [CharAndDigital(12, MinimumLength = 6, IsAllowedEmpty = true, ErrorMessage = "agent_account_format_error")]
        public string agentAccount { set; get; }
    }
}
