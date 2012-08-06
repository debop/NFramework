using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes {
    /// <summary>
    /// Deflate 압축 알고리즘을 이용하여, 문자열을 압축하여 DB에 저장하는 UserType 입니다.
    /// </summary>
    /// <seealso cref="DeflateCompressor"/>
    /// <seealso cref="GZipStringUserType"/>
    public sealed class DeflateStringUserType : AbstractCompressStringUserType {
        private static readonly ICompressor _compressor = new DeflateCompressor();

        /// <summary>
        /// 압축기
        /// </summary>
        protected override ICompressor Compressor {
            get { return _compressor; }
        }
    }
}