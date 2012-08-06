using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Networks {
    /// <summary>
    /// .NET 2.0 FtpWebRequest, FtpWebResponse를 이용한 FTP Client Class
    /// </summary>
    // TODO : SSL 적용
    // TODO : 이어받기, 이어 올리기
    public class FtpClient // : IDisposable
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Path delimiter used in FTP Protocol
        /// </summary>
        public const string FTP_PATH_DELIMITER = "/";

        /// <summary>
        /// URI Schema for Ftp protocol 
        /// </summary>
        public const string FTP_SCHEMA = "ftp://";

        /// <summary>
        /// localhost
        /// </summary>
        public const string FTP_LOCALHOST = "localhost";

        /// <summary>
        /// default ftp login user name (anonymous)
        /// </summary>
        public const string DEFAULT_USERNAME = "anonymous";

        /// <summary>
        /// default ftp login password
        /// </summary>
        public const string DEFAULT_PASSWORD = "";

        /// <summary>
        /// file buffer used in transfer file stream.
        /// </summary>
        public const int FTP_FILE_BUFFER = 4096;

        private string _currentDir = FTP_PATH_DELIMITER;

        /// <summary>
        /// FTP 서버의 현재 디렉토리
        /// </summary>
        public string CurrentDirectory {
            get {
                if(_currentDir.EndsWith(FTP_PATH_DELIMITER))
                    return _currentDir;

                return _currentDir + FTP_PATH_DELIMITER;
            }
            set {
                Guard.Assert(value.IsNotWhiteSpace() && value.StartsWith(FTP_PATH_DELIMITER),
                             @"Directory는 항상 [{0}] 문자로 시작해야 합니다.", FTP_PATH_DELIMITER);
                _currentDir = value;
            }
        }

        private string _hostname = FTP_LOCALHOST;

        /// <summary>
        /// URI 형식의 FTP 서버 주소 (예: ftp://localhost )
        /// </summary>
        public string Hostname {
            get { return (_hostname.StartsWith(FTP_SCHEMA)) ? _hostname : FTP_SCHEMA + _hostname; }
            set {
                value.ShouldNotBeWhiteSpace("Hostname");
                _hostname = value;
            }
        }

        // public string LastDir { get; set; }

        private string _username = DEFAULT_USERNAME;

        /// <summary>
        /// FTP 로그인 ID
        /// </summary>
        public string Username {
            get { return _username ?? DEFAULT_USERNAME; }
            set { _username = value; }
        }

        private string _password = DEFAULT_PASSWORD;

        /// <summary>
        /// FTP 로그인 비밀번호
        /// </summary>
        public string Password {
            get { return _password ?? DEFAULT_PASSWORD; }
            set { _password = value; }
        }

        /// <summary>
        /// 연결을 유지할 것인가 
        /// </summary>
        public bool KeepAlive { get; set; }

        /// <summary>
        /// Initialize a new instance of NSoft.NFramework.Networks.Ftp.FtpClient class.
        /// </summary>
        public FtpClient() {
            if(IsDebugEnabled)
                log.Debug("Create new instance of FtpClient.");

            // LastDir = string.Empty;
            CurrentDirectory = FTP_PATH_DELIMITER;
            KeepAlive = true;
        }

        /// <summary>
        /// Initialize a new instance of NSoft.NFramework.Networks.Ftp.FtpClient class with ftp host name and login account information.
        /// </summary>
        /// <param name="hostname">ftp server host name</param>
        /// <param name="username">login username</param>
        /// <param name="password">login password</param>
        public FtpClient(string hostname, string username = null, string password = null)
            : this() {
            hostname.ShouldNotBeWhiteSpace("hostname");
            Hostname = hostname;
            Username = username;
            Password = password;
        }

        /// <summary>
        /// 서버의 모든 디렉토리를 찾는다.
        /// </summary>
        /// <param name="directory">검색할 디렉토리 (예 : "/Users/Debop")</param>
        /// <returns></returns>
        public bool DirectoryExists(string directory) {
            if(IsDebugEnabled)
                log.Debug("원격 디렉토리가 존재하는지 확인합니다... directory=[{0}]", directory);

            directory = AdjustDir(directory);

            if(directory == FTP_PATH_DELIMITER)
                return true;

            try {
                // 디렉토리에 대해 리스트를 조회해본다.
                // 조회가 된다면, 서브 디렉토리가 없더라도, 550 예외가 발생하지 않으므로, 디렉토리가 존재한다.
                // 디렉토리가 없다면 550 예외가 발생하므로, false 이다.
                //
                ListDirectory(directory);

                if(IsDebugEnabled)
                    log.Debug("원격 디렉토리가 존재합니다!!! directory=[{0}]", directory);

                return true;
            }
            catch(WebException wex) {
                if(wex.Message.Contains("(550)")) {
                    if(IsDebugEnabled)
                        log.Debug("원격 서버의 디렉토리에 대한 검색 또는 접근이 불가합니다. 디렉토리가 존재하지 않는다고 간주합니다. directory=[{0}]", directory);
                }

                return false;
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("디렉토리 존재 여부를 확인하는데 예외가 발생했습니다. directory=[{0}]", directory);
                    log.Warn(ex);
                }
                return false;
            }
        }

        /// <summary>
        /// 현재 디렉토리의 서브 디렉토리 이름만 가져온다.
        /// </summary>
        /// <returns></returns>
        public ICollection<string> ListDirectory() {
            return ListDirectory(string.Empty);
        }

        /// <summary>
        /// 지정한 디렉토리의 서브 디렉토리 이름만 가져온다.
        /// </summary>
        /// <param name="directory">UNIX 스타일의 경로</param>
        /// <returns>없으면 길이가 0인 배열을 반환</returns>
        public ICollection<string> ListDirectory(string directory) {
            string dir = ListDirectoryInternal(directory, WebRequestMethods.Ftp.ListDirectory);
            return new Collection<string>(dir.Split(new[] { '\r' }, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// 하위 폴더도 포함하여 디렉토리 목록을 빌드하여 <paramref name="paths"/>에 추가한다.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="paths"></param>
        /// <param name="recursive"></param>
        public void ListDirectory(string directory, List<string> paths, bool recursive) {
            paths.ShouldNotBeNull("paths");

            if(IsDebugEnabled)
                log.Debug("디렉토리의 모든 엔트리를 조회합니다. directory=[{0}], recursive=[{1}]", directory, recursive);

            directory = AdjustDir(directory);
            paths.Add(directory);

            if(directory.EndsWith(FTP_PATH_DELIMITER) == false)
                directory += FTP_PATH_DELIMITER;

            var subDirs = ListDirectory(directory);

            if(subDirs.Count > 0 && recursive) {
                foreach(var subDir in subDirs) {
                    var subPath = PathCombine(directory, subDir);
                    ListDirectory(subPath, paths, true);
                }
            }
            else {
                paths.AddRange(subDirs);
            }
        }

        /// <summary>
        /// 현재 디렉토리의 서브 디렉토리의 정보를 가져온다.
        /// </summary>
        /// <returns></returns>
        public FtpDirectory ListDirectoryDetail() {
            return ListDirectoryDetail(string.Empty);
        }

        /// <summary>
        /// 지정한 디렉토리의 서브 디렉토리의 정보를 가져온다.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public FtpDirectory ListDirectoryDetail(string directory) {
            var dir = ListDirectoryInternal(directory, WebRequestMethods.Ftp.ListDirectoryDetails);
            return new FtpDirectory(dir, directory);
        }

        /// <summary>
        /// 지정된 디렉토리의 엔트리 정보를 가져온다.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private string ListDirectoryInternal(string directory, string command) {
            if(IsDebugEnabled)
                log.Debug("원격 서버의 디렉토리 정보를 가져옵니다... directory=[{0}], command=[{1}]", directory, command);

            if(directory.EndsWith(FTP_PATH_DELIMITER) == false)
                directory += FTP_PATH_DELIMITER;

            var request = GetRequest(directory);
            request.Method = command;

            var strResponse = SendRequest(request);
            return strResponse.Replace("\r\n", "\r").TrimEnd('\r');
        }

        /// <summary>
        /// 지정된 로칼 파일을 FTP의 현재 디렉토리에 같은 파일명으로 전송한다.
        /// </summary>
        /// <param name="localFilename">file to upload</param>
        public bool Upload(string localFilename) {
            return Upload(localFilename, string.Empty);
        }

        /// <summary>
        /// 지정된 로칼 파일을 FTP 서버에 지정된 파일명으로 저장한다.
        /// </summary>
        /// <param name="localFilename"></param>
        /// <param name="remoteFilename"></param>
        public bool Upload(string localFilename, string remoteFilename) {
            if(!File.Exists(localFilename))
                throw new FileNotFoundException("Local file not found to upload.", localFilename);

            return Upload(new FileInfo(localFilename), remoteFilename);
        }

        /// <summary>
        /// 지정된 로칼 파일을 FTP의 현재 디렉토리에 같은 파일명으로 전송한다.
        /// </summary>
        /// <param name="fi"></param>
        /// <returns></returns>
        public bool Upload(FileInfo fi) {
            return Upload(fi, string.Empty);
        }

        /// <summary>
        /// 지정된 로칼 파일을 FTP 서버에 지정된 파일명으로 저장한다.
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="remoteFilename"></param>
        /// <returns></returns>
        public virtual bool Upload(FileInfo fi, string remoteFilename) {
            fi.ShouldNotBeNull("fi");

            if(IsDebugEnabled)
                log.Debug("Upload local file to ftp server. local file=[{0}], remoteFilename=[{1}]", fi.FullName, remoteFilename);

            if(remoteFilename.IsWhiteSpace()) // 원격파일명을 지정하지 않은 경우는
                remoteFilename = CurrentDirectory + fi.Name;

            using(var bs = new BufferedStream(fi.OpenRead()))
                return Upload(bs, remoteFilename);
        }

        /// <summary>
        /// 지정된 스트림 정보를 FTP서버에 지정된 파일명으로 저장한다.
        /// </summary>
        /// <param name="localStream"></param>
        /// <param name="remoteFilename"></param>
        /// <returns></returns>
        public bool Upload(Stream localStream, string remoteFilename) {
            if(IsDebugEnabled)
                log.Debug("Upload localStream to ftp server. remoteFilename=[{0}]", remoteFilename);

            localStream.ShouldNotBeNull("localStream");
            remoteFilename = AdjustRemoteFilename(remoteFilename);

            var uri = Hostname + remoteFilename;
            var buffer = new byte[FTP_FILE_BUFFER];

            try {
                using(Stream requestStream = GetUploadRequestStream(uri)) {
                    int dataRead;
                    do {
                        dataRead = localStream.Read(buffer, 0, FTP_FILE_BUFFER);
                        requestStream.Write(buffer, 0, dataRead);
                    } while(dataRead > 0);
                }

                if(IsDebugEnabled)
                    log.Debug("Upload file is success. remoteFilename=[{0}]", remoteFilename);

                return true;
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("Upload a file is failed. eat the exception and return false");
                    log.Warn(ex);
                }

                return false;
            }
        }

        /// <summary>
        /// 원격 파일을 다운 받아 로칼 파일로 저장한다.
        /// </summary>
        /// <param name="remoteFilename">원격 파일 전체 경로 (예: /Users/debop/readme.txt)</param>
        /// <param name="localFilename">로칼 파일 전체 경로</param>
        /// <param name="canOverwrite">겹쳐쓰기 여부</param>
        /// <returns>다운로드 여부</returns>
        public bool Download(string remoteFilename, string localFilename, bool canOverwrite = false) {
            return Download(remoteFilename, new FileInfo(localFilename), canOverwrite);
        }

        /// <summary>
        /// FTP 원격파일을 다운로드 받는다.
        /// </summary>
        /// <param name="remoteFI">원격 파일 정보</param>
        /// <param name="localFilename">로칼 파일 전체 경로</param>
        /// <param name="canOverwrite">겹쳐쓰기 여부</param>
        /// <returns>다운로드 여부</returns>
        public bool Download(FtpFileInfo remoteFI, string localFilename, bool canOverwrite = false) {
            return Download(remoteFI.FullName, new FileInfo(localFilename), canOverwrite);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteFI">원격 파일 정보</param>
        /// <param name="localFI">로칼 파일 정보</param>
        /// <param name="canOverwrite">겹쳐쓰기 여부</param>
        /// <returns>다운로드 여부</returns>
        public bool Download(FtpFileInfo remoteFI, FileInfo localFI, bool canOverwrite = false) {
            return Download(remoteFI.FullName, localFI, canOverwrite);
        }

        /// <summary>
        /// FTP 원격 파일을 지정한 로칼 파일로 저장한다.
        /// </summary>
        /// <param name="remoteFilename">원격 파일 전체 경로 (예: /Users/debop/readme.txt)</param>
        /// <param name="localFI">로칼 파일 정보</param>
        /// <param name="canOverwrite">겹쳐쓰기 여부</param>
        /// <returns>다운로드 여부</returns>
        public virtual bool Download(string remoteFilename, FileInfo localFI, bool canOverwrite = false) {
            localFI.ShouldNotBeNull("localFI");

            if(IsDebugEnabled)
                log.Debug("원격 파일을 다운로드 합니다... remoteFilename=[{0}], localFilename=[{1}], canOverwrite=[{2}]",
                          remoteFilename, localFI.FullName, canOverwrite);

            if(localFI.Exists && !canOverwrite)
                throw new FtpException("로컬 파일이 이미 존재합니다. localFile=" + localFI.FullName);

            bool isDownload;

            using(var localStream = new BufferedStream(localFI.OpenWrite())) {
                isDownload = Download(remoteFilename, localStream);

                if(isDownload)
                    localStream.Flush();
            }

            if(isDownload == false)
                With.TryAction(() => localFI.Delete());

            if(IsDebugEnabled)
                log.Debug("원격 파일[{0}]을 다운로드 작업을 마쳤습니다. 로컬 파일=[{1}], 성공여부=[{2}]",
                          remoteFilename, localFI.FullName, isDownload);

            return isDownload;
        }

        /// <summary>
        /// 원격 파일을 다운받아 지정된 스트림에 쓴다.
        /// </summary>
        /// <param name="remoteFilename">원격 파일 전체 경로 (예: /Users/debop/readme.txt)</param>
        /// <param name="localStream">원격파일 정보를 저장할 Stream 객체</param>
        /// <returns>다운로드 여부</returns>
        public bool Download(string remoteFilename, Stream localStream) {
            localStream.ShouldNotBeNull("localStream");

            if(IsDebugEnabled)
                log.Debug("원격 파일을 다운로드 받아 localStream에 씁니다.... remoteFilename=[{0}]", remoteFilename);

            remoteFilename = AdjustRemoteFilename(remoteFilename);
            var uri = Hostname + remoteFilename;

            try {
                using(var responseStream = GetDownloadResponseStream(uri)) {
                    byte[] buffer = new byte[FTP_FILE_BUFFER];
                    int readCount;

                    do {
                        readCount = responseStream.Read(buffer, 0, FTP_FILE_BUFFER);
                        localStream.Write(buffer, 0, readCount);
                    } while(readCount > 0);

                    // localStream.Flush();
                }

                if(IsDebugEnabled)
                    log.Debug("Download the remote file is success. remoteFilename=[{0}]", remoteFilename);

                return true;
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("Fail to download the remote file. remoteFilename=[{0}]", remoteFilename);
                    log.Warn(ex);
                }

                return false;
            }
        }

        /// <summary>
        /// FTP 서버에 있는 파일을 삭제한다.
        /// </summary>
        /// <param name="remoteFilename"></param>
        /// <returns></returns>
        public bool DeleteFile(string remoteFilename) {
            if(IsDebugEnabled)
                log.Debug("Delete the remote file. remoteFilename=[{0}]", remoteFilename);

            var uri = Hostname + GetFullPath(remoteFilename);

            try {
                var request = GetRequest(uri);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                SendRequestOnly(request);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("Fail to delete the remote file. remoteFilename=[{0}]", remoteFilename);
                    log.Warn(ex);
                }

                return false;
            }
            return true;
        }

        /// <summary>
        /// FTP 서버에 지정된 파일이 있는지 검사한다.
        /// </summary>
        /// <param name="remoteFilename"></param>
        /// <returns></returns>
        public bool FileExists(string remoteFilename) {
            if(remoteFilename.IsWhiteSpace())
                return false;

            if(IsDebugEnabled)
                log.Debug("Check the remote file is exist. remoteFilename=[{0}]", remoteFilename);

            try {
                GetFileSize(remoteFilename);
            }
            catch(WebException wex) {
                //if(log.IsWarnEnabled)
                //    log.WarnException("Fail to check the remote file is exists. remoteFilename=" + remoteFilename, wex);

                if(wex.Message.Contains("550"))
                    return false;

                throw;
            }
            return true;
        }

        /// <summary>
        /// 지정된 원격 파일의 크기를 가져온다.
        /// </summary>
        /// <param name="remoteFilename"></param>
        /// <returns></returns>
        public long GetFileSize(string remoteFilename) {
            if(IsDebugEnabled)
                log.Debug("Get size of the remote file. remoteFilename=[{0}]", remoteFilename);

            var uri = Hostname + GetFullPath(remoteFilename);

            var request = GetRequest(uri);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            return GetSize(request);
        }

        /// <summary>
        /// FTP 서버에 있는 파일명을 변경합니다.
        /// NOTE: .NET 4.0에서는 버그가 있습니다.
        /// http://stackoverflow.com/questions/4159903/problem-renaming-file-on-ftp-server-in-net-framework-4-0-only/5897531#5897531
        /// </summary>
        /// <param name="srcFilename">원본 파일명</param>
        /// <param name="destFilename">변경할 파일명</param>
        /// <returns></returns>
        public bool RenameFile(string srcFilename, string destFilename) {
            srcFilename.ShouldNotBeWhiteSpace("srcFilename");
            destFilename.ShouldNotBeWhiteSpace("destFilename");

            if(IsDebugEnabled)
                log.Debug("원격 파일명을 변경합니다... srcFilename=[{0}], destFilename=[{1}]", srcFilename, destFilename);

            var src = GetFullPath(srcFilename);
            var dest = GetFullPath(destFilename);

            if(src == dest)
                return true;

            try {
                var request = GetRequest(src);
                request.Method = WebRequestMethods.Ftp.Rename;
                request.RenameTo = destFilename; // dest;

                SendRequestOnly(request);

                if(IsDebugEnabled)
                    log.Debug("원격 파일명을 변경했습니다... src=[{0}], dest=[{1}]", src, dest);
            }
            catch(Exception ex) {
                if(IsDebugEnabled) {
                    log.Debug("원격파일명을 변경하는데 실퍠했습니다. src=[{0}], destFilename=[{1}]", src, destFilename);
                    log.Debug(ex);
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// 지정된 경로를 생성한다. (다중 폴더도 생성합니다. 예 : "/사용자/배성혁/개발 자료" )
        /// </summary>
        /// <param name="remoteDir"></param>
        /// <returns></returns>
        public bool CreateDirectory(string remoteDir) {
            if(IsDebugEnabled)
                log.Debug("원격 디렉토리를 생성합니다... remoteDir=[{0}]", remoteDir);

            string dir = AdjustDir(remoteDir);

            if(dir.Length < 2)
                return true;

            if(DirectoryExists(dir))
                return true;

            var uri = Hostname + dir;

            try {
                var request = GetRequest(uri);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                SendRequestOnly(request);

                if(IsDebugEnabled)
                    log.Debug("새로운 디렉토리를 생성했습니다. uri=[{0}]", uri);

                return true;
            }
            catch(WebException wex) {
                if(wex.Message.Contains("(550)"))
                    return false;
                throw;
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("Fail to create remote directory. uri=[{0}]", uri);
                    log.Warn(ex);
                }

                throw;
            }
        }

        /// <summary>
        /// 지정된 FTP 경로의 Dirctory를 제거한다.
        /// </summary>
        /// <param name="remoteDir"></param>
        /// <returns></returns>
        public bool DeleteDirectory(string remoteDir) {
            if(IsDebugEnabled)
                log.Debug("원격 디렉토리를 삭제합니다... remoteDir=[{0}]", remoteDir);

            if(DirectoryExists(remoteDir) == false)
                return false;

            var uri = Hostname + AdjustDir(remoteDir);

            try {
                var request = GetRequest(uri);
                request.Method = WebRequestMethods.Ftp.RemoveDirectory;

                SendRequestOnly(request);

                if(IsDebugEnabled)
                    log.Debug("원격 디렉토리를 삭제했습니다. uri=[{0}]", uri);

                return true;
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("원격 디렉토리를 삭제하는데 실패했습니다. uri=[{0}]", uri);
                    log.Warn(ex);
                }

                return false;
            }
        }

        /// <summary>
        /// 지정된 FTP 경로 및 하위 디렉토리도 모두 제거한다.
        /// </summary>
        /// <param name="remoteDir">remote directory name to delete</param>
        /// <param name="recursive">delete sub directories with recursive.</param>
        public bool DeleteDirectory(string remoteDir, bool recursive) {
            remoteDir.ShouldNotBeWhiteSpace("remoteDir");

            if(IsDebugEnabled)
                log.Debug("원격 디렉토리를 삭제합니다... remoteDir=[{0}], recursive=[{1}]", remoteDir, recursive);

            if(DirectoryExists(remoteDir) == false)
                return true;

            if(recursive) {
                // 디렉토리에 파일이 있을 경우에는 삭제가 안됩니다!!!
                //
                ICollection<string> subDirs = ListDirectory(remoteDir);
                if(subDirs.Count > 0) {
                    foreach(var dir in subDirs)
                        DeleteDirectory(remoteDir + FTP_PATH_DELIMITER + dir, true);
                }
            }
            return DeleteDirectory(remoteDir);
        }

        /// <summary>
        /// 지정된 URI에 대한 Ftp Request 객체를 생성한다.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        internal FtpWebRequest GetRequest(string uri) {
            uri.ShouldNotBeWhiteSpace("uri");

            if(uri.StartsWith(Hostname) == false)
                uri = Hostname + uri;

            var request = (FtpWebRequest)WebRequest.Create(new Uri(uri));

            // NOTE: FTP 는 HTTP와는 달리 Proxy를 지원하지 않는다. 꼭 설정해주어야 한다.
            //
            request.Proxy = null;
            request.Credentials = GetCredentials();

            // Request 연결 유지를 설정한다.
            request.KeepAlive = KeepAlive;
            //request.UsePassive = false;
            request.UseBinary = true;

            if(IsDebugEnabled)
                log.Debug("Create FtpWebRequest with uri=[{0}]", uri);

            return request;
        }

        /// <summary>
        /// FtpWebRequest의 RequestStream을 얻는다.
        /// </summary>
        /// <param name="uri">Upload할 위치를 나타내는 Uri string</param>
        /// <returns></returns>
        // NOTE : 통신 예외가 자주 발생해서 여러번 시도 할 수 있도록 바꾸었다.
        internal Stream GetUploadRequestStream(string uri) {
            uri.ShouldNotBeWhiteSpace("uri");

            Stream stream = null;
            var count = 0;

            do {
                count++;
                try {
                    var request = GetRequest(uri);
                    request.Method = WebRequestMethods.Ftp.UploadFile;

                    stream = request.GetRequestStream();
                }
                catch(Exception ex) {
                    if(log.IsWarnEnabled) {
                        log.Warn("FTP 요청 객체의 스트림을 얻는데 실패했습니다. uri=[{0}]", uri);
                        log.Warn(ex);
                    }

                    // NOTE : 여기서 몇 번 정도의 예외는 무시합니다.
                }
            } while(count < 3 && stream == null);

            if(IsDebugEnabled)
                log.Debug("FTP 요청 객체의 스트림 얻기. 결과 : " + ((stream != null) ? "성공" : "실패"));

            return stream;
        }

        /// <summary>
        /// FtpWebResponse의 ResponseStream을 얻는다. 
        /// </summary>
        /// <param name="uri">다운로드할 위치를 나타내는 URI string.</param>
        /// <returns>Response stream.</returns>
        internal Stream GetDownloadResponseStream(string uri) {
            uri.ShouldNotBeWhiteSpace("uri");

            Stream stream = null;
            var count = 0;

            do {
                count++;
                try {
                    var request = GetRequest(uri);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;

                    var response = (FtpWebResponse)request.GetResponse();
                    stream = response.GetResponseStream();
                }
                catch(Exception ex) {
                    if(log.IsWarnEnabled) {
                        log.Warn("FTP 응답 객체의 스트림을 얻는데 실패했습니다. uri=[{0}]", uri);
                        log.Warn(ex);
                    }

                    // NOTE: 네트웍 작업 실패는 몇번은 무시한다.
                }
            } while(count < 3 && stream == null);

            if(IsDebugEnabled)
                log.Debug("FTP 요청 객체의 스트림 얻기. 결과 : " + ((stream != null) ? "성공" : "실패"));

            return stream;
        }

        /// <summary>
        /// 네트웍 사용자 인정 정보
        /// </summary>
        /// <returns>a new instance of <see cref="NetworkCredential"/> class.</returns>
        internal ICredentials GetCredentials() {
            return new NetworkCredential(Username, Password);
        }

        /// <summary>
        /// 현재 디렉토리를 기준으로 지정된 파일명의 전체 경로를 가져온다.
        /// </summary>
        /// <param name="file">file name</param>
        /// <returns>full path string of a specified file which is located current directory.</returns>
        internal string GetFullPath(string file) {
            if(file.Contains("/"))
                return AdjustDir(file);

            return CurrentDirectory + file;
        }

        /// <summary>
        /// 원격 파일명으로 FTP 전체 경로가 포함된 파일 경로로 만든다.
        /// </summary>
        /// <param name="remoteFilename"></param>
        /// <returns></returns>
        internal string AdjustRemoteFilename(string remoteFilename) {
            remoteFilename.ShouldNotBeWhiteSpace("remoteFilename");

            if(remoteFilename.Contains(FTP_PATH_DELIMITER))
                return AdjustDir(remoteFilename); // 경로가 있을 경우

            return CurrentDirectory + remoteFilename; // 경로없이 파일명만 있을 경우
        }

        /// <summary>
        /// FTP 경로 형식에 맞게 '/' 로 시작하고 끝의 '/' 는 제거한다.
        /// </summary>
        /// <param name="path">대상 경로 문자열</param>
        /// <returns>FTP 경로 형식에 맞는 문자열</returns>
        internal static string AdjustDir(string path) {
            path = path ?? FTP_PATH_DELIMITER;
            var result = (path.StartsWith(FTP_PATH_DELIMITER)) ? path : FTP_PATH_DELIMITER + path;

            if(result.Length > 1 && result.EndsWith(FTP_PATH_DELIMITER))
                return result.TrimEnd(FTP_PATH_DELIMITER[0]);

            return result;
        }

        /// <summary>
        /// FTP 두개의 경로를 합한다.
        /// </summary>
        /// <param name="path1">first path</param>
        /// <param name="path2">second path</param>
        /// <returns>combined path with first path and second path with path delimiter</returns>
        internal static string PathCombine(string path1, string path2) {
            path1 = AdjustDir(path1);
            path2 = AdjustDir(path2);

            string result = AdjustDir(AdjustDir(path1) + AdjustDir(path2));

            result = FTP_PATH_DELIMITER + result.TrimStart(FTP_PATH_DELIMITER[0]);

            if(IsDebugEnabled)
                log.Debug("Combine two path with path delimiter. path1=[{0}], path2=[{1}], combined path=[{2}]", path1, path2, result);

            return result;
        }

        /// <summary>
        /// FTP 파일 전체경로에서 파일이름 부분을 뺀 경로만 가져온다.
        /// </summary>
        /// <param name="remoteFilename">FTP 서버의 파일 경로</param>
        /// <returns>path that extracted filename.</returns>
        public string ExtractPath(string remoteFilename) {
            if(remoteFilename.IsWhiteSpace())
                return string.Empty;

            var index = remoteFilename.LastIndexOf(FTP_PATH_DELIMITER);
            return index > 1 ? remoteFilename.Substring(0, index) : remoteFilename;
        }

        /// <summary>
        /// 현재 디렉토리의 전체 경로를 URI 형식으로 얻는다.
        /// </summary>
        /// <returns>current directory</returns>
        internal string GetDirectory() {
            return GetDirectory(string.Empty);
        }

        /// <summary>
        /// 지정된 디렉토리의 전체 경로를 URI 형식으로 얻는다.
        /// </summary>
        /// <param name="dir">대상경로</param>
        /// <returns>FTP Uri 형식의 문자열</returns>
        internal string GetDirectory(string dir) {
            if(IsDebugEnabled)
                log.Debug("Get URI string for remote directory. dir=[{0}]", dir);

            if(dir.IsWhiteSpace()) {
                dir = CurrentDirectory;
            }
            else {
                Guard.Assert(dir.StartsWith(FTP_PATH_DELIMITER), @"디렉토리는 [{0}] 문자로 시작해야 합니다.", FTP_PATH_DELIMITER);
            }

            var uri = Hostname + dir;
            // LastDir = dir;

            if(IsDebugEnabled)
                log.Debug("Get directory is success. dir=[{0}], uri=[{1}]", dir, uri);

            return uri;
        }

        /// <summary>
        /// FTP 서버로부터 응답을 받아, 문자열로 변환하여 반환한다.
        /// </summary>
        /// <remarks>
        /// FTP 서버들은 대부분 문자열 반환에 개행문자로 CRLF (MS-DOS 스타일) 를 쓰지 않고, CR (UNIX 스타일) 만을 보낸다.
        /// 문자열을 MS-DOS 스타일로 사용하고 싶으면 CR을 CRLF로 변환한 후 사용해야한다.
        /// </remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string SendRequest(WebRequest request) {
            request.ShouldNotBeNull("request");

            using(var response = (FtpWebResponse)request.GetResponse()) {
                return response.GetResponseStream().ToText();
            }
        }

        /// <summary>
        /// FTP로 요청을 보낸다. 응답을 받지만 파싱하지는 않는다.
        /// </summary>
        /// <param name="request"></param>
        internal static void SendRequestOnly(FtpWebRequest request) {
            request.ShouldNotBeNull("request");

            using(request.GetResponse()) {
                if(IsDebugEnabled)
                    log.Debug("Send request... no response parsing...");
            }
            request = null;
        }

        /// <summary>
        /// FTP 요청에 대한 응답의 크기를 구한다.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static long GetSize(WebRequest request) {
            request.ShouldNotBeNull("request");

            using(var response = (FtpWebResponse)request.GetResponse()) {
                if(IsDebugEnabled)
                    log.Debug("Get Size of response. response.ContentLength=[{0}]", response.ContentLength);

                return response.ContentLength;
            }
        }
    }
}