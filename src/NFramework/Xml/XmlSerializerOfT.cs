namespace NSoft.NFramework.Xml {
    /// <summary>
    /// 객체를 XML 형식으로 직렬화 / 역직렬화를 수행합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XmlSerializer<T> : ISerializer<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static XmlSerializer<T> Instance {
            get { return SingletonTool<XmlSerializer<T>>.Instance; }
        }

        /// <summary>
        /// 지정된 객체를 Serialize를 수행한다.
        /// </summary>
        /// <param name="graph">object to be serialized.</param>
        /// <returns>직렬화된 객체, graph가 null이면, 길이가 0인 byte[] 반환</returns>
        public byte[] Serialize(T graph) {
            byte[] data;

            if(XmlTool.Serialize(graph, out data))
                return data;

            return new byte[0];
        }

        /// <summary>
        /// Serialized 된 정보를 Deserialize 를 수행해서 객체로 반환한다.
        /// </summary>
        /// <param name="data">serialized data to be deserialized.</param>
        /// <returns>deserialized object</returns>
        public T Deserialize(byte[] data) {
            return XmlTool.Deserialize<T>(data);
        }
    }
}