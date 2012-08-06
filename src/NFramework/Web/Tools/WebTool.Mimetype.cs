using System;
using System.Collections.Generic;
using System.IO;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web.Tools {
    public static partial class WebTool {
        /// <summary>
        /// 기본 Mime type
        /// </summary>
        public const string DefaultMimetype = @"application/octet-stream";

        /// <summary>
        /// Javascript 파일의 Mime type (application/javascript)
        /// </summary>
        public const string JavascriptMimetype = @"application/javascript";

        /// <summary>
        /// fileExtension - Mimetype 을 쌍으로 가지는 캐시 정보입니다.
        /// </summary>
        private static readonly Dictionary<string, string>
            _mimetypeCache = new Dictionary<string, string>
                             {
                                 { "bmp", "image/bmp" },
                                 { "cs", "text/plain" },
                                 { "css", "text/css" },
                                 { "cvs", "application/vnd.ms-excel" },
                                 { "dll", "application/x-msdownload" },
                                 { "doc", "application/msword" },
                                 { "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                                 { "exe", "application/x-msdownload" },
                                 { "gif", "image/gif" },
                                 { "hta", "application/hta" },
                                 { "htm", "text/html" },
                                 { "html", "text/html" },
                                 { "ico", "image/ico" },
                                 { "ini", "text/plain" },
                                 { "java", "text/plain" },
                                 { "javascript", "application/javascript" },
                                 { "jpeg", "image/jpeg" },
                                 { "jpg", "image/jpeg" },
                                 { "js", "application/javascript" },
                                 { "mhtml", "message/rfc822" },
                                 { "mpeg", "video/mpeg" },
                                 { "mpg", "video/mpeg" },
                                 { "png", "image/png" },
                                 { "ppt", "application/vnd.ms-powerpoint" },
                                 { "pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
                                 { "rmi", "audio/mid" },
                                 { "ruleset", "application/xml" },
                                 { "shtm", "text/html" },
                                 { "shtml", "text/html" },
                                 { "svg", "image/svg+xml" },
                                 { "swf", "application/x-shockwave-flash" },
                                 { "tar", "application/x-tar" },
                                 { "txt", "text/plain" },
                                 { "wav", "audio/wav" },
                                 { "xaml", "application/xaml+xml" },
                                 { "xbap", "application/x-ms-xbap" },
                                 { "xht", "application/xhtml+xml" },
                                 { "xhtml", "application/xhtml+xml" },
                                 { "xls", "application/vnd.ms-excel" },
                                 { "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                                 { "xml", "text/xml" },
                                 { "xps", "application/vnd.ms-xpsdocument" },
                                 { "xsd", "text/xml" },
                                 { "xsl", "text/xml" },
                                 { "zip", "application/x-zip-compressed" },
                             };

        /// <summary>
        /// 지정된 파일명의 MIME Type을 구합니다.
        /// </summary>
        /// <param name="filename">파일명이나 extensions (예: png)</param>
        /// <returns></returns>
        public static string GetMime(this string filename) {
            if(IsDebugEnabled)
                log.Debug("파일[{0}]의 Mimetype을 얻습니다...", filename);

            if(filename.IsWhiteSpace())
                return DefaultMimetype;

            string mimeType = null;
            string ext = null;
            try {
                ext = Path.GetExtension(filename).TrimStart('.').ToLower();

                if(_mimetypeCache.TryGetValue(ext, out mimeType) == false) {
                    using(var regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext)) {
                        if(regKey != null && regKey.GetValue("Content Type") != null)
                            mimeType = regKey.GetValue("Content Type").ToString();
                    }
                }
            }
            catch(Exception ex) {
                if(log.IsInfoEnabled) {
                    log.Info("Mimetype을 얻는데 실패했습니다. 기본 값을 설정합니다. filename=[{0}]", filename);
                    log.Info(ex);
                }
            }

            mimeType = mimeType ?? DefaultMimetype;

            if(ext != null)
                _mimetypeCache.AddValue(ext, mimeType);

            if(IsDebugEnabled)
                log.Debug("파일[{0}]의 MimeType은 [{1}]입니다.", filename, mimeType);

            return mimeType;
        }
    }
}