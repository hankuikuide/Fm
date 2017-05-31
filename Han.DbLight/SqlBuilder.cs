

using Han.EnsureThat;

namespace Han.DbLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;
    using System.Linq;
    using Han.Infrastructure.Extensions;
    using Han.Infrastructure;
    /// <summary>
    /// 构建参数化sql语句
    /// </summary>
    public class SqlBuilder
    {
        public DatabaseInfo DatabaseInfo { get; set; }
        private DbParameterCreater dbParameterCreater = new DbParameterCreater();
        private string dbParameterConstant;
        private IDictionary<string, object> dbParams;
        public IDictionary<string, object> DbParams
        {
            get
            {
                return dbParams;
            }
        }
        private readonly StringBuilder sql;


        public SqlBuilder(DatabaseInfo databaseInfo, StringBuilder sql, IDictionary<string, object> dbParams = null)
        {
            DatabaseInfo = databaseInfo;
            dbParameterConstant = DatabaseInfo.SqlDialect.DbParameterConstant;
            this.dbParams = dbParams;
            if (sql == null)
                this.sql = new StringBuilder();
            else
            {
                this.sql = sql;
            }

        }
        #region Public Methods and Operators

        /// <summary>
        /// 增加参数化sql
        /// </summary>
        /// <param name="property">只能是简单类型</param>
        /// <param name="sqlPart">将要增加的sql，参数展位符为{0}</param>
        ///<remarks></remarks>
        /// <code>AppendWhereNotNull(() => startDate, "and COUNT_MONTH>={0}");</code>
        public SqlBuilder AppendWhereNotNull<TProperty>(Expression<Func<TProperty>> property, string sqlPart)
        {
            if (!(property.Body is MemberExpression) && !(property.Body is MethodCallExpression))
            {
                throw new ArgumentException("propery 表达式必须为MemberExpression或者MethodCallExpression");
            }
            Func<bool> condition = null;
            TProperty value = property.Compile()();
           // Ensure.That(value.GetType().IsPrimitive(), "属性类型不能为复杂类型");
            
            condition = () => value != null&& value.GetType().IsPrimitive();
            sql.Append(InternalConcatSql(condition, value, property, sqlPart));

            return this;
        }

        //public SqlBuilder AppendWhereNotNullOrEmpty(Expression<Func<string>> property, string sqlPart)
        //{
        //    Func<bool> condition = null;
        //    string value = property.Compile()();
        //    condition = () => !string.IsNullOrEmpty(value);
        //    sql.Append(InternalConcatSql(condition, value, property, sqlPart));
        //    return this;
        //}

        /// <summary>
        /// 增加参数化sql
        /// </summary>
        /// <param name="property"></param>
        /// <param name="sqlPart"></param>
        /// <param name="valueConverter"></param>
        /// <returns></returns>
        public SqlBuilder AppendWhereNotNullOrEmpty(Expression<Func<string>> property, string sqlPart, string valueConverter = null)
        {
            if (!(property.Body is MemberExpression) && !(property.Body is MethodCallExpression))
            {
                throw new ArgumentException("propery 表达式必须为MemberExpression或者MethodCallExpression");
            }
            Func<bool> condition = null;
            string value = property.Compile()();
            if (!string.IsNullOrEmpty(value) && valueConverter != null)
            {
                value = string.Format(valueConverter, value);
            }
            condition = () => !string.IsNullOrEmpty(value);
            sql.Append(InternalConcatSql(condition, value, property, sqlPart));
            return this;
        }

        /// <summary>
        /// 非空不是-1时连接
        /// todo 删除此方法使用Append
        /// </summary>
        /// <param name="property"></param>
        /// <param name="sqlPart"></param>
        /// <returns></returns>
        public SqlBuilder AppendWhereNotNullPositive(Expression<Func<string>> property, string sqlPart)
        {
            if (!(property.Body is MemberExpression) && !(property.Body is MethodCallExpression))
            {
                throw new ArgumentException("propery 表达式必须为MemberExpression或者MethodCallExpression");
            }
            Func<bool> condition = null;
            string value = property.Compile()();
            condition = () => !string.IsNullOrEmpty(value) && value != "-1";
            sql.Append(InternalConcatSql(condition, value, property, sqlPart));
            return this;
        }

        /// <summary>
        /// 自动生成sql语句,要求TDomain上有列信息。并且属性的类型与数据库类型一致。
        /// </summary>
        /// <typeparam name="TDomain">对应于数据库表的实体，根据实体生成sql</typeparam>
        /// <param name="preString">牵制文本</param>
        /// <param name="property">where条件的属性表达式</param>
        /// <param name="mapperStrategy">映射策略，如何把属性名称映射为列</param>
        /// <param name="isUseAlias">是否使用别名生成sql </param>
        /// <returns></returns>
        /// <remarks>当</remarks>
        internal SqlBuilder AppendByAutoGen<TDomain>(string preString, Expression<Func<TDomain, bool>> property, ColumnMapperStrategy mapperStrategy, bool isUseAlias = true)
        {

            IWhereQueryTranslator whereQueryTranslator = new WhereQueryTranslator(DatabaseInfo);
            whereQueryTranslator.IsUseAlias = isUseAlias;
            whereQueryTranslator.ColumnMapperStrategy = mapperStrategy;
            whereQueryTranslator.DbParams = dbParams;
            whereQueryTranslator.Table = DatabaseInfo.TablesInfo.LoadTable(mapperStrategy, typeof(TDomain));
            string whereClause = whereQueryTranslator.Translate(property);
            if (!string.IsNullOrEmpty(whereClause))
            {
                whereClause = " " + preString + " " + whereClause;
            }
            sql.Append(whereClause);
            return this;
        }
        /// <summary>
        /// 增加 in 参数化sql
        /// 泛型参数为：string int char基本类型
        /// </summary>
        /// <param name="property">in 值</param>
        /// <param name="sqlPart">sql 语句块</param>
        /// <returns></returns>
        public SqlBuilder AppendInWhereHasValue<T>(Expression<Func<List<T>>> property, string sqlPart)
        {
            if (!(property.Body is MemberExpression) && !(property.Body is MethodCallExpression))
            {
                throw new ArgumentException("propery 表达式必须为MemberExpression或者MethodCallExpression");
            }
            string partSql = null;
            List<T> value = property.Compile()();
            if (value != null && value.Any() && dbParams != null && property.Body is MemberExpression)
            {

                string name = dbParameterCreater.GenerateKey(property);
                var collectionParams = new StringBuilder(dbParameterConstant);
                var counter = 0;
                foreach (var obj in value)
                {
                    var param = name + counter;
                    //增加参数化
                    if (!DbParams.ContainsKey(param))
                    {
                        dbParams.Add(param, obj);
                    }
                    //dbParams.Add(param, obj);
                    collectionParams.Append(param);
                    collectionParams.Append(", " + dbParameterConstant);
                    counter++;
                }
                collectionParams.Remove(collectionParams.Length - 3, 3);
                partSql = string.Format(sqlPart + " ", collectionParams);
                sql.Append(partSql);
            }
            return this;

        }
        /// <summary>
        /// 根据property list的长度，生成sql，如果长度为1，生成=，大于1 生成in ，list长度不能大于1000
        /// sqlPart 不包含in 或 =
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="sqlPart"></param>
        /// <returns></returns>
        public SqlBuilder Append<T>(Expression<Func<List<T>>> property, string sqlPart)
        {
            if (!(property.Body is MemberExpression) && !(property.Body is MethodCallExpression))
            {
                throw new ArgumentException("propery 表达式必须为MemberExpression或者MethodCallExpression");
            }
            List<T> value = property.Compile()();
            if (value != null && value.Any() && dbParams != null && property.Body is MemberExpression)
            {
                var builder = new StringBuilder();

                var index = sqlPart.IndexOf('(');
                var hasOnlyOne = value.Count == 1;
                sqlPart = sqlPart.Insert(index - 1, hasOnlyOne ? " = " : " in ");

                if(hasOnlyOne)
                {
                    sqlPart = sqlPart.Replace('(', ' ').Replace(')',' ');
                }

                if (value.Count == 1)
                {
                    sql.Append(InternalConcatSql(() => true, value.First(), property, sqlPart));
                }
                else
                {
                    var andIndex = sqlPart.ToLower().IndexOf("and");

                    var tempSqlPart = sqlPart.Substring(andIndex + 3 );

                    string name = dbParameterCreater.GenerateKey(property);
                    var counter = 0;

                    var pageSize = 1000;
                    var pageCount = (int)Math.Ceiling((decimal)value.Count / pageSize);

                    for (int i = 0; i < pageCount; i++)
                    {
                        var collectionParams = new StringBuilder();
                        var pageList = value.Skip((i) * pageSize).Take(pageSize).ToList();

                        if (i > 0)
                        {
                            builder.Append(" or ");
                        }

                        var tempIndex = 0;
                        foreach (var item in pageList)
                        {
                            var param = name + counter;
                            //增加参数化
                            if (!DbParams.ContainsKey(param))
                            {
                                dbParams.Add(param, item);
                            }

                            if (tempIndex > 0)
                            {
                                collectionParams.Append(",");
                            }

                            collectionParams.AppendFormat("{0}{1}", dbParameterConstant, param);

                            ++counter;
                            tempIndex++;
                        }

                        builder.Append(string.Format(tempSqlPart, collectionParams));                        
                    }

                    if(pageCount > 1)
                    {
                        sql.Append(string.Format(" and ( {0} ) ",builder.ToString()));
                    }
                    else
                    {
                        sql.Append(string.Format(" and {0} ", builder.ToString()));
                    }
                }
            }

            return this;

        }

        /// <summary>
        /// condition为true时，增加sql
        /// </summary>
        /// <param name="condition">为true是连接sql</param>
        /// <param name="property">值</param>
        /// <param name="sqlPart"></param>
        /// <returns></returns>
        public SqlBuilder Append<TProperty>(Func<bool> condition, Expression<Func<TProperty>> property, string sqlPart)
        {
            if (!(property.Body is MemberExpression) && !(property.Body is MethodCallExpression))
            {
                throw new ArgumentException("propery 表达式必须为MemberExpression或者MethodCallExpression");
            }

            object value = property.Compile()();
            Ensure.That(value.GetType().IsPrimitive(), "属性类型不能为复杂类型");
            sql.Append(InternalConcatSql(condition, value, property, sqlPart));
            return this;
        }
        /// <summary>
        /// 无条件增加sql，参数sql
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property">值</param>
        /// <param name="sqlPart">增加的sql</param>
        /// <returns></returns>
        public SqlBuilder Append<TProperty>(Expression<Func<TProperty>> property, string sqlPart)
        {
            if (!(property.Body is MemberExpression) && !(property.Body is MethodCallExpression))
            {
                throw new ArgumentException("propery 表达式必须为MemberExpression或者MethodCallExpression");
            }
            Func<bool> condition = null;
            TProperty value = property.Compile()();
            Ensure.That(value.GetType().IsPrimitive(), "属性类型不能为复杂类型");
            condition = () => true;
            sql.Append(InternalConcatSql(condition, value, property, sqlPart));
            return this;
        }
        /// <summary>
        /// 增加非参数化sql
        /// </summary>
        /// <param name="sqlPart"></param>
        /// <returns></returns>
        public SqlBuilder Append(StringBuilder sqlPart)
        {
            this.sql.Append(sqlPart);
            return this;
        }
        /// <summary>
        /// condition为true时，增加非参数化sql
        /// </summary>
        /// <param name="condition">为true是连接sql</param>
        /// <param name="sqlPart"></param>
        /// <returns></returns>
        public SqlBuilder Append(bool condition, string sqlPart)
        {
            if (condition)
            {
                this.sql.Append(sqlPart);
            }
            return this;
        }
        /// <summary>
        /// 增加非参数化sql
        /// </summary>
        /// <param name="sqlPart"></param>
        /// <returns></returns>
        public SqlBuilder Append(string sqlPart)
        {
            this.sql.Append(sqlPart);
            return this;
        }
        /// <summary>
        /// 获取sql
        /// </summary>
        /// <returns></returns>
        public string ToSql()
        {
            return sql.ToString();
        }
        public override string ToString()
        {
            return ToSql();
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="condition"></param>
        /// <param name="value"></param>
        /// <param name="property">仅仅支持方法属性，字段</param>
        /// <param name="sqlPart"></param>
        /// <returns></returns>
        private string InternalConcatSql<TProperty>(Func<bool> condition, dynamic value, Expression<Func<TProperty>> property, string sqlPart)
        {
            string partSql = null;
            if (property.Body is MemberExpression || property.Body is MethodCallExpression)
            {

                string propertyName = dbParameterCreater.GenerateKey(property);// property.GetPropertySymbol().Replace(".", ""); //防止重名回溯属性父对象
                if (condition())
                {
                    partSql = string.Format(sqlPart + " ", dbParameterConstant + propertyName);
                    //增加参数化
                    if (dbParams != null)
                    {
                        if (!DbParams.ContainsKey(propertyName))
                        {
                            dbParams.Add(propertyName, value);
                        }

                    }
                    return partSql;
                }
                else
                {
                    return null;
                }
            }

            throw new ArgumentException("property 必须是属性");
        }



        private string InternalConcatSql<TProperty>(Func<bool> condition, Expression<Func<TProperty>> property, string sqlPart)
        {
            string partSql = null;
            if (property.Body is MemberExpression)
            {
                string propertyName = dbParameterCreater.GenerateKey(property);//property.GetPropertySymbol().Replace(".", ""); //防止重名回溯属性父对象
                if (condition())
                {
                    partSql = string.Format(sqlPart + " ", dbParameterConstant + propertyName);
                    return partSql;
                }
                else
                {
                    return null;
                }
            }
            throw new ArgumentException("property 必须是属性");
        }
        #endregion
    }
}