using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.Mappers {
    /// <summary>
    /// 일반적으로 DATABASE의 컬럼명들은 대문자, 구분자('_', '[', ']', '_' 등)이 사용됩니다.
    /// 이런 Data 객체 (<see cref="IDataReader"/>, <see cref="DataTable"/>) 에서 Class로 만드려면 
    /// DATABASE의 컬럼 명과 Class의 Property 명이 매핑시켜줘야 합니다.
    /// 
    /// NHibernate는 hbm.xml 이라는 Mapping 파일을 사용하고, Active Record는 Property에 Attribute를 정의해서 사용합니다.
    /// Solution을 만들때에는 위와 같이 정형적인 방식이 유리합니다만....
    /// 
    /// SI의 경우에는 좀 다릅니다. 아주 많은 Class를 만들어야 하고, 
    /// 그 속성 정의뿐 아니라 Table 만이 아니라 필요에 따라 View, Stored Procedure 등으로부터 정보를 얻어 Instance로 만들어야 할 때가 있습니다.
    /// (물론 DataTable을 직접 이용하는 것이 Microsoft가 주장하는 가장 생산성이 높은 방법입니다. - 유지보수 등에서 문제가 있습니다.)
    /// 
    /// 이 때 가장 좋은 방법은 Database 컬럼명과 Class의 속성명에 일정한 규칙을 정해 놓으면, 그 규칙에 따라 자동으로 매핑정보를 생성할 수 있습니다.
    /// 
    /// 예를 들어 PROJECT_NAME => ProjectName, PROJECT_ID => ProjectId 등으로 변경할 수 있습니다.
    /// 좀 더 특수하게는 PROJECT_ID => Id 로 변경할 수 있겠죠. (다만 다른 컬럼도 접미사로 _ID 값을 가진다면 매핑에 실패할 것입니다.)
    /// </summary>
    public static class NameMappingUtil {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        ///<summary>
        /// 변경 없이 컬럼명 그대로 속성명으로 매핑한다.
        ///</summary>
        ///<returns>아무런 변화를 주지 않는 함수 즉 컬럼명이 속성명과 같다.</returns>
        public static Func<string, string> NoChangeFunc() {
            return columnName => columnName;
        }

        /// <summary>
        /// 컬럼명에서 속성명에 들어갈 수 없는 문자나 '_' 등의 구분 문자를 제거하여, 단어의 첫문자만 대문자로 (Capitalize) 만들어 속성명으로 매핑시킨다.
        /// </summary>
        /// <returns>Pascal naming 규칙에 따라 단어단위로 첫글자만 대문자로 만드는 함수</returns>
        public static Func<string, string> CapitalizeMappingFunc() {
            return columnName => (columnName.IsNotWhiteSpace()) ? columnName.Capitalize() : string.Empty;
        }

        /// <summary>
        /// 컬럼명에서 속성명에 들어갈 수 없는 문자나 '_' 등의 구분 문자를 제거하여, 단어의 첫문자만 대문자로 (Capitalize) 만들어 속성명으로 매핑시킨다.
        /// </summary>
        /// <param name="deleteChars">제거할 문자들</param>
        /// <returns>Pascal naming 규칙에 따라 단어단위로 첫글자만 대문자로 만드는 함수</returns>
        public static Func<string, string> CapitalizeMappingFunc(params char[] deleteChars) {
            if(deleteChars == null || deleteChars.Length == 0)
                return CapitalizeMappingFunc();

            return columnName => columnName.Capitalize().DeleteCharAny(deleteChars);
        }

        /// <summary>
        /// 컬럼명에서 속성명에 들어갈 수 없는 문자나 '_' 등의 구분 문자를 제거하여, 단어의 첫문자만 대문자로 (Capitalize) 만들어 속성명으로 매핑시킨다.
        /// </summary>
        /// <param name="prefix">접두사</param>
        /// <returns>Parameter의 접두사를 제거하는 함수</returns>
        public static Func<string, string> RemovePrefixFunction(string prefix) {
            if(prefix.IsEmpty())
                return NoChangeFunc();

            return columnName => columnName.StartsWith(prefix) ? columnName.Remove(0, prefix.Length) : columnName;
        }

        /// <summary>
        /// 컬럼명=속성명 매핑함수를 이용하여 지정된 DataReader의 컬럼명을 속성명으로 매핑한다.
        /// </summary>
        /// <param name="reader">instance of IDataReader</param>
        /// <param name="mappingFunc">mapping function</param>
        /// <param name="propertyNamesToExclude">매핑에서 제외할 속성명</param>
        /// <returns>instance of <see cref="INameMap"/></returns>
        public static INameMap Mapping(this IDataReader reader, Func<string, string> mappingFunc, params string[] propertyNamesToExclude) {
            reader.ShouldNotBeNull("reader");
            mappingFunc.ShouldNotBeNull("mappingFunc");

            if(IsDebugEnabled)
                log.Debug("지정된 DataReader의 컬럼명을 속성명으로 매핑합니다...");

            var nameMap = new NameMap();
            var excludeNames = (propertyNamesToExclude != null) ? propertyNamesToExclude.ToList() : new List<string>();

            for(int i = 0; i < reader.FieldCount; i++) {
                var columnName = reader.GetName(i);

                if(columnName.IsNotWhiteSpace()) {
                    var propertyName = mappingFunc(columnName);

                    if(propertyName.IsNotWhiteSpace() && excludeNames.Contains(propertyName, StringComparer.Ordinal) == false)
                        nameMap.Add(columnName, propertyName);
                }
            }

            if(IsDebugEnabled)
                log.Debug("컬럼명-속성명 매핑 결과 = " + nameMap.CollectionToString());

            return nameMap;
        }

        /// <summary>
        /// 컬럼명=속성명 매핑함수를 이용하여 지정된 DataTable의 컬럼명을 속성명으로 매핑한다.
        /// </summary>
        /// <param name="table">instance of DataTable</param>
        /// <param name="mappingFunc">mapping function</param>
        /// <param name="propertyNamesToExclude">매핑에서 제외할 속성명</param>
        /// <returns>instance of <see cref="INameMap"/></returns>
        public static INameMap Mapping(this DataTable table, Func<string, string> mappingFunc, params string[] propertyNamesToExclude) {
            table.ShouldNotBeNull("table");
            mappingFunc.ShouldNotBeNull("mappingFunc");

            if(IsDebugEnabled)
                log.Debug("지정된 DataTable의 컬럼명을 속성명으로 매핑합니다...");

            var nameMap = new NameMap();
            var excludeNames = (propertyNamesToExclude != null) ? propertyNamesToExclude.ToList() : new List<string>();

            for(int i = 0; i < table.Columns.Count; i++) {
                var columnName = table.Columns[i].ColumnName;

                if(columnName.IsNotWhiteSpace()) {
                    var propertyName = mappingFunc(columnName);

                    if(propertyName.IsNotWhiteSpace() && excludeNames.Contains(propertyName, StringComparer.Ordinal) == false)
                        nameMap.Add(columnName, propertyName);
                }
            }

            if(IsDebugEnabled)
                log.Debug("컬럼명-속성명 매핑 결과 = " + nameMap.CollectionToString());

            return nameMap;
        }

        /// <summary>
        /// Command의 Parameter 정보와 속성 정보를 매핑한다. (ParameterName = 속성명) 형식
        /// </summary>
        /// <param name="command">instance of DbCommand to execute</param>
        /// <param name="mappingFunc">Mapping function. input = parameter name of command , result = property name of persistent object </param>
        /// <param name="propertyNamesToExclude">매핑에서 제외할 속성명</param>
        /// <returns>instance of <see cref="INameMap"/>, Key = parameter name of a specified command, Value = property name of persistent object</returns>
        public static INameMap Mapping(this DbCommand command, Func<string, string> mappingFunc, params string[] propertyNamesToExclude) {
            command.ShouldNotBeNull("command");
            mappingFunc.ShouldNotBeNull("mappingFunc");

            if(IsDebugEnabled)
                log.Debug("Command 파라미터명을 속성명으로 매핑합니다...");

            var nameMap = new NameMap();
            var excludeNames = (propertyNamesToExclude != null) ? propertyNamesToExclude.ToList() : new List<string>();

            foreach(IDataParameter parameter in command.Parameters) {
                var paramName = parameter.ParameterName.RemoveParameterPrefix();

                if(paramName.IsNotWhiteSpace()) {
                    var propertyName = mappingFunc(paramName);

                    if(propertyName.IsNotWhiteSpace() && excludeNames.Contains(propertyName, StringComparer.Ordinal) == false)
                        nameMap.Add(paramName, propertyName);
                }
            }

            if(IsDebugEnabled)
                log.Debug("Command 파라미터명-속성명 매핑 결과 = " + nameMap.CollectionToString());

            return nameMap;
        }

        /// <summary>
        /// 지정된 DataReader의 컬럼명을 속성명으로 변경한다. 매퍼는 IoC를 통해 얻는다.
        /// </summary>
        /// <param name="reader">instance of IDataReader</param>
        /// <param name="nameMapper">Mapper of column name and property name</param>
        /// <param name="propertyNamesToExclude">매핑에서 제외할 속성명</param>
        /// <returns>name mapping table</returns>
        /// <seealso cref="INameMapper"/>
        /// <seealso cref="CapitalizeNameMapper"/>
        /// <see cref="TrimNameMapper"/>
        public static INameMap NameMapping(this IDataReader reader, INameMapper nameMapper, params string[] propertyNamesToExclude) {
            return Mapping(reader, nameMapper.MapToPropertyName, propertyNamesToExclude);
        }

        /// <summary>
        /// 지정된 DataTable의 컬럼명을 속성명으로 변경한다. 매퍼는 IoC를 통해 얻는다.
        /// </summary>
        /// <param name="table">Data source</param>
        /// <param name="nameMapper">Parameter명을 속성명으로 매핑하는 Mapper</param>
        /// <param name="propertyNamesToExclude">매핑에서 제외할 속성명</param>
        /// <returns>name mapping table</returns>
        /// <seealso cref="INameMapper"/>
        /// <seealso cref="CapitalizeNameMapper"/>
        /// <see cref="TrimNameMapper"/>
        public static INameMap NameMapping(this DataTable table, INameMapper nameMapper, params string[] propertyNamesToExclude) {
            return Mapping(table, nameMapper.MapToPropertyName, propertyNamesToExclude);
        }

        /// <summary>
        /// 지정된 DbCommand의 Parameter Name을 속성명으로 변경한다. 
        /// </summary>
        /// <param name="command">실행할 DbCommand</param>
        /// <param name="nameMapper">Command Parameter명을 Persistent Object의 속성명으로 매핑하는 Mapper</param>
        /// <param name="propertyNamesToExclude">매핑에서 제외할 속성명</param>
        /// <returns>name mapping table</returns>
        /// <seealso cref="INameMapper"/>
        /// <seealso cref="CapitalizeNameMapper"/>
        /// <see cref="TrimNameMapper"/>
        public static INameMap NameMapping(this DbCommand command, INameMapper nameMapper, params string[] propertyNamesToExclude) {
            return Mapping(command, nameMapper.MapToPropertyName, propertyNamesToExclude);
        }
    }
}