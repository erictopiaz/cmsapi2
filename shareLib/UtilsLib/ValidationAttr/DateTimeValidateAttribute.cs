using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace UtilsLib.ValidationAttr
{
    public class DateTimeValidateAttribute : ValidationAttribute
    {
        /// <summary>
        /// 最大长度
        /// </summary>
        public int MaximumLength { get; }
        /// <summary>
        /// 最小长度
        /// </summary>
        public int MinimumLength { get; set; }
        /// <summary>
        /// 数字默认为0
        /// </summary>
        public bool IsAllowedEmpty { set; get; }

        public bool _hms { set; get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MaximumLength">时间字串最大长度</param>
        /// <param name="hms">false显示时分秒</param>
        public DateTimeValidateAttribute(int MaximumLength, bool hms)
        {
            this.MaximumLength = MaximumLength;
            this._hms = hms;
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

                string sRxStr = @"^\d{4}-\d{1,2}-\d{1,2} \d{1,2}:\d{1,2}:\d{1,2}$";
                if (!_hms)
                {
                    sRxStr = @"^\d{4}-\d{1,2}-\d{1,2}$";
                }
                Regex sRx = new Regex(sRxStr, RegexOptions.IgnoreCase);
                //string sRxStr = @"^\d{" + MinimumLength.ToString() + "," + MaximumLength.ToString() + "}$";
                //Regex sRx = new Regex(sRxStr, RegexOptions.IgnoreCase);
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
