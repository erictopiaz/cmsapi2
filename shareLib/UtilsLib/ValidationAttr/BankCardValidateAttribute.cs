using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace UtilsLib.ValidationAttr
{
    /// <summary>
    /// 检查银行卡号
    /// </summary>
    public class BankCardValidateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var _value = value == null ? "" : value.ToString();
                var errorMsg = string.Format(@"{0}[{1}]", ErrorMessageString, _value.ToString());
                if (_value == null || _value.ToString() == "")
                    return new ValidationResult(errorMsg);
                string[] RxStr = new string[] { @"^\d{16}$", @"^\d{17}$", @"^\d{18}$", @"^\d{19}$" };
                int Len = RxStr.Length;
                for (int i = 0; i < Len; i++)
                {
                    Regex sRx = new Regex(RxStr[i], RegexOptions.IgnoreCase);
                    if (sRx.IsMatch(_value.ToString(), 0))
                        return ValidationResult.Success;
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
