using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes {
    /// <summary>
    /// GZip Compressor를 이용하여, 문자열을 압축하여 DB에 저장합니다.
    /// </summary>
    public sealed class GZipStringUserType : AbstractCompressStringUserType {
        private static readonly ICompressor _compressor = new GZipCompressor();

        /// <summary>
        /// 압축기
        /// </summary>
        protected override ICompressor Compressor {
            get { return _compressor; }
        }
    }
}