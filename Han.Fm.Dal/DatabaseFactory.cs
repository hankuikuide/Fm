/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/20 14:05:49
 * ***********************************************/

namespace Han.Fm.Dal
{
    using DbLight.MySQl;
    using Han.DbLight;
    using Han.DbLight.Oracle;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class DatabaseFactory
    {
        // private static readonly OracleQuerySession QuerySession;
        private static readonly MySqlQuerySession QuerySession;
        static DatabaseFactory()
        {
            var dbInfo = new DatabaseInfo("Fm:DbConn", new MySqlDialect())
            {
                DbTypeConverter = new MySqlDbTypeConverter()
            };

            QuerySession = new MySqlQuerySession(dbInfo);
        }

        public static IQuerySession CreateQuerySession()
        {
            return QuerySession;
        }
    }
}
