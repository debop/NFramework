using System;
using System.Web.UI.WebControls.Adapters;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.Web.ControlAdapters {
    /// <summary>
    /// ASP Image Control의 ImageUrl이 Full Path인 경우, 브라우저의 다운로드가 빠르다. 이 점에 착안하여, IamgeUrl 이 full path를 가지도록 합니다.
    /// </summary>
    public class AspImageControlAdapter : WebControlAdapter {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// ASP Image control의 ImageUrl 가 http로 시작하지 않고, 상대 경로를 사용하다면, 절대경로로 바꾼다. 
        /// 이렇게 하면 여러 이미지를 한꺼번에 다운로드 받아 페이지 로딩 속도가 빨라집니다.
        /// </summary>
        /// <param name="writer"></param>
        protected override void BeginRender(System.Web.UI.HtmlTextWriter writer) {
            var image = Control as System.Web.UI.WebControls.Image;

            if(image != null && image.ImageUrl.IsNotWhiteSpace()) {
                if(image.ImageUrl.StartsWith("http", StringComparison.InvariantCultureIgnoreCase) == false) {
                    image.ImageUrl = WebTool.ServerName + Page.ResolveUrl(image.ImageUrl).ToLower();

                    if(IsDebugEnabled)
                        log.Debug("ASP Image Control의 ImageUrl이 상대경로로 되어있어 절대경로로 변경했습니다. image.ImageUrl=[{0}]", image.ImageUrl);
                }
            }

            base.BeginRender(writer);
        }
    }
}