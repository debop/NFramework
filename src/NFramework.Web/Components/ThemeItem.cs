using System;
using NSoft.NFramework.Data;

namespace NSoft.NFramework.Web
{
    /// <summary>
    /// 테마 정보를 표현합니다.
    /// </summary>
    [Serializable]
    public class ThemeItem : DataObjectBase, IEquatable<ThemeItem>
    {
        /// <summary>
        /// 생성자
        /// </summary>
        protected ThemeItem() { }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="name">테마명</param>
        /// <param name="assemblyName">테마 AssemblyName</param>
        public ThemeItem(string name, string assemblyName)
        {
            name.ShouldNotBeWhiteSpace("name");
            assemblyName.ShouldNotBeWhiteSpace("assemblyName");

            Name = name;
            AssemblyName = assemblyName;
        }

        /// <summary>
        /// 테마명
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 테마 AssemblyName
        /// </summary>
        public virtual string AssemblyName { get; set; }

        public virtual bool Equals(ThemeItem other)
        {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode()
        {
            return HashTool.Compute(Name);
        }

        public override string ToString()
        {
            return string.Format("ThemeItem# Name=[{0}], AssemblyName=[{1}]", Name, AssemblyName);
        }
    }
}