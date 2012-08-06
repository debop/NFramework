using System;
using System.Net;
using NSoft.NFramework.Parallelism.Tools;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// 웹의 리소스 정보를 비동기 방식으로 다운로드 받아 저장해 놓는 캐시입니다.
    /// </summary>
    [Serializable]
    public sealed class HtmlAsyncCache : AsyncCache<Uri, string> {
        /// <summary>
        /// 생성자
        /// </summary>
        public HtmlAsyncCache() : base(uri => new WebClient().DownloadStringTask(uri)) {}
    }
}