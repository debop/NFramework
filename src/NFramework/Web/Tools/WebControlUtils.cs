using System.IO;
using System.Web.UI;

namespace NSoft.NFramework.Web {
    /// <summary>
    /// UserControl 조작에 사용되는 Utility Class
    /// </summary>
    public static class WebControlUtils {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <see cref="Control"/>를 Rendering 해서 문자열로 반환한다.
        /// </summary>
        /// <param name="ctrl">control to render as string</param>
        /// <returns>string of control</returns>
        public static string RenderControlAsString(this System.Web.UI.Control ctrl) {
            if(IsDebugEnabled)
                log.Debug("웹 컨트롤을 문자열로 렌더링을 수행합니다... ctrl=[{0}]", ctrl);

            string result;

            using(var sw = new StringWriter())
            using(var writer = new HtmlTextWriter(sw)) {
                ctrl.RenderControl(writer);
                result = sw.ToString();
            }

            if(IsDebugEnabled)
                log.Debug("웹 컨트롤을 문자열로 렌더링을 수행했습니다. result=[{0}]", result);

            return result;
        }
    }
}