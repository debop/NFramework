using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// 엔티티의 변화 시간을 추적할 수 있도록 UpdateTimeStamp 속성을 가지는 엔티티를 나타냅니다.
    /// </summary>
    public interface IUpdateTimestampedEntity {
        /// <summary>
        /// 엔티티가 변경된 최종 시각
        /// </summary>
        DateTime? UpdateTimestamp { get; set; }
    }
}