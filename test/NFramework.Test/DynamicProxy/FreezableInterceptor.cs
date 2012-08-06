using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;

namespace NSoft.NFramework.DynamicProxy {
    [TestFixture]
    public class ProxyFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        [Test]
        public void IsFreezable_should_be_false_for_objects_created_with_ctor() {
            var nonFreezablePet = new Pet();
            Assert.IsFalse(Freezable.IsFreezable(nonFreezablePet));
        }

        [Test]
        public void IsFreezable_should_be_true_for_objects_created_with_MakeFreezable() {
            var pet = Freezable.MakeFreezable<Pet>();
            Assert.IsTrue(Freezable.IsFreezable(pet));
        }

        [Test]
        public void Freezable_should_work_normally() {
            var rex = Freezable.MakeFreezable<Pet>();
            rex.Name = "Rex";
            rex.Age += 50;
            Assert.AreEqual(50, rex.Age);
            rex.Deceased = true;

            Freezable.Freeze(rex);

            Assert.IsTrue(Freezable.IsFrozen(rex));

            try {
                rex.Age++;
                Assert.Fail("rex is frozen!!!");
            }
            catch(ObjectFreezenException) {
                Console.WriteLine("Oops. it's frozen. Can't change that anymore.");
            }
        }

        [Test]
        public void Frozen_object_should_throw_ObjectFreezenException_when_trying_to_set_a_property() {
            var pet = Freezable.MakeFreezable<Pet>();
            pet.Age = 3;

            Freezable.Freeze(pet);
            Assert.IsTrue(Freezable.IsFrozen(pet));

            Assert.Throws<ObjectFreezenException>(() => pet.Name = "This should throw");
        }

        [Test]
        public void Frozen_object_should_not_throw_when_trying_to_get_property() {
            var pet = Freezable.MakeFreezable<Pet>();
            pet.Age = 3;

            Freezable.Freeze(pet);

            var age = pet.Age;
            var name = pet.Name;
            var deceased = pet.Deceased;
            var str = pet.ToString();
        }

        [Test]
        public void Freeze_nonFreezable_object_should_throw_InvalidOperationException() {
            var rex = new Pet();
            Assert.Throws<InvalidOperationException>(() => Freezable.Freeze(rex));
        }

        [Test]
        public void Freezable_Should_Not_Intercept_Property_Getters() {
            var pet = Freezable.MakeFreezable<Pet>();
            var notUsed = pet.Age; // not intercepted 
            var interceptedMethodsCount = GetInterceptedMethodsCountFor<FreezableInterceptor>(pet);
            Assert.AreEqual(0, interceptedMethodsCount);
        }

        [Test]
        public void Freezable_should_not_intercept_normal_methods() {
            var pet = Freezable.MakeFreezable<Pet>();
            Freezable.Freeze(pet);
            Assert.IsTrue(Freezable.IsFrozen(pet));

            var dummy = pet.ToString(); // not intercepted
            var interceptedMethodsCount = GetInterceptedMethodsCountFor<FreezableInterceptor>(pet);
            Assert.AreEqual(0, interceptedMethodsCount);
        }

        [Test]
        public void Freezable_should_intercept_property_setters() {
            var pet = Freezable.MakeFreezable<Pet>();
            pet.Age = 5; // should intercept
            var interceptedMethodsCount = GetInterceptedMethodsCountFor<FreezableInterceptor>(pet);
            Assert.AreEqual(1, interceptedMethodsCount);
        }

        [Test]
        public void DynProxyGetTarget_Should_Return_Proxy_Itself() {
            var pet = Freezable.MakeFreezable<Pet>();
            var hack = pet as IProxyTargetAccessor;
            Assert.IsNotNull(hack);

            // DynProxyGetTarget()는 proxyed object를 반환한다.
            Assert.AreEqual(pet, hack.DynProxyGetTarget());
        }

        [Test]
        public void Freezable_should_log_getters_and_setters() {
            var pet = Freezable.MakeFreezable<Pet>();
            pet.Age = 4;
            var age = pet.Age;

            int logsCount = GetInterceptedMethodsCountFor<CallLoggingInterceptor>(pet);
            int freezeCount = GetInterceptedMethodsCountFor<FreezableInterceptor>(pet);

            Assert.AreEqual(2, logsCount);
            Assert.AreEqual(1, freezeCount);
        }

        [Test]
        public void Freezable_should_not_intercept_methods() {
            var pet = Freezable.MakeFreezable<Pet>();
            pet.ToString();

            int logsCount = GetInterceptedMethodsCountFor<CallLoggingInterceptor>(pet);
            int freezeCount = GetInterceptedMethodsCountFor<FreezableInterceptor>(pet);

            Assert.AreEqual(3, logsCount);
            Assert.AreEqual(0, freezeCount);
        }

        [Test]
        public void Freezable_should_not_hold_any_reference_to_created_objects() {
            var pet = Freezable.MakeFreezable<Pet>();
            var petWeakReference = new WeakReference(pet, false);
            pet = null;

            GC.Collect();
            GC.WaitForFullGCComplete();
            Assert.IsFalse(petWeakReference.IsAlive, "Object should have been released.");
        }

        private static int GetInterceptedMethodsCountFor<TInterceptor>(object obj)
            where TInterceptor : IInterceptor, ICountable {
            Assert.IsTrue(Freezable.IsFreezable(obj));

            var hack = obj as IProxyTargetAccessor;
            Assert.IsNotNull(hack);
            var interceptor =
                hack.GetInterceptors().Where(i => i is TInterceptor).Single() as ICountable;

            log.Debug("TInterceptor:{0}, Interceptor instance:{1}", typeof(TInterceptor).Name, interceptor.GetType().Name);
            return interceptor.Count;
        }

        public static class Freezable {
            public static bool IsFreezable(object obj) {
                return AsFreezable(obj) != null;
                // return obj != null && _freezables.ContainsKey(obj);
            }

            private static IFreezable AsFreezable(object target) {
                if(target == null)
                    return null;

                var hack = target as IProxyTargetAccessor;
                if(hack == null)
                    return null;

                return hack.GetInterceptors().FirstOrDefault(i => i is FreezableInterceptor) as IFreezable;
            }

            public static void Freeze(object freezable) {
                var interceptor = AsFreezable(freezable);
                if(interceptor == null) {
                    if(freezable != null)
                        log.Warn("the target object is not freesable. target={0}#{1}", freezable.GetType().FullName, freezable);

                    throw new InvalidOperationException("Not freezable: " + freezable);
                }

                interceptor.Freeze();
            }

            public static bool IsFrozen(object obj) {
                var freezable = AsFreezable(obj);
                return freezable != null && freezable.IsFrozen;
            }

            private static readonly ProxyGenerator _generator = new ProxyGenerator();
            private static readonly IInterceptorSelector _selector = new FreezableInterceptorSelector();

            public static TFreezable MakeFreezable<TFreezable>() where TFreezable : class, new() {
                var freezableInterceptor = new FreezableInterceptor();
                var options = new ProxyGenerationOptions(new FreezableProxyGenerationHook()) { Selector = _selector };
                var proxy = _generator.CreateClassProxy(typeof(TFreezable), options, new CallLoggingInterceptor(),
                                                        freezableInterceptor);
                return proxy as TFreezable;
            }
        }

        public class FreezableProxyGenerationHook : IProxyGenerationHook {
            #region << logger >>

            private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

            #endregion

            #region << IProxyGenerationHook >>

            /// <summary>
            /// Invoked by the generation process to determine if the specified method should be proxied.
            /// </summary>
            /// <param name="type">The type which declares the given method.</param><param name="methodInfo">The method to inspect.</param>
            /// <returns>
            /// True if the given method should be proxied; false otherwise.
            /// </returns>
            public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo) {
                var result = methodInfo.IsSpecialName && (IsSetterName(methodInfo.Name) || IsGetterName(methodInfo.Name));
                log.Debug("should intercept method: {0} : {1}", methodInfo.Name, result);

                return result;
            }

            /// <summary>
            /// Invoked by the generation process to notify that a member was not marked as virtual.
            /// </summary>
            /// <param name="type">The type which declares the non-virtual member.</param><param name="memberInfo">The non-virtual member.</param>
            /// <remarks>
            /// This method gives an opportunity to inspect any non-proxyable member of a type that has 
            ///             been requested to be proxied, and if appropriate - throw an exception to notify the caller.
            /// </remarks>
            public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo) {
                var method = memberInfo as MethodInfo;
                if(method != null)
                    ValidateNotSetter(method);
            }

            /// <summary>
            /// Invoked by the generation process to notify that a member was not marked as virtual.
            /// </summary>
            /// <param name="type">The type which declares the non-virtual member.</param><param name="memberInfo">The non-virtual member.</param>
            /// <remarks>
            /// Non-virtual members cannot be proxied. This method gives an opportunity to inspect
            ///             any non-virtual member of a type that has been requested to be proxied, and if
            ///             appropriate - throw an exception to notify the caller.
            /// </remarks>
            public void NonVirtualMemberNotification(Type type, MemberInfo memberInfo) {
                var method = memberInfo as MethodInfo;
                if(method != null)
                    ValidateNotSetter(method);
            }

            /// <summary>
            /// Invoked by the generation process to notify that the whole process has completed.
            /// </summary>
            public void MethodsInspected() {}

            #endregion

            private static bool IsSetterName(string name) {
                return name.StartsWith("set_", StringComparison.Ordinal);
            }

            private static bool IsGetterName(string name) {
                return name.StartsWith("get_", StringComparison.Ordinal);
            }

            private static void ValidateNotSetter(MethodInfo method) {
                if(method.IsSpecialName && IsSetterName(method.Name))
                    throw new InvalidOperationException(
                        string.Format("Property {0} is not virtual. Can't freeze classes with non-virtual properties.",
                                      method.Name.Substring("set_".Length)));
            }
        }

        public interface IFreezable {
            bool IsFrozen { get; }
            void Freeze();
        }

        public interface ICountable {
            int Count { get; }
        }

        public class FreezableInterceptor : IInterceptor, IFreezable, ICountable {
            #region << logger >>

            private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

            #endregion

            private bool _isFrozen;
            private int _count;

            public int Count {
                get { return _count; }
            }

            public void Intercept(IInvocation invocation) {
                log.Debug("Invocation method=" + invocation.Method.Name);

                if(_isFrozen && invocation.Method.Name.StartsWith("set_", StringComparison.Ordinal))
                    throw new ObjectFreezenException();

                _count++;
                invocation.Proceed();
            }

            public bool IsFrozen {
                get { return _isFrozen; }
            }

            public void Freeze() {
                _isFrozen = true;
            }
        }

        public class CallLoggingInterceptor : IInterceptor, ICountable {
            #region << logger >>

            private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

            #endregion

            private int _indention;
            private int _count;

            public int Count {
                get { return _count; }
            }

            public void Intercept(IInvocation invocation) {
                try {
                    _indention++;
                    _count++;
                    log.Debug("Invocation method=" + invocation.Method.Name);

                    invocation.Proceed();
                }
                catch(Exception ex) {
                    if(log.IsErrorEnabled)
                        log.Error(ex);

                    throw;
                }
                finally {
                    _indention--;
                }
            }
        }

        public class FreezableInterceptorSelector : IInterceptorSelector {
            public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors) {
                if(IsSetter(method))
                    return interceptors;

                return interceptors.Where(i => !(i is FreezableInterceptor)).ToArray();
            }

            private bool IsSetter(MethodInfo method) {
                return method.IsSpecialName && method.Name.StartsWith("set_", StringComparison.Ordinal);
            }

            private bool IsGetter(MethodInfo method) {
                return method.IsSpecialName && method.Name.StartsWith("get_", StringComparison.Ordinal);
            }
        }
    }

    public class ObjectFreezenException : InvalidOperationException {}

    public class Pet {
        public virtual string Name { get; set; }
        public virtual int Age { get; set; }
        public virtual bool Deceased { get; set; }

        public override string ToString() {
            return string.Format("Pet# Name=[{0}], Age=[{1}], Deceased=[{2}]", Name, Age, Deceased);
        }
    }
}