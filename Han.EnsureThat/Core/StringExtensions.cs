#region copyright
// <copyright file="StringExtensions.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>丁浩</author>
// <datecreated>2012-11-07</datecreated>
#endregion
namespace Han.EnsureThat.Core
{
    using System.Diagnostics;

    internal static class StringExtensions
    {
        #region Methods

        [DebuggerStepThrough]
        internal static string Inject(this string format, params object[] formattingArgs)
        {
            return string.Format(format, formattingArgs);
        }

        [DebuggerStepThrough]
        internal static string Inject(this string format, params string[] formattingArgs)
        {
            return string.Format(format, formattingArgs);
        }

        #endregion
    }
}