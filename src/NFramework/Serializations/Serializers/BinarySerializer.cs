using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NSoft.NFramework.Serializations.Serializers {
    /// <summary>
    /// object를 binary 형식으로 serialize를 수행한다.
    /// </summary>
    [Serializable]
    public class BinarySerializer : AbstractSerializer {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// BinarySerializer의 Singleton Instance
        /// </summary>
        public static BinarySerializer Instance {
            get { return SingletonTool<BinarySerializer>.Instance; }
        }

        private readonly BinaryFormatter _formatter = new BinaryFormatter();

        /// <summary>
        /// 지정된 객체를 Binary 형식으로 Serialize를 수행합니다.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns>직렬화된 객체, graph가 null이면, 길이가 0인 byte[] 반환</returns>
        public override byte[] Serialize(object graph) {
            if(IsDebugEnabled)
                log.Debug("지정된 객체를 Binary 형식으로 Serialize를 수행합니다... graph=[{0}]", graph);

            if(ReferenceEquals(graph, null)) {
                if(IsDebugEnabled)
                    log.Debug("직렬화 대상 객체가 null이라 new byte[0]을 반환합니다.");

                return new byte[0];
            }

            using(var ms = new MemoryStream()) {
                SerializerTool.Serialize(graph, ms, _formatter);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Binary 형식으로 Serialized 된 정보를 Deserialize 를 수행해서 객체로 반환한다.
        /// </summary>
        /// <param name="data">직렬화된 정보</param>
        /// <returns>역직렬화 객체</returns>
        public override object Deserialize(byte[] data) {
            if(IsDebugEnabled)
                log.Debug("Binary 형식으로 Serialized 된 정보를 Deserialize 를 수행해서 객체로 반환합니다...");

            if(data == null || data.Length == 0)
                return null;

            using(var ms = new MemoryStream(data)) {
                return SerializerTool.Deserialize<object>(ms, _formatter);
            }
        }
    }
}