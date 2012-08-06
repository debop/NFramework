using System;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;

namespace NSoft.NFramework.Serializations.Serializers {
    /// <summary>
    /// Decorator 패턴을 이용하여, 직렬화 후 압축해주는 Decorator 입니다.
    /// </summary>
    /// <typeparam name="T">직렬화 대상 수형</typeparam>
    [Serializable]
    public class CompressSerializer<T> : AbstractSerializerDecorator<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        public CompressSerializer() : base() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="serializer"></param>
        public CompressSerializer(ISerializer<T> serializer) : base(serializer) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="serializer">실제 Serializer</param>
        /// <param name="compressor">압축기</param>
        public CompressSerializer(ISerializer<T> serializer, ICompressor compressor) : base(serializer) {
            Compressor = compressor ?? new GZipCompressor();
        }

        private ICompressor _compressor;

        /// <summary>
        /// 압축기 (기본은 <see cref="SharpBZip2Compressor"/>입니다.)
        /// </summary>
        public ICompressor Compressor {
            get { return _compressor ?? (_compressor = new SharpBZip2Compressor()); }
            set { _compressor = value; }
        }

        /// <summary>
        /// 지정된 객체를 Serialize를 수행한다.
        /// </summary>
        /// <param name="graph">직렬화할 객체</param>
        /// <returns>직렬화된 객체 정보, graph가 null이면, 길이가 0인 byte[] 반환</returns>
        public override byte[] Serialize(T graph) {
            if(IsDebugEnabled)
                log.Debug("객체를 직렬화 후 압축을 수행합니다... Compressor=[{0}]", Compressor);

            var result = base.Serialize(graph);
            return Compressor.Compress(result);
        }

        /// <summary>
        /// Serialized 된 정보를 Deserialize 를 수행해서 객체로 반환한다.
        /// </summary>
        /// <param name="data">객체를 직렬화한 정보</param>
        /// <returns>역직렬화한 객체</returns>
        public override T Deserialize(byte[] data) {
            if(IsDebugEnabled)
                log.Debug("객체의 압축을 푼 후 역직렬화를 수행합니다. Compressor=[{0}]", Compressor);

            var input = Compressor.Decompress(data);
            return base.Deserialize(input);
        }
    }
}