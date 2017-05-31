// -----------------------------------------------------------------------
// <copyright file="DateTimeHelper.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Han.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DateTimeHelper
    {
        /// <summary>
        /// 计算年龄
        /// </summary>
        /// <param name="end"></param>
        /// <param name="birth"></param>
        /// <returns></returns>
        public static int? GetAge(DateTime end, DateTime birth )
        {
            int? age = end.Year - birth.Year;
            //if (birth > end.AddYears(-age)) age--;
            if (birth.AddYears(age.Value) > end && age > 0)
                age--;
            return age < 0 ? null : age;
        }
        public static DateTime Min(DateTime date1, DateTime date2)
        {
            return (date1 < date2 ? date1 : date2);
        }
        /// <summary>
        /// 计算年龄new
        /// </summary>
        /// <param name="end"></param>
        /// <param name="birth"></param>
        /// <returns></returns>
        public static string GetAge_New(DateTime end, DateTime birth)
        {
            string strAge = "";

            // 计算天数
            int day = end.Day - birth.Day;

            if (day < 0)
            {
                //当天数差小于0时月数减1并且补上天数
                end = end.AddMonths(-1);
                day += DateTime.DaysInMonth(end.Year, end.Month);
            }

            // 计算月数
            int month = end.Month - birth.Month;

            if (month < 0)
            {
                //当月数差小于0时年数减1并且补上月数
                month += 12;
                end = end.AddYears(-1);
            }

            // 计算年数
            int year = end.Year - birth.Year;

            //一周岁以上显示年数
            if (year >= 1)
            {
                strAge = year.ToString();
                return strAge;
            }

            // 1月至1周岁之内显示月数
            if (month > 0)
            {
                //当月的天数
                double daysInMonth = Convert.ToDouble(DateTime.DaysInMonth(end.Year, end.Month));
                double a = Convert.ToDouble(day) / daysInMonth;
                strAge = (month + a).ToString("0.0") + "月";

                return strAge;
            }

            if (day <= 28)        // 28天以内的算日龄
            {
                if (day == 0)
                {
                    strAge = "1日";
                }
                else
                {
                    strAge = day.ToString() + "日";
                }
            }
            else                  //28至一个月之内算1.0月
            {
                strAge = "1.0月";
            }

            return strAge;

        }

        /// <summary>
        /// 获取给定时间月份第一天
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetsFirstDay(DateTime dt)
        {
            return dt.AddDays(1 - dt.Day);//给定时间的月份第一天
        }

        /// <summary>
        /// 获取给定时间月份最后一天
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetsEndDay(DateTime dt)
        {
            //1. 取得月初
            //2. 月份+1 的前一天
            return dt.AddDays(1 - dt.Day).AddMonths(1).AddDays(-1);//给定时间月份最后一天
        }
    }
}
