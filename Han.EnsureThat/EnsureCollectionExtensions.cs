#region copyright
// <copyright file="EnsureCollectionExtensions.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>¶¡ºÆ</author>
// <datecreated>2012-11-07</datecreated>
#endregion
namespace Han.EnsureThat
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    using Han.EnsureThat.Resources;

    public static class EnsureCollectionExtensions
    {
        #region Public Methods and Operators

        [DebuggerStepThrough]
        public static Param<T> HasItems<T>(this Param<T> param) where T : class, ICollection
        {
            if (param.Value == null || param.Value.Count < 1)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsEmptyCollection);
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<Collection<T>> HasItems<T>(this Param<Collection<T>> param)
        {
            if (param.Value == null || param.Value.Count < 1)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsEmptyCollection);
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<ICollection<T>> HasItems<T>(this Param<ICollection<T>> param)
        {
            if (param.Value == null || param.Value.Count < 1)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsEmptyCollection);
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<T[]> HasItems<T>(this Param<T[]> param)
        {
            if (param.Value == null || param.Value.Length < 1)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsEmptyCollection);
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<List<T>> HasItems<T>(this Param<List<T>> param)
        {
            if (param.Value == null || param.Value.Count < 1)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsEmptyCollection);
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<IList<T>> HasItems<T>(this Param<IList<T>> param)
        {
            if (param.Value == null || param.Value.Count < 1)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsEmptyCollection);
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<IDictionary<TKey, TValue>> HasItems<TKey, TValue>(
            this Param<IDictionary<TKey, TValue>> param)
        {
            if (param.Value == null || param.Value.Count < 1)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsEmptyCollection);
            }

            return param;
        }

        #endregion
    }
}