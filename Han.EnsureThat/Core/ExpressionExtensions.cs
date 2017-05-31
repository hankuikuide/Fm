#region copyright
// <copyright file="ExpressionExtensions.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>丁浩</author>
// <datecreated>2012-11-07</datecreated>
#endregion
namespace Han.EnsureThat.Core
{
    using System;
    using System.Linq.Expressions;

    using Han.EnsureThat.Resources;

    internal static class ExpressionExtensions
    {
        #region Methods

        internal static MemberExpression GetRightMostMember(this Expression e)
        {
            if (e is LambdaExpression)
            {
                return GetRightMostMember(((LambdaExpression)e).Body);
            }

            if (e is MemberExpression)
            {
                return (MemberExpression)e;
            }

            if (e is MethodCallExpression)
            {
                var callExpression = (MethodCallExpression)e;
                Expression member = callExpression.Arguments.Count > 0
                                        ? callExpression.Arguments[0]
                                        : callExpression.Object;
                return GetRightMostMember(member);
            }

            if (e is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)e;
                return GetRightMostMember(unaryExpression.Operand);
            }

            throw new Exception(ExceptionMessages.ExpressionUtils_GetRightMostMember_NoMemberFound.Inject(e.ToString()));
        }

        internal static string ToPath(this MemberExpression e)
        {
            string path = "";
            var parent = e.Expression as MemberExpression;

            if (parent != null)
            {
                path = parent.ToPath() + ".";
            }

            return path + e.Member.Name;
        }

        #endregion
    }
}