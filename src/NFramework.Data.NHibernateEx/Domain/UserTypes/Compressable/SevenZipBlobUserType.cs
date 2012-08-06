using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes {
    /// <summary>
    /// 7-Zip 압축 알고리즘으로 이진 데이타를 압축하여 DB에 저장할 수 있도록 하는 UserType입니다.
    /// </summary>
    public sealed class SevenZipBlobUserType : AbstractCompressBlobUserType {
        private static readonly ICompressor _compressor = new SevenZipCompressor();

        /// <summary>
        /// 압축기
        /// </summary>
        protected override ICompressor Compressor {
            get { return _compressor; }
        }
    }
}