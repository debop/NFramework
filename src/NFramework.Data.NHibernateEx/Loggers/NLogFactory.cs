using System;
using System.Collections.Concurrent;
using NHibernate;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NHibernate의 로그 작업을 수행할 NLog logger를 생성하는 Factory입니다. <br/>
    /// 참고: http://nhlogging.codeplex.com/
    /// 참고: http://jfromaniello.blogspot.com/2011/05/nhibernatenlog-support-in-nuget.html?utm_source=feedburner&utm_medium=feed&utm_campaign=Feed%3A+JoseFRomaniello+%28Jos%C3%A9+F.+Romaniello%29 <br/>
    /// 참고: https://bitbucket.org/jfromaniello/nhibernate.nlog/src/142abf128c68/pack/Content/NLog-NHibernate.cs.pp <br/>
    /// </summary>
    public class NLogFactory : ILoggerFactory {
        private static readonly ConcurrentDictionary<string, IInternalLogger> _loggers =
            new ConcurrentDictionary<string, IInternalLogger>();

        public IInternalLogger LoggerFor(string keyName) {
            keyName.ShouldNotBeWhiteSpace("keyName");

            return _loggers.GetOrAdd(keyName, (key) => new NLogLogger(NLog.LogManager.GetLogger(key)));
        }

        public IInternalLogger LoggerFor(Type type) {
            type.ShouldNotBeNull("type");
            return LoggerFor(type.FullName);
        }
    }
}