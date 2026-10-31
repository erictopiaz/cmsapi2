using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.BB
{
    public class BBinCreateMemberResponse
    {
        public bool result { set; get; }
        public CreateMemberData data { set; get; }
    }

    public class CreateMemberData
    {
        public string Code { set; get; }
        public string Message { set; get; }

        public string sessionid { set; get; }
    }
}
