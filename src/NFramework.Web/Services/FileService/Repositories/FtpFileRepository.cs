using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSoft.NFramework.IO;
using NSoft.NFramework.Networks;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Web.Services.FileService.Repositories
{
    /// <summary>
    /// Ftp 파일 저장소
    /// </summary>
    public class FtpFileRepository : FileRepositoryBase
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// FtpClient
        /// </summary>
        private readonly FtpClient _ftpClient;

        private string _rootPath = string.Empty;

        /// <summary>
        /// 저장소 Root 경로
        /// </summary>
        public override string RootPath
        {
            get
            {
                if(_rootPath.EndsWith(FtpClient.FTP_PATH_DELIMITER))
                    return _rootPath;

                return _rootPath + FtpClient.FTP_PATH_DELIMITER;
            }
            protected set { _rootPath = value; }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="hostname">ftp host</param>
        /// <param name="username">접속계정 Id</param>
        /// <param name="password">접속계정 Password</param>
        public FtpFileRepository(string hostname, string username, string password)
        {
            _ftpClient = new FtpClient(hostname, username, password);
        }

        /// <summary>
        /// 디렉터리 존재여부
        /// </summary>
        /// <param name="path">경로</param>
        /// <returns>존재여부</returns>
        public override bool DirectoryExists(string path)
        {
            if(IsDebugEnabled)
                log.Debug("==>S path={0}", path);

            path.ShouldNotBeWhiteSpace("경로정보가 없습니다.");

            path = GetFullPath(path);

            return _ftpClient.DirectoryExists(path);
        }

        /// <summary>
        /// 디렉터리 존재여부
        /// </summary>
        /// <param name="path">경로</param>
        public override void CreateDirectory(string path)
        {
            if(IsDebugEnabled)
                log.Debug("==>S path={0}", path);

            path.ShouldNotBeWhiteSpace("경로정보가 없습니다.");

            path = GetFullPath(path);

            var task = _ftpClient.CreateDirectoryTask(path);

            if(IsDebugEnabled)
                log.Debug("==>E task.Result={0}", task.Result);
        }

        /// <summary>
        /// 파일 존재여부
        /// </summary>
        /// <param name="fileName">파일명</param>
        /// <returns>존재여부</returns>
        public override bool FileExists(string fileName)
        {
            if(IsDebugEnabled)
                log.Debug("==>S fileName={0}", fileName);

            fileName.ShouldNotBeWhiteSpace("파일정보가 없습니다.");

            fileName = GetFullPath(fileName);

            var task = _ftpClient.FileExistsTask(fileName);

            if(IsDebugEnabled)
                log.Debug("==>E task.Result={0}", task.Result);

            return task.Result;
        }

        /// <summary>
        /// 파일 삭제
        /// </summary>
        /// <param name="fileNames">삭제할 파일 목록</param>
        public override void DeleteFile(params string[] fileNames)
        {
            if(IsDebugEnabled)
                log.Debug("==>S 삭제요청에 대해서 시작합니다. fileNames={0}", fileNames.CollectionToString());

            fileNames.ShouldNotBeNull("삭제할 파일정보가 없습니다.");

            if(fileNames.Length > 0)
            {
                var task = Task.Factory.StartNew(() =>
                                                 {
                                                     foreach(var filePath in fileNames)
                                                     {
                                                         var path = string.Empty;
                                                         try
                                                         {
                                                             path = GetFullPath(filePath);

                                                             if(IsDebugEnabled)
                                                                 log.Debug("파일을 삭제합니다. filePath=[{0}], path=[{1}]", filePath, path);

                                                             var deleteFileTask = _ftpClient.DeleteFileTask(path);

                                                             if(IsDebugEnabled)
                                                                 log.Debug("deleteFileTask.Result=[{0}]", deleteFileTask.Result);

                                                             if(IsDebugEnabled)
                                                                 log.Debug("파일을 삭제되었습니다. path=[{0}]", path);
                                                         }
                                                         catch(Exception ex)
                                                         {
                                                             if(log.IsWarnEnabled)
                                                             {
                                                                 log.Warn("파일 삭제중 오류가 발생하였습니다.filePath=[{0}]", path);
                                                                 log.Warn(ex);
                                                             }
                                                         }
                                                     }
                                                 }
                    );

                task.Wait();
            }


            if(IsDebugEnabled)
                log.Debug("==>E 삭제요청에 대해서 완료합니다.");
        }

        /// <summary>
        /// 지정된 파일명이 있다면 새로운 파일명을 생성한다.
        /// </summary>
        /// <param name="fileName">파일명</param>
        /// <returns>새로운 파일명</returns>
        public override string GetNewFileName(string fileName)
        {
            if(IsDebugEnabled)
                log.Debug("==>S fileName={0}", fileName);

            fileName.ShouldNotBeWhiteSpace("파일정보가 없습니다.");
            fileName = GetFullPath(fileName);

            var newFileName = FindNewFileName(fileName);

            if(IsDebugEnabled)
                log.Debug("==>E newFileName={0}", newFileName);

            return newFileName;
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
        private string FindNewFileName(string filename)
        {
            filename.ShouldNotBeWhiteSpace("filename");

            string newFileName = filename;

            if(FileExists(newFileName))
            {
                string path = Path.GetDirectoryName(filename);
                string file = Path.GetFileNameWithoutExtension(filename);
                string ext = Path.GetExtension(filename);

                int n = 1;
                do
                {
                    newFileName = string.Format(@"{0}[{1}]{2}", file, n, ext);
                    newFileName = Path.Combine(path, newFileName);

                    if(!FileExists(newFileName))
                        break;
                    n++;
                } while(n < 10000);
            }

            return newFileName;
        }

        /// <summary>
        /// 저장할 파일경로을 반환합니다.
        /// </summary>
        /// <param name="destFile">대상 파일경로</param>
        /// <param name="overwrite">덮어쓰기 여부</param>
        /// <returns></returns>
        private string GetDestFile(string destFile, bool overwrite)
        {
            if(IsDebugEnabled)
                log.Debug("==>S destFile={0}, overwrite={1}", destFile, overwrite);

            destFile.ShouldNotBeWhiteSpace("저장할 대상파일정보가 없습니다.");

            destFile = GetFullPath(destFile);

            var directory = destFile.ExtractFilePath();

            if(IsDebugEnabled)
                log.Debug("Directory를 생성합니다. directory={0}", directory);

            try
            {
                if(DirectoryExists(directory) == false)
                    CreateDirectory(directory);
            }
            catch(Exception ex)
            {
                if(log.IsWarnEnabled)
                    log.WarnException("이미 Directory 있습니다. directory=" + directory, ex);
            }

            var exists = FileExists(destFile);

            if(IsDebugEnabled)
                log.Debug("파일 존재여부 exists={0}", exists);

            //overwrite이고 파일이 존재한다면
            if(overwrite && exists)
                DeleteFile(destFile);
            else if((overwrite == false) && exists)
                destFile = GetNewFileName(destFile);

            return destFile;
        }

        /// <summary>
        /// 저장소에 저장한다.
        /// </summary>
        /// <param name="stream">저장할 Stream</param>
        /// <param name="destFile">대상 파일경로</param>
        /// <param name="overwrite">덮어쓰기 여부</param>
        /// <returns>저장된 파일명</returns>
        public override Task<string> SaveAs(Stream stream, string destFile, bool overwrite)
        {
            if(IsDebugEnabled)
                log.Debug("==>S stream={0}, destFile={1}, overwrite={2}", stream, destFile, overwrite);

            stream.ShouldNotBeNull("저장할 파일정보가 없습니다.");
            destFile.ShouldNotBeWhiteSpace("저장할 대상파일정보가 없습니다.");

            var destFilePath = destFile;
            var task = Task.Factory.StartNew(() =>
                                             {
                                                 var path = GetDestFile(destFilePath, overwrite);
                                                 var uploadTask = _ftpClient.UploadTask(stream, path);

                                                 if(IsDebugEnabled)
                                                     log.Debug("task.Result={0}", uploadTask.Result);

                                                 return path;
                                             }).ContinueWith((antecedent) => antecedent.Result);
            task.Wait();
            destFile = task.Result;

            if(IsDebugEnabled)
                log.Debug("==>>E task.Result={0}, destFile={1}", task.Result, destFile);

            return task;
        }

        /// <summary>
        /// 저장소에 저장한다.
        /// </summary>
        /// <param name="srcFile">원본 파일경로</param>
        /// <param name="destFile">대상 파일경로</param>
        /// <param name="overwrite">덮어쓰기 여부</param>
        /// <returns>저장된 파일명</returns>
        public override Task<string> SaveAs(string srcFile, string destFile, bool overwrite)
        {
            if(IsDebugEnabled)
                log.Debug("==>S srcFile={0}, destFile={1}, overwrite={2}", srcFile, destFile, overwrite);

            srcFile.ShouldNotBeNull("저장할 파일정보가 없습니다.");
            destFile.ShouldNotBeWhiteSpace("저장할 대상파일정보가 없습니다.");

            var destFilePath = destFile;
            var task = Task.Factory.StartNew(() =>
                                             {
                                                 var path = GetDestFile(destFilePath, overwrite);
                                                 var uploadTask = _ftpClient.UploadTask(srcFile, path);

                                                 if(IsDebugEnabled)
                                                     log.Debug("task.Result={0}", uploadTask.Result);

                                                 return path;
                                             }).ContinueWith((antecedent) => antecedent.Result);
            task.Wait();
            destFile = task.Result;

            if(IsDebugEnabled)
                log.Debug("==>>E task.Result={0}, destFile={1}", task.Result, destFile);

            return task;
        }

        /// <summary>
        /// 저장한 폴더내에 있는 파일목록을 반환합니다.
        /// </summary>
        /// <param name="path">경로</param>
        /// <returns>파일 목록</returns>
        public override string[] GetFiles(string path)
        {
            if(IsDebugEnabled)
                log.Debug("==>S path={0}", path);

            path.ShouldNotBeWhiteSpace("경로정보가 없습니다.");
            try
            {
                path = GetFullPath(path);

                var task = _ftpClient.ListDirectoryDetailTask(path);

                if(IsDebugEnabled)
                    log.Debug("task.Result={0}", task.Result);

                var files = task.Result.GetFiles().Select(item => item.FullName).ToArray();

                if(IsDebugEnabled)
                    log.Debug("==>E files={0}", files.CollectionToString());

                return files;
            }
            catch(AggregateException age)
            {
                //foreach(var innerException in age.Flatten().InnerExceptions)
                //{
                //    if(log.IsErrorEnabled)
                //        log.ErrorException("예외처리", innerException);
                //}
                age.Handle((ex) =>
                           {
                               if(log.IsErrorEnabled)
                                   log.ErrorException("예외처리", ex);

                               return true;
                           });
            }

            return null;
        }

        /// <summary>
        /// 파일의 크기를 반환합니다.
        /// </summary>
        /// <param name="fileName">파일경로</param>
        /// <returns>사이즈</returns>
        public override long GetFileSize(string fileName)
        {
            if(IsDebugEnabled)
                log.Debug("==>S fileName={0}", fileName);

            fileName.ShouldNotBeWhiteSpace("경로정보가 없습니다.");

            fileName = GetFullPath(fileName);

            var task = _ftpClient.GetFileSizeTask(fileName);

            if(IsDebugEnabled)
                log.Debug("==>>E task.Result={0}", task.Result);

            return task.Result;
        }

        /// <summary>
        /// 파일의 Stream 을 반환합니다.
        /// </summary>
        /// <param name="fileName">파일경로</param>
        /// <returns>Stream</returns>
        public override Stream GetFileStream(string fileName)
        {
            if(IsDebugEnabled)
                log.Debug("==>S fileName={0}", fileName);

            fileName.ShouldNotBeWhiteSpace("경로정보가 없습니다.");

            var tempFileName = Path.Combine(FileTool.GetTempPath(), Guid.NewGuid().ToString());

            if(IsDebugEnabled)
                log.Debug("==> 임시파일을 생성합니다. tempFileName={0}", tempFileName);

            var stream = FileTool.GetFileStream(tempFileName, FileOpenMode.ReadWrite);

            var task = _ftpClient.DownloadTask(fileName, stream);

            if(IsDebugEnabled)
                log.Debug("==>>E task.Result={0}", task.Result);

            //임시파일을 삭제한다.
            //tempFileName.DeleteFile(false);

            return stream;
        }

        /// <summary>
        /// Root 경로와 해당 경로를 Combine한 경로를 반환합니다.
        /// </summary>
        /// <param name="fileName">파일 경로</param>
        /// <returns>전체경로</returns>
        public override string GetFullPath(string fileName)
        {
            if(IsDebugEnabled)
                log.Debug("==>S fileName={0}", fileName);

            fileName.ShouldNotBeWhiteSpace("경로정보가 없습니다.");

            if(fileName.Contains(Path.DirectorySeparatorChar))
                fileName = fileName.Replace(Path.DirectorySeparatorChar.ToString(), FtpClient.FTP_PATH_DELIMITER);

            string result = (fileName.StartsWith(FtpClient.FTP_PATH_DELIMITER)) ? fileName : FtpClient.FTP_PATH_DELIMITER + fileName;

            if(result.Length > 1 && result.EndsWith(FtpClient.FTP_PATH_DELIMITER))
                return result.TrimEnd(FtpClient.FTP_PATH_DELIMITER[0]);

            return result;
        }
    }
}