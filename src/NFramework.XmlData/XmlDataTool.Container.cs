using System;
using NSoft.NFramework.Data;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.XmlData.Messages;

namespace NSoft.NFramework.XmlData {
    public static partial class XmlDataTool {
        /// <summary>
        /// 지정된 DB에 대해 작업을 수행하는 <see cref="IXmlDataManagerAdapter"/>를 Container로부터 Resolve 합니다.
        /// Component 명은 <see cref="XmlDataManagerAdapterPrefix"/>.<see cref="AdoTool.DefaultDatabaseName"/> 형식입니다.
        /// </summary>
        /// <returns><see cref="IXmlDataManagerAdapter"/> 인스턴스</returns>
        public static IXmlDataManagerAdapter ResolveXmlDataManagerAdapter() {
            return ResolveXmlDataManagerAdapter(AdoTool.DefaultDatabaseName);
        }

        /// <summary>
        /// 지정된 DB에 대해 작업을 수행하는 <see cref="IXmlDataManagerAdapter"/>를 Container로부터 Resolve 합니다.
        /// Component 명은 <see cref="XmlDataManagerAdapterPrefix"/>.<paramref name="dbName"/> 형식입니다.
        /// </summary>
        /// <param name="dbName">Data 요청을 처리할 DB ConnectionString Name</param>
        /// <returns><see cref="IXmlDataManagerAdapter"/> 인스턴스</returns>
        public static IXmlDataManagerAdapter ResolveXmlDataManagerAdapter(string dbName) {
            var componentId = XmlDataManagerAdapterPrefix + "." + dbName.GetDatabaseName();

            if(IsDebugEnabled)
                log.Debug("IoC로부터 XmlDataManagerAdapter 컴포넌트를 Resolve합니다. dbName=[{0}], componentId=[{1}]", dbName, componentId);

            try {
                return IoC.ResolveOrDefault<IXmlDataManagerAdapter>(componentId);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("XmlDataManagerAdapter 컴포넌트를 IoC 로부터 Resolve하는데 실패했습니다.", ex);
                throw;
            }
        }

        public static IXmlDataManager ResolveXmlDataManager() {
            return ResolveXmlDataManager(AdoTool.DefaultDatabaseName);
        }

        /// <summary>
        /// 지정된 Database Source에 요청 정보의 SQL 문을 실행하는 <see cref="XmlDataManager"/>를 생성한다.
        /// </summary>
        /// <param name="dbName">Database ConnectionString Name</param>
        /// <returns>IoC를 통해 <see cref="IXmlDataManager"/>를 구현한 클래스의 인스턴트스를 반환합니다. IoC에 정의되어 있지 않다면 <see cref="XmlDataManager"/>를 반환합니다.</returns>
        public static IXmlDataManager ResolveXmlDataManager(string dbName) {
            try {
                var componentId = XmlDataManagerPrefix + "." + dbName;
                return IoC.ResolveOrDefault<IXmlDataManager>(componentId);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("IoC 환경설정에 IXmlDataManager service가 정의되어 있지 않습니다. 시스템 기본 XmlDataManager를 생성합니다.", ex);

                return new XmlDataManager(ResolveAdoRepository(dbName));
            }
        }

        /// <summary>
        /// DB별 AdoRepository를 Resolve 합니다.
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static IAdoRepository ResolveAdoRepository(string dbName) {
            var componentId = AdoRepositoryPrefix + "." + dbName.GetDatabaseName();
            return IoC.ResolveOrDefault<IAdoRepository>(componentId);
        }

        /// <summary>
        /// <see cref="ISerializer{XdsRequestDocument}"/>를 생성합니다.
        /// </summary>
        /// <returns></returns>
        public static ISerializer<XdsRequestDocument> ResolveRequestSerializer() {
            return ResolveRequestSerializer(AdoTool.DefaultDatabaseName);
        }

        /// <summary>
        /// <see cref="ISerializer{XdsRequestDocument}"/>를 생성합니다.
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static ISerializer<XdsRequestDocument> ResolveRequestSerializer(string dbName) {
            var componentId = MessageSerializerPrefix + "." + dbName.GetDatabaseName();
            return IoC.ResolveOrDefault<ISerializer<XdsRequestDocument>>(componentId);
        }

        /// <summary>
        /// <see cref="ISerializer{XdsResponseDocument}"/>를 생성합니다.
        /// </summary>
        /// <returns></returns>
        public static ISerializer<XdsResponseDocument> ResolveResponseSerializer() {
            return ResolveResponseSerializer(AdoTool.DefaultDatabaseName);
        }

        /// <summary>
        /// <see cref="ISerializer{XdsResponseDocument}"/>를 생성합니다.
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static ISerializer<XdsResponseDocument> ResolveResponseSerializer(string dbName) {
            var componentId = MessageSerializerPrefix + "." + dbName.GetDatabaseName();
            return IoC.ResolveOrDefault<ISerializer<XdsResponseDocument>>(componentId);
        }

        /// <summary>
        /// <paramref name="xmlDataManager"/>의 주요 속성에 대한 유효성 검사를 수행합니다. 유효성을 만족하지 못하면, <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        public static void AssertXmlDataManager(this IXmlDataManager xmlDataManager) {
            xmlDataManager.ShouldNotBeNull("xmlDataManager");

            if(xmlDataManager.Ado == null)
                throw new InvalidOperationException("DataService의 AdoRepository 속성 값이 NULL입니다.");

            if(xmlDataManager.Ado.QueryProvider == null)
                throw new InvalidOperationException("XmlDataManager.AdoRepository의 QueryProvider가 NULL입니다.");
        }
    }
}