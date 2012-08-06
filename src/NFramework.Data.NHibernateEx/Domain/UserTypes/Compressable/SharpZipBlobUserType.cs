using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes {
    /// <summary>
    /// 이진 데이타를 SharpGZip 으로 압축하여 저장하는 UserType입니다.
    /// </summary>
    public sealed class SharpZipBlobUserType : AbstractCompressBlobUserType {
        private static readonly ICompressor _compressor = new SharpGZipCompressor();

        /// <summary>
        /// 압축기
        /// </summary>
        protected override ICompressor Compressor {
            get { return _compressor; }
        }
    }
}