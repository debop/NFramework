using System.Diagnostics;
using NUnit.Framework;

namespace NSoft.NFramework.WindowsSystem {
    [TestFixture]
    public class EventLoggerFixture {
        private const string CustomLogName = @"Application";

        [Test]
        public void CreateCustomEventLogTest() {
            var eventLog = new EventLogger(CustomLogName);
            Assert.IsNotNull(eventLog);
        }

        [Test]
        public void WriteTest() {
            using(var eventLog = new EventLogger(CustomLogName, EventLogger.DEFAULT_MACHINE_NAME, string.Empty)) {
                eventLog.WriteEntry("Write Test Message", EventLogEntryType.Error, 1, 0);
            }
        }

        [Test]
        public void ClearEventLogTest() {
            using(var eventLog = new EventLogger(CustomLogName))
                eventLog.ClearLog();
        }

        [Test]
        public void DeleteCustomLogTest() {
            using(var eventLog = new EventLogger(CustomLogName))
                eventLog.DeleteLog();
        }
    }
}