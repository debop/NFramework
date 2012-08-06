using System;

namespace NSoft.NFramework {
    /// <summary>
    /// 하나의 ThreadContext 내에 정보를 저장합니다.
    /// </summary>
    public interface ILocalData {
        /// <summary>
        /// Get or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <param name="key">키 값</param>
        /// <returns>저장된 값을 반환합니다. 저장된 값이 없다면 null을 반환합니다.</returns>
        object this[object key] { get; set; }

        /// <summary>
        /// 지정된 키에 해당하는 값을 조회합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetValue(object key, out object value);

        /// <summary>
        /// 저장소의 모든 정보를 삭제합니다.
        /// </summary>
        void Clear();

        /// <summary>
        /// 저장된 값을 반환합니다. 저장된 값이 없다면, <paramref name="valueFactory"/>를 이용하여 생성한 값을 저장소에 저장한 후 저장한 값을 반환한다.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        TValue GetOrAdd<TValue>(object key, Func<TValue> valueFactory);

        /// <summary>
        /// 지정된 키에 이용하여 생성한 값을 저장소에 저장한 후 저장한 값을 반환한다.
        /// </summary>
        /// <param name="key">키 값</param>
        /// <param name="valuePropertySetter">Value 속성 값을 변경할 Action</param>
        /// <returns></returns>
        TValue SetValue<TValue>(object key, Action<TValue> valuePropertySetter);

        /// <summary>
        /// 지정된 키에 <paramref name="valueFactory"/>를 이용하여 생성한 값을 저장소에 저장한 후 저장한 값을 반환한다.
        /// </summary>
        /// <param name="key">키 값</param>
        /// <param name="valueFactory">해당 정보가 없을 때 사용할 Value 생성자</param>
        /// <param name="valuePropertySetter">Value 속성 값을 변경할 Action</param>
        /// <returns></returns>
        TValue SetValue<TValue>(object key, Func<TValue> valueFactory, Action<TValue> valuePropertySetter);
    }
}