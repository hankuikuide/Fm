#region copyright
// <copyright file="EnsureStringExtensions.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>¶¡ºÆ</author>
// <datecreated>2012-11-07</datecreated>
#endregion
namespace Han.EnsureThat
{
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    using Han.EnsureThat.Core;
    using Han.EnsureThat.Resources;

    public static class EnsureStringExtensions
    {
        #region Public Methods and Operators

        [DebuggerStepThrough]
        public static Param<string> HasLengthBetween(this Param<string> param, int minLength, int maxLength)
        {
            if (string.IsNullOrEmpty(param.Value))
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotNullOrEmpty);
            }

            int length = param.Value.Length;

            if (length < minLength)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name,
                    ExceptionMessages.EnsureExtensions_IsNotInRange_ToShort.Inject(minLength, maxLength, length));
            }

            if (length > maxLength)
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name,
                    ExceptionMessages.EnsureExtensions_IsNotInRange_ToLong.Inject(minLength, maxLength, length));
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<string> IsNotNullOrEmpty(this Param<string> param)
        {
            if (string.IsNullOrEmpty(param.Value))
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotNullOrEmpty);
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<string> IsNotNullOrWhiteSpace(this Param<string> param)
        {
            if (string.IsNullOrWhiteSpace(param.Value))
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_IsNotNullOrWhiteSpace);
            }

            return param;
        }

        [DebuggerStepThrough]
        public static Param<string> Matches(this Param<string> param, string match)
        {
            return Matches(param, new Regex(match));
        }

        [DebuggerStepThrough]
        public static Param<string> Matches(this Param<string> param, Regex match)
        {
            if (!match.IsMatch(param.Value))
            {
                throw ExceptionFactory.CreateForParamValidation(
                    param.Name, ExceptionMessages.EnsureExtensions_NoMatch.Inject(param.Value, match));
            }
            return param;
        }

        #endregion
    }
}