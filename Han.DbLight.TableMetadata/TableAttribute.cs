#region copyright
// <copyright file="TableAttribute.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>丁浩</author>
// <datecreated>2012-12-17</datecreated>
#endregion
namespace Han.DbLight.TableMetadata
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 表信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {
        #region Fields

        

        #endregion

        #region Constructors and Destructors

        public TableAttribute(string name)
        {
            this.Name = name;
        }

        #endregion

        #region Public Properties

        public string Name { get; set; }

        #endregion
    }
    /// <summary>
    /// Dto信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public class DtoAttribute : Attribute
    {
        public DtoAttribute()
        {
           
        }
    }
}