
namespace Han.DbLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 数据库信息，每个数据库一个
    /// 
    /// wcf ioc 不好用。。。。。。。
    /// 分两种，每个库与没种库
    /// </summary>
    public class DatabaseInfo
    {
        public DatabaseInfo(string connectionStringName, ISqlDialect dialect)
        {
            SqlDialect = dialect;
            TablesInfo = new TablesInfo(dialect);
            ConnectionStringName = connectionStringName;
           //bTypeConverter=new DbTypeConverter();
            //WhereQueryTranslator=new WhereQueryTranslator();
        }
        //
       // public WhereQueryTranslator WhereQueryTranslator { get; set; }
       
        /// <summary>
        /// 数据库类型每种数据库一个
        /// </summary>
        public ISqlDialect SqlDialect { get; set; }
      

        public string ConnectionStringName { get; set; }
        /// <summary>
        /// 数据库表信息
        /// </summary>
        public TablesInfo TablesInfo { get; private set; }
        /// <summary>
        /// 数据库类型转换，每种数据库一个
        /// </summary>
        public IDbTypeConverter DbTypeConverter { get; set; }
    }
    
}
