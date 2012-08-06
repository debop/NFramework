using System.Collections.Generic;

namespace NSoft.NFramework {
    /// <summary>
    /// Paging된 List의 정보
    /// </summary>
    /// <typeparam name="T">Type of persistent object</typeparam>
    public interface IPagingList<T> : IList<T>, IPagingInfo {}
}