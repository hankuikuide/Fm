#region copyright
// <copyright file="DbHelper.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>丁浩</author>
// <datecreated>2012-12-17</datecreated>
#endregion

using Han.Log;

namespace Han.DbLight
{
    using System;
    using System.Data;

    using Han.Infrastructure;
    using Han.Infrastructure.Extensions;

    public static class DbHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// 转换类型，dbnull转换为null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ConvertFromDbVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default(T); // returns the default value for the type
            }
            else
            {
                return typeof(T).DynamicCast(obj);
            }
        }

        /// <summary>
        /// 基本类型转换,列名不存在不抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetColumnValue<T>(IDataRecord row, string name)
        {
            if (typeof(T).IsPrimitive())
            {
                int index;
                if (TryGetColumnIndex(row, name, out index))
                {
                    object t = row.GetValue(index);
                    return ConvertFromDbVal<T>(t);
                }
                return default(T);
                //throw new Exception("不能找到列名："+name);
            }
            throw new Exception("非基本类型不能转换");
        }

       
       
       
     

        /// <summary>
        /// 根据列名获取列的索引，
        /// </summary>
        /// <param name="row"></param>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static bool TryGetColumnIndex(IDataRecord row, string name, out int index)
        {
            try
            {
                index = row.GetOrdinal(name);
                return true;
            }
            catch (Exception)
            {
                index = -1;
                return false;
            }
        }

        /// <summary>
        /// 返回指定列的值，返回false
        /// </summary>
        /// <param name="row"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool TryGetColumnValue(IDataRecord row, string name, out object val)
        {
            int index;
            val = null;
            if (TryGetColumnIndex(row, name, out index))
            {
                try
                {
                    val = row.GetValue(index);
                }
                catch (Exception ex)
                {
                    Log.Logger.Log(Level.Error,"列名:"+name+ ex.ToString());
                    val = null;
                    return false;
                }
               
                if (val is DBNull)
                {
                    val = null;
                }
                return true;
            }
            return false;
        }
        ///// <summary>
        ///// 转义字符避免注入
        ///// </summary>
        ///// <param name="strRawText"></param>
        ///// <returns></returns>
        //public static string SqlSafe(string strRawText)
        //{
        //    string strCleanedText = "";
        //    int iCharPos = 0;

        //    while (iCharPos < strRawText.Length)
        //    {
        //        if (strRawText.Substring(iCharPos, 1) == "'")
        //        {
        //            strCleanedText = strCleanedText + "''";
        //            if (iCharPos != strRawText.Length)
        //            {
        //                if (strRawText.Substring(iCharPos + 1, 1) == "'")
        //                {
        //                    iCharPos = iCharPos + 1;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            strCleanedText = strCleanedText + strRawText.Substring(iCharPos, 1);
        //        }

        //        iCharPos++;
        //    }

        //    return strCleanedText.Trim();
        //}
        #endregion
    }

}