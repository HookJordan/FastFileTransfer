using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FFT.Core.Encryption
{
    class AES
    {
        private static byte[] SALT = Encoding.ASCII.GetBytes("FFT - FastFileTransfer - AES SALT");
        public static byte[] Encrypt(byte[] input, byte[] password)
        {
            byte[] encrypted = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    using (var key = new Rfc2898DeriveBytes(password, SALT, 1000))
                    {
                        AES.Key = key.GetBytes(AES.KeySize / 8);
                        AES.IV = key.GetBytes(AES.BlockSize / 8);
                    }

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(input, 0, input.Length);
                        cs.Close();
                    }

                    encrypted = ms.ToArray();
                }
            }

            return encrypted;
        }

        public static byte[] Decrypt(byte[] input, byte[] password)
        {
            byte[] decrypted = null; 
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    using (var key = new Rfc2898DeriveBytes(password, SALT, 1000))
                    {
                        AES.Key = key.GetBytes(AES.KeySize / 8);
                        AES.IV = key.GetBytes(AES.BlockSize / 8);
                    }

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(input, 0, input.Length);
                        cs.Close();
                    }

                    decrypted = ms.ToArray();
                }
            }

            return decrypted;
        }
    }
}
