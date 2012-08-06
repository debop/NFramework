using System;

namespace NSoft.NFramework.Serializations.Serializers {
    /// <summary>
    /// Serializer의 추상 클래스
    /// </summary>
    public abstract class AbstractSerializer : ISerializer {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정된 객체를 Serialize를 수행한다.
        /// </summary>
        /// <param name="graph">직렬화할 객체</param>
        /// <returns>직렬화된 객체 정보, graph가 null이면, 길이가 0인 byte[] 반환</returns>
        /// <exception cref="InvalidOperationException"><paramref name="graph"/>의 형식이 byte[] 가 아닌 경우</exception>
        public virtual byte[] Serialize(object graph) {
            if(IsDebugEnabled)
                log.Debug("지정된 객체를 Serialize를 수행한다... graph=[{0}]", graph);

            if(graph != null)
                if(graph.GetType().IsAssignableFrom(typeof(byte[])) == false)
                    throw new InvalidOperationException("graph가 byte[] 형식이 아닙니다.");

            return (byte[])graph;
        }

        /// <summary>
        /// Serialized 된 정보를 Deserialize 를 수행해서 객체로 반환한다.
        /// </summary>
        /// <param name="data">객체를 직렬화한 정보</param>
        /// <returns>역직렬화한 객체</returns>
        public virtual object Deserialize(byte[] data) {
            if(IsDebugEnabled)
                log.Debug("Serialized 된 정보를 Deserialize 를 수행해서 객체로 반환한다... data=[{0}]", data);

            return data;
        }
    }
}