using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Tools {
    /// <summary>
    /// Utility class for IDictionary{T,V}
    /// </summary>
    public static class DictionaryTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static object _syncLock = new object();

        /// <summary>
        /// Dictionary의 지정된 키에 해당하는 값을 반환한다. 지정된 키가 없다면, 예외가 발생한다.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
            TValue value;

            if(dictionary.TryGetValue(key, out value) == false)
                throw new KeyNotFoundException("The key '" + key + "' was not found in the dictionary");

            return value;
        }

        /// <summary>
        /// Dictionary의 지정된 키에 해당하는 값을 반환한다. 해당 키가 없다면, 기본 값을 반환한다. 
        /// </summary>
        /// <typeparam name="TKey">Type of key of dictionary</typeparam>
        /// <typeparam name="TValue">Type of value of dictionary</typeparam>
        /// <param name="dictionary">dictionary</param>
        /// <param name="key">key to retrieve value</param>
        /// <param name="defval">default value</param>
        /// <returns>value matched the specified key, if key does not exists, return defval</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defval) {
            TValue value;

            if(dictionary.TryGetValue(key, out value))
                return value;

            return defval;
        }

        /// <summary>
        /// Dictionary에 값을 추가합니다. Key가 있다면, 값을 대체하고, Key가 없다면 추가합니다.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value) {
            if(dictionary is ConcurrentDictionary<TKey, TValue>) {
                ((ConcurrentDictionary<TKey, TValue>)dictionary).AddOrUpdate(key, value, (k, v) => value);
                return;
            }

            lock(dictionary) {
                if(dictionary.ContainsKey(key))
                    dictionary[key] = value;
                else
                    dictionary.Add(key, value);
            }
        }

        /// <summary>
        /// Dictionary에서 지정한 키에 해당하는 값을 가져옵니다. 만약 없다면, <paramref name="valueFactory"/>로 값을 등록한 후 반환합니다.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory) {
            valueFactory.ShouldNotBeNull("valueFactory");

            if(dictionary is ConcurrentDictionary<TKey, TValue>)
                return ((ConcurrentDictionary<TKey, TValue>)dictionary).GetOrAdd(key, valueFactory);


            lock(dictionary) {
                if(dictionary.ContainsKey(key) == false) {
                    dictionary.Add(key, valueFactory(key));

                    if(IsDebugEnabled)
                        log.Debug("Dictionary[{0}]에 새로운 값을 추가하였습니다. value=[{1}]", key, dictionary[key]);
                }
                return dictionary[key];
            }
        }

        /// <summary>
        /// Dictionary에 지정한 키에 해당하는 값을 찾아서, <paramref name="valuePropertySetter"/>로 값을 변경합니다.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="valuePropertySetter"></param>
        /// <returns></returns>
        public static TValue SetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
                                                    Action<TValue> valuePropertySetter) {
#if !SILVERLIGHT
            return SetValue(dictionary, key, k => ActivatorTool.CreateInstance<TValue>(), valuePropertySetter);
#else
			return SetValue(dictionary, key, k => Activator.CreateInstance<TValue>(), valuePropertySetter);
#endif
        }

        /// <summary>
        /// Dictionary에 지정한 키에 해당하는 값을 찾아서, <paramref name="valuePropertySetter"/>로 값을 변경합니다.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="valueFactory"></param>
        /// <param name="valuePropertySetter"></param>
        /// <returns></returns>
        public static TValue SetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                    TKey key,
                                                    Func<TKey, TValue> valueFactory,
                                                    Action<TValue> valuePropertySetter) {
            valuePropertySetter.ShouldNotBeNull("valuePropertySetter");

            var value = GetOrAdd(dictionary, key, valueFactory);

            valuePropertySetter(value);

            return value;
        }

#if !SILVERLIGHT

        /// <summary>
        /// 지정된 Dictionary 정보를 문자열로 표현합니다. (거의 JSON 포맷과 같습니다!!!)
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static string AsString(this IDictionary dictionary) {
            return string.Concat("{",
                                 dictionary.Keys
                                     //.ToList<object>()
                                     .Cast<object>()
                                     .Select(key => string.Format("[{0},{1}]", key, dictionary[key].ObjectToString()))
                                     .AsJoinedText(","),
                                 "}");
        }

        /// <summary>
        /// 지정된 Dictionary 정보를 문자열로 표현합니다.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static string AsString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) {
            return string.Concat("{",
                                 dictionary.Keys
                                     .Select(key => string.Format("[{0},{1}]", key, dictionary[key].ObjectToString()))
                                     .AsJoinedText(","),
                                 "}");
        }

#endif
    }
}