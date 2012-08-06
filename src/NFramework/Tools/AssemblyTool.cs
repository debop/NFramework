using System;
using System.Reflection;

namespace NSoft.NFramework.Tools {
    /// <summary>
    /// Assembly extension methods
    /// </summary>
    public static class AssemblyTool {
        /// <summary>
        /// get title attribute of assembly.
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static string GetTitle(this Assembly asm) {
            var titleAttr = asm.GetAttribute<AssemblyTitleAttribute>();
            return (titleAttr != null) ? titleAttr.Title : string.Empty;
        }

        /// <summary>
        /// get description of assembly
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static string GetDescription(this Assembly asm) {
            var attr = asm.GetAttribute<AssemblyDescriptionAttribute>();
            return (attr != null) ? attr.Description : string.Empty;
        }

        /// <summary>
        /// get location of assembly file.
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static string GetCodeBase(this Assembly asm) {
            return asm.CodeBase;
        }

        /// <summary>
        /// get company attribute of assembly.
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static string GetCompany(this Assembly asm) {
            var attr = asm.GetAttribute<AssemblyCompanyAttribute>();
            return (attr != null) ? attr.Company : string.Empty;
        }

        /// <summary>
        /// get product attribute of assembly.
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static string GetProduct(this Assembly asm) {
            var attr = asm.GetAttribute<AssemblyProductAttribute>();
            return (attr != null) ? attr.Product : string.Empty;
        }

        /// <summary>
        /// get copyright attribute of assembly
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static string GetCopyright(this Assembly asm) {
            var attr = asm.GetAttribute<AssemblyCopyrightAttribute>();
            return (attr != null) ? attr.Copyright : string.Empty;
        }

        /// <summary>
        /// get culture attribute of assembly
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static string GetCulture(this Assembly asm) {
            var attr = asm.GetAttribute<AssemblyCultureAttribute>();
            return (attr != null) ? attr.Culture : string.Empty;
        }

        /// <summary>
        /// get DefaultAlias attribute of assembly
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static string GetDefaultAlias(this Assembly asm) {
            var attr = asm.GetAttribute<AssemblyDefaultAliasAttribute>();
            return (attr != null) ? attr.DefaultAlias : string.Empty;
        }

        /// <summary>
        /// Assembly의 버전 정보를 가져옵니다.
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static Version GetVersion(this Assembly asm) {
            // 이렇게 해서 나오지 않는다.
            //var attr = asm.GetAttribute<AssemblyVersionAttribute>();
            //return (attr != null) ? attr.Version : string.Empty;
            return asm.GetName().Version;
        }

        /// <summary>
        /// Assembly의 파일버전 정보를 가져옵니다.
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static string GetFileVersion(this Assembly asm) {
            var attr = asm.GetAttribute<AssemblyFileVersionAttribute>();
            return (attr != null) ? attr.Version : string.Empty;
        }

        /// <summary>
        /// Assembly의 정보용 버전 정보를 가져옵니다.
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static string GetInformationalVersion(this Assembly asm) {
            var attr = asm.GetAttribute<AssemblyInformationalVersionAttribute>();
            return (attr != null) ? attr.InformationalVersion : string.Empty;
        }

        /// <summary>
        /// Assembly의 Configuration 정보를 가져옵니다.
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static string GetConfiguration(this Assembly asm) {
            var attr = asm.GetAttribute<AssemblyConfigurationAttribute>();
            return (attr != null) ? attr.Configuration : string.Empty;
        }

        /// <summary>
        /// Assembly에서 Trademark 정보를 가져옵니다.
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static string GetTrandemark(this Assembly asm) {
            var attr = asm.GetAttribute<AssemblyTrademarkAttribute>();
            return (attr != null) ? attr.Trademark : string.Empty;
        }

        /// <summary>
        /// Assembly에서 특정 Custom Attribute를 찾습니다. 없으면 null을 반환합니다.
        /// </summary>
        /// <typeparam name="T">Custom Attribute의 수형</typeparam>
        /// <param name="asm"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Assembly asm, bool inherit = true) where T : Attribute {
            var attrs = asm.GetCustomAttributes(typeof(T), inherit);

            return (attrs.Length > 0) ? (T)attrs[0] : null;
        }
    }
}