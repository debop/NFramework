using System;

namespace NSoft.NFramework.Cryptography.Aria {
    /// <summary>
    /// ANSIX923 패딩
    /// </summary>
    [Serializable]
    internal sealed class AnsiX923Padding : BlockPadding {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const byte PaddingValue = 0x00;

        /// <summary>
        /// 현 인스턴스의 패딩 타입
        /// </summary>
        public override string PaddingType {
            get { return ANSIX; }
        }

        /// <summary>
        /// <paramref name="blockSize"/> 만큼 <paramref name="source"/>에 padding을 추가합니다.
        /// </summary>
        /// <param name="source">암호화를 위한 대상</param>
        /// <param name="blockSize">블록 크기</param>
        /// <returns>암호화 대상에 패딩 정보가 포함된 정보</returns>
        public override byte[] AddPadding(byte[] source, int blockSize) {
            source.ShouldNotBeNull("source");
            blockSize.ShouldBePositive("blockSize");

            var paddingCount = blockSize - (source.Length % blockSize);

            if(IsDebugEnabled)
                log.Debug("대상 정보에 Padding을 추가합니다... blockSize=[{0}], paddingCount=[{1}]", blockSize, paddingCount);

            if(paddingCount == 0 || paddingCount == blockSize)
                return source;

            var buffer = new byte[source.Length + paddingCount];

            Buffer.BlockCopy(source, 0, buffer, 0, source.Length);

            //for(var i = 0; i < paddingCount - 1; i++)
            //    buffer[source.Length + i] = PaddingValue;

            return buffer;
        }

        /// <summary>
        /// <paramref name="blockSize"/> 만큼 <paramref name="source"/>에 padding을 제거합니다.
        /// </summary>
        /// <param name="source">암호화를 위한 대상</param>
        /// <param name="blockSize">블록 크기</param>
        /// <returns>암호화 대상에 패딩 정보가 제거된 정보</returns>
        public override byte[] RemovePadding(byte[] source, int blockSize) {
            source.ShouldNotBeNull("source");
            blockSize.ShouldBePositive("blockSize");

            var paddingCount = source[source.Length - 1];

            if(IsDebugEnabled)
                log.Debug("대상 정보에 Padding을 제거합니다... blockSize=[{0}], paddingCount=[{1}]", blockSize, paddingCount);

            if(paddingCount >= (blockSize - 1))
                return source;

            var zeroPaddingCount = paddingCount - 1;

            for(var i = 2; i < (zeroPaddingCount + 2); i++) {
                if(source[source.Length - i] != PaddingValue)
                    return source;
            }

            if(source.Length % blockSize == 0)
                if(paddingCount < 0)
                    return source;

            var buffer = new byte[source.Length - paddingCount];
            Buffer.BlockCopy(source, 0, buffer, 0, buffer.Length);

            return buffer;
        }
    }
}