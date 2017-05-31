
using System.Diagnostics.CodeAnalysis;

namespace Han.DbLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Han.EnsureThat;
    using Han.Infrastructure.Reflection;
    using Han.Log;
    using Han.DbLight.Mapper;
    using Han.DbLight.TableMetadata;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    /// <typeparam name="TDomain"> 可以映射到数据库表的实体类 </typeparam>
    public class SelectBuilder<TDomain> : QueryBuilderBase
    {
          #region Fields

       

        private string selectTemplate = "select {0} from {1} ";

        #endregion

        #region Constructors and Destructors

         public SelectBuilder(Table table,ISqlDialect sqlDialect, IList<string> usedProperies = null, IDictionary<string, object> dbParams = null)
        {
            this.SqlDialect = sqlDialect;
            this.usedProperies = usedProperies;
            this.Table = table;
            this.DbParams = dbParams ?? new Dictionary<string, object>();
            this.sql = new StringBuilder();
           
        }

        #endregion

        #region Public Properties

        public string TableAlias { get; set; }

      

        private IList<string> usedProperies;

        #endregion

        #region Public Methods and Operators

        protected override void Build()
        {
            List<IColumn> dbCol = Table.DbColumns;
            string colNames = " ";
            if (usedProperies!=null&&usedProperies.Count != 0)
            {
                dbCol = dbCol.Where(col => usedProperies.Contains(col.PropertyName)).ToList();
//#if DEBUG

//                if (usedProperies != null)
//                {
//                    var cols = dbCol.Where(col => !usedProperies.Contains(col.PropertyName)).ToList();
//                    string message = cols.Aggregate<IColumn, string>(null, (current, col) => current + (col.ColumnName + ","));
//                    if (message != null)
//                        throw new System.Exception("insert有不包含列的属性");

//                }

//#endif
                //Logger.Log(Level.Debug,()=>
                //    { 
                //        var cols = dbCol.Where(col => !usedProperies.Contains(col.PropertyName)).ToList();
                //        string message = cols.Aggregate<IColumn, string>(null, (current, col) => current + (col.ColumnName + ","));
                //        if (message != null)
                //            return "数据库不存在列：" + message;
                //        else
                //        {
                //            return message;
                //        }
                //    });
                if(dbCol.Count!=0)
                {
                    colNames = dbCol.Aggregate(colNames, (current, column) => current + (EncodeName(column.ColumnName) + ","));
                    colNames = colNames.Remove(colNames.Length - 1);
                }
                else
                {
                    colNames = "*";
                }
               
            }
            var sb = new StringBuilder(string.Format(this.selectTemplate, colNames, this.EncodeName(this.Table.TableName)));
            this.sql.Append(sb);
        }

        public string CreateSql()
        {
            List<IColumn> dbCol = Table.DbColumns;
            string colNames = " ";
            if (usedProperies != null && usedProperies.Count != 0)
            {
                dbCol = dbCol.Where(col => usedProperies.Contains(col.PropertyName)).ToList();
                //#if DEBUG

                //                if (usedProperies != null)
                //                {
                //                    var cols = dbCol.Where(col => !usedProperies.Contains(col.PropertyName)).ToList();
                //                    string message = cols.Aggregate<IColumn, string>(null, (current, col) => current + (col.ColumnName + ","));
                //                    if (message != null)
                //                        throw new System.Exception("insert有不包含列的属性");

                //                }

                //#endif
                //Logger.Log(Level.Debug,()=>
                //    { 
                //        var cols = dbCol.Where(col => !usedProperies.Contains(col.PropertyName)).ToList();
                //        string message = cols.Aggregate<IColumn, string>(null, (current, col) => current + (col.ColumnName + ","));
                //        if (message != null)
                //            return "数据库不存在列：" + message;
                //        else
                //        {
                //            return message;
                //        }
                //    });
                if (dbCol.Count != 0)
                {
                    colNames = dbCol.Aggregate(colNames, (current, column) => current + (EncodeName(column.ColumnName) + ","));
                    colNames = colNames.Remove(colNames.Length - 1);
                }
                else
                {
                    colNames = "*";
                }

            }
            var sb = new StringBuilder(string.Format(this.selectTemplate, colNames, this.EncodeName(this.Table.TableName)));
            sb.Append(" WHERE ");
            return sb.ToString();
        }

        #endregion
    }
}
