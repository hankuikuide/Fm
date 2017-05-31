
namespace Han.DbLight
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    /// <summary>
    /// Dbtype与clrtype转换
    /// </summary>
    public  interface  IDbTypeConverter
    {
        /// <summary>
        /// 将值转换为sql string
        /// todo 不同数据库不同转换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="customConverter"></param>
        /// <returns></returns>
        string ToDbString(object obj, Func<object, string> customConverter = null);

        DbType ClrToDbType(Type type);
        Type DbTypeToClr(DbType dbType);

        DbParameter GetDbParameter(DbCommand cmd, string name, object value);

        void AddParam(DbCommand cmd, object value);
        void AddParams(DbCommand cmd, IDictionary<string, object> dbParameters);
        void AddParams(DbCommand cmd, params object[] args);

        IDictionary<string, object> ToDicParams(params object[] args);
    }
}