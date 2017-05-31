
namespace Han.DbLight.Mapper
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Han.Infrastructure;
    using Han.Infrastructure.Extensions;
    using Han.Infrastructure.Reflection;
    using Han.Log;
    using Han.DbLight.Extensions;

    

    /// <summary>
    /// 属性名称做为列名映射
    /// </summary>
    /// <remarks>只支持基本类型属性</remarks>
    public class PropertyEqualColumnMapper<T> : IRowMapper<T>
        where T :  new()
    {
        #region Constructors and Destructors
        /// <summary>
        /// 需要映射的属性名称
        /// </summary>
        protected List<string> ObjProperties = new List<string>();
        /// <summary>
        /// 第一次通过反射读取，速度为自定义映射的2倍，300条记录为12毫秒，自定义为7毫秒左右
        /// </summary>
        public PropertyEqualColumnMapper()
        {
        }
        public TablesInfo TablesInfo { get; set; }
        public virtual T MapRow(IDataRecord row)
        {
            var mappedObj = new T();
            if (this.ObjProperties.Count == 0)
            {
                var propertys = typeof(T).GetPrimitivePropertys().Select(p => p.Name).ToList();
                foreach (var col in row.GetColumns())
                {
                    var tableCol = propertys.FirstOrDefault(p => p.ToUpperInvariant() == col.ToUpperInvariant());
                    if (tableCol == null)
                    {
#if DEBUG
                        throw new System.Exception(string.Format("PropertyEqualColumnMapper:列{0}在对象{1}中没有对应的属性", col, typeof(T)));
#endif
#pragma warning disable CS0162 // 检测到无法访问的代码
                        Logger.Log(Level.Trace, string.Format("PropertyEqualColumnMapper:列{0}在对象{1}中没有对应的属性", col, typeof(T)));
#pragma warning restore CS0162 // 检测到无法访问的代码
                    }
                    else
                    {
                        ObjProperties.Add(tableCol);
                    }
                }
            }
            if (this.ObjProperties.Count != 0)
            {
                foreach (string objProperty in this.ObjProperties)
                {
                    object value;
                    if (DbHelper.TryGetColumnValue(row, objProperty, out value))
                    {
                        Reflection.SetPropertyValue(mappedObj, value, objProperty);
                    }
                }
            }

            return mappedObj;
        }
        #endregion
    }
}