using FFT.Core.Compression;
using FFT.Core.Encryption;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace FFT.Core
{
    public class Configuration
    {
        private static byte[] KEY = Encoding.ASCII.GetBytes("F@$tF1l3Tr@n$f3rK3y$$1024256%");

        public int Port;
        public int BufferSize;

        public CompressionAlgorithm compressionAlgorithm;
        public CryptoAlgorithm cryptoAlgorithm;

        // Added in 1.1.0
        public List<string> ProtectedFolders { get; set; }

        // Added in 1.2.0
        public bool DebugMode { get; set; }

        // Added in 1.3.0 
        public bool UnattendedAccess { get; set; }
        public bool StartWithWindows { get; set; }
        public string PersonalPassword { get; set; }
        private Configuration()
        {
            // Defaults
            this.Port = 15056;
            this.BufferSize = 256;
            this.compressionAlgorithm = CompressionAlgorithm.GZIP;
            this.cryptoAlgorithm = CryptoAlgorithm.RC4;

            this.ProtectedFolders = new List<string>();
            this.ProtectedFolders.AddRange(new string[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.Windows)
            });

            this.DebugMode = false;
            this.StartWithWindows = false;
            this.UnattendedAccess = false;
            this.PersonalPassword = string.Empty;
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

                    bw.Write(ProtectedFolders.Count);
                    foreach (string dir in ProtectedFolders)
                    {
                        bw.Write(dir);
                    }

                    bw.Write(DebugMode);

                    bw.Write(UnattendedAccess);
                    bw.Write(StartWithWindows);

                    // Encrypt personal password before storage
                    byte[] personalPass = Encoding.ASCII.GetBytes(PersonalPassword);
                    RC4.Perform(ref personalPass, KEY);
                    bw.Write(personalPass.Length);
                    bw.Write(personalPass);
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

                        try
                        {
                            // This will cause errors until the config files have been updated...
                            int pfCount = br.ReadInt32();
                            config.ProtectedFolders = new List<string>();

                            for (int i = 0; i < pfCount; i++)
                            {
                                config.ProtectedFolders.Add(br.ReadString());
                            }

                            config.DebugMode = br.ReadBoolean();

                            config.UnattendedAccess = br.ReadBoolean();
                            config.StartWithWindows = br.ReadBoolean();

                            // Decrypt personal password
                            int passLen = br.ReadInt32();
                            byte[] rawPass = br.ReadBytes(passLen);
                            RC4.Perform(ref rawPass, KEY);
                            config.PersonalPassword = Encoding.ASCII.GetString(rawPass);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                return config;
            }
        }
    }
}
