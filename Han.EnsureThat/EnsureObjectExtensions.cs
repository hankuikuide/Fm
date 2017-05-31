#region copyright
// <copyright file="EnsureObjectExtensions.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>¶¡ºÆ</author>
// <datecreated>2012-11-07</datecreated>
#endregion
namespace Han.EnsureThat
{
    using System.Diagnostics;

    using Han.EnsureThat.Resources;

    public static class EnsureObjectExtensions
    {
        #region Public Methods and Operators

        [DebuggerStepThrough]
        public static Param<T> IsNotNull<T>(this Param<T> param) where T : class
        {
            if (param.Value == null)
            {
                throw ExceptionFactory.CreateForParamNullValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotNull);
            }

            return param;
        }
        public static Param<T> IsNull<T>(this Param<T> param) where T : class
        {
            if (param.Value != null)
            {
                throw ExceptionFactory.CreateForParamNullValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotNull);
            }

            return param;
        }
        #endregion
    }
}