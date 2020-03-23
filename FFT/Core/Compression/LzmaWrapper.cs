using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFT.Core.Compression
{
    class LzmaWrapper
    {
        public static byte[] Compress(byte[] data)
        {
            using (MemoryStream rtn = new MemoryStream())
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    SevenZip.Helper.Compress(ms, rtn);
                }

                return rtn.ToArray();
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            using (MemoryStream rtn = new MemoryStream())
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    SevenZip.Helper.Decompress(ms, rtn);
                }

                return rtn.ToArray();
            }
        }
    }
}
