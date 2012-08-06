using System.Collections.Generic;
using FluentNHibernate.Conventions;

namespace NSoft.NFramework.Data.NHibernateEx.Fluents {
    /// <summary>
    /// FluentNHibernate의 Convention 기능을 기본적으로 활용하는 Convention 인터페이스
    /// </summary>
    public interface IFluentConvention : IConvention {
        /// <summary>
        /// Convention Option
        /// </summary>
        ConventionOptions Options { get; set; }

        /// <summary>
        /// 컬럼명을 클래스명 + 속성명으로 표현되도록 하는 속성명의 컬렉션입니다.
        /// 예: Code 는 EntityName + Code (예: Company.Code 는 컬럼명이 CompanyCode | COMPANY_CODE 로 변환된다)
        /// </summary>
        IList<string> PropertyWithClassNames { get; set; }

        /// <summary>
        /// 컬럼명으로 매핑시에 약어로 매핑해야 할 이름(단어) 매핑이다. (예: Department-Dept, Locale-Loc, Configuration-Conf 등)
        /// </summary>
        IDictionary<string, string> AbbrNameMap { get; }

        /// <summary>
        /// <paramref name="text"/>에 <see cref="AbbrNameMap"/>에 등록된 약어 변환 단어가 있더면 약어로 변환하여 반환합니다.
        /// </summary>
        /// <param name="text">원본 문자열</param>
        /// <returns></returns>
        string GetAbbrName(string text);
    }
}