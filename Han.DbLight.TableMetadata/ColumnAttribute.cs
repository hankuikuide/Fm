#region copyright
// <copyright file="ColumnAttribute.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>丁浩</author>
// <datecreated>2012-12-17</datecreated>
#endregion
namespace Han.DbLight.TableMetadata
{
    using System;

    /// <summary>
    /// 映射属性名称到列名
    /// </summary>
    //[Serializable]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute : Attribute, IColumn
    {
        #region Fields

        protected bool isPrimaryKey;

        protected bool isSqlGenColumn;

        protected bool isAliasColumn;

        #endregion

        #region Constructors and Destructors
        /// <summary>
        /// 编码后的列名，防止列名与关键字相同[]
        /// </summary>
        public string EncodeColumnName
        {
            get
            {
                return "[" + ColumnName + "]";
            }
        }
        public ColumnAttribute(string columnName):this(columnName,false,false,false)
        {
            
        }
        protected ColumnAttribute(string columnName, bool isPrimaryKey, bool isSqlGenColumn, bool isAliasColumn)
        {
            this.isPrimaryKey = isPrimaryKey;
            this.isSqlGenColumn = isSqlGenColumn;
            this.isAliasColumn = isAliasColumn;
            this.ColumnName = columnName;
        }
        /// <summary>
        /// 本对象中存在的列名
        /// </summary>
        public string ColumnName { get; set; }

        #endregion

        #region Public Properties


        public Type PropertyType { get; set; }
        public bool IsPrimaryKey
        {
            get
            {
                return this.isPrimaryKey;
            }
            set
            {
                this.isPrimaryKey = value;
            }
        }

        public bool IsSqlGenColumn
        {
            get
            {
                return this.isSqlGenColumn;
            }
            set
            {
                this.isSqlGenColumn = value;
            }
        }
        public bool IsComplex { get; set; }
        /// <summary>
        /// 列对应的属性名称
        /// </summary>
        public string PropertyName { get; set; }

       // public object DefaultValue { get; set; }
        #endregion

        #region Public Methods and Operators

        public virtual string GetSql()
        {
            return this.EncodeColumnName;
        }
        public override string ToString()
        {
            return this.GetSql();
        }
        #endregion

        public bool IsAliasColumn
        {
            get
            {
                return this.isAliasColumn;
            }
            set
            {
                this.isAliasColumn = value;
            }
           
        }


        public bool IsAutoInsert { get; set; }
    }
}