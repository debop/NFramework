using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NSoft.NFramework.Reflections {
    public static partial class ReflectionTool {
        public const string ValueFormat = @"{0}={1}";

        public const string PropertyDelimiter = "|";

        /// <summary>
        /// Item 구분자 (',')
        /// </summary>
        public const string ItemDelimiter = ",";

        /// <summary>
        /// 컬렉션 시작 구분자 ('{')
        /// </summary>
        public const string OpenBrace = "{";

        /// <summary>
        /// 컬렉션 끝 구분자 ('}')
        /// </summary>
        public const string CloseBrace = "}";

        /// <summary>
        /// 지정된 객체의 속성 정보를 "속성명=속성값" 형태의 문자열로 빌드한다.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="showCollections"></param>
        /// <returns></returns>
        public static string ObjectToString(this object instance, bool showCollections = false) {
            if(ReferenceEquals(instance, null))
                return string.Empty;

            var sb = new StringBuilder();

            try {
                var instanceType = instance.GetType();
#if !SILVERLIGHT
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor(instanceType, true);
#endif

                // object 의 type name을 표시한다.
                sb.Append(instanceType.FullName).Append("#");

                var first = true;

                foreach(var pi in GetPropertyInfos(instanceType)) {
                    if(first == false)
                        sb.Append(PropertyDelimiter);
#if !SILVERLIGHT
                    // 속성중 ValueType, Enum, string 형식만을 제공한다.
                    if(pi.PropertyType.IsValueType || pi.PropertyType.IsEnum || pi.PropertyType.Equals(typeof(string))) {
                        sb.AppendFormat(ValueFormat, pi.Name, accessor.GetPropertyValue(instance, pi.Name));
                    }
                    if(showCollections) {
                        if(pi.PropertyType.IsAssignableFrom(typeof(IDictionary)))
                            sb.AppendFormat(ValueFormat, pi.Name,
                                            DictionaryToString(accessor.GetPropertyValue(instance, pi.Name) as IDictionary));
                        else if(pi.PropertyType.IsAssignableFrom(typeof(IEnumerable)))
                            sb.AppendFormat(ValueFormat, pi.Name,
                                            CollectionToString(accessor.GetPropertyValue(instance, pi.Name) as IEnumerable));
                    }
#else
    // 속성중 ValueType, Enum, string 형식만을 제공한다.
					if (pi.PropertyType.IsValueType || pi.PropertyType.IsEnum || pi.PropertyType.Equals(typeof (string)))
					{
						sb.AppendFormat(ValueFormat, pi.Name, instanceType.GetProperty(pi.Name).GetValue(instance, null));
					}
					if (showCollections)
					{
						if (pi.PropertyType.IsAssignableFrom(typeof (IDictionary)))
							sb.AppendFormat(ValueFormat, pi.Name, DictionaryToString(instanceType.GetProperty(pi.Name).GetValue(instance, null) as IDictionary));
						else if (pi.PropertyType.IsAssignableFrom(typeof (IEnumerable)))
							sb.AppendFormat(ValueFormat, pi.Name, CollectionToString(instanceType.GetProperty(pi.Name).GetValue(instance, null) as IEnumerable));
					}
#endif

                    if(first)
                        first = false;
                }
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("객체를 문자열로 빌드하는데 실패했습니다.", ex);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 지정된 형식의 <see cref="IEnumerable"/> 의 요소들을 집합형태의 문자열로 표현한다.
        /// </summary>
        /// <param name="collection">표현할 컬렉션 객체</param>
        /// <param name="delimiter">요소 구분자</param>
        /// <param name="openBrace">컬렉션의 시작 구분자. 예: '{', '['</param>
        /// <param name="closeBrace">컬렉션의 끝 구분자. 예: '}', ']'</param>
        /// <param name="detailValues">요소의 값 정보를 자세히 보일것인가 여부</param>
        /// <returns>집합 표현형태의 문자열</returns>
        ///	<example>
        /// 요소가 1, 2, 3 이 있을 때 
        ///	<code>
        /// string s = RwCollection.AsString&lt;int&gt;(collection, ",", "{", "}");
        /// ==> {1,2,3}   // 이렇게 나온다.
        /// </code>
        /// </example>
        public static string CollectionToString(this IEnumerable collection,
                                                bool detailValues = false,
                                                string delimiter = ItemDelimiter,
                                                string openBrace = OpenBrace,
                                                string closeBrace = CloseBrace) {
            if(collection == null)
                return openBrace + closeBrace;

            var sb = new StringBuilder();
            bool first = true;

            sb.Append(openBrace);

            var iter = collection.GetEnumerator();
            while(iter.MoveNext()) {
                if(!first)
                    sb.Append(delimiter);
                else
                    first = false;

                var item = iter.Current;
                sb.Append(Equals(item, null)
                              ? "null"
                              : ((detailValues) ? ObjectToString(item) : item.ToString()));
            }

            //lock(collection)
            //{
            //    foreach(object item in collection)
            //    {
            //        if(!first)
            //            sb.Append(delimiter);
            //        else
            //            first = false;

            //        sb.Append(Equals(item, null)
            //                    ? "null"
            //                    : ((detailValues) ? ObjectToString(item) : item.AsString()));
            //    }
            //}
            sb.Append(closeBrace);

            return sb.ToString();
        }

        /// <summary>
        /// 지정된 형식의 <see cref="IDictionary"/> 의 요소들을 집합형태의 문자열로 표현한다.
        /// </summary>
        /// <param name="dictionary">표현할 <see cref="IDictionary"/>객체</param>
        /// <param name="delimiter">요소 구분자</param>
        /// <param name="openBrace">Dictionary의 시작 구분자. 예: '{', '['</param>
        /// <param name="closeBrace">Dictionary의 끝 구분자. 예: '}', ']'</param>
        /// <param name="detailValues">요소의 값 정보를 자세히 보일것인가 여부</param>
        /// <returns>집합 표현형태의 문자열</returns>
        ///	<example>
        /// A=1, B=2, C=3
        ///	<code>
        /// string s = RwCollection.AsString&lt;int&gt;(dictionary, ",", "{", "}");
        /// ==> {A=1,B=2,C=3}   // 이렇게 나온다.
        /// </code>
        /// </example>
        public static string DictionaryToString(this IDictionary dictionary,
                                                bool detailValues = false,
                                                string delimiter = ItemDelimiter,
                                                string openBrace = OpenBrace,
                                                string closeBrace = CloseBrace) {
            if(dictionary == null)
                return openBrace + closeBrace;

            var sb = new StringBuilder(dictionary.Count * 100);
            bool first = true;

            sb.Append(openBrace);

            try {
                var iter = dictionary.GetEnumerator();
                while(iter.MoveNext()) {
                    if(!first)
                        sb.Append(delimiter);
                    else
                        first = false;

                    var key = ((DictionaryEntry)iter.Current).Key;
                    var value = ((DictionaryEntry)iter.Current).Value;

                    sb.AppendFormat(ValueFormat,
                                    key,
                                    ReferenceEquals(value, null)
                                        ? "null"
                                        : ((detailValues) ? ObjectToString(value) : value.ToString()));
                }
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("Dictionary 객체를 문자열로 빌드하는데 실패했습니다.", ex);
            }

            sb.Append(closeBrace);

            return sb.ToString();
        }

        /// <summary>
        /// 지정된 형식의 <see cref="IEnumerable{T}"/> 의 요소들을 집합형태의 문자열로 표현한다.
        /// </summary>
        /// <param name="collection">표현할 컬렉션 객체</param>
        /// <param name="delimiter">요소 구분자</param>
        /// <param name="openBrace">컬렉션의 시작 구분자. 예: '{', '['</param>
        /// <param name="closeBrace">컬렉션의 끝 구분자. 예: '}', ']'</param>
        /// <param name="showDetail">요소의 값 정보를 자세히 보일것인가 여부</param>
        /// <returns>집합 표현형태의 문자열</returns>
        ///	<example>
        /// 요소가 1, 2, 3 이 있을 때 
        ///	<code>
        /// string s = RwCollection.AsString&lt;int&gt;(collection, ",", "{", "}");
        /// ==> {1,2,3}   // 이렇게 나온다.
        /// </code>
        /// </example>
        public static string CollectionToString<T>(this IEnumerable<T> collection,
                                                   bool showDetail = false,
                                                   string delimiter = ItemDelimiter,
                                                   string openBrace = OpenBrace,
                                                   string closeBrace = CloseBrace) {
            if(collection == null)
                return openBrace + closeBrace;

            var sb = new StringBuilder();
            var first = true;

            sb.Append(openBrace);

            try {
                using(var iter = collection.GetEnumerator())
                    while(iter.MoveNext()) {
                        if(!first)
                            sb.Append(delimiter);
                        else
                            first = false;

                        sb.Append(ReferenceEquals(iter.Current, null)
                                      ? "null"
                                      : ((showDetail)
                                             ? ObjectToString(iter.Current)
                                             : iter.Current.ToString()));
                    }
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("Collection 정보를 문자열로 생성하는데 예외가 발생했습니다.", ex);
            }

            sb.Append(closeBrace);

            return sb.ToString();
        }

        /// <summary>
        /// 지정된 형식의 <see cref="IDictionary{K,V}"/> 의 요소들을 집합형태의 문자열로 표현한다.
        /// </summary>
        /// <param name="dictionary">표현할 <see cref="IDictionary"/>객체</param>
        /// <param name="delimiter">요소 구분자</param>
        /// <param name="openBrace">Dictionary의 시작 구분자. 예: '{', '['</param>
        /// <param name="closeBrace">Dictionary의 끝 구분자. 예: '}', ']'</param>
        /// <param name="showDetail">요소의 값 정보를 자세히 보일것인가 여부</param>
        /// <returns>집합 표현형태의 문자열</returns>
        ///	<example>
        /// A=1, B=2, C=3
        ///	<code>
        /// string s = RwCollection.AsString&lt;int&gt;(dictionary, ",", "{", "}");
        /// ==> {A=1,B=2,C=3}   // 이렇게 나온다.
        /// </code>
        /// </example>
        public static string DictionaryToString<K, V>(this IDictionary<K, V> dictionary,
                                                      bool showDetail = false,
                                                      string delimiter = ItemDelimiter,
                                                      string openBrace = OpenBrace,
                                                      string closeBrace = CloseBrace) {
            if(dictionary == null)
                return openBrace + closeBrace;

            var sb = new StringBuilder();
            var first = true;

            sb.Append(openBrace);

            try {
                using(var iter = dictionary.GetEnumerator())
                    while(iter.MoveNext()) {
                        if(!first)
                            sb.Append(delimiter);
                        else
                            first = false;

                        var key = iter.Current.Key;
                        var value = iter.Current.Value;

                        sb.AppendFormat(ValueFormat,
                                        key,
                                        (ReferenceEquals(value, null)
                                             ? "null"
                                             : ((showDetail) ? ObjectToString(value) : value.ToString())));
                    }
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("Dictionary 객체를 문자열로 빌드하는데 실패했습니다.", ex);
            }

            sb.Append(closeBrace);

            return sb.ToString();
        }
    }
}