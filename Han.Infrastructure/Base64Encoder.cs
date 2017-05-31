/* ***********************************************
 * author :  xieb
 * function: 药品信息解密
 * history:  created by xieb 2015/9/25 11:26:15 
 * ***********************************************/
using System;
using System.Text;

namespace Han.Infrastructure
{
    /// <summary>
    /// Base64
    /// </summary>
    public static class Base64Encoder
    {
        /// <summary>
        /// 编码解密
        /// </summary>
        /// <param name="code"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ConvertData(string code, string content)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(content))
            {
                return "";
            }
            try
            {
                string key = Encryption.EncryptAES(Convert.ToBase64String(Encoding.UTF8.GetBytes(code)));
                int len = key.Length / 2;

                content = content.Replace(key.Substring(0, len), "");
                content = content.Replace(key.Substring(len), "");
                byte[] bcontent = Convert.FromBase64String(content);
                content = Encoding.UTF8.GetString(bcontent, 0, bcontent.Length);
                return content;
            }
            catch
            {
                return "";
            }

        }
    }
}
