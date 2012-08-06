using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Networks {
    /// <summary>
    /// FTP의 Directory 에 대한 표현 및 관련 작업
    /// </summary>
    [Serializable]
    public class FtpDirectory : List<FtpFileInfo> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Ftp Directory 구분자 ('/')
        /// </summary>
        public const char DirectorySeparator = '/';

        /// <summary>
        /// 기본생성자
        /// </summary>
        public FtpDirectory() {}

        /// <summary>
        /// Initialize a new instance of FtpDirectory with directory name and remote path
        /// </summary>
        /// <param name="dir">directory name</param>
        /// <param name="path"></param>
        public FtpDirectory(string dir, string path) {
            if(IsDebugEnabled)
                log.Debug("Create a new instance of FtpDirectory class with dir=[{0}], path=[{1}]", dir, path);

            var lines = dir.Replace("\n", "").Split(StringSplitOptions.RemoveEmptyEntries, '\r');

            foreach(var line in lines) {
                if(line.IsNotWhiteSpace())
                    Add(new FtpFileInfo(line, path));
            }
        }

        /// <summary>
        /// 현 디렉토리에 지정한 파일 정보가 있는지 검사한다.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool FileExists(string filename) {
            filename.ShouldNotBeWhiteSpace("filename");

            if(IsDebugEnabled)
                log.Debug("Check file exists. filename=[{0}]", filename);

            foreach(var fi in this) {
                if(filename.EqualTo(fi.Filename)) {
                    if(IsDebugEnabled)
                        log.Debug("The specified file does exist! filename=[{0}]", fi.FullName);

                    return true;
                }
            }

            if(IsDebugEnabled)
                log.Debug("The specified file does NOT exist. filename=[{0}]", filename);

            return false;
        }

        /// <summary>
        /// 현 디렉토리의 서브 디렉토리 목록을 가져온다.
        /// </summary>
        /// <returns></returns>
        public FtpDirectory GetDirectories() {
            return GetDirectories(string.Empty);
        }

        /// <summary>
        /// 현 디렉토리의 서브 디렉토리 목록을 가져온다.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public FtpDirectory GetDirectories(string extension) {
            return GetFtpEntry(FtpEntryKind.Directory, extension);
        }

        /// <summary>
        /// 현 디렉토리의 파일 목록
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public FtpDirectory GetFiles(string extension = null) {
            return GetFtpEntry(FtpEntryKind.File, extension.AsText());
        }

        /// <summary>
        /// 지정한 형식의 엔트리(파일 or 디렉토리) 중 지정한 확장자를 가진 엔트리의 정보를 가져온다.
        /// </summary>
        /// <param name="kind">엔트리 종류(파일 or 디렉토리</param>
        /// <param name="extension">파일 확장자</param>
        /// <returns></returns>
        private FtpDirectory GetFtpEntry(FtpEntryKind kind, string extension = null) {
            extension = extension ?? extension.AsText();
            if(IsDebugEnabled)
                log.Debug("Get FtpEntry with EntryType=[{0}], extensions=[{1}]", kind, extension);

            FtpDirectory ftpDir = new FtpDirectory();

            foreach(FtpFileInfo fi in this.Where(f => f.FileKind == kind)) {
                if(extension.IsWhiteSpace() || extension.EqualTo(fi.Extension))
                    ftpDir.Add(fi);
            }

            if(IsDebugEnabled)
                log.Debug("조회된 FtpEntry의 갯수=[{0}]", ftpDir.Count);

            return ftpDir;
        }

        /// <summary>
        /// 지정된 디렉토리의 상위 디렉토리를 반환한다.
        /// </summary>
        /// <param name="dir">기준이 되는 디렉토리</param>
        /// <returns>상위 디렉토리가 없다면 null을 반환한다.</returns>
        public static string GetParentDirectory(string dir) {
            var tmp = dir.TrimEnd(DirectorySeparator);
            var index = tmp.LastIndexOf(DirectorySeparator);

            if(index > 0)
                return tmp.Substring(0, index - 1);

            // return null;
            throw new FtpException("지정된 디렉토리는 ROOT 디렉토리이므로 부모 디렉토리가 없습니다. dir=" + dir);
        }
    }
}