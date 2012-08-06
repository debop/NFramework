using System;

namespace NSoft.NFramework.Data.QueryProviders {
    /// <summary>
    /// Ini 파일을 이용한 ADO.NET용 Query Provider
    /// </summary>
    [Serializable]
    public sealed class IniAdoQueryProvider : InIQueryProviderBase {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="queryFilePath">Query String 이 정의된 파일의 전체 경로</param>
        public IniAdoQueryProvider(string queryFilePath) : base(queryFilePath) {}
    }
}