using System.Web;
using NSoft.NFramework.Web.Services.FileService.Repositories;

namespace NSoft.NFramework.Web.Services.FileService
{
    /// <summary>
    /// Http File을 처리하는 Sender
    /// </summary>
    public interface IFileSender
    {
        /// <summary>
        /// 파일 저장소
        /// </summary>
        IFileRepository Repository { get; }

        /// <summary>
        /// 파일을 클라이언트에 내려준다.
        /// </summary>
        /// <param name="ctx">HttpContext</param>
        /// <param name="fileItem">파일정보</param>
        /// <param name="attach">파일 다운로드 방식</param>
        void Download(HttpContext ctx, FileItem fileItem, bool attach);
    }
}