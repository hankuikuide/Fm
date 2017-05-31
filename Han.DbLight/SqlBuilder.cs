

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
    /// ����������sql���
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
        /// ���Ӳ�����sql
        /// </summary>
        /// <param name="property">ֻ���Ǽ�����</param>
        /// <param name="sqlPart">��Ҫ���ӵ�sql������չλ��Ϊ{0}</param>
        ///<remarks></remarks>
        /// <code>AppendWhereNotNull(() => startDate, "and COUNT_MONTH>={0}");</code>
        public SqlBuilder AppendWhereNotNull<TProperty>(Expression<Func<TProperty>> property, string sqlPart)
        {
            if (!(property.Body is MemberExpression) && !(property.Body is MethodCallExpression))
            {
                throw new ArgumentException("propery ���ʽ����ΪMemberExpression����MethodCallExpression");
            }
            Func<bool> condition = null;
            TProperty value = property.Compile()();
           // Ensure.That(value.GetType().IsPrimitive(), "�������Ͳ���Ϊ��������");
            
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
        /// ���Ӳ�����sql
        /// </summary>
        /// <param name="property"></param>
        /// <param name="sqlPart"></param>
        /// <param name="valueConverter"></param>
        /// <returns></returns>
        public SqlBuilder AppendWhereNotNullOrEmpty(Expression<Func<string>> property, string sqlPart, string valueConverter = null)
        {
            if (!(property.Body is MemberExpression) && !(property.Body is MethodCallExpression))
            {
                throw new ArgumentException("propery ���ʽ����ΪMemberExpression����MethodCallExpression");
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
        /// �ǿղ���-1ʱ����
        /// todo ɾ���˷���ʹ��Append
        /// </summary>
        /// <param name="property"></param>
        /// <param name="sqlPart"></param>
        /// <returns></returns>
        public SqlBuilder AppendWhereNotNullPositive(Expression<Func<string>> property, string sqlPart)
        {
            if (!(property.Body is MemberExpression) && !(property.Body is MethodCallExpression))
            {
                throw new ArgumentException("propery ���ʽ����ΪMemberExpression����MethodCallExpression");
            }
            Func<bool> condition = null;
            string value = property.Compile()();
            condition = () => !string.IsNullOrEmpty(value) && value != "-1";
            sql.Append(InternalConcatSql(condition, value, property, sqlPart));
            return this;
        }

        /// <summary>
        /// �Զ�����sql���,Ҫ��TDomain��������Ϣ���������Ե����������ݿ�����һ�¡�
        /// </summary>
        /// <typeparam name="TDomain">��Ӧ�����ݿ���ʵ�壬����ʵ������sql</typeparam>
        /// <param name="preString">ǣ���ı�</param>
        /// <param name="property">where���������Ա��ʽ</param>
        /// <param name="mapperStrategy">ӳ����ԣ���ΰ���������ӳ��Ϊ��</param>
        /// <param name="isUseAlias">�Ƿ�ʹ�ñ�������sql </param>
        /// <returns></returns>
        /// <remarks>��</remarks>
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
        /// ���� in ������sql
        /// ���Ͳ���Ϊ��string int char��������
        /// </summary>
        /// <param name="property">in ֵ</param>
        /// <param name="sqlPart">sql ����</param>
        /// <returns></returns>
        public SqlBuilder AppendInWhereHasValue<T>(Expression<Func<List<T>>> property, string sqlPart)
        {
            if (!(property.Body is MemberExpression) && !(property.Body is MethodCallExpression))
            {
                throw new ArgumentException("propery ���ʽ����ΪMemberExpression����MethodCallExpression");
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
                    //���Ӳ�����
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
        /// ����property list�ĳ��ȣ�����sql���������Ϊ1������=������1 ����in ��list���Ȳ��ܴ���1000
        /// sqlPart ������in �� =
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="sqlPart"></param>
        /// <returns></returns>
        public SqlBuilder Append<T>(Expression<Func<List<T>>> property, string sqlPart)
        {
            if (!(property.Body is MemberExpression) && !(property.Body is MethodCallExpression))
            {
                throw new ArgumentException("propery ���ʽ����ΪMemberExpression����MethodCallExpression");
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
                            //���Ӳ�����
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
        /// conditionΪtrueʱ������sql
        /// </summary>
        /// <param name="condition">Ϊtrue������sql</param>
        /// <param name="property">ֵ</param>
        /// <param name="sqlPart"></param>
        /// <returns></returns>
        public SqlBuilder Append<TProperty>(Func<bool> condition, Expression<Func<TProperty>> property, string sqlPart)
        {
            if (!(property.Body is MemberExpression) && !(property.Body is MethodCallExpression))
            {
                throw new ArgumentException("propery ���ʽ����ΪMemberExpression����MethodCallExpression");
            }

            object value = property.Compile()();
            Ensure.That(value.GetType().IsPrimitive(), "�������Ͳ���Ϊ��������");
            sql.Append(InternalConcatSql(condition, value, property, sqlPart));
            return this;
        }
        /// <summary>
        /// ����������sql������sql
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property">ֵ</param>
        /// <param name="sqlPart">���ӵ�sql</param>
        /// <returns></returns>
        public SqlBuilder Append<TProperty>(Expression<Func<TProperty>> property, string sqlPart)
        {
            if (!(property.Body is MemberExpression) && !(property.Body is MethodCallExpression))
            {
                throw new ArgumentException("propery ���ʽ����ΪMemberExpression����MethodCallExpression");
            }
            Func<bool> condition = null;
            TProperty value = property.Compile()();
            Ensure.That(value.GetType().IsPrimitive(), "�������Ͳ���Ϊ��������");
            condition = () => true;
            sql.Append(InternalConcatSql(condition, value, property, sqlPart));
            return this;
        }
        /// <summary>
        /// ���ӷǲ�����sql
        /// </summary>
        /// <param name="sqlPart"></param>
        /// <returns></returns>
        public SqlBuilder Append(StringBuilder sqlPart)
        {
            this.sql.Append(sqlPart);
            return this;
        }
        /// <summary>
        /// conditionΪtrueʱ�����ӷǲ�����sql
        /// </summary>
        /// <param name="condition">Ϊtrue������sql</param>
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
        /// ���ӷǲ�����sql
        /// </summary>
        /// <param name="sqlPart"></param>
        /// <returns></returns>
        public SqlBuilder Append(string sqlPart)
        {
            this.sql.Append(sqlPart);
            return this;
        }
        /// <summary>
        /// ��ȡsql
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
        /// <param name="property">����֧�ַ������ԣ��ֶ�</param>
        /// <param name="sqlPart"></param>
        /// <returns></returns>
        private string InternalConcatSql<TProperty>(Func<bool> condition, dynamic value, Expression<Func<TProperty>> property, string sqlPart)
        {
            string partSql = null;
            if (property.Body is MemberExpression || property.Body is MethodCallExpression)
            {

                string propertyName = dbParameterCreater.GenerateKey(property);// property.GetPropertySymbol().Replace(".", ""); //��ֹ�����������Ը�����
                if (condition())
                {
                    partSql = string.Format(sqlPart + " ", dbParameterConstant + propertyName);
                    //���Ӳ�����
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

            throw new ArgumentException("property ����������");
        }



        private string InternalConcatSql<TProperty>(Func<bool> condition, Expression<Func<TProperty>> property, string sqlPart)
        {
            string partSql = null;
            if (property.Body is MemberExpression)
            {
                string propertyName = dbParameterCreater.GenerateKey(property);//property.GetPropertySymbol().Replace(".", ""); //��ֹ�����������Ը�����
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
            throw new ArgumentException("property ����������");
        }
        #endregion
    }
}