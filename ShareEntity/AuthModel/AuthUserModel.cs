using ShareEntity.InputModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.AuthModel
{
    public class AuthUserModel : AccountModel
    {
        public string password { set; get; }
    }
}
