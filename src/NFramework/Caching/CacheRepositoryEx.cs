using System;
using System.Threading.Tasks;

namespace NSoft.NFramework {
    public static class CacheRepositoryEx {
        /// <summary>
        /// 비동기 방식으로 캐시에 저장된 항목을 로드합니다.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Task<object> GetAsync(this ICacheRepository repository, string key) {
            key.ShouldNotBeWhiteSpace("key");
            return Task.Factory.StartNew(() => repository.Get(key));
        }

        /// <summary>
        /// 비동기 방식으로 캐시에 정보를 저장합니다.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="validFor"></param>
        /// <returns></returns>
        public static Task SetAsync(this ICacheRepository repository, string key, object value, TimeSpan validFor = default(TimeSpan)) {
            key.ShouldNotBeWhiteSpace("key");
            return Task.Factory.StartNew(() => repository.Set(key, value, validFor));
        }

        /// <summary>
        /// 비동기 방식으로 캐시에서 정보를 삭제합니다.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Task RemoveAsync(this ICacheRepository repository, string key) {
            return Task.Factory.StartNew(() => repository.Remove(key));
        }

        /// <summary>
        /// 비동기 방식으로 캐시에 저장된 모든 정보를 삭제합니다.
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static Task ClearAsync(this ICacheRepository repository) {
            return Task.Factory.StartNew(() => repository.Clear());
        }
    }
}