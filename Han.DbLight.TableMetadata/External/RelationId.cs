// -----------------------------------------------------------------------
// <copyright file="Class1.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Han.DbLight.TableMetadata.External
{
    using System;

    /// <summary>
    /// 不支持，仅支持自动加载、自动更新删除插入时才有意义。如果使用会被当做ColumnAttribute处理，无关系含义
    /// 关系键：建立与其他表的关联,如外键  
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RelationId : ColumnAttribute
    {
        public RelationId(string columnName)
            : base(columnName, false, false, false)
        {
            
        }

        //public string PropertyName { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ManyToOne : RelationId
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName">本实体中的列名</param>
        /// <param name="relationColumnName">关系实体中的相应列名</param>
        public ManyToOne(string columnName, string relationColumnName)
            : base(columnName)
        {
            this.RelationColumnName = relationColumnName;
            //this.RelationTable = relationTableName;
        }
        public string RelationColumnName { get; set; }
        //public string PropertyName { get; set; }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OneToMany : RelationId
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName">本实体中的列名</param>
        /// <param name="relationColumnName">关系实体中的相应列名</param>
        public OneToMany(string columnName, string relationColumnName)
            : base(columnName)
        {
            this.RelationColumnName = relationColumnName;
            //this.RelationTable = relationTableName;
        }

        public string RelationColumnName { get; set; }
        /// <summary>
        /// 关系表名称
        /// </summary>
        public string RelationTable { get; set; }
    }
    /// <summary>
    /// 只支持只有键值的关系表。
    /// 否则需要将关系表映射为实体，不需要manytomany映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ManyToMany : RelationId
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName">本实体中的列名</param>
        /// <param name="relationColumnName">关系实体中的相应列名</param>
        /// <param name="relationTableName">关系表名</param>
        public ManyToMany(string columnName, string relationColumnName,string relationTableName)
            : base(columnName)
        {
            this.RelationColumnName = relationColumnName;
            this.RelationTable = relationTableName;
        }

        public string RelationColumnName { get; set; }
        /// <summary>
        /// 关系表名称
        /// </summary>
        public string RelationTable { get; set; }
        //public string PropertyName { get; set; }
    }
    //关系涉及到两端的导航性。如order.User ,User.Orders,如果映射涉及到许多自动生成sql，容易影响性能
    //一端导航性如：order.User 
    //无导航性如order.UserInfoID
    //同时使用UserInfoID与UserInfo，不使用导航时使用UserInfoID，性能最好。需要导航时，使用UserInfo。UserInfoID优先级高
    //只使用UserInfo。不使用导航时，UserInfo.ID。多实例化一个对象。
    //只使用UserInfoID，不支持导航

    /**************/
    //关系映射推荐用法：
    //不使用自动关系加载支持:自动关系加载可以通过aop实现
    //同时使用UserInfoID与UserInfo。默认只加载serInfoID， 关系的导航对象通过到手动加载,RelationId也不需要。而非自动。
}
