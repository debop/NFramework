using System;
using System.IO;
using System.Reflection;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// SqlCe Database 용 Helper Class
    /// </summary>
    public static class SqlCeTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Fullname of SqlCe Database Engine Type.
        /// </summary>
        public const string EngineTypeName = "System.Data.SqlServerCe.SqlCeEngine, System.Data.SqlServerCe";

        private static Type _type;
        private static PropertyInfo _localConnectionString;
        private static MethodInfo _createDatabase;

        /// <summary>
        /// SqlServerCe 용 Database 파일을 만듭니다.
        /// </summary>
        /// <param name="filename">SqlCe Database 파일의 전체 경로</param>
        public static void CreateDatabaseFile(string filename) {
            if(IsDebugEnabled)
                log.Debug("SqlCe 용 Database 파일을 생성합니다... filename=[{0}]", filename);

            if(File.Exists(filename)) {
                try {
                    File.Delete(filename);
                }
                catch(Exception ex) {
                    if(log.IsWarnEnabled) {
                        log.Warn("파일이 이미 존재합니다. 이 파일을 재사용합니다. filename=[{0}]" + filename);
                        log.Warn(ex);
                    }

                    return;
                }
            }

            if(_type == null) {
                _type = Type.GetType(EngineTypeName);
                if(_type == null)
                    throw new InvalidOperationException("System.Data.SqlServerCe 어셈블리를 로드할 수 없습니다. 어플리케이션 경로에 있는지 확인해 주세요");

                _localConnectionString = _type.GetProperty("LocalConnectionString");
                _createDatabase = _type.GetMethod("CreateDatabase");
            }


            var engine = _type.CreateInstance(); //Activator.CreateInstance(_type);
            _localConnectionString.SetValue(engine, string.Format("Data Source='{0}';", filename), null);
            _createDatabase.Invoke(engine, new object[0]);

            if(IsDebugEnabled)
                log.Debug("SqlCE 용 Database 파일을 생성했습니다. filename=[{0}]", filename);
        }
    }
}