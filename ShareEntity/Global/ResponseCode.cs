using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.Global
{
    /// <summary>
    /// API 返回状态代码
    /// </summary>
    public class ResponseCode
    {
        public enum code { 
            FAIL = 0,
            SUCCESS = 1
        }
    }
}
