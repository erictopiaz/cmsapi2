using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game
{
    public class GameRequestBaseParamModel
    {
        public bool wait { set; get; }
        public string method { set; get; }
        public string remote { set; get; }
        public string String_Send { set; get; }
        public string ContentType { set; get; }
    }
}
