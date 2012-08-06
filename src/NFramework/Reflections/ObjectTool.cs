using System;

namespace NSoft.NFramework.Reflections {
    /// <summary>
    /// Object를 위한 Extension Methods
    /// </summary>
    public static class ObjectExtensions {
        /// <summary>
        /// 지정된 수형의 인스턴스의 필드와 속성 정보를 동적으로 접근할 수 있는 <see cref="IDynamicAccessor"/>를 제공해줍니다.
        /// </summary>
        /// <param name="instanceType"></param>
        /// <returns></returns>
        public static IDynamicAccessor GetDynamicAccessor(this Type instanceType) {
            instanceType.ShouldNotBeNull("instanceType");

            return DynamicAccessorFactory.CreateDynamicAccessor(instanceType, false);
        }

        /// <summary>
        /// 지정된 인스턴스에 대해 필드, 속성 값을 조작할 수 있는 <see cref="IDynamicAccessor"/>를 생성합니다.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IDynamicAccessor GetDynamicAccessor(this object instance) {
            instance.ShouldNotBeNull("instance");
            return DynamicAccessorFactory.CreateDynamicAccessor(instance.GetType());
        }

        /// <summary>
        /// 인스턴스의 속성명에 해당하는 값을 반환한다.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetProperty(this object instance, string propertyName) {
            instance.ShouldNotBeNull("instance");
            propertyName.ShouldNotBeWhiteSpace("propertyName");

            return
                instance
                    .GetDynamicAccessor()
                    .GetPropertyValue(instance, propertyName);
        }

        /// <summary>
        /// 인스턴스의 속성명에 해당하는 값을 반환한다.
        /// </summary>
        /// <typeparam name="T">Type of property value</typeparam>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static T GetProperty<T>(this object instance, string propertyName) {
            instance.ShouldNotBeNull("instance");
            propertyName.ShouldNotBeWhiteSpace("propertyName");

            return
                (T)instance
                       .GetDynamicAccessor()
                       .GetPropertyValue(instance, propertyName);
        }

        /// <summary>
        /// 인스턴스의 속성에 지정된 속성 값을 설정합니다.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public static void SetProperty(this object instance, string propertyName, object propertyValue) {
            instance.ShouldNotBeNull("instance");
            propertyName.ShouldNotBeWhiteSpace("propertyName");

            instance
                .GetDynamicAccessor()
                .SetPropertyValue(instance, propertyName, propertyValue);
        }

        /// <summary>
        /// 인스턴스의 속성에 지정된 속성 값을 설정합니다.
        /// </summary>
        /// <typeparam name="T">type of property value</typeparam>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public static void SetProperty<T>(this object instance, string propertyName, T propertyValue) {
            instance.ShouldNotBeNull("instance");
            propertyName.ShouldNotBeWhiteSpace("propertyName");

            instance
                .GetDynamicAccessor()
                .SetPropertyValue(instance, propertyName, propertyValue);
        }

        /// <summary>
        /// 인스턴스의 필드 값을 반환합니다.
        /// </summary>
        /// <param name="instance">필드 값을 가져올 인스턴스</param>
        /// <param name="fieldName">값을 조회할 필드 명</param>
        /// <returns>인스턴스의 필드 값</returns>
        public static object GetField(this object instance, string fieldName) {
            instance.ShouldNotBeNull("instance");
            fieldName.ShouldNotBeWhiteSpace("fieldName");

            return
                instance
                    .GetDynamicAccessor()
                    .GetFieldValue(instance, fieldName);
        }

        /// <summary>
        /// 인스턴스의 필드 값을 반환합니다.
        /// </summary>
        /// <param name="instance">필드 값을 가져올 인스턴스</param>
        /// <param name="fieldName">값을 조회할 필드 명</param>
        /// <returns>인스턴스의 필드 값</returns>
        public static T GetField<T>(this object instance, string fieldName) {
            instance.ShouldNotBeNull("instance");
            fieldName.ShouldNotBeWhiteSpace("fieldName");

            return
                (T)instance
                       .GetDynamicAccessor()
                       .GetFieldValue(instance, fieldName);
        }

        /// <summary>
        /// 인스턴스의 필드에 값을 지정합니다.
        /// </summary>
        /// <param name="instance">필드 값을 설정할 인스턴스</param>
        /// <param name="fieldName">값을 조회할 필드 명</param>
        /// <param name="fieldValue">필드 값</param>
        public static void SetField(this object instance, string fieldName, object fieldValue) {
            instance.ShouldNotBeNull("instance");
            fieldName.ShouldNotBeWhiteSpace("fieldName");

            instance
                .GetDynamicAccessor()
                .SetFieldValue(instance, fieldName, fieldValue);
        }

        /// <summary>
        /// 인스턴스의 필드에 값을 지정합니다.
        /// </summary>
        /// <param name="instance">필드 값을 설정할 인스턴스</param>
        /// <param name="fieldName">값을 조회할 필드 명</param>
        /// <param name="fieldValue">필드 값</param>
        public static void SetField<T>(this object instance, string fieldName, T fieldValue) {
            instance.ShouldNotBeNull("instance");
            fieldName.ShouldNotBeWhiteSpace("fieldName");

            instance
                .GetDynamicAccessor()
                .SetFieldValue(instance, fieldName, fieldValue);
        }
    }
}