using System;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// Query 문이 정의된 DataSource (파일/DB)로 부터 쿼리문을 제공합니다.
    /// </summary>
    public interface IIniQueryProvider {
        /// <summary>
        /// Query String이 저장된 File 경로
        /// </summary>
        string QueryFilePath { get; }

        /// <summary>
        /// 지정된 쿼리 키에 해당하는 쿼리문을 가져온다.
        /// </summary>
        /// <param name="queryKey">쿼리 키, 형식: [Section,] QueryName</param>
        /// <returns>쿼리 문장</returns>
        /// <exception cref="InvalidOperationException">정의된 쿼리 키가 없을때</exception>
        string GetQuery(string queryKey);

        /// <summary>
        /// 지정된 섹션, 지정된 쿼리명에 해당하는 쿼리문을 가져온다.
        /// </summary>
        /// <param name="section">섹션 명</param>
        /// <param name="queryName">쿼리 명</param>
        /// <returns>쿼리 문장</returns>
        /// <exception cref="InvalidOperationException">정의된 섹션, 쿼리명이 없을때</exception>
        string GetQuery(string section, string queryName);

        /// <summary>
        /// 지정된 쿼리 키에 해당하는 쿼리 문장을 가져옵니다. 없으면 false를 반환하고, <paramref name="queryString"/>에는 빈 문자열이 설정됩니다.
        /// </summary>
        /// <param name="queryKey">쿼리 키, 형식: [Section,] QueryName</param>
        /// <param name="queryString">조회된 쿼리 문장</param>
        /// <returns>조회 여부</returns>
        bool TryGetQuery(string queryKey, out string queryString);

        /// <summary>
        /// 지정된 섹션, 쿼리명에 해당하는 쿼리 문장을 가져옵니다. 없으면 false를 반환하고, <paramref name="queryString"/>에는 빈 문자열이 설정됩니다.
        /// </summary>
        /// <param name="section">섹션 명</param>
        /// <param name="queryName">쿼리 명</param>
        /// <param name="queryString">쿼리 문장</param>
        /// <returns>조회여부</returns>
        bool TryGetQuery(string section, string queryName, out string queryString);

        /// <summary>
        /// 해당되는 모든 Query정보를 제공한다.
        /// </summary>
        /// <returns></returns>
        QueryTable GetQueries();

        /// <summary>
        /// QueryKey [Section,]QueryName 에 해당하는 Query문장을 나타냅니다.
        /// </summary>
        /// <param name="queryKey">쿼리 키, 형식: [Section,] QueryName</param>
        /// <returns>조회된 쿼리 문장</returns>
        /// <exception cref="InvalidOperationException">정의된 섹션, 쿼리명이 없을때</exception>
        string this[string queryKey] { get; }

        /// <summary>
        /// 지정된 section의 QueryName에 해당하는 Query 문장을 반환합니다.
        /// </summary>
        /// <param name="section">섹션 명</param>
        /// <param name="queryName">쿼리 명</param>
        /// <returns>쿼리 문장</returns>
        /// <exception cref="InvalidOperationException">정의된 섹션, 쿼리명이 없을때</exception>
        string this[string section, string queryName] { get; }
    }
}