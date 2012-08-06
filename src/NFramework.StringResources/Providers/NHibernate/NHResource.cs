using System;
using NSoft.NFramework.Data;
using NSoft.NFramework.Data.NHibernateEx.Domain;

namespace NSoft.NFramework.StringResources.NHibernate {
    /// <summary>
    /// NHibernate Resource entity
    /// NOTE: Mappging 파일에서 NHResource는 lazy=false 이어야합니다.
    /// </summary>
    [Serializable]
    public class NHResource : LocaledEntityBase<Int32, NHResourceLocale> {
        /// <summary>
        /// 기본 생성자
        /// </summary>
        protected NHResource() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="assemblyName">Assembly name (Ini 의 파일명, External Resource의 assembly 명)</param>
        /// <param name="sectionName">Section Name</param>
        /// <param name="resourceKey"></param>
        public NHResource(string assemblyName, string sectionName, string resourceKey) {
            assemblyName.ShouldNotBeWhiteSpace("assemblyName");
            sectionName.ShouldNotBeWhiteSpace("sectionName");
            resourceKey.ShouldNotBeWhiteSpace("resourceKey");

            AssemblyName = assemblyName;
            Section = sectionName;
            ResourceKey = resourceKey;
        }

        /// <summary>
        /// Assembly name (Ini 의 파일명, External Resource의 assembly 명)
        /// </summary>
        public virtual string AssemblyName { get; protected set; }

        /// <summary>
        /// Section Name
        /// </summary>
        public virtual string Section { get; protected set; }

        /// <summary>
        /// Resource Key
        /// </summary>
        public virtual string ResourceKey { get; protected set; }

        /// <summary>
        /// default locale value
        /// </summary>
        public virtual string ResourceValue { get; set; }

        /// <summary>
        /// Hash Code를 계산합니다.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(AssemblyName, Section, ResourceKey);
        }

        /// <summary>
        /// 현재 인스턴스 정보를 문자열로 표현합니다.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format("NHResource# AssemblyName=[{0}],Section=[{1}],ResourceKey=[{2}]", AssemblyName, Section, ResourceKey);
        }
    }

    /// <summary>
    /// Localization information of Resource
    /// </summary>
    [Serializable]
    public class NHResourceLocale : DataObjectBase, ILocaleValue {
        /// <summary>
        /// Localized resource value
        /// </summary>
        public virtual string ResourceValue { get; set; }

        public override int GetHashCode() {
            return ResourceValue.CalcHash();
        }

        /// <summary>
        /// 현재 인스턴스 정보를 문자열로 표현합니다.
        /// </summary>
        public override string ToString() {
            return ResourceValue ?? string.Empty;
        }
    }
}