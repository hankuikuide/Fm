#region copyright
// <copyright file="EnsureBoolExtensions.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>¶¡ºÆ</author>
// <datecreated>2012-11-07</datecreated>
#endregion
namespace Han.EnsureThat
{
    using System.Diagnostics;

    using Han.EnsureThat.Resources;

    public static class EnsureBoolExtensions
    {
        #region Public Methods and Operators

        [DebuggerStepThrough]
        public static Param<bool> IsFalse(this Param<bool> param)
        {
            if (param.Value)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotFalse);
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<bool> IsTrue(this Param<bool> param)
        {
            if (!param.Value)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotTrue);
            }

            return param;
        }

        #endregion
    }
}