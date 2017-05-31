#region copyright
// <copyright file="SequenceIdAttribute.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>丁浩</author>
// <datecreated>2012-12-17</datecreated>
#endregion
namespace Han.DbLight.TableMetadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 主键通过Sequence方式产生
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = false)]
    public class SequenceIdAttribute : PrimaryKeyAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="sequence"></param>
        public SequenceIdAttribute(string columnName, string sequence)
            : base(columnName, true)
        {
            this.Sequence = sequence;
        }
        /// <summary>
        /// oracle Sequece name，使用sequence产生iD
        /// </summary>
        public string Sequence { get; set; }
        public override string GetSql()
        {
            return Sequence;
        }
    }
   
}
