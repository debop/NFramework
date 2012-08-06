using System;
using System.Text;
using System.Web;
using System.Web.Routing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web {
    /// <summary>
    /// URL 문자열을 Fluent 방식으로 빌드해주는 클래스입니다.
    /// </summary>
    public class UrlBuilder {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static UrlBuilder Create(string path) {
            return Create(path, null);
        }

        public static UrlBuilder Create(string path, object parameters) {
            return new UrlBuilder(path, parameters);
        }

        public static implicit operator string(UrlBuilder builder) {
            if(builder != null)
                return builder.ToString();

            return string.Empty;
        }

        private static readonly VirtualPathUtilityWrapper _defaultVirtualPathUtility = new VirtualPathUtilityWrapper();
        private readonly StringBuilder _params;
        private string _path;
        private readonly VirtualPathUtilityBase _virtualPathUtility;

        public UrlBuilder() : this(null, null) {}
        public UrlBuilder(object parameters) : this(null, parameters) {}
        public UrlBuilder(string path) : this(path, null) {}
        public UrlBuilder(string path, object parameters) : this(GetHttpContext(), null, path, parameters) {}

        internal UrlBuilder(HttpContextBase httpContext, VirtualPathUtilityBase virtualPathUtility, string path, object parameters) {
            _params = new StringBuilder();
            _virtualPathUtility = virtualPathUtility;

            Uri uri;
            if(Uri.TryCreate(path, UriKind.Absolute, out uri)) {
                _path = uri.GetLeftPart(UriPartial.Path);
                _params.Append(uri.Query);
            }
            else {
                _path = GetPageRelativePath(httpContext, path);
                var index = (_path ?? string.Empty).IndexOf('?');
                if(index != -1) {
                    _params.Append(_path.Substring(index));
                    _path = _path.Substring(0, index);
                }
            }

            if(parameters != null) {
                AddParam(parameters);
            }
        }

        public string Path {
            get { return _path; }
        }

        public string QueryString {
            get { return _params.ToString(); }
        }

        public VirtualPathUtilityBase VirtualPathUtility {
            get { return _virtualPathUtility ?? _defaultVirtualPathUtility; }
        }

        public UrlBuilder AddParam(object values) {
            var dictionary = new RouteValueDictionary(values);
            foreach(var pair in dictionary)
                AddParam(pair.Key, pair.Value);

            return this;
        }

        public UrlBuilder AddParam(string name, object value) {
            if(name.IsNotWhiteSpace()) {
                _params.Append(_params.Length == 0 ? '?' : '&');
                _params
                    .Append(HttpUtility.UrlEncode(name))
                    .Append('=')
                    .Append(HttpUtility.UrlEncode(value.AsText()));
            }
            return this;
        }

        public UrlBuilder AddPath(string path) {
            _path = EnsureTralingSlash(_path);

            if(path.IsNotWhiteSpace())
                _path = _path + HttpUtility.UrlPathEncode(path.TrimStart(new char[] { '/' }));

            return this;
        }

        public UrlBuilder AddPath(params string[] pathTokens) {
            foreach(var path in pathTokens)
                AddPath(path);

            return this;
        }

        public override string ToString() {
            return _path + _params;
        }

        private string GetPageRelativePath(HttpContextBase httpContext, string path) {
            if(httpContext == null)
                return path;

            return VirtualPathUtility.ToAbsolute(path ?? "~/");
        }

        private static string EnsureTralingSlash(string path) {
            if(path.IsNotWhiteSpace() && path[path.Length - 1] != '/')
                path = path + '/';

            return path;
        }

        private static HttpContextBase GetHttpContext() {
            if(HttpContext.Current == null)
                return null;

            return new HttpContextWrapper(HttpContext.Current);
        }
    }
}