#region copyright
// <copyright file="EnsureDateTimeExtensions.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>¶¡ºÆ</author>
// <datecreated>2012-11-07</datecreated>
#endregion
namespace Han.EnsureThat
{
    using System;
    using System.Diagnostics;

    using Han.EnsureThat.Core;
    using Han.EnsureThat.Resources;

    public static class EnsureDateTimeExtensions
    {
        #region Public Methods and Operators

        [DebuggerStepThrough]
        public static Param<DateTime> IsGt(this Param<DateTime> param, DateTime limit)
        {
            if (param.Value <= limit)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotGt.Inject(param.Value, limit));
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<DateTime> IsGte(this Param<DateTime> param, DateTime limit)
        {
            if (!(param.Value >= limit))
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotGte.Inject(param.Value, limit));
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<DateTime> IsInRange(this Param<DateTime> param, DateTime min, DateTime max)
        {
            if (param.Value < min)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotInRange_ToLow.Inject(param.Value, min));
            }

            if (param.Value > max)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotInRange_ToHigh.Inject(param.Value, max));
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<DateTime> IsLt(this Param<DateTime> param, DateTime limit)
        {
            if (param.Value >= limit)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotLt.Inject(param.Value, limit));
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<DateTime> IsLte(this Param<DateTime> param, DateTime limit)
        {
            if (!(param.Value <= limit))
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotLte.Inject(param.Value, limit));
            }

            return param;
        }

        #endregion
    }
}