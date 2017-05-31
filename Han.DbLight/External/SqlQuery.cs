namespace Han.DbLight.External
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using Han.DbLight.TableMetadata;

    public class SqlQuery// : ISqlQuery
    {

      
        public List<Constraint> Constraints = new List<Constraint>();
        internal DbType GetConstraintDbType(string tableName, string Name, object constraintValue)
        {
            return DbType.String;
        }
        public string[] SelectColumnList = new string[0];
    }
}