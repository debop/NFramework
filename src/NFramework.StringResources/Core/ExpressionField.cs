using System;
using System.Web.Compilation;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// Web Application에서 사용할 String Resource를 위한 <see cref="ResourceExpressionBuilder"/>의 Field를 표현하는 클래스
    /// </summary>
    [Serializable]
    public sealed class ExpressionField : IExpressionField {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Initialize a new instance of ExpressionField with class key, resource key
        /// </summary>
        /// <param name="classKey">리소스의 위치를 나타내는 키값</param>
        /// <param name="resourceKey">리소스 키</param>
        public ExpressionField(string classKey, string resourceKey) {
            classKey.ShouldNotBeWhiteSpace("classKey");
            resourceKey.ShouldNotBeWhiteSpace("resourceKey");

            if(IsDebugEnabled)
                log.Debug(@"ExpressionField 인스턴스를 생성합니다. classKey=[{0}], resourceKey=[{1}]", classKey, resourceKey);

            ClassKey = classKey;
            ResourceKey = resourceKey;
        }

        /// <summary>
        /// 리소스 위치를 나타내는 키값
        /// </summary>
        public string ClassKey { get; private set; }

        /// <summary>
        /// Resouce Value를 얻기 위한 키 값
        /// </summary>
        public string ResourceKey { get; private set; }

        /// <summary>
        /// 현재 개체가 동일한 형식의 다른 개체와 같은지 여부를 나타냅니다.
        /// </summary>
        /// <returns>
        /// 현재 개체가 <paramref name="other"/> 매개 변수와 같으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="other">이 개체와 비교할 개체입니다.</param>
        public bool Equals(IExpressionField other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        /// <summary>
        /// 지정한 <see cref="T:System.Object"/>가 현재 <see cref="T:System.Object"/>와 같은지 여부를 확인합니다.
        /// </summary>
        /// <returns>
        /// 지정된 <see cref="T:System.Object"/>가 현재 <see cref="T:System.Object"/>와 같으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="obj">현재 <see cref="T:System.Object"/>와 비교할 <see cref="T:System.Object"/>입니다. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj) {
            return (obj != null) && (obj is IExpressionField) && Equals((IExpressionField)obj);
        }

        /// <summary>
        /// 특정 형식에 대한 해시 함수 역할을 합니다. 
        /// </summary>
        /// <returns>
        /// 현재 <see cref="T:System.Object"/>의 해시 코드입니다.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode() {
            return HashTool.Compute(ClassKey, ResourceKey);
        }

        /// <summary>
        /// 현재 <see cref="T:System.Object"/>를 나타내는 <see cref="T:System.String"/>을 반환합니다.
        /// </summary>
        /// <returns>
        /// 현재 <see cref="T:System.Object"/>를 나타내는 <see cref="T:System.String"/>입니다.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() {
            return string.Format(@"ExpressionField# ClassKey=[{0}], ResourceKey=[{1}]", ClassKey, ResourceKey);
        }
    }
}