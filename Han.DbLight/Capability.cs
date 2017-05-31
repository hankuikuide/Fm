
namespace Han.DbLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 框架或者表支持功能
    /// </summary>
    public class Capability
    {
        /// <summary>
        /// 实体关系
        /// </summary>
        public Relation EntityRelation { get; set; }
    }
    /// <summary>
    /// 实体关系
    /// 
    /// </summary>
    public enum Relation
    {
        //涉及到更新插入，删除。目前这三种操作都只处理简单类型对象，只更新单表。忽略关系
        None,
        ManyToMany,
        ManyToOne,
        OneToMany,
        OneToOne
    }
}
