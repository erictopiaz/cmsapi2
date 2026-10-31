using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace UtilsLib.SqlInject
{
    public class ExpressSqlInject
    {
        public ExpressSqlInject()
        {

        }
        /// <summary>
        /// 检查IP地址是否正确
        /// </summary>
        /// <param name="IPAddr"></param>
        /// <param name="minLen">日期字串长度要求最小值</param>
        /// <param name="maxLen">日期字串长以要求最大值</param>
        /// <returns></returns>
        public static bool IPAddress(string IPAddr, int minLen, int maxLen)
        {
            if (IPAddr == null || IPAddr == string.Empty || IPAddr.Length < minLen || IPAddr.Length > maxLen) return false;
            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";
            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(IPAddr);
        }
        /// <summary>
        /// 检查仅为字母和数字
        /// </summary>
        /// <param name="Str"></param>
        /// <param name="MinLength"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public static bool CharAndDigital(string Str, int MinLength, int MaxLength)
        {
            bool bReturnValue = false;
            if (Str.Length < MinLength || Str.Length > MaxLength) 
                return false;
            string sRxStr = @"^\w+$";
            Regex sRx = new Regex(sRxStr, RegexOptions.IgnoreCase);
            bReturnValue = sRx.IsMatch(Str, 0);
            return bReturnValue;
        }
        /// <summary>
        /// 检查仅为字母和数字_
        /// </summary>
        /// <param name="Str"></param>
        /// <param name="MinLength"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public static bool CharAndDigitalAndUnderScore(string Str, int MinLength, int MaxLength)
        {
            bool bReturnValue = false;
            if (Str.Length < MinLength || Str.Length > MaxLength) return false;
            //string sRxStr = @"^\w+$";
            string sRxStr = @"^[0-9a-zA-Z_]*$";
            Regex sRx = new Regex(sRxStr, RegexOptions.IgnoreCase);
            bReturnValue = sRx.IsMatch(Str, 0);
            return bReturnValue;
        }

        /// <summary>
        /// 检查仅为汉字、字母、数字
        /// </summary>
        /// <param name="Str"></param>
        /// <param name="type">0:只为汉字,1:只为字母,2:字母或数字,3:检查仅为汉字或字母或数字,4:检查仅为汉字或字母或数字或空格</param>
        /// <param name="MinLength"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public static bool ChineseAndChar(string Str, int type, int MinLength, int MaxLength)
        {
            bool bReturnValue = false;
            if (Str.Length < MinLength || Str.Length > MaxLength) return false;
            string sRxStr = string.Empty;
            switch (type)
            {
                case 0: sRxStr = @"^[\u4e00-\u9fa5]+$"; break;
                case 1: sRxStr = @"^[A-Za-z]+$"; break;
                case 2: sRxStr = @"^\w+$"; break;
                case 3: sRxStr = @"^[A-Za-z0-9\u4e00-\u9fa5]+$"; break;
                case 4: sRxStr = @"^[A-Za-z0-9\u4e00-\u9fa5 ]+$"; break;
                default: break;

            }
            Regex sRx = new Regex(sRxStr, RegexOptions.IgnoreCase);
            bReturnValue = sRx.IsMatch(Str, 0);
            return bReturnValue;
        }
        /// <summary>
        /// 检查邮箱
        /// </summary>
        /// <param name="Str"></param>
        /// <param name="MinLength"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public static bool EmailValidate(string Str, int MinLength, int MaxLength)
        {
            bool bReturnValue = false;
            if (Str.Length < MinLength || Str.Length > MaxLength) return false;
            string sRxStr = @"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$";
            Regex sRx = new Regex(sRxStr, RegexOptions.IgnoreCase);
            bReturnValue = sRx.IsMatch(Str, 0);
            return bReturnValue;
        }
        /// <summary>
        /// 检查数字
        /// </summary>
        /// <param name="Str"></param>
        /// <param name="MinLength"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public static bool DigitalValidate(string Str, int MinLength, int MaxLength)
        {
            bool bReturnValue = false;
            string sRxStr = @"^\d{" + MinLength.ToString() + "," + MaxLength.ToString() + "}$";
            Regex sRx = new Regex(sRxStr, RegexOptions.IgnoreCase);
            bReturnValue = sRx.IsMatch(Str, 0);
            return bReturnValue;
        }
        /// <summary>
        /// 检查银行卡
        /// </summary>
        /// <param name="Str"></param>
        /// <param name="MinLength"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public static bool BankCardValidate(string Str, int MinLength, int MaxLength)
        {
            bool bReturnValue = false;
            string[] RxStr = new string[] { @"^\d{12}$", @"^\d{14}$", @"^\d{15}$", @"^\d{16}$", @"^\d{17}$", @"^\d{18}$", @"^\d{19}$" };
            int Len = RxStr.Length;
            for (int i = 0; i < Len; i++)
            {
                Regex sRx = new Regex(RxStr[i], RegexOptions.IgnoreCase);
                bReturnValue = sRx.IsMatch(Str, 0);
                if (bReturnValue)
                {
                    break;
                }
            }
            return bReturnValue;
        }
        /// <summary>
        /// 检查电话号码
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns></returns>
        public static bool PhoneDigital(string Phone)
        {
            bool bReturnValue = false;
            string[] RxStr = new string[] { "^[0-9]{8}$", "^(11[0-9]{9})$", "^(12[0-9]{9})$", "^(13[0-9]{9})$", "^(14[0-9]{9})$", "^(15[0-9]{9})$", "^(16[0-9]{9})$", "^(17[0-9]{9})$", "^(18[0-9]{9})$", "^(19[0-9]{9})$", "^([0-9]{4}-[0-9]{8})$", "^([0-9]{3}-[0-9]{8})$", "^([0-9]{4}-[0-9]{7})$", "^[0-9]{7}$", @"^0(([1-9]\d)|([3-9]\d{2}))\d{8}$" };
            int Len = RxStr.Length;
            for (int i = 0; i < Len; i++)
            {
                Regex sRx = new Regex(RxStr[i], RegexOptions.IgnoreCase);
                bReturnValue = sRx.IsMatch(Phone, 0);
                if (bReturnValue)
                {
                    break;
                }
            }
            return bReturnValue;
        }

        /// <summary>
        /// 检查数据字串是否为Int
        /// 如果是并返回其int value
        /// </summary>
        /// <param name="inputInt">int字串</param>
        /// <param name="outInt">转换后的int</param>
        /// <returns></returns>
        public static bool IntValidate(string inputInt, out int outInt)
        {
            return int.TryParse(inputInt, out outInt);
        }
        /// <summary>
        /// 检查数据字串是否为byte
        /// 如果是并返回其byte value
        /// </summary>
        /// <param name="inputInt"></param>
        /// <param name="outInt"></param>
        /// <returns></returns>
        public static bool ByteValidate(string inputInt, out byte outInt)
        {
            return byte.TryParse(inputInt, out outInt);
        }
        /// <summary>
        /// 检查数据字串是否为byte
        /// 如果是并返回其byte value
        /// </summary>
        /// <param name="inputInt"></param>
        /// <param name="outInt"></param>
        /// <returns></returns>
        public static bool ByteValidate(string inputInt, out byte outInt, byte minlen, byte maxlen)
        {
            outInt = 0;
            if (inputInt.Length < minlen || inputInt.Length > maxlen) return false;
            return byte.TryParse(inputInt, out outInt);
        }
        /// <summary>
        /// 检查数据字串是否为Int
        /// 如果是并返回其int value
        /// </summary>
        /// <param name="inputInt">int字串</param>
        /// <param name="outInt">转换后的int</param>
        /// <returns></returns>
        public static bool IntValidate(string inputInt, out int outInt, int minlen, int maxlen)
        {
            outInt = 0;
            if (inputInt.Length < minlen || inputInt.Length > maxlen) return false;
            return int.TryParse(inputInt, out outInt);
        }
        /// <summary>
        /// 检查数据字串是否为Decimal
        /// 如果是并返回其decimal value
        /// </summary>
        /// <param name="inputDecimal">decimal字串</param>
        /// <param name="outDecimal">转换后的decimal</param>
        /// <returns></returns>
        public static bool DecimalValidate(string inputDecimal, out decimal outDecimal)
        {
            outDecimal = 0.0m;
            return decimal.TryParse(inputDecimal, out outDecimal);
        }
        /// <summary>    
        /// 检测是否含有危险字符（防止Sql注入）    
        /// </summary>    
        /// <param name="contents">预检测的内容</param>    
        /// <returns>返回True或false</returns>    
        public static bool HasInJectKeywords(string contents)
        {
            bool bReturnValue = false;
            if (!string.IsNullOrEmpty(contents))
            {
                string sLowerStr = contents.ToLower();
                string sRxStr = @"(\sand\s)|(\sand\s)|(\slike\s)|(select\s)|(insert\s)|(delete\s)|(update\s[\s\S].*\sset)|(create\s)|(\stable)|(<[iframe|/iframe|script|/script])|(')|(\sexec)|(declare)|(\struncate)|(\smaster)|(\sbackup)|(\smid)|(\scount)|(cast)|(\sadd\s)|(\salter\s)|(\sdrop\s)|(\sfrom\s)|(\struncate\s)|(\sxp_cmdshell\s)";
                Regex sRx = new Regex(sRxStr, RegexOptions.IgnoreCase);
                bReturnValue = sRx.IsMatch(sLowerStr, 0);
            }
            return bReturnValue;
        }
        /// <summary>
        /// 过滤标记
        /// </summary>
        /// <param name="NoHTML">包括HTML，脚本，数据库关键字，特殊字符的源码 </param>
        /// <returns>已经去除标记后的文字</returns>
        public static string FilterHtmlInject(string Htmlstring)
        {
            if (Htmlstring == null)
            {
                return "";
            }
            else
            {
                //删除脚本
                Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
                //删除HTML
                Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
                //删除转移字符
                Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "xp_cmdshell", "", RegexOptions.IgnoreCase);

                //删除与数据库相关的词
                Htmlstring = Regex.Replace(Htmlstring, "select", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "insert", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "delete from", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "count''", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "drop table", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "truncate", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "asc", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "mid", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "char", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "xp_cmdshell", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "exec master", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "net localgroup administrators", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, " and", "", RegexOptions.IgnoreCase);
                return Htmlstring;
            }
        }


        /// <summary>
        /// ching add for check inject
        /// </summary>
        /// <param name="args"></param>
        public static bool SqlInjectP(params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (null != args[i] && (args[i].ToString().Contains("'") || args[i].ToString().Contains("--")
                    || args[i].ToString().Contains("\\") || args[i].ToString().Contains("\\|")
                    || args[i].ToString().Contains("insert ") || args[i].ToString().Contains("drop ")
                    || args[i].ToString().Contains("alter ") || args[i].ToString().Contains("update ")
                    || args[i].ToString().Contains("delete ") || args[i].ToString().Contains("execute ")
                    || args[i].ToString().Contains("create ") || args[i].ToString().Contains("exec ")
                    || args[i].ToString().Contains("rollback ") || args[i].ToString().Contains("backup ")
                    || args[i].ToString().Contains("declare ") || args[i].ToString().Contains("reverse ")
                    || args[i].ToString().Contains("union ") || args[i].ToString().Contains("union ") || (args[i].ToString().Contains("like") && args[i].ToString().Contains("%"))
                    || args[i].ToString().Contains("<") || args[i].ToString().Contains(">")
                    || args[i].ToString().Contains("\r\n") || args[i].ToString().Contains("*/")
                    || args[i].ToString().Contains("/\\*") || args[i].ToString().Contains("xp_") || args[i].ToString().Contains(" or "))
                    )
                {
                    return false;// args[i] = args[i].ToString().Replace("'", "''");
                }
            }
            return true;
        }
    }
}
