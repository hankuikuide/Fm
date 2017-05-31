#region copyright
// <copyright file="KeyEqualityComparer.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>¶¡ºÆ</author>
// <datecreated>2012-11-07</datecreated>
#endregion
namespace Han.Infrastructure
{
    using System;
    using System.Collections.Generic;

    public class KeyEqualityComparer<T, TKey> : IEqualityComparer<T>
    {
        #region Fields

        protected readonly Func<T, TKey> keyExtractor;

        #endregion

        #region Constructors and Destructors

        public KeyEqualityComparer(Func<T, TKey> keyExtractor)
        {
            this.keyExtractor = keyExtractor;
        }

        #endregion

        #region Public Methods and Operators

        public virtual bool Equals(T x, T y)
        {
            return this.keyExtractor(x).Equals(this.keyExtractor(y));
        }

        public int GetHashCode(T obj)
        {
            return this.keyExtractor(obj).GetHashCode();
        }

        #endregion
    }
}