using System;
using System.Reflection;

namespace NSoft.NFramework.Reflections {
    public static partial class FasterflectTool {
#if !SIVLERLIGHT
        /// <summary>
        /// 특정 형식의 기본 생성자에 델리게이트를 생성해줍니다.
        /// </summary>
        /// <param name="targetType">생성하고자 하는 수형</param>
        /// <returns></returns>
        public static Fasterflect.ConstructorInvoker GetConstructorInvoker(this Type targetType) {
            return Fasterflect.ConstructorExtensions.DelegateForCreateInstance(targetType);
        }

        /// <summary>
        /// 특정 형식의 기본 생성자에 델리게이트를 생성해줍니다.
        /// </summary>
        /// <param name="targetType">생성하고자 하는 수형</param>
        /// <param name="parameterTypes">생성자 메소드의 Parameter의 수형들</param>
        /// <returns></returns>
        public static Fasterflect.ConstructorInvoker GetConstructorInvoker(this Type targetType, params Type[] parameterTypes) {
            return Fasterflect.ConstructorExtensions.DelegateForCreateInstance(targetType, parameterTypes);
        }

        /// <summary>
        /// 특정 형식의 기본 생성자에 델리게이트를 생성해줍니다.
        /// </summary>
        /// <param name="targetType">생성하고자 하는 수형</param>
        /// <param name="bindingFlags">BindingFlags</param>
        /// <param name="parameterTypes">생성자 메소드의 Parameter의 수형들</param>
        /// <returns></returns>
        public static Fasterflect.ConstructorInvoker GetConstructorInvoker(this Type targetType, BindingFlags bindingFlags,
                                                                           params Type[] parameterTypes) {
            return Fasterflect.ConstructorExtensions.DelegateForCreateInstance(targetType, bindingFlags, parameterTypes);
        }
#endif
    }
}