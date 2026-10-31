using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace UtilsLib.ValidationAttr
{
    /// <summary>
    /// 检查数字字串
    /// </summary>
    public class DecimalValidateAttribute : ValidationAttribute
    {
        public int MaximumLength { get; }
        public int MinimumLength { get; set; }
        /// <summary>
        /// 数字默认为0
        /// </summary>
        public bool IsAllowedEmpty { set; get; }
        public DecimalValidateAttribute(int MaximumLength)
        {
            this.MaximumLength = MaximumLength;
        }
       
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var _value = value == null ? "" : value.ToString();
                var errorMsg = string.Format(@"{0}[{1}]", ErrorMessageString, _value.ToString());
                if (IsAllowedEmpty && (_value == null || _value.ToString() == "" || _value.ToString() == "0"))
                    return ValidationResult.Success;
                if (!IsAllowedEmpty && (_value == null || _value.ToString() == "" || _value.ToString() == "0"))
                    return new ValidationResult(errorMsg);
                string sRxStr = @"^\d{" + MinimumLength.ToString() + "," + MaximumLength.ToString() + "}$";
                Regex sRx = new Regex(sRxStr, RegexOptions.IgnoreCase);
                decimal outDecimal = 0.0m;
                
                if (!decimal.TryParse(_value, out outDecimal))
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
