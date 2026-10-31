using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace UtilsLib.ValidationAttr
{
    public class ChineseAndCharAndUnderScoreAttribute: ValidationAttribute
    {
        public int MaximumLength { get; }
        public int MinimumLength { get; set; }
        /// <summary>
        /// 属性是否允许为空 true:允许，false:不允许
        /// </summary>
        public bool IsAllowedEmpty { set; get; }

        public ChineseAndCharAndUnderScoreAttribute(int MaximumLength)
        {
            this.MaximumLength = MaximumLength;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var _value = value == null ? "" : value.ToString();
                var errorMsg = string.Format(@"{0}[{1}]", ErrorMessageString, _value);
                if (IsAllowedEmpty && (_value == null || _value.ToString() == ""))
                    return ValidationResult.Success;
                if (!IsAllowedEmpty && (_value == null || _value.ToString() == ""))
                    return new ValidationResult(errorMsg);
                if (_value.ToString().Length < MinimumLength || _value.ToString().Length > MaximumLength)
                    return new ValidationResult(errorMsg); ;
                var sRxStr = @"^[0-9a-zA-Z_]*$";
                var sRxStr_cn = @"^[\u4e00-\u9fa5]+$";
                Regex sRx = new Regex(sRxStr, RegexOptions.IgnoreCase);
                Regex sRx_cn = new Regex(sRxStr_cn, RegexOptions.IgnoreCase);
                if (!sRx.IsMatch(_value.ToString(), 0) 
                    && !sRx_cn.IsMatch(_value.ToString(), 0))
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
