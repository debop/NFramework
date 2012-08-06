using NSoft.NFramework.Web.Services.FileService.Repositories;

namespace NSoft.NFramework.Web.Services.FileService
{
    /// <summary>
    /// HttpPostedFile을 이용한 기본 파일 서비스
    /// </summary>
    public class DefaultFileService : FileServiceBase
    {
        /// <summary>
        /// 생성자
        /// </summary>
        public DefaultFileService(IFileRepository repository) : base(repository) {}
    }
}