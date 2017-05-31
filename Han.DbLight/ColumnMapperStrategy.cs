
namespace Han.DbLight
{
    /// <summary>
    /// 映射策略
    /// </summary>
    public enum ColumnMapperStrategy
    {
        /// <summary>
        /// 属性名称映射为列
        /// </summary>
        Property,
        /// <summary>
        /// 属性的attribute值映射为列
        /// </summary>
        ColumnAttribute
    }
}
