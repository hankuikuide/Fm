#region copyright
// <copyright file="ExceptionFactory.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>丁浩</author>
// <datecreated>2012-11-07</datecreated>
#endregion
namespace Han.EnsureThat
{
    using System;

    internal static class ExceptionFactory
    {
        #region Methods

        internal static ArgumentNullException CreateForParamNullValidation(string paramName, string message)
        {
            return new ArgumentNullException(paramName, message);
        }

        internal static ArgumentException CreateForParamValidation(string paramName, string message)
        {
            return new ArgumentException(message, paramName);
        }

        #endregion
    }
}