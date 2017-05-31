
namespace Han.DbLight
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using Han.Infrastructure;
    using Han.EnsureThat;
    using Han.Infrastructure.Reflection;
    using Han.Log;
    using Han.DbLight.Mapper;
    using Han.DbLight.TableMetadata;
    using System.Text.RegularExpressions;

    /// <summary>
    /// 查询对象，仅仅单表查询，不生成join sql
    /// 不处理关系，sql是参数化查询sql 
    /// todo 在dao上增加domain属性于列的映射重载（支持多多等）
    /// </summary>
    /// <typeparam name="TDomain">与数据库建立关联的领域对象</typeparam>
    public class SingleTableDao<TDomain> : ITableDao<TDomain>
        where TDomain : class, new()
    {
        #region Fields

        /// <summary>
        /// 子类必须传入。todo ioc or abstract factory
        /// </summary>
        public IQuerySession QuerySession;
        #endregion

        #region Constructors and Destructors
        //
        public SingleTableDao(IQuerySession querySession, ColumnMapperStrategy mapperStrategy = ColumnMapperStrategy.ColumnAttribute)
        {
            this.DefaultColumnMapperStrategy = mapperStrategy;
            this.QuerySession = querySession;
            if (this.DefaultColumnMapperStrategy == ColumnMapperStrategy.ColumnAttribute)
            {
                this.TableInfoFromAttr = this.QuerySession.DatabaseInfo.TablesInfo.LoadTable(this.DefaultColumnMapperStrategy, typeof(TDomain));
                this.tableName = this.TableInfoFromAttr.TableName;
                encodeTableName = tableName;

            }
            else
            {
                this.TableInfoFromProperty = this.QuerySession.DatabaseInfo.TablesInfo.LoadTable(this.DefaultColumnMapperStrategy, typeof(TDomain));
                this.tableName = this.TableInfoFromProperty.TableName;
                encodeTableName = tableName;
                //不是所有属性都是数据库列。查询schema，然后过滤
                //todo 编译时缓存表结构
                Table dbTable = this.QuerySession.DatabaseInfo.TablesInfo.LoadFromSchema(tableName, this.LoadFromSchema);
                Ensure.That(dbTable).IsNotNull();//确保存在表，不存在表属于异常
                Logger.Log(Level.Debug, () =>
                {
                    //属性对应的列不存记录日志，不属于异常
                    //记录删除的列
                    var cols = this.TableInfoFromProperty.Columns.Where((col) => !dbTable.Columns.Exists((c) => c.ColumnName == col.ColumnName));
                    string message = cols.Aggregate<IColumn, string>(null, (current, col) => current + (col.ColumnName + ","));
                    if (message != null)
                        return tableName + "不存在列：" + message;
                    else
                    {
                        return message;
                    }
                });
                this.TableInfoFromProperty.Columns.RemoveAll((col) => !dbTable.Columns.Exists((c) => c.ColumnName == col.ColumnName));
                //if(dbTable==null)
                //{

                //    ///没有属性定义的表
                //    Logger.Log(Level.Debug, "没有通过类名定义的表：" + tableName);

                //    // todo 返回null 需要修改很多，下版本
                //    TableInfoFromProperty= new Table();

                //}
                //else
                //{

                //}

                //列主键信息的复制，如果数据库表结构中有主键则复制
                this.TableInfoFromProperty.Columns.ForEach(col =>
                {
                    var temp = dbTable.Columns.FirstOrDefault(c => c.ColumnName == col.ColumnName);
                    if (temp != null)
                    {
                        if (temp.IsPrimaryKey)
                            col.IsPrimaryKey = temp.IsPrimaryKey;
                        if (temp.IsAutoInsert)
                            col.IsAutoInsert = temp.IsAutoInsert;
                    }
                });

            }

            this.DefaultAlias = this.GetType().Name.ToLowerInvariant();
            this.SqlLog = new SqlLog(this.QuerySession.DatabaseInfo);
        }

        #endregion

        #region Public Properties

        public string DefaultAlias { get; set; }

        /// <summary>
        /// 列默认映射规则
        /// </summary>
        public ColumnMapperStrategy DefaultColumnMapperStrategy { get; set; }


        //public Capability Capability { get; set; }
        public SqlLog SqlLog { get; protected set; }

        /// <summary>
        /// column attribute定义的表结构
        /// </summary>
        public Table TableInfoFromAttr { get; private set; }

        /// <summary>
        /// 属性定义的表结构
        /// </summary>
        public Table TableInfoFromProperty { get; private set; }

        private string tableName;
        private string encodeTableName;
        #endregion

        #region Public Methods and Operators


        private Table LoadFromSchema()
        {
            // todo 判断是否存在表，表不存在时返回null
            List<IColumn> cols = QuerySession.ExecuteSqlString(QuerySession.DatabaseInfo.SqlDialect.SchemaSql, new PropertyEqualColumnMapper<Column>(), tableName).Cast<IColumn>().ToList();
            if (cols.Count == 0)
                return null;
            var table = new Table(cols);
            table.TableName = tableName;
            return table;
        }
        /// <summary>
        /// 数据数量
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="columnMapperStrategy">领域对象到数据库的映射规则</param>
        /// <returns></returns>
        public virtual int Count(Expression<Func<TDomain, bool>> where, ColumnMapperStrategy columnMapperStrategy)
        {

            var sql = new StringBuilder("SELECT COUNT(*) FROM " + this.encodeTableName + " ");
            SqlBuilder sqlBuilder = this.CreateSqlBuilder(sql);
            if (where != null)
                sqlBuilder.AppendByAutoGen(" where ", where, columnMapperStrategy, false);
            return this.QuerySession.ExecuteScalar<int>(sqlBuilder.ToSql(), sqlBuilder.DbParams);
        }
        /// <summary>
        /// 数据数量
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual int Count(Expression<Func<TDomain, bool>> where)
        {
            return this.Count(where, this.DefaultColumnMapperStrategy);
        }
        /// <summary>
        /// 数据数量count(*)
        /// </summary>
        /// <remarks>采用默认映射规则</remarks>
        /// <returns></returns>
        public virtual int Count()
        {
            return this.Count(null, this.DefaultColumnMapperStrategy);
        }
        /// <summary>
        /// 计算非空列行数count(cols)
        /// </summary>
        /// <typeparam name="TProp">对应数据库列的dommian属性名称</typeparam>
        /// <param name="expression">列名称</param>
        /// <returns></returns>
        public int Count<TProp>(Expression<Func<TDomain, TProp>> expression)
        {

            return this.Count(expression, null, DefaultColumnMapperStrategy);
        }
        /// <summary>
        /// 计算非空列行数count(cols)
        /// </summary>
        /// <typeparam name="TProp">对应数据库列的dommian属性名称</typeparam>
        /// <param name="expression">列名称</param>
        /// <param name="where">查询条件</param>
        /// <param name="columnMapperStrategy">映射策略</param>
        /// <returns></returns>
        public virtual int Count<TProp>(Expression<Func<TDomain, TProp>> expression, Expression<Func<TDomain, bool>> where, ColumnMapperStrategy columnMapperStrategy)
        {
            string property = expression.PropertyName();
            Table table = this.GetTable(columnMapperStrategy);
            List<IColumn> dbCol = table.DbColumns;
            var colName = dbCol.FirstOrDefault(col => col.PropertyName == property);
            Ensure.That(colName).IsNotNull();
            var sql = string.Format("SELECT COUNT({0}) FROM {1}", colName.ColumnName, encodeTableName);


            SqlBuilder sqlBuilder = this.CreateSqlBuilder(new StringBuilder(sql));
            if (where != null)
                sqlBuilder.AppendByAutoGen(" where ", where, columnMapperStrategy, false);
            return this.QuerySession.ExecuteScalar<int>(sqlBuilder.ToSql());
        }
        /// <summary>
        /// 删除所有数据
        /// </summary>
        public virtual void Delete()
        {
            var sql = new StringBuilder("delete " + this.encodeTableName + " ");
            this.QuerySession.ExecuteNonQuery(sql.ToString());
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="where">查询条件，不支持实体属性如：User.Roles.Count==1，Roles为实体属性，非简单属性不能生成正确sql，todo</param>
        public virtual void Delete(Expression<Func<TDomain, bool>> where)
        {
            this.Delete(where, this.DefaultColumnMapperStrategy);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="columnMapperStrategy"></param>
        public virtual void Delete(Expression<Func<TDomain, bool>> where, ColumnMapperStrategy columnMapperStrategy)
        {
            Table table = this.GetTable(columnMapperStrategy);
            DeleteBuilder<TDomain> deleteBuilder = new DeleteBuilder<TDomain>(table, QuerySession.DatabaseInfo.SqlDialect);
            if (where != null)
            {
                SqlBuilder deleteSqlBuilder = this.CreateSqlBuilder(null);
                deleteSqlBuilder.AppendByAutoGen("where", where, columnMapperStrategy, false);
                deleteBuilder.SqlBuilder = deleteSqlBuilder;
            }

            this.QuerySession.ExecuteNonQuery(deleteBuilder.GetSql(), deleteBuilder.DbParams);
        }

        /// <summary>
        /// 插入数据，多主键表未测试
        /// </summary>
        /// <param name="domain">要插入数据库的数据</param>
        /// <typeparam name="TDomain">与数据库建立关联的领域对象</typeparam>
        /// <param name="columnMapperStrategy">映射策略</param>
        /// <param name="usedProperies">TDomain中需要插入到数据库的属性，该属性必须对应数据库字段</param>
        private void Insert(TDomain domain, ColumnMapperStrategy columnMapperStrategy, IList<string> usedProperies)
        {
            //  Ensure.That(usedProperies).HasItems();
            Ensure.That(domain).IsNotNull();
            Table table = this.GetTable(columnMapperStrategy);
            InsertBuilder<TDomain> insertBuilder = new InsertBuilder<TDomain>(table, domain, QuerySession.DatabaseInfo.SqlDialect, usedProperies);

            this.QuerySession.ExecuteNonQuery(insertBuilder.GetSql(), insertBuilder.DbParams);
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="domain">要插入数据库的数据</param>
        /// <param name="columnMapperStrategy">映射策略</param>
        /// <param name="entityProperties">TDomain中需要插入到数据库的属性，为空默认所有列</param>
        public virtual void Insert(TDomain domain, ColumnMapperStrategy columnMapperStrategy, List<Expression<Func<TDomain, object>>> entityProperties)
        {
            Ensure.That(entityProperties).IsNotNull();
            // Ensure.That(entityProperties).HasItems();
            string[] usedProperies = entityProperties.Select(expression => expression.PropertyName()).ToArray();

            this.Insert(domain, columnMapperStrategy, usedProperies);
        }

        /// <summary>
        ///插入数据，使用默认映射策略
        /// </summary>
        /// <param name="domain">要插入数据库的数据</param>
        /// <param name="entityProperties">TDomain中需要插入到数据库的属性，，为空默认所有列</param>
        public virtual void Insert(TDomain domain, List<Expression<Func<TDomain, object>>> entityProperties)
        {

            this.Insert(domain, this.DefaultColumnMapperStrategy, entityProperties);
        }
        /// <summary>
        ///插入数据
        /// </summary>
        /// <param name="domain">要插入数据库的数据</param>
        /// <param name="entityProperties">TDomain中需要插入到数据库的属性，该属性必须对应数据库字段.如不传更新所有列</param>
        public virtual void Insert(TDomain domain, params Expression<Func<TDomain, object>>[] entityProperties)
        {

            this.Insert(domain, this.DefaultColumnMapperStrategy, entityProperties.ToList());
        }
        ///// <summary>
        ///// 插入多条数据，使用事务 todo 批量插入更新，sqlserver SqlBulkCopy  oracle odp.net支持 OracleCommand.ArrayBindCount 
        ///// </summary>
        ///// <param name="domains">要插入数据库的数据列表</param>
        ///// <param name="entityProperties">TDomain中需要插入到数据库的属性，该属性必须对应数据库字段.如不传更新所有列</param>
        //public virtual void Insert(List<TDomain> domains, params Expression<Func<TDomain, object>>[] entityProperties)
        //{
        //    //todo c# 4.5 根据调用方法缓存属性。

        //    using (var transactionScope = new TransactionScope(this.QuerySession))
        //    {
        //        foreach (var domain in domains)
        //        {
        //            this.Insert(domain, entityProperties);
        //        }
        //        transactionScope.Complete();
        //    }
        //}
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="columnMapperStrategy">映射策略</param>
        /// <param name="usedProperies">TDomain中需要插入到数据库的属性，该属性必须对应数据库字段 ,usedProperies为空时查询所有字段</param>
        /// <returns></returns>
        private List<TDomain> Select(Expression<Func<TDomain, bool>> where, ColumnMapperStrategy columnMapperStrategy, params string[] usedProperies)
        {
            //Ensure.That(usedProperies).HasItems();
            IRowMapper<TDomain> rowMapper = null;
            if (columnMapperStrategy == ColumnMapperStrategy.Property)
                rowMapper = new PropertyEqualColumnMapper<TDomain>();
            else
                rowMapper = new ColumnAttributeMapper<TDomain>();
            Table table = this.GetTable(columnMapperStrategy);
            SelectBuilder<TDomain> selectBuilder = new SelectBuilder<TDomain>(table, QuerySession.DatabaseInfo.SqlDialect, usedProperies);
            SqlBuilder whereSqlBuilder = this.CreateSqlBuilder(null);
            whereSqlBuilder.AppendByAutoGen("where", where, columnMapperStrategy, false);
            selectBuilder.SqlBuilder = whereSqlBuilder;
            return this.QuerySession.ExecuteSqlString(selectBuilder.GetSql(), rowMapper, selectBuilder.DbParams).ToList();
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="where"></param>
        /// <param name="columnMapperStrategy"></param>
        /// <param name="entityProperties">TDomain中需要插入到数据库的属性，为空默认所有列</param>
        /// <returns></returns>
        public virtual List<TDomain> Select(Expression<Func<TDomain, bool>> where, ColumnMapperStrategy columnMapperStrategy, Expression<Func<TDomain, object>>[] entityProperties)
        {
            Ensure.That(entityProperties).IsNotNull();
            // Ensure.That(entityProperties).HasItems();
            string[] usedProperies = entityProperties.Select(expression => expression.PropertyName()).ToArray();
            return this.Select(where, columnMapperStrategy, usedProperies);
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="where"></param>
        /// <param name="entityProperties">TDomain中需要插入到数据库的属性，该属性必须对应数据库字段.如不传更新所有列</param>
        /// <returns></returns>
        public virtual List<TDomain> SelectByExp(Expression<Func<TDomain, bool>> where, params Expression<Func<TDomain, object>>[] entityProperties)
        {
            string[] usedProperies = null;
            if (entityProperties.Any())
            {
                usedProperies = entityProperties.Select(expression => expression.PropertyName()).ToArray();
            }
            return this.Select(where, DefaultColumnMapperStrategy, usedProperies);
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="entityPropertiesCreator">属性生成器，返回查询的属性列表</param>
        /// <returns></returns>
        public virtual List<TDomain> Select(Expression<Func<TDomain, bool>> where, Func<string[]> entityPropertiesCreator)
        {
            return this.Select(where, this.DefaultColumnMapperStrategy, entityPropertiesCreator());
        }

        /// <summary>
        /// insert或者update对象列表，如果主键不为空则insert，否则update
        /// </summary>
        /// <param name="domain">要update或者insert的数据</param>
        /// <param name="columnMapperStrategy">映射策略</param>
        /// <param name="entityProperties"></param>
        /// <returns></returns>
        public virtual void Save(TDomain domain, ColumnMapperStrategy columnMapperStrategy, List<Expression<Func<TDomain, object>>> entityProperties)
        {
            Ensure.That(domain).IsNotNull();
            // Ensure.That(entityProperties).HasItems();
            Table table = this.GetTable(columnMapperStrategy);
            IColumn key = table.KeyColumns.FirstOrDefault();
            Ensure.That(key).IsNotNull();
            object keyValue = Reflection.GetPropertyValue(domain, key.PropertyName);
            if (keyValue == null)
            {
                this.Insert(domain, columnMapperStrategy, entityProperties);
            }
            else
            {
                this.Update(domain, columnMapperStrategy, entityProperties);
            }
        }
        /// <summary>
        /// insert或者update对象列表，如果主键不为空则insert，否则update
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="entityProperties">TDomain中需要插入到数据库的属性，为空默认所有列</param>
        public virtual void Save(TDomain domain, List<Expression<Func<TDomain, object>>> entityProperties)
        {
            Ensure.That(domain).IsNotNull();
            //Ensure.That(entityProperties).HasItems();
            this.Save(domain, this.DefaultColumnMapperStrategy, entityProperties);
        }
        /// <summary>
        /// insert或者update对象列表，如果主键不为空则insert，否则update
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="entityProperties">TDomain中需要插入到数据库的属性，为空默认所有列</param>
        public virtual void Save(TDomain domain, params Expression<Func<TDomain, object>>[] entityProperties)
        {
            Ensure.That(domain).IsNotNull();
            //  Ensure.That(entityProperties).HasItems();
            this.Save(domain, this.DefaultColumnMapperStrategy, entityProperties.ToList());
        }
        /// <summary>
        /// insert或者update对象列表，如果主键不为空则insert，否则update
        /// </summary>
        /// <param name="domains"></param>
        /// <param name="entityProperties">TDomain中需要插入到数据库的属性，该属性必须对应数据库字段，不能为空</param>
        public virtual void Save(List<TDomain> domains, params Expression<Func<TDomain, object>>[] entityProperties)
        {
            //todo c# 4.5 根据调用方法缓存属性。
            Ensure.That(domains).HasItems();
            //Ensure.That(entityProperties).HasItems();
            using (var transactionScope = new TransactionScope(this.QuerySession))
            {
                foreach (var domain in domains)
                {
                    this.Save(domain, entityProperties);
                }
                transactionScope.Complete();
            }
        }
        /// <summary>
        /// 根据主键执行update
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="columnMapperStrategy">映射方式 </param>
        /// <param name="usedProperies">TDomain中需要插入到数据库的属性，为空默认所有列</param>
        /// <returns></returns>
        private void InternalUpdate(TDomain domain, ColumnMapperStrategy columnMapperStrategy, IList<string> usedProperies)
        {
            Ensure.That(domain).IsNotNull();
            // Ensure.That(usedProperies).HasItems();
            Table table = this.GetTable(columnMapperStrategy);
            UpdateBuilder<TDomain> updateBuilder = new UpdateBuilder<TDomain>(table, QuerySession.DatabaseInfo.SqlDialect, domain, usedProperies);

            this.QuerySession.ExecuteNonQuery(updateBuilder.GetSql(), updateBuilder.DbParams);
        }
        /// <summary>
        /// 根据条件执行更新
        /// 参考ef接口
        /// context.Categories.Update(c => c.Name.EndsWith("1"), c => new Category() { Name = "Update" });
        /// 第一个参数where
        /// 第二个返回tdomain。对应于TDomain domain
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="columnMapperStrategy"></param>
        /// <param name="where"></param>
        /// <param name="entityProperties"></param>
        public virtual void Update(TDomain domain, ColumnMapperStrategy columnMapperStrategy, Expression<Func<TDomain, bool>> where, List<Expression<Func<TDomain, object>>> entityProperties)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///根据主键执行update
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="columnMapperStrategy"></param>
        /// <param name="entityProperties">为空默认所有列</param>
        public virtual void Update(TDomain domain, ColumnMapperStrategy columnMapperStrategy, List<Expression<Func<TDomain, object>>> entityProperties)
        {
            Ensure.That(domain).IsNotNull();
            //Ensure.That(entityProperties).HasItems();
            string[] usedProperies = entityProperties.Select(expression => expression.PropertyName()).ToArray();
            InternalUpdate(domain, columnMapperStrategy, usedProperies);
        }
        /// <summary>
        /// 根据主键执行update
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="entityProperties">为空默认所有列</param>
        public virtual void Update(TDomain domain, List<Expression<Func<TDomain, object>>> entityProperties)
        {
            this.Update(domain, this.DefaultColumnMapperStrategy, entityProperties);
        }
        /// <summary>
        /// 根据主键执行update
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="entityProperties"></param>
        public virtual void Update(TDomain domain, params Expression<Func<TDomain, object>>[] entityProperties)
        {
            //todo 增加关系映射处理。如果多对多...
            this.Update(domain, this.DefaultColumnMapperStrategy, entityProperties.ToList());
        }

        #endregion


        #region Methods

        protected SqlBuilder CreateSqlBuilder(StringBuilder sql)
        {

            var sqlbuilder = new SqlBuilder(this.QuerySession.DatabaseInfo, sql, new Dictionary<string, object>());
            return sqlbuilder;
        }

        protected SqlBuilder CreateSqlBuilder(StringBuilder sql, IDictionary<string, object> dbParams)
        {
            var sqlbuilder = new SqlBuilder(this.QuerySession.DatabaseInfo, sql, dbParams);
            return sqlbuilder;
        }



        protected Table GetTable(ColumnMapperStrategy columnMapperStrategy)
        {
            if (columnMapperStrategy == ColumnMapperStrategy.ColumnAttribute)
            {
                if (this.TableInfoFromAttr != null)
                {
                    return this.TableInfoFromAttr;
                }
                else
                {
                    this.TableInfoFromAttr = this.QuerySession.DatabaseInfo.TablesInfo.LoadTable(columnMapperStrategy, typeof(TDomain));
                    return this.TableInfoFromAttr;
                }
            }
            else
            {
                if (this.TableInfoFromProperty != null)
                {
                    return this.TableInfoFromProperty;
                }
                else
                {
                    this.TableInfoFromProperty = this.QuerySession.DatabaseInfo.TablesInfo.LoadTable(columnMapperStrategy, typeof(TDomain));
                    return this.TableInfoFromProperty;
                }
            }
        }
        #endregion
    }

}