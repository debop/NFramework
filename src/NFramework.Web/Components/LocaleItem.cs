using System;

namespace NSoft.NFramework.Web
{
    /// <summary>
    /// Localization 정보
    /// </summary>
    public class LocaleItem : ValueObjectBase, IEquatable<LocaleItem>
    {
        /// <summary>
        /// 생성자
        /// </summary>
        public LocaleItem() : this("ko-KR", "Korean") { }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="localeKey">지역화코드</param>
        /// <param name="localeKeyName">지역화명</param>
        public LocaleItem(string localeKey = "ko-KR", string localeKeyName = "Korean")
        {
            LocaleKey = localeKey;
            LocaleKeyName = localeKeyName;
        }

        /// <summary>
        /// 지역화코드 (en, ko, ko-KR)
        /// </summary>
        public string LocaleKey { get; set; }

        /// <summary>
        /// 지역화명 (English, Korean)
        /// </summary>
        public string LocaleKeyName { get; set; }


        public bool Equals(LocaleItem other)
        {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode()
        {
            return HashTool.Compute(LocaleKey);
        }

        public override string ToString()
        {
            return string.Format("LocaleItem# LocaleKey=[{0}],LocaleKeyName=[{1}]", LocaleKey, LocaleKeyName);
        }
    }
}