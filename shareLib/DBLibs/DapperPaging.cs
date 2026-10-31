using Dapper;
using DBLibs.WXDapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilsLib;

namespace DBLibs
{
    /// <summary>
    /// 数据分页查询返回
    /// 分页从只读库中取
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DapperPaging<T>
    {
        private static string getTableName(string viewName)
        {
            var table = viewName.Split(' ').ToList();
            table.Remove("");
            return table.FirstOrDefault();
        }
        public static async Task<ApiResultModel<PageModel<T>>> QueryPage(string whereSql, string viewName,
            string leftjoinString, 
            string fields,
            string orderBy,
            string keyName,
            int pageSize,
            int pageIndex,
            string connectStringBase = "")
        {
            try
            {
                var tableName = getTableName(viewName);
                #region sp params
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@recordTotal", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                dp.Add("@viewName", viewName, DbType.AnsiString, ParameterDirection.Input, size: 2550);
                dp.Add("@pageSize", pageSize, dbType: DbType.Int32, direction: ParameterDirection.Input, size: 4);
                dp.Add("@pageNo", pageIndex, dbType: DbType.Int32, direction: ParameterDirection.Input, size: 4);
                dp.Add("@orderString", orderBy, dbType: DbType.AnsiString, direction: ParameterDirection.Input, size: 200);

                dp.Add("@fieldName", fields, dbType: DbType.AnsiString, direction: ParameterDirection.Input, size: 5000);
                dp.Add("@keyName", keyName, dbType: DbType.AnsiString, direction: ParameterDirection.Input, size: 200);
                dp.Add("@whereString", whereSql, dbType: DbType.String, direction: ParameterDirection.Input, size: 800);
                dp.Add("@leftjoinString", leftjoinString, dbType: DbType.String, direction: ParameterDirection.Input, size: 300);
                dp.Add("@tableName", tableName, DbType.AnsiString, ParameterDirection.Input, size: 50);

                #endregion

                var result = await DapperBase.QueryAsync<T>("SP_ViewPager_join", dp,
                    commandType: CommandType.StoredProcedure, commandTimeout: 15000,
                    ConnectionString: string.IsNullOrEmpty(connectStringBase) ? DapperStringBase.ConnectionStringReadOnly : connectStringBase);
                int totalRecords = dp.Get<int>("@recordTotal");//
                int pages = totalRecords / pageSize;
                pages = pages * pageSize < totalRecords ? pages + 1 : pages;

                return new ApiResultModel<PageModel<T>>()
                {
                    code = result != null ? "true" : "false",
                    message = "",
                    message_code = result != null ? "success" : "failed",
                    data = new PageModel<T>
                    {
                        TotalRecord = totalRecords,
                        TotalPages = pages,
                        PageIndex = pageIndex,
                        modelList = result.ToList()
                    }
                };
            }
            catch (Exception)
            {
                //日志
                return new ApiResultModel<PageModel<T>>()
                {
                    code = "false",
                    message = "查询-失败e",
                    message_code = "failed",
                    data = new PageModel<T>
                    {
                        TotalRecord = 0,
                        TotalPages = 0,
                        PageIndex = 0,
                        modelList = new List<T>()
                    }

                };
            }
        }


        public static object QueryPageSync(string whereSql, string viewName,
            string leftjoinString,
            string fields,
            string orderBy,
            string keyName,
            int pageSize,
            int pageIndex,
            string connectStringBase = "") {
            try {
                var tableName = getTableName(viewName);
                #region sp params
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@recordTotal", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                dp.Add("@viewName", viewName, DbType.AnsiString, ParameterDirection.Input, size: 2550);
                dp.Add("@pageSize", pageSize, dbType: DbType.Int32, direction: ParameterDirection.Input, size: 4);
                dp.Add("@pageNo", pageIndex, dbType: DbType.Int32, direction: ParameterDirection.Input, size: 4);
                dp.Add("@orderString", orderBy, dbType: DbType.AnsiString, direction: ParameterDirection.Input, size: 200);

                dp.Add("@fieldName", fields, dbType: DbType.AnsiString, direction: ParameterDirection.Input, size: 5000);
                dp.Add("@keyName", keyName, dbType: DbType.AnsiString, direction: ParameterDirection.Input, size: 200);
                dp.Add("@whereString", whereSql, dbType: DbType.String, direction: ParameterDirection.Input, size: 800);
                dp.Add("@leftjoinString", leftjoinString, dbType: DbType.String, direction: ParameterDirection.Input, size: 300);
                dp.Add("@tableName", tableName, DbType.AnsiString, ParameterDirection.Input, size: 50);

                #endregion

                var result = DapperBase.Query<T>("SP_ViewPager_join", dp,
                    commandType: CommandType.StoredProcedure, commandTimeout: 15000,
                    ConnectionString: string.IsNullOrEmpty(connectStringBase) ? DapperStringBase.ConnectionStringReadOnly : connectStringBase);
                int totalRecords = dp.Get<int>("@recordTotal");//
                int pages = totalRecords / pageSize;
                pages = pages * pageSize < totalRecords ? pages + 1 : pages;

                return new ApiResultModel<PageModel<T>>() {
                    code = result != null ? "true" : "false",
                    message = "",
                    message_code = result != null ? "success" : "failed",
                    data = new PageModel<T> {
                        TotalRecord = totalRecords,
                        TotalPages = pages,
                        PageIndex = pageIndex,
                        modelList = result.ToList()
                    }
                };
            }
            catch (Exception) {
                //日志
                return new ApiResultModel<PageModel<T>>() {
                    code = "false",
                    message = "查询-失败e",
                    message_code = "failed",
                    data = new PageModel<T> {
                        TotalRecord = 0,
                        TotalPages = 0,
                        PageIndex = 0,
                        modelList = new List<T>()
                    }

                };
            }
        }


        public static async Task<ApiResponseModel<PageModel<T>>> QueryPageV1(string whereSql, string viewName,
            string leftjoinString,
            string fields,
            string orderBy,
            string keyName,
            int pageSize,
            int pageIndex,
            string connectStringBase = "")
        {
            try
            {
                var tableName = getTableName(viewName);
                #region sp params
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@recordTotal", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                dp.Add("@viewName", viewName, DbType.AnsiString, ParameterDirection.Input, size: 2550);
                dp.Add("@pageSize", pageSize, dbType: DbType.Int32, direction: ParameterDirection.Input, size: 4);
                dp.Add("@pageNo", pageIndex, dbType: DbType.Int32, direction: ParameterDirection.Input, size: 4);
                dp.Add("@orderString", orderBy, dbType: DbType.AnsiString, direction: ParameterDirection.Input, size: 200);

                dp.Add("@fieldName", fields, dbType: DbType.AnsiString, direction: ParameterDirection.Input, size: 5000);
                dp.Add("@keyName", keyName, dbType: DbType.AnsiString, direction: ParameterDirection.Input, size: 200);
                dp.Add("@whereString", whereSql, dbType: DbType.String, direction: ParameterDirection.Input, size: 800);
                dp.Add("@leftjoinString", leftjoinString, dbType: DbType.String, direction: ParameterDirection.Input, size: 500);
                dp.Add("@tableName", tableName, DbType.AnsiString, ParameterDirection.Input, size: 50);

                #endregion
                var result = await DapperBase.QueryAsync<T>("SP_ViewPager_join", dp,
                    commandType: CommandType.StoredProcedure, commandTimeout: 15000,
                    ConnectionString: string.IsNullOrEmpty(connectStringBase) ? DapperStringBase.ConnectionStringReadOnly : connectStringBase);
                int totalRecords = dp.Get<int>("@recordTotal");//
                int pages = totalRecords / pageSize;
                pages = pages * pageSize < totalRecords ? pages + 1 : pages;

                return new ApiResponseModel<PageModel<T>>()
                {
                    code = 1,//result != null ? 1 : 0,
                    message = "",
                    message_code = "success", //result != null ? "success" : "failed",
                    data = new PageModel<T>
                    {
                        TotalRecord = totalRecords,
                        TotalPages = pages,
                        PageIndex = pageIndex,
                        modelList = result.ToList()
                    }
                };
            }
            catch (Exception)
            {
                //日志
                return new ApiResponseModel<PageModel<T>>()
                {
                    code = 0,
                    message = "数据查询,处理异常",
                    message_code = "failed",
                    data = new PageModel<T>
                    {
                        TotalRecord = 0,
                        TotalPages = 0,
                        PageIndex = 0,
                        modelList = new List<T>()
                    }

                };
            }
        }

        public static async Task<object> QueryPageV2<K>(string whereSql, string viewName,
           string leftjoinString,
           string fields,
           string orderBy,
           string keyName,
           int pageSize,
           int pageIndex,
           string connectStringBase = "") where K : new()
        {
            try
            {
                var tableName = getTableName(viewName);
                #region sp params
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@recordTotal", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                dp.Add("@viewName", viewName, DbType.AnsiString, ParameterDirection.Input, size: 2550);
                dp.Add("@pageSize", pageSize, dbType: DbType.Int32, direction: ParameterDirection.Input, size: 4);
                dp.Add("@pageNo", pageIndex, dbType: DbType.Int32, direction: ParameterDirection.Input, size: 4);
                dp.Add("@orderString", orderBy, dbType: DbType.AnsiString, direction: ParameterDirection.Input, size: 200);

                dp.Add("@fieldName", fields, dbType: DbType.AnsiString, direction: ParameterDirection.Input, size: 5000);
                dp.Add("@keyName", keyName, dbType: DbType.AnsiString, direction: ParameterDirection.Input, size: 200);
                dp.Add("@whereString", whereSql, dbType: DbType.String, direction: ParameterDirection.Input, size: 800);
                dp.Add("@leftjoinString", leftjoinString, dbType: DbType.String, direction: ParameterDirection.Input, size: 300);
                dp.Add("@tableName", tableName, DbType.AnsiString, ParameterDirection.Input, size: 50);

                #endregion

                var result = await DapperBase.QueryAsync<T>("SP_ViewPager_join", dp,
                    commandType: CommandType.StoredProcedure, commandTimeout: 15000,
                    ConnectionString: string.IsNullOrEmpty(connectStringBase) ? DapperStringBase.ConnectionStringReadOnly : connectStringBase);
                int totalRecords = dp.Get<int>("@recordTotal");//
                int pages = totalRecords / pageSize;
                pages = pages * pageSize < totalRecords ? pages + 1 : pages;

                return new ApiResponseModel<PageTupleModel<T, K>>()
                {
                    code = result != null ? 1 : 0,
                    message = "",
                    message_code = result != null ? "success" : "failed",
                    data = new PageTupleModel<T, K>
                    {
                        TotalRecord = totalRecords,
                        TotalPages = pages,
                        PageIndex = pageIndex,
                        modelList = result.ToList(),
                        totalData = new K()
                    }
                };
            }
            catch (Exception)
            {
                //日志
                return new ApiResponseModel<PageTupleModel<T, K>>()
                {
                    code = 0,
                    message = "查询-失败e",
                    message_code = "failed",
                    data = new PageTupleModel<T, K>
                    {
                        TotalRecord = 0,
                        TotalPages = 0,
                        PageIndex = 0,
                        modelList = new List<T>(),
                        totalData = new K()
                        
                    }

                };
            }
        }


        public static async Task<object> QueryPageV3(string whereSql, string viewName,
           string leftjoinString,
           string fields,
           string orderBy,
           string keyName,
           int pageSize,
           int pageIndex,
           string connectStringBase = "") {
            try {

                var tableName = getTableName(viewName);

                var startIndex = (pageIndex - 1) * pageSize + 1;
                var endIndex = pageIndex * pageSize;

                #region 总行数
                var totalRowsSql = $"SELECT a.Row from (select t.name as TableName,i.rows as [Row] from sys.tables as t, sysindexes as i where t.object_id = i.id and i.indid <=1 and t.name='{tableName}') as a";
                if(!string.IsNullOrEmpty(whereSql)) {
                    totalRowsSql = $"SELECT COUNT(*) FROM (SELECT {fields} FROM {viewName} {leftjoinString} WHERE {whereSql}) AS my_temp";
                }
                var totalRows = await DapperBase.ExecuteScalarValueAsync(totalRowsSql,
                         null, commandTimeout: 15000, ConnectionString: string.IsNullOrEmpty(connectStringBase) ? DapperStringBase.ConnectionStringReadOnly : connectStringBase);

                #endregion


                #region 新的查询方式
                var queryPageSql = $"SELECT * FROM (SELECT ROW_NUMBER() OVER (order by {orderBy} ) AS rows ,{fields} "
                                + $"FROM {viewName}"
                                + $"{leftjoinString}"
                                + $"WHERE {whereSql} ) AS main_temp "
                                + $"WHERE rows BETWEEN {startIndex} AND {endIndex}";
                #endregion

                var models = await DapperBase.QueryAsync<T>(queryPageSql,
                         null, commandTimeout: 15000, ConnectionString: string.IsNullOrEmpty(connectStringBase) ? DapperStringBase.ConnectionStringReadOnly : connectStringBase);

                int totalRecords = (int)totalRows;
                int pages = totalRecords / pageSize;
                pages = pages * pageSize < totalRecords ? pages + 1 : pages;

                return new ApiResponseModel<PageModel<T>>() {
                    code = 1,
                    message = "",
                    message_code = "success",
                    data = new PageModel<T> {
                        TotalRecord = totalRecords,
                        TotalPages = pages,
                        PageIndex = pageIndex,
                        modelList = models?.ToList()
                    }
                };
            }
            catch (Exception) {
                //日志
                return new ApiResponseModel<PageModel<T>>() {
                    code = 0,
                    message = "数据查询,处理异常",
                    message_code = "failed",
                    data = new PageModel<T> {
                        TotalRecord = 0,
                        TotalPages = 0,
                        PageIndex = 0,
                        modelList = new List<T>()
                    }

                };
            }
        }

    }
}
