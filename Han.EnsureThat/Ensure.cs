#region copyright
// <copyright file="Ensure.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>����</author>
// <datecreated>2012-11-07</datecreated>
#endregion
namespace Han.EnsureThat
{
    using System;
    using System.Linq.Expressions;

    using Han.EnsureThat.Core;

    public static class Ensure
    {
        #region Public Methods and Operators

        /// <summary>
        /// ȷ��ֵ���Ϲ���
        ///   Ensure.That(value).IsNotNullOrEmpty();
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">ֵ</param>
        /// <param name="name">������</param>
        /// <returns></returns>
        public static Param<T> That<T>(T value, string name = Param.DefaultName)
        {
            return new Param<T>(name, value);
        }

        /// <summary>
        /// ȷ��ֵ���Ϲ���
        /// Ensure.That(()=>!string.IsNullOrEmpty(value)).IsTrue();
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Param<T> That<T>(Expression<Func<T>> expression)
        {
            MemberExpression memberExpression = expression.GetRightMostMember();

            return new Param<T>(memberExpression.ToPath(), expression.Compile().Invoke());
        }

        /// <summary>
        /// ȷ��ֵ���ͷ��Ϲ���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name">����������ѡ</param>
        /// <returns></returns>
        public static TypeParam ThatTypeFor<T>(T value, string name = Param.DefaultName)
        {
            return new TypeParam(name, value.GetType());
        }

        #endregion
    }
}