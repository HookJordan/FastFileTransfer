using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFT.Core.Encryption
{
    public enum CryptoAlgorithm
    {
        Disabled = 0,
        XOR,
        RC4,
        AES,
        Blowfish
    }
    public class CryptoProvider
    {
        public CryptoAlgorithm algorithm { get; private set; }
        private byte[] key;
        private BlowFish blowFish;

        public CryptoProvider(CryptoAlgorithm cryptoAlgorithm, string key)
        {
            this.algorithm = cryptoAlgorithm;
            this.key = Encoding.ASCII.GetBytes(SHA.Encode(key));

            if (cryptoAlgorithm == CryptoAlgorithm.Blowfish)
            {
                // Resize key for blowfish
                Array.Resize(ref this.key, 56);
                this.blowFish = new BlowFish(this.key);
                this.blowFish.SetRandomIV();
            }
        }

        public byte[] Encrypt(byte[] input)
        {
            switch (algorithm)
            {
                case CryptoAlgorithm.XOR:
                    XOR.Perform(ref input, key);
                    break;
                case CryptoAlgorithm.RC4:
                    RC4.Perform(ref input, key);
                    break;
                case CryptoAlgorithm.AES:
                    input = AES.Encrypt(input, key);
                    break;
                case CryptoAlgorithm.Blowfish:
                    input = blowFish.Encrypt_ECB(input);
                    break;
                default: // Invalid algorithm or non selected
                    break;
            }

            return input;
        }

        public byte[] Decrypt(byte[] input)
        {
            switch (algorithm)
            {
                case CryptoAlgorithm.XOR:
                    XOR.Perform(ref input, key);
                    break;
                case CryptoAlgorithm.RC4:
                    RC4.Perform(ref input, key);
                    break;
                case CryptoAlgorithm.AES:
                    input = AES.Decrypt(input, key);
                    break;
                case CryptoAlgorithm.Blowfish:
                    input = blowFish.Decrypt_ECB(input);
                    break;
                default: // Invalid algorithm or non selected
                    break;
            }

            return input;
        }
    }
}
