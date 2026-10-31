using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;

namespace AWC2019.Core.Validation
{
    public static class FluentValidationRuleExtensions
    {
        /// <summary>
        /// 后台密码
        /// </summary>
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                //.Matches("[A-Z]").WithMessage("1")
                //.Matches("[a-z]").WithMessage("1")
                //.Matches("[0-9]").WithMessage("1")
                .MinimumLength(6).WithMessage("长度6-12位")
                .MaximumLength(12).WithMessage("长度6-12位")
                .Matches("^[a-zA-Z0-9_]*$").WithMessage("只允许字母和数字");
            return options;
        }

        public static IRuleBuilder<T, string> DigitOnly<T>(this IRuleBuilder<T, string> ruleBuilder, int min, int max, bool isAllowedEmpty = true)
        {
            var message = string.Format(@"长度{0}-{1}位", min, max);
            var options = ruleBuilder
                .MinimumLength(min).WithMessage("长度4-12位")
                .MaximumLength(max).WithMessage("长度4-12位")
                .Matches("^[0-9]*$").WithMessage("只允许字母和数字");
            return options;
        }
        public static IRuleBuilder<T, string> EmailAddressNullable<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                //.Cascade(CascadeMode.StopOnFirstFailure)
                .EmailAddress()
                .When(x => { 
                    return !string.IsNullOrWhiteSpace(x.ToString()); })
                .WithMessage("Invalid email address format");
        }

        //return ruleBuilder
        //    .EmailAddress()
        //    .When(dto => !string.IsNullOrWhiteSpace(dto))
        //    .WithMessage("Invalid email address format");

        //return ruleBuilder.Must((rootObject, x, context) =>
        //{
        //    string response = "";
        //    //try
        //    if (string.IsNullOrEmpty(x))
        //        return true;


        //    return true;
        //})
    //}

        public static IRuleBuilder<T, string> AmountStr<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must((rootObject, x, context) =>
            {
                if (string.IsNullOrEmpty(x)) return true;

                float amountFloat = 0;
                if (!float.TryParse(x, out amountFloat)) return false;

                if (amountFloat < 0)
                {
                    context.MessageFormatter.AppendArgument("Response", "金额不可小于0");
                    return false;
                }
                return true;
            })
                .WithMessage("{Response}");

        }


        public static IRuleBuilder<T, long> Id<T>(this IRuleBuilder<T, long> ruleBuilder)
        {
            var options = ruleBuilder
                .GreaterThanOrEqualTo(0)
                .LessThan(999999999).WithMessage("ID不再范围里");
            return options;
        }


        /// <summary>
        /// 后台账号
        /// </summary>
        public static IRuleBuilder<T, string> Account<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must((rootObject, x, context) =>
            {
                string response = "";
                var checkresult = CharDigitCheckNullable(x, 2, 20, out response);
                if (checkresult) return true;
                context.MessageFormatter.AppendArgument("Response", response);
                return false;

            })
    .WithMessage("{Response}");
            //var options = ruleBuilder
            //    .MinimumLength(2).WithMessage("长度2-20位")
            //    .MaximumLength(20).WithMessage("长度2-20位")
            //    .Matches("^[a-zA-Z0-9_]*$").WithMessage("只允许字母和数字");
            //return options;
        }
        public static IRuleBuilder<T, string> AccountPlayer<T>(this IRuleBuilder<T, string> ruleBuilder, int min, int max, bool isAllowedEmpty = true)
        {
            var message = string.Format(@"长度{0}-{1}位", min, max);
            var options = ruleBuilder
                .MinimumLength(2).WithMessage(message)
                .MaximumLength(20).WithMessage(message)
                .Empty()
                .Matches("^[a-zA-Z0-9_]*$").WithMessage("只允许字母和数字");
            return options;
        }

        /// <summary>
        /// 时间格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilder<T, DateTime?> Datetime<T>(this IRuleBuilder<T, DateTime?> ruleBuilder)
        {
            var options = ruleBuilder
                .Must(BeAValidDate).WithMessage("时间格式有误");
            return options;
        }

        public static IRuleBuilder<T, DateTime> Datetime<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
        {
            var options = ruleBuilder
                .Must((rootobject, x, context) =>
                {
                    if (x == null) return false;
                    return !x.Equals(default(System.DateTime));
                }).WithMessage("时间格式有误");
            return options;
        }





        /// <summary>
        /// IP Address
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilder<T, string> Realname<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must((rootObject, x, context) =>
            {
                //DateTime parsed_datetime/*;*/
                Regex reg = new Regex(@"[\p{Lo}\p{S}]+$");

                context.MessageFormatter.AppendArgument("OrigValue", x);
                if (string.IsNullOrEmpty(x)) return true;

                bool comparison = reg.IsMatch(x);
                return comparison;
            })
            .WithMessage("RealName不对格式有误 {OrigValue}");
        }





        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilder<T, string> AccountRegex<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must((rootObject, x, context) =>
                {
                    string response = "";
                    var checkresult = CharDigitCheckNullable(x, 4, 20, out response);
                    if (checkresult) return true;
                    context.MessageFormatter.AppendArgument("Response", response);
                    return false;

                })
                .WithMessage("{Response}");
        }


        /// <summary>
        /// IP Address
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilder<T, string> DateTimeStr<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must((rootObject, x, context) =>
            {
                DateTime parsed_datetime;
                //Regex reg = new Regex(@"\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.|$)){4}\b");

                context.MessageFormatter.AppendArgument("OrigValue", x);
                if (string.IsNullOrEmpty(x)) return true;

                bool comparison = DateTime.TryParse(x, out parsed_datetime);
                return comparison;
            })
            .WithMessage("DateTime不对格式有误 {OrigValue}");
        }



        public static IRuleBuilder<T, string> CharAndDigital<T>(this IRuleBuilder<T, string> ruleBuilder, int min, int max, bool isAllowedEmpty = true)
        {
            var message = string.Format(@"长度{0}-{1}位", min, max);
            var options = ruleBuilder
                .MinimumLength(min).WithMessage("长度4-12位")
                .MaximumLength(max).WithMessage("长度4-12位")
                .Matches("^[a-zA-Z0-9_]*$").WithMessage("只允许字母和数字");
            return options;
        }


        public static IRuleBuilder<T, string> CharDigitalChinese<T>(this IRuleBuilder<T, string> ruleBuilder, int min, int max, bool isAllowedEmpty = true)
        {
            var message = string.Format(@"长度{0}-{1}位", min, max);
            var options = ruleBuilder
                .MinimumLength(min).WithMessage("长度4-12位")
                .MaximumLength(max).WithMessage("长度4-12位")
                .Matches("^[\u4e00-\u9fa5_a-zA-Z0-9]+$").WithMessage("只允许字母和数字和中文");
            return options;
        }



        public static IRuleBuilder<T, string> CharDigitalChineseNullable<T>(this IRuleBuilder<T, string> ruleBuilder, int min, int max, bool isAllowedEmpty = true)
        {
            return ruleBuilder.Must((rootObject, x, context) =>
            {
                string response = "";
                var check = CharDigitChineseCheckNullable(x, min, max, out response);
                if (!check)
                {
                    context.MessageFormatter.AppendArgument("Response", response);
                    return false;
                }
                return true;

            })
                .WithMessage("{Response}");
        }


        public static IRuleBuilder<T, string?> CharAndDigitalNullable<T>(this IRuleBuilder<T, string?> ruleBuilder, int min, int max, bool isAllowedEmpty = true)
        {
            return ruleBuilder.Must((rootObject, x, context) =>
            {
                string response = "";
                var check = CharDigitCheckNullable(x, min, max, out response);
                if (!check)
                {
                    context.MessageFormatter.AppendArgument("Response", response);
                    return false;
                }
                return true;
                //if (x.Length < min)
                //{
                //    context.MessageFormatter.AppendArgument("Response", "长度不可小于" + min.ToString());
                //    return false;
                //}

                //if (x.Length >= max)
                //{
                //    context.MessageFormatter.AppendArgument("Response", "长度不可大于" + max.ToString());
                //    return false;
                //}
                //return true;
            })
                .WithMessage("{Response}");



            var message = string.Format(@"长度{0}-{1}位", min, max);
            var options = ruleBuilder
                .MinimumLength(min).WithMessage("长度4-12位")
                .MaximumLength(max).WithMessage("长度4-12位")
                .Matches("^[a-zA-Z0-9_]*$").WithMessage("只允许字母和数字");
            return options;
        }



        public static IRuleBuilder<T, string?> DigitalNullable<T>(this IRuleBuilder<T, string?> ruleBuilder, int min, int max, bool isAllowedEmpty = true)
        {
            return ruleBuilder.Must((rootObject, x, context) =>
            {
                string response = "";
                var check = CharDigitCheckNullable(x, min, max, out response);
                if (!check)
                {
                    context.MessageFormatter.AppendArgument("Response", response);
                    return false;
                }
                return true;
                //if (x.Length < min)
                //{
                //    context.MessageFormatter.AppendArgument("Response", "长度不可小于" + min.ToString());
                //    return false;
                //}

                //if (x.Length >= max)
                //{
                //    context.MessageFormatter.AppendArgument("Response", "长度不可大于" + max.ToString());
                //    return false;
                //}
                //return true;
            })
                .WithMessage("{Response}");
        }


        public static IRuleBuilder<T, string> Affiliate<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must((rootObject, x, context) =>
            {
                string response = "";
                var checkresult = CharDigitCheckNullable(x, 4, 20, out response);
                if (checkresult) return true;
                context.MessageFormatter.AppendArgument("Response", response);
                return false;
            }).WithMessage("{Response}");

        }
        /// <summary>
        /// 处理旧的代理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilder<T, string> AffiliateOld<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .MinimumLength(6).WithMessage("长度6-18位")
                .MaximumLength(18).WithMessage("长度6-18位")
                .Matches("^[a-zA-Z0-9_]*$").WithMessage("只允许字母和数字");
            return options;
        }

        /// <summary>
        /// PageSize Limiter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilder<T, int> PageSize<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder.Must((rootObject, x, context) =>
            {
                string response = "";
                var result = IntegerRangeNullable(x, 1, 32000, out response);
                if (!result)
                {
                    context.MessageFormatter.AppendArgument("Response", response);
                    return false;
                }
                return true;
            }).WithMessage("{Response}");
        }

        /// <summary>
        /// PageSize Limiter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilder<T, int> Level<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder.Must((rootObject, x, context) =>
            {
                string response = "";
                var result = IntegerRangeNullable(x, -2, 20000, out response);
                if (!result)
                {
                    context.MessageFormatter.AppendArgument("Response", response);
                    return false;
                }
                return true;
            }).WithMessage("{Response}");
        }


        public static IRuleBuilder<T, int> Page<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder.Must((rootObject, x, context) =>
            {
                string response = "";
                var result = IntegerRangeNullable(x, 0, 999999, out response);
                if (!result)
                {
                    context.MessageFormatter.AppendArgument("Response", response);
                    return false;
                }
                return true;
            }).WithMessage("{Response}");
        }


        /// <summary>
        /// PageSize Limiter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilder<T, int> Status<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder.Must((rootObject, x, context) =>
            {
                string response = "";
                var result = IntegerRangeNullable(x, -2, 1000, out response);
                if (!result)
                {
                    context.MessageFormatter.AppendArgument("Response", response);
                    return false;
                }
                return true;
            }).WithMessage("{Response}");

            //var options = ruleBuilder
            //    .GreaterThanOrEqualTo(-2).WithMessage("不可以小于-2")
            //    .LessThanOrEqualTo(10).WithMessage("不可以大于10");
            //return options;
        }


        public static IRuleBuilder<T, decimal> DecimalNonNegative<T>(this IRuleBuilder<T, decimal> ruleBuilder)
        {
            return ruleBuilder.Must((rootObject, x, context) =>
            {
                string response = "";
                var result = DecimalRangeNullable(x, 0, 999999999, out response);
                if (!result)
                {
                    context.MessageFormatter.AppendArgument("Response", response);
                    return false;
                }
                return true;
            }).WithMessage("{Response}");
        }


        public static IRuleBuilder<T, float> FloatNonNegative<T>(this IRuleBuilder<T, float> ruleBuilder)
        {
            return ruleBuilder.Must((rootObject, x, context) =>
            {
                string response = "";
                var result = FloatRangeNullable(x, 0, 999999999, out response);
                if (!result)
                {
                    context.MessageFormatter.AppendArgument("Response", response);
                    return false;
                }
                return true;
            }).WithMessage("{Response}");
        }




        /// <summary>
        /// 银行卡
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilder<T, string> CardNo<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .MinimumLength(10).WithMessage("长度10-25位")
                .MaximumLength(25).WithMessage("长度10-25位")
                .Matches("^[0-9]*$").WithMessage("只能包含数字");
            return options;
        }



        /// <summary>
        /// IP Address
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilder<T, string> IP<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must((rootObject, x, context) =>
            {
                Regex reg = new Regex(@"\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.|$)){4}\b");

                bool comparison = true;
                if (!string.IsNullOrEmpty(x)) comparison = reg.IsMatch(x);

                if (!comparison)
                    context.MessageFormatter.AppendArgument("IP", x);
                return comparison;
            })
            .WithMessage("IP不对格式有误 {IP}");


            //var options = ruleBuilder
            //    .Matches(@"\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.|$)){4}\b")
            //    .MinimumLength(10).WithMessage("长度10-25位")
            //    .MaximumLength(25).WithMessage("长度10-25位")
            //    .Matches("^[0-9]*$").WithMessage("只能包含数字");
            //return options;
        }




        //public static IRuleBuilder<T, string> Account<T>(this IRuleBuilder<T, string> ruleBuilder, int minimumLength = 6)
        //{
        //    var options = ruleBuilder
        //        .NotEmpty().WithMessage("")
        //        .MinimumLength(minimumLength).WithMessage("")
        //        .Matches("[A-Z]").WithMessage("")
        //        .Matches("[a-z]").WithMessage("")
        //        .Matches("[0-9]").WithMessage("")
        //        .Matches("[^a-zA-Z0-9]").WithMessage("");
        //    return options;
        //}

        private static bool BeAValidDate(System.DateTime? date)
        {
            if (date == null) return false;
            return !date.Equals(default(System.DateTime));
        }



        private static bool IntegerRangeNullable(int? data, int min, int max, out string response)
        {
            response = "";
            int temp = 0;
            if (data == null) return true;


            if (data < min)
            {
                response = "不可小于" + min.ToString();
                return false;
            }

            if (data > max)
            {
                response = "不可大于" + max.ToString();
                return false;
            }

            return true;
        }




        private static bool DecimalRangeNullable(decimal? data, int min, int max, out string response)
        {
            response = "";
            int temp = 0;
            if (data == null) return true;


            if (data < min)
            {
                response = "不可小于" + min.ToString();
                return false;
            }

            if (data > max)
            {
                response = "不可大于" + max.ToString();
                return false;
            }

            return true;
        }

        private static bool FloatRangeNullable(float? data, int min, int max, out string response)
        {
            response = "";
            int temp = 0;
            if (data == null) return true;


            if (data < min)
            {
                response = "不可小于" + min.ToString();
                return false;
            }

            if (data > max)
            {
                response = "不可大于" + max.ToString();
                return false;
            }

            return true;
        }



        private static bool CharDigitCheckNullable(string? data)
        {
            if (string.IsNullOrEmpty(data)) return true;

            Regex reg = new Regex(@"^[a-zA-Z0-9_]*$");

            if (reg.IsMatch(data))
            {
                return true;
            }
            return false;
        }


        private static bool CharDigitChineseCheckNullable(string data, int min, int max, out string response)
        {
            response = "";
            if (string.IsNullOrEmpty(data)) return true;

            Regex reg = new Regex(@"^[\u4e00-\u9fa5_a-zA-Z0-9]+$");

            if (!reg.IsMatch(data))
            {
                response = "格式错误";
                return false;
            }

            if (data.Length < min)
            {
                response = "长度不可小于" + min.ToString();
                return false;
            }

            if (data.Length > max)
            {
                response = "长度不可大于" + max.ToString();
                return false;
            }

            return true;
        }

        private static bool CharDigitCheckNullable(string? data, int min, int max, out string response)
        {
            response = "";
            if (string.IsNullOrEmpty(data)) return true;

            Regex reg = new Regex(@"^[a-zA-Z0-9_*]*$");

            if (!reg.IsMatch(data))
            {
                response = "格式错误";
                return false;
            }

            if (data.Length < min)
            {
                response = "长度不可小于" + min.ToString();
                return false;
            }

            if (data.Length > max)
            {
                response = "长度不可大于" + max.ToString();
                return false;
            }

            return true;
        }



    }
}
