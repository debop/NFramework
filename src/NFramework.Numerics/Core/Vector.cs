using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Vector
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Vector : IEnumerable<double>, IEquatable<Vector>, ISerializable, ICloneable {
        /// <summary>
        /// Add
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector Add(Vector u, Vector v) {
            VectorTool.CheckDimension(u, v);
            var result = new Vector(u.Length);
            for(int i = 0; i < u.Length; i++)
                result[i] = u[i] + v[i];

            return result;
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="u"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Vector Add(Vector u, double s) {
            var result = new Vector(u.Length);
            for(int i = 0; i < u.Length; i++)
                result[i] = u[i] + s;

            return result;
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="s"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static Vector Add(double s, Vector u) {
            return Add(u, s);
        }

        /// <summary>
        /// Subtract
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector Subtract(Vector u, Vector v) {
            VectorTool.CheckDimension(u, v);
            var result = new Vector(u.Length);
            for(int i = 0; i < u.Length; i++)
                result[i] = u[i] - v[i];

            return result;
        }

        /// <summary>
        /// Subtract
        /// </summary>
        /// <param name="u"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Vector Subtract(Vector u, double s) {
            var result = new Vector(u.Length);
            for(int i = 0; i < u.Length; i++)
                result[i] = u[i] - s;

            return result;
        }

        /// <summary>
        /// Subtract
        /// </summary>
        /// <param name="s"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static Vector Subtract(double s, Vector u) {
            var result = new Vector(u.Length);
            for(int i = 0; i < u.Length; i++)
                result[i] = s - u[i];

            return result;
        }

        /// <summary>
        /// Multiply
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector Multiply(Vector u, Vector v) {
            VectorTool.CheckDimension(u, v);
            var result = new Vector(u.Length);
            for(int i = 0; i < u.Length; i++)
                result[i] = u[i] * v[i];

            return result;
        }

        /// <summary>
        /// Multiply
        /// </summary>
        /// <param name="u"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Vector Multiply(Vector u, double s) {
            var result = new Vector(u.Length);
            for(int i = 0; i < u.Length; i++)
                result[i] = u[i] * s;

            return result;
        }

        /// <summary>
        /// Multiply
        /// </summary>
        /// <param name="s"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static Vector Multiply(double s, Vector u) {
            return Multiply(u, s);
        }

        /// <summary>
        /// divide
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector Divide(Vector u, Vector v) {
            VectorTool.CheckDimension(u, v);
            var result = new Vector(u.Length);
            for(int i = 0; i < u.Length; i++)
                result[i] = u[i] / v[i];

            return result;
        }

        /// <summary>
        /// divide (u / s)
        /// </summary>
        /// <param name="u"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Vector Divide(Vector u, double s) {
            var result = new Vector(u.Length);
            for(int i = 0; i < u.Length; i++)
                result[i] = u[i] / s;

            return result;
        }

        /// <summary>
        /// divide (s / u)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static Vector Divide(double s, Vector u) {
            var result = new Vector(u.Length);
            for(int i = 0; i < u.Length; i++)
                result[i] = s / u[i];

            return result;
        }

        /// <summary>
        /// -v 를 만든다. (Vector의 모든 요소 값을 negate 한다.)
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector Negate(Vector v) {
            v.ShouldNotBeNull("v");
            var u = new Vector(v.Length);

            for(int i = 0; i < u.Length; i++)
                u[i] = -v[i];

            return u;
        }

        /// <summary>
        /// 벡터 요소의 역순 정렬
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector Reverse(Vector v) {
            v.ShouldNotBeNull("v");
            var u = new Vector(v.Length);

            for(int i = 0; i < v.Length; i++)
                u[i] = v[v.Length - 1 - i];

            return u;
        }

        private readonly double[] _data;

        /// <summary>
        /// default constructor (2차원 vector)
        /// </summary>
        public Vector() : this(2) {}

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="length"></param>
        public Vector(int length) {
            length.ShouldBePositive("length");
            _data = new double[length];
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="length"></param>
        /// <param name="initValue"></param>
        public Vector(int length, double initValue)
            : this(length) {
            for(int i = 0; i < _data.Length; i++)
                _data[i] = initValue;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="collection"></param>
        public Vector(IEnumerable<double> collection)
            : this(collection.Count()) {
            int i = 0;
            foreach(double x in collection)
                _data[i++] = x;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="collection"></param>
        public Vector(ICollection collection)
            : this(collection.Count) {
            int i = 0;
            foreach(double x in collection)
                _data[i++] = x;
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="src"></param>
        public Vector(Vector src) {
            Array.Copy(src.Data, 0, Data, 0, Length);
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected Vector(SerializationInfo info, StreamingContext context) {
            _data = (double[])info.GetValue("Data", typeof(double[]));
        }

        /// <summary>
        /// elements of vector
        /// </summary>
        public double[] Data {
            get { return _data; }
        }

        /// <summary>
        /// Length of vector
        /// </summary>
        public int Length {
            get { return _data.Length; }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double this[int index] {
            get { return _data[index]; }
            set { _data[index] = value; }
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="v"></param>
        public void Add(Vector v) {
            var n = Add(this, v);

            for(int i = 0; i < Length; i++)
                Data[i] = n[i];
        }

        /// <summary>
        /// Subtract
        /// </summary>
        /// <param name="v"></param>
        public void Subtract(Vector v) {
            var n = Subtract(this, v);

            for(int i = 0; i < Length; i++)
                Data[i] = n[i];
        }

        /// <summary>
        /// Get sub vector
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public Vector SubVector(int startIndex, int count) {
            var n = new Vector(count - startIndex);
            Array.Copy(Data, startIndex, n.Data, 0, count);
            return n;
        }

        /// <summary>
        /// 해당 Index의 요소들만으로 벡터를 만든다.
        /// </summary>
        /// <param name="indexes"></param>
        /// <returns></returns>
        public Vector SubVector(params int[] indexes) {
            indexes.ShouldNotBeEmpty("indexes");

            var n = new Vector(indexes.Length);
            for(int i = 0; i < indexes.Length; i++)
                n[i] = Data[indexes[i]];

            return n;
        }

        /// <summary>
        /// Equal operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Vector a, Vector b) {
            return Equals(a, b);
        }

        /// <summary>
        /// Not euqal operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Vector a, Vector b) {
            return !Equals(a, b);
        }

        /// <summary>
        /// Negate operator
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector operator -(Vector v) {
            return Negate(v);
        }

        /// <summary>
        /// addition operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector operator +(Vector a, Vector b) {
            return Add(a, b);
        }

        /// <summary>
        /// addition operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Vector operator +(Vector a, double s) {
            return Add(a, s);
        }

        /// <summary>
        /// addition operator
        /// </summary>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector operator +(double s, Vector a) {
            return Add(s, a);
        }

        /// <summary>
        /// subtraction operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector operator -(Vector a, Vector b) {
            return Subtract(a, b);
        }

        /// <summary>
        /// subtraction operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Vector operator -(Vector a, double s) {
            return Subtract(a, s);
        }

        /// <summary>
        /// subtraction operator
        /// </summary>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector operator -(double s, Vector a) {
            return Subtract(s, a);
        }

        /// <summary>
        /// multiply operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector operator *(Vector a, Vector b) {
            return Multiply(a, b);
        }

        /// <summary>
        /// multiply operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Vector operator *(Vector a, double s) {
            return Multiply(a, s);
        }

        /// <summary>
        /// multiply operator
        /// </summary>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector operator *(double s, Vector a) {
            return Multiply(s, a);
        }

        /// <summary>
        /// divide operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector operator /(Vector a, Vector b) {
            return Divide(a, b);
        }

        /// <summary>
        /// divide operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Vector operator /(Vector a, double s) {
            return Divide(a, s);
        }

        /// <summary>
        /// divide operator
        /// </summary>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector operator /(double s, Vector a) {
            return Divide(s, a);
        }

        /// <summary>
        /// Hashcode 얻기
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return HashTool.Compute(Data.Cast<object>().ToArray()) * 31 + Length;
        }

        /// <summary>
        /// 객체 비교
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            return (obj != null) && (obj is Vector) && Equals((Vector)obj);
        }

        public virtual bool Equals(Vector other) {
            return (other != null) && (Length == other.Length) && Data.SequenceEqual(other.Data);
        }

        /// <summary>
        /// Vector 를 문자열로 표시
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return ToString(",");
        }

        /// <summary>
        /// Vector 를 문자열로 표시
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public string ToString(string separator) {
            return ToString(separator, string.Empty);
        }

        /// <summary>
        /// Vector 를 문자열로 표시
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(string separator, string format) {
            var sb = new StringBuilder();
            string delimiter = string.Empty;

            sb.Append("{");
            for(int i = 0; i < Length; i++) {
                sb.Append(delimiter);
                sb.Append(_data[i].ToString(format));

                if(delimiter.Length == 0)
                    delimiter = separator;
            }
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// Vector 를 문자열로 표시
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public string ToString(string separator, IFormatProvider provider) {
            var sb = new StringBuilder();
            string delimiter = string.Empty;

            sb.Append("{");
            for(int i = 0; i < Length; i++) {
                sb.Append(delimiter);
                sb.Append(_data[i].ToString(provider));

                if(delimiter.Length == 0)
                    delimiter = separator;
            }
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// Vector 를 문자열로 표시
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public string ToString(string separator, string format, IFormatProvider provider) {
            var sb = new StringBuilder();
            string delimiter = string.Empty;

            sb.Append("{");
            for(int i = 0; i < Length; i++) {
                sb.Append(delimiter);
                sb.Append(_data[i].ToString(format, provider));

                if(delimiter.Length == 0)
                    delimiter = separator;
            }
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// 컬렉션을 반복하는 열거자를 반환합니다.
        /// </summary>
        /// <returns>
        /// 컬렉션을 반복하는 데 사용할 수 있는 <see cref="T:System.Collections.Generic.IEnumerator`1" />입니다.
        /// </returns>
        public IEnumerator<double> GetEnumerator() {
            return Data.AsEnumerable().GetEnumerator();
        }

        /// <summary>
        /// 컬렉션을 반복하는 열거자를 반환합니다.
        /// </summary>
        /// <returns>
        /// 컬렉션을 반복하는 데 사용할 수 있는 <see cref="T:System.Collections.IEnumerator" /> 개체입니다.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// 대상 개체를 serialize하는 데 필요한 데이터로 <see cref="T:System.Runtime.Serialization.SerializationInfo" />를 채웁니다.
        /// </summary>
        /// <param name="info">데이터로 채울 <see cref="T:System.Runtime.Serialization.SerializationInfo" />입니다. </param>
        /// <param name="context">이 serialization에 대한 대상입니다(<see cref="T:System.Runtime.Serialization.StreamingContext" /> 참조). </param>
        /// <exception cref="T:System.Security.SecurityException">호출자에게 필요한 권한이 없는 경우 </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("Data", _data);
        }

        /// <summary>
        /// 복사
        /// </summary>
        /// <returns></returns>
        public virtual Vector Clone() {
            return new Vector(this);
        }

        /// <summary>
        /// 현재 인스턴스의 복사본인 새 개체를 만듭니다.
        /// </summary>
        /// <returns>
        /// 이 인스턴스의 복사본인 새 개체입니다.
        /// </returns>
        object ICloneable.Clone() {
            return Clone();
        }
    }
}