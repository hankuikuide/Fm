namespace Han.Infrastructure
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class RsaPublicKey
    {
        public string Exponent { get; set; }
        public string Modulus { get; set; }
    }

    /// <summary> 
    /// RSA加密解密及RSA签名和验证
    /// </summary> 
    public class RSACryption
    {
        /// <summary>
        /// The rsap.
        /// </summary>
        private readonly RSAParameters rsap = new RSAParameters()
        {
            Modulus =
                Convert.FromBase64String(
                    @"sB6+4rtO2sYeIZ8kJGGM647PIm+dJkwvSPNWcQ01D2cwPjIGV2c41h39FjYuzgAKzrIFjSvuBpG4y/PFEHuN+
        LackSt6MU7qcbs7lzub8V97XZ5fddPaq/GWXo9mrIMMFDMW7z88WrukLGTvwkqySPBemc22rjua1uTR3azae7U="), 
            Exponent = Convert.FromBase64String(@"AQAB"), 
            P =
                Convert.FromBase64String(
                    @"8yUCFVCufr3z2LDAwHaUO4r3na3WZqhAb3J7aXv/rj9UEXQWwZoG8IbUzV2fUhMXjnFXyrRSqywWdpxeE6oLWw=="), 
            Q =
                Convert.FromBase64String(
                    @"uW6NlpzkBl4Do7K4RUDCsZ9uiVqnU0cbm7JVuygWJts+pu1ho5s0auUekQy5al6p4xifjWIcCsLvPxsLuWISLw=="), 
            DP =
                Convert.FromBase64String(
                    @"rDsf0ad4I3E8hNcXgn28nLzgj8Hu6ILwOcGXZ+4c+/oB++cGo5cOqVxo6xwRWhsKCa2B6aV4FaZCNzymazl9lw=="), 
            DQ =
                Convert.FromBase64String(
                    @"dVVT+FKMIs9IZEPJP+DrkTM94WHgcNyUxp9Aii2iXrHqYfvhBYJG18Dk54lypbECtLU2+GJ1NgYFFxxI/ePldw=="), 
            InverseQ =
                Convert.FromBase64String(
                    @"z8qRY0+yyfZFNFPMtlTumpYyCXUbK+GpWnFp2hOyTABya/h7g4DCRE6iO9UZKgW4paB5K75mJwdBgVib5NgFiQ=="), 
            D =
                Convert.FromBase64String(@"W1ZWoLeLWaJNlho2YDfHIZLakX1Y/reb/jVUqySyU96sAlVnPITn0QOUcaR/+Y3EDRX+EwypUPbZ48v0c2vgYDHwIb
rIbsEyN+vHoUNJ319R5kUZ8Wlfw/w6/6BSclqbWQ8OdSj1cKwx/EEJh4iipqJ8HBTsmoT0anQHP/jdybE=")
        };

        #region RSA 加密解密 

        #region RSA 的密钥产生 

        public static byte[] HexStringToBytes(string hex)
        {
            if (hex.Length == 0)
            {
                return new byte[] { 0 };
            }

            if (hex.Length % 2 == 1)
            {
                hex = "0" + hex;
            }

            byte[] result = new byte[hex.Length / 2];

            for (int i = 0; i < hex.Length / 2; i++)
            {
                result[i] = byte.Parse(hex.Substring(2 * i, 2), NumberStyles.AllowHexSpecifier);
            }

            return result;
        }

        /// <summary>
        /// RSA 的密钥产生 产生私钥 和公钥 
        /// </summary>
        /// <param name="publicKey">
        /// The public Key.
        /// </param>
        /// <param name="xmlPrivateKey">
        /// The xml Private Key.
        /// </param>
        public void RSAKey(RsaPublicKey publicKey, out string xmlPrivateKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsap);
                xmlPrivateKey = rsa.ToXmlString(true);

                publicKey.Exponent = BytesToHexString(rsap.Exponent);
                publicKey.Modulus = BytesToHexString(rsap.Modulus);
            }
        }

        /// <summary>
        /// The get public key.
        /// </summary>
        /// <returns>
        /// The <see cref="RsaPublicKey"/>.
        /// </returns>
        public RsaPublicKey GetPublicKey()
        {
            var publicKey = new RsaPublicKey()
            {
                Exponent = BytesToHexString(rsap.Exponent), 
                Modulus = BytesToHexString(rsap.Modulus)
            };

            return publicKey;
        }

        /// <summary>
        /// The get private key.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetPrivateKey()
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsap);
                return rsa.ToXmlString(true);
            }
        }

        #endregion

        #region RSA的加密函数 

        // ############################################################################## 
        // RSA 方式加密 
        // 说明KEY必须是XML的行式,返回的是字符串 
        // 在有一点需要说明！！该加密方式有 长度 限制的！！ 
        // ############################################################################## 

        // RSA的加密函数  string
        public string RSAEncrypt(string xmlPublicKey, string m_strEncryptString)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xmlPublicKey);

                var plainTextBArray = (new UnicodeEncoding()).GetBytes(m_strEncryptString);
                var cypherTextBArray = rsa.Encrypt(plainTextBArray, false);
                var result = Convert.ToBase64String(cypherTextBArray);

                return result;
            }
        }

        // RSA的加密函数 byte[]
        public string RSAEncrypt(string xmlPublicKey, byte[] encryptString)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xmlPublicKey);

                var cypherTextBArray = rsa.Encrypt(encryptString, false);
                var result = Convert.ToBase64String(cypherTextBArray);

                return result;
            }
        }

        #endregion

        #region RSA的解密函数 

        // RSA的解密函数  string
        public string RSADecrypt(string xmlPrivateKey, string m_strDecryptString)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xmlPrivateKey);

                var plainTextBArray = HexStringToBytes(m_strDecryptString);
                var result = rsa.Decrypt(plainTextBArray, false);
                var enc = new ASCIIEncoding();

                return enc.GetString(result);
            }
        }

        // RSA的解密函数  byte
        public string RSADecrypt(string xmlPrivateKey, byte[] decryptString)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xmlPrivateKey);

                var dypherTextBArray = rsa.Decrypt(decryptString, false);
                var result = (new UnicodeEncoding()).GetString(dypherTextBArray);

                return result;
            }
        }

        #endregion

        #endregion

        #region RSA数字签名 

        #region 获取Hash描述表 

        // 获取Hash描述表 ，outofmemory.cn
        public bool GetHash(string m_strSource, ref byte[] hashData)
        {
            // 从字符串中取得Hash描述 
            var md5 = HashAlgorithm.Create("MD5");
            var buffer = Encoding.GetEncoding("GB2312").GetBytes(m_strSource);

            if (md5 != null)
            {
                hashData = md5.ComputeHash(buffer);
            }

            return true;
        }

        // 获取Hash描述表 
        public bool GetHash(string m_strSource, ref string strHashData)
        {
            // 从字符串中取得Hash描述 
            var md5 = HashAlgorithm.Create("MD5");
            var buffer = Encoding.GetEncoding("GB2312").GetBytes(m_strSource);

            if (md5 != null)
            {
                var hashData = md5.ComputeHash(buffer);
                strHashData = Convert.ToBase64String(hashData);
            }

            return true;
        }

        // 获取Hash描述表 
        public bool GetHash(FileStream objFile, ref byte[] hashData)
        {
            // 从文件中取得Hash描述 
            var md5 = HashAlgorithm.Create("MD5");

            if (md5 != null)
            {
                hashData = md5.ComputeHash(objFile);
            }

            objFile.Close();

            return true;
        }

        // 获取Hash描述表 
        public bool GetHash(FileStream objFile, ref string strHashData)
        {
            // 从文件中取得Hash描述 
            var md5 = HashAlgorithm.Create("MD5");

            if (md5 != null)
            {
                var hashData = md5.ComputeHash(objFile);

                objFile.Close();

                strHashData = Convert.ToBase64String(hashData);
            }

            return true;
        }

        #endregion
        #endregion

        private string BytesToHexString(byte[] input)
        {
            var hexString = new StringBuilder(64);

            for (int i = 0; i < input.Length; i++)
            {
                hexString.Append(string.Format("{0:X2}", input[i]));
            }

            return hexString.ToString();
        }
    }

    #region RSA

    #endregion RSA
}
