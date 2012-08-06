using System;
using NSoft.NFramework.Data.Mappers;

namespace NSoft.NFramework.Data.Persisters {
    /// <summary>
    /// IDataReader의 정보로부터 Persistent Object를 빌드합니다. 컬럼:속성 매핑 정보는 Capitalize Name Mapping 정보를 사용합니다.
    /// </summary>
    /// <typeparam name="T">Type of persistent object</typeparam>
    public sealed class CapitalizeReaderPersister<T> : ReaderPersister<T> {
        /// <summary>
        /// 생성자
        /// </summary>
        public CapitalizeReaderPersister() : base(new CapitalizeNameMapper()) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="factoryFunc"></param>
        public CapitalizeReaderPersister(Func<T> factoryFunc) : base(new CapitalizeNameMapper(), factoryFunc) {}
    }
}