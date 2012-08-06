using System;

namespace NSoft.NFramework.Raven.Entities {
    [Serializable]
    public class Company : IEquatable<Company> {
        public string Name { get; set; }

        public string Region { get; set; }

        /// <summary>
        /// 현재 개체가 동일한 형식의 다른 개체와 같은지 여부를 나타냅니다.
        /// </summary>
        /// <returns>
        /// 현재 개체가 <paramref name="other"/> 매개 변수와 같으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="other">이 개체와 비교할 개체입니다.</param>
        public bool Equals(Company other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is Company) && Equals((Company)obj);
        }

        public override int GetHashCode() {
            return HashTool.Compute(Name, Region);
        }

        public override string ToString() {
            return string.Format("Company# Name=[{0}], Region=[{1}]", Name, Region);
        }
    }
}