using System.IO;
using System.Threading.Tasks;

namespace NSoft.NFramework.Web.Services.FileService.Repositories
{
    /// <summary>
    /// 파일저장소 관련 API 제공 서비스
    /// </summary>
    public abstract class FileRepositoryBase : IFileRepository
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 저장소 Root 경로
        /// </summary>
        public virtual string RootPath { get; protected set; }

        /// <summary>
        /// 접속계정 Id
        /// </summary>
        public virtual string Username { get; protected set; }

        /// <summary>
        /// 접속계정 Password
        /// </summary>
        public virtual string Password { get; protected set; }

        /// <summary>
        /// 디렉터리 존재여부
        /// </summary>
        /// <param name="path">경로</param>
        /// <returns>존재여부</returns>
        public abstract bool DirectoryExists(string path);

        /// <summary>
        /// 디렉터리 존재여부
        /// </summary>
        /// <param name="path">경로</param>
        public abstract void CreateDirectory(string path);

        /// <summary>
        /// 파일 존재여부
        /// </summary>
        /// <param name="fileName">파일명</param>
        /// <returns>존재여부</returns>
        public abstract bool FileExists(string fileName);

        /// <summary>
        /// 파일 삭제
        /// </summary>
        /// <param name="fileNames">삭제할 파일 목록</param>
        public abstract void DeleteFile(params string[] fileNames);

        /// <summary>
        /// 지정된 파일명이 있다면 새로운 파일명을 생성한다.
        /// </summary>
        /// <param name="fileName">파일명</param>
        /// <returns>새로운 파일명</returns>
        public abstract string GetNewFileName(string fileName);

        /// <summary>
        /// 저장소에 저장한다.
        /// </summary>
        /// <param name="stream">저장할 Stream</param>
        /// <param name="destFile">대상 파일경로</param>
        /// <param name="overwrite">덮어쓰기 여부</param>
        /// <returns>저장된 파일명</returns>
        public abstract Task<string> SaveAs(Stream stream, string destFile, bool overwrite);

        /// <summary>
        /// 저장소에 저장한다.
        /// </summary>
        /// <param name="srcFile">원본 파일경로</param>
        /// <param name="destFile">대상 파일경로</param>
        /// <param name="overwrite">덮어쓰기 여부</param>
        /// <returns>저장된 파일명</returns>
        public abstract Task<string> SaveAs(string srcFile, string destFile, bool overwrite);

        /// <summary>
        /// 저장한 폴더내에 있는 파일목록을 반환합니다.
        /// </summary>
        /// <param name="path">경로</param>
        /// <returns>파일 목록</returns>
        public abstract string[] GetFiles(string path);

        /// <summary>
        /// 파일의 크기를 반환합니다.
        /// </summary>
        /// <param name="fileName">파일경로</param>
        /// <returns>사이즈</returns>
        public abstract long GetFileSize(string fileName);

        /// <summary>
        /// 파일의 Stream 을 반환합니다.
        /// </summary>
        /// <param name="fileName">파일경로</param>
        /// <returns>Stream</returns>
        public abstract Stream GetFileStream(string fileName);

        /// <summary>
        /// Root 경로와 해당 경로를 Combine한 경로를 반환합니다.
        /// </summary>
        /// <param name="fileName">파일 경로</param>
        /// <returns>전체경로</returns>
        public virtual string GetFullPath(string fileName)
        {
            if(IsDebugEnabled)
                log.Debug("==>S fileName={0}", fileName);

            fileName.ShouldNotBeWhiteSpace("경로정보가 없습니다.");

            string path = (Path.IsPathRooted(fileName)) ? fileName : Path.Combine(RootPath, fileName);

            if(IsDebugEnabled)
                log.Debug("==>E path={0}", path);

            return path;
        }
    }
}