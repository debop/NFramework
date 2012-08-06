using System;
using System.Collections;
using System.Threading;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework {
    /// <summary>
    /// 이 클래스는 Thread context별로 local data를 관리하기 위해서 사용됩니다.
    /// 단 비동기 방식에서는 Thread 가 달라지므로 사용할 수 없습니다. 
    /// 예외적으로, 웹 환경에서는 비동기 방식의 작업 환경이라도, 저장소가 일정하므로 가능합니다.
    /// </summary>
    /// <example>
    /// <code>
    /// // 각 Thread 마다 원하는 Data를 독립적으로 저장할 수 있다.
    /// [Test]
    /// [ThreadedRepeat(5)]
    /// public void CanSaveAndLoadData()
    /// {
    ///     string key = "LocalDataFixture.Key";
    ///     string value = Guid.NewSequentialGuid().AsString();
    /// 
    /// 
    ///     Local.Data[key] = value;
    /// 
    ///     if (log.IsDebugEnabled)
    ///         log.DebugFormat("Key=[{0}], Value=[{1}]", key, Local.Data[key]);
    /// 
    ///     Assert.AreEqual(value, Local.Data[key]);
    /// }
    /// </code>
    /// </example>
    public class Local {
        private const string LocalDataKey = @"NSoft.NFramework.Local.Data.Key";
        private static readonly ILocalData _current = new LocalData();

        [Serializable]
        private class LocalData : ILocalData {
            private static readonly object _syncLock = new object();

            #region << Stable Code >>

            //
            // NOTE: CallContext를 사용하니 제대로 Thread Context가 제대로 인식이 안되어 저장이 안되었다.
            // NOTE: 꼭 ThreadStatic을 사용해야 한다.
            //

            [ThreadStatic] private static Hashtable _threadHashtable;

            private static Hashtable ObjectContexts {
                get {
                    if(IsInWebContext == false) {
                        if(_threadHashtable == null)
                            lock(_syncLock)
                                if(_threadHashtable == null) {
                                    var hashtable = new Hashtable();
                                    Thread.MemoryBarrier();
                                    _threadHashtable = hashtable;
                                }

                        return _threadHashtable;
                    }

                    var web_hashtable = System.Web.HttpContext.Current.Items[LocalDataKey] as Hashtable;

                    if(web_hashtable == null)
                        lock(_syncLock)
                            System.Web.HttpContext.Current.Items[LocalDataKey] = web_hashtable = new Hashtable();

                    return web_hashtable;
                }
            }

            #endregion

            #region << ILocalData Members >>

            /// <summary>
            /// Get or sets the <see cref="System.Object"/> with the specified key.
            /// </summary>
            /// <param name="key">key for quering</param>
            /// <returns>if storage has key, return value, otherwise return null.</returns>
            public object this[object key] {
                get { return ObjectContexts[key]; }
                set { ObjectContexts[key] = value; }
            }

            /// <summary>
            /// try get value which mapped with the specified key.
            /// </summary>
            /// <param name="key">key for quering</param>
            /// <param name="value">value mapped to key.</param>
            /// <returns>if key is exists, return true, otherwise false</returns>
            public bool TryGetValue(object key, out object value) {
                return ((value = ObjectContexts[key]) != null);
            }

            /// <summary>
            /// Clear this instance.
            /// </summary>
            public void Clear() {
                // 같은 스레드에서 lock을 쓸 필요가 없다.
                // lock(_syncLock)
                ObjectContexts.Clear();
            }

            /// <summary>
            /// 기존 데이타가 있다면 반환하고, 없다면, valueFactory 함수를 실행한 반환값을 Value로 저장한 후, Value를 반환합니다.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="valueFactory"></param>
            /// <returns></returns>
            public TValue GetOrAdd<TValue>(object key, Func<TValue> valueFactory) {
                valueFactory.ShouldNotBeNull("valueFactory");

                //if(ObjectContexts[key] == null)
                //    lock(_syncLock)
                if(ObjectContexts[key] == null) {
                    ObjectContexts[key] = valueFactory();
                }

                return (TValue)ObjectContexts[key];
            }

            /// <summary>
            /// 지정된 키에 해당하는 Value의 속성에 값을 설정합니다.
            /// </summary>
            /// <param name="key">키 값</param>
            /// <param name="valuePropertySetter">Value 속성 값을 변경할 Action</param>
            /// <returns></returns>
            public TValue SetValue<TValue>(object key, Action<TValue> valuePropertySetter) {
                return SetValue<TValue>(key, ActivatorTool.CreateInstance<TValue>, valuePropertySetter);
            }

            /// <summary>
            /// 지정된 키에 해당하는 Value의 속성에 값을 설정합니다. 
            /// </summary>
            /// <param name="key">키 값</param>
            /// <param name="valueFactory">해당 정보가 없을 때 사용할 Value 생성자</param>
            /// <param name="valuePropertySetter">Value 속성 값을 변경할 Action</param>
            /// <returns></returns>
            public TValue SetValue<TValue>(object key, Func<TValue> valueFactory, Action<TValue> valuePropertySetter) {
                var value = GetOrAdd(key, valueFactory);
                valuePropertySetter(value);
                return value;
            }

            #endregion
        }

        /// <summary>
        /// Get the current data. 이 속성은 Singleton 이지만, 내부 저장소는 Thread별로 관리된다.
        /// </summary>
        public static ILocalData Data {
            get { return _current; }
        }

        /// <summary>
        /// indicating whether running in the web context
        /// </summary>
        public static bool IsInWebContext {
            get { return System.Web.HttpContext.Current != null; }
        }
    }
}