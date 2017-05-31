/* ***********************************************
 * author :  闫刘盘
 * function: 批量扣款Dao
 * history:  created by 闫刘盘 2015/07/02 
 * ***********************************************/
namespace Han.Infrastructure.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class IEnumerableExtention
    {
        /// <summary>
        /// 将数组平滑平展，中间用逗号隔开
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static string ToSmoothString<T>(this IEnumerable<T> lst)
        {
            string strReturn = string.Empty;
            if (lst != null)
            {
                var enumerable = lst as T[] ?? lst.ToArray();
                if (enumerable.Any())
                {
                    var sb = new StringBuilder();
                    foreach (var item in enumerable)
                    {
                        sb.Append(item + ",");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    strReturn = sb.ToString();
                }
            }

            return strReturn;
        }
    }
}
