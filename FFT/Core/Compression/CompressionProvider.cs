namespace FFT.Core.Compression
{
    public enum CompressionAlgorithm
    {
        Disabled = 0,
        GZIP,
        LZMA,
        LZO
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
                case CompressionAlgorithm.LZO:
                    return LZO.LZO.Compress(input);
                case CompressionAlgorithm.LZMA:
                    return LzmaWrapper.Compress(input);
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
                case CompressionAlgorithm.LZO:
                    return LZO.LZO.Decompress(input);
                case CompressionAlgorithm.LZMA:
                    return LzmaWrapper.Decompress(input);
                default:
                    return input;
            }
        }

    }
}
