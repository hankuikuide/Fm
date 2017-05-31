
namespace Han.DbLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Han.EnsureThat;
    using Han.Infrastructure.Reflection;
    using Han.DbLight.TableMetadata;

    public class UpdateBuilder<TDomain> : QueryBuilderBase
    {
       

        #region Fields

       

        private string updateTemplate = "UPDATE {0} SET {1} WHERE {2} ";

        private TDomain domain;
        #endregion

        #region Constructors and Destructors

        public UpdateBuilder(Table table,ISqlDialect sqlDialect, TDomain domain,IList<string> usedProperies=null, IDictionary<string, object> dbParams = null)
        {
            SqlDialect = sqlDialect;
            this.domain = domain;
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
            if (usedProperies!=null&&usedProperies.Count != 0)
            {
                dbCol = dbCol.Where(col => usedProperies.Contains(col.PropertyName) || col.IsPrimaryKey).ToList();
#if DEBUG

                if (usedProperies != null)
                {
                    var cols = dbCol.Where(col => !usedProperies.Contains(col.PropertyName)).ToList();
                    for (int i = 0; i < cols.Count; i++)
                    {
                        if (cols[i].IsAutoInsert || cols[i].IsSqlGenColumn)
                        {
                            cols.RemoveAt(i);
                        }
                    }

                    string message = cols.Aggregate<IColumn, string>(null, (current, col) => current + (col.ColumnName + ","));
                    if (message != null)
                        throw new System.Exception("insert有不包含列的属性" + message);

                }

#endif
            }

            var sbKeys = new StringBuilder();
            var sbWhere = new StringBuilder();
            Ensure.That(dbCol.Exists(col => col.IsPrimaryKey), "Entity must contain a primary key").IsTrue();
            foreach (IColumn col in dbCol)
            {
                if (!col.IsPrimaryKey)
                {
                    sbKeys.AppendFormat("{0} = {1}{2}, \r\n", EncodeName(col.ColumnName), SqlDialect.DbParameterConstant, col.PropertyName);
                    DbParams.Add(col.PropertyName, Reflection.GetPropertyValue(domain, col.PropertyName));
                }
                else
                {
                    sbWhere.AppendFormat(" {0} = {1}{2} \r\n and", EncodeName(col.ColumnName), SqlDialect.DbParameterConstant, col.PropertyName);
                    DbParams.Add(col.PropertyName, Reflection.GetPropertyValue(domain, col.PropertyName));
                }
            }
            string keys = sbKeys.ToString().Substring(0, sbKeys.Length - 4); //去掉, \r\n
            string where = sbWhere.ToString().Substring(0, sbWhere.Length - 4);
            string tsql = string.Format(this.updateTemplate, this.EncodeName(this.Table.TableName), keys, where);

            this.sql.Append(tsql);
        }
        public string CreateSql()
        {
            List<IColumn> dbCol = new List<IColumn>();
            if (this.usedProperies != null && this.usedProperies.Count != 0)
            {
                var allColumn = this.Table.DbColumns;
                foreach (var usedPropery in this.usedProperies)
                {
                    var col = allColumn.Find(c => c.PropertyName == usedPropery);
                    if (col != null)
                    {
                        dbCol.Add(col);
                    }
                }
             
            }
          
            var sbKeys = new StringBuilder();
            int i = 0;
           // Ensure.That(dbCol.Exists(col => col.IsPrimaryKey), "Entity must contain a primary key").IsTrue();
            foreach (IColumn col in dbCol)
            {
                sbKeys.AppendFormat("{0} = {1}{2}, \r\n", EncodeName(col.ColumnName), SqlDialect.DbParameterConstant, i);
                i++;
            }
            string keys = sbKeys.ToString().Substring(0, sbKeys.Length - 4); //去掉, \r\n
            string tsql = string.Format(this.updateTemplate, this.EncodeName(this.Table.TableName), keys, "");

            return tsql;
        }
       

        #endregion
    }
}
