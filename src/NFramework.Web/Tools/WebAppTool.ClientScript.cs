using System.Text;
using System.Web.UI.WebControls;

namespace NSoft.NFramework.Web.Tools
{
    /// <summary>
    /// 웹 Application에서 공통으로 사용할 Utility Class입니다.
    /// </summary>
    public static partial class WebAppTool
    {
        /// <summary>
        /// Wraps the script tag.
        /// </summary>
        /// <param name="scripts">The scripts.</param>
        /// <returns>The script.</returns>
        public static string WrapScriptTag(params string[] scripts)
        {
            if(scripts != null && scripts.Length > 0)
            {
                var sb = new StringBuilder();
                sb.Append("\r\n<script language=\"javascript\" type=\"text/javascript\">\r\n<!--\r\n");

                foreach(string script in scripts)
                {
                    sb.Append(script.EndsWith(";") || script.EndsWith("}") ? script : script + ";");
                }

                sb.Append("\r\n//-->\r\n</script>\r\n");
                return sb.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Calls the client validator.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="validators">The validators.</param>
        /// <returns>The script.</returns>
        internal static string CallClientValidator(string prefix, params BaseValidator[] validators)
        {
            if(validators != null && validators.Length > 0)
            {
                var sb = new StringBuilder();
                foreach(BaseValidator validator in validators)
                {
                    sb.Append(string.Format(" ValidatorValidate({1}{0}); ", validator.ID, prefix));
                }
                return sb.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Toes the js string array.
        /// </summary>
        /// <param name="strs">The STRS.</param>
        /// <returns>The script.</returns>
        public static string ToJsStringArray(params string[] strs)
        {
            if(strs != null && strs.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(" new Array(");

                foreach(string str in strs)
                {
                    sb.Append(string.Format("'{0}', ", str.Replace("'", "\\'")));
                }

                return sb.ToString().TrimEnd(',', ' ') + ");";
            }

            return " new Array;";
        }
    }
}