using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilsLib.LinqPage
{
    /// <summary>
    /// linq 分页使用
    /// </summary>
    public static class PagingUtils
    {
        public static IEnumerable<T> Page<T>(this IEnumerable<T> en, int pageSize, int page)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            return en.Skip((page-1) * pageSize).Take(pageSize);
        }
        public static IQueryable<T> Page<T>(this IQueryable<T> en, int pageSize, int page)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            return en.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
