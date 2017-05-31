#region copyright
// <copyright file="TypeExtensions.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>����</author>
// <datecreated>2012-11-20</datecreated>
#endregion
namespace Han.Infrastructure.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class TypeExtensions
    {
        #region Public Methods and Operators
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetPropertys(this Type type)
        {

            return Reflection.Reflection.GetPropertys(type);
        }
        /// <summary>
        /// ��ȡ�����������ԣ��������Ͱ���IsPrimitive()�ж����������Enum�Լ�IsNullableEnum
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetPrimitivePropertys(this Type type)
        {
            
            return Reflection.Reflection.GetPropertys(type).Where(p => p.PropertyType.IsPrimitive()||p.PropertyType.IsNullableEnum()||p.PropertyType.IsEnum).ToList();
        }
        /// <summary>
        /// �ж������Ƿ�ǿ�ö��
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsNullableEnum(this Type t)
        {
            Type u = Nullable.GetUnderlyingType(t);
            return (u != null) && u.IsEnum;
        }
       
        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetComplexPropertys(this Type type)
        {

            return Reflection.Reflection.GetPropertys(type).Where(p => !p.PropertyType.IsPrimitive()).ToList();
        }
        /// <exception cref="System.InvalidCastException"></exception>
        public static dynamic DynamicCast(this Type T, dynamic o)
        {
            return
                typeof(TypeExtensions).GetMethod("Cast", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod
                    (T).Invoke(null, new object[] { o });
        }
        public static bool IsPrimitive(this Type T)
        {
            // TODO: put any type here that you consider as primitive as I didn't
            // quite understand what your definition of primitive type is
            return
                new[]
                    {
                        typeof(string), typeof(char), typeof(byte), typeof(ushort), typeof(short), typeof(uint),
                        typeof(int), typeof(ulong), typeof(long), typeof(float), typeof(double), typeof(decimal),
                        typeof(DateTime), typeof(Guid), typeof(char?), typeof(byte?), typeof(ushort?), typeof(short?),
                        typeof(uint?), typeof(int?), typeof(ulong?), typeof(long?), typeof(float?), typeof(double?),
                        typeof(decimal?), typeof(DateTime?), typeof(Guid?),typeof(bool),typeof(bool?),typeof(byte[])
                    }.Contains(T);
        }
        #endregion

        #region Methods
        /// <summary>
        /// ת���������ͣ�+string "false 0 true ,1"ת��Ϊbool
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static T Cast<T>(dynamic value)
        {
            if (typeof(T) == typeof(bool) && value is string)
            {
                if (value.ToString() == "1")
                {
                    return (T)Convert.ChangeType("True", typeof(T));
                }
                else if (value.ToString() == "0")
                {
                    return (T)Convert.ChangeType("False", typeof(T));
                }
                return (T)Convert.ToBoolean(value);
            }
            return (T)value;
        }
        //public static dynamic DynamicCast2<T>(this Type TT, dynamic o)
        //{
        //    return (T)o;
        //}
        #endregion
    }
}