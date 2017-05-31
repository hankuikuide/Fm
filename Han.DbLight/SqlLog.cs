
namespace Han.DbLight
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq.Expressions;
    using System.Text;
    using System.Diagnostics;

    using Han.Infrastructure;
    using System.Text.RegularExpressions;

    /// <summary>
    /// 根据参数化查询sql，生成可执行sql。不一定与数据库实际执行sql一致
    /// </summary>
    public class SqlLog
    {
        private DatabaseInfo databaseInfo;

        private DbParameterCreater dbParameterCreater=new DbParameterCreater();

        private string parameterConstant;
        public SqlLog(DatabaseInfo databaseInfo)
        {
            this.databaseInfo = databaseInfo;
            parameterConstant = databaseInfo.SqlDialect.DbParameterConstant;
        }
       
        /// <summary>
        /// 参数化查询语句转换为实际sql
        /// </summary>
        /// <param name="sql">参数化sql</param>
        /// <param name="parameters">参数值</param>
        /// <returns></returns>
        public string GetLogSql(string sql, IDictionary<string, object> parameters)
        {
            foreach (var kv in parameters)
            {
                //string regexKey = string.Format(@"{0}{1}(?=[\)\, ]?)", parameterConstant, kv.Key);
                string regexKey = string.Format(@"{0}{1}\b", parameterConstant, kv.Key);
                string part = databaseInfo.DbTypeConverter.ToDbString(kv.Value);
                sql= Regex.Replace(sql, regexKey,part);
              
            }

            return sql;
        }
        /// <summary>
        /// todo
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public string GetLogSql(string sql, object[] parameterValues)
        {
            int i = 0;
            foreach (var obj in parameterValues)
            {
                //todo 1,不能替换12的1
                i++;
                string regexKey = string.Format(@"{0}{1}(?=[\)\, ])", parameterConstant,i);
                string part = databaseInfo.DbTypeConverter.ToDbString(obj);
                sql = Regex.Replace(sql, regexKey, part);

            }

            return sql;
        }
        public string GetLogSql<TProperty>(string sql, Expression<Func<TProperty>> property)
        {
            StringBuilder sb = new StringBuilder(sql);
            string propertyName = dbParameterCreater.GenerateKey(property);
            object value = property.Compile()();
            IDictionary<string, object> names = new Dictionary<string, object>();
            names[propertyName] = value;
            return GetLogSql(sql,names);
        }
       
    }
}
