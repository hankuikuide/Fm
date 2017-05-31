
namespace Han.DbLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 数据库信息
    /// </summary>
    public interface ISqlDialect
    {
        /// <summary>
        /// 不同数据库的查询参数，如：oracle：
        /// sql server:@
        /// </summary>
        string DbParameterConstant { get; }
        /// <summary>
        /// 查询表结构sql，使用properytocol映射
        /// </summary>
        string SchemaSql { get; }
        /// <summary>
        /// 编码关键字列名,表名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string EncodeName(string name);
    }
   
}
