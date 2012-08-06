using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Castle.Core;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Data.Persisters;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data {
    public static partial class AdoTool {
        /// <summary>
        /// IDataReader의 ColumnName과 지정된 인스턴스 속성이 같은 놈만 추려낸다. (대소문자 구분 없음)
        /// </summary>
        /// <param name="instanceType">Type of persistent object</param>
        /// <param name="columnNames">필터링할 컬럼명</param>
        public static IEnumerable<string> GetCommonPropertyNames(Type instanceType, params string[] columnNames) {
            IEnumerable<string> result = null;

            if(columnNames != null && columnNames.Length > 0) {
                var propertyInfos = instanceType.GetProperties();

                result =
                    propertyInfos
                        .Select(pi => pi.Name)
                        .Intersect(columnNames, StringComparer.Create(CultureInfo.InvariantCulture, true));
            }

            return result ?? Enumerable.Empty<string>();
        }

        /// <summary>
        /// 지정된 IDataReader로부터 컬럼명-속성명 매퍼를 기준으로, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      params Expression<Func<T, object>>[] propertyExprsToExclude) where T : new() {
            return Map(reader, ActivatorTool.CreateInstance<T>, 0, 0, (Action<IDataReader, T>)null, propertyExprsToExclude);
        }

        /// <summary>
        /// 지정된 IDataReader로부터 컬럼명-속성명 매퍼를 기준으로, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작합니다. null이면 0으로 간주)</param>
        /// <param name="maxResults">매핑할 최대 레코드 수 (0이면, IDataReader의 끝까지 매핑합니다)</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      int firstResult,
                                      int maxResults,
                                      params Expression<Func<T, object>>[] propertyExprsToExclude) where T : new() {
            return Map(reader, ActivatorTool.CreateInstance<T>, firstResult, maxResults, (Action<IDataReader, T>)null, propertyExprsToExclude);
        }

        /// <summary>
        /// 지정된 IDataReader로부터 컬럼명-속성명 매퍼를 기준으로, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작합니다. null이면 0으로 간주)</param>
        /// <param name="maxResults">매핑할 최대 레코드 수 (0이면, IDataReader의 끝까지 매핑합니다)</param>
        /// <param name="additionalMapping">컬럼-속성 단순 값 매핑 이외에 부가적인 매핑을 수행하기 위해 제공되는 델리게이트</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      int firstResult,
                                      int maxResults,
                                      Action<IDataReader, T> additionalMapping,
                                      Expression<Func<T, object>>[] propertyExprsToExclude) where T : new() {
            return Map(reader, ActivatorTool.CreateInstance<T>, firstResult, maxResults, additionalMapping, propertyExprsToExclude);
        }

        /// <summary>
        /// 지정된 IDataReader로부터 컬럼명-속성명 매퍼를 기준으로, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="targetFactory">대상 객체 생성용 Factory</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      Func<T> targetFactory,
                                      params Expression<Func<T, object>>[] propertyExprsToExclude) {
            return Map(reader, targetFactory, 0, 0, (Action<IDataReader, T>)null, propertyExprsToExclude);
        }

        /// <summary>
        /// 지정된 IDataReader로부터 컬럼명-속성명 매퍼를 기준으로, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="targetFactory">대상 객체 생성용 Factory</param>
        /// <param name="additionalMapping">컬럼-속성 단순 값 매핑 이외에 부가적인 매핑을 수행하기 위해 제공되는 델리게이트</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      Func<T> targetFactory,
                                      Action<IDataReader, T> additionalMapping,
                                      Expression<Func<T, object>>[] propertyExprsToExclude) {
            return Map(reader, targetFactory, 0, 0, additionalMapping, propertyExprsToExclude);
        }

        /// <summary>
        /// 지정된 IDataReader로부터 컬럼명-속성명 매퍼를 기준으로, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="targetFactory">대상 객체 생성용 Factory</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작합니다. null이면 0으로 간주)</param>
        /// <param name="maxResults">매핑할 최대 레코드 수 (0이면, IDataReader의 끝까지 매핑합니다)</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      Func<T> targetFactory,
                                      int firstResult,
                                      int maxResults,
                                      params Expression<Func<T, object>>[] propertyExprsToExclude) {
            return Map(reader, targetFactory, firstResult, maxResults, (Action<IDataReader, T>)null, propertyExprsToExclude);
        }

        /// <summary>
        /// 지정된 IDataReader로부터 컬럼명-속성명 매퍼를 기준으로, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="targetFactory">대상 객체 생성용 Factory</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작합니다. null이면 0으로 간주)</param>
        /// <param name="maxResults">매핑할 최대 레코드 수 (0이면, IDataReader의 끝까지 매핑합니다)</param>
        /// <param name="additionalMapping">컬럼-속성 단순 값 매핑 이외에 부가적인 매핑을 수행하기 위해 제공되는 델리게이트</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      Func<T> targetFactory,
                                      int firstResult,
                                      int maxResults,
                                      Action<IDataReader, T> additionalMapping,
                                      Expression<Func<T, object>>[] propertyExprsToExclude) {

            reader.ShouldNotBeNull("reader");
            targetFactory.ShouldNotBeNull("targetFactory");
            Guard.Assert(reader.IsClosed == false, "DataReader가 이미 닫혀있습니다!!!");

            var propertyNamesToExclude = propertyExprsToExclude.Select(expr => expr.Body.FindMemberName()).ToArray();

            var nameMapper = IoC.TryResolve<INameMapper, TrimNameMapper>(LifestyleType.Thread);
            var nameMap = reader.NameMapping(nameMapper, propertyNamesToExclude);

            return Map(reader, targetFactory, nameMap, firstResult, maxResults, additionalMapping);
        }

        /// <summary>
        /// <paramref name="reader"/> 정보를 읽어들여, <see cref="TrimNameMapper"/> 매핑정보를 기반으로 T 수형의 인스턴스를 생성하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">생성할 Persistent object의 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="propertyNamesToExclude">매핑에서 제외할 속성명</param>
        /// <returns>Persistent object의 열거자</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      string[] propertyNamesToExclude) where T : new() {
            return Map(reader, ActivatorTool.CreateInstance<T>, 0, 0, (Action<IDataReader, T>)null, propertyNamesToExclude);
        }

        /// <summary>
        /// <paramref name="reader"/> 정보를 읽어들여, <see cref="TrimNameMapper"/> 매핑정보를 기반으로 T 수형의 인스턴스를 생성하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">생성할 Persistent object의 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <param name="propertyNamesToExclude">매핑에서 제외할 속성명</param>
        /// <returns>Persistent object의 열거자</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      int firstResult,
                                      int maxResults,
                                      string[] propertyNamesToExclude) where T : new() {
            return Map(reader, ActivatorTool.CreateInstance<T>, firstResult, maxResults, (Action<IDataReader, T>)null,
                       propertyNamesToExclude);
        }

        /// <summary>
        /// <paramref name="reader"/>정보를 읽어들여, <see cref="TrimNameMapper"/> 매핑정보를 기반으로 T 수형의 인스턴스를 생성하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">생성할 Persistent object의 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <param name="propertyNamesToExclude">매핑에서 제외할 속성명</param>
        /// <param name="additionalMapping">속성 이외의 추가 매핑을 위한 Action</param>
        /// <returns>Persistent object의 열거자</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      int firstResult,
                                      int maxResults,
                                      Action<IDataReader, T> additionalMapping,
                                      string[] propertyNamesToExclude) where T : new() {
            return Map(reader, ActivatorTool.CreateInstance<T>, firstResult, maxResults, additionalMapping, propertyNamesToExclude);
        }

        /// <summary>
        /// <paramref name="reader"/>정보를 읽어들여, <see cref="TrimNameMapper"/> 매핑정보를 기반으로 T 수형의 인스턴스를 생성하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">생성할 Persistent object의 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="targetFactory">Persistent object 생성 함수</param>
        /// <param name="propertyNamesToExclude">매핑에서 제외할 속성명</param>
        /// <returns>Persistent object의 열거자</returns>
        public static IList<T> Map<T>(this IDataReader reader, Func<T> targetFactory, string[] propertyNamesToExclude) {
            return Map(reader, targetFactory, 0, 0, null, propertyNamesToExclude);
        }

        /// <summary>
        /// <paramref name="reader"/>정보를 읽어들여, <see cref="TrimNameMapper"/> 매핑정보를 기반으로 T 수형의 인스턴스를 생성하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">생성할 Persistent object의 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <param name="targetFactory">Persistent object 생성 함수</param>
        /// <param name="propertyNamesToExclude">매핑에서 제외할 속성명</param>
        /// <returns>Persistent object의 열거자</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      Func<T> targetFactory,
                                      int firstResult,
                                      int maxResults,
                                      string[] propertyNamesToExclude) {
            return Map(reader, targetFactory, firstResult, maxResults, (Action<IDataReader, T>)null, propertyNamesToExclude);
        }

        /// <summary>
        /// <paramref name="reader"/> 정보를 읽어들여, <see cref="TrimNameMapper"/> 매핑정보를 기반으로 T 수형의 인스턴스를 생성하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">생성할 Persistent object의 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="targetFactory">Persistent object 생성 함수</param>
        /// <param name="additionalMapping">속성 이외의 추가 매핑을 위한 Action</param>
        /// <param name="propertyNamesToExclude">매핑에서 제외할 속성명</param>
        /// <returns>Persistent object의 열거자</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      Func<T> targetFactory,
                                      Action<IDataReader, T> additionalMapping,
                                      string[] propertyNamesToExclude = null) {
            return Map(reader, targetFactory, 0, 0, additionalMapping, propertyNamesToExclude);
        }

        /// <summary>
        /// <paramref name="reader"/> 정보를 읽어들여, <see cref="TrimNameMapper"/> 매핑정보를 기반으로 T 수형의 인스턴스를 생성하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">생성할 Persistent object의 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <param name="targetFactory">Persistent object 생성 함수</param>
        /// <param name="additionalMapping">속성 이외의 추가 매핑을 위한 Action</param>
        /// <param name="propertyNamesToExclude">매핑에서 제외할 속성명</param>
        /// <returns>Persistent object의 열거자</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      Func<T> targetFactory,
                                      int firstResult,
                                      int maxResults,
                                      Action<IDataReader, T> additionalMapping,
                                      string[] propertyNamesToExclude) {
            reader.ShouldNotBeNull("reader");
            targetFactory.ShouldNotBeNull("targetFactory");

            Guard.Assert(reader.IsClosed == false, "IDataReaer가 이미 닫혔습니다.");

            if(IsDebugEnabled)
                log.Debug("DataReader 정보를 기반으로 TrimNameMapper를 이용하여 [{0}] 형식의 인스턴스를 빌드합니다. firstResult=[{1}], maxResults=[{2}]",
                          typeof(T).Name, firstResult, maxResults);

            var nameMapper = IoC.TryResolve<INameMapper, TrimNameMapper>(LifestyleType.Thread);
            var nameMap = reader.NameMapping(nameMapper, propertyNamesToExclude);

            return Map(reader, targetFactory, nameMap, firstResult, maxResults, additionalMapping);
        }

        /// <summary>
        /// 지정된 IDataReader로부터 컬럼명-속성명 매퍼(<paramref name="nameMapper"/>) 를 기준으로, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="nameMapper">DB 컬럼명- 클래스 속성명 매핑 규칙( <seealso cref="TrimNameMapper"/>, <seealso cref="CapitalizeNameMapper"/> 등)</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      INameMapper nameMapper,
                                      params Expression<Func<T, object>>[] propertyExprsToExclude) where T : new() {
            return Map(reader, ActivatorTool.CreateInstance<T>, nameMapper, 0, 0, (Action<IDataReader, T>)null, propertyExprsToExclude);
        }

        /// <summary>
        /// 지정된 IDataReader로부터 컬럼명-속성명 매퍼(<paramref name="nameMapper"/>) 를 기준으로, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="nameMapper">DB 컬럼명- 클래스 속성명 매핑 규칙( <seealso cref="TrimNameMapper"/>, <seealso cref="CapitalizeNameMapper"/> 등)</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작합니다. null이면 0으로 간주)</param>
        /// <param name="maxResults">매핑할 최대 레코드 수 (0이면, IDataReader의 끝까지 매핑합니다)</param>	
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      INameMapper nameMapper,
                                      int firstResult,
                                      int maxResults,
                                      params Expression<Func<T, object>>[] propertyExprsToExclude) where T : new() {
            return Map(reader, ActivatorTool.CreateInstance<T>, nameMapper, firstResult, maxResults, (Action<IDataReader, T>)null,
                       propertyExprsToExclude);
        }

        /// <summary>
        /// 지정된 IDataReader로부터 컬럼명-속성명 매퍼(<paramref name="nameMapper"/>) 를 기준으로, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="targetFactory">대상 객체 생성용 Factory</param>
        /// <param name="nameMapper">DB 컬럼명- 클래스 속성명 매핑 규칙( <seealso cref="TrimNameMapper"/>, <seealso cref="CapitalizeNameMapper"/> 등)</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      Func<T> targetFactory,
                                      INameMapper nameMapper,
                                      params Expression<Func<T, object>>[] propertyExprsToExclude) {
            return Map(reader, targetFactory, nameMapper, 0, 0, (Action<IDataReader, T>)null, propertyExprsToExclude);
        }

        /// <summary>
        /// 지정된 IDataReader로부터 컬럼명-속성명 매퍼(<paramref name="nameMapper"/>) 를 기준으로, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="targetFactory">대상 객체 생성용 Factory</param>
        /// <param name="nameMapper">DB 컬럼명- 클래스 속성명 매핑 규칙( <seealso cref="TrimNameMapper"/>, <seealso cref="CapitalizeNameMapper"/> 등)</param>
        /// <param name="additionalMapping">컬럼-속성 단순 값 매핑 이외에 부가적인 매핑을 수행하기 위해 제공되는 델리게이트</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      Func<T> targetFactory,
                                      INameMapper nameMapper,
                                      Action<IDataReader, T> additionalMapping,
                                      params Expression<Func<T, object>>[] propertyExprsToExclude) {
            return Map(reader, targetFactory, nameMapper, 0, 0, additionalMapping, propertyExprsToExclude);
        }

        /// <summary>
        /// 지정된 IDataReader로부터 컬럼명-속성명 매퍼(<paramref name="nameMapper"/>) 를 기준으로, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="targetFactory">대상 객체 생성용 Factory</param>
        /// <param name="nameMapper">DB 컬럼명- 클래스 속성명 매핑 규칙( <seealso cref="TrimNameMapper"/>, <seealso cref="CapitalizeNameMapper"/> 등)</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작합니다. null이면 0으로 간주)</param>
        /// <param name="maxResults">매핑할 최대 레코드 수 (0이면, IDataReader의 끝까지 매핑합니다)</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      Func<T> targetFactory,
                                      INameMapper nameMapper,
                                      int firstResult,
                                      int maxResults,
                                      params Expression<Func<T, object>>[] propertyExprsToExclude) {
            return Map(reader, targetFactory, nameMapper, firstResult, maxResults, (Action<IDataReader, T>)null, propertyExprsToExclude);
        }

        /// <summary>
        /// 지정된 IDataReader로부터 컬럼명-속성명 매퍼(<paramref name="nameMapper"/>) 를 기준으로, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="targetFactory">대상 객체 생성용 Factory</param>
        /// <param name="nameMapper">DB 컬럼명- 클래스 속성명 매핑 규칙( <seealso cref="TrimNameMapper"/>, <seealso cref="CapitalizeNameMapper"/> 등)</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작합니다. null이면 0으로 간주)</param>
        /// <param name="maxResults">매핑할 최대 레코드 수 (0이면, IDataReader의 끝까지 매핑합니다)</param>
        /// <param name="additionalMapping">컬럼-속성 단순 값 매핑 이외에 부가적인 매핑을 수행하기 위해 제공되는 델리게이트</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      Func<T> targetFactory,
                                      INameMapper nameMapper,
                                      int firstResult,
                                      int maxResults,
                                      Action<IDataReader, T> additionalMapping,
                                      params Expression<Func<T, object>>[] propertyExprsToExclude) {
            reader.ShouldNotBeNull("reader");

            Guard.Assert(reader.IsClosed == false, "DataReader가 이미 닫혀있습니다!!!");

            if(nameMapper == null)
                nameMapper = IoC.TryResolve<INameMapper, TrimNameMapper>(LifestyleType.Thread);

            var propertyNamesToExclude = propertyExprsToExclude.Select(expr => expr.Body.FindMemberName()).ToArray();
            var nameMap = reader.NameMapping(nameMapper, propertyNamesToExclude);

            return Map(reader, targetFactory, nameMap, firstResult, maxResults, additionalMapping);
        }

        /// <summary>
        /// 지정된 IDataReader로부터 (컬럼명-속성명) 매핑 정보(<paramref name="nameMap"/>) 에 따라, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="nameMap">DB 컬럼명- 클래스 속성명 매핑 정보</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader, INameMap nameMap) where T : new() {
            return Map(reader, ActivatorTool.CreateInstance<T>, nameMap, 0, 0, (Action<IDataReader, T>)null);
        }

        /// <summary>
        /// 지정된 IDataReader로부터 (컬럼명-속성명) 매핑 정보(<paramref name="nameMap"/>) 에 따라, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="nameMap">DB 컬럼명- 클래스 속성명 매핑 정보</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작합니다. null이면 0으로 간주)</param>
        /// <param name="maxResults">매핑할 최대 레코드 수 (0이면, IDataReader의 끝까지 매핑합니다)</param>	
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      INameMap nameMap,
                                      int firstResult,
                                      int maxResults) where T : new() {
            return Map(reader, ActivatorTool.CreateInstance<T>, nameMap, firstResult, maxResults, (Action<IDataReader, T>)null);
        }

        /// <summary>
        /// 지정된 IDataReader로부터 (컬럼명-속성명) 매핑 정보(<paramref name="nameMap"/>) 에 따라, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="targetFactory">대상 객체 생성용 Factory</param>
        /// <param name="nameMap">DB 컬럼명- 클래스 속성명 매핑 정보</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      Func<T> targetFactory,
                                      INameMap nameMap) {
            return Map(reader, targetFactory, nameMap, 0, 0, (Action<IDataReader, T>)null);
        }

        /// <summary>
        /// 지정된 IDataReader로부터 (컬럼명-속성명) 매핑 정보(<paramref name="nameMap"/>) 에 따라, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="targetFactory">대상 객체 생성용 Factory</param>
        /// <param name="nameMap">DB 컬럼명- 클래스 속성명 매핑 정보</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작합니다. null이면 0으로 간주)</param>
        /// <param name="maxResults">매핑할 최대 레코드 수 (0이면, IDataReader의 끝까지 매핑합니다)</param>		
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      Func<T> targetFactory,
                                      INameMap nameMap,
                                      int firstResult,
                                      int maxResults) {
            return Map(reader, targetFactory, nameMap, firstResult, maxResults, (Action<IDataReader, T>)null);
        }

        /// <summary>
        /// 지정된 IDataReader로부터 (컬럼명-속성명) 매핑 정보(<paramref name="nameMap"/>) 에 따라, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="targetFactory">대상 객체 생성용 Factory</param>
        /// <param name="nameMap">DB 컬럼명- 클래스 속성명 매핑 정보</param>
        /// <param name="additionalMapping">컬럼-속성 단순 값 매핑 이외에 부가적인 매핑을 수행하기 위해 제공되는 델리게이트</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      Func<T> targetFactory,
                                      INameMap nameMap,
                                      Action<IDataReader, T> additionalMapping) {
            return Map(reader, targetFactory, nameMap, 0, 0, additionalMapping);
        }

        /// <summary>
        /// 지정된 IDataReader로부터 (컬럼명-속성명) 매핑 정보(<paramref name="nameMap"/>) 에 따라, 대상 객체를 인스턴싱하고, 속성에 값을 설정하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader 객체</param>
        /// <param name="targetFactory">대상 객체 생성용 Factory</param>
        /// <param name="nameMap">DB 컬럼명- 클래스 속성명 매핑 정보</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작합니다. null이면 0으로 간주)</param>
        /// <param name="maxResults">매핑할 최대 레코드 수 (0이면, IDataReader의 끝까지 매핑합니다)</param>
        /// <param name="additionalMapping">컬럼-속성 단순 값 매핑 이외에 부가적인 매핑을 수행하기 위해 제공되는 델리게이트</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      Func<T> targetFactory,
                                      INameMap nameMap,
                                      int firstResult,
                                      int maxResults,
                                      Action<IDataReader, T> additionalMapping) {
            reader.ShouldNotBeNull("reader");
            nameMap.ShouldNotBeNull("nameMap");
            targetFactory.ShouldNotBeNull("targetFactory");

            Guard.Assert(reader.IsClosed == false, "지정된 IDataReader가 이미 닫혀 있습니다!!!");

            if(IsDebugEnabled)
                log.Debug("IDataReader를 읽어 INameMap을 이용하여 [{0}] 형식의 인스턴스 컬렉션을 반환합니다. " +
                          "nameMap=[{1}], firstResult=[{2}], maxResults=[{3}]",
                          typeof(T).FullName, nameMap.CollectionToString(), firstResult, maxResults);

            var targets = new List<T>();

            var firstIndex = firstResult;
            var maxCount = maxResults;

            while(firstIndex-- > 0) {
                if(reader.Read() == false)
                    return targets;
            }

            if(maxCount <= 0)
                maxCount = Int32.MaxValue;

            var targetAccessor = GetDynamicAccessor<T>();

            var mappedCount = 0;

            var columNames = nameMap.Keys.ToArray();

            while(reader.Read() && (mappedCount < maxCount)) {
                var target = targetFactory();

                columNames.RunEach(colName => targetAccessor.SetPropertyValue(target, nameMap[colName], reader.AsValue(colName)));

                if(additionalMapping != null)
                    additionalMapping(reader, target);

                targets.Add(target);
                mappedCount++;
            }

            if(IsDebugEnabled)
                log.Debug("IDataReader로부터 [{0}] 수형의 인스턴스 [{1}]개를 생성했습니다.", typeof(T).FullName, targets.Count);

            return targets;
        }

        /// <summary>
        /// DataReader 정보를 <paramref name="persister"/>가 처리하여 대상 객체로 빌드합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="persister">대상 객체를 빌드하는 Persister</param>
        /// <returns>생성된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader, IReaderPersister<T> persister) {
            return Map(reader, persister, 0, 0, (Action<IDataReader, T>)null);
        }

        /// <summary>
        /// DataReader 정보를 <paramref name="persister"/>가 처리하여 대상 객체로 빌드합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="persister">대상 객체를 빌드하는 Persister</param>
        /// <param name="additinalMapping">부가 매핑 작업을 수행하는 델리게이트</param>
        /// <returns>생성된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      IReaderPersister<T> persister,
                                      Action<IDataReader, T> additinalMapping) {
            return Map(reader, persister, 0, 0, additinalMapping);
        }

        /// <summary>
        ///  DataReader 정보를 <paramref name="persister"/>가 처리하여 대상 객체로 빌드합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="persister">대상 객체를 빌드하는 Persister</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작합니다. null이면 0으로 간주)</param>
        /// <param name="maxResults">매핑할 최대 레코드 수 (0이면, IDataReader의 끝까지 매핑합니다)</param>
        /// <returns>생성된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader, IReaderPersister<T> persister, int firstResult, int maxResults) {
            return Map(reader, persister, firstResult, maxResults, (Action<IDataReader, T>)null);
        }

        /// <summary>
        /// DataReader 정보를 <paramref name="persister"/>가 처리하여 대상 객체로 빌드합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="persister">대상 객체를 빌드하는 Persister</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작합니다. null이면 0으로 간주)</param>
        /// <param name="maxResults">매핑할 최대 레코드 수 (0이면, IDataReader의 끝까지 매핑합니다)</param>
        /// <param name="additionalMapping">부가 매핑 작업을 수행하는 델리게이트</param>
        /// <returns>생성된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      IReaderPersister<T> persister,
                                      int firstResult,
                                      int maxResults,
                                      Action<IDataReader, T> additionalMapping) {
            reader.ShouldNotBeNull("reader");
            persister.ShouldNotBeNull("persister");
            Guard.Assert(reader.IsClosed == false, "IDataReader가 이미 닫혀있습니다.");

            if(IsDebugEnabled)
                log.Debug("Persister를 이용하여 DataReader로부터 [{0}] 수형의 객체를 생성합니다. firstResult=[{1}], maxResults=[{2}]",
                          typeof(T).FullName, firstResult, maxResults);

            var targets = new List<T>();

            // 첫 번째 인덱스 전에 것들은 Skip 해버립니다.
            //
            while(firstResult-- > 0) {
                if(reader.Read() == false)
                    return targets;
            }

            if(maxResults <= 0)
                maxResults = Int32.MaxValue;

            var instanceCount = 0;

            while(reader.Read() && (instanceCount < maxResults)) {
                var target = persister.Persist(reader);

                if(additionalMapping != null)
                    additionalMapping(reader, target);

                targets.Add(target);

                instanceCount++;
            }

            if(IsDebugEnabled)
                log.Debug("Persister를 이용하여 DataReader로부터 [{0}] 수형의 객체 [{1}]개를 매핑했습니다.", typeof(T).FullName, targets.Count);

            return targets;
        }

        /// <summary>
        /// 지정한 DataReader 정보를 <paramref name="readerMapFunc"/>가 처리하여 대상 객체로 빌드합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="readerMapFunc">대상 객체를 빌드하는 함수</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader, Func<IDataReader, T> readerMapFunc) {
            return Map(reader, readerMapFunc, 0, 0);
        }

        /// <summary>
        /// 지정한 DataReader 정보를 <paramref name="readerMapFunc"/>가 처리하여 대상 객체로 빌드합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="readerMapFunc">대상 객체를 빌드하는 함수</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작합니다. null이면 0으로 간주)</param>
        /// <param name="maxResults">매핑할 최대 레코드 수 (0이면, IDataReader의 끝까지 매핑합니다)</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> Map<T>(this IDataReader reader,
                                      Func<IDataReader, T> readerMapFunc,
                                      int firstResult,
                                      int maxResults) {
            reader.ShouldNotBeNull("reader");
            readerMapFunc.ShouldNotBeNull("readerMapFunc");
            Guard.Assert(reader.IsClosed == false, "IDataReader가 이미 닫혀있습니다.");

            if(IsDebugEnabled)
                log.Debug("매핑함수를 이용하여 DataReader로부터 [{0}] 수형의 객체를 생성합니다. firstResult=[{1}], maxResults=[{2}]",
                          typeof(T).FullName, firstResult, maxResults);

            var targets = new List<T>();

            // 첫 번째 인덱스 전에 것들은 Skip 해버립니다.
            //
            while(firstResult-- > 0) {
                if(reader.Read() == false)
                    return targets;
            }

            if(maxResults <= 0)
                maxResults = Int32.MaxValue;

            var instanceCount = 0;

            while(reader.Read() && (instanceCount < maxResults)) {
                targets.Add(readerMapFunc(reader));
                instanceCount++;
            }

            if(IsDebugEnabled)
                log.Debug("Mapping 함수를 이용하여 DataReader로부터 [{0}] 수형의 객체 [{1}]개를 매핑했습니다.", typeof(T).FullName, targets.Count);

            return targets;
        }

        /// <summary>
        /// <paramref name="reader"/>의 레코드 정보를 필터링 (<paramref name="rowFilter"/>)애 통과한 Row만 <paramref name="nameMapper"/>를 통해 매핑을 수행합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체</typeparam>
        /// <param name="reader">IDataReader 객체</param>
        /// <param name="targetFactory">대상 객체 생성용 Factory</param>
        /// <param name="nameMapper">DB 컬럼명- 클래스 속성명 매퍼</param>
        /// <param name="rowFilter">매핑할 Row를 선별할 필터 함수</param>
        /// <param name="additionalMapping">컬럼-속성 단순 값 매핑 이외에 부가적인 매핑을 수행하기 위해 제공되는 델리게이트</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>매핑된 대상 객체 컬렉션</returns>
        public static IEnumerable<T> MapIf<T>(this IDataReader reader,
                                              Func<T> targetFactory,
                                              INameMapper nameMapper,
                                              Func<IDataReader, bool> rowFilter,
                                              Action<IDataReader, T> additionalMapping,
                                              params Expression<Func<T, object>>[] propertyExprsToExclude) {
            targetFactory.ShouldNotBeNull("targetFactory");
            nameMapper.ShouldNotBeNull("nameMapper");

            var propertyNamesToExclude = propertyExprsToExclude.Select(expr => expr.Body.FindMemberName()).ToArray();
            var nameMap = reader.NameMapping(nameMapper, propertyNamesToExclude);

            return MapIf(reader, targetFactory, nameMap, rowFilter, additionalMapping);
        }

        /// <summary>
        /// <paramref name="reader"/>의 레코드 정보를 필터링 (<paramref name="rowFilter"/>)애 통과한 Row만 <paramref name="nameMap"/>를 통해 매핑을 수행합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체</typeparam>
        /// <param name="reader">IDataReader 객체</param>
        /// <param name="targetFactory">대상 객체 생성용 Factory</param>
        /// <param name="nameMap">DB 컬럼명- 클래스 속성명 매핑 정보</param>
        /// <param name="rowFilter">매핑할 Row를 선별할 필터 함수</param>
        /// <param name="additionalMapping">컬럼-속성 단순 값 매핑 이외에 부가적인 매핑을 수행하기 위해 제공되는 델리게이트</param>
        /// <returns>매핑된 대상 객체 컬렉션</returns>
        public static IEnumerable<T> MapIf<T>(this IDataReader reader,
                                              Func<T> targetFactory,
                                              INameMap nameMap,
                                              Func<IDataReader, bool> rowFilter,
                                              Action<IDataReader, T> additionalMapping = null) {
            targetFactory.ShouldNotBeNull("targetFactory");
            nameMap.ShouldNotBeNull("nameMap");

            var columNames = nameMap.Keys.ToArray();
            var targetAccessor = GetDynamicAccessor<T>();

            Func<IDataReader, T> @readerMapFunc =
                dr => {
                    var target = targetFactory();

                    columNames.RunEach(colName => targetAccessor.SetPropertyValue(target,
                                                                                  nameMap[colName],
                                                                                  dr.AsValue(colName)));

                    if(additionalMapping != null)
                        additionalMapping(dr, target);

                    return target;
                };

            return MapIf(reader, readerMapFunc, rowFilter);
        }

        /// <summary>
        /// DataReader 정보를 <paramref name="persister"/>가 처리하여 대상 객체로 빌드합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="persister">대상 객체를 빌드하는 Persister</param>
        /// <param name="rowFilter">매핑할 Row를 선별할 필터 함수</param>
        /// <param name="additionalMapping">부가 매핑 작업을 수행하는 델리게이트</param>
        /// <returns>생성된 대상 객체의 컬렉션</returns>
        public static IEnumerable<T> MapIf<T>(this IDataReader reader,
                                              IReaderPersister<T> persister,
                                              Func<IDataReader, bool> rowFilter,
                                              Action<IDataReader, T> additionalMapping = null) {
            persister.ShouldNotBeNull("persister");

            Func<IDataReader, T> @readerMapFunc =
                dr => {
                    var target = persister.Persist(reader);

                    if(additionalMapping != null)
                        additionalMapping(dr, target);

                    return target;
                };

            return MapIf(reader, readerMapFunc, rowFilter);
        }

        /// <summary>
        /// <paramref name="reader"/>의 레코드 정보를 필터링 (<paramref name="rowFilter"/>)애 통과한 Row만 <paramref name="readerMapFunc"/>를 통해 매핑을 수행합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체</typeparam>
        /// <param name="reader">IDataReader 객체</param>
        /// <param name="readerMapFunc">매핑 함수</param>
        /// <param name="rowFilter">매핑할 Row를 선별할 필터 함수</param>
        /// <returns>매핑된 대상 객체 컬렉션</returns>
        public static IEnumerable<T> MapIf<T>(this IDataReader reader,
                                              Func<IDataReader, T> readerMapFunc,
                                              Func<IDataReader, bool> rowFilter) {
            reader.ShouldNotBeNull("reader");
            readerMapFunc.ShouldNotBeNull("readerMapFunc");
            rowFilter.ShouldNotBeNull("rowFilter");

            Guard.Assert(reader.IsClosed == false, "IDataReader가 이미 닫혀있습니다.");

            if(IsDebugEnabled)
                log.Debug("Filtering을 거친 IDataReader 정보를 Func<IDataReader,T>를 이용하여 [{0}] 수형의 객체를 생성합니다.", typeof(T).FullName);

            while(reader.Read()) {
                if(rowFilter(reader))
                    yield return readerMapFunc(reader);
            }
        }

        /// <summary>
        /// <paramref name="reader"/>의 레코드 정보를 <paramref name="nameMapper"/>를 통해 매핑을 수행하는데, 계속조건 (<paramref name="continuationCondition"/>) 이 만족할때까지만 수행한다.
        /// </summary>
        /// <typeparam name="T">대상 객체</typeparam>
        /// <param name="reader">IDataReader 객체</param>
        /// <param name="targetFactory">대상 객체 생성용 Factory</param>
        /// <param name="nameMapper">DB 컬럼명- 클래스 속성명 매퍼</param>
        /// <param name="continuationCondition">진행 조건 (False가 나올 때까지 진행합니다)</param>
        /// <param name="additionalMapping">컬럼-속성 단순 값 매핑 이외에 부가적인 매핑을 수행하기 위해 제공되는 델리게이트</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>매핑된 대상 객체 컬렉션</returns>
        public static IEnumerable<T> MapWhile<T>(this IDataReader reader,
                                                 Func<T> targetFactory,
                                                 INameMapper nameMapper,
                                                 Func<IDataReader, bool> continuationCondition,
                                                 Action<IDataReader, T> additionalMapping,
                                                 params Expression<Func<T, object>>[] propertyExprsToExclude) {
            targetFactory.ShouldNotBeNull("targetFactory");
            nameMapper.ShouldNotBeNull("nameMapper");

            var propertyNamesToExclude = propertyExprsToExclude.Select(expr => expr.Body.FindMemberName()).ToArray();
            var nameMap = reader.NameMapping(nameMapper, propertyNamesToExclude);

            return MapWhile(reader, targetFactory, nameMap, continuationCondition, additionalMapping);
        }

        /// <summary>
        /// <paramref name="reader"/>의 레코드 정보를 <paramref name="nameMap"/>를 통해 매핑을 수행하는데, 계속조건 (<paramref name="continuationCondition"/>) 이 만족할때까지만 수행한다.
        /// </summary>
        /// <typeparam name="T">대상 객체</typeparam>
        /// <param name="reader">IDataReader 객체</param>
        /// <param name="targetFactory">대상 객체 생성용 Factory</param>
        /// <param name="nameMap">DB 컬럼명- 클래스 속성명 매핑 정보</param>
        /// <param name="continuationCondition">진행 조건 (False가 나올 때까지 진행합니다)</param>
        /// <param name="additionalMapping">컬럼-속성 단순 값 매핑 이외에 부가적인 매핑을 수행하기 위해 제공되는 델리게이트</param>
        /// <returns>매핑된 대상 객체 컬렉션</returns>
        public static IEnumerable<T> MapWhile<T>(this IDataReader reader,
                                                 Func<T> targetFactory,
                                                 INameMap nameMap,
                                                 Func<IDataReader, bool> continuationCondition,
                                                 Action<IDataReader, T> additionalMapping = null) {
            targetFactory.ShouldNotBeNull("targetFactory");
            nameMap.ShouldNotBeNull("nameMap");

            var columNames = nameMap.Keys.ToArray();
            var targetAccessor = GetDynamicAccessor<T>();

            Func<IDataReader, T> @readerMapFunc = dr => {
                var target = targetFactory();

                columNames.RunEach(colName =>
                                   targetAccessor.SetPropertyValue(target,
                                                                   nameMap[colName],
                                                                   dr.AsValue(colName)));

                if(additionalMapping != null)
                    additionalMapping(dr, target);

                return target;
            };

            return MapWhile(reader, readerMapFunc, continuationCondition);
        }

        /// <summary>
        /// <paramref name="reader"/>의 레코드 정보를 <paramref name="persister"/>를 통해 매핑을 수행하는데, 계속조건 (<paramref name="continuationCondition"/>) 이 만족할때까지만 수행한다.
        /// </summary>
        /// <typeparam name="T">대상 객체</typeparam>
        /// <param name="reader">IDataReader 객체</param>
        /// <param name="persister">대상 객체를 빌드하는 Persister</param>
        /// <param name="continuationCondition">진행 조건 (False가 나올 때까지 진행합니다)</param>
        /// <param name="additionalMapping">컬럼-속성 단순 값 매핑 이외에 부가적인 매핑을 수행하기 위해 제공되는 델리게이트</param>
        /// <returns>매핑된 대상 객체 컬렉션</returns>
        public static IEnumerable<T> MapWhile<T>(this IDataReader reader,
                                                 IReaderPersister<T> persister,
                                                 Func<IDataReader, bool> continuationCondition,
                                                 Action<IDataReader, T> additionalMapping = null) {
            Func<IDataReader, T> @readerMapFunc =
                dr => {
                    var target = persister.Persist(reader);

                    if(additionalMapping != null)
                        additionalMapping(dr, target);

                    return target;
                };

            return MapWhile(reader, readerMapFunc, continuationCondition);
        }

        /// <summary>
        /// <paramref name="reader"/>의 레코드 정보를 <paramref name="readerMapFunc"/>를 통해 매핑을 수행하는데, 계속조건 (<paramref name="continuationCondition"/>) 이 만족할때까지만 수행한다.
        /// </summary>
        /// <typeparam name="T">대상 객체</typeparam>
        /// <param name="reader">IDataReader 객체</param>
        /// <param name="readerMapFunc">매핑 함수</param>
        /// <param name="continuationCondition">진행 조건 (False가 나올 때까지 진행합니다)</param>
        /// <returns>매핑된 대상 객체 컬렉션</returns>
        public static IEnumerable<T> MapWhile<T>(this IDataReader reader,
                                                 Func<IDataReader, T> readerMapFunc,
                                                 Func<IDataReader, bool> continuationCondition) {
            reader.ShouldNotBeNull("reader");
            readerMapFunc.ShouldNotBeNull("readerMapFunc");
            continuationCondition.ShouldNotBeNull("continuationCondition");
            Guard.Assert(reader.IsClosed == false, "IDataReader가 이미 닫혀있습니다.");

            if(IsDebugEnabled)
                log.Debug("IDataReader 정보를 읽어 계속 조건에 만족하면 Func<IDataReader,T>를 이용하여 [{0}] 수형의 인스턴스로 매핑합니다.", typeof(T).FullName);

            while(reader.Read() && continuationCondition(reader))
                yield return readerMapFunc(reader);
        }
    }
}