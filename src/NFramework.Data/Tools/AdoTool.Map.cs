using System;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// ADO.NET 객체(DataReader,DataTable)을 Persistent Object로 Mapping (변환)을 지원하는 Extension 메소드들입니다.
    /// </summary>
    public static partial class AdoTool {
        /// <summary>
        /// 수형 {T} 의 인스턴스의 속성 및 필드에 동적으로 접근할 수 있도록 해주는 <see cref="IDynamicAccessor{T}"/>를 생성합니다.
        /// </summary>
        /// <typeparam name="T">대상 수형</typeparam>
        /// <param name="mapOptions">매핑 옵션</param>
        /// <seealso cref="TypeConvertableDynamicAccessor{T}"/>
        internal static IDynamicAccessor<T> GetDynamicAccessor<T>(MapPropertyOptions mapOptions = null) {
            return DynamicAccessorFactory.CreateDynamicAccessor<T>(mapOptions ?? MapPropertyOptions.Safety);
        }

        /// <summary>
        /// 지정된 수형의 속성, 필드를 동적으로 접근할 수 있도록 해주는 <see cref="IDynamicAccessor"/>를 생성합니다.
        /// </summary>
        /// <param name="type">대상 수형</param>
        /// <param name="mapOptions">매핑 옵션</param>
        internal static IDynamicAccessor GetDynamicAccessor(Type type, MapPropertyOptions mapOptions = null) {
            return DynamicAccessorFactory.CreateDynamicAccessor(type, mapOptions ?? MapPropertyOptions.Safety);
        }
    }
}