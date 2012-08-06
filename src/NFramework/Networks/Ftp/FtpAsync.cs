using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NSoft.NFramework.IO;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Networks {
    /// <summary>
    /// FTP 통신을 비동기 방식으로 수행하도록 합니다.
    /// </summary>
    public static class FtpAsync {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 요청만을 보냅니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        public static Task SendRequestOnlyTask(this FtpClient ftpClient, string uri, string method) {
            return
                ftpClient
                    .GetResponseTask(uri, method)
                    .ContinueWith(task => task.Result.Close(),
                                  TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        /// <summary>
        /// 비동기 방식으로 요청 스트림을 구합니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Task<Stream> GetRequestStreamTask(this FtpClient ftpClient, string uri, string method) {
            if(IsDebugEnabled)
                log.Debug("Ftp ResponseStream을 비동기 방식으로 호출합니다. uri=[{0}], method=[{1}]", uri, method);

            var request = ftpClient.GetRequest(uri);
            request.Method = method;

            return Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream,
                                                  request.EndGetRequestStream,
                                                  null);
        }

        /// <summary>
        /// <paramref name="ftpClient"/>를 이용하여 비동기 방식으로 웹 응답을 받습니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <example>
        /// <code>
        ///		// 특정 파일을 FTP 서버에서 다운로드 받습니다.
        ///		var responseTask = ftpClient.GetResponseTask(uri, WebRequestMethods.Ftp.DownloadFile);
        ///		var stream = ((FtpWebResponse)responseTask.Result).GetResponseStream();
        /// </code>
        /// </example>
        /// <seealso cref="GetResponseStreamTask"/>
        public static Task<WebResponse> GetResponseTask(this FtpClient ftpClient, string uri, string method) {
            var request = ftpClient.GetRequest(uri);
            request.Method = method;

            return GetResponseTask(ftpClient, request);
        }

        /// <summary>
        /// <paramref name="ftpClient"/>를 이용하여 비동기 방식으로 웹 응답을 받습니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="request">Ftp 웹 요청 객체</param>
        /// <returns></returns>
        /// <example>
        /// <code>
        ///		// 특정 파일을 FTP 서버에서 다운로드 받습니다.
        ///		var responseTask = ftpClient.GetResponseTask(uri, WebRequestMethods.Ftp.DownloadFile);
        ///		var stream = ((FtpWebResponse)responseTask.Result).GetResponseStream();
        /// </code>
        /// </example>
        /// <seealso cref="GetResponseStreamTask"/>
        public static Task<WebResponse> GetResponseTask(this FtpClient ftpClient, FtpWebRequest request) {
            request.ShouldNotBeNull("request");

            if(IsDebugEnabled)
                log.Debug("Ftp ResponseStream을 비동기 방식으로 호출합니다. uri=[{0}], method=[{1}]", request.RequestUri, request.Method);

            return Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse,
                                                       request.EndGetResponse,
                                                       null);
        }

        /// <summary>
        /// 비동기 방식으로 웹 응답을 받고, 그 응답의 스트림을 반환합니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Task<Stream> GetResponseStreamTask(this FtpClient ftpClient, string uri, string method) {
            return
                GetResponseTask(ftpClient, uri, method)
                    .ContinueWith(task => task.Result.GetResponseStream(),
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// 응답 스트림을 얻고, 문자열로 변환하여 반환합니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Task<string> GetResponseStringTask(this FtpClient ftpClient, string uri, string method) {
            return
                ftpClient
                    .GetResponseStreamTask(uri, method)
                    .ContinueWith(task => {
                                      var result = task.Result.ToText();
                                      task.Result.Close();
                                      return result;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// FTP 서버에 <paramref name="directory"/> 가 존재하는지 확인합니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static Task<bool> DirectoryExistsTask(this FtpClient ftpClient, string directory) {
            if(IsDebugEnabled)
                log.Debug("원격 디렉토리가 존재하는지 확인하는 비동기 작업을 directory=[{0}]", directory);

            directory = FtpClient.AdjustDir(directory);

            if(directory == FtpClient.FTP_PATH_DELIMITER)
                return Task.Factory.FromResult(true);

            var tcs = new TaskCompletionSource<bool>();
            var isSuccess = false;

            try {
                // 디렉토리에 대해 리스트를 조회해본다.
                // 조회가 된다면, 서브 디렉토리가 없더라도, 550 예외가 발생하지 않으므로, 디렉토리가 존재한다.
                // 디렉토리가 없다면 550 예외가 발생하므로, false 이다.
                //
                ftpClient.ListDirectoryTask(directory).Wait();

                if(IsDebugEnabled)
                    log.Debug("원격 디렉토리가 존재합니다. directory=[{0}]", directory);

                isSuccess = true;
            }
            catch(AggregateException age) {
                age.Handle(error => true);
            }

            tcs.TrySetResult(isSuccess);
            return tcs.Task;
        }

        /// <summary>
        /// 원격 파일을 비동기 방식으로 삭제합니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="remoteFilename">삭제할 파일명</param>
        /// <returns></returns>
        public static Task<bool> DeleteFileTask(this FtpClient ftpClient, string remoteFilename) {
            if(IsDebugEnabled)
                log.Debug("FTP 서버에 있는 원격 파일을 삭제하는 작업을 생성합니다... 원격파일=[{0}]", remoteFilename);

            var uri = ftpClient.Hostname + ftpClient.GetFullPath(remoteFilename);

            return
                ftpClient
                    .GetResponseTask(uri, WebRequestMethods.Ftp.DeleteFile)
                    .ContinueWith(task => {
                                      var result = task.IsCompleted;
                                      task.Result.Close();
                                      return result;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// 원격 파일이 존재하는지 확인합니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="remoteFilename"></param>
        /// <returns></returns>
        public static Task<bool> FileExistsTask(this FtpClient ftpClient, string remoteFilename) {
            if(remoteFilename.IsWhiteSpace())
                return Task.Factory.FromResult(false);

            if(IsDebugEnabled)
                log.Debug("원격 파일이 존재하는지 확인합니다... remoteFilename=[{0}]", remoteFilename);

            var isSuccess = false;

            try {
                var task = ftpClient.GetFileSizeTask(remoteFilename);
                task.Wait();

                isSuccess = task.IsCompleted;

                if(IsDebugEnabled)
                    log.Debug("원격 파일[{0}]의 존재 여부=[{1}]", remoteFilename, isSuccess);
            }
            catch(AggregateException age) {
                age.Handle(ex => true);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("원격 파일 [{0}] 존재를 확인할 때, 예외가 발생했습니다", remoteFilename);
                    log.Warn(ex);
                }
            }

            return Task.Factory.FromResult(isSuccess);
        }

        /// <summary>
        /// 원격 파일의 크기를 조회합니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="remoteFilename"></param>
        /// <returns></returns>
        public static Task<long> GetFileSizeTask(this FtpClient ftpClient, string remoteFilename) {
            if(IsDebugEnabled)
                log.Debug("원격 파일의 크기를 조회합니다... remoteFilename=[{0}]", remoteFilename);

            var uri = ftpClient.Hostname + ftpClient.GetFullPath(remoteFilename);

            return
                ftpClient
                    .GetResponseTask(uri, WebRequestMethods.Ftp.GetFileSize)
                    .ContinueWith(task => {
                                      var fileSize = task.Result.ContentLength;
                                      task.Result.Close();

                                      if(IsDebugEnabled)
                                          log.Debug("원격 파일[{0}] 의 크기는 [{1}] bytes", remoteFilename, fileSize);

                                      return fileSize;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// FTP 서버의 원격 파일명을 변경합니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="srcFilename"></param>
        /// <param name="destFilename"></param>
        /// <returns></returns>
        public static Task<bool> RenameFileTask(this FtpClient ftpClient, string srcFilename, string destFilename) {
            srcFilename.ShouldNotBeWhiteSpace("srcFilename");
            destFilename.ShouldNotBeWhiteSpace("destFilename");

            if(IsDebugEnabled)
                log.Debug("원격 파일명을 변경합니다... srcFilename=[{0}], destFilename=[{1}]", srcFilename, destFilename);

            var src = ftpClient.GetFullPath(srcFilename);
            var dest = ftpClient.GetFullPath(destFilename);

            if(src == dest)
                return Task.Factory.FromResult(true);

            var request = ftpClient.GetRequest(src);
            request.Method = WebRequestMethods.Ftp.Rename;
            request.RenameTo = destFilename;

            return
                ftpClient
                    .GetResponseTask(request)
                    .ContinueWith(task => {
                                      var result = task.IsCompleted;
                                      task.Result.Close();

                                      if(IsDebugEnabled)
                                          log.Debug("원격 파일명을 비동기 방식으로 변경했습니다!!! srcFilename=[{0}], destFilename=[{1}]", srcFilename,
                                                    destFilename);

                                      return result;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// FTP 서버의 원격 파일을 로컬로 비동기 방식으로 다운로드 받습니다.
        /// </summary>
        public static Task<bool> DownloadTask(this FtpClient ftpClient, string remoteFilename, string localFilename,
                                              bool overwrite = false) {
            if(IsDebugEnabled)
                log.Debug("FTP 서버의 원격 파일을 로컬로 비동기 방식으로 다운로드 받습니다... " +
                          @"remoteFilename=[{0}], localFilename=[{1}], overwrite=[{2}]",
                          remoteFilename, localFilename, overwrite);

            var localFi = new FileInfo(localFilename);

            if(overwrite == false && localFi.Exists)
                throw new InvalidOperationException("로컬 파일이 이미 존재합니다. localFilename=" + localFilename);

            return DownloadTask(ftpClient, remoteFilename, localFi);
        }

        /// <summary>
        /// FTP 서버의 원격 파일을 로컬로 비동기 방식으로 다운로드 받습니다.
        /// </summary>
        public static Task<bool> DownloadTask(this FtpClient ftpClient, string remoteFilename, FileInfo localFi) {
            localFi.ShouldNotBeNull("localFi");

            if(IsDebugEnabled)
                log.Debug("원격 파일을 다운로드 받는 작업을 생성합니다... remoteFilename=[{0}], localFi=[{1}]", remoteFilename, localFi.FullName);

            var stream = localFi.OpenWrite();

            return
                DownloadTask(ftpClient, remoteFilename, stream)
                    .ContinueWith(t => {
                                      stream.Close();
                                      return t.Result;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// FTP 서버에 있는 원격 파일을 다운로드하여, <paramref name="localStream"/>에 씁니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="remoteFilename"></param>
        /// <param name="localStream"></param>
        /// <returns></returns>
        public static Task<bool> DownloadTask(this FtpClient ftpClient, string remoteFilename, Stream localStream) {
            localStream.ShouldNotBeNull("localStream");

            if(IsDebugEnabled)
                log.Debug("원격 파일을 다운로드 받는 작업을 생성합니다... remoteFilename=[{0}]", remoteFilename);

            remoteFilename = ftpClient.AdjustRemoteFilename(remoteFilename);
            var uri = ftpClient.Hostname + remoteFilename;

            return
                ftpClient
                    .GetResponseStreamTask(uri, WebRequestMethods.Ftp.DownloadFile)
                    .ContinueWith(task => {
                                      task.Result.CopyStreamToStream(localStream);
                                      task.Result.Close();

                                      if(IsDebugEnabled)
                                          log.Debug("원격 파일 다운로드가 완료되었습니다. remoteFilename=[{0}]", remoteFilename);

                                      return true;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// <paramref name="ftpClient"/>를 이용하여, 로컬 파일 스트림을 FTP 서버로 Upload 합니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="localFilename">로컬 파일 경로</param>
        /// <param name="remoteFilename">원격 파일 경로</param>
        /// <returns></returns>
        public static Task<bool> UploadTask(this FtpClient ftpClient, string localFilename, string remoteFilename = null) {
            if(IsDebugEnabled)
                log.Debug("FTP서버로 로컬 파일을 비동기 방식으로 Upload합니다... localFilename=[{0}], remoteFilename=[{1}]", localFilename, remoteFilename);

            var localFi = new FileInfo(localFilename);
            Guard.Assert(localFi.Exists, "Upload할 로컬 파일이 존재하지 않습니다. localFilename=[{0}]", localFilename);

            return UploadTask(ftpClient, localFi, remoteFilename);
        }

        /// <summary>
        /// <paramref name="ftpClient"/>를 이용하여, 로컬 파일 스트림을 FTP 서버로 Upload 합니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="localFi"></param>
        /// <param name="remoteFilename">원격 파일 경로</param>
        /// <returns></returns>
        public static Task<bool> UploadTask(this FtpClient ftpClient, FileInfo localFi, string remoteFilename = null) {
            localFi.ShouldNotBeNull("localFi");

            if(IsDebugEnabled)
                log.Debug("FTP 서버로 파일[{0}]을 비동기 방식으로 업로드합니다... 원격파일=[{1}]", localFi.FullName, remoteFilename);

            var stream = localFi.OpenRead();
            return
                UploadTask(ftpClient, stream, remoteFilename ?? localFi.Name)
                    .ContinueWith(t => {
                                      stream.Close();
                                      return t.Result;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// <paramref name="ftpClient"/>를 이용하여, 로컬 파일 스트림을 FTP 서버로 Upload 합니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="localStream"></param>
        /// <param name="remoteFilename"></param>
        /// <returns></returns>
        public static Task<bool> UploadTask(this FtpClient ftpClient, Stream localStream, string remoteFilename) {
            if(IsDebugEnabled)
                log.Debug("FTP 서버로 스트림을 비동기 방식으로 업로드합니다... RemoteFile=[{0}]", remoteFilename);

            localStream.ShouldNotBeNull("localStream");
            remoteFilename = ftpClient.AdjustRemoteFilename(remoteFilename);

            var uri = ftpClient.Hostname + remoteFilename;

            return
                GetRequestStreamTask(ftpClient, uri, WebRequestMethods.Ftp.UploadFile)
                    .ContinueWith(task => {
                                      localStream.CopyStreamToStream(task.Result);
                                      task.Result.Close();

                                      if(IsDebugEnabled)
                                          log.Debug("FTP 서버로 파일을 업로드를 완료했습니다. RemoteFile=[{0}]", remoteFilename);

                                      return true;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// FTP 서버에 디렉토리를 비동기 방식으로 생성합니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="remoteDir"></param>
        /// <returns></returns>
        public static Task<bool> CreateDirectoryTask(this FtpClient ftpClient, string remoteDir) {
            if(IsDebugEnabled)
                log.Debug("Create ftp directory. remoteDir=[{0}]", remoteDir);

            string dir = FtpClient.AdjustDir(remoteDir);

            if(dir.Length < 2)
                return Task.Factory.FromResult(true);


            var uri = ftpClient.Hostname + dir;

            var tcs = new TaskCompletionSource<bool>();
            var isSuccess = false;

            try {
                var task = SendRequestOnlyTask(ftpClient, uri, WebRequestMethods.Ftp.MakeDirectory);
                task.Wait();
                isSuccess = task.IsCompleted;

                if(IsDebugEnabled)
                    log.Debug("FTP 서버에 Directory를 생성했습니다. remoteDir=[{0}]", remoteDir);
            }
            catch(AggregateException age) {
                age.Handle(ex => true);
            }

            tcs.TrySetResult(isSuccess);
            return tcs.Task;
        }

        /// <summary>
        /// FTP 서버에 디렉토리를 비동기 방식으로 삭제합니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="remoteDir"></param>
        /// <returns></returns>
        public static Task<bool> DeleteDirectoryTask(this FtpClient ftpClient, string remoteDir) {
            if(IsDebugEnabled)
                log.Debug("Delete remote directory. remoteDir=[{0}]", remoteDir);

            string uri = ftpClient.Hostname + FtpClient.AdjustDir(remoteDir);

            var tcs = new TaskCompletionSource<bool>();
            var isSuccess = false;
            try {
                var task = ftpClient.SendRequestOnlyTask(uri, WebRequestMethods.Ftp.RemoveDirectory);
                task.Wait();
                isSuccess = task.IsCompleted;
            }
            catch(AggregateException age) {
                age.Handle(ex => true);
            }

            tcs.TrySetResult(isSuccess);
            return tcs.Task;
        }

        /// <summary>
        /// FTP 서버의 지정한 디렉토리를 삭제합니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="remoteDir"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public static Task<bool> DeleteDirectoryTask(this FtpClient ftpClient, string remoteDir, bool recursive) {
            remoteDir.ShouldNotBeWhiteSpace("remoteDir");

            if(IsDebugEnabled)
                log.Debug("원격 디렉토리를 비동기 방식으로 삭제합니다... remoteDir=[{0}], recursive=[{1}]", remoteDir, recursive);

            if(ftpClient.DirectoryExistsTask(remoteDir).Result == false)
                return Task.Factory.FromResult(true);

            if(recursive) {
                // 디렉토리에 파일이 있을 경우에는 삭제가 안됩니다!!!
                //
                var subDirs = ftpClient.ListDirectoryTask(remoteDir).Result;

                if(subDirs.Count > 0) {
                    var tasks = new List<Task>();

                    foreach(var dir in subDirs) {
                        var task = ftpClient.DeleteDirectoryTask(remoteDir + FtpClient.FTP_PATH_DELIMITER + dir, true);
                        tasks.Add(task);
                    }

                    Task.WaitAll(tasks.ToArray());
                }
            }
            return ftpClient.DeleteDirectoryTask(remoteDir);
        }

        /// <summary>
        /// FTP 서버의 디렉토리를 열거합니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <returns></returns>
        public static Task<ICollection<string>> ListDirectoryTask(this FtpClient ftpClient) {
            return ListDirectoryTask(ftpClient, string.Empty);
        }

        /// <summary>
        /// FTP 서버의 <paramref name="directory"/>의 하위 디렉토리 및 파일명을 열거합니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static Task<ICollection<string>> ListDirectoryTask(this FtpClient ftpClient, string directory) {
            return
                ftpClient
                    .ListDirectoryInternalTask(directory, WebRequestMethods.Ftp.ListDirectory)
                    .ContinueWith<ICollection<string>>(
                        task => task.Result.Split(new[] { '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                        TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// FTP 서버의 <paramref name="directory"/>의 하위 디렉토리 및 파일명을 열거합니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="directory"></param>
        /// <param name="recursive">재귀 호출 여부</param>
        /// <returns></returns>
        public static Task<IList<string>> ListDirectoryTask(this FtpClient ftpClient, string directory, bool recursive) {
            if(IsDebugEnabled)
                log.Debug("List all entry in the specified directory. directory=[{0}], recursive=[{1}]", directory, recursive);

            var paths = new List<string>();
            directory = FtpClient.AdjustDir(directory);
            paths.Add(directory);

            if(!directory.EndsWith(FtpClient.FTP_PATH_DELIMITER))
                directory += FtpClient.FTP_PATH_DELIMITER;

            var task = ftpClient.ListDirectoryTask(directory);

            if(recursive) {
                task.ContinueWith(dt => dt.Result
                                            .Select(subDir => FtpClient.PathCombine(directory, subDir))
                                            .RunEach(subPath => paths.AddRange(ftpClient.ListDirectoryTask(subPath, true).Result)),
                                  TaskContinuationOptions.ExecuteSynchronously);
            }
            else {
                paths.AddRange(task.Result);
            }

            return Task.Factory.FromResult<IList<string>>(paths);
        }

        /// <summary>
        /// FTP 서버의 <paramref name="directory"/>의 상세 정보를 가져옵니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static Task<FtpDirectory> ListDirectoryDetailTask(this FtpClient ftpClient, string directory = null) {
            return
                ftpClient
                    .ListDirectoryInternalTask(directory, WebRequestMethods.Ftp.ListDirectoryDetails)
                    .ContinueWith(task => new FtpDirectory(task.Result, directory.AsText()),
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// FTP 서버의 <paramref name="directory"/>의 정보를 가져옵니다.
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="directory"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        internal static Task<string> ListDirectoryInternalTask(this FtpClient ftpClient, string directory, string method) {
            if(IsDebugEnabled)
                log.Debug("FTP 서버의 디렉토리의 정보를 가져온다. directory=[{0}], method=[{1}]", directory, method);

            Guard.Assert(method == WebRequestMethods.Ftp.ListDirectory || method == WebRequestMethods.Ftp.ListDirectoryDetails);

            directory = directory ?? string.Empty;

            if(!directory.EndsWith(FtpClient.FTP_PATH_DELIMITER))
                directory += FtpClient.FTP_PATH_DELIMITER;

            return
                ftpClient
                    .GetResponseStringTask(directory, method)
                    .ContinueWith(task => task.Result.Replace("\r\n", "\r").TrimEnd('\r'),
                                  TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}