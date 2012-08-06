using System;
using NUnit.Framework;

namespace NSoft.NFramework.Collections {
    [TestFixture]
    public class FrequencyTableFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [Test]
        public void FrequencyTableTest() {
            /* Test the Add command and the GetTableAsArray command with strings */
            var table = new FrequencyTable<string>();

            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("B");
            table.Add("A");
            table.Add("A");
            table.Add("A");
            table.Add("A");
            table.Add("B");
            table.Add("B");
            table.Add("A");
            table.Add("C");
            table.Add("C");
            table.Add("C");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("r");
            table.Add("t");
            table.Add("t");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("r");
            table.Add("Z");
            table.Add("Z");
            table.Add("C");
            table.Add("C");
            table.Add("C");
            table.Add("t");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("Z");
            table.Add("t");
            table.Add("C");
            table.Add("d");
            table.Add("Z");
            table.Add("Z");
            table.Add("C");
            table.Add("C");
            table.Add("C");
            table.Add("e");
            table.Add("e");
            table.Add("e");
            table.Add("C");
            table.Add("C");
            table.Add("C");
            table.Add("C");
            table.Add("C");
            table.Add("C");
            table.Add("e");
            table.Add("C");
            table.Add("C");
            var r = table.GetTableAsArray(FrequencyTableSortOrder.None);
            var r1 = table.GetTableAsArray(FrequencyTableSortOrder.Value_Ascending);
            var r2 = table.GetTableAsArray(FrequencyTableSortOrder.Value_Descending);
            var r3 = table.GetTableAsArray(FrequencyTableSortOrder.Frequency_Ascending);
            var r4 = table.GetTableAsArray(FrequencyTableSortOrder.Frequency_Descending);

            Console.WriteLine("Table unsorted:");
            Console.WriteLine("***************************************************");
            foreach(FrequencyTableEntry<string> f in r) {
                log.Debug("{0}  {1}   {2}   {3}",
                          f.Value, f.AbsoluteFrequency, f.RelativeFrequency, Math.Round(f.Percentage, 2));
            }
            Console.WriteLine("***************************************************");
            Console.WriteLine("");
            Console.WriteLine("Table sorted by value - ascending:");
            Console.WriteLine("***************************************************");
            foreach(FrequencyTableEntry<string> f in r1) {
                log.Debug("{0}  {1}   {2}   {3}", f.Value, f.AbsoluteFrequency, f.RelativeFrequency,
                          Math.Round(f.Percentage, 2));
            }
            Console.WriteLine("***************************************************");
            Console.WriteLine("");
            Console.WriteLine("Table sorted by value - descending:");
            Console.WriteLine("***************************************************");
            foreach(FrequencyTableEntry<string> f in r2) {
                log.Debug("{0}  {1}   {2}   {3}", f.Value, f.AbsoluteFrequency, f.RelativeFrequency,
                          Math.Round(f.Percentage, 2));
            }
            Console.WriteLine("***************************************************");
            Console.WriteLine("");
            Console.WriteLine("Table sorted by frequency - ascending:");
            Console.WriteLine("***************************************************");
            foreach(FrequencyTableEntry<string> f in r3) {
                log.Debug("{0}  {1}   {2}   {3}", f.Value, f.AbsoluteFrequency, f.RelativeFrequency,
                          Math.Round(f.Percentage, 2));
            }
            Console.WriteLine("***************************************************");
            log.Debug("Scarcest Value:\t{0}\tFrequency: {1}", table.ScarcestValue, table.SmallestFrequency);
            log.Debug("Mode:\t\t{0}\tFrequency: {1}", table.Mode, table.HighestFrequency);
            Console.WriteLine("");
            Console.WriteLine("Table sorted by frequency - descending:");
            Console.WriteLine("***************************************************");
            foreach(FrequencyTableEntry<string> f in r4) {
                log.Debug("{0}  {1}   {2}   {3}", f.Value, f.AbsoluteFrequency, f.RelativeFrequency,
                          Math.Round(f.Percentage, 2));
            }
            Console.WriteLine("***************************************************");
            Console.WriteLine("");
            /* now test the class with integers
			 * initialize a new frequency table using an integer array*/
            var test = new[] { 1, 1, 1, 2, 3, 3, 2, 2, 1, 1, 1, 2, 3, 4, 23, 1, 1, 1 };
            var table1 = new FrequencyTable<int>(test);
            Console.WriteLine("");
            Console.WriteLine("Integer table unsorted:");
            Console.WriteLine("***************************************************");
            foreach(FrequencyTableEntry<int> f in table1) {
                log.Debug("{0}  {1}   {2}   {3}", f.Value, f.AbsoluteFrequency, f.RelativeFrequency,
                          Math.Round(f.Percentage, 2));
            }

            /* now test the class using a string */
            var testString =
                new FrequencyTable<string>("NON NOBIS DOMINE, NON NOBIS, SED NOMINI TUO DA GLORIAM", TextAnalyzeMode.LettersOnly);
            var stringArray = testString.GetTableAsArray(FrequencyTableSortOrder.Frequency_Descending);
            Console.WriteLine("");
            Console.WriteLine("Character table sorted by frequency - descending:");
            Console.WriteLine("***************************************************");
            foreach(FrequencyTableEntry<string> f in stringArray) {
                log.Debug("{0}  {1}   {2}   {3}", f.Value, f.AbsoluteFrequency, f.RelativeFrequency,
                          Math.Round(f.Percentage, 2));
            }
            Console.Write("Press any key to exit");
        }
    }
}