using System;
using System.Runtime.InteropServices;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Array 의 subset을 만들 때 사용한다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct DataBlock<T> {
        private readonly T[] _data;
        private readonly int _offset;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="block">복사할 원본 DataBlock</param>
        /// <param name="offsetIncrement">offset</param>
        public DataBlock(DataBlock<T> block, int offsetIncrement = 0) {
            _data = block.Data;
            _offset = block.Offset + offsetIncrement;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="data">복사할 Data</param>
        /// <param name="offset">offset index</param>
        public DataBlock(T[] data, int offset = 0) {
            data.ShouldNotBeNull("data");
            offset.ShouldBePositiveOrZero("offset");

            _data = new T[data.Length];
            Array.Copy(data, _data, data.Length);
            _offset = offset;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="length">DataBlock의 내부 버퍼 크기</param>
        public DataBlock(int length) {
            length.ShouldBePositiveOrZero("length");

            _data = new T[length];
            _offset = 0;
        }

        /// <summary>
        /// Data 배열
        /// </summary>
        public T[] Data {
            get { return _data; }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="index">내부 저장소 접근을 위한 인덱스</param>
        /// <returns>저장소 값</returns>
        public T this[int index] {
            get { return _data[index + _offset]; }
            set { _data[index + _offset] = value; }
        }

        /// <summary>
        /// Offset
        /// </summary>
        public int Offset {
            get { return _offset; }
        }

        /// <summary>
        /// 사용가능한 저장소의 크기
        /// </summary>
        /// <returns></returns>
        public int GetLength() {
            if(_data == null)
                return -1;

            return _data.Length - _offset;
        }
    }
}