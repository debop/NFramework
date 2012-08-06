using System.IO;
using System.Threading.Tasks;

namespace NSoft.NFramework.Web.Services.FileService.Repositories
{
    /// <summary>
    /// 파일저장소 관련 API 제공 서비스
    /// </summary>
    public interface IFileRepository
    {
        /// <summary>
        /// 저장소 Root 경로
        /// </summary>
        string RootPath { get; }

        /// <summary>
        /// 접속계정 Id
        /// </summary>
        string Username { get; }

        /// <summary>
        /// 접속계정 Password
        /// </summary>
        string Password { get; }

        /// <summary>
        /// 디렉터리 존재여부
        /// </summary>
        /// <param name="path">경로</param>
        /// <returns>존재여부</returns>
        bool DirectoryExists(string path);

        /// <summary>
        /// 디렉터리 존재여부
        /// </summary>
        /// <param name="path">경로</param>
        void CreateDirectory(string path);

        /// <summary>
        /// 파일 존재여부
        /// </summary>
        /// <param name="fileName">파일명</param>
        /// <returns>존재여부</returns>
        bool FileExists(string fileName);

        /// <summary>
        /// 파일 삭제
        /// </summary>
        /// <param name="fileNames">삭제할 파일 목록</param>
        void DeleteFile(params string[] fileNames);

        /// <summary>
        /// 지정된 파일명이 있다면 새로운 파일명을 생성한다.
        /// </summary>
        /// <param name="fileName">파일명</param>
        /// <returns>새로운 파일명</returns>
        string GetNewFileName(string fileName);

        /// <summary>
        /// 저장소에 저장한다.<br/>
        /// overwrite:false 인 경우 동일한 파일명이 있는경우 새로운 파일명으로 저장후 파일명을 반환합니다.
        /// </summary>
        /// <param name="stream">저장할 Stream</param>
        /// <param name="destFile">대상 파일경로</param>
        /// <param name="overwrite">덮어쓰기 여부</param>
        /// <returns>저장된 파일명</returns>
        Task<string> SaveAs(Stream stream, string destFile, bool overwrite);

        /// <summary>
        /// 저장소에 저장한다.<br/>
        /// overwrite:false 인 경우 동일한 파일명이 있는경우 새로운 파일명으로 저장후 파일명을 반환합니다.
        /// </summary>
        /// <param name="srcFile">원본 파일경로</param>
        /// <param name="destFile">대상 파일경로</param>
        /// <param name="overwrite">덮어쓰기 여부</param>
        /// <returns>저장된 파일명</returns>
        Task<string> SaveAs(string srcFile, string destFile, bool overwrite);

        /// <summary>
        /// 저장한 폴더내에 있는 파일목록을 반환합니다.
        /// </summary>
        /// <param name="path">경로</param>
        /// <returns>파일 목록</returns>
        string[] GetFiles(string path);

        /// <summary>
        /// 파일의 크기를 반환합니다.
        /// </summary>
        /// <param name="fileName">파일경로</param>
        /// <returns>사이즈</returns>
        long GetFileSize(string fileName);

        /// <summary>
        /// 파일의 Stream 을 반환합니다.
        /// </summary>
        /// <param name="fileName">파일경로</param>
        /// <returns>Stream</returns>
        Stream GetFileStream(string fileName);
    }
}