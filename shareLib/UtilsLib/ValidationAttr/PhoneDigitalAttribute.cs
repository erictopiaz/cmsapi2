using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace UtilsLib.ValidationAttr
{
    /// <summary>
    /// 手机号检查
    /// </summary>
    public class PhoneDigitalAttribute : ValidationAttribute
    {
        public bool IsAllowedEmpty { set; get; }
        
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
                string[] RxStr = new string[] { "^[0-9]{8}$", "^(11[0-9]{9})$", "^(12[0-9]{9})$", "^(13[0-9]{9})$", "^(14[0-9]{9})$", "^(15[0-9]{9})$", "^(16[0-9]{9})$", "^(17[0-9]{9})$", "^(18[0-9]{9})$", "^(19[0-9]{9})$", "^([0-9]{4}-[0-9]{8})$", "^([0-9]{3}-[0-9]{8})$", "^([0-9]{4}-[0-9]{7})$", "^[0-9]{7}$", @"^0(([1-9]\d)|([3-9]\d{2}))\d{8}$" };
                int Len = RxStr.Length;
                for (int i = 0; i < Len; i++)
                {
                    Regex sRx = new Regex(RxStr[i], RegexOptions.IgnoreCase);
                    if (sRx.IsMatch(_value.ToString(), 0))
                    {
                        return ValidationResult.Success;
                    }
                }
                return new ValidationResult(errorMsg);
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
