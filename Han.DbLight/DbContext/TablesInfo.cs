
namespace Han.DbLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Han.Cache;
    using Han.Infrastructure;
    using Han.Infrastructure.Extensions;
    using Han.DbLight.Mapper;
    using Han.DbLight.TableMetadata;

    /// <summary>
    /// 数据库中的表信息，每个数据库一个
    /// </summary>
    public class TablesInfo
    {
        #region Constants

        private const string columnAttrMapTableName = "columnAttrTableName*{0}";
       // private const string dtoMapTableName = "dtoTableName*{0}";
        private const string propertyMapTableName = "propertyTableName*{0}";
        private const string dbSchemaMapTableName = "schemaTableName*{0}";
        #endregion

        #region Static Fields

        private static readonly ICacheProvider<Table> tableCache = new LruMemoryCache<Table>();

        private ISqlDialect dialect;

        #endregion

        #region Public Methods and Operators
       
        public TablesInfo(ISqlDialect dialect)
        {
            this.dialect = dialect;
            //this.schemaSql = schemaSql;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Table LoadTable(ColumnMapperStrategy columnMapperStrategy, Type type)
        {
            if (columnMapperStrategy == ColumnMapperStrategy.ColumnAttribute)
            {
                return this.LoadFromMetadata(type);
            }
            else
            {
                return this.LoadFromProperty(type);
            }
        }
        public bool IsSupportAttributeMap(Type type)
        {
            TableAttribute tableAttr = type.GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
            return tableAttr != null;
        }
        public Table LoadFromSchema(string tableName,Func<Table> getTable)
        {
            string key = string.Format(dbSchemaMapTableName, tableName);
            return tableCache.GetOrAdd(key, getTable);
           
        }
        #endregion

        //public static Table LoadFromDb(string tableName)
        //{

        //    return null;

        //}

        #region Methods

        private Table GetTableFromProperty(Type type)
        {
            var table = new Table();
            table.TableName = type.Name;
            List<PropertyInfo> props = type.GetPrimitivePropertys();
            foreach (PropertyInfo property in props)
            {
                var tt  = property.GetCustomAttributes(true);
                PrimaryKeyAttribute columnMaperAttribute = property.GetCustomAttributes(true).OfType<PrimaryKeyAttribute>().FirstOrDefault();
                IColumn t = columnMaperAttribute;
                if (t != null)
                {
                    //主键
                    t.PropertyName = property.Name;
                    t.PropertyType = property.PropertyType;
                    table.Columns.Add(t);
                }
                else
                {
                    table.Columns.Add(new Column { ColumnName = property.Name, PropertyName = property.Name });
                }
            }
            return table;
        }

        private Table GetTableFromAttribute(string tname, Type type)
        {
            var table = new Table();
            table.TableName = tname;
            List<PropertyInfo> props = type.GetPrimitivePropertys();
            foreach (PropertyInfo property in props)
            {
                ColumnAttribute columnMaperAttribute = property.GetCustomAttributes(true).OfType<ColumnAttribute>().FirstOrDefault();
                if (columnMaperAttribute != null)
                {
                    IColumn t = columnMaperAttribute;
                    if (t != null)
                    {
                        t.PropertyName = property.Name;
                        t.PropertyType = property.PropertyType;
                        table.Columns.Add(t);
                    }
                }
                else
                {
                   // Type subType = property.GetType();
                    //if (!subType.IsPrimitive())
                    //{
                    //    Table subTable = Table.LoadFromDomain(subType);
                    //    this.SubTable = subTable;
                    //}
                }

                //if(property)
            }
            return table;
        }

        private Table LoadFromMetadata(Type type)
        {
            TableAttribute tableAttr = type.GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
            string tname;
            if (tableAttr == null)
            {
                
                DtoAttribute dto  = type.GetCustomAttributes(true).OfType<DtoAttribute>().FirstOrDefault();
                if(dto==null)
                {
                    throw new ArgumentException(string.Format("类型{0}不包含表信息", type));
                }
                else
                {
                    tname= type.Name;
                }
               
            }
            else
            {
                tname = tableAttr.Name;
            }
            
            string key = string.Format(columnAttrMapTableName, tname);
            
            return tableCache.GetOrAdd(key, () => this.GetTableFromAttribute(tname, type));
        }

        /// <summary>
        /// 属性名称作为列名称。无主键等信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Table LoadFromProperty(Type type)
        {
            string key = string.Format(propertyMapTableName, type.Name);
            return tableCache.GetOrAdd(key, () => this.GetTableFromProperty(type));
        }
       
        #endregion
    }
}