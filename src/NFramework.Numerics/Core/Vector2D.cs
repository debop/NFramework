using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 2차원 벡터
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(Vector2DConverter))]
    public struct Vector2D : ISerializable, IComparable<Vector2D>, IEquatable<Vector2D>, IComparable, ICloneable {
        /// <summary>
        /// V (0, 0)
        /// </summary>
        public static readonly Vector2D Zero = new Vector2D(0.0d, 0.0d);

        /// <summary>
        /// V (1,0)
        /// </summary>
        public static readonly Vector2D XAxis = new Vector2D(1.0d, 0.0d);

        /// <summary>
        /// V (0, 1)
        /// </summary>
        public static readonly Vector2D YAxis = new Vector2D(0.0d, 1.0d);

        /// <summary>
        /// 문자열을 파싱하여 Vector2D 인스턴스를 빌드합니다.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Vector2D Parse(string s) {
            s.ShouldNotBeWhiteSpace("s");

            var m = ParseReg.Match(s);

            if(m.Success)
                return new Vector2D(double.Parse(m.Result("${x}")), double.Parse(m.Result("${y}")));

            throw new InvalidOperationException("can't parse  : [" + s + "] to Vector2D");
        }

        /// <summary>
        /// v + w
        /// </summary>
        public static Vector2D Add(Vector2D v, Vector2D w) {
            return new Vector2D(v.X + w.X, v.Y + w.Y);
        }

        /// <summary>
        /// v + s
        /// </summary>
        public static Vector2D Add(Vector2D v, double s) {
            return new Vector2D(v.X + s, v.Y + s);
        }

        /// <summary>
        /// w = u + v
        /// </summary>
        public static void Add(Vector2D u, Vector2D v, Vector2D w) {
            w.X = u.X + v.X;
            w.Y = u.Y + v.Y;
        }

        /// <summary>
        /// w = u + s
        /// </summary>
        public static void Add(Vector2D u, double s, Vector2D w) {
            w.X = u.X + s;
            w.Y = u.Y + s;
        }

        /// <summary>
        ///  v - w
        /// </summary>
        /// <returns></returns>
        public static Vector2D Subtract(Vector2D v, Vector2D w) {
            return new Vector2D(v.X - w.X, v.Y - w.Y);
        }

        /// <summary>
        /// v - s
        /// </summary>
        public static Vector2D Subtract(Vector2D v, double s) {
            return new Vector2D(v.X - s, v.Y - s);
        }

        /// <summary>
        /// s - v
        /// </summary>
        public static Vector2D Subtract(double s, Vector2D v) {
            return new Vector2D(s - v.X, s - v.Y);
        }

        /// <summary>
        /// w = u - v
        /// </summary>
        public static void Subtract(Vector2D u, Vector2D v, Vector2D w) {
            w.X = u.X - v.X;
            w.Y = u.Y - v.Y;
        }

        /// <summary>
        /// w = u - s
        /// </summary>
        public static void Subtract(Vector2D u, double s, Vector2D w) {
            w.X = u.X - s;
            w.Y = u.Y - s;
        }

        /// <summary>
        /// w = s - v
        /// </summary>
        public static void Subtract(double s, Vector2D u, Vector2D w) {
            w.X = s - u.X;
            w.Y = s - u.Y;
        }

        /// <summary>
        /// v / w
        /// </summary>
        public static Vector2D Divide(Vector2D v, Vector2D w) {
            Guard.Assert(Math.Abs(w.X - 0.0d) > double.Epsilon, "w.X should not be zero");
            Guard.Assert(Math.Abs(w.Y - 0.0d) > double.Epsilon, "w.Y should not be zero");

            return new Vector2D(v.X / w.X, v.Y / w.Y);
        }

        /// <summary>
        /// v / s
        /// </summary>
        public static Vector2D Divide(Vector2D v, double s) {
            Guard.Assert<DivideByZeroException>(Math.Abs(s - 0.0) > double.Epsilon, "s is zero");
            return new Vector2D(v.X / s, v.Y / s);
        }

        /// <summary>
        /// s / v
        /// </summary>
        public static Vector2D Divide(double s, Vector2D v) {
            Guard.Assert(Math.Abs(v.X - 0.0d) > double.Epsilon, "v.X should not be zero");
            Guard.Assert(Math.Abs(v.Y - 0.0d) > double.Epsilon, "v.Y should not be zero");

            return new Vector2D(s / v.X, s / v.Y);
        }

        /// <summary>
        /// w = u / v
        /// </summary>
        public static void Divide(Vector2D u, Vector2D v, ref Vector2D w) {
            Guard.Assert(Math.Abs(v.X - 0.0d) > double.Epsilon, "v.X should not be zero");
            Guard.Assert(Math.Abs(v.Y - 0.0d) > double.Epsilon, "v.Y should not be zero");

            w.X = u.X / v.X;
            w.Y = u.Y / v.Y;
        }

        /// <summary>
        /// w = u / s
        /// </summary>
        public static void Divide(Vector2D u, double s, ref Vector2D w) {
            Guard.Assert<DivideByZeroException>(Math.Abs(s - 0.0) > double.Epsilon, "s is zero");
            w.X = u.X / s;
            w.Y = u.Y / s;
        }

        /// <summary>
        /// w = s / u
        /// </summary>
        /// <param name="s"></param>
        /// <param name="u"></param>
        /// <param name="w"></param>
        public static void Divide(double s, Vector2D u, ref Vector2D w) {
            Guard.Assert(Math.Abs(u.X - 0.0d) > double.Epsilon, "u.X should not be zero");
            Guard.Assert(Math.Abs(u.Y - 0.0d) > double.Epsilon, "u.Y should not be zero");

            w.X = s / u.X;
            w.Y = s / u.Y;
        }

        /// <summary>
        /// return v * s
        /// </summary>
        public static Vector2D Multiply(Vector2D v, double s) {
            return new Vector2D(v.X * s, v.Y * s);
        }

        /// <summary>
        /// return s * v
        /// </summary>
        public static Vector2D Multiply(double s, Vector2D v) {
            return Multiply(v, s);
        }

        /// <summary>
        /// w = u * s
        /// </summary>
        public static void Multiply(Vector2D u, double s, Vector2D w) {
            w.X = u.X * s;
            w.Y = u.Y * s;
        }

        /// <summary>
        /// w = s * u 
        /// </summary>
        public static void Multiply(double s, Vector2D u, Vector2D w) {
            Multiply(u, s, w);
        }

        /// <summary>
        /// 두 벡터의 Dot를 계산한다.
        /// </summary>
        /// <remarks>
        /// DotProduct의 계산식은 다음과 같다.<br/>
        /// DotProduct(u, v) = (u.X*v.X) + (u.Y*v.Y)
        /// </remarks>
        public static double DotProduct(Vector2D u, Vector2D v) {
            return (u.X * v.X) + (u.Y * v.Y);
        }

        /// <summary>
        /// 두 벡터의 Cross Product를 계산한다.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// CrossProduct의 계산식은 다음과 같다.<br/>
        /// CrossProduct(u, v) = u.X * v.Y - u.Y * v.X;
        /// </remarks>
        public static double CrossProduct(Vector2D u, Vector2D v) {
            return u.X * v.Y - u.Y * v.X;
        }

        /// <summary>
        /// return -v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2D Negate(Vector2D v) {
            return new Vector2D(-v.X, -v.Y);
        }

        /// <summary>
        /// 두 벡터가 오차범위 내에서 같은 값을 가지는지 검사한다.
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool ApproxEqual(Vector2D u, Vector2D v, double tolerance = MathTool.Epsilon) {
            return
                (Math.Abs(u.X - v.X) <= tolerance) &&
                (Math.Abs(u.Y - v.Y) <= tolerance);
        }

        /// <summary>
        /// Vector2D를 문자열로 표현한 것을 정규표현식으로 파싱하기 위한 정규표현식 파서입니다.
        /// </summary>
        private static readonly Regex ParseReg = new Regex(@"\((?<x>.*),(?<y>.*)\)", RegexOptions.Compiled);

        private double _x;
        private double _y;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="x">x 값</param>
        /// <param name="y">y 값</param>
        public Vector2D(double x, double y) {
            _x = x;
            _y = y;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public Vector2D(double[] coordinates) {
            coordinates.ShouldNotBeNull("coordinates");
            Guard.Assert(coordinates.Length > 1, "coordinates length must be greater than 1. coordinate.Length=[{0}]",
                         coordinates.Length);

            _x = coordinates[0];
            _y = coordinates[1];
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="v"></param>
        public Vector2D(Vector2D v) {
            _x = v.X;
            _y = v.Y;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private Vector2D(SerializationInfo info, StreamingContext context) {
            _x = info.GetDouble("X");
            _y = info.GetDouble("Y");
        }

        /// <summary>
        /// X-coordinate value
        /// </summary>
        public double X {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        /// Y-coordinate value
        /// </summary>
        public double Y {
            get { return _y; }
            set { _y = value; }
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <param name="index">0 or 1</param>
        /// <returns></returns>
        public double this[int index] {
            get {
                switch(index) {
                    case 0:
                        return _x;
                    case 1:
                        return _y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set {
                switch(index) {
                    case 0:
                        _x = value;
                        break;
                    case 1:
                        _y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// 현재 인스턴스 복사
        /// </summary>
        /// <returns></returns>
        public Vector2D Clone() {
            return new Vector2D(this);
        }

        object ICloneable.Clone() {
            return Clone();
        }

        /// <summary>
        /// Serialization/
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("X", _x);
            info.AddValue("Y", _y);
        }

        /// <summary>
        /// Vector2D 정보를 Normalize를 수행합니다.
        /// </summary>
        public void Normalize() {
            double length = GetLength();

            length.ShouldNotBeZero("length");

            _x /= length;
            _y /= length;
        }

        /// <summary>
        /// 현재 인스턴스의 Normalize된 벡터를 빌드합니다.
        /// </summary>
        /// <returns></returns>
        public Vector2D GetNormalize() {
            var result = new Vector2D(this);
            result.Normalize();
            return result;
        }

        /// <summary>
        /// 벡터의 길이
        /// </summary>
        /// <returns></returns>
        public double GetLength() {
            return Math.Sqrt(Norm());
        }

        /// <summary>
        /// 벡터의 길이의 제곱
        /// </summary>
        public double GetLengthSquared() {
            return Norm();
        }

        /// <summary>
        /// Norm 값 계산
        /// </summary>
        public double Norm() {
            return (_x * _x + _y * _y);
        }

        /// <summary>
        /// 벡터 방향을 시계방향으로 90도 회전한 벡터
        /// </summary>
        public Vector2D Perpendicular() {
            return new Vector2D(_y, -_x);
        }

        /// <summary>
        /// 벡터 방향을 시계방향으로 90도 회전한 벡터의 단위 벡터
        /// </summary>
        /// <returns></returns>
        public Vector2D UnitPerpendicular() {
            var perp = Perpendicular();
            perp.Normalize();
            return perp;
        }

        /// <summary>
        /// 벡터 요소들이 0.0과 지정한 tolerance 내의 오차에 있다면, 벡터 요소들을 0으로 설정합니다.
        /// </summary>
        /// <param name="tolerance"></param>
        public void ClampZero(double tolerance) {
            _x = _x.Clamp(0.0, tolerance);
            _y = _y.Clamp(0.0, tolerance);
        }

        /// <summary>
        /// 벡터 요소들이 0.0과 Epsilon 내의 오차에 있다면, 벡터 요소들을 0으로 설정합니다.
        /// </summary>
        public void ClampZero() {
            _x = _x.Clamp(0.0);
            _y = _y.Clamp(0.0);
        }

        /// <summary>
        /// HashCode 계산
        /// </summary>
        public override int GetHashCode() {
            return HashTool.Compute(_x, _y);
        }

        /// <summary>
        /// 값을 비교합니다.
        /// </summary>
        public int CompareTo(object obj) {
            Guard.Assert(obj is Vector2D, "obj is not Vector2D type.");
            return CompareTo((Vector2D)obj);
        }

        /// <summary>
        /// 두 벡터를 비교합니다.
        /// </summary>
        public int CompareTo(Vector2D other) {
            if(Equals(this, other))
                return 0;

            return Norm() > other.Norm() ? 1 : -1;
        }

        /// <summary>
        /// 지정한 값과 비교합니다.
        /// </summary>
        public override bool Equals(object obj) {
            return (obj != null) && (obj is Vector2D) && Equals((Vector2D)obj);
        }

        /// <summary>
        /// 값이 같은지 검사합니다.
        /// </summary>
        public bool Equals(Vector2D other) {
            return GetHashCode().Equals(other.GetHashCode());
        }

        /// <summary>
        /// represent current by string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format("({0}, {1})", _x, _y);
        }

        /// <summary>
        /// equal operator
        /// </summary>
        public static bool operator ==(Vector2D u, Vector2D v) {
            return Equals(u, v);
        }

        /// <summary>
        /// not equal operator
        /// </summary>
        public static bool operator !=(Vector2D u, Vector2D v) {
            return !Equals(u, v);
        }

        /// <summary>
        /// greater than operator
        /// </summary>
        public static bool operator >(Vector2D u, Vector2D v) {
            return (u._x > v._x) && (u._y > v._y);
        }

        /// <summary>
        /// less than operator
        /// </summary>
        public static bool operator <(Vector2D u, Vector2D v) {
            return (u._x < v._x) && (u._y < v._y);
        }

        /// <summary>
        /// greater	than or equal operator
        /// </summary>
        public static bool operator >=(Vector2D u, Vector2D v) {
            return (u._x >= v._x) && (u._y >= v._y);
        }

        /// <summary>
        /// less than or equal operator
        /// </summary>
        public static bool operator <=(Vector2D u, Vector2D v) {
            return (u._x <= v._x) && (u._y <= v._y);
        }

        /// <summary>
        /// Negate operator
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2D operator -(Vector2D v) {
            return Vector2D.Negate(v);
        }

        /// <summary>
        /// Add operator
        /// </summary>
        public static Vector2D operator +(Vector2D u, Vector2D v) {
            return Vector2D.Add(u, v);
        }

        /// <summary>
        /// Add operator
        /// </summary>
        public static Vector2D operator +(Vector2D u, double s) {
            return Vector2D.Add(u, s);
        }

        /// <summary>
        /// Subtract operator
        /// </summary>
        public static Vector2D operator -(Vector2D u, Vector2D v) {
            return Vector2D.Subtract(u, v);
        }

        /// <summary>
        /// Subtract operator
        /// </summary>
        public static Vector2D operator -(Vector2D u, double s) {
            return Vector2D.Subtract(u, s);
        }

        /// <summary>
        /// Subtract operator
        /// </summary>
        public static Vector2D operator -(double s, Vector2D u) {
            return Vector2D.Subtract(s, u);
        }

        /// <summary>
        /// 곱하기 연산자
        /// </summary>
        public static Vector2D operator *(Vector2D u, double s) {
            return Vector2D.Multiply(u, s);
        }

        /// <summary>
        /// 곱하기 연산자
        /// </summary>
        public static Vector2D operator *(double s, Vector2D u) {
            return Vector2D.Multiply(u, s);
        }

        /// <summary>
        /// 나누기 연산자
        /// </summary>
        public static Vector2D operator /(Vector2D u, double s) {
            return Vector2D.Divide(u, s);
        }

        /// <summary>
        /// 나누기 연산자
        /// </summary>
        public static Vector2D operator /(double s, Vector2D u) {
            return Vector2D.Divide(s, u);
        }

        /// <summary>
        /// Vector2D 의 요소를 double 의 배열 (double[])로 명시적으로 변환해주는 연산자입니다.
        /// </summary>
        public static explicit operator double[](Vector2D v) {
            var array = new double[2];
            array[0] = v.X;
            array[1] = v.Y;

            return array;
        }

        //public static explicit operator System.Drawing.PointF(Vector2D v)
        //{
        //    return GDIConvert.ToGDI((Vector2F)v);
        //}
    }
}