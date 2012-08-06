using System;
using NSoft.NFramework.Data.Mappers;

namespace NSoft.NFramework.Data.Persisters {
    /// <summary>
    /// <see cref="SameNameMapper"/>를 이용하여, 하나의 DataRow로 부터 Persistent Object를 빌드하는 Persister입니다.
    /// </summary>
    /// <typeparam name="T">Type of persistent object</typeparam>
    public sealed class SameRowPersister<T> : RowPersister<T> {
        /// <summary>
        /// 생성자
        /// </summary>
        public SameRowPersister() : base(new SameNameMapper()) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="factoryFunc"></param>
        public SameRowPersister(Func<T> factoryFunc) : base(new SameNameMapper(), factoryFunc) {}
    }
}