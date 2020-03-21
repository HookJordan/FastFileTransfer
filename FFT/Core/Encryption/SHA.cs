using System.Security.Cryptography;
using System.Text;

namespace FFT.Core.Encryption
{
    class SHA
    {
        public static string Encode(string data)
        {
            return Encode(Encoding.ASCII.GetBytes(data));
        }

        public static string Encode(byte[] data)
        {
            using (SHA256 sha = SHA256.Create())
            {
                data = sha.ComputeHash(data);

                StringBuilder sb = new StringBuilder();
                for(int i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
