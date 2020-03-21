using System.IO;
using System.IO.Compression;

namespace FFT.Core.IO
{
    class Compression
    {
        public static byte[] Compress(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gz = new GZipStream(ms, CompressionMode.Compress))
                {
                    gz.Write(data, 0, data.Length);
                }

                return ms.ToArray();
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            using (MemoryStream rtn = new MemoryStream())
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (GZipStream gz = new GZipStream(ms, CompressionMode.Decompress))
                    {
                        gz.CopyTo(rtn);
                    }

                }

                return rtn.ToArray();
            }
        }
    }
}
