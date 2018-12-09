using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HB.Common.Core.Framework.Helpers
{
    /// <summary>
    /// 加密编码工具类
    /// </summary>
    public class EncryptHelper
    {
        private static string key = "8HBGSE4S6V3GD3B2H4S5FD";
        public enum Strength
        {
            Invalid,
            Weak,
            Normal,
            Strong
        }

        /// <summary>
        /// MD5X编码
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5XEncode(string password)
        {
            string result;
            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password))
            {
                result = string.Empty;
            }
            else
            {
                UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
                byte[] bytes = unicodeEncoding.GetBytes(password);
                System.Security.Cryptography.MD5CryptoServiceProvider mD5CryptoServiceProvider = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] value = mD5CryptoServiceProvider.ComputeHash(bytes);
                string text = BitConverter.ToString(value);
                text += "ZX";
                System.Security.Cryptography.SHA1 sHA = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                value = sHA.ComputeHash(unicodeEncoding.GetBytes(text));
                result = BitConverter.ToString(value).Replace("-", string.Empty);
            }
            return result;
        }


        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SHA1(string str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            byte[] value = System.Security.Cryptography.SHA1.Create().ComputeHash(bytes);
            return BitConverter.ToString(value).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string EncodeBase64(string source)
        {
            return EncryptHelper.EncodeBase64(Encoding.UTF8, source);
        }

        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="encode">编码类型</param>
        /// <param name="source"></param>
        /// <returns>编码结果</returns>
        public static string EncodeBase64(Encoding encode, string source)
        {
            string result = "";
            byte[] bytes = encode.GetBytes(source);
            try
            {
                result = Convert.ToBase64String(bytes);
            }
            catch
            {
                result = source;
            }
            return result;
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="encode"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string DecodeBase64(Encoding encode, string result)
        {
            string result2 = "";
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                result2 = encode.GetString(bytes);
            }
            catch
            {
                result2 = result;
            }
            return result2;
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string DecodeBase64(string result)
        {
            return EncryptHelper.DecodeBase64(Encoding.UTF8, result);
        }

        public static EncryptHelper.Strength PasswordStrength(string password)
        {
            EncryptHelper.Strength result;
            if (string.IsNullOrEmpty(password))
            {
                result = EncryptHelper.Strength.Invalid;
            }
            else
            {
                int num = 0;
                int num2 = 0;
                int num3 = 0;
                for (int i = 0; i < password.Length; i++)
                {
                    char c = password[i];
                    if (c >= '0' && c <= '9')
                    {
                        num++;
                    }
                    else
                    {
                        if (c >= 'a' && c <= 'z')
                        {
                            num2++;
                        }
                        else
                        {
                            if (c >= 'A' && c <= 'Z')
                            {
                                num2++;
                            }
                            else
                            {
                                num3++;
                            }
                        }
                    }
                }
                if (num2 == 0 && num3 == 0)
                {
                    result = EncryptHelper.Strength.Weak;
                }
                else
                {
                    if (num == 0 && num2 == 0)
                    {
                        result = EncryptHelper.Strength.Weak;
                    }
                    else
                    {
                        if (num == 0 && num3 == 0)
                        {
                            result = EncryptHelper.Strength.Weak;
                        }
                        else
                        {
                            if (password.Length <= 6)
                            {
                                result = EncryptHelper.Strength.Weak;
                            }
                            else
                            {
                                if (num2 == 0)
                                {
                                    result = EncryptHelper.Strength.Normal;
                                }
                                else
                                {
                                    if (num3 == 0)
                                    {
                                        result = EncryptHelper.Strength.Normal;
                                    }
                                    else
                                    {
                                        if (num == 0)
                                        {
                                            result = EncryptHelper.Strength.Normal;
                                        }
                                        else
                                        {
                                            if (password.Length <= 10)
                                            {
                                                result = EncryptHelper.Strength.Normal;
                                            }
                                            else
                                            {
                                                result = EncryptHelper.Strength.Strong;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// AES
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static byte[] GetAesKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "Aes密钥不能为空");
            }
            if (key.Length < 32)
            {
                key = key.PadRight(32, '0');
            }
            if (key.Length > 32)
            {
                key = key.Substring(0, 32);
            }
            return Encoding.UTF8.GetBytes(key);
        }

        /// <summary>
        /// AES编码
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string EncryptAes(string source, string key)
        {
            string result;
            using (AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider())
            {
                aesCryptoServiceProvider.Key = EncryptHelper.GetAesKey(key);
                aesCryptoServiceProvider.Mode = CipherMode.ECB;
                aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryptoTransform = aesCryptoServiceProvider.CreateEncryptor())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(source);
                    byte[] array = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
                    aesCryptoServiceProvider.Clear();
                    aesCryptoServiceProvider.Dispose();
                    result = Convert.ToBase64String(array, 0, array.Length);
                }
            }
            return result;
        }

        /// <summary>
        /// AES解码
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DecryptAes(string source, string key)
        {
            string @string;
            using (AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider())
            {
                aesCryptoServiceProvider.Key = EncryptHelper.GetAesKey(key);
                aesCryptoServiceProvider.Mode = CipherMode.ECB;
                aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryptoTransform = aesCryptoServiceProvider.CreateDecryptor())
                {
                    byte[] array = Convert.FromBase64String(source);
                    byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
                    aesCryptoServiceProvider.Clear();
                    @string = Encoding.UTF8.GetString(bytes);
                }
            }
            return @string;
        }

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns> 
        public static string DESEncrypt(string encryptString)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(key.Substring(0, 8));
                byte[] rgbIV = rgbKey;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                cStream.Close();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns> 
        public static string DESDecrypt(string decryptString)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(key.Substring(0, 8));
                byte[] rgbIV = rgbKey;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                cStream.Close();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }


        public static string Md5(string str)
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("x2");
            }
            return pwd;
        }

    }
}
