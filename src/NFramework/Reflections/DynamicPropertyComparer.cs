using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace NSoft.NFramework.Reflections {
    /// <summary>
    /// 지정된 형식의 특정 속성을 비교하는 <see cref="IComparer{T}"/>을 만든다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class DynamicPropertyComparer<T> : IComparer<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private delegate int CompareHandler(T x, T y);

        private readonly CompareHandler _compare;
        private readonly Type _concreteType = typeof(T);

        /// <summary>
        /// 기본 생성자
        /// </summary>
        /// <param name="propertyName">비교할 속성 또는 필드명</param>
        public DynamicPropertyComparer(string propertyName) {
            _compare = CreateCompareHandler(propertyName);
        }

        /// <summary>
        /// 속성 또는 필드 값으로 비교를 수행하는 동적 함수를 만든다.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private CompareHandler CreateCompareHandler(string propertyName) {
            if(IsDebugEnabled)
                log.Debug("Create CompareHandler. type=[{0}], propertyName=[{1}]", _concreteType.Name, propertyName);

            var pi = _concreteType.GetProperty(propertyName,
                                               BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
                                               BindingFlags.Instance);

            Guard.Assert(pi != null, "[{0}] 형식에 [{1}] 속성이나 필드가 없습니다.", _concreteType.Name, propertyName);

            var propertyType = pi.PropertyType; // 비교하고자 하는 속성
            var propertyGetMethod = pi.GetGetMethod(true);

            Guard.Assert(propertyGetMethod != null, "{0}.{1}은 getter 함수가 없습니다.", _concreteType.Name, propertyName);

            var comparerType = typeof(Comparer<>).MakeGenericType(new[] { propertyType });
            var getDefaultMethod = comparerType.GetProperty("Default").GetGetMethod();
            var compareMethod = getDefaultMethod.ReturnType.GetMethod("Compare");

            var dynMethod = new DynamicMethod("Compare_" + propertyName,
                                              typeof(int),
                                              new[] { _concreteType, _concreteType },
                                              comparerType);

            var generator = dynMethod.GetILGenerator();

            // Load comparer<memberType>.Default onto the stack
            generator.EmitCall(OpCodes.Call, getDefaultMethod, null);

            // Load the member from arg 0 onto the stack
            generator.Emit(OpCodes.Ldarg_0);
            generator.EmitCall(OpCodes.Callvirt, propertyGetMethod, null);

            // Load the member from arg 1 onto the stack
            generator.Emit(OpCodes.Ldarg_1);
            generator.EmitCall(OpCodes.Callvirt, propertyGetMethod, null);

            // Call the Compare method
            generator.EmitCall(OpCodes.Callvirt, compareMethod, null);
            generator.Emit(OpCodes.Ret);

            return (CompareHandler)dynMethod.CreateDelegate(typeof(CompareHandler));
        }

        /// <summary>
        /// 두 값을 비교합니다.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(T x, T y) {
            return _compare(x, y);
        }
    }
}