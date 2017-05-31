using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Han.Infrastructure
{
    public class NumHelper
    {
        private readonly static char[] chnGenText = new char[] { '零', '一', '二', '三', '四', '五', '六', '七', '八', '九' };
        private readonly static char[] chnGenDigit = new char[] { '十', '百', '千', '万', '亿' };

        public static string ToChinese(int num)
        {
            // 去掉数字前面所有的'0'
            // 并把数字分割到字符数组中
            char[] integral = (num.ToString()).ToCharArray();

            // 定义结果字符串
            StringBuilder strInt = new StringBuilder();

            int digit;
            digit = integral.Length - 1;

            // 使用正确的引用
            char[] chnText = chnGenText;
            char[] chnDigit = chnGenDigit;

            // 变成中文数字并添加中文数位
            // 处理最高位到十位的所有数字
            int i;
            for (i = 0; i < integral.Length - 1; i++)
            {
                // 添加数字
                strInt.Append(chnText[integral[i] - '0']);

                // 添加数位
                if (0 == digit % 4)     // '万' 或 '亿'
                {
                    if (4 == digit || 12 == digit)
                    {
                        strInt.Append(chnDigit[3]); // '万'
                    }
                    else if (8 == digit)
                    {
                        strInt.Append(chnDigit[4]); // '亿'
                    }
                }
                else         // '十'，'百'或'千'
                {
                    strInt.Append(chnDigit[digit % 4 - 1]);
                }

                digit--;
            }

            // 如果个位数不是'0'
            // 或者只有一位数
            // 则添加相应的中文数字
            if ('0' != integral[integral.Length - 1] || 1 == integral.Length)
            {
                strInt.Append(chnText[integral[i] - '0']);
            }

            // 遍历整个字符串
            i = 0;
            string strTemp; // 临时存储字符串
            int j;    // 查找“零x”结构时用
            bool bDoSomething; // 找到“零万”或“零亿”时为真

            while (i < strInt.Length)
            {
                j = i;

                bDoSomething = false;

                // 查找所有相连的“零x”结构
                while (j < strInt.Length - 1 && "零" == strInt.ToString().Substring(j, 1))
                {
                    strTemp = strInt.ToString().Substring(j + 1, 1);

                    // 如果是“零万”或者“零亿”则停止查找
                    if (chnDigit[3].ToString() /* 万或萬 */ == strTemp ||
                     chnDigit[4].ToString() /* 亿或億 */ == strTemp)
                    {
                        bDoSomething = true;
                        break;
                    }

                    j += 2;
                }

                if (j != i) // 如果找到非“零万”或者“零亿”的“零x”结构，则全部删除
                {
                    strInt = strInt.Remove(i, j - i);

                    // 除了在最尾处，或后面不是"零万"或"零亿"的情况下, 
                    // 其他处均补入一个“零”
                    if (i <= strInt.Length - 1 && !bDoSomething)
                    {
                        strInt = strInt.Insert(i, '零');
                        i++;
                    }
                }

                if (bDoSomething) // 如果找到"零万"或"零亿"结构
                {
                    strInt = strInt.Remove(i, 1); // 去掉'零'
                    i++;
                    continue;
                }

                // 指针每次可移动2位
                i += 2;
            }

            // 遇到“亿万”变成“亿零”或"亿"
            strTemp = chnDigit[4].ToString() + chnDigit[3].ToString(); // 定义字符串“亿万”或“億萬”
            int index = strInt.ToString().IndexOf(strTemp);
            if (-1 != index)
            {
                if (strInt.Length - 2 != index && // 如果"亿万"不在末尾
                 (index + 2 < strInt.Length
                  && "零" != strInt.ToString().Substring(index + 2, 1))) // 并且其后没有"零"
                {
                    strInt = strInt.Replace(strTemp, chnDigit[4].ToString(), index, 2); // 变“亿万”为“亿零”
                    strInt = strInt.Insert(index + 1, "零");
                }
                else // 如果“亿万”在末尾或是其后已经有“零”
                {
                    strInt = strInt.Replace(strTemp, chnDigit[4].ToString(), index, 2); // 变“亿万”为“亿”
                }
            }

            // 开头为“一十”改为“十”
            if (strInt.Length > 1 && "一十" == strInt.ToString().Substring(0, 2))
            {
                strInt = strInt.Remove(0, 1);
            }

            return strInt.ToString();
        }
    }
}
