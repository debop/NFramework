using System;
using System.Reflection;
using System.Threading.Tasks;
using Fasterflect;
using NSoft.NFramework.Parallelism.Tools;

namespace NSoft.NFramework.Reflections {
    public static partial class FasterflectTool {
        /// <summary>
        /// 특정 수형의 특정 메소드를 호출할 수 있는 <see cref="MethodInvoker"/>를 입력 받아 <paramref name="action"/>을 수행합니다.
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="action"></param>
        /// <param name="methodName"></param>
        /// <param name="parameterTypes"></param>
        public static void InvokeMethod(this Type instanceType, Action<MethodInvoker> action, string methodName,
                                        params Type[] parameterTypes) {
            instanceType.ShouldNotBeNull("instanceType");
            methodName.ShouldNotBeWhiteSpace("methodName");
            action.ShouldNotBeNull("action");

            var invoker = GetMethodInvoker(instanceType, methodName, parameterTypes);

            if(invoker != null) {
                if(IsDebugEnabled)
                    log.Debug("메소드[{0}]를 수행합니다...", methodName);

                action(invoker);
            }
        }

        /// <summary>
        /// 특정 수형의 특정 메소드를 호출할 수 있는 <see cref="MethodInvoker"/>를 입력 받아 <paramref name="action"/>을 수행합니다.
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="action"></param>
        /// <param name="methodName"></param>
        /// <param name="parameterTypes"></param>
        public static Task InvokeMethodAsync(this Type instanceType, Action<MethodInvoker> action, string methodName,
                                             params Type[] parameterTypes) {
            return Task.Factory.StartNew(() => InvokeMethod(instanceType, action, methodName, parameterTypes));
        }

        /// <summary>
        /// 특정 수형의 특정 메소드를 호출할 수 있는 <see cref="MethodInvoker"/>를 입력 받아 <paramref name="action"/>을 수행합니다.
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="fromInclusive"></param>
        /// <param name="toExclusive"></param>
        /// <param name="action"></param>
        /// <param name="methodName"></param>
        /// <param name="parameterTypes"></param>
        public static ParallelLoopResult InvokeMethodAsParallel(this Type instanceType,
                                                                int fromInclusive,
                                                                int toExclusive,
                                                                Action<int, MethodInvoker> action,
                                                                string methodName,
                                                                params Type[] parameterTypes) {
            instanceType.ShouldNotBeNull("instanceType");
            action.ShouldNotBeNull("action");

            var methodInvoker = instanceType.GetMethodInvoker(methodName, DefaultFlags, parameterTypes);

            if(methodInvoker != null) {
                if(IsDebugEnabled)
                    log.Debug("메소드[{0}]를 병렬 방식으로 수행합니다...", methodName);

                return
                    Parallel.For(fromInclusive,
                                 toExclusive,
                                 i => action(i, methodInvoker));
            }

            return new ParallelLoopResult();
        }

        /// <summary>
        /// 특정 수형의 특정 메소드를 호출할 수 있는 <see cref="MethodInvoker"/>를 입력 받아 <paramref name="action"/>을 수행합니다.
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="fromInclusive"></param>
        /// <param name="toExclusive"></param>
        /// <param name="parallelOptions"></param>
        /// <param name="action"></param>
        /// <param name="methodName"></param>
        /// <param name="parameterTypes"></param>
        public static ParallelLoopResult InvokeMethodAsParallel(this Type instanceType,
                                                                int fromInclusive,
                                                                int toExclusive,
                                                                ParallelOptions parallelOptions,
                                                                Action<int, MethodInvoker> action,
                                                                string methodName,
                                                                params Type[] parameterTypes) {
            instanceType.ShouldNotBeNull("instanceType");
            action.ShouldNotBeNull("action");

            var methodInvoker = instanceType.GetMethodInvoker(methodName, DefaultFlags, parameterTypes);
            if(methodInvoker != null) {
                if(IsDebugEnabled)
                    log.Debug("메소드[{0}]를 병렬 방식으로 수행합니다...", methodName);

                return Parallel.For(fromInclusive,
                                    toExclusive,
                                    parallelOptions ?? ParallelTool.DefaultParallelOptions,
                                    i => action(i, methodInvoker));
            }

            return new ParallelLoopResult();
        }

        /// <summary>
        /// 특정 수형의 특정 메소드를 호출할 수 있는 델리게이트를 제공합니다.
        /// </summary>
        /// <seealso cref="MethodExtensions.DelegateForCallMethod(System.Type,string,System.Type[])"/>
        public static MethodInvoker GetMethodInvoker(this Type instanceType, string methodName, params Type[] parameterTypes) {
            instanceType.ShouldNotBeNull("instanceType");
            methodName.ShouldNotBeWhiteSpace("methodName");

            if(IsDebugEnabled)
                log.Debug("Get CallMethod for Delegate... instanceType=[{0}], methodName=[{1}], parameterTypes=[{2}]",
                          instanceType, methodName, parameterTypes.CollectionToString());

            return MethodExtensions.DelegateForCallMethod(instanceType, methodName, parameterTypes);
        }

        /// <summary>
        /// 특정 수형의 특정 메소드를 호출할 수 있는 델리게이트를 제공합니다.
        /// </summary>
        /// <seealso cref="MethodExtensions.DelegateForCallMethod(System.Type,string,Fasterflect.Flags,System.Type[])"/>
        public static MethodInvoker GetMethodInvoker(this Type instanceType, string methodName, BindingFlags flags,
                                                     params Type[] parameterTypes) {
            instanceType.ShouldNotBeNull("instanceType");
            methodName.ShouldNotBeWhiteSpace("methodName");

            if(IsDebugEnabled)
                log.Debug("Get CallMethod for Delegate... instanceType=[{0}], methodName=[{1}], flags=[{2}], parameterTypes=[{3}]",
                          instanceType, methodName, flags, parameterTypes.CollectionToString());

            return MethodExtensions.DelegateForCallMethod(instanceType, methodName, flags, parameterTypes);
        }
    }
}