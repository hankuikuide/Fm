#region copyright
// <copyright file="TypeParam.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>¶¡ºÆ</author>
// <datecreated>2012-11-07</datecreated>
#endregion
namespace Han.EnsureThat
{
    using System;

    public class TypeParam : Param
    {
        #region Fields

        public readonly Type Type;

        #endregion

        #region Constructors and Destructors

        internal TypeParam(string name, Type type)
            : base(name)
        {
            this.Type = type;
        }

        #endregion
    }
}