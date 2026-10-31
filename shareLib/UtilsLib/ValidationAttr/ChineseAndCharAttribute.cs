using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace UtilsLib.ValidationAttr
{
    /// <summary>
    /// 检查汉字、字母组合
    /// </summary>
    public class ChineseAndCharAttribute : ValidationAttribute
    {
        public int MaximumLength { get; }
        public int MinimumLength { get; set; }
        public int _type { set; get; }

        public bool IsAllowedEmpty { set; get; }
        /// <summary>
        /// 构造初始化参数
        /// </summary>
        /// <param name="MaximumLength"></param>
        /// <param name="type">0:只为汉字,1:只为字母,2:字母或数字,3:检查仅为汉字或字母或数字,4:检查仅为汉字或字母或数字或空格</param>
        public ChineseAndCharAttribute(int MaximumLength,int type)
        {
            this.MaximumLength = MaximumLength;
            this._type = type;
        }
        //public override bool IsValid(object value)
        //{
        //    try
        //    {
        //        if (IsAllowedEmpty && (value == null || value.ToString() == ""))
        //            return true;
        //        if (!IsAllowedEmpty && (value == null || value.ToString() == ""))
        //            return false;
        //        bool bReturnValue = false;
        //        if (value.ToString().Length < MinimumLength || value.ToString().Length > MaximumLength) return false;
        //        string sRxStr = string.Empty;
        //        switch (_type)
        //        {
        //            case 0: sRxStr = @"^[\u4e00-\u9fa5]+$"; break;
        //            case 1: sRxStr = @"^[A-Za-z]+$"; break;
        //            case 2: sRxStr = @"^\w+$"; break;
        //            case 3: sRxStr = @"^[A-Za-z0-9\u4e00-\u9fa5]+$"; break;
        //            default: break;

        //        }
        //        Regex sRx = new Regex(sRxStr, RegexOptions.IgnoreCase);
        //        bReturnValue = sRx.IsMatch(value.ToString(), 0);
        //        return bReturnValue;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var _value = value == null ? "" : value.ToString();
                var errorMsg = string.Format(@"{0}[{1}]", ErrorMessageString, _value.ToString());
                if (IsAllowedEmpty && (_value == null || _value.ToString() == ""))
                    return ValidationResult.Success;
                if (!IsAllowedEmpty && (_value == null || _value.ToString() == ""))
                    return new ValidationResult(errorMsg);
                if (_value.ToString().Length < MinimumLength || _value.ToString().Length > MaximumLength)
                    return new ValidationResult(errorMsg);
                string sRxStr = string.Empty;
                switch (_type)
                {
                    case 0: sRxStr = @"^[\u4e00-\u9fa5]+$"; break;
                    case 1: sRxStr = @"^[A-Za-z]+$"; break;
                    case 2: sRxStr = @"^\w+$"; break;
                    case 3: sRxStr = @"^[A-Za-z0-9\u4e00-\u9fa5]+$"; break;
                    case 4: sRxStr = @"^[A-Za-z0-9\u4e00-\u9fa5 ]+$"; break;
                    default: break;

                }
                Regex sRx = new Regex(sRxStr, RegexOptions.IgnoreCase);
                if (!sRx.IsMatch(_value.ToString(), 0))
                    return new ValidationResult(errorMsg);
                else
                    return ValidationResult.Success;
            }
            catch (Exception)
            {
                var _value = value == null ? "" : value.ToString();
                var errorMsg = string.Format(@"{0}[{1}]e", ErrorMessageString, _value.ToString());
                return new ValidationResult(errorMsg);
            }
        }
        
        public override string FormatErrorMessage(string name)
        {
            return base.FormatErrorMessage(name);
        }
    }
}
