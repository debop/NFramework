using System.IO;
using System.Web;
using NSoft.NFramework.IO;
using NSoft.NFramework.Web.Services.FileService.Repositories;

namespace NSoft.NFramework.Web.Services.FileService
{
    /// <summary>
    /// Http File을 처리하는 Sender
    /// </summary>
    public abstract class FileSenderBase : IFileSender
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 파일 저장소
        /// </summary>
        public IFileRepository Repository { get; protected set; }

        /// <summary>
        /// 생성자
        /// </summary>
        protected FileSenderBase() {}

        /// <summary>
        /// 생성자
        /// </summary>
        protected FileSenderBase(IFileRepository repository)
        {
            repository.ShouldNotBeNull("repository");

            if(IsDebugEnabled)
                log.Debug("FindSenderBase 인스턴스가 생성되었습니다. repository={0}", repository);

            Repository = repository;
        }

        /// <summary>
        /// 파일을 클라이언트에 내려준다.
        /// </summary>
        /// <param name="ctx">HttpContext</param>
        /// <param name="fileItem">파일정보</param>
        /// <param name="attach">파일 다운로드 방식</param>
        public virtual void Download(HttpContext ctx, FileItem fileItem, bool attach)
        {
            if(IsDebugEnabled)
                log.Debug("==>S ctx={0}, fileItem={1}, attach={2}", ctx, fileItem, attach);

            ctx.ShouldNotBeNull("HttpContext정보가 없습니다.");
            fileItem.ShouldNotBeNull("파일정보가 없습니다.");

            //if(Repository.FileExists(fileItem.ServerFileName) == false)
            //    throw new FileNotFoundException("파일이 존재하지 않습니다.", fileItem.ToString());

            BuildHeader(ctx, fileItem, attach);
            WriteFile(ctx, fileItem.ServerFileName);
        }

        /// <summary>
        /// Header 값을 추가한다.
        /// </summary>
        /// <param name="ctx">HttpContext</param>
        /// <param name="fileItem">클라이언트로 내려줄 파일 정보</param>
        /// <param name="attach">Content-Disposition의 Type<br/>
        /// false : inline<br/>
        /// true : attachment</param>
        public virtual void BuildHeader(HttpContext ctx, FileItem fileItem, bool attach)
        {
            if(IsDebugEnabled)
                log.Debug("==>S ctx={0}, fileItem={1}, attach={2}", ctx, fileItem, attach);

            ctx.ShouldNotBeNull("HttpContext가 없습니다.");
            fileItem.ShouldNotBeNull("파일정보가 없습니다.");

            ctx.Response.ContentType = fileItem.ContentType;
            ctx.Response.Expires = 0;

            AddHeader(ctx, fileItem, attach);
        }

        /// <summary>
        /// 실제 파일 내용을 읽어 Response.OutputStream에 쓴다.
        /// </summary>
        /// <param name="ctx">HttpContext</param>
        /// <param name="path">파일 경로</param>
        public virtual void WriteFile(HttpContext ctx, string path)
        {
            if(IsDebugEnabled)
                log.Debug("==>S path={0}, ctx={1}", path, ctx);

            if(Repository.FileExists(path) == false)
                throw new FileNotFoundException("파일이 존재하지 않습니다.", path);

            using(var fs = Repository.GetFileStream(path))
            using(var bs = new BufferedStream(fs))
            {
                var buffer = new byte[FileTool.DEFAULT_BUFFER_SIZE];

                while(ctx.Response.IsClientConnected)
                {
                    int readCount = bs.Read(buffer, 0, FileTool.DEFAULT_BUFFER_SIZE);
                    if(readCount == 0) break;

                    ctx.Response.OutputStream.Write(buffer, 0, readCount);
                    ctx.Response.Flush();
                }
            }
            ctx.Response.Flush();
        }

        /// <summary>
        /// Header 값을 추가한다.
        /// </summary>
        /// <param name="ctx">HttpContext</param>
        /// <param name="fileItem">클라이언트로 내려줄 파일 정보</param>
        /// <param name="attach">Content-Disposition의 Type<br/>
        /// false : inline<br/>
        /// true : attachment</param>
        public static void AddHeader(HttpContext ctx, FileItem fileItem, bool attach)
        {
            var downloadFileMode = new string[] {"inline", "attachment"};
            var attachMode = (attach) ? 1 : 0;

            ctx.Response.AddHeader("Pragma", "no-cache");
            ctx.Response.AddHeader("cache-control", "private, no-cache, must-revalidate no-store pre-check=0 post-check=0 max-stale=0");
            ctx.Response.AddHeader("Expires", "0");
            ctx.Response.AddHeader("Content-Disposition", string.Format("{0};filename={1}", downloadFileMode[attachMode], DownloadFileName(fileItem.GetName())));
        }

        /// <summary>
        /// 다운로드 파일명
        /// </summary>
        /// <param name="fileName">파일명</param>
        /// <returns>다운로드 파일명</returns>
        public static string DownloadFileName(string fileName)
        {
            return fileName.Replace(" ", "%20");
        }
    }
}