namespace NSoft.NFramework.Cryptography {
    // TODO: rename to EncryptedTextFormat

    /// <summary>
    /// 암호화한 문자열로 변환할 때, 문자열 포맷 종류
    /// </summary>
    public enum EncryptionStringFormat {
        /// <summary>
        /// 16 진수 포맷
        /// </summary>
        HexDecimal = 0,

        /// <summary>
        /// Base64 인코딩 포맷
        /// </summary>
        Base64 = 1
    }
}