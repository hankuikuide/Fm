#region copyright
// <copyright file="AliasAttribute.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>丁浩</author>
// <datecreated>2012-12-17</datecreated>
#endregion
namespace Han.DbLight.TableMetadata
{
    using System;

    /// <summary>
    /// 属性映射为别名,非真实列。update，insert 不处理这种类型的列 
    /// todo 是否去掉此attribute ，保持简单
    /// </summary>
    /// <remarks>尽量通过 column as alias，而不是通过attribute固定在domain中</remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AliasAttribute : ColumnAttribute
    {
        #region Constructors and Destructors

        public AliasAttribute(string aliasName)
            : base(aliasName,false,false,true)
        {
        }

        #endregion

        #region Public Properties

        public string AliasName
        {
            get
            {
                return this.ColumnName;
            }
            set
            {
                this.ColumnName = value;
            }
        }

        #endregion
    }
}