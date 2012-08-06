using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes {
    /// <summary>
    /// SharpBZip2 Compressor를 이용하여, 문자열을 압축하여 DB에 저장합니다.
    /// </summary>
    public sealed class SharpBZip2StringUserType : AbstractCompressStringUserType {
        private static readonly ICompressor _compressor = new SharpBZip2Compressor();

        protected override ICompressor Compressor {
            get { return _compressor; }
        }
    }
}