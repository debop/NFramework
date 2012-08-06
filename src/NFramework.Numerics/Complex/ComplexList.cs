using System.Collections.Generic;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// <see cref="Complex"/>를 요소로 가지는 리스트
    /// </summary>
    public class ComplexList : List<Complex> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        public ComplexList() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="capacity"></param>
        public ComplexList(int capacity) : base(capacity) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="collection"></param>
        public ComplexList(IEnumerable<Complex> collection) : base(collection) {}
    }
}