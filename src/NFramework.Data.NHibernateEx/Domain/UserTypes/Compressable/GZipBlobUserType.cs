using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes {
    /// <summary>
    /// 이진 데이타를 Gzip으로 압축하여 저장하는 UserType입니다.
    /// </summary>
    public sealed class GZipBlobUserType : AbstractCompressBlobUserType {
        private static readonly ICompressor _compressor = new GZipCompressor();

        /// <summary>
        /// 압축기
        /// </summary>
        protected override ICompressor Compressor {
            get { return _compressor; }
        }
    }
}