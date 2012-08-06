using System.Threading.Tasks;
using NSoft.NFramework.Web.Services.FileService.Repositories;

namespace NSoft.NFramework.Web.Services.FileService
{
    /// <summary>
    /// Http File을 처리하는 Receiver
    /// </summary>
    public interface IFileReceiver
    {
        /// <summary>
        /// 파일 저장소
        /// </summary>
        IFileRepository Repository { get; }

        /// <summary>
        /// 파일을 저장합니다.
        /// </summary>
        /// <typeparam name="T">저장할 파일형식</typeparam>
        /// <param name="file">파일정보</param>
        /// <param name="filePath">저장될 파일경로</param>
        /// <returns>저장된 파일명</returns>
        Task<string> SaveAs<T>(T file, string filePath);

        /// <summary>
        /// 파일을 저장합니다.<br/>
        /// overwrite:false 인 경우 동일한 파일명이 있는경우 새로운 파일명으로 저장후 파일명을 반환합니다.
        /// </summary>
        /// <typeparam name="T">저장할 파일형식</typeparam>
        /// <param name="file">파일정보</param>
        /// <param name="filePath">저장될 파일경로</param>
        /// <param name="overwrite">덮어쓰기여부(default:false)</param>
        /// <returns>저장된 파일명</returns>
        Task<string> SaveAs<T>(T file, string filePath, bool overwrite);

        /// <summary>
        /// 파일삭제
        /// </summary>
        /// <param name="filePaths">삭제할 파일 경로</param>
        void DeleteFile(params string[] filePaths);
    }
}