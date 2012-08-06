using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using NSoft.NFramework.Data;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// 실제 Database에 접근해서 StringResource 정보를 가져오는 Repository입니다.
    /// </summary>
    public class AdoResourceRepository {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 성능을 높히기 위해 모든 파라미터의 Length를 255로 했음
        /// </summary>
        public const int ParameterLength = 255;

        /// <summary>
        /// 모든 Resource 정보를 가져오는 SQL 문
        /// </summary>
        public static readonly string GetResourcesSql =
            @"SELECT ResourceKey, ResourceValue FROM {0} " +
            @"WHERE AssemblyName=@AssemblyName AND ResourceName=@ResourceName AND LocaleKey=@LocaleKey";

        /// <summary>
        /// 특정 리소스 키에 해당하는 리소스 정보를 가져오는  SQL 문
        /// </summary>
        public static readonly string GetResourceSql = GetResourcesSql + @" AND ResourceKey=@ResourceKey";

        /// <summary>
        /// 생성자 
        /// </summary>
        /// <param name="databaseName">String Resource 정보가 있는 Database의 connection string name</param>
        /// <param name="tableName">String Resources 정보를 담은 Table 명</param>
        /// <param name="assemblyName">대분류</param>
        /// <param name="resourceName">String Resources의 중분류</param>
        public AdoResourceRepository(string resourceName = null, string assemblyName = null, string databaseName = null,
                                     string tableName = null) {
            if(IsDebugEnabled)
                log.Debug("AdoResourceRepository 인스턴스 생성. databaseName=[{0}], tableName=[{1}], assemblyName=[{2}], resourceName=[{3}]",
                          databaseName, tableName, assemblyName, resourceName);

            DatabaseName = databaseName ?? AdoTool.DefaultDatabaseName;
            TableName = tableName ?? AdoResourceProvider.DefaultStringResourceTableName;
            AssemblyName = assemblyName ?? AdoResourceProvider.DefaultAssemblyName;
            ResourceName = resourceName ?? string.Empty;

            Ado = AdoRepositoryFactory.Instance.CreateRepository(DatabaseName);
        }

        /// <summary>
        /// 생성자 
        /// </summary>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="tableName">String Resources 정보를 담은 Table 명</param>
        /// <param name="assemblyName">대분류</param>
        /// <param name="resourceName">String Resources의 중분류</param>
        public AdoResourceRepository(IAdoRepository repository, string resourceName = null, string assemblyName = null,
                                     string tableName = null) {
            repository.ShouldNotBeNull("repository");

            if(IsDebugEnabled)
                log.Debug("AdoResourceRepository 인스턴스 생성. databaseName=[{0}], tableName=[{1}], assemblyName=[{2}], resourceName=[{3}]",
                          repository.DbName, tableName, assemblyName, resourceName);

            DatabaseName = repository.DbName ?? AdoTool.DefaultDatabaseName;
            TableName = tableName ?? AdoResourceProvider.DefaultStringResourceTableName;
            AssemblyName = assemblyName ?? AdoResourceProvider.DefaultAssemblyName;
            ResourceName = resourceName ?? string.Empty;

            Ado = repository;
        }

        /// <summary>
        /// String Resource 정보가 있는 Database의 connection string name
        /// </summary>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// String Resources 정보를 담은 Table
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// Assembly Name
        /// </summary>
        public string AssemblyName { get; private set; }

        /// <summary>
        /// String Resources의 대분류(Section)
        /// </summary>
        public string ResourceName { get; private set; }

        /// <summary>
        /// AdoRepository for <see cref="DatabaseName"/> 
        /// </summary>
        public IAdoRepository Ado { get; set; }

        /// <summary>
        /// 지정한 문화권의 모든 리소스 정보를 Hashtable로 반환한다.
        /// </summary>
        public Hashtable GetResources(CultureInfo culture) {
            if(IsDebugEnabled)
                log.Debug("Get resources map... resourceType=[{0}], culture=[{1}]", ResourceName, culture);

            var resourceMap = new Hashtable();
            try {
                culture = culture.GetOrCurrentCulture();

                using(var cmd = GetResourcesCommand(culture.Name))
                using(var reader = Ado.ExecuteReader(cmd)) {
                    while(reader.Read()) {
                        var key = reader.AsString("ResourceKey");
                        var value = reader.AsString("ResourceValue");

                        resourceMap.Add(key, value);
                    }
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("리소스를 얻는데 실패했습니다. culture=[{0}]", culture);
                    log.Error(ex);
                }

                throw;
            }
            return resourceMap;
        }

        /// <summary>
        /// 리소스 값 얻기
        /// </summary>
        public string GetResource(CultureInfo culture, string resourceKey) {
            if(IsDebugEnabled)
                log.Debug("리소스 얻기... resourceType=[{0}], culture=[{1}], resourceKey=[{2}]", ResourceName, culture, resourceKey);

            string resourceValue;

            try {
                culture = culture.GetCulture();

                // 지정된 문화권에 해당 키가 정의되어 있지 않다면, 상위 문화권에서 찾는다.
                //
                resourceValue = GetResourceInternal(culture, resourceKey);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("리소스를 얻는데 실패했습니다. 예외를 발생시키지는 않고, 빈 문자열을 반환합니다. culture=[{0}], resourceKey=[{1}", culture, resourceKey);
                    log.Warn(ex);
                }
                resourceValue = string.Empty;
            }

            if(IsDebugEnabled)
                log.Debug("리소스를 얻었습니다. resourceType=[{0}], culture=[{1}], resourceKey=[{2}], resourceValue=[{3}]",
                          ResourceName, culture, resourceKey, resourceValue);

            return resourceValue;
        }

        /// <summary>
        /// 내부적으로 문화권에 맞는 리소스 정보를 가져옵니다.
        /// </summary>
        private string GetResourceInternal(CultureInfo culture, string resourceKey) {
            var resourceValues = new List<string>();
            var result = string.Empty;

            if(culture.IsNullCulture())
                return result;

            if(IsDebugEnabled)
                log.Debug("문화권에 맞는 리소스 정보를 가져옵니다... culture=[{0}], resourceKey=[{1}]", culture, resourceKey);

            try {
                using(var cmd = GetResourceCommand(culture.Name, resourceKey))
                using(var reader = Ado.ExecuteReader(cmd)) {
                    while(reader.Read())
                        resourceValues.Add(reader.AsString("ResourceValue"));
                }
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("Database에서 Resource정보를 얻는데 실패했습니다. 빈 문자열을 반환합니다. culture=[{0}], resourceKey=[{1}]", culture, resourceKey);
                    log.Warn(ex);
                }
            }

            if(resourceValues.Count == 0) {
                result = GetResourceInternal(culture.Parent, resourceKey);
            }
            else if(resourceValues.Count == 1) {
                result = resourceValues[0];
            }
            else {
                throw new InvalidOperationException("Duplicated values in resourceKey=" + resourceKey);
            }

            return result;
        }

        /// <summary>
        /// <see cref="ResourceName"/>에 해당하는 리소스 중에 <paramref name="localeKey"/>, <paramref name="resourceKey"/>에 
        /// 해당하는 리소스 정보를 조회하는 <see cref="DbCommand"/>를 생성합니다.
        /// </summary>
        /// <param name="localeKey">지역화 정보( <see cref="CultureInfo.Name"/>)</param>
        /// <param name="resourceKey">Resource Key to retrive</param>
        private DbCommand GetResourceCommand(string localeKey, string resourceKey) {
            var sql = string.Format(GetResourceSql, TableName);

            return Ado.GetSqlStringCommand(sql,
                                           AssemblyNameParameter,
                                           ResourceNameParameter,
                                           new AdoParameter("LocaleKey", localeKey),
                                           new AdoParameter("ResourceKey", resourceKey));
        }

        /// <summary>
        /// <see cref="ResourceName"/>에 해당하는 리소스 중에 <paramref name="localeKey"/>에 
        /// 해당하는 모든 리소스 정보를 조회하는 <see cref="DbCommand"/>를 생성합니다.
        /// </summary>
        /// <param name="localeKey">지역화 정보( <see cref="CultureInfo.Name"/>)</param>
        /// <returns></returns>
        private DbCommand GetResourcesCommand(string localeKey) {
            var sql = string.Format(GetResourcesSql, TableName);

            return Ado.GetSqlStringCommand(sql,
                                           AssemblyNameParameter,
                                           ResourceNameParameter,
                                           new AdoParameter("LocaleKey", localeKey));
        }

        private IAdoParameter _assemblyNameParameter;

        /// <summary>
        /// AssemblyName에 대한 AdoParameter 인스턴스
        /// </summary>
        protected IAdoParameter AssemblyNameParameter {
            get { return _assemblyNameParameter ?? (_assemblyNameParameter = new AdoParameter("AssemblyName", AssemblyName)); }
        }

        private IAdoParameter _resourceNameParameter;

        /// <summary>
        /// ResourceName에 대한 AdoParameter 인스턴스
        /// </summary>
        protected IAdoParameter ResourceNameParameter {
            get { return _resourceNameParameter ?? (_resourceNameParameter = new AdoParameter("ResourceName", ResourceName)); }
        }
    }
}