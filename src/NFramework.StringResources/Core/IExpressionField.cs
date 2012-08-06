using System;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// Expression Builder에서 해석할 Expression 요소 정보입니다.
    /// </summary>
    public interface IExpressionField : IEquatable<IExpressionField> {
        /// <summary>
        /// 리소스 위치를 나타내는 키값
        /// </summary>
        string ClassKey { get; }

        /// <summary>
        /// Resouce Value를 얻기 위한 키 값
        /// </summary>
        string ResourceKey { get; }
    }
}