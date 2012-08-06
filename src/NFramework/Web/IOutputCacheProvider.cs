using System;

namespace NSoft.NFramework.Web {
    public interface IOutputCacheProvider {
        /// <summary>
        /// 실제 캐시 저장소에 데이타를 저장/조회하는 API를 제공하는 Repository입니다.
        /// </summary>
        ICacheRepository CacheRepository { get; }

        /// <summary>
        /// 출력 캐시에서 지정된 항목에 대한 참조를 반환합니다.
        /// </summary>
        /// <returns>
        /// 캐시에서 지정된 항목을 식별하는 <paramref name="key"/> 값이거나 캐시에 지정된 항목이 없는 경우 null입니다.
        /// </returns>
        /// <param name="key">출력 캐시에서 캐시된 항목에 대한 고유 식별자입니다. </param>
        object Get(string key);

        /// <summary>
        /// 지정된 항목을 출력 캐시에 삽입합니다. 
        /// </summary>
        /// <returns>
        /// 지정된 공급자에 대한 참조입니다. 
        /// </returns>
        /// <param name="key"><paramref name="entry"/>에 대한 고유 식별자입니다.</param><param name="entry">출력 캐시에 추가할 내용입니다.</param>
        /// <param name="utcExpiry">캐시된 항목이 만료되는 시각입니다.</param>
        object Add(string key, object entry, DateTime utcExpiry);

        /// <summary>
        /// 지정된 항목을 출력 캐시에 삽입하고 이미 캐시되어 있는 경우 해당 항목을 덮어씁니다.
        /// </summary>
        /// <param name="key"><paramref name="entry"/>에 대한 고유 식별자입니다.</param><param name="entry">출력 캐시에 추가할 내용입니다.</param>
        /// <param name="utcExpiry">캐시된 <paramref name="entry"/>가 만료되는 시각입니다.</param>
        void Set(string key, object entry, DateTime utcExpiry);

        /// <summary>
        /// 출력 캐시에서 지정된 항목을 제거합니다.
        /// </summary>
        /// <param name="key">출력 캐시에서 제거할 항목에 대한 고유 식별자입니다. </param>
        void Remove(string key);
    }
}