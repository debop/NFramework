using System;
using NSoft.NFramework.Threading;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Matrix Helper Class
    /// </summary>
    public static class MatrixTool {
        internal static readonly Random Rnd = new ThreadSafeRandom();

        /// <summary>
        /// 지정한 두개의 배열이 같은 차원의 배열인지 검사합니다.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public static void CheckSameDimension(double[][] src, double[][] dest) {
            src.ShouldNotBeNull("src");
            dest.ShouldNotBeNull("dest");
            Guard.Assert(src.Length == dest.Length, "Length of two array is different.");
            Guard.Assert(src.Rank == dest.Rank, "Rank of two array is different.");

            for(int rank = 0; rank < src.Rank; rank++)
                Guard.Assert(src.GetLength(rank) == dest.GetLength(rank), "Not same dimension.");
        }

        /// <summary>
        /// 두개의 <see cref="Matrix"/>가 같은 차원인지 검사합니다.
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        public static void CheckSameDimension(Matrix m1, Matrix m2) {
            CheckSameDimension(m1.Data, m2.Data);
        }

        /// <summary>
        /// 두 2차원 배열이 같은 차원인지 검사합니다.
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <returns></returns>
        public static bool IsSameDimension(double[][] data1, double[][] data2) {
            if(data1 == null || data2 == null)
                return false;

            if(data1.Length != data2.Length)
                return false;

            if(data1.Rank != data2.Rank)
                return false;

            for(var r = 0; r < data1.Rank; r++)
                if(data1.GetLength(r) != data2.GetLength(r))
                    return false;

            return true;
        }

        /// <summary>
        /// 두개의 <see cref="Matrix"/>가 같은 차원인지 검사합니다.
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static bool IsSameDimension(this Matrix m1, Matrix m2) {
            m1.ShouldNotBeNull("m1");
            m2.ShouldNotBeNull("m2");

            return IsSameDimension(m1.Data, m2.Data);
        }

        /// <summary>
        /// 지정한 행과 열의크기를 가진 행렬을 생성합니다.
        /// </summary>
        /// <param name="rows">행의 수 (양수)</param>
        /// <param name="cols">열의 수 (양수)</param>
        /// <returns></returns>
        public static Matrix CreateMatrix(int rows, int cols) {
            rows.ShouldBePositive("rows");
            cols.ShouldBePositive("cols");

            return new Matrix(rows, cols);
        }

        /// <summary>
        /// Matrix의 모든 요소 값을 <paramref name="value"/>로 설정합니다.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="value"></param>
        public static void Initialize(this Matrix m, double value = 0.0) {
            m.ShouldNotBeNull("m");

            for(var r = 0; r <= m.Rows; r++)
                for(var c = 0; c <= m.Cols; c++)
                    m[r, c] = value;
        }

        /// <summary>
        /// <paramref name="src"/> 매트릭스 정보를 <paramref name="dest"/> 매트릭스로 복사합니다.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public static void Copy(this Matrix src, Matrix dest) {
            CopyData(src.Data, dest.Data);
        }

        /// <summary>
        /// <paramref name="srcData"/> 배열을 <paramref name="destData"/> 배열로 복사합니다.
        /// </summary>
        /// <param name="srcData"></param>
        /// <param name="destData"></param>
        public static void CopyData(this double[][] srcData, double[][] destData) {
            CheckSameDimension(srcData, destData);
            Array.Copy(srcData, destData, destData.Length);
        }

        /// <summary>
        /// Matrix의 모든 요소를 <paramref name="action"/>에 인자로 넣어 실행합니다.
        /// </summary>
        /// <param name="m">matrix</param>
        /// <param name="action">실행할 delegate</param>
        public static void ForEach(this Matrix m, Action<double> action) {
            m.ShouldNotBeNull("m");
            action.ShouldNotBeNull("action");

            for(var r = 0; r < m.Rows; r++)
                for(var c = 0; c < m.Cols; c++)
                    action(m[r, c]);
        }

        /// <summary>
        /// a + b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Add(Matrix a, Matrix b) {
            a.ShouldNotBeNull("a");
            b.ShouldNotBeNull("b");
            CheckSameDimension(a, b);

            var result = CreateMatrix(a.Rows, a.Cols);
            result.Add(a, b);

            return result;
        }

        /// <summary>
        /// result = a + b
        /// </summary>
        /// <param name="result"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Add(this Matrix result, Matrix a, Matrix b) {
            result.ShouldNotBeNull("result");
            a.ShouldNotBeNull("a");
            b.ShouldNotBeNull("b");
            CheckSameDimension(a, b);
            CheckSameDimension(a, result);

            for(var r = 0; r < a.Rows; r++)
                for(var c = 0; c < a.Cols; c++)
                    result[r, c] = a[r, c] + b[r, c];
        }

        /// <summary>
        /// a - b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Subtract(Matrix a, Matrix b) {
            a.ShouldNotBeNull("a");
            b.ShouldNotBeNull("b");
            CheckSameDimension(a, b);

            var result = CreateMatrix(a.Rows, a.Cols);
            result.Subtract(a, b);

            return result;
        }

        /// <summary>
        /// result = a  b
        /// </summary>
        /// <param name="result"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Subtract(this Matrix result, Matrix a, Matrix b) {
            result.ShouldNotBeNull("result");
            a.ShouldNotBeNull("a");
            b.ShouldNotBeNull("b");
            CheckSameDimension(a, b);
            CheckSameDimension(a, result);

            for(var r = 0; r < a.Rows; r++)
                for(var c = 0; c < a.Cols; c++)
                    result[r, c] = a[r, c] - b[r, c];
        }

        /// <summary>
        /// return a * b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Multiply(Matrix a, Matrix b) {
            a.ShouldNotBeNull("a");
            b.ShouldNotBeNull("b");
            Guard.Assert(a.Cols == b.Rows, "Matrix dimension is not match. a.Cols equals b.Rows for Multipling.");

            var result = new Matrix(a.Rows, b.Cols);
            result.Multiply(a, b);

            return result;
        }

        /// <summary>
        /// result = a * b
        /// </summary>
        /// <param name="result"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Multiply(this Matrix result, Matrix a, Matrix b) {
            result.ShouldNotBeNull("result");
            a.ShouldNotBeNull("a");
            b.ShouldNotBeNull("b");

            Guard.Assert(result.Rows == a.Rows, "result.Rows is not equals a.Rows, [{0}] <> [{1}]", result.Rows, a.Rows);
            Guard.Assert(result.Cols == b.Cols, "result.Cols is not equals b.Cols, [{0}] <> [{1}]", result.Cols, b.Cols);

            result.Initialize();

            for(var r = 0; r < a.Rows; r++)
                for(var c = 0; c < b.Cols; c++)
                    for(var k = 0; k < a.Cols; k++)
                        result[r, c] += a[r, k] * b[k, c];
        }

        /// <summary>
        /// return a * s
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Matrix Multiply(Matrix a, double s) {
            a.ShouldNotBeNull("a");

            var result = new Matrix(a.Rows, a.Cols);
            result.Multiply(a, s);
            return result;
        }

        /// <summary>
        /// result = a * s
        /// </summary>
        /// <param name="result"></param>
        /// <param name="a"></param>
        /// <param name="s"></param>
        public static void Multiply(this Matrix result, Matrix a, double s) {
            result.ShouldNotBeNull("result");
            a.ShouldNotBeNull("a");
            CheckSameDimension(result, a);

            for(var r = 0; r < a.Rows; r++)
                for(var c = 0; c < a.Cols; c++)
                    result[r, c] = a[r, c] * s;
        }

        /// <summary>
        /// return a / b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Divide(Matrix a, Matrix b) {
            a.ShouldNotBeNull("a");
            b.ShouldNotBeNull("b");
            CheckSameDimension(a, b);
            var result = new Matrix(a.Rows, b.Cols);
            result.Divide(a, b);

            return result;
        }

        /// <summary>
        /// result = a  / b 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Divide(this Matrix result, Matrix a, Matrix b) {
            result.ShouldNotBeNull("result");
            a.ShouldNotBeNull("a");
            b.ShouldNotBeNull("b");
            CheckSameDimension(result, a);
            CheckSameDimension(a, b);

            for(var r = 0; r < a.Rows; r++)
                for(var c = 0; c < b.Cols; c++)
                    result[r, c] = a[r, c] / b[r, c];
        }

        /// <summary>
        /// return a / s
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Matrix Divide(Matrix a, double s) {
            a.ShouldNotBeNull("a");
            Guard.Assert<DivideByZeroException>(s != 0.0, @"s is zero.");

            var result = new Matrix(a.Rows, a.Cols);
            result.Divide(a, s);
            return result;
        }

        /// <summary>
        /// result = a  / s 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="a"></param>
        /// <param name="s"></param>
        public static void Divide(this Matrix result, Matrix a, double s) {
            result.ShouldNotBeNull("result");
            a.ShouldNotBeNull("a");
            Guard.Assert<DivideByZeroException>(s != 0.0, @"s is zero.");
            CheckSameDimension(result, a);

            for(var r = 0; r < a.Rows; r++)
                for(var c = 0; c < a.Cols; c++)
                    result[r, c] = a[r, c] / s;
        }

        /// <summary>
        /// Matrix의 지정한 범위에 해당하는 영역을 Sub matrix 로 만듭니다.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <param name="c0"></param>
        /// <param name="c1"></param>
        /// <returns></returns>
        public static Matrix SubMatrix(this Matrix m, int r0, int r1, int c0, int c1) {
            m.ShouldNotBeNull("m");

            var s = new Matrix(r1 - r0 + 1, c1 - c0 + 1);

            for(int r = r0; r <= r1; r++)
                for(int c = c0; c <= c1; c++)
                    s[r - r0, c - c0] = m[r, c];

            return s;
        }

        /// <summary>
        /// Matirx 에서 지정된 행, 렬의 인덱스에 해당하는 요소들만으로 Sub matrix를 생성합니다. 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public static Matrix SubMatrix(this Matrix m, int[] rows, int[] cols) {
            m.ShouldNotBeNull("m");
            rows.ShouldNotBeEmpty("rows");
            cols.ShouldNotBeEmpty("cols");

            var s = new Matrix(rows.Length, cols.Length);

            for(int r = 0; r < rows.Length; r++)
                for(int c = 0; c < cols.Length; c++)
                    s[r, c] = m[rows[r], cols[c]];

            return s;
        }

        /// <summary>
        /// Matirx 에서 지정된 행, 렬의 인덱스에 해당하는 요소들만으로 Sub matrix를 생성합니다. 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public static Matrix SubMatrix(this Matrix m, int r0, int r1, int[] cols) {
            var rows = new int[r1 - r0 + 1];
            for(int r = 0; r < rows.Length; r++)
                rows[r] = r0 + r;

            return m.SubMatrix(rows, cols);
        }

        /// <summary>
        /// 지정한 매트릭스에 지정된 인덱스에 해당하는 행과 
        /// <paramref name="c0"/> ~ <paramref name="c1"/> 범위의 컬럼에 해당하는 요소로 Sub matrix를 빌드합니다.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="rows"></param>
        /// <param name="c0"></param>
        /// <param name="c1"></param>
        /// <returns></returns>
        public static Matrix SubMatrix(this Matrix m, int[] rows, int c0, int c1) {
            var cols = new int[c1 - c0 + 1];
            for(var c = 0; c < cols.Length; c++)
                cols[c] = c0 + c;

            return m.SubMatrix(rows, cols);
        }

        /// <summary>
        /// <paramref name="m"/> matrix 의 요소값을 <paramref name="s"/>의 요소로 설정합니다.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="s"></param>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public static void SetMatrix(this Matrix m, Matrix s, int[] rows, int[] cols) {
            m.ShouldNotBeNull("m");
            s.ShouldNotBeNull("s");
            rows.ShouldNotBeNull("rows");
            cols.ShouldNotBeNull("cols");
            Guard.Assert(s.Rows >= rows.Length, @"s.Rows >= rows.Length, s.Rows=[{0}], rows.Length=[{1}]", s.Rows, rows.Length);
            Guard.Assert(s.Cols >= cols.Length, @"s.Cols >= cols.Length, s.Cols=[{0}], cols.Length=[{1}]", s.Cols, cols.Length);

            for(var r = 0; r < rows.Length; r++)
                for(var c = 0; c < cols.Length; c++)
                    m[rows[r], cols[c]] = s[r, c];
        }

        /// <summary>
        /// <paramref name="m"/> matrix 의 요소값을 <paramref name="s"/>의 요소로 설정합니다.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="s"></param>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <param name="c0"></param>
        /// <param name="c1"></param>
        public static void SetMatrix(this Matrix m, Matrix s, int r0, int r1, int c0, int c1) {
            m.ShouldNotBeNull("m");
            s.ShouldNotBeNull("s");
            Guard.Assert(r1 > r0, "r1 should be greater than r0, r0=[{0}], r1=[{1}]", r0, r1);
            Guard.Assert(c1 > c0, "c1 should be greater than c0, c0=[{0}], c1=[{1}]", c0, c1);

            var rows = new int[r1 - r0 + 1];
            for(int r = 0; r < rows.Length; r++)
                rows[r] = r0 + r;

            var cols = new int[c1 - c0 + 1];
            for(int c = 0; c < cols.Length; c++)
                cols[c] = c0 + c;

            SetMatrix(m, s, rows, cols);
        }

        /// <summary>
        /// <paramref name="m"/> matrix 의 요소값을 <paramref name="s"/>의 요소로 설정합니다.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="s"></param>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <param name="cols"></param>
        public static void SetMatrix(this Matrix m, Matrix s, int r0, int r1, int[] cols) {
            m.ShouldNotBeNull("m");
            s.ShouldNotBeNull("s");
            Guard.Assert(r1 > r0, "r1 should be greater than r0, r0=[{0}], r1=[{1}]", r0, r1);
            cols.ShouldNotBeNull("cols");

            var rows = new int[r1 - r0 + 1];
            for(int r = 0; r < rows.Length; r++)
                rows[r] = r0 + r;

            SetMatrix(m, s, rows, cols);
        }

        /// <summary>
        /// <paramref name="m"/> matrix 의 요소값을 <paramref name="s"/>의 요소로 설정합니다.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="s"></param>
        /// <param name="rows"></param>
        /// <param name="c0"></param>
        /// <param name="c1"></param>
        public static void SetMatrix(this Matrix m, Matrix s, int[] rows, int c0, int c1) {
            m.ShouldNotBeNull("m");
            s.ShouldNotBeNull("s");
            rows.ShouldNotBeNull("rows");
            Guard.Assert(c1 > c0, "c1 should be greater than c0, c0=[{0}], c1=[{1}]", c0, c1);

            var cols = new int[c1 - c0 + 1];
            for(var c = 0; c < cols.Length; c++)
                cols[c] = c0 + c;

            SetMatrix(m, s, rows, cols);
        }

        /// <summary>
        /// Matrix의 행, 렬을 지정한 값으로 설정합니다.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public static void Resize(this Matrix m, int rows, int cols) {
            m.ShouldNotBeNull("m");
            rows.ShouldBePositive("rows");
            cols.ShouldBePositive("cols");

            var data = m.Data;
            Array.Resize(ref data, rows);

            for(var r = 0; r < rows; r++) {
                if(data[r] == null)
                    data[r] = new double[cols];
                else
                    Array.Resize(ref data[r], cols);
            }
        }

        /// <summary>
        /// 음수
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Matrix Negate(this Matrix m) {
            m.ShouldNotBeNull("m");

            var n = new Matrix(m);

            for(var r = 0; r < m.Rows; r++)
                for(var c = 0; c < m.Cols; c++)
                    n[r, c] = -m[r, c];

            return n;
        }

        /// <summary>
        /// 행/열 변환
        /// </summary>
        public static Matrix Transpose(this Matrix m) {
            m.ShouldNotBeNull("m");

            var t = new Matrix(m.Cols, m.Rows);

            for(var r = 0; r < m.Rows; r++)
                for(var c = 0; c < m.Cols; c++)
                    t[c, r] = m[r, c];

            return t;
        }

        /// <summary>
        /// 고유 매트릭스 (대각선만 1)
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public static Matrix Identity(int rows, int cols) {
            var mi = CreateMatrix(rows, cols);

            for(var r = 0; r < mi.Rows; r++)
                for(var c = 0; c < mi.Cols; c++)
                    mi[r, c] = (r == c) ? 1 : 0;

            return mi;
        }

        /// <summary>
        /// Matrix Trace. 대각선 요소들의 합
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static double Trace(this Matrix m) {
            m.ShouldNotBeNull("m");

            double sum = 0;

            var N = Math.Min(m.Rows, m.Cols);
            for(var i = 0; i < N; i++)
                sum += m[i, i];

            return sum;
        }

        /// <summary>
        /// Randomize for Testing
        /// </summary>
        /// <param name="m"></param>
        public static void Randomize(this Matrix m) {
            m.ShouldNotBeNull("m");

            for(var r = 0; r < m.Rows; r++)
                for(var c = 0; c < m.Cols; c++)
                    m[r, c] = Rnd.NextDouble();
        }

        /// <summary>
        /// Maximum Column Abs Sum
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static double Norm1(this Matrix m) {
            m.ShouldNotBeNull("m");

            double f = 0;
            for(var c = 0; c < m.Cols; c++) {
                double s = 0;
                for(var r = 0; r < m.Rows; r++)
                    s += Math.Abs(m[r, c]);
                if(f < s)
                    f = s;
            }
            return f;
        }

        /// <summary>
        /// Maximum singular value
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static double Norm2(this Matrix m) {
            m.ShouldNotBeNull("m");

            // TODO : SingularValueDecomposition을 제작해야 합니다.
            // return new SingularValueDecomposition(m).Norm2();
            throw new NotImplementedException("SingularValueDecomposition을 구현해야 합니다.");
        }

        /// <summary>
        /// Maximum Row Abs Sum
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static double NormInf(this Matrix m) {
            m.ShouldNotBeNull("m");

            double f = 0;
            for(var r = 0; r < m.Rows; r++) {
                double s = 0;
                for(var c = 0; c < m.Cols; c++)
                    s += Math.Abs(m[r, c]);
                if(f < s)
                    f = s;
            }
            return f;
        }

        /// <summary>
        /// Frobenius norm
        /// </summary>
        /// <remarks>Sqrt of Sum Of Squares of all elements</remarks>
        public static double NormF(this Matrix m) {
            double f = 0;

            for(var r = 0; r < m.Rows; r++)
                for(var c = 0; c < m.Cols; c++)
                    f = f.Hypot(m[r, c]);

            return f;
        }
    }
}