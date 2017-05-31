#region copyright
// <copyright file="Comparer.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>¶¡ºÆ</author>
// <datecreated>2012-11-07</datecreated>
#endregion
namespace Han.Infrastructure
{
    using System;
    using System.Collections.Generic;

    public class Comparer<T> : IComparer<T>
    {
        #region Fields

        private readonly Func<T, T, int> _compareFn;

        #endregion

        #region Constructors and Destructors

        public Comparer(Func<T, T, int> fn)
        {
            this._compareFn = fn;
        }

        #endregion

        #region Public Methods and Operators

        public int Compare(T x, T y)
        {
            return this._compareFn(x, y);
        }

        #endregion
    }
}