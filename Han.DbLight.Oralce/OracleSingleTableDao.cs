using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Han.EnsureThat;
using Han.Infrastructure;
using Han.Log;

namespace Han.DbLight.Oracle
{
    public class OracleSingleTableDao<TDomain> : SingleTableDao<TDomain> where TDomain : class, new()
    {
        public OracleSingleTableDao(IQuerySession querySession, ColumnMapperStrategy mapperStrategy = ColumnMapperStrategy.ColumnAttribute) : base(querySession, mapperStrategy)
        {
        }

        /// <summary>
        /// 批量更新,sequence Id 不需要处理，domain上加attribute
        /// domian中可空类型string与datetime默认更新为""与当前日期 ,其他可空类型必须传入有效值
        /// </summary>
        /// <param name="domains">domain 列表</param>
        /// <param name="cols">需要插入的列</param>
        public virtual int BatchInsert(List<TDomain> domains, params Expression<Func<TDomain, object>>[] cols)
        {
            if (domains.Count == 0)
            {
                return 0;
            }
            BatchOracleHelper oracleHelper = new BatchOracleHelper(QuerySession);

            var columnRowData = GetData(domains, cols);
            string[] usedProperies = cols.Select(expression => expression.PropertyName()).ToArray();
            Table table = GetTable(this.DefaultColumnMapperStrategy);
            InsertBuilder<TDomain> insertBuilder = new InsertBuilder<TDomain>(table, domains[0], QuerySession.DatabaseInfo.SqlDialect, usedProperies);
            string sql = insertBuilder.CreateSql();
            return oracleHelper.BatchInsertOrUpdate(sql, columnRowData);
        }
        public virtual int Update(TDomain domain, string where, params Expression<Func<TDomain, object>>[] cols)
        {
            Ensure.That(domain != null).IsTrue();
            List<TDomain> domains = new List<TDomain>() { domain };
            return BatchUpdate(domains, where, cols);
        }
        /// <summary>
        ///批量更新 domian中可空类型string与datetime默认更新为""与当前日期 ,其他可空类型必须传入有效值
        /// </summary>
        /// <param name="domains">待更新domains</param>
        /// <param name="where">where sql 字句,如：ID=:5 ,数字为cols 参数 从0开始的index值</param>
        /// <param name="cols">需要更新列与where字句值，where字句在最后</param>
        /// <remarks></remarks>
        public virtual int BatchUpdate(List<TDomain> domains, string where, params Expression<Func<TDomain, object>>[] cols)
        {
            if (domains.Count == 0)
            {
                return 0;
            }
            Expression<Func<TDomain, object>>[] setCols = new Expression<Func<TDomain, object>>[20];
            if (!string.IsNullOrEmpty(where))
            {
                var whereCount = Regex.Matches(where, @":\d+");
                var count = int.Parse(whereCount[whereCount.Count - 1].Value.TrimStart(':')) + 1;
                if (count != cols.Count())
                {
                    throw new ArgumentException("cols 参数长度与 sql中需要绑定的值不一致");
                }
                Ensure.That(whereCount.Count != 0).IsTrue();

                if (cols.Count() < whereCount.Count)
                {
                    throw new ArgumentException("where 字句中的   ：数字  参数个数不正确");
                }
                var t = int.Parse(whereCount[0].Value.TrimStart(':'));
                setCols = cols.Take(t).ToArray();
            }
            else
            {
                setCols = cols;
            }

            BatchOracleHelper oracleHelper = new BatchOracleHelper(QuerySession);
            var columnRowData = GetData(domains, cols);

            string[] usedProperies = setCols.Select(expression => expression.PropertyName()).ToArray();//性能不高
            Table table = GetTable(this.DefaultColumnMapperStrategy);
            UpdateBuilder<TDomain> updateBuilder = new UpdateBuilder<TDomain>(table, QuerySession.DatabaseInfo.SqlDialect, domains[0], usedProperies);
            // private string updateTemplate = "UPDATE {0} SET {1} WHERE {2} ";
            var sql = updateBuilder.CreateSql() + where;
            if (string.IsNullOrEmpty(where))
            {
                sql = sql.Substring(0, sql.Length - 7);
            }

            return oracleHelper.BatchInsertOrUpdate(sql, columnRowData);
        }
        /// <summary>
        /// 执行删除
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="whereSql"></param>
        /// <param name="colFilter"></param>
        /// <returns></returns>
        /// <example>Delete(user,"Id=:0",u=>u.ID);</example>
        public virtual int Delete(TDomain domain, string whereSql, params Expression<Func<TDomain, object>>[] colFilter)
        {
            return BatchDelete(new List<TDomain>() { domain }, whereSql, colFilter);
        }
        /// <summary>
        /// 执行批量删除
        /// </summary>
        /// <param name="domains">需要删除的domain</param>
        /// <param name="whereSql">where sql 子句</param>
        /// <param name="colFilter">where 列</param>
        /// <returns></returns>
        public virtual int BatchDelete(List<TDomain> domains, string whereSql, params Expression<Func<TDomain, object>>[] colFilter)
        {
            if (domains.Count == 0)
            {
                return 0;
            }
            BatchOracleHelper oracleHelper = new BatchOracleHelper(QuerySession);
            Table table = GetTable(this.DefaultColumnMapperStrategy);
            DeleteBuilder<TDomain> builder = new DeleteBuilder<TDomain>(table, QuerySession.DatabaseInfo.SqlDialect);
            var sql = builder.CreateSql() + "where " + whereSql;
            var columnRowData = GetData(domains, colFilter);
            return oracleHelper.BatchInsertOrUpdate(sql, columnRowData);
        }
        /// <summary>
        /// 批量查询。where不允许空
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="domains"></param>
        /// <param name="where">where 子句 绑定索引从0 开始</param>
        /// <param name="rowMapper"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public virtual List<TResult> Query<TResult>(List<TDomain> domains, string where, Mapper.IRowMapper<TResult> rowMapper, params Expression<Func<TDomain, object>>[] cols)
        {
            if (domains.Count == 0)
            {
                return null;
            }
            Expression<Func<TDomain, object>>[] selectCols = new Expression<Func<TDomain, object>>[20];
            Expression<Func<TDomain, object>>[] whereCols = new Expression<Func<TDomain, object>>[20];
            if (!string.IsNullOrEmpty(where))
            {
                var whereCount = Regex.Matches(where, @":\d+");
                Ensure.That(whereCount.Count != 0).IsTrue();
                var t = int.Parse(whereCount[whereCount.Count - 1].Value.TrimStart(':')) + 1;
                int s = cols.Length - t;
                selectCols = cols.Take(s).ToArray();
                whereCols = cols.Skip(s).ToArray();
            }
            else
            {
                //苗建龙 添加System.前缀
                throw new System.Exception("where 条件不允许为空");
            }

            BatchOracleHelper oracleHelper = new BatchOracleHelper(QuerySession);
            var columnRowData = GetData(domains, whereCols);

            string[] usedProperies = selectCols.Select(expression => expression.PropertyName()).ToArray();//性能不高
            Table table = GetTable(this.DefaultColumnMapperStrategy);
            SelectBuilder<TDomain> selectBuilder = new SelectBuilder<TDomain>(table, QuerySession.DatabaseInfo.SqlDialect, usedProperies);
            // private string updateTemplate = "UPDATE {0} SET {1} WHERE {2} ";
            var sql = selectBuilder.CreateSql() + where;
            if (string.IsNullOrEmpty(where))
            {
                sql = sql.Substring(0, sql.Length - 7);
            }

            return oracleHelper.Query(sql, columnRowData, rowMapper);
            // throw new NotImplementedException();
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
#if DEBUG
                        throw new ArgumentNullException(expression.PropertyName(), "批量更新包含空值");
#endif
                        //objs[j] = (object)DBNull.Value;
                        objs[j] = GetDefaultValue(type);
                    }
                }
                columnRowData.Add(i, objs);
                i++;
            }
            return columnRowData;
        }
        /// <summary>
        /// 处理空值 只处理string与日期。其他必须有调用方传入
        /// TODO 空值插入dbnull
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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


            //if (type==typeof(bool?))
            //{
            //    return false;
            //}

            throw new ArgumentException("批量操作遇到不能处理的空值");
        }

        /// <summary>
        /// 插入临时表数据
        /// </summary>
        /// <param name="values"></param>
        /// <param name="colName"></param>
        /// <param name="tableName"></param>
        public int AddTempTable(IEnumerable<object> values, string colName, string tableName = "Global_Temp_Search")
        {
            BatchOracleHelper oracleHelper = new BatchOracleHelper(QuerySession);
            string sql = string.Format("insert into {0}({1}) values(:0)", tableName, colName);
            Dictionary<int, object[]> columnRowData = new Dictionary<int, object[]>();
            columnRowData.Add(0, values.ToArray());
            return oracleHelper.BatchInsertOrUpdate(sql, columnRowData);
        }
    }
}
