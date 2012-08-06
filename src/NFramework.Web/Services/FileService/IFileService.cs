namespace NSoft.NFramework.Web.Services.FileService
{
    /// <summary>
    /// 파일 저장 및 다운로드 관련 서비스
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// HttpFile Receiver
        /// </summary>
        IFileReceiver Receiver { get; }

        /// <summary>
        /// HttpFile Sender
        /// </summary>
        IFileSender Sender { get; }
    }
}