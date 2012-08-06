using System;
using System.Reflection;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework {
    /// <summary>
    /// Assembly의 AssemblyInfo에 정의된 정보를 제공합니다. <see cref="AssemblyTool"/>을 이용해도 되지만, 
    /// 이 함수를 사용하면 한꺼번에 정보를 추출하여 가지고 있으므로, 복제할 때 좋다.
    /// </summary>
    /// <seealso cref="AssemblyTool"/>
    [Serializable]
    public sealed class AsmInfo : ValueObjectBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assembly"></param>
        public AsmInfo(Assembly assembly) {
            assembly.ShouldNotBeNull("assembly");

            FullName = assembly.FullName;
            AssemblyName = assembly.GetName();

            // NOTE : CodeBase는 동적으로 Loading 한 Assembly에 대해서는 값을 조회할 수 없다.
            // this.CodeBase = assembly.CodeBase;
            Company = assembly.GetCompany();
            Configuration = assembly.GetConfiguration();
            Copyright = assembly.GetCopyright();
            Culture = assembly.GetCulture();
            DefaultAlias = assembly.GetDefaultAlias();
            Description = assembly.GetDescription();
            InfomationalVersion = assembly.GetInformationalVersion();
            Product = assembly.GetProduct();
            Title = assembly.GetTitle();
            Trademark = assembly.GetTrandemark();
            Version = AssemblyName.Version;
            Win32FileVersion = assembly.GetFileVersion();
        }

        /// <summary>
        /// Assembly Full Name.
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Assembly Name
        /// </summary>
        public AssemblyName AssemblyName { get; private set; }

        /// <summary>
        /// Codebase는 동적으로 Loading한 Assembly에 대해서는 값을 조회할 수 없습니다.
        /// </summary>
        public string CodeBase { get; set; }

        /// <summary>
        /// Company
        /// </summary>
        public string Company { get; private set; }

        /// <summary>
        /// Configuration
        /// </summary>
        public string Configuration { get; private set; }

        /// <summary>
        /// Copyright
        /// </summary>
        public string Copyright { get; private set; }

        /// <summary>
        /// Culture
        /// </summary>
        public string Culture { get; private set; }

        /// <summary>
        /// DefaultAlias
        /// </summary>
        public string DefaultAlias { get; private set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Informational Version
        /// </summary>
        public string InfomationalVersion { get; private set; }

        /// <summary>
        /// Product
        /// </summary>
        public string Product { get; private set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Trademark
        /// </summary>
        public string Trademark { get; private set; }

        /// <summary>
        /// Version
        /// </summary>
        public Version Version { get; private set; }

        /// <summary>
        /// File Version
        /// </summary>
        public string Win32FileVersion { get; private set; }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode() {
            return HashTool.Compute(FullName, AssemblyName.FullName);
        }
    }
}