using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 기간의 시작시각, 완료시각에 대해 영역의 포함여부를 조절할 수 있도록 Offset에 대한 처리를 제공합니다.
    /// </summary>
    public interface ITimePeriodMapper {
        /// <summary>
        /// <paramref name="moment"/>를 StartOffset 을 적용하여 매핑합니다.
        /// </summary>
        /// <param name="moment">대상 일자</param>
        /// <returns>매핑된 일자</returns>
        DateTime MapStart(DateTime moment);

        /// <summary>
        /// <paramref name="moment"/>를 EndOffset 을 적용하여 매핑합니다.
        /// </summary>
        /// <param name="moment">대상 일자</param>
        /// <returns>매핑된 일자</returns>
        DateTime MapEnd(DateTime moment);

        /// <summary>
        /// <paramref name="moment"/>를 StartOffset 적용을 해제합니다.
        /// </summary>
        /// <param name="moment">Offset이 적용된 일자</param>
        /// <returns>Offset 적용을 제거한 일자</returns>
        DateTime UnmapStart(DateTime moment);

        /// <summary>
        /// <paramref name="moment"/>를 EndOffset 적용을 해제합니다.
        /// </summary>
        /// <param name="moment">Offset이 적용된 일자</param>
        /// <returns>Offset 적용을 제거한 일자</returns>
        DateTime UnmapEnd(DateTime moment);
    }
}