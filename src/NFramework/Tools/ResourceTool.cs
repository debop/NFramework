using System.IO;
using System.Linq;
using System.Reflection;

namespace NSoft.NFramework.Tools {
    /// <summary>
    /// 리소스 조회를 위한 Tool 클래스입니다.
    /// </summary>
    public static class ResourceTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정된 Assembly에 포함된 리소스 파일의 정보를 반환한다.
        /// 참고 : http://www.attilan.com/2006/08/accessing_embedded_resources_u.php
        /// http://msdn.microsoft.com/en-us/library/ht9h2dk8.aspx
        /// </summary>
        /// <param name="assembly">리소스를 읽어올 Assembly</param>
        /// <param name="filename">리소스로 포함된 파일의 이름 (실제로는 NameSpace까지 줘야하지만, 파일명만 주면, 첫번째 리소스 파일을 반환한다.</param>
        /// <returns>파일 내용을 담은 Stream, 리소스 파일이 없을 경우는 null을 반환한다.</returns>
        /// <example>
        /// <code>
        ///		var asm = ReflectionTool.LoadAssembly("NSoft.NFramework.FusionCharts.dll");
        ///		var swf = asm.GetEmbeddedResourceFile("line.swf");					// 실제 리소스 파일명은 NSoft.NFramework.FusionCharts.Resources.Line.swf 이다.
        ///     Assert.IsNotNull(swf);
        ///     
        ///		// 여러군데 같은 파일명이 있을 경우 최소한의 Namespace로 구분할 수 있으면 된다.
        ///		var swf = asm.GetEmbeddedResourceFile("Resources.line.swf");
        /// </code>
        /// </example>
        public static Stream GetEmbeddedResourceFile(this Assembly assembly, string filename) {
            assembly.ShouldNotBeNull("assembly");
            filename.ShouldNotBeWhiteSpace("filename");

            if(IsDebugEnabled)
                log.Debug("Read Embedded Resource File. assembly=[{0}], filename=[{1}]", assembly.FullName, filename);

            var names = assembly.GetManifestResourceNames();
            filename = filename.ToLower();
            var resfile = names.FirstOrDefault(name => name.IsNotWhiteSpace() && name.ToLower().EndsWith(filename));

            return resfile.IsWhiteSpace() ? null : assembly.GetManifestResourceStream(resfile);
        }
    }
}