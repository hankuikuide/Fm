/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/25 14:37:28
 * ***********************************************/

namespace Han.DbLight.MySQl
{
    using Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MySqlSingleTableDao<TDomain> : SingleTableDao<TDomain> where TDomain : class, new()
    {
        public MySqlSingleTableDao(IQuerySession querySession, ColumnMapperStrategy mapperStrategy = ColumnMapperStrategy.ColumnAttribute) : base(querySession, mapperStrategy)
        {
        }

        public int BatchInsert(List<TDomain> domains, params Expression<Func<TDomain, object>>[] cols)
        {
            if (domains.Count == 0)
            {
                return 0;
            }
            BatchMySqlHelper mySqlHelper = new BatchMySqlHelper(QuerySession);

            var columnRowData = GetData(domains, cols);
            string[] usedProperties = cols.Select(expression => expression.PropertyName()).ToArray();
            Table table = GetTable(this.DefaultColumnMapperStrategy);
            InsertBuilder<TDomain> insertBuilder = new InsertBuilder<TDomain>(table, domains[0], QuerySession.DatabaseInfo.SqlDialect, usedProperties);

            string sql = insertBuilder.CreateSql();
            return mySqlHelper.BatchInsertOrUpdate(sql, columnRowData);
        }

        public int BatchDelete(List<TDomain> domains, string where, params Expression<Func<TDomain, object>>[] cols)
        {
            if (domains.Count == 0)
            {
                return 0; ;
            }

            BatchMySqlHelper mySqlHelper = new BatchMySqlHelper(QuerySession);
            string[] usedProperties = cols.Select(expression => expression.PropertyName()).ToArray();
            Table table = GetTable(this.DefaultColumnMapperStrategy);
            DeleteBuilder<TDomain> deleteBuilder = new DeleteBuilder<TDomain>(table,QuerySession.DatabaseInfo.SqlDialect);
            var sql = deleteBuilder.GetSql() + "where " + where  ;
            var columnRowData = GetData(domains, cols);
            
            return mySqlHelper.BatchInsertOrUpdate(sql, columnRowData) ;
        }


        public int BatchUpdate(List<TDomain> domains, string where, params Expression<Func<TDomain, object>>[] cols)
        {
            if (domains.Count == 0)
            {
                return 0;
            }

            Expression<Func<TDomain, object>>[] setCols = new Expression<Func<TDomain, object>>[20];
            if (!string.IsNullOrEmpty(where))
            {
                var whereCount = Regex.Matches(where, @"\?\d+");
                var count = int.Parse(whereCount[whereCount.Count - 1].Value.TrimStart('?')) + 1;
                if (count != cols.Count())
                {
                    throw new ArgumentException("cols 参数长度sql中需要绑定的值不一定");
                }

                if (cols.Count() < whereCount.Count)
                {
                    throw new ArgumentException("where 字句中的 :数字 参数个数不正确");
                }

                var t = int.Parse(whereCount[0].Value.TrimStart('?'));
                setCols = cols.Take(t).ToArray();
            }
            else
            {
                setCols = cols;
            }

            BatchMySqlHelper mySqlHelper = new BatchMySqlHelper(QuerySession);
            var columnRowData = GetData(domains, cols);

            string[] usedProperties = setCols.Select(expression => expression.PropertyName()).ToArray();
            Table table = GetTable(this.DefaultColumnMapperStrategy);
            UpdateBuilder<TDomain> updateBuilder = new UpdateBuilder<TDomain>(table, QuerySession.DatabaseInfo.SqlDialect, domains[0], usedProperties);

            var sql = updateBuilder.CreateSql() + where;
            if (string.IsNullOrEmpty(where))
            {
                sql = sql.Substring(0, sql.Length - 7);
            }

            return mySqlHelper.BatchInsertOrUpdate(sql, columnRowData);

        }


        private Dictionary<int, object[]> GetData(List<TDomain> domains, Expression<Func<TDomain, object>>[] cols)
        {
            Dictionary<int, object[]> columnRowData = new Dictionary<int, object[]>();
            int i = 0;
            foreach (var expression in cols)
            {
                object[] objs = domains.Select(expression.Compile()).ToArray();
                var type = expression.GetRightMostMember().Type;

                for (int j = 0; j < objs.Length; j++)
                {
                    if (objs[j] == null)
                    {
                        objs[j] = GetDefaultValue(type);
                    }
                }
                columnRowData.Add(i, objs);
                i++;
            }
            return columnRowData;
        }

        private object GetDefaultValue(Type type)
        {
            if (type == typeof(string))
            {
                return "";
            }
            if (type == typeof(DateTime?))
            {
                return DateTime.Now;
            }
            if (type == typeof(int?) || type == typeof(decimal?) || type == typeof(long?) || type == typeof(float?))
            {
                return 0;
            }

            throw new ArgumentException("批量操作遇到不能处理的空值");
        }

    }
}
