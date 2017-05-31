// -----------------------------------------------------------------------
// <copyright file="DateTimeExtension.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Han.Infrastructure
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class DateTimeExtension
    {
        public static DateTime ToStartDate(this DateTime dateTime)
        {
            var str = dateTime.ToString("yyyy-MM-dd") + " 00:00:00";
            return DateTime.Parse(str);
        }
        public static DateTime ToEndDate(this DateTime dateTime)
        {
            var str = dateTime.ToString("yyyy-MM-dd") + " 23:59:59";
            return DateTime.Parse(str);
        }
        /// <summary>
        /// 转换为20100701格式字符串
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int ToYearMonth(this DateTime dateTime)
        {
            var str = dateTime.ToString("yyyyMMdd");
            return int.Parse(str);
        }

        /// <summary>
        /// 得到自定义的开始日期和结束日期
        /// </summary>
        /// <param name="inputMonth">日期的月份</param>
        /// <param name="lastMonthBeginDay">上月的天数</param>
        /// <param name="currentMonthEndDay">当月的天数</param>
        /// <returns></returns>
        public static Dictionary<int, DateTime> GetCustomDate(this DateTime inputMonth, int lastMonthBeginDay, int currentMonthEndDay)
        {
            var dic = new Dictionary<int, DateTime>();
            var beginDate = Convert.ToDateTime(string.Format("{0:yyyy-MM-01}", inputMonth));
            var endDate = beginDate.AddMonths(1);
            if (lastMonthBeginDay == 0)
            {
                dic.Add(0, beginDate);
                dic.Add(1, endDate);
            }
            else
            {
                if (currentMonthEndDay == 0)
                {
                    dic.Add(0, beginDate);
                    dic.Add(1, endDate);
                }
                else
                {
                    var lastMonth = beginDate.AddMonths(-1);
                    var lastMonthDaysInMonth = DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month);
                    var currentMonthDaysInMonth = DateTime.DaysInMonth(inputMonth.Year, inputMonth.Month);

                    var isBeginDayRight = lastMonthBeginDay > 0 && lastMonthBeginDay <= lastMonthDaysInMonth;
                    var isEndDayRight = currentMonthEndDay > 0 && currentMonthEndDay <= currentMonthDaysInMonth;

                    if (isBeginDayRight && isEndDayRight)
                    {
                        dic.Add(0,DateTime.Parse(string.Format("{0}-{1}-{2}",lastMonth.Year,lastMonth.Month,lastMonthBeginDay)));
                        dic.Add(1, DateTime.Parse(string.Format("{0}-{1}-{2}", beginDate.Year, beginDate.Month, currentMonthEndDay)).AddDays(1));
                    }
                    else
                    {
                        dic.Add(0, beginDate);
                        dic.Add(1, endDate);
                    }
                }
            }

            return dic;
        }

        /// <summary>
        /// 得到日期的自定义的月份
        /// </summary>
        /// <param name="inputDateTime"></param>
        /// <param name="lastMonthBeginDay"></param>
        /// <param name="currentMonthEndDay"></param>
        /// <returns></returns>
        public static DateTime GetCurrentMonth(this DateTime inputDateTime, int lastMonthBeginDay, int currentMonthEndDay)
        {
            if (lastMonthBeginDay == 0)
            {
                return inputDateTime;
            }

            if (currentMonthEndDay == 0)
            {
                return inputDateTime;
            }

            var dic = inputDateTime.GetCustomDate(lastMonthBeginDay, currentMonthEndDay);
            if (inputDateTime >= dic[0] && inputDateTime < dic[1])
            {
                return inputDateTime;
            }

            if (inputDateTime < dic[0])
            {
                return inputDateTime.AddMonths(-1);
            }

            return inputDateTime >= dic[1] ? inputDateTime.AddMonths(1) : inputDateTime;
        }

    }
}
