using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web.Tools
{
    /// <summary>
    /// 웹 Application에서 공통으로 사용할 Utility Class입니다.
    /// </summary>
    public static partial class WebAppTool
    {
        /// <summary>
        /// Url에서 ? 구분자로 호출하려는 서버페이지
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUri(string url)
        {
            string str = string.Empty;
            str = url;

            if(str.IndexOf("?") > -1)
            {
                str = str.Substring(0, str.IndexOf("?"));
            }

            return str;
        }

        /// <summary>
        /// Url에서 ? 구분자로 Parameter 반환한다.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetParamString(string url)
        {
            string str = string.Empty;
            str = url;

            if(str.IndexOf("?") > -1)
                str = str.Substring(str.IndexOf("?") + 1);
            else
                str = string.Empty;

            return str;
        }

        /// <summary>
        /// Url에서 ? 구분자로 Parameter 반환한다.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string[] GetParams(string url)
        {
            return StringTool.Split(GetParamString(url).Replace("&amp;", "&", true), '&');
        }

        /// <summary>
        /// Url에서 ? 구분자로 Parameter 반환한다.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static NameValueCollection GetParam(string url)
        {
            var cols = new System.Collections.Specialized.NameValueCollection();

            if(url.IndexOf("?") < 0)
                return cols;

            string[] param = GetParams(url);

            foreach(string pr in param)
            {
                string[] p = StringTool.Split(pr, '=');

                if(p.Length < 2)
                    continue;

                if(string.IsNullOrEmpty(p[0]))
                    continue;

                cols.Add(p[0], (p.Length > 1) ? p[1] : string.Empty);
            }

            return cols;
        }

        /// <summary>
        /// url0 + url1 + param 합하여 반환한다.
        /// </summary>
        /// <param name="url0"></param>
        /// <param name="url1"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string UrlParamConcat(string url0, string url1, string param)
        {
            int i = 0;

            if(url0.EndsWith("/"))
            {
                for(i = (url0.Length - 1); i >= 0; i--)
                {
                    if(!url0.EndsWith("/"))
                        break;

                    if(url0[i].Equals('/'))
                        url0 = url0.Substring(0, i);
                }
            }

            int scnt = 0;

            for(i = 0; i < url1.Length; i++)
            {
                if(!url1.StartsWith("/"))
                    break;

                if(url1[0].Equals('/'))
                {
                    url1 = url1.Substring(1, (url1.Length - 1));
                    scnt++;
                }
            }

            for(i = 0; i < param.Length; i++)
            {
                if(!param.StartsWith("?"))
                    break;

                if(param[0].Equals('?'))
                    param = param.Substring(1, (param.Length - 1));
            }

            for(i = 0; i < param.Length; i++)
            {
                if(!param.StartsWith("&"))
                    break;

                if(param[0].Equals('&'))
                    param = param.Substring(1, (param.Length - 1));
            }

            if(scnt > 0)
                url1 = string.Concat("/", url1);

            if(url1.IndexOf("?") < 0)
                url1 += "?";

            if(!url1.EndsWith("&") && !url1.EndsWith("?"))
                url1 += "&";

            return string.Concat(url0, url1, param);
        }

        /// <summary>
        /// url과 파라미터 값을 연결한다.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string UrlParamConcat(string url, string param)
        {
            return UrlParamConcat(string.Empty, url, param);
        }

        /// <summary>
        /// url의 파라미터 값을 urlEncode
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ParamUrlEncode(string url)
        {
            if(url.IndexOf("?") < 0)
                return url;

            string[] param = GetParams(url);

            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

            foreach(string pr in param)
            {
                string[] p = StringTool.Split(pr, '=');

                if(p.Length < 2)
                    continue;

                if(string.IsNullOrEmpty(p[0]))
                    continue;

                stringBuilder.AppendFormat("{0}={1}&", p[0], AntiXssTool.UrlEncode((p.Length > 1) ? p[1] : string.Empty));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 파라미터 값을 변경한다.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="keyName"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static string ReplaceParamValue(string url, string keyName, object v)
        {
            if(url.IndexOf("?") < 0)
                return url;

            string[] param = GetParams(url);
            url = GetUri(url);

            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

            foreach(string pr in param)
            {
                string[] p = StringTool.Split(pr, '=');

                if(p.Length < 2)
                    continue;

                if(string.IsNullOrEmpty(p[0]))
                    continue;

                if(p[0].ToUpper().Equals(keyName.ToUpper()))
                    stringBuilder.AppendFormat("{0}={1}&", p[0], (v != null) ? v.ToString() : string.Empty);
                else
                    stringBuilder.AppendFormat("{0}={1}&", p[0], (p.Length > 1) ? p[1] : string.Empty);
            }

            return url + "?" + stringBuilder;
        }

        /// <summary>
        /// url의 파람값들을 collection 값에 있는 파람값들로 교체한다.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cols"></param>
        /// <param name="isEnc">UrlEncode 여부</param>
        /// <returns></returns>
        public static string ReplaceParamValue(string url, NameValueCollection cols, bool isEnc)
        {
            if(url.IndexOf("?") < 0)
                return url;

            var ps = GetParam(url);
            url = GetUri(url);

            for(int i = 0; i < ps.Count; i++)
            {
                foreach(string ckey in cols.Keys)
                {
                    if(string.IsNullOrEmpty(ckey))
                        continue;

                    if(ps.GetKey(i).ToUpper().Equals(ckey.ToUpper()))
                    {
                        ps[ps.GetKey(i)] = cols[ckey];
                        break;
                    }
                }
            }

            var p = new StringBuilder();

            foreach(string k in ps.Keys)
            {
                p.AppendFormat("{0}={1}&", k, isEnc ? AntiXssTool.UrlEncode(ps[k]) : ps[k]);
            }

            return UrlParamConcat(url, p.ToString());
        }

        /// <summary>
        /// 파라미터중 값이 없는 것은 제외시킨다.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string RemoveNullStringParam(string url)
        {
            if(url.IndexOf("?") < 0)
                return url;

            string p = string.Empty, v = string.Empty;
            System.Collections.Specialized.NameValueCollection ps = GetParam(url);
            url = GetUri(url);

            foreach(string k in ps.Keys)
            {
                v = ps[k];
                if(!string.IsNullOrEmpty(v))
                    p += string.Format("{0}={1}&", k, v);
            }

            return UrlParamConcat(url, p);
        }

        /// <summary>
        /// URL을 요청 클라이언트에서 사용할 수 있는 URL로 변환합니다.(Page.ResolveUrl)
        /// </summary>
        /// <param name="relativeUrl">System.Web.UI.Control.TemplateSourceDirectory 속성과 관련된 URL입니다.</param>
        /// <returns>변환된 URL입니다.</returns>
        public static string ResolveUrl(string relativeUrl)
        {
            if(relativeUrl.IsWhiteSpace())
                return string.Empty;

            // ~로 시작되는 url 인경우
            if(relativeUrl.StartsWith("~"))
            {
                if(HttpContext.Current != null)
                    return HttpContext.Current.Request.ApplicationPath + relativeUrl.Substring(1).Replace("//", "/");

                throw new ArgumentException("Invalid URL: Relative URL not allowed. relativeUrl=" + relativeUrl);
            }

            return relativeUrl;
        }

        /// <summary>
        /// Script 전체 경로를 반환한다.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static string GetScriptPath(string virtualPath)
        {
            if(WebTool.IsWebContext == false)
                return virtualPath;

            string path = Path.Combine(WebTool.ServerName, WebTool.AppPath.Replace("/", string.Empty));

            if(virtualPath.StartsWith("~"))
            {
                path = Path.Combine(path, virtualPath.Substring(1).TrimStart('/', Path.DirectorySeparatorChar));
            }

            return path.Replace(Path.DirectorySeparatorChar, '/');
        }
    }
}