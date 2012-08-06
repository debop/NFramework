using System;
using System.Data;
using System.Linq;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.Persisters {
    /// <summary>
    /// DataReader 정보로 부터 Persistent object를 빌드하는 Persister입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReaderPersister<T> : AdoPersisterBase<IDataReader, T>, IReaderPersister<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        public ReaderPersister() : this(new TrimNameMapper(), ActivatorTool.CreateInstance<T>) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="nameMapper">name mapper</param>
        /// <param name="persistentFactory">Persistent object 생성 delegate</param>
        public ReaderPersister(INameMapper nameMapper, Func<T> persistentFactory = null) : base(nameMapper, persistentFactory) {}

        /// <summary>
        /// DataSource로부터 새로운 Persistent object를 빌드합니다.
        /// </summary>
        /// <param name="dataReader">데이타 소스</param>
        /// <returns>Persistent object</returns>
        public override T Persist(IDataReader dataReader) {
            dataReader.ShouldNotBeNull("dataReader");

            if(IsDebugEnabled)
                log.Debug("Build Persistent object of Type [{0}] from IDataReader... Name Mapping method=[{1}]",
                          typeof(T).FullName, NameMapper.GetType().FullName);

            var persistent = FactoryFunction();
            var columnNames = Enumerable.Range(0, dataReader.FieldCount).Select<int, string>(dataReader.GetName).ToArray();

            foreach(var columnName in columnNames) {
                var propertyName = NameMapper.MapToPropertyName(columnName);

                if(propertyName.IsNotWhiteSpace()) {
                    var columnValue = dataReader.AsValue(columnName);

                    if(IsDebugEnabled)
                        log.Debug("속성명 [{0}]에 DataReader 컬럼명 [{1}]의 값 [{2}]를 설정합니다...", propertyName, columnName, columnValue);

                    DynamicAccessor.SetPropertyValue(persistent, propertyName, columnValue);
                }
            }
            return persistent;
        }
    }
}