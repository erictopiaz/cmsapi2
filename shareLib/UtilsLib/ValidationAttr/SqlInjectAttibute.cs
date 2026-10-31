using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using UtilsLib.SqlInject;

namespace UtilsLib.ValidationAttr
{
    public class SqlInjectAttibute : ValidationAttribute
    {
        /// <summary>
        /// 属性是否允许为空 true:允许，false:不允许
        /// </summary>
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
                if (!ExpressSqlInject.SqlInjectP(_value.ToString()) ||
                    ExpressSqlInject.HasInJectKeywords(_value.ToString()))
                {
                    return new ValidationResult(errorMsg);
                }
                else
                {
                    return ValidationResult.Success;
                }
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
