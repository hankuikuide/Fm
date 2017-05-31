
namespace Han.DbLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Han.Log;
    using Han.DbLight.TableMetadata;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DeleteBuilder<TDomain> : QueryBuilderBase
    {
        #region Fields

       
        private const string deletSqlTemplate = "DELETE FROM {0} ";

        #endregion

        #region Constructors and Destructors

         public DeleteBuilder(Table table,ISqlDialect sqlDialect, IDictionary<string, object> dbParams = null)
         {
             SqlDialect = sqlDialect;
            this.Table = table;
            this.DbParams = dbParams ?? new Dictionary<string, object>();
            this.sql = new StringBuilder();

        }

        #endregion

        #region Public Properties

        public string TableAlias { get; set; }

     

        #endregion

        #region Public Methods and Operators

        protected override void Build()
        {
            var sb = new StringBuilder(string.Format(deletSqlTemplate, this.EncodeName(this.Table.TableName)));
            this.sql.Append(sb);
        }
        public string CreateSql()
        {
            var sb = new StringBuilder(string.Format(deletSqlTemplate, this.EncodeName(this.Table.TableName)));
            return sb.ToString();
        }
        #endregion
    }
}
