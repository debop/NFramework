using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Tools {
    [TestFixture]
    public class CommandLineParserFixture {
        [TestCase("-param1 value1 --height:100px /width 200px")]
        public void ParsingTest(string cmdParams) {
            var args = cmdParams.Split(" ");

            var parser = new CommandLineParser(args);

            parser["param1"].IsNotWhiteSpace().Should().Be.True();
            parser["param1"].Should().Be("value1");

            parser["height"].IsNotWhiteSpace().Should().Be.True();
            parser["height"].Should().Be("100px");

            parser["width"].IsNotWhiteSpace().Should().Be.True();
            parser["width"].Should().Be("200px");
        }
    }
}