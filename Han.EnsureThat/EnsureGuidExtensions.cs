#region copyright
// <copyright file="EnsureGuidExtensions.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>¶¡ºÆ</author>
// <datecreated>2012-11-07</datecreated>
#endregion
namespace Han.EnsureThat
{
    using System;
    using System.Diagnostics;

    using Han.EnsureThat.Resources;

    public static class EnsureGuidExtensions
    {
        #region Public Methods and Operators

        [DebuggerStepThrough]
        public static Param<Guid> IsNotEmpty(this Param<Guid> param)
        {
            if (Guid.Empty.Equals(param.Value))
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsEmptyGuid);
            }

            return param;
        }

        #endregion
    }
}