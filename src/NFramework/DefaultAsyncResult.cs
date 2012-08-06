using System;

namespace NSoft.NFramework {
    /// <summary>
    /// <see cref="IAsyncResult"/> 의 기본 구현체입니다. 비동기 방식의 작업에 대해, 취소, 실패, 성공을 지정할 수 있습니다.
    /// </summary>
    [Serializable]
    public class DefaultAsyncResult : AbstractAsyncResult {
        public DefaultAsyncResult() : base(null) {}
        public DefaultAsyncResult(object asyncState) : base(asyncState) {}
    }
}