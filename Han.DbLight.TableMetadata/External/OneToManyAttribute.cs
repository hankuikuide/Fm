using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Han.DbLight.TableMetadata.External
{
    /// <summary>
    /// 一对多关系
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OneToManyAttribute : RelationId
    {
        /// <summary>
        /// </summary>
        /// <param name="columnName">本实体中的列名</param>
        /// <param name="relationColumnName">关系实体中的相应列名</param>
        public OneToManyAttribute(string columnName, string relationColumnName)
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
