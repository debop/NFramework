using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Tools {
    [TestFixture]
    public class StringToolFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const string WhiteSpaces = "\r\n\f\t";

        private const string S1 = @"ABC,DEF,EF;FF";
        private const string S2 = @",가;나;다,라";
        private const string S3 = @"동해물과 백두산이 마르고 닳도록 ; ~!@#$%^&*()_+ ; Hello World ; 동해물과 백두산이 마르고 닳도록";

        #region << IsNull / IsEmpty / IsNotEmpty / IsWhiteSpace / IsNotWhiteSpace / IsMultiByteString >>

        [Test]
        public void StringIsNull() {
            string s = null;
            s.IsNull().Should().Be.True();

            s = string.Empty;
            string.Empty.IsNull().Should().Be.False();

            WhiteSpaces.IsNull().Should().Be.False();

            S1.IsNull().Should().Be.False();
        }

        [Test]
        public void StringIsEmpty() {
            string s = null;
            s.IsEmpty().Should().Be.True();

            s = string.Empty;
            string.Empty.IsEmpty().Should().Be.True();

            WhiteSpaces.IsEmpty().Should().Be.False();

            S1.IsEmpty().Should().Be.False();
        }

        [Test]
        public void StringIsNotEmpty() {
            string s = null;
            s.IsNotEmpty().Should().Be.False();

            s = string.Empty;
            string.Empty.IsNotEmpty().Should().Be.False();

            WhiteSpaces.IsNotEmpty().Should().Be.True();

            S1.IsNotEmpty().Should().Be.True();
        }

        [Test]
        public void StringIsWhiteSpace() {
            string s = null;
            s.IsWhiteSpace().Should().Be.True();

            s = string.Empty;
            string.Empty.IsWhiteSpace().Should().Be.True();

            WhiteSpaces.IsWhiteSpace().Should().Be.True();

            S1.IsWhiteSpace().Should().Be.False();
        }

        [Test]
        public void StringIsNotWhiteSpace() {
            string s = null;
            s.IsNotWhiteSpace().Should().Be.False();

            s = string.Empty;
            string.Empty.IsNotWhiteSpace().Should().Be.False();

            WhiteSpaces.IsNotWhiteSpace().Should().Be.False();

            S1.IsNotWhiteSpace().Should().Be.True();
        }

        #endregion

        #region String Manupulation

        [Test]
        public void DeleteCharAnyTest() {
            Assert.AreEqual(S1.DeleteCharAny(',', ';'), "ABCDEFEFFF");
            Assert.AreEqual(S2.DeleteCharAny(',', ';'), "가나다라");

            var db = "RDT_ABC_DEF 123";
            var words = db.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);

            foreach(var word in words) {
                Assert.AreEqual(3, word.Length);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void QuotedStrTest() {
            Assert.AreEqual("NULL", StringTool.QuotedStr(null));
            Assert.AreEqual("''", string.Empty.QuotedStr());
            Assert.AreEqual("'AAA''AAA'", "AAA'AAA".QuotedStr());
        }

        [Test]
        public void JoinTest() {
            Assert.AreEqual("abc;def;ghi", StringTool.Join(";", "abc", "def", "ghi"));
        }

        [Test]
        public void IndexOfNTest() {
            Assert.AreEqual(5, S1.IndexOfN("E", 1));
            Assert.AreEqual(8, S1.IndexOfN("E", 2));
            Assert.AreEqual(-1, S1.IndexOfN("E", 3));
        }

        [Test]
        public void ReverseTest() {
            string rev = S1.Reverse();
            Console.WriteLine("Original = " + S1);
            Console.WriteLine("Reversed = " + rev);

            Assert.AreEqual(S1, rev.Reverse());
        }

        public void ReplicateTest() {
            string r = "-".Replicate(50);
            string r2 = "-*-".Replicate(50);

            Assert.AreEqual(50, r.Length);
            Assert.AreEqual(150, r2.Length);

            Console.WriteLine(r);
            Console.WriteLine(r2);
        }

        [Test]
        public void SplitTest() {
            string original = "ABC,def,EF;FF||ABCDEF|ADFDSAF||DAFSADFASDF";

            var splitedStr = StringTool.Split(original, ' ', ',');

            Assert.AreEqual(3, splitedStr.Length);
            Assert.AreEqual("ABC", splitedStr[0]);

            var splitedStr2 = original.Split("def", false);
            Assert.AreEqual(2, splitedStr2.Length);
            Assert.AreEqual("ABC,", splitedStr2[0]);
        }

        [Test]
        public void WordCountTest() {
            Assert.AreEqual(2, StringTool.WordCount(S3, "이"));
            Assert.AreEqual(4, StringTool.WordCount(S1, "F"));
            Assert.AreEqual(4, StringTool.WordCount(S1, "f"));
            Assert.AreEqual(0, StringTool.WordCount(S1, "f", false));
        }

        [Test]
        public void ReplaceTest() {
            int count = 100;

            var sb = new StringBuilder(S3.Length * count);

            for(int i = 0; i < count; i++)
                sb.AppendLine(S3);

            string largeStr = sb.ToString();

            Assert.AreEqual(count * 2, StringTool.WordCount(largeStr, "동해물"));

            string replaced = largeStr.Replace("동해물", "서해물", true);
            Assert.AreEqual(0, StringTool.WordCount(replaced, "동해물"));
            Assert.AreEqual(count * 2, StringTool.WordCount(replaced, "서해물"));
        }

        [Test]
        public void CapitalizeTest() {
            string str = "aBC PROJECT_ID 한글 oH nO file name fileName";
            Console.WriteLine("\"{0}\" to TitleCase: {1}", str, str.Capitalize());
        }

        [TestCase("CompanyName", Result = "COMPANY_NAME")]
        [TestCase("UserId", Result = "USER_ID")]
        [TestCase("Description", Result = "DESCRIPTION")]
        [TestCase("ExAttr1", Result = "EX_ATTR1")]
        [TestCase("IX_CompanyName", Result = "IX_COMPANY_NAME")]
        [TestCase("IX_User_CompanyId", Result = "IX_USER_COMPANY_ID")]
        public string OracleNamaingTest(string name) {
            return name.ToOracleNaming();
        }

        [Test]
        public void HexDumpStringTest() {
            string hex = S3.ToBytes().GetHexDumpString();
            Console.WriteLine(hex);
        }

        [Test]
        [TestCase(5)]
        [TestCase(4)]
        public void Ellipsis(int length) {
            var ellipsisLength = length + 3;
            var str = S3; // "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var ellipsisStr = str.EllipsisChar(length);
            Console.WriteLine("EllipsisChar: " + ellipsisStr);
            Assert.AreEqual(ellipsisLength, ellipsisStr.Length);
            Assert.IsTrue(ellipsisStr.EndsWith("..."));

            ellipsisStr = str.EllipsisFirst(length);
            Console.WriteLine("EllipsisFirst: " + ellipsisStr);
            Assert.AreEqual(ellipsisLength, ellipsisStr.Length);
            Assert.IsTrue(ellipsisStr.StartsWith("..."));

            ellipsisStr = str.EllipsisPath(length);
            Console.WriteLine("EllipsisPath: " + ellipsisStr);
            Assert.AreEqual(ellipsisLength, ellipsisStr.Length);
            Assert.IsTrue(ellipsisStr.Contains("..."));
            Assert.IsFalse(ellipsisStr.EndsWith("..."));
            Assert.IsFalse(ellipsisStr.StartsWith("..."));
        }

        [Test]
        public void FormatWith() {
            var format = @"Hello {0}. 안녕하세요 {1}. 1년은 {2}일입니다.";
            var expected = @"Hello World. 안녕하세요 세계여. 1년은 365일입니다.";

            Assert.AreEqual(expected, format.FormatWith("World", "세계여", 365));
        }

        #endregion

        #region FirstOf, LastOf String

        [Test]
        public void StringFirstOf() {
            string ret = "abcdefg".FirstOf("d");
            Assert.AreEqual(ret, "abc", "Unexpected return.");
        }

        [Test]
        public void StringLastOf() {
            string ret = "abcdefg".LastOf("d");
            Assert.AreEqual(ret, "efg");
        }

        #endregion

        #region Encoding Test

        [Test]
        public void EncodingTest() {
            var encKR = Encoding.GetEncoding("ks_c_5601-1987");

            string krStr = S3.ToString(encKR);

            if(IsDebugEnabled) {
                log.Debug("{0}: {1} ==> {2}: {3}", Encoding.Default.BodyName, S3, encKR.BodyName, krStr);
                log.Debug("{0}: {1} ==> {2}: {3}", encKR.BodyName, krStr, Encoding.Default.BodyName,
                          krStr.ToString(Encoding.Default));
                log.Debug("{0}: {1} ==> {2}: {3}", Encoding.Default.BodyName, S3, Encoding.UTF8.BodyName,
                          krStr.ToString(Encoding.UTF8));
            }

            string utf8Str = S3.Utf8Encode();

            if(IsDebugEnabled) {
                log.Debug("Utf8Encode([{0}]) ==> [{1}]", S3, utf8Str);
                log.Debug("Utf8Decode([{0}]) ==> [{1}]", utf8Str, utf8Str.Utf8Decode());
            }

            const string fileName = "바탕화면03.jpg";
            Console.WriteLine(fileName);

            var euckR = Encoding.GetEncoding("euc-kr");
            Console.WriteLine("EUC-KR : " + euckR.GetString(Encoding.Default.GetBytes(fileName)));
            Console.WriteLine("ks_c_5601-1987 : " + encKR.GetString(Encoding.Default.GetBytes(fileName)));
        }

        [Test]
        public void AsciiEncoding() {
            var codePage = Encoding.ASCII.CodePage;

            if(IsDebugEnabled)
                log.Debug("ASCII codePage=" + codePage);
            log.Debug("ASCII name=" + Encoding.ASCII.BodyName);
        }

        #endregion

        #region << HighlightText >>

        public void HighlightText_By_Default() {
            S3.HighlightText("백두산").Contains("[백두산]").Should().Be.True();
            S3.HighlightText("Hello").Contains("[Hello]").Should().Be.True();

            S3.HighlightText("백두산").Contains("[Hello]").Should().Be.False();
            S3.HighlightText("Hello").Contains("[백두산]").Should().Be.False();

            S3.HighlightText("Hello ").Contains("[Hello ]").Should().Be.True();
            S3.HighlightText(" Hello").Contains("[ Hello]").Should().Be.True();
            S3.HighlightText(" Hello ").Contains("[ Hello ]").Should().Be.True();

            S3.HighlightText("Hello ").Contains("[Hello]").Should().Be.False();
            S3.HighlightText(" Hello").Contains("[Hello]").Should().Be.False();
            S3.HighlightText(" Hello ").Contains("[Hello]").Should().Be.False();
        }

        public void HighlightText_With_HighlightFunc() {
            Func<string, string> highlightFunc = (str) => string.Concat("*", str, "*");

            S3.HighlightText("백두산", highlightFunc).Contains("*백두산*").Should().Be.True();
            S3.HighlightText("Hello", highlightFunc).Contains("*Hello*").Should().Be.True();

            S3.HighlightText("백두산").Contains("*Hello*").Should().Be.False();
            S3.HighlightText("Hello").Contains("*백두산*").Should().Be.False();

            S3.HighlightText("Hello ", highlightFunc).Contains("*Hello *").Should().Be.True();
            S3.HighlightText(" Hello", highlightFunc).Contains("* Hello*").Should().Be.True();
            S3.HighlightText(" Hello ", highlightFunc).Contains("* Hello *").Should().Be.True();

            S3.HighlightText("Hello ", highlightFunc).Contains("*Hello*").Should().Be.False();
            S3.HighlightText(" Hello", highlightFunc).Contains("*Hello*").Should().Be.False();
            S3.HighlightText(" Hello ", highlightFunc).Contains("*Hello*").Should().Be.False();
        }

        public void HighlightText_With_Sensitive_HighlightFunc() {
            Func<string, string> highlightFunc = (str) => string.Concat("*", str, "*");

            S3.HighlightText("백두산", false, highlightFunc).Contains("*백두산*").Should().Be.True();
            S3.HighlightText("Hello", false, highlightFunc).Contains("*Hello*").Should().Be.True();

            S3.HighlightText("백두산", false, highlightFunc).Contains("*Hello*").Should().Be.False();
            S3.HighlightText("HELLO", false, highlightFunc).Contains("*Hello*").Should().Be.False();
            S3.HighlightText("hello", false, highlightFunc).Contains("*Hello*").Should().Be.False();
        }

        #endregion

        #region Convert To Stream, Bytes

        [Test]
        public void ByteArrayIndexOfTest() {
            const int multiply = 5000;
            var sb = new StringBuilder(S3.Length * multiply);

            for(int i = 0; i < multiply; i++) {
                sb.Append(S3);
            }
            sb.Append(S1);

            var buffer = Encoding.Default.GetBytes(sb.ToString());
            var boundary = Encoding.Default.GetBytes(S1);

            int pos = buffer.ByteArrayIndexOf(boundary);

            Console.WriteLine("\n--------------------------");
            Console.WriteLine("Buffer Size: " + buffer.Length);
            Console.WriteLine("Position: " + pos);
        }

        [Test]
        public void ToStream_ToString() {
            Assert.AreEqual(S3, StringTool.ToString(S3.ToStream()));
            Assert.AreEqual(S3, StringTool.ToString(S3.ToStream(Encoding.UTF8), Encoding.UTF8));
        }

        [Test]
        public void ToStream_ToString_With_Encoding() {
            using(var ms = S3.ToStream(Encoding.UTF8)) {
                string str2 = StringTool.ToString(ms, Encoding.UTF8);
                Assert.AreEqual(S3, str2);
            }
        }

        [Test]
        public void ToStream_ToText() {
            Assert.AreEqual(S3, S3.ToStream().ToText());
            Assert.AreEqual(S3, S3.ToStream(Encoding.UTF8).ToText(Encoding.UTF8));
        }

        [Test]
        public void ToStream_ToText_With_Length() {
            Assert.AreEqual(S3.Substring(0, 12), S3.ToStream().ToText(12));
        }

        [Test]
        public void ToStream_ToText_With_Encoding() {
            var encodings = new Encoding[] { Encoding.Default, Encoding.Unicode, Encoding.UTF8 };

            foreach(var enc in encodings) {
                Assert.AreEqual(S3, S3.ToStream(enc).ToText(enc));
                Assert.AreEqual(S3, S3.ToBytes(enc).ToText(enc));
            }
        }

        [Test]
        public void ToBytes_ToText_With_Encoding() {
            var encodings = new[] { Encoding.Default, Encoding.Unicode, Encoding.UTF8 };

            foreach(var enc in encodings) {
                Assert.AreEqual(S3, S3.ToBytes(enc).ToText(enc));
            }
        }

        [Test]
        public void Base64StringTest() {
            string enc = S3.ToBytes().Base64Encode();
            string dec = StringTool.ToString(enc.Base64Decode());

            Assert.AreEqual(S3, dec);
        }

        [Test]
        public void Base64_Encode_Decode() {
            Assert.AreEqual(S3, S3.ToBytes().Base64Encode().Base64Decode().ToText());
            Assert.AreEqual(S3, S3.ToBytes().Base64Encode().Base64Decode().ToTextUnsafe());

            Assert.AreEqual(S3, S3.ToBytes().Base64Encode().ToBytes().ToText().Base64Decode().ToText());
            Assert.AreEqual(S3, S3.ToBytes().Base64Encode().ToBytes().ToTextUnsafe().Base64Decode().ToTextUnsafe());
        }

        [Test]
        public void Utf8EncodingTest() {
            Assert.AreEqual(S3, S3.Utf8Encode().ToBytes().ToText().Utf8Decode());
            Assert.AreEqual(S3, S3.Utf8Encode().ToBytes().ToTextUnsafe().Utf8Decode());

            Assert.AreEqual(S3, S3.Utf8Encode().Utf8Decode());

            Assert.AreEqual(S3, S3.ToStream().Utf8Encode().Utf8Decode());

            Assert.AreEqual(S3, S3.Utf8Encode().ToStream().Utf8Decode());
        }

        [Test]
        public void IsMultibyte_Test() {
            var pairs = new[]
                        {
                            new KeyValuePair<string, bool>("English only.", false),
                            new KeyValuePair<string, bool>("한글 전용입니다.", true),
                            new KeyValuePair<string, bool>("한글 English 혼용입니다.", true)
                        };

            pairs.All(pair => Equals(pair.Value, pair.Key.IsMultiByteString())).Should().Be.True();
        }

        #endregion

        #region << Algorithms >>

        /// <summary>
        /// 최대 공배수
        /// </summary>
        [Test]
        public void Can_Get_LongestCommonSubstring() {
            const string str1 = S1 + S2 + S3 + S3;
            const string str2 = S2 + S1 + S1 + S3;

            string substring;
            var index = StringTool.GetLongestCommonSubstring(str1, str2, out substring);

            Assert.IsTrue(index > 0);
            Assert.IsNotEmpty(substring);
            Assert.AreEqual(S3, substring);

            Console.WriteLine("Longest common substring=" + substring);
        }

        /// <summary>
        /// 최소 공약수
        /// </summary>
        [Test]
        public void Can_Get_LongestCommonSequence() {
            const string str1 = S1 + S2 + S3 + S3;
            const string str2 = S2 + S1 + S1 + S3;

            string sequence;
            var index = StringTool.GetLongestCommonSequence(str1, str2, out sequence);

            Assert.IsTrue(index > 0);
            Assert.IsNotEmpty(sequence);
            Assert.AreEqual(S1 + S3, sequence);

            Console.WriteLine("Longest common sequence=" + sequence);
        }

        #endregion

        #region << Unicode >>

        [Test]
        public void CharAsUnicode() {
            string result = StringTool.AsUnicode('a');

            Assert.AreEqual(@"\u0061", result);

            result = StringTool.AsUnicode('<');
            Assert.AreEqual(@"\u003c", result);

            result = StringTool.AsUnicode('>');
            Assert.AreEqual(@"\u003e", result);

            result = StringTool.AsUnicode('\'');
            Assert.AreEqual(@"\u0027", result);

            result = StringTool.AsUnicode('\u0000');
            Assert.AreEqual(@"\u0000", result);

            result = StringTool.AsUnicode('\t');
            Assert.AreEqual(@"\u0009", result);
        }

        #endregion
    }
}