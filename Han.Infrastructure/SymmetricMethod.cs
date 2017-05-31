using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Han.Log;

namespace Han.Infrastructure
{
    /// <summary>
    ///     加密类
    /// </summary>
    public class SymmetricMethod
    {
        private readonly string Key;
        private readonly SymmetricAlgorithm mobjCryptoService;

        /// <summary>
        ///     构造函数
        /// </summary>
        public SymmetricMethod()
        {
            mobjCryptoService = new AesManaged();
            Key = @"Guz(%&hj7x89H$yuBI0456FtmaT5&fvHUFCy76*h%(HilJ$lhj!y6&(*jkP87jH7";
        }

        /// <summary>
        ///     获得密钥
        /// </summary>
        /// <returns></returns>
        private byte[] GetLegalKey()
        {
            string sTemp = Key;
            mobjCryptoService.GenerateKey();
            byte[] bytTemp = mobjCryptoService.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return Encoding.UTF8.GetBytes(sTemp);
        }

        private byte[] GetLegalIV()
        {
            string sTemp = @"E4ghj*Ghg7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%$hjk";
            mobjCryptoService.GenerateIV();
            byte[] bytTemp = mobjCryptoService.IV;
            int IVLength = bytTemp.Length;
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return Encoding.UTF8.GetBytes(sTemp);
        }

        /// <summary>
        ///     加密方法
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public string Encrypto(string Source)
        {
            byte[] bytIn = Encoding.UTF8.GetBytes(Source);
            var ms = new MemoryStream();
            mobjCryptoService.Key = GetLegalKey();
            mobjCryptoService.IV = GetLegalIV();
            ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();
            var cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
            cs.Write(bytIn, 0, bytIn.Length);
            cs.FlushFinalBlock();
            ms.Close();
            byte[] bytOut = ms.ToArray();
            return Convert.ToBase64String(bytOut);
        }

        /// <summary>
        ///     解密方法
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public string Decrypto(string Source)
        {
            try
            {
                byte[] bytIn = Convert.FromBase64String(Source);
                var ms = new MemoryStream(bytIn, 0, bytIn.Length);
                mobjCryptoService.Key = GetLegalKey();
                mobjCryptoService.IV = GetLegalIV();
                ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
                var cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
                var sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            catch (Exception)
            {
               // Logger.LogException(exp);
                return "";
            }
        }
    }
}
