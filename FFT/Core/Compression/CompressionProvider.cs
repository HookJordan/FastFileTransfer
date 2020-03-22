using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFT.Core.Compression
{
    public enum CompressionAlgorithm
    {
        Disabled = 0,
        GZIP,
        LZMA
    }
    public class CompressionProvider
    {
        public CompressionAlgorithm algorithm { get; private set; }

        public CompressionProvider(CompressionAlgorithm algorithm)
        {
            this.algorithm = algorithm;
        }

        public byte[] Compress(byte[] input)
        {
            switch (algorithm)
            {
                case CompressionAlgorithm.GZIP:
                    return GZip.Compress(input);
                default:
                    return input;
            }

        }

        public byte[] Decompress(byte[] input)
        {
            switch (algorithm)
            {
                case CompressionAlgorithm.GZIP:
                    return GZip.Decompress(input);
                default:
                    return input;
            }
        }

    }
}
