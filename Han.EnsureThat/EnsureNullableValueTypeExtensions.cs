#region copyright
// <copyright file="EnsureNullableValueTypeExtensions.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>¶¡ºÆ</author>
// <datecreated>2012-11-07</datecreated>
#endregion
namespace Han.EnsureThat
{
    using System.Diagnostics;

    using Han.EnsureThat.Resources;

    public static class EnsureNullableValueTypeExtensions
    {
        #region Public Methods and Operators

        [DebuggerStepThrough]
        public static Param<T?> IsNotNull<T>(this Param<T?> param) where T : struct
        {
            if (param.Value == null || !param.Value.HasValue)
            {
                throw ExceptionFactory.CreateForParamNullValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotNull);
            }

            return param;
        }

        #endregion
    }
}