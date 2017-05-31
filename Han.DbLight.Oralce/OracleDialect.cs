#region copyright
// <copyright file="OracleDialect.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>丁浩</author>
// <datecreated>2012-12-20</datecreated>
#endregion
namespace Han.DbLight.Oracle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class OracleDialect : ISqlDialect
    {
        public string DbParameterConstant
        {
            get
            {
                return ":";
            }
        }

        public string SchemaSql
        {
            get
            {
                return "select COLUMN_NAME as ColumnName,'0' as IsAliasColumn   from   user_tab_columns  where table_name  =Upper(:0)";
            }
        }

        public string EncodeName(string name)
        {
            return string.Format("{0}", name);
        }
        //public string EncodeChar
        //{
        //    //如果关键字做为表名，列名需要编码。区分大小写
        //    //get { return (@"""{0}"""); }
        //    get { return ("{0}"); }
        //}
    }
}
