using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Networks {
    /// <summary>
    /// FTP 서버의 파일이나 디렉토리 엔트리의 정보를 나타낸다.
    /// </summary>
    [Serializable]
    public class FtpFileInfo : ValueObjectBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 파일 포맷을 파싱하기 위해
        /// </summary>
        private static readonly string[] ParseFormats
            = new[]
              {
                  @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\w+\s+\w+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{4})\s+(?<name>.+)"
                  ,
                  @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\d+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{4})\s+(?<name>.+)"
                  ,
                  @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\d+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{1,2}:\d{2})\s+(?<name>.+)"
                  ,
                  @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\w+\s+\w+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{1,2}:\d{2})\s+(?<name>.+)"
                  ,
                  @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})(\s+)(?<size>(\d+))(\s+)(?<ctbit>(\w+\s\w+))(\s+)(?<size2>(\d+))\s+(?<timestamp>\w+\s+\d+\s+\d{2}:\d{2})\s+(?<name>.+)"
                  ,
                  @"(?<timestamp>\d{2}\-\d{2}\-\d{2}\s+\d{2}:\d{2}[Aa|Pp][mM])\s+(?<dir>\<\w+\>){0,1}(?<size>\d+){0,1}\s+(?<name>.+)"
              };

        private static readonly Regex[] _regexs;

        private static readonly CultureInfo English = new CultureInfo("en-US");

        /// <summary>
        /// static 생성자
        /// </summary>
        static FtpFileInfo() {
            _regexs = new Regex[ParseFormats.Length];

            for(int i = 0; i < ParseFormats.Length; i++)
                _regexs[i] = new Regex(ParseFormats[i], RegexOptions.Compiled);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="line">FTP에서 전송한 directory 정보</param>
        /// <param name="path">현재 디렉토리 경로</param>
        public FtpFileInfo(string line, string path) {
            if(IsDebugEnabled)
                log.Debug("Create a new instance of FtpFileInfo with line=[{0}], path=[{1}]", line.EllipsisChar(48),
                          path.EllipsisPath(48));

            var match = GetMatchingRegex(line);

            if(match == null)
                throw new FtpException("Unable to parse line=" + line);

            Filename = match.Groups["name"].Value;
            FilePath = path;
            FullName = string.Concat(FilePath, "/", Filename);
            Permission = match.Groups["permission"].Value;
            Size = match.Groups["size"].Value.AsLong(0L);

            string dir = match.Groups["dir"].Value;

            FileKind = (dir != string.Empty && dir != "-") ? FtpEntryKind.Directory : FtpEntryKind.File;

            var timestamp = match.Groups["timestamp"].AsText();

            if(timestamp.IsNotWhiteSpace())
                FileDateTime = ConvertTool.ConvertValue(timestamp, typeof(DateTime), English).AsDateTimeNullable();
        }

        /// <summary>
        /// 파일의 전체 경로 (예 : /Users/debop/readme.txt )
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// 원격 파일 명 (예 : readme.txt)
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// 원격 파일의 경로 (예 : /Users/debop )
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Ftp 엔트리가 파일인가 디렉토리인가 구분
        /// </summary>
        public FtpEntryKind FileKind { get; private set; }

        /// <summary>
        /// FTP 엔트리(파일 또는 디렉토리)에 대한 권한 ( UNIX 스타일 : rw )
        /// </summary>
        public string Permission { get; private set; }

        /// <summary>
        /// 파일 크기
        /// </summary>
        public long Size { get; private set; }

        /// <summary>
        /// 파일 생성 날짜
        /// </summary>
        public DateTime? FileDateTime { get; private set; }

        /// <summary>
        /// 파일 확장자 (예 : .txt)
        /// </summary>
        public string Extension {
            get { return Path.GetExtension(Filename); }
        }

        /// <summary>
        /// 확장자를 제외한 파일 이름만 (예 : readme )
        /// </summary>
        public string NameOnly {
            get { return Path.GetFileNameWithoutExtension(Filename); }
        }

        /// <summary>
        /// 주어진 정보를 정규식으로 파싱한다.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>실패시에는 null 반환</returns>
        private static Match GetMatchingRegex(string line) {
            if(IsDebugEnabled)
                log.Debug("Get Match with the specified line. line=[{0}]", line.EllipsisChar(24));

            for(var i = 0; i < ParseFormats.Length; i++) {
                var match = _regexs[i].Match(line);

                if(match.Success)
                    return match;
            }

            if(log.IsWarnEnabled)
                log.Warn("Get match of regular expression is FAIL!!!");

            return null;
        }

        public override int GetHashCode() {
            return HashTool.Compute(FullName, FileKind, Size, FileDateTime);
        }
    }
}