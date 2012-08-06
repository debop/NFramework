using System;
using System.Text;
using NUnit.Framework;

namespace NSoft.NFramework.Samples {
    [TestFixture]
    public class StringFormatFixture : AbstractFixture {
        private const int Id = 12345;
        private const double Weight = 12.3558;
        private const char Row = 'Z';
        private const string Section = "1A2C";

        [Test]
        public void StringFormatTest() {
            var sb = new StringBuilder();

            sb.AppendLine("{index, alignment:formatString}");

            sb.AppendLine("G : genenral format");
            sb.AppendFormat("The Item ID={0:G} having weight = {1, 5:G} is founded in row {2, 5:G} and section {3, 5:G}",
                            Id, Weight, Row, Section)
                .AppendLine();

            sb.AppendLine("E: scientific notation");
            sb.AppendFormat("The Item ID={0:N} having weight = {1, 5:E} is founded in row {2, 5:E} and section {3,5:E}",
                            Id, Weight, Row, Section)
                .AppendLine();

            sb.AppendLine("N: number format");
            sb.AppendFormat("The Item ID={0:N} having weight = {1:N} is founded in row {2:N} and section {3:D}",
                            Id, Weight, Row, Section)
                .AppendLine();

            sb.AppendLine("custom format");
            sb.AppendFormat("The Item ID={0:[#####]} having weight = {1:0000.00 lbs} is founded in row {2:N} and section {3:D}",
                            Id, Weight, Row, Section)
                .AppendLine();

            Console.WriteLine(sb.ToString());
        }
    }
}