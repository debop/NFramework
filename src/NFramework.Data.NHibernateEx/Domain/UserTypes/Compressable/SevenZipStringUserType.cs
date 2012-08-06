using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes {
    /// <summary>
    /// 7-Zip 압축 알고리즘을 이용하여, 문자열을 압축하여 저장할 수 있도록 하는 UserType 입니다.
    /// </summary>
    public sealed class SevenZipStringUserType : AbstractCompressStringUserType {
        private static readonly ICompressor _compressor = new SevenZipCompressor();

        /// <summary>
        /// 압축기
        /// </summary>
        protected override ICompressor Compressor {
            get { return _compressor; }
        }
    }
}