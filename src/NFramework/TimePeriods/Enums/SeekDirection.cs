using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 검색 방향
    /// </summary>
    [Serializable]
    public enum SeekDirection {
        /// <summary>
        /// 미래로 (시간 값을 증가 시키는 방향)
        /// </summary>
        Forward,

        /// <summary>
        /// 과거로 (시간 값을 감소 시키는 방향)
        /// </summary>
        Backward
    }
}