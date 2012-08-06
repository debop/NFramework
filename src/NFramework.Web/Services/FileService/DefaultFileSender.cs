using NSoft.NFramework.Web.Services.FileService.Repositories;

namespace NSoft.NFramework.Web.Services.FileService
{
    /// <summary>
    /// Http File을 처리하는 Sender
    /// </summary>
    public class DefaultFileSender : FileSenderBase
    {
        /// <summary>
        /// 생성자
        /// </summary>
        public DefaultFileSender() : base() {}

        /// <summary>
        /// 생성자
        /// </summary>
        public DefaultFileSender(IFileRepository repository) : base(repository) {}
    }
}