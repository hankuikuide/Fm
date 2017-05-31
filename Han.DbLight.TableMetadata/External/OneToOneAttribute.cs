using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Han.DbLight.TableMetadata.External
{
    /// <summary>
    /// 一对一
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OneToOneAttribute : RelationId
    {
        /// <summary>
        /// 一对一关系
        /// </summary>
        /// <param name="columnName">本实体类列名</param>
        /// <param name="relationColumnName">关联实体类列名</param>
        public OneToOneAttribute(string columnName, string relationColumnName)
            : base(columnName)
        {
            this.ColumnName = columnName;
            this.RelationColumnName = relationColumnName;
        }

        /// <summary>
        /// 关联表列的名称
        /// </summary>
        public string RelationColumnName { get; set; }
    }

}
