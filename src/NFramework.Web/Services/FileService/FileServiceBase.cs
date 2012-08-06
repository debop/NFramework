using System;
using System.Collections.Generic;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Web.Services.FileService.Repositories;

namespace NSoft.NFramework.Web.Services.FileService
{
    /// <summary>
    /// 파일 관련 서비스 Base
    /// </summary>
    public abstract class FileServiceBase : IFileService
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion
        
        /// <summary>
        /// HttpFile Receiver
        /// </summary>
        public IFileReceiver Receiver { get; protected set; }

        /// <summary>
        /// HttpFile Sender
        /// </summary>
        public IFileSender Sender { get; protected set; }

        /// <summary>
        /// 생성자
        /// </summary>
        protected FileServiceBase(IFileRepository repository)
        {
            if(IsDebugEnabled)
                log.Debug("FileSerivce 인스턴스를 생성합니다... repository=[{0}]", repository);

            var parameters = new Dictionary<object, object> {{"repository", repository}};

            Receiver = IoC.Resolve<IFileReceiver>(parameters);
            Sender = IoC.Resolve<IFileSender>(parameters);
        }
    }
}