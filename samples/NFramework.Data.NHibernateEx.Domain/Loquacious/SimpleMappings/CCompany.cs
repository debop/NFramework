using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    [Serializable]
    public class CCompany : LocaledMetadataEntityBase<Guid, CCompanyLocale> {
        protected CCompany() {
            UpdateTimestamp = DateTime.Now;
        }

        public CCompany(string code)
            : this() {
            code.ShouldNotBeWhiteSpace("code");

            Code = code;
            Name = code;

            IsActive = true;
        }

        /// <summary>
        /// 코드
        /// </summary>
        public string Code { get; protected set; }

        /// <summary>
        /// 회사명
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 설명
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 사용여부
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// 확정 속성
        /// </summary>
        public string ExAttr { get; set; }

        /// <summary>
        /// 최종갱신일
        /// </summary>
        public DateTime? UpdateTimestamp { get; set; }

        private IList<CUser> _users;

        /// <summary>
        /// 소속 직원들
        /// </summary>
        public IList<CUser> Users {
            get { return _users ?? (_users = new List<CUser>()); }
            protected set { _users = value; }
        }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Code);
        }

        public override string ToString() {
            return string.Format("Company# Id=[{0}],Code=[{1}], Name=[{2}], IsActive=[{3}]", Id, Code, Name, IsActive);
        }
    }

    /// <summary>
    /// 지역화 정보
    /// </summary>
    [Serializable]
    public class CCompanyLocale : DataObjectBase, ILocaleValue {
        /// <summary>
        /// 회사명
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 설명
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 확정 속성
        /// </summary>
        public string ExAttr { get; set; }
    }
}