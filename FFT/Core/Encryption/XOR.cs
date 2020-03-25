namespace FFT.Core.Encryption
{
    class XOR
    {
        public static void Perform(ref byte[] input, byte[] key)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = (byte)(input[i] ^ key[i % key.Length]);
            }
        }
    }
}
