//using CMS.Data.ClientModel;
using ShareEntity.ClientModel;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Game
{
    public class UpdateGamePasswordInputModel
    {
        [CharAndDigital(12, MinimumLength = 6, IsAllowedEmpty = false, ErrorMessage = "game_account_formart_error")]
        public string account { set; get; }

        [CharAndDigital(12, MinimumLength = 6, IsAllowedEmpty = true, ErrorMessage = "game_password_formart_error")]
        public string password { set; get; }

        public int gameid { set; get; }
    }

    public class GameBalanceInputModel: IPInputModel
    {
        [CharAndDigitalAndUnderScore(16, MinimumLength = 6, IsAllowedEmpty = false, ErrorMessage = "game_account_formart_error")]
        public string account { set; get; }

        public int gameid { set; get; }
    }
    public class CreateGameAccountInputModel
    {
        [CharAndDigital(12, MinimumLength = 6, IsAllowedEmpty = false, ErrorMessage = "game_account_formart_error")]
        public string account { set; get; }

        [CharAndDigital(12, MinimumLength = 6, IsAllowedEmpty = true, ErrorMessage = "game_password_formart_error")]
        public string password { set; get; }

        public int gameid { set; get; }
    }
}
