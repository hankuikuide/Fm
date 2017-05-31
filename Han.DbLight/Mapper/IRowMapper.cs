
namespace Han.DbLight.Mapper
{
    using System;
    using System.Data;

    /// <summary>
   /// 将查询结果映射为对象
   /// </summary>
   /// <typeparam name="TResult"></typeparam>
    public interface IRowMapper<out TResult>
    {
        
        /// <summary>
        /// When implemented by a class, returns a new TResult based on row.
        /// </summary>
        /// <param name="row"> The System.Data.IDataRecord to map.</param>
        /// <returns>The instance of TResult that is based on row.</returns>
        TResult MapRow(IDataRecord row);
        /// <summary>
        /// 增加自定义转换，todo 复杂列（eee#bb）转换
        /// </summary>
        /// <param name="convert"></param>
        //void AddCustomConverter(Converter convert);
        /// <summary>
        /// 表信息，每个数据库一个
        /// </summary>
        TablesInfo TablesInfo { get; set; }
    }
    public class ColumnConverter
    {
        public string ColumnName { get; set; }
        public string PropertyPath { get; set; }
        /// <summary>
        /// 数据库值转换为对象值
        /// </summary>
        public Func<object> Convert { get; set; }
    }
}
