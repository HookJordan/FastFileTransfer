using FFT.Core.Compression;
using FFT.Core.Encryption;
using System.IO;

namespace FFT.Core
{
    public class Configuration
    {
        public int Port;
        public int BufferSize;

        public CompressionAlgorithm compressionAlgorithm;
        public CryptoAlgorithm cryptoAlgorithm;

        private Configuration()
        {
            // Defaults
            this.Port = 15056;
            this.BufferSize = 256;
            this.compressionAlgorithm = CompressionAlgorithm.GZIP;
            this.cryptoAlgorithm = CryptoAlgorithm.RC4;
        }

        public void Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(Port);
                    bw.Write(BufferSize);
                    bw.Write((int)compressionAlgorithm);
                    bw.Write((int)cryptoAlgorithm);
                }
            }
        }

        public static Configuration FromFile(string path)
        {
            var config = new Configuration();
            // If configuration doesn't exist, create new configuration file
            if (!File.Exists(path))
            {
                config.Save(path);
                return config;
            }
            else
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        config.Port = br.ReadInt32();
                        config.BufferSize = br.ReadInt32();
                        config.compressionAlgorithm = (CompressionAlgorithm)br.ReadInt32();
                        config.cryptoAlgorithm = (CryptoAlgorithm)br.ReadInt32();
                    }
                }
                return config;
            }
        }
    }
}
