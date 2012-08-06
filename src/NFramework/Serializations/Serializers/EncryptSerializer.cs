using System;
using NSoft.NFramework.Cryptography;
using NSoft.NFramework.Cryptography.Encryptors;

namespace NSoft.NFramework.Serializations.Serializers {
    [Serializable]
    public class EncryptSerializer : AbstractSerializer {
        public EncryptSerializer(ISerializer serializer) : this(serializer, null) {}

        public EncryptSerializer(ISerializer serializer, ISymmetricEncryptor encryptor) {
            serializer.ShouldNotBeNull("serializer");

            Serializer = serializer;
            Encryptor = encryptor ?? new RC2SymmetricEncryptor();
        }

        /// <summary>
        /// 실제 Serializer
        /// </summary>
        public ISerializer Serializer { get; protected set; }

        /// <summary>
        /// 대칭형 암호화 기
        /// </summary>
        public ISymmetricEncryptor Encryptor { get; protected set; }

        /// <summary>
        /// 지정된 객체를 Serialize를 수행한다.
        /// </summary>
        /// <param name="graph">직렬화할 객체</param>
        /// <returns>직렬화된 객체 정보, graph가 null이면, 길이가 0인 byte[] 반환</returns>
        /// <exception cref="InvalidOperationException"><paramref name="graph"/>의 형식이 byte[] 가 아닌 경우</exception>
        public override byte[] Serialize(object graph) {
            return Encryptor.Encrypt(Serializer.Serialize(graph));
        }

        /// <summary>
        /// Serialized 된 정보를 Deserialize 를 수행해서 객체로 반환한다.
        /// </summary>
        /// <param name="data">객체를 직렬화한 정보</param>
        /// <returns>역직렬화한 객체</returns>
        public override object Deserialize(byte[] data) {
            return Serializer.Deserialize(Encryptor.Decrypt(data));
        }
    }
}