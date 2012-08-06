using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes {
    /// <summary>
    /// Blob 데이타를 <see cref="DeflateCompressor"/>를 통해 압축하여 저장할 수 있도록 하는 UserType입니다.
    /// </summary>
    /// <seealso cref="DeflateCompressor"/>
    /// <seealso cref="GZipBlobUserType"/>
    public sealed class DeflateBlobUserType : AbstractCompressBlobUserType {
        private static readonly ICompressor _compressor = new DeflateCompressor();

        /// <summary>
        /// 압축기
        /// </summary>
        protected override ICompressor Compressor {
            get { return _compressor; }
        }
    }
}