using System;
using System.IO;
using System.Threading.Tasks;
using NSoft.NFramework.IO;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.Web.Services.FileService.Repositories
{
    /// <summary>
    /// 파일저장소 관련 API 제공 서비스
    /// </summary>
    public class DefaultFileRepository : FileRepositoryBase
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        public DefaultFileRepository() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="rootPath">폴더 Root경로</param>
        /// <param name="username">접속계정 Id</param>
        /// <param name="password">접속계정 Password</param>
        public DefaultFileRepository(string rootPath, string username = null, string password = null)
        {
            Init(rootPath, username, password);
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

            return path.DirectoryExists();
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

            path.CreateDirectory();
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

            return fileName.FileExists();
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
                                                                 log.Debug("파일을 삭제합니다. filePath={0}, path={1}", filePath, path);

                                                             path.DeleteFile();
                                                         }
                                                         catch(Exception ex)
                                                         {
                                                             if(log.IsWarnEnabled)
                                                             {
                                                                 log.Warn("파일 삭제중 오류가 발생하였습니다.filePath={0}, message={1}", path, ex.Message);
                                                                 log.Warn(path);
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

            return fileName.GetNewFileName();
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

            if(DirectoryExists(directory) == false)
                CreateDirectory(directory);

            var exists = destFile.FileExists();

            if(IsDebugEnabled)
                log.Debug("파일 존재여부 exists={0}", exists);

            //overwrite이고 파일이 존재한다면
            if(overwrite && exists)
                destFile.DeleteFile();
            else if((overwrite == false) && exists)
                destFile = destFile.GetNewFileName();
            return destFile;
        }

        /// <summary>
        /// 저장소에 저장한다.<br/>
        /// overwrite:false 인 경우 동일한 파일명이 있는경우 새로운 파일명으로 저장후 파일명을 반환합니다.
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
                                                 FileTool.Save(path, stream, overwrite);
                                                 return path;
                                             }).ContinueWith((antecedent) => antecedent.Result);
            task.Wait();
            destFile = task.Result;

            if(IsDebugEnabled)
                log.Debug("==>>E destFile={0}", destFile);

            //return destFile;
            return task;
        }

        /// <summary>
        /// 저장소에 저장한다.<br/>
        /// overwrite:false 인 경우 동일한 파일명이 있는경우 새로운 파일명으로 저장후 파일명을 반환합니다.
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
                                                 srcFile.CopyFile(path, overwrite);
                                                 return path;
                                             }).ContinueWith((antecedent) => antecedent.Result);
            task.Wait();
            destFile = task.Result;

            if(IsDebugEnabled)
                log.Debug("==>>E destFile={0}", destFile);

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

            path = GetFullPath(path);

            return FileTool.GetFiles(path);
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

            return fileName.GetFileSize();
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

            //return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return FileTool.GetFileStream(fileName, FileOpenMode.Read);
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

            string path = (Path.IsPathRooted(fileName)) ? fileName : Path.Combine(RootPath, fileName);

            if(IsDebugEnabled)
                log.Debug("==>E path={0}", path);

            return path;
        }

        /// <summary>
        /// 초기화
        /// </summary>
        /// <param name="rootPath">폴더 Root경로</param>
        /// <param name="username">접속계정 Id</param>
        /// <param name="password">접속계정 Password</param>
        private void Init(string rootPath, string username, string password)
        {
            RootPath = FileTool.GetPhysicalPath(rootPath);
            Username = username;
            Password = password;

            //접속자 정보가 없다면 네트웍 접속정보를 할당한다.
            if(Username.IsNotWhiteSpace())
                NetworkDriveTool.Connect(RootPath, Username, Password, null);
        }
    }
}