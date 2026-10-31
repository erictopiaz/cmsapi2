using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace UtilsLib.ValidationAttr
{
    /// <summary>
    /// ip4 ip6 格式检查
    /// </summary>
    public class IpValidateAttribute : ValidationAttribute
    {
        public int MaximumLength { get; }
        public int MinimumLength { get; set; }

        /// <summary>
        /// 属性是否允许为空 true:允许，false:不允许
        /// </summary>
        public bool IsAllowedEmpty { set; get; }
        public IpValidateAttribute(int MaximumLength)
        {
            this.MaximumLength = MaximumLength;
        }
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
                IPAddress address;
                var isIP = IPAddress.TryParse(_value, out address);
                if (!IPAddress.TryParse(_value, out address))
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
