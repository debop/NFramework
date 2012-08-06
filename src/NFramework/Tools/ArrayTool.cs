using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NSoft.NFramework.Tools {
    /// <summary>
    /// Utility class for management of array object.
    /// </summary>
    public static class ArrayTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        // for compare values by hash algorithm
        private static readonly Lazy<HashAlgorithm> _hashObject = new Lazy<HashAlgorithm>(() => new SHA256Managed(), true);

        /// <summary>
        /// 값 비교를 빠르게 하기 위해서 HashAlgorithm 개체를 사용한다.
        /// </summary>
        private static HashAlgorithm HashObject {
            get { return _hashObject.Value; }
        }

        /// <summary>
        /// 두개의 Array가 같은 값들을 가졌는지 (equivalent) 를 알아본다.
        /// </summary>
        /// <remarks>
        /// 요소를 하나하나 비교하므로 너무 느리다. CompareString, CompareBytes, CompareStream을 사용할 것
        /// </remarks>
        /// <typeparam name="T">비교할 배열의 수형, IComparable을 구현해야 한다.</typeparam>
        /// <param name="arr1">비교할 대상</param>
        /// <param name="arr2">비교할 대상</param>
        /// <returns>두 인자가 모두 null 이면 false, 두 인자의 배열길이가 다르면 false입니다.</returns>
        public static bool Compare<T>(this T[] arr1, T[] arr2) where T : IComparable<T> {
            if(arr1 == null || arr2 == null)
                return false;

            if(arr1.Length != arr2.Length)
                return false;

            for(int i = 0; i < arr1.Length; i++) {
                if(arr1[i].CompareTo(arr2[i]) != 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 입력값의 해쉬코드를 계산하고, 그 결과를 16진수 포맷의 문자열로 반환한다.
        /// </summary>
        /// <remarks>
        /// BitConverter를 사용하면 '-' 를 구분자로 사용한다.
        /// <code>
        /// BitConverter.AsString(HashObject.ComputeHash(buffer));
        /// </code>
        /// </remarks>
        /// <param name="buffer">해쉬코드를 계산할 입력값입니다.</param>
        /// <returns>해쉬코드를 계산한 결과를 16진수 포맷의 문자열</returns>
        private static string GetHashString(this byte[] buffer) {
            // BitConverter를 사용하면 '-' 를 구분자로 사용한다.
            //return BitConverter.AsString(HashObject.ComputeHash(buffer));
            return HashObject.ComputeHash(buffer).GetHexStringFromBytes();
        }

        /// <summary>
        /// HashAlgorithm을 이용하여 긴 문자열 비교를 수행한다.
        /// </summary>
        /// <remarks>긴 두 문자열을 비교하기 위해 해쉬값을 계산한 후 두 해쉬값만 비교한다.</remarks>
        public static bool CompareString(this string a, string b, Encoding enc = null) {
            a.ShouldNotBeEmpty("a");
            b.ShouldNotBeEmpty("b");

            enc = enc ?? Encoding.UTF8;

            var h1 = HashObject.ComputeHash(enc.GetBytes(a));
            var h2 = HashObject.ComputeHash(enc.GetBytes(b));

            return (GetHashString(h1) == GetHashString(h2));
        }

        /// <summary>
        /// HashAlgorithm을 이용하여 길이가 긴 Stream 들을 비교한다. (성능이 빠르다.)
        /// </summary>
        /// <remarks>
        /// 크기가 큰 파일인 경우에는 BufferedStream을 만들어서 비교하면 성능이 빠르다.
        /// </remarks>
        /// <returns>둘다 null이면 참, 둘중 하나만 null이면 거짓, 둘의 길이 차이가 있으면 거짓</returns>
        public static bool CompareStream(this Stream a, Stream b) {
            if(a == null || b == null)
                return false;

            var h1 = HashObject.ComputeHash(a);
            var h2 = HashObject.ComputeHash(b);

            return (GetHashString(h1) == GetHashString(h2));
        }

        /// <summary>
        /// HashAlgorithm을 사용하여 두 배열을 비교한다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>둘다 null이면 참, 둘중 하나만 null이면 거짓, 둘의 길이 차이가 있으면 거짓</returns>
        public static bool CompareBytes(this byte[] a, byte[] b) {
            if(a == null && b == null)
                return true;
            if(a == null || b == null)
                return false;
            if(a.Length != b.Length)
                return false;

            var h1 = HashObject.ComputeHash(a);
            var h2 = HashObject.ComputeHash(b);

            return (GetHashString(h1) == GetHashString(h2));
        }

        /// <summary>
        /// 지정된 배열의 용량을 지정된 최소 용량보다 작을 때에는 지정된 최소용량이나 기존 용량의 두배 크기로 키운다. 
        /// 기존 데이타는 보존된다.
        /// </summary>
        /// <typeparam name="T">지정된 배열의 수형</typeparam>
        /// <param name="array">크기를 조절할 배열</param>
        /// <param name="minCapacity">최소 용량</param>
        public static void EnsureCapacity<T>(ref T[] array, int minCapacity) {
            EnsureCapacity(ref array, minCapacity, 16);
        }

        /// <summary>
        /// 지정된 배열의 용량을 지정된 최소 용량보다 작을 때에는 최소용량이나 기본 용량의 두배 크기로 키운다. 
        /// 기존 데이타는 보존된다.
        /// </summary>
        /// <typeparam name="T">지정된 배열의 수형</typeparam>
        /// <param name="array">크기를 조절할 배열</param>
        /// <param name="minCapacity">최소 용량</param>
        /// <param name="defaultCapacity"></param>
        public static void EnsureCapacity<T>(ref T[] array, int minCapacity, int defaultCapacity) {
            if(array == null || array.Length == 0)
                array = new T[defaultCapacity];

            var oldCapacity = array.Length;

            if(minCapacity > oldCapacity) {
                var newCapacity = (oldCapacity == 0) ? defaultCapacity : oldCapacity * 2;

                if(minCapacity > newCapacity)
                    newCapacity = minCapacity;

                Array.Resize(ref array, newCapacity);
            }
        }

        /// <summary>
        /// 두개의 배열을 합쳐서 하나의 배열로 만든다.
        /// </summary>
        /// <typeparam name="T">지정된 배열의 수형</typeparam>
        /// <param name="buffer1"></param>
        /// <param name="buffer2"></param>
        /// <returns>두 배열을 합친 새로운 배열</returns>
        /// <exception cref="ArgumentNullException">인자가 null 인 경우</exception>
        public static T[] Combine<T>(this T[] buffer1, T[] buffer2) where T : struct {
            buffer1.ShouldNotBeNull("buffer1");
            buffer2.ShouldNotBeNull("buffer2");

            if(buffer1.Length == 0 && buffer2.Length == 0)
                return new T[0];

            T[] combine;

            if(buffer1.Length == 0 || buffer2.Length == 0) {
                var source = (buffer1.Length == 0) ? buffer2 : buffer1;
                combine = new T[source.Length];
                Array.Copy(source, 0, combine, 0, source.Length);
            }
            else {
                combine = new T[buffer1.Length + buffer2.Length];

                Array.Copy(buffer1, 0, combine, 0, buffer1.Length);
                Array.Copy(buffer2, 0, combine, buffer1.Length, buffer2.Length);
            }

            return combine;
        }

        /// <summary>
        /// 원본 배열로부터 복사를 수행한다.
        /// </summary>
        public static T[] Copy<T>(this T[] src, int index, int length) {
            Guard.Assert(() => length >= 0);

            var dest = new T[length];
            Array.Copy(src, index, dest, 0, dest.Length);

            return dest;
        }

        /// <summary>
        /// <paramref name="src"/> 정보를 <paramref name="dest"/>로 복사한다.
        /// </summary>
        /// <typeparam name="T">배열 요소의 수형</typeparam>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public static void Copy<T>(this T[] src, T[] dest) {
            Array.Copy(src, 0, dest, 0, src.Length);
        }

        /// <summary>
        /// 암호화 모듈 같은데서 사용하기 위해서 지정한 크기의 Random 한 byte 배열을 만든다. 
        /// </summary>
        /// <param name="size">랜덤 배열의 크기</param>
        /// <returns>램덤 값이 채워진 1차원 바이트 배열</returns>
        public static byte[] GetRandomBytes(int size) {
            Guard.Assert(() => size >= 0);

            if(size == 0)
                return new byte[0];

            var randomBytes = new byte[size];
            GetRandomBytes(randomBytes);
            return randomBytes;
        }

        /// <summary>
        /// 암호화 모듈 같은데서 사용하기 위해서 Random 한 byte 배열을 만든다. 
        /// </summary>
        /// <param name="bytes">Random 값을 채울 바이트 배열</param>
        public static void GetRandomBytes(byte[] bytes) {
#if !SILVERLIGHT
            RandomNumberGenerator.Create().GetBytes(bytes);
#else
			if (bytes != null && bytes.Length > 0)
			{
				var rnd = new Random((int) DateTime.Now.Ticks);
				rnd.NextBytes(bytes);
			}
#endif
        }

        /// <summary>
        /// 지정된 배열의 모든 요소의 값을 지정된 값으로 설정한다.
        /// </summary>
        /// <typeparam name="T">지정된 배열의 수형</typeparam>
        /// <param name="array">초기화할 배열</param>
        /// <param name="value">초기화할 값</param>
        public static void InitArray<T>(this T[] array, T value = default(T)) //where T : struct
        {
            if(array == null || array.Length == 0)
                return;

            for(var i = 0; i < array.Length; i++)
                array[i] = value;
        }

        /// <summary>
        /// 지정된 배열의 모든 요소를 string 형식으로 변환하여 배열을 표현하게 한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns>배열표현 형식의 문자열</returns>
        /// <example>
        /// <code>
        ///     double[] array = new double[] { 100.0, 200.0, 300.0};
        ///     string abc = ArrayTool.AsString(array);
        ///     
        ///     Console.WriteLine(abc);
        ///     // console out is
        ///     // { 100.0, 200.0, 300.0 }
        /// 
        ///     Console.WriteLine(ArrayTool.AsString(null));
        ///     // console out is "null"
        /// 
        ///     Console.WriteLine(ArrayTool.AsString(new object[0]));
        ///     // console out is "{}"
        /// </code>
        /// </example>
        public static string AsString<T>(this T[] array) //where T : struct
        {
            if(array == null)
                return StringTool.Null;

            if(array.Length == 0)
                return "{}";

            var delimiter = string.Empty;
            var result = new StringBuilder(array.Length * 2);

            result.Append("{");
            foreach(T item in array) {
                result.Append(delimiter);
                result.Append((Equals(item, null) ? StringTool.Null : item.ToString()));

                if(delimiter.Length == 0)
                    delimiter = ",";
            }
            result.Append("}");

            return result.ToString();
        }

        /// <summary>
        /// 지정된 두 변수의 값을 교환한다.
        /// </summary>
        /// <typeparam name="T">지정된 수형</typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Swap<T>(ref T a, ref T b) {
            var temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// 배열이 null 이거나 zero length 인지 여부를 판단한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns>배열이 null이거나, 길이가 0이면 true를 반환</returns>
        public static bool IsZeroLengthArray<T>(this T[] array) {
            if(array == null || array.Length == 0)
                return true;

            return false;
        }

        /// <summary>
        /// 대상 배열에서 검색할 값 배열이 존재하는지 찾고, 위치를 반환한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">대상 배열</param>
        /// <param name="values">검색할 값 배열</param>
        /// <param name="startIndex">시작 위치</param>
        /// <returns>검색한 배열 위치, 실패시에는 -1을 반환한다.</returns>
        public static int IndexOf<T>(this T[] array, T[] values, int startIndex = 0) where T : IComparable<T> {
            if(IsZeroLengthArray(array))
                return -1;

            if(IsZeroLengthArray(values))
                return -1;

            var index = Array.IndexOf(array, values[0], startIndex);

            var src = new T[values.Length];

            while(index >= 0) {
                Array.Copy(array, index, src, 0, src.Length);

                if(Compare(src, values))
                    return index;

                index = Array.IndexOf(array, values[0], index + 1);
            }

            return index;
        }

        /// <summary>
        /// 지정된 바이트 배열에서 지정된 값 배열의 위치를 찾는다.
        /// </summary>
        /// <param name="array">원본 배열</param>
        /// <param name="values">찾고자하는 배열</param>
        /// <param name="startIndex">시작 인덱스</param>
        /// <param name="count">찾을 범위 - 시작 인덱스로부터 범위</param>
        /// <returns>찾은 위치</returns>
        /// <remarks>
        ///	Array.IndexOf 는 한 byte에 대해서는 찾을 수 있지만 찾고자하는 키값이 1차원 배열일 때 이 함수를 사용해야 한다.
        /// </remarks>
        /// <example>
        ///	스트링에서 원하는 배열의 스트링을 찾는다.
        ///	<code>
        ///		public void TestOfByteArrayIndexOf()
        ///		{
        ///			const int multiply = 50000;
        ///			StringBuilder result = new StringBuilder(str.Length * multiply);
        ///		
        ///			for(int i=0; i &lt; multiply; i++)
        ///			{				
        ///				result.Append(str);
        ///			}
        ///			result.Append(s);
        ///		
        ///			byte[] buffer = Encoding.Default.GetBytes(result.AsString());
        ///			byte[] boundary = Encoding.Default.GetBytes(s);
        ///		
        ///			PerfCount.Start();
        ///			
        ///			int pos = StringTool.ByteArrayIndexOf(buffer, boundary);
        ///		
        ///			Console.WriteLine("\n--------------------------");
        ///			Console.WriteLine("Buffer Size: " + buffer.Length.AsString());
        ///			Console.WriteLine("Position: " + pos);
        ///			Console.WriteLine("Search Time: " + PerfCount.End());
        ///		
        ///			result = null;
        ///		}
        ///	</code>
        /// </example>
        public static int ByteArrayIndexOf(this byte[] array, byte[] values, int startIndex = 0, int count = int.MaxValue) {
            var firstPos = -1;
            count = Math.Min(array.Length - startIndex, count);

            while(startIndex < array.Length) {
                firstPos = Array.IndexOf(array, values[0], startIndex, count);
                int nextPos = 0;

                if(firstPos == -1 || values.Length <= 1)
                    break;

                for(int i = 1; i < values.Length; i++) {
                    if(array[firstPos + i] != values[i]) {
                        nextPos = -1;
                        break;
                    }
                }

                if(nextPos == 0)
                    break;

                count = Math.Min(count + startIndex - firstPos, count);
                startIndex = firstPos + 1;
            }

            return firstPos;
        }
    }
}