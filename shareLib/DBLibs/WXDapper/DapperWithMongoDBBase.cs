using DBLibs.Mongo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using MongoDB.Driver;

namespace DBLibs.WXDapper {
    public class DapperWithMongoDBBase {


        /// <summary>
        /// 同时更新sql server 和 mongodb add
        /// 如果mongodb 执行失败 也要回滚
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="t"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <param name="ConnectionString"></param>
        /// <param name="mongodbHost"></param>
        /// <returns></returns>
        public static async Task<bool> ExecuteTransctionWithMongoDBAddAsync<T>(
            string sql,
            T t,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType commandType = CommandType.Text,
            string ConnectionString = "", MongodbHost mongodbHost = null) 
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString)) {
                conn.Open();
                using (transaction = conn.BeginTransaction()) {
                    try {
                        int count = await conn.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
                        if (count > 0) {
                            var r_int = await TMongodbHelper.AddAsync<T>(mongodbHost, t);
                            if (r_int > 0) {
                                //提交事务
                                transaction.Commit();
                                return true;
                            }
                        }
                        //提交事务
                        transaction.Rollback();
                        return false;


                    }
                    catch (Exception) {
                        //出现异常，事务Rollback
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }


        /// <summary>
        /// 同时更新sql server 和 mongodb update
        /// 如果mongodb 执行失败 也要回滚
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="t"></param>
        /// <param name="filterTransferId">mongodb 要更新条件</param>
        /// <param name="keyNames">mongodb 要更新的字段</param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <param name="ConnectionString"></param>
        /// <param name="mongodbHost"></param>
        /// <returns></returns>
        public static async Task<bool> ExecuteTransctionWithMongoDBUpdateAsync<T>(
            string sql,
            T t,
            FilterDefinition<T> filterTransferId,
            List<string> keyNames,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType commandType = CommandType.Text,
            string ConnectionString = "", MongodbHost mongodbHost = null) {
            using (IDbConnection conn = new SqlConnection(ConnectionString)) {
                conn.Open();
                using (transaction = conn.BeginTransaction()) {
                    try {
                        int count = await conn.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
                        var r_update = await TMongodbHelper.UpdateAsync<T>(mongodbHost, t, filterTransferId, keyNames);
                        if (count > 0 && r_update.ModifiedCount > 0) {
                            //提交事务
                            transaction.Commit();
                            return true;
                        }
                        else {
                            //提交事务
                            transaction.Rollback();
                            return false;
                        }


                    }
                    catch (Exception) {
                        //出现异常，事务Rollback
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }



    }
}
