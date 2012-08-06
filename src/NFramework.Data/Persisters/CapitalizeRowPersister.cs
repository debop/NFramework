using System;
using NSoft.NFramework.Data.Mappers;

namespace NSoft.NFramework.Data.Persisters {
    /// <summary>
    /// <see cref="CapitalizeNameMapper"/>를 이용하여, 하나의 DataRow로부터 Persistent Object를 빌드하는 Persister입니다.
    /// </summary>
    /// <typeparam name="T">Persistent Object </typeparam>
    public sealed class CapitalizeRowPersister<T> : RowPersister<T> {
        /// <summary>
        /// 생성자
        /// </summary>
        public CapitalizeRowPersister() : base(new CapitalizeNameMapper()) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="factoryFunc"></param>
        public CapitalizeRowPersister(Func<T> factoryFunc) : base(new CapitalizeNameMapper(), factoryFunc) {}
    }
}