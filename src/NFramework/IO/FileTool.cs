using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.IO {

    /// <summary>
    /// 파일 IO 관련 Helper Class
    /// </summary>
    public static partial class FileTool {
        
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 버전 정보를 나타내는 포맷 - Major.Minor.Release.Build
        /// </summary>
        public const string FMT_VERSION = @"{0}.{1}.{2}.{3}";

        /// <summary>
        /// 알 수 없는 버전에 대한 상수 값 = "0.0.0.0"
        /// </summary>
        public const string UNKNOWN_VERSION = @"0.0.0.0";

        /// <summary>
        /// 기본 파일 버퍼 사이즈 (4Kbyte)
        /// </summary>
        public const int DEFAULT_BUFFER_SIZE = 4096;

        /// <summary>
        /// 기본 Mime Type 입니다. (application/octet-stream)
        /// </summary>
        public const string DEFAULT_MIME_TYPE = @"application/octet-stream";

        /// <summary>
        /// 가상경로를 절대 경로로 계산한다.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static string GetPhysicalPath(this string virtualPath) {
            if(IsDebugEnabled)
                log.Debug("지정한 가상경로로부터 물리적경로를 찾습니다. virtualPath=[{0}]", virtualPath);

            string path;
            var rootPath = AppDomain.CurrentDomain.BaseDirectory;

            if(virtualPath.IsWhiteSpace())
                path = rootPath;

            else if(virtualPath.StartsWith("~"))
                path = Path.Combine(rootPath, virtualPath.Substring(1).TrimStart('/', Path.DirectorySeparatorChar));

            else if(virtualPath.StartsWith("."))
                path = Path.GetFullPath(virtualPath);

            else
                path = Path.Combine(rootPath, virtualPath);

            var physicalPath = path.Replace('/', Path.DirectorySeparatorChar);

            if(IsDebugEnabled)
                log.Debug("virtualPath=[{0}], physicalPath=[{1}]", virtualPath, physicalPath);

            return physicalPath;

            // BUG : 웹 응용프로그램에서 '~' 와 '/' 가 첫번째로 시작하지 않으면, 제대로 물리적 경로를 찾지 못한다.

            #region << VirtualPathUtility를 사용한 코드인데, 엄격하다. 그래서 더 문제다 >>

            //string physicalPath;

            //if(Path.IsPathRooted(virtualPath))
            //{
            //    if(IsDebugEnabled)
            //        log.Debug("입력된 값이 가상경로가 아니라 물리적경로이므로, 그대로 반환합니다.");

            //    physicalPath = virtualPath;
            //}

            //// 웹 응용프로그램으로 Hosting되고 있다면...
            //else if(HostingEnvironment.IsHosted)
            //{
            //    if(IsDebugEnabled)
            //        log.Debug("웹 호스팅 프로그램이므로, HostingEnvironment.MapPath를 이용하여 물리적 경로를 구합니다.");

            //    physicalPath = HostingEnvironment.MapPath(virtualPath);
            //}
            //else
            //{
            //    string path = virtualPath;
            //    string rootPath = AppDomain.CurrentDomain.BaseDirectory;

            //    if (VirtualPathUtility.IsAppRelative(virtualPath))
            //    {
            //        path = VirtualPathUtility.ToAbsolute(virtualPath, "/");
            //        path = path.Substring(1).Replace('/', Path.DirectorySeparatorChar);
            //        path = Path.Combine(rootPath, path);
            //    }
            //    else if (virtualPath.StartsWith("."))
            //        path = Path.GetFullPath(virtualPath);
            //    else
            //        path = Path.Combine(rootPath, virtualPath);

            //    physicalPath = path.Replace('/', Path.DirectorySeparatorChar);
            //}

            //if(IsDebugEnabled)
            //    log.Debug("가상경로=[{0}] => 물리적경로=[{1}]", virtualPath, physicalPath);

            //return physicalPath;

            #endregion
        }

        /// <summary>
        /// 시스템의 Temporary Directory를 반환한다. (C:\Documents and Settings\Administrator\Local Settings\Temp)
        /// </summary>
        /// <returns>임시 폴더</returns>
        public static string GetTempPath() {
            return Path.GetTempPath();
        }

        /// <summary>
        /// Application 현재 폴더 (Path)를 구한다.
        /// </summary>
        /// <returns>현재 폴더의 전체경로</returns>
        public static string GetCurrentPath() {
            return Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Windows System Directory의 경로를 구한다. (C:\Windows\System32)
        /// </summary>
        /// <returns></returns>
        public static string GetSystemPath() {
            return Environment.SystemDirectory;
        }

        /// <summary>
        /// Windows Directory의 경로를 구한다. (C:\Windows)
        /// </summary>
        /// <returns></returns>
        public static string GetWindowsPath() {
            string systemPath = Environment.SystemDirectory;
            return systemPath.Substring(0, systemPath.LastIndexOf(Path.DirectorySeparatorChar));
            // return systemPath.TrimEnd(Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Windows Program Files 경로를 구한다. (C:\Program Files)
        /// </summary>
        /// <returns></returns>
        public static string GetProgramFilesPath() {
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        }

        /// <summary>
        /// Programs 폴더의 경로를 구한다. (C:\Documents and Settings\Administrator)
        /// </summary>
        /// <returns></returns>
        public static string GetProgramsPath() {
            return Environment.GetFolderPath(Environment.SpecialFolder.Programs);
        }

        /// <summary>
        /// Shell의 특정 폴더를 찾는다. <br/>
        /// 시스템 특수 폴더는 공용 정보가 들어 있는 
        /// Program Files, Programs, System 또는 Startup 등과 같은 폴더입니다. 
        /// 특수 폴더는 Windows를 설치할 때 기본적으로 시스템에서 설정하거나 사용자가 명시적으로 설정합니다.
        /// </summary>
        /// <param name="folder">Environment.SpecialFolder</param>
        /// <returns>특정 폴더 Path</returns>
        public static string GetSpecialFolderPath(this Environment.SpecialFolder folder) {
            return Environment.GetFolderPath(folder);
        }

        /// <summary>
        /// 지정한 접두사로 고유한 임시 파일을 만들고, 디스크에 해당 이름으로 크기가 0 바이트인 파일을 만듭니다. 
        /// </summary>
        /// <param name="prefix">파일명에 쓰일 접두사</param>
        /// <returns>임시파일명</returns>
        public static string GetTempFileName(this string prefix) {
            if(IsDebugEnabled)
                log.Debug("Get Temp File name with the specified prefix. Prefix:" + prefix);

            string tempFilename;

            if(prefix.IsWhiteSpace()) {
                tempFilename = Path.GetTempFileName();
            }
            else {
                var tempPath = Path.GetTempFileName();
                var tempFile = prefix + ExtractFileName(tempPath);

                tempPath = ExtractFilePath(tempPath);
                tempFilename = Path.Combine(tempPath, tempFile);
            }

            if(IsDebugEnabled)
                log.Debug("New TempFileName:" + tempFilename);

            return tempFilename;
        }

        /// <summary>
        /// 고유한 임시 파일 이름을 반환하고, 디스크에 해당 이름으로 크기가 0 바이트인 파일을 만듭니다.
        /// </summary>
        /// <returns>임시파일명</returns>
        public static string GetTempFileName() {
            return GetTempFileName(string.Empty);
        }

        /// <summary>
        /// 파일의 버전 정보를 구한다.
        /// </summary>
        /// <param name="filename">버전 정보를 검색할 파일의 전체경로 및 이름</param>
        /// <returns>FileVersionInfo 인스턴스 반환, 파일이 없으면 null 반환</returns>
        public static FileVersionInfo GetFileVersionInfo(this string filename) {
            return File.Exists(filename) ? FileVersionInfo.GetVersionInfo(filename) : null;
        }

        /// <summary>
        /// 지정된 파일의 Version 정보를 반환한다.
        /// </summary>
        /// <param name="filename">검색할 파일의 전체경로</param>
        /// <returns>파일버전 정보, 파일이 없다면 UNKNOWN_VERSION을 반환한다.</returns>
        public static string GetFileVersion(this string filename) {
            if(File.Exists(filename)) {
                var fvi = FileVersionInfo.GetVersionInfo(filename);
                return fvi.FileVersion;
            }

            return UNKNOWN_VERSION;
        }

        /// <summary>
        /// 파일 버전 정보를 분해하여 각 구분별로 숫자로 나타낸다. (major.minor.release.build)
        /// </summary>
        /// <param name="version">버전 정보</param>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="release"></param>
        /// <param name="build"></param>
        /// <remarks>파싱이 불가능할 때에는 모든 값들이 0이 된다.</remarks>
        public static void ParseVersion(string version, out int major, out int minor, out int release, out int build) {
            major = 0;
            minor = 0;
            release = 0;
            build = 0;

            var versionArray = StringTool.Split(version, '.');

            if(versionArray.Length > 0)
                major = int.Parse(versionArray[0]);

            if(versionArray.Length > 1)
                minor = int.Parse(versionArray[1]);

            if(versionArray.Length > 2)
                release = int.Parse(versionArray[2]);

            if(versionArray.Length > 3)
                build = int.Parse(versionArray[3]);

            if(IsDebugEnabled)
                log.Debug("Parse Version:{0} ==> {1}.{2}.{3}.{4}", version, major, minor, release, build);
        }

        /// <summary>
        /// 지정된 버전 요소 값으로 버전을 나타내는 문자열을 만든다.
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="release"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public static string MakeVersion(int major, int minor, int release, int build) {
            return MakeVersion(new[] { major, minor, release, build });
        }

        /// <summary>
        /// 지정된 버전 요소 값으로 버전을 나타내는 문자열을 만든다.
        /// </summary>
        /// <param name="versionArray"></param>
        /// <returns></returns>
        private static string MakeVersion(params int[] versionArray) {
            var paramArray = new object[] { 0, 0, 0, 0 };

            for(var i = 0; i < versionArray.Length; i++)
                paramArray[i] = versionArray[i];

            return string.Format(FMT_VERSION, paramArray);
        }

        /// <summary>
        /// 두 버전을 비교한다.
        /// </summary>
        /// <param name="version1"></param>
        /// <param name="version2"></param>
        /// <returns>반환값이 양수이면 version1 이 더 높고, 같으면 두 버전이 같으며, 음수이면 version2 가 더 높은 버전이다.</returns>
        public static int CompareVersion(string version1, string version2) {
            var v1 = new int[4];
            var v2 = new int[4];

            ParseVersion(version1, out v1[0], out v1[1], out v1[2], out v1[3]);
            ParseVersion(version2, out v2[0], out v2[1], out v2[2], out v2[3]);

            for(int i = 0; i < 4; i++)
                if(v1[i] != v2[i])
                    return (v1[i] > v2[i]) ? 1 : -1;

            return 0;
        }

        /// <summary>
        /// 해당 디렉토리가 존재하는지 확인한다.
        /// </summary>
        /// <param name="path">검사할 경로</param>
        /// <returns>경로 존재 여부</returns>
        public static bool DirectoryExists(this string path) {
            if(path.IsWhiteSpace())
                return false;

            return Directory.Exists(path);
        }

        /// <summary>
        /// 경로 문자열에서 Directory 명만 추출해 낸다.
        /// </summary>
        /// <param name="path">경로명</param>
        /// <example>
        /// string assemPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        /// 
        /// Console.WriteLine();
        /// Console.WriteLine("Path : " + assemPath);
        /// Console.WriteLine("ExtractFilePath : " + FileUtil.ExtractFilePath(assemPath));
        /// Console.WriteLine("ExtractFileName : " + FileUtil.ExtractFileName(assemPath));
        /// Console.WriteLine("ExtractFileExt : " + FileUtil.ExtractFileExt(assemPath));
        /// 
        /// // Output
        /// Path : C:\NSoft\NFramework\test\NFramework.Test\bin\Debug\NFramework.Test.dll
        /// ExtractFilePath : C:\NSoft\NFramework\test\NFramework.Test\bin\Debug
        /// ExtractFileName : NSoft.NFramework.Test.dll
        /// ExtractFileExt : dll
        /// </example>
        /// <returns>경로만</returns>
        public static string ExtractFilePath(this string path) {
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// 경로 문자열에서 파일 이름에 해당하는 문자열만 추출한다.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <example>
        /// string assemPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        /// 
        /// Console.WriteLine();
        /// Console.WriteLine("Path : " + assemPath);
        /// Console.WriteLine("ExtractFilePath : " + FileUtil.ExtractFilePath(assemPath));
        /// Console.WriteLine("ExtractFileName : " + FileUtil.ExtractFileName(assemPath));
        /// Console.WriteLine("ExtractFileExt : " + FileUtil.ExtractFileExt(assemPath));
        /// 
        /// // Output
        /// Path : C:\NSoft\NFramework\test\NFramework.Test\bin\Debug\NFramework.Test.dll
        /// ExtractFilePath : C:\NSoft\NFramework\test\NFramework.Test\bin\Debug
        /// ExtractFileName : NSoft.NFramework.Test.dll
        /// ExtractFileExt : dll
        /// </example>
        public static string ExtractFileName(this string path) {
            path.ShouldNotBeWhiteSpace("path");
            return Path.GetFileName(path);
        }

        /// <summary>
        /// 경로 문자열에서 파일의 Extension 문자열만 추출한다.
        /// </summary>
        /// <param name="path"></param>
        /// <example>
        /// string assemPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        /// 
        /// Console.WriteLine();
        /// Console.WriteLine("Path : " + assemPath);
        /// Console.WriteLine("ExtractFilePath : " + FileUtil.ExtractFilePath(assemPath));
        /// Console.WriteLine("ExtractFileName : " + FileUtil.ExtractFileName(assemPath));
        /// Console.WriteLine("ExtractFileExt : " + FileUtil.ExtractFileExt(assemPath));
        /// 
        /// // Output
        /// Path : C:\NSoft\NFramework\test\NFramework.Test\bin\Debug\NFramework.Test.dll
        /// ExtractFilePath : C:\NSoft\NFramework\test\NFramework.Test\bin\Debug
        /// ExtractFileName : NSoft.NFramework.Test.dll
        /// ExtractFileExt : dll
        /// </example>
        /// <returns>파일명이 temp.txt라면 "txt" 만 반환한다.</returns>
        public static string ExtractFileExt(this string path) {
            path.ShouldNotBeWhiteSpace("path");
            return Path.GetExtension(path).Trim('.');
        }

        /// <summary>
        /// 지정한 경로의 파일시스템를 반환합니다.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FileSystemInfo GetFileSystemInfo(this string path) {
            if(File.Exists(path))
                return new FileInfo(path);

            return Directory.Exists(path) ? new DirectoryInfo(path) : null;
        }

        /// <summary>
        /// 지정된 파일 경로에 금칙 문자를 주어진 대체 문자열로 변경하여, 유효한 경로 문자열을 만든다.
        /// </summary>
        /// <param name="filename">변경할 파일명</param>
        /// <param name="invalidReplacement">금칙문자를 대체할 문자열</param>
        /// <param name="spaceReplacement">공백을 대체할 문자열 (null이면 공백은 그대로 둔다)</param>
        /// <example>
        /// <code>
        /// string filename = @"abc&lt;&gt;def|{}|gh.txt";
        /// string validFilename = FileTool.GetValidFileName(path, "_");
        /// Console.WriteLine("Input FileName={0}\tValid FileName={1}", filename, validFilename);
        /// 
        /// // output : Input Path=abc&lt;&gt;def|{}|gh.txt	Valid FileName=abc__def_{}_gh.txt
        /// </code>
        /// </example>
        /// <returns>유효한 문자열</returns>
        public static string GetValidFileName(this string filename, string invalidReplacement = null, string spaceReplacement = null) {
            filename.ShouldNotBeNull("filename");

            var sb = new StringBuilder(filename);

            foreach(char invalidPathChar in Path.GetInvalidFileNameChars())
                sb.Replace(invalidPathChar.ToString(), invalidReplacement);

            if(spaceReplacement != null)
                sb.Replace(StringTool.WhiteSpace, spaceReplacement);

            return sb.ToString();
        }

        /// <summary>
        /// 제대로 된 경로명인가 확인한다.
        /// </summary>
        /// <param name="path">Path 문자열</param>
        /// <returns>빈문자열이거나 경로명이 잘못되었으면 False를 반환</returns>
        public static bool IsValidPath(this string path) {
            if(path.IsWhiteSpace())
                return false;

            return (path.IndexOfAny(Path.GetInvalidPathChars()) == -1);
        }

        /// <summary>
        /// 지정된 파일 경로에 금칙 문자를 주어진 대체 문자열로 변경하여, 유효한 경로 문자열을 만든다.
        /// </summary>
        /// <param name="path">변경할 경로</param>
        /// <param name="invalidReplacement">금칙문자를 대체할 문자열</param>
        /// <param name="spaceReplacement">공백을 대체할 문자열 (null이면 공백은 그대로 둔다)</param>
        /// <example>
        /// <code>
        /// string path = @"c:\temp\abc&lt;&gt;def|{}|gh.txt";
        /// string validPath = FileUtil.GetValidPath(path, "_");
        /// Console.WriteLine("Input Path={0}\tValid Path={1}", path, validPath);
        /// 
        /// // output : Input Path=c:\temp\abc&lt;&gt;def|{}|gh.txt	Valid Path=c:\temp\abc__def_{}_gh.txt
        /// </code>
        /// </example>
        /// <returns>유효한 문자열</returns>
        public static string GetValidPath(this string path, string invalidReplacement = null, string spaceReplacement = null) {
            var sb = new StringBuilder(path);

            foreach(char invalidPathChar in Path.GetInvalidPathChars())
                sb.Replace(invalidPathChar.ToString(), invalidReplacement);

            if(spaceReplacement != null)
                sb.Replace(StringTool.WhiteSpace, spaceReplacement);

            var validPath = sb.ToString();

            // Path의 금칙문자보다 FileName의 금칙문자가 더 많다.
            //
            var filename = Path.GetFileName(validPath);
            if(filename.IsNotWhiteSpace()) {
                var validFileName = GetValidFileName(filename, invalidReplacement, spaceReplacement);
                sb.Replace(filename, validFileName);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 지정된 경로의 크기 (서브 디렉토리와 파일의 크기를 합산) 을 구한다.
        /// </summary>
        /// <param name="path">크기를 구하고자 하는 폴더의 경로</param>
        /// <param name="withSubDirectory">서브디렉토리 포함 여부</param>
        /// <returns></returns>
        public static long GetDirectorySize(this string path, bool withSubDirectory) {
            if(path.IsWhiteSpace())
                return 0L;

            var searchOption = withSubDirectory ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            return
                Directory
                    .GetFiles(path, "*", searchOption)
                    .AsParallel()
                    .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                    .Sum(file => new FileInfo(file).Length);
        }

        /// <summary>
        /// 해당 파일의 크기를 구한다.
        /// </summary>
        /// <param name="filename">검사할 파일의 전체 경로</param>
        /// <returns>파일 크기, 파일이 존재하지 않으면 -1을 반환</returns>
        public static long GetFileSize(this string filename) {
            if(FileExists(filename) == false)
                return -1;

            var fi = new FileInfo(filename);
            return (fi.Exists) ? fi.Length : -1L;
        }

        /// <summary>
        /// 파일의 생성 시간을 구한다.
        /// </summary>
        /// <param name="filename">검사할 파일의 전체 경로</param>
        /// <returns>파일 생성된 시간</returns>
        public static DateTime GetFileCreateTime(this string filename) {
            return File.GetCreationTime(filename);
        }

        /// <summary>
        /// 파일에 마지막 Access한 시간
        /// </summary>
        /// <param name="filename">파일의 전체 경로</param>
        /// <returns></returns>
        public static DateTime GetFileLastWriteTime(this string filename) {
            return File.GetLastWriteTime(filename);
        }

        /// <summary>
        /// 파일이 존재 하는지 검사하다.
        /// </summary>
        /// <param name="filename">검사할 파일의 전체 경로</param>
        /// <returns>존재 유무</returns>
        public static bool FileExists(this string filename) {
            return filename.IsNotWhiteSpace() && File.Exists(filename);
        }

        /// <summary>
        /// File 속성을 쓰기 가능하도록 한다.
        /// </summary>
        /// <param name="filename">속성을 설정할 파일의 전체경로</param>
        /// <exception cref="FileNotFoundException">지정한 파일을 찾을 수 없을 경우</exception>
        public static void ChangeFileAttributesToWritable(this string filename) {
            if(IsDebugEnabled)
                log.Debug("파일특성을 쓰기가능으로 설정합니다. filename=[{0}]", filename);

            Guard.Assert<FileNotFoundException>(() => FileExists(filename), "파일을 찾을 수 없습니다. filename=[{0}]", filename);

            var attributes = File.GetAttributes(filename);
            var attr = attributes | FileAttributes.ReadOnly;

            if(attr == attributes) {
                attributes ^= FileAttributes.ReadOnly;

                try {
                    File.SetAttributes(filename, attributes);
                }
                catch(Exception ex) {
                    if(log.IsErrorEnabled) {
                        log.Error("쓰기 가능 파일로 변경하는 데 실퍠했습니다. filename=[{0}]", filename);
                        log.Error(ex);
                    }

                    throw;
                }
            }
        }

        /// <summary>
        /// 파일이 읽기 전용인지 검사
        /// </summary>
        /// <param name="filename">검사할 파일의 전체 경로</param>
        /// <returns><see langword="true"/> 읽기 전용인 경우, 그렇지 않다면 false</returns>
        /// <exception cref="FileNotFoundException">지정한 파일이 없는 경우</exception>
        public static bool IsFileReadOnly(this string filename) {
            if(IsDebugEnabled)
                log.Debug("파일이 읽기전용(ReadOnly)인지 파악합니다... filename=[{0}]", filename);

            if(FileExists(filename) == false)
                throw new FileNotFoundException("파일을 찾을 수 없습니다!!!", filename);

            var attributes = File.GetAttributes(filename);
            var attr = attributes | FileAttributes.ReadOnly;

            return (attr == attributes);
        }

        /// <summary>
        /// 지정한 Directory가 존재하지 않는다면 새로 생성한다.
        /// </summary>
        /// <param name="path">전체 경로</param>
        public static void CreateDirectory(this string path) {
            path.ShouldNotBeWhiteSpace("path");

            if(Directory.Exists(path) == false) {
                if(IsDebugEnabled)
                    log.Debug("디렉토리를 생성합니다... path=[{0}]", path);

                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 지정된 Directory를 삭제한다. 해당 Directory에 속한 Sub Directory와 파일도 삭제 가능하다.
        /// </summary>
        /// <param name="path">삭제할 디렉토리 경로</param>
        /// <param name="recursive">하위 디렉토리나 파일을 삭제할지 여부</param>
        public static void DeleteDirectory(this string path, bool recursive = true) {
            if(path.IsWhiteSpace())
                return;

            if(Directory.Exists(path) == false)
                return;

            if(IsDebugEnabled)
                log.Debug("디렉토리를 삭제합니다... path=[{0}], recursive=[{1}]", path, recursive);

            Directory.Delete(path, recursive);
        }

        /// <summary>
        /// 디렉토리 전체를 복사한다.
        /// </summary>
        /// <param name="srcPath">원본 경로</param>
        /// <param name="destPath">대상 경로</param>
        /// <param name="overwrite">겹쳐쓰기 옵션</param>
        /// <exception cref="DirectoryNotFoundException">원본 경로가 없을 시, 대상 경로의 부모 경로가 없을 시</exception>
        /// <remarks>
        /// 대상경로가 존재하지 않을 시에는 새로 만들어 사용한다.
        /// </remarks>
        public static void CopyDirectory(this string srcPath, string destPath, bool overwrite = false) {
            srcPath.ShouldNotBeWhiteSpace("srcPath");
            destPath.ShouldNotBeWhiteSpace("destPath");

            if(IsDebugEnabled)
                log.Debug("디렉토리를 복사합니다... srcPath=[{0}], destPath=[{1}], overwrite=[{2}]", srcPath, destPath, overwrite);

            var srcDi = new DirectoryInfo(srcPath);
            var destDi = new DirectoryInfo(destPath);

            Guard.Assert(() => srcDi.Exists, "원본 디렉토리가 존재하지 않습니다. srcPath=[{0}]", srcPath);

            if(destDi.Exists == false)
                CreateDirectory(destPath);

            srcDi
                .GetFiles()
                .Where(fi => fi.Directory != destDi)
                .AsParallel()
                .RunEach(file => {
                             var destFile = Path.Combine(destDi.FullName, file.Name);

                             //
                             // Note - 만약에 여기서 에러가 발생하거나 원치 않는 조작을 한다면 
                             // FileInfo.CopyTo() 함수가 내부적으로 파일을 새로 만들지 못하는 것이다.
                             //
                             if(file.IsReadOnly == false) {
                                 try {
                                     file.CopyTo(destFile, overwrite);
                                 }
                                 catch(Exception ex) {
                                     if(log.IsWarnEnabled) {
                                         log.Warn("파일 복사에 실퍠했습니다!!! 원본파일=[{0}], 대상파일=[{1}]", file.FullName, destFile);
                                         log.Warn(ex);
                                     }
                                 }
                             }
                         });

            srcDi
                .GetDirectories()
                .Where(di => di.FullName != destDi.FullName)
                .AsParallel()
                .RunEach(di => CopyDirectory(di.FullName, Path.Combine(destDi.FullName, di.Name), overwrite));
        }

        /// <summary>
        /// 원본 디렉토리를 대상 디렉토리로 이동시킨다.
        /// </summary>
        /// <param name="srcPath">이동할 파일 또는 경로</param>
        /// <param name="destPath">새위치</param>
        public static void MoveDirectory(this string srcPath, string destPath) {
            if(DirectoryExists(srcPath))
                Directory.Move(srcPath, destPath);
        }

        /// <summary>
        /// 파일 복사
        /// </summary>
        public static void CopyFile(this string srcFile, string destFile, bool overwrite = false) {
            if(FileExists(srcFile))
                File.Copy(srcFile, destFile, overwrite);
        }

        /// <summary>
        /// 파일 이동
        /// </summary>
        public static void MoveFile(this string srcFile, string destFile, bool overwrite = false) {
            if(FileExists(srcFile) == false)
                return;

            if(overwrite)
                if(FileExists(destFile))
                    DeleteFile(destFile);

            File.Move(srcFile, destFile);
        }

        /// <summary>
        /// 길이가 0인 파일을 만든다.
        /// </summary>
        /// <param name="filePath">빈 파일의 전체 경로</param>
        public static void CreateEmptyFile(this string filePath) {
            filePath.ShouldNotBeWhiteSpace("filePath");

            using(File.Create(filePath))
                Thread.Sleep(1);
        }

        /// <summary>
        /// 파일 삭제
        /// </summary>
        /// <param name="filename">삭제할 파일명</param>
        /// <param name="fireException">에러발생시 throw 할 것인지 여부</param>
        public static void DeleteFile(this string filename, bool fireException = true) {
            try {
                File.Delete(filename);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("파일 삭제에 실패했습니다. filename=[{0}]", filename);
                    log.Warn(ex);
                }

                if(fireException)
                    throw;
            }
        }

        /// <summary>
        /// 파일명 변경, <see cref="MoveFile"/> 와 같음
        /// </summary>
        /// <param name="srcFileName">원본 파일 경로</param>
        /// <param name="destFileName">대상 파일 경로</param>
        public static void RenameFile(this string srcFileName, string destFileName) {
            MoveFile(srcFileName, destFileName);
        }

        /// <summary>
        ///  파일명 변경, <see cref="MoveFile"/> 와 같음
        /// </summary>
        /// <param name="srcFileInfo">원본 파일에 대한 FileInfo 인스턴스 개체</param>
        /// <param name="destFileName">대상 파일명</param>
        /// <exception cref="IOException">대상 파일이 존재하면 덮어쓰기 할 수 없습니다.</exception>
        public static void RenameFile(this FileInfo srcFileInfo, string destFileName) {
            destFileName.ShouldNotBeWhiteSpace("destFileName");

            if(srcFileInfo.Exists)
                srcFileInfo.MoveTo(destFileName);
        }

        /// <summary>
        /// 파일을 겹쳐쓰기를 방지하기위해 같은 이름의 파일이 있으면 새로운 파일 이름을 반환한다.
        /// </summary>
        /// <param name="filename">원하는 파일명</param>
        /// <returns>새로운 파일명</returns>
        /// <remarks>
        /// 해당 파일을 찾고, 그 파일이 없으면 해당 파일명을 반환하고,
        /// 중복되는 파일명이 있으면 "FileName[1].ext" 와 같이 뒤에 인덱스를 붙여서 만든다.
        /// </remarks>
        public static string FindNewFileName(this string filename) {
            filename.ShouldNotBeWhiteSpace("filename");

            string newFileName = filename;

            if(FileExists(newFileName)) {
                string path = Path.GetDirectoryName(filename);
                string file = Path.GetFileNameWithoutExtension(filename);
                string ext = Path.GetExtension(filename);

                int n = 1;
                do {
                    newFileName = string.Format("{0}[{1}]{2}", file, n, ext);
                    newFileName = Path.Combine(path, newFileName);

                    if(!File.Exists(newFileName))
                        break;
                    n++;
                } while(n < 10000);
            }

            return newFileName;
        }

        /// <summary>
        /// 파일을 겹쳐쓰기를 방지하기위해 지정된 파일명이 있다면, IE 처럼 파일명을 새로 생성한다.
        /// </summary>
        /// <param name="filename">원하는 파일명</param>
        /// <returns>새로운 파일명</returns>
        /// <remarks>
        /// 해당 파일을 찾고, 그 파일이 없으면 해당 파일명을 반환하고,
        /// 중복되는 파일명이 있으면 "FileName[1].ext" 와 같이 뒤에 인덱스를 붙여서 만든다.
        /// </remarks>
        public static string GetNewFileName(this string filename) {
            return FindNewFileName(filename);
        }

        /// <summary>
        /// 파일 스트림 <see cref="System.IO.FileStream"/>을 생성해 준다.
        /// </summary>
        /// <param name="filename">대상 파일 경로</param>
        /// <param name="openMode">열기 모드</param>
        /// <returns>파일 스트림, 실패시에는 null 반환</returns>
        /// <exception cref="FileNotFoundException">파일이 존재하지 않을 때</exception>
        public static FileStream GetFileStream(string filename, FileOpenMode openMode) {
            filename.ShouldNotBeWhiteSpace("filename");

            if(openMode == FileOpenMode.Read && !FileExists(filename))
                throw new FileNotFoundException("파일이 존재하지 않습니다.", filename);

            switch(openMode) {
                case FileOpenMode.Read:
                    return new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);

                case FileOpenMode.ReadWrite:
                    return new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

                case FileOpenMode.Write:
                    return new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);

                default:
                    return new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            }
        }

        /// <summary>
        /// 파일 스트림을 데코레이션한 버퍼 스트림을 생성한다.
        /// </summary>
        /// <param name="filename">파일 경로 명</param>
        /// <param name="openMode">열기 모드</param>
        /// <returns>버퍼링되는 파일 스트림, 실패시에는 null 반환</returns>
        public static BufferedStream GetBufferedFileStream(string filename, FileOpenMode openMode) {
            filename.ShouldNotBeWhiteSpace("filename");

            if(IsDebugEnabled)
                log.Debug("파일에 대한 BufferedStream을 생성합니다... filename=[{0}], openMode=[{1}]", filename, openMode);

            return new BufferedStream(GetFileStream(filename, openMode));
        }

        /// <summary>
        /// <paramref name="stream"/>을 읽어, <paramref name="filepath"/>의 파일에 저장합니다.
        /// 해당 파일이 존재하면 겹쳐 쓰거나 추가를 할 수 있습니다. 파일이 없으면 새로 생성합니다. 
        /// </summary>
        /// <param name="filepath">대상 파일 경로</param>
        /// <param name="stream">저장할 내용</param>
        /// <param name="overwrite">겹쳐쓰기 여부</param>
        public static void Save(string filepath, Stream stream, bool overwrite) {
            filepath.ShouldNotBeWhiteSpace("filepath");
            stream.ShouldNotBeNull("stream");

            if(IsDebugEnabled)
                log.Debug("스트림 내용을 파일에 저장합니다... filepath=[{0}], overwrite=[{1}]", filepath, overwrite);

            stream.SetStreamPosition(0);

            if(overwrite && FileExists(filepath)) {
                if(IsDebugEnabled)
                    log.Debug("기존 파일이 존재하고, 겹쳐쓰기를 해야 하므로, 기존 파일을 삭제합니다. filepath=[{0}]", filepath);

                DeleteFile(filepath);
            }

            //using(var fs = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            //using(var bs = new BufferedStream(fs))
            using(var bs = GetBufferedFileStream(filepath, FileOpenMode.ReadWrite)) {
                var buffer = new byte[DEFAULT_BUFFER_SIZE];

                int readCount;
                while((readCount = stream.Read(buffer, 0, DEFAULT_BUFFER_SIZE)) > 0) {
                    bs.Write(buffer, 0, readCount);
                }
                bs.Flush();
            }
        }

        /// <summary>
        /// 지정된 파일에 문자열을 지정된 인코딩 방식으로 저장합니다. 
        /// 해당 파일이 존재하면 겹쳐 쓰거나 추가를 할 수 있습니다. 파일이 없으면 새로 생성합니다.
        /// </summary>
        /// <param name="filepath">대상 파일 경로</param>
        /// <param name="text">저장할 문자열</param>
        /// <param name="overwrite">덮어 쓰기 여부</param>
        /// <param name="enc">인코딩</param>
        public static void Save(string filepath, string text, bool overwrite = true, Encoding enc = null) {
            filepath.ShouldNotBeWhiteSpace("filepath");
            enc = enc ?? StringTool.DefaultEncoding;

            if(IsDebugEnabled)
                log.Debug("지정된 문자열을 파일로 저장합니다... filepath=[{0}], text=[{1}], overwrite=[{2}], enc=[{3}]",
                          filepath, text.EllipsisChar(80), overwrite, enc);

            // 파일이 없다면, Directory도 없을 수 있으므로 Directory부터 만든다.
            //
            if(!File.Exists(filepath)) {
                var path = Path.GetDirectoryName(filepath);

                if(path.IsNotEmpty())
                    CreateDirectory(path);
            }
            using(var sw = new StreamWriter(filepath, !overwrite, enc)) {
                sw.Write(text ?? string.Empty);
            }
        }

        /// <summary>
        /// 바이트 배열을 지정한 파일에 저장합니다.
        /// </summary>
        /// <param name="filepath">파일 이름</param>
        /// <param name="bytes">저장할 바이트 배열</param>
        /// <param name="overwrite">겹쳐쓰기 옵션</param>
        public static void Save(string filepath, byte[] bytes, bool overwrite) {
            filepath.ShouldNotBeWhiteSpace("filepath");

            if(bytes == null || bytes.Length == 0)
                return;

            //using(var fs = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            //using(var bs = new BufferedStream(fs))
            using(var bs = GetBufferedFileStream(filepath, FileOpenMode.ReadWrite)) {
                if(bs.CanWrite)
                    bs.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// 파일을 읽어서 Byte Array로 반환한다.
        /// </summary>
        /// <param name="filepath">읽을 파일명</param>
        /// <returns>파일이 존재하지 않거나, 파일 내용이 없으면 길이가 0인 Byte Array가 반환된다.</returns>
        /// <example>
        ///	이진 파일을 Byte Array로 반환한다.
        ///	<code>
        ///		byte[] buff = FileTool.ToByteArray(filename);
        ///		string s = StringTool.AsString(buff);
        ///	</code>
        /// </example>
        public static byte[] ToByteArray(string filepath) {
            filepath.ShouldNotBeWhiteSpace("filepath");

            if(!FileExists(filepath))
                return new byte[0];

            byte[] buffer = null;

            using(var fs = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using(var bs = new BufferedStream(fs)) {
                if(bs.CanRead) {
                    buffer = new byte[fs.Length];
                    bs.Read(buffer, 0, (int)fs.Length);
                }
            }
            return buffer ?? new byte[0];
        }

        /// <summary>
        /// 파일 내용을 읽어서 메모리 스트림으로 변환합니다.
        /// </summary>
        /// <param name="filepath">읽을 파일 전체 경로</param>
        /// <param name="enc">인코딩 형식</param>
        /// <returns></returns>
        public static Stream ToStream(string filepath, Encoding enc = null) {
            filepath.ShouldNotBeWhiteSpace("filepath");
            enc = enc ?? StringTool.DefaultEncoding;

            if(IsDebugEnabled)
                log.Debug("파일을 읽어 Stream에 씁니다... filepath=[{0}], enc=[{1}]", filepath, enc);

            var buffer = new byte[DEFAULT_BUFFER_SIZE];

            //using(var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
            //using(var bs = new BufferedStream(fs))
            using(var bs = GetBufferedFileStream(filepath, FileOpenMode.Read)) {
                var ms = new MemoryStream();
                var sw = new StreamWriter(ms, enc);

                while(bs.Read(buffer, 0, buffer.Length) > 0) {
                    sw.Write(buffer);
                }
                return ms;
            }
        }

        /// <summary>
        /// 파일 내용을 지정된 인코딩 방식으로 문자열로 변환합니다.
        /// </summary>
        /// <param name="filepath">읽을 파일 전체 경로</param>
        /// <param name="enc">인코딩 형식</param>
        /// <returns></returns>
        public static string ToString(string filepath, Encoding enc = null) {
            filepath.ShouldNotBeWhiteSpace("filepath");
            enc = enc ?? StringTool.DefaultEncoding;

            if(IsDebugEnabled)
                log.Debug("파일을 읽어 문자열로 반환합니다... filepath=[{0}], enc=[{1}]", filepath, enc);

            using(var reader = new StreamReader(filepath, enc)) {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// 파일 내용을 읽어 Base64 형식의 문자열로 변환합니다.<br/>
        /// Http Protocol에서는 이진 Data는 전송할 수 없으므로, Base64 Encoding 방식을 기본으로 사용한다.
        /// </summary>
        /// <param name="filepath">읽을 파일 경로</param>
        /// <returns>Base64로 인코딩된 문자열</returns>
        /// <example>
        ///	파일을 읽어 Base64형식의 문자열로 인코딩하고, 다시 디코딩하여 파일을 저장하는 예제입니다.
        ///	<code>
        ///	const string srcFile  = @"C:\Windows\부채.bmp";
        ///	const string destFile = @"C:\Windows\부채2.bmp";
        ///	
        ///	PerfCount.Start();
        ///	
        ///	string base64String;
        ///	
        ///	Console.WriteLine("\n-----------------------------------------------");
        ///	Console.WriteLine(srcFile + " 을 읽어서 Base64String을 만듭니다.");
        ///	base64String = FileTool.Base64Encode(srcFile);
        ///	Console.WriteLine(base64String);
        ///	
        ///	Console.WriteLine("\n-----------------------------------------------");
        ///	Console.WriteLine(destFile + "로 위의 내용을 Decoding해서 저장합니다.");
        ///	
        ///	byte[] buffer = StringTool.Base64Decode(base64String);
        ///	base64String = null;
        ///	FileTool.Save(destFile, buffer, true);
        ///	buffer = null;
        ///	
        ///	// 성능 측정을 위해서 쓰는 것입니다.
        ///	Console.WriteLine("Base64Encode , Base64Decode Time : {0} sec", PerfCount.End() * 1000);
        ///	</code>
        /// </example>
        public static string Base64Encode(string filepath) {
            if(IsDebugEnabled)
                log.Debug("파일 정보를 Base64 인코딩하여 반환합니다... filepath=[{0}]", filepath);

            var result = string.Empty;

            if(FileExists(filepath) == false)
                return result;

            return ToByteArray(filepath).Base64Encode();
        }

        /// <summary>
        /// 파일 내용을 읽어 Base64 형식의 문자열로 변환합니다.<br/>
        /// Http Protocol에서는 이진 Data는 전송할 수 없으므로, Base64 Encoding 방식을 기본으로 사용한다.
        /// </summary>
        /// <param name="filepath">읽을 파일 경로</param>
        /// <param name="outArray">Base64로 인코딩된 파일 내용</param>
        public static void Base64Encode(this string filepath, out char[] outArray) {
            if(IsDebugEnabled)
                log.Debug("파일 정보를 Base64 인코딩하여 char 배열에 씁니다... filepath=[{0}]", filepath);

            outArray = new char[0];

            if(FileExists(filepath) == false)
                return;

            var buffer = ToByteArray(filepath);
            buffer.Base64Encode(out outArray);
        }

        /// <summary>
        /// 해당 폴더를 기준으로 검색 패턴과 일치하는 폴더를 검색한다.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern">검색할 패턴</param>
        /// <returns>검색된 폴더의 string array</returns>
        /// <example>
        ///	<code>
        ///		string[] dirs = FileTool.GetDirectories("C:\", "W*");
        ///		foreach(string dir in dirs)
        ///			Console.WriteLine(dir);
        ///	</code>
        /// </example>
        public static string[] GetDirectories(string path, string searchPattern) {
            return Directory.GetDirectories(path, searchPattern);
        }

        /// <summary>
        /// 해당 폴더를 기준으로 하위 폴더의 Path를 문자열 배열로 반환한다.
        /// </summary>
        /// <param name="path">경로</param>
        /// <returns>하위 폴더 이름의 배열</returns>
        /// <example>
        ///	<code>
        ///		string[] dirList = FileTool.GetDirectories("C:\");
        ///		foreach(string dir in dirList)
        ///			Console.WriteLine(dir);
        ///	</code>
        /// </example>
        public static string[] GetDirectories(string path) {
            return Directory.GetDirectories(path);
        }

        /// <summary>
        /// 지정된 폴더에 있는 파일중에 검색 패턴과 일치하는 파일의 이름을 배열로 반환한다.
        /// </summary>
        /// <param name="path">경로</param>
        /// <param name="searchPattern">검색 패턴</param>
        /// <returns>검색된 파일들의 이름 배열</returns>
        public static string[] GetFiles(string path, string searchPattern) {
            return Directory.GetFiles(path, searchPattern);
        }

        /// <summary>
        /// 지정된 폴더에 있는 모든 파일의 이름을 배열로 반환한다.
        /// </summary>
        /// <param name="path">검색할 폴더</param>
        /// <returns>지정된 폴더에 있는 모든 파일</returns>
        public static string[] GetFiles(string path) {
            return Directory.GetFiles(path);
        }

        /// <summary>
        /// 파일 경로를 분석해서 root (drive), directory, file, extension 정보를 구한다.
        /// <c>Path.GetPathRoot()</c> 참조
        /// </summary>
        /// <param name="path">분석할 파일 경로</param>
        /// <param name="root">"C:\" 같은 문자열이거나 빈 문자열</param>
        /// <param name="dirName">디렉토리 정보 혹은 빈 문자열</param>
        /// <param name="fullFileName">전체 파일이름 (extension 포함) 혹은 빈 문자열</param>
        /// <param name="extension">파일 Extension 혹은 빈 문자열</param>
        public static void ParsePath(string path, out string root, out string dirName, out string fullFileName, out string extension) {
            root = Path.GetPathRoot(path);
            dirName = ExtractFilePath(path); // Path.GetDirectoryName(path);
            fullFileName = ExtractFileName(path); // Path.GetFileName(path);
            extension = ExtractFileExt(path); // Path.GetExtension(path);
        }
    }
}