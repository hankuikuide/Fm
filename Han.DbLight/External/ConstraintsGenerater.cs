
namespace Han.DbLight.External
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Han.Infrastructure;
    using Han.DbLight.TableMetadata;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ConstraintsGenerater : IConstraintsGenerater
    {
        public ConstraintsGenerater(ISqlDialect sqlDialect, Table table)
        {
            this.sqlDialect = sqlDialect;
            this.table = table;
        }
        public IColumn FindColumn(string columnName)
        {
            return table.GetColumn(columnName);
        }

        private ISqlDialect sqlDialect;

        private Table table;
        public string GenerateSql2(List<Constraint> constraints)
        {
            foreach (Constraint c in constraints)
            {

            }
            return null;
        }

        public string GenerateSql(List<Constraint> constraints)
        {
            return null;
        }
    }
    public interface IConstraintsGenerater
    {
        string GenerateSql(List<Constraint> constraints);
    }
}
