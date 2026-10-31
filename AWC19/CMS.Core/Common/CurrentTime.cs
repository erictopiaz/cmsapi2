using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Core
{
    public static partial class Common
    {
        public static DateTime CurrentTime {get {return DateTime.UtcNow.AddHours(8);}}
        public static string TimeStamp { get
            {
                return CurrentTime.ToString("yyyyMMddHHmmssffff");
            }
        }
    }
}
