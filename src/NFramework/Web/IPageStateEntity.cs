using System;

namespace NSoft.NFramework.Web {
    /// <summary>
    /// Web Page 상태 정보를 Persister가 저장하고 로드하는 엔티티의 인터페이스입니다.
    /// </summary>
    public interface IPageStateEntity : IEquatable<IPageStateEntity> {
        /// <summary>
        /// 엔티티의 Identifier
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// 압축 여부
        /// </summary>
        bool IsCompressed { get; set; }

        /// <summary>
        /// Page 상태 정보의 Snapshot
        /// </summary>
        byte[] Value { get; set; }

        /// <summary>
        /// 생성 일자
        /// </summary>
        DateTime CreatedDate { get; set; }
    }
}