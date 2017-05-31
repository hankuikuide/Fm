// -----------------------------------------------------------------------
// <copyright file="ExpressionExtension.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Han.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Serialization;
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class ExpressionExtension
    {
        //public static string GetPropertySymbol<T, TResult>(Expression<Func<T, TResult>> expression)
        //{
        //    return String.Join(".",
        //        GetMembersOnPath(expression.Body as MemberExpression)
        //            .Select(m => m.Member.Name)
        //            .Reverse());

        //}
        public static MemberInfo GetMemberInfo1(this Expression expression)
        {
            var lambda = (LambdaExpression)expression;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            return memberExpression.Member;
        }
        public static string GetPropertySymbol<TResult>(this Expression<Func<TResult>> expression)
        {
            return String.Join(".",
                GetMembersOnPath(expression.Body as MemberExpression)
                    .Select(m => m.Member.Name)
                    .Reverse());

        }
        private static IEnumerable<MemberExpression> GetMembersOnPath(MemberExpression expression)
        {
            while (expression != null)
            {
                yield return expression;
                expression = expression.Expression as MemberExpression;
            }
        }
        public static MemberExpression GetRightMostMember(this Expression e)
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

            throw new Exception(e.ToString());
        }

        public static string ToPath(this MemberExpression e)
        {
            string path = "";
            var parent = e.Expression as MemberExpression;

            if (parent != null)
            {
                path = parent.ToPath() + ".";
            }

            return path + e.Member.Name;
        }
        public static string PropertyName<TDomain,TProp>(this Expression<Func<TDomain, TProp>> expression)
        {
            LambdaExpression lambda = expression as LambdaExpression;
            if (lambda == null)
                throw new ArgumentNullException("expression");

            MemberExpression memberExpr = null;

            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                memberExpr =
                    ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }

            if (memberExpr == null)
                throw new ArgumentException("method");

            return memberExpr.Member.Name;
        }

        #region Shicz,2013-7-29 修改
        public static MemberExpression ConvertMemberExpressionForConstant(this MemberExpression expression)
        {
            if (expression.Expression is ConstantExpression)
            {
                return expression;
            }
            else
            {
                return ConvertMemberExpressionForConstant(expression.Expression as MemberExpression);
            }
        }
        #endregion

        /// <summary>
        /// 深拷贝方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T source)
        {
            var serializer = new DataContractSerializer(typeof(T));
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, source);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)serializer.ReadObject(ms);
            }
        }
    }
}
