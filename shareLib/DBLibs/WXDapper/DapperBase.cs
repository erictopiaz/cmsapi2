using Dapper;
using DBLibs.Mongo;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DBLibs.WXDapper
{
    public class DapperBase : DapperWithMongoDBBase {
        /// <summary>
        /// exec using T-SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static bool Execute(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "")
        {
            // 所有查询sql 追加 OPTION(RECOMPILE)
            if (commandType == CommandType.Text
                && !sql.ToLower().Contains("begin"))
                sql = sql.Contains("RECOMPILE") ? sql : string.Format(@"{0} OPTION(RECOMPILE)", sql);
            using (IDbConnection conn = new SqlConnection(ConnectionString) )
            {
                conn.Open();
                int count = conn.Execute(sql, param, transaction, commandTimeout, commandType);
                return count > 0;
            }
        }

        /// <summary>
        /// exec using T-SQL Asynchronous
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static async Task<bool> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "")
        {
            // 所有查询sql 追加 OPTION(RECOMPILE)
            if (commandType == CommandType.Text 
                && !sql.ToLower().Contains("begin"))
                sql = sql.Contains("RECOMPILE") ? sql : string.Format(@"{0} OPTION(RECOMPILE)", sql);
            using (IDbConnection conn = new SqlConnection(ConnectionString) )
            {
                conn.Open();
                int count = await conn.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
                return count > 0;
            }
        }
       
        /// <summary>
        /// 执行sql事务 异步
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static async Task<bool> ExecuteTransctionAsync(List<string> sqlArray, List<object> param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "")
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString) )
            {
                conn.Open();
                using (transaction = conn.BeginTransaction())
                {
                    try
                    {
                        //多条sql执行
                        for (int i = 0; i < sqlArray.Count; i++)
                        {
                            int count = await conn.ExecuteAsync(sqlArray[i], param[i], transaction, commandTimeout, commandType);
                            if (count > 0)
                                continue;
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                        //提交事务
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)//
                    {
                        //出现异常，事务Rollback
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
        /// <summary>
        /// 执行sql事务 同步
        /// </summary>
        /// <param name="sqlArray"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public static bool ExecuteTransction(List<string> sqlArray, List<object> param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "")
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (transaction = conn.BeginTransaction())
                {
                    try
                    {
                        //多条sql执行
                        for (int i = 0; i < sqlArray.Count; i++)
                        {
                            int count = conn.Execute(sqlArray[i], param[i], transaction, commandTimeout, commandType);
                            if (count > 0)
                                continue;
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                        //提交事务
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        //出现异常，事务Rollback
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 执行sql事务 同步
        /// </summary>
        /// <param name="sqlArray"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public static Tuple<bool,string> ExecuteTransctionWithError(List<string> sqlArray, List<object> param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "")
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (transaction = conn.BeginTransaction())
                {
                    try
                    {
                        //多条sql执行
                        for (int i = 0; i < sqlArray.Count; i++)
                        {
                            int count = conn.Execute(sqlArray[i], param[i], transaction, commandTimeout, commandType);
                            if (count > 0)
                                continue;
                            else
                            {
                                transaction.Rollback();
                                return new Tuple<bool, string>(false, "Execute count lte 0"); ;
                            }
                        }
                        //提交事务
                        transaction.Commit();
                        return new Tuple<bool, string>(true, ""); ;
                    }
                    catch (Exception ex)
                    {
                        //出现异常，事务Rollback
                        transaction.Rollback();
                        return new Tuple<bool, string>(false, ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行sql事务 异步
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static async Task<bool> ExecuteTransctionAsync(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "")
        {
            // 所有查询sql 追加 OPTION(RECOMPILE)
            if (commandType == CommandType.Text
                && !sql.ToLower().Contains("begin"))
                sql = sql.Contains("RECOMPILE") ? sql : string.Format(@"{0} OPTION(RECOMPILE)", sql);
            using (IDbConnection conn = new SqlConnection(ConnectionString) )
            {
                conn.Open();
                using (transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int count = await conn.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
                        if (count > 0)
                        {
                            //提交事务
                            transaction.Commit();
                            return true;
                        }
                        else
                        {
                            //提交事务
                            transaction.Rollback();
                            return false;
                        }
                    }
                    catch (Exception)
                    {
                        //出现异常，事务Rollback
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }


        /// <summary>
        /// 执行sql事务 同步
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static bool ExecuteTransction(string sql, object param = null, IDbTransaction transaction = null,
           int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "")
        {
            // 所有查询sql 追加 OPTION(RECOMPILE)
            if (commandType == CommandType.Text 
                && !sql.ToLower().Contains("begin"))
                sql = sql.Contains("RECOMPILE") ? sql : string.Format(@"{0} OPTION(RECOMPILE)", sql);
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int count = conn.Execute(sql, param, transaction, commandTimeout, commandType);
                        if (count > 0)
                        {
                            //提交事务
                            transaction.Commit();
                            return true;
                        }
                        else
                        {
                            //提交事务
                            transaction.Rollback();
                            return false;
                        }
                    }
                    catch (Exception)
                    {
                        //出现异常，事务Rollback
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 同表多条记录插入操作
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public static async Task<bool> ExecuteTransctionMultipleAsync(string sql, object param = null, IDbTransaction transaction = null,
           int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "")
        {
            // 所有查询sql 追加 OPTION(RECOMPILE)
            if (commandType == CommandType.Text 
                && !sql.ToLower().Contains("begin"))
                sql = sql.Contains("RECOMPILE") ? sql : string.Format(@"{0} OPTION(RECOMPILE)", sql);
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int count = await conn.ExecuteAsync(sql, param, transaction: transaction, commandTimeout);
                        if (count > 0)
                        {
                            //提交事务
                            transaction.Commit();
                            return true;
                        }
                        else
                        {
                            //提交事务
                            transaction.Rollback();
                            return false;
                        }
                    }
                    catch (Exception)
                    {
                        //出现异常，事务Rollback
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
        /// <summary>
        /// 执行命令对象的SQL语句，如果SQL语句是SELECT查询
        /// 刚会判断是查根据WHERE条件，查询列是否存在行ID,返回bool
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static async Task<bool> ExecuteScalarAsync(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "")
        {
            // 所有查询sql 追加 OPTION(RECOMPILE)
            if (commandType == CommandType.Text
                && !sql.ToLower().Contains("begin"))
                sql = sql.Contains("RECOMPILE") ? sql : string.Format(@"{0} OPTION(RECOMPILE)", sql);
            using (IDbConnection conn = new SqlConnection(ConnectionString) )
            {
                conn.Open();
                object obj = await conn.ExecuteScalarAsync(sql, param, transaction, commandTimeout, commandType);
                var count = Convert.ToInt64(obj);
                return count > 0;
            }
        }
        public static bool ExecuteScalar(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "")
        {
            // 所有查询sql 追加 OPTION(RECOMPILE)
            if (commandType == CommandType.Text 
                && !sql.ToLower().Contains("begin"))
                sql = sql.Contains("RECOMPILE") ? sql : string.Format(@"{0} OPTION(RECOMPILE)", sql);
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                object obj = conn.ExecuteScalar(sql, param, transaction, commandTimeout, commandType);
                var count = Convert.ToInt64(obj);
                return count > 0;
            }
        }

        /// <summary>
        /// 执行命令对象的SQL语句，如果SQL语句是SELECT查询，则仅仅返回查询结果集中的第1行第1列，而忽略其他的行 和列。
        /// 该方法所返回的结果为object类型，在使用之前必须强制转换为所需的类型
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static async Task<object> ExecuteScalarValueAsync(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "")
        {
            // 所有查询sql 追加 OPTION(RECOMPILE)
            if (commandType == CommandType.Text 
                && !sql.ToLower().Contains("begin"))
                sql = sql.Contains("RECOMPILE") ? sql : string.Format(@"{0} OPTION(RECOMPILE)", sql);
            using (IDbConnection conn = new SqlConnection(ConnectionString) )
            {
                conn.Open();
                return await conn.ExecuteScalarAsync(sql, param, transaction, commandTimeout, commandType);
            }
        }
        public static object ExecuteScalarValue(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "") 
        {
            // 所有查询sql 追加 OPTION(RECOMPILE)
            if (commandType == CommandType.Text 
                && !sql.ToLower().Contains("begin"))
                sql = sql.Contains("RECOMPILE") ? sql : string.Format(@"{0} OPTION(RECOMPILE)", sql);
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                return conn.ExecuteScalar(sql, param, transaction, commandTimeout, commandType);
            }
        }
        /// <summary>
        /// exec using stored procedure
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static bool ExecuteForProc(string storedProcedureName, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType commandType = CommandType.StoredProcedure, string ConnectionString = "")
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString) )
            {
                conn.Open();
                int count = conn.Execute(storedProcedureName, param, transaction, commandTimeout, commandType);
                return count > 0;
            }
        }
        /// <summary>
        /// exec using stored procedure Asynchronous
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static async Task<bool> ExecuteForProcAsync(string storedProcedureName, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType commandType = CommandType.StoredProcedure, string ConnectionString = "")
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString) )
            {
                conn.Open();
                int count = await conn.ExecuteAsync(storedProcedureName, param, transaction, commandTimeout, commandType);
                return count > 0;
            }
        }
        /// <summary>
        /// query using T-SQL 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static IList<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null,
            bool buffered = true, int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "")
        {
            // 所有查询sql 追加 OPTION(RECOMPILE)
            if (commandType == CommandType.Text
                && !sql.ToLower().Contains("begin"))
                sql = sql.Contains("RECOMPILE") ? sql : string.Format(@"{0} OPTION(RECOMPILE)", sql);
            using (IDbConnection conn = new SqlConnection(ConnectionString) )
            {
                conn.Open();
                return conn.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType).ToList();
            }
        }

        /// <summary>
        /// query using T-SQL Asynchronous
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static async Task<IList<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "")
        {
            // 所有查询sql 追加 OPTION(RECOMPILE)
            if(commandType == CommandType.Text 
                && !sql.ToLower().Contains("begin"))
                sql = sql.Contains("RECOMPILE") ? sql : string.Format(@"{0} OPTION(RECOMPILE)", sql);

            using (IDbConnection conn = new SqlConnection(ConnectionString) )
            {
                conn.Open();
                var res = await conn.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
                return res.ToList();
            }
        }

        public static async Task<T> QueryModelAsync<T>(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "")
        {
            // 所有查询sql 追加 OPTION(RECOMPILE)
            if (commandType == CommandType.Text 
                && !sql.ToLower().Contains("begin"))
                sql = sql.Contains("RECOMPILE") ? sql : string.Format(@"{0} OPTION(RECOMPILE)", sql);
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var res = await conn.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
                return res.ToList().First();
            }
        }


        public static async Task<object> QueryMultipleAsync(string sql, object param = null, IDbTransaction transaction = null,
           int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "")
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString) )
            {
                conn.Open();
                var res = await conn.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType);
                var list = res.Read().ToList();
                return res;
            }
        }
        /// <summary>
        /// 执行多条sql 返回
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public static async Task<Tuple<IEnumerable<T>, IEnumerable<M>>> QueryMultipleSqlAsync<T, M>(string sql, object param = null,IDbTransaction transaction = null,
           int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "")
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var res = await conn.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType);
                var t = await res.ReadAsync<T>();
                var m = await res.ReadAsync<M>();
                return new Tuple<IEnumerable<T>, IEnumerable<M>>(t, m);
            }
        }

        /// <summary>
        /// query using stored procedure
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedProcedureName"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static IList<T> QueryForProc<T>(string storedProcedureName, object param = null, IDbTransaction transaction = null,
            bool buffered = true, int? commandTimeout = null, CommandType commandType = CommandType.StoredProcedure, string ConnectionString = "")
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString) )
            {
                conn.Open();
                return conn.Query<T>(storedProcedureName, param, transaction, buffered, commandTimeout, commandType).ToList();
            }
        }

        /// <summary>
        /// query using stored procedure Asynchronous
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedProcedureName"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static async Task<IList<T>> QueryForProcAsync<T>(string storedProcedureName, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType commandType = CommandType.StoredProcedure, string ConnectionString = "")
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString) )
            {
                conn.Open();
                var res = await conn.QueryAsync<T>(storedProcedureName, param, transaction, commandTimeout, commandType);
                return res.ToList();
            }
        }

        /// <summary>
        /// dynamic query using sql 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static dynamic Query(string sql, object param = null, IDbTransaction transaction = null,
            bool buffered = true, int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "")
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString) )
            {
                conn.Open();
                return conn.Query(sql, param, transaction, buffered, commandTimeout, commandType).ToList();
            }
        }

        /// <summary>
        /// dynamic query using sql Asynchronous
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static async Task<dynamic> QueryAsync(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType commandType = CommandType.Text, string ConnectionString = "")
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString) )
            {
                conn.Open();
                var res = await conn.QueryAsync(sql, param, transaction, commandTimeout, commandType);
                return res.ToList();
            }
        }

        /// <summary>
        /// dynamic query using stored procedure
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static dynamic QueryForProc(string storedProcedureName, object param = null, IDbTransaction transaction = null,
           bool buffered = true, int? commandTimeout = null, CommandType commandType = CommandType.StoredProcedure, string ConnectionString = "")
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString) )
            {
                conn.Open();
                return conn.Query(storedProcedureName, param, transaction, buffered, commandTimeout, commandType).ToList();
            }
        }

        /// <summary>
        /// dynamic query using stored procedure Asynchronous
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static async Task<dynamic> QueryForProcAsync(string storedProcedureName, object param = null, IDbTransaction transaction = null,
           int? commandTimeout = null, CommandType commandType = CommandType.StoredProcedure , string ConnectionString = "")
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString) )
            {
                conn.Open();
                var res = await conn.QueryAsync(storedProcedureName, param, transaction, commandTimeout, commandType);
                return res.ToList();
            }
        }


        #region 多库事务处理


        /// <summary>
        /// 不同库，执行sql事务 异步  测试没通过
        /// </summary>
        /// <param name="sqlFirst"></param>
        /// /// <param name="sqlSecond"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static async  Task<bool> ExecuteTransctionScopeAsync(string sqlFirst, string sqlSecond, 
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null, 
            CommandType commandType = CommandType.Text, 
            string ConnectionString1 = "", 
            string ConnectionString2 = "") 
        {
            var returnValue_1 = 0;
            TransactionOptions transactionOptions = new TransactionOptions() {
                Timeout = TransactionManager.DefaultTimeout,
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            };
            try {
                using (TransactionScope scope = new TransactionScope()) {
                    using (var conn1 = new SqlConnection(ConnectionString1)) {
                        conn1.Open();
                        returnValue_1 = await conn1.ExecuteAsync(sqlFirst, param, transaction, commandTimeout, commandType);
                        using (var conn2 = new SqlConnection(ConnectionString2)) {
                            conn2.Open();
                            returnValue_1 = 0;
                            returnValue_1 = await conn2.ExecuteAsync(sqlSecond, param, transaction, commandTimeout, commandType);
                        }
                    }

                    scope.Complete();
                }
                return returnValue_1 > 0;
            }
            catch (Exception) {
                return returnValue_1 > 0;
            }
        }


        /// <summary>
        /// 不同库，执行sql事务 异步
        /// </summary>
        /// <param name="sqlFirst"></param>
        /// /// <param name="sqlSecond"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static async Task<bool> ExecuteTransctionScopeAsync(string sqlFirst, string sqlSecond,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType commandType = CommandType.Text,
            string ConnectionString = "") 
        {
            var returnValue = 0;
            try {
                using (TransactionScope scope = new TransactionScope()) {
                    using (var conn1 = new SqlConnection(ConnectionString)) {
                        conn1.Open();
                        returnValue = await conn1.ExecuteAsync(sqlFirst, param, transaction, commandTimeout, commandType);
                        returnValue = 0;

                    }
                    scope.Complete();
                }
                return returnValue > 0;
            }
            catch (Exception) {

                return returnValue > 0;
            }
        }

        #endregion


    }
}
