using CMS.Data.Models;
using Dapper.FluentMap.Mapping;
using ShareEntity.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CMS.Core.DapperMapper
{
    public class ofThirdpayMap : EntityMap<ofThirdpay> {
        public ofThirdpayMap() {
            Map(u => u._class).ToColumn("class");
        }

    }
    public class ofNetBankMap : EntityMap<ofNetbank> {
        public ofNetBankMap() {
            Map(u => u.classtype).ToColumn("class");
        }

    }

}
