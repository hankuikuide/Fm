#region copyright
// <copyright file="EnsureDecimalExtensions.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>����</author>
// <datecreated>2012-11-07</datecreated>
#endregion
namespace Han.EnsureThat
{
    using System.Diagnostics;

    using Han.EnsureThat.Core;
    using Han.EnsureThat.Resources;

    public static class EnsureDecimalExtensions
    {
        #region Public Methods and Operators

        [DebuggerStepThrough]
        public static Param<decimal> IsGt(this Param<decimal> param, decimal limit)
        {
            if (param.Value <= limit)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotGt.Inject(param.Value, limit));
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<decimal> IsGte(this Param<decimal> param, decimal limit)
        {
            if (!(param.Value >= limit))
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotGte.Inject(param.Value, limit));
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<decimal> IsInRange(this Param<decimal> param, decimal min, decimal max)
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
        public static Param<decimal> IsLt(this Param<decimal> param, decimal limit)
        {
            if (param.Value >= limit)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotLt.Inject(param.Value, limit));
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<decimal> IsLte(this Param<decimal> param, decimal limit)
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