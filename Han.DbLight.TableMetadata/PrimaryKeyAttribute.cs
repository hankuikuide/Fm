#region copyright
// <copyright file="PrimaryKeyAttribute.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>丁浩</author>
// <datecreated>2012-12-26</datecreated>
#endregion
namespace Han.DbLight.TableMetadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 主键
    /// 通过属性名称映射与通过attribute映射都需要知道数据库主键，update用到
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class PrimaryKeyAttribute:ColumnAttribute
    {
        protected PrimaryKeyAttribute(string columnName, bool isSqlGenColumn)
            : base(columnName, true, isSqlGenColumn, false)
        {
            
        }
    }
    /// <summary>
    /// sqlserver 自动增长列
    /// </summary>
    public class AutoGenerateIdAttribute : PrimaryKeyAttribute
    {
        public AutoGenerateIdAttribute(string columnName)
            : base(columnName,true)
        {
            IsAutoInsert = true;
        }
        /// <summary>
        /// 返回null
        /// </summary>
        /// <returns></returns>
        public override string GetSql()
        {
            return null;
        }

    }
    /// <summary>
    ///主键通过赋值方式产生
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AssignIdAttribute : PrimaryKeyAttribute
    {
        public AssignIdAttribute(string columnName)
            : base(columnName, false)
        {
        }


    }
}
