using System;

namespace NSoft.NFramework {

    /// <summary>
    /// Cache 시스템을 이용하여, 정보를 캐싱하도록 해주는 Client 의 인터페이스입니다.
    /// </summary>
    public interface ICacheRepository {

        /// <summary>
        /// 캐시 항목의 유효 기간
        /// </summary>
        TimeSpan Expiry { get; set; }

        /// <summary>
        /// 캐시에 저장할 객체에 대해 직렬화/역직화를 수행할 경우에 사용합니다.
        /// </summary>
        ISerializer Serializer { get; }

        /// <summary>
        /// 캐시에 저장된 항목을 반환합니다.
        /// </summary>
        /// <param name="key">캐시 키</param>
        /// <returns>저장된 항목, 없으면 null 반환</returns>
        object Get(string key);

        /// <summary>
        /// 캐시에 항목을 저장합니다.
        /// </summary>
        /// <param name="key">캐시 키</param>
        /// <param name="item">저장할 항목</param>
        /// <param name="validFor">유효 기간</param>
        void Set(string key, object item, TimeSpan validFor = default(TimeSpan));

        /// <summary>
        /// 캐시에서 항목을 제거합니다.
        /// </summary>
        /// <param name="key">캐시 키</param>
        void Remove(string key);

        /// <summary>
        /// 캐시의 모든 항목을 삭제합니다.
        /// </summary>
        void Clear();
    }
}