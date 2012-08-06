using System;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;

namespace NSoft.NFramework.Serializations.Serializers {
    /// <summary>
    /// Decorator 패턴을 이용하여, 직렬화 후 압축해주는 Decorator 입니다.
    /// </summary>
    [Serializable]
    public class CompressSerializer : AbstractSerializer {
        public CompressSerializer(ISerializer serializer) : this(serializer, null) {}

        public CompressSerializer(ISerializer serializer, ICompressor compressor) {
            serializer.ShouldNotBeNull("serializer");

            Serializer = serializer;
            Compressor = compressor ?? new GZipCompressor();
        }

        /// <summary>
        /// 실제 Serializer
        /// </summary>
        public ISerializer Serializer { get; protected set; }

        /// <summary>
        /// Compressor
        /// </summary>
        public ICompressor Compressor { get; protected set; }

        /// <summary>
        /// 지정된 객체를 Serialize를 수행한다.
        /// </summary>
        /// <param name="graph">object to be serialized.</param>
        /// <returns>직렬화된 객체, graph가 null이면, 길이가 0인 byte[] 반환</returns>
        public override byte[] Serialize(object graph) {
            return Compressor.Compress(Serializer.Serialize(graph));
        }

        /// <summary>
        /// Serialized 된 정보를 Deserialize 를 수행해서 객체로 반환한다.
        /// </summary>
        /// <param name="data">serialized data to be deserialized.</param>
        /// <returns>deserialized object</returns>
        public override object Deserialize(byte[] data) {
            return Serializer.Deserialize(Compressor.Decompress(data));
        }
    }
}