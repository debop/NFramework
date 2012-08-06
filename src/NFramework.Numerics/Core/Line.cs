using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 선분의 일반식인 A*x + B*y + C = 0 의 계수들로 선분을 표현합니다.
    /// </summary>
    [Serializable]
    public struct Line : IEquatable<Line> {
        public Line(double a, double b, double c)
            : this() {
            A = a;
            B = b;
            C = c;
        }

        /// <summary>
        /// X의 계수
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// Y의 계수
        /// </summary>
        public double B { get; set; }

        /// <summary>
        /// 상수 항
        /// </summary>
        public double C { get; set; }

        public bool Equals(Line other) {
            return GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is Line) && Equals((Line)obj);
        }

        public override int GetHashCode() {
            return HashTool.Compute(A, B, C);
        }

        public override string ToString() {
            return string.Format("Line# A=[{0}], B=[{1}], C=[{2}]", A, B, C);
        }
    }
}