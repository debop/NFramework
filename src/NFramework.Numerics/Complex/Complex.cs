using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 복소수를 표현하는 클래스
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [TypeConverter(typeof(ComplexConverter))]
    [DebuggerDisplay("({Re},{Im})")]
    public struct Complex : IComparable<Complex>, IComparable, IEquatable<Complex>, ICloneable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Complex string reqular expression pattern
        /// </summary>
        public const string ComplexStringExpression = @"\((?<real>.*),(?<imaginary>.*)\)";

        /// <summary>
        /// Complex Reqular expression 
        /// </summary>
        public static readonly Regex ComplexParser = new Regex(ComplexStringExpression, RegexOptions.Compiled);

        /// <summary>
        /// Zero complex number
        /// </summary>
        public static readonly Complex Zero = new Complex(0, 0);

        /// <summary>
        /// Unit of Real part
        /// </summary>
        public static readonly Complex One = new Complex(1, 0);

        /// <summary>
        /// Unit of Imaginary part
        /// </summary>
        public static readonly Complex I = new Complex(0, 1);

        /// <summary>
        /// MaxValue
        /// </summary>
        public static readonly Complex MaxValue = new Complex(double.MaxValue, double.MaxValue);

        /// <summary>
        /// MinValue
        /// </summary>
        public static readonly Complex MinValue = new Complex(double.MinValue, double.MinValue);

        /// <summary>
        /// Create new instance of Complex
        /// </summary>
        /// <param name="re"></param>
        /// <param name="im"></param>
        /// <returns></returns>
        public static Complex CreateComplex(double re = 0, double im = 0) {
            return new Complex(re, im);
        }

        /// <summary>
        /// 복소수의 길이와 각도를 가지고 Complex로 변환한다.
        /// </summary>
        /// <param name="modulus"></param>
        /// <param name="argument">라디안 단위의 각도</param>
        /// <returns></returns>
        public static Complex FromModulusAndArgument(double modulus, double argument) {
            var re = modulus * Math.Cos(argument);
            var im = modulus * Math.Sin(argument);

            return new Complex(re, im);
        }

        /// <summary>
        /// Normalize
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Complex Nomalize(Complex source) {
            var modulus = source.GetModulus();

            return CreateComplex(source.Re / modulus, source.Im / modulus);
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex Add(Complex a, Complex b) {
            return new Complex(a.Re + b.Re, a.Im + b.Im);
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Complex Add(Complex a, double s) {
            return new Complex(a.Re + s, a.Im);
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Add(double s, Complex a) {
            return new Complex(s + a.Re, a.Im);
        }

        /// <summary>
        /// Subtract
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex Subtract(Complex a, Complex b) {
            return new Complex(a.Re - b.Re, a.Im - b.Im);
        }

        /// <summary>
        /// Subtract
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Complex Subtract(Complex a, double s) {
            return new Complex(a.Re - s, a.Im);
        }

        /// <summary>
        /// Subtract
        /// </summary>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Subtract(double s, Complex a) {
            return new Complex(s - a.Re, a.Im);
        }

        /// <summary>
        /// Multiply
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex Multiply(Complex a, Complex b) {
            return new Complex(a.Re * b.Re - a.Im * b.Im, a.Re * b.Im + a.Im + b.Re);
        }

        /// <summary>
        /// Multiply
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Complex Multiply(Complex a, double s) {
            return new Complex(a.Re * s, a.Im * s);
        }

        /// <summary>
        /// Multiply
        /// </summary>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Multiply(double s, Complex a) {
            return Multiply(a, s);
        }

        /// <summary>
        /// Divide
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex Divide(Complex a, Complex b) {
            var result = Zero;

            var modulusSquared = b.GetModulusSquared();

            if(Math.Abs(modulusSquared - 0.0) < double.Epsilon)
                throw new DivideByZeroException("modulus of b is zero.");

            double invModulusSquared = 1.0 / modulusSquared;

            result.Re = (a.Re * b.Re + a.Im * b.Im) / invModulusSquared;
            result.Im = (a.Im * b.Re + a.Re * b.Im) / invModulusSquared;

            return result;
        }

        /// <summary>
        /// Divide
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Complex Divide(Complex a, double s) {
            return new Complex(a.Re / s, a.Im / s);
        }

        /// <summary>
        /// Divide
        /// </summary>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Divide(double s, Complex a) {
            a.Re.ShouldNotBeZero("real part of a");
            a.Im.ShouldNotBeZero("imaginary part of a");

            return new Complex(s / a.Re, s / a.Im);
        }

        /// <summary>
        /// Negate
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Negative(Complex a) {
            return new Complex(-a.Re, -a.Im);
        }

        /// <summary>
        /// Approximate Equal with two complex
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool ApproximateEquals(Complex a, Complex b, double epsilon = MathTool.Epsilon) {
            return a.Re.ApproximateEqual(b.Re, epsilon) &&
                   a.Im.ApproximateEqual(b.Im, epsilon);
        }

        /// <summary>
        /// Parse string and build a new instance of Complex 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Complex Parse(string s) {
            s.ShouldNotBeWhiteSpace("s");

            var m = ComplexParser.Match(s);
            if(m.Success) {
                return new Complex(double.Parse(m.Result("${real}")),
                                   double.Parse(m.Result("${imaginary}")));
            }

            throw new InvalidOperationException("Can't parse string to Compex : (re, im)");
        }

        /// <summary>
        /// Try Parse the specified string to build a new instance of complex
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out Complex result) {
            var parsed = false;

            try {
                result = Parse(s);
                parsed = true;
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("문자열을 파싱하여 복소수로 만드는데 예외가 발생했습니다. s=[{0}]", s);
                    log.Warn(ex);
                }

                result = Zero;
            }

            return parsed;
        }

        /// <summary>
        /// Sin
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Sin(Complex a) {
            var result = Zero;
            if(Math.Abs(a.Im - 0.0) < double.Epsilon) {
                result.Re = Math.Sin(a.Re);
                result.Im = 0.0;
            }
            else {
                result.Re = Math.Sin(a.Re) * Math.Cosh(a.Im);
                result.Im = Math.Cos(a.Re) * Math.Sinh(a.Im);
            }

            return result;
        }

        /// <summary>
        /// Cosine
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Cos(Complex a) {
            var result = Zero;
            if(Math.Abs(a.Im - 0.0) < double.Epsilon) {
                result.Re = Math.Cos(a.Re);
                result.Im = 0.0;
            }
            else {
                result.Re = Math.Cos(a.Re) * Math.Cosh(a.Im);
                result.Im = -Math.Sin(a.Re) * Math.Sinh(a.Im);
            }

            return result;
        }

        /// <summary>
        /// Tan
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Tan(Complex a) {
            var result = Zero;

            if(Math.Abs(a.Im - 0.0) < double.Epsilon) {
                result.Re = Math.Tan(a.Re);
                result.Im = 0.0;
            }
            else {
                var re2 = 2.0 * a.Re;
                var im2 = 2.0 * a.Im;
                var denom = Math.Cos(re2) + Math.Cosh(re2);

                result.Re = Math.Sin(re2) / denom;
                result.Im = Math.Sinh(im2) / denom;
            }

            return result;
        }

        /// <summary>
        /// Sinh
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Sinh(Complex a) {
            var x = CreateComplex(a.Re, a.Im);

            var e = Math.Exp(x.Re);
            var f = 1 / e;

            x.Re = 0.5 * (e - f) * Math.Cos(x.Im);
            x.Im = 0.5 * (e + f) * Math.Sin(x.Im);

            return x;
        }

        /// <summary>
        /// Cosh
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Cosh(Complex a) {
            var x = CreateComplex(a.Re, a.Im);
            var e = Math.Exp(x.Re);
            var f = 1 / e;

            x.Re = 0.5 * (e + f) * Math.Cos(x.Im);
            x.Im = 0.5 * (e - f) * Math.Sin(x.Im);

            return x;
        }

        /// <summary>
        /// Tanh
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Tanh(Complex a) {
            var x = CreateComplex(a.Re, a.Im);
            double e = Math.Exp(x.Re);
            double f = 1 / e;
            double d = 0.5 * (e + f) * Math.Cos(2 * x.Im);

            x.Re = 0.5 * (e - f) / d;
            x.Im = Math.Sin(2 * x.Im) / d;

            return x;
        }

        /// <summary>
        /// Sqrt
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Sqrt(Complex a) {
            var result = Zero;

            if(a.Re.ApproximateEqual(0.0) && a.Im.ApproximateEqual(0.0))
                return result;

            if(a.Im.ApproximateEqual(0.0)) {
                result.Re = (a.Re > 0.0) ? Math.Sqrt(a.Re) : Math.Sqrt(-a.Re);
                result.Im = 0.0;
            }
            else {
                var modulus = a.GetModulus();
                result.Re = Math.Sqrt(0.5 * (modulus + a.Re));
                result.Im = Math.Sqrt(0.5 * (modulus - a.Re));

                if(a.Im < 0.0)
                    result.Im = -result.Im;
            }
            return result;
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Log(Complex a) {
            var x = Zero;

            if(a.Re > 0.0 && a.Im.ApproximateEqual(0.0)) {
                x.Re = Math.Log(a.Re);
                x.Im = 0.0;
            }
            else if(a.Re.ApproximateEqual(0.0)) {
                if(a.Im > 0.0) {
                    x.Re = Math.Log(a.Im);
                    x.Im = -MathTool.Pi / 2.0;
                }
                else {
                    x.Re = Math.Log(-a.Im);
                    x.Im = -MathTool.Pi / 2.0;
                }
            }
            else {
                x.Re = Math.Log(a.GetModulus());
                x.Im = Math.Atan2(a.Im, a.Re);
            }

            return x;
        }

        /// <summary>
        /// Exponential
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Exp(Complex a) {
            var x = Zero;
            double r = Math.Exp(a.Re);

            x.Re = r * Math.Cos(a.Im);
            x.Im = r * Math.Sin(a.Im);

            return x;
        }

        /// <summary>
        /// 자승
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex Pow(Complex a, Complex b) {
            return Exp(Multiply(b, Log(a)));
        }

        /// <summary>
        /// 절대값
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double Abs(Complex a) {
            if(Math.Abs(a.Re - 0.0) < double.Epsilon)
                return Math.Abs(a.Im);
            if(Math.Abs(a.Im - 0.0) < double.Epsilon)
                return Math.Abs(a.Re);

            double x = Math.Abs(a.Re);
            double y = Math.Abs(a.Im);
            double z;

            if(y > x) {
                z = x / y;
                return y * Math.Sqrt(1.0 + z * z);
            }

            z = y / x;
            return x * Math.Sqrt(1.0 + z * z);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="re">real value</param>
        /// <param name="im">imaginary value</param>
        public Complex(double re = 0, double im = 0)
            : this() {
            Re = re;
            Im = im;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="source"></param>
        public Complex(Complex source)
            : this() {
            Re = source.Re;
            Im = source.Im;
        }

        /// <summary>
        /// casting operator to Complex type
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static implicit operator Complex(double x) {
            return new Complex(x, 0.0);
        }

        /// <summary>
        /// casting operator to Complex
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static implicit operator Complex(float x) {
            return new Complex(x, 0.0);
        }

        /// <summary>
        /// casting operator to Complex
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static implicit operator Complex(decimal x) {
            return new Complex((double)x, 0.0);
        }

        /// <summary>
        /// casting operator to Complex
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static implicit operator Complex(long x) {
            return new Complex(x, 0.0);
        }

        /// <summary>
        /// casting operator to Complex
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static implicit operator Complex(int x) {
            return new Complex(x, 0.0);
        }

        /// <summary>
        /// casting operator to double
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static implicit operator Double(Complex c) {
            return c.Re;
        }

        /// <summary>
        /// Equal operator
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(Complex lhs, Complex rhs) {
            return Equals(lhs, rhs);
        }

        /// <summary>
        /// Not equal operator
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(Complex lhs, Complex rhs) {
            return !Equals(lhs, rhs);
        }

        /// <summary>
        /// Negate operator
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex operator -(Complex a) {
            return Negative(a);
        }

        /// <summary>
        /// Addition operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator +(Complex a, Complex b) {
            return Add(a, b);
        }

        /// <summary>
        /// Addition operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Complex operator +(Complex a, double s) {
            return Add(a, s);
        }

        /// <summary>
        /// Addition operator
        /// </summary>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex operator +(double s, Complex a) {
            return Add(s, a);
        }

        /// <summary>
        /// Subtraction operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator -(Complex a, Complex b) {
            return Subtract(a, b);
        }

        /// <summary>
        /// Subtraction operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Complex operator -(Complex a, double s) {
            return Subtract(a, s);
        }

        /// <summary>
        /// Subtraction operator
        /// </summary>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex operator -(double s, Complex a) {
            return Subtract(s, a);
        }

        /// <summary>
        /// Multiply operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator *(Complex a, Complex b) {
            return Multiply(a, b);
        }

        /// <summary>
        /// Multiply operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Complex operator *(Complex a, double s) {
            return Multiply(a, s);
        }

        /// <summary>
        /// Multiply operator
        /// </summary>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex operator *(double s, Complex a) {
            return Multiply(s, a);
        }

        /// <summary>
        /// divide operation
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator /(Complex a, Complex b) {
            return Divide(a, b);
        }

        /// <summary>
        /// divide operation
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Complex operator /(Complex a, double s) {
            return Divide(a, s);
        }

        /// <summary>
        /// divide operation
        /// </summary>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex operator /(double s, Complex a) {
            return Divide(s, a);
        }

        /// <summary>
        /// 실수부
        /// </summary>
        public double Re { get; set; }

        /// <summary>
        /// 허수부
        /// </summary>
        public double Im { get; set; }

        public double Magnitude {
            get { return Abs(this); }
        }

        /// <summary>
        /// 절대값 (복소수 좌표상의 길이)
        /// </summary>
        /// <returns></returns>
        public double GetModulus() {
            return Math.Sqrt(GetModulusSquared());
        }

        /// <summary>
        /// 절대값의 제곱
        /// </summary>
        /// <returns></returns>
        public double GetModulusSquared() {
            return Re * Re + Im * Im;
        }

        /// <summary>
        /// 복소수의 계수 (복소수 좌표상의 각도) : 라디안 단위
        /// </summary>
        /// <returns></returns>
        public double GetArgument() {
            return Math.Atan2(Im, Re);
        }

        /// <summary>
        /// 켤레 복소수 
        /// </summary>
        /// <returns></returns>
        public Complex GetConjugate() {
            return CreateComplex(Re, -Im);
        }

        /// <summary>
        /// Normalize
        /// </summary>
        public void Normailize() {
            double modulus = GetModulus();

            if(modulus.ApproximateEqual(0.0))
                throw new DivideByZeroException("modulus of this complex number is zero.");

            Re /= modulus;
            Im /= modulus;
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return HashTool.Compute(Re, Im);
        }

        /// <summary>
        /// 현재 개체가 동일한 형식의 다른 개체와 같은지 여부를 나타냅니다.
        /// </summary>
        /// <returns>
        /// 현재 개체가 <paramref name="other" /> 매개 변수와 같으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="other">이 개체와 비교할 개체입니다.</param>
        public bool Equals(Complex other) {
            return (Math.Abs(Re - other.Re) < double.Epsilon) && (Math.Abs(Im - other.Im) < double.Epsilon);
        }

        /// <summary>
        /// 객체가 같은지 검사
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            if(obj is Complex)
                return Equals((Complex)obj);

            return false;
        }

        /// <summary>
        /// 현재 객체를 문자열로 표현
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format("({0}, {1})", Re, Im);
        }

        /// <summary>
        /// 현재 개체를 동일한 형식의 다른 개체와 비교합니다.
        /// </summary>
        /// <returns>
        /// 비교되는 개체의 상대 순서를 나타내는 부호 있는 32비트 정수입니다. 반환 값에는 다음과 같은 의미가 있습니다. 값 의미 0보다 작음 이 개체는 <paramref name="other" /> 매개 변수보다 작습니다.0 이 개체는 <paramref name="other" />와 같습니다. 0보다 큼 이 개체는 <paramref name="other" />보다 큽니다. 
        /// </returns>
        /// <param name="other">이 개체와 비교할 개체입니다.</param>
        public int CompareTo(Complex other) {
            return GetModulus().CompareTo(other.GetModulus());
        }

        /// <summary>
        /// 현재 인스턴스를 동일한 형식의 다른 개체와 비교합니다.
        /// </summary>
        /// <returns>
        /// 비교되는 개체의 상대 순서를 나타내는 부호 있는 32비트 정수입니다. 반환 값에는 다음과 같은 의미가 있습니다. 값 의미 0보다 작음 이 인스턴스는 <paramref name="obj" />보다 작습니다. 0 이 인스턴스는 <paramref name="obj" />와 같습니다. 0보다 큼 이 인스턴스는 <paramref name="obj" />보다 큽니다. 
        /// </returns>
        /// <param name="obj">이 인스턴스와 비교할 개체입니다. </param>
        /// <exception cref="T:System.ArgumentException"><paramref name="obj" />가 이 인스턴스와 같은 형식이 아닌 경우 </exception>
        int IComparable.CompareTo(object obj) {
            if(obj == null)
                return 1;

            if(obj is Complex)
                return GetModulus().CompareTo(((Complex)obj).GetModulus());

            if(ReflectionTool.CanAssign(obj, typeof(double)))
                return GetModulus().CompareTo((double)obj);

            return -1;
        }

        /// <summary>
        /// 현재 인스턴스의 복사본인 새 개체를 만듭니다.
        /// </summary>
        /// <returns>
        /// 이 인스턴스의 복사본인 새 개체입니다.
        /// </returns>
        public object Clone() {
            return new Complex(this);
        }
    }
}