
namespace Han.DbLight.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    using Han.Infrastructure;
    using Han.Infrastructure.Reflection;
    using Han.Log;
    using Han.DbLight.Extensions;
    using Han.DbLight.TableMetadata;

    

    /// <summary>
    /// 通过类型上的ColumnMaperAttribute映射,对于复杂类型的属性通过MetaData#ItemName映射。
    /// </summary>
    /// <remarks>#指明映射时：属性名称作为列名，并且作为属性分隔符</remarks>
    public class ColumnAttributeMapper<T> : IRowMapper<T>
        where T :  new()
    {
        #region Fields

    //    private string sql;
    //public ColumnAttributeMapper(string sql)
    //{
    //    this.sql = sql;
    //}
        public ColumnAttributeMapper(TablesInfo tablesInfo)
        {
            this.TablesInfo = tablesInfo;
        }

        public ColumnAttributeMapper()
    {
       
    }
        //protected IDictionary<string, string> ColumnToProperty = new Dictionary<string, string>();
        protected List<IColumn>  Columns=new List<IColumn>();
        #endregion

        #region Public Methods and Operators

        public T MapRow(IDataRecord row)
        {
            var mappedObj = new T();
           
            if (this.Columns.Count == 0)
            {
                var tableColumns= this.InitTableMetadata(typeof(T));
                foreach (var col in row.GetColumns())
                {
                    if(col.Contains("#"))
                    {
                        var spl = col.Split('#').Where(s=>!string.IsNullOrEmpty(s)).ToList();
                        if(spl.Count==1)
                        {
                            //处理Item#列，列通过属性映射而不是attribute
                            Columns.Add(new Column() { ColumnName = col, PropertyName = spl.First(), IsComplex = false, IsAliasColumn = true });
                        }
                        else
                        {
                            //处理Item#Id列，列通过属性映射而不是attribute
                            Columns.Add(new Column() { ColumnName = col, PropertyName = col, IsComplex = true, IsAliasColumn = true });
                        }
                       
                    }
                    else
                    {
                        //通过attribute映射的不区分大小写。oracle中不区分。sqlserver？？是否区分
                        var cols= tableColumns.Where(c => c.ColumnName.ToUpperInvariant() == col.ToUpperInvariant()).ToList();
                        if(cols.Count!=0)
                        {
                            foreach (var tableCol in cols)
                            {
                                Columns.Add(tableCol);
                            }
                        }
                        else
                        {
#if DEBUG
                            throw new System.Exception(string.Format("ColumnAttributeMapper:列{0}在对象{1}中没有对应的属性", col, typeof(T)));
#endif
                            Logger.Log(Level.Trace, string.Format("ColumnAttributeMapper:列{0}在对象{1}中没有对应的属性", col, typeof(T)));
                                   
                        }
                        //var tableCol = tableColumns.FirstOrDefault(c => c.ColumnName.ToUpperInvariant() == col.ToUpperInvariant());
                        //if(tableCol==null)
                        //{
                        //    //表的元数据中没有定义列
                        //    Logger.Log(Level.Trace, string.Format("ColumnAttributeMapper:列{0}在对象{1}中没有对应的属性", col, typeof(T)));
                        //    //throw new ColumnMapException(col);
                        //}
                        //else
                        //{
                        //    Columns.Add(tableCol);
                        //}
                    }
                }
                
            }
            if (this.Columns.Count != 0)
            {
                foreach (var column in this.Columns)
                {
                    
                    object value;

                    if(column.IsComplex)
                    {
                        if (DbHelper.TryGetColumnValue(row, column.ColumnName, out value))
                        {
                            var propertyPath = column.PropertyName.Replace("#", ".");
                            Reflection.SetPropertyValueByPath(mappedObj, value, propertyPath);
                        }
                       
                    }
                    else
                    {
                        if (DbHelper.TryGetColumnValue(row, column.ColumnName, out value))
                        {
                           
                            Reflection.SetPropertyValue(mappedObj, value, column.PropertyName);
                        }
                    }
                    
                }
            }

            return mappedObj;
        }

        public TablesInfo TablesInfo { get; set; }
        #endregion

        #region Methods

        private List<IColumn> InitTableMetadata(Type type)
        {
            Table tableMetadate = TablesInfo.LoadTable(ColumnMapperStrategy.ColumnAttribute,type);
          
            return  tableMetadate.Columns;
         
        }

        #endregion
    }

}