#region copyright
// <copyright file="IColumn.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>丁浩</author>
// <datecreated>2012-12-17</datecreated>
#endregion
using System;
namespace Han.DbLight.TableMetadata
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IColumn
    {
        /// <summary>
        ///// 编码后的列名，防止列名与关键字相同[]
        ///// </summary>
        // string EncodeColumnName { get; }
         string ColumnName { get; set; }
         string PropertyName { get; set; }
        //domain 中定义，如果数据库定义了默认值则domain中指定相同值。update inset时，也更新，不做区分
       //  object DefaultValue { get; set; }
         bool IsPrimaryKey { get; set; }
        //sequenc
        /// <summary>
        /// 列值是否通过数据库产生
        /// </summary>
        bool IsSqlGenColumn { get; set; }
        /// <summary>
        /// 新增时，是否需要框架处理此列。自动增长列不需要框架处理
        /// </summary>
        bool IsAutoInsert { get; set; }
        /// <summary>
        /// 列是否是别名，别名列不是数据库中是实际列
        /// </summary>
         bool IsAliasColumn { get; set; }
        //通过convention
        /// <summary>
         /// 复杂类型，不是真实列IsAliasColumn=true
        /// </summary>
        bool IsComplex{ get; set; }
        /// <summary>
         /// 返回列名，对于IsSqlGenColumn，返回sql语句
        /// </summary>
        /// <returns></returns>
        string GetSql();

        Type PropertyType { get; set; }
    }
}
