using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace UtilsLib.XmlHelper
{
    public class XmlHelper
    {
        /// <summary>
        /// 检查当前xml否合法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool CheckXmlFormatValidata(string data)
        {
            XmlDocument Doc = new XmlDocument();
            try
            {
                Doc.LoadXml(data);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 获取第一个节点
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string QueryXmlNodeValue(string data)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;
                doc.LoadXml(data);
                XmlNode root = doc.FirstChild;
                return doc.LastChild.InnerText;
            }
            catch (Exception)
            {

                return "";
            }
        }
    }

    public class IgnoreNamespaceXmlTextReader : XmlTextReader {
        public IgnoreNamespaceXmlTextReader(TextReader reader) : base(reader) { }

        public override string NamespaceURI => "";  // Ignores namespaces
    }
}
