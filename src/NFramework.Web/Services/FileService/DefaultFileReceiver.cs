using System;
using System.Threading.Tasks;
using System.Web;
using NSoft.NFramework.Web.Services.FileService.Repositories;

namespace NSoft.NFramework.Web.Services.FileService
{
    /// <summary>
    /// Http File을 처리하는 Receiver
    /// </summary>
    public class DefaultFileReceiver : FileReceiverBase
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        public DefaultFileReceiver() : base() {}

        /// <summary>
        /// 생성자
        /// </summary>
        public DefaultFileReceiver(IFileRepository repository) : base(repository) {}

        /// <summary>
        /// 파일을 저장합니다.<br/>
        /// overwrite:false 인 경우 동일한 파일명이 있는경우 새로운 파일명으로 저장후 파일명을 반환합니다.
        /// </summary>
        /// <param name="file">파일정보</param>
        /// <param name="filePath">저장될 파일경로</param>
        /// <param name="overwrite">덮어쓰기여부(default:false)</param>
        public override Task<string> SaveAs<T>(T file, string filePath, bool overwrite)
        {
            if(IsDebugEnabled)
                log.Debug("==>>S file={0}, filePath={1}, overwrite={2}", file, filePath, overwrite);

            var httpPostedFile = file as HttpPostedFile;

            if(httpPostedFile == null)
                throw new NotSupportedException(string.Format("지원하지 않은 타입입니다.HttpPostedFile 만 지원합니다.file={0}", file));

            //filePath = Repository.SaveAs(httpPostedFile.InputStream, filePath, overwrite);
            var task = Repository.SaveAs(httpPostedFile.InputStream, filePath, overwrite);

            if(IsDebugEnabled)
                log.Debug("==>>E filePath={0}", filePath);

            return task;
        }
    }
}