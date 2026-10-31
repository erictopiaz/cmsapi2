//using CMS.Data.ClientModel;
using ShareEntity.ClientModel;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Game
{
    public class GameTransferInputModel : IPInputModel
    {
        [CharAndDigital(12, MinimumLength = 6, IsAllowedEmpty = false, ErrorMessage = "game_account_formart_error")]
        public string account { set; get; }

        [CharAndDigital(12, MinimumLength = 6, IsAllowedEmpty = true, ErrorMessage = "game_password_formart_error")]
        public string password { set; get; }

        public int gameid { set; get; }
        /// <summary>
        /// 游戏转账额度
        /// </summary>
        public int amount { set; get; }
        /// <summary>
        /// 转账代码
        /// </summary>
        public int transferType { set; get; }
        /// <summary>
        /// 转账的优惠代码
        /// </summary>
        [CharAndDigital(2, MinimumLength = 1, IsAllowedEmpty = true, ErrorMessage = "优惠代码要求1到2个字符")]
        public string promotionCode { set; get; } = "0";
    }
}
