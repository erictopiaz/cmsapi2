using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.agent
{
    public class AgentDisableOrEnableInputModel : AccountModel
    {
        public int status { set; get; }
    }
}
