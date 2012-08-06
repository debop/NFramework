using System.Web;

namespace NSoft.NFramework.Web {
    internal sealed class VirtualPathUtilityWrapper : VirtualPathUtilityBase {
        public override string Combine(string basePath, string relativePath) {
            return VirtualPathUtility.Combine(basePath, relativePath);
        }

        public override string ToAbsolute(string virtualPath) {
            return VirtualPathUtility.ToAbsolute(virtualPath);
        }
    }
}