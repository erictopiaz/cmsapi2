using Microsoft.DotNet.PlatformAbstractions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmsLib.Models {
    public class ITNIOErrorCode {
        public static Dictionary<int, ErrorModel> errorCodes = new Dictionary<int, ErrorModel>() {
            { 0, new ErrorModel() { en = " success", zh="成功"} },
            { -1, new ErrorModel() { en="Authentication error", zh="认证错误"}},
            { -2, new ErrorModel() { en = "Restricted IP access", zh = "Ip访问受限"}},
            { -3, new ErrorModel() { en = "Sensitive characters in SMS content", zh = "短信内容含有敏感字符"}},
            { -4, new ErrorModel() { en = "SMS content is empty", zh = "短信内容为空"}},
            { -5, new ErrorModel() { en = "The SMS content is too long", zh = "短信内容过长"}},
            { -6, new ErrorModel() { en  = "SMS that is not a template", zh = "不是模板的短信"}},
            { -7, new ErrorModel() { en = "Phone number exceeds limit", zh = "号码个数过多"}},
            { -8, new ErrorModel() { en = "The phone number is empty", zh = "号码为空"}},
            { -9, new ErrorModel() { en = "Abnormal phone number", zh = "号码异常"}},
            { -10, new ErrorModel() { en = "The customer's balance is insufficient", zh = "客户余额不足，不能满足本次发送"}},
            { -11, new ErrorModel() { en = "Incorrect time format", zh = "定时时间格式不对"}},
            { -12, new ErrorModel() { en = "Platform reason, SMS submit fail, Please contact the administrator", zh = "由于平台的原因，批量提交出错，请与管理员联系"}},
            { -13, new ErrorModel() { en = "User locked", zh = "用户被锁定"}},
            { -14, new ErrorModel() { en = "Field is empty or query ID is abnormal", zh = "Field为空或者查询id异常"}},
            { -15, new ErrorModel() { en = "Frequent query", zh = "查询过频繁"}},
            { -16, new ErrorModel() { en = "Timestamp expires", zh = "超出时间范围限制"}},
            { -17, new ErrorModel() { en = "SMS template is empty", zh = "短信模版不能为空"}},
            { -18, new ErrorModel() { en = "port program unusual", zh = "端口程序异常"}},
            { -19, new ErrorModel() { en = "Please contact the business managers binding channel", zh = "请联系业务经理绑定通道"}},
            { -20, new ErrorModel() { en= "Data existing", zh = "数据已存在"}},
            { -21, new ErrorModel() { en = "Data validation exception", zh = "数据验证异常"}},
            { -22, new ErrorModel() { en = "Parameter exception", zh = "参数异常"} },
            { -23, new ErrorModel() { en = "Data caps", zh = "数据达到上限"}},
            { -24, new ErrorModel() { en = "Data unexisting", zh = "数据不存在"}},
            { -25, new ErrorModel() { en = "Out of time range", zh = "超出时间范围"}},
            { -26, new ErrorModel() { en = "Getting fee faily", zh = "查询费用失败"}},
            { -27, new ErrorModel() { en = "Period total send limit", zh = "发送总数限制"}},
            { -28, new ErrorModel() { en = "Period phone send limit", zh = "号码发送限制"}},
            { -29, new ErrorModel() { en = "WorkSpace is disabled", zh = "WorkSpace 空间已禁用"}},
            { -30, new ErrorModel() { en = "API Key is disabled", zh = "API Key 已禁用"}},
            { -31, new ErrorModel() { en = "IP is not in the whitelist", zh = "IP 不在白名单内"}},
            { -32, new ErrorModel() { en = "API Key does not have permission to access the current application", zh = "API Key 无权限访问当前应用"}}
        };
    }

    public class ErrorModel {
        public string en { set; get; }
        public string zh { set; get; }
    }
}
