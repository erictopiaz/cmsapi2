using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.Turbo {
    public class TurboCreateUserResponse {
        /// <summary>
        /// 1000:user existed
        /// 0: 成功
        /// </summary>
        public int code {  get; set; }
        public string msg { get; set; }

    }

    public class TurboLoginResponse : TurboCreateUserResponse {
        public string data { get; set; }
    }
}
