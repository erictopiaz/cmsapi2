using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.SqlCommandText
{
    public class TransferSqlCommandText
    {
        /// <summary>
        /// 根据id  查看当前记录详情
        /// </summary>
        public const string viewTransferSql = @"select * from ofTransferNew where id=@id";
        /// <summary>
        /// 转账失败补单,返回金额给客户
        /// </summary>
        public const string returnTransferAmount = @"update ofUser set amountactive = amountactive + (select amount from ofTransferNew where id=@transferId and status=0)";
        /// <summary>
        /// 更新当前转账记录状态
        /// </summary>
        public const string UpdateTransferStatus = @"update ofTransferNew set status=1, ipaddress=@ipaddress, verifycustomer=@adminAccount where id=@transferId and status=0";
    }
}
