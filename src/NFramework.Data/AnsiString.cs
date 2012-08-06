using System.Data;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// <see cref="DbType.AnsiString"/>을 표현합니다.
    /// </summary>
    public class AnsiString {
        public AnsiString(string value) {
            Value = value;
        }

        public string Value { get; private set; }
    }
}