using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.SqlCommandText
{
    public class EuroSqlText
    {
        /// <summary>
        /// 查询账号竞猜数据
        /// </summary>
        public const string LoadBetTeamsByIdSql = @"select * from ofEuroBetTeams 
where id={0} ";

        /// <summary>
        /// 更新竞猜数据的状态
        /// </summary>
        public const string UpdateBetTeamsByIdSql = @"update ofEuroBetTeams set b_status={1}, verify_at='{2}', who_verify='{3}'
where id={0} and b_status=0 ";


    }
}
