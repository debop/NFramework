namespace NSoft.NFramework.Cryptography {
    /// <summary>
    /// Symmetric cryptography algorithm에 사용되는 vector 값
    /// </summary>
    internal static class InitVector {
        internal static readonly byte[] IV_8 = new byte[] { 2, 63, 9, 36, 235, 174, 78, 12 };

        internal static readonly byte[] IV_16 = new byte[]
                                                {
                                                    15, 199, 56, 77, 244, 126, 107, 239,
                                                    9, 10, 88, 72, 24, 202, 31, 108
                                                };

        internal static readonly byte[] IV_24 = new byte[]
                                                {
                                                    37, 28, 19, 44, 25, 170, 122, 25,
                                                    25, 57, 127, 5, 22, 1, 66, 65,
                                                    14, 155, 224, 64, 9, 77, 18, 251
                                                };

        internal static readonly byte[] IV_32 = new byte[]
                                                {
                                                    133, 206, 56, 64, 110, 158, 132, 22,
                                                    99, 190, 35, 129, 101, 49, 204, 248,
                                                    251, 243, 13, 194, 160, 195, 89, 152,
                                                    149, 227, 245, 5, 218, 86, 161, 124
                                                };

        // Salt value used to encrypt a plain text key. Again, this can be whatever you like
        internal static readonly byte[] SALT_BYTES = new byte[] { 162, 27, 98, 1, 28, 239, 64, 30, 156, 102, 223 };
    }
}