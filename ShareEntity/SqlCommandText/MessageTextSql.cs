using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.SqlCommandText
{
    /// <summary>
    /// 原注：GameBetRecordService2021.SqlCommandText
    /// </summary>
    public class MessageTextSql
    {
        /// <summary>
        /// 客人短信验证码记录
        /// </summary>
        public const string InserPhoneCodeSql = @"INSERT INTO [dbo].[ofAccountValidate]
           ([account],[type],[validate],[ipaddress],[macaddress],[body],[accountType],[phCode])
     VALUES
           (@account
           ,@type,@validate,@ipaddress,@macaddress,@body,@accountType,@phCode)";

        /// <summary>
        /// 核对当前输入的手机号是否为会员所有
        /// </summary>
        public const string CheckPhoneSql = @"select id from ofUser where account=@account and telephone=@telephone";


        /// <summary>
        /// 查询已经被禁用的电话号
        /// </summary>
        public const string QueryForbiddenPhonesSql = @"select body from ofSettings where code='all_public_configs'";

        /// <summary>
        /// 查询国际电话配置
        /// </summary>
        public const string QueryMessageConfigSql = @"select body from ofSettings where code='intl_mobilemessage_configs'";
    }
}
