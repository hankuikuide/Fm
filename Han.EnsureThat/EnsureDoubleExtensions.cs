#region copyright
// <copyright file="EnsureDoubleExtensions.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>¶¡ºÆ</author>
// <datecreated>2012-11-07</datecreated>
#endregion
namespace Han.EnsureThat
{
    using System.Diagnostics;

    using Han.EnsureThat.Core;
    using Han.EnsureThat.Resources;

    public static class EnsureDoubleExtensions
    {
        #region Public Methods and Operators

        [DebuggerStepThrough]
        public static Param<double> IsGt(this Param<double> param, double limit)
        {
            if (param.Value <= limit)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotGt.Inject(param.Value, limit));
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<double> IsGte(this Param<double> param, double limit)
        {
            if (!(param.Value >= limit))
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotGte.Inject(param.Value, limit));
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<double> IsInRange(this Param<double> param, double min, double max)
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
        public static Param<double> IsLt(this Param<double> param, double limit)
        {
            if (param.Value >= limit)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotLt.Inject(param.Value, limit));
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<double> IsLte(this Param<double> param, double limit)
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