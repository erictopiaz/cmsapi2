using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Pay
{
   
    /// <summary>
    /// 每个平台存款的次数
    /// </summary>
    public class DepositTimesByPlatform
    {
        public int payplatform { set; get; }
        public int times { set; get; }

    }
}
