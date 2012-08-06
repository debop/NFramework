using System;
using NHibernate.Criterion;

namespace NSoft.NFramework.Data.NHibernateEx {
    public static partial class CriteriaTool {
        /// <summary>
        /// LIKE 검색 시에 검색 값에 LIKE 검색이 가능하도록 검색 예약어를 붙인다( 예 "abc" + "%" )
        /// </summary>
        /// <param name="matchString"></param>
        /// <param name="matchMode"></param>
        /// <returns></returns>
        [Obsolete("NH 2.0에서 버그가 있어서 만들었던 것임.")]
        public static string ToMatchString(this string matchString, MatchMode matchMode) {
            if(matchMode == MatchMode.Exact)
                return matchString;
            if(matchMode == MatchMode.Start)
                return matchString + '%';
            if(matchMode == MatchMode.End)
                return '%' + matchString;
            if(matchMode == MatchMode.Anywhere)
                return '%' + matchString + '%';

            return matchString;
        }
    }
}