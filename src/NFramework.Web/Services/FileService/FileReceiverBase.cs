using System.Threading.Tasks;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Web.Services.FileService.Repositories;

namespace NSoft.NFramework.Web.Services.FileService
{
    /// <summary>
    /// Http File을 처리하는 Receiver
    /// </summary>
    public abstract class FileReceiverBase : IFileReceiver
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        protected FileReceiverBase() {}

        /// <summary>
        /// 생성자
        /// </summary>
        protected FileReceiverBase(IFileRepository repository)
        {
            if(IsDebugEnabled)
                log.Debug("==>S repository={0}", repository);

            Repository = repository;
        }

        /// <summary>
        /// 파일 저장소
        /// </summary>
        public IFileRepository Repository { get; protected set; }

        /// <summary>
        /// 파일을 저장합니다.
        /// </summary>
        /// <typeparam name="T">저장할 파일형식</typeparam>
        /// <param name="file">파일정보</param>
        /// <param name="filePath">저장될 파일경로</param>
        /// <returns>저장된 파일명</returns>
        public virtual Task<string> SaveAs<T>(T file, string filePath)
        {
            return SaveAs(file, filePath, false);
        }

        /// <summary>
        /// 파일을 저장합니다.<br/>
        /// overwrite:false 인 경우 동일한 파일명이 있는경우 새로운 파일명으로 저장후 파일명을 반환합니다.
        /// </summary>
        /// <typeparam name="T">저장할 파일형식</typeparam>
        /// <param name="file">파일정보</param>
        /// <param name="filePath">저장될 파일경로</param>
        /// <param name="overwrite">덮어쓰기여부(default:false)</param>
        /// <returns>저장된 파일명</returns>
        public abstract Task<string> SaveAs<T>(T file, string filePath, bool overwrite);

        /// <summary>
        /// 파일삭제
        /// </summary>
        /// <param name="filePaths">삭제할 파일 경로</param>
        public virtual void DeleteFile(params string[] filePaths)
        {
            if(IsDebugEnabled)
                log.Debug("==>S 삭제요청에 대해서 시작합니다. filePaths={0}", filePaths.CollectionToString());

            filePaths.ShouldNotBeNull("삭제할 파일정보가 없습니다.");

            Repository.DeleteFile(filePaths);

            if(IsDebugEnabled)
                log.Debug("==>E 삭제요청에 대해서 완료합니다.");
        }
    }
}