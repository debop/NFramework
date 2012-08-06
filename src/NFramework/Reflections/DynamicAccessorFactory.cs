using System;
using System.Collections.Concurrent;

namespace NSoft.NFramework.Reflections {
    /// <summary>
    /// Factory for <see cref="IDynamicAccessor"/>. <br/>
    /// DynamicAccessorFactory is thread-safe
    /// </summary>
    public static class DynamicAccessorFactory {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly object _syncLock = new object();

        /// <summary>
        /// Cache for IDynamicAccessor
        /// </summary>
        private static readonly ConcurrentDictionary<DynamicAccessorKey, IDynamicAccessor> _accessors =
            new ConcurrentDictionary<DynamicAccessorKey, IDynamicAccessor>();

        /// <summary>
        /// Cache for IDynamicAccessor{T}
        /// </summary>
        private static readonly ConcurrentDictionary<DynamicAccessorKey, object> _genericAccessors =
            new ConcurrentDictionary<DynamicAccessorKey, object>();

        /// <summary>
        /// create <see cref="IDynamicAccessor"/> for accessing specified instance.
        /// </summary>
        /// <param name="type">속성/필드값을 얻고자하는 인스턴스의 형식</param>
        /// <param name="suppressError">예외 발생시 무시할 것인가 여부</param>
        /// <param name="ignoreCase">속성명, 필드명의 대소문자를 구분할 것인가?</param>
        /// <returns>생성된 Dynamic Accessor 인스턴스</returns>
        public static IDynamicAccessor CreateDynamicAccessor(Type type, bool suppressError = false, bool ignoreCase = false) {
            return CreateDynamicAccessor(type, new MapPropertyOptions { SuppressException = suppressError, IgnoreCase = ignoreCase });
        }

        /// <summary>
        /// 지정한 수형의 속성/필드 정보를 동적으로 접근하고 설정할 수 있는 <see cref="IDynamicAccessor"/>를 빌드합니다.
        /// </summary>
        /// <param name="type">대상 객체의 수형</param>
        /// <param name="mapOption">속성명/필드명 매핑 옵션</param>
        /// <returns></returns>
        public static IDynamicAccessor CreateDynamicAccessor(Type type, MapPropertyOptions mapOption) {
            type.ShouldNotBeNull("type");

            var key = new DynamicAccessorKey(type, mapOption);

            return _accessors.GetOrAdd(key,
                                       (k) =>
                                       new TypeConvertableDynamicAccessor(k.TargetType, mapOption.SuppressException,
                                                                          mapOption.IgnoreCase));
        }

        /// <summary>
        /// create <see cref="IDynamicAccessor"/> for accessing specified instance.
        /// </summary>
        /// <param name="type">속성/필드값을 얻고자하는 인스턴스의 형식</param>
        /// <param name="accessorFactory">IDynamicAccessor 인스턴스 생성 함수</param>
        /// <returns>생성된 Dynamic Accessor 인스턴스</returns>
        public static IDynamicAccessor CreateDynamicAccessor(Type type, Func<Type, IDynamicAccessor> accessorFactory) {
            return accessorFactory(type);
        }

        /// <summary>
        /// Create Instance of <see cref="IDynamicAccessor{T}"/>
        /// </summary>
        /// <typeparam name="T">속성/필드값을 얻고자하는 인스턴스의 형식</typeparam>
        /// <param name="suppressError">indicate to supppress to raise exception when access to target object.</param>
        /// <param name="ignoreCase">속성명, 필드명의 대소문자를 구분할 것인가?</param>
        /// <returns></returns>
        public static IDynamicAccessor<T> CreateDynamicAccessor<T>(bool suppressError = false, bool ignoreCase = false) {
            return CreateDynamicAccessor<T>(new MapPropertyOptions { SuppressException = suppressError, IgnoreCase = ignoreCase });
        }

        /// <summary>
        /// 지정한 수형의 속성/필드 정보를 동적으로 접근하고 설정할 수 있는 <see cref="IDynamicAccessor{T}"/>를 빌드합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="mapOption">속성명/필드명 매핑 옵션</param>
        /// <returns></returns>
        public static IDynamicAccessor<T> CreateDynamicAccessor<T>(MapPropertyOptions mapOption) {
            var targetType = typeof(T);

            var key = new DynamicAccessorKey(targetType, mapOption);

            return
                (IDynamicAccessor<T>)_genericAccessors
                                         .GetOrAdd(key,
                                                   (k) => new TypeConvertableDynamicAccessor<T>(mapOption.SuppressException,
                                                                                                mapOption.IgnoreCase));
        }

        /// <summary>
        /// create <see cref="IDynamicAccessor{T}"/> for accessing specified instance.
        /// </summary>
        /// <typeparam name="T">속성/필드값을 얻고자하는 인스턴스의 형식</typeparam>
        /// <param name="accessorFactory">IDynamicAccessor 인스턴스 생성 함수</param>
        /// <returns>생성된 Dynamic Accessor 인스턴스</returns>
        public static IDynamicAccessor<T> CreateDynamicAccessor<T>(Func<IDynamicAccessor<T>> accessorFactory) {
            return accessorFactory();
        }

        /// <summary>
        /// <see cref="IDynamicAccessor"/> 캐시를 메모리에서 제거한다.
        /// </summary>
        public static void ResetCache() {
            lock(_syncLock) {
                _accessors.Clear();
                _genericAccessors.Clear();
            }

            if(IsDebugEnabled)
                log.Debug("DynamicAccessor 캐시를 비웠습니다.");
        }

        /// <summary>
        /// 옵션에 따라 IDynamicAccessor를 여러개를 만들어야 하므로 캐시 키로 옵션 정보도 포함해야 합니다.
        /// </summary>
        internal class DynamicAccessorKey : IEquatable<DynamicAccessorKey> {
            internal DynamicAccessorKey(Type targetType, MapPropertyOptions mapOption) {
                targetType.ShouldNotBeNull("targetType");

                TargetType = targetType;
                MapOption = mapOption ?? MapPropertyOptions.Default;
            }

            internal Type TargetType { get; private set; }

            internal MapPropertyOptions MapOption { get; private set; }

            public bool Equals(DynamicAccessorKey other) {
                return (other != null) && GetHashCode().Equals(other.GetHashCode());
            }

            public override bool Equals(object obj) {
                return (obj != null) && (obj is DynamicAccessorKey) && Equals((DynamicAccessorKey)obj);
            }

            public override int GetHashCode() {
                return HashTool.Compute(TargetType, MapOption);
            }

            public override string ToString() {
                return string.Format("DynamicAccessorKey# TargetType=[{0}], MapOption=[{1}]", TargetType, MapOption);
            }
        }
    }
}