using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace UtilsLib.IPHelper
{
    public class Helper
    {
        
        public static string GetClientInfo(string DataPath, string ipaddress)
        {
            var ipScanner = new IpScanner
            {
                DataPath = string.Format(@"{0}{1}", DataPath, "/App_Data/qqwry.dat"),//HttpContext.Current.Server.MapPath(@"/App_Data/qqwry.dat"),
                IP = ipaddress
            };
            var address = ipScanner.IPLocation();
            if (address.Contains("CZ88.NET"))
                address = address.Replace("CZ88.NET", " ").Trim();
            var macaddress = address;

            
            return macaddress;
        }


    }
}