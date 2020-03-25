using System.IO.Compression;
using System.IO;

namespace FFT.Core.Compression
{
    class GZip
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
