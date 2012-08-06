using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions;
using NSoft.NFramework.IO;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// FluentNHibernate 라이브러리를 이용하여, 매핑을 수행하는 UnitOfWorkFactory 입니다.
    /// </summary>
    public class FluentNHUnitOfWorkFactory : NHUnitOfWorkFactory {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        public FluentNHUnitOfWorkFactory() {}

        public FluentNHUnitOfWorkFactory(string cfgFilename) : base(cfgFilename) {
            if(IsInfoEnabled)
                log.Info("FluentNHibernate를 이용하는 FluentNHUnitOfWorkFactory를 생성합니다.");
        }

        public FluentNHUnitOfWorkFactory(Assembly[] assembilies) : base(assembilies) {}

        public FluentNHUnitOfWorkFactory(Assembly[] assemblies, string cfgFilename) : base(assemblies, cfgFilename) {}

        public FluentNHUnitOfWorkFactory(string[] assemblyNames) : base(assemblyNames) {}

        public FluentNHUnitOfWorkFactory(string[] assemblyNames, string cfgFilename) : base(assemblyNames, cfgFilename) {}

        /// <summary>
        /// FluentNHibernate Convention 입니다.
        /// </summary>
        public IConvention Convention { get; set; }

        /// <summary>
        /// configure with specified nhibernate configuration file.
        /// </summary>
        /// <param name="configFilePath">physical path of nhibernate configuration file</param>
        /// <returns></returns>
        protected override NHibernate.Cfg.Configuration ConfigureCfgFile(string configFilePath) {
            configFilePath = FileTool.GetPhysicalPath(configFilePath);

            if(File.Exists(configFilePath) == false)
                throw new FileNotFoundException("NHibernate 환경설정 파일을 찾을 수 없습니다. configFilePath=" + configFilePath);

            if(log.IsInfoEnabled)
                log.Info("NHibernate Configuration 빌드를 시작합니다... configFilePath=[{0}]", configFilePath);

            var cfg = new NHibernate.Cfg.Configuration();

            var conventions = new List<IConvention>();

            if(Convention != null)
                conventions.Add(Convention);

            try {
                cfg.Configure(configFilePath);

                var fluentCfg = Fluently.Configure(cfg);

                foreach(var asm in cfg.GetMappingAssemblies()) {
                    var mapAsm = asm;

                    fluentCfg.Mappings(m => {
                                           var container = m.FluentMappings.AddFromAssembly(mapAsm);
                                           if(conventions.Count > 0)
                                               container.Conventions.Add(conventions.ToArray());
                                       });
                    fluentCfg.Mappings(m => m.HbmMappings.AddFromAssembly(mapAsm));
                }

                // Configuration 파일에서 정의한 Mapping Assembly 외에 
                // IoC를 통해 NHUnitOfWorkFactory에 할당된 추가 Assembly들도 매핑이 가능하게 열어준다.
                //
                if(_assemblies != null && _assemblies.Length > 0) {
                    var loadedAssemblies = cfg.GetMappingAssemblies().ToList();
                    var assemblies = _assemblies.Where(asm => loadedAssemblies.Contains(asm) == false).ToList();
                    fluentCfg.Mappings(m => {
                                           assemblies.ForEach(asm => {
                                                                  var container = m.FluentMappings.AddFromAssembly(asm);
                                                                  if(conventions.Count > 0)
                                                                      container.Conventions.Add(conventions.ToArray());
                                                              });
                                           assemblies.ForEach(asm => m.HbmMappings.AddFromAssembly(asm));
                                       });
                }

                cfg = fluentCfg.BuildConfiguration();
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("NHibernate 환경설정에 실패했습니다. configFilePath=[{0}]", configFilePath);
                    log.Error(ex);
                }

                throw;
            }

            if(log.IsInfoEnabled)
                log.Info("NHibernate Configuration 빌드를 완료했습니다!!! configFilePath=[{0}]", configFilePath);

            return cfg;
        }
    }
}