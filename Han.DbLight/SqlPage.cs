

using System.Globalization;

namespace Han.DbLight
{
    /// <summary>
    ///  对sql进行分页（oracle）
    /// </summary>
    public class SqlPage
    {
        #region Public Methods and Operators

        /// <summary>
        /// 格式化sql语句，做分页
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        //public static string PagedSql(string sql, int pageIndex, int pageSize)
        //{
        //    int pageLower;
        //    int pageUpper;
        //    pageLower = pageSize * pageIndex;
        //    pageUpper = pageLower + pageSize;

        //    string strSQL = "SELECT tempSQL.*, count(1) OVER () as mycount FROM ( " + sql + ") tempSQL";

        //    return "SELECT * FROM ( SELECT ROWNUM as numrow, zz.*  from ( " + strSQL + @" ) zz WHERE ROWNUM <=  " + pageUpper + @"  ) WHERE numrow > " + pageLower;
        //}
        //public static string PagedSql(string sql, int pageIndex, int pageSize)
        //{

           
        //}

        private static string GetTotalCountSql(string sql, int pageIndex, int pageSize)
        {
            int pageLower;
            int pageUpper;
            pageLower = pageSize * pageIndex;
            pageUpper = pageLower + pageSize;
            //加总页数sql
            sql =sql.Trim();
            if(sql.StartsWith("select",true,CultureInfo.InvariantCulture))
            {
               sql= sql.Remove(0, 6);
               sql = "select count(*) over () as total, " + sql;
            }
            else
            {
                throw new System.Exception("分页必须为select语句");
            }
            string tt = string.Format(@"select * 
  from (select row_.*, rownum NumRow from ({0}) row_ where rownum <= {1})
 where NumRow > {2}", sql, pageUpper, pageLower);
            return tt;
        }
        /// <summary>
        /// 分页sql ，总记录在total列中
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static string ContainTotalCountPagedSql(string sql, int pageIndex, int pageSize)
        {
            string pagedsql = GetTotalCountSql(sql, pageIndex, pageSize);
            return pagedsql;
        }
        /// <summary>
        /// 分页，不要传入orderBy，性能不好
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageIndex">从0开始 第一页传入0</param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public static string PagedSql(string sql, int pageIndex, int pageSize,string orderBy=null)
        {
            int pageLower;
            int pageUpper;
            pageLower = pageSize * pageIndex;
            pageUpper = pageLower + pageSize;
            return RangedSql(sql, pageLower, pageUpper, orderBy);
        }
        public static string RangedSql(string sql, int start, int end, string orderBy = null)
        {
            int pageLower;
            int pageUpper;
            pageLower = start;
            pageUpper = end;
            if (string.IsNullOrEmpty(orderBy))
            {
                string tt = string.Format(@"select *
  from (select row_.*, rownum NumRow from ({0}) row_ where rownum <= {1})
 where NumRow > {2}", sql, pageUpper, pageLower);

                //                string tt = string.Format(@"SELECT *
                //                        FROM (SELECT rownum AS NumRow,paged.*
                //                                FROM ({0}) paged)
                //                        WHERE NumRow > {1}
                //                        AND NumRow <= {2}", sql, pageLower, pageUpper);
                return tt;

            }
            else
            {

                string tt = string.Format(@"SELECT *
                        FROM (SELECT ROW_NUMBER() OVER(ORDER BY {3}) AS NumRow,paged.*
                                FROM ({0}) paged)
                        WHERE NumRow > {1}
                        AND NumRow <= {2}", sql, pageLower, pageUpper, orderBy);
                //                string tt = string.Format(@"SELECT rownum , paged.*  FROM ({0}) paged
                //                        WHERE r > {1}
                //                        AND r <= {2}", sql, pageLower, pageUpper);
                //                tt += " order by " + orderBy;
                return tt;
            }

            //如果需要排序使用
            //ROW_NUMBER() OVER(ORDER BY ID) AS NumRow
        }
        #endregion
    }
}