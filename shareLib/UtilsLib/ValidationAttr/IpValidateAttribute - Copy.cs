using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace UtilsLib.ValidationAttr
{
    /// <summary>
    /// ip格式检查
    /// </summary>
    public class IpValidateAttribute_bak : ValidationAttribute
    {
        public int MaximumLength { get; }
        public int MinimumLength { get; set; }

        /// <summary>
        /// 属性是否允许为空 true:允许，false:不允许
        /// </summary>
        public bool IsAllowedEmpty { set; get; }
        public IpValidateAttribute_bak(int MaximumLength)
        {
            this.MaximumLength = MaximumLength;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var _value = value == null ? "" : value.ToString();
                var errorMsg = string.Format(@"{0}_value:{1}", ErrorMessageString, _value.ToString());
                if (IsAllowedEmpty && (_value == null || _value.ToString() == ""))
                    return ValidationResult.Success;
                if (!IsAllowedEmpty && (_value == null || _value.ToString() == ""))
                    return new ValidationResult(errorMsg);
                if (_value.ToString().Length < MinimumLength || _value.ToString().Length > MaximumLength)
                    return new ValidationResult(errorMsg);
                string sRxStr = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";
                Regex sRx = new Regex(sRxStr, RegexOptions.IgnoreCase);
                if (!sRx.IsMatch(_value.ToString(), 0))
                    return new ValidationResult(errorMsg);
                else
                    return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                var _value = value == null ? "" : value.ToString();
                var errorMsg = string.Format(@"{0}_value:{1}_ex.msg:{2}", ErrorMessageString, _value.ToString(), ex.Message);
                return new ValidationResult(errorMsg);
            }
        }
        
        public override string FormatErrorMessage(string name)
        {
            return base.FormatErrorMessage(name);
        }
    }
}
