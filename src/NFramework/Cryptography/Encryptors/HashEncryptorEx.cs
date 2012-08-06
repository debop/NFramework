using System.Threading.Tasks;

namespace NSoft.NFramework.Cryptography.Encryptors {
    public static class HashEncryptorEx {
        public static string ComputeHashToText(this IHashEncryptor encryptor, string text,
                                               EncryptionStringFormat format = EncryptionStringFormat.HexDecimal) {
            return encryptor.ComputeHash(text, format);
        }

        public static Task<byte[]> ComputeHashAsync(this IHashEncryptor encryptor, string text) {
            return Task.Factory.StartNew(() => encryptor.ComputeHash(text));
        }

        public static Task<string> ComputeHashToTextAsync(this IHashEncryptor encryptor, string text,
                                                          EncryptionStringFormat format = EncryptionStringFormat.HexDecimal) {
            return Task.Factory.StartNew(() => encryptor.ComputeHash(text, format));
        }
    }
}