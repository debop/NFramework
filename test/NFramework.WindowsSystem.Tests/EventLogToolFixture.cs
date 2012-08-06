using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace NSoft.NFramework.WindowsSystem {
    [TestFixture]
    public class EventLogToolFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private const string EventLogName = "Application";

        private static readonly string EventLogSource =
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetName().Name;

        [Test]
        public void WriteEntryTest() {
            EventLogTool.WriteEntry(EventLogName,
                                    EventLogSource,
                                    "EventLog Information Message Test",
                                    EventLogEntryType.Information);

            EventLogTool.WriteEntry(EventLogName,
                                    EventLogSource,
                                    "EventLog Error Message Test",
                                    EventLogEntryType.Error);
        }

        [Test]
        public void FindEntryBySourceTest() {
            using(var eventLog = EventLogTool.GetEventLog(EventLogName)) {
                var entries = EventLogTool.FindEntryBySource(eventLog, EventLogSource, true);
                DisplayEntries(entries, "FindLogEntryBySource");
            }
        }

        [Test]
        public void FindEntryByEntryTypeTest() {
            using(var eventLog = EventLogTool.GetEventLog(EventLogName)) {
                DisplayEntries(EventLogTool.FindEntryByEntryType(eventLog, EventLogEntryType.Error),
                               "FindEntryByEntryType");
            }
        }

        [Test]
        public void FindEntryByGeneratedTimeTest() {
            using(var eventLog = EventLogTool.GetEventLog(EventLogName)) {
                DisplayEntries(EventLogTool.FindEntryByGeneratedTime(eventLog, DateTime.Now.AddMinutes(-5), false),
                               "FindEntryByGeneratedTime");
            }
        }

        private static void DisplayEntries(IEnumerable<EventLogEntry> entries, string title) {
            Console.WriteLine();
            Console.WriteLine(title);

            if(entries == null || entries.Count() == 0)
                Console.WriteLine("EventLogEntry is empty.");
            else
                foreach(EventLogEntry entry in entries)
                    Console.WriteLine("{0}:{1}:{2}:{3}", entry.Index, entry.Source, entry.Message, entry.EntryType);

            Console.WriteLine();
        }
    }
}