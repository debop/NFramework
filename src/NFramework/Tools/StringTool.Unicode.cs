using System.IO;

namespace NSoft.NFramework.Tools {
    public static partial class StringTool {
        /// <summary>
        /// Char를 Unicode char의 문자열로 표현합니다.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string AsUnicode(this char c) {
            using(var writer = new StringWriter()) {
                WriteCharAsUnicode(writer, c);
                return writer.ToString();
            }
        }

        /// <summary>
        /// 지정한 char 의 unicode string을 <paramref name="writer"/>에 쓴다.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="c"></param>
        public static void WriteCharAsUnicode(TextWriter writer, char c) {
            writer.ShouldNotBeNull("writer");

            var h1 = HexTextTool.IntToHex((c >> 12) & '\x000f');
            var h2 = HexTextTool.IntToHex((c >> 8) & '\x000f');
            var h3 = HexTextTool.IntToHex((c >> 4) & '\x000f');
            var h4 = HexTextTool.IntToHex(c & '\x000f');

            writer.Write('\\');
            writer.Write('u');
            writer.Write(h1);
            writer.Write(h2);
            writer.Write(h3);
            writer.Write(h4);
        }
    }
}