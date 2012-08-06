using System.Text;
using System.Web.UI.WebControls;

namespace NSoft.NFramework.Web {
    /// <summary>
    /// Client Script (javascript, vbscript)를 생성하기 위한 Utility Class
    /// </summary>
    public static class ClientScriptUtils {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Toes the js single quote safe string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>The formated str.</returns>
        public static string ToJsSingleQuoteSafeString(this string str) {
            return str.Replace("'", "\\'");
        }

        /// <summary>
        /// Toes the js double quote safe string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>The formated str.</returns>
        public static string ToJsDoubleQuoteSafeString(this string str) {
            return str.Replace("\"", "\\\"");
        }

        /// <summary>
        /// Toes the VBS quote safe string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>The formated str.</returns>
        public static string ToVbsQuoteSafeString(this string str) {
            return str.Replace("\"", "\"\"");
        }

        /// <summary>
        /// Wraps the script tag.
        /// </summary>
        /// <param name="scripts">The scripts.</param>
        /// <returns>The script.</returns>
        public static string WrapScriptTag(params string[] scripts) {
            if(scripts != null && scripts.Length > 0) {
                var sb = new StringBuilder();
                sb.Append("\r\n<script language=\"javascript\" type=\"text/javascript\">\r\n<!--\r\n");

                foreach(string script in scripts) {
                    sb.Append(script.EndsWith(";") || script.EndsWith("}") ? script : script + ";");
                }

                sb.Append("\r\n//-->\r\n</script>\r\n");
                return sb.ToString();
            }
            else {
                return string.Empty;
            }
        }

        /// <summary>
        /// Pops the alert.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns>The script.</returns>
        public static string PopAlert(this string msg) {
            return string.Format(" window.alert('{0}'); ", ToJsSingleQuoteSafeString(msg));
        }

        /// <summary>
        /// Pops the confirm.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns>The script.</returns>
        public static string PopConfirm(this string msg) {
            return string.Format(" window.confirm('{0}') ", ToJsSingleQuoteSafeString(msg));
        }

        /// <summary>
        /// Pops the prompt.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The script.</returns>
        public static string PopPrompt(this string msg, string defaultValue) {
            return
                string.Format(" window.prompt('{0}', '{1}') ", ToJsSingleQuoteSafeString(msg),
                              ToJsSingleQuoteSafeString(defaultValue));
        }

        /// <summary>
        /// Closes the self.
        /// </summary>
        /// <returns>The script.</returns>
        public static string CloseSelf() {
            return " window.close(); ";
        }

        /// <summary>
        /// Closes the parent.
        /// </summary>
        /// <returns>The script.</returns>
        public static string CloseParent() {
            return " if (window.parent) { window.parent.close(); } ";
        }

        /// <summary>
        /// Closes the opener.
        /// </summary>
        /// <returns>The script.</returns>
        public static string CloseOpener() {
            return " if (window.opener) { window.opener.close(); } ";
        }

        /// <summary>
        /// Refreshes the self.
        /// </summary>
        /// <returns>The script.</returns>
        public static string RefreshSelf() {
            return " window.location += ' '; ";
        }

        /// <summary>
        /// Refreshes the opener.
        /// </summary>
        /// <returns>The script.</returns>
        public static string RefreshOpener() {
            return " if (window.opener) { window.opener.location += ' '; } ";
        }

        /// <summary>
        /// Refreshes the parent.
        /// </summary>
        /// <returns>The script.</returns>
        public static string RefreshParent() {
            return " if (window.parent) { window.parent.location += ' '; } ";
        }

        /// <summary>
        /// Shows the modal dialog.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="status">if set to <c>true</c> [status].</param>
        /// <param name="resizable">if set to <c>true</c> [resizable].</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="top">The top.</param>
        /// <param name="left">The left.</param>
        /// <param name="scroll">if set to <c>true</c> [scroll].</param>
        /// <returns>The script.</returns>
        public static string ShowModalDialog(string url, bool status, bool resizable, int height, int width, int top, int left,
                                             bool scroll = false) {
            return
                string.Format(
                    " window.showModalDialog('{0}', window, 'status={1},resizable={2},dialogHeight={3}px,dialogWidth={4}px,dialogTop={5},dialogLeft={6},scroll={7},unadorne=yes'); ",
                    ToJsSingleQuoteSafeString(url), (status ? 1 : 0), (resizable ? 1 : 0), height, width, top, left, (scroll ? 1 : 0));
        }

        /// <summary>
        /// Shows the modal dialog.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="status">if set to <c>true</c> [status].</param>
        /// <param name="resizable">if set to <c>true</c> [resizable].</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="center">if set to <c>true</c> [center].</param>
        /// <param name="scroll">if set to <c>true</c> [scroll].</param>
        /// <returns>The script.</returns>
        public static string ShowModalDialog(string url, bool status, bool resizable, int height, int width, bool center,
                                             bool scroll = false) {
            return
                string.Format(
                    " window.showModalDialog('{0}', window, 'status={1},resizable={2},dialogHeight={3}px,dialogWidth={4}px,center={5},scroll={6},unadorne=yes'); ",
                    ToJsSingleQuoteSafeString(url), (status ? 1 : 0), (resizable ? 1 : 0), height, width, (center ? 1 : 0),
                    (scroll ? 1 : 0));
        }

        /// <summary>
        /// Shows the modeless dialog.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="status">if set to <c>true</c> [status].</param>
        /// <param name="resizable">if set to <c>true</c> [resizable].</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="top">The top.</param>
        /// <param name="left">The left.</param>
        /// <param name="scroll">if set to <c>true</c> [scroll].</param>
        /// <returns>The script.</returns>
        public static string ShowModelessDialog(string url, bool status, bool resizable, int height, int width, int top, int left,
                                                bool scroll = true) {
            return
                string.Format(
                    " window.showModelessDialog('{0}', window, 'status={1},resizable={2},dialogHeight={3}px,dialogWidth={4}px,dialogTop={5},dialogLeft={6},scroll={7},unadorne=yes'); ",
                    ToJsSingleQuoteSafeString(url), (status ? 1 : 0), (resizable ? 1 : 0), height, width, top, left, (scroll ? 1 : 0));
        }

        /// <summary>
        /// Shows the modeless dialog.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="status">if set to <c>true</c> [status].</param>
        /// <param name="resizable">if set to <c>true</c> [resizable].</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="center">if set to <c>true</c> [center].</param>
        /// <param name="scroll">if set to <c>true</c> [scroll].</param>
        /// <returns>The script.</returns>
        public static string ShowModelessDialog(string url, bool status, bool resizable, int height, int width, bool center,
                                                bool scroll = true) {
            return
                string.Format(
                    " window.showModelessDialog('{0}', window, 'status={1},resizable={2},dialogHeight={3}px,dialogWidth={4}px,center={5},scroll={6},unadorne=yes'); ",
                    ToJsSingleQuoteSafeString(url), (status ? 1 : 0), (resizable ? 1 : 0), height, width, (center ? 1 : 0),
                    (scroll ? 1 : 0));
        }

        /// <summary>
        /// Selfs the go back.
        /// </summary>
        /// <returns>The script.</returns>
        public static string SelfGoBack() {
            return " window.history.back(); ";
        }

        /// <summary>
        /// Parents the go back.
        /// </summary>
        /// <returns>The script.</returns>
        public static string ParentGoBack() {
            return " if (window.parent) { window.parent.history.back(); } ";
        }

        /// <summary>
        /// Openers the go back.
        /// </summary>
        /// <returns>The script.</returns>
        public static string OpenerGoBack() {
            return " if (window.opener) { window.opener.history.back(); } ";
        }

        /// <summary>
        /// Opens the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="frameName">Name of the frame.</param>
        /// <param name="status">if set to <c>true</c> [status].</param>
        /// <param name="location">if set to <c>true</c> [location].</param>
        /// <param name="menubar">if set to <c>true</c> [menubar].</param>
        /// <param name="resizable">if set to <c>true</c> [resizable].</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="top">The top.</param>
        /// <param name="left">The left.</param>
        /// <param name="scrollbars">if set to <c>true</c> [scrollbars].</param>
        /// <param name="toolbar">if set to <c>true</c> [toolbar].</param>
        /// <returns>The script.</returns>
        public static string Open(string url, string frameName, bool status, bool location, bool menubar,
                                  bool resizable, int height, int width, int top, int left, bool scrollbars = true, bool toolbar = true) {
            return
                string.Format(
                    " window.open('{0}', '{1}', 'status={2},location={3},menubar={4},resizable={5},height={6}px,width={7}px,top={8},left={9},scrollbars={10},toolbar={11}'); ",
                    ToJsSingleQuoteSafeString(url), ToJsSingleQuoteSafeString(frameName), (status ? 1 : 0), (location ? 1 : 0),
                    (menubar ? 1 : 0), (resizable ? 1 : 0), height, width, top, left, (scrollbars ? 1 : 0), (toolbar ? 1 : 0));
        }

        /// <summary>
        /// Opens the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="frameName">Name of the frame.</param>
        /// <returns>The script.</returns>
        public static string Open(string url, string frameName) {
            return
                string.Format(" window.open('{0}', '{1}'); ", ToJsSingleQuoteSafeString(url), ToJsSingleQuoteSafeString(frameName));
        }

        /// <summary>
        /// Calls the client validator.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="validators">The validators.</param>
        /// <returns>The script.</returns>
        internal static string CallClientValidator(this string prefix, params BaseValidator[] validators) {
            if(validators != null && validators.Length > 0) {
                var sb = new StringBuilder();
                foreach(BaseValidator validator in validators) {
                    sb.Append(string.Format(" ValidatorValidate({1}{0}); ", validator.ID, prefix));
                }
                return sb.ToString();
            }
            else {
                return string.Empty;
            }
        }

        /// <summary>
        /// Toes the js string array.
        /// </summary>
        /// <param name="strs">The STRS.</param>
        /// <returns>The script.</returns>
        public static string ToJsStringArray(params string[] strs) {
            if(strs != null && strs.Length > 0) {
                var sb = new StringBuilder();
                sb.Append(" new Array(");

                foreach(string str in strs) {
                    sb.Append(string.Format("'{0}', ", str.Replace("'", "\\'")));
                }

                return sb.ToString().TrimEnd(',', ' ') + ");";
            }
            else {
                return " new Array;";
            }
        }
    }
}