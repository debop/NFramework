using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NSoft.NFramework.Serializations.Serializers {
    public class BinarySerializer<T> : AbstractSerializer<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// BinarySerializer{T} 의 Singleton Instance
        /// </summary>
        public static BinarySerializer<T> Instance {
            get { return SingletonTool<BinarySerializer<T>>.Instance; }
        }

        private readonly BinaryFormatter _formatter = new BinaryFormatter();

        /// <summary>
        /// 지정된 객체를 Serialize를 수행한다.
        /// </summary>
        /// <param name="graph">object to be serialized.</param>
        /// <returns>직렬화된 객체, graph가 null이면, 길이가 0인 byte[] 반환</returns>
        public override byte[] Serialize(T graph) {
            if(IsDebugEnabled)
                log.Debug("지정된 객체를 Binary 형식으로 Serialize를 수행합니다... graph=[{0}]", graph);

            if(ReferenceEquals(graph, null))
                return SerializerTool.EmptyBytes;

            using(var ms = new MemoryStream()) {
                SerializerTool.Serialize(graph, ms, _formatter);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Serialized 된 정보를 Deserialize 를 수행해서 객체로 반환한다.
        /// </summary>
        /// <param name="data">serialized data to be deserialized.</param>
        /// <returns>deserialized object</returns>
        public override T Deserialize(byte[] data) {
            if(IsDebugEnabled)
                log.Debug("Binary 형식으로 Serialized 된 정보를 Deserialize 를 수행해서 객체로 반환한다... data=[{0}]", data);

            if(data == null || data.Length == 0)
                return default(T);

            using(var ms = new MemoryStream(data)) {
                return SerializerTool.Deserialize<T>(ms, _formatter);
            }
        }
    }
}