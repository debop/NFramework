using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.Serialization;
using System.Text;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Matrix 
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Matrix : ISerializable, IEnumerable<double>, IEquatable<Matrix>, ICloneable {
        private static double[][] CreateItems(int rows, int cols) {
            var items = new double[rows][];

            for(int r = 0; r < rows; r++)
                items[r] = new double[cols];

            return items;
        }

        private readonly double[][] _data;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public Matrix(int rows, int cols) {
            _data = CreateItems(rows, cols);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="scalar"></param>
        public Matrix(int rows, int cols, double scalar)
            : this(rows, cols) {
            this.Initialize(scalar);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="data"></param>
        public Matrix(double[][] data) {
            data.ShouldNotBeNull("data");

            _data = CreateItems(data.Length, data[0].Length);
            data.CopyData(_data);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="dv"></param>
        public Matrix(DataView dv) {
            dv.ShouldNotBeNull("dv");
            _data = CreateItems(dv.Count, dv.Table.Columns.Count);

            for(int r = 0; r <= Rows; r++)
                for(int c = 0; c <= Cols; c++)
                    _data.SetValue(dv[r][c], r, c);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="src"></param>
        public Matrix(Matrix src) {
            src.ShouldNotBeNull("src");

            _data = CreateItems(src.Rows, src.Cols);
            src.Data.CopyData(_data);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected Matrix(SerializationInfo info, StreamingContext context) {
            _data = (double[][])info.GetValue("Data", typeof(double[][]));
            _data.ShouldNotBeNull("data");
        }

        /// <summary>
        /// 매트릭스 요소 정보
        /// </summary>
        public double[][] Data {
            get { return _data; }
        }

        /// <summary>
        /// 행 수
        /// </summary>
        public int Rows {
            get { return _data.Length; }
        }

        /// <summary>
        /// 열 수
        /// </summary>
        public int Cols {
            get { return _data[0].Length; }
        }

        /// <summary>
        /// 지정한 행의 요소들
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public double[] this[int row] {
            get { return _data[row]; }
        }

        /// <summary>
        /// M(row, col)
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public double this[int row, int col] {
            get { return _data[row][col]; }
            set { _data[row][col] = value; }
        }

        /// <summary>
        /// 정방형 매트릭스인가? (행과 열의 크기가 같은가?)
        /// </summary>
        public bool IsSquare {
            get { return (Rows == Cols); }
        }

        /// <summary>
        /// 대칭형 매트릭스
        /// </summary>
        public bool IsSymmetric {
            get {
                if(IsSquare) {
                    for(var r = 0; r < Rows; r++)
                        for(var c = 0; c <= r; c++)
                            if(Math.Abs((sbyte)(this[r][c] - this[c][r])) > double.Epsilon)
                                return false;

                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Column 단위로 1차원 배열을 만든다.
        /// </summary>
        public double[] GetColumnPackedArray() {
            var array = new double[Rows * Cols];

            for(int c = 0; c < Cols; c++) {
                var cn = c * Rows;
                for(var r = 0; r < Rows; r++)
                    array[cn + r] = this[r, c];
            }

            return array;
        }

        /// <summary>
        /// Role 단위로 1차원 배열을 만든다.
        /// </summary>
        public double[] GetRowPackedArray() {
            var dest = new double[Rows * Cols];

            for(int r = 0; r < Rows; r++) {
                var rn = r * Cols;
                for(var c = 0; c < Cols; c++)
                    dest[rn + c] = this[r, c];
            }

            return dest;
        }

        /// <summary>
        /// Equal operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Matrix a, Matrix b) {
            return Equals(a, b);
        }

        /// <summary>
        /// Not equal operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Matrix a, Matrix b) {
            return !Equals(a, b);
        }

        /// <summary>
        /// Negate operator
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Matrix operator -(Matrix m) {
            return m.Negate();
        }

        /// <summary>
        /// Add operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix operator +(Matrix a, Matrix b) {
            return MatrixTool.Add(a, b);
        }

        /// <summary>
        /// Subtract operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix operator -(Matrix a, Matrix b) {
            return MatrixTool.Subtract(a, b);
        }

        /// <summary>
        /// Multiply operator
        /// </summary>
        /// <param name="m"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Matrix operator *(Matrix m, double s) {
            return MatrixTool.Multiply(m, s);
        }

        /// <summary>
        /// Multiply operator
        /// </summary>
        /// <param name="s"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Matrix operator *(double s, Matrix m) {
            return MatrixTool.Multiply(m, s);
        }

        /// <summary>
        /// Multiply operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix operator *(Matrix a, Matrix b) {
            return MatrixTool.Multiply(a, b);
        }

        /// <summary>
        /// Divide operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Matrix operator /(Matrix a, double s) {
            return MatrixTool.Divide(a, s);
        }

        /// <summary>
        /// Divide operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix operator /(Matrix a, Matrix b) {
            return MatrixTool.Divide(a, b);
        }

        /// <summary>
        /// HashCode 계산
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return _data.GetHashCode();
        }

        /// <summary>
        /// Matrix가 같은지 검사합니다.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool Equals(Matrix m) {
            if(m == null)
                return false;

            if(this.IsSameDimension(m) == false)
                return false;

            using(var enumerator1 = GetEnumerator())
            using(var enumerator2 = m.GetEnumerator())
                while(enumerator1.MoveNext() && enumerator2.MoveNext())
                    if(Math.Abs(enumerator1.Current - enumerator2.Current) > double.Epsilon)
                        return false;

            return true;
        }

        /// <summary>
        /// Matrix와 지정된 객체가 같은지 검사합니다.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            return (obj != null) && (obj is Matrix) && Equals((Matrix)obj);
        }

        /// <summary>
        /// 매트릭스를 문자열로 표현합니다.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return ToString(" ");
        }

        /// <summary>
        /// 매트릭스를 문자열로 표현합니다.
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public virtual string ToString(string separator) {
            separator = separator ?? " ";

            var sb = new StringBuilder();

            for(var r = 0; r < Rows; r++) {
                var delimiter = string.Empty;

                for(var c = 0; c < Cols; c++) {
                    sb.Append(delimiter).Append(this[r, c]);
                    if(delimiter.Length == 0)
                        delimiter = separator;
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("Data", _data);
        }

        /// <summary>
        /// 매트릭스 요소를 열거하기 위한 열거자를 반환합니다. 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<double> GetEnumerator() {
            return GetColumnFirstEnumerator();
        }

        /// <summary>
        /// 매트릭스 요소를 열거하기 위한 열을 먼저 열거하는 열거자를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<double> GetColumnFirstEnumerator() {
            for(var r = 0; r < Rows; r++)
                for(var c = 0; c < Cols; c++)
                    yield return this[r, c];
        }

        /// <summary>
        /// 매트릭스 요소를 열거하기 위한 행을 먼저 열거하는 열거자를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<double> GetRowFirstEnumerator() {
            for(var c = 0; c < Cols; c++)
                for(var r = 0; r < Rows; r++)
                    yield return this[r, c];
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// 현재 인스턴스의 복사본인 새 개체를 만듭니다.
        /// </summary>
        /// <returns></returns>
        public Matrix Clone() {
            return new Matrix(this);
        }

        object ICloneable.Clone() {
            return Clone();
        }
    }
}