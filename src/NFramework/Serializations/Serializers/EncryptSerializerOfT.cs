using System;
using NSoft.NFramework.Cryptography;
using NSoft.NFramework.Cryptography.Encryptors;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Serializations.Serializers {
    /// <summary>
    /// Decorator 패턴을 이용하여, 직렬화 수행 후 암호화를 수행합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class EncryptSerializer<T> : AbstractSerializerDecorator<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public EncryptSerializer() {}
        public EncryptSerializer(ISerializer<T> serializer) : base(serializer) {}

        public EncryptSerializer(ISerializer<T> serializer, ISymmetricEncryptor encryptor)
            : base(serializer) {
            Encryptor = encryptor;
        }

        private ISymmetricEncryptor _encryptor;

        /// <summary>
        /// 암호기. 기본암호기는 <see cref="AriaSymmetricEncryptor"/> 입니다.
        /// </summary>
        public virtual ISymmetricEncryptor Encryptor {
            get { return _encryptor ?? (_encryptor = IoC.TryResolve<ISymmetricEncryptor, AriaSymmetricEncryptor>()); }
            set { _encryptor = value; }
        }

        /// <summary>
        /// 지정된 객체를 Serialize를 수행하고, 암호화를 합니다.
        /// </summary>
        /// <param name="graph">직렬화할 객체</param>
        /// <returns>직렬화된 객체 정보, graph가 null이면, 길이가 0인 byte[] 반환</returns>
        public override byte[] Serialize(T graph) {
            if(IsDebugEnabled)
                log.Debug("직렬화 후 암호화를 수행합니다... Encryptor=[{0}]", Encryptor);

            var result = base.Serialize(graph);
            return Encryptor.Encrypt(result);
        }

        /// <summary>
        /// 암호화된 Serialized 된 정보를 복호화하고, Deserialize 를 수행해서 객체로 반환한다.
        /// </summary>
        /// <param name="data">객체를 직렬화한 정보</param>
        /// <returns>역직렬화한 객체</returns>
        public override T Deserialize(byte[] data) {
            if(IsDebugEnabled)
                log.Debug("암호를 복호화 후 역직렬화를 수행합니다... Encryptor=[{0}]", Encryptor);

            var input = Encryptor.Decrypt(data);
            return base.Deserialize(input);
        }
    }
}