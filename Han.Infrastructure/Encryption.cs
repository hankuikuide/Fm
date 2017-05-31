#region copyright
// <copyright file="HupMd5.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>丁浩</author>
// <datecreated>2012-11-08</datecreated>
#endregion

using System.IO;

namespace Han.Infrastructure
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class Encryption
    {

        /// <summary>
        /// 计算 SHA256 哈希值
        /// </summary>
        /// <param name="inString"></param>
        /// <returns></returns>
        public static string Sha256(string inString)
        {
            byte[] _byte = Encoding.UTF8.GetBytes(inString);
            SHA256 _sha256 = new SHA256Managed();
            byte[] _result = _sha256.ComputeHash(_byte);
            return Convert.ToBase64String(_result, 0, _result.Length);
        }

        #region AES 加密解密

        // 使用AES加密字符串
        /// <summary>
        /// 使用AES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密字符串</param>
        /// <returns>加密结果，加密失败则返回源串</returns>
        public static string EncryptAES(string encryptString)
        {
            return EncryptAES(encryptString, "", "");
        }

        // 使用AES加密字符串
        /// <summary>
        /// 使用AES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密字符串</param>
        /// <param name="encryptKey">加密密匙</param>
        /// <param name="salt">盐</param>
        /// <returns>加密结果，加密失败则返回源串</returns>
        private static string EncryptAES(string encryptString, string encryptKey, string salt)
        {
            AesManaged aes = null;
            MemoryStream ms = null;
            CryptoStream cs = null;

            try
            {
                if (encryptKey == "")
                    encryptKey = "CIS20141125";

                if (salt == "")
                    salt = "CIS20141125";

                Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(encryptKey, Encoding.UTF8.GetBytes(salt));

                aes = new AesManaged();
                aes.Key = rfc2898.GetBytes(aes.KeySize / 8);
                aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);

                ms = new MemoryStream();
                cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);

                byte[] data = Encoding.UTF8.GetBytes(encryptString);
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();

                return Convert.ToBase64String(ms.ToArray());
            }
            catch
            {
                return encryptString;
            }
            finally
            {
                if (cs != null)
                    cs.Close();

                if (ms != null)
                    ms.Close();

                if (aes != null)
                    aes.Clear();
            }
        }

        // 使用AES解密字符串
        /// <summary>
        /// 使用AES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密字符串</param>
        /// <returns>解密结果，解谜失败则返回源串</returns>
        public static string DecryptAES(string decryptString)
        {
            return DecryptAES(decryptString, "", "");
        }

        // 使用AES解密字符串
        /// <summary>
        /// 使用AES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密字符串</param>
        /// <param name="decryptKey">解密密匙</param>
        /// <param name="salt">盐</param>
        /// <returns>解密结果，解谜失败则返回源串</returns>
        private static string DecryptAES(string decryptString, string decryptKey, string salt)
        {
            AesManaged aes = null;
            MemoryStream ms = null;
            CryptoStream cs = null;

            try
            {
                Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(decryptKey, Encoding.UTF8.GetBytes(salt));

                aes = new AesManaged();
                aes.Key = rfc2898.GetBytes(aes.KeySize / 8);
                aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);

                ms = new MemoryStream();
                cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);

                byte[] data = Convert.FromBase64String(decryptString);
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();

                return Encoding.UTF8.GetString(ms.ToArray(), 0, ms.ToArray().Length);
            }
            catch
            {
                return decryptString;
            }
            finally
            {
                if (cs != null)
                    cs.Close();

                if (ms != null)
                    ms.Close();

                if (aes != null)
                    aes.Clear();
            }
        }

        #endregion AES

    }
}
