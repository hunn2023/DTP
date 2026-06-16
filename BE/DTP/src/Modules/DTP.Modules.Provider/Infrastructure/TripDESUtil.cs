using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Infrastructure
{
    public class TripDESUtil
    {
        public static string Encrypt(string textToEncrypt, string key)
        {
            byte[] MyEncryptedArray = UTF8Encoding.UTF8.GetBytes(textToEncrypt);

            byte[] MysecurityKeyArray = UTF8Encoding.UTF8.GetBytes(key);

            var MyTripleDESCryptoService = new TripleDESCryptoServiceProvider();
            MyTripleDESCryptoService.Key = MysecurityKeyArray;
            MyTripleDESCryptoService.Mode = CipherMode.ECB;
            MyTripleDESCryptoService.Padding = PaddingMode.PKCS7;

            var MyCrytpoTransform = MyTripleDESCryptoService.CreateEncryptor();

            byte[] MyresultArray = MyCrytpoTransform.TransformFinalBlock(MyEncryptedArray, 0, MyEncryptedArray.Length);
            MyTripleDESCryptoService.Clear();

            StringBuilder hex = new StringBuilder(MyresultArray.Length * 2);
            foreach (byte b in MyresultArray)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }

        public static string Decrypt(string textToDecrypt, string key)
        {
            byte[] MyDecryptArray = Hex2Byte(textToDecrypt);
            byte[] MysecurityKeyArray = UTF8Encoding.UTF8.GetBytes(key);

            var MyTripleDESCryptoService = new TripleDESCryptoServiceProvider();

            MyTripleDESCryptoService.Key = MysecurityKeyArray;
            MyTripleDESCryptoService.Mode = CipherMode.ECB;
            MyTripleDESCryptoService.Padding = PaddingMode.PKCS7;

            var MyCrytpoTransform = MyTripleDESCryptoService.CreateDecryptor();

            byte[] MyresultArray = MyCrytpoTransform.TransformFinalBlock(MyDecryptArray, 0, MyDecryptArray.Length);
            MyTripleDESCryptoService.Clear();

            return UTF8Encoding.UTF8.GetString(MyresultArray);
        }

        public static byte[] Hex2Byte(string hex)
        {
            if ((hex.Length % 2) != 0)
            {
                throw new ArgumentException();
            }
            char[] chArray = hex.ToCharArray();
            byte[] buffer = new byte[hex.Length / 2];
            int index = 0;
            int num2 = 0;
            int length = hex.Length;
            while (index < length)
            {
                int num4 = Convert.ToInt16("" + chArray[index++] + chArray[index], 0x10) & 0xff;
                buffer[num2] = Convert.ToByte(num4);
                index++;
                num2++;
            }
            return buffer;
        }
    }
}
