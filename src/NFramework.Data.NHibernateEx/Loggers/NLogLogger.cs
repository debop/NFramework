using System;
using NLog;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NLog를 이용하는 NHibernate용 Logger
    /// </summary>
    public class NLogLogger : NHibernate.IInternalLogger {
        private readonly Logger _log;

        public NLogLogger(Logger log) {
            _log = log;
        }

        public void Error(object message) {
            _log.Error(message.AsText());
        }

        public void Error(object message, Exception exception) {
            _log.ErrorException(message.AsText(), exception);
        }

        public void ErrorFormat(string format, params object[] args) {
            _log.Error(format, args);
        }

        public void Fatal(object message) {
            _log.Fatal(message.AsText());
        }

        public void Fatal(object message, Exception exception) {
            _log.FatalException(message.AsText(), exception);
        }

        public void Debug(object message) {
            _log.Debug(message.AsText());
        }

        public void Debug(object message, Exception exception) {
            _log.DebugException(message.AsText(), exception);
        }

        public void DebugFormat(string format, params object[] args) {
            _log.Debug(format, args);
        }

        public void Info(object message) {
            _log.Info(message.AsText());
        }

        public void Info(object message, Exception exception) {
            _log.InfoException(message.AsText(), exception);
        }

        public void InfoFormat(string format, params object[] args) {
            _log.Info(format, args);
        }

        public void Warn(object message) {
            _log.Warn(message.AsText());
        }

        public void Warn(object message, Exception exception) {
            _log.WarnException(message.AsText(), exception);
        }

        public void WarnFormat(string format, params object[] args) {
            _log.Warn(format, args);
        }

        public bool IsErrorEnabled {
            get { return _log.IsErrorEnabled; }
        }

        public bool IsFatalEnabled {
            get { return _log.IsFatalEnabled; }
        }

        public bool IsDebugEnabled {
            get { return _log.IsDebugEnabled; }
        }

        public bool IsInfoEnabled {
            get { return _log.IsInfoEnabled; }
        }

        public bool IsWarnEnabled {
            get { return _log.IsWarnEnabled; }
        }
    }
}