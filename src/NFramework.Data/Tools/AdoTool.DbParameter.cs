using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Data;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// Helper class for ADO.NET Operation
    /// </summary>
    public static partial class AdoTool {
        /// <summary>
        /// <paramref name="parameters"/> 중에 Return Value에 해당하는 Parameter의 값을 반환합니다. 없으면 null 을 반환합니다.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object GetReturnValue(this IEnumerable<IAdoParameter> parameters) {
            return
                parameters
                    .Where(p => p.Direction == ParameterDirection.ReturnValue)
                    .Select(p => p.Value)
                    .FirstOrDefault();
        }

        /// <summary>
        /// <paramref name="parameters"/> 중에 Direction이 <see cref="ParameterDirection.Input"/>을 제외한 파라미터 컬렉션을 열거합니다.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IEnumerable<IAdoParameter> GetOutputParameters(this IEnumerable<IAdoParameter> parameters) {
            return parameters.Where(p => p.Direction != ParameterDirection.Input);
        }

        /// <summary>
        /// Stored Procedure의 Return Value 값을 반환한다.
        /// </summary>
        /// <param name="db">instance of <see cref="Database"/>defined in DAAB</param>
        /// <param name="cmd">instance of DbCommand</param>
        /// <returns>return value of DbCommand, if not found return value, return null.</returns>
        public static object GetReturnValue(Database db, DbCommand cmd) {
            cmd.ShouldNotBeNull("cmd");

            return
                cmd.Parameters
                    .Cast<DbParameter>()
                    .Where(p => p.Direction == ParameterDirection.ReturnValue)
                    .Select(p => p.Value)
                    .FirstOrDefault();
        }

        /// <summary>
        /// Command의 ReturnValue Parameter의 값을 반환한다. 없다면 지정된 기본값을 반환한다.
        /// </summary>
        /// <typeparam name="T">Type of return value</typeparam>
        /// <param name="db">instance of <see cref="Database"/> defined in DAAB</param>
        /// <param name="cmd">instance of DbCommand</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>return value of DbCommand</returns>
        public static T GetReturnValue<T>(Database db, DbCommand cmd, T defaultValue) {
            cmd.ShouldNotBeNull("cmd");

            return GetReturnValue(db, cmd).AsValue(defaultValue);
        }

        /// <summary>
        /// DbCommand의 Parameter 정보를 <see cref="GetOutputParameters(System.Collections.Generic.IEnumerable{NFramework.Data.IAdoParameter})"/>를 통해 얻고, 특정 Parameter의 값을 가져옵니다.
        /// </summary>
        /// <param name="db">DAAB Database instance</param>
        /// <param name="parameters">Output Parameters</param>
        /// <param name="parameterName">Parameter 명 (Prefix를 안붙여도 됩니다)</param>
        /// <returns>Parameter에 해당하는 값</returns>
        public static object GetParameterValue(Database db, IEnumerable<IAdoParameter> parameters, string parameterName) {
            if(parameters == null)
                return null;

            return
                parameters.FirstOrDefault(p => p.Name.Compare(parameterName.RemoveParameterPrefix(), true) == 0);
        }

        /// <summary>
        /// DB마다 다른 Parameter Prefix ('?', '@', ':' 등) 을 제거한다.
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>DB별로 특성화된 Parameter Prefix가 제거된 Parameter name</returns>
        public static string RemoveParameterPrefix(this string parameterName) {
            return parameterName.IsWhiteSpace() ? parameterName : parameterName.TrimStart(PARAMETER_PREFIX);
        }

        /// <summary>
        /// 지정된 Command 의 Parameter 값을 <see cref="DBNull.Value"/> 값으로 초기화한다.
        /// </summary>
        /// <param name="db">instance of <see cref="Database"/> defined in DAAB</param>
        /// <param name="cmd">instance of DbCommand</param>
        public static void InitializeParameterValues(Database db, DbCommand cmd) {
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("Command의 Parameter들을 초기화합니다...");

            cmd.Parameters
                .Cast<IDataParameter>()
                .RunEach(p => db.SetParameterValue(cmd, p.ParameterName, DBNull.Value));
        }

        /// <summary>
        /// <see cref="DbCommand"/>의 Parameter에 값을 설정합니다.
        /// </summary>
        /// <param name="db">instance of <see cref="Database"/> defined in DAAB</param>
        /// <param name="cmd">instance of DbCommand</param>
        /// <param name="adoParameters">Command Parameter에 설정할 정보</param>
        public static void SetParameterValues(Database db, DbCommand cmd, params IAdoParameter[] adoParameters) {
            if(adoParameters.IsEmptySequence())
                return;

            // NOTE: output parameter 등의 값을 초기화 해주어야 한다.
            //
            InitializeParameterValues(db, cmd);

            var cmdParamNames = cmd.Parameters.Cast<IDataParameter>().Select(dp => dp.ParameterName.RemoveParameterPrefix()).ToList();

            foreach(var p in adoParameters) {
                var pName = cmdParamNames.FirstOrDefault(cpn => cpn.Compare(p.Name, true) == 0);

                if(pName.IsNotWhiteSpace()) {
                    db.SetParameterValue(cmd, pName, p.Value);

                    if(IsDebugEnabled)
                        log.Debug("DbCommand의 파라미터 값을 설정했습니다. parameter=[{0}]", p);
                }
                else {
                    try {
                        if(p.Value != null && p.ValueType == DbType.Object)
                            p.ValueType = DbFunc.GetDbType(p.Value.GetType());

                        if(p.Size.HasValue)
                            db.AddParameter(cmd, p.Name, p.ValueType, p.Size.Value, p.Direction, true, 0, 0, p.SourceColumn,
                                            p.SourceVersion, p.Value);
                        else
                            db.AddParameter(cmd, p.Name, p.ValueType, p.Direction, p.SourceColumn, p.SourceVersion, p.Value);
                    }
                    catch {
                        // NOTE: SQLite에서는 Direction을 지정하지 못합니다.
                        //
                        db.AddInParameter(cmd, p.Name, p.ValueType, p.Value);
                    }

                    if(IsDebugEnabled)
                        log.Debug("Command에 파라미터를 추가했습니다. parameter=[{0}]", p);
                }
            }
        }

        /// <summary>
        /// <see cref="ConvertAll{TEntity}(IDataReader,INameMap)"/>와는 반대로, 
        /// Persistent Object의 속성정보를 DbCommand Parameter Value로 설정한다.
        /// </summary>
        /// <param name="db">instance of <see cref="Database"/> defined in DAAB</param>
        /// <param name="cmd">instance of DbCommand</param>
        /// <param name="entity">Command로 실행할 Persistent object</param>
        /// <param name="nameMaps">DbCommand 인자명 : Persistent Object 속성명의 매핑 정보</param>
        /// <param name="ignoreCase">컬럼명과 속성명의 대소문자 구분없이 매칭합니다.</param>
        public static void SetParameterValues<T>(this Database db, DbCommand cmd, T entity, INameMap nameMaps, bool ignoreCase = true) {
            cmd.ShouldNotBeNull("cmd");
            entity.ShouldNotBeNull("entity");
            nameMaps.ShouldNotBeNull("nameMaps");

            var accessor = DynamicAccessorFactory.CreateDynamicAccessor<T>(true);
            var propertyNames = accessor.GetPropertyNames();

            if(IsDebugEnabled)
                log.Debug("대상 Entity의 속성명 정보=[{0}]", propertyNames.CollectionToString());

            var parameters = new List<IAdoParameter>();

            foreach(string paramName in nameMaps.Keys) {
                // remove parameter prefix
                var pName = paramName.RemoveParameterPrefix();

                var propertyName = propertyNames.FirstOrDefault(pn => pn.EqualTo(nameMaps[pName], ignoreCase));

                if(propertyName.IsNotWhiteSpace())
                    parameters.Add(new AdoParameter(pName, accessor.GetPropertyValue(entity, propertyName)));
            }

            SetParameterValues(db, cmd, parameters.ToArray());
        }

        /// <summary>
        /// Command 실행 후 Parameter 중에 ParameterDirection이 Input 이 아닌, InputOutput, Output, ReturnValue에 해당하는 Parameter 정보들만 따로 받는다.
        /// </summary>
        /// <param name="db">instance of <see cref="Database"/> defined in DAAB</param>
        /// <param name="cmd">instance of DbCommand</param>
        /// <returns>DbCommand의 Paramter 중에 ParameterDirection이 Input 이 아닌 Parameter의 정보</returns>
        public static IAdoParameter[] GetOutputParameters(Database db, DbCommand cmd) {
            cmd.ShouldNotBeNull("cmd");

            return
                cmd.Parameters
                    .Cast<DbParameter>()
                    .Where(p => p.Direction != ParameterDirection.Input)
                    .Select(p => new AdoParameter(p.ParameterName, p.Value, p.DbType, p.Direction))
                    .ToArray<IAdoParameter>();
        }

        /// <summary>
        /// Command Parameters의 이름 중에 접두사('@', ':' 등)을 제거하고 대문자로 변환하여 가져온다.
        /// </summary>
        /// <param name="cmd">instance of DbCommand</param>
        /// <returns>Collection of parameter name that defined in the specified command.</returns>
        public static IList<string> GetParameterNames(DbCommand cmd) {
            cmd.ShouldNotBeNull("cmd");

            return
                cmd.Parameters
                    .Cast<IDataParameter>()
                    .Select(p => p.ParameterName.RemoveParameterPrefix().ToUpper())
                    .ToList();
        }

        /// <summary>
        /// ASP.NET Raw 형태의 Command에 Parameter를 빌드한다. RDBMS별 Parameter 접두사에 대한 관리를 해주지 않습니다.
        /// </summary>
        /// <param name="cmd">instance of DbCommand</param>
        /// <param name="parameters">DbCommand Parameter에 설정할 정보</param>
        public static void CreateCommandParameters(DbCommand cmd, params INamedParameter[] parameters) {
            if(parameters == null)
                return;

            foreach(var p in parameters) {
                if(cmd.Parameters.Contains(p.Name)) {
                    cmd.Parameters[p.Name].Value = p.Value;
                }
                else {
                    IDbDataParameter arg = cmd.CreateParameter();
                    arg.ParameterName = p.Name;
                    arg.Value = p.Value;

                    if(p is IAdoParameter)
                        arg.DbType = ((IAdoParameter)p).ValueType;

                    cmd.Parameters.Add(arg);
                }
            }
        }

        /// <summary>
        /// 지정된 <see cref="IDataReader"/>의 전체 크기를 구한다. Count를 구한 후에는 reader를 사용할 수 없고, Disposing을 꼭 해주어야 한다.
        /// </summary>
        /// <param name="reader">Data Source</param>
        /// <returns>Record 갯수</returns>
        /// <example>
        /// <code>
        ///		long count = 0;
        ///		using(IDataReader reader = AdoRepository.ExecuteReader("SELECT * FROM sysobjects"))
        ///			count = reader.LongCount();
        /// 
        ///		
        ///		// 앞 5개, 뒤 5개를 뺀 중간 값들...
        ///     using(IDataReader reader = AdoRepository.ExecuteReader("SELECT * FROM sysobjects"))
        ///		{
        ///			var sysObjects = AdoTool.ConvertAll&lt;SysObject&gt;(reader, reader.NameMapping(), 5, count-5);
        ///		}
        /// </code>
        /// </example>
        public static int Count(this IDataReader reader) {
            reader.ShouldNotBeNull("reader");

            var count = 0;

            while(reader.Read())
                count++;

            if(IsDebugEnabled)
                log.Debug("Count of record in a specified IDataReader is [{0}]", count);

            return count;
        }
    }
}