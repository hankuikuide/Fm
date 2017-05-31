namespace Han.DbLight
{
    using System.Collections.Generic;
    using System.Text;

    public abstract class QueryBuilderBase
    {
        #region Fields

        protected StringBuilder sql;

        #endregion

        #region Public Properties

        public ISqlDialect SqlDialect { get; set; }
        public IDictionary<string, object> DbParams { get; protected set; }

        public SqlBuilder SqlBuilder { get; set; }

        public Table Table { get; set; }

        #endregion

        #region Public Methods and Operators
        public string EncodeName(string name)
        {
            return SqlDialect.EncodeName(name);
        }
        public string GetSql()
        {
            this.Build();
            if (this.SqlBuilder != null)
            {
                this.sql.Append(this.SqlBuilder.ToSql());
                foreach (var dbParam in this.SqlBuilder.DbParams)
                {
                    this.DbParams.Add(dbParam);
                }
            }

            return this.sql.ToString();
        }

        #endregion

        #region Methods

        protected abstract void Build();

        #endregion
    }
}