using System;

namespace NSoft.NFramework.Caching.SharedCache {
    /// <summary>
    /// 캐시에 저장할 작업 정보
    /// </summary>
    [Serializable]
    public class TaskCacheItem : CacheItemBase<Guid> {
        public TaskCacheItem() : base(Guid.NewGuid()) {
            IsDone = false;
        }

        /// <summary>
        /// 작업 정보 요약
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 완료 여부. 기본은 False
        /// </summary>
        public bool? IsDone { get; set; }

        public byte[] Data { get; set; }

        /// <summary>
        /// 작업정보를 속성값이 제대로 설정되었는지 검사합니다.
        /// </summary>
        public virtual void Validate() {
            Summary.ShouldNotBeWhiteSpace("Summary");
        }
    }
}