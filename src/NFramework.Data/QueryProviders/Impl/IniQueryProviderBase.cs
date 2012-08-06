using System;
using System.IO;
using NSoft.NFramework.IO;
using NSoft.NFramework.Nini.Config;
using NSoft.NFramework.Nini.Ini;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.QueryProviders {
    /// <summary>
    /// 쿼리 정의 파일로부터 쿼리문장을 제공하는 기본 클래스입니다.
    /// </summary>
    public abstract class InIQueryProviderBase : IIniQueryProvider {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Ini 파일의 Section과 Key의 구분자 (',')
        /// </summary>
        public const string SECTION_DELIMITER = ",";

        /// <summary>
        /// Initialize a new instance of InIQueryProviderBase with query file.
        /// </summary>
        /// <param name="queryFilePath">Query String이 정의된 ini 파일의 전체경로</param>
        protected InIQueryProviderBase(string queryFilePath) {
            if(IsDebugEnabled)
                log.Debug("쿼리문을 제공하는 IQueryProvider의 인스턴스를 생성합니다. queryFilePath=[{0}]", queryFilePath);

            try {
                queryFilePath = FileTool.GetPhysicalPath(queryFilePath);

                if(queryFilePath.FileExists() == false)
                    throw new FileNotFoundException("QueryStringFile이 없습니다. queryFilePath=" + queryFilePath, queryFilePath);

                QueryFilePath = queryFilePath;

                // Ini File을 SambaStyle로 해야 다중라인을 이해한다. 다중라인의 구분은 backslash ('\') 로 구분한다.
                //
                QuerySource = new IniConfigSource(new IniDocument(QueryFilePath, IniFileType.SambaStyle));

                // parameter value값들을 계산해둔다.
                //
                QuerySource.ExpandKeyValues();
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("InIQueryProviderBase를 초기화하는데 실패했습니다!!! queryFilePath=[{0}]" + QueryFilePath);
                    log.Error(ex);
                }

                throw;
            }

            if(IsDebugEnabled)
                log.Debug("Query String 정보를 파싱했습니다. queryFilePath=[{0}]", QueryFilePath);
        }

        /// <summary>
        /// 쿼리 문장을 제공하는 <see cref="Nini.Config.IConfigSource"/>의 인스턴스
        /// </summary>
        protected virtual IConfigSource QuerySource { get; private set; }

        /// <summary>
        /// 쿼리 문장이 정의된 Ini 파일의 경로
        /// </summary>
        public string QueryFilePath { get; private set; }

        /// <summary>
        /// 지정된 쿼리 키에 해당하는 쿼리문을 가져온다.
        /// </summary>
        /// <param name="queryKey">쿼리 키, 형식: [Section,] QueryName</param>
        /// <returns>쿼리 문장</returns>
        /// <exception cref="InvalidOperationException">정의된 쿼리 키가 없을때</exception>
        public string GetQuery(string queryKey) {
            string queryString;

            if(TryGetQuery(queryKey, out queryString))
                return queryString;

            var pex =
                new InvalidDataException(string.Format("지정한 Query 문자을 가져오지 못했습니다. QueryFilePath=[{0}], queryKey=[{1}]",
                                                       QueryFilePath, queryKey));
            if(log.IsWarnEnabled)
                log.Warn(pex);

            throw pex;
        }

        /// <summary>
        /// 지정된 섹션, 지정된 쿼리명에 해당하는 쿼리문을 가져온다.
        /// </summary>
        /// <param name="section">섹션 명</param>
        /// <param name="queryName">쿼리 명</param>
        /// <returns>쿼리 문장</returns>
        /// <exception cref="InvalidOperationException">정의된 섹션, 쿼리명이 없을때</exception>
        public virtual string GetQuery(string section, string queryName) {
            string queryString;

            if(TryGetQuery(section, queryName, out queryString))
                return queryString;

            var pex =
                new InvalidDataException(string.Format("지정한 Query 문자을 가져오지 못했습니다!!! " +
                                                       @"QueryFilePath=[{0}], section=[{1}], queryName=[{2}]",
                                                       QueryFilePath, section, queryName));

            if(log.IsWarnEnabled)
                log.Warn(pex);

            throw pex;
        }

        /// <summary>
        /// 지정된 쿼리 키에 해당하는 쿼리 문장을 가져옵니다. 없으면 false를 반환하고, <paramref name="queryString"/>에는 빈 문자열이 설정됩니다.
        /// </summary>
        /// <param name="queryKey">쿼리 키, 형식: [Section,] QueryName</param>
        /// <param name="queryString">조회된 쿼리 문장</param>
        /// <returns>조회 여부</returns>
        public bool TryGetQuery(string queryKey, out string queryString) {
            if(IsDebugEnabled)
                log.Debug("쿼리키에 해당하는 쿼리문장을 조회해 봅니다... queryKey=[{0}]", queryKey);

            queryString = string.Empty;

            string section;
            string queryName;

            if(TryParseQueryKey(queryKey, out section, out queryName))
                return TryGetQuery(section, queryName, out queryString);

            return false;
        }

        /// <summary>
        /// 지정된 섹션, 쿼리명에 해당하는 쿼리 문장을 가져옵니다. 없으면 false를 반환하고, <paramref name="queryString"/>에는 빈 문자열이 설정됩니다.
        /// </summary>
        /// <param name="section">섹션 명</param>
        /// <param name="queryName">쿼리 명</param>
        /// <param name="queryString">쿼리 문장</param>
        /// <returns>조회여부</returns>
        public virtual bool TryGetQuery(string section, string queryName, out string queryString) {
            if(IsDebugEnabled)
                log.Debug("쿼리문 조회를 시도합니다... section=[{0}], queryName=[{1}]", section, queryName);

            queryString = string.Empty;

            if(section.IsWhiteSpace()) {
                foreach(IConfig config in QuerySource.Configs) {
                    queryString = config.GetString(queryName, null);
                    if(queryString.IsNotEmpty())
                        return true;
                }
            }
            else {
                var config = QuerySource.Configs[section];
                if(config != null)
                    queryString = config.GetString(queryName, null);
            }

            if(queryString.IsEmpty()) {
                if(log.IsWarnEnabled)
                    log.Warn("지정한 Query 문자을 가져오지 못했습니다. 빈 문자열을 반환합니다!!! QueryFilePath=[{0}], section=[{1}], queryName=[{2}]",
                             QueryFilePath, section, queryName);

                queryString = string.Empty;
                return false;
            }

            if(IsDebugEnabled)
                log.Debug("쿼리문을 조회하는데 성공했습니다!!! section=[{0}], queryName=[{1}], query=[{2}]", section, queryName, queryString);

            return true;
        }

        /// <summary>
        /// 모든 QueryString 을 Table 로 반환한다.
        /// </summary>
        /// <returns>Query 정보</returns>
        public virtual QueryTable GetQueries() {
            var queryTable = new QueryTable();

            if(IsDebugEnabled)
                log.Debug("모든 쿼리 문장 정보를 로드합니다...");

            foreach(IConfig config in QuerySource.Configs)
                foreach(string queryName in config.GetKeys()) {
                    string queryKey = config.Name + SECTION_DELIMITER + queryName;
                    queryTable.Add(queryKey, config.GetString(queryName, string.Empty));
                }

            return queryTable;
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="queryKey">[Section,] QueryName 형태의 키</param>
        /// <returns>query string</returns>
        public string this[string queryKey] {
            get { return GetQuery(queryKey); }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="section">section</param>
        /// <param name="queryKey">key</param>
        /// <returns>query string</returns>
        public string this[string section, string queryKey] {
            get { return GetQuery(section, queryKey); }
        }

        /// <summary>
        /// queryKey값을 파싱하여, section과 queryName 을 구한다.
        /// </summary>
        /// <param name="queryKey">NIni의 값에 parameterized value인 ${section|key}를 파싱한다. </param>
        /// <param name="section"></param>
        /// <param name="queryName"></param>
        protected virtual bool TryParseQueryKey(string queryKey, out string section, out string queryName) {
            if(IsDebugEnabled)
                log.Debug("쿼리키 파싱을 시도합니다... queryKey=[{0}]", queryKey);

            section = string.Empty;
            queryName = string.Empty;

            var items = queryKey.Split(new[] { SECTION_DELIMITER }, StringSplitOptions.RemoveEmptyEntries);

            if(items.Length == 2) {
                section = items[0].Trim();
                queryName = items[1].Trim();
            }
            else if(items.Length == 1) {
                queryName = items[0].Trim();
            }
            else {
                if(log.IsWarnEnabled)
                    log.Warn("쿼리 키 형식이 잘못되었습니다. [Section,] QueryName 이어야 합니다. queryKey=[{0}]", queryKey);

                return false;
            }

            if(IsDebugEnabled)
                log.Debug("쿼리 키 파싱에 성공했습니다.. section=[{0}], queryName=[{1}]", section, queryName);

            return true;
        }
    }
}