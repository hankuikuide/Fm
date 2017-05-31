#region copyright
// <copyright file="Column.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>丁浩</author>
// <datecreated>2012-12-18</datecreated>
#endregion
namespace Han.DbLight.TableMetadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Column : IColumn
    {
        private string columnName;
        
        
        ///// <summary>
        ///// 编码后的列名，防止列名与关键字相同[]
        ///// </summary>
        //public string EncodeColumnName
        //{
        //    get
        //    {
        //        return "[" + columnName + "]";
        //    }
        //}
        public string ColumnName
        {
            get
            {
                return this.columnName;
            }
            set
            {
              
                this.columnName = value;
            }
        }

        public bool IsComplex { get; set; }
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsSqlGenColumn { get; set; }

        public bool IsAliasColumn { get; set; }
        public bool IsAutoInsert { get; set; }
        public string GetSql()
        {
            return ColumnName;
        }
        public override string ToString()
        {
            return this.GetSql();
        }
       
    }
}
