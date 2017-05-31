/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/26 13:13:16
 * ***********************************************/

namespace Han.DbLight.MySQl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MySqlDialect : ISqlDialect
    {

        public string DbParameterConstant
        {
            get
            {
                return "?";
            }
        }

        public string SchemaSql
        {
            get
            {
                return "select column_name as ColumnName, '0' as IsAliasColumn from user_tab_column where table_name = upper(:0)";
            }
        }

        public string EncodeName(string name)
        {
            return string.Format("{0}", name);
        }
    }
}
