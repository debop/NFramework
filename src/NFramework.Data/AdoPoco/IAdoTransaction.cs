using System;

namespace NSoft.NFramework.Data.AdoPoco {
    /// <summary>
    /// Transaction 정보
    /// </summary>
    public interface IAdoTransaction : IDisposable {
        /// <summary>
        /// Transaction 을 완료합니다.
        /// </summary>
        void Complete();
    }
}