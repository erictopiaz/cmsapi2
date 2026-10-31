using DBLibs.Mongo;
using DBLibs.WXDapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LogLib.Helper {
    public class WriteToMongoDB {
        /// <summary>
        /// 记录到账数据到 当前日志(mongo db add)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="tableName"></param>
        /// <param name="writeToFile"></param>
        /// <param name="funtionName"></param>
        /// <returns></returns>
        public static async Task<bool> LogEntityToMongo<T>(T t, string tableName="requst_log_models",string funtionName = "") {
            try {
                var host = new MongodbHost() {
                    Connection = DapperStringBase.ConnectionMongoString,
                    DataBase = "admin",
                    Table = tableName
                };
                var r_int = await TMongodbHelper.AddAsync<T>(host, t);
                if (r_int > 0) {
                    return true;
                }
                else {
                    var title = string.Format(@"/{0}/WriteToMongoDB_failed", funtionName);
                    WriteToFile.TxtWriteTo(title, funtionName,
                            string.Format(@"sync event data : {0} ", JsonConvert.SerializeObject(t)), true);
                    return false;
                }
            }
            catch (Exception) {
                var title = string.Format(@"/{0}/WriteToMongoDB_ex", funtionName);
                WriteToFile.TxtWriteTo(title, funtionName,
                        string.Format(@"sync event data : {0} ", JsonConvert.SerializeObject(t)), true);
                return false;
            }
        }
    }
}
