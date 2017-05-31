


namespace Han.DbLight
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Han.Infrastructure.Reflection;
    using Han.DbLight.TableMetadata;
    using Han.Log;
    /// <summary>
    /// TODO: 增加where。join
    /// where +join ==sqlbuiler 
    /// </summary>
    public class InsertBuilder<TDomain> : QueryBuilderBase
    {
     

        #region Fields
        private const string insertTemplate = "INSERT INTO {0} ({1}) \r\n VALUES ({2})";

        private IList<string> usedProperies;

        private TDomain domain;
        #endregion

        #region Constructors and Destructors

        public InsertBuilder(Table table,TDomain domain,ISqlDialect sqlDialect, IList<string> usedProperies=null, IDictionary<string, object> dbParams = null)
        {
            SqlDialect = sqlDialect;
            this.Table = table;
            this.domain = domain;
            this.usedProperies = usedProperies;
            this.DbParams = dbParams ?? new Dictionary<string, object>();
            this.sql = new StringBuilder();
        }

        #endregion

        #region Public Properties

    

      

        #endregion

        #region Public Methods and Operators
        
        protected override void Build()
        {
            List<IColumn> dbCol = this.Table.DbColumns;
            if (this.usedProperies != null && this.usedProperies.Count != 0)
            {
                dbCol = dbCol.Where(col => this.usedProperies.Contains(col.PropertyName) || col.IsPrimaryKey).ToList();
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
            var sbVals = new StringBuilder();
            foreach (IColumn col in dbCol)
            {
                if (col.IsAutoInsert)
                {
                    continue;
                }
                if (!col.IsSqlGenColumn)
                {

                    sbKeys.AppendFormat("{0},", EncodeName(col.ColumnName));
                    sbVals.AppendFormat(SqlDialect.DbParameterConstant + "{0},", col.ColumnName);
                    this.DbParams.Add(col.ColumnName, Reflection.GetPropertyValue(domain, col.PropertyName));
                }
                else
                {
                    sbKeys.AppendFormat("{0},", EncodeName(col.ColumnName));
                    sbVals.AppendFormat("{0},", col.GetSql());
                }
            }
            string keys = sbKeys.ToString().Substring(0, sbKeys.Length - 1);
            string vals = sbVals.ToString().Substring(0, sbVals.Length - 1);
            this.sql.Append(string.Format(insertTemplate, this.EncodeName(this.Table.TableName), keys, vals));
        }

        //public string CreateSql()
        //{
        //    List<IColumn> dbCol = this.Table.DbColumns;
        //    if (this.usedProperies != null && this.usedProperies.Count != 0)
        //    {
        //        dbCol = dbCol.Where(col => this.usedProperies.Contains(col.PropertyName) || col.IsPrimaryKey).ToList();
        //    }
        //    var sbKeys = new StringBuilder();
        //    var sbVals = new StringBuilder();
        //    int i = 0;
        //    foreach (IColumn col in dbCol)
        //    {
        //        if (col.IsAutoInsert)
        //        {
        //            continue;
        //        }
        //        if (!col.IsSqlGenColumn)
        //        {

        //            sbKeys.AppendFormat("{0},", EncodeName(col.ColumnName));
        //            sbVals.AppendFormat(SqlDialect.DbParameterConstant + "{0},", i);
        //            i++;
        //            //this.DbParams.Add(col.PropertyName, Reflection.GetPropertyValue(domain, col.PropertyName));
        //        }
        //        else
        //        {
        //            sbKeys.AppendFormat("{0},", EncodeName(col.ColumnName));
        //            sbVals.AppendFormat("{0},", col.GetSql());
        //        }

        //    }
        //    string keys = sbKeys.ToString().Substring(0, sbKeys.Length - 1);
        //    string vals = sbVals.ToString().Substring(0, sbVals.Length - 1);
        //    this.sql.Append(string.Format(insertTemplate, this.EncodeName(this.Table.TableName), keys, vals));
        //    return sql.ToString();
        //}

        public string CreateSql()
        {
            List<IColumn> dbCol = new List<IColumn>();
            if (this.usedProperies != null && this.usedProperies.Count != 0)
            {
                var allColumn = this.Table.DbColumns;
                var primaryCols = allColumn.FindAll(c => c.IsPrimaryKey);
                dbCol.AddRange(primaryCols);

                foreach (var usedPropery in this.usedProperies)
                {
                    var col = allColumn.Find(c => c.PropertyName == usedPropery && !c.IsPrimaryKey);
                    if (col != null)
                    {
                        dbCol.Add(col);
                    }
                }
                //dbCol = dbCol.Where(col => this.usedProperies.Contains(col.PropertyName) || col.IsPrimaryKey).ToList();
            }
            var sbKeys = new StringBuilder();
            var sbVals = new StringBuilder();
            int i = 0;
            foreach (IColumn col in dbCol)
            {
                if (col.IsAutoInsert)
                {
                    continue;
                }
                if (!col.IsSqlGenColumn)
                {

                    sbKeys.AppendFormat("{0},", EncodeName(col.ColumnName));
                    sbVals.AppendFormat(SqlDialect.DbParameterConstant + "{0},", i);
                    i++;
                    //this.DbParams.Add(col.PropertyName, Reflection.GetPropertyValue(domain, col.PropertyName));
                }
                else
                {
                    sbKeys.AppendFormat("{0},", EncodeName(col.ColumnName));
                    sbVals.AppendFormat("{0},", col.GetSql());
                }

            }
            string keys = sbKeys.ToString().Substring(0, sbKeys.Length - 1);
            string vals = sbVals.ToString().Substring(0, sbVals.Length - 1);
            this.sql.Append(string.Format(insertTemplate, this.EncodeName(this.Table.TableName), keys, vals));
            return sql.ToString();
        }
        #endregion
    }
}