
namespace Han.DbLight.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Dynamic;

    using Han.DbLight.Mapper;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class DataReaderExtension
    {
        #region Public Methods and Operators

        /// <summary>
        /// 通过Mapper转换对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rdr"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public static T RecordTo<T>(this IDataReader rdr, IRowMapper<T> mapper) where T : class, new()
        {
            var tt = (IDataRecord)rdr;
            T obj = mapper.MapRow(tt);
            return obj;
        }
        public static List<string> GetColumns (this IDataReader rdr)
        {
            List<string> columnNames = new List<string>();
            for (int i = 0; i < rdr.FieldCount; i++)
            {
                columnNames.Add(rdr.GetName(i));
            }
            return columnNames;
        }
        public static dynamic RecordToExpando(this IDataReader rdr)
        {
            dynamic e = new ExpandoObject();
            var d = e as IDictionary<string, object>;
            for (int i = 0; i < rdr.FieldCount; i++)
            {
                d.Add(rdr.GetName(i), DBNull.Value.Equals(rdr[i]) ? null : rdr[i]);
            }
            return e;
        }

        #endregion
    }
}